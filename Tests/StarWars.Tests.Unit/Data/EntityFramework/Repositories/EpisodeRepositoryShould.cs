using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StarWars.Core.Models;
using StarWars.Data.EntityFramework;
using StarWars.Data.EntityFramework.Repositories;
using StarWars.Data.EntityFramework.Seed;
using Xunit;

namespace StarWars.Tests.Unit.Data.EntityFramework.Repositories
{
    public class EpisodeRepositoryShould
    {
        private readonly EpisodeRepository _episodeRepository;
        private DbContextOptions<StarWarsContext> _options;
        private Mock<ILogger<StarWarsContext>> _dbLogger;
        public EpisodeRepositoryShould()
        {
            // Given
            _dbLogger = new Mock<ILogger<StarWarsContext>>();
            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory
            _options = new DbContextOptionsBuilder<StarWarsContext>()
                .UseInMemoryDatabase(databaseName: "StarWars_EpisodeRepositoryShould")
                .Options;
            using (var context = new StarWarsContext(_options, _dbLogger.Object))
            {
                context.EnsureSeedData();
            }
            var starWarsContext = new StarWarsContext(_options, _dbLogger.Object);
            var repoLogger = new Mock<ILogger<EpisodeRepository>>();
            _episodeRepository = new EpisodeRepository(starWarsContext, repoLogger.Object);
        }

        [Fact]
        public async void ReturnJediEpisodeGivenIdOf6()
        {
            // When
            var jedi = await _episodeRepository.Get(6);

            // Then
            Assert.NotNull(jedi);
            Assert.Equal("JEDI", jedi.Title);
        }

        [Fact]
        public async void AddNewEpisode()
        {
            // Given
            var episode101 = new Episode { Id = 101, Title = "Episode101" };

            // When
            _episodeRepository.Add(episode101);
            var saved = await _episodeRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var episode = await db.Episodes.FindAsync(101);
                Assert.NotNull(episode);
                Assert.Equal(101, episode.Id);
                Assert.Equal("Episode101", episode.Title);

                // Cleanup
                db.Episodes.Remove(episode);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void UpdateExistingEpisode()
        {
            // Given
            var newhope = await _episodeRepository.Get(4);
            newhope.Title = "Episode4";

            // When
            _episodeRepository.Update(newhope);
            var saved = await _episodeRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var episode = await db.Episodes.FindAsync(4);
                Assert.NotNull(episode);
                Assert.Equal(4, episode.Id);
                Assert.Equal("Episode4", episode.Title);

                // Cleanup
                episode.Title = "NEWHOPE";
                db.Episodes.Update(episode);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void DeleteExistingEpisode()
        {
            // Given
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var episode102 = new Episode { Id = 102, Title = "NEWHOPE" };
                await db.Episodes.AddAsync(episode102);
                await db.SaveChangesAsync();
            }

            // When
            _episodeRepository.Delete(102);
            var saved = await _episodeRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var deletedEpisode = await db.Episodes.FindAsync(102);
                Assert.Null(deletedEpisode);
            }
        }
    }
}
