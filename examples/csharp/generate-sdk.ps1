[CmdletBinding()]
param (
    [string]
    $ApiVersion = "5.3",

    [string]
    $ApiMetadataUrl = "https://api.ed-fi.org/v$ApiVersion/api/metadata",

    [string]
    $NetFramework = "netcoreapp3.1",
    # https://openapi-generator.tech/docs/generators/csharp-netcore

    [switch]
    $IncludeChangeQueries,

    [switch]
    $IncludeEnrollmentComposite
)
$generator = "csharp-netcore"

function Get-OpenApiGenerator() {
    if ($null -eq (Get-Command java -ErrorAction SilentlyContinue))
    {
        Write-Host "Unable to find java in your PATH"
    }

    if (-not (Test-Path "openapi-generator-cli.jar")) {
        $url = "https://repo1.maven.org/maven2/org/openapitools/openapi-generator-cli/5.4.0/openapi-generator-cli-5.4.0.jar"
        Invoke-WebRequest -OutFile openapi-generator-cli.jar $url
    }
}

function New-EdFiClient() {
    $params = @(
        "-jar", "openapi-generator-cli.jar",
        "generate",
        "-g", $generator,
        "-o", "ed-fi-client-$ApiVersion",
        "--additional-properties=netCoreProjectFile",
        "--additional-properties=packageName=EdFi.Roster.Sdk",
        "--additional-properties=targetFramework=$NetFramework",
        "--additional-properties=modelPropertyNaming=PascalCase"
    )

    $resourcesUrl = "$ApiMetadataUrl/data/v3/resources/swagger.json"
    &java @params -i $resourcesUrl --api-package Api.Resources --model-package Models.Resources

    $descriptorsUrl = "$ApiMetadataUrl/data/v3/descriptors/swagger.json"
    &java @params -i $descriptorsUrl --api-package Api.Descriptors --model-package Models.Resources

    if ($IncludeEnrollmentComposite) {
        $enrollmentUrl = "$ApiMetadataUrl/composites/v1/ed-fi/enrollment/swagger.json"
        &java @params -i $enrollmentUrl --api-package Api.EnrollmentComposites --model-package Models.EnrollmentComposites
    }

    if ($IncludeChangeQueries) {
        $changeQueriesUrl = "$ApiMetadataUrl/changequeries/v1/swagger.json"
        &java @params -i $changeQueriesUrl --api-package Api.ChangeQueries --model-package Models.ChangeQueries
    }
}

function Invoke-DotnetBuild() {
    # First need to add the SDK project to the solution file
    &dotnet sln add ed-fi-client-$ApiVersion/src/EdFi.Roster.Sdk/EdFi.Roster.Sdk.csproj
    &dotnet sln add ed-fi-client-$ApiVersion/src/EdFi.Roster.Sdk.Test/EdFi.Roster.Sdk.Test.csproj
    &dotnet add reference ed-fi-client-$ApiVersion/src/EdFi.Roster.Sdk/EdFi.Roster.Sdk.csproj
    &dotnet build ./get-students-by-grade.sln
}

Get-OpenApiGenerator
New-EdFiClient
Invoke-DotnetBuild

Write-Host "Client has been built. Sample run command for SchoolId 255901001 and 9th grade"
Write-Host './bin/Debug/netcoreapp3.1/get-students-by-grade.exe https://api.ed-fi.org/v5.3/api yourKey yourSecret 255901001 "Ninth grade"'
