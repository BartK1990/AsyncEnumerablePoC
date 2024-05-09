using MongoDB.Driver;

namespace AsyncEnumerablePoC.Client.DataAccess;
public class SaveDataDbContext
{
    private readonly string _database;
    private readonly IMongoClient _client;

    public SaveDataDbContext(string connectionString, string database)
    {
        _database = database;
        _client = new MongoClient(connectionString);
    }

    public  IMongoDatabase Database => _client.GetDatabase(_database);

    public SaveDataDbSet DbSet => new(_client, _database);
}