using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DetailsVideoCueDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_battlegroundsFinisherId;
  [SerializeField]
  private DbfLocValue m_captionTitle;
  [SerializeField]
  private DbfLocValue m_captionSubtitle;
  [SerializeField]
  private double m_startSeconds;

  [DbfField("BATTLEGROUNDS_FINISHER_ID")]
  public int BattlegroundsFinisherId => this.m_battlegroundsFinisherId;

  [DbfField("CAPTION_TITLE")]
  public DbfLocValue CaptionTitle => this.m_captionTitle;

  [DbfField("CAPTION_SUBTITLE")]
  public DbfLocValue CaptionSubtitle => this.m_captionSubtitle;

  [DbfField("START_SECONDS")]
  public double StartSeconds => this.m_startSeconds;

  public override object GetVar(string name)
  {
    if (name == "BATTLEGROUNDS_FINISHER_ID")
      return (object) this.m_battlegroundsFinisherId;
    if (name == "CAPTION_TITLE")
      return (object) this.m_captionTitle;
    if (name == "CAPTION_SUBTITLE")
      return (object) this.m_captionSubtitle;
    return name == "START_SECONDS" ? (object) this.m_startSeconds : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "BATTLEGROUNDS_FINISHER_ID"))
    {
      if (!(name == "CAPTION_TITLE"))
      {
        if (!(name == "CAPTION_SUBTITLE"))
        {
          if (!(name == "START_SECONDS"))
            return;
          this.m_startSeconds = (double) val;
        }
        else
          this.m_captionSubtitle = (DbfLocValue) val;
      }
      else
        this.m_captionTitle = (DbfLocValue) val;
    }
    else
      this.m_battlegroundsFinisherId = (int) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "BATTLEGROUNDS_FINISHER_ID")
      return typeof (int);
    if (name == "CAPTION_TITLE")
      return typeof (DbfLocValue);
    if (name == "CAPTION_SUBTITLE")
      return typeof (DbfLocValue);
    return name == "START_SECONDS" ? typeof (double) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadDetailsVideoCueDbfRecords loadRecords = new LoadDetailsVideoCueDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    DetailsVideoCueDbfAsset videoCueDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (DetailsVideoCueDbfAsset)) as DetailsVideoCueDbfAsset;
    if ((UnityEngine.Object) videoCueDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("DetailsVideoCueDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < videoCueDbfAsset.Records.Count; ++index)
      videoCueDbfAsset.Records[index].StripUnusedLocales();
    records = videoCueDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_captionTitle.StripUnusedLocales();
    this.m_captionSubtitle.StripUnusedLocales();
  }
}
