using Blizzard.T5.Configuration;
using System;

[Serializable]
public class DevicePreset : ICloneable
{
  public static readonly DevicePresetList s_devicePresets;
  public string name = "No Emulation";
  public OSCategory os = OSCategory.PC;
  public InputCategory input;
  public ScreenCategory screen = ScreenCategory.PC;
  public ScreenDensityCategory screenDensity = ScreenDensityCategory.High;

  public object Clone() => this.MemberwiseClone();

  public void ReadFromConfig(ConfigFile config)
  {
    this.name = config.Get("Emulation.DeviceName", this.name.ToString());
    DevicePreset devicePreset = DevicePreset.s_devicePresets.Find((Predicate<DevicePreset>) (x => x.name.Equals(this.name)));
    this.os = devicePreset.os;
    this.input = devicePreset.input;
    this.screen = devicePreset.screen;
    this.screenDensity = devicePreset.screenDensity;
  }

  static DevicePreset()
  {
    DevicePresetList devicePresetList = new DevicePresetList();
    devicePresetList.Add(new DevicePreset()
    {
      name = "No Emulation"
    });
    devicePresetList.Add(new DevicePreset()
    {
      name = "PC",
      os = OSCategory.PC,
      screen = ScreenCategory.PC,
      input = InputCategory.Mouse
    });
    devicePresetList.Add(new DevicePreset()
    {
      name = "iPhone",
      os = OSCategory.iOS,
      screen = ScreenCategory.Phone,
      input = InputCategory.Touch
    });
    devicePresetList.Add(new DevicePreset()
    {
      name = "iPad",
      os = OSCategory.iOS,
      screen = ScreenCategory.Tablet,
      input = InputCategory.Touch
    });
    devicePresetList.Add(new DevicePreset()
    {
      name = "Android Phone",
      os = OSCategory.Android,
      screen = ScreenCategory.Phone,
      input = InputCategory.Touch
    });
    devicePresetList.Add(new DevicePreset()
    {
      name = "Android Tablet",
      os = OSCategory.Android,
      screen = ScreenCategory.Tablet,
      input = InputCategory.Touch,
      screenDensity = ScreenDensityCategory.Normal
    });
    DevicePreset.s_devicePresets = devicePresetList;
  }
}
