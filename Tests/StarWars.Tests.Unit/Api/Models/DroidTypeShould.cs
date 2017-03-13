using StarWars.Api.Models;
using Xunit;

namespace StarWars.Tests.Unit.Api.Models
{
    public class DroidTypeShould
    {
        [Fact]
        [Trait("test", "unit")]
        public void HaveIdAndNameFields()
        {
            // When
            var droidType = new DroidType();

            // Then
            Assert.NotNull(droidType);
            Assert.True(droidType.HasField("Id"));
            Assert.True(droidType.HasField("Name"));
        }
    }
}
