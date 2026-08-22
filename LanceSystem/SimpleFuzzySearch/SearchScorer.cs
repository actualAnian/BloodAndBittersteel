using System;
using System.Linq;
using TaleWorlds.Core;

namespace LanceSystem.SimpleFuzzySearch;

public sealed class SearchScorer
{
    private readonly ITextNormalizer _normalizer;
    private readonly IFuzzyMatcher _fuzzyMatcher;

    public SearchScorer(ITextNormalizer normalizer, IFuzzyMatcher fuzzyMatcher)
    {
        _normalizer = normalizer;
        _fuzzyMatcher = fuzzyMatcher;
    }

    public SearchScore Score(string text, string query)
    {
        var normalizedText = _normalizer.Normalize(text);
        var normalizedQuery = _normalizer.Normalize(query);

        if (string.IsNullOrEmpty(normalizedText) || string.IsNullOrEmpty(normalizedQuery))
        {
            return new SearchScore(0, 0, 0, 0);
        }

        var exact = normalizedText == normalizedQuery ? 1.0 : 0.0;

        var prefix =
            normalizedText.StartsWith(
                normalizedQuery,
                StringComparison.Ordinal)
                    ? 1.0
                    : 0.0;

        var tokenSubstring =
            CalculateTokenSubstringScore(
                normalizedText,
                normalizedQuery);

        var fuzzy =
            CalculateFuzzyScore(
                normalizedText,
                normalizedQuery);

        return new SearchScore(
            exact,
            prefix,
            tokenSubstring,
            fuzzy);
    }

    private static double CalculateTokenSubstringScore(
        string text,
        string query)
    {
        if (text.Contains(query))
            return 1.0;

        var textTokens = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        var queryTokens = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (queryTokens.Length == 0)
            return 0.0;

        var matched = queryTokens.Count(queryToken =>
            textTokens.Any(textToken =>
                textToken.Contains(queryToken)));

        return (double)matched / queryTokens.Length;
    }

    private double CalculateFuzzyScore(
        string text,
        string query)
    {
        var bestScore =
            _fuzzyMatcher.Similarity(text, query);

        var tokens = text.Split(
            new[] { ' ' },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            var score =
                _fuzzyMatcher.Similarity(token, query);

            bestScore = Math.Max(
                bestScore,
                score);
        }

        return bestScore;
    }
}
