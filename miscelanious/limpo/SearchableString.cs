using Blizzard.T5.Core;
using System;
using System.Globalization;
using System.Text;

public class SearchableString
{
  private static Map<char, string> s_europeanConversionTable = new Map<char, string>()
  {
    {
      'œ',
      "oe"
    },
    {
      'æ',
      "ae"
    },
    {
      '’',
      "'"
    },
    {
      '«',
      "\""
    },
    {
      '»',
      "\""
    },
    {
      'ä',
      "ae"
    },
    {
      'ü',
      "ue"
    },
    {
      'ö',
      "oe"
    },
    {
      'ß',
      "ss"
    }
  };

  public string Text { get; }

  public string TextNonEuropean { get; }

  public string TextNoDiacritics { get; }

  public SearchableString(string text)
  {
    this.Text = UberText.RemoveMarkupAndCollapseWhitespaces(text).Trim().ToLower();
    this.TextNonEuropean = SearchableString.ConvertEuropeanCharacters(text);
    this.TextNoDiacritics = SearchableString.RemoveDiacritics(text);
  }

  public bool Search(string text) => SearchableString.SearchTexts(text, this.Text, this.TextNonEuropean, this.TextNoDiacritics);

  public static bool SearchInternationalText(string textToSearchIn, string textToSearchFor)
  {
    string lower = UberText.RemoveMarkupAndCollapseWhitespaces(textToSearchIn).Trim().ToLower();
    return lower.Contains(textToSearchFor, StringComparison.OrdinalIgnoreCase) || SearchableString.ConvertEuropeanCharacters(lower).Contains(textToSearchFor, StringComparison.OrdinalIgnoreCase) || SearchableString.RemoveDiacritics(lower).Contains(textToSearchFor, StringComparison.OrdinalIgnoreCase);
  }

  public static string ConvertEuropeanCharacters(string input)
  {
    int length = input.Length;
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < length; ++index)
    {
      string str;
      if (SearchableString.s_europeanConversionTable.TryGetValue(input[index], out str))
        stringBuilder.Append(str);
      else
        stringBuilder.Append(input[index]);
    }
    return stringBuilder.ToString();
  }

  public static string RemoveDiacritics(string input)
  {
    string str = input.Normalize(NormalizationForm.FormD);
    int length = str.Length;
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < length; ++index)
    {
      if (CharUnicodeInfo.GetUnicodeCategory(str[index]) != UnicodeCategory.NonSpacingMark)
        stringBuilder.Append(str[index]);
    }
    return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
  }

  private static bool SearchTexts(string textToSearchFor, params string[] textsToSearch)
  {
    foreach (string str in textsToSearch)
    {
      if (str.Contains(textToSearchFor, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }
}
