# tmt-productizer

Test implementation for työmarkkinatori productizer. Actual API is not yet released, so we have to mock the data.

## Running with live data

### AWS Credentials

The TMT API credentials are fetched at runtime from AWS Secrets Manager. To run the app locally with live data, you need to have the AWS CLI installed and configured with credentials that have access to the secrets. The docker-compose.yml file is configured to mount the AWS credentials from your home directory, with a predefined profile name. If you want to use a different profile name or credentials path, you need to change the docker-compose.yml file.

### Running the app

To run the app with live data, run the following command:

```
docker compose up
```

Or with native dotnet:

```
export AWS_PROFILE=virtualfinland
dotnet run --project ./src/TMTProductizer/TMTProductizer.csproj
```

The endpoint should be available at: `http://localhost:5286/test/lassipatanen/Job/JobPosting`

### Locally testing the authentication with the Authentication GW

In local development the [Authentication GW](https://github.com/Virtual-Finland-Development/authentication-gw) checks are disabled by default. To enable them, you need to set the `ASPNETCORE_ENVIRONMENT` environment variable to `Staging` or `Production` when running the app. For example:

```
export ASPNETCORE_ENVIRONMENT=Staging
dotnet run --project ./src/TMTProductizer/TMTProductizer.csproj
```

The auth headers `Authorization` and `X-Authorization-Provider` are required in every enviroment stage. For stages `Development` and `Mock` the values for these headers are not important, as long as they are present.

### Generating models from Open API spec

Execute `generate-api-models.sh` shell script in terminal

### Mocking data with Prism

#### Install Prism

`npm install -g @stoplight/prism-cli`

#### Run Prism

Run the following command in `openapi` directory:

`prism mock TMT.yaml`

If you wish to generate dynamic data, use `--dynamic` flag with the previous command.

### Running productizer locally with mock data, mockally

Start up Prism mock server and take note of the port it is using (4010 by default). Open `appsettings.Development.json` and
make sure `TmtOptions:ApiEndpoint` host and port are matching that of Prism mock server.
To query job data send POST request with correct body to `http://localhost:5286/test/lassipatanen/Job/JobPosting`

Alternatively you can run both prism and productizer with Docker compose command

`docker compose -f docker-compose.prism.yml up`

Note: Prism doesn't seem to work in docker container if host machine is M1 Mac :(
Note2: when using mock data, the authorization methods are not used, so you can use any values for them.
