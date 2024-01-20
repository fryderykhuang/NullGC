. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
Copy-Item $BenchmarkArtifactsDir\results\*.html $BenchmarkResultPageDir
dotnet run --project .\BenchmarkResultPageGenerator\BenchmarkResultPageGenerator.csproj (Join-Path $BenchmarkArtifactsDir results) (Join-Path $BenchmarkResultPageDir index.html)
if (!$?) { throw }
Pop-Location
