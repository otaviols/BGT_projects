using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementSectionItemDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_achievementSubcategoryId;
  [SerializeField]
  private int m_achievementSectionId;
  [SerializeField]
  private int m_sortOrder;

  [DbfField("ACHIEVEMENT_SUBCATEGORY_ID")]
  public int AchievementSubcategoryId => this.m_achievementSubcategoryId;

  public AchievementSectionDbfRecord AchievementSectionRecord => GameDbf.AchievementSection.GetRecord(this.m_achievementSectionId);

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "ACHIEVEMENT_SUBCATEGORY_ID")
      return (object) this.m_achievementSubcategoryId;
    if (name == "ACHIEVEMENT_SECTION")
      return (object) this.m_achievementSectionId;
    return name == "SORT_ORDER" ? (object) this.m_sortOrder : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "ACHIEVEMENT_SUBCATEGORY_ID"))
      {
        if (!(name == "ACHIEVEMENT_SECTION"))
        {
          if (!(name == "SORT_ORDER"))
            return;
          this.m_sortOrder = (int) val;
        }
        else
          this.m_achievementSectionId = (int) val;
      }
      else
        this.m_achievementSubcategoryId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "ACHIEVEMENT_SUBCATEGORY_ID")
      return typeof (int);
    if (name == "ACHIEVEMENT_SECTION")
      return typeof (int);
    return name == "SORT_ORDER" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAchievementSectionItemDbfRecords loadRecords = new LoadAchievementSectionItemDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AchievementSectionItemDbfAsset sectionItemDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AchievementSectionItemDbfAsset)) as AchievementSectionItemDbfAsset;
    if ((UnityEngine.Object) sectionItemDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AchievementSectionItemDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < sectionItemDbfAsset.Records.Count; ++index)
      sectionItemDbfAsset.Records[index].StripUnusedLocales();
    records = sectionItemDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
