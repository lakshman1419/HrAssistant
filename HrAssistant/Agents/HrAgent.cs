using HrAssistant.Services;
using OpenAI.Chat;
using System.Text.Json;

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
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                """
                You are an enterprise HR assistant.

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
                return completion.Content[0].Text;
            }

            messages.Add(
                new AssistantChatMessage(completion));

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

                        messages.Add(
                            new ToolChatMessage(
                                toolCall.Id,
                                JsonSerializer.Serialize(employee)));

                        break;


                    case "GetLeaveBalance":

                        var balance =
                            await _tools.Leave
                                .GetLeaveBalanceAsync(
                                    arguments!["employeeId"]);

                        messages.Add(
                            new ToolChatMessage(
                                toolCall.Id,
                                balance.ToString()));

                        break;


                    case "SearchPolicy":

                        var policy =
                            _tools.Policy
                                .SearchPolicyAsync(
                                    arguments!["question"]);

                        messages.Add(
                            new ToolChatMessage(
                                toolCall.Id,
                                policy.ToString()));

                        break;
                }
            }

            var finalResponse =
                await _client.CompleteChatAsync(
                    messages,
                    options);

            return finalResponse
                .Value
                .Content[0]
                .Text;
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
                    "Gets employee leave balance.",
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
                    "SearchPolicy",
                    "Search HR policy documents.",
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