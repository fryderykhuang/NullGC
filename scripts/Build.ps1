. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

$Env:Platform = ''
foreach ($project in $Projects)
{
    dotnet build --no-restore -c Release .\$project\$project.csproj
    if (!$?) { throw }
}

$Env:Platform = 'x64'
foreach ($project in $Projects)
{
    dotnet build --no-restore -c Release .\$project\$project.csproj
    if (!$?) { throw }
}

Pop-Location
