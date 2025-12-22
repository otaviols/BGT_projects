using UnityEngine;

public class MeshSwapPegButton : PegUIElement
{
  public GameObject upState;
  public GameObject overState;
  public GameObject downState;
  public GameObject disabledState;
  public Vector3 downOffset;
  private Vector3 originalPosition;
  private Vector3 originalScale;
  private int m_buttonID;
  public TextMesh buttonText;

  protected override void Awake()
  {
    this.originalPosition = this.upState.transform.localPosition;
    base.Awake();
    this.SetState(PegUIElement.InteractionState.Up);
    Bounds boundsOfChildren = TransformUtil.GetBoundsOfChildren(this.gameObject);
    MeshRenderer component;
    if (!this.TryGetComponent<MeshRenderer>(out component))
      return;
    component.bounds.SetMinMax(boundsOfChildren.min, boundsOfChildren.max);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if (!this.gameObject.activeSelf)
      return;
    this.SetState(PegUIElement.InteractionState.Over);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    if (!this.gameObject.activeSelf)
      return;
    this.SetState(PegUIElement.InteractionState.Up);
  }

  protected override void OnPress()
  {
    if (!this.gameObject.activeSelf)
      return;
    this.SetState(PegUIElement.InteractionState.Down);
  }

  protected override void OnRelease()
  {
    if (!this.gameObject.activeSelf)
      return;
    this.SetState(PegUIElement.InteractionState.Over);
  }

  public void SetButtonText(string s) => this.buttonText.text = s;

  public void SetButtonID(int id) => this.m_buttonID = id;

  public int GetButtonID() => this.m_buttonID;

  public void SetState(PegUIElement.InteractionState state)
  {
    if ((Object) this.overState != (Object) null)
      this.overState.SetActive(false);
    if ((Object) this.disabledState != (Object) null)
      this.disabledState.SetActive(false);
    if ((Object) this.upState != (Object) null)
      this.upState.SetActive(false);
    if ((Object) this.downState != (Object) null)
      this.downState.SetActive(false);
    this.SetEnabled(true);
    switch (state)
    {
      case PegUIElement.InteractionState.Over:
        this.overState.SetActive(true);
        break;
      case PegUIElement.InteractionState.Down:
        this.downState.transform.localPosition = this.originalPosition + this.downOffset;
        this.downState.SetActive(true);
        break;
      case PegUIElement.InteractionState.Up:
        this.upState.SetActive(true);
        this.downState.transform.localPosition = this.originalPosition;
        break;
      case PegUIElement.InteractionState.Disabled:
        this.disabledState.SetActive(true);
        this.SetEnabled(false);
        break;
    }
  }

  public void Show()
  {
    this.gameObject.SetActive(true);
    this.SetState(PegUIElement.InteractionState.Up);
  }

  public void Hide() => this.gameObject.SetActive(false);
}
