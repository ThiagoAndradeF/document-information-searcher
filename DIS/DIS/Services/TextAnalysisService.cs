using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.Managers;
public interface ITextAnalysisService
{
    Task<string> AnalyzeTextAsync(List<string> texts);
}
public class TextAnalysisService : ITextAnalysisService
{
    private readonly OpenAIService _openAIService;

    public TextAnalysisService(OpenAIService openAIService)
    {
        _openAIService = openAIService;
    }

    public async Task<string> AnalyzeTextAsync(List<string> texts)
    {
        List<ChatMessage> messages = new List<ChatMessage>();
        foreach (var text in texts)
        {
            messages.Add(ChatMessage.FromUser(text));
        }
        messages.Insert(0, ChatMessage.FromSystem("You are a analizer to texts"));

        var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = Models.Gpt_3_5_Turbo,
            Temperature = 0.5f
        });

        if (completionResult.Successful)
        {
            return completionResult.Choices.First().Message.Content;
        }
        else
        {
            return $"Failed to process request: {completionResult.Error?.Code}: {completionResult.Error?.Message}";
        }
    }
}

