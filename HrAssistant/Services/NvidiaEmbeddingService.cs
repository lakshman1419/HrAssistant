using OpenAI;
using OpenAI.Embeddings;
using System.ClientModel;

namespace HrAssistant.Services
{
    public class NvidiaEmbeddingService
    {

        private readonly EmbeddingClient _client;


        public NvidiaEmbeddingService(
            IConfiguration config)
        {

            var openAI =
                new OpenAIClient(
                new ApiKeyCredential(
                    config["Nvidia:ApiKey"]!),

                new OpenAIClientOptions
                {
                    Endpoint =
                    new Uri(
                    config["Nvidia:Endpoint"]!)
                });


            _client =
            openAI.GetEmbeddingClient(
            "nvidia/nv-embedqa-e5-v5");
        }



        public async Task<float[]> CreateEmbedding(
            string text)
        {
   var result =
            await _client.GenerateEmbeddingAsync(
                text);
         


            return result.Value
                .ToFloats()
                .ToArray();
        }

    }
}
