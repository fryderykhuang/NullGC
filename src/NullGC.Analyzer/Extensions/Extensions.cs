using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NullGC.Analyzer.Extensions;

internal static class Extensions
{
    public static bool IsNullGC(this INamespaceSymbol? ns)
    {
        if (ns is null) return false;
        return ns.MetadataName == "NullGC" && (ns.ContainingNamespace?.IsGlobalNamespace ?? false);
    }

    public static bool IsNullGCLinq(this INamespaceSymbol? ns)
    {
        if (ns is null) return false;
        return ns.MetadataName == "Linq" && ns.ContainingNamespace?.MetadataName == "NullGC" &&
               (ns.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace ?? false);
    }

    public static bool IsSystemLinq(this INamespaceSymbol? ns)
    {
        if (ns is null) return false;
        return ns.MetadataName == nameof(System.Linq) && ns.ContainingNamespace?.MetadataName == nameof(System) &&
               (ns.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace ?? false);
    }

    public static bool IsExplicitLifetime(this ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null) return false;
        return typeSymbol.IsValueType && typeSymbol.AllInterfaces.Any(x =>
            x.MetadataName == "IExplicitOwnership`1" && x.ContainingNamespace.IsNullGC());
    }

    private static bool IsBorrowedAttribute(this INamedTypeSymbol? symbol)
    {
        if (symbol is null) return false;
        return symbol.MetadataName == "BorrowedAttribute" && symbol.ContainingNamespace.IsNullGC();
    }

    private static bool IsOwnedAttribute(this INamedTypeSymbol? symbol)
    {
        if (symbol is null) return false;
        return symbol.MetadataName == "OwnedAttribute" && symbol.ContainingNamespace.IsNullGC();
    }

    private static bool IsReadOnlyAttribute(this INamedTypeSymbol? symbol)
    {
        if (symbol is null) return false;
        return symbol.MetadataName == "ReadOnlyAttribute" && symbol.ContainingNamespace.IsNullGC();
    }

    public static bool IsIDisposable(this ISymbol? symbol)
    {
        if (symbol is null) return false;
        return symbol.MetadataName == nameof(IDisposable) &&
               symbol.ContainingNamespace?.MetadataName == nameof(System) &&
               (symbol.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace ?? false);
    }

    public static bool IsReadOnlyBehavior(this ISymbol? symbol)
    {
        if (symbol is null) return false;

        foreach (var data in symbol.GetAttributes())
        {
            if (data.AttributeClass.IsReadOnlyAttribute())
                return true;
        }

        return false;
    }

    private static ExplicitOwnershipKind GetOwnershipKindFromAttributes(ImmutableArray<AttributeData> attributes)
    {
        ExplicitOwnershipKind ret = ExplicitOwnershipKind.Unspecified;
        foreach (var data in attributes)
        {
            if (data.AttributeClass.IsBorrowedAttribute())
            {
                if (ret != ExplicitOwnershipKind.Unspecified)
                    return ExplicitOwnershipKind.Conflict;
                ret = ExplicitOwnershipKind.Borrowed;
            }
            else if (data.AttributeClass.IsOwnedAttribute())
            {
                if (ret != ExplicitOwnershipKind.Unspecified)
                    return ExplicitOwnershipKind.Conflict;
                ret = ExplicitOwnershipKind.Owned;
            }
        }

        return ret;
    }


    private static ExplicitOwnershipKind GetOwnershipKindCore(ISymbol? symbol, out IMethodSymbol? origMethodSym,
        out bool inConclusive)
    {
        ExplicitOwnershipKind ret = ExplicitOwnershipKind.Unspecified;
        var methodSymbol = symbol as IMethodSymbol;
        if (symbol is IPropertySymbol propSym)
        {
            methodSymbol = propSym.GetMethod;
        }

        if (methodSymbol is null)
        {
            inConclusive = false;
            origMethodSym = null;
            return ExplicitOwnershipKind.Unspecified;
        }

        origMethodSym = methodSymbol;

        while (methodSymbol is not null)
        {
            ret = GetOwnershipKindFromAttributes(methodSymbol.GetReturnTypeAttributes());
            if (ret != ExplicitOwnershipKind.Unspecified)
            {
                inConclusive = false;
                return ret;
            }

            methodSymbol = methodSymbol.OverriddenMethod;
        }

        var typeSym = origMethodSym.ContainingType;
        if (typeSym is null)
        {
            inConclusive = false;
            return ret;
        }

        foreach (var interfaceSym in typeSym.AllInterfaces)
        {
            foreach (var memberSym in interfaceSym.GetMembers(origMethodSym.Name).OfType<IMethodSymbol>())
            {
                if (SymbolEqualityComparer.Default.Equals(
                        typeSym.FindImplementationForInterfaceMember(memberSym), origMethodSym))
                {
                    ret = GetOwnershipKindFromAttributes(memberSym.GetReturnTypeAttributes());
                    if (ret != ExplicitOwnershipKind.Unspecified)
                    {
                        inConclusive = false;
                        return ret;
                    }
                }
            }
        }

        inConclusive = true;
        return ret;
    }

    public static ExplicitOwnershipKind GetOwnershipKindOnReturnValue(this ISymbol? symbol, SemanticModel sm)
    {
        var ret = GetOwnershipKindCore(symbol, out var methodSym, out var inConclusive);

        if (!inConclusive) return ret;

        if (methodSym is null) return ExplicitOwnershipKind.Unspecified;

        foreach (var syntaxReference in methodSym.DeclaringSyntaxReferences)
        {
            var methodSyntax = syntaxReference.GetSyntax();
            BlockSyntax body;
            if (methodSyntax is MethodDeclarationSyntax {Body: not null} methodDeclaration)
            {
                body = methodDeclaration.Body;
            }
            else if (methodSyntax is AccessorDeclarationSyntax {Body: not null} accessorDeclarationSyn)
            {
                body = accessorDeclarationSyn.Body;
            }
            else
            {
                continue;
            }

            int ownedCount = 0, borrowedCount = 0, conflictCount = 0, unspecCount = 0;
            foreach (var returnSyn in body.DescendantNodesAndSelf()
                         .OfType<ReturnStatementSyntax>().Select(x => x.Expression).Where(x => x is not null))
            {
                var returnSym = sm.GetSymbolInfo(returnSyn!).Symbol;
                if (returnSym is IMethodSymbol ms)
                {
                    switch (GetOwnershipKindCore(ms, out _, out inConclusive))
                    {
                        case ExplicitOwnershipKind.Unspecified:
                            unspecCount++;
                            break;
                        case ExplicitOwnershipKind.Borrowed:
                            borrowedCount++;
                            break;
                        case ExplicitOwnershipKind.Owned:
                            ownedCount++;
                            break;
                        case ExplicitOwnershipKind.Conflict:
                            conflictCount++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    unspecCount++;
                }
            }

            if (conflictCount > 0) return ExplicitOwnershipKind.Conflict;
            var total = ownedCount + borrowedCount + conflictCount + unspecCount;
            if (total == 0 || unspecCount == total) return ExplicitOwnershipKind.Unspecified;
            else if (ownedCount == total) return ExplicitOwnershipKind.Owned;
            else if (borrowedCount == total) return ExplicitOwnershipKind.Borrowed;
            else if (unspecCount > 0) return ExplicitOwnershipKind.PartialExplicit;
            else return ExplicitOwnershipKind.ExplicitButDifferent;
        }

        return ret;
    }
}