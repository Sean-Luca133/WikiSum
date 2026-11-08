
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("Android", "Windows")]
    [string]$Platform
)

$ProjectPath = Join-Path $PSScriptRoot "wikisum"

Set-Location $ProjectPath

switch ($Platform) {
    "Android" {
        Write-Host "Starting MAUI Android app..." -ForegroundColor Green
        dotnet build -f net10.0-android
        dotnet build -t:Run -f net10.0-android
    }

    "Windows" {
        Write-Host "Starting MAUI Windows app..." -ForegroundColor Green
        dotnet build -f net10.0-windows10.0.19041.0
        dotnet build -t:Run -f net10.0-windows10.0.19041.0
    }
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build or run failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}
Set-Location $PSScriptRoot
