using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudStorageManager : MonoBehaviour
{
  private static CloudStorageManager s_Instance;
  private bool m_isInitialized;
  private bool m_isInitializing;
  private bool m_isShowingContext;
  private bool m_continueInitialize;
  private bool m_isConnecting;
  private bool m_isAPIUnavailable;
  private bool m_isSignInRequired;
  private bool m_isShowingThirdPartyPermission;
  private List<CloudStorageManager.OnInitializedFinished> m_onInitializedFinishedHandlers = new List<CloudStorageManager.OnInitializedFinished>();

  private void Awake() => CloudStorageManager.s_Instance = this;

  private void OnDestroy() => CloudStorageManager.s_Instance = (CloudStorageManager) null;

  public static CloudStorageManager Get() => CloudStorageManager.s_Instance;

  public static bool ShouldDisallowCloudStorage()
  {
    if (!Options.Get().GetBool(Option.DISALLOWED_CLOUD_STORAGE))
      return false;
    Log.CloudStorage.Print("Cloud Storage is Disallowed");
    return true;
  }

  public void DisallowCloudStorage()
  {
    Log.CloudStorage.Print("Setting Cloud Storage to Disallowed");
    Options.Get().SetBool(Option.DISALLOWED_CLOUD_STORAGE, true);
    this.m_isShowingThirdPartyPermission = false;
  }

  public void ShowCloudStorageContext(string contextBody)
  {
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_CLOUD_STORAGE_CONTEXT_HEADER"),
      m_text = contextBody,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
      m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnCloudStorageContextResponse)
    });
    this.m_isShowingContext = true;
  }

  private void OnCloudStorageContextResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
    {
      Log.CloudStorage.Print("Cloud Storage prompt permission not granted");
      this.m_continueInitialize = false;
      this.DisallowCloudStorage();
    }
    else
    {
      Log.CloudStorage.Print("Cloud Storage prompt permission granted");
      this.m_continueInitialize = true;
    }
    this.m_isShowingContext = false;
  }

  public void StartInitialize(
    CloudStorageManager.OnInitializedFinished onInitializedFinishedHandler,
    string contextBody)
  {
    this.StartCoroutine(this.Initialize(onInitializedFinishedHandler, contextBody));
  }

  public bool SetString(string key, string value)
  {
    if (CloudStorageManager.ShouldDisallowCloudStorage())
      return false;
    if (!this.m_isInitialized)
    {
      Log.CloudStorage.PrintWarning("Cloud Storage is not Initialized!");
      return false;
    }
    Log.CloudStorage.Print("Set string \"" + (value == null ? "null" : value) + "\" for key \"" + (key == null ? "null" : key) + "\"");
    CloudStorageManager.CloudSetString(key, value);
    return true;
  }

  public string GetString(string key)
  {
    if (CloudStorageManager.ShouldDisallowCloudStorage())
      return (string) null;
    if (!this.m_isInitialized)
    {
      Log.CloudStorage.PrintWarning("Cloud Storage is not Initialized!");
      return (string) null;
    }
    string str = CloudStorageManager.CloudGetString(key);
    Log.CloudStorage.Print("Get string \"" + (str == null ? "null" : str) + "\" from key \"" + (key == null ? "null" : key) + "\"");
    return str;
  }

  public void RemoveObject(string key)
  {
    if (CloudStorageManager.ShouldDisallowCloudStorage())
      return;
    if (!this.m_isInitialized)
    {
      Log.CloudStorage.PrintWarning("Cloud Storage is not Initialized!");
    }
    else
    {
      Log.CloudStorage.Print("Remove object for key \"" + (key == null ? "null" : key) + "\"");
      CloudStorageManager.CloudRemoveObject(key);
    }
  }

  public bool IsShowingContext() => this.m_isShowingContext;

  public bool ContinueInitialize() => this.m_continueInitialize;

  public bool GetIsShowingThirdPartyPermission() => this.m_isShowingThirdPartyPermission;

  public bool IsConnecting() => this.m_isConnecting;

  public bool IsAPIUnavailable() => this.m_isAPIUnavailable;

  public bool IsSignInRequired() => this.m_isSignInRequired;

  public void APIUnavailable()
  {
    Log.CloudStorage.Print("API Unavailable");
    this.m_isConnecting = false;
    this.m_isAPIUnavailable = true;
    this.m_isSignInRequired = false;
  }

  public void APISignInRequired()
  {
    Log.CloudStorage.Print("API Sign In Required");
    this.m_isConnecting = false;
    this.m_isAPIUnavailable = false;
    this.m_isSignInRequired = true;
  }

  public void APIConnected()
  {
    Log.CloudStorage.Print("API Connected");
    this.m_isConnecting = false;
    this.m_isAPIUnavailable = false;
    this.m_isSignInRequired = false;
    this.m_isShowingThirdPartyPermission = false;
  }

  private IEnumerator Initialize(
    CloudStorageManager.OnInitializedFinished onInitializedFinishedHandler,
    string contextBody)
  {
    Log.CloudStorage.Print(nameof (Initialize));
    if (this.m_isInitialized)
    {
      Log.CloudStorage.PrintWarning("Cloud Storage is already Initialized!");
      if (onInitializedFinishedHandler != null)
        onInitializedFinishedHandler();
    }
    else
    {
      if (onInitializedFinishedHandler != null)
        this.m_onInitializedFinishedHandlers.Add(onInitializedFinishedHandler);
      if (this.m_isInitializing)
      {
        Log.CloudStorage.PrintWarning("Cloud Storage is being Initialized!");
      }
      else
      {
        this.m_isInitializing = true;
        Log.CloudStorage.PrintWarning("Cloud Storage has finished initializing!");
        this.m_isInitializing = false;
        this.m_isInitialized = true;
        foreach (CloudStorageManager.OnInitializedFinished initializedFinishedHandler in this.m_onInitializedFinishedHandlers)
          initializedFinishedHandler();
        this.m_onInitializedFinishedHandlers.Clear();
        yield break;
      }
    }
  }

  private static bool IsStorageEnabledOnAndroidPlatform()
  {
    AndroidDeviceSettings androidDeviceSettings = AndroidDeviceSettings.Get();
    if (androidDeviceSettings != null)
      return androidDeviceSettings.GetAndroidStore() != AndroidStore.HUAWEI;
    Log.CloudStorage.PrintError("Android device settings unexpectedly null");
    return false;
  }

  private static void CloudSetString(string key, string value)
  {
  }

  private static string CloudGetString(string key) => (string) null;

  private static void CloudRemoveObject(string key)
  {
  }

  public delegate void OnInitializedFinished();
}
