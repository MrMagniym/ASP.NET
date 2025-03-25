using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Contexts;

namespace Pcf.Administration.DataAccess.Data;

public class MongoDbInitializer(MongoDbContext dataContext) : IDbInitializer
{
    private readonly MongoDbContext _dataContext = dataContext;

    public void InitializeDb()
    {
        var database = _dataContext.Database;

        database.DropCollection(nameof(Role));
        database.DropCollection(nameof(Employee));
        
        database.CreateCollection(nameof(Role));
        database.CreateCollection(nameof(Employee));

        var roles = database.GetCollection<Role>(nameof(Role));
        var employees = database.GetCollection<Employee>(nameof(Employee));

        roles.InsertMany(FakeDataFactory.Roles);
        employees.InsertMany(FakeDataFactory.Employees);
    }
}
