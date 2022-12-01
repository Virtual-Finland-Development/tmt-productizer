build:
	dotnet build --no-restore -o ./deployment/release

test: build
	dotnet test ./deployment/release/TMTProductizer.UnitTests.dll --no-build --verbosity normal
