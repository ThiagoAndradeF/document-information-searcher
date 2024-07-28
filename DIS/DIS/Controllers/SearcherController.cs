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

        public SearcherController(DocumentService documentationService)
        {
            _documentationService = documentationService;
        }

        [HttpPost]
        public async Task<ActionResult<List<string>>> GetChunks([FromBody] FilePathRequest request)
        {
            if (string.IsNullOrEmpty(request.FilePath))
            {
                return BadRequest("File path is required.");
            }

            try
            {
                List<string> chunks = _documentationService.ProcessDocument(request.FilePath);
                return Ok(chunks);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the document: " + ex.Message);
            }
        }
    }
}
