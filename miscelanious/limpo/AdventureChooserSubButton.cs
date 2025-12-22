using UnityEngine;

[CustomEditClass]
public class AdventureChooserSubButton : ChooserSubButton
{
  [CustomEditField(Sections = "Progress UI")]
  public GameObject m_progressCounter;
  [CustomEditField(Sections = "Progress UI")]
  public UberText m_progressCounterText;
  [CustomEditField(Sections = "Progress UI")]
  public GameObject m_heroicSkull;
  public float m_ComingSoonBannerHeightOverride;
  private AdventureDbId m_TargetAdventure;
  private AdventureModeDbId m_TargetMode;

  public void SetAdventure(AdventureDbId id, AdventureModeDbId mode)
  {
    this.m_TargetAdventure = id;
    this.m_TargetMode = mode;
    this.ShowRemainingProgressCount();
  }

  public AdventureDbId GetAdventure() => this.m_TargetAdventure;

  public AdventureModeDbId GetMode() => this.m_TargetMode;

  public void ShowRemainingProgressCount()
  {
    int num = 0;
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) this.m_TargetAdventure, (int) this.m_TargetMode);
    if (adventureDataRecord != null && adventureDataRecord.ShowPlayableScenariosCount)
      num = this.m_TargetMode != AdventureModeDbId.CLASS_CHALLENGE ? AdventureProgressMgr.Get().GetNumPlayableAdventureScenarios(this.m_TargetAdventure, this.m_TargetMode) : AdventureProgressMgr.Get().GetPlayableClassChallenges(this.m_TargetAdventure, this.m_TargetMode);
    if (GameUtils.IsModeHeroic(this.m_TargetMode))
    {
      if ((Object) this.m_heroicSkull != (Object) null)
      {
        if (num > 0)
          this.m_heroicSkull.SetActive(true);
        else
          this.m_heroicSkull.SetActive(false);
      }
      if (!((Object) this.m_progressCounter != (Object) null))
        return;
      this.m_progressCounter.SetActive(false);
    }
    else
    {
      if ((Object) this.m_heroicSkull != (Object) null)
        this.m_heroicSkull.SetActive(false);
      if (!((Object) this.m_progressCounter != (Object) null))
        return;
      if (num > 0)
      {
        this.m_progressCounter.SetActive(true);
        this.m_progressCounterText.Text = num.ToString();
      }
      else
        this.m_progressCounter.SetActive(false);
    }
  }
}
