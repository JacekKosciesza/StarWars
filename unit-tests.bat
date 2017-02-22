mkdir coverage\unit
OpenCover.Console.exe -target:"dotnet.exe" -targetargs:"test -f netcoreapp1.1 -c Release Tests/StarWars.Tests.Unit/StarWars.Tests.Unit.csproj" -hideskipped:File -output:coverage/unit/coverage.xml -oldStyle -filter:"+[StarWars*]* -[StarWars.Tests*]* -[StarWars.Api]*Program -[StarWars.Api]*Startup -[StarWars.Data]*EntityFramework.Workaround.Program -[StarWars.Data]*EntityFramework.Migrations* -[StarWars.Data]*EntityFramework.Seed*" -searchdirs:"Tests/StarWars.Tests.Unit/bin/Release/netcoreapp1.1" -register:user
ReportGenerator.exe -reports:coverage/unit/coverage.xml -targetdir:coverage/unit -verbosity:Error
start .\coverage\unit\index.htm
