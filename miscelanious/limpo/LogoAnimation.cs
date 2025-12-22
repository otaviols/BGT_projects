using Blizzard.T5.Jobs;
using Hearthstone;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
  public GameObject m_logoContainer;
  private static LogoAnimation s_instance;
  private GameObject m_logo;
  public UberText m_logoCopyright;
  private AssetReference m_LogoAssetRef = (AssetReference) "LogoImage.prefab:c7bbbc47f4498224491bb952df4c6bcb";

  private void Awake()
  {
    LogoAnimation.s_instance = this;
    this.m_logo = AssetLoader.Get().InstantiatePrefab(this.m_LogoAssetRef);
    this.m_logo.SetActive(true);
    GameUtils.SetParent(this.m_logo, this.m_logoContainer, true);
    this.m_logoContainer.SetActive(false);
    if (Localization.GetLocale() == Locale.zhCN)
    {
      this.m_logoCopyright.gameObject.SetActive(true);
      RenderUtils.SetAlpha(this.m_logoCopyright.gameObject, 1f);
    }
    HearthstoneApplication.Get().WillReset += new Action(this.OnWillReset);
  }

  public static LogoAnimation Get() => LogoAnimation.s_instance;

  private void OnDestroy()
  {
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      HearthstoneApplication.Get().WillReset -= new Action(this.OnWillReset);
    LogoAnimation.s_instance = (LogoAnimation) null;
  }

  public void HideLogo()
  {
    if (!((UnityEngine.Object) this.m_logoContainer != (UnityEngine.Object) null))
      return;
    this.m_logoContainer.SetActive(false);
  }

  public IEnumerator<IAsyncJobResult> Job_FadeLogoIn()
  {
    float seconds = 0.5f;
    this.m_logoContainer.SetActive(true);
    iTween.FadeTo(this.m_logo, iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) seconds, (object) "includechildren", (object) true, (object) "easeType", (object) iTween.EaseType.easeInCubic));
    yield return (IAsyncJobResult) new WaitForDuration(seconds);
  }

  public IEnumerator<IAsyncJobResult> Job_FadeLogoOut()
  {
    float seconds = 0.5f;
    iTween.FadeTo(this.m_logo, iTween.Hash((object) "amount", (object) 0.0f, (object) "delay", (object) 0.0f, (object) "time", (object) seconds, (object) "easeType", (object) iTween.EaseType.linear));
    yield return (IAsyncJobResult) new WaitForDuration(seconds);
    this.DestroyLogoAnimation();
  }

  private void DestroyLogoAnimation() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

  public void ShowLogo()
  {
    if (!((UnityEngine.Object) this.m_logoContainer != (UnityEngine.Object) null))
      return;
    this.m_logoContainer.SetActive(true);
  }

  private void OnWillReset() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
}
