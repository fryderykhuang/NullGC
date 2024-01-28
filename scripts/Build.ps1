. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

foreach ($project in $Projects)
{
    dotnet build --no-restore -c Debug .\$project\$project.csproj
    if (!$?) { throw }
    dotnet build --no-restore -c Release .\$project\$project.csproj
    if (!$?) { throw }
}

foreach ($project in $MsbuildProjects)
{
    msbuild .\$project\$project.csproj -t:Rebuild -verbosity:minimal -property:Configuration=Debug
    if (!$?) { throw }
    msbuild .\$project\$project.csproj -t:Rebuild -verbosity:minimal -property:Configuration=Release
    if (!$?) { throw }
}

# $Env:Platform = 'x64'
# foreach ($project in $Projects)
# {
#     dotnet build --no-restore -c Release .\$project\$project.csproj
#     if (!$?) { throw }
# }

Pop-Location
