using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using StarWars.Api;
using System.Net.Http;
using Xunit;

namespace StarWars.Tests.Integration.Api.Controllers
{
    public class GraphQLControllerShould
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public GraphQLControllerShould()
        {
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async void ReturnNotNullViewResult()
        {
            // When
            var response = await _client.GetAsync("/graphql");
            response.EnsureSuccessStatusCode();

            // Then
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
        }
    }
}
