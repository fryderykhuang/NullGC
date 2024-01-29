. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

foreach ($project in $Projects)
{
    dotnet clean -c Release .\$project\$project.csproj
    if (!$?) { throw }
    dotnet clean -c Debug .\$project\$project.csproj
    if (!$?) { throw }
}

foreach ($project in $MsbuildProjects)
{
    msbuild .\$project\$project.csproj -t:Clean -verbosity:minimal -p:Configuration=Debug
    if (!$?) { throw }
    msbuild .\$project\$project.csproj -t:Clean -verbosity:minimal -p:Configuration=Release
    if (!$?) { throw }
}

# $Env:Platform = 'x64'
# foreach ($project in $Projects)
# {
#     dotnet clean -c Release .\$project\$project.csproj
#     dotnet clean -c Debug .\$project\$project.csproj
# }

Pop-Location
