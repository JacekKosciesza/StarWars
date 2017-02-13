using StarWars.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.Data.EntityFramework.Seed
{
    public class StarWarsSeedData
    {
        private readonly StarWarsContext _db;
        public StarWarsSeedData(StarWarsContext db)
        {
            _db = db;
        }

        public async Task EnsureSeedData()
        {
            if (!_db.Droids.Any())
            {
                var droid = new Droid
                {
                    Id = 1,
                    Name = "R2-D2"
                };
                _db.Droids.Add(droid);
                await _db.SaveChangesAsync();
            }
        }
    }
}
