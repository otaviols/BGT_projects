using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClassDbfRecord : DbfRecord
{
  [SerializeField]
  private Class.AssetFlags m_assetFlags;

  public override object GetVar(string name) => name == "ASSET_FLAGS" ? (object) this.m_assetFlags : (object) null;

  public override void SetVar(string name, object val)
  {
    if (!(name == "ASSET_FLAGS"))
      return;
    switch (val)
    {
      case null:
        this.m_assetFlags = Class.AssetFlags.NONE;
        break;
      case Class.AssetFlags _:
      case int _:
        this.m_assetFlags = (Class.AssetFlags) val;
        break;
      case string _:
        this.m_assetFlags = Class.ParseAssetFlagsValue((string) val);
        break;
    }
  }

  public override System.Type GetVarType(string name) => name == "ASSET_FLAGS" ? typeof (Class.AssetFlags) : (System.Type) null;

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadClassDbfRecords loadRecords = new LoadClassDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ClassDbfAsset classDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ClassDbfAsset)) as ClassDbfAsset;
    if ((UnityEngine.Object) classDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ClassDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < classDbfAsset.Records.Count; ++index)
      classDbfAsset.Records[index].StripUnusedLocales();
    records = classDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
