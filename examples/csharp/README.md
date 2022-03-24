# Get Students by Grade

This is an example C# program using the SDK to retrieve all students for a
particular grade and school. The code is "quick and dirty", intended to
highlight how to interact with the API, at the expense of the clean code
polishing that should go into a production application.

To get started, run the Powershell script [generate-sdk.ps1](generate-sdk.ps1).
This script:

1. Downloads the [OpenAPI Generator
   CLI](https://openapi-generator.tech/docs/installation/)
2. Generates a new SDK in C#
3. Integrates the sample console application with the generated SDK code
4. Builds the solution so that it is ready to run.

The script accepts the following arguments, all of which have default values:

| Argument              | Default                                 | Explanation                                                                                   |
| --------------------- | --------------------------------------- | --------------------------------------------------------------------------------------------- |
| $ApiVersion           | 5.3                                     | The ODS/API version for which this SDK is being generated                                     |
| $ApiMetadataUrl       | https://api.ed-fi.org/v5.3/api/metadata | The metadata URL from the running API                                                         |
| $NetFramework         | netcoreapp3.1                           | [Which framework](https://openapi-generator.tech/docs/generators/csharp-netcore) to support |
| $IncludeChangeQueries | false                                   | (switch) When true, generates SDK for change queries                                          |
| $IncludeEnrollments   | false                                   | (switch) When true, generates SDK for the enrollments composite                               |

Examples:

```pshell
# Run with all defaults
./generate-sdk.ps1

# Or override all values
$params = @{
    ApiVersion = "5.2"
    ApiMetadataUrl = "https://api.ed-fi.org/v5.2/api/metadata"
    NetFramework = "net6.0"
    IncludeChangeQueries = $True
    IncludeEnrollments = $True
}
./generate-sdk.ps1 @params
```

The generated SDK will be in the `ed-fi-client-$ApiVersion` directory. The
console application is in this root source directory.

To run the console application, you will need to provide five pieces of
information as runtime arguments:

* Base Url for the API, example: https://api.ed-fi.org/v5.2/api
* A client key
* The secret that goes with that client key
* A schoolId (integer) - this must be a SchoolId that is actually in the ODS/API
  already
* Grade level (string) - the specific string will depend on the implementation.
  Out of the box, this might be something like "Ninth grade" (not "9th grade"),
  but any actual implementation can change the descriptor. This will match on
  the `descriptor.CodeValue` field. The match is case insensitive.

Example:

```pshell
$url = "https://api.ed-fi.org/v5.2/api"
$key = "<yourKey>"
$secret = "<yourSecret>"
$schoolId = 255901001
$grade = "ninth grade"
&./bin/debug/netcoreapp3.1/get-students-by-grade.exe $url $key $secret $schoolId $grade
```
