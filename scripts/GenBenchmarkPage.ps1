. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
if (!(Test-Path -PathType Container $BenchmarkResultPageDir)) { New-Item -ItemType Directory -Path $BenchmarkResultPageDir }
Copy-Item $BenchmarkArtifactsDir\results\*.html $BenchmarkResultPageDir
dotnet run --project .\BenchmarkResultPageGenerator $BenchmarkArtifactsDir\results '' $BenchmarkResultPageDir\index.html
Pop-Location
