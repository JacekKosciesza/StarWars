# Star Wars example with GraphQL for .NET, ASP.NET Core, Entity Framework Core

1. Create 'StarWars' empty solution
![empty-solution](https://cloud.githubusercontent.com/assets/8171434/22863729/0831261c-f146-11e6-9fef-040e20462bfe.png)

2. Add 'ASP.NET Core Web Application (.NET Core)' project named 'StarWars.Api'
![aspnet-core-web-application](https://cloud.githubusercontent.com/assets/8171434/22863870/edcbf2f0-f147-11e6-96eb-6a8cc14c3588.png)

3. Select Web API template
![aspnet-core-web-api](https://cloud.githubusercontent.com/assets/8171434/22863872/eff07d80-f147-11e6-9614-d853da97d1aa.png)

4. Update all NuGet packages
![update-all-nuget-packages](https://cloud.githubusercontent.com/assets/8171434/22864011/93533b2e-f149-11e6-883a-ead44bf1f403.png)

5. Update project.json with correct runtime
```json
"runtimes": {
   "win10-x64": { }
}
```
6. Install GraphQL NuGet package
![install-graphql-nuget-package](https://cloud.githubusercontent.com/assets/8171434/22864254/50c06f70-f14e-11e6-80d2-6c94f3088c8a.png)

7. Create 'StarWars.Core' project
![starwars-core-project](https://cloud.githubusercontent.com/assets/8171434/22866500/e4313996-f177-11e6-90ce-44843e588dff.png)

8. Create 'Droid' model
```csharp
namespace StarWars.Core.Models
{
    public class Droid
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
```

9. Create 'DroidType' model
```csharp
using GraphQL.Types;
using StarWars.Core.Models;

namespace StarWars.Api.Models
{
    public class DroidType : ObjectGraphType<Droid>
    {
        public DroidType()
        {
            Field(x => x.Id).Description("The Id of the Droid.");
            Field(x => x.Name, nullable: true).Description("The name of the Droid.");
        }
    }
}
```
10. Create 'StarWarsQuery' model
```csharp
using GraphQL.Types;
using StarWars.Core.Models;

namespace StarWars.Api.Models
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery()
        {
            Field<DroidType>(
              "hero",
              resolve: context => new Droid { Id = "1", Name = "R2-D2" }
            );
        }
    }
}
```

11. Create 'GraphQLQuery' model
```csharp
namespace StarWars.Api.Models
{
    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public string Variables { get; set; }
    }
}
```

12. Create 'GraphQLController'
```csharp
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
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            var schema = new Schema { Query = new StarWarsQuery() };

            var result = await new DocumentExecuter().ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = query.Query;

            }).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
```

13. Test using Postman
![postman-test-query](https://cloud.githubusercontent.com/assets/8171434/22866705/17985b54-f17b-11e6-848c-6482b45e4934.png)