using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class QuestProgressToast : GameToast
{
  public UberText m_questTitle;
  public UberText m_questDescription;
  public UberText m_questProgressCount;
  public GameObject m_questProgressCountBg;
  public GameObject m_background;

  private void Awake()
  {
    this.m_intensityMaterials.Add(this.m_questProgressCountBg.GetComponent<Renderer>().GetMaterial());
    this.m_intensityMaterials.Add(this.m_background.GetComponent<Renderer>().GetMaterial());
  }

  public void UpdateDisplay(string title, string description, int progress, int maxProgress)
  {
    if (maxProgress > 1)
    {
      this.m_questProgressCountBg.SetActive(true);
      this.m_questProgressCount.Text = GameStrings.Format("GLOBAL_QUEST_PROGRESS_COUNT", (object) progress, (object) maxProgress);
    }
    else
      this.m_questProgressCountBg.SetActive(false);
    this.m_questTitle.Text = title;
    this.m_questDescription.Text = description;
  }
}
