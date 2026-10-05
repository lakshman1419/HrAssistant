using HrAssistant.Services;
using OpenAI.Chat;
using System.Text.Json;
using System.Linq;

namespace HrAssistant.Agents
{
    public class HrAgent
    {
        private readonly ChatClient _client;
        private readonly HrToolRegistry _tools;

        public HrAgent(
            ChatClient client,
            HrToolRegistry tools)
        {
            _client = client;
            _tools = tools;
        }

        public async Task<string> Ask(string question)
        {
            if (IsGreeting(question))
            {
                return "Hello! I'm your HR assistant. What can I help you with?";
            }

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                """
                You are an enterprise HR assistant.

                Only answer questions about HR, employment, and the organization's policies and procedures. For unrelated requests, including current events or news, do not call tools or guess; politely explain that you can only help with HR-related questions and invite an HR question instead.
                Respond naturally to greetings and simple conversation without calling a tool.
                For HR policy questions, use SearchPolicy and treat its output as source excerpts, not as text to repeat verbatim.
                Synthesize the relevant excerpts into a complete, readable answer with clear sentences and useful formatting.
                Never return fragments, broken words, raw PDF text, or unrelated excerpts. If the source excerpts conflict or appear incomplete, say so explicitly and summarize the discrepancy without guessing.
                Ask for an employee identifier only when the user requests personal employee information.
                For an employee-specific leave balance question, always call GetLeaveBalance with the employee ID and requested leave type.
                Do not present an overall leave balance as a category-specific balance. Explain clearly when the tool reports that category data is unavailable.

                Rules:

                - Use tools for employee information.
                - Never invent employee data.
                - Protect employee privacy.
                - Only access information through approved tools.
                - Ask for employee identification when required.
                - Do not reveal unauthorized employee information.
                """),

                new UserChatMessage(question)
            };

            var options = new ChatCompletionOptions();

            foreach (var tool in GetTools())
            {
                options.Tools.Add(tool);
            }

            var response =
                await _client.CompleteChatAsync(
                    messages,
                    options);

            var completion = response.Value;

            if (completion.ToolCalls.Count == 0)
            {
                return GetTextFromCompletion(completion)
                    is { Length: > 0 } answer
                    ? answer
                    : "Hello! I'm your HR assistant. What can I help you with?";
            }

            messages.Add(
                new AssistantChatMessage(completion));

            string? toolResult = null;

            foreach (var toolCall in completion.ToolCalls)
            {
                var arguments =
                    JsonSerializer.Deserialize<Dictionary<string, string>>
                    (
                        toolCall.FunctionArguments.ToString()
                    );

                switch (toolCall.FunctionName)
                {
                    case "GetEmployee":

                        var employee =
                            await _tools.Employee
                                .GetEmployeeAsync(
                                    arguments!["employeeId"]);

                        toolResult = JsonSerializer.Serialize(employee);

                        messages.Add(
                            new ToolChatMessage(
                                toolCall.Id,
                                toolResult));

                        break;


                    case "GetLeaveBalance":

                        var balance =
                            await _tools.Leave
                                .GetLeaveBalanceAsync(
                                    arguments!["employeeId"],
                                    arguments.GetValueOrDefault("leaveType"));

                        toolResult = balance;

                        messages.Add(
                            new ToolChatMessage(
                                toolCall.Id,
                                balance));

                        break;


                    case "SearchPolicy":

                        var policy =
                            await _tools.Policy
                                .SearchPolicyAsync(
                                    arguments!["question"]);

                        if (policy == VectorStoreService.NoRelevantPolicyMatch)
                        {
                            return "I can only help with HR-related questions. I couldn't find a relevant HR policy for that request.";
                        }

                        toolResult = policy;

                        messages.Add(
                            new ToolChatMessage(
                                toolCall.Id,
                                policy));

                        break;
                }
            }

            var finalResponse =
                await _client.CompleteChatAsync(
                    messages,
                    options);

            var finalAnswer = GetTextFromCompletion(finalResponse.Value);
            return !string.IsNullOrWhiteSpace(finalAnswer)
                ? finalAnswer
                : toolResult ?? "I couldn't retrieve a response for that request. Please try again.";
        }


        private static string GetTextFromCompletion(ChatCompletion completion)
        {
            if (completion?.Content == null)
                return string.Empty;

            // Prefer the first non-empty text content
            var firstText = completion.Content
                .Select(c => c.Text)
                .FirstOrDefault(t => !string.IsNullOrEmpty(t));        


            return firstText ?? string.Empty;

           
        }

        private static bool IsGreeting(string question)
        {
            var normalized = question
                .Trim()
                .TrimEnd('!', '.', '?', ',')
                .Trim()
                .ToLowerInvariant();

            return normalized is "hi" or "hello" or "hey" or "howdy"
                or "hi there" or "hello there" or "hey there"
                or "good morning" or "good afternoon" or "good evening";
        }


        private List<ChatTool> GetTools()
        {
            return new()
            {
                ChatTool.CreateFunctionTool(
                    "GetEmployee",
                    "Gets employee details by employee id.",
                    BinaryData.FromString(
                    """
                    {
                      "type": "object",
                      "properties": {
                        "employeeId": {
                          "type": "string",
                          "description": "Employee identifier"
                        }
                      },
                      "required": [
                        "employeeId"
                      ]
                    }
                    """)),


                ChatTool.CreateFunctionTool(
                    "GetLeaveBalance",
                                        "Gets an employee's leave balance. Call this for employee-specific leave balance questions. Specify leaveType when the user asks for a category such as CL, SL, or EL. Never treat an overall balance as a category balance.",
                    BinaryData.FromString(
                    """
                    {
                      "type": "object",
                      "properties": {
                        "employeeId": {
                          "type": "string",
                          "description": "Employee identifier"
                                                },
                                                "leaveType": {
                                                    "type": "string",
                                                    "description": "Requested leave category, such as CL, SL, or EL. Use TOTAL for an overall balance; omit when no category is requested."
                        }
                      },
                      "required": [
                        "employeeId"
                      ]
                    }
                    """)),


                ChatTool.CreateFunctionTool(
                    "SearchPolicy",
                    "Search the organization's HR policy documents only. Use only for HR, employment, or organization-policy questions. Never use this tool for news, current events, or unrelated requests.",
                    BinaryData.FromString(
                    """
                    {
                      "type": "object",
                      "properties": {
                        "question": {
                          "type": "string",
                          "description": "HR policy question"
                        }
                      },
                      "required": [
                        "question"
                      ]
                    }
                    """))
            };
        }
    }
}