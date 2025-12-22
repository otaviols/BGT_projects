using System.Collections;
using UnityEngine;

public class HeroXPBar : PegUIElement
{
  public ProgressBar m_progressBar;
  public UberText m_heroLevelText;
  public UberText m_barText;
  public GameObject m_simpleFrame;
  public GameObject m_heroFrame;
  public bool m_isAnimated;
  public float m_delay;
  public bool m_isOnDeck;
  public int m_soloLevelLimit;
  public HeroXPBar.PlayLevelUpEffectCallback m_levelUpCallback;
  private NetCache.HeroLevel m_heroLevel;
  private int m_totalLevel;
  private string m_rewardTitle;
  private string m_rewardDesc;

  protected override void Awake()
  {
    base.Awake();
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnProgressBarOver));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnProgressBarOut));
  }

  public void UpdateDisplay(NetCache.HeroLevel heroLevel, int totalLevel)
  {
    if (heroLevel == null)
    {
      this.gameObject.SetActive(false);
    }
    else
    {
      this.m_heroLevel = heroLevel;
      this.m_totalLevel = totalLevel;
      RewardUtils.GetNextHeroLevelRewardText(this.m_heroLevel.Class, this.m_heroLevel.CurrentLevel.Level, this.m_totalLevel, out this.m_rewardTitle, out this.m_rewardDesc);
      this.gameObject.SetActive(true);
      if (this.m_isOnDeck)
      {
        this.m_simpleFrame.SetActive(true);
        this.m_heroFrame.SetActive(false);
      }
      else
      {
        this.m_simpleFrame.SetActive(false);
        this.m_heroFrame.SetActive(true);
      }
      this.SetBarText("");
      if (this.m_isAnimated && this.m_heroLevel.PrevLevel != null)
      {
        this.m_heroLevelText.Text = this.m_heroLevel.PrevLevel.Level.ToString();
        if (this.m_heroLevel.PrevLevel.IsMaxLevel())
        {
          this.SetBarValue(1f);
        }
        else
        {
          this.SetBarValue((float) this.m_heroLevel.PrevLevel.XP / (float) this.m_heroLevel.PrevLevel.MaxXP);
          this.StartCoroutine(this.DelayBarAnimation(this.m_heroLevel.PrevLevel, this.m_heroLevel.CurrentLevel));
        }
      }
      else
      {
        this.m_heroLevelText.Text = this.m_heroLevel.CurrentLevel.Level.ToString();
        if (this.m_heroLevel.CurrentLevel.IsMaxLevel())
          this.SetBarValue(1f);
        else
          this.SetBarValue((float) this.m_heroLevel.CurrentLevel.XP / (float) this.m_heroLevel.CurrentLevel.MaxXP);
      }
    }
  }

  public void AnimateBar(
    NetCache.HeroLevel.LevelInfo previousLevelInfo,
    NetCache.HeroLevel.LevelInfo currentLevelInfo)
  {
    this.m_heroLevelText.Text = previousLevelInfo.Level.ToString();
    if (previousLevelInfo.Level < currentLevelInfo.Level)
    {
      this.m_progressBar.AnimateProgress((float) previousLevelInfo.XP / (float) previousLevelInfo.MaxXP, 1f);
      this.StartCoroutine(this.AnimatePostLevelUpXp(this.m_progressBar.GetAnimationTime(), currentLevelInfo));
    }
    else
    {
      float prevVal = (float) previousLevelInfo.XP / (float) previousLevelInfo.MaxXP;
      float currVal = (float) currentLevelInfo.XP / (float) currentLevelInfo.MaxXP;
      if (currentLevelInfo.IsMaxLevel())
        currVal = 1f;
      this.m_progressBar.AnimateProgress(prevVal, currVal);
    }
  }

  public void SetBarValue(float barValue) => this.m_progressBar.SetProgressBar(barValue);

  public void SetBarText(string barText)
  {
    if (!((Object) this.m_barText != (Object) null))
      return;
    this.m_barText.Text = barText;
  }

  private IEnumerator AnimatePostLevelUpXp(
    float delayTime,
    NetCache.HeroLevel.LevelInfo currentLevelInfo)
  {
    yield return (object) new WaitForSeconds(delayTime);
    if (currentLevelInfo.Level == 3 && !Options.Get().GetBool(Option.HAS_SEEN_LEVEL_3, false) && UserAttentionManager.CanShowAttentionGrabber("HeroXPBar.AnimatePostLevelUpXp:" + (object) Option.HAS_SEEN_LEVEL_3))
    {
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_LEVEL3_TIP"), "VO_INNKEEPER_LEVEL3_TIP.prefab:0f82ce6c91fccf249b6abcc9f153ff1e");
      Options.Get().SetBool(Option.HAS_SEEN_LEVEL_3, true);
    }
    this.m_heroLevelText.Text = currentLevelInfo.Level.ToString();
    this.m_progressBar.AnimateProgress(0.0f, (float) currentLevelInfo.XP / (float) currentLevelInfo.MaxXP);
    if (this.m_levelUpCallback != null)
      this.m_levelUpCallback();
  }

  private IEnumerator DelayBarAnimation(
    NetCache.HeroLevel.LevelInfo prevInfo,
    NetCache.HeroLevel.LevelInfo currInfo)
  {
    yield return (object) new WaitForSeconds(this.m_delay);
    this.AnimateBar(prevInfo, currInfo);
  }

  private void ShowTooltip()
  {
    if (string.IsNullOrEmpty(this.m_rewardTitle))
      return;
    TooltipZone component = this.gameObject.GetComponent<TooltipZone>();
    float num = !SceneMgr.Get().IsInGame() ? (float) TooltipPanel.COLLECTION_MANAGER_SCALE : (float) TooltipPanel.MULLIGAN_SCALE;
    if ((bool) UniversalInputManager.UsePhoneUI)
      num *= 1.1f;
    string rewardTitle = this.m_rewardTitle;
    string rewardDesc = this.m_rewardDesc;
    double scale = (double) num;
    component.ShowTooltip(rewardTitle, rewardDesc, (float) scale);
  }

  private void OnProgressBarOver(UIEvent e) => this.ShowTooltip();

  private void OnProgressBarOut(UIEvent e) => this.gameObject.GetComponent<TooltipZone>().HideTooltip();

  public delegate void PlayLevelUpEffectCallback();
}
