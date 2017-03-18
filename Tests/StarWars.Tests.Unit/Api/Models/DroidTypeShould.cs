using AutoMapper;
using Moq;
using StarWars.Api.Models;
using StarWars.Core.Data;
using Xunit;

namespace StarWars.Tests.Unit.Api.Models
{
    public class DroidTypeShould
    {
        [Fact]
        [Trait("test", "unit")]
        public void HaveIdAndNameFields()
        {
            // Given
            var characterRepository = new Mock<ICharacterRepository>();
            var mapper = new Mock<IMapper>();

            // When
            var droidType = new DroidType(characterRepository.Object, mapper.Object);

            // Then
            Assert.NotNull(droidType);
            Assert.True(droidType.HasField("Id"));
            Assert.True(droidType.HasField("Name"));
        }
    }
}
