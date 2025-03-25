using MongoDB.Driver;

namespace Pcf.Administration.DataAccess.Contexts;

public class MongoDbContext(IMongoClient client, string databaseName)
{
    public IMongoDatabase Database { get; } = client.GetDatabase(databaseName);
}
