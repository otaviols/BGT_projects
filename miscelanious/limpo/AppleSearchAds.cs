using System.Collections;

public static class AppleSearchAds
{
  private static readonly object thisLock_ = new object();
  private static bool done_ = false;
  private static bool success_ = false;
  private static string jsonString_;
  private static int errorCode_ = -1;
  private static string errorMessage_;

  private static void OnReturnAttributionDetails(
    bool success,
    string jsonString,
    int errorCode,
    string errorMessage)
  {
    lock (AppleSearchAds.thisLock_)
    {
      AppleSearchAds.success_ = success;
      AppleSearchAds.jsonString_ = jsonString;
      AppleSearchAds.errorCode_ = errorCode;
      AppleSearchAds.errorMessage_ = errorMessage;
      AppleSearchAds.done_ = true;
    }
  }

  private static bool RequestAttributionDetails(AppleSearchAds.AttributionCallback callback)
  {
    callback(true, "{\"Version3.1\":{\"iad-keyword\":\"Keyword\",\"iad-adgroup-id\":\"1234567890\",\"iad-campaign-id\":\"1234567890\",\"iad-lineitem-id\":\"1234567890\",\"iad-campaign-name\":\"CampaignName\",\"iad-org-name\":\"OrgName\",\"iad-conversion-date\":\"2018-07-27T22:47:19Z\",\"iad-creative-name\":\"CreativeName\",\"iad-creative-id\":\"1234567890\",\"iad-click-date\":\"2018-07-27T22:47:19Z\",\"iad-attribution\":\"true\",\"iad-adgroup-name\":\"AdGroupName\",\"iad-lineitem-name\":\"LineName\"}}", -1, "");
    return true;
  }

  public static IEnumerator RequestAsync(
    AppleSearchAds.AttributionCallback completionCallback)
  {
    if (AppleSearchAds.RequestAttributionDetails(new AppleSearchAds.AttributionCallback(AppleSearchAds.OnReturnAttributionDetails)))
    {
      while (true)
      {
        lock (AppleSearchAds.thisLock_)
        {
          if (AppleSearchAds.done_)
            break;
        }
        yield return (object) null;
      }
      bool success;
      string jsonString;
      int errorCode;
      string errorMessage;
      lock (AppleSearchAds.thisLock_)
      {
        success = AppleSearchAds.success_;
        jsonString = string.Copy(AppleSearchAds.jsonString_);
        errorCode = AppleSearchAds.errorCode_;
        errorMessage = string.Copy(AppleSearchAds.errorMessage_);
      }
      completionCallback(success, jsonString, errorCode, errorMessage);
    }
  }

  public delegate void AttributionCallback(
    bool success,
    string jsonString,
    int errorCode,
    string errorMessage);
}
