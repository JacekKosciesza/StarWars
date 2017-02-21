using StarWars.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarWars.Core.Models;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace StarWars.Data.InMemory
{
    public class DroidRepository : IDroidRepository
    {
        private readonly ILogger _logger;

        public DroidRepository(ILogger<DroidRepository> logger)
        {
            _logger = logger;
        }

        private List<Droid> _droids = new List<Droid> {
            new Droid { Id = 1, Name = "R2-D2" }
        };

        public Task<Droid> Get(int id)
        {
            _logger.LogInformation("Get droid with id = {id}", id);
            return Task.FromResult(_droids.FirstOrDefault(droid => droid.Id == id));
        }
    }
}
