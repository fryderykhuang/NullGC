using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;
using NullGC.Analyzer.Extensions;

namespace NullGC.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NullGCAnalyzer : DiagnosticAnalyzer
{
    public const string BoxedDiagnosticId = "NGC10";
    public const string LinqStructWillBeBoxedDiagnosticId = "NGC11";
    public const string BoxingOnNotImplementedLinqOperationDiagnosticId = "NGC12";
    public const string OwnershipShouldBeExplicitOnValuePassingParameterDiagnosticId = "NGC20";
    public const string PotentialDoubleFreeSituationSuggestExplicitOwnershipDiagnosticId = "NGC25";

    public const string ConflictOwnershipAttributesDiagnosticId = "NGC21";
    public const string OwnershipShouldBeExplicitOnRefOrOutParametersDiagnosticId = "NGC22";
    public const string OwnershipShouldBeExplicitOnReturnDiagnosticId = "NGC23";

    public const string TargetOwnershipKindIsIncompatibleWithSourceOwnershipKindDiagnosticId = "NGC24";

    public const string ReturnPathsArePartialExplicitDiagnosticId = "NGC26";
    public const string ReturnPathsAreExplicitButDifferentDiagnosticId = "NGC27";

    // public const string OwnershipTransferredAndLost = "NGC21";
    // public const string CannotTakeOwnershipFromBorrowedValue = "NGC22";

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

