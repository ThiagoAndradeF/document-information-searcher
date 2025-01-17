
using DIS.Services;
using OpenAI.Managers;
public interface ITextAnalysisClient
{
    Task CreateCollection(string filepath, string collectionName);
    Task<string> QueryByCollection(string query, string collectionName);
}
public class TextAnalysisClient : ITextAnalysisClient 
{
    private readonly OpenAIService _openAiService;
    private readonly VContext _vContext;
    private readonly DocumentService _documentService;
    public TextAnalysisClient(OpenAIService openAIService, VContext vContext, DocumentService documentService)
    {
        _openAiService = openAIService;
        _vContext = vContext;
        _documentService = documentService;
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
    public async Task<string> QueryByCollection(string query, string collectionName){
        try{
            var result  = await _vContext.QueryByCollection(query, collectionName);
            return result;
        }
        catch(Exception ex){
            throw new Exception("There was an error querying the collection ", ex);
        }
    }
}