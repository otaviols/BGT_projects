using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class ClassProgressBar : PegUIElement
{
  public TAG_CLASS m_class;
  public UberText m_classNameText;
  public UberText m_levelText;
  public GameObject m_classLockedGO;
  public ProgressBar m_progressBar;
  public GameObject m_classIcon;
  public GameObject m_levelFrame;
  public GameObject m_tooltipRoot;
  public UberText m_tooltipTitle;
  public UberText m_tooltipDesc;
  public UberText m_tooltipLevelText;
  private string m_rewardText;
  private bool m_tooltipAvailable;
  private bool m_locked;

  protected override void Awake()
  {
    base.Awake();
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnProgressBarOver));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnProgressBarOut));
  }

  public void Init()
  {
    if (!((Object) this.m_classNameText != (Object) null))
      return;
    this.m_classNameText.Text = GameStrings.GetClassName(this.m_class);
  }

  public void SetTooltipText(string title, string desc, string currentLevel)
  {
    if (!((Object) this.m_tooltipRoot != (Object) null))
      return;
    this.m_tooltipAvailable = true;
    this.m_tooltipRoot.GetComponent<TooltipPanel>().Initialize(title, desc);
    this.m_tooltipLevelText.Text = currentLevel.ToString();
    foreach (Component componentsInChild in this.m_tooltipRoot.GetComponentsInChildren<Transform>(true))
      componentsInChild.gameObject.layer = this.gameObject.layer;
    this.m_tooltipRoot.SetActive(false);
  }

  public void SetPremium(bool isPremium)
  {
    if (isPremium)
      return;
    this.GetComponent<Renderer>().GetMaterial().SetTexture("_MaskTex", (Texture) null);
  }

  public void Lock()
  {
    this.m_locked = true;
    this.m_classLockedGO.SetActive(true);
    this.m_levelFrame.SetActive(false);
  }

  private void OnProgressBarOver(UIEvent e)
  {
    if (this.m_locked || !this.m_tooltipAvailable)
      return;
    this.m_tooltipRoot.SetActive(true);
  }

  private void OnProgressBarOut(UIEvent e)
  {
    if (this.m_locked)
      return;
    this.m_tooltipRoot.SetActive(false);
  }
}
