﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

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
                    throw new Exception("Unsupported file type.");
            }
            List<string> chunks = SplitBidNoticeBySections(content);
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
        private List<string> SplitBidNoticeBySections(string text)
        {
            List<string> sections = new List<string>();

            // Regex para capturar seções baseadas em numeração ou títulos em maiúsculas
            Regex sectionRegex = new Regex(@"(^[0-9]+\..*|^[A-Z\s]+)$", RegexOptions.Multiline);

            MatchCollection matches = sectionRegex.Matches(text);
            int lastIndex = 0;

            foreach (Match match in matches)
            {
                int currentIndex = match.Index;

                if (lastIndex < currentIndex)
                {
                    string section = text.Substring(lastIndex, currentIndex - lastIndex).Trim();
                    if (!string.IsNullOrEmpty(section))
                        sections.Add(section);
                }

                lastIndex = currentIndex;
            }

            // Adiciona o último pedaço de texto
            if (lastIndex < text.Length)
            {
                string lastSection = text.Substring(lastIndex).Trim();
                if (!string.IsNullOrEmpty(lastSection))
                    sections.Add(lastSection);
            }

            return sections;
        }
        public async Task<string> DownloadFileFromUrl(string fileUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(fileUrl);
                response.EnsureSuccessStatusCode();

                string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf"); // Supondo que seja um PDF
                await using (FileStream fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fs);
                }

                return tempFilePath;
            }
        }

    }
}
