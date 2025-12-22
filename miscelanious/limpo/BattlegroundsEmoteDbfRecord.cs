using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattlegroundsEmoteDbfRecord : DbfRecord
{
  [SerializeField]
  private bool m_enabled = true;
  [SerializeField]
  private int m_rarityId = 2;
  [SerializeField]
  private DbfLocValue m_collectionShortName;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private string m_animationPath;
  [SerializeField]
  private double m_xOffset;
  [SerializeField]
  private double m_zOffset = -0.079;
  [SerializeField]
  private bool m_isDefault;
  [SerializeField]
  private bool m_isAnimating;
  [SerializeField]
  private BattlegroundsEmote.Bordertype m_borderType;

  [DbfField("RARITY")]
  public int Rarity => this.m_rarityId;

  [DbfField("COLLECTION_SHORT_NAME")]
  public DbfLocValue CollectionShortName => this.m_collectionShortName;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("ANIMATION_PATH")]
  public string AnimationPath => this.m_animationPath;

  [DbfField("X_OFFSET")]
  public double XOffset => this.m_xOffset;

  [DbfField("Z_OFFSET")]
  public double ZOffset => this.m_zOffset;

  [DbfField("IS_DEFAULT")]
  public bool IsDefault => this.m_isDefault;

  [DbfField("IS_ANIMATING")]
  public bool IsAnimating => this.m_isAnimating;

  [DbfField("BORDER_TYPE")]
  public BattlegroundsEmote.Bordertype BorderType => this.m_borderType;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ANIMATION_PATH":
        return (object) this.m_animationPath;
      case "BORDER_TYPE":
        return (object) this.m_borderType;
      case "COLLECTION_SHORT_NAME":
        return (object) this.m_collectionShortName;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "ENABLED":
        return (object) this.m_enabled;
      case "ID":
        return (object) this.ID;
      case "IS_ANIMATING":
        return (object) this.m_isAnimating;
      case "IS_DEFAULT":
        return (object) this.m_isDefault;
      case "RARITY":
        return (object) this.m_rarityId;
      case "X_OFFSET":
        return (object) this.m_xOffset;
      case "Z_OFFSET":
        return (object) this.m_zOffset;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 200515501:
        if (!(name == "ANIMATION_PATH"))
          break;
        this.m_animationPath = (string) val;
        break;
      case 447902318:
        if (!(name == "IS_ANIMATING"))
          break;
        this.m_isAnimating = (bool) val;
        break;
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1691910384:
        if (!(name == "BORDER_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_borderType = BattlegroundsEmote.Bordertype.NONE;
            return;
          case BattlegroundsEmote.Bordertype _:
          case int _:
            this.m_borderType = (BattlegroundsEmote.Bordertype) val;
            return;
          case string _:
            this.m_borderType = BattlegroundsEmote.ParseBordertypeValue((string) val);
            return;
          default:
            return;
        }
      case 2294480894:
        if (!(name == "ENABLED"))
          break;
        this.m_enabled = (bool) val;
        break;
      case 2401654691:
        if (!(name == "IS_DEFAULT"))
          break;
        this.m_isDefault = (bool) val;
        break;
      case 2516563249:
        if (!(name == "Z_OFFSET"))
          break;
        this.m_zOffset = (double) val;
        break;
      case 2805908475:
        if (!(name == "X_OFFSET"))
          break;
        this.m_xOffset = (double) val;
        break;
      case 2975427914:
        if (!(name == "RARITY"))
          break;
        this.m_rarityId = (int) val;
        break;
      case 4011828978:
        if (!(name == "COLLECTION_SHORT_NAME"))
          break;
        this.m_collectionShortName = (DbfLocValue) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ANIMATION_PATH":
        return typeof (string);
      case "BORDER_TYPE":
        return typeof (BattlegroundsEmote.Bordertype);
      case "COLLECTION_SHORT_NAME":
        return typeof (DbfLocValue);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "ENABLED":
        return typeof (bool);
      case "ID":
        return typeof (int);
      case "IS_ANIMATING":
        return typeof (bool);
      case "IS_DEFAULT":
        return typeof (bool);
      case "RARITY":
        return typeof (int);
      case "X_OFFSET":
        return typeof (double);
      case "Z_OFFSET":
        return typeof (double);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadBattlegroundsEmoteDbfRecords loadRecords = new LoadBattlegroundsEmoteDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BattlegroundsEmoteDbfAsset battlegroundsEmoteDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BattlegroundsEmoteDbfAsset)) as BattlegroundsEmoteDbfAsset;
    if ((UnityEngine.Object) battlegroundsEmoteDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BattlegroundsEmoteDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < battlegroundsEmoteDbfAsset.Records.Count; ++index)
      battlegroundsEmoteDbfAsset.Records[index].StripUnusedLocales();
    records = battlegroundsEmoteDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_collectionShortName.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
