
using DIS.Services;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using Qdrant.Client;
using Qdrant.Client.Grpc;
public interface ITextAnalysisService
{
    Task CreateCollection(string filepath, string collectionName);
    Task<string> QueryByCollection(string query, string collectionName);
}
public class TextAnalysisService : ITextAnalysisService 
{
    private readonly OpenAIService _openAiService;
    private readonly VetorialDataBaseService _vetorialDatabaseService;
    private readonly DocumentService _documentService;
    public TextAnalysisService(OpenAIService openAIService, VetorialDataBaseService vetorialDatabaseService, DocumentService documentService)
    {
        _openAiService = openAIService;
        _vetorialDatabaseService = vetorialDatabaseService;
        _documentService = documentService;
    }
    public async Task CreateCollection(string filepath, string collectionName){
        try{
            List<string> chunks = _documentService.ProcessDocument(filepath);
            await _vetorialDatabaseService.UploadDataOnQdrant(chunks, collectionName);
        }
        catch(Exception ex){
            throw new Exception("There was an error creating the collection ", ex);
        }
    }
    public async Task<string> QueryByCollection(string query, string collectionName){
        try{
            var result  = await _vetorialDatabaseService.QueryByCollection(query, collectionName);
            return result;
        }
        catch(Exception ex){
            throw new Exception("There was an error querying the collection ", ex);
        }
    }
}