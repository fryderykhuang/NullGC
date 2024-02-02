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
        ImmutableArray.Create(NullGCAnalyzer.BoxingOnNotImplementedLinqOperationDiagnosticId,
            NullGCAnalyzer.OwnershipShouldBeExplicitOnValuePassingParameterDiagnosticId,
            NullGCAnalyzer.OwnershipShouldBeExplicitOnRefOrOutParametersDiagnosticId,
            NullGCAnalyzer.PotentialDoubleFreeSituationSuggestExplicitOwnershipDiagnosticId);

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
            else if (diag.Id is NullGCAnalyzer.OwnershipShouldBeExplicitOnValuePassingParameterDiagnosticId
                     or NullGCAnalyzer.OwnershipShouldBeExplicitOnRefOrOutParametersDiagnosticId
                     or NullGCAnalyzer.PotentialDoubleFreeSituationSuggestExplicitOwnershipDiagnosticId)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        CodeFixResources.AddBorrow_Title,
                        c => AddCallAsync(context, diag, "Borrow", c),
                        nameof(CodeFixResources.AddBorrow_Title)),
                    diag);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        CodeFixResources.AddTake_Title,
                        c => AddCallAsync(context, diag, "Take", c),
                        nameof(CodeFixResources.AddTake_Title)),
                    diag);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        CodeFixResources.SuppressByComment_Title,
                        c => AddLeadingCommentAsync(context, diag, "/*ReadOnly*/", c),
                        nameof(CodeFixResources.SuppressByComment_Title)),
                    diag);
            }

        return Task.CompletedTask;
    }

    private async Task<Document> AddLeadingCommentAsync(CodeFixContext context, Diagnostic diag, string comment, CancellationToken cancellationToken)
    {
        var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var node = root?.FindNode(diag.Location.SourceSpan);
        if (node is not ArgumentSyntax)
        {
            node = node?.Parent;
        }

        if (node is ArgumentSyntax argSyntax)
        {
            var updatedArgSyntax = argSyntax.WithLeadingTrivia(SyntaxFactory.Comment(comment));
            return context.Document.WithSyntaxRoot(root.ReplaceNode(argSyntax, updatedArgSyntax));
        }

        return context.Document;

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

    private async Task<Document> AddCallAsync(CodeFixContext context, Diagnostic diag, string methodName,
        CancellationToken cancellationToken)
    {
        var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) return context.Document;
        var node = root.FindNode(diag.Location.SourceSpan);
        if (node is ExpressionSyntax exprSyn)
        {
            // TODO deal with explicit interface implementation
            var updatedNode = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, exprSyn,
                    SyntaxFactory.IdentifierName(methodName)));
            return context.Document.WithSyntaxRoot(root.ReplaceNode(node, updatedNode));
        }
        else if (node is ArgumentSyntax argSyn)
        {
            // TODO deal with explicit interface implementation
            var updatedNode = SyntaxFactory.Argument(SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, argSyn.Expression,
                    SyntaxFactory.IdentifierName(methodName))));
            return context.Document.WithSyntaxRoot(root.ReplaceNode(node, updatedNode));
        }

        return context.Document;
    }
}