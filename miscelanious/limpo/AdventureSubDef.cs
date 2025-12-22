using UnityEngine;

[CustomEditClass]
public class AdventureSubDef : MonoBehaviour
{
  [CustomEditField(Sections = "Mission Display", T = EditType.TEXTURE)]
  public string m_WatermarkTexture;
  [CustomEditField(Sections = "Chooser", T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_ChooserDescriptionPrefab;
  [CustomEditField(Sections = "Chooser", T = EditType.TEXTURE)]
  public string m_Texture;
  [CustomEditField(Sections = "Chooser")]
  public Vector2 m_TextureTiling = Vector2.one;
  [CustomEditField(Sections = "Chooser")]
  public Vector2 m_TextureOffset = Vector2.zero;
  private AdventureModeDbId m_AdventureModeId;
  private int m_SortOrder;
  private string m_ShortName;
  private string m_LockedShortName;
  private string m_Description;
  private string m_LockedDescription;
  private string m_RequirementsDescription;
  private string m_CompleteBannerText;

  public void Init(AdventureDataDbfRecord advDataRecord)
  {
    this.m_AdventureModeId = (AdventureModeDbId) advDataRecord.ModeId;
    this.m_SortOrder = advDataRecord.SortOrder;
    this.m_ShortName = (string) advDataRecord.ShortName;
    this.m_LockedShortName = (string) advDataRecord.LockedShortName;
    this.m_Description = (string) ((bool) UniversalInputManager.UsePhoneUI ? advDataRecord.ShortDescription : advDataRecord.Description);
    this.m_LockedDescription = (string) ((bool) UniversalInputManager.UsePhoneUI ? advDataRecord.LockedShortDescription : advDataRecord.LockedDescription);
    this.m_RequirementsDescription = (string) advDataRecord.RequirementsDescription;
    this.m_CompleteBannerText = (string) advDataRecord.CompleteBannerText;
  }

  public AdventureModeDbId GetAdventureModeId() => this.m_AdventureModeId;

  public int GetSortOrder() => this.m_SortOrder;

  public string GetShortName() => this.m_ShortName;

  public string GetLockedShortName() => this.m_LockedShortName;

  public string GetDescription() => this.m_Description;

  public string GetLockedDescription() => this.m_LockedDescription;

  public string GetRequirementsDescription() => this.m_RequirementsDescription;

  public string GetCompleteBannerText() => this.m_CompleteBannerText;
}
