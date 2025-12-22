using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchieveDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private Achieve.Type m_achType = Achieve.ParseTypeValue("invalid");
  [SerializeField]
  private bool m_enabled = true;
  [SerializeField]
  private string m_parentAch;
  [SerializeField]
  private string m_linkTo;
  [SerializeField]
  private int m_sharedAchieveId;
  [SerializeField]
  private Achieve.ClientFlags m_clientFlags;
  [SerializeField]
  private Achieve.Trigger m_triggered = Achieve.ParseTriggerValue("none");
  [SerializeField]
  private int m_achQuota;
  [SerializeField]
  private Achieve.GameMode m_gameMode = Achieve.ParseGameModeValue("any");
  [SerializeField]
  private int m_raceId;
  [SerializeField]
  private int m_cardSetId;
  [SerializeField]
  private int m_myHeroClassId;
  [SerializeField]
  private int m_enemyHeroClassId;
  [SerializeField]
  private int m_maxDefense;
  [SerializeField]
  private Achieve.PlayerType m_playerType = Achieve.ParsePlayerTypeValue("any");
  [SerializeField]
  private int m_leagueVersionMin;
  [SerializeField]
  private int m_leagueVersionMax;
  [SerializeField]
  private int m_scenarioId;
  [SerializeField]
  private int m_adventureId;
  [SerializeField]
  private int m_adventureModeId;
  [SerializeField]
  private int m_adventureWingId;
  [SerializeField]
  private int m_boosterId;
  [SerializeField]
  private Achieve.RewardTiming m_rewardTiming = Achieve.ParseRewardTimingValue("immediate");
  [SerializeField]
  private string m_reward = "none";
  [SerializeField]
  private long m_rewardData1;
  [SerializeField]
  private long m_rewardData2;
  [SerializeField]
  private Achieve.Unlocks m_unlocks = Achieve.ParseUnlocksValue("none");
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private Achieve.AltTextPredicate m_altTextPredicate = Achieve.ParseAltTextPredicateValue("none");
  [SerializeField]
  private DbfLocValue m_altName;
  [SerializeField]
  private DbfLocValue m_altDescription;
  [SerializeField]
  private string m_customVisualWidget;
  [SerializeField]
  private bool m_useGenericRewardVisual;
  [SerializeField]
  private Achieve.ShowToReturningPlayer m_showToReturningPlayer = Achieve.ParseShowToReturningPlayerValue("always");
  [SerializeField]
  private int m_questDialogId;
  [SerializeField]
  private bool m_autoDestroy;
  [SerializeField]
  private string m_questTilePrefab;
  [SerializeField]
  private Achieve.AttentionBlocker m_attentionBlocker = Achieve.ParseAttentionBlockerValue("NONE");
  [SerializeField]
  private bool m_enabledWithProgression = true;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("ACH_TYPE")]
  public Achieve.Type AchType => this.m_achType;

  [DbfField("ENABLED")]
  public bool Enabled => this.m_enabled;

  [DbfField("PARENT_ACH")]
  public string ParentAch => this.m_parentAch;

  [DbfField("LINK_TO")]
  public string LinkTo => this.m_linkTo;

  [DbfField("SHARED_ACHIEVE_ID")]
  public int SharedAchieveId => this.m_sharedAchieveId;

  [DbfField("CLIENT_FLAGS")]
  public Achieve.ClientFlags ClientFlags => this.m_clientFlags;

  [DbfField("TRIGGERED")]
  public Achieve.Trigger Triggered => this.m_triggered;

  [DbfField("ACH_QUOTA")]
  public int AchQuota => this.m_achQuota;

  [DbfField("GAME_MODE")]
  public Achieve.GameMode GameMode => this.m_gameMode;

  [DbfField("RACE")]
  public int Race => this.m_raceId;

  [DbfField("CARD_SET")]
  public int CardSet => this.m_cardSetId;

  [DbfField("MY_HERO_CLASS_ID")]
  public int MyHeroClassId => this.m_myHeroClassId;

  [DbfField("ENEMY_HERO_CLASS_ID")]
  public int EnemyHeroClassId => this.m_enemyHeroClassId;

  [DbfField("MAX_DEFENSE")]
  public int MaxDefense => this.m_maxDefense;

  [DbfField("PLAYER_TYPE")]
  public Achieve.PlayerType PlayerType => this.m_playerType;

  [DbfField("ADVENTURE_ID")]
  public int AdventureId => this.m_adventureId;

  [DbfField("ADVENTURE_MODE_ID")]
  public int AdventureModeId => this.m_adventureModeId;

  [DbfField("ADVENTURE_WING_ID")]
  public int AdventureWingId => this.m_adventureWingId;

  [DbfField("BOOSTER")]
  public int Booster => this.m_boosterId;

  [DbfField("REWARD_TIMING")]
  public Achieve.RewardTiming RewardTiming => this.m_rewardTiming;

  [DbfField("REWARD")]
  public string Reward => this.m_reward;

  [DbfField("REWARD_DATA1")]
  public long RewardData1 => this.m_rewardData1;

  [DbfField("REWARD_DATA2")]
  public long RewardData2 => this.m_rewardData2;

  [DbfField("UNLOCKS")]
  public Achieve.Unlocks Unlocks => this.m_unlocks;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("ALT_TEXT_PREDICATE")]
  public Achieve.AltTextPredicate AltTextPredicate => this.m_altTextPredicate;

  [DbfField("ALT_NAME")]
  public DbfLocValue AltName => this.m_altName;

  [DbfField("ALT_DESCRIPTION")]
  public DbfLocValue AltDescription => this.m_altDescription;

  [DbfField("CUSTOM_VISUAL_WIDGET")]
  public string CustomVisualWidget => this.m_customVisualWidget;

  [DbfField("USE_GENERIC_REWARD_VISUAL")]
  public bool UseGenericRewardVisual => this.m_useGenericRewardVisual;

  [DbfField("SHOW_TO_RETURNING_PLAYER")]
  public Achieve.ShowToReturningPlayer ShowToReturningPlayer => this.m_showToReturningPlayer;

  [DbfField("QUEST_DIALOG_ID")]
  public int QuestDialogId => this.m_questDialogId;

  [DbfField("AUTO_DESTROY")]
  public bool AutoDestroy => this.m_autoDestroy;

  [DbfField("QUEST_TILE_PREFAB")]
  public string QuestTilePrefab => this.m_questTilePrefab;

  [DbfField("ATTENTION_BLOCKER")]
  public Achieve.AttentionBlocker AttentionBlocker => this.m_attentionBlocker;

  [DbfField("ENABLED_WITH_PROGRESSION")]
  public bool EnabledWithProgression => this.m_enabledWithProgression;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ACH_QUOTA":
        return (object) this.m_achQuota;
      case "ACH_TYPE":
        return (object) this.m_achType;
      case "ADVENTURE_ID":
        return (object) this.m_adventureId;
      case "ADVENTURE_MODE_ID":
        return (object) this.m_adventureModeId;
      case "ADVENTURE_WING_ID":
        return (object) this.m_adventureWingId;
      case "ALT_DESCRIPTION":
        return (object) this.m_altDescription;
      case "ALT_NAME":
        return (object) this.m_altName;
      case "ALT_TEXT_PREDICATE":
        return (object) this.m_altTextPredicate;
      case "ATTENTION_BLOCKER":
        return (object) this.m_attentionBlocker;
      case "AUTO_DESTROY":
        return (object) this.m_autoDestroy;
      case "BOOSTER":
        return (object) this.m_boosterId;
      case "CARD_SET":
        return (object) this.m_cardSetId;
      case "CLIENT_FLAGS":
        return (object) this.m_clientFlags;
      case "CUSTOM_VISUAL_WIDGET":
        return (object) this.m_customVisualWidget;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "ENABLED":
        return (object) this.m_enabled;
      case "ENABLED_WITH_PROGRESSION":
        return (object) this.m_enabledWithProgression;
      case "ENEMY_HERO_CLASS_ID":
        return (object) this.m_enemyHeroClassId;
      case "GAME_MODE":
        return (object) this.m_gameMode;
      case "ID":
        return (object) this.ID;
      case "LEAGUE_VERSION_MAX":
        return (object) this.m_leagueVersionMax;
      case "LEAGUE_VERSION_MIN":
        return (object) this.m_leagueVersionMin;
      case "LINK_TO":
        return (object) this.m_linkTo;
      case "MAX_DEFENSE":
        return (object) this.m_maxDefense;
      case "MY_HERO_CLASS_ID":
        return (object) this.m_myHeroClassId;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "PARENT_ACH":
        return (object) this.m_parentAch;
      case "PLAYER_TYPE":
        return (object) this.m_playerType;
      case "QUEST_DIALOG_ID":
        return (object) this.m_questDialogId;
      case "QUEST_TILE_PREFAB":
        return (object) this.m_questTilePrefab;
      case "RACE":
        return (object) this.m_raceId;
      case "REWARD":
        return (object) this.m_reward;
      case "REWARD_DATA1":
        return (object) this.m_rewardData1;
      case "REWARD_DATA2":
        return (object) this.m_rewardData2;
      case "REWARD_TIMING":
        return (object) this.m_rewardTiming;
      case "SCENARIO_ID":
        return (object) this.m_scenarioId;
      case "SHARED_ACHIEVE_ID":
        return (object) this.m_sharedAchieveId;
      case "SHOW_TO_RETURNING_PLAYER":
        return (object) this.m_showToReturningPlayer;
      case "TRIGGERED":
        return (object) this.m_triggered;
      case "UNLOCKS":
        return (object) this.m_unlocks;
      case "USE_GENERIC_REWARD_VISUAL":
        return (object) this.m_useGenericRewardVisual;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 190375133:
        if (!(name == "ADVENTURE_MODE_ID"))
          break;
        this.m_adventureModeId = (int) val;
        break;
      case 190718801:
        if (!(name == "ADVENTURE_ID"))
          break;
        this.m_adventureId = (int) val;
        break;
      case 378746893:
        if (!(name == "USE_GENERIC_REWARD_VISUAL"))
          break;
        this.m_useGenericRewardVisual = (bool) val;
        break;
      case 493513528:
        if (!(name == "ACH_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_achType = Achieve.Type.INVALID;
            return;
          case Achieve.Type _:
          case int _:
            this.m_achType = (Achieve.Type) val;
            return;
          case string _:
            this.m_achType = Achieve.ParseTypeValue((string) val);
            return;
          default:
            return;
        }
      case 655598188:
        if (!(name == "LEAGUE_VERSION_MIN"))
          break;
        this.m_leagueVersionMin = (int) val;
        break;
      case 679831291:
        if (!(name == "BOOSTER"))
          break;
        this.m_boosterId = (int) val;
        break;
      case 693605261:
        if (!(name == "SCENARIO_ID"))
          break;
        this.m_scenarioId = (int) val;
        break;
      case 787754026:
        if (!(name == "CLIENT_FLAGS"))
          break;
        switch (val)
        {
          case null:
            this.m_clientFlags = Achieve.ClientFlags.NONE;
            return;
          case Achieve.ClientFlags _:
          case int _:
            this.m_clientFlags = (Achieve.ClientFlags) val;
            return;
          case string _:
            this.m_clientFlags = Achieve.ParseClientFlagsValue((string) val);
            return;
          default:
            return;
        }
      case 829736880:
        if (!(name == "CARD_SET"))
          break;
        this.m_cardSetId = (int) val;
        break;
      case 902917601:
        if (!(name == "MY_HERO_CLASS_ID"))
          break;
        this.m_myHeroClassId = (int) val;
        break;
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1145534743:
        if (!(name == "LINK_TO"))
          break;
        this.m_linkTo = (string) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1421814995:
        if (!(name == "AUTO_DESTROY"))
          break;
        this.m_autoDestroy = (bool) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1521075980:
        if (!(name == "ATTENTION_BLOCKER"))
          break;
        switch (val)
        {
          case null:
            this.m_attentionBlocker = Achieve.AttentionBlocker.NONE;
            return;
          case Achieve.AttentionBlocker _:
          case int _:
            this.m_attentionBlocker = (Achieve.AttentionBlocker) val;
            return;
          case string _:
            this.m_attentionBlocker = Achieve.ParseAttentionBlockerValue((string) val);
            return;
          default:
            return;
        }
      case 1629023597:
        if (!(name == "ALT_DESCRIPTION"))
          break;
        this.m_altDescription = (DbfLocValue) val;
        break;
      case 1638382104:
        if (!(name == "ACH_QUOTA"))
          break;
        this.m_achQuota = (int) val;
        break;
      case 1832324731:
        if (!(name == "SHOW_TO_RETURNING_PLAYER"))
          break;
        switch (val)
        {
          case null:
            this.m_showToReturningPlayer = Achieve.ShowToReturningPlayer.ALWAYS;
            return;
          case Achieve.ShowToReturningPlayer _:
          case int _:
            this.m_showToReturningPlayer = (Achieve.ShowToReturningPlayer) val;
            return;
          case string _:
            this.m_showToReturningPlayer = Achieve.ParseShowToReturningPlayerValue((string) val);
            return;
          default:
            return;
        }
      case 1984182043:
        if (!(name == "GAME_MODE"))
          break;
        switch (val)
        {
          case null:
            this.m_gameMode = Achieve.GameMode.ANY;
            return;
          case Achieve.GameMode _:
          case int _:
            this.m_gameMode = (Achieve.GameMode) val;
            return;
          case string _:
            this.m_gameMode = Achieve.ParseGameModeValue((string) val);
            return;
          default:
            return;
        }
      case 2013650947:
        if (!(name == "ENABLED_WITH_PROGRESSION"))
          break;
        this.m_enabledWithProgression = (bool) val;
        break;
      case 2076288754:
        if (!(name == "UNLOCKS"))
          break;
        switch (val)
        {
          case null:
            this.m_unlocks = Achieve.Unlocks.FORGE;
            return;
          case Achieve.Unlocks _:
          case int _:
            this.m_unlocks = (Achieve.Unlocks) val;
            return;
          case string _:
            this.m_unlocks = Achieve.ParseUnlocksValue((string) val);
            return;
          default:
            return;
        }
      case 2099820198:
        if (!(name == "MAX_DEFENSE"))
          break;
        this.m_maxDefense = (int) val;
        break;
      case 2222469004:
        if (!(name == "CUSTOM_VISUAL_WIDGET"))
          break;
        this.m_customVisualWidget = (string) val;
        break;
      case 2294480894:
        if (!(name == "ENABLED"))
          break;
        this.m_enabled = (bool) val;
        break;
      case 2349802968:
        if (!(name == "RACE"))
          break;
        this.m_raceId = (int) val;
        break;
      case 2513575373:
        if (!(name == "ADVENTURE_WING_ID"))
          break;
        this.m_adventureWingId = (int) val;
        break;
      case 2537485753:
        if (!(name == "REWARD_TIMING"))
          break;
        switch (val)
        {
          case null:
            this.m_rewardTiming = Achieve.RewardTiming.IMMEDIATE;
            return;
          case Achieve.RewardTiming _:
          case int _:
            this.m_rewardTiming = (Achieve.RewardTiming) val;
            return;
          case string _:
            this.m_rewardTiming = Achieve.ParseRewardTimingValue((string) val);
            return;
          default:
            return;
        }
      case 2851436049:
        if (!(name == "QUEST_TILE_PREFAB"))
          break;
        this.m_questTilePrefab = (string) val;
        break;
      case 2951112623:
        if (!(name == "REWARD_DATA2"))
          break;
        this.m_rewardData2 = (long) val;
        break;
      case 2967890242:
        if (!(name == "REWARD_DATA1"))
          break;
        this.m_rewardData1 = (long) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3082479862:
        if (!(name == "QUEST_DIALOG_ID"))
          break;
        this.m_questDialogId = (int) val;
        break;
      case 3403422626:
        if (!(name == "TRIGGERED"))
          break;
        switch (val)
        {
          case null:
            this.m_triggered = Achieve.Trigger.UNKNOWN;
            return;
          case Achieve.Trigger _:
          case int _:
            this.m_triggered = (Achieve.Trigger) val;
            return;
          case string _:
            this.m_triggered = Achieve.ParseTriggerValue((string) val);
            return;
          default:
            return;
        }
      case 3531042190:
        if (!(name == "PARENT_ACH"))
          break;
        this.m_parentAch = (string) val;
        break;
      case 3638028361:
        if (!(name == "ENEMY_HERO_CLASS_ID"))
          break;
        this.m_enemyHeroClassId = (int) val;
        break;
      case 3661704248:
        if (!(name == "SHARED_ACHIEVE_ID"))
          break;
        this.m_sharedAchieveId = (int) val;
        break;
      case 3714292274:
        if (!(name == "ALT_NAME"))
          break;
        this.m_altName = (DbfLocValue) val;
        break;
      case 4057912964:
        if (!(name == "ALT_TEXT_PREDICATE"))
          break;
        switch (val)
        {
          case null:
            this.m_altTextPredicate = Achieve.AltTextPredicate.NONE;
            return;
          case Achieve.AltTextPredicate _:
          case int _:
            this.m_altTextPredicate = (Achieve.AltTextPredicate) val;
            return;
          case string _:
            this.m_altTextPredicate = Achieve.ParseAltTextPredicateValue((string) val);
            return;
          default:
            return;
        }
      case 4110404606:
        if (!(name == "LEAGUE_VERSION_MAX"))
          break;
        this.m_leagueVersionMax = (int) val;
        break;
      case 4274580549:
        if (!(name == "PLAYER_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_playerType = Achieve.PlayerType.ANY;
            return;
          case Achieve.PlayerType _:
          case int _:
            this.m_playerType = (Achieve.PlayerType) val;
            return;
          case string _:
            this.m_playerType = Achieve.ParsePlayerTypeValue((string) val);
            return;
          default:
            return;
        }
      case 4286938522:
        if (!(name == "REWARD"))
          break;
        this.m_reward = (string) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ACH_QUOTA":
        return typeof (int);
      case "ACH_TYPE":
        return typeof (Achieve.Type);
      case "ADVENTURE_ID":
        return typeof (int);
      case "ADVENTURE_MODE_ID":
        return typeof (int);
      case "ADVENTURE_WING_ID":
        return typeof (int);
      case "ALT_DESCRIPTION":
        return typeof (DbfLocValue);
      case "ALT_NAME":
        return typeof (DbfLocValue);
      case "ALT_TEXT_PREDICATE":
        return typeof (Achieve.AltTextPredicate);
      case "ATTENTION_BLOCKER":
        return typeof (Achieve.AttentionBlocker);
      case "AUTO_DESTROY":
        return typeof (bool);
      case "BOOSTER":
        return typeof (int);
      case "CARD_SET":
        return typeof (int);
      case "CLIENT_FLAGS":
        return typeof (Achieve.ClientFlags);
      case "CUSTOM_VISUAL_WIDGET":
        return typeof (string);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "ENABLED":
        return typeof (bool);
      case "ENABLED_WITH_PROGRESSION":
        return typeof (bool);
      case "ENEMY_HERO_CLASS_ID":
        return typeof (int);
      case "GAME_MODE":
        return typeof (Achieve.GameMode);
      case "ID":
        return typeof (int);
      case "LEAGUE_VERSION_MAX":
        return typeof (int);
      case "LEAGUE_VERSION_MIN":
        return typeof (int);
      case "LINK_TO":
        return typeof (string);
      case "MAX_DEFENSE":
        return typeof (int);
      case "MY_HERO_CLASS_ID":
        return typeof (int);
      case "NAME":
        return typeof (DbfLocValue);
      case "NOTE_DESC":
        return typeof (string);
      case "PARENT_ACH":
        return typeof (string);
      case "PLAYER_TYPE":
        return typeof (Achieve.PlayerType);
      case "QUEST_DIALOG_ID":
        return typeof (int);
      case "QUEST_TILE_PREFAB":
        return typeof (string);
      case "RACE":
        return typeof (int);
      case "REWARD":
        return typeof (string);
      case "REWARD_DATA1":
        return typeof (long);
      case "REWARD_DATA2":
        return typeof (long);
      case "REWARD_TIMING":
        return typeof (Achieve.RewardTiming);
      case "SCENARIO_ID":
        return typeof (int);
      case "SHARED_ACHIEVE_ID":
        return typeof (int);
      case "SHOW_TO_RETURNING_PLAYER":
        return typeof (Achieve.ShowToReturningPlayer);
      case "TRIGGERED":
        return typeof (Achieve.Trigger);
      case "UNLOCKS":
        return typeof (Achieve.Unlocks);
      case "USE_GENERIC_REWARD_VISUAL":
        return typeof (bool);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAchieveDbfRecords loadRecords = new LoadAchieveDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AchieveDbfAsset achieveDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AchieveDbfAsset)) as AchieveDbfAsset;
    if ((UnityEngine.Object) achieveDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AchieveDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < achieveDbfAsset.Records.Count; ++index)
      achieveDbfAsset.Records[index].StripUnusedLocales();
    records = achieveDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
    this.m_altName.StripUnusedLocales();
    this.m_altDescription.StripUnusedLocales();
  }
}
