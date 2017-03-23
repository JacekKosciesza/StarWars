using AutoMapper;
using Moq;
using StarWars.Api.Models;
using StarWars.Core.Data;
using StarWars.Core.Logic;
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
            var trilogyHeroes = new Mock<ITrilogyHeroes>();
            var droidRepository = new Mock<IDroidRepository>();
            var humanRepository = new Mock<IHumanRepository>();
            var mapper = new Mock<IMapper>();

            // When
            var starWarsQuery = new StarWarsQuery(trilogyHeroes.Object, droidRepository.Object, humanRepository.Object, mapper.Object);            

            // Then
            Assert.NotNull(starWarsQuery);
            Assert.True(starWarsQuery.HasField("hero"));
        }
    }
}
