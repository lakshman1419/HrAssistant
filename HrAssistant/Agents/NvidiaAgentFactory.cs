using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace HrAssistant.Agents
{
    public static class NvidiaAgentFactory
    {
        public static ChatClient Create(IConfiguration config)
        {
            var client = new OpenAIClient(
                new ApiKeyCredential(
                    config["Nvidia:ApiKey"]!),

                new OpenAIClientOptions
                {
                    Endpoint = new Uri(
                        config["Nvidia:Endpoint"]!)
                });

            return client.GetChatClient(
                config["Nvidia:Model"]!);
        }
    }
}