$ErrorActionPreference = "Stop"

$toolPath = Join-Path $PSScriptRoot "..\_Shared\RimWorldModTools.ps1"
. $toolPath

$runtimeRoot = Join-Path $PSScriptRoot "Runtime Only\Stats Forked"

Invoke-RimWorldModDeploy `
    -ModName "Stats Forked" `
    -SourceRoot $runtimeRoot `
    -BuildPath (Join-Path $PSScriptRoot "Stats.sln") `
    -Configuration "Release" `
    -DotNetHome (Join-Path $PSScriptRoot ".dotnet") `
    -BuildArguments @("-m:1", "/p:UseSharedCompilation=false") `
    -Folders @("About", "Core", "Biotech", "Anomaly", "Odyssey", "CE") `
    -Files @("LoadFolders.xml") `
    -RemoveFilePatterns @("*.pdb")
