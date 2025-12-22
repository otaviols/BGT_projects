using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScenarioDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_players;
  [SerializeField]
  private int m_player1HeroCardId;
  [SerializeField]
  private int m_player2HeroCardId;
  [SerializeField]
  private bool m_isTutorial;
  [SerializeField]
  private bool m_isExpert = true;
  [SerializeField]
  private bool m_isCoop;
  [SerializeField]
  private bool m_oneSimPerPlayer;
  [SerializeField]
  private Scenario.BoardLayout m_boardLayout = Scenario.ParseBoardLayoutValue("standard");
  [SerializeField]
  private int m_adventureId;
  [SerializeField]
  private int m_wingId;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private int m_modeId;
  [SerializeField]
  private int m_clientPlayer2HeroCardId;
  [SerializeField]
  private int m_clientPlayer2HeroPowerCardId;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_shortName;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private DbfLocValue m_shortDescription;
  [SerializeField]
  private DbfLocValue m_opponentName;
  [SerializeField]
  private DbfLocValue m_completedDescription;
  [SerializeField]
  private int m_player1DeckId;
  [SerializeField]
  private int m_deckRulesetId;
  [SerializeField]
  private Scenario.RuleType m_ruleType;
  [SerializeField]
  private DbfLocValue m_chooseHeroText;
  [SerializeField]
  private string m_tbTexture;
  [SerializeField]
  private string m_tbTexturePhone;
  [SerializeField]
  private double m_tbTexturePhoneOffsetY;
  [SerializeField]
  private int m_gameSaveDataProgressSubkeyId;
  [SerializeField]
  private int m_gameSaveDataProgressMax;
  [SerializeField]
  private bool m_hideBossHeroPowerInUi;
  [SerializeField]
  private string m_scriptObject;

  [DbfField("PLAYERS")]
  public int Players => this.m_players;

  [DbfField("PLAYER1_HERO_CARD_ID")]
  public int Player1HeroCardId => this.m_player1HeroCardId;

  [DbfField("PLAYER2_HERO_CARD_ID")]
  public int Player2HeroCardId => this.m_player2HeroCardId;

  [DbfField("IS_COOP")]
  public bool IsCoop => this.m_isCoop;

  [DbfField("ADVENTURE_ID")]
  public int AdventureId => this.m_adventureId;

  [DbfField("WING_ID")]
  public int WingId => this.m_wingId;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("MODE_ID")]
  public int ModeId => this.m_modeId;

  [DbfField("CLIENT_PLAYER2_HERO_CARD_ID")]
  public int ClientPlayer2HeroCardId => this.m_clientPlayer2HeroCardId;

  [DbfField("CLIENT_PLAYER2_HERO_POWER_CARD_ID")]
  public int ClientPlayer2HeroPowerCardId => this.m_clientPlayer2HeroPowerCardId;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("SHORT_NAME")]
  public DbfLocValue ShortName => this.m_shortName;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("SHORT_DESCRIPTION")]
  public DbfLocValue ShortDescription => this.m_shortDescription;

  [DbfField("OPPONENT_NAME")]
  public DbfLocValue OpponentName => this.m_opponentName;

  [DbfField("COMPLETED_DESCRIPTION")]
  public DbfLocValue CompletedDescription => this.m_completedDescription;

  [DbfField("PLAYER1_DECK_ID")]
  public int Player1DeckId => this.m_player1DeckId;

  [DbfField("DECK_RULESET_ID")]
  public int DeckRulesetId => this.m_deckRulesetId;

  [DbfField("RULE_TYPE")]
  public Scenario.RuleType RuleType => this.m_ruleType;

  [DbfField("CHOOSE_HERO_TEXT")]
  public DbfLocValue ChooseHeroText => this.m_chooseHeroText;

  [DbfField("TB_TEXTURE")]
  public string TbTexture => this.m_tbTexture;

  [DbfField("TB_TEXTURE_PHONE")]
  public string TbTexturePhone => this.m_tbTexturePhone;

  [DbfField("TB_TEXTURE_PHONE_OFFSET_Y")]
  public double TbTexturePhoneOffsetY => this.m_tbTexturePhoneOffsetY;

  [DbfField("GAME_SAVE_DATA_PROGRESS_SUBKEY")]
  public int GameSaveDataProgressSubkey => this.m_gameSaveDataProgressSubkeyId;

  [DbfField("GAME_SAVE_DATA_PROGRESS_MAX")]
  public int GameSaveDataProgressMax => this.m_gameSaveDataProgressMax;

  [DbfField("HIDE_BOSS_HERO_POWER_IN_UI")]
  public bool HideBossHeroPowerInUi => this.m_hideBossHeroPowerInUi;

  [DbfField("SCRIPT_OBJECT")]
  public string ScriptObject => this.m_scriptObject;

  public List<ClassExclusionsDbfRecord> ClassExclusions
  {
    get
    {
      int id = this.ID;
      List<ClassExclusionsDbfRecord> classExclusions = new List<ClassExclusionsDbfRecord>();
      List<ClassExclusionsDbfRecord> records = GameDbf.ClassExclusions.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        ClassExclusionsDbfRecord exclusionsDbfRecord = records[index];
        if (exclusionsDbfRecord.ScenarioId == id)
          classExclusions.Add(exclusionsDbfRecord);
      }
      return classExclusions;
    }
  }

  public void SetNoteDesc(string v) => this.m_noteDesc = v;

  public void SetPlayers(int v) => this.m_players = v;

  public void SetPlayer1HeroCardId(int v) => this.m_player1HeroCardId = v;

  public void SetPlayer2HeroCardId(int v) => this.m_player2HeroCardId = v;

  public void SetIsExpert(bool v) => this.m_isExpert = v;

  public void SetIsCoop(bool v) => this.m_isCoop = v;

  public void SetAdventureId(int v) => this.m_adventureId = v;

  public void SetWingId(int v) => this.m_wingId = v;

  public void SetSortOrder(int v) => this.m_sortOrder = v;

  public void SetModeId(int v) => this.m_modeId = v;

  public void SetClientPlayer2HeroCardId(int v) => this.m_clientPlayer2HeroCardId = v;

  public void SetDeckRulesetId(int v) => this.m_deckRulesetId = v;

  public void SetRuleType(Scenario.RuleType v) => this.m_ruleType = v;

  public void SetTbTexture(string v) => this.m_tbTexture = v;

  public void SetTbTexturePhone(string v) => this.m_tbTexturePhone = v;

  public void SetTbTexturePhoneOffsetY(double v) => this.m_tbTexturePhoneOffsetY = v;

  public void SetScriptObject(string v) => this.m_scriptObject = v;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return (object) this.m_adventureId;
      case "BOARD_LAYOUT":
        return (object) this.m_boardLayout;
      case "CHOOSE_HERO_TEXT":
        return (object) this.m_chooseHeroText;
      case "CLIENT_PLAYER2_HERO_CARD_ID":
        return (object) this.m_clientPlayer2HeroCardId;
      case "CLIENT_PLAYER2_HERO_POWER_CARD_ID":
        return (object) this.m_clientPlayer2HeroPowerCardId;
      case "COMPLETED_DESCRIPTION":
        return (object) this.m_completedDescription;
      case "DECK_RULESET_ID":
        return (object) this.m_deckRulesetId;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "GAME_SAVE_DATA_PROGRESS_MAX":
        return (object) this.m_gameSaveDataProgressMax;
      case "GAME_SAVE_DATA_PROGRESS_SUBKEY":
        return (object) this.m_gameSaveDataProgressSubkeyId;
      case "HIDE_BOSS_HERO_POWER_IN_UI":
        return (object) this.m_hideBossHeroPowerInUi;
      case "ID":
        return (object) this.ID;
      case "IS_COOP":
        return (object) this.m_isCoop;
      case "IS_EXPERT":
        return (object) this.m_isExpert;
      case "IS_TUTORIAL":
        return (object) this.m_isTutorial;
      case "MODE_ID":
        return (object) this.m_modeId;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "ONE_SIM_PER_PLAYER":
        return (object) this.m_oneSimPerPlayer;
      case "OPPONENT_NAME":
        return (object) this.m_opponentName;
      case "PLAYER1_DECK_ID":
        return (object) this.m_player1DeckId;
      case "PLAYER1_HERO_CARD_ID":
        return (object) this.m_player1HeroCardId;
      case "PLAYER2_HERO_CARD_ID":
        return (object) this.m_player2HeroCardId;
      case "PLAYERS":
        return (object) this.m_players;
      case "RULE_TYPE":
        return (object) this.m_ruleType;
      case "SCRIPT_OBJECT":
        return (object) this.m_scriptObject;
      case "SHORT_DESCRIPTION":
        return (object) this.m_shortDescription;
      case "SHORT_NAME":
        return (object) this.m_shortName;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "TB_TEXTURE":
        return (object) this.m_tbTexture;
      case "TB_TEXTURE_PHONE":
        return (object) this.m_tbTexturePhone;
      case "TB_TEXTURE_PHONE_OFFSET_Y":
        return (object) this.m_tbTexturePhoneOffsetY;
      case "WING_ID":
        return (object) this.m_wingId;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 179436941:
        if (!(name == "GAME_SAVE_DATA_PROGRESS_MAX"))
          break;
        this.m_gameSaveDataProgressMax = (int) val;
        break;
      case 190718801:
        if (!(name == "ADVENTURE_ID"))
          break;
        this.m_adventureId = (int) val;
        break;
      case 191153791:
        if (!(name == "IS_COOP"))
          break;
        this.m_isCoop = (bool) val;
        break;
      case 286783624:
        if (!(name == "IS_TUTORIAL"))
          break;
        this.m_isTutorial = (bool) val;
        break;
      case 806552859:
        if (!(name == "PLAYER1_DECK_ID"))
          break;
        this.m_player1DeckId = (int) val;
        break;
      case 861726706:
        if (!(name == "BOARD_LAYOUT"))
          break;
        switch (val)
        {
          case null:
            this.m_boardLayout = Scenario.BoardLayout.STANDARD;
            return;
          case Scenario.BoardLayout _:
          case int _:
            this.m_boardLayout = (Scenario.BoardLayout) val;
            return;
          case string _:
            this.m_boardLayout = Scenario.ParseBoardLayoutValue((string) val);
            return;
          default:
            return;
        }
      case 912310022:
        if (!(name == "TB_TEXTURE_PHONE_OFFSET_Y"))
          break;
        this.m_tbTexturePhoneOffsetY = (double) val;
        break;
      case 931009045:
        if (!(name == "ONE_SIM_PER_PLAYER"))
          break;
        this.m_oneSimPerPlayer = (bool) val;
        break;
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1388406934:
        if (!(name == "PLAYER2_HERO_CARD_ID"))
          break;
        this.m_player2HeroCardId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1460468812:
        if (!(name == "SCRIPT_OBJECT"))
          break;
        this.m_scriptObject = (string) val;
        break;
      case 1559555090:
        if (!(name == "WING_ID"))
          break;
        this.m_wingId = (int) val;
        break;
      case 1832013819:
        if (!(name == "COMPLETED_DESCRIPTION"))
          break;
        this.m_completedDescription = (DbfLocValue) val;
        break;
      case 1960700408:
        if (!(name == "CLIENT_PLAYER2_HERO_CARD_ID"))
          break;
        this.m_clientPlayer2HeroCardId = (int) val;
        break;
      case 2167425274:
        if (!(name == "IS_EXPERT"))
          break;
        this.m_isExpert = (bool) val;
        break;
      case 2392988442:
        if (!(name == "OPPONENT_NAME"))
          break;
        this.m_opponentName = (DbfLocValue) val;
        break;
      case 2418820992:
        if (!(name == "SHORT_DESCRIPTION"))
          break;
        this.m_shortDescription = (DbfLocValue) val;
        break;
      case 2575670047:
        if (!(name == "TB_TEXTURE"))
          break;
        this.m_tbTexture = (string) val;
        break;
      case 2626514961:
        if (!(name == "CHOOSE_HERO_TEXT"))
          break;
        this.m_chooseHeroText = (DbfLocValue) val;
        break;
      case 2822633211:
        if (!(name == "HIDE_BOSS_HERO_POWER_IN_UI"))
          break;
        this.m_hideBossHeroPowerInUi = (bool) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3183294954:
        if (!(name == "CLIENT_PLAYER2_HERO_POWER_CARD_ID"))
          break;
        this.m_clientPlayer2HeroPowerCardId = (int) val;
        break;
      case 3226467965:
        if (!(name == "SHORT_NAME"))
          break;
        this.m_shortName = (DbfLocValue) val;
        break;
      case 3270521116:
        if (!(name == "TB_TEXTURE_PHONE"))
          break;
        this.m_tbTexturePhone = (string) val;
        break;
      case 3306344503:
        if (!(name == "DECK_RULESET_ID"))
          break;
        this.m_deckRulesetId = (int) val;
        break;
      case 3321980672:
        if (!(name == "GAME_SAVE_DATA_PROGRESS_SUBKEY"))
          break;
        this.m_gameSaveDataProgressSubkeyId = (int) val;
        break;
      case 3439379241:
        if (!(name == "PLAYERS"))
          break;
        this.m_players = (int) val;
        break;
      case 3741890713:
        if (!(name == "PLAYER1_HERO_CARD_ID"))
          break;
        this.m_player1HeroCardId = (int) val;
        break;
      case 3917898540:
        if (!(name == "RULE_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_ruleType = Scenario.RuleType.NONE;
            return;
          case Scenario.RuleType _:
          case int _:
            this.m_ruleType = (Scenario.RuleType) val;
            return;
          case string _:
            this.m_ruleType = Scenario.ParseRuleTypeValue((string) val);
            return;
          default:
            return;
        }
      case 3959141178:
        if (!(name == "MODE_ID"))
          break;
        this.m_modeId = (int) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return typeof (int);
      case "BOARD_LAYOUT":
        return typeof (Scenario.BoardLayout);
      case "CHOOSE_HERO_TEXT":
        return typeof (DbfLocValue);
      case "CLIENT_PLAYER2_HERO_CARD_ID":
        return typeof (int);
      case "CLIENT_PLAYER2_HERO_POWER_CARD_ID":
        return typeof (int);
      case "COMPLETED_DESCRIPTION":
        return typeof (DbfLocValue);
      case "DECK_RULESET_ID":
        return typeof (int);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "GAME_SAVE_DATA_PROGRESS_MAX":
        return typeof (int);
      case "GAME_SAVE_DATA_PROGRESS_SUBKEY":
        return typeof (int);
      case "HIDE_BOSS_HERO_POWER_IN_UI":
        return typeof (bool);
      case "ID":
        return typeof (int);
      case "IS_COOP":
        return typeof (bool);
      case "IS_EXPERT":
        return typeof (bool);
      case "IS_TUTORIAL":
        return typeof (bool);
      case "MODE_ID":
        return typeof (int);
      case "NAME":
        return typeof (DbfLocValue);
      case "NOTE_DESC":
        return typeof (string);
      case "ONE_SIM_PER_PLAYER":
        return typeof (bool);
      case "OPPONENT_NAME":
        return typeof (DbfLocValue);
      case "PLAYER1_DECK_ID":
        return typeof (int);
      case "PLAYER1_HERO_CARD_ID":
        return typeof (int);
      case "PLAYER2_HERO_CARD_ID":
        return typeof (int);
      case "PLAYERS":
        return typeof (int);
      case "RULE_TYPE":
        return typeof (Scenario.RuleType);
      case "SCRIPT_OBJECT":
        return typeof (string);
      case "SHORT_DESCRIPTION":
        return typeof (DbfLocValue);
      case "SHORT_NAME":
        return typeof (DbfLocValue);
      case "SORT_ORDER":
        return typeof (int);
      case "TB_TEXTURE":
        return typeof (string);
      case "TB_TEXTURE_PHONE":
        return typeof (string);
      case "TB_TEXTURE_PHONE_OFFSET_Y":
        return typeof (double);
      case "WING_ID":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadScenarioDbfRecords loadRecords = new LoadScenarioDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ScenarioDbfAsset scenarioDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ScenarioDbfAsset)) as ScenarioDbfAsset;
    if ((UnityEngine.Object) scenarioDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ScenarioDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < scenarioDbfAsset.Records.Count; ++index)
      scenarioDbfAsset.Records[index].StripUnusedLocales();
    records = scenarioDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_shortName.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
    this.m_shortDescription.StripUnusedLocales();
    this.m_opponentName.StripUnusedLocales();
    this.m_completedDescription.StripUnusedLocales();
    this.m_chooseHeroText.StripUnusedLocales();
  }
}
