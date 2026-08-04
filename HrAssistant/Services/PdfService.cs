using System.Text;
using UglyToad.PdfPig;

namespace HrAssistant.Services
{
    public class PdfService
    {

        public string ExtractText(string filePath)
        {
            using var document =
                PdfDocument.Open(filePath);


            var text = new StringBuilder();


            foreach (var page in document.GetPages())
            {
                text.AppendLine(
                    page.Text);
            }


            return text.ToString();
        }

    }
}
