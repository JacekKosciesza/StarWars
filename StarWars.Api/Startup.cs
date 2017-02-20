using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarWars.Core.Data;
using StarWars.Api.Models;
using StarWars.Data.EntityFramework;
using StarWars.Data.EntityFramework.Seed;
using Microsoft.EntityFrameworkCore;
using StarWars.Data.EntityFramework.Repositories;
using GraphQL.Types;
using GraphQL;

namespace StarWars.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Env = env;
        }

        public IConfigurationRoot Configuration { get; }
        private IHostingEnvironment Env { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddTransient<StarWarsQuery>();
            services.AddTransient<IDroidRepository, DroidRepository>();
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
            services.AddTransient<IDocumentExecuter, DocumentExecuter>();
            var sp = services.BuildServiceProvider();
            services.AddTransient<ISchema>(_ => new Schema { Query = sp.GetService<StarWarsQuery>() });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
                              ILoggerFactory loggerFactory, StarWarsContext db)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();
            app.UseMvc();

            db.EnsureSeedData();
        }
    }
}
