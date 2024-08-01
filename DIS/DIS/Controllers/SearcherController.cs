using DIS.Services;
using Microsoft.AspNetCore.Mvc;
using DIS.Modules;
namespace DIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class SearcherController : ControllerBase
    {
        private readonly DocumentService _documentationService;
        private readonly TextAnalysisService _textAnalysisService;
        
        public SearcherController(DocumentService documentationService, TextAnalysisService textAnalysisService)
        {
            _documentationService = documentationService;
            _textAnalysisService = textAnalysisService;
        }

        [HttpPost]
        public async Task<ActionResult<List<string>>> GetInformationByDocument([FromBody] FilePathRequest request)
        {
            if (string.IsNullOrEmpty(request.FilePath))
            {
                return BadRequest("File path is required.");
            }
            try
            {
                List<string> chunks = _documentationService.ProcessDocument(request.FilePath);
                var result = _textAnalysisService.AnalyzeTextAsync(chunks);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the document: " + ex.Message);
            }
        }
        
    }
}
