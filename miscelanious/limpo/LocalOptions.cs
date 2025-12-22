using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Hearthstone.Core.Streaming;
using Hearthstone.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LocalOptions
{
  private static LocalOptions s_instance;
  private string m_path;
  private LocalOptions.LoadResult m_loadResult;
  private int m_currentLineVersion;
  private Map<string, object> m_options = new Map<string, object>();
  private List<string> m_sortedKeys = new List<string>();
  private List<string> m_temporaryKeys = new List<string>();
  private bool m_dirty;
  private Dictionary<string, Option> m_cachedOptionDescriptions;

  public static LocalOptions Get()
  {
    if (LocalOptions.s_instance == null)
      LocalOptions.s_instance = new LocalOptions();
    return LocalOptions.s_instance;
  }

  public static string OptionsPath
  {
    get
    {
      string path = string.Format("{0}/{1}", (object) PlatformFilePaths.ExternalDataPath, (object) PlatformFilePaths.GetOptionsFileName());
      if (!File.Exists(path))
        path = string.Format("{0}/{1}", (object) PlatformFilePaths.PersistentDataPath, (object) PlatformFilePaths.GetOptionsFileName());
      return path;
    }
  }

  public void Initialize()
  {
    this.m_path = LocalOptions.OptionsPath;
    this.m_currentLineVersion = 2;
    if (this.Load())
      OptionsMigration.UpgradeClientOptions();
    LaunchArguments.AddEnabledLogInOptions((string) null);
  }

  public void Clear()
  {
    this.m_dirty = false;
    this.m_options.Clear();
    this.m_sortedKeys.Clear();
  }

  public bool Has(string key) => this.m_options.ContainsKey(key);

  public void Delete(string key)
  {
    if (!this.m_options.Remove(key))
      return;
    this.m_sortedKeys.Remove(key);
    this.m_dirty = true;
    this.Save(key);
  }

  public T Get<T>(string key)
  {
    object obj;
    return !this.m_options.TryGetValue(key, out obj) ? default (T) : (T) obj;
  }

  public bool GetBool(string key) => this.Get<bool>(key);

  public int GetInt(string key) => this.Get<int>(key);

  public long GetLong(string key) => this.Get<long>(key);

  public ulong GetULong(string key) => this.Get<ulong>(key);

  public float GetFloat(string key) => this.Get<float>(key);

  public string GetString(string key) => this.Get<string>(key);

  public void Set(string key, object val) => this.Set(key, val, true);

  public void Set(string key, object val, bool permanent)
  {
    object obj;
    if (this.m_options.TryGetValue(key, out obj))
    {
      if (obj == val || obj != null && obj.Equals(val))
        return;
    }
    else
    {
      this.m_sortedKeys.Add(key);
      this.SortKeys();
    }
    this.m_options[key] = val;
    if (permanent)
    {
      this.m_temporaryKeys.Remove(key);
      this.m_dirty = true;
      this.Save(key);
    }
    else
      this.m_temporaryKeys.Add(key);
  }

  public void SetByLine(string line, bool permanent)
  {
    string key;
    object val;
    if (!this.LoadLine(line, out int _, out key, out val, out bool _))
      Log.ConfigFile.PrintError("LoadLine failed with '{0}'", (object) line);
    else
      this.Set(key, val, permanent);
  }

  private bool Load()
  {
    this.Clear();
    Log.ConfigFile.Print("Loading Options File: {0}", (object) this.m_path);
    if (!File.Exists(this.m_path))
    {
      this.m_loadResult = LocalOptions.LoadResult.SUCCESS;
      return true;
    }
    string[] lines;
    if (!this.LoadFile(out lines))
    {
      this.m_loadResult = LocalOptions.LoadResult.FAIL;
      return false;
    }
    bool formatChanged = false;
    if (!this.LoadAllLines(lines, out formatChanged))
    {
      this.m_loadResult = LocalOptions.LoadResult.FAIL;
      return false;
    }
    foreach (string key in this.m_options.Keys)
      this.m_sortedKeys.Add(key);
    this.SortKeys();
    this.m_loadResult = LocalOptions.LoadResult.SUCCESS;
    if (formatChanged)
    {
      this.m_dirty = true;
      this.Save();
    }
    return true;
  }

  private bool LoadFile(out string[] lines)
  {
    try
    {
      lines = File.ReadAllLines(this.m_path);
      return true;
    }
    catch (Exception ex)
    {
      Debug.LogError((object) string.Format("LocalOptions.LoadFile() - Failed to read {0}. Exception={1}", (object) this.m_path, (object) ex.Message));
      lines = (string[]) null;
      return false;
    }
  }

  private bool LoadAllLines(string[] lines, out bool formatChanged)
  {
    formatChanged = false;
    int num = 0;
    for (int index = 0; index < lines.Length; ++index)
    {
      string line = lines[index].Trim();
      if (line.Length != 0 && !line.StartsWith("#"))
      {
        int version;
        string key;
        object val;
        bool formatChanged1;
        if (!this.LoadLine(line, out version, out key, out val, out formatChanged1))
        {
          Debug.LogError((object) string.Format("LocalOptions.LoadAllLines() - Failed to load line {0}\n\"{1}\".", (object) (index + 1), (object) line));
          ++num;
          if (num > 4)
          {
            this.m_loadResult = LocalOptions.LoadResult.FAIL;
            return false;
          }
        }
        else
        {
          this.m_options[key] = val;
          formatChanged = ((formatChanged ? 1 : (version != this.m_currentLineVersion ? 1 : 0)) | (formatChanged1 ? 1 : 0)) != 0;
        }
      }
    }
    return true;
  }

  private bool LoadLine(
    string line,
    out int version,
    out string key,
    out object val,
    out bool formatChanged)
  {
    version = 0;
    key = (string) null;
    val = (object) null;
    formatChanged = false;
    int num = 0;
    string key1 = (string) null;
    string str = (string) null;
    bool flag1 = false;
    string separator = "=";
    line = line.Trim();
    string[] arr = line.Split(separator[0]);
    if (arr.Length >= 2)
    {
      key1 = arr[0].Trim();
      str = arr.Length != 2 ? string.Join(separator, arr.Slice<string>(1)).Trim() : arr[1].Trim();
      if (string.IsNullOrEmpty(key1) || string.IsNullOrEmpty(str))
        flag1 = true;
      num = 2;
    }
    else
      flag1 = true;
    if (flag1)
      return false;
    if (this.m_cachedOptionDescriptions == null)
    {
      Array values = Enum.GetValues(typeof (Option));
      this.m_cachedOptionDescriptions = new Dictionary<string, Option>(values.Length);
      foreach (Option enumVal in values)
        this.m_cachedOptionDescriptions.Add(EnumUtils.GetString<Option>(enumVal), enumVal);
    }
    Option key2 = Option.INVALID;
    if (!this.m_cachedOptionDescriptions.TryGetValue(key1, out key2))
    {
      version = num;
      key = key1;
      val = (object) str;
      return true;
    }
    bool flag2 = false;
    int val1;
    Locale outVal;
    if (key2 == Option.LOCALE && GeneralUtils.TryParseInt(str, out val1) && EnumUtils.TryCast<Locale>((object) val1, out outVal))
    {
      str = outVal.ToString();
      flag2 = true;
    }
    System.Type type = OptionDataTables.s_typeMap[key2];
    if (type == typeof (bool))
      val = (object) GeneralUtils.ForceBool(str);
    else if (type == typeof (int))
      val = (object) GeneralUtils.ForceInt(str);
    else if (type == typeof (long))
      val = (object) GeneralUtils.ForceLong(str);
    else if (type == typeof (ulong))
      val = (object) GeneralUtils.ForceULong(str);
    else if (type == typeof (float))
    {
      val = (object) GeneralUtils.ForceFloat(str);
    }
    else
    {
      if (!(type == typeof (string)))
        return false;
      val = (object) str;
    }
    version = num;
    key = key1;
    formatChanged = flag2;
    return true;
  }

  private bool Save(string triggerKey)
  {
    switch (this.m_loadResult)
    {
      case LocalOptions.LoadResult.INVALID:
      case LocalOptions.LoadResult.FAIL:
        return false;
      default:
        return this.Save();
    }
  }

  private bool Save()
  {
    if (!this.m_dirty)
      return true;
    List<string> lines = new List<string>();
    for (int index = 0; index < this.m_sortedKeys.Count; ++index)
    {
      string sortedKey = this.m_sortedKeys[index];
      if (!this.m_temporaryKeys.Contains(sortedKey))
      {
        object option = this.m_options[sortedKey];
        string str = string.Format("{0}={1}", (object) sortedKey, option);
        lines.Add(str);
      }
    }
    bool flag = this.WriteOptionsFile(string.Format("{0}/{1}", (object) PlatformFilePaths.ExternalDataPath, (object) PlatformFilePaths.GetOptionsFileName()), lines);
    if (!flag)
      flag = this.WriteOptionsFile(string.Format("{0}/{1}", (object) PlatformFilePaths.PersistentDataPath, (object) PlatformFilePaths.GetOptionsFileName()), lines);
    return flag;
  }

  private bool WriteOptionsFile(string optionsFilePath, List<string> lines)
  {
    try
    {
      File.WriteAllLines(optionsFilePath, lines.ToArray(), (Encoding) new UTF8Encoding());
    }
    catch (Exception ex)
    {
      Debug.LogError((object) string.Format("LocalOptions.Save() - Failed to save {0}. Exception={1}", (object) optionsFilePath, (object) ex.Message));
      return false;
    }
    this.m_dirty = false;
    return true;
  }

  private void SortKeys() => this.m_sortedKeys.Sort(new Comparison<string>(this.KeyComparison));

  private int KeyComparison(string key1, string key2) => string.Compare(key1, key2, true);

  private enum LoadResult
  {
    INVALID,
    SUCCESS,
    FAIL,
  }
}
