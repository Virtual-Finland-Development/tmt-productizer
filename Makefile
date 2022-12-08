build:
	dotnet build --no-restore -o ./deployment/release

test: build
	dotnet test ./deployment/release/TMTProductizer.UnitTests.dll --no-build --verbosity normal

run:
	dotnet run --project ./src/TMTProductizer/TMTProductizer.csproj

deploy: build
	pulumi -C deployment up --yes --config tmt-productizer:artifactPath=release/
update-cache:
	dotnet run --project ./src/TMTCacheUpdater/TMTCacheUpdater.csproj