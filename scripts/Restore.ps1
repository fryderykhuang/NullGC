. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
foreach ($project in $Projects)
{
    dotnet restore .\$project\$project.csproj
    if (!$?) { throw }
}

foreach ($project in $MsbuildProjects)
{
    msbuild .\$project\$project.csproj -t:Restore -verbosity:minimal -p:Configuration=Debug
    if (!$?) { throw }
    msbuild .\$project\$project.csproj -t:Restore -verbosity:minimal -p:Configuration=Release
    if (!$?) { throw }
}

Pop-Location
