namespace HrAssistant.Services
{
    public class TextChunkService
    {

        public List<string> Split(
            string text,
            int size = 500)
        {

            var chunks =
                new List<string>();


            for (int i = 0; i < text.Length; i += size)
            {
                var length =
                    Math.Min(
                        size,
                        text.Length - i);


                chunks.Add(
                    text.Substring(
                        i,
                        length));
            }


            return chunks;
        }

    }
}
