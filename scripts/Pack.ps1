. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

foreach ($project in $NuGetProjects) {
    dotnet pack --no-build -c Release -o $ArtifactsDir "-p:PackageVersion=$Env:GitVersion_NuGetVersionV2" .\$project\$project.csproj
    if (!$?) { throw }
}

Pop-Location
