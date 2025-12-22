using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class RAFChest : PegUIElement
{
  public Renderer m_chestQuad;
  public GameObject m_tooltipBone;
  private bool m_isChestOpen;

  public void SetOpen(bool isChestOpen)
  {
    if (this.m_isChestOpen == isChestOpen)
      return;
    this.m_isChestOpen = isChestOpen;
    this.m_chestQuad.GetMaterial().SetTextureOffset("_MainTex", new Vector2(this.m_isChestOpen ? 0.5f : 0.0f, 0.5f));
    this.gameObject.GetComponent<UIBHighlight>().EnableResponse = !this.m_isChestOpen;
  }

  public bool IsOpen() => this.m_isChestOpen;
}
