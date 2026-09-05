namespace LanceSystem.SimpleFuzzySearch;

public interface IFuzzyMatcher
{
    double Similarity(string left, string right);
}
