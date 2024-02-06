using System.Collections.Immutable;
using System.Linq;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.Diagnostics;
using Task = System.Threading.Tasks.Task;

namespace NullGC.Analyzer.Tests
{
    [TestClass]
    public class NullGCAnalyzerUnitTest
    {
        [TestMethod]
        public async Task Test()
        {
            // if (!MSBuildLocator.IsRegistered)
            // {
            //     var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            //     MSBuildLocator.RegisterInstance(instances.OrderByDescending(x => x.Version).First());
            // }

            // MSBuildLocator.RegisterDefaults();
            // var workspace = MSBuildWorkspace.Create(new Dictionary<string, string>()
            //     {{"Configuration", "Debug"}, {"Platform", "AnyCPU"}});
            // workspace.SkipUnrecognizedProjects = true;
            // workspace.WorkspaceFailed += (sender, args) =>
            // {
            //     if (args.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
            //     {
            //         Console.Error.WriteLine(args.Diagnostic.Message);
            //     }
            // };

            // var solution =
            //     await workspace.OpenSolutionAsync(@"D:\projects\NullGC\src\NullGC.Analyzer.Tests.Solution.sln");
            // var project = solution.Projects.Single();

            var analyzerMgr = new AnalyzerManager();
            var prj = analyzerMgr.GetProject(
                @"..\..\..\..\NullGC.Analyzer.Tests.Project\NullGC.Analyzer.Tests.Project.csproj");
            AdhocWorkspace workspace = new AdhocWorkspace();
            var roslynProject = prj.AddToWorkspace(workspace);
            var compilation = await roslynProject.GetCompilationAsync();

            var diags = await compilation!
                .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new NullGCAnalyzer()))
                .GetAllDiagnosticsAsync();
            Assert.AreEqual(1, diags.Count(x => x.Id == NullGCAnalyzer.BoxingOnNotImplementedLinqOperationDiagnosticId));
            Assert.AreEqual(1, diags.Count(x => x.Id == NullGCAnalyzer.LinqStructWillBeBoxedDiagnosticId));
            Assert.AreEqual(1,  diags.Count(x => x.Id == NullGCAnalyzer.OwnershipShouldBeExplicitOnValuePassingParameterDiagnosticId));
            Assert.AreEqual(3,  diags.Count(x => x.Id == NullGCAnalyzer.PotentialDoubleFreeSituationSuggestExplicitOwnershipDiagnosticId));
            Assert.AreEqual(1,  diags.Count(x => x.Id == NullGCAnalyzer.ReturnPathsArePartialExplicitDiagnosticId));
        }
    }
}