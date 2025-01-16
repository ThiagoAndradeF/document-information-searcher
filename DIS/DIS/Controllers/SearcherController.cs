using DIS.Services;
using Microsoft.AspNetCore.Mvc;
using DIS.Modules;
namespace DIS.Controllers
{
    [ApiController]
    [Route("api/")] 
    public class SearcherController : ControllerBase
    {
        private readonly TextAnalysisService _textAnalysisService;
        
        public SearcherController(TextAnalysisService textAnalysisService)
        {
            _textAnalysisService = textAnalysisService;
        }

        [HttpGet("{collectionName:string}")]
        public async Task<ActionResult<string>> GetInformationByCollection([FromBody]string query, [FromRoute]string collectionName)
        {
            try
            {
                var result = await _textAnalysisService.QueryByCollection(query, collectionName);
                return Ok("Query Result!:  " + result);
            }
            catch (Exception ex)
            {
               throw new Exception("There was an error querying the collection ", ex);
            }
        }
        [HttpPost("{collectionName:string}")]
        public async Task<ActionResult> CreateCollection([FromBody]string filePath, string collectionName)
        {
            try
            {
                await _textAnalysisService.CreateCollection(filePath, collectionName);
                return Ok("Created Success!");
            }
            catch (Exception ex)
            {
               throw new Exception("There was an error querying the collection ", ex);
            }
        }
        
    }
}
