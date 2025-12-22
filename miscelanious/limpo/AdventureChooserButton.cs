using UnityEngine;

[CustomEditClass]
public class AdventureChooserButton : ChooserButton
{
  private AdventureDbId m_AdventureId;

  public void SetAdventure(AdventureDbId id) => this.m_AdventureId = id;

  public AdventureDbId GetAdventure() => this.m_AdventureId;

  public AdventureChooserSubButton CreateSubButton(
    AdventureDbId adventureDbId,
    AdventureModeDbId adventureModeDbId,
    AdventureSubDef subDef,
    string subButtonPrefab,
    bool useAsLastSelected)
  {
    ChooserSubButton subButton1 = this.CreateSubButton(subButtonPrefab, useAsLastSelected);
    AdventureChooserSubButton subButton2 = (Object) subButton1 != (Object) null ? (AdventureChooserSubButton) subButton1 : (AdventureChooserSubButton) null;
    if ((Object) subButton2 == (Object) null)
    {
      Debug.LogError((object) "newAdvSubButton cannot be null. Unable to create newAdvSubButton.", (Object) this);
      return (AdventureChooserSubButton) null;
    }
    string str = subDef.GetShortName();
    if (!AdventureConfig.CanPlayMode(this.m_AdventureId, adventureModeDbId, false) && !string.IsNullOrEmpty(subDef.GetLockedShortName()))
      str = subDef.GetLockedShortName();
    subButton2.gameObject.name = string.Format("{0}_{1}", (object) subButton2.gameObject.name, (object) adventureModeDbId);
    subButton2.SetAdventure(adventureDbId, adventureModeDbId);
    subButton2.SetButtonText(str);
    subButton2.SetPortraitTexture(subDef.m_Texture);
    subButton2.SetPortraitTiling(subDef.m_TextureTiling);
    subButton2.SetPortraitOffset(subDef.m_TextureOffset);
    return subButton2;
  }

  public AdventureChooserSubButton CreateComingSoonSubButton(
    AdventureModeDbId adventureModeDbId,
    string comingSoonSubButtonPrefab)
  {
    ChooserSubButton subButton = this.CreateSubButton(comingSoonSubButtonPrefab, true);
    AdventureChooserSubButton comingSoonSubButton = (Object) subButton != (Object) null ? (AdventureChooserSubButton) subButton : (AdventureChooserSubButton) null;
    if ((Object) comingSoonSubButton == (Object) null)
    {
      Debug.LogError((object) "comingSoonSubButton cannot be null. Unable to create comingSoonSubButton.", (Object) this);
      return (AdventureChooserSubButton) null;
    }
    comingSoonSubButton.SetEnabled(false);
    string str = GameStrings.Get("GLOBAL_DATETIME_COMING_SOON");
    this.SubButtonHeight = comingSoonSubButton.m_ComingSoonBannerHeightOverride;
    comingSoonSubButton.SetAdventure(this.m_AdventureId, adventureModeDbId);
    comingSoonSubButton.SetButtonText(str);
    return comingSoonSubButton;
  }
}
