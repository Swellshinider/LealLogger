# Navigate to the project directory
Set-Location -Path ".\LealLogger"

# Pack the project in Release mode
dotnet pack -c Release

# Search for the new package created
$packagePath = Get-ChildItem -Filter *.nupkg -Recurse | Sort-Object LastWriteTime -Descending | Select-Object -First 1

# Check if the package was found
if (-not $packagePath) {
    Write-Host "Package not found. Please check if the package was created."
    Set-Location -Path "..\"
    exit 1
}

# Get the nuget key from the environment variable
$nugetApiKey = $env:LealLogger_nugetkey

# Check if the nuget key is set
if (-not $nugetApiKey) {
    Write-Host "Nuget key not set. Please set the environment variable 'LealLogger_nugetkey' with the nuget key."
    Set-Location -Path "..\"
    exit 1
}

# Publish the package
dotnet nuget push $packagePath --source "https://api.nuget.org/v3/index.json" --api-key $nugetApiKey --skip-duplicate

# back to the root directory
Set-Location -Path "..\"