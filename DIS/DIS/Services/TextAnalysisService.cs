
using DIS.Services;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using Qdrant.Client.Grpc;
public interface ITextAnalysisClient
{
    Task CreateCollection(string filepath, string collectionName);
    Task<string> CreateConversationIfNotExists(string conversationId, string? collectionName);
    Task RetrieveHistoryContext(string conversationId);
    Task<string> SendMessage(string conversationId, string query, string instructionByDocument);
}
public class TextAnalysisClient : ITextAnalysisClient 
{
    private readonly OpenAIService _openAiService;
    private readonly VContext _vContext;
    private readonly DocumentService _documentService;
    private readonly Context _context;
    private List<ChatMessage?>? memory_messages = new List<ChatMessage>();
    private List<ScoredPoint?>? memory_points = new List<ScoredPoint>();
    private string? collection_name; 
    public TextAnalysisClient(OpenAIService openAIService, VContext vContext, DocumentService documentService, Context context)
    {
        _openAiService = openAIService;
        _vContext = vContext;
        _documentService = documentService;
        _context = context;
    }
    public async Task CreateCollection(string filepath, string collectionName){
        try{
            List<string> chunks = _documentService.ProcessDocument(filepath);
            await _vContext.UploadDataOnQdrant(chunks, collectionName);
        }
        catch(Exception ex){
            throw new Exception("There was an error creating the collection ", ex);
        }
    }
    public async Task<string> CreateConversationIfNotExists(string conversationId, string? collectionName){
        try{
            //VERIFICAR SE CONVERSA EXISTE NO HISTORICO 
            if(!await _context.ConversationExistsAsync(conversationId) && collectionName != null){
                return await _context.StartConversationAsync(collectionName);
            }
            return "Conversation With History!";
        }
        catch(Exception ex){
            throw new Exception("There was an error querying the collection ", ex);
        }
    }
    //RECUPERAR CONTEXTO DO HISORICO DE MENSAGENS E PESQUISAS
    public async Task RetrieveHistoryContext(string conversationId){
        memory_messages = await _context.GetMessagesHistoryAsync(conversationId);
        memory_points = await _context.GetPointsInContextAsync(conversationId);
        collection_name = await _context.GetCollectionNameAsync(conversationId);
    }
    //ADICIONAR MENSAGEM NO HISTORICO
    public async Task<string> SendMessage(string conversationId, string query,  string instructionByDocument = "Use these excerpts as a basis for your answers."){
        await _context.AddMessageHistoryAsync(conversationId, query, true);
        //QUERY NO QDRABT
        var referencialResult = await _vContext.QueryByCollection(query, collection_name);
        //SALVAR POINTS RETORNADOS PELO QDRANT E ATUALIZAR INMEMORY TAMBEM 
        foreach (var point in referencialResult)
        {
            await _context.AddHistoryReferencePointAsync(point, conversationId, memory_points);
        }   
        string informationByDocument = instructionByDocument;
        for (int i = 0; i < memory_points.Count(); i++)
        {
            informationByDocument += "\n";   
            informationByDocument += memory_points[i].Payload["document_content"];
        }
        memory_messages.Insert(0, ChatMessage.FromSystem(informationByDocument));
        var completionResult = await _openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = memory_messages,
            Model = Models.Gpt_3_5_Turbo,
            Temperature = 0.5f
        });
        var responseAssistant = completionResult.Choices.First().Message.Content;
        if (completionResult.Successful)
        {
            memory_messages.Add(ChatMessage.FromAssistant(responseAssistant)); 
            await _context.AddMessageHistoryAsync(conversationId, responseAssistant, false);
            return responseAssistant;
        }
        else
        {
            return $"Failed to process request: {completionResult.Error?.Code}: {completionResult.Error?.Message}";
        }

    }




}