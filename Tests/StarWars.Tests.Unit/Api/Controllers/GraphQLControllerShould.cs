using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StarWars.Api.Controllers;
using StarWars.Api.Models;
using System.Threading.Tasks;
using Xunit;

namespace StarWars.Tests.Unit.Api.Controllers
{
    public class GraphQLControllerShould
    {
        private GraphQLController _graphqlController { get; set; }

        ExecutionOptions ddd()
        {
            return new ExecutionOptions();
        }

        public GraphQLControllerShould()
        {
            // Given
            var documentExecutor = new Mock<IDocumentExecuter>();
            documentExecutor.Setup(x => x.ExecuteAsync(It.IsAny<ExecutionOptions>())).Returns(Task.FromResult(new ExecutionResult()));
            var schema = new Mock<ISchema>();
            _graphqlController = new GraphQLController(documentExecutor.Object, schema.Object);
        }

        [Fact]
        public void ReturnNotNullViewResult()
        {
            // When
            var result = _graphqlController.Index() as ViewResult;

            // Then
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void ReturnNotNullExecutionResult()
        {
            // Given
            var query = new GraphQLQuery { Query = @"{ ""query"": ""query { hero { id name } }""" };

            // When
            var result = await _graphqlController.Post(query);

            // Then
            Assert.NotNull(result);
            var okObjectResult =  Assert.IsType<OkObjectResult>(result);
            var executionResult = okObjectResult.Value;
            Assert.NotNull(executionResult);
        }
    }
}
