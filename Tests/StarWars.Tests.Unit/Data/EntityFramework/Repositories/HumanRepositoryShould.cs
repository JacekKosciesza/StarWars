using System.Linq;
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
    public class HumanRepositoryShould
    {
        private readonly HumanRepository _humanRepository;
        private DbContextOptions<StarWarsContext> _options;
        private Mock<ILogger<StarWarsContext>> _dbLogger;
        public HumanRepositoryShould()
        {
            // Given
            _dbLogger = new Mock<ILogger<StarWarsContext>>();
            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory
            _options = new DbContextOptionsBuilder<StarWarsContext>()
                .UseInMemoryDatabase(databaseName: "StarWars_HumanRepositoryShould")
                .Options;
            using (var context = new StarWarsContext(_options, _dbLogger.Object))
            {
                context.EnsureSeedData();
            }
            var starWarsContext = new StarWarsContext(_options, _dbLogger.Object);
            var repoLogger = new Mock<ILogger<HumanRepository>>();
            _humanRepository = new HumanRepository(starWarsContext, repoLogger.Object);
        }

        [Fact]
        public async void ReturnLukeGivenIdOf1000()
        {
            // When
            var luke = await _humanRepository.Get(1000);

            // Then
            Assert.NotNull(luke);
            Assert.Equal("Luke Skywalker", luke.Name);
        }

        [Fact]
        public async void ReturnLukeFriendsAndEpisodes()
        {
            // When
            var character = await _humanRepository.Get(1000, includes: new[] { "CharacterEpisodes.Episode", "CharacterFriends.Friend" });

            // Then
            Assert.NotNull(character);
            Assert.NotNull(character.CharacterEpisodes);
            var episodes = character.CharacterEpisodes.Select(e => e.Episode.Title);
            Assert.Equal(new[] { "NEWHOPE", "EMPIRE", "JEDI" }, episodes);
            Assert.NotNull(character.CharacterFriends);
            var friends = character.CharacterFriends.Select(e => e.Friend.Name);
            Assert.Equal(new[] { "Han Solo", "Leia Organa", "C-3PO", "R2-D2" }, friends);
        }

        [Fact]
        public async void ReturnLukesHomePlanet()
        {
            // When
            var luke = await _humanRepository.Get(1000, include: "HomePlanet");

            // Then
            Assert.NotNull(luke);
            Assert.NotNull(luke.HomePlanet);
            Assert.Equal("Tatooine", luke.HomePlanet.Name);
        }

        [Fact]
        public async void AddNewHuman()
        {
            // Given
            var human10101 = new Human { Id = 10101, Name = "Human10101" };

            // When
            _humanRepository.Add(human10101);
            var saved = await _humanRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var human = await db.Humans.FindAsync(10101);
                Assert.NotNull(human);
                Assert.Equal(10101, human.Id);
                Assert.Equal("Human10101", human.Name);

                // Cleanup
                db.Humans.Remove(human);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void UpdateExistingHuman()
        {
            // Given
            var vader = await _humanRepository.Get(1001);
            vader.Name = "Human1001";

            // When
            _humanRepository.Update(vader);
            var saved = await _humanRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var human = await db.Humans.FindAsync(1001);
                Assert.NotNull(human);
                Assert.Equal(1001, human.Id);
                Assert.Equal("Human1001", human.Name);

                // Cleanup
                human.Name = "Darth Vader";
                db.Humans.Update(human);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void DeleteExistingHuman()
        {
            // Given
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var human10102 = new Human { Id = 10102, Name = "Human10102" };
                await db.Humans.AddAsync(human10102);
                await db.SaveChangesAsync();
            }

            // When
            _humanRepository.Delete(10102);
            var saved = await _humanRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var deletedHuman = await db.Humans.FindAsync(10101);
                Assert.Null(deletedHuman);
            }
        }
    }
}
