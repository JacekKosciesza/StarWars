using StarWars.Core.Models;
using Microsoft.Extensions.Logging;
using StarWars.Core.Data;

namespace StarWars.Data.EntityFramework.Repositories
{
    public class CharacterRepository : BaseRepository<Character, int>, ICharacterRepository
    {
        public CharacterRepository() { }

        public CharacterRepository(StarWarsContext db, ILogger<CharacterRepository> logger)
            : base(db, logger)
        {
        }
    }
}
