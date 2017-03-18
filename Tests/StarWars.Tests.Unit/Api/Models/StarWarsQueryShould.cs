using AutoMapper;
using Moq;
using StarWars.Api.Models;
using StarWars.Core.Data;
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
            var humanRepository = new Mock<IHumanRepository>();
            var mapper = new Mock<IMapper>();

            // When
            var starWarsQuery = new StarWarsQuery(droidRepository.Object, humanRepository.Object, mapper.Object);            

            // Then
            Assert.NotNull(starWarsQuery);
            Assert.True(starWarsQuery.HasField("hero"));
        }
    }
}
