. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

foreach ($project in $Tests)
{
    dotnet test -c Debug .\$project\$project.csproj
    dotnet test -c Release .\$project\$project.csproj
}

# $Env:Platform = 'x64'
# foreach ($project in $Tests)
# {
#     dotnet test -c Debug -a x64 --logger "console;verbosity=detailed" .\$project\$project.csproj
#     if (!$?) { throw }
#     dotnet test -c Release -a x64 --logger "console;verbosity=detailed" .\$project\$project.csproj
#     if (!$?) { throw }
# }

Pop-Location
