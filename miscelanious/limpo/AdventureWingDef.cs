using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureWingDef : MonoBehaviour
{
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride m_WingPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_CoinPrefab;
  public bool CoinsStartFaceUp;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_RewardsPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_UnlockSpellPrefab;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public string m_AccentPrefab;
  [CustomEditField(Sections = "Opening Quote", T = EditType.GAME_OBJECT)]
  public string m_OpenQuotePrefab;
  [CustomEditField(Sections = "Opening Quote", T = EditType.GAME_OBJECT)]
  public string m_OpenQuoteVOLine;
  [CustomEditField(Sections = "Opening Quote")]
  public float m_OpenQuoteDelay;
  [CustomEditField(Sections = "Opening Quote")]
  public bool m_PlayOpenQuoteInHeroic = true;
  [CustomEditField(Sections = "Wing Open Popup", T = EditType.GAME_OBJECT)]
  public string m_WingOpenPopup;
  [CustomEditField(Sections = "Complete Quote", T = EditType.GAME_OBJECT)]
  public string m_CompleteQuotePrefab;
  [CustomEditField(Sections = "Complete Quote", T = EditType.GAME_OBJECT)]
  public string m_CompleteQuoteVOLine;
  [CustomEditField(Sections = "Complete Quote", T = EditType.GAME_OBJECT)]
  public string m_CompleteQuoteNextWingLockedPrefab;
  [CustomEditField(Sections = "Complete Quote", T = EditType.GAME_OBJECT)]
  public string m_CompleteQuoteNextWingLockedVOLine;
  [CustomEditField(Sections = "Complete Quote", T = EditType.GAME_OBJECT)]
  public bool m_PlayCompleteQuoteInHeroic = true;
  [CustomEditField(Sections = "Rewards Preview")]
  public List<string> m_SpecificRewardsPreviewCards;
  [CustomEditField(Sections = "Rewards Preview")]
  public List<int> m_SpecificRewardsPreviewCardBacks;
  [CustomEditField(Sections = "Rewards Preview")]
  public List<BoosterDbId> m_SpecificRewardsPreviewBoosters;
  [CustomEditField(Sections = "Rewards Preview")]
  public int m_HiddenRewardsPreviewCount;
  [CustomEditField(Sections = "Loc Strings")]
  public string m_LockedLocString;
  [CustomEditField(Sections = "Loc Strings")]
  public string m_LockedPurchaseLocString;
  private AdventureDbId m_AdventureId;
  private WingDbId m_WingId;
  private WingDbId m_OwnershipPrereq;
  private int m_SortOrder;
  private int m_UnlockOrder;
  private string m_WingName;
  private string m_ComingSoonLabel;
  private string m_RequiresLabel;
  private WingDbId m_OpenPrereq;
  private string m_OpeningDiscouragedLabel;
  private string m_OpeningDiscouragedWarning;
  private bool m_MustCompleteOpenPrereq;
  private bool m_UnlocksAutomatically;

  public void Init(WingDbfRecord wingRecord)
  {
    this.m_AdventureId = (AdventureDbId) wingRecord.AdventureId;
    this.m_WingId = (WingDbId) wingRecord.ID;
    this.m_OwnershipPrereq = (WingDbId) wingRecord.OwnershipPrereqWingId;
    this.m_SortOrder = wingRecord.SortOrder;
    this.m_UnlockOrder = wingRecord.UnlockOrder;
    this.m_WingName = (string) wingRecord.Name;
    this.m_ComingSoonLabel = (string) wingRecord.ComingSoonLabel;
    this.m_RequiresLabel = (string) wingRecord.RequiresLabel;
    this.m_OpenPrereq = (WingDbId) wingRecord.OpenPrereqWingId;
    this.m_OpeningDiscouragedLabel = (string) wingRecord.OpenDiscouragedLabel;
    this.m_OpeningDiscouragedWarning = (string) wingRecord.OpenDiscouragedWarning;
    this.m_MustCompleteOpenPrereq = wingRecord.MustCompleteOpenPrereq;
    this.m_UnlocksAutomatically = wingRecord.UnlocksAutomatically;
  }

  public AdventureDbId GetAdventureId() => this.m_AdventureId;

  public WingDbId GetWingId() => this.m_WingId;

  public WingDbId GetOwnershipPrereqId() => this.m_OwnershipPrereq;

  public int GetSortOrder() => this.m_SortOrder;

  public int GetUnlockOrder() => this.m_UnlockOrder;

  public string GetWingName() => this.m_WingName;

  public string GetComingSoonLabel() => this.m_ComingSoonLabel;

  public string GetRequiresLabel() => this.m_RequiresLabel;

  public WingDbId GetOpenPrereqId() => this.m_OpenPrereq;

  public string GetOpeningNotRecommendedLabel() => this.m_OpeningDiscouragedLabel;

  public string GetOpeningNotRecommendedWarning() => this.m_OpeningDiscouragedWarning;

  public bool GetMustCompleteOpenPrereq() => this.m_MustCompleteOpenPrereq;

  public bool GetUnlocksAutomatically() => this.m_UnlocksAutomatically;
}
