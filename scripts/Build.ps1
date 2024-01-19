. $PSScriptRoot\Variables.ps1
pushd $SolutionDir

$Env:Platform = ''
foreach ($project in $Projects)
{
    dotnet build --no-restore -c Release .\$project\$project.csproj
}

$Env:Platform = 'x64'
foreach ($project in $Projects)
{
    dotnet build --no-restore -c Release .\$project\$project.csproj
}

popd
