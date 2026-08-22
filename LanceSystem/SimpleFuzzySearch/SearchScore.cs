namespace LanceSystem.SimpleFuzzySearch;

public sealed record SearchScore(double Exact, double Prefix, double TokenSubstring, double Fuzzy)
{
    public double Combined =>
        Exact * 1.00 +
        Prefix * 0.80 +
        TokenSubstring * 0.60 +
        Fuzzy * 0.40;
}
