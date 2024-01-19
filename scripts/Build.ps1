. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

foreach ($project in $Projects)
{
    dotnet build --no-restore -c Debug .\$project\$project.csproj
    dotnet build --no-restore -c Release .\$project\$project.csproj
    if (!$?) { throw }
}

# $Env:Platform = 'x64'
# foreach ($project in $Projects)
# {
#     dotnet build --no-restore -c Release .\$project\$project.csproj
#     if (!$?) { throw }
# }

Pop-Location
