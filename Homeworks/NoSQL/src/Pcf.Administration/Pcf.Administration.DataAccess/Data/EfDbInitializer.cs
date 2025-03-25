namespace Pcf.Administration.DataAccess.Data;

public class EfDbInitializer(DataContext dataContext)
        : IDbInitializer
{
    private readonly DataContext _dataContext = dataContext;

    public void InitializeDb()
    {
        _dataContext.Database.EnsureDeleted();
        _dataContext.Database.EnsureCreated();
        
        _dataContext.AddRange(FakeDataFactory.Employees);
        _dataContext.SaveChanges();
    }
}