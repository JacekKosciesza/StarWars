using StarWars.Core.Models;
using Microsoft.Extensions.Logging;
using StarWars.Core.Data;

namespace StarWars.Data.EntityFramework.Repositories
{
    public class EpisodeRepository : BaseRepository<Episode, int>, IEpisodeRepository
    {
        public EpisodeRepository() { }

        public EpisodeRepository(StarWarsContext db, ILogger<EpisodeRepository> logger)
            : base(db, logger)
        {
        }
    }
}
