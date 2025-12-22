using Blizzard.T5.Core;
using System.Globalization;

public class ClipLengthEstimator
{
  public static readonly float MINIMUM_SAFE_DURATION = 0.5f;
  private static Map<Locale, float> perLocaleCharacterReadTime = new Map<Locale, float>()
  {
    {
      Locale.enUS,
      0.075f
    },
    {
      Locale.koKR,
      0.375f
    },
    {
      Locale.zhTW,
      0.375f
    },
    {
      Locale.zhCN,
      0.375f
    }
  };

  public static float StringToReadTime(string input)
  {
    int lengthInTextElements = new StringInfo(input).LengthInTextElements;
    float num1 = ClipLengthEstimator.perLocaleCharacterReadTime[Locale.enUS];
    Locale locale = Localization.GetLocale();
    if (ClipLengthEstimator.perLocaleCharacterReadTime.ContainsKey(locale))
      num1 = ClipLengthEstimator.perLocaleCharacterReadTime[locale];
    float num2 = (float) lengthInTextElements * num1;
    return (double) num2 < (double) ClipLengthEstimator.MINIMUM_SAFE_DURATION ? ClipLengthEstimator.MINIMUM_SAFE_DURATION : num2;
  }
}
