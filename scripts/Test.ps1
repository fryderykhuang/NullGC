. $PSScriptRoot\Variables.ps1
pushd $SolutionDir
$Env:Platform = ''
foreach ($project in $Tests)
{
    dotnet test -c Debug -a x86 .\$project\$project.csproj
    dotnet test -c Release -a x86 .\$project\$project.csproj
}

$Env:Platform = 'x64'
foreach ($project in $Tests)
{
    dotnet test -c Debug -a x64 .\$project\$project.csproj
    dotnet test -c Release -a x64 .\$project\$project.csproj
}

popd
