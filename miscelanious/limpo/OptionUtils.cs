using System;

public static class OptionUtils
{
  public static Option GetOptionFromString(string optionName)
  {
    if (string.IsNullOrEmpty(optionName))
      return Option.INVALID;
    object obj = Enum.Parse(typeof (Option), optionName, true);
    return obj == null ? Option.INVALID : (Option) obj;
  }
}
