using StarWars.Api.Models;
using Xunit;

namespace StarWars.Tests.Unit.Api.Models
{
    public class GraphQLQueryShould
    {
        [Fact]
        [Trait("test", "unit")]
        public void ReturnFormattedText()
        {
            // Given
            var query = new GraphQLQuery
            {
                OperationName = "OperationName",
                NamedQuery = "NamedQuery",
                Query = "Query",
                Variables = "Variables"
            };

            // When
            var text = query.ToString();

            // Then
            Assert.NotEmpty(text);
            Assert.Equal("\r\nOperationName = OperationName\r\nNamedQuery = NamedQuery\r\nQuery = Query\r\nVariables = Variables\r\n", text);
        }
    }
}
