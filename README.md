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
