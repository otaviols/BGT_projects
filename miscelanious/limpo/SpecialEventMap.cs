using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpecialEventMap : ScriptableObject
{
  public const string kAssetFullPath = "Assets/Game/DBF-Asset/EventMap.asset";
  public const string kAssetPath = "Assets/Game/DBF-Asset/";
  public const string kAssetName = "/EventMap.asset";
  [SerializeField]
  private int m_currentId = 10000000;
  [SerializeField]
  private bool m_mappingInit;
  [SerializeField]
  private List<string> m_Keys = new List<string>();
  [SerializeField]
  private List<int> m_Values = new List<int>();
  private Dictionary<string, SpecialEventType> m_eventMap = new Dictionary<string, SpecialEventType>();

  public int CurrentId => this.m_currentId;

  public List<string> Keys => this.m_Keys;

  public List<int> Values => this.m_Values;

  public void Reset()
  {
    this.m_currentId = 10000000;
    this.m_mappingInit = false;
    this.m_Keys.Clear();
    this.m_Values.Clear();
    this.m_eventMap.Clear();
  }

  public void Initialize()
  {
    for (int index = 0; index < this.m_Keys.Count; ++index)
      this.m_eventMap.Add(this.m_Keys[index], (SpecialEventType) this.m_Values[index]);
  }

  public SpecialEventType ConvertStringToSpecialEvent(string eventName)
  {
    if (string.IsNullOrEmpty(eventName))
      return SpecialEventType.UNKNOWN;
    if (!this.m_mappingInit)
    {
      this.m_mappingInit = true;
      foreach (SpecialEventType enumVal in Enum.GetValues(typeof (SpecialEventType)))
      {
        string key = EnumUtils.GetString<SpecialEventType>(enumVal);
        this.m_eventMap.Add(key, enumVal);
        this.m_Keys.Add(key);
        this.m_Values.Add((int) enumVal);
      }
    }
    SpecialEventType specialEvent;
    if (this.m_eventMap.TryGetValue(eventName, out specialEvent))
      return specialEvent;
    ++this.m_currentId;
    SpecialEventType currentId = (SpecialEventType) this.m_currentId;
    this.m_eventMap[eventName] = currentId;
    this.m_Keys.Add(eventName);
    this.m_Values.Add((int) currentId);
    return currentId;
  }
}
