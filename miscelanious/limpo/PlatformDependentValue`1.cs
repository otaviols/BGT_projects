using System;
using UnityEngine;

[Serializable]
public class PlatformDependentValue<T>
{
  private bool resolved;
  private T result;
  [SerializeField]
  private PlatformCategory type;
  private T defaultValue;
  [SerializeField]
  private T[] settings = new T[14];
  [SerializeField]
  private bool[] isSet = new bool[14];

  public T PC
  {
    set => this.SetValue(PlatformSettingType.PC, value);
  }

  public T Mac
  {
    set => this.SetValue(PlatformSettingType.Mac, value);
  }

  public T iOS
  {
    set => this.SetValue(PlatformSettingType.iOS, value);
  }

  public T Android
  {
    set => this.SetValue(PlatformSettingType.Android, value);
  }

  public T Tablet
  {
    set => this.SetValue(PlatformSettingType.Tablet, value);
  }

  public T MiniTablet
  {
    set => this.SetValue(PlatformSettingType.MiniTablet, value);
  }

  public T Phone
  {
    set => this.SetValue(PlatformSettingType.Phone, value);
  }

  public T Mouse
  {
    set => this.SetValue(PlatformSettingType.Mouse, value);
  }

  public T Touch
  {
    set => this.SetValue(PlatformSettingType.Touch, value);
  }

  public T LowMemory
  {
    set => this.SetValue(PlatformSettingType.LowMemory, value);
  }

  public T MediumMemory
  {
    set => this.SetValue(PlatformSettingType.MediumMemory, value);
  }

  public T HighMemory
  {
    set => this.SetValue(PlatformSettingType.HighMemory, value);
  }

  public PlatformDependentValue(PlatformCategory t)
  {
    this.type = t;
    this.InitSettingsMap();
  }

  public static implicit operator T(PlatformDependentValue<T> val) => val.Value;

  private void InitSettingsMap()
  {
    for (int index = 0; index < 14; ++index)
    {
      this.settings[index] = default (T);
      this.isSet[index] = false;
    }
  }

  public T Value
  {
    get
    {
      if (this.resolved)
        return this.result;
      switch (this.type)
      {
        case PlatformCategory.OS:
          this.result = this.GetOSSetting(PlatformSettings.OS);
          break;
        case PlatformCategory.Screen:
          this.result = this.GetScreenSetting(PlatformSettings.Screen);
          break;
        case PlatformCategory.Memory:
          this.result = this.GetMemorySetting(PlatformSettings.Memory);
          break;
        case PlatformCategory.Input:
          this.result = this.GetInputSetting(PlatformSettings.Input);
          break;
      }
      this.resolved = true;
      return this.result;
    }
  }

  private void SetValue(PlatformSettingType type, T value)
  {
    this.settings[(int) type] = value;
    this.isSet[(int) type] = true;
  }

  public T GetValue(PlatformSettingType type) => this.settings[(int) type];

  public bool IsSet(PlatformSettingType type) => this.isSet[(int) type];

  private T GetOSSetting(OSCategory os)
  {
    switch (os)
    {
      case OSCategory.PC:
        if (this.IsSet(PlatformSettingType.PC))
          return this.GetValue(PlatformSettingType.PC);
        break;
      case OSCategory.Mac:
        return !this.IsSet(PlatformSettingType.Mac) ? this.GetOSSetting(OSCategory.PC) : this.GetValue(PlatformSettingType.Mac);
      case OSCategory.iOS:
        return !this.IsSet(PlatformSettingType.iOS) ? this.GetOSSetting(OSCategory.PC) : this.GetValue(PlatformSettingType.iOS);
      case OSCategory.Android:
        return !this.IsSet(PlatformSettingType.Android) ? this.GetOSSetting(OSCategory.PC) : this.GetValue(PlatformSettingType.Android);
    }
    Debug.LogError((object) "Could not find OS dependent value");
    return default (T);
  }

  private T GetScreenSetting(ScreenCategory screen)
  {
    switch (screen)
    {
      case ScreenCategory.Phone:
        return !this.IsSet(PlatformSettingType.Phone) ? this.GetScreenSetting(ScreenCategory.Tablet) : this.GetValue(PlatformSettingType.Phone);
      case ScreenCategory.MiniTablet:
        return !this.IsSet(PlatformSettingType.MiniTablet) ? this.GetScreenSetting(ScreenCategory.Tablet) : this.GetValue(PlatformSettingType.MiniTablet);
      case ScreenCategory.Tablet:
        return !this.IsSet(PlatformSettingType.Tablet) ? this.GetScreenSetting(ScreenCategory.PC) : this.GetValue(PlatformSettingType.Tablet);
      case ScreenCategory.PC:
        if (this.IsSet(PlatformSettingType.PC))
          return this.GetValue(PlatformSettingType.PC);
        break;
    }
    Debug.LogError((object) "Could not find screen dependent value");
    return default (T);
  }

  private T GetMemorySetting(MemoryCategory memory)
  {
    switch (memory)
    {
      case MemoryCategory.Low:
        if (this.IsSet(PlatformSettingType.LowMemory))
          return this.GetValue(PlatformSettingType.LowMemory);
        break;
      case MemoryCategory.Medium:
        return !this.IsSet(PlatformSettingType.MediumMemory) ? this.GetMemorySetting(MemoryCategory.Low) : this.GetValue(PlatformSettingType.MediumMemory);
      case MemoryCategory.High:
        return !this.IsSet(PlatformSettingType.HighMemory) ? this.GetMemorySetting(MemoryCategory.Medium) : this.GetValue(PlatformSettingType.HighMemory);
    }
    Debug.LogError((object) "Could not find memory dependent value");
    return default (T);
  }

  private T GetInputSetting(InputCategory input)
  {
    switch (input)
    {
      case InputCategory.Mouse:
        if (this.IsSet(PlatformSettingType.Mouse))
          return this.GetValue(PlatformSettingType.Mouse);
        break;
      case InputCategory.Touch:
        return !this.IsSet(PlatformSettingType.Touch) ? this.GetInputSetting(InputCategory.Mouse) : this.GetValue(PlatformSettingType.Touch);
    }
    Debug.LogError((object) "Could not find input dependent value");
    return default (T);
  }
}
