. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

foreach ($project in $Projects)
{
    dotnet clean -c Release .\$project\$project.csproj
    dotnet clean -c Debug .\$project\$project.csproj
}

# $Env:Platform = 'x64'
# foreach ($project in $Projects)
# {
#     dotnet clean -c Release .\$project\$project.csproj
#     dotnet clean -c Debug .\$project\$project.csproj
# }

Pop-Location
