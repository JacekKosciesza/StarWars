# Star Wars example with GraphQL for .NET, ASP.NET Core, Entity Framework Core

## Examples
* Basic - simple 'Hello GraphQL!' example based on console version from [GraphQL for .NET on GitHub](https://github.com/graphql-dotnet/graphql-dotnet),
but using ASP.NET Core, Entity Framework Core and some best practices, patterns and principles like:
    * DDD (Domain Driven Design) style hexagonal architecture    
    * SOLID principle (especially Single Responsibility and Dependency Inversion)
    * Repository pattern

## Tutorials

### Basic


* Create 'StarWars' empty solution
![empty-solution](https://cloud.githubusercontent.com/assets/8171434/22863729/0831261c-f146-11e6-9fef-040e20462bfe.png)

* Add 'ASP.NET Core Web Application (.NET Core)' project named 'StarWars.Api'
![aspnet-core-web-application](https://cloud.githubusercontent.com/assets/8171434/22863870/edcbf2f0-f147-11e6-96eb-6a8cc14c3588.png)

* Select Web API template
![aspnet-core-web-api](https://cloud.githubusercontent.com/assets/8171434/22863872/eff07d80-f147-11e6-9614-d853da97d1aa.png)

* Update all NuGet packages
![update-all-nuget-packages](https://cloud.githubusercontent.com/assets/8171434/22864011/93533b2e-f149-11e6-883a-ead44bf1f403.png)

* Update project.json with correct runtime
```json
"runtimes": {
   "win10-x64": { }
}
```

* Install GraphQL NuGet package
![install-graphql-nuget-package](https://cloud.githubusercontent.com/assets/8171434/22864254/50c06f70-f14e-11e6-80d2-6c94f3088c8a.png)

* Create 'StarWars.Core' project
![starwars-core-project](https://cloud.githubusercontent.com/assets/8171434/22866500/e4313996-f177-11e6-90ce-44843e588dff.png)

* Create 'Droid' model
```csharp
namespace StarWars.Core.Models
{
    public class Droid
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
```

* Create 'DroidType' model
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

* Create 'StarWarsQuery' model
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
              resolve: context => new Droid { Id = 1, Name = "R2-D2" }
            );
        }
    }
}
```

* Create 'GraphQLQuery' model
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

* Create 'GraphQLController'
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

* Test using Postman
![postman-test-query](https://cloud.githubusercontent.com/assets/8171434/22866705/17985b54-f17b-11e6-848c-6482b45e4934.png)

* Create 'IDroidRepository' interface
```csharp
using StarWars.Core.Models;
using System.Threading.Tasks;

namespace StarWars.Core.Data
{
    public interface IDroidRepository
    {
        Task<Droid> Get(int id);
    }
}
```
* Create 'StarWars.Data' project
![starwars-data-project](https://cloud.githubusercontent.com/assets/8171434/22888090/dd357674-f204-11e6-8613-e2cac5087180.png)

* Create in memory 'DroidRepository'
```csharp
using StarWars.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarWars.Core.Models;
using System.Linq;

namespace StarWars.Data.InMemory
{
    public class DroidRepository : IDroidRepository
    {
        private List<Droid> _droids = new List<Droid> {
            new Droid { Id = 1, Name = "R2-D2" }
        };

        public Task<Droid> Get(int id)
        {
            return Task.FromResult(_droids.FirstOrDefault(droid => droid.Id == id));
        }
    }
}
```

* Use 'IDroidRepository' in StarWarsQuery
```csharp
using GraphQL.Types;
using StarWars.Core.Data;

namespace StarWars.Api.Models
{
    public class StarWarsQuery : ObjectGraphType
    {
        private IDroidRepository _droidRepository { get; set; }

        public StarWarsQuery(IDroidRepository _droidRepository)
        {
            Field<DroidType>(
              "hero",
              resolve: context => _droidRepository.Get(1)
            );
        }
    }
}
```

* Update creation of StarWarsQuery in GraphQLController
```csharp
// ...
public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
{
    var schema = new Schema { Query = new StarWarsQuery(new DroidRepository()) };
// ...
```

* Test using Postman

* Configure dependency injection in Startup.cs
```csharp
// ...
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    services.AddTransient<StarWarsQuery>();
    services.AddTransient<IDroidRepository, DroidRepository>();
}
// ...
```


* Use constructor injection of StarWarsQuery in GraphQLController
```csharp
// ...
public class GraphQLController : Controller
{
    private StarWarsQuery _starWarsQuery { get; set; }

    public GraphQLController(StarWarsQuery starWarsQuery)
    {
        _starWarsQuery = starWarsQuery;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
    {
        var schema = new Schema { Query = _starWarsQuery };
// ...
```

* Add Microsoft.EntityFrameworkCore and Microsoft.EntityFrameworkCore.SqlServer Nuget packages to StarWars.Data project
![entity-framework-core-nuget](https://cloud.githubusercontent.com/assets/8171434/22892209/0ff54e92-f212-11e6-9c95-e55f103b6c79.png)
![entity-framework-sql-server-provider-nuget](https://cloud.githubusercontent.com/assets/8171434/22892268/39ac8b24-f212-11e6-805b-8462ce9a5c02.png)

* Create StarWarsContext
```csharp
using Microsoft.EntityFrameworkCore;
using StarWars.Core.Models;

namespace StarWars.Data.EntityFramework
{
    public class StarWarsContext : DbContext
    {
        public StarWarsContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Droid> Droids { get; set; }
    }
}
```

* Update 'appsetting.json' with database connection
```json
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "ConnectionStrings": {
    "StarWarsDatabaseConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=StarWars;Integrated Security=SSPI;integrated security=true;MultipleActiveResultSets=True;"
  }
}
```

* Create EF droid repository
```csharp
using StarWars.Core.Data;
using System.Threading.Tasks;
using StarWars.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace StarWars.Data.EntityFramework.Repositories
{
    public class DroidRepository : IDroidRepository
    {
        private StarWarsContext _db { get; set; }

        public DroidRepository(StarWarsContext db)
        {
            _db = db;
        }

        public Task<Droid> Get(int id)
        {
            return _db.Droids.FirstOrDefaultAsync(droid => droid.Id == id);
        }
    }
}
```

* Create seed data as an extension to StarWarsContext
```csharp
using StarWars.Core.Models;
using System.Linq;

namespace StarWars.Data.EntityFramework.Seed
{
    public static class StarWarsSeedData
    {
        public static void EnsureSeedData(this StarWarsContext db)
        {
            if (!db.Droids.Any())
            {
                var droid = new Droid
                {
                    Name = "R2-D2"
                };
                db.Droids.Add(droid);
                db.SaveChanges();
            }
        }
    }
}
```

* Configure dependency injection and run data seed in Startup.cs
```csharp
// ...
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    services.AddTransient<StarWarsQuery>();
    services.AddTransient<IDroidRepository, DroidRepository>();
    services.AddDbContext<StarWarsContext>(options => 
        options.UseSqlServer(Configuration["ConnectionStrings:StarWarsDatabaseConnection"])
    );
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env,
                        ILoggerFactory loggerFactory, StarWarsContext db)
{
    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
    loggerFactory.AddDebug();

    app.UseMvc();

    db.EnsureSeedData();
}
// ...
```
* Run application and make sure database is created
![ssms-starwars-database-created](https://cloud.githubusercontent.com/assets/8171434/22923521/1fb046da-f2a2-11e6-8e56-c00661e27318.png)

* Final test using Postman
![postman-test-query](https://cloud.githubusercontent.com/assets/8171434/22866705/17985b54-f17b-11e6-848c-6482b45e4934.png)