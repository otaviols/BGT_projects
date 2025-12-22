using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementSubcategoryDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_achievementCategoryId;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private string m_icon;
  [SerializeField]
  private int m_sortOrder;

  [DbfField("ACHIEVEMENT_CATEGORY_ID")]
  public int AchievementCategoryId => this.m_achievementCategoryId;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("ICON")]
  public string Icon => this.m_icon;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  public List<AchievementSectionItemDbfRecord> Sections
  {
    get
    {
      int id = this.ID;
      List<AchievementSectionItemDbfRecord> sections = new List<AchievementSectionItemDbfRecord>();
      List<AchievementSectionItemDbfRecord> records = GameDbf.AchievementSectionItem.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        AchievementSectionItemDbfRecord sectionItemDbfRecord = records[index];
        if (sectionItemDbfRecord.AchievementSubcategoryId == id)
          sections.Add(sectionItemDbfRecord);
      }
      return sections;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "ACHIEVEMENT_CATEGORY_ID")
      return (object) this.m_achievementCategoryId;
    if (name == "NAME")
      return (object) this.m_name;
    if (name == "ICON")
      return (object) this.m_icon;
    return name == "SORT_ORDER" ? (object) this.m_sortOrder : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "ACHIEVEMENT_CATEGORY_ID"))
      {
        if (!(name == "NAME"))
        {
          if (!(name == "ICON"))
          {
            if (!(name == "SORT_ORDER"))
              return;
            this.m_sortOrder = (int) val;
          }
          else
            this.m_icon = (string) val;
        }
        else
          this.m_name = (DbfLocValue) val;
      }
      else
        this.m_achievementCategoryId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "ACHIEVEMENT_CATEGORY_ID")
      return typeof (int);
    if (name == "NAME")
      return typeof (DbfLocValue);
    if (name == "ICON")
      return typeof (string);
    return name == "SORT_ORDER" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAchievementSubcategoryDbfRecords loadRecords = new LoadAchievementSubcategoryDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AchievementSubcategoryDbfAsset subcategoryDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AchievementSubcategoryDbfAsset)) as AchievementSubcategoryDbfAsset;
    if ((UnityEngine.Object) subcategoryDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AchievementSubcategoryDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < subcategoryDbfAsset.Records.Count; ++index)
      subcategoryDbfAsset.Records[index].StripUnusedLocales();
    records = subcategoryDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_name.StripUnusedLocales();
}
