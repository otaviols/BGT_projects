using UnityEngine;

public class RAFInfo : UIBPopup
{
  public UIBButton m_okayButton;
  public UIBButton m_cancelButton;
  public UberText m_headlineText;
  public UberText m_messageText;
  public MultiSliceElement m_allSections;
  public GameObject m_midSection;
  private PegUIElement m_inputBlockerPegUIElement;

  protected override void Awake()
  {
    base.Awake();
    this.m_okayButton.SetText(GameStrings.Get("GLUE_RAF_INFO_MORE_INFO_BUTTON"));
    this.m_cancelButton.SetText(GameStrings.Get("GLUE_RAF_INFO_GOT_IT_BUTTON"));
    this.m_okayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOkayPressed));
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
  }

  private void OnDestroy() => this.Hide(false);

  public override void Show()
  {
    if (this.IsShown())
      return;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.Show(false);
    this.gameObject.SetActive(true);
    if ((Object) this.m_inputBlockerPegUIElement != (Object) null)
    {
      Object.Destroy((Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "RAFInfoInputBlocker");
    LayerUtils.SetLayer(inputBlocker, this.gameObject.layer);
    this.m_inputBlockerPegUIElement = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlockerPegUIElement.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerRelease));
    TransformUtil.SetPosY((Component) this.m_inputBlockerPegUIElement, this.gameObject.transform.position.y - 5f);
    RAFManager.Get().GetRAFFrame().DarkenInputBlocker(inputBlocker, 0.5f);
  }

  protected override void Hide(bool animate)
  {
    if (!this.IsShown())
      return;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    this.m_okayButton.SetEnabled(true);
    if ((Object) this.m_inputBlockerPegUIElement != (Object) null)
    {
      Object.Destroy((Object) this.m_inputBlockerPegUIElement.gameObject);
      this.m_inputBlockerPegUIElement = (PegUIElement) null;
    }
    this.gameObject.SetActive(false);
    base.Hide(animate);
  }

  private void OnOkayPressed(UIEvent e) => RAFManager.Get().GotoRAFWebsite();

  private void OnCancelPressed(UIEvent e) => this.Hide(true);

  private bool OnNavigateBack()
  {
    this.Hide(true);
    return true;
  }

  private void OnInputBlockerRelease(UIEvent e) => this.Hide(true);
}
