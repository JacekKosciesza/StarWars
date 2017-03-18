using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarWars.Core.Models;

namespace StarWars.Core.Data
{
    public interface ICharacterRepository : IBaseRepository<Character, int>
    {
        Task<ICollection<Character>> GetFriends(int id);
        Task<ICollection<Episode>> GetEpisodes(int id);
    }
}
