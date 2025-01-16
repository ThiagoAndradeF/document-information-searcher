
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using Qdrant.Client;
using Qdrant.Client.Grpc;
public class VetorialDataBaseService 
{
    private readonly OpenAIService _openAiService;
    private readonly QdrantClient _qdrantClient;
    public VetorialDataBaseService(OpenAIService openAIService, QdrantClient qdrantClient)
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
                    Vectors = embedding,
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
    public async Task<float[]> GetEmbeddingAsync(string chunk)
    {
        try{
            var embeddingResult = await _openAiService.Embeddings.CreateEmbedding(new EmbeddingCreateRequest
            {
                Input = chunk,
                Model = "text-embedding-ada-002"
            });

            if (embeddingResult.Successful)
            {
                return embeddingResult.Data.First().Embedding.Select(d => (float)d).ToArray();
            }
            else
            {
                Console.WriteLine($"Embedding Error: {embeddingResult.Error?.Message}");
                return [];
            }
        }catch{
            throw new Exception("there was an error getting embedding from openai");
        }
    }
    public async Task<string> QueryByCollection(string query,string collectionName){
        var embedQuery = await GetEmbeddingAsync(query);
        var result = await _qdrantClient.SearchAsync(collectionName, embedQuery, null);
        if(result==null){
            return "No related information found in the document in this collection";
        }

        return "";
    }
}