. $PSScriptRoot\Variables.ps1
pushd $SolutionDir
foreach ($project in $Projects)
{
    dotnet restore .\$project\$project.csproj
}

popd
