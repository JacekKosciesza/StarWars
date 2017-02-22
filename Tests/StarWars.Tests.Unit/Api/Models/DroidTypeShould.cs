using StarWars.Api.Models;
using Xunit;

namespace StarWars.Tests.Unit.Api.Models
{
    public class DroidTypeShould
    {
        [Fact]
        public void HaveIdAndNameFields()
        {
            // When
            var droidType = new DroidType();

            // Then
            Assert.NotNull(droidType);
            Assert.True(droidType.HasField("id"));
            Assert.True(droidType.HasField("name"));
        }
    }
}
