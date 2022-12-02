build:
	dotnet build --no-restore -o ./deployment/release

test: build
	dotnet test ./deployment/release/TMTProductizer.UnitTests.dll --no-build --verbosity normal

deploy: build
	pulumi -C deployment up --yes --config tmt-productizer:artifactPath=release/

run:
	dotnet run --project ./src/TMTProductizer/TMTProductizer.csproj

update-cache:
	dotnet run --project ./src/TMTCacheUpdater/TMTCacheUpdater.csproj