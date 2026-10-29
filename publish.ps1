$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

foreach ($rid in @("win-x64", "osx-arm64", "linux-x64")) {
    foreach ($project in @("HomeBudget.App", "HomeBudget.Desktop")) {
        Write-Host "== $rid / $project"
        dotnet publish $project -c Release -r $rid `
            --self-contained true -o "publish/$rid/$project"
    }
}
