$ErrorActionPreference = 'Stop'

$ModsDir = Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'My Games\Stationeers\mods'

if (-not (Test-Path $ModsDir)) {
    Write-Error "Mods directory not found: $ModsDir. Is Stationeers installed?"
    exit 1
}

$ModDir = Join-Path $ModsDir 'StationeersCombustionFix'

dotnet build "$PSScriptRoot\StationeersCombustionFix\StationeersCombustionFix.csproj" -c Release

New-Item -ItemType Directory -Path $ModDir -Force | Out-Null

Copy-Item "$PSScriptRoot\About" $ModDir -Recurse -Force

$Dll = Join-Path $PSScriptRoot 'StationeersCombustionFix\bin\Release\netstandard2.0\StationeersCombustionFix.dll'
Copy-Item $Dll $ModDir -Force

Write-Host "Plugin deployed to $ModDir" -ForegroundColor Green
