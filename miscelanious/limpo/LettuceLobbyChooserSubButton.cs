using Assets;
using UnityEngine;

public class LettuceLobbyChooserSubButton : ChooserSubButton
{
  [CustomEditField(Sections = "New Unlocks UI")]
  public GameObject m_unlocksCounter;
  [CustomEditField(Sections = "New Unlocks UI")]
  public UberText m_unlocksCounterText;
  private string m_customLockedText;
  private MercenariesDataUtil.MercenariesBountyLockedReason m_lockedReason;
  private SceneMgr.Mode m_mode;
  private int m_bountySetRecord;
  private LettuceBounty.MercenariesBountyDifficulty m_difficulty = LettuceBounty.MercenariesBountyDifficulty.NORMAL;

  public void LockButton(
    MercenariesDataUtil.MercenariesBountyLockedReason lockReason)
  {
    this.m_lockedReason = lockReason;
    this.SetDesaturate(true);
  }

  public void SetUnlocks(int amount)
  {
    if (amount > 0)
    {
      this.m_unlocksCounter.SetActive(true);
      this.m_unlocksCounterText.Text = string.Concat((object) amount);
    }
    else
      this.m_unlocksCounter.SetActive(false);
  }

  public void SetBountySetRecord(int record) => this.m_bountySetRecord = record;

  public LettuceBountySetDbfRecord GetBountySetRecord() => GameDbf.LettuceBountySet.GetRecord(this.m_bountySetRecord);

  public void SetCustomLockedText(string newText) => this.m_customLockedText = newText;

  public string GetCustomLockedText() => this.m_customLockedText;

  public MercenariesDataUtil.MercenariesBountyLockedReason GetLockedReason() => this.m_lockedReason;

  public void SetMode(SceneMgr.Mode newMode) => this.m_mode = newMode;

  public SceneMgr.Mode GetMode() => this.m_mode;

  public void SetDifficulty(
    LettuceBounty.MercenariesBountyDifficulty difficulty)
  {
    this.m_difficulty = difficulty;
  }

  public LettuceBounty.MercenariesBountyDifficulty GetDifficulty() => this.m_difficulty;
}
