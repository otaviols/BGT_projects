using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GlobalDbfRecord : DbfRecord
{
  [SerializeField]
  private Global.AssetFlags m_assetFlags = Global.AssetFlags.NOT_PACKAGED_IN_CLIENT;
  [SerializeField]
  private Global.PresenceStatus m_presenceStatus;
  [SerializeField]
  private Global.Region m_region;
  [SerializeField]
  private Global.FormatType m_formatType;
  [SerializeField]
  private Global.RewardType m_rewardType;
  [SerializeField]
  private Global.CardPremiumLevel m_cardPremiumLevel;
  [SerializeField]
  private Global.MissionEventType m_missionEventType;
  [SerializeField]
  private Global.BnetGameType m_bnetGameType;
  [SerializeField]
  private Global.SoundCategory m_soundCategory;
  [SerializeField]
  private Global.GameStringCategory m_gameStringCategory;
  [SerializeField]
  private Global.MercenariesPremium m_mercenariesPremium;
  [SerializeField]
  private Global.MercenaryRewardRuleFlag m_mercenaryRewardRuleFlag;
  [SerializeField]
  private Global.MercenaryRewardSourceFlag m_mercenaryRewardSourceFlag;
  [SerializeField]
  private Global.MercenariesBountyDifficulty m_mercenariesBountyDifficulty = Global.MercenariesBountyDifficulty.NORMAL;
  [SerializeField]
  private Global.CardEmoteEvent m_cardEmoteEvent;
  [SerializeField]
  private Global.RewardTrackType m_rewardTrackType;
  [SerializeField]
  private Global.Boardvisualstate m_boardvisualstate;
  [SerializeField]
  private Global.Baconcombatstep m_baconcombatstep;
  [SerializeField]
  private Global.LeagueType m_leagueType = Global.ParseLeagueTypeValue("unknown");
  [SerializeField]
  private Global.LettuceFaction m_lettuceFaction;

  [DbfField("ASSET_FLAGS")]
  public Global.AssetFlags AssetFlags => this.m_assetFlags;

  [DbfField("PRESENCE_STATUS")]
  public Global.PresenceStatus PresenceStatus => this.m_presenceStatus;

  [DbfField("REGION")]
  public Global.Region Region => this.m_region;

  [DbfField("FORMAT_TYPE")]
  public Global.FormatType FormatType => this.m_formatType;

  [DbfField("REWARD_TYPE")]
  public Global.RewardType RewardType => this.m_rewardType;

  [DbfField("CARD_PREMIUM_LEVEL")]
  public Global.CardPremiumLevel CardPremiumLevel => this.m_cardPremiumLevel;

  [DbfField("MISSION_EVENT_TYPE")]
  public Global.MissionEventType MissionEventType => this.m_missionEventType;

  [DbfField("BNET_GAME_TYPE")]
  public Global.BnetGameType BnetGameType => this.m_bnetGameType;

  [DbfField("SOUND_CATEGORY")]
  public Global.SoundCategory SoundCategory => this.m_soundCategory;

  [DbfField("GAME_STRING_CATEGORY")]
  public Global.GameStringCategory GameStringCategory => this.m_gameStringCategory;

  [DbfField("MERCENARIES_PREMIUM")]
  public Global.MercenariesPremium MercenariesPremium => this.m_mercenariesPremium;

  [DbfField("Mercenary_Reward_Rule_Flag")]
  public Global.MercenaryRewardRuleFlag MercenaryRewardRuleFlag => this.m_mercenaryRewardRuleFlag;

  [DbfField("Mercenary_Reward_Source_Flag")]
  public Global.MercenaryRewardSourceFlag MercenaryRewardSourceFlag => this.m_mercenaryRewardSourceFlag;

  [DbfField("Mercenaries_Bounty_Difficulty")]
  public Global.MercenariesBountyDifficulty MercenariesBountyDifficulty => this.m_mercenariesBountyDifficulty;

  [DbfField("Card_Emote_Event")]
  public Global.CardEmoteEvent CardEmoteEvent => this.m_cardEmoteEvent;

  [DbfField("REWARD_TRACK_TYPE")]
  public Global.RewardTrackType RewardTrackType => this.m_rewardTrackType;

  [DbfField("BoardVisualState")]
  public Global.Boardvisualstate Boardvisualstate => this.m_boardvisualstate;

  [DbfField("BaconCombatStep")]
  public Global.Baconcombatstep Baconcombatstep => this.m_baconcombatstep;

  [DbfField("LEAGUE_TYPE")]
  public Global.LeagueType LeagueType => this.m_leagueType;

  [DbfField("Lettuce_Faction")]
  public Global.LettuceFaction LettuceFaction => this.m_lettuceFaction;

  public void SetAssetFlags(Global.AssetFlags v) => this.m_assetFlags = v;

  public void SetPresenceStatus(Global.PresenceStatus v) => this.m_presenceStatus = v;

  public void SetRegion(Global.Region v) => this.m_region = v;

  public void SetFormatType(Global.FormatType v) => this.m_formatType = v;

  public void SetRewardType(Global.RewardType v) => this.m_rewardType = v;

  public void SetCardPremiumLevel(Global.CardPremiumLevel v) => this.m_cardPremiumLevel = v;

  public void SetMissionEventType(Global.MissionEventType v) => this.m_missionEventType = v;

  public void SetBnetGameType(Global.BnetGameType v) => this.m_bnetGameType = v;

  public void SetSoundCategory(Global.SoundCategory v) => this.m_soundCategory = v;

  public void SetGameStringCategory(Global.GameStringCategory v) => this.m_gameStringCategory = v;

  public void SetMercenariesPremium(Global.MercenariesPremium v) => this.m_mercenariesPremium = v;

  public void SetMercenaryRewardRuleFlag(Global.MercenaryRewardRuleFlag v) => this.m_mercenaryRewardRuleFlag = v;

  public void SetMercenaryRewardSourceFlag(Global.MercenaryRewardSourceFlag v) => this.m_mercenaryRewardSourceFlag = v;

  public void SetMercenariesBountyDifficulty(Global.MercenariesBountyDifficulty v) => this.m_mercenariesBountyDifficulty = v;

  public void SetCardEmoteEvent(Global.CardEmoteEvent v) => this.m_cardEmoteEvent = v;

  public void SetRewardTrackType(Global.RewardTrackType v) => this.m_rewardTrackType = v;

  public void SetBoardvisualstate(Global.Boardvisualstate v) => this.m_boardvisualstate = v;

  public void SetBaconcombatstep(Global.Baconcombatstep v) => this.m_baconcombatstep = v;

  public void SetLeagueType(Global.LeagueType v) => this.m_leagueType = v;

  public void SetLettuceFaction(Global.LettuceFaction v) => this.m_lettuceFaction = v;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ASSET_FLAGS":
        return (object) this.m_assetFlags;
      case "BNET_GAME_TYPE":
        return (object) this.m_bnetGameType;
      case "BaconCombatStep":
        return (object) this.m_baconcombatstep;
      case "BoardVisualState":
        return (object) this.m_boardvisualstate;
      case "CARD_PREMIUM_LEVEL":
        return (object) this.m_cardPremiumLevel;
      case "Card_Emote_Event":
        return (object) this.m_cardEmoteEvent;
      case "FORMAT_TYPE":
        return (object) this.m_formatType;
      case "GAME_STRING_CATEGORY":
        return (object) this.m_gameStringCategory;
      case "LEAGUE_TYPE":
        return (object) this.m_leagueType;
      case "Lettuce_Faction":
        return (object) this.m_lettuceFaction;
      case "MERCENARIES_PREMIUM":
        return (object) this.m_mercenariesPremium;
      case "MISSION_EVENT_TYPE":
        return (object) this.m_missionEventType;
      case "Mercenaries_Bounty_Difficulty":
        return (object) this.m_mercenariesBountyDifficulty;
      case "Mercenary_Reward_Rule_Flag":
        return (object) this.m_mercenaryRewardRuleFlag;
      case "Mercenary_Reward_Source_Flag":
        return (object) this.m_mercenaryRewardSourceFlag;
      case "PRESENCE_STATUS":
        return (object) this.m_presenceStatus;
      case "REGION":
        return (object) this.m_region;
      case "REWARD_TRACK_TYPE":
        return (object) this.m_rewardTrackType;
      case "REWARD_TYPE":
        return (object) this.m_rewardType;
      case "SOUND_CATEGORY":
        return (object) this.m_soundCategory;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 96691247:
        if (!(name == "FORMAT_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_formatType = Global.FormatType.FT_UNKNOWN;
            return;
          case Global.FormatType _:
          case int _:
            this.m_formatType = (Global.FormatType) val;
            return;
          case string _:
            this.m_formatType = Global.ParseFormatTypeValue((string) val);
            return;
          default:
            return;
        }
      case 393858368:
        if (!(name == "BNET_GAME_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_bnetGameType = Global.BnetGameType.BGT_UNKNOWN;
            return;
          case Global.BnetGameType _:
          case int _:
            this.m_bnetGameType = (Global.BnetGameType) val;
            return;
          case string _:
            this.m_bnetGameType = Global.ParseBnetGameTypeValue((string) val);
            return;
          default:
            return;
        }
      case 572669000:
        if (!(name == "BoardVisualState"))
          break;
        switch (val)
        {
          case null:
            this.m_boardvisualstate = Global.Boardvisualstate.NONE;
            return;
          case Global.Boardvisualstate _:
          case int _:
            this.m_boardvisualstate = (Global.Boardvisualstate) val;
            return;
          case string _:
            this.m_boardvisualstate = Global.ParseBoardvisualstateValue((string) val);
            return;
          default:
            return;
        }
      case 1019332525:
        if (!(name == "SOUND_CATEGORY"))
          break;
        switch (val)
        {
          case null:
            this.m_soundCategory = Global.SoundCategory.NONE;
            return;
          case Global.SoundCategory _:
          case int _:
            this.m_soundCategory = (Global.SoundCategory) val;
            return;
          case string _:
            this.m_soundCategory = Global.ParseSoundCategoryValue((string) val);
            return;
          default:
            return;
        }
      case 1042759642:
        if (!(name == "Lettuce_Faction"))
          break;
        switch (val)
        {
          case null:
            this.m_lettuceFaction = Global.LettuceFaction.NONE;
            return;
          case Global.LettuceFaction _:
          case int _:
            this.m_lettuceFaction = (Global.LettuceFaction) val;
            return;
          case string _:
            this.m_lettuceFaction = Global.ParseLettuceFactionValue((string) val);
            return;
          default:
            return;
        }
      case 1098446823:
        if (!(name == "REWARD_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_rewardType = Global.RewardType.NONE;
            return;
          case Global.RewardType _:
          case int _:
            this.m_rewardType = (Global.RewardType) val;
            return;
          case string _:
            this.m_rewardType = Global.ParseRewardTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1163148812:
        if (!(name == "Mercenary_Reward_Source_Flag"))
          break;
        switch (val)
        {
          case null:
            this.m_mercenaryRewardSourceFlag = Global.MercenaryRewardSourceFlag.NONE;
            return;
          case Global.MercenaryRewardSourceFlag _:
          case int _:
            this.m_mercenaryRewardSourceFlag = (Global.MercenaryRewardSourceFlag) val;
            return;
          case string _:
            this.m_mercenaryRewardSourceFlag = Global.ParseMercenaryRewardSourceFlagValue((string) val);
            return;
          default:
            return;
        }
      case 1351746555:
        if (!(name == "REWARD_TRACK_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_rewardTrackType = Global.RewardTrackType.NONE;
            return;
          case Global.RewardTrackType _:
          case int _:
            this.m_rewardTrackType = (Global.RewardTrackType) val;
            return;
          case string _:
            this.m_rewardTrackType = Global.ParseRewardTrackTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1399452099:
        if (!(name == "PRESENCE_STATUS"))
          break;
        switch (val)
        {
          case null:
            this.m_presenceStatus = Global.PresenceStatus.LOGIN;
            return;
          case Global.PresenceStatus _:
          case int _:
            this.m_presenceStatus = (Global.PresenceStatus) val;
            return;
          case string _:
            this.m_presenceStatus = Global.ParsePresenceStatusValue((string) val);
            return;
          default:
            return;
        }
      case 1539131483:
        if (!(name == "MISSION_EVENT_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_missionEventType = Global.MissionEventType.INVALID;
            return;
          case Global.MissionEventType _:
          case int _:
            this.m_missionEventType = (Global.MissionEventType) val;
            return;
          case string _:
            this.m_missionEventType = Global.ParseMissionEventTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1615316160:
        if (!(name == "CARD_PREMIUM_LEVEL"))
          break;
        switch (val)
        {
          case null:
            this.m_cardPremiumLevel = Global.CardPremiumLevel.NORMAL;
            return;
          case Global.CardPremiumLevel _:
          case int _:
            this.m_cardPremiumLevel = (Global.CardPremiumLevel) val;
            return;
          case string _:
            this.m_cardPremiumLevel = Global.ParseCardPremiumLevelValue((string) val);
            return;
          default:
            return;
        }
      case 1949384501:
        if (!(name == "LEAGUE_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_leagueType = Global.LeagueType.UNKNOWN;
            return;
          case Global.LeagueType _:
          case int _:
            this.m_leagueType = (Global.LeagueType) val;
            return;
          case string _:
            this.m_leagueType = Global.ParseLeagueTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1949502575:
        if (!(name == "Card_Emote_Event"))
          break;
        switch (val)
        {
          case null:
            this.m_cardEmoteEvent = Global.CardEmoteEvent.INVALID;
            return;
          case Global.CardEmoteEvent _:
          case int _:
            this.m_cardEmoteEvent = (Global.CardEmoteEvent) val;
            return;
          case string _:
            this.m_cardEmoteEvent = Global.ParseCardEmoteEventValue((string) val);
            return;
          default:
            return;
        }
      case 2109182889:
        if (!(name == "Mercenary_Reward_Rule_Flag"))
          break;
        switch (val)
        {
          case null:
            this.m_mercenaryRewardRuleFlag = Global.MercenaryRewardRuleFlag.NONE;
            return;
          case Global.MercenaryRewardRuleFlag _:
          case int _:
            this.m_mercenaryRewardRuleFlag = (Global.MercenaryRewardRuleFlag) val;
            return;
          case string _:
            this.m_mercenaryRewardRuleFlag = Global.ParseMercenaryRewardRuleFlagValue((string) val);
            return;
          default:
            return;
        }
      case 2334788369:
        if (!(name == "Mercenaries_Bounty_Difficulty"))
          break;
        switch (val)
        {
          case null:
            this.m_mercenariesBountyDifficulty = Global.MercenariesBountyDifficulty.NONE;
            return;
          case Global.MercenariesBountyDifficulty _:
          case int _:
            this.m_mercenariesBountyDifficulty = (Global.MercenariesBountyDifficulty) val;
            return;
          case string _:
            this.m_mercenariesBountyDifficulty = Global.ParseMercenariesBountyDifficultyValue((string) val);
            return;
          default:
            return;
        }
      case 2626823578:
        if (!(name == "BaconCombatStep"))
          break;
        switch (val)
        {
          case null:
            this.m_baconcombatstep = Global.Baconcombatstep.INVALID;
            return;
          case Global.Baconcombatstep _:
          case int _:
            this.m_baconcombatstep = (Global.Baconcombatstep) val;
            return;
          case string _:
            this.m_baconcombatstep = Global.ParseBaconcombatstepValue((string) val);
            return;
          default:
            return;
        }
      case 2674204159:
        if (!(name == "ASSET_FLAGS"))
          break;
        switch (val)
        {
          case null:
            this.m_assetFlags = Global.AssetFlags.NONE;
            return;
          case Global.AssetFlags _:
          case int _:
            this.m_assetFlags = (Global.AssetFlags) val;
            return;
          case string _:
            this.m_assetFlags = Global.ParseAssetFlagsValue((string) val);
            return;
          default:
            return;
        }
      case 2744607693:
        if (!(name == "MERCENARIES_PREMIUM"))
          break;
        switch (val)
        {
          case null:
            this.m_mercenariesPremium = Global.MercenariesPremium.PREMIUM_NORMAL;
            return;
          case Global.MercenariesPremium _:
          case int _:
            this.m_mercenariesPremium = (Global.MercenariesPremium) val;
            return;
          case string _:
            this.m_mercenariesPremium = Global.ParseMercenariesPremiumValue((string) val);
            return;
          default:
            return;
        }
      case 2906194274:
        if (!(name == "GAME_STRING_CATEGORY"))
          break;
        switch (val)
        {
          case null:
            this.m_gameStringCategory = Global.GameStringCategory.INVALID;
            return;
          case Global.GameStringCategory _:
          case int _:
            this.m_gameStringCategory = (Global.GameStringCategory) val;
            return;
          case string _:
            this.m_gameStringCategory = Global.ParseGameStringCategoryValue((string) val);
            return;
          default:
            return;
        }
      case 3781468093:
        if (!(name == "REGION"))
          break;
        switch (val)
        {
          case null:
            this.m_region = Global.Region.REGION_UNKNOWN;
            return;
          case Global.Region _:
          case int _:
            this.m_region = (Global.Region) val;
            return;
          case string _:
            this.m_region = Global.ParseRegionValue((string) val);
            return;
          default:
            return;
        }
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ASSET_FLAGS":
        return typeof (Global.AssetFlags);
      case "BNET_GAME_TYPE":
        return typeof (Global.BnetGameType);
      case "BaconCombatStep":
        return typeof (Global.Baconcombatstep);
      case "BoardVisualState":
        return typeof (Global.Boardvisualstate);
      case "CARD_PREMIUM_LEVEL":
        return typeof (Global.CardPremiumLevel);
      case "Card_Emote_Event":
        return typeof (Global.CardEmoteEvent);
      case "FORMAT_TYPE":
        return typeof (Global.FormatType);
      case "GAME_STRING_CATEGORY":
        return typeof (Global.GameStringCategory);
      case "LEAGUE_TYPE":
        return typeof (Global.LeagueType);
      case "Lettuce_Faction":
        return typeof (Global.LettuceFaction);
      case "MERCENARIES_PREMIUM":
        return typeof (Global.MercenariesPremium);
      case "MISSION_EVENT_TYPE":
        return typeof (Global.MissionEventType);
      case "Mercenaries_Bounty_Difficulty":
        return typeof (Global.MercenariesBountyDifficulty);
      case "Mercenary_Reward_Rule_Flag":
        return typeof (Global.MercenaryRewardRuleFlag);
      case "Mercenary_Reward_Source_Flag":
        return typeof (Global.MercenaryRewardSourceFlag);
      case "PRESENCE_STATUS":
        return typeof (Global.PresenceStatus);
      case "REGION":
        return typeof (Global.Region);
      case "REWARD_TRACK_TYPE":
        return typeof (Global.RewardTrackType);
      case "REWARD_TYPE":
        return typeof (Global.RewardType);
      case "SOUND_CATEGORY":
        return typeof (Global.SoundCategory);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadGlobalDbfRecords loadRecords = new LoadGlobalDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    GlobalDbfAsset globalDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (GlobalDbfAsset)) as GlobalDbfAsset;
    if ((UnityEngine.Object) globalDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("GlobalDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < globalDbfAsset.Records.Count; ++index)
      globalDbfAsset.Records[index].StripUnusedLocales();
    records = globalDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
