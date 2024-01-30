using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace NullGC.Analyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NullGCAnalyzerCodeFixProvider))]
[Shared]
public class NullGCAnalyzerCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(NullGCAnalyzer.BoxingOnNotImplementedLinqOperationDiagnosticId, NullGCAnalyzer.ShouldBorrowDiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider()
    {
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        foreach (var diag in context.Diagnostics)
            if (diag.Id == NullGCAnalyzer.BoxingOnNotImplementedLinqOperationDiagnosticId)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        CodeFixResources.NGC12_Title,
                        c => ReportAsync(context, diag, c),
                        nameof(CodeFixResources.NGC12_Title)),
                    diag);
            }
            else if (diag.Id == NullGCAnalyzer.ShouldBorrowDiagnosticId)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        CodeFixResources.NGC20_Title,
                        c => AddCallToBorrowAsync(context, diag, c),
                        nameof(CodeFixResources.NGC20_Title)),
                    diag);
            }

        return Task.CompletedTask;
    }

    private async Task<Solution> ReportAsync(CodeFixContext context, Diagnostic diag, CancellationToken c)
    {
        var title = $"[LINQ] Linq operator '{diag.Properties["OperatorName"]}' is not implemented.";
        var body = @$"InputSignature:
```csharp
{diag.Properties["InputSignature"]}
```
MethodSignature:
```csharp
{diag.Properties["MethodSignature"]}
```
Comment:
";

        var url =
            @$"https://github.com/fryderykhuang/NullGC/issues/new?template=linq-operation-not-implemented.md&title={WebUtility.UrlEncode(title)}&body={WebUtility.UrlEncode(body)}";
        // Duplicate execution in VS, error in Rider
        // try
        // {
        //     Process.Start(url);
        // }
        // catch
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var node = root?.FindNode(diag.Location.SourceSpan);
            if (node is not null)
            {
                return context.Document.WithSyntaxRoot(root.ReplaceNode(node,
                        node.WithLeadingTrivia(node.GetLeadingTrivia()
                            .Add(SyntaxFactory.Comment(@$"/*
Click to submit a feature request:
{url}
*/"))))).Project
                    .Solution;
            }
        }

        return context.Document.Project.Solution;
    }

    private async Task<Solution> AddCallToBorrowAsync(CodeFixContext context, Diagnostic diag, CancellationToken cancellationToken)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root?.FindNode(diag.Location.SourceSpan) is ArgumentSyntax argSyntax)
        {
            var updatedArgSyntax = SyntaxFactory.Argument(SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, argSyntax.Expression,
                    SyntaxFactory.IdentifierName("Borrow"))));
            return context.Document.WithSyntaxRoot(root.ReplaceNode(argSyntax, updatedArgSyntax)).Project.Solution;
        }

        return context.Document.Project.Solution;
    }
}