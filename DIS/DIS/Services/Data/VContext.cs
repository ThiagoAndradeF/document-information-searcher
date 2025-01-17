using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using Qdrant.Client;
using Qdrant.Client.Grpc;
public class VContext 
{
    private readonly OpenAIService _openAiService;
    private readonly QdrantClient _qdrantClient;
    public VContext(OpenAIService openAIService, QdrantClient qdrantClient)
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
                    Payload = {{"document_content", chunks[i]}}
                };
                embedChunks.Add(point);
            }
            // Verificar se a coleção já existe
            bool collectionExists = await _qdrantClient.CollectionExistsAsync(collectionName);
            if (collectionExists)
            {
                throw new Exception("Collection alred exist");
            }
            var vectorConfig = new VectorParams
            {
                Size = 1536,
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
    public async Task<string> QueryByCollection(string query,string collectionName,string briefExplanationAboutTheQuery="The following information is an excerpt from a document that is related to this question. Use these excerpts to help you answer the questions."){
        var embedQuery = await GetEmbeddingAsync(query);
        var result = await _qdrantClient.SearchAsync(collectionName, embedQuery, null);
        if(result==null){
            return "No related information found in the document in this collection";
        }
        List<string> documents = new List<string>();
        foreach (var scoredPoint in result)
        {
            documents.Add(scoredPoint.Payload["document_content"].ToString());
        }
        return string.Join(briefExplanationAboutTheQuery,documents);
    }

    //For this method to work, add REDIS
    public async Task RemoveInativeCollection(string collectionName, int timeInMinutes)
    {
        try{
            
            await _qdrantClient.GetCollectionInfoAsync(collectionName);
            await _qdrantClient.DeleteCollectionAsync(collectionName);
        }
        catch(Exception ex){
            throw new Exception("There was an error removing the collection ", ex);
        }
    }


    // queryForGPT += $"\n\nA pergunta é:\n{query}";
    //     List<ChatMessage> messages = new List<ChatMessage>();
    //     messages.Add(ChatMessage.FromUser(queryForGPT));
    //     var completionResult = await _openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
    //     {
    //         Messages = messages,
    //         Model = Models.Gpt_3_5_Turbo,
    //         Temperature = 0.5f
    //     });
    // // Retorna a resposta do GPT ou um erro
    //     if (completionResult.Successful)
    //     {
    //         return completionResult.Choices.First().Message.Content;
    //     }
    //     else
    //     {
    //         return $"Failed to process request: {completionResult.Error?.Code}: {completionResult.Error?.Message}";
    //     }
}