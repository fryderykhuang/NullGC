. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir

dotnet clean -c Release
dotnet clean -c Debug

# $Env:Platform = 'x64'
#     dotnet clean -c Release
#     dotnet clean -c Debug

Pop-Location
