using UnityEngine;

[CustomEditClass]
public class AdventureCompleteReward : Reward
{
  private const string s_EventShowHurt = "ShowHurt";
  private const string s_EventShowBadlyHurt = "ShowBadlyHurt";
  private const string s_EventHide = "Hide";
  [CustomEditField(Sections = "State Event Table")]
  public StateEventTable m_StateTable;
  [CustomEditField(Sections = "Banner")]
  public UberText m_BannerTextObject;
  [CustomEditField(Sections = "Banner")]
  public GameObject m_BannerObject;
  [CustomEditField(Sections = "Banner")]
  public Vector3_MobileOverride m_BannerScaleOverride;
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected override void InitData() => this.SetData((RewardData) new AdventureCompleteRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    if (this.IsShown)
      return;
    AdventureCompleteRewardData data = this.Data as AdventureCompleteRewardData;
    if ((Object) this.m_StateTable != (Object) null)
      this.m_StateTable.TriggerState(!GameUtils.IsModeHeroic(data.ModeId) || !this.m_StateTable.HasState("ShowBadlyHurt") ? "ShowHurt" : "ShowBadlyHurt");
    if ((Object) this.m_BannerTextObject != (Object) null)
      this.m_BannerTextObject.Text = data.BannerText;
    if ((Object) this.m_BannerObject != (Object) null && this.m_BannerScaleOverride != null)
    {
      Vector3 bannerScaleOverride = (Vector3) (MobileOverrideValue<Vector3>) this.m_BannerScaleOverride;
      if (bannerScaleOverride != Vector3.zero)
        this.m_BannerObject.transform.localScale = bannerScaleOverride;
    }
    this.FadeFullscreenEffectsIn();
  }

  protected override void PlayShowSounds()
  {
  }

  protected override void HideReward()
  {
    if (!this.IsShown)
      return;
    base.HideReward();
    if ((Object) this.m_StateTable != (Object) null)
      this.m_StateTable.TriggerState("Hide");
    this.FadeFullscreenEffectsOut();
  }

  private void FadeFullscreenEffectsIn()
  {
    if (this.m_screenEffectsHandle == null)
      this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignettePerspective with
    {
      Blur = new BlurParameters(brightness: 0.85f)
    });
  }

  private void FadeFullscreenEffectsOut()
  {
    if (FullScreenFXMgr.Get() == null)
      Debug.LogWarning((object) "AdventureCompleteReward: FullScreenFXMgr.Get() returned null!");
    else
      this.m_screenEffectsHandle.StopEffect();
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    if (!(this.Data is AdventureCompleteRewardData))
    {
      Debug.LogWarning((object) string.Format("AdventureCompleteReward.OnDataSet() - Data {0} is not AdventureCompleteRewardData", (object) this.Data));
    }
    else
    {
      this.EnableClickCatcher(true);
      this.RegisterClickListener((Reward.OnClickedCallback) ((reward, userData) => this.HideReward()));
      this.SetReady(true);
    }
  }

  private void DestroyThis() => Object.DestroyImmediate((Object) this.gameObject);
}
