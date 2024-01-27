. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
foreach ($project in $NuGetProjects) {
    if ($project -eq 'NullGC.Analyzer.Package') {
        $project = 'NullGC.Analyzer'
    }
    dotnet nuget push (Join-Path $ArtifactsDir "${project}.${Env:GitVersion_NuGetVersionV2}.nupkg" | Resolve-Path) --source 'https://api.nuget.org/v3/index.json' --api-key $Env:NUGET_API_KEY --skip-duplicate
}

Pop-Location
