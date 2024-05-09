using AsyncEnumerablePoC.Client.DataAccess.Model;
using MongoDB.Driver;

namespace AsyncEnumerablePoC.Client.DataAccess;

public class SaveDataDbSet
{
    public IMongoDatabase Database { get; }

    public SaveDataDbSet(IMongoClient client, string databaseName) => Database = client.GetDatabase(databaseName);

    public IMongoCollection<HistoricalTransformedData> HistoricalTransformedDataSets => 
        Database.GetCollection<HistoricalTransformedData>(nameof(HistoricalTransformedDataSets));
    
    public IMongoCollection<HistoricalTransformedComplexData> HistoricalTransformedComplexDataSets => 
        Database.GetCollection<HistoricalTransformedComplexData>(nameof(HistoricalTransformedComplexDataSets));
}