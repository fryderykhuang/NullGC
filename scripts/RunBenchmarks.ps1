. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
if (!(Test-Path -PathType Container $BenchmarkArtifactsDir)) { New-Item -ItemType Directory -Path $BenchmarkArtifactsDir }
$Env:BenchmarkDotNet_ArtifactsPath = $BenchmarkArtifactsDir;
$Env:Platform = 'x64'; dotnet run -c Release --project .\NullGC.DragRace\NullGC.DragRace.csproj
if (!$?) { throw }
Pop-Location
