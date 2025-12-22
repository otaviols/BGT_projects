using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using UnityEngine;

public class RAFRecruitBar : PegUIElement
{
  public UberText m_battleTag;
  public UberText m_level;
  public ProgressBar m_progressBar;
  public MeshRenderer m_progressBarRenderer;
  public MaterialContainer m_progressCompleteMaterialContainer;
  private BnetId m_gameAccountId;
  private bool m_isLocked = true;

  private void Start()
  {
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnBarOver));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnBarOut));
  }

  public void SetGameAccountId(BnetId gameAccountId) => this.m_gameAccountId = gameAccountId;

  public BnetId GetGameAccountId() => this.m_gameAccountId;

  public void SetBattleTag(string battleTag) => this.m_battleTag.Text = battleTag;

  public void SetLevel(int level)
  {
    this.m_level.Text = level.ToString();
    if (level >= 20)
    {
      this.m_progressBarRenderer.SetMaterial(this.m_progressCompleteMaterialContainer.m_material);
      this.m_progressBar.SetMaterial(this.m_progressBarRenderer.GetMaterial());
    }
    this.m_progressBar.SetProgressBar((float) level / 20f);
  }

  public void SetLocked(bool isLocked)
  {
    this.m_isLocked = isLocked;
    if (this.m_isLocked)
    {
      this.m_progressBar.gameObject.SetActive(false);
      this.SetBattleTag(GameStrings.Format("GLUE_RAF_PROGRESS_FRAME_LOCKED_BAR_NAME"));
    }
    else
      this.m_progressBar.gameObject.SetActive(true);
  }

  private void OnBarOver(UIEvent e)
  {
    TooltipZone component = this.GetComponent<TooltipZone>();
    if ((Object) component == (Object) null)
      return;
    component.ShowLayerTooltip(GameStrings.Get("GLUE_RAF_RECRUIT_BAR_TOOLTIP_HEADLINE"), GameStrings.Get("GLUE_RAF_RECRUIT_BAR_TOOLTIP_DESC"));
  }

  private void OnBarOut(UIEvent e)
  {
    TooltipZone component = this.GetComponent<TooltipZone>();
    if (!((Object) component != (Object) null))
      return;
    component.HideTooltip();
  }
}
