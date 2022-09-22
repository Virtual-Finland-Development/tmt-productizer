# tmt-productizer
Test implementation for työmarkkinatori productizer. Actual API is not yet released, so we have to mock the data.

### Generating models from Open API spec

`docker run --rm -v "${PWD}:/local" openapitools/openapi-generator-cli generate \
    -i /local/openapi/TMT.yaml \
    -g csharp-netcore \
    -o /local/generated/TMTDataModels \
    --global-property=apiTests=false,modelTests=false,modelDocs=false \
    -c /local/config.yaml`

### Mocking data with Prism

#### Install Prism
`npm install -g @stoplight/prism-cli`

#### Run Prism
Run the following command in openapi directory:

`prism mock TMT.yaml`

If you wish to generate dynamic data, use `--dynamic` flag with the previous command.

### Running productizer locally

Start up Prism mock server and take note of the port it is using (4010 by default). Open `appsettings.Development.json` and
make sure `TmtOptions:ApiEndpoint` host and port are matching that of Prism mock server.

To query job data send POST request with correct body to `http://localhost:5286/jobs`

