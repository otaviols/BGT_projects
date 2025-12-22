using System.Collections.Generic;
using UnityEngine;

public class RegionMenu : ButtonListMenu
{
  public Transform m_menuBone;
  private List<UIBButton> m_buttons;
  protected string m_menuDefPrefabOverride = "ButtonListMenuDef_RegionMenu:a74fe28bd9261474dbc2b9493e2e14f6";

  protected override void Awake()
  {
    Debug.Log((object) "region menu awake!");
    this.m_menuDefPrefab = this.m_menuDefPrefabOverride;
    this.m_menuParent = this.m_menuBone;
    this.m_targetLayer = GameLayer.HighPriorityUI;
    base.Awake();
    this.m_menu.m_headerText.Text = GameStrings.Get("GLUE_PICK_A_REGION");
    this.gameObject.SetActive(false);
  }

  public void SetButtons(List<UIBButton> buttons) => this.m_buttons = buttons;

  public override void Show(bool playSound = true) => base.Show(playSound);

  public override void Hide()
  {
    base.Hide();
    Object.Destroy((Object) this.gameObject);
  }

  protected override List<UIBButton> GetButtons() => this.m_buttons;
}
