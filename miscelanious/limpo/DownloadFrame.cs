using Hearthstone.Core.Streaming;
using Hearthstone.Streaming;
using UnityEngine;

public class DownloadFrame : MonoBehaviour
{
  public UberText m_progress;
  public GameObject m_downloadArrow;
  public GameObject m_background;
  public PegUIElement m_mouseOverZone;
  private bool m_currencyIsShowing;
  private bool m_wasShowing;
  private bool m_isAwake;

  private void Awake()
  {
    if (this.m_isAwake)
      return;
    this.m_mouseOverZone.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnFrameMouseOver));
    this.m_mouseOverZone.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnFrameMouseOut));
    this.HideInternal();
    this.m_isAwake = true;
  }

  private void Update()
  {
    if (this.ShouldShow() && this.m_currencyIsShowing)
    {
      if (!this.m_wasShowing)
        this.ShowInternal();
      this.m_progress.Text = string.Format("{0:0.}%", (object) (float) ((double) GameDownloadManagerProvider.Get().GetContentDownloadStatus(DownloadTags.Content.Base).Progress * 100.0));
    }
    else
    {
      if (!this.m_wasShowing)
        return;
      this.HideInternal();
    }
  }

  public GameObject GetTooltipObject()
  {
    TooltipZone component = this.GetComponent<TooltipZone>();
    return (Object) component != (Object) null ? component.GetTooltipObject() : (GameObject) null;
  }

  private void SetChildrenActive(bool active)
  {
    for (int index = 0; index < this.transform.childCount; ++index)
      this.transform.GetChild(index).gameObject.SetActive(active);
  }

  public void Hide()
  {
    this.gameObject.SetActive(false);
    this.m_currencyIsShowing = false;
    this.HideInternal();
  }

  private void HideInternal()
  {
    this.m_wasShowing = false;
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.SetChildrenActive(false);
    else
      iTween.FadeTo(this.gameObject, iTween.Hash((object) "amount", (object) 0.0f, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutCubic));
  }

  public void Show()
  {
    this.m_currencyIsShowing = true;
    this.Awake();
    if (!this.ShouldShow())
      return;
    this.gameObject.SetActive(true);
    this.ShowInternal();
  }

  private void ShowInternal()
  {
    if (!this.m_currencyIsShowing)
      return;
    this.m_wasShowing = true;
    this.SetChildrenActive(true);
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    iTween.FadeTo(this.gameObject, iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutCubic));
  }

  private void OnFrameMouseOver(UIEvent e)
  {
    if (!this.ShouldShow() || !this.m_currencyIsShowing)
      return;
    string key1 = "GLUE_TOOLTIP_DOWNLOAD_HEADER";
    string key2 = "GLUE_TOOLTIP_DOWNLOAD_DESCRIPTION";
    TooltipPanel src = this.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get(key1), GameStrings.Get(key2), 0.7f);
    LayerUtils.SetLayer(src.gameObject, GameLayer.BattleNet);
    src.transform.localEulerAngles = new Vector3(270f, 0.0f, 0.0f);
    src.transform.localScale = new Vector3(70f, 70f, 70f);
    if ((bool) UniversalInputManager.UsePhoneUI)
      TransformUtil.SetPoint((Component) src, Anchor.TOP, (Component) this.m_mouseOverZone, Anchor.BOTTOM, Vector3.zero);
    else
      TransformUtil.SetPoint((Component) src, Anchor.BOTTOM, (Component) this.m_mouseOverZone, Anchor.TOP, Vector3.zero);
  }

  private void OnFrameMouseOut(UIEvent e) => this.GetComponent<TooltipZone>().HideTooltip();

  private bool ShouldShow() => GameDownloadManagerProvider.Get().IsAnyDownloadRequestedAndIncomplete;
}
