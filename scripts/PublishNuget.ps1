. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
$Env:Platform = ''
foreach ($project in $Projects)
{
    dotnet nuget push ".\artifacts\$project.$Env:GitVersion_NuGetVersionV2.nupkg" --skip-duplicate --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
}

Pop-Location
