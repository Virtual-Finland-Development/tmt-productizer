build:
	dotnet restore
	dotnet build --no-restore -o ./deployment/release
	dotnet publish src/TMTProductizer/TMTProductizer.csproj -c Release -o ./deployment/release --nologo
	dotnet publish src/TMTCacheUpdater/TMTCacheUpdater.csproj -c Release -o ./deployment/release --nologo

test: build
	dotnet test ./deployment/release/TMTProductizer.UnitTests.dll --no-build --verbosity normal

run:
	dotnet run --project ./src/TMTProductizer/TMTProductizer.csproj

deploy: build
	pulumi -C deployment up --yes --config tmt-productizer:artifactPath=release/

update-cache:
	dotnet run --project ./src/TMTCacheUpdater.CLI/TMTCacheUpdater.CLI.csproj