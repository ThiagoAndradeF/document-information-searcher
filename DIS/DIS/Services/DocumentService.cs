using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DIS.Services
{
    public class DocumentService
    {
        public DocumentService(){}
        public List<string> ProcessDocument(string filepath)
        {
            string content = readWordDocument(filepath);
            List<string> chunks = chunkText(content);
            return chunks;
        }
        private static string readWordDocument(string filepath)
        {

            WordprocessingDocument wordDocument = WordprocessingDocument.Open(filepath, false);
            Body body = wordDocument.MainDocumentPart.Document.Body;
            return body.InnerText;
        }

        private static List<string> chunkText(string text)
        {
            List<string> chunks = new List<string>();
            var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");
            chunks.AddRange(sentences);
            return chunks;
        }
    }
}
