. $PSScriptRoot\Variables.ps1
pushd $SolutionDir

$Env:Platform = ''
foreach ($project in $Projects)
{
    dotnet clean -c Release .\$project\$project.csproj
    dotnet clean -c Debug .\$project\$project.csproj
}

$Env:Platform = 'x64'
foreach ($project in $Projects)
{
    dotnet clean -c Release .\$project\$project.csproj
    dotnet clean -c Debug .\$project\$project.csproj
}

popd
