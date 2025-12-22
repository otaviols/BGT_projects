using System;
using UnityEngine;

[Serializable]
public class MobileOverrideValue<T>
{
  public ScreenCategory[] screens;
  public T[] values;

  public MobileOverrideValue()
  {
    this.screens = new ScreenCategory[1];
    this.screens[0] = ScreenCategory.PC;
    this.values = new T[1];
    this.values[0] = default (T);
  }

  public MobileOverrideValue(T defaultValue)
  {
    this.screens = new ScreenCategory[1]
    {
      ScreenCategory.PC
    };
    this.values = new T[1]{ defaultValue };
  }

  public static implicit operator T(MobileOverrideValue<T> val)
  {
    if (val == null)
      return default (T);
    ScreenCategory[] screens = val.screens;
    T[] values = val.values;
    if (screens.Length < 1)
    {
      Debug.LogError((object) "MobileOverrideValue should always have at least one value!");
      return default (T);
    }
    T obj = values[0];
    ScreenCategory screen = PlatformSettings.Screen;
    for (int index = 1; index < screens.Length; ++index)
    {
      if (screen == screens[index])
        obj = values[index];
    }
    return obj;
  }

  public T[] GetValues() => this.values;

  public T GetValueForScreen(ScreenCategory screen, object defaultValue)
  {
    int index = Array.IndexOf<ScreenCategory>(this.screens, screen);
    return index != -1 ? this.values[index] : (T) defaultValue;
  }
}
