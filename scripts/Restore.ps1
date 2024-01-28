. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
foreach ($project in $Projects)
{
    dotnet restore .\$project\$project.csproj
    if (!$?) { throw }
}

foreach ($project in $MsbuildProjects)
{
    msbuild .\$project\$project.csproj -t:Restore -verbosity:minimal -property:Configuration=Debug
    if (!$?) { throw }
    msbuild .\$project\$project.csproj -t:Restore -verbosity:minimal -property:Configuration=Release
    if (!$?) { throw }
}

Pop-Location
