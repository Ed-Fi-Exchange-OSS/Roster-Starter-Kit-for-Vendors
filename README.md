# Roster-Starter-Kit-for-Vendors

The Roster Starter Kit provides a step-by-step process that can be used to integrate rostering via the Ed-Fi ODS / API into a technology provider platform.

## Local Sample Databases

This system uses local SQLite files to store data fetched from the ODS / API and to track its own progress during Change Queries sync operations.

This is simply a local cache of data for demonstration purposes and can safely be deleted/reset at any time. To reset this data at the command line, simply execute the following PowerShell command:

> ./reset-sample-databases.ps1

## Code Generation

This solution includes an ODS / API SDK using code generation. For a detailed walk-through of the code generation process, please see [Generating Client Libraries Using OpenAPI Generator](https://techdocs.ed-fi.org/display/SK/Generating+Client+Libraries+Using+OpenAPI+Generator)

The following commands generated the SDK in this solution:

**One-Time Command to Fetch the Code Generator Tools**

> Invoke-WebRequest -OutFile openapi-generator-cli.jar https://repo1.maven.org/maven2/org/openapitools/openapi-generator-cli/5.1.1/openapi-generator-cli-5.1.1.jar

**Commands Which Generate the SDK**

> java -jar openapi-generator-cli.jar generate -g csharp-netcore -i https://api.ed-fi.org/v5.2/api/metadata/composites/v1/ed-fi/enrollment/swagger.json --api-package `
Api.EnrollmentComposites --model-package Models.EnrollmentComposites -o csharp-netcore --additional-properties=netCoreProjectFile `
--additional-properties=packageName='EdFi.Roster.Sdk' --additional-properties=targetFramework=netcoreapp3.1 --additional-properties=modelPropertyNaming=PascalCase
>
> java -jar openapi-generator-cli.jar generate -g csharp-netcore -i https://api.ed-fi.org/v5.2/api/metadata/changequeries/v1/swagger.json --api-package `
Api.ChangeQueries --model-package Models.ChangeQueries -o csharp-netcore --additional-properties=netCoreProjectFile `
--additional-properties=packageName='EdFi.Roster.Sdk' --additional-properties=targetFramework=netcoreapp3.1 --additional-properties=modelPropertyNaming=PascalCase
>
> java -jar openapi-generator-cli.jar generate -g csharp-netcore -i https://api.ed-fi.org/v5.2/api/metadata/data/v3/resources/swagger.json --api-package `
Api.Resources --model-package Models.Resources -o csharp-netcore --additional-properties=netCoreProjectFile `
--additional-properties=packageName='EdFi.Roster.Sdk' --additional-properties=targetFramework=netcoreapp3.1 --additional-properties=modelPropertyNaming=PascalCase


## Legal Information

Copyright (c) 2021 Ed-Fi Alliance, LLC and contributors.

Licensed under the [Apache License, Version 2.0](LICENSE) (the "License").

Unless required by applicable law or agreed to in writing, software distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied. See the License for the
specific language governing permissions and limitations under the License.

See [NOTICES](NOTICES.md) for additional copyright and license notifications.

This applicaton uses Google Analytics for usage monitoring and is covered under the Ed-Fi Terms of Use and Privacy Policy available here: https://www.ed-fi.org/terms-of-use-and-privacy-policy/.
