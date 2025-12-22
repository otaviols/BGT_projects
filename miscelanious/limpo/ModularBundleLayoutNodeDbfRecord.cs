using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModularBundleLayoutNodeDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_nodeLayoutId;
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private ModularBundleLayoutNode.DisplayType m_displayType = ModularBundleLayoutNode.ParseDisplayTypeValue("invalid");
  [SerializeField]
  private int m_displayData;
  [SerializeField]
  private string m_displayPrefab;
  [SerializeField]
  private DbfLocValue m_displayText;
  [SerializeField]
  private string m_displayTextGlowSize;
  [SerializeField]
  private int m_displayCount;
  [SerializeField]
  private string m_entryAnimation;
  [SerializeField]
  private string m_exitAnimation;
  [SerializeField]
  private string m_entrySound;
  [SerializeField]
  private string m_landingSound;
  [SerializeField]
  private string m_exitSound;
  [SerializeField]
  private int m_nodeIndex;
  [SerializeField]
  private double m_entryDelay;
  [SerializeField]
  private double m_animSpeedMultiplier = 1.0;
  [SerializeField]
  private int m_shakeWeight;

  [DbfField("NODE_LAYOUT_ID")]
  public int NodeLayoutId => this.m_nodeLayoutId;

  [DbfField("DISPLAY_TYPE")]
  public ModularBundleLayoutNode.DisplayType DisplayType => this.m_displayType;

  [DbfField("DISPLAY_DATA")]
  public int DisplayData => this.m_displayData;

  [DbfField("DISPLAY_PREFAB")]
  public string DisplayPrefab => this.m_displayPrefab;

  [DbfField("DISPLAY_TEXT")]
  public DbfLocValue DisplayText => this.m_displayText;

  [DbfField("DISPLAY_TEXT_GLOW_SIZE")]
  public string DisplayTextGlowSize => this.m_displayTextGlowSize;

  [DbfField("DISPLAY_COUNT")]
  public int DisplayCount => this.m_displayCount;

  [DbfField("ENTRY_ANIMATION")]
  public string EntryAnimation => this.m_entryAnimation;

  [DbfField("EXIT_ANIMATION")]
  public string ExitAnimation => this.m_exitAnimation;

  [DbfField("ENTRY_SOUND")]
  public string EntrySound => this.m_entrySound;

  [DbfField("LANDING_SOUND")]
  public string LandingSound => this.m_landingSound;

  [DbfField("EXIT_SOUND")]
  public string ExitSound => this.m_exitSound;

  [DbfField("NODE_INDEX")]
  public int NodeIndex => this.m_nodeIndex;

  [DbfField("ENTRY_DELAY")]
  public double EntryDelay => this.m_entryDelay;

  [DbfField("ANIM_SPEED_MULTIPLIER")]
  public double AnimSpeedMultiplier => this.m_animSpeedMultiplier;

  [DbfField("SHAKE_WEIGHT")]
  public int ShakeWeight => this.m_shakeWeight;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ANIM_SPEED_MULTIPLIER":
        return (object) this.m_animSpeedMultiplier;
      case "DISPLAY_COUNT":
        return (object) this.m_displayCount;
      case "DISPLAY_DATA":
        return (object) this.m_displayData;
      case "DISPLAY_PREFAB":
        return (object) this.m_displayPrefab;
      case "DISPLAY_TEXT":
        return (object) this.m_displayText;
      case "DISPLAY_TEXT_GLOW_SIZE":
        return (object) this.m_displayTextGlowSize;
      case "DISPLAY_TYPE":
        return (object) this.m_displayType;
      case "ENTRY_ANIMATION":
        return (object) this.m_entryAnimation;
      case "ENTRY_DELAY":
        return (object) this.m_entryDelay;
      case "ENTRY_SOUND":
        return (object) this.m_entrySound;
      case "EXIT_ANIMATION":
        return (object) this.m_exitAnimation;
      case "EXIT_SOUND":
        return (object) this.m_exitSound;
      case "ID":
        return (object) this.ID;
      case "LANDING_SOUND":
        return (object) this.m_landingSound;
      case "NODE_INDEX":
        return (object) this.m_nodeIndex;
      case "NODE_LAYOUT_ID":
        return (object) this.m_nodeLayoutId;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "SHAKE_WEIGHT":
        return (object) this.m_shakeWeight;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 233364829:
        if (!(name == "DISPLAY_COUNT"))
          break;
        this.m_displayCount = (int) val;
        break;
      case 568925309:
        if (!(name == "DISPLAY_TEXT"))
          break;
        this.m_displayText = (DbfLocValue) val;
        break;
      case 1083713952:
        if (!(name == "LANDING_SOUND"))
          break;
        this.m_landingSound = (string) val;
        break;
      case 1180807003:
        if (!(name == "ENTRY_SOUND"))
          break;
        this.m_entrySound = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2054175731:
        if (!(name == "ENTRY_DELAY"))
          break;
        this.m_entryDelay = (double) val;
        break;
      case 2277209517:
        if (!(name == "EXIT_SOUND"))
          break;
        this.m_exitSound = (string) val;
        break;
      case 2396003294:
        if (!(name == "DISPLAY_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_displayType = ModularBundleLayoutNode.DisplayType.INVALID;
            return;
          case ModularBundleLayoutNode.DisplayType _:
          case int _:
            this.m_displayType = (ModularBundleLayoutNode.DisplayType) val;
            return;
          case string _:
            this.m_displayType = ModularBundleLayoutNode.ParseDisplayTypeValue((string) val);
            return;
          default:
            return;
        }
      case 2452196822:
        if (!(name == "ANIM_SPEED_MULTIPLIER"))
          break;
        this.m_animSpeedMultiplier = (double) val;
        break;
      case 2588884954:
        if (!(name == "NODE_INDEX"))
          break;
        this.m_nodeIndex = (int) val;
        break;
      case 2706375718:
        if (!(name == "SHAKE_WEIGHT"))
          break;
        this.m_shakeWeight = (int) val;
        break;
      case 2923745662:
        if (!(name == "DISPLAY_DATA"))
          break;
        this.m_displayData = (int) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3107145325:
        if (!(name == "DISPLAY_TEXT_GLOW_SIZE"))
          break;
        this.m_displayTextGlowSize = (string) val;
        break;
      case 3360314162:
        if (!(name == "ENTRY_ANIMATION"))
          break;
        this.m_entryAnimation = (string) val;
        break;
      case 3837921320:
        if (!(name == "NODE_LAYOUT_ID"))
          break;
        this.m_nodeLayoutId = (int) val;
        break;
      case 4068007548:
        if (!(name == "EXIT_ANIMATION"))
          break;
        this.m_exitAnimation = (string) val;
        break;
      case 4202061068:
        if (!(name == "DISPLAY_PREFAB"))
          break;
        this.m_displayPrefab = (string) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ANIM_SPEED_MULTIPLIER":
        return typeof (double);
      case "DISPLAY_COUNT":
        return typeof (int);
      case "DISPLAY_DATA":
        return typeof (int);
      case "DISPLAY_PREFAB":
        return typeof (string);
      case "DISPLAY_TEXT":
        return typeof (DbfLocValue);
      case "DISPLAY_TEXT_GLOW_SIZE":
        return typeof (string);
      case "DISPLAY_TYPE":
        return typeof (ModularBundleLayoutNode.DisplayType);
      case "ENTRY_ANIMATION":
        return typeof (string);
      case "ENTRY_DELAY":
        return typeof (double);
      case "ENTRY_SOUND":
        return typeof (string);
      case "EXIT_ANIMATION":
        return typeof (string);
      case "EXIT_SOUND":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "LANDING_SOUND":
        return typeof (string);
      case "NODE_INDEX":
        return typeof (int);
      case "NODE_LAYOUT_ID":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "SHAKE_WEIGHT":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadModularBundleLayoutNodeDbfRecords loadRecords = new LoadModularBundleLayoutNodeDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ModularBundleLayoutNodeDbfAsset layoutNodeDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ModularBundleLayoutNodeDbfAsset)) as ModularBundleLayoutNodeDbfAsset;
    if ((UnityEngine.Object) layoutNodeDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ModularBundleLayoutNodeDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < layoutNodeDbfAsset.Records.Count; ++index)
      layoutNodeDbfAsset.Records[index].StripUnusedLocales();
    records = layoutNodeDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_displayText.StripUnusedLocales();
}
