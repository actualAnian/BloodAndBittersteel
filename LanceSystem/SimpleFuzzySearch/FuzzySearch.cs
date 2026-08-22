using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace LanceSystem.SimpleFuzzySearch;

public sealed record SearchResult<T>(T Item, SearchScore Score);
public static class FuzzySearchManager
{
    private static readonly SearchScorer Scorer = new(new TextNormalizer(), new LevenshteinMatcher());

    public static IReadOnlyList<SearchResult<T>> TrySearch<T>(
        string query, Func<T, string> selector)
    {
        IEnumerable<T> items = MBObjectManager.Instance
            .CreateObjectTypeList(typeof(T))
            .Cast<T>();

        var engine = new FuzzySearch<T>(Scorer);

        return engine.Search(items, query, selector);
    }
}
public sealed class FuzzySearch<T>
{
    private readonly SearchScorer _scorer;
    public FuzzySearch(SearchScorer scorer)
    {
        _scorer = scorer;
    }

    public IReadOnlyList<SearchResult<T>> Search(
        IEnumerable<T> items,
        string query,
        Func<T, string> selector,
        double minimumScore = 0.3,
        int maxResults = 20)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<SearchResult<T>>();

        return items
            .Select(item =>
            {
                var value = selector(item);

                var score = _scorer.Score(value, query);

                return new SearchResult<T>(item, score);
            })
            .Where(result =>
                result.Score.Combined >= minimumScore)
            .OrderByDescending(result =>
                result.Score.Combined)
            .Take(maxResults)
            .ToList();
    }
}