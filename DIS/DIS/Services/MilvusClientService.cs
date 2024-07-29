using Milvus.Client;
namespace DIS.Services;

public class MilvusClientService
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

    public async Task getDatabase()
    {
        var databases = await _milvusClient.ListDatabasesAsync();
        foreach (var database in databases)
        {
            Console.WriteLine(database);
        }
    }
}