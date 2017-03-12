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
    public class CharacterRepositoryShould
    {
        private readonly CharacterRepository _characterRepository;
        private DbContextOptions<StarWarsContext> _options;
        private Mock<ILogger<StarWarsContext>> _dbLogger;
        public CharacterRepositoryShould()
        {
            // Given
            _dbLogger = new Mock<ILogger<StarWarsContext>>();
            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory
            _options = new DbContextOptionsBuilder<StarWarsContext>()
                .UseInMemoryDatabase(databaseName: "StarWars_CharacterRepositoryShould")
                .Options;
            using (var context = new StarWarsContext(_options, _dbLogger.Object))
            {
                context.EnsureSeedData();
            }
            var starWarsContext = new StarWarsContext(_options, _dbLogger.Object);
            var repoLogger = new Mock<ILogger<CharacterRepository>>();
            _characterRepository = new CharacterRepository(starWarsContext, repoLogger.Object);
        }

        [Fact]
        public async void ReturnR2D2CharacterGivenIdOf2001()
        {
            // When
            var character = await _characterRepository.Get(2001);

            // Then
            Assert.NotNull(character);
            Assert.Equal("R2-D2", character.Name);
        }

        [Fact]
        public async void ReturnR2D2FriendsAndEpisodes()
        {
            // When
            var character = await _characterRepository.Get(2001, includes: new[] { "CharacterEpisodes.Episode", "CharacterFriends.Friend" });

            // Then
            Assert.NotNull(character);
            Assert.NotNull(character.CharacterEpisodes);
            var episodes = character.CharacterEpisodes.Select(e => e.Episode.Title);
            Assert.Equal(new[] { "NEWHOPE", "EMPIRE", "JEDI" }, episodes);
            Assert.NotNull(character.CharacterFriends);
            var friends = character.CharacterFriends.Select(e => e.Friend.Name);
            Assert.Equal(new[] { "Luke Skywalker", "Han Solo", "Leia Organa" }, friends);
        }

        [Fact]
        public async void ReturnLukeGivenIdOf1000()
        {
            // When
            var character = await _characterRepository.Get(1000);

            // Then
            Assert.NotNull(character);
            Assert.Equal("Luke Skywalker", character.Name);
        }

        [Fact]
        public async void ReturnLukeFriendsAndEpisodes()
        {
            // When
            var character = await _characterRepository.Get(1000, includes: new[] { "CharacterEpisodes.Episode", "CharacterFriends.Friend" });

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
        public async void AddNewCharacter()
        {
            // Given
            var character3101 = new Character { Id = 3101, Name = "Character3101" };

            // When
            _characterRepository.Add(character3101);
            var saved = await _characterRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var character = await db.Characters.FindAsync(3101);
                Assert.NotNull(character);
                Assert.Equal(3101, character.Id);
                Assert.Equal("Character3101", character.Name);

                // Cleanup
                db.Characters.Remove(character);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void AddNewDroid()
        {
            // Given
            var droid2101 = new Droid { Id = 2101, Name = "Droid2101", PrimaryFunction = "Function2101" };

            // When
            _characterRepository.Add(droid2101);
            var saved = await _characterRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var character = await db.Characters.FindAsync(2101);
                Assert.NotNull(character);
                Assert.IsType<Droid>(character);
                Assert.Equal(2101, character.Id);
                Assert.Equal("Droid2101", character.Name);

                // Cleanup
                db.Characters.Remove(character);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void AddNewHuman()
        {
            // Given
            var human10101 = new Human { Id = 10101, Name = "Human10101" };

            // When
            _characterRepository.Add(human10101);
            var saved = await _characterRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var character = await db.Characters.FindAsync(10101);
                Assert.NotNull(character);
                Assert.IsType<Human>(character);
                Assert.Equal(10101, character.Id);
                Assert.Equal("Human10101", character.Name);

                // Cleanup
                db.Characters.Remove(character);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void UpdateExistingCharacter()
        {
            // Given
            var threepio = await _characterRepository.Get(2000);
            threepio.Name = "Character2000";

            // When
            _characterRepository.Update(threepio);
            var saved = await _characterRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var character = await db.Characters.FindAsync(2000);
                Assert.NotNull(character);
                Assert.Equal(2000, character.Id);
                Assert.Equal("Character2000", character.Name);

                // Cleanup
                character.Name = "C-3PO";
                db.Characters.Update(character);
                await db.SaveChangesAsync();
            }
        }

        [Fact]
        public async void DeleteExistingCharacter()
        {
            // Given
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var character3100 = new Character { Id = 3100, Name = "Character3100" };
                await db.Characters.AddAsync(character3100);
                await db.SaveChangesAsync();
            }

            // When
            _characterRepository.Delete(3100);
            var saved = await _characterRepository.SaveChangesAsync();

            // Then
            Assert.True(saved);
            using (var db = new StarWarsContext(_options, _dbLogger.Object))
            {
                var deletedCharacter = await db.Characters.FindAsync(3100);
                Assert.Null(deletedCharacter);
            }
        }
    }
}
