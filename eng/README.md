# Code Generation

This solution includes an ODS / API SDK using code generation. The following commands generated that code:

## One-Time Command to Fetch the Code Generator Tools

Invoke-WebRequest -OutFile openapi-generator-cli.jar https://repo1.maven.org/maven2/org/openapitools/openapi-generator-cli/5.1.1/openapi-generator-cli-5.1.1.jar

## Commands Which Generate the SDK


java -jar openapi-generator-cli.jar generate -g csharp-netcore -i https://api.ed-fi.org/v5.2/api/metadata/composites/v1/ed-fi/enrollment/swagger.json --api-package `
Api.EnrollmentComposites --model-package Models.EnrollmentComposites -o csharp-netcore --additional-properties=netCoreProjectFile `
--additional-properties=packageName='EdFi.Roster.Sdk' --additional-properties=targetFramework=netcoreapp3.1 --additional-properties=modelPropertyNaming=PascalCase

java -jar .\\openapi-generator-cli.jar generate -g csharp-netcore -i https://api.ed-fi.org/v5.2/api/metadata/changequeries/v1/swagger.json --api-package `
Api.ChangeQueries --model-package Models.ChangeQueries -o csharp-netcore --additional-properties=netCoreProjectFile `
--additional-properties=packageName='EdFi.Roster.Sdk' --additional-properties=targetFramework=netcoreapp3.1 --additional-properties=modelPropertyNaming=PascalCase

java -jar .\\openapi-generator-cli.jar generate -g csharp-netcore -i https://api.ed-fi.org/v5.2/api/metadata/data/v3/resources/swagger.json --api-package `
Api.Resources --model-package Models.Resources -o csharp-netcore --additional-properties=netCoreProjectFile `
--additional-properties=packageName='EdFi.Roster.Sdk' --additional-properties=targetFramework=netcoreapp3.1 --additional-properties=modelPropertyNaming=PascalCase
