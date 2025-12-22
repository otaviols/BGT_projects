using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModularBundleLayoutDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_modularBundleId;
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_hiddenLicenseId;
  [SerializeField]
  private string m_accentTexture;
  [SerializeField]
  private string m_regions = "*";
  [SerializeField]
  private string m_abValue = "A";
  [SerializeField]
  private string m_prefab;
  [SerializeField]
  private DbfLocValue m_descriptionHeadline;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private DbfLocValue m_orderSummaryName;
  [SerializeField]
  private bool m_animateAfterPurchase;
  [SerializeField]
  private double m_storeShakeDelay = 1.0;

  [DbfField("MODULAR_BUNDLE_ID")]
  public int ModularBundleId => this.m_modularBundleId;

  [DbfField("HIDDEN_LICENSE_ID")]
  public int HiddenLicenseId => this.m_hiddenLicenseId;

  [DbfField("ACCENT_TEXTURE")]
  public string AccentTexture => this.m_accentTexture;

  [DbfField("REGIONS")]
  public string Regions => this.m_regions;

  [DbfField("PREFAB")]
  public string Prefab => this.m_prefab;

  [DbfField("DESCRIPTION_HEADLINE")]
  public DbfLocValue DescriptionHeadline => this.m_descriptionHeadline;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("ORDER_SUMMARY_NAME")]
  public DbfLocValue OrderSummaryName => this.m_orderSummaryName;

  [DbfField("ANIMATE_AFTER_PURCHASE")]
  public bool AnimateAfterPurchase => this.m_animateAfterPurchase;

  [DbfField("STORE_SHAKE_DELAY")]
  public double StoreShakeDelay => this.m_storeShakeDelay;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "AB_VALUE":
        return (object) this.m_abValue;
      case "ACCENT_TEXTURE":
        return (object) this.m_accentTexture;
      case "ANIMATE_AFTER_PURCHASE":
        return (object) this.m_animateAfterPurchase;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "DESCRIPTION_HEADLINE":
        return (object) this.m_descriptionHeadline;
      case "HIDDEN_LICENSE_ID":
        return (object) this.m_hiddenLicenseId;
      case "ID":
        return (object) this.ID;
      case "MODULAR_BUNDLE_ID":
        return (object) this.m_modularBundleId;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "ORDER_SUMMARY_NAME":
        return (object) this.m_orderSummaryName;
      case "PREFAB":
        return (object) this.m_prefab;
      case "REGIONS":
        return (object) this.m_regions;
      case "STORE_SHAKE_DELAY":
        return (object) this.m_storeShakeDelay;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 556572225:
        if (!(name == "ANIMATE_AFTER_PURCHASE"))
          break;
        this.m_animateAfterPurchase = (bool) val;
        break;
      case 877574174:
        if (!(name == "ORDER_SUMMARY_NAME"))
          break;
        this.m_orderSummaryName = (DbfLocValue) val;
        break;
      case 1041024899:
        if (!(name == "PREFAB"))
          break;
        this.m_prefab = (string) val;
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
      case 1775586830:
        if (!(name == "DESCRIPTION_HEADLINE"))
          break;
        this.m_descriptionHeadline = (DbfLocValue) val;
        break;
      case 2090525019:
        if (!(name == "HIDDEN_LICENSE_ID"))
          break;
        this.m_hiddenLicenseId = (int) val;
        break;
      case 2186144642:
        if (!(name == "MODULAR_BUNDLE_ID"))
          break;
        this.m_modularBundleId = (int) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3211248554:
        if (!(name == "REGIONS"))
          break;
        this.m_regions = (string) val;
        break;
      case 3227453679:
        if (!(name == "STORE_SHAKE_DELAY"))
          break;
        this.m_storeShakeDelay = (double) val;
        break;
      case 3915924868:
        if (!(name == "AB_VALUE"))
          break;
        this.m_abValue = (string) val;
        break;
      case 4036182035:
        if (!(name == "ACCENT_TEXTURE"))
          break;
        this.m_accentTexture = (string) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "AB_VALUE":
        return typeof (string);
      case "ACCENT_TEXTURE":
        return typeof (string);
      case "ANIMATE_AFTER_PURCHASE":
        return typeof (bool);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "DESCRIPTION_HEADLINE":
        return typeof (DbfLocValue);
      case "HIDDEN_LICENSE_ID":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "MODULAR_BUNDLE_ID":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "ORDER_SUMMARY_NAME":
        return typeof (DbfLocValue);
      case "PREFAB":
        return typeof (string);
      case "REGIONS":
        return typeof (string);
      case "STORE_SHAKE_DELAY":
        return typeof (double);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadModularBundleLayoutDbfRecords loadRecords = new LoadModularBundleLayoutDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ModularBundleLayoutDbfAsset bundleLayoutDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ModularBundleLayoutDbfAsset)) as ModularBundleLayoutDbfAsset;
    if ((UnityEngine.Object) bundleLayoutDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ModularBundleLayoutDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < bundleLayoutDbfAsset.Records.Count; ++index)
      bundleLayoutDbfAsset.Records[index].StripUnusedLocales();
    records = bundleLayoutDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_descriptionHeadline.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
    this.m_orderSummaryName.StripUnusedLocales();
  }
}
