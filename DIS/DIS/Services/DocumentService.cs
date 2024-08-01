using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Text.RegularExpressions;

namespace DIS.Services
{
    public class DocumentService
    {
        public DocumentService() { }

        public List<string> ProcessDocument(string filepath)
        {
            string extension = System.IO.Path.GetExtension(filepath).ToLower();
            string content;

            switch (extension)
            {
                case ".docx":
                    content = ReadWordDocument(filepath);
                    break;
                case ".pdf":
                    content = ReadPdfDocument(filepath);
                    break;
                default:
                    throw new System.Exception("Unsupported file type.");
            }

            List<string> chunks = ChunkText(content);
            return chunks;
        }

        private static string ReadWordDocument(string filepath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filepath, false))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;
                return body.InnerText;
            }
        }

       public string ReadPdfDocument(string filepath)
        {
            using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(filepath))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                return text.ToString();
            }
        }

        private static List<string> ChunkText(string text)
        {
            List<string> chunks = new List<string>();
            var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");
            chunks.AddRange(sentences);
            return chunks;
        }
    }
}
