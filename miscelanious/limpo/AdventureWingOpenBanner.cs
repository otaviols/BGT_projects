using System;
using UnityEngine;

[CustomEditClass]
public class AdventureWingOpenBanner : MonoBehaviour
{
  public PegUIElement m_clickCatcher;
  public GameObject m_root;
  public iTween.EaseType m_showEase = iTween.EaseType.easeOutElastic;
  public float m_showTime = 0.5f;
  public float m_hideTime = 0.5f;
  [CustomEditField(Sections = "Shown Quote")]
  public string m_VOQuotePrefab;
  [CustomEditField(Sections = "Shown Quote")]
  public string m_VOQuoteLine;
  [CustomEditField(Sections = "Shown Quote")]
  public Vector3 m_VOQuotePosition = new Vector3(0.0f, 0.0f, -55f);
  [CustomEditField(Sections = "Dismissed Quote", T = EditType.GAME_OBJECT)]
  public string m_BannerDismissedQuotePrefab;
  [CustomEditField(Sections = "Dismissed Quote")]
  public string m_BannerDismissedQuoteLine;
  [CustomEditField(Sections = "Dismissed Quote")]
  public Vector3 m_BannerDismissedQuotePosition = new Vector3(0.0f, 0.0f, -55f);
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_showSound;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_hideSound;
  private Vector3 m_originalScale;
  private AdventureWingOpenBanner.OnBannerHidden m_bannerHiddenCallback;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    if ((UnityEngine.Object) this.m_clickCatcher != (UnityEngine.Object) null)
      this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.HideBanner()));
    if ((UnityEngine.Object) this.m_root != (UnityEngine.Object) null)
      this.m_root.SetActive(false);
    OverlayUI.Get().AddGameObject(this.gameObject, destroyOnSceneLoad: true);
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  public void ShowBanner(
    AdventureWingOpenBanner.OnBannerHidden onBannerHiddenCallback = null)
  {
    if ((UnityEngine.Object) this.m_root == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "m_root not defined in banner!");
    }
    else
    {
      this.m_bannerHiddenCallback = onBannerHiddenCallback;
      this.m_originalScale = this.m_root.transform.localScale;
      this.m_root.SetActive(true);
      iTween.ScaleFrom(this.m_root, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "time", (object) this.m_showTime, (object) "easetype", (object) this.m_showEase));
      if (!string.IsNullOrEmpty(this.m_showSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_showSound);
      if (!string.IsNullOrEmpty(this.m_VOQuotePrefab) && !string.IsNullOrEmpty(this.m_VOQuoteLine))
      {
        string legacyAssetName = new AssetReference(this.m_VOQuoteLine).GetLegacyAssetName();
        NotificationManager.Get().CreateCharacterQuote(this.m_VOQuotePrefab, this.m_VOQuotePosition, GameStrings.Get(legacyAssetName), this.m_VOQuoteLine, anchorPoint: CanvasAnchor.CENTER);
      }
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = this.m_showTime
      });
    }
  }

  public void HideBanner()
  {
    if ((UnityEngine.Object) this.m_root == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "m_root not defined in banner!");
    }
    else
    {
      this.m_screenEffectsHandle.StopEffect();
      this.m_root.transform.localScale = this.m_originalScale;
      iTween.ScaleTo(this.m_root, iTween.Hash((object) "scale", (object) new Vector3(0.01f, 0.01f, 0.01f), (object) "oncomplete", (object) (Action<object>) (o =>
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        if (this.m_bannerHiddenCallback == null)
          return;
        this.m_bannerHiddenCallback();
      }), (object) "time", (object) this.m_hideTime));
      if (!string.IsNullOrEmpty(this.m_hideSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_hideSound);
      if (string.IsNullOrEmpty(this.m_BannerDismissedQuotePrefab) || string.IsNullOrEmpty(this.m_BannerDismissedQuoteLine))
        return;
      string legacyAssetName = new AssetReference(this.m_BannerDismissedQuoteLine).GetLegacyAssetName();
      NotificationManager.Get().CreateCharacterQuote(this.m_BannerDismissedQuotePrefab, this.m_BannerDismissedQuotePosition, GameStrings.Get(legacyAssetName), this.m_BannerDismissedQuoteLine, anchorPoint: CanvasAnchor.CENTER);
    }
  }

  public delegate void OnBannerHidden();
}
