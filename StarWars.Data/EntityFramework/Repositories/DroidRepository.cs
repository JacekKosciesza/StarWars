using StarWars.Core.Data;
using System.Threading.Tasks;
using StarWars.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace StarWars.Data.EntityFramework.Repositories
{
    public class DroidRepository : IDroidRepository
    {
        private StarWarsContext _db { get; set; }

        public DroidRepository(StarWarsContext db)
        {
            _db = db;
        }

        public Task<Droid> Get(int id)
        {
            return _db.Droids.FirstOrDefaultAsync(droid => droid.Id == id);
        }
    }
}
