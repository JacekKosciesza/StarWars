using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWars.Core.Data
{
    public interface IBaseRepository<TEntity, in TKey>
        where TEntity : class
    {
        Task<List<TEntity>> GetAll();
        Task<List<TEntity>> GetAll(string include);
        Task<List<TEntity>> GetAll(IEnumerable<string> includes);

        Task<TEntity> Get(TKey id);
        Task<TEntity> Get(TKey id, string include);
        Task<TEntity> Get(TKey id, IEnumerable<string> includes);

        TEntity Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Delete(TKey id);
        void Update(TEntity entity);
        Task<bool> SaveChangesAsync();
    }
}
