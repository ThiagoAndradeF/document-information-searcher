
using Newtonsoft.Json;
using OpenAI.Managers;
using StackExchange.Redis;
public class ContextDataBaseService 
{
    private readonly OpenAIService _openAiService;
    private readonly IConnectionMultiplexer _redis;
    public ContextDataBaseService(OpenAIService openAIService, IConnectionMultiplexer redis)
    {
        _openAiService = openAIService;
        _redis = redis;
    }
    
    public async Task<string> StartConversationAsync(string initialPointId)
    {
        var db = _redis.GetDatabase();
        var conversationId = Guid.NewGuid().ToString();
        var createdAt = DateTime.UtcNow;
        await db.StringSetAsync($"conversation:{conversationId}:createdAt", createdAt.ToString("o")); // ISO 8601
        await db.StringSetAsync($"conversation:{conversationId}:initialPoint", initialPointId);
        await db.SetAddAsync($"conversation:{conversationId}:points", initialPointId);
        return conversationId;
    }

    public async Task AddMessageUserAsync(Guid conversationId, string content)
    {
        var db = _redis.GetDatabase();
        var message = new
        {
            Role = "user",
            Content = content,
            Moment = DateTime.UtcNow
        };
        var messageKey = $"conversation:{conversationId}:messages";
        await db.ListRightPushAsync(messageKey, JsonConvert.SerializeObject(message));
    }

    public async Task AddMessageAssistentAsync(Guid conversationId, string content)
    {
        var db = _redis.GetDatabase();
        var message = new
        {
            Role = "assistant",
            Content = content,
            Moment = DateTime.UtcNow
        };

        var messageKey = $"conversation:{conversationId}:messages";
        await db.ListRightPushAsync(messageKey, JsonConvert.SerializeObject(message));
    }

    public async Task<bool> AddPointToContextAsync(string conversationId, string pointId)
    {
        var db = _redis.GetDatabase();
        return await db.SetAddAsync($"conversation:{conversationId}:points", pointId);
    }

    public async Task<bool> IsPointInContextAsync(string conversationId, string pointId)
    {
        var db = _redis.GetDatabase();
        var pointsKey = $"conversation:{conversationId}:points";
        if (await db.SetContainsAsync(pointsKey, pointId))
        {
            var pointsListKey = $"conversation:{conversationId}:points:ordered";
            await db.ListRemoveAsync(pointsListKey, pointId);
            await db.ListRightPushAsync(pointsListKey, pointId);
            return true;
        }
        return false;
    }
    
    public async Task<List<string>> GetPointsInContextAsync(string conversationId)
    {
        var db = _redis.GetDatabase();
        var points = await db.SetMembersAsync($"conversation:{conversationId}:points");
        return points.Select(p => p.ToString()).ToList();
    }

    public async Task ClearContextAsync(string conversationId)
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync($"conversation:{conversationId}:points");
    }
    
}