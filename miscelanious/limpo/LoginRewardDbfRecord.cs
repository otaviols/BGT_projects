using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LoginRewardDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_message;
  [SerializeField]
  private string m_styleName;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "MESSAGE")
      return (object) this.m_message;
    return name == "STYLE_NAME" ? (object) this.m_styleName : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "MESSAGE"))
      {
        if (!(name == "STYLE_NAME"))
          return;
        this.m_styleName = (string) val;
      }
      else
        this.m_message = (string) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "MESSAGE")
      return typeof (string);
    return name == "STYLE_NAME" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLoginRewardDbfRecords loadRecords = new LoadLoginRewardDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LoginRewardDbfAsset loginRewardDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LoginRewardDbfAsset)) as LoginRewardDbfAsset;
    if ((UnityEngine.Object) loginRewardDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LoginRewardDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < loginRewardDbfAsset.Records.Count; ++index)
      loginRewardDbfAsset.Records[index].StripUnusedLocales();
    records = loginRewardDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
