using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopTierDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private string m_style;
  [SerializeField]
  private string m_tags;
  [SerializeField]
  private DbfLocValue m_header;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private bool m_disabled;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "DISABLED":
        return (object) this.m_disabled;
      case "HEADER":
        return (object) this.m_header;
      case "ID":
        return (object) this.ID;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "STYLE":
        return (object) this.m_style;
      case "TAGS":
        return (object) this.m_tags;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 304993157:
        if (!(name == "DISABLED"))
          break;
        this.m_disabled = (bool) val;
        break;
      case 1450621046:
        if (!(name == "STYLE"))
          break;
        this.m_style = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2263853280:
        if (!(name == "HEADER"))
          break;
        this.m_header = (DbfLocValue) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 4187495584:
        if (!(name == "TAGS"))
          break;
        this.m_tags = (string) val;
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
      case "DISABLED":
        return typeof (bool);
      case "HEADER":
        return typeof (DbfLocValue);
      case "ID":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "SORT_ORDER":
        return typeof (int);
      case "STYLE":
        return typeof (string);
      case "TAGS":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadShopTierDbfRecords loadRecords = new LoadShopTierDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ShopTierDbfAsset shopTierDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ShopTierDbfAsset)) as ShopTierDbfAsset;
    if ((UnityEngine.Object) shopTierDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ShopTierDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < shopTierDbfAsset.Records.Count; ++index)
      shopTierDbfAsset.Records[index].StripUnusedLocales();
    records = shopTierDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_header.StripUnusedLocales();
}
