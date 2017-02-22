# GraphQL 'Star Wars' example using GraphQL for .NET, ASP.NET Core, Entity Framework Core

## Examples
* Basic - simple 'Hello GraphQL!' example based on console version from [GraphQL for .NET on GitHub](https://github.com/graphql-dotnet/graphql-dotnet),
but using ASP.NET Core, Entity Framework Core and some best practices, patterns and principles.

## Roadmap
- [x] Basic
    - [x] Simple tutorial (step/screenshot/code)
    - [ ] Detailed tutorial (steps explanation)
    - [x] 3-Layers (Api, Core, Data) architecture
    - [x] DDD (Domain Driven Design) hexagonal architecture
    - [x] Dependency Inversion (deafult ASP.NET Core IoC container)
    - [x] GraphQL controller
    - [x] In Memory 'Droid' Repository
    - [x] Entity Framework 'Droid' Repository
    - [x] Automatic database creation
    - [x] Seed database data
    - [x] EF Migrations
    - [x] Graph*i*QL
    - [x] Unit Tests
    - [x] Visual Studio 2017 RC upgrade
    - [x] Integration Tests
    - [x] Logs
    - [x] Code Coverage
    - [ ] Continous Integration
    

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

#### Entity Framework Migrations

* Add 'Microsoft.EntityFrameworkCore.Design' NuGet package to 'StarWars.Data' project
![ef-design-nuget](https://cloud.githubusercontent.com/assets/8171434/22964859/fde4e42a-f35a-11e6-89ad-1b5dfeda4e67.png)

* Add 'Microsoft.EntityFrameworkCore.Tools.DotNet' NuGet package to 'StarWars.Data' project
![ef-tools-dotnet-nuget](https://cloud.githubusercontent.com/assets/8171434/22964861/ff9afdf4-f35a-11e6-8e42-87ed30009877.png)

* Add tools section in project.json (StarWars.Data)
```json
"tools": {
    "Microsoft.EntityFrameworkCore.Tools.DotNet": "1.1.0-preview4-final"
}
```
* Add official workaround for problems with targeting class library (Modify your class library to be a startup application)
    * Add main entry point
    ```csharp
    namespace StarWars.Data.EntityFramework.Workaround
    {
        // WORKAROUND: https://docs.efproject.net/en/latest/miscellaneous/cli/dotnet.html#targeting-class-library-projects-is-not-supported
        public static class Program
        {
            public static void Main() { }
        }
    }
    ```
    * Add build option in project.json
    ```json
    "buildOptions": {
        "emitEntryPoint": true
    }
    ```
* Run migrations command from the console
```
dotnet ef migrations add Inital -o .\EntityFramework\Migrations
```
![dotnet-ef-migrations](https://cloud.githubusercontent.com/assets/8171434/22965430/d8d02eda-f35d-11e6-8425-6edeaeaa88c9.png)

#### Grahp*i*QL

* Add NPM configuration file 'package.json' to StarWars.Api project
![npm-configuration-file](https://cloud.githubusercontent.com/assets/8171434/22973053/b6e4c0a0-f37c-11e6-898b-938c50bd2f83.png)

* Add GraphiQL dependencies and webpack bundle task
```json
{
  "version": "1.0.0",
  "name": "starwars-graphiql",
  "private": true,
  "scripts": {
    "start": "webpack --progress"
  },
  "dependencies": {
    "graphiql": "^0.7.8",
    "graphql": "^0.7.0",
    "isomorphic-fetch": "^2.1.1",
    "react": "^15.3.1",
    "react-dom": "^15.3.1"
  },
  "devDependencies": {
    "babel": "^5.6.14",
    "babel-loader": "^5.3.2",
    "css-loader": "^0.24.0",
    "extract-text-webpack-plugin": "^1.0.1",
    "postcss-loader": "^0.10.1",
    "style-loader": "^0.13.1",
    "webpack": "^1.13.0"
  }
}
```

* Add webpack configuration 'webpack.config.js'
```javascript
var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');

var output = './wwwroot';

module.exports = {
    entry: {
        'bundle': './Scripts/app.js'
    },

    output: {
        path: output,
        filename: '[name].js'
    },

    resolve: {
        extensions: ['', '.js', '.json']
    },

    module: {
        loaders: [
          { test: /\.js/, loader: 'babel', exclude: /node_modules/ },
          { test: /\.css$/, loader: ExtractTextPlugin.extract('style-loader', 'css-loader!postcss-loader') }
        ]
    },

    plugins: [
      new ExtractTextPlugin('style.css', { allChunks: true })
    ]
};
```

* Install 'NPM Task Runner' extension
![npm-task-runner](https://cloud.githubusercontent.com/assets/8171434/22973925/2a6cd7ee-f380-11e6-89b8-b27fe68a6d1b.png)

* Configure 'After Build' step in 'Task Runner Explorer'
![after-build-task-runner-explorer](https://cloud.githubusercontent.com/assets/8171434/22974769/25727768-f384-11e6-96ca-8670b67b805c.png)
```json
  "-vs-binding": { "AfterBuild": [ "start" ] }
```

* Add 'Get' action to GraphQL controller and GraphiQL view (~/Views/GraphQL/index.cshtml)
```csharp
// ...
public class GraphQLController : Controller
{
    // ...
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }    
// ...
}
```
```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width" />
    <title>GraphiQL</title>
    <link rel="stylesheet" href="~/style.css" />
</head>
<body>
    <div id="app"></div>
    <script src="~/bundle.js" type="text/javascript"></script>
</body>
</html>
```

* Add GraphiQL scripts and styles (app.js and app.css to ~/GraphiQL)
    * app.js
    ```javascript
    import React from 'react';
    import ReactDOM from 'react-dom';
    import GraphiQL from 'graphiql';
    import fetch from 'isomorphic-fetch';
    import 'graphiql/graphiql.css';
    import './app.css';

    function graphQLFetcher(graphQLParams) {
        return fetch(window.location.origin + '/graphql', {
            method: 'post',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(graphQLParams)
        }).then(response => response.json());
    }

    ReactDOM.render(<GraphiQL fetcher={graphQLFetcher}/>, document.getElementById('app'));
    ```
    * app.css
    ```css
    html, body {
        height: 100%;
        margin: 0;
        overflow: hidden;
        width: 100%;
    }

    #app {
        height: 100vh;
    }
    ```

* Add static files support
    * Add 'Microsoft.AspNetCore.StaticFiles' NuGet
    ![static-files-nuget](https://cloud.githubusercontent.com/assets/8171434/22978442/fa3872d2-f392-11e6-8839-634f380fee51.png)

    * Update configuration in 'Startup.cs'
    ```csharp
    // ...
    public void Configure(IApplicationBuilder app, IHostingEnvironment env,
                              ILoggerFactory loggerFactory, StarWarsContext db)
    {
        loggerFactory.AddConsole(Configuration.GetSection("Logging"));
        loggerFactory.AddDebug();

        app.UseStaticFiles();
        app.UseMvc();

        db.EnsureSeedData();
    }
    // ...
    ```
* Build project and check if bundles were created by webpack under ~/wwwroot
![bundles-created-by-webpack](https://cloud.githubusercontent.com/assets/8171434/22978874/b01d6372-f394-11e6-80eb-8b3f8fbe7ad0.png)

* Run project and enjoy Graph*i*QL
![graphiql](https://cloud.githubusercontent.com/assets/8171434/22978970/fe8700fe-f394-11e6-9821-f1d0606a2b9b.png)

#### Unit Tests

* Create 'Class Library (.NET Core)' type 'StarWars.Tests.Unit' project
![unit-tests-project](https://cloud.githubusercontent.com/assets/8171434/22991495/ae9ead08-f3bc-11e6-83e0-17ee0015823f.png)

* Install 'xunit' NuGet package in StarWars.Tests.Unit project
![xunit-nuget](https://cloud.githubusercontent.com/assets/8171434/23019197/6236a94a-f441-11e6-86fd-2d2195366d2a.png)

* Install 'dotnet-test-xunit' NuGet package in StarWars.Tests.Unit project
![dotnet-test-xunit-nuget](https://cloud.githubusercontent.com/assets/8171434/23020067/94a2a3da-f445-11e6-8674-0b33847435c1.png)

* Make changes to project.json
    * Set 'testRunner'
    * Reference 'StarWars.Data' project
    * Set 'runtimes'
```json
{
  "version": "1.0.0-*",
  "testRunner": "xunit",
  "dependencies": {
    "dotnet-test-xunit": "2.2.0-preview2-build1029",
    "Microsoft.NETCore.App": "1.1.0",
    "xunit": "2.1.0",
    "StarWars.Data": {
      "target": "project"
    }
  },

  "frameworks": {
    "netcoreapp1.1": {
      "imports": [
        "dotnet5.6",
        "portable-net45+win8"
      ]
    }
  },

  "runtimes": {
    "win10-x64": {}
  }
}
```

* Create first test for in memory droid repository
```csharp
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
            Assert.Equal("WRONG_NAME", droid.Name);
        }
    }
}
```

* Build and make sure that test is discovered by 'Test Explorer'
![test-explorer-first-unit-test](https://cloud.githubusercontent.com/assets/8171434/23020737/c1b268ee-f448-11e6-96de-80928174a335.png)

* Run test - it should fail (we want to make sure that we are testing the right thing)
![first-unit-test-fail](https://cloud.githubusercontent.com/assets/8171434/23022083/56733e6c-f44f-11e6-8f64-84e9da06d879.png)

* Fix test
```csharp
// ...
[Fact]
public async void ReturnR2D2DroidGivenIdOf1()
{
    // When
    var droid = await _droidRepository.Get(1);

    // Then
    Assert.NotNull(droid);
    Assert.Equal("R2-D2", droid.Name);
}
// ...
```
* Run test again - it should pass
![first-unit-test-pass](https://cloud.githubusercontent.com/assets/8171434/23022004/ff769a14-f44e-11e6-90d8-12d33eaac801.png)

* Install 'Moq' NuGet package
![moq-nuget](https://cloud.githubusercontent.com/assets/8171434/23029811/03d08832-f46c-11e6-8b63-1d458e16c37d.png)

* Install 'Microsoft.EntityFrameworkCore.InMemory' NuGet package
![ef-in-memory-nuget](https://cloud.githubusercontent.com/assets/8171434/23035364/1c0eb1f4-f47f-11e6-852d-765a4e2d4a0e.png)

* Add reference to 'StarWars.Core' in project.json
```json
{
  "dependencies": {
    "StarWars.Core": {
      "target": "project"
    }
  }
}
```

* Create EF droid repository unit test
```csharp
using Microsoft.EntityFrameworkCore;
using StarWars.Core.Models;
using StarWars.Data.EntityFramework;
using StarWars.Data.EntityFramework.Repositories;
using Xunit;

namespace StarWars.Tests.Unit.Data.EntityFramework.Repositories
{
    public class DroidRepositoryShould
    {
        private readonly DroidRepository _droidRepository;
        public DroidRepositoryShould()
        {
            // Given
            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory
            var options = new DbContextOptionsBuilder<StarWarsContext>()
                .UseInMemoryDatabase(databaseName: "StarWars")
                .Options;
            using (var context = new StarWarsContext(options))
            {
                context.Droids.Add(new Droid { Id = 1, Name = "R2-D2" });
                context.SaveChanges();
            }
            var starWarsContext = new StarWarsContext(options);
            _droidRepository = new DroidRepository(starWarsContext);
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
```

* Create GraphQLController unit test
    * First refactor controller to be more testable by using constructor injection
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
    ```
    * Configure dependency injection in 'Startup.cs'
    ```csharp
    // ...
    public void ConfigureServices(IServiceCollection services)
    {
        // ...        
        services.AddTransient<IDocumentExecuter, DocumentExecuter>();
        var sp = services.BuildServiceProvider();
        services.AddTransient<ISchema>(_ => new Schema { Query = sp.GetService<StarWarsQuery>() });
    }
    // ...
    ```
    * Create test for 'Index' and 'Post' actions
    ```csharp
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
    ```

#### Visual Studio 2017 RC upgrade

* Open solution in VS 2017 and let the upgrade tool do the job
![vs-2017-rc-upgrade](https://cloud.githubusercontent.com/assets/8171434/23065886/0d9aae72-f518-11e6-8f60-f9c226c6b49a.png)

* Upgrade of 'StarWars.Tests.Unit' failed, so I had to remove all project dependencies and reload it
```json
{
  "dependencies": {
    // remove this:
    "StarWars.Data": {
      "target": "project"
    },
    "StarWars.Core": {
      "target": "project"
    }
    // ...
  }
}
```

* Replace old test txplorer runner for the xUnit.net framework (dotnet-test-xunit) with new one (xunit.runner.visualstudio)
![xunit-runner-visualstudio-nuget](https://cloud.githubusercontent.com/assets/8171434/23066060/e808053c-f518-11e6-8426-5398565ff8a1.png)

* Install (xunit.runner.visualstudio) dependency (Microsoft.DotNet.InternalAbstractions)
![microsoft-dotnet-internal-abstractions-nuget](https://cloud.githubusercontent.com/assets/8171434/23066097/24ff49f0-f519-11e6-9a1c-8f6645bf66a4.png)

#### Integration Tests

* Create 'xUnit Test Project (.NET Core)' type 'StarWars.Tests.Integration' project
![integration-tests-project](https://cloud.githubusercontent.com/assets/8171434/23093375/6b3f9fe6-f5e1-11e6-8d4c-d89f16ef3204.png)

* Change target framework from 'netcoreapp1.0' to 'netcoreapp1.1'
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>    
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <!--...-->
</Project>
```

* Install 'Microsoft.AspNetCore.TestHost' NuGet package
![test-host-nuget](https://cloud.githubusercontent.com/assets/8171434/23100598/a5576138-f685-11e6-9ba1-aa04e9a31917.png)

* Use EF in memory database for 'Test' evironment
    * Install 'Microsoft.EntityFrameworkCore.InMemory' NuGet package
    ![ef-in-memory-nuget-api-project](https://cloud.githubusercontent.com/assets/8171434/23127639/14204be2-f77c-11e6-81de-336d88cb5db7.png)

    * Configure it in 'Startup.cs'
    ```csharp
    // ...
    private IHostingEnvironment Env { get; set; }

    public class Startup
    {
        // ...
        Env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        if (Env.IsEnvironment("Test"))
        {
            services.AddDbContext<StarWarsContext>(options =>
                options.UseInMemoryDatabase(databaseName: "StarWars"));
        }
        else
        {
            services.AddDbContext<StarWarsContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:StarWarsDatabaseConnection"]));
        }
        // ...
    }
    // ...
    ```
* Create integration test for GraphQL query (POST)
```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using StarWars.Api;
using System.Net.Http;
using System.Text;
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
                .UseEnvironment("Test")
                .UseStartup<Startup>()
            );
            _client = _server.CreateClient();
        }

        [Fact]
        public async void ReturnR2D2Droid()
        {
            // Given
            var query = @"{
                ""query"": ""query { hero { id name } }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("R2-D2", responseString);
        }
    }
}
```
#### Logs

* Make sure that logger is configured in Startup.cs
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env,
                              ILoggerFactory loggerFactory, StarWarsContext db)
{
    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
    loggerFactory.AddDebug();
    // ...
}
```

* Override ToString method of GraphQLQuery class
```csharp
public override string ToString()
{
    var builder = new StringBuilder();
    builder.AppendLine();
    if (!string.IsNullOrWhiteSpace(OperationName))
    {
        builder.AppendLine($"OperationName = {OperationName}");
    }
    if (!string.IsNullOrWhiteSpace(NamedQuery))
    {
        builder.AppendLine($"NamedQuery = {NamedQuery}");
    }
    if (!string.IsNullOrWhiteSpace(Query))
    {
        builder.AppendLine($"Query = {Query}");
    }
    if (!string.IsNullOrWhiteSpace(Variables))
    {
        builder.AppendLine($"Variables = {Variables}");
    }

    return builder.ToString();
}
```
* Add logger to GraphQLController
```csharp
public class GraphQLController : Controller
{
    // ...
    private readonly ILogger _logger;

    public GraphQLController(IDocumentExecuter documentExecuter, ISchema schema, ILogger<GraphQLController> logger)
    {
        // ...
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("Got request for GraphiQL. Sending GUI back");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
    {
        // ...
        if (result.Errors?.Count > 0)
        {
            _logger.LogError("GraphQL errors: {0}", result.Errors);
            return BadRequest(result);
        }

        _logger.LogDebug("GraphQL execution result: {result}", JsonConvert.SerializeObject(result.Data));
        return Ok(result);
    }
}
```

* Add logger to DroidRepository
```csharp
namespace StarWars.Data.EntityFramework.Repositories
{
    public class DroidRepository : IDroidRepository
    {
        private StarWarsContext _db { get; set; }
        private readonly ILogger _logger;

        public DroidRepository(StarWarsContext db, ILogger<DroidRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public Task<Droid> Get(int id)
        {
            _logger.LogInformation("Get droid with id = {id}", id);
            return _db.Droids.FirstOrDefaultAsync(droid => droid.Id == id);
        }
    }
}
```

* Add logger to StarWarsContext
```csharp
namespace StarWars.Data.EntityFramework
{
    public class StarWarsContext : DbContext
    {
        public readonly ILogger _logger;

        public StarWarsContext(DbContextOptions options, ILogger<StarWarsContext> logger)
            : base(options)
        {
            _logger = logger;
            // ...
        }
    }
}
```

* Add logger to StarWarsSeedData
```csharp
namespace StarWars.Data.EntityFramework.Seed
{
    public static class StarWarsSeedData
    {
        public static void EnsureSeedData(this StarWarsContext db)
        {
            db._logger.LogInformation("Seeding database");
            if (!db.Droids.Any())
            {
                db._logger.LogInformation("Seeding droids");
                // ...
            }
        }
    }
}
```

* Fix controller unit test
```csharp
public class GraphQLControllerShould
{
    public GraphQLControllerShould()
    {
        // ...
        var logger = new Mock<ILogger<GraphQLController>>();
        _graphqlController = new GraphQLController(documentExecutor.Object, schema.Object, logger.Object);
    }
    // ...
}
```

* Fix repository unit test
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StarWars.Core.Models;
using StarWars.Data.EntityFramework;
using StarWars.Data.EntityFramework.Repositories;
using Xunit;

namespace StarWars.Tests.Unit.Data.EntityFramework.Repositories
{
    public class DroidRepositoryShould
    {
        public DroidRepositoryShould()
        {
            var dbLogger = new Mock<ILogger<StarWarsContext>>();
            // ...
            using (var context = new StarWarsContext(options, dbLogger.Object))
            {
                // ...
            }
            // ...
            var repoLogger = new Mock<ILogger<DroidRepository>>();
            _droidRepository = new DroidRepository(starWarsContext, repoLogger.Object);
        }
    }
}
```

* Enjoy console logs
![console-logs-1](https://cloud.githubusercontent.com/assets/8171434/23155559/6b17afe8-f813-11e6-88e0-2923270f4ee8.png)
![console-logs-2](https://cloud.githubusercontent.com/assets/8171434/23155802/68f8e604-f814-11e6-90a7-44d09e851a51.png)

#### Code Coverage

* Install OpenCover NuGet package
![open-cover-nuget](https://cloud.githubusercontent.com/assets/8171434/23206746/247aa694-f8ef-11e6-9b14-d0ade0453b94.png)

* Add path to OpenCover tools to 'Path' environment variable. In my case it was:
```
C:\Users\Jacek_Kosciesza\.nuget\packages\opencover\4.6.519\tools\
```

* Set 'Full' debug type in all projects (StarWars.Api.csproj, StarWars.Core.csproj, StarWars.Data.csproj). This is needed to produce *.pdb files which are understandable by OpenCover.
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <!--...-->
    <DebugType>Full</DebugType>
  </PropertyGroup>
  <!--...-->
</Project>
```
* Run OpenCover in the console
```
OpenCover.Console.exe
    -target:"dotnet.exe"
    -targetargs:"test -f netcoreapp1.1 -c Release Tests/StarWars.Tests.Unit/StarWars.Tests.Unit.csproj"
    -hideskipped:File
    -output:coverage/unit/coverage.xml
    -oldStyle
    -filter:"+[StarWars*]* -[StarWars.Tests*]* -[StarWars.Api]*Program -[StarWars.Api]*Startup -[StarWars.Data]*EntityFramework.Workaround.Program -[StarWars.Data]*EntityFramework.Migrations* -[StarWars.Data]*EntityFramework.Seed*"
    -searchdirs:"Tests/StarWars.Tests.Unit/bin/Release/netcoreapp1.1"
    -register:user
```
![open-cover-unit-tests-results](https://cloud.githubusercontent.com/assets/8171434/23207392/70e8c8b0-f8f1-11e6-8284-2c6a1ead0f0c.png)

* Install 'ReportGenerator' NuGet package
![report-generator-nuget](https://cloud.githubusercontent.com/assets/8171434/23207452/a84e4e1a-f8f1-11e6-87fb-a28888f59ce8.png)

* Create simple script (unit-tests.bat)
```
mkdir coverage\unit
OpenCover.Console.exe -target:"dotnet.exe" -targetargs:"test -f netcoreapp1.1 -c Release Tests/StarWars.Tests.Unit/StarWars.Tests.Unit.csproj" -hideskipped:File -output:coverage/unit/coverage.xml -oldStyle -filter:"+[StarWars*]* -[StarWars.Tests*]* -[StarWars.Api]*Program -[StarWars.Api]*Startup -[StarWars.Data]*EntityFramework.Workaround.Program -[StarWars.Data]*EntityFramework.Migrations* -[StarWars.Data]*EntityFramework.Seed*" -searchdirs:"Tests/StarWars.Tests.Unit/bin/Release/netcoreapp1.1" -register:user
ReportGenerator.exe -reports:coverage/unit/coverage.xml -targetdir:coverage/unit -verbosity:Error
start .\coverage\unit\index.htm
```

* Enjoy HTML based code coverage report
![open-cover-html-report-results](https://cloud.githubusercontent.com/assets/8171434/23207608/2d314e7a-f8f2-11e6-81bc-1a91d1254c78.png)
 