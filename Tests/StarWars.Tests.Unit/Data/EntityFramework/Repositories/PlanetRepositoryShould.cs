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
    public class PlanetRepositoryShould
    {
        private readonly PlanetRepository _planetRepository;
        private DbContextOptions<StarWarsContext> _options;
        private Mock<ILogger<StarWarsContext>> _dbLogger;
        public PlanetRepositoryShould()
        {
            // Given
            _dbLogger = new Mock<ILogger<StarWarsContext>>();
            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory
            _options = new DbContextOptionsBuilder<StarWarsContext>()
                .UseInMemoryDatabase(databaseName: "StarWars_PlanetRepositoryShould")
                .Options;
            using (var context = new StarWarsContext(_options, _dbLogger.Object))
            {
                context.EnsureSeedData();
            }
            var starWarsContext = new StarWarsContext(_options, _dbLogger.Object);
            var repoLogger = new Mock<ILogger<PlanetRepository>>();
            _planetRepository = new PlanetRepository(starWarsContext, repoLogger.Object);
        }

        [Fact]
        public async void ReturnTatooinePlanetGivenIdOf1()
        {
            // When
            var tatooine = await _planetRepository.Get(1);

            // Then
            Assert.NotNull(tatooine);
            Assert.Equal("Tatooine", tatooine.Name);
        }

        [Fact]
        public async void AddNewPlanet()
        {
            // Given
            var planet101 = new Planet { Id = 101, Name = "Planet101" };

            // When
            _planetRepository.Add(planet101);
            var saved = await _planetRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var planet = await db.Planets.FindAsync(101);
                Assert.NotNull(planet);
                Assert.Equal(101, planet.Id);
                Assert.Equal("Planet101", planet.Name);

                // Cleanup
                db.Planets.Remove(planet);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void UpdateExistingPlanet()
        {
            // Given
            var alderaan = await _planetRepository.Get(2);
            alderaan.Name = "Planet2";

            // When
            _planetRepository.Update(alderaan);
            var saved = await _planetRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var planet = await db.Planets.FindAsync(2);
                Assert.NotNull(planet);
                Assert.Equal(2, planet.Id);
                Assert.Equal("Planet2", planet.Name);

                // Cleanup
                planet.Name = "Alderaan";
                db.Planets.Update(planet);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void DeleteExistingPlanet()
        {
            // Given
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var planet102 = new Planet { Id = 102, Name = "Planet102" };
                await db.Planets.AddAsync(planet102);
                await db.SaveChangesAsync();
            }

            // When
            _planetRepository.Delete(102);
            var saved = await _planetRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var deletedPlanet = await db.Planets.FindAsync(102);
                Assert.Null(deletedPlanet);
            }
        }
    }
}
