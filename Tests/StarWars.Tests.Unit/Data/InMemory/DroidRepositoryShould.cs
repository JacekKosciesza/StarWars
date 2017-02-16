using StarWars.Data.InMemory;
using Xunit;

namespace StarWars.Tests.Unit.Data.InMemory
{
    public class DroidRepositoryShould
    {
        private readonly DroidRepository _droidRepository;
        public DroidRepositoryShould()
        {
            // Given
            _droidRepository = new DroidRepository();
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
