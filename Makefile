config ?= Debug
version ?= 0.0.0

build:
	dotnet build -c $(config) NPolyBool.sln

nuget:
	dotnet pack -c $(config) /p:Version=$(version) -o $(PWD)/out NPolyBool.sln

publish:
	dotnet nuget push out/*.nupkg -k $(nugetkey) -s https://api.nuget.org/v3/index.json --skip-duplicate
