using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Diagnostics;
using Microsoft.Build.Locator;
using Microsoft.Build.Logging.StructuredLogger;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;
using NullGC.Allocators;
using NullGC.Collections;
using NullGC.Linq;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;
using Task = System.Threading.Tasks.Task;
using VerifyCS = NullGC.Analyzer.Tests.CSharpCodeFixVerifier<
    NullGC.Analyzer.NullGCAnalyzer,
    NullGC.Analyzer.NullGCAnalyzerCodeFixProvider>;

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
            diags.Single(x => x.Id == NullGCAnalyzer.BoxingOnNotImplementedLinqOperationDiagnosticId);
            diags.Single(x => x.Id == NullGCAnalyzer.ShouldBorrowDiagnosticId);
        }
    }
}