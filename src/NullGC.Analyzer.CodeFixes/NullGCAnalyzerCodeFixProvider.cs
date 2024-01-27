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

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
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
                return;
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

    }

    private Task<Solution> ReportAsync(CodeFixContext context, Diagnostic diag, CancellationToken c)
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

        Process.Start(@$"https://github.com/fryderykhuang/NullGC/issues/new?template=linq-operation-not-implemented.md&title={WebUtility.UrlEncode(title)}&body={WebUtility.UrlEncode(body)}");
        return Task.FromResult(context.Document.Project.Solution);
    }

    private async Task<Solution> AddCallToBorrowAsync(CodeFixContext context, Diagnostic diag, CancellationToken cancellationToken)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var node = root.FindNode(diag.Location.SourceSpan);

        if (node is ArgumentSyntax argSyntax)
        {
            var updatedArgSyntax = SyntaxFactory.Argument(SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, argSyntax.Expression, SyntaxFactory.IdentifierName("Borrow"))));
            return context.Document.WithSyntaxRoot(root.ReplaceNode(argSyntax, updatedArgSyntax)).Project.Solution;
        }

        return context.Document.Project.Solution;

        //var sm = await context.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        //if (sm != null)
        //{
        //    var syminfo = sm.GetSymbolInfo(node, cancellationToken);
        //    if (syminfo.Symbol is IParameterSymbol prmSym)
        //    {
        //        prmSym.
        //    }

        //}

        //// Compute new uppercase name.
        //var identifierToken = typeDecl.Identifier;
        //var newName = identifierToken.Text.ToUpperInvariant();

        //// Get the symbol representing the type to be renamed.
        //var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        //var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

        //// Produce a new solution that has all references to that type renamed, including the declaration.
        //var originalSolution = document.Project.Solution;
        //var optionSet = originalSolution.Workspace.Options;
        //var newSolution = await Renamer
        //    .RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken)
        //    .ConfigureAwait(false);

        //// Return the new solution with the now-uppercase type name.
        //return newSolution;
    }
}