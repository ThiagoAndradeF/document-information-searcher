
using Newtonsoft.Json;
using OpenAI.ObjectModels.RequestModels;
using Qdrant.Client.Grpc;
using StackExchange.Redis;
public class Context 
{
    private readonly IConnectionMultiplexer _redis;
    private readonly MathService _mathService;

    public Context(IConnectionMultiplexer redis, MathService mathService)
    {
        _redis = redis;
        _mathService = mathService;
    }
    public async Task<string> StartConversationAsync(string initialMessage)
    {
        var db = _redis.GetDatabase();
        var conversationId = Guid.NewGuid().ToString();
        var createdAt = DateTime.UtcNow;
        var formatedMessage = ChatMessage.FromUser(initialMessage);
        await db.StringSetAsync($"conversation:{conversationId}:createdAt", createdAt.ToString("o")); // ISO 8601
        var messageKey = $"conversation:{conversationId}:messages";
        await db.ListRightPushAsync(messageKey, JsonConvert.SerializeObject(formatedMessage));
        return conversationId;
    }
    public async Task AddMessageHistoryAsync(string conversationId, string content, bool isUser)
    {
        var db = _redis.GetDatabase();
        var formatedMessage = isUser? ChatMessage.FromUser(content) : ChatMessage.FromAssistant(content);
        var messageKey = $"conversation:{conversationId}:messages";
        await db.ListRightPushAsync(messageKey, JsonConvert.SerializeObject(formatedMessage));
    }
    public async Task AddHistoryReferencePointAsync(PointStruct pointStruct, string conversationId)
    {
        var db = _redis.GetDatabase();
        //LIST EXISTENT POINTS
        var points = await GetPointsInContextAsync(conversationId); 
        //COMPARE ALREDY EXIST THIS SPECIFIC POINT
        if(points.Any(p=>p.Id == pointStruct.Id)){
            return;
        }
        //COMPARE ALREDY EXIST SIMILAR POINTS AND IF DON'T EXIST IT, ADD
        if(hasSimilar(points, pointStruct)){
            return; 
        }else{
            await db.ListRightPushAsync($"conversation:{conversationId}:points", JsonConvert.SerializeObject(pointStruct));
        }
    }

    private bool hasSimilar(List<PointStruct> pointStructs, PointStruct newPoint)
    {
        foreach (var point in pointStructs)
        {
           
            var newPointVector = newPoint.Vectors.Vector.Data.Select(d => (float)d).ToArray();
            var existingPointVector = point.Vectors.Vector.Data.Select(d => (float)d).ToArray();

            var similarity = _mathService.CalculateCosineSimilarity(newPointVector, existingPointVector);

            if (similarity > 0.9) // Limite de similaridade ajustável
                return true; 
        }
        return false;
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
    
    public async Task<List<PointStruct?>> GetPointsInContextAsync(string conversationId)
    {
        var db = _redis.GetDatabase();
        var points = await db.SetMembersAsync($"conversation:{conversationId}:points");
        
        return points
            .Select(point => JsonConvert.DeserializeObject<PointStruct>(point))
            .Where(point => point != null) 
            .ToList();
    }


    public async Task ClearContextAsync(string conversationId)
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync($"conversation:{conversationId}:points");
    }

    
    
}