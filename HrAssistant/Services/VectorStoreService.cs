using HrAssistant.Models;

namespace HrAssistant.Services
{
    public class VectorStoreService
    {
        public const string NoRelevantPolicyMatch = "NO_RELEVANT_HR_POLICY_MATCH";
        private const float MinimumPolicySimilarity = 0.35f;

        private readonly NvidiaEmbeddingService _embeddingService;

        private readonly List<DocumentChunk> _documents = new();

        private int _nextId = 1;

        public VectorStoreService(
            NvidiaEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        public async Task AddDocumentAsync(
            List<string> chunks,
            string sourceDocument)
        {
            for (var chunkIndex = 0; chunkIndex < chunks.Count; chunkIndex++)
            {
                var chunk = chunks[chunkIndex];
                var embedding =
                    await _embeddingService.CreateEmbedding(chunk);

                _documents.Add(new DocumentChunk
                {
                    Id = _nextId++,
                    SourceDocument = sourceDocument,
                    ChunkIndex = chunkIndex,
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

            var rankedResults =
                _documents
                    .Select(d => new
                    {
                        Chunk = d,
                        Score = CosineSimilarity(
                            queryEmbedding,
                            d.Embedding)
                    })
                    .OrderByDescending(x => x.Score)
                    .ToList();

            if (rankedResults.Count == 0
                || rankedResults[0].Score < MinimumPolicySimilarity)
            {
                return NoRelevantPolicyMatch;
            }

            var bestMatch = rankedResults[0].Chunk;
            var results = _documents
                .Where(document => document.SourceDocument == bestMatch.SourceDocument
                    && document.ChunkIndex >= bestMatch.ChunkIndex - 1
                    && document.ChunkIndex <= bestMatch.ChunkIndex + 1)
                .OrderBy(document => document.ChunkIndex)
                .Select(document => document.Text);

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