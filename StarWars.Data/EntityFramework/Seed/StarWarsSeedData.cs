using StarWars.Core.Models;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace StarWars.Data.EntityFramework.Seed
{
    public static class StarWarsSeedData
    {
        public static void EnsureSeedData(this StarWarsContext db)
        {
            db._logger.LogInformation("Seeding database");
            if (!db.Droids.Any())
            {
                db._logger.LogInformation("Seeding droids");
                var droid = new Droid
                {
                    Name = "R2-D2"
                };
                db.Droids.Add(droid);
                db.SaveChanges();
            }
        }
    }
}
