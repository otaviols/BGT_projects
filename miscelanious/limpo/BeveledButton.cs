using UnityEngine;

public class BeveledButton : PegUIElement
{
  public TextMesh m_label;
  public UberText m_uberLabel;
  public GameObject m_highlight;

  protected override void Awake()
  {
    base.Awake();
    this.SetOriginalLocalPosition();
    this.m_highlight.SetActive(false);
  }

  protected override void OnPress()
  {
    Vector3 originalLocalPosition = this.GetOriginalLocalPosition();
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) new Vector3(originalLocalPosition.x, originalLocalPosition.y - 0.3f * this.transform.localScale.y, originalLocalPosition.z), (object) "isLocal", (object) true, (object) "time", (object) 0.15f));
  }

  protected override void OnRelease() => iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.GetOriginalLocalPosition(), (object) "isLocal", (object) true, (object) "time", (object) 0.15f));

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    Vector3 originalLocalPosition = this.GetOriginalLocalPosition();
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) new Vector3(originalLocalPosition.x, originalLocalPosition.y + 0.5f * this.transform.localScale.y, originalLocalPosition.z), (object) "isLocal", (object) true, (object) "time", (object) 0.15f));
    this.m_highlight.SetActive(true);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.GetOriginalLocalPosition(), (object) "isLocal", (object) true, (object) "time", (object) 0.15f));
    this.m_highlight.SetActive(false);
  }

  public void SetText(string text)
  {
    if ((Object) this.m_uberLabel != (Object) null)
      this.m_uberLabel.Text = text;
    else
      this.m_label.text = text;
  }

  public void Show()
  {
    this.gameObject.SetActive(true);
    this.m_highlight.SetActive(false);
  }

  public void Hide() => this.gameObject.SetActive(false);

  public UberText GetUberText() => this.m_uberLabel;
}
