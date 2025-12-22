using System.Collections.Generic;
using System.IO;

public class SpellStatistics
{
  private static Dictionary<int, string> s_hashToAssetRef = new Dictionary<int, string>();
  private static Dictionary<string, int> s_currentSpellCounts = new Dictionary<string, int>();
  private static Dictionary<string, int> s_unpooledSpellAcquisitionCount = new Dictionary<string, int>();
  private static Dictionary<string, int> s_pooledSpellAcquisitionCount = new Dictionary<string, int>();
  private static Dictionary<string, int> s_poolExpansionCount = new Dictionary<string, int>();
  private static Dictionary<string, int> s_exceededMaxCount = new Dictionary<string, int>();

  public static void AddSpell(Spell spell)
  {
    string name = spell.name;
    if (!SpellStatistics.s_currentSpellCounts.ContainsKey(name))
      SpellStatistics.s_currentSpellCounts.Add(name, 0);
    ++SpellStatistics.s_currentSpellCounts[name];
  }

  public static void LogNewPool(int hash, string spellAssetRef)
  {
    if (SpellStatistics.s_hashToAssetRef.ContainsKey(hash))
      return;
    SpellStatistics.s_hashToAssetRef.Add(hash, spellAssetRef);
  }

  public static void IncreasePooledSpellAcquisitionCount(int assetRefHash)
  {
    string key;
    if (!SpellStatistics.s_hashToAssetRef.TryGetValue(assetRefHash, out key))
      return;
    if (!SpellStatistics.s_pooledSpellAcquisitionCount.ContainsKey(key))
      SpellStatistics.s_pooledSpellAcquisitionCount.Add(key, 0);
    ++SpellStatistics.s_pooledSpellAcquisitionCount[key];
  }

  public static void IncreaseUnpooledSpellAcquisitionCount(string spellPrefabName)
  {
    if (!SpellStatistics.s_unpooledSpellAcquisitionCount.ContainsKey(spellPrefabName))
      SpellStatistics.s_unpooledSpellAcquisitionCount.Add(spellPrefabName, 0);
    ++SpellStatistics.s_unpooledSpellAcquisitionCount[spellPrefabName];
  }

  public static void CheckPoolSizeStats(int assetRefHash, Pool<Spell> spellPool)
  {
    string key;
    if (!SpellStatistics.s_hashToAssetRef.TryGetValue(assetRefHash, out key))
      return;
    if (spellPool.GetFreeList().Count == 0)
    {
      if (!SpellStatistics.s_poolExpansionCount.ContainsKey(key))
        SpellStatistics.s_poolExpansionCount.Add(key, 0);
      ++SpellStatistics.s_poolExpansionCount[key];
    }
    if (spellPool.GetMaxReleasedItemCount() > spellPool.GetActiveList().Count)
      return;
    if (!SpellStatistics.s_exceededMaxCount.ContainsKey(key))
      SpellStatistics.s_exceededMaxCount.Add(key, 0);
    ++SpellStatistics.s_exceededMaxCount[key];
  }

  public static void LogCurrentSpellCounts(FileInfo fileInfo)
  {
    using (StreamWriter streamWriter = new StreamWriter((Stream) fileInfo.OpenWrite()))
    {
      streamWriter.WriteLine("SpellName, Current Instantiated Count");
      int num1 = 0;
      foreach (KeyValuePair<string, int> currentSpellCount in SpellStatistics.s_currentSpellCounts)
      {
        int num2 = currentSpellCount.Value;
        num1 += num2;
        streamWriter.WriteLine(string.Format("{0},{1}", (object) currentSpellCount.Key, (object) num2));
      }
      streamWriter.WriteLine(string.Format(",Total,{0}", (object) num1));
    }
  }

  public static void LogUnpooledSpellAcquiredCounts(FileInfo fileInfo)
  {
    using (StreamWriter streamWriter = new StreamWriter((Stream) fileInfo.OpenWrite()))
    {
      streamWriter.WriteLine("SpellPrefab,Unique Instances Acquired");
      foreach (KeyValuePair<string, int> keyValuePair in SpellStatistics.s_unpooledSpellAcquisitionCount)
      {
        int num = keyValuePair.Value;
        streamWriter.WriteLine(string.Format("{0},{1}", (object) keyValuePair.Key, (object) num));
      }
    }
  }

  public static void LogPooledSpellAcquiredCounts(FileInfo fileInfo)
  {
    using (StreamWriter streamWriter = new StreamWriter((Stream) fileInfo.OpenWrite()))
    {
      streamWriter.WriteLine("SpellPrefab,Pooled Instances Acquired");
      foreach (KeyValuePair<string, int> keyValuePair in SpellStatistics.s_pooledSpellAcquisitionCount)
      {
        int num = keyValuePair.Value;
        streamWriter.WriteLine(string.Format("{0},{1}", (object) keyValuePair.Key, (object) num));
      }
    }
  }

  public static void LogPoolExpansions(FileInfo fileInfo)
  {
    using (StreamWriter streamWriter = new StreamWriter((Stream) fileInfo.OpenWrite()))
    {
      streamWriter.WriteLine("Spell Pool, Times Expanded");
      foreach (KeyValuePair<string, int> keyValuePair in SpellStatistics.s_poolExpansionCount)
      {
        int num = keyValuePair.Value;
        streamWriter.WriteLine(string.Format("{0},{1}", (object) keyValuePair.Key, (object) num));
      }
    }
  }

  public static void LogPoolExceedsMaxSize(FileInfo fileInfo)
  {
    using (StreamWriter streamWriter = new StreamWriter((Stream) fileInfo.OpenWrite()))
    {
      streamWriter.WriteLine("Spell Pool, Times exceeded maximum pool size");
      foreach (KeyValuePair<string, int> keyValuePair in SpellStatistics.s_exceededMaxCount)
      {
        int num = keyValuePair.Value;
        streamWriter.WriteLine(string.Format("{0},{1}", (object) keyValuePair.Key, (object) num));
      }
    }
  }

  internal static void RemoveSpell(Spell spell)
  {
    string name = spell.name;
    if (!SpellStatistics.s_currentSpellCounts.ContainsKey(name))
      return;
    --SpellStatistics.s_currentSpellCounts[name];
  }
}
