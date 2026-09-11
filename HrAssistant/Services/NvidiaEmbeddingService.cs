using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HrAssistant.Services
{
    public class NvidiaEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private const string Model = "nvidia/nemotron-3-embed-1b";

        public NvidiaEmbeddingService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<float[]> CreateEmbedding(
            string text,
            string inputType = "passage")
        {
            var apiKey = _configuration["Nvidia:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "Nvidia:ApiKey is not configured.");

            var endpoint = _configuration["Nvidia:Endpoint"];

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException(
                    "Nvidia:Endpoint is not configured.");

            var request = new
            {
                input = new[] { text },
                model = Model,
                input_type = inputType,
                modality = "text"
            };

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                endpoint.TrimEnd('/') + "/embeddings");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    apiKey);

            httpRequest.Content =
                new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _httpClient.SendAsync(httpRequest);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"NVIDIA embedding request failed. " +
                    $"Status: {(int)response.StatusCode} " +
                    $"{response.StatusCode}. " +
                    $"Response: {responseContent}");
            }

            using var json =
                JsonDocument.Parse(responseContent);

            var embedding =
                json.RootElement
                    .GetProperty("data")[0]
                    .GetProperty("embedding");

            return embedding
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();
        }
    }
}