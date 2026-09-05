using System;

namespace LanceSystem.SimpleFuzzySearch;

public class LevenshteinMatcher : IFuzzyMatcher
{
    public double Similarity(string left, string right)
    {
        if (left == right)
            return 1.0;

        if (string.IsNullOrEmpty(left) ||
            string.IsNullOrEmpty(right))
            return 0.0;

        var distance = CalculateDistance(left, right);
        var maxLength = Math.Max(left.Length, right.Length);

        return 1.0 - (double)distance / maxLength;
    }

    private static int CalculateDistance(string left, string right)
    {
        var previous = new int[right.Length + 1];
        var current = new int[right.Length + 1];

        for (var j = 0; j <= right.Length; j++)
            previous[j] = j;

        for (var i = 1; i <= left.Length; i++)
        {
            current[0] = i;

            for (var j = 1; j <= right.Length; j++)
            {
                var cost = left[i - 1] == right[j - 1] ? 0 : 1;

                current[j] = Math.Min(
                    Math.Min(
                        current[j - 1] + 1,
                        previous[j] + 1),
                    previous[j - 1] + cost);
            }

            (previous, current) = (current, previous);
        }

        return previous[right.Length];
    }
}
