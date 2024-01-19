. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

# the GHA setup-dotnet action currently not support specify architecture, and x86 SDK is not installed.
# $Env:Platform = ''
# foreach ($project in $Tests)
# {
#     dotnet test -c Debug -a x86 .\$project\$project.csproj
#     dotnet test -c Release -a x86 .\$project\$project.csproj
# }

$Env:Platform = 'x64'
foreach ($project in $Tests)
{
    dotnet test -c Debug -a x64 -v n .\$project\$project.csproj
    if (!$?) { throw }
    dotnet test -c Release -a x64 -v n .\$project\$project.csproj
    if (!$?) { throw }
}

Pop-Location
