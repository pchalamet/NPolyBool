config ?= Debug
version ?= 0.0.0

build:
	dotnet build -c $(config) NPolyBool.csproj

nuget:
	dotnet pack -c $(config) /p:Version=$(version) -o out NPolyBool.csproj

publish:
	dotnet nuget push out/*.nupkg -k $(nugetkey) -s https://api.nuget.org/v3/index.json --skip-duplicate
