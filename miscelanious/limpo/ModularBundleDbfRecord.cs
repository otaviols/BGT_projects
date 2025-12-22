using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModularBundleDbfRecord : DbfRecord
{
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private string m_selectorPrefab;
  [SerializeField]
  private int m_selectorPackAmountBanner;
  [SerializeField]
  private string m_layoutButtonSize = "small";
  [SerializeField]
  private string m_background;
  [SerializeField]
  private string m_playlist;
  [SerializeField]
  private string m_logoTexture;
  [SerializeField]
  private string m_logoTextureGlow;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private bool m_showAfterPurchase;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("SELECTOR_PREFAB")]
  public string SelectorPrefab => this.m_selectorPrefab;

  [DbfField("SELECTOR_PACK_AMOUNT_BANNER")]
  public int SelectorPackAmountBanner => this.m_selectorPackAmountBanner;

  [DbfField("LAYOUT_BUTTON_SIZE")]
  public string LayoutButtonSize => this.m_layoutButtonSize;

  [DbfField("BACKGROUND")]
  public string Background => this.m_background;

  [DbfField("PLAYLIST")]
  public string Playlist => this.m_playlist;

  [DbfField("LOGO_TEXTURE")]
  public string LogoTexture => this.m_logoTexture;

  [DbfField("LOGO_TEXTURE_GLOW")]
  public string LogoTextureGlow => this.m_logoTextureGlow;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("SHOW_AFTER_PURCHASE")]
  public bool ShowAfterPurchase => this.m_showAfterPurchase;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "BACKGROUND":
        return (object) this.m_background;
      case "ID":
        return (object) this.ID;
      case "LAYOUT_BUTTON_SIZE":
        return (object) this.m_layoutButtonSize;
      case "LOGO_TEXTURE":
        return (object) this.m_logoTexture;
      case "LOGO_TEXTURE_GLOW":
        return (object) this.m_logoTextureGlow;
      case "NAME":
        return (object) this.m_name;
      case "PLAYLIST":
        return (object) this.m_playlist;
      case "SELECTOR_PACK_AMOUNT_BANNER":
        return (object) this.m_selectorPackAmountBanner;
      case "SELECTOR_PREFAB":
        return (object) this.m_selectorPrefab;
      case "SHOW_AFTER_PURCHASE":
        return (object) this.m_showAfterPurchase;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 226638260:
        if (!(name == "SELECTOR_PACK_AMOUNT_BANNER"))
          break;
        this.m_selectorPackAmountBanner = (int) val;
        break;
      case 589867744:
        if (!(name == "LAYOUT_BUTTON_SIZE"))
          break;
        this.m_layoutButtonSize = (string) val;
        break;
      case 670183358:
        if (!(name == "LOGO_TEXTURE"))
          break;
        this.m_logoTexture = (string) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1397850731:
        if (!(name == "PLAYLIST"))
          break;
        this.m_playlist = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2200459534:
        if (!(name == "LOGO_TEXTURE_GLOW"))
          break;
        this.m_logoTextureGlow = (string) val;
        break;
      case 2319408461:
        if (!(name == "SHOW_AFTER_PURCHASE"))
          break;
        this.m_showAfterPurchase = (bool) val;
        break;
      case 2601291421:
        if (!(name == "BACKGROUND"))
          break;
        this.m_background = (string) val;
        break;
      case 3656563063:
        if (!(name == "SELECTOR_PREFAB"))
          break;
        this.m_selectorPrefab = (string) val;
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
      case "BACKGROUND":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "LAYOUT_BUTTON_SIZE":
        return typeof (string);
      case "LOGO_TEXTURE":
        return typeof (string);
      case "LOGO_TEXTURE_GLOW":
        return typeof (string);
      case "NAME":
        return typeof (DbfLocValue);
      case "PLAYLIST":
        return typeof (string);
      case "SELECTOR_PACK_AMOUNT_BANNER":
        return typeof (int);
      case "SELECTOR_PREFAB":
        return typeof (string);
      case "SHOW_AFTER_PURCHASE":
        return typeof (bool);
      case "SORT_ORDER":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadModularBundleDbfRecords loadRecords = new LoadModularBundleDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ModularBundleDbfAsset modularBundleDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ModularBundleDbfAsset)) as ModularBundleDbfAsset;
    if ((UnityEngine.Object) modularBundleDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ModularBundleDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < modularBundleDbfAsset.Records.Count; ++index)
      modularBundleDbfAsset.Records[index].StripUnusedLocales();
    records = modularBundleDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_name.StripUnusedLocales();
}
