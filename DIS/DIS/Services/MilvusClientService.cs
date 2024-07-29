using Milvus.Client;
namespace DIS.Services;
public interface IMilvusClientService
{
    public Task CreateDatabaseAsync(string databaseName);
    
}
public class MilvusClientService : IMilvusClientService
{
    private readonly MilvusClient _milvusClient;
    public MilvusClientService(MilvusClient milvusClient)
    {
        _milvusClient = milvusClient;
    }
    public async Task CreateDatabaseAsync()
    {
        await _milvusClient.CreateDatabaseAsync("book");
    }
    public async Task CreateDatabaseAsync(string databaseName)
    {
        await _milvusClient.CreateDatabaseAsync(databaseName);
    }

    public async Task getDatabase()
    {
        var databases = await _milvusClient.ListDatabasesAsync();
        foreach (var database in databases)
        {
            Console.WriteLine(database);
        }
    }
}