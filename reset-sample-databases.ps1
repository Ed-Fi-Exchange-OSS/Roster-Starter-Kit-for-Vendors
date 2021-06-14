$ErrorActionPreference = "Stop"

function Reset-SqliteDatabase($fullPath) {
    if (Test-Path $fullPath) {
        $relative = Resolve-Path -Relative $fullPath
        Write-Host "Resetting $relative"
        Remove-Item $fullPath
    } else {
        Write-Host "No work to perform for $fullPath"
    }
}

Reset-SqliteDatabase "$PSScriptRoot/src/EdFi.Roster.Explorer/RosterDatabase.db"
Reset-SqliteDatabase "$PSScriptRoot/src/EdFi.Roster.ChangeQueries/ChangeQueriesDatabase.db"
