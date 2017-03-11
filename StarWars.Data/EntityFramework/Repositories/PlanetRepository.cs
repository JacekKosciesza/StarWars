using StarWars.Core.Models;
using Microsoft.Extensions.Logging;
using StarWars.Core.Data;

namespace StarWars.Data.EntityFramework.Repositories
{
    public class PlanetRepository : BaseRepository<Planet, int>, IPlanetRepository
    {
        public PlanetRepository() { }

        public PlanetRepository(StarWarsContext db, ILogger<PlanetRepository> logger)
            : base(db, logger)
        {
        }
    }
}
