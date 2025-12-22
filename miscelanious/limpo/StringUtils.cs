using System;
using System.Text.RegularExpressions;

public static class StringUtils
{
  public static readonly char[] SPLIT_LINES_CHARS_ARRAY = "\n\r".ToCharArray();
  public static readonly char[] REGEX_RESERVED_CHARS_ARRAY = "\\*.+?^$()[]{}".ToCharArray();

  public static string StripNonNumbers(string str) => Regex.Replace(str, "[^0-9]", string.Empty);

  public static string StripNewlines(string str) => Regex.Replace(str, "[\\r\\n]", string.Empty);

  public static bool CompareIgnoreCase(string a, string b) => string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;

  public static bool Contains(this string str, string val, StringComparison comparison) => str.IndexOf(val, comparison) >= 0;

  public static bool Contains(this string s, char c) => s.IndexOf(c) >= 0;
}
