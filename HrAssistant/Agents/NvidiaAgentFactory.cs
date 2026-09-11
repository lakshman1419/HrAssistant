using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace HrAssistant.Agents
{
    public static class NvidiaAgentFactory
    {
        public static ChatClient Create(IConfiguration config)
        {
            var apiKey = config["Nvidia:ApiKey"]
                ?? throw new InvalidOperationException("NVIDIA API key is missing.");

            var endpoint = config["Nvidia:Endpoint"]
                ?? throw new InvalidOperationException("NVIDIA endpoint is missing.");

            var model = config["Nvidia:Model"]
                ?? throw new InvalidOperationException("NVIDIA model is missing.");

            var client = new OpenAIClient(
                new ApiKeyCredential(apiKey),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri(endpoint)
                });

            return client.GetChatClient(model);
        }
    }
}