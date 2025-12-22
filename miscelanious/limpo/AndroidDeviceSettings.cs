using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using System;
using System.Text;
using UnityEngine;

public class AndroidDeviceSettings
{
  private static AndroidDeviceSettings s_instance;
  private string m_bestTexture = "";
  public bool m_determineSDCard;
  public string m_deviceModel = string.Empty;
  public int densityDpi = 300;
  public bool isExtraLarge = true;
  public bool isTablet = true;
  public string applicationStorageFolder;
  public string assetBundleFolder;
  public string externalStorageFolder;
  public string m_HSStore;
  public int m_AndroidSDKVersion;

  private AndroidDeviceSettings()
  {
    int num = Application.isEditor ? 1 : 0;
  }

  public string InstalledTexture
  {
    get
    {
      if (!string.IsNullOrEmpty(this.m_bestTexture))
        return this.m_bestTexture;
      this.m_bestTexture = Vars.Key("Mobile.Texture").GetStr("");
      if (!string.IsNullOrEmpty(this.m_bestTexture))
      {
        Log.Downloader.PrintInfo("m_bestTexture is already set to " + this.m_bestTexture);
        return this.m_bestTexture;
      }
      this.m_bestTexture = "astc";
      return this.m_bestTexture;
    }
  }

  public void AskForSDCard() => this.m_determineSDCard = true;

  public bool IsCurrentTextureFormatSupported()
  {
    bool flag = SystemInfo.SupportsTextureFormat(new Map<string, TextureFormat>()
    {
      {
        "",
        TextureFormat.ARGB32
      },
      {
        "etc1",
        TextureFormat.ETC_RGB4
      },
      {
        "etc2",
        TextureFormat.ETC2_RGBA8
      },
      {
        "astc",
        TextureFormat.ASTC_12x12
      }
    }[this.InstalledTexture]);
    Debug.Log((object) ("Checking whether texture format of build (" + this.InstalledTexture + ") is supported? " + flag.ToString()));
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("All supported texture formats: ");
    foreach (TextureFormat format in Enum.GetValues(typeof (TextureFormat)))
    {
      try
      {
        if (SystemInfo.SupportsTextureFormat(format))
          stringBuilder.Append(((object) format).ToString() + ", ");
      }
      catch (ArgumentException ex)
      {
      }
    }
    Log.Graphics.Print(stringBuilder.ToString());
    return flag;
  }

  public AndroidStore GetAndroidStore() => AndroidStore.NONE;

  public bool IsNonStoreAppAllowed() => false;

  public string GetPatchUrlFromArgument() => "";

  public bool AllowUnknownApps() => false;

  public void TriggerUnknownSources(string responseFuncName)
  {
  }

  public void ProcessInstallAPK(string apkPath, string installAPKFuncName)
  {
  }

  public bool OpenAppStore()
  {
    UpdateUtils.OpenAppStore();
    return true;
  }

  public void DeleteOldNotificationChannels()
  {
  }

  public static AndroidDeviceSettings Get()
  {
    if (AndroidDeviceSettings.s_instance == null)
      AndroidDeviceSettings.s_instance = new AndroidDeviceSettings();
    return AndroidDeviceSettings.s_instance;
  }
}
