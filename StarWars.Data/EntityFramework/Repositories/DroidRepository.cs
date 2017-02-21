using StarWars.Core.Data;
using System.Threading.Tasks;
using StarWars.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace StarWars.Data.EntityFramework.Repositories
{
    public class DroidRepository : IDroidRepository
    {
        private StarWarsContext _db { get; set; }
        private readonly ILogger _logger;

        public DroidRepository(StarWarsContext db, ILogger<DroidRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public Task<Droid> Get(int id)
        {
            _logger.LogInformation("Get droid with id = {id}", id);
            return _db.Droids.FirstOrDefaultAsync(droid => droid.Id == id);
        }
    }
}
