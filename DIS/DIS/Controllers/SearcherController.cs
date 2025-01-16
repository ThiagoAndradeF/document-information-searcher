using DIS.Services;
using Microsoft.AspNetCore.Mvc;
using DIS.Modules;
namespace DIS.Controllers
{
    [ApiController]
    [Route("api")] 
    public class SearcherController : ControllerBase
    {
        private readonly ITextAnalysisService _textAnalysisService;
        
        public SearcherController(ITextAnalysisService textAnalysisService)
        {
            _textAnalysisService = textAnalysisService;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetInformationByCollection()
        {
            try
            {
                string query = "Quais são os documentos necessários para participar desta licitação?Poderia me listar quais são as Habilitalções?";
                string collectionName = "Edital1";
                var result = await _textAnalysisService.QueryByCollection(query, collectionName);
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
                await _textAnalysisService.CreateCollection(filepath, collectionName);
                return Ok("Created Success!");
            }
            catch (Exception ex)
            {
               throw new Exception("There was an error querying the collection ", ex);
            }
        }
        
    }
}
