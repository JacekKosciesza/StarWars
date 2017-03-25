using Microsoft.EntityFrameworkCore;
using StarWars.Data.EntityFramework;
using System.Linq;
using Xunit;

namespace StarWars.Tests.Integration.Data.EntityFramework
{
    public class StarWarsContextShould
    {
        //[Fact]
        //[Fact(Skip = "It uses real database")]
        [Trait("test", "integration")]
        public async void ReturnR2D2Droid()
        {
            // Given
            using (var db = new StarWarsContext())
            {
                // When
                var r2d2 = await db.Droids
                    .Include("CharacterEpisodes.Episode")
                    .Include("CharacterFriends.Friend")
                    .FirstOrDefaultAsync(d => d.Id == 2001);

                // Then
                Assert.NotNull(r2d2);
                Assert.Equal("R2-D2", r2d2.Name);
                Assert.Equal("Astromech", r2d2.PrimaryFunction);
                var episodes = r2d2.CharacterEpisodes.Select(e => e.Episode.Title);
                Assert.Equal(new[] { "NEWHOPE", "EMPIRE", "JEDI" }, episodes);
                var friends = r2d2.CharacterFriends.Select(e => e.Friend.Name);
                Assert.Equal(new[] { "Luke Skywalker", "Han Solo", "Leia Organa" }, friends);
            }
        }
    }
}
