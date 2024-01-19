. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

foreach ($project in $Projects) {
    dotnet pack --no-build -c Release -o $ArtifactsDir "-p:Version=$Env:GitVersion_NuGetVersionV2" .\$project\$project.csproj
    if (!$?) { throw }
}

Pop-Location
