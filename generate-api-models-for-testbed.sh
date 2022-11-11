docker run --rm \
    -v "/Users/lassi.patanen/_dev/definitions/DataProducts/test/lassipatanen/Job:/definition" \
    -v "${PWD}:/local" \
     openapitools/openapi-generator-cli generate \
    -i /definition/JobPosting.json \
    -g csharp-netcore \
    -o /local/ \
    --global-property=apiTests=false,modelTests=false,modelDocs=false \
    -c /local/generate-api-models-for-testbed.sh
