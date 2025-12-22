using UnityEngine;

[CustomEditClass]
public class UIBInfoButton : PegUIElement
{
  private const float RAISE_TIME = 0.1f;
  private const float DEPRESS_TIME = 0.1f;
  [CustomEditField(Sections = "Button Objects")]
  [SerializeField]
  public GameObject m_RootObject;
  [CustomEditField(Sections = "Button Objects")]
  [SerializeField]
  public Transform m_UpBone;
  [CustomEditField(Sections = "Button Objects")]
  [SerializeField]
  public Transform m_DownBone;
  [CustomEditField(Sections = "Highlight")]
  [SerializeField]
  public GameObject m_Highlight;
  private UIBHighlight m_UIBHighlight;

  protected override void Awake()
  {
    base.Awake();
    UIBHighlight uibHighlight = this.GetComponent<UIBHighlight>();
    if ((Object) uibHighlight == (Object) null)
      uibHighlight = this.gameObject.AddComponent<UIBHighlight>();
    this.m_UIBHighlight = uibHighlight;
    if (!((Object) this.m_UIBHighlight != (Object) null))
      return;
    this.m_UIBHighlight.m_MouseOverHighlight = this.m_Highlight;
    this.m_UIBHighlight.m_HideMouseOverOnPress = false;
  }

  public void Select() => this.Depress();

  public void Deselect() => this.Raise();

  private void Raise() => iTween.MoveTo(this.m_RootObject, iTween.Hash((object) "position", (object) this.m_UpBone.localPosition, (object) "time", (object) 0.1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));

  private void Depress() => iTween.MoveTo(this.m_RootObject, iTween.Hash((object) "position", (object) this.m_DownBone.localPosition, (object) "time", (object) 0.1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));
}
