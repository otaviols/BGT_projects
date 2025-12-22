using Blizzard.T5.Jobs;
using Hearthstone;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Dbf<T> : IDbf where T : DbfRecord, new()
{
  private string m_name;
  private List<T> m_records = new List<T>();
  private Dictionary<int, T> m_recordsById = new Dictionary<int, T>();

  private event Dbf<T>.RecordAddedListener m_recordAddedListener;

  private event Dbf<T>.RecordsRemovedListener m_recordsRemovedListener;

  public Dbf(string name) => this.m_name = name;

  public void CopyRecords(Dbf<T> other)
  {
    this.m_records.AddRange((IEnumerable<T>) other.m_records);
    foreach (KeyValuePair<int, T> keyValuePair in other.m_recordsById)
      this.m_recordsById.Add(keyValuePair.Key, keyValuePair.Value);
  }

  public void AddListeners(
    Dbf<T>.RecordAddedListener addedListener,
    Dbf<T>.RecordsRemovedListener removedListener)
  {
    if (addedListener != null)
      this.m_recordAddedListener += addedListener;
    if (removedListener == null)
      return;
    this.m_recordsRemovedListener += removedListener;
  }

  public DbfRecord CreateNewRecord() => (DbfRecord) new T();

  public void AddRecord(DbfRecord record)
  {
    T record1 = (T) record;
    this.m_records.Add(record1);
    this.m_recordsById[record.ID] = record1;
    if (this.m_recordAddedListener == null)
      return;
    this.m_recordAddedListener(record1);
  }

  public List<T> GetRecords() => this.m_records;

  public List<T> GetRecords(Predicate<T> predicate, int limit = -1) => limit >= 0 ? this.m_records.FindAll(predicate).GetRange(0, limit) : this.m_records.FindAll(predicate);

  public static Dbf<T> Load(string name, DbfFormat format)
  {
    string assetPath = format == DbfFormat.XML ? Dbf<T>.GetXmlPath(name) : Dbf<T>.GetAssetPath(name, Localization.GetActualLocale());
    return Dbf<T>.Load(name, assetPath, format);
  }

  public static Dbf<T> Load(string name, string assetPath, DbfFormat format)
  {
    Dbf<T> dbf = new Dbf<T>(name);
    dbf.Clear();
    bool flag = format == DbfFormat.XML;
    if (!(!flag ? dbf.LoadScriptableObject(assetPath) : DbfXml.Load<T>(assetPath, dbf)))
    {
      dbf.Clear();
      Log.Dbf.Print(string.Format("Dbf.Load[{0}] - failed to load {1} at {2}", flag ? (object) "Xml" : (object) "ScriptableObject", (object) name, (object) assetPath));
    }
    GameDbf.RegisterDbf((IDbf) dbf);
    return dbf;
  }

  public static JobDefinition CreateLoadAsyncJob(
    string name,
    DbfFormat format,
    ref Dbf<T> dbf)
  {
    string assetPath = format == DbfFormat.XML ? Dbf<T>.GetXmlPath(name) : Dbf<T>.GetAssetPath(name, Localization.GetActualLocale());
    return Dbf<T>.CreateLoadAsyncJob(name, assetPath, format, ref dbf);
  }

  public static JobDefinition CreateLoadAsyncJob(
    string name,
    string assetPath,
    DbfFormat format,
    ref Dbf<T> dbf)
  {
    dbf = new Dbf<T>(name);
    dbf.Clear();
    GameDbf.RegisterDbf((IDbf) dbf);
    return format != DbfFormat.XML ? new JobDefinition(Dbf<T>.MakeJobName(typeof (T)), dbf.Job_LoadScriptableObjectAsync(assetPath), Array.Empty<IJobDependency>()) : new JobDefinition(Dbf<T>.MakeJobName(typeof (T)), DbfXml.Job_LoadAsync<T>(assetPath, dbf), JobFlags.StartImmediately, Array.Empty<IJobDependency>());
  }

  private static string MakeJobName(System.Type t) => string.Format("Dbf.LoadAsync[{0}]", (object) t.ToString());

  public string GetName() => this.m_name;

  public void Clear()
  {
    this.m_records.Clear();
    this.m_recordsById.Clear();
  }

  public T GetRecord(int id)
  {
    T record;
    this.m_recordsById.TryGetValue(id, out record);
    return record;
  }

  public T GetRecord(Predicate<T> match) => this.m_records.Find(match);

  public bool HasRecord(int id)
  {
    T obj = default (T);
    this.m_recordsById.TryGetValue(id, out obj);
    return (object) obj != null;
  }

  public bool HasRecord(Predicate<T> match) => (object) this.GetRecord(match) != null;

  public void ReplaceRecordByRecordId(T record)
  {
    int index = this.m_records.FindIndex((Predicate<T>) (r => r.ID == record.ID));
    if (index == -1)
    {
      this.AddRecord((DbfRecord) record);
    }
    else
    {
      T record1 = this.m_records[index];
      int num = (object) record1 != (object) record ? 1 : 0;
      if (num != 0 && this.m_recordsRemovedListener != null)
        this.m_recordsRemovedListener(new List<T>()
        {
          record1
        });
      this.m_records[index] = record;
      this.m_recordsById[record.ID] = record;
      if (num == 0 || this.m_recordAddedListener == null)
        return;
      this.m_recordAddedListener(record);
    }
  }

  public void RemoveRecordsWhere(Predicate<T> match)
  {
    List<int> intList = (List<int>) null;
    int index1 = 0;
    for (int count = this.m_records.Count; index1 < count; ++index1)
    {
      if (match(this.m_records[index1]))
      {
        if (intList == null)
          intList = new List<int>();
        intList.Add(index1);
      }
    }
    if (intList == null)
      return;
    List<T> removedRecords = (List<T>) null;
    if (this.m_recordsRemovedListener != null)
      removedRecords = new List<T>(intList.Count);
    for (int index2 = intList.Count - 1; index2 >= 0; --index2)
    {
      int index3 = intList[index2];
      T record = this.m_records[index3];
      if (removedRecords != null && (object) record != null)
        removedRecords.Add(record);
      T obj;
      if (this.m_recordsById.TryGetValue(record.ID, out obj))
        this.m_recordsById.Remove(obj.ID);
      this.m_records.RemoveAt(index3);
    }
    if (this.m_recordsRemovedListener == null)
      return;
    this.m_recordsRemovedListener(removedRecords);
  }

  public override string ToString() => this.m_name;

  private static string GetXmlPath(string name)
  {
    string subPath = string.Format("UnimportedAssets/DBF/{0}.xml", (object) name);
    string outPath;
    if (HearthstoneApplication.TryGetStandaloneLocalDataPath(subPath, out outPath))
      return outPath;
    if (!Application.isEditor)
      subPath = string.Format("DBF/{0}.xml", (object) name);
    return subPath;
  }

  private static string GetAssetPath(string name, Locale locale) => string.Format("Assets/Game/DBF-Asset/{0}/{1}.asset", (object) locale, (object) name);

  public bool LoadScriptableObject(string resourcePath)
  {
    if (!new T().LoadRecordsFromAsset<T>(resourcePath, out this.m_records))
      return false;
    if (this.m_records.Count < 1 && this.m_name != "SUBSET_CARD")
      Debug.LogErrorFormat("{0} DBF Asset has 0 records! Something went wrong generating it. Try checking the generated XMLs in the DBF folder.", (object) this.m_name);
    int index = 0;
    for (int count = this.m_records.Count; index < count; ++index)
    {
      T record = this.m_records[index];
      this.m_recordsById[record.ID] = record;
      if (this.m_recordAddedListener != null)
        this.m_recordAddedListener(record);
    }
    return true;
  }

  public IEnumerator<IAsyncJobResult> Job_LoadScriptableObjectAsync(
    string resourcePath)
  {
    Action<List<T>> resultHandler = (Action<List<T>>) (records =>
    {
      this.m_records = records ?? new List<T>();
      if (this.m_records.Count < 1 && this.m_name != "SUBSET_CARD")
        Debug.LogErrorFormat("{0} DBF Asset has 0 records! Something went wrong generating it. Try checking the generated XMLs in the DBF folder.", (object) this.m_name);
      int index = 0;
      for (int count = this.m_records.Count; index < count; ++index)
      {
        T record = this.m_records[index];
        this.m_recordsById[record.ID] = record;
        if (this.m_recordAddedListener != null)
          this.m_recordAddedListener(record);
      }
    });
    return new T().Job_LoadRecordsFromAssetAsync<T>(resourcePath, resultHandler);
  }

  public delegate void RecordAddedListener(T record) where T : DbfRecord, new();

  public delegate void RecordsRemovedListener(List<T> removedRecords) where T : DbfRecord, new();
}
