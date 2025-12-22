using Assets;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameStringTable
{
  public const string KEY_FIELD_NAME = "TAG";
  public const string VALUE_FIELD_NAME = "TEXT";
  private Global.GameStringCategory m_category;
  private Dictionary<string, string> m_table = new Dictionary<string, string>();

  public bool Load(Global.GameStringCategory cat, bool native = false)
  {
    string pathWithLoadOrder1 = GameStringTable.GetFilePathWithLoadOrder(cat, native, new GameStringTable.FilePathFromCategoryCallback(GameStringTable.GetFilePathFromCategory));
    string pathWithLoadOrder2 = GameStringTable.GetFilePathWithLoadOrder(cat, false, new GameStringTable.FilePathFromCategoryCallback(GameStringTable.GetAudioFilePathFromCategory));
    return this.Load(cat, pathWithLoadOrder1, pathWithLoadOrder2);
  }

  public bool Load(Global.GameStringCategory cat, Locale locale, bool native = false)
  {
    string pathFromCategory1 = GameStringTable.GetFilePathFromCategory(cat, locale, native);
    string pathFromCategory2 = GameStringTable.GetAudioFilePathFromCategory(cat, locale, native);
    return this.Load(cat, pathFromCategory1, pathFromCategory2);
  }

  public bool Load(Global.GameStringCategory cat, string path, string audioPath)
  {
    this.m_category = Global.GameStringCategory.INVALID;
    this.m_table.Clear();
    if (File.Exists(path) && !this.LoadFile(path))
    {
      Error.AddDevWarningNonRepeating("GameStrings Error", "GameStringTable.Load() - Failed to load {0} for category {1}.", (object) path, (object) cat);
      return false;
    }
    if (File.Exists(audioPath) && !this.LoadFile(audioPath))
    {
      Error.AddDevWarningNonRepeating("GameStrings Error", "GameStringTable.Load() - Failed to load {0} for category {1}.", (object) audioPath, (object) cat);
      return false;
    }
    if (this.m_table.Count == 0)
    {
      Error.AddDevWarningNonRepeating("GameStrings Error", "GameStringTable.Load() - There are no entries for category {0} - path: {1}.", (object) cat, (object) path);
      return false;
    }
    this.m_category = cat;
    return true;
  }

  public string Get(string key)
  {
    string str;
    this.m_table.TryGetValue(key, out str);
    return str;
  }

  public Dictionary<string, string> GetAll() => this.m_table;

  public Global.GameStringCategory GetCategory() => this.m_category;

  public bool LoadFile(string path)
  {
    GameStringTable.Header header = (GameStringTable.Header) null;
    bool flag = false;
    try
    {
      using (ReadOnlySpanStreamReader spanStreamReader = new ReadOnlySpanStreamReader(path))
      {
        while (spanStreamReader.CanReadLine())
        {
          ReadOnlySpan<char> lineSpan = spanStreamReader.ReadLine();
          if (!(lineSpan == (ReadOnlySpan<char>) (char[]) null))
          {
            if (!flag)
            {
              header = GameStringTable.LoadFileHeader(lineSpan);
              flag = header != null;
            }
            else
              this.LoadEntry(header, lineSpan);
          }
          else
            break;
        }
      }
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) string.Format("GameStringTable.LoadFile() - Failed to read \"{0}\".\n\nException: {1}", (object) path, (object) ex.Message));
      return false;
    }
    if (flag)
      return true;
    Debug.LogWarning((object) string.Format("GameStringTable.LoadFile() - \"{0}\" had a malformed header.", (object) path));
    return false;
  }

  private static string GetFilePathWithLoadOrder(
    Global.GameStringCategory cat,
    bool native,
    GameStringTable.FilePathFromCategoryCallback pathCallback)
  {
    Locale actualLocale = Localization.GetActualLocale();
    string path = pathCallback(cat, actualLocale, native);
    if (File.Exists(path))
      return path;
    Log.Downloader.PrintDebug("category {0}, native {1}, locale {2}.", (object) cat, (object) native, (object) Localization.GetLocaleName());
    return (string) null;
  }

  private static string GetFilePathFromCategory(
    Global.GameStringCategory cat,
    Locale locale,
    bool native)
  {
    string fileName = string.Format("{0}.txt", (object) cat);
    return GameStrings.GetAssetPath(locale, fileName, native);
  }

  private static string GetAudioFilePathFromCategory(
    Global.GameStringCategory cat,
    Locale locale,
    bool native)
  {
    string fileName = string.Format("{0}_AUDIO.txt", (object) cat);
    return GameStrings.GetAssetPath(locale, fileName, native);
  }

  private static GameStringTable.Header LoadFileHeader(ReadOnlySpan<char> lineSpan)
  {
    if (lineSpan.Length == 0)
      return (GameStringTable.Header) null;
    if (lineSpan[0] == '#')
      return (GameStringTable.Header) null;
    GameStringTable.Header header = new GameStringTable.Header();
    ReadOnlySpanExtensions.SplitEnumerator splitEnumerator = lineSpan.Split('\t');
    int num = 0;
    foreach (ReadOnlySpan<char> span in splitEnumerator)
    {
      if (MemoryExtensions.Equals(span, "TAG".AsSpan(), StringComparison.InvariantCulture))
      {
        header.m_keyIndex = num;
        if (header.m_valueIndex >= 0)
          break;
      }
      else if (MemoryExtensions.Equals(span, "TEXT".AsSpan(), StringComparison.InvariantCulture))
      {
        header.m_valueIndex = num;
        if (header.m_keyIndex >= 0)
          break;
      }
      ++num;
    }
    return header.m_keyIndex < 0 && header.m_valueIndex < 0 ? (GameStringTable.Header) null : header;
  }

  private void LoadEntry(GameStringTable.Header header, ReadOnlySpan<char> lineSpan)
  {
    if (lineSpan.Length == 0 || lineSpan[0] == '#' || !lineSpan.HasNonSpaceCharacter())
      return;
    ReadOnlySpanExtensions.SplitEnumerator splitEnumerator = lineSpan.Split('\t');
    string key = (string) null;
    string str = (string) null;
    int num = 0;
    bool flag1 = false;
    bool flag2 = false;
    foreach (ReadOnlySpan<char> span in splitEnumerator)
    {
      if (num == header.m_keyIndex)
      {
        key = span.ToString();
        flag1 = true;
      }
      else if (num == header.m_valueIndex)
      {
        str = TextUtils.DecodeWhitespaces(span.Trim().ToString());
        flag2 = true;
      }
      if (!(flag1 & flag2))
        ++num;
      else
        break;
    }
    if (!flag1)
      return;
    this.m_table[key] = flag2 ? str : string.Empty;
  }

  public class Entry
  {
    public string m_key;
    public string m_value;
  }

  public class Header
  {
    public int m_entryStartIndex = -1;
    public int m_keyIndex = -1;
    public int m_valueIndex = -1;
  }

  private delegate string FilePathFromCategoryCallback(
    Global.GameStringCategory cat,
    Locale locale,
    bool native);
}
