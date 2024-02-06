namespace NullGC.Analyzer;

public enum ExplicitOwnershipKind
{
    Unspecified = 0,
    Borrowed = 1,
    Owned = 2,
    ExplicitButDifferent = 3,
    PartialExplicit = 4,
    Conflict = -1
}