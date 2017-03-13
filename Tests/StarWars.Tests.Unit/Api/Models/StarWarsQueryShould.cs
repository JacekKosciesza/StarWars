using Moq;
using StarWars.Api.Models;
using StarWars.Data.EntityFramework.Repositories;
using Xunit;

namespace StarWars.Tests.Unit.Api.Models
{
    public class StarWarsQueryShould
    {
        [Fact]
        [Trait("test", "unit")]
        public void HaveHeroField()
        {
            // Given
            var droidRepository = new Mock<DroidRepository>();

            // When
            var starWarsQuery = new StarWarsQuery(droidRepository.Object);            

            // Then
            Assert.NotNull(starWarsQuery);
            Assert.True(starWarsQuery.HasField("hero"));
        }
    }
}
