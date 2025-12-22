using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProductClientDataDbfRecord : DbfRecord
{
  [SerializeField]
  private long m_pmtProductId;
  [SerializeField]
  private DbfLocValue m_popupTitle;
  [SerializeField]
  private DbfLocValue m_popupBody;

  [DbfField("PMT_PRODUCT_ID")]
  public long PmtProductId => this.m_pmtProductId;

  [DbfField("POPUP_TITLE")]
  public DbfLocValue PopupTitle => this.m_popupTitle;

  [DbfField("POPUP_BODY")]
  public DbfLocValue PopupBody => this.m_popupBody;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "PMT_PRODUCT_ID")
      return (object) this.m_pmtProductId;
    if (name == "POPUP_TITLE")
      return (object) this.m_popupTitle;
    return name == "POPUP_BODY" ? (object) this.m_popupBody : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "PMT_PRODUCT_ID"))
      {
        if (!(name == "POPUP_TITLE"))
        {
          if (!(name == "POPUP_BODY"))
            return;
          this.m_popupBody = (DbfLocValue) val;
        }
        else
          this.m_popupTitle = (DbfLocValue) val;
      }
      else
        this.m_pmtProductId = (long) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "PMT_PRODUCT_ID")
      return typeof (long);
    if (name == "POPUP_TITLE")
      return typeof (DbfLocValue);
    return name == "POPUP_BODY" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadProductClientDataDbfRecords loadRecords = new LoadProductClientDataDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ProductClientDataDbfAsset clientDataDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ProductClientDataDbfAsset)) as ProductClientDataDbfAsset;
    if ((UnityEngine.Object) clientDataDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ProductClientDataDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < clientDataDbfAsset.Records.Count; ++index)
      clientDataDbfAsset.Records[index].StripUnusedLocales();
    records = clientDataDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_popupTitle.StripUnusedLocales();
    this.m_popupBody.StripUnusedLocales();
  }
}
