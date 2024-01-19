. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
if (!(Test-Path -PathType Container $BenchmarkResultPageDir)) { New-Item -ItemType Directory -Path $BenchmarkResultPageDir }
Copy-Item $BenchmarkArtifactsDir\results\*.html $BenchmarkResultPageDir
Get-ChildItem -Recurse -Force .
dotnet run --project .\BenchmarkResultPageGenerator\BenchmarkResultPageGenerator.csproj $BenchmarkArtifactsDir\results $BenchmarkResultPageDir\index.html
if (!$?) { throw }
Pop-Location
