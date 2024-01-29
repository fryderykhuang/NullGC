. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

foreach ($project in $Projects)
{
    dotnet build --no-restore -c Debug -p:Version=$Env:GitVersion_NuGetVersionV2 .\$project\$project.csproj
    if (!$?) { throw }
    dotnet build --no-restore -c Release -p:Version=$Env:GitVersion_NuGetVersionV2 .\$project\$project.csproj
    if (!$?) { throw }
}

foreach ($project in $MsbuildProjects)
{
    msbuild .\$project\$project.csproj -t:Rebuild -verbosity:minimal -p:Configuration=Debug -p:Version=$Env:GitVersion_NuGetVersionV2
    if (!$?) { throw }
    msbuild .\$project\$project.csproj -t:Rebuild -verbosity:minimal -p:Configuration=Release -p:Version=$Env:GitVersion_NuGetVersionV2
    if (!$?) { throw }
}

# $Env:Platform = 'x64'
# foreach ($project in $Projects)
# {
#     dotnet build --no-restore -c Release .\$project\$project.csproj
#     if (!$?) { throw }
# }

Pop-Location
