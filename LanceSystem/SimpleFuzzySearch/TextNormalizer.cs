using System.Globalization;
using System.Text;

namespace LanceSystem.SimpleFuzzySearch;

public sealed class TextNormalizer : ITextNormalizer
{
    public string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var normalized = text
            .Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);

            if (category != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }
}
