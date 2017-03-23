using System.Threading.Tasks;
using StarWars.Core.Models;

namespace StarWars.Core.Logic
{
    public interface ITrilogyHeroes
    {
        Task<Character> GetHero(int? episodeId);
    }
}
