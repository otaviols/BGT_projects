using SwrveUnity;
using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SwrveComponent : MonoBehaviour
{
  public SwrveSDK SDK;
  public bool FlushEventsOnApplicationQuit = true;
  protected static SwrveComponent instance;

  public static SwrveComponent Instance
  {
    get
    {
      if (!(bool) (UnityEngine.Object) SwrveComponent.instance)
      {
        if (UnityEngine.Object.FindObjectsOfType(typeof (SwrveComponent)) is SwrveComponent[] objectsOfType && objectsOfType.Length != 0)
          SwrveComponent.instance = objectsOfType[0];
        else
          SwrveLog.LogError((object) "There needs to be one active SwrveComponent script on a GameObject in your scene.");
      }
      return SwrveComponent.instance;
    }
  }

  public SwrveComponent() => this.SDK = (SwrveSDK) new SwrveEmpty();

  public void Init(int appId, string apiKey, SwrveConfig config = null)
  {
    if (this.SDK == null || this.SDK is SwrveEmpty)
      this.SDK = true ? (SwrveSDK) new SwrveEmpty() : new SwrveSDK();
    if (config == null)
      config = new SwrveConfig();
    this.SDK.Init((MonoBehaviour) this, appId, apiKey, config);
  }

  public void Start() => this.useGUILayout = false;

  public void OnGUI() => this.SDK.OnGUI();

  public void Update()
  {
    if (this.SDK == null || !this.SDK.Initialised)
      return;
    this.SDK.Update();
  }

  public void OnDestroy()
  {
    if (this.SDK.Initialised)
      this.SDK.OnSwrveDestroy();
    this.StopAllCoroutines();
  }

  public void OnApplicationQuit()
  {
    if (!this.SDK.Initialised || !this.FlushEventsOnApplicationQuit)
      return;
    this.SDK.OnSwrveDestroy();
  }

  public void OnApplicationPause(bool pauseStatus)
  {
    if (this.SDK == null || !this.SDK.Initialised)
      return;
    if (pauseStatus)
      this.SDK.OnSwrvePause();
    else
      this.SDK.OnSwrveResume();
  }

  public void UserUpdate(string userUpdate)
  {
    try
    {
      Dictionary<string, object> dictionary = (Dictionary<string, object>) Json.Deserialize(userUpdate);
      Dictionary<string, string> attributes = new Dictionary<string, string>();
      Dictionary<string, object>.Enumerator enumerator = dictionary.GetEnumerator();
      while (enumerator.MoveNext())
        attributes[enumerator.Current.Key] = string.Format("{0}", enumerator.Current.Value);
      this.SDK.UserUpdate(attributes);
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ex.ToString(), nameof (userUpdate));
    }
  }

  public void NativeConversationClosed(string msg)
  {
    try
    {
      this.SDK.ConversationClosed();
    }
    catch (Exception ex)
    {
      SwrveLog.LogError((object) ex.ToString(), "nativeConversationClosed");
    }
  }

  private void Awake() => UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.transform.gameObject);
}
