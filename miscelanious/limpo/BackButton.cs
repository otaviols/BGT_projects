using UnityEngine;

public class BackButton : PegUIElement
{
  public GameObject m_highlight;
  public UberText m_backText;
  public static KeyCode backKey;

  protected override void Awake()
  {
    base.Awake();
    this.SetOriginalLocalPosition();
    this.m_highlight.SetActive(false);
    if (!(bool) (Object) this.m_backText)
      return;
    this.m_backText.Text = GameStrings.Get("GLOBAL_BACK");
  }

  protected override void OnPress()
  {
    Vector3 originalLocalPosition = this.GetOriginalLocalPosition();
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) new Vector3(originalLocalPosition.x, originalLocalPosition.y - 0.3f, originalLocalPosition.z), (object) "isLocal", (object) true, (object) "time", (object) 0.15f));
  }

  protected override void OnRelease() => iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.GetOriginalLocalPosition(), (object) "isLocal", (object) true, (object) "time", (object) 0.15f));

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    Vector3 originalLocalPosition = this.GetOriginalLocalPosition();
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) new Vector3(originalLocalPosition.x, originalLocalPosition.y + 0.5f, originalLocalPosition.z), (object) "isLocal", (object) true, (object) "time", (object) 0.15f));
    this.m_highlight.SetActive(true);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) this.GetOriginalLocalPosition(), (object) "isLocal", (object) true, (object) "time", (object) 0.15f));
    this.m_highlight.SetActive(false);
  }
}
