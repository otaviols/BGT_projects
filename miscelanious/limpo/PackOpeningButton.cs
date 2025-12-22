using UnityEngine;

public class PackOpeningButton : BoxMenuButton
{
  public UberText m_count;
  public GameObject m_countFrame;

  public string GetGetPackCount() => this.m_count.Text;

  public void SetPackCount(int packs)
  {
    if ((Object) this.m_countFrame == (Object) null || (Object) this.m_count == (Object) null)
      return;
    if (packs <= 0)
    {
      this.m_count.Text = "";
      this.m_countFrame.SetActive(false);
    }
    else
    {
      this.m_countFrame.SetActive(true);
      this.m_count.Text = GameStrings.Format("GLUE_PACK_OPENING_BOOSTER_COUNT", (object) packs);
    }
  }
}
