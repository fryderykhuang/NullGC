. $PSScriptRoot\Variables.ps1
pushd $SolutionDir

$Env:Platform = ''
    dotnet clean -c Release
    dotnet clean -c Debug

$Env:Platform = 'x64'
    dotnet clean -c Release
    dotnet clean -c Debug


popd
