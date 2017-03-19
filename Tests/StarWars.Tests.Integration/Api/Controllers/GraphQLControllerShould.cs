using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using StarWars.Api;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Xunit;

namespace StarWars.Tests.Integration.Api.Controllers
{
    public class GraphQLControllerShould
    {
        private readonly HttpClient _client;

        public GraphQLControllerShould()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Test")
                .UseStartup<Startup>()
            );
            _client = server.CreateClient();
        }

        // https://github.com/graphql-dotnet/graphql-dotnet#usage

        // {
        //   "hero": {
        //     "id": "2001",
        //     "name": "R2-D2"
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ReturnR2D2Droid()
        {
            // Given
            const string query = @"{
                ""query"": ""query { hero { id name } }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""hero"":{""id"":""2001"",""name"":""R2-D2""}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal(2001, (int)jobj["data"]["hero"]["id"]);
            Assert.Equal("R2-D2", (string)jobj["data"]["hero"]["name"]);
        }

        // https://github.com/facebook/graphql#query-syntax

        // {
        //   "hero": {
        //     "name": "R2-D2"
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteHeroNameQuery()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query HeroNameQuery {
                        hero {
                            name
                        }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""hero"":{""name"":""R2-D2""}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal("R2-D2", (string)jobj["data"]["hero"]["name"]);
        }

        // {
        //   "hero": {
        //     "id": "2001",
        //     "name": "R2-D2",
        //     "friends": [
        //       {
        //         "id": "1000",
        //         "name": "Luke Skywalker"
        //       },
        //       {
        //         "id": "1002",
        //         "name": "Han Solo"
        //       },
        //       {
        //         "id": "1003",
        //         "name": "Leia Organa"
        //       }
        //     ]
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteHeroNameAndFriendsQuery()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query HeroNameAndFriendsQuery {
                        hero {
                            id
                            name
                            friends {
                                id
                                name
                            }
                        }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""hero"":{""id"":""2001"",""name"":""R2-D2"",""friends"":[{""id"":""1000"",""name"":""Luke Skywalker""},{""id"":""1002"",""name"":""Han Solo""},{""id"":""1003"",""name"":""Leia Organa""}]}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal(3, ((JArray)jobj["data"]["hero"]["friends"]).Count);
            Assert.Equal("Luke Skywalker", (string)jobj["data"]["hero"]["friends"][0]["name"]);
            Assert.Equal("Han Solo", (string)jobj["data"]["hero"]["friends"][1]["name"]);
            Assert.Equal("Leia Organa", (string)jobj["data"]["hero"]["friends"][2]["name"]);
        }

        // {
        //   "hero": {
        //     "name": "R2-D2",
        //     "friends": [
        //       {
        //         "name": "Luke Skywalker",
        //         "appearsIn": [ "NEWHOPE", "EMPIRE", "JEDI" ],
        //         "friends": [
        //           { "name": "Han Solo" },
        //           { "name": "Leia Organa" },
        //           { "name": "C-3PO" },
        //           { "name": "R2-D2" }
        //         ]
        //       },
        //       {
        //         "name": "Han Solo",
        //         "appearsIn": [ "NEWHOPE", "EMPIRE", "JEDI" ],
        //         "friends": [
        //           { "name": "Luke Skywalker" },
        //           { "name": "Leia Organa" },
        //           { "name": "R2-D2" }
        //         ]
        //       },
        //       {
        //         "name": "Leia Organa",
        //         "appearsIn": [ "NEWHOPE", "EMPIRE", "JEDI" ],
        //         "friends": [
        //           { "name": "Luke Skywalker" },
        //           { "name": "Han Solo" },
        //           { "name": "C-3PO" },
        //           { "name": "R2-D2" }
        //         ]
        //       }
        //     ]
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteNestedQuery()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query NestedQuery {
                        hero {
                            name
                            friends {
                                name
                                appearsIn
                                friends {
                                    name
                                }
                            }
                        }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""hero"":{""name"":""R2-D2"",""friends"":[{""name"":""Luke Skywalker"",""appearsIn"":[""NEWHOPE"",""EMPIRE"",""JEDI""],""friends"":[{""name"":""Han Solo""},{""name"":""Leia Organa""},{""name"":""C-3PO""},{""name"":""R2-D2""}]},{""name"":""Han Solo"",""appearsIn"":[""NEWHOPE"",""EMPIRE"",""JEDI""],""friends"":[{""name"":""Luke Skywalker""},{""name"":""Leia Organa""},{""name"":""R2-D2""}]},{""name"":""Leia Organa"",""appearsIn"":[""NEWHOPE"",""EMPIRE"",""JEDI""],""friends"":[{""name"":""Luke Skywalker""},{""name"":""Han Solo""},{""name"":""C-3PO""},{""name"":""R2-D2""}]}]}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            var luke = jobj["data"]["hero"]["friends"][0];
            var episodes = ((JArray) luke["appearsIn"]).Select(e => (string)e).ToArray();
            Assert.Equal(new[] { "NEWHOPE", "EMPIRE", "JEDI" }, episodes);
            Assert.Equal(4, ((JArray)luke["friends"]).Count);
            Assert.Equal("Han Solo", (string)luke["friends"][0]["name"]);
            Assert.Equal("Leia Organa", (string)luke["friends"][1]["name"]);
            Assert.Equal("C-3PO", (string)luke["friends"][2]["name"]);
            Assert.Equal("R2-D2", (string)luke["friends"][3]["name"]);
        }

        // {
        //   "human": {
        //     "name": "Luke Skywalker"
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteFetchLukeQuery()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query FetchLukeQuery {
                        human(id: \""1000\"") {
                            name
                        }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""human"":{""name"":""Luke Skywalker""}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal("Luke Skywalker", (string)jobj["data"]["human"]["name"]);
        }

        // {
        //   "luke": {
        //     "name": "Luke Skywalker"
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteFetchLukeAliased()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query FetchLukeAliased {
                        luke: human(id: \""1000\"") {
                            name
                        }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""luke"":{""name"":""Luke Skywalker""}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal("Luke Skywalker", (string)jobj["data"]["luke"]["name"]);
        }

        // {
        //   "luke": {
        //     "name": "Luke Skywalker"
        //   },
        //   "leia": {
        //     "name": "Leia Organa"
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteFetchLukeAndLeiaAliased()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query FetchLukeAliased {
                        luke: human(id: \""1000\"") {
                            name
                        }
                        leia: human(id: \""1003\"") {
                            name
                        }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""luke"":{""name"":""Luke Skywalker""},""leia"":{""name"":""Leia Organa""}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal("Luke Skywalker", (string)jobj["data"]["luke"]["name"]);
            Assert.Equal("Leia Organa", (string)jobj["data"]["leia"]["name"]);
        }

        // {
        //   "luke": {
        //     "name": "Luke Skywalker",
        //     "homePlanet": "Tatooine"
        //   },
        //   "leia": {
        //     "name": "Leia Organa",
        //     "homePlanet": "Alderaan"
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteDuplicateFields()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query DuplicateFields {
                        luke: human(id: \""1000\"") {
                            name
                            homePlanet
                        }
                        leia: human(id: \""1003\"") {
                            name
                            homePlanet
                        }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""luke"":{""name"":""Luke Skywalker"",""homePlanet"":""Tatooine""},""leia"":{""name"":""Leia Organa"",""homePlanet"":""Alderaan""}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal("Luke Skywalker", (string)jobj["data"]["luke"]["name"]);
            Assert.Equal("Tatooine", (string)jobj["data"]["luke"]["homePlanet"]);
            Assert.Equal("Leia Organa", (string)jobj["data"]["leia"]["name"]);
            Assert.Equal("Alderaan", (string)jobj["data"]["leia"]["homePlanet"]);
        }

        // {
        //   "luke": {
        //     "name": "Luke Skywalker",
        //     "homePlanet": "Tatooine"
        //   },
        //   "leia": {
        //     "name": "Leia Organa",
        //     "homePlanet": "Alderaan"
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteUseFragment()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query UseFragment {
                        luke: human(id: \""1000\"") {
                            ...HumanFragment
                        }
                        leia: human(id: \""1003\"") {
                            ...HumanFragment
                        }
                    }

                    fragment HumanFragment on Human {
                        name
                        homePlanet
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""luke"":{""name"":""Luke Skywalker"",""homePlanet"":""Tatooine""},""leia"":{""name"":""Leia Organa"",""homePlanet"":""Alderaan""}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal("Luke Skywalker", (string)jobj["data"]["luke"]["name"]);
            Assert.Equal("Tatooine", (string)jobj["data"]["luke"]["homePlanet"]);
            Assert.Equal("Leia Organa", (string)jobj["data"]["leia"]["name"]);
            Assert.Equal("Alderaan", (string)jobj["data"]["leia"]["homePlanet"]);
        }

        // {
        //   "hero": {
        //     "__typename": "Droid",
        //     "name": "R2-D2"
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteCheckTypeOfR2()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query CheckTypeOfR2 {
                        hero {
                            __typename
                            name
                        }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""hero"":{""__typename"":""Droid"",""name"":""R2-D2""}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal("Droid", (string)jobj["data"]["hero"]["__typename"]);
            Assert.Equal("R2-D2", (string)jobj["data"]["hero"]["name"]);
        }

        // {
        //   "hero": {
        //     "__typename": "Human",
        //     "name": "Luke Skywalker"
        //   }
        // }
        [Fact]
        [Trait("test", "integration")]
        public async void ExecuteCheckTypeOfLuke()
        {
            // Given
            const string query = @"{
                ""query"":
                    ""query CheckTypeOfLuke {
                       hero(episode: EMPIRE) {
                            __typename
                            name
                       }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            //const string responseString = @"{""data"":{""hero"":{""__typename"":""Human"",""name"":""Luke Skywalker""}}}";
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal("Human", (string)jobj["data"]["hero"]["__typename"]);
            Assert.Equal("Luke Skywalker", (string)jobj["data"]["hero"]["name"]);
        }
    }
}
