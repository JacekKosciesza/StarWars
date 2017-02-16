using Microsoft.EntityFrameworkCore;
using StarWars.Core.Models;
using StarWars.Data.EntityFramework;
using StarWars.Data.EntityFramework.Repositories;
using Xunit;

namespace StarWars.Tests.Unit.Data.EntityFramework.Repositories
{
    public class DroidRepositoryShould
    {
        private readonly DroidRepository _droidRepository;
        public DroidRepositoryShould()
        {
            // Given
            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory
            var options = new DbContextOptionsBuilder<StarWarsContext>()
                .UseInMemoryDatabase(databaseName: "StarWars")
                .Options;
            using (var context = new StarWarsContext(options))
            {
                context.Droids.Add(new Droid { Id = 1, Name = "R2-D2" });
                context.SaveChanges();
            }
            var starWarsContext = new StarWarsContext(options);
            _droidRepository = new DroidRepository(starWarsContext);
        }

        [Fact]
        public async void ReturnR2D2DroidGivenIdOf1()
        {
            // When
            var droid = await _droidRepository.Get(1);

            // Then
            Assert.NotNull(droid);
            Assert.Equal("R2-D2", droid.Name);
        }
    }
}
