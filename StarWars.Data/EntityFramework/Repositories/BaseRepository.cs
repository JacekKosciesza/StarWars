using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StarWars.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWars.Data.EntityFramework.Repositories
{
    public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        protected DbContext _db;
        protected readonly ILogger _logger;

        protected BaseRepository() { }

        protected BaseRepository(DbContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public virtual Task<List<TEntity>> GetAll()
        {
            return _db.Set<TEntity>().ToListAsync();
        }

        public virtual Task<TEntity> Get(TKey id)
        {
            _logger.LogInformation("Get {type} with id = {id}", typeof(TEntity).Name, id);
            return _db.Set<TEntity>().SingleOrDefaultAsync(c => c.Id.Equals(id));
        }

        public virtual TEntity Add(TEntity entity)
        {
            _db.Set<TEntity>().Add(entity);
            return entity;
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _db.Set<TEntity>().AddRange(entities);
        }

        public virtual void Delete(TKey id)
        {
            var entity = new TEntity { Id = id };
            _db.Set<TEntity>().Attach(entity);
            _db.Set<TEntity>().Remove(entity);
        }

        public virtual async Task<bool> SaveChangesAsync()
        {
            return (await _db.SaveChangesAsync()) > 0;
        }

        public virtual void Update(TEntity entity)
        {
            _db.Set<TEntity>().Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }
    }
}
