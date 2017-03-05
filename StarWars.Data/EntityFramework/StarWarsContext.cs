using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using StarWars.Core.Models;

namespace StarWars.Data.EntityFramework
{
    public class StarWarsContext : DbContext
    {
        public readonly ILogger _logger;
        private bool _migrations;

        public StarWarsContext() {
            _migrations = true;
        }
        public StarWarsContext(DbContextOptions options, ILogger<StarWarsContext> logger)
            : base(options)
        {
            _logger = logger;
            //Database.EnsureCreated();
            //Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_migrations)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=StarWars;Integrated Security=SSPI;integrated security=true;MultipleActiveResultSets=True;");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // https://docs.microsoft.com/en-us/ef/core/modeling/relationships
            // http://stackoverflow.com/questions/38520695/multiple-relationships-to-the-same-table-in-ef7core

            // episodes
            modelBuilder.Entity<Episode>().HasKey(c => c.Id);
            modelBuilder.Entity<Episode>().Property(e => e.Id).ValueGeneratedNever();

            // planets
            modelBuilder.Entity<Planet>().HasKey(c => c.Id);
            modelBuilder.Entity<Planet>().Property(e => e.Id).ValueGeneratedNever();

            // characters
            modelBuilder.Entity<Character>().HasKey(c => c.Id);
            modelBuilder.Entity<Character>().Property(e => e.Id).ValueGeneratedNever();

            // characters-friends
            modelBuilder.Entity<CharacterFriend>().HasKey(t => new { t.CharacterId, t.FriendId});

            modelBuilder.Entity<CharacterFriend>()
                .HasOne(cf => cf.Character)
                .WithMany(c => c.CharacterFriends)
                .HasForeignKey(cf => cf.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CharacterFriend>()
                .HasOne(cf => cf.Friend)
                .WithMany(t => t.FriendCharacters)
                .HasForeignKey(cf => cf.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            // characters-episodes
            modelBuilder.Entity<CharacterEpisode>().HasKey(t => new { t.CharacterId, t.EpisodeId });

            modelBuilder.Entity<CharacterEpisode>()
                .HasOne(cf => cf.Character)
                .WithMany(c => c.CharacterEpisodes)
                .HasForeignKey(cf => cf.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CharacterEpisode>()
                .HasOne(cf => cf.Episode)
                .WithMany(t => t.CharacterEpisodes)
                .HasForeignKey(cf => cf.EpisodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // humans
            modelBuilder.Entity<Human>().HasOne(h => h.HomePlanet).WithMany(p => p.Humans);
        }

        public virtual DbSet<Episode> Episodes { get; set; }
        public virtual DbSet<Planet> Planets { get; set; }
        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<CharacterFriend> CharacterFriends { get; set; }
        public virtual DbSet<CharacterEpisode> CharacterEpisodes { get; set; }
        public virtual DbSet<Droid> Droids { get; set; }
        public virtual DbSet<Human> Humans { get; set; }
    }
}
