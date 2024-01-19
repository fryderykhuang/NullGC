. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
foreach ($project in $Projects)
{
    dotnet restore .\$project\$project.csproj
    if (!$?) { throw }
}

Pop-Location
