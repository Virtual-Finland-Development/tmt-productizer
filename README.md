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
`prism mock TMT.yaml`

or to generate dynamic data use `--dynamic` flag

