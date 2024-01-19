. $PSScriptRoot\Variables.ps1
Push-Location $SolutionDir
$Env:BenchmarkDotNet_ArtifactsPath = $BenchmarkArtifactsDir;
Write-Output BenchmarkDotNet_ArtifactsPath=$Env:BenchmarkDotNet_ArtifactsPath
dotnet run -c Release --project .\NullGC.DragRace\NullGC.DragRace.csproj
if (!$?) { throw }
Pop-Location
