using System;

namespace LanceSystem.SimpleFuzzySearch;

public sealed record SearchScore(double Exact, double Prefix, double TokenSubstring, double Fuzzy) : IComparable<SearchScore>
{
    public double Combined =>
        Exact * 1.00 +
        Prefix * 0.80 +
        TokenSubstring * 0.60 +
        Fuzzy * 0.40;
    public int CompareTo(SearchScore? other)
    {
        if (other is null)
            return 1;

        return Combined.CompareTo(other.Combined);
    }
}
