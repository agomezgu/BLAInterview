#Requires -Version 5.1
# Starts the backend Web API, the identity host (IdP), and the Vite frontend in separate consoles.
# Typical dev URLs: API http://localhost:5245/swagger , IdP http://localhost:5249/swagger , FE http://localhost:5173
$ErrorActionPreference = "Stop"
$repoRoot = $PSScriptRoot
$beSrc = Join-Path $repoRoot "BE\src"
$webApiProject = Join-Path $beSrc "BLAInterview.WebApi\BLAInterview.WebApi.csproj"
$idpProject = Join-Path $beSrc "BLAInterview.Idp\BLAInterview.Idp.csproj"
$feDir = Join-Path $repoRoot "FE"

@(
    @{ Name = "Web API";  Path = $webApiProject }
    @{ Name = "IdP";     Path = $idpProject }
) | ForEach-Object {
    if (-not (Test-Path $_.Path)) {
        throw "Missing project file for $($_.Name): $($_.Path)"
    }
}
if (-not (Test-Path $feDir)) {
    throw "Missing FE folder: $feDir"
}
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet CLI not found. Install the .NET SDK and ensure it is on PATH."
}
if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
    throw "npm not found. Install Node.js and ensure it is on PATH."
}

$webApiFull = (Resolve-Path $webApiProject).Path
$idpFull = (Resolve-Path $idpProject).Path

# Identity provider (OpenIddict / OIDC host)
Start-Process powershell -ArgumentList @(
    "-NoExit", "-NoLogo", "-Command",
    "Set-Location -LiteralPath ""$repoRoot""; " +
    "Write-Host 'BLAInterview IdP' -ForegroundColor Cyan; " +
    "dotnet run --project ""$idpFull"" --launch-profile https"
)

# REST API
Start-Process powershell -ArgumentList @(
    "-NoExit", "-NoLogo", "-Command",
    "Set-Location -LiteralPath ""$repoRoot""; " +
    "Write-Host 'BLAInterview WebApi' -ForegroundColor Cyan; " +
    "dotnet run --project ""$webApiFull"" --launch-profile https"
)

# Frontend (Vite; default http://localhost:5173)
Start-Process powershell -WorkingDirectory $feDir -ArgumentList @(
    "-NoExit", "-NoLogo", "-Command",
    "Write-Host 'Vite (FE)' -ForegroundColor Cyan; npm run dev"
)

Write-Host "Started IdP, Web API, and FE in new PowerShell windows." -ForegroundColor Green