    private static readonly LocalizableString NGC21_Title =
        new LocalizableResourceString(nameof(Resources.NGC21_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC21_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC21_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC21_Description =
        new LocalizableResourceString(nameof(Resources.NGC21_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC22_Title =
        new LocalizableResourceString(nameof(Resources.NGC22_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC22_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC22_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC22_Description =
        new LocalizableResourceString(nameof(Resources.NGC22_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC23_Title =
        new LocalizableResourceString(nameof(Resources.NGC23_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC23_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC23_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC23_Description =
        new LocalizableResourceString(nameof(Resources.NGC23_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC24_Title =
        new LocalizableResourceString(nameof(Resources.NGC24_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC24_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC24_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC24_Description =
        new LocalizableResourceString(nameof(Resources.NGC24_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC25_Title =
        new LocalizableResourceString(nameof(Resources.NGC25_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC25_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC25_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC25_Description =
        new LocalizableResourceString(nameof(Resources.NGC25_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC26_Title =
        new LocalizableResourceString(nameof(Resources.NGC26_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString NGC26_MessageFormat =
        new LocalizableResourceString(nameof(Resources.NGC26_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString NGC26_Description =
        new LocalizableResourceString(nameof(Resources.NGC26_Description), Resources.ResourceManager,
            typeof(Resources));

    private static readonly DiagnosticDescriptor Rule10 = new(BoxedDiagnosticId, NGC10_Title,
        NGC10_MessageFormat, Category, DiagnosticSeverity.Info, true, NGC10_Description);

    private static readonly DiagnosticDescriptor Rule11 = new(LinqStructWillBeBoxedDiagnosticId, NGC11_Title,
        NGC11_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC11_Description);

    private static readonly DiagnosticDescriptor Rule12 = new(BoxingOnNotImplementedLinqOperationDiagnosticId,
        NGC12_Title,
        NGC12_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC12_Description);

    private static readonly DiagnosticDescriptor Rule20 = new(
        OwnershipShouldBeExplicitOnValuePassingParameterDiagnosticId,
        NGC20_Title,
        NGC20_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC20_Description);

    private static readonly DiagnosticDescriptor Rule21 = new(ConflictOwnershipAttributesDiagnosticId, NGC21_Title,
        NGC21_MessageFormat, Category, DiagnosticSeverity.Error, true, NGC21_Description);

    private static readonly DiagnosticDescriptor Rule22 = new(OwnershipShouldBeExplicitOnRefOrOutParametersDiagnosticId,
        NGC22_Title,
        NGC22_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC22_Description);

    private static readonly DiagnosticDescriptor Rule23 = new(OwnershipShouldBeExplicitOnReturnDiagnosticId,
        NGC23_Title,
        NGC23_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC23_Description);

    private static readonly DiagnosticDescriptor Rule24 = new(
        TargetOwnershipKindIsIncompatibleWithSourceOwnershipKindDiagnosticId, NGC24_Title,
        NGC24_MessageFormat, Category, DiagnosticSeverity.Error, true, NGC24_Description);

    private static readonly DiagnosticDescriptor Rule25 = new(
        PotentialDoubleFreeSituationSuggestExplicitOwnershipDiagnosticId, NGC25_Title,
        NGC25_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC25_Description);

    private static readonly DiagnosticDescriptor Rule26 = new(
        ReturnPathsArePartialExplicitDiagnosticId, NGC26_Title,
        NGC26_MessageFormat, Category, DiagnosticSeverity.Warning, true, NGC26_Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule10, Rule11, Rule12, Rule20, Rule21, Rule22, Rule23, Rule24, Rule25, Rule26);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeMethodInvocationOrPropertyGetterOperations, OperationKind.Invocation);
        context.RegisterOperationAction(AnalyzeSimpleAssignmentOperations, OperationKind.SimpleAssignment);
        //context.RegisterOperationAction(AnalyzeSimpleAssignmentOperations, OperationKind.creat);

        // // context.RegisterOperationAction(AnalyzeAssignmentOperations, OperationKind.SimpleAssignment);
        // context.RegisterOperationAction(AnalyzeReferenceOperations, OperationKind.FieldReference);
        //
        // // Borrow/Take/Attribute annotated method calls
        // context.RegisterSyntaxNodeAction(AnalyzeInvocationSyntaxNodes, SyntaxKind.InvocationExpression);
        //
        // // Property invocation. https://stackoverflow.com/questions/21841262/find-property-invocations-with-roslyn
        // context.RegisterSyntaxNodeAction(AnalyzePossiblePropertySyntaxNodes, SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression);
        //
        //
        // context.RegisterSyntaxNodeAction(AnalyzeReturnStatementSyntaxNodes, SyntaxKind.IdentifierName, SyntaxKind.ReturnStatement);
        //
        // context.RegisterSyntaxNodeAction(AnalyzeMethods, SyntaxKind.MethodDeclaration,
        //     SyntaxKind.AnonymousMethodExpression);


        // context.RegisterSyntaxNodeAction(AnalyzeAssignmentSyntaxNode, SyntaxKind.AddAssignmentExpression,
        //     SyntaxKind.AndAssignmentExpression, SyntaxKind.CoalesceAssignmentExpression,
        //     SyntaxKind.DivideAssignmentExpression, SyntaxKind.ModuloAssignmentExpression,
        //     SyntaxKind.MultiplyAssignmentExpression, SyntaxKind.OrAssignmentExpression,
        //     SyntaxKind.SimpleAssignmentExpression, SyntaxKind.SubtractAssignmentExpression,
        //     SyntaxKind.ExclusiveOrAssignmentExpression, SyntaxKind.LeftShiftAssignmentExpression,
        //     SyntaxKind.RightShiftAssignmentExpression, SyntaxKind.UnsignedRightShiftAssignmentExpression);

        // context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    private void AnalyzeSimpleAssignmentOperations(OperationAnalysisContext context)
    {
        var sm = context.Operation.SemanticModel;
        if (sm is null) return;
        if (context.Operation is ISimpleAssignmentOperation simpleAssignOp)
        {
            var targetSym = sm.GetSymbolInfo(simpleAssignOp.Target.Syntax).Symbol;
            if (targetSym is not IParameterSymbol paramSym) return;
            if (paramSym.RefKind is not RefKind.Ref and not RefKind.Out) return;
            var valueTypeSym = simpleAssignOp.Value.Type;
            if (!valueTypeSym.IsExplicitLifetime()) return;
            var valueOwnershipKind = valueTypeSym.GetOwnershipKindOnReturnValue(sm);
            if (valueOwnershipKind == ExplicitOwnershipKind.Conflict)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule21, simpleAssignOp.Value.Syntax.GetLocation()));
                return;
            }

            if (valueOwnershipKind == ExplicitOwnershipKind.Unspecified)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule22, simpleAssignOp.Value.Syntax.GetLocation(),
                    simpleAssignOp.Value.Syntax.GetText()));
            }
        }
        
        // TODO when assigning to field or property
    }

    // private void AnalyzeMethods(SyntaxNodeAnalysisContext obj)
    // {
    //     throw new System.NotImplementedException();
    // }
    //
    // private void AnalyzeReferenceOperations(OperationAnalysisContext context)
    // {
    //     if (context.Operation is IFieldReferenceOperation fieldRefOp)
    //     {
    //         fieldRefOp.
    //     }
    //     else if (context.Operation is IObjectCreationOperation objCreationOp)
    //     {
    //         if (objCreationOp.Type.IsExplicitLifetime())
    //         {
    //             
    //         }
    //     }
    // }

    // private void AnalyzeAssignmentOperations(OperationAnalysisContext context)
    // {
    //     if (context.Operation is not ISimpleAssignmentOperation assignOp) return;
    //     assignOp
    // }
    //
    // private void AnalyzeReturnStatementSyntaxNodes(SyntaxNodeAnalysisContext context)
    // {
    //     if (context.Node is not ReturnStatementSyntax returnSyn) return;
    //     returnSyn
    // }
    //
    // private void AnalyzePossiblePropertySyntaxNodes(SyntaxNodeAnalysisContext obj)
    // { 
    //     
    // }

    // // 
    // private void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext obj)
    // {
    //     // TODO cleanup removed nodes
    // }
    //
    // private ConcurrentDictionary<SyntaxNode, bool> _activeNodes = new ConcurrentDictionary<SyntaxNode, bool>();
    //
    // private ConcurrentDictionary<SyntaxNode, SyntaxNodeProperties> _nodeProperties =
    //     new ConcurrentDictionary<SyntaxNode, SyntaxNodeProperties>();
    //
    // private interface INode
    // {
    //     INode Upstream { get; set; }
    //     IReadOnlyCollection<INode> DownStreams { get; }
    //
    //     void AddDownstream(INode node);
    // }
    //
    // private enum OwnershipState
    // {
    //     Owned,
    //     Moved,
    //     Borrowed
    // }

    private static readonly Regex ReadOnlyCommentRegex = new(@"/\*\s*ReadOnly\s*\*/", RegexOptions.Compiled);

    private void AnalyzeMethodInvocationOrPropertyGetterOperations(OperationAnalysisContext context)
    {
        if (context.Operation is not IInvocationOperation invocationOp) return;
        var sm = context.Operation.SemanticModel;
        if (sm is null) return;

        if (invocationOp.Syntax is InvocationExpressionSyntax invocationSyn)
        {
            var invocationSym = sm.GetSymbolInfo(invocationSyn).Symbol;
            if (invocationSym is IMethodSymbol ms)
            {
                if (ms.IsExtensionMethod)
                {
                    if (ms.ReceiverType is not null)
                    {
                        if (!ms.ReceiverType.IsValueType)
                        {
                            if (invocationSyn.Expression is MemberAccessExpressionSyntax memberAccessSyn)
                            {
                                ITypeSymbol? returnTypeSym = null;
                                Location? lloc = null;
                                if (memberAccessSyn.Expression is MemberAccessExpressionSyntax leftMemberAccessSyn)
                                {
                                    lloc = leftMemberAccessSyn.Name.GetLocation();
                                    var leftMemberAccessSym = sm.GetSymbolInfo(leftMemberAccessSyn).Symbol;
                                    if (leftMemberAccessSym is IPropertySymbol lps)
                                    {
                                        returnTypeSym = lps.Type;
                                    }
                                    else if (leftMemberAccessSym is IFieldSymbol lfs)
                                    {
                                        returnTypeSym = lfs.Type;
                                    }
                                }
                                else if (memberAccessSyn.Expression is InvocationExpressionSyntax leftInvocationSyn)
                                {
                                    var leftInvocationSym = sm.GetSymbolInfo(leftInvocationSyn).Symbol;
                                    if (leftInvocationSym is IMethodSymbol lms)
                                    {
                                        returnTypeSym = lms.ReturnType;
                                        if (leftInvocationSyn.Expression is MemberAccessExpressionSyntax lmaSyn)
                                        {
                                            lloc = lmaSyn.Name.GetLocation();
                                        }
                                        else
                                        {
                                            lloc = leftInvocationSyn.GetLocation();
                                        }
                                    }
                                }
                                else if (memberAccessSyn.Expression is ObjectCreationExpressionSyntax objCreateSyn)
                                {
                                    var objCreateSym = sm.GetSymbolInfo(objCreateSyn).Symbol;
                                    if (objCreateSym is IMethodSymbol lms)
                                    {
                                        returnTypeSym = lms.ReturnType;
                                        lloc = objCreateSyn.Type.GetLocation();
                                    }
                                }

                                if (returnTypeSym is not null && lloc is not null)
                                {
                                    if (returnTypeSym.IsValueType &&
                                        returnTypeSym.FindImplementationForInterfaceMember(ms.ReceiverType) is null)
                                    {
                                        if (returnTypeSym.AllInterfaces.Any(x =>
                                                x.MetadataName == "ILinqEnumerable`2" &&
                                                x.ContainingNamespace.IsNullGCLinq()))
                                        {
                                            if (ms.ContainingType.MetadataName == nameof(Enumerable) &&
                                                ms.ContainingType.ContainingNamespace.IsSystemLinq())
                                            {
                                                context.ReportDiagnostic(Diagnostic.Create(Rule12,
                                                    memberAccessSyn.Name.GetLocation(), ImmutableDictionary.CreateRange(
                                                        new[]
                                                        {
                                                            new KeyValuePair<string, string>("OperatorName", ms.Name),
                                                            new KeyValuePair<string, string>("InputSignature",
                                                                returnTypeSym.ToDisplayString()),
                                                            new KeyValuePair<string, string>("MethodSignature",
                                                                ms.ToDisplayString()),
                                                        })!, ms.MetadataName));
                                            }
                                            else
                                            {
                                                context.ReportDiagnostic(Diagnostic.Create(Rule11,
                                                    lloc,
                                                    returnTypeSym.ToDisplayString(),
                                                    ms.ReceiverType.ToDisplayString()));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            for (var i = 0; i < invocationOp.Arguments.Length; i++)
            {
                var argOp = invocationOp.Arguments[i];
                var paramSym = argOp.Parameter;
                if (paramSym is null)
                    continue;
                var trivia = invocationSyn.FindTrivia(argOp.Syntax.FullSpan.Start - 1);
                if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
                    if (ReadOnlyCommentRegex.IsMatch(trivia.ToString()))
                        continue;

                if (paramSym.RefKind == RefKind.None)
                {
                    bool paramNotDisposable = false;
                    ExplicitOwnershipKind valueOwnershipKind;
                    if (!argOp.Value.Type.IsExplicitLifetime())
                    {
                        if (argOp.Value is IConversionOperation convOp)
                        {
                            if (!convOp.Operand.Type.IsExplicitLifetime())
                                continue;
                            paramNotDisposable = !convOp.Type.IsIDisposable();
                            valueOwnershipKind = sm.GetSymbolInfo(convOp.Operand.Syntax).Symbol
                                .GetOwnershipKindOnReturnValue(sm);
                        }
                        else
                            continue;
                    }
                    else
                    {
                        valueOwnershipKind = sm.GetSymbolInfo(argOp.Value.Syntax).Symbol
                            .GetOwnershipKindOnReturnValue(sm);
                    }

                    var isReadOnlyBehavior = paramSym.IsReadOnlyBehavior();
                    if (valueOwnershipKind == ExplicitOwnershipKind.Conflict)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule21, argOp.Value.Syntax.GetLocation()));
                    }
                    else if (valueOwnershipKind is ExplicitOwnershipKind.Borrowed or ExplicitOwnershipKind.Owned)
                    {
                    }
                    else if (valueOwnershipKind == ExplicitOwnershipKind.ExplicitButDifferent)
                    {
                        //TODO report as information
                    }
                    else
                    {
                        if (isReadOnlyBehavior)
                        {
                        }
                        else
                        {
                            if (valueOwnershipKind == ExplicitOwnershipKind.PartialExplicit)
                            {
                                context.ReportDiagnostic(Diagnostic.Create(Rule26, argOp.Value.Syntax.GetLocation()));
                            }

                            if (paramNotDisposable)
                            {
                                if (!invocationOp.TargetMethod.ContainingNamespace.IsSystemLinq())
                                {
                                    context.ReportDiagnostic(Diagnostic.Create(Rule25, argOp.Value.Syntax.GetLocation(),
                                        argOp.Value.Syntax.GetText()));
                                }
                            }
                            else
                            {
                                context.ReportDiagnostic(Diagnostic.Create(Rule20, argOp.Value.Syntax.GetLocation(),
                                    argOp.Value.Syntax.GetText()));
                            }
                        }
                    }
                }
            }
        }
    }


    // private SyntaxAnnotation _borrowedAnnotation = new("Borrowed");

    // private void AnalyzeInvocationSyntaxNodes(SyntaxNodeAnalysisContext context)
    // {
    //     if (context.Node is InvocationExpressionSyntax invocationSyntax)
    //     {
    //         
    //         
    //         var methodSymInfo = context.SemanticModel.GetSymbolInfo(invocationSyntax);
    //         if (methodSymInfo.Symbol is IMethodSymbol ms)
    //         {
    //
    //             // Analyze parameter passing.
    //             for (var i = 0; i < ms.Parameters.Length; i++)
    //             {
    //                 var prmSymbol = ms.Parameters[i];
    //                 if (prmSymbol.RefKind == RefKind.None)
    //                     if (prmSymbol.Type.Interfaces.Any(x => x.MetadataName == "ISingleDisposable`1" &&
    //                             x.ContainingNamespace?.MetadataName == "NullGC" && (x.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace ?? false)
    //                             ))
    //                     {
    //                         var argSyntax = invocationSyntax.ArgumentList.Arguments[i];
    //                         if (argSyntax.HasAnnotation(_borrowedAnnotation))
    //                         {
    //                             continue;
    //                         }
    //                         else if (argSyntax.Expression is InvocationExpressionSyntax invokeSyn)
    //                         {
    //                             var argSymInfo = context.SemanticModel.GetSymbolInfo(invokeSyn);
    //                             if (argSymInfo.Symbol is IMethodSymbol argMs)
    //                             {
    //                                 if (argMs.ConstructedFrom.Name == "Borrow" && (argMs.ConstructedFrom.ContainingType?.Interfaces.Any(x => x.MetadataName == "ISingleDisposable`1" && x.ContainingNamespace.IsNullGC()) ?? false))
    //                                 {
    //                                     continue;
    //                                 }
    //                             }
    //                         }
    //
    //                         context.ReportDiagnostic(Diagnostic.Create(Rule20,
    //                             argSyntax.GetLocation(), argSyntax.GetText()));
    //                     }
    //             }
    //
    //             if (ms.IsExtensionMethod)
    //                 if (ms.ReceiverType is not null)
    //                     if (!ms.ReceiverType.IsValueType)
    //                         if (invocationSyntax.Expression is MemberAccessExpressionSyntax memberAccessSyntax)
    //                             if (memberAccessSyntax.Expression is InvocationExpressionSyntax leftInvocationSyntax)
    //                             {
    //                                 var leftInvocationSymbol = context.SemanticModel.GetSymbolInfo(leftInvocationSyntax);
    //                                 if (leftInvocationSymbol.Symbol is IMethodSymbol lms)
    //                                     if (lms.ReturnType.IsValueType &&
    //                                         lms.ReturnType.FindImplementationForInterfaceMember(ms.ReceiverType) is
    //                                             null)
    //                                     {
    //                                         if (lms.ReturnType.Interfaces.Any(x =>
    //                                                 x.MetadataName == "ILinqEnumerable`2" &&
    //                                                 x.ContainingNamespace?.MetadataName == "Linq" &&
    //                                                 x.ContainingNamespace.ContainingNamespace?.MetadataName ==
    //                                                 "NullGC" && (x.ContainingNamespace.ContainingNamespace
    //                                                     .ContainingNamespace?.IsGlobalNamespace ?? false)))
    //                                         {
    //                                             if (ms.ReducedFrom?.ContainingType?.MetadataName == "Enumerable" &&
    //                                                 ms.ReducedFrom.ContainingType.ContainingNamespace?.MetadataName ==
    //                                                 "Linq" &&
    //                                                 ms.ReducedFrom.ContainingType.ContainingNamespace
    //                                                     .ContainingNamespace?.MetadataName == "System" &&
    //                                                 (ms.ReducedFrom.ContainingType.ContainingNamespace
    //                                                     .ContainingNamespace.ContainingNamespace?.IsGlobalNamespace ?? false))
    //                                             {
    //                                                 context.ReportDiagnostic(Diagnostic.Create(Rule12,
    //                                                     invocationSyntax.GetLocation(), ImmutableDictionary.CreateRange(new[]
    //                                                     {
    //                                                         new KeyValuePair<string, string>("OperatorName", ms.Name),
    //                                                         new KeyValuePair<string, string>("InputSignature", lms.ReturnType.ToDisplayString()),
    //                                                         new KeyValuePair<string, string>("MethodSignature", ms.ToDisplayString()),
    //                                                     }), ms.MetadataName));
    //                                             }
    //                                             else
    //                                             {
    //                                                 context.ReportDiagnostic(Diagnostic.Create(Rule11,
    //                                                     leftInvocationSyntax.GetLocation(),
    //                                                     lms.ReturnType.ToDisplayString(),
    //                                                     ms.ReceiverType.ToDisplayString()));
    //                                             }
    //                                         }
    //                                         else
    //                                         {
    //                                             context.ReportDiagnostic(Diagnostic.Create(Rule10,
    //                                                 leftInvocationSyntax.GetLocation(),
    //                                                 lms.ReturnType.ToDisplayString(),
    //                                                 ms.ReceiverType.ToDisplayString()));
    //                                         }
    //                                     }
    //                             }
    //         }
    //     }
    // }
}