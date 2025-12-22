using Blizzard.T5.MaterialService.Extensions;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureBossCoin : PegUIElement
{
  private const string s_EventCoinFlip = "Flip";
  public GameObject m_Coin;
  public MeshRenderer m_PortraitRenderer;
  public int m_PortraitMaterialIndex = 1;
  public GameObject m_Connector;
  public StateEventTable m_CoinStateTable;
  public PegUIElement m_DisabledCollider;
  private bool m_Enabled;
  private static bool neverRun;

  public void SetPortraitMaterial(AdventureBossDef bossDef)
  {
    MeshRenderer portraitRenderer = this.m_PortraitRenderer;
    List<Material> materials = portraitRenderer != null ? portraitRenderer.GetMaterials() : (List<Material>) null;
    if (materials == null || this.m_PortraitMaterialIndex >= materials.Count)
      return;
    Material material = bossDef.m_CoinPortraitMaterial.GetMaterial();
    materials[this.m_PortraitMaterialIndex] = material;
    this.m_PortraitRenderer.SetMaterials(materials);
  }

  public void ShowConnector(bool show)
  {
    if (!((Object) this.m_Connector != (Object) null))
      return;
    this.m_Connector.SetActive(show);
  }

  public void Enable(bool flag, bool animate = true)
  {
    this.GetComponent<Collider>().enabled = flag;
    if ((Object) this.m_DisabledCollider != (Object) null)
      this.m_DisabledCollider.gameObject.SetActive(!flag);
    if (this.m_Enabled == flag)
      return;
    this.m_Enabled = flag;
    if (animate & flag)
    {
      this.ShowCoin(false);
      this.m_CoinStateTable.TriggerState("Flip");
    }
    else
      this.ShowCoin(flag);
  }

  public void Select(bool selected)
  {
    UIBHighlight component = this.GetComponent<UIBHighlight>();
    if ((Object) component == (Object) null)
      return;
    component.AlwaysOver = selected;
    if (!selected)
      return;
    this.EnableFancyHighlight(false);
  }

  public void HighlightOnce()
  {
    UIBHighlight component = this.GetComponent<UIBHighlight>();
    if ((Object) component == (Object) null)
      return;
    component.HighlightOnce();
  }

  public void ShowNewLookGlow() => this.EnableFancyHighlight(true);

  private void EnableFancyHighlight(bool enable)
  {
    UIBHighlightStateControl component = this.GetComponent<UIBHighlightStateControl>();
    if ((Object) component == (Object) null)
      return;
    component.Select(enable);
  }

  private void ShowCoin(bool show)
  {
    if ((Object) this.m_Coin == (Object) null)
      return;
    TransformUtil.SetEulerAngleZ(this.m_Coin, show ? 0.0f : -180f);
  }

  private void Update()
  {
    if (!AdventureBossCoin.neverRun)
      return;
    Debug.Log((object) "TEST");
  }
}
