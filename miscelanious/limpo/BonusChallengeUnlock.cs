using System;
using UnityEngine;

[CustomEditClass]
public class BonusChallengeUnlock : Reward
{
  [CustomEditField(Sections = "Container")]
  public UIBObjectSpacing m_cardContainer;
  [CustomEditField(Sections = "Text Settings")]
  public UberText m_headerText;
  private Actor m_bonusChallengeBossActor;
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected override void Awake()
  {
    base.Awake();
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_rewardBanner.transform.localScale = this.m_rewardBanner.transform.localScale * 8f;
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected override void InitData() => this.SetData((RewardData) new BonusChallengeUnlockData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    this.m_root.SetActive(true);
    this.m_cardContainer.UpdatePositions();
    this.m_cardContainer.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180f);
    iTween.RotateAdd(this.m_cardContainer.gameObject, iTween.Hash((object) "amount", (object) new Vector3(0.0f, 0.0f, 540f), (object) "time", (object) 1.5f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self));
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = 1f
    });
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_screenEffectsHandle.StopEffect(new Action(this.DestroyBonusChallengeUnlock));
    this.m_root.SetActive(false);
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    if (!(this.Data is BonusChallengeUnlockData data))
    {
      Debug.LogWarning((object) string.Format("BonusChallengeUnlock.OnDataSet() - Data {0} is not BonusChallengeUnlockData", (object) this.Data));
    }
    else
    {
      BannerManager.Get().ShowBanner(data.PrefabToDisplay, (string) null, GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_INTRO_BANNER_BUTTON"), new BannerManager.DelOnCloseBanner(((Reward) this).HideReward));
      this.EnableClickCatcher(true);
    }
  }

  private void DestroyBonusChallengeUnlock() => UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.gameObject);
}
