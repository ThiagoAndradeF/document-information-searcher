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

        // Correção no template de rota
        [HttpGet("{conversationId}/{collectionName}")]
        public async Task<ActionResult<string>> ContextualizeCollection([FromRoute] string collectionName, [FromRoute] string conversationId)
        {
            try
            {
                var result = await _TextAnalysisClient.CreateConversationIfNotExists(conversationId, collectionName);
                return Ok("Conversa criada com o id " + result);
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error querying the collection", ex);
            }
        }

        // Correção no template de rota
        [HttpGet("{conversationId}")]
        public async Task<ActionResult<string>> RetriverContext([FromRoute] string conversationId)
        {
            try
            {
                await _TextAnalysisClient.RetrieveHistoryContext(conversationId);
                return Ok("Conversa do id " + conversationId + " recuperada com sucesso!");
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error querying the collection", ex);
            }
        }

        // Correção no template de rota
        [HttpPost("{conversationId}")]
        public async Task<ActionResult> SendMessage([FromRoute] string conversationId, [FromBody] string query)
        {
            try
            {
                await _TextAnalysisClient.SendMessage(conversationId, query, null);
                return Ok("Mensagem enviada com sucesso!");
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error querying the collection", ex);
            }
        }
    }
}
