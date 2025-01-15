
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using Qdrant.Client;
using Qdrant.Client.Grpc;
public interface ITextAnalysisService
{
    Task<bool> UploadDataOnQdrant(List<string> chuncks, string collectionName);
}
public class TextAnalysisService : ITextAnalysisService
{
    private readonly OpenAIService _openAiService;
    private readonly QdrantClient _qdrantClient;
    public TextAnalysisService(OpenAIService openAIService, QdrantClient qdrantClient)
    {
        _openAiService = openAIService;
        _qdrantClient = qdrantClient;
    }

    public async Task<bool> UploadDataOnQdrant(List<string> chunks, string collectionName)
    {
        try{
            var embedChunks = new List<PointStruct>();
            for(int i=0;i<chunks.Count();i++){
                var embedding = await GetEmbeddingAsync(chunks[i]);
                var point = new PointStruct
                {
                    // Gerando um ID único para o ponto
                    Id = (ulong)i,
                    // Convertendo o array de double para o formato esperado (float[])
                    Vectors = embedding.Select(d => (float)d).ToArray(),
                    Payload = {}
                };
            }
            // Verificar se a coleção já existe
            bool collectionExists = await _qdrantClient.CollectionExistsAsync(collectionName);
            if (collectionExists)
            {
                throw new Exception("Collection alred exist");
            }
            var vectorConfig = new VectorParams
            {
                Size = (ulong)embedChunks.Count(),
                Distance = Distance.Cosine
            };
            await _qdrantClient.CreateCollectionAsync(collectionName, vectorConfig);
            var operationInfo = await _qdrantClient.UpsertAsync(collectionName, embedChunks);
            return true;
        }
        catch(Exception ex){
            throw new Exception("There was an error loading data into qdrant ", ex);
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