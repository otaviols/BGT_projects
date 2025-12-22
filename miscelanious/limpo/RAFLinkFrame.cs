using UnityEngine;

public class RAFLinkFrame : MonoBehaviour
{
  public UIBButton m_copyButton;
  public UberText m_url;
  public HighlightState m_copyButtonHighlight;
  private PegUIElement m_inputBlockerPegUIElement;
  private bool m_isShown;
  private string m_fullUrl;

  private void Awake()
  {
    this.m_copyButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCopyButtonReleased));
    this.m_copyButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnCopyButtonOver));
    this.m_copyButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnCopyButtonOut));
  }

  public void Show()
  {
    if (this.m_isShown)
      return;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.m_isShown = true;
    this.gameObject.SetActive(true);
    if ((Object) this.m_inputBlockerPegUIElement != (Object) null)
    {
      Object.Destroy((Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "RAFLinkInputBlocker");
    LayerUtils.SetLayer(inputBlocker, this.gameObject.layer);
    this.m_inputBlockerPegUIElement = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlockerPegUIElement.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerRelease));
    TransformUtil.SetPosY((Component) this.m_inputBlockerPegUIElement, this.gameObject.transform.position.y - 5f);
    RAFManager.Get().GetRAFFrame().DarkenInputBlocker(inputBlocker, 0.5f);
  }

  public void Hide()
  {
    if (!this.m_isShown)
      return;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    if ((Object) this.m_inputBlockerPegUIElement != (Object) null && (Object) this.m_inputBlockerPegUIElement.gameObject != (Object) null)
    {
      Object.Destroy((Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    this.m_isShown = false;
    if (!((Object) this.gameObject != (Object) null))
      return;
    this.gameObject.SetActive(false);
  }

  public void SetURL(string displayUrl, string fullUrl)
  {
    this.m_url.Text = displayUrl;
    this.m_fullUrl = fullUrl;
  }

  private bool OnNavigateBack()
  {
    this.Hide();
    return true;
  }

  private void OnInputBlockerRelease(UIEvent e) => this.Hide();

  private void OnCopyButtonReleased(UIEvent e)
  {
    ClipboardUtils.CopyToClipboard(this.m_fullUrl);
    UIStatus.Get().AddInfo(GameStrings.Get("GLUE_RAF_COPY_COMPLETE"));
    this.Hide();
  }

  private void OnCopyButtonOver(UIEvent e) => this.m_copyButtonHighlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_MOUSE_OVER);

  private void OnCopyButtonOut(UIEvent e) => this.m_copyButtonHighlight.ChangeState(ActorStateType.NONE);
}
