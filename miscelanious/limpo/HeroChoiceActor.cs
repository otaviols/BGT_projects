using UnityEngine;

public class HeroChoiceActor : Actor
{
  public UberText m_nameText;

  public void SetNameText(string text)
  {
    if (!((Object) this.m_nameText != (Object) null))
      return;
    this.m_nameText.Text = text;
  }

  public void SetNameTextActive(bool active)
  {
    if (!((Object) this.m_nameText != (Object) null))
      return;
    this.m_nameText.gameObject.SetActive(active);
  }

  protected override void ShowImpl(bool ignoreSpells)
  {
    base.ShowImpl(ignoreSpells);
    if (!((Object) this.m_nameTextMesh != (Object) null))
      return;
    this.m_nameTextMesh.gameObject.SetActive(false);
    if (!(bool) (Object) this.m_nameTextMesh.RenderOnObject)
      return;
    this.m_nameTextMesh.RenderOnObject.GetComponent<Renderer>().enabled = false;
  }
}
