using Blizzard.T5.Core;
using System.Collections.Generic;
using UnityEngine;

public abstract class AdventureDefCacheBase<DefType, DefIDType, DbfRecord> where DefType : Component
{
  private Map<DefIDType, DefType> m_defCache = new Map<DefIDType, DefType>();

  public IEnumerable<DefType> Values => (IEnumerable<DefType>) this.m_defCache.Values;

  public AdventureDefCacheBase(bool preloadRecords)
  {
    if (!preloadRecords)
      return;
    this.LoadFromRecords(this.GetRecords());
  }

  public DefType GetDef(DefIDType defId)
  {
    DefType def = default (DefType);
    this.m_defCache.TryGetValue(defId, out def);
    return def;
  }

  public bool LoadDefForId(DefIDType defId)
  {
    if (this.m_defCache.ContainsKey(defId))
    {
      Debug.LogWarningFormat("Attempted to load a {0} that was already loaded for id {1}", (object) typeof (DefType).Name, (object) defId);
      return true;
    }
    DbfRecord recordForDefId = this.GetRecordForDefId(defId);
    if ((object) recordForDefId == null || string.IsNullOrEmpty(this.GetPrefabFromRecord(recordForDefId)))
      return false;
    this.AddDef(GameUtils.LoadGameObjectWithComponent<DefType>(this.GetPrefabFromRecord(recordForDefId)), recordForDefId);
    return true;
  }

  public void Unload()
  {
    foreach (KeyValuePair<DefIDType, DefType> keyValuePair in this.m_defCache)
    {
      if (!((Object) keyValuePair.Value == (Object) null))
      {
        GameObject gameObject = keyValuePair.Value.gameObject;
        if ((Object) gameObject != (Object) null)
          Object.Destroy((Object) gameObject);
      }
    }
    this.m_defCache.Clear();
  }

  private void LoadFromRecords(List<DbfRecord> records)
  {
    if (this.m_defCache.Count > 0)
      Debug.LogErrorFormat("Attempting to load all {0} when they were already loaded!", (object) typeof (DefType).Name);
    foreach (DbfRecord record in records)
    {
      DefType def = GameUtils.LoadGameObjectWithComponent<DefType>(this.GetPrefabFromRecord(record));
      if (!((Object) def == (Object) null))
        this.AddDef(def, record);
    }
  }

  private void AddDef(DefType def, DbfRecord record)
  {
    this.InitalizeDef(def, record);
    this.m_defCache.Add(this.GetDefId(def), def);
  }

  protected abstract List<DbfRecord> GetRecords();

  protected abstract string GetPrefabFromRecord(DbfRecord record);

  protected abstract void InitalizeDef(DefType def, DbfRecord record);

  protected abstract DefIDType GetDefId(DefType def);

  protected abstract DbfRecord GetRecordForDefId(DefIDType id);
}
