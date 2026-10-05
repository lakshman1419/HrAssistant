namespace HrAssistant.Models
{
    public class DocumentChunk
    {
        public int Id { get; set; }        

        public string SourceDocument { get; set; } = string.Empty;

        public int ChunkIndex { get; set; }

        public string Text { get; set; } = string.Empty;

        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
