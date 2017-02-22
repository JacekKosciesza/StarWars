mkdir coverage\integration
OpenCover.Console.exe -target:"dotnet.exe" -targetargs:"test -f netcoreapp1.1 -c Release Tests/StarWars.Tests.Integration/StarWars.Tests.Integration.csproj" -hideskipped:File -output:coverage/integration/coverage.xml -oldStyle -filter:"+[StarWars*]* -[StarWars.Tests*]* -[StarWars.Api]*Program -[StarWars.Api]*Startup -[StarWars.Data]*EntityFramework.Workaround.Program -[StarWars.Data]*EntityFramework.Migrations* -[StarWars.Data]*EntityFramework.Seed*" -searchdirs:"Tests/StarWars.Tests.Integration/bin/Release/netcoreapp1.1" -register:user
ReportGenerator.exe -reports:coverage/integration/coverage.xml -targetdir:coverage/integration -verbosity:Error
start .\coverage\integration\index.htm
