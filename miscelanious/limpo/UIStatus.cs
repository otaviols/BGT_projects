using Hearthstone.Core;
using System;
using System.Collections;
using UnityEngine;

public class UIStatus : MonoBehaviour
{
  public UberText m_Text;
  public Color m_InfoColor;
  public Color m_ErrorColor;
  public float m_FadeDelaySec = 2f;
  public float m_FadeSec = 0.5f;
  public iTween.EaseType m_FadeEaseType = iTween.EaseType.linear;
  private static UIStatus s_instance;
  private UIStatus.StatusType m_currentStatusType;

  private void Awake() => Processor.RunCoroutine(this.Initialize());

  private void OnDestroy() => UIStatus.s_instance = (UIStatus) null;

  public static UIStatus Get()
  {
    if ((UnityEngine.Object) UIStatus.s_instance == (UnityEngine.Object) null)
    {
      GameObject gameObject = AssetLoader.Get()?.InstantiatePrefab((AssetReference) "UIStatus.prefab:8fe3c92addcd14427a5277cfedc2341c");
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      {
        Log.UIStatus.PrintError("Failed to instantiate UI status prefab.");
        return (UIStatus) null;
      }
      UIStatus.s_instance = gameObject.GetComponent<UIStatus>();
    }
    return UIStatus.s_instance;
  }

  public void AddInfo(string message) => this.AddInfo(message, UIStatus.StatusType.GENERIC);

  public void AddInfo(string message, float delay) => this.AddInfo(message, UIStatus.StatusType.GENERIC, delay);

  public void AddInfo(string message, UIStatus.StatusType statusType) => this.AddInfo(message, statusType, -1f);

  public void AddInfo(string message, UIStatus.StatusType statusType, float delay)
  {
    this.m_currentStatusType = statusType;
    this.m_Text.TextColor = this.m_InfoColor;
    this.ShowMessage(message, delay);
  }

  public void AddInfoNoRichText(string message, float delay = -1f)
  {
    this.m_Text.TextColor = this.m_InfoColor;
    this.ShowMessage(message, delay, false);
  }

  public void AddError(string message, float delay = -1f)
  {
    this.m_Text.TextColor = this.m_ErrorColor;
    this.ShowMessage(message, delay);
  }

  public void HideIfScreenshotMessage()
  {
    if (this.m_currentStatusType != UIStatus.StatusType.SCREENSHOT)
      return;
    iTween.Stop(this.m_Text.gameObject);
    this.OnFadeComplete();
  }

  private IEnumerator Initialize()
  {
    UIStatus uiStatus = this;
    UIStatus.s_instance = uiStatus;
    uiStatus.m_Text.gameObject.SetActive(false);
    yield return (object) new WaitUntil((Func<bool>) (() => (UnityEngine.Object) OverlayUI.Get() != (UnityEngine.Object) null));
    OverlayUI.Get().AddGameObject(uiStatus.gameObject);
  }

  private void ShowMessage(string message) => this.ShowMessage(message, -1f);

  private void ShowMessage(string message, float delay, bool richText = true)
  {
    Log.UIStatus.PrintDebug(message);
    if (message.Equals(this.m_Text.Text) && this.m_Text.gameObject.activeSelf)
      return;
    this.m_Text.Text = string.Empty;
    this.m_Text.RichText = richText;
    if (message.Contains("\n"))
    {
      this.m_Text.ResizeToFit = false;
      this.m_Text.WordWrap = true;
      this.m_Text.ForceWrapLargeWords = true;
    }
    else
    {
      this.m_Text.ResizeToFit = true;
      this.m_Text.WordWrap = false;
      this.m_Text.ForceWrapLargeWords = false;
    }
    this.m_Text.Text = message;
    this.m_Text.gameObject.SetActive(true);
    this.m_Text.TextAlpha = 1f;
    iTween.Stop(this.m_Text.gameObject, true);
    if ((double) delay < 0.0)
      delay = this.m_FadeDelaySec;
    iTween.FadeTo(this.m_Text.gameObject, iTween.Hash((object) "amount", (object) 0.0f, (object) nameof (delay), (object) delay, (object) "time", (object) this.m_FadeSec, (object) "easeType", (object) this.m_FadeEaseType, (object) "oncomplete", (object) "OnFadeComplete", (object) "oncompletetarget", (object) this.gameObject));
  }

  private void OnFadeComplete()
  {
    this.m_currentStatusType = UIStatus.StatusType.GENERIC;
    this.m_Text.gameObject.SetActive(false);
  }

  public enum StatusType
  {
    GENERIC,
    SCREENSHOT,
  }
}
