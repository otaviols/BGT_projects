using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceMapNodeTypeDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private bool m_usesGameplayScene;
  [SerializeField]
  private LettuceMapNodeType.LettuceMapBossType m_bossType;
  [SerializeField]
  private DbfLocValue m_hoverTooltipHeader;
  [SerializeField]
  private DbfLocValue m_hoverTooltipBody;
  [SerializeField]
  private DbfLocValue m_playButtonText;
  [SerializeField]
  private int m_scenarioOverrideId;
  [SerializeField]
  private int m_grantMercenaryId;
  [SerializeField]
  private string m_nodeVisualId = "BOSS";
  [SerializeField]
  private LettuceMapNodeType.Visitlogictype m_visitLogic;
  [SerializeField]
  private bool m_repeatable;
  [SerializeField]
  private bool m_autoPlay;

  [DbfField("USES_GAMEPLAY_SCENE")]
  public bool UsesGameplayScene => this.m_usesGameplayScene;

  [DbfField("BOSS_TYPE")]
  public LettuceMapNodeType.LettuceMapBossType BossType => this.m_bossType;

  [DbfField("HOVER_TOOLTIP_HEADER")]
  public DbfLocValue HoverTooltipHeader => this.m_hoverTooltipHeader;

  [DbfField("HOVER_TOOLTIP_BODY")]
  public DbfLocValue HoverTooltipBody => this.m_hoverTooltipBody;

  [DbfField("PLAY_BUTTON_TEXT")]
  public DbfLocValue PlayButtonText => this.m_playButtonText;

  [DbfField("SCENARIO_OVERRIDE")]
  public int ScenarioOverride => this.m_scenarioOverrideId;

  [DbfField("GRANT_MERCENARY")]
  public int GrantMercenary => this.m_grantMercenaryId;

  [DbfField("NODE_VISUAL_ID")]
  public string NodeVisualId => this.m_nodeVisualId;

  [DbfField("VISIT_LOGIC")]
  public LettuceMapNodeType.Visitlogictype VisitLogic => this.m_visitLogic;

  [DbfField("REPEATABLE")]
  public bool Repeatable => this.m_repeatable;

  [DbfField("AUTO_PLAY")]
  public bool AutoPlay => this.m_autoPlay;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "AUTO_PLAY":
        return (object) this.m_autoPlay;
      case "BOSS_TYPE":
        return (object) this.m_bossType;
      case "GRANT_MERCENARY":
        return (object) this.m_grantMercenaryId;
      case "HOVER_TOOLTIP_BODY":
        return (object) this.m_hoverTooltipBody;
      case "HOVER_TOOLTIP_HEADER":
        return (object) this.m_hoverTooltipHeader;
      case "ID":
        return (object) this.ID;
      case "NODE_VISUAL_ID":
        return (object) this.m_nodeVisualId;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "PLAY_BUTTON_TEXT":
        return (object) this.m_playButtonText;
      case "REPEATABLE":
        return (object) this.m_repeatable;
      case "SCENARIO_OVERRIDE":
        return (object) this.m_scenarioOverrideId;
      case "USES_GAMEPLAY_SCENE":
        return (object) this.m_usesGameplayScene;
      case "VISIT_LOGIC":
        return (object) this.m_visitLogic;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 606835680:
        if (!(name == "SCENARIO_OVERRIDE"))
          break;
        this.m_scenarioOverrideId = (int) val;
        break;
      case 630626561:
        if (!(name == "USES_GAMEPLAY_SCENE"))
          break;
        this.m_usesGameplayScene = (bool) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1656892710:
        if (!(name == "PLAY_BUTTON_TEXT"))
          break;
        this.m_playButtonText = (DbfLocValue) val;
        break;
      case 1711657377:
        if (!(name == "AUTO_PLAY"))
          break;
        this.m_autoPlay = (bool) val;
        break;
      case 2721624087:
        if (!(name == "HOVER_TOOLTIP_HEADER"))
          break;
        this.m_hoverTooltipHeader = (DbfLocValue) val;
        break;
      case 2914300258:
        if (!(name == "GRANT_MERCENARY"))
          break;
        this.m_grantMercenaryId = (int) val;
        break;
      case 2951393070:
        if (!(name == "REPEATABLE"))
          break;
        this.m_repeatable = (bool) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3211449921:
        if (!(name == "BOSS_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_bossType = LettuceMapNodeType.LettuceMapBossType.NONE;
            return;
          case LettuceMapNodeType.LettuceMapBossType _:
          case int _:
            this.m_bossType = (LettuceMapNodeType.LettuceMapBossType) val;
            return;
          case string _:
            this.m_bossType = LettuceMapNodeType.ParseLettuceMapBossTypeValue((string) val);
            return;
          default:
            return;
        }
      case 3513445834:
        if (!(name == "HOVER_TOOLTIP_BODY"))
          break;
        this.m_hoverTooltipBody = (DbfLocValue) val;
        break;
      case 3652714661:
        if (!(name == "VISIT_LOGIC"))
          break;
        switch (val)
        {
          case null:
            this.m_visitLogic = LettuceMapNodeType.Visitlogictype.NONE;
            return;
          case LettuceMapNodeType.Visitlogictype _:
          case int _:
            this.m_visitLogic = (LettuceMapNodeType.Visitlogictype) val;
            return;
          case string _:
            this.m_visitLogic = LettuceMapNodeType.ParseVisitlogictypeValue((string) val);
            return;
          default:
            return;
        }
      case 3840307666:
        if (!(name == "NODE_VISUAL_ID"))
          break;
        this.m_nodeVisualId = (string) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "AUTO_PLAY":
        return typeof (bool);
      case "BOSS_TYPE":
        return typeof (LettuceMapNodeType.LettuceMapBossType);
      case "GRANT_MERCENARY":
        return typeof (int);
      case "HOVER_TOOLTIP_BODY":
        return typeof (DbfLocValue);
      case "HOVER_TOOLTIP_HEADER":
        return typeof (DbfLocValue);
      case "ID":
        return typeof (int);
      case "NODE_VISUAL_ID":
        return typeof (string);
      case "NOTE_DESC":
        return typeof (string);
      case "PLAY_BUTTON_TEXT":
        return typeof (DbfLocValue);
      case "REPEATABLE":
        return typeof (bool);
      case "SCENARIO_OVERRIDE":
        return typeof (int);
      case "USES_GAMEPLAY_SCENE":
        return typeof (bool);
      case "VISIT_LOGIC":
        return typeof (LettuceMapNodeType.Visitlogictype);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceMapNodeTypeDbfRecords loadRecords = new LoadLettuceMapNodeTypeDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceMapNodeTypeDbfAsset nodeTypeDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceMapNodeTypeDbfAsset)) as LettuceMapNodeTypeDbfAsset;
    if ((UnityEngine.Object) nodeTypeDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceMapNodeTypeDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < nodeTypeDbfAsset.Records.Count; ++index)
      nodeTypeDbfAsset.Records[index].StripUnusedLocales();
    records = nodeTypeDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_hoverTooltipHeader.StripUnusedLocales();
    this.m_hoverTooltipBody.StripUnusedLocales();
    this.m_playButtonText.StripUnusedLocales();
  }
}
