using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DIS.Services
{
    public class DocumentService
    {
        DocumentService()
        {

        }
        public List<string> ProcessDocument(string filepath)
        {
            string content = ReadWordDocument(filepath);
            List<string> chunks = ChunkText(content);
            return chunks;
        }

        public static string ReadWordDocument(string filepath)
        {
            WordprocessingDocument wordDocument = WordprocessingDocument.Open(filepath, false);
            Body body = wordDocument.MainDocumentPart.Document.Body;
            return body.InnerText;
        }

        public static List<string> ChunkText(string text)
        {
            List<string> chunks = new List<string>();
            var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");
            chunks.AddRange(sentences);
            return chunks;
        }
    }
}
