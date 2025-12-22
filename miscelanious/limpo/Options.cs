using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Options
{
  private static readonly ServerOption[] s_serverFlagContainers = new ServerOption[10]
  {
    ServerOption.FLAGS1,
    ServerOption.FLAGS2,
    ServerOption.FLAGS3,
    ServerOption.FLAGS4,
    ServerOption.FLAGS5,
    ServerOption.FLAGS6,
    ServerOption.FLAGS7,
    ServerOption.FLAGS8,
    ServerOption.FLAGS9,
    ServerOption.FLAGS10
  };
  private static Options s_instance;
  private Map<Option, string> m_clientOptionMap;
  private Map<Option, ServerOption> m_serverOptionMap;
  private Map<Option, ServerOptionFlag> m_serverOptionFlagMap;
  private Map<Option, List<Options.ChangedListener>> m_changedListeners = new Map<Option, List<Options.ChangedListener>>();
  private List<Options.ChangedListener> m_globalChangedListeners = new List<Options.ChangedListener>();

  public static Options Get()
  {
    if (Options.s_instance == null)
    {
      Options.s_instance = new Options();
      Options.s_instance.Initialize();
    }
    return Options.s_instance;
  }

  public Map<Option, string> GetClientOptions() => this.m_clientOptionMap;

  public System.Type GetOptionType(Option option)
  {
    System.Type optionType;
    if (OptionDataTables.s_typeMap.TryGetValue(option, out optionType))
      return optionType;
    return this.m_serverOptionFlagMap.ContainsKey(option) ? typeof (bool) : (System.Type) null;
  }

  public System.Type GetServerOptionType(ServerOption serverOption)
  {
    if (Array.Exists<ServerOption>(Options.s_serverFlagContainers, (Predicate<ServerOption>) (flagContainer => flagContainer == serverOption)))
      return typeof (ulong);
    foreach (KeyValuePair<Option, ServerOption> serverOption1 in this.m_serverOptionMap)
    {
      if (serverOption1.Value == serverOption)
      {
        Option key = serverOption1.Key;
        System.Type serverOptionType;
        if (OptionDataTables.s_typeMap.TryGetValue(key, out serverOptionType))
          return serverOptionType;
        break;
      }
    }
    return (System.Type) null;
  }

  public static FormatType GetFormatType() => Options.Get().GetEnum<FormatType>(Option.FORMAT_TYPE);

  public static void SetFormatType(FormatType formatType)
  {
    if (formatType == FormatType.FT_UNKNOWN)
      RankMgr.LogMessage("Options.SetFormatType()  format type somehow got passed in as FT_UNKOWN", nameof (SetFormatType), "D:\\builders\\work\\source\\25.0.0\\Pegasus\\Client\\Assets\\Shared\\Scripts\\Game\\Options.cs", 151);
    else
      Options.Get().SetEnum<FormatType>(Option.FORMAT_TYPE, formatType);
  }

  public static bool GetInRankedPlayMode() => Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE);

  public static void SetInRankedPlayMode(bool inRankedPlayMode) => Options.Get().SetBool(Option.IN_RANKED_PLAY_MODE, inRankedPlayMode);

  public bool RegisterChangedListener(Option option, Options.ChangedCallback callback) => this.RegisterChangedListener(option, callback, (object) null);

  public bool RegisterChangedListener(
    Option option,
    Options.ChangedCallback callback,
    object userData)
  {
    Options.ChangedListener changedListener = new Options.ChangedListener();
    changedListener.SetCallback(callback);
    changedListener.SetUserData(userData);
    List<Options.ChangedListener> changedListenerList;
    if (!this.m_changedListeners.TryGetValue(option, out changedListenerList))
    {
      changedListenerList = new List<Options.ChangedListener>();
      this.m_changedListeners.Add(option, changedListenerList);
    }
    else if (changedListenerList.Contains(changedListener))
      return false;
    changedListenerList.Add(changedListener);
    return true;
  }

  public bool UnregisterChangedListener(Option option, Options.ChangedCallback callback) => this.UnregisterChangedListener(option, callback, (object) null);

  public bool UnregisterChangedListener(
    Option option,
    Options.ChangedCallback callback,
    object userData)
  {
    Options.ChangedListener changedListener = new Options.ChangedListener();
    changedListener.SetCallback(callback);
    changedListener.SetUserData(userData);
    List<Options.ChangedListener> changedListenerList;
    if (!this.m_changedListeners.TryGetValue(option, out changedListenerList) || !changedListenerList.Remove(changedListener))
      return false;
    if (changedListenerList.Count == 0)
      this.m_changedListeners.Remove(option);
    return true;
  }

  public bool HasOption(Option option)
  {
    string key;
    if (this.m_clientOptionMap.TryGetValue(option, out key))
      return LocalOptions.Get().Has(key);
    ServerOption type;
    if (this.m_serverOptionMap.TryGetValue(option, out type))
      return NetCache.Get().ClientOptionExists(type);
    ServerOptionFlag serverOptionFlag;
    return this.m_serverOptionFlagMap.TryGetValue(option, out serverOptionFlag) && this.HasServerOptionFlag(serverOptionFlag);
  }

  public void DeleteOption(Option option)
  {
    string optionName;
    if (this.m_clientOptionMap.TryGetValue(option, out optionName))
    {
      this.DeleteClientOption(option, optionName);
    }
    else
    {
      ServerOption serverOption;
      if (this.m_serverOptionMap.TryGetValue(option, out serverOption))
      {
        this.DeleteServerOption(option, serverOption);
      }
      else
      {
        ServerOptionFlag serverOptionFlag;
        if (!this.m_serverOptionFlagMap.TryGetValue(option, out serverOptionFlag))
          return;
        this.DeleteServerOptionFlag(option, serverOptionFlag);
      }
    }
  }

  public void DeleteOption(string optionStr)
  {
    Option option;
    try
    {
      option = EnumUtils.GetEnum<Option>(optionStr, StringComparison.OrdinalIgnoreCase);
    }
    catch (ArgumentException ex)
    {
      Debug.LogErrorFormat("No matched option with '{0}'", (object) optionStr);
      return;
    }
    this.DeleteOption(option);
  }

  public object GetOption(Option option)
  {
    object val;
    if (this.GetOptionImpl(option, out val))
      return val;
    object obj;
    return OptionDataTables.s_defaultsMap.TryGetValue(option, out obj) ? obj : (object) null;
  }

  public object GetOption(Option option, object defaultVal)
  {
    object val;
    return this.GetOptionImpl(option, out val) ? val : defaultVal;
  }

  public bool GetBool(Option option)
  {
    bool val;
    if (this.GetBoolImpl(option, out val))
      return val;
    object obj;
    return OptionDataTables.s_defaultsMap.TryGetValue(option, out obj) && (bool) obj;
  }

  public bool GetBool(Option option, bool defaultVal)
  {
    bool val;
    return this.GetBoolImpl(option, out val) ? val : defaultVal;
  }

  public int GetInt(Option option)
  {
    int val;
    if (this.GetIntImpl(option, out val))
      return val;
    object obj;
    return OptionDataTables.s_defaultsMap.TryGetValue(option, out obj) ? (int) obj : 0;
  }

  public int GetInt(Option option, int defaultVal)
  {
    int val;
    return this.GetIntImpl(option, out val) ? val : defaultVal;
  }

  public long GetLong(Option option)
  {
    long val;
    if (this.GetLongImpl(option, out val))
      return val;
    object obj;
    return OptionDataTables.s_defaultsMap.TryGetValue(option, out obj) ? (long) obj : 0L;
  }

  public float GetFloat(Option option)
  {
    float val;
    if (this.GetFloatImpl(option, out val))
      return val;
    object obj;
    return OptionDataTables.s_defaultsMap.TryGetValue(option, out obj) ? (float) obj : 0.0f;
  }

  public float GetFloat(Option option, float defaultVal)
  {
    float val;
    return this.GetFloatImpl(option, out val) ? val : defaultVal;
  }

  public ulong GetULong(Option option)
  {
    ulong val;
    if (this.GetULongImpl(option, out val))
      return val;
    object obj;
    return OptionDataTables.s_defaultsMap.TryGetValue(option, out obj) ? (ulong) obj : 0UL;
  }

  public ulong GetULong(Option option, ulong defaultVal)
  {
    ulong val;
    return this.GetULongImpl(option, out val) ? val : defaultVal;
  }

  public string GetString(Option option)
  {
    string val;
    if (this.GetStringImpl(option, out val))
      return val;
    object obj;
    return OptionDataTables.s_defaultsMap.TryGetValue(option, out obj) ? (string) obj : "";
  }

  public string GetString(Option option, string defaultVal)
  {
    string val;
    return this.GetStringImpl(option, out val) ? val : defaultVal;
  }

  public T GetEnum<T>(Option option)
  {
    T val;
    object genericVal;
    return this.GetEnumImpl<T>(option, out val) || OptionDataTables.s_defaultsMap.TryGetValue(option, out genericVal) && this.TranslateEnumVal<T>(option, genericVal, out val) ? val : default (T);
  }

  public T GetEnum<T>(Option option, T defaultVal)
  {
    T val;
    return this.GetEnumImpl<T>(option, out val) ? val : defaultVal;
  }

  public void SetOption(Option option, object val)
  {
    System.Type optionType = this.GetOptionType(option);
    if (optionType == typeof (bool))
      this.SetBool(option, (bool) val);
    else if (optionType == typeof (int))
      this.SetInt(option, (int) val);
    else if (optionType == typeof (long))
      this.SetLong(option, (long) val);
    else if (optionType == typeof (float))
      this.SetFloat(option, (float) val);
    else if (optionType == typeof (string))
      this.SetString(option, (string) val);
    else if (optionType == typeof (ulong))
      this.SetULong(option, (ulong) val);
    else
      Error.AddDevFatal("Options.SetOption() - option {0} has unsupported underlying type {1}", (object) option, (object) optionType);
  }

  public void SetBool(Option option, bool val)
  {
    string key;
    if (this.m_clientOptionMap.TryGetValue(option, out key))
    {
      bool existed = LocalOptions.Get().Has(key);
      bool prevVal = LocalOptions.Get().GetBool(key);
      if (existed && prevVal == val)
        return;
      LocalOptions.Get().Set(key, (object) val);
      this.FireChangedEvent(option, (object) prevVal, existed);
    }
    else
    {
      ServerOptionFlag flag;
      if (!this.m_serverOptionFlagMap.TryGetValue(option, out flag))
        return;
      ServerOption container;
      ulong flagBit;
      ulong existenceBit;
      this.GetServerOptionFlagInfo(flag, out container, out flagBit, out existenceBit);
      ulong ulongOption = NetCache.Get().GetULongOption(container);
      bool prevVal = (ulongOption & flagBit) > 0UL;
      bool existed = (ulongOption & existenceBit) > 0UL;
      if (existed && prevVal == val)
        return;
      ulong val1 = (val ? ulongOption | flagBit : ulongOption & ~flagBit) | existenceBit;
      NetCache.Get().SetULongOption(container, val1);
      this.FireChangedEvent(option, (object) prevVal, existed);
    }
  }

  public void SetInt(Option option, int val)
  {
    string key;
    if (this.m_clientOptionMap.TryGetValue(option, out key))
    {
      bool existed = LocalOptions.Get().Has(key);
      int prevVal = LocalOptions.Get().GetInt(key);
      if (existed && prevVal == val)
        return;
      LocalOptions.Get().Set(key, (object) val);
      this.FireChangedEvent(option, (object) prevVal, existed);
    }
    else
    {
      ServerOption type;
      if (!this.m_serverOptionMap.TryGetValue(option, out type))
        return;
      int ret;
      bool intOption = NetCache.Get().GetIntOption(type, out ret);
      if (intOption && ret == val)
        return;
      NetCache.Get().SetIntOption(type, val);
      this.FireChangedEvent(option, (object) ret, intOption);
    }
  }

  public void SetLong(Option option, long val)
  {
    string key;
    if (this.m_clientOptionMap.TryGetValue(option, out key))
    {
      bool existed = LocalOptions.Get().Has(key);
      long prevVal = LocalOptions.Get().GetLong(key);
      if (existed && prevVal == val)
        return;
      LocalOptions.Get().Set(key, (object) val);
      this.FireChangedEvent(option, (object) prevVal, existed);
    }
    else
    {
      ServerOption type;
      if (!this.m_serverOptionMap.TryGetValue(option, out type))
        return;
      long ret;
      bool longOption = NetCache.Get().GetLongOption(type, out ret);
      if (longOption && ret == val)
        return;
      NetCache.Get().SetLongOption(type, val);
      this.FireChangedEvent(option, (object) ret, longOption);
    }
  }

  public void SetFloat(Option option, float val)
  {
    string key;
    if (this.m_clientOptionMap.TryGetValue(option, out key))
    {
      bool existed = LocalOptions.Get().Has(key);
      float prevVal = LocalOptions.Get().GetFloat(key);
      if (existed && (double) prevVal == (double) val)
        return;
      LocalOptions.Get().Set(key, (object) val);
      this.FireChangedEvent(option, (object) prevVal, existed);
    }
    else
    {
      ServerOption type;
      if (!this.m_serverOptionMap.TryGetValue(option, out type))
        return;
      float ret;
      bool floatOption = NetCache.Get().GetFloatOption(type, out ret);
      if (floatOption && (double) ret == (double) val)
        return;
      NetCache.Get().SetFloatOption(type, val);
      this.FireChangedEvent(option, (object) ret, floatOption);
    }
  }

  public void SetULong(Option option, ulong val)
  {
    string key;
    if (this.m_clientOptionMap.TryGetValue(option, out key))
    {
      bool existed = LocalOptions.Get().Has(key);
      ulong prevVal = LocalOptions.Get().GetULong(key);
      if (existed && (long) prevVal == (long) val)
        return;
      LocalOptions.Get().Set(key, (object) val);
      this.FireChangedEvent(option, (object) prevVal, existed);
    }
    else
    {
      ServerOption type;
      if (!this.m_serverOptionMap.TryGetValue(option, out type))
        return;
      ulong ret;
      bool ulongOption = NetCache.Get().GetULongOption(type, out ret);
      if (ulongOption && (long) ret == (long) val)
        return;
      NetCache.Get().SetULongOption(type, val);
      this.FireChangedEvent(option, (object) ret, ulongOption);
    }
  }

  public void SetString(Option option, string val)
  {
    string key;
    if (!this.m_clientOptionMap.TryGetValue(option, out key))
      return;
    bool existed = LocalOptions.Get().Has(key);
    string prevVal = LocalOptions.Get().GetString(key);
    if (existed && !(prevVal != val))
      return;
    LocalOptions.Get().Set(key, (object) val);
    this.FireChangedEvent(option, (object) prevVal, existed);
  }

  public void SetEnum<T>(Option option, T val)
  {
    if (!Enum.IsDefined(typeof (T), (object) val))
    {
      Error.AddDevFatal("Options.SetEnum() - {0} is not convertible to enum type {1} for option {2}", (object) val, (object) typeof (T), (object) option);
    }
    else
    {
      System.Type optionType = this.GetOptionType(option);
      if (optionType == typeof (int))
        this.SetInt(option, Convert.ToInt32((object) val));
      else if (optionType == typeof (long))
        this.SetLong(option, Convert.ToInt64((object) val));
      else
        Error.AddDevFatal("Options.SetEnum() - option {0} has unsupported underlying type {1}", (object) option, (object) optionType);
    }
  }

  private void Initialize()
  {
    Array values = Enum.GetValues(typeof (Option));
    Map<string, Option> options = new Map<string, Option>();
    foreach (Option option in values)
    {
      if (option != Option.INVALID)
      {
        string key = option.ToString();
        options.Add(key, option);
      }
    }
    this.BuildClientOptionMap(options);
    this.BuildServerOptionMap(options);
    this.BuildServerOptionFlagMap(options);
  }

  private void BuildClientOptionMap(Map<string, Option> options)
  {
    this.m_clientOptionMap = new Map<Option, string>();
    foreach (ClientOption clientOption in Enum.GetValues(typeof (ClientOption)))
    {
      if (clientOption != ClientOption.INVALID)
      {
        string key = clientOption.ToString();
        Option option;
        if (!options.TryGetValue(key, out option))
          Debug.LogError((object) string.Format("Options.BuildClientOptionMap() - ClientOption {0} is not mirrored in the Option enum", (object) clientOption));
        else if (!OptionDataTables.s_typeMap.TryGetValue(option, out System.Type _))
        {
          Debug.LogError((object) string.Format("Options.BuildClientOptionMap() - ClientOption {0} has no type. Please add its type to the type map.", (object) clientOption));
        }
        else
        {
          string str = EnumUtils.GetString<Option>(option);
          this.m_clientOptionMap.Add(option, str);
        }
      }
    }
  }

  private void BuildServerOptionMap(Map<string, Option> options)
  {
    this.m_serverOptionMap = new Map<Option, ServerOption>();
    foreach (ServerOption serverOption in Enum.GetValues(typeof (ServerOption)))
    {
      switch (serverOption)
      {
        case ServerOption.INVALID:
        case ServerOption.LIMIT:
          continue;
        default:
          string key1 = serverOption.ToString();
          if (!key1.StartsWith("FLAGS") && !key1.StartsWith("DEPRECATED"))
          {
            Option key2;
            if (!options.TryGetValue(key1, out key2))
            {
              Debug.LogError((object) string.Format("Options.BuildServerOptionMap() - ServerOption {0} is not mirrored in the Option enum", (object) serverOption));
              continue;
            }
            System.Type type;
            if (!OptionDataTables.s_typeMap.TryGetValue(key2, out type))
            {
              Debug.LogError((object) string.Format("Options.BuildServerOptionMap() - ServerOption {0} has no type. Please add its type to the type map.", (object) serverOption));
              continue;
            }
            if (type == typeof (bool))
            {
              Debug.LogError((object) string.Format("Options.BuildServerOptionMap() - ServerOption {0} is a bool. You should convert it to a ServerOptionFlag.", (object) serverOption));
              continue;
            }
            this.m_serverOptionMap.Add(key2, serverOption);
            continue;
          }
          continue;
      }
    }
  }

  private void BuildServerOptionFlagMap(Map<string, Option> options)
  {
    this.m_serverOptionFlagMap = new Map<Option, ServerOptionFlag>();
    foreach (ServerOptionFlag serverOptionFlag in Enum.GetValues(typeof (ServerOptionFlag)))
    {
      if (serverOptionFlag != ServerOptionFlag.LIMIT)
      {
        string key1 = serverOptionFlag.ToString();
        if (!key1.StartsWith("DEPRECATED"))
        {
          Option key2;
          if (!options.TryGetValue(key1, out key2))
            Debug.LogError((object) string.Format("Options.BuildServerOptionFlagMap() - ServerOptionFlag {0} is not mirrored in the Option enum", (object) serverOptionFlag));
          else
            this.m_serverOptionFlagMap.Add(key2, serverOptionFlag);
        }
      }
    }
  }

  private void GetServerOptionFlagInfo(
    ServerOptionFlag flag,
    out ServerOption container,
    out ulong flagBit,
    out ulong existenceBit)
  {
    int num1;
    int index = Mathf.FloorToInt((float) (num1 = 2 * (int) flag) * (1f / 64f));
    int num2 = num1 % 64;
    int num3 = 1 + num2;
    container = Options.s_serverFlagContainers[index];
    flagBit = (ulong) (1L << num2);
    existenceBit = (ulong) (1L << num3);
  }

  private bool HasServerOptionFlag(ServerOptionFlag serverOptionFlag)
  {
    ServerOption container;
    ulong existenceBit;
    this.GetServerOptionFlagInfo(serverOptionFlag, out container, out ulong _, out existenceBit);
    return (NetCache.Get().GetULongOption(container) & existenceBit) > 0UL;
  }

  private void DeleteClientOption(Option option, string optionName)
  {
    if (!LocalOptions.Get().Has(optionName))
      return;
    object clientOption = this.GetClientOption(option, optionName);
    LocalOptions.Get().Delete(optionName);
    this.RemoveListeners(option, clientOption);
  }

  private void DeleteServerOption(Option option, ServerOption serverOption)
  {
    if (!NetCache.Get().ClientOptionExists(serverOption))
      return;
    object serverOption1 = this.GetServerOption(option, serverOption);
    NetCache.Get().DeleteClientOption(serverOption);
    this.RemoveListeners(option, serverOption1);
  }

  private void DeleteServerOptionFlag(Option option, ServerOptionFlag serverOptionFlag)
  {
    ServerOption container;
    ulong flagBit;
    ulong existenceBit;
    this.GetServerOptionFlagInfo(serverOptionFlag, out container, out flagBit, out existenceBit);
    ulong ulongOption = NetCache.Get().GetULongOption(container);
    if ((ulongOption & existenceBit) <= 0UL)
      return;
    bool prevVal = (ulongOption & flagBit) > 0UL;
    ulong val = ulongOption & ~existenceBit;
    NetCache.Get().SetULongOption(container, val);
    this.RemoveListeners(option, (object) prevVal);
  }

  private object GetClientOption(Option option, string optionName)
  {
    System.Type optionType = this.GetOptionType(option);
    if (optionType == typeof (bool))
      return (object) LocalOptions.Get().GetBool(optionName);
    if (optionType == typeof (int))
      return (object) LocalOptions.Get().GetInt(optionName);
    if (optionType == typeof (long))
      return (object) LocalOptions.Get().GetLong(optionName);
    if (optionType == typeof (ulong))
      return (object) LocalOptions.Get().GetULong(optionName);
    if (optionType == typeof (float))
      return (object) LocalOptions.Get().GetFloat(optionName);
    if (optionType == typeof (string))
      return (object) LocalOptions.Get().GetString(optionName);
    Error.AddDevFatal("Options.GetClientOption() - option {0} has unsupported underlying type {1}", (object) option, (object) optionType);
    return (object) null;
  }

  private object GetServerOption(Option option, ServerOption serverOption)
  {
    System.Type optionType = this.GetOptionType(option);
    if (optionType == typeof (int))
      return (object) NetCache.Get().GetIntOption(serverOption);
    if (optionType == typeof (long))
      return (object) NetCache.Get().GetLongOption(serverOption);
    if (optionType == typeof (float))
      return (object) NetCache.Get().GetFloatOption(serverOption);
    if (optionType == typeof (ulong))
      return (object) NetCache.Get().GetULongOption(serverOption);
    Error.AddDevFatal("Options.GetServerOption() - option {0} has unsupported underlying type {1}", (object) option, (object) optionType);
    return (object) null;
  }

  private bool GetOptionImpl(Option option, out object val)
  {
    val = (object) null;
    string str;
    if (this.m_clientOptionMap.TryGetValue(option, out str))
    {
      if (LocalOptions.Get().Has(str))
        val = this.GetClientOption(option, str);
    }
    else
    {
      ServerOption container;
      if (this.m_serverOptionMap.TryGetValue(option, out container))
      {
        if (NetCache.Get().ClientOptionExists(container))
          val = this.GetServerOption(option, container);
      }
      else
      {
        ServerOptionFlag flag;
        if (this.m_serverOptionFlagMap.TryGetValue(option, out flag))
        {
          ulong flagBit;
          ulong existenceBit;
          this.GetServerOptionFlagInfo(flag, out container, out flagBit, out existenceBit);
          ulong ulongOption = NetCache.Get().GetULongOption(container);
          if ((ulongOption & existenceBit) > 0UL)
            val = (object) ((ulongOption & flagBit) > 0UL);
        }
      }
    }
    return val != null;
  }

  private bool GetBoolImpl(Option option, out bool val)
  {
    val = false;
    object val1;
    if (!this.GetOptionImpl(option, out val1))
      return false;
    val = (bool) val1;
    return true;
  }

  private bool GetIntImpl(Option option, out int val)
  {
    val = 0;
    object val1;
    if (!this.GetOptionImpl(option, out val1))
      return false;
    val = (int) val1;
    return true;
  }

  private bool GetLongImpl(Option option, out long val)
  {
    val = 0L;
    object val1;
    if (!this.GetOptionImpl(option, out val1))
      return false;
    val = (long) val1;
    return true;
  }

  private bool GetFloatImpl(Option option, out float val)
  {
    val = 0.0f;
    object val1;
    if (!this.GetOptionImpl(option, out val1))
      return false;
    val = (float) val1;
    return true;
  }

  private bool GetULongImpl(Option option, out ulong val)
  {
    val = 0UL;
    object val1;
    if (!this.GetOptionImpl(option, out val1))
      return false;
    val = (ulong) val1;
    return true;
  }

  private bool GetStringImpl(Option option, out string val)
  {
    val = "";
    object val1;
    if (!this.GetOptionImpl(option, out val1))
      return false;
    val = (string) val1;
    return true;
  }

  private bool GetEnumImpl<T>(Option option, out T val)
  {
    val = default (T);
    object val1;
    return this.GetOptionImpl(option, out val1) && this.TranslateEnumVal<T>(option, val1, out val);
  }

  private bool TranslateEnumVal<T>(Option option, object genericVal, out T val)
  {
    val = default (T);
    if (genericVal == null)
      return true;
    System.Type type = genericVal.GetType();
    System.Type enumType = typeof (T);
    try
    {
      if (type == enumType)
      {
        val = (T) genericVal;
        return true;
      }
      object obj = Convert.ChangeType(genericVal, Enum.GetUnderlyingType(enumType));
      val = (T) obj;
      return true;
    }
    catch (Exception ex)
    {
      Debug.LogErrorFormat("Options.TranslateEnumVal() - option {0} has value {1} ({2}), which cannot be converted to type {3}: {4}", (object) option, genericVal, (object) type, (object) enumType, (object) ex.ToString());
      return false;
    }
  }

  private void RemoveListeners(Option option, object prevVal)
  {
    this.FireChangedEvent(option, prevVal, true);
    this.m_changedListeners.Remove(option);
  }

  private void FireChangedEvent(Option option, object prevVal, bool existed)
  {
    List<Options.ChangedListener> changedListenerList;
    if (this.m_changedListeners.TryGetValue(option, out changedListenerList))
    {
      foreach (Options.ChangedListener changedListener in changedListenerList.ToArray())
        changedListener.Fire(option, prevVal, existed);
    }
    foreach (Options.ChangedListener changedListener in this.m_globalChangedListeners.ToArray())
      changedListener.Fire(option, prevVal, existed);
  }

  public delegate void ChangedCallback(
    Option option,
    object prevValue,
    bool existed,
    object userData);

  private class ChangedListener : EventListener<Options.ChangedCallback>
  {
    public void Fire(Option option, object prevValue, bool didExist) => this.m_callback(option, prevValue, didExist, this.m_userData);
  }
}
