
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
public interface ITextAnalysisService
{
    Task<bool> UploadDataOnQdrant(List<string> chuncks);
}
public class TextAnalysisService : ITextAnalysisService
{
    private readonly OpenAIService _openAiService;

    public TextAnalysisService(OpenAIService openAIService)
    {
        _openAiService = openAIService;
    }

    public async Task<bool> UploadDataOnQdrant(List<string> chunks)
    {
        try{
            var embedChuncks = new List<List<double>>();
            foreach (var chunk in chunks)
            {
                var embedding = await GetEmbeddingAsync(chunk);
                embedChuncks.Add(embedding);
            }




            return true;
        }catch(Exception ex){
            throw new Exception("there was an error loading data into qdrant ", ex);
        }
    }
    private async Task<List<double>> GetEmbeddingAsync(string chunk)
    {
        try{
            var embeddingResult = await _openAiService.Embeddings.CreateEmbedding(new EmbeddingCreateRequest
            {
                Input = chunk,
                Model = "text-embedding-ada-002"
            });

            if (embeddingResult.Successful)
            {
                return embeddingResult.Data.First().Embedding;
            }
            else
            {
                Console.WriteLine($"Embedding Error: {embeddingResult.Error?.Message}");
                return new List<double>();
            }
        }catch{
            throw new Exception("there was an error getting embedding from openai");
        }
    }
}


//     public async Task<bool> InsertDocumentOnQdrant(Guid tenantId, string blobPath, string fileName)
// {
//     try
//     {
//         // Nome da coleção concatenando tenantId e fileName
//         string collectionName = $"{tenantId}_{fileName}";
//         // Inserir mensagem de sistema
//         List<ChatMessage> messages = new List<ChatMessage>
//         {
//             ChatMessage.FromSystem("Seu nome é Lummy, uma assistente virtual especializada em responder dúvidas sobre licitações públicas. Analise cuidadosamente o conteúdo do documento fornecido e extraia todas as informações relevantes. Você deve utilizar essas informações como base para responder exclusivamente a perguntas relacionadas à licitação especificada no documento. Mantenha suas respostas precisas, objetivas e alinhadas ao que está escrito no material fornecido. Não inclua informações externas ou irrelevantes ao contexto do documento.")
//         };

//         // Obter informações do arquivo no blob storage
//         var blobFileInfo = await _fileService.GetFileAsync(tenantId, blobPath, fileName);
//         if (blobFileInfo == null)
//         {
//             throw new Exception("Arquivo não encontrado.");
//         }

//         // Extrair o conteúdo do documento
//         string content = _extractorService.ProcessDocumentByGemni(blobFileInfo);

//         // Gerar valores embutidos (embeddings) usando o cliente Gemini
//         var embeddedValues = await _geminiClient.EmbeddedContentsPrompt(content);

//         // Verificar se a coleção já existe
//         bool collectionExists = await _qdrantClient.CollectionExistsAsync(collectionName);
//         if (collectionExists)
//         {
//             return true;
//         }
//         return false;

//     }
//     catch{
//         throw new Exception("there was an error inserting document into qdrant");
//     }
// }

