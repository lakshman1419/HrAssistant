using HrAssistant.Models;

namespace HrAssistant.Services
{
    public class VectorStoreService
    {
        private readonly NvidiaEmbeddingService _embeddingService;

        private readonly List<DocumentChunk> _documents = new();

        private int _nextId = 1;

        public VectorStoreService(
            NvidiaEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        public async Task AddDocumentAsync(List<string> chunks)
        {
            foreach (var chunk in chunks)
            {
                var embedding =
                    await _embeddingService.CreateEmbedding(chunk);

                _documents.Add(new DocumentChunk
                {
                    Id = _nextId++,
                    Text = chunk,
                    Embedding = embedding
                });
            }
        }

        public async Task<string> Search(string question)
        {
            if (_documents.Count == 0)
                return "No HR documents have been indexed.";

            var queryEmbedding =
                await _embeddingService.CreateEmbedding(question);

            var results =
                _documents
                    .Select(d => new
                    {
                        Chunk = d,
                        Score = CosineSimilarity(
                            queryEmbedding,
                            d.Embedding)
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(3)
                    .Select(x => x.Chunk.Text);

            return string.Join("\n\n", results);
        }

        private static float CosineSimilarity(
            float[] a,
            float[] b)
        {
            float dot = 0;
            float magA = 0;
            float magB = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }

            if (magA == 0 || magB == 0)
                return 0;

            return dot /
                (MathF.Sqrt(magA) *
                 MathF.Sqrt(magB));
        }
    }
}