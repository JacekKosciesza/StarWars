using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWars.Core.Data
{
    public interface IBaseRepository<TEntity, in TKey>
        where TEntity : class
    {
        Task<List<TEntity>> GetAll();
        Task<TEntity> Get(TKey id);
        TEntity Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Delete(TKey id);
        void Update(TEntity entity);
        Task<bool> SaveChangesAsync();
    }
}
