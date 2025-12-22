using Blizzard.T5.Core;
using System;

public class DeeplinkUtils
{
  public static Map<string, string> GetDeepLinkArgs(string[] deeplink)
  {
    Map<string, string> deepLinkArgs = new Map<string, string>();
    if (deeplink != null && deeplink.Length != 0)
    {
      string argsString = DeeplinkUtils.GetArgsString(deeplink);
      char[] chArray = new char[1]{ '&' };
      foreach (string str1 in argsString.Split(chArray))
      {
        string[] strArray = str1.Split('=');
        if (strArray.Length != 2 || strArray[0].Length == 0)
        {
          Log.DeepLink.PrintInfo("Skipping invalid formed arg {0}", (object) str1);
        }
        else
        {
          string key = strArray[0];
          string str2 = Uri.UnescapeDataString(strArray[1]);
          if (deepLinkArgs.ContainsKey(key))
            Log.DeepLink.PrintInfo("Duplicate arg {0} in deeplink, overwritting previous value {1} with {2}", (object) key, (object) deepLinkArgs[key], (object) str2);
          Log.DeepLink.PrintDebug("Found deeplink arg {0} = {1}", (object) key, (object) str2);
          deepLinkArgs[key] = str2;
        }
      }
    }
    return deepLinkArgs;
  }

  private static string GetArgsString(string[] deeplink)
  {
    int index = deeplink.Length - 1;
    string str = deeplink[index];
    int startIndex = str.LastIndexOf('?') + 1;
    return startIndex >= str.Length ? string.Empty : str.Substring(startIndex);
  }
}
