using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StarWars.Core.Models;

namespace StarWars.Data.EntityFramework
{
    public class StarWarsContext : DbContext
    {
        private readonly IConfigurationRoot _config;

        public StarWarsContext() { }

        public StarWarsContext(IConfigurationRoot config, DbContextOptions options)
            : base(options)
        {
            _config = config;
        }

        public DbSet<Droid> Droids { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(_config["ConnectionStrings:StarWarsDatabaseConnection"]);
        }
    }
}
