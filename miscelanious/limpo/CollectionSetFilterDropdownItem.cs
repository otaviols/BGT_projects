using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CollectionSetFilterDropdownItem : PegUIElement
{
  public UberText m_dropdownText;
  public MeshRenderer m_iconRenderer;
  public GameObject m_mouseOverBar;
  public GameObject m_selectedBar;
  public Color m_mouseOverColor;
  public Color m_selectedColor;
  public Color m_unselectedColor;
  private bool m_selected;
  private Vector2? m_iconMaterialOffset;

  public Vector2? GetIconMaterialOffset() => this.m_iconMaterialOffset;

  public void Select(bool selection)
  {
    this.m_selected = selection;
    this.SetItemColors(this.m_selected ? this.m_selectedColor : this.m_unselectedColor);
    this.m_selectedBar.SetActive(selection);
    if (!this.m_selected)
      return;
    this.m_mouseOverBar.SetActive(false);
  }

  public void SetName(string name) => this.m_dropdownText.Text = name;

  public void SetIconMaterialOffset(Vector2 offset)
  {
    this.m_iconMaterialOffset = new Vector2?(offset);
    this.m_iconRenderer.GetMaterial().SetTextureOffset("_MainTex", offset);
  }

  public void DisableIconMaterial()
  {
    this.m_iconMaterialOffset = new Vector2?();
    this.m_iconRenderer.GetMaterial().SetTextureScale("_MainTex", Vector2.zero);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if (this.m_selected)
      return;
    this.SetItemColors(this.m_mouseOverColor);
    this.m_mouseOverBar.SetActive(true);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    this.m_mouseOverBar.SetActive(false);
    this.SetItemColors(this.m_selected ? this.m_selectedColor : this.m_unselectedColor);
  }

  private void SetItemColors(Color color)
  {
    this.m_iconRenderer.GetMaterial().SetColor("_Color", color);
    this.m_dropdownText.TextColor = color;
  }
}
