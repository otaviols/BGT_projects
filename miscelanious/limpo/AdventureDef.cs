using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureDef : MonoBehaviour
{
  [CustomEditField(Sections = "Reward Banners")]
  public AdventureDef.BannerRewardType m_BannerRewardType;
  [CustomEditField(Sections = "Reward Banners", T = EditType.GAME_OBJECT)]
  public string m_BannerRewardPrefab;
  [CustomEditField(Sections = "Quotes", T = EditType.GAME_OBJECT)]
  public string m_AdventureCompleteQuotePrefab;
  [CustomEditField(Sections = "Quotes", T = EditType.GAME_OBJECT)]
  public string m_AdventureCompleteQuoteVOLine;
  [CustomEditField(Sections = "Quotes", T = EditType.GAME_OBJECT)]
  public string m_AdventureEntryQuotePrefab;
  [CustomEditField(Sections = "Quotes", T = EditType.GAME_OBJECT)]
  public string m_AdventureEntryQuoteVOLine;
  [CustomEditField(Sections = "Banners", T = EditType.GAME_OBJECT)]
  public string m_AdventureIntroBannerPrefab;
  [CustomEditField(Sections = "Banners", T = EditType.GAME_OBJECT)]
  public string m_AdventureDeckSelectionTutorialBannerPrefab;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_ProgressDisplayPrefab;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_WingBottomBorderPrefab;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_DefaultQuotePrefab;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_ChooserButtonPrefab;
  [CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
  public string m_ChooserSubButtonPrefab;
  [CustomEditField(Sections = "Chooser Button", T = EditType.TEXTURE)]
  public string m_Texture;
  [CustomEditField(Sections = "Chooser Button")]
  public Vector2 m_TextureTiling = Vector2.one;
  [CustomEditField(Sections = "Chooser Button")]
  public Vector2 m_TextureOffset = Vector2.zero;
  [CustomEditField(Sections = "Chooser Button")]
  public AdventureDbId m_AdventureToNestUnder;
  [CustomEditField(Sections = "Intro Conversation")]
  public bool m_ShouldOnlyPlayIntroOnFirstSeen;
  [CustomEditField(Sections = "Intro Conversation")]
  public List<AdventureDef.IntroConversationLine> m_IntroConversationLines;
  private AdventureDbId m_AdventureId;
  private string m_AdventureName;
  private Map<AdventureModeDbId, AdventureSubDef> m_SubDefs = new Map<AdventureModeDbId, AdventureSubDef>();
  private int m_SortOrder;

  public void Init(AdventureDbfRecord advRecord, List<AdventureDataDbfRecord> advDataRecords)
  {
    this.m_AdventureId = (AdventureDbId) advRecord.ID;
    this.m_AdventureName = (string) advRecord.Name;
    this.m_SortOrder = advRecord.SortOrder;
    foreach (AdventureDataDbfRecord advDataRecord in advDataRecords)
    {
      if ((AdventureDbId) advDataRecord.AdventureId == this.m_AdventureId)
      {
        string adventureSubDefPrefab = advDataRecord.AdventureSubDefPrefab;
        if (!string.IsNullOrEmpty(adventureSubDefPrefab))
        {
          GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) adventureSubDefPrefab);
          if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null))
          {
            AdventureSubDef component = gameObject.GetComponent<AdventureSubDef>();
            if ((UnityEngine.Object) component == (UnityEngine.Object) null)
            {
              Debug.LogError((object) string.Format("{0} object does not contain AdventureSubDef component.", (object) adventureSubDefPrefab));
              UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
            }
            else
            {
              component.Init(advDataRecord);
              this.m_SubDefs.Add(component.GetAdventureModeId(), component);
            }
          }
        }
      }
    }
  }

  public AdventureDbId GetAdventureId() => this.m_AdventureId;

  public string GetAdventureName() => this.m_AdventureName;

  public AdventureSubDef GetSubDef(AdventureModeDbId modeId)
  {
    AdventureSubDef subDef = (AdventureSubDef) null;
    this.m_SubDefs.TryGetValue(modeId, out subDef);
    return subDef;
  }

  public List<AdventureSubDef> GetSortedSubDefs()
  {
    List<AdventureSubDef> sortedSubDefs = new List<AdventureSubDef>((IEnumerable<AdventureSubDef>) this.m_SubDefs.Values);
    sortedSubDefs.Sort((Comparison<AdventureSubDef>) ((l, r) => l.GetSortOrder() - r.GetSortOrder()));
    return sortedSubDefs;
  }

  public int GetSortOrder() => this.m_SortOrder;

  public bool IsActiveAndPlayable()
  {
    foreach (WingDbfRecord record in GameDbf.Wing.GetRecords())
    {
      if ((AdventureDbId) record.AdventureId == this.GetAdventureId() && AdventureProgressMgr.IsWingEventActive(record.ID))
        return true;
    }
    return false;
  }

  public bool IsNestedUnderAnotherAdventureOnChooserScreen => this.m_AdventureToNestUnder != 0;

  public enum BannerRewardType
  {
    AdventureCompleteReward,
    BannerManagerPopup,
  }

  [Serializable]
  public class IntroConversationLine
  {
    public string CharacterPrefab;
    public string VoLinePrefab;
  }
}
