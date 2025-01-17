using Microsoft.AspNetCore.Mvc;
namespace DIS.Controllers
{
    [ApiController]
    [Route("api")] 
    public class SearcherController : ControllerBase
    {
        private readonly ITextAnalysisClient _TextAnalysisClient;
        public SearcherController(ITextAnalysisClient TextAnalysisClient)
        {
            _TextAnalysisClient = TextAnalysisClient;
        }
        [HttpGet]
        public async Task<ActionResult<string>> GetInformationByCollection()
        {
            try
            {
                string query = "Quais são os documentos necessários para participar desta licitação?Poderia me listar quais são as Habilitalções?";
                string collectionName = "Edital1";
                var result = await _TextAnalysisClient.QueryByCollection(query, collectionName);
                return Ok("Query Result!:  " + result);
            }
            catch (Exception ex)
            {
               throw new Exception("There was an error querying the collection ", ex);
            }
        }
        [HttpPost]
        public async Task<ActionResult> CreateCollection()
        {
            try
            {   string filepath = "C:\\Edital.pdf";
                string collectionName = "Edital1";
                await _TextAnalysisClient.CreateCollection(filepath, collectionName);
                return Ok("Created Success!");
            }
            catch (Exception ex)
            {
               throw new Exception("There was an error querying the collection ", ex);
            }
        }
        
    }
}
