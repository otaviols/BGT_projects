using System;
using System.Collections;
using UnityEngine;

public class InstallReferrer
{
  private readonly object thisLock_ = new object();
  private bool done_;
  private string referrerUrl_;
  private int responseCode_ = -1;

  public bool RequestAsync(
    AndroidJavaObject context,
    InstallReferrer.ReferrerReceivedCallback callback)
  {
    if (Application.platform == RuntimePlatform.Android)
    {
      using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.blizzard.telemetry.sdk.platform.InstallReferrer", Array.Empty<object>()))
        androidJavaObject.Call("RequestReferrer", (object) context, (object) new InstallReferrer.ReferrerCallback(callback));
      return true;
    }
    Debug.LogError((object) "Install referrer url is only supported on Android devices.");
    return false;
  }

  private void OnReferrerReceived(int responseCode, string referrerUrl)
  {
    lock (this.thisLock_)
    {
      this.responseCode_ = responseCode;
      this.referrerUrl_ = referrerUrl;
      this.done_ = true;
    }
  }

  public static IEnumerator RequestCoroutine(
    InstallReferrer.ReferrerReceivedCallback callback)
  {
    if (Application.platform != RuntimePlatform.Android)
    {
      Debug.LogError((object) "Install referrer url is only supported on Android devices.");
    }
    else
    {
      using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
      {
        using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        {
          using (AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext"))
          {
            InstallReferrer referrer = new InstallReferrer();
            if (referrer.RequestAsync(context, new InstallReferrer.ReferrerReceivedCallback(referrer.OnReferrerReceived)))
            {
              while (true)
              {
                lock (referrer.thisLock_)
                {
                  if (referrer.done_)
                    break;
                }
                yield return (object) null;
              }
              int responseCode;
              string referrerUrl;
              lock (referrer.thisLock_)
              {
                responseCode = referrer.responseCode_;
                referrerUrl = string.Copy(referrer.referrerUrl_);
              }
              callback(responseCode, referrerUrl);
            }
            else
              callback(-1, "");
            referrer = (InstallReferrer) null;
          }
        }
      }
    }
  }

  public delegate void ReferrerReceivedCallback(int responseCode, string referrerUrl);

  private class ReferrerCallback : AndroidJavaProxy
  {
    private InstallReferrer.ReferrerReceivedCallback callback_;

    public ReferrerCallback(InstallReferrer.ReferrerReceivedCallback callback)
      : base("com.blizzard.telemetry.sdk.platform.InstallReferrerCallback")
    {
      this.callback_ = callback;
    }
  }
}
