using Blizzard.T5.MaterialService.Extensions;
using Hearthstone;
using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class FiresideGatheringChooserSubButton : ChooserSubButton
{
  [CustomEditField(Sections = "Fireside Gathering Sub Buttons")]
  public UIBButton m_officialRotationBrawlIcon;
  [CustomEditField(Sections = "Fireside Gathering Sub Buttons")]
  public List<Material> m_alternateMaterials;
  [CustomEditField(Sections = "Fireside Gathering Sub Buttons")]
  public Renderer m_buttonMesh;
  [CustomEditField(Sections = "Fireside Gathering Sub Buttons")]
  public int m_buttonFaceMaterialIndex;
  [CustomEditField(Sections = "Fireside Gathering Sub Buttons")]
  public GameObject m_TooltipBone;

  public FiresideGatheringManager.FiresideGatheringMode AssociatedMode { get; set; }

  public FormatType AssociatedFormatType { get; set; }

  public int AssociatedBrawlLibraryItemId { get; set; }

  protected override void Awake()
  {
    base.Awake();
    this.m_officialRotationBrawlIcon.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnHearthstonIconOver));
    this.m_officialRotationBrawlIcon.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHearthstonIconOut));
  }

  protected override void OnDestroy()
  {
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((Object) hearthstoneApplication != (Object) null)
      hearthstoneApplication.UnloadUnusedAssets();
    this.m_officialRotationBrawlIcon.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnHearthstonIconOver));
    this.m_officialRotationBrawlIcon.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHearthstonIconOut));
    base.OnDestroy();
  }

  public void SetOfficialBrawlRotationIcon(bool active) => this.m_officialRotationBrawlIcon.gameObject.SetActive(active);

  public void SetMaterialFromButtonIndex(int index)
  {
    if (this.m_alternateMaterials == null || this.m_alternateMaterials.Count == 0)
      return;
    this.m_buttonMesh.SetMaterial(this.m_buttonFaceMaterialIndex, this.m_alternateMaterials[index % this.m_alternateMaterials.Count]);
  }

  private void OnHearthstonIconOver(UIEvent e)
  {
    TooltipZone component = this.m_TooltipBone.GetComponent<TooltipZone>();
    if ((Object) component == (Object) null)
      return;
    component.ShowTooltip(GameStrings.Get("GLUE_FIRESIDE_GATHERING_BRAWL"), GameStrings.Get("GLUE_FIRESIDE_GATHERING_OFFICIAL_FIRESIDE_DESCRIPTION"), 4f);
  }

  private void OnHearthstonIconOut(UIEvent e)
  {
    TooltipZone component = this.m_TooltipBone.GetComponent<TooltipZone>();
    if (!((Object) component != (Object) null))
      return;
    component.HideTooltip();
  }
}
