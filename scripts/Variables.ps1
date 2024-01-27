$SolutionDir = Join-Path $PSScriptRoot '..\src'
$ArtifactsDir = Join-Path $PSScriptRoot '..\artifacts'
$Projects = @("NullGC.Abstractions", "NullGC.Allocators", "NullGC.Collections", "NullGC.Linq", "NullGC.Analyzer", "NullGC.Analyzer.CodeFixes", "NullGC.Analyzer.Package");
$NuGetProjects = @("NullGC.Abstractions", "NullGC.Allocators", "NullGC.Collections", "NullGC.Linq", "NullGC.Analyzer.Package")
$Tests = @("NullGC.Allocators.Tests", "NullGC.Collections.Tests", "NullGC.Linq.Tests", "NullGC.Analyzer.Tests");
$BenchmarkArtifactsDir = Join-Path $PSScriptRoot '..\artifacts\BenchmarkArtifacts'
$BenchmarkResultPageDir = Join-Path $PSScriptRoot '..\artifacts\BenchmarkResultPage'
if (!(Test-Path -PathType Container $ArtifactsDir)) { New-Item -ItemType Directory -Path $ArtifactsDir }
$ArtifactsDir = $ArtifactsDir | Resolve-Path
if (!(Test-Path -PathType Container $BenchmarkArtifactsDir)) { New-Item -ItemType Directory -Path $BenchmarkArtifactsDir }
$BenchmarkArtifactsDir = $BenchmarkArtifactsDir | Resolve-Path
if (!(Test-Path -PathType Container $BenchmarkResultPageDir)) { New-Item -ItemType Directory -Path $BenchmarkResultPageDir }
$BenchmarkResultPageDir = $BenchmarkResultPageDir | Resolve-Path
if (!$Env:GitVersion_NuGetVersionV2) {
    # local run
    if (!(& dotnet tool list | Out-String).Contains('gitversion.tool')) { dotnet tool restore }
    $gitver = (& dotnet gitversion -output json) | ConvertFrom-Json
    $Env:GitVersion_NuGetVersionV2 = $gitver.NuGetVersionV2;
}
