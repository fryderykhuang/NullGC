. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
foreach ($project in $Projects)
{
    dotnet nuget push (Join-Path $ArtifactsDir $project.$Env:GitVersion_NuGetVersionV2.nupkg) --skip-duplicate --api-key $Env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
}

Pop-Location
