using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using StarWars.Api.Models;
using System.Threading.Tasks;

namespace StarWars.Api.Controllers
{
    [Route("graphql")]
    public class GraphQLController : Controller
    {
        private IDocumentExecuter _documentExecuter { get; set; }
        private ISchema _schema { get; set; }

        public GraphQLController(IDocumentExecuter documentExecuter, ISchema schema)
        {
            _documentExecuter = documentExecuter;
            _schema = schema;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            var executionOptions = new ExecutionOptions { Schema = _schema, Query = query.Query };
            var result = await _documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }
    }
}
