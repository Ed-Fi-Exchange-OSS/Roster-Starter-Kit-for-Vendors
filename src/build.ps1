$ErrorActionPreference = "Stop"

function step($command) {
    Write-Host ([Environment]::NewLine + $command.ToString().Trim()) -fore CYAN
    $global:lastexitcode = 0
    & $command
    if ($lastexitcode -ne 0) { throw "Unexpected return code: $lastexitcode." }
}

step { dotnet --version }
step { dotnet clean -c Release --nologo -v minimal }
step { dotnet build -c Release --nologo }