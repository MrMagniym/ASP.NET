using MongoDB.Driver;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain;
using Pcf.Administration.DataAccess.Contexts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pcf.Administration.DataAccess.Repositories;

public class MongoRepository<T>(MongoDbContext context) : IRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection = context.Database.GetCollection<T>(typeof(T).Name);

    public async Task AddAsync(T entity)
        => await _collection.InsertOneAsync(entity);

    public async Task DeleteAsync(T entity)
        => await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id));

    public async Task UpdateAsync(T entity)
        => await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity);    

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();    

    public async Task<T> GetByIdAsync(Guid id)
        => await _collection.Find(Builders<T>.Filter.Eq(e => e.Id, id)).FirstOrDefaultAsync();    

    public async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
        => await _collection.Find(Builders<T>.Filter.Where(predicate)).FirstOrDefaultAsync();
    
    public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
        => await _collection.Find(Builders<T>.Filter.In(e => e.Id, ids)).ToListAsync();    

    public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        => await _collection.Find(Builders<T>.Filter.Where(predicate)).ToListAsync();

}
