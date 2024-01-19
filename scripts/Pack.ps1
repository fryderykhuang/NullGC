. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

if (!(Test-Path -PathType Container $ArtifactsDir)) { New-Item -ItemType Directory -Path $ArtifactsDir }

$Env:Platform = ''
foreach ($project in $Projects) {
    dotnet pack --no-build -c Release -o $ArtifactsDir "-p:Version=$Env:GitVersion_NuGetVersionV2" .\$project\$project.csproj
    if (!$?) { throw }
}

Pop-Location
