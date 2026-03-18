# Packages the mod for Thunderstore upload.
# Run from the repo root: .\thunderstore\package.ps1
#
# Requires: icon.png (256x256) in thunderstore/
# Produces: thunderstore/LuckOfTheLefty-LethalEvents-1.0.0.zip
#
# NOTE: This mod requires websocket-sharp.dll and Newtonsoft.Json.dll bundled
#       alongside the main DLL. The script pulls them from the NuGet cache.

$ErrorActionPreference = "Stop"

$version = "1.0.0"
$outDir = "$PSScriptRoot\build"
$zipName = "LuckOfTheLefty-LethalEvents-$version.zip"

# Clean
if (Test-Path $outDir) { Remove-Item $outDir -Recurse -Force }
New-Item -ItemType Directory -Path $outDir | Out-Null

# Build
Push-Location "$PSScriptRoot\.."
dotnet build -c Release
Pop-Location

# Gather files
$dllSource = "$PSScriptRoot\..\LethalEvents\bin\Release\netstandard2.1\com.github.luckofthelefty.LethalEvents.dll"

Copy-Item "$PSScriptRoot\manifest.json" $outDir
Copy-Item "$PSScriptRoot\CHANGELOG.md" $outDir
Copy-Item "$PSScriptRoot\..\README.md" $outDir

if (Test-Path "$PSScriptRoot\icon.png") {
    Copy-Item "$PSScriptRoot\icon.png" $outDir
} else {
    Write-Warning "icon.png not found in thunderstore/ - you must add a 256x256 PNG before uploading!"
}

# Create plugins subfolder with DLLs
$pluginsDir = "$outDir\plugins"
New-Item -ItemType Directory -Path $pluginsDir | Out-Null
Copy-Item $dllSource $pluginsDir

# Bundle dependency DLLs from NuGet cache
$nugetCache = "$env:USERPROFILE\.nuget\packages"

$wsSharpDll = Get-ChildItem "$nugetCache\websocketsharp-netstandard\1.0.1\lib\netstandard2.0\websocket-sharp.dll" -ErrorAction SilentlyContinue
$newtonsoftDll = Get-ChildItem "$nugetCache\newtonsoft.json\13.0.3\lib\netstandard2.0\Newtonsoft.Json.dll" -ErrorAction SilentlyContinue

if ($wsSharpDll) {
    Copy-Item $wsSharpDll.FullName $pluginsDir
    Write-Host "Bundled: websocket-sharp.dll" -ForegroundColor Gray
} else {
    Write-Warning "websocket-sharp.dll not found in NuGet cache! Run 'dotnet restore' first."
}

if ($newtonsoftDll) {
    Copy-Item $newtonsoftDll.FullName $pluginsDir
    Write-Host "Bundled: Newtonsoft.Json.dll" -ForegroundColor Gray
} else {
    Write-Warning "Newtonsoft.Json.dll not found in NuGet cache! Run 'dotnet restore' first."
}

# Zip
$zipPath = "$PSScriptRoot\$zipName"
if (Test-Path $zipPath) { Remove-Item $zipPath }
Compress-Archive -Path "$outDir\*" -DestinationPath $zipPath

Write-Host "Package created: $zipPath" -ForegroundColor Green
Write-Host "Upload this zip to https://thunderstore.io" -ForegroundColor Cyan
