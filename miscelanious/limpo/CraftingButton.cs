using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CraftingButton : PegUIElement
{
  public Material undoMaterial;
  public Material disabledMaterial;
  public Material enabledMaterial;
  public Material upgradeMaterial;
  public UberText labelText;
  public MeshRenderer buttonRenderer;
  public GameObject m_costObject;
  public Transform m_disabledCostBone;
  public Transform m_enabledCostBone;
  private CraftingButton.CraftingState m_craftingState;

  public virtual void DisableButton()
  {
    this.OnEnabled(false);
    this.SetCraftingState(CraftingButton.CraftingState.Disabled);
    this.buttonRenderer.SetMaterial(this.disabledMaterial);
    this.labelText.Text = "";
  }

  public virtual void EnterUndoMode()
  {
    this.OnEnabled(true);
    this.SetCraftingState(CraftingButton.CraftingState.Undo);
    this.buttonRenderer.SetMaterial(this.undoMaterial);
    this.labelText.Text = GameStrings.Get("GLUE_CRAFTING_UNDO");
  }

  public virtual void EnableButton()
  {
    this.OnEnabled(true);
    if ((this.m_craftingState == CraftingButton.CraftingState.Upgrade || this.m_craftingState == CraftingButton.CraftingState.CreateUpgrade) && (Object) this.upgradeMaterial != (Object) null)
      this.buttonRenderer.SetMaterial(this.upgradeMaterial);
    else
      this.buttonRenderer.SetMaterial(this.enabledMaterial);
  }

  public bool IsButtonEnabled() => this.gameObject.activeSelf;

  public CraftingButton.CraftingState GetCraftingState() => this.m_craftingState;

  public void SetCraftingState(CraftingButton.CraftingState state) => this.m_craftingState = state;

  private void OnEnabled(bool enable)
  {
    if ((Object) this.m_costObject != (Object) null)
    {
      if ((Object) this.m_enabledCostBone != (Object) null && (Object) this.m_disabledCostBone != (Object) null)
        this.m_costObject.transform.position = enable ? this.m_enabledCostBone.position : this.m_disabledCostBone.position;
      else
        this.m_costObject.SetActive(enable);
    }
    this.gameObject.SetActive(enable);
    this.gameObject.GetComponent<Collider>().enabled = enable;
  }

  public enum CraftingState
  {
    Disabled,
    Create,
    Disenchant,
    Undo,
    CreateUpgrade,
    Upgrade,
  }
}
