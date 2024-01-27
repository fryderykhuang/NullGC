using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NullGC.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NullGCAnalyzer : DiagnosticAnalyzer
{
    public const string BoxedDiagnosticId = "NGC10";
    public const string LinqStructBoxedDiagnosticId = "NGC11";
    public const string BoxingOnNotImplementedLinqOperationDiagnosticId = "NGC12";
    public const string ShouldBorrowDiagnosticId = "NGC20";

    private const string Category = "Performance";

    private static readonly LocalizableString NGC10_Title =
        new LocalizableResourceString(nameof(Resources.NGC10_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC10_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC10_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC10_Description =
        new LocalizableResourceString(nameof(Resources.NGC10_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC11_Title =
        new LocalizableResourceString(nameof(Resources.NGC11_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC11_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC11_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC11_Description =
        new LocalizableResourceString(nameof(Resources.NGC11_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC12_Title =
        new LocalizableResourceString(nameof(Resources.NGC12_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC12_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC12_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC12_Description =
        new LocalizableResourceString(nameof(Resources.NGC12_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC20_Title =
        new LocalizableResourceString(nameof(Resources.NGC20_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC20_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC20_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC20_Description =
        new LocalizableResourceString(nameof(Resources.NGC20_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly DiagnosticDescriptor Rule10 = new(BoxedDiagnosticId, NGC10_Title,
        NGC10_MessageFormat, Category, DiagnosticSeverity.Info, true, NGC10_Description);

    private static readonly DiagnosticDescriptor Rule11 = new(LinqStructBoxedDiagnosticId, NGC11_Title,
        NGC11_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC11_Description);

    private static readonly DiagnosticDescriptor Rule12 = new(BoxingOnNotImplementedLinqOperationDiagnosticId,
        NGC12_Title,
        NGC12_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC12_Description);

    private static readonly DiagnosticDescriptor Rule20 = new(ShouldBorrowDiagnosticId, NGC20_Title,
        NGC20_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC20_Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule10, Rule11, Rule12, Rule20);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext obj)
    {
        if (obj.Node is InvocationExpressionSyntax invocationSyntax)
        {
            var methodSymInfo = obj.SemanticModel.GetSymbolInfo(invocationSyntax);
            if (methodSymInfo.Symbol is IMethodSymbol ms)
            {
                for (var i = 0; i < ms.Parameters.Length; i++)
                {
                    var prmSymbol = ms.Parameters[i];
                    if (prmSymbol.RefKind == RefKind.None)
                        if (prmSymbol.Type.Interfaces.Any(x => x.MetadataName == "ISingleDisposable`1" &&
                                x.ContainingNamespace?.MetadataName == "NullGC" && (x.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace ?? false)
                                ))
                        {
                            var argSyntax = invocationSyntax.ArgumentList.Arguments[i];
                            if (argSyntax.Expression is InvocationExpressionSyntax invokeSyn)
                            {
                                var argSymInfo = obj.SemanticModel.GetSymbolInfo(invokeSyn);
                                if (argSymInfo.Symbol is IMethodSymbol argMs)
                                {
                                    if (argMs.ConstructedFrom.Name == "Borrow" && (argMs.ConstructedFrom.ContainingType?.Interfaces.Any(x => x.MetadataName == "ISingleDisposable`1" && x.ContainingNamespace?.MetadataName == "NullGC" && (x.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace ?? false)) ?? false))
                                    {
                                        continue;
                                    }
                                }
                            }

                            obj.ReportDiagnostic(Diagnostic.Create(Rule20,
                                argSyntax.GetLocation(), argSyntax.GetText()));
                        }
                }

                if (ms.IsExtensionMethod)
                    if (ms.ReceiverType is not null)
                        if (!ms.ReceiverType.IsValueType)
                            if (invocationSyntax.Expression is MemberAccessExpressionSyntax memberAccessSyntax)
                                if (memberAccessSyntax.Expression is InvocationExpressionSyntax leftInvocationSyntax)
                                {
                                    var leftInvocationSymbol = obj.SemanticModel.GetSymbolInfo(leftInvocationSyntax);
                                    if (leftInvocationSymbol.Symbol is IMethodSymbol lms)
                                        if (lms.ReturnType.IsValueType &&
                                            lms.ReturnType.FindImplementationForInterfaceMember(ms.ReceiverType) is
                                                null)
                                        {
                                            if (lms.ReturnType.Interfaces.Any(x =>
                                                    x.MetadataName == "ILinqEnumerable`2" &&
                                                    x.ContainingNamespace?.MetadataName == "Linq" &&
                                                    x.ContainingNamespace.ContainingNamespace?.MetadataName ==
                                                    "NullGC" && (x.ContainingNamespace.ContainingNamespace
                                                        .ContainingNamespace?.IsGlobalNamespace ?? false)))
                                            {
                                                if (ms.ReducedFrom?.ContainingType?.MetadataName == "Enumerable" &&
                                                    ms.ReducedFrom.ContainingType.ContainingNamespace?.MetadataName ==
                                                    "Linq" &&
                                                    ms.ReducedFrom.ContainingType.ContainingNamespace
                                                        .ContainingNamespace?.MetadataName == "System" &&
                                                    (ms.ReducedFrom.ContainingType.ContainingNamespace
                                                        .ContainingNamespace.ContainingNamespace?.IsGlobalNamespace ?? false))
                                                {
                                                    obj.ReportDiagnostic(Diagnostic.Create(Rule12,
                                                        invocationSyntax.GetLocation(), ImmutableDictionary.CreateRange(new[]
                                                        {
                                                            new KeyValuePair<string, string>("OperatorName", ms.Name),
                                                            new KeyValuePair<string, string>("InputSignature", lms.ReturnType.ToDisplayString()),
                                                            new KeyValuePair<string, string>("MethodSignature", ms.ToDisplayString()),
                                                        }), ms.MetadataName));
                                                }
                                                else
                                                {
                                                    obj.ReportDiagnostic(Diagnostic.Create(Rule11,
                                                        leftInvocationSyntax.GetLocation(),
                                                        lms.ReturnType.ToDisplayString(),
                                                        ms.ReceiverType.ToDisplayString()));
                                                }
                                            }
                                            else
                                            {
                                                obj.ReportDiagnostic(Diagnostic.Create(Rule10,
                                                    leftInvocationSyntax.GetLocation(),
                                                    lms.ReturnType.ToDisplayString(),
                                                    ms.ReceiverType.ToDisplayString()));
                                            }
                                        }
                                }
            }
        }
    }
}