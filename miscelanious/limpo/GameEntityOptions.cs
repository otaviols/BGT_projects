using Blizzard.T5.Core;
using System.Collections.Generic;

public class GameEntityOptions
{
  private Map<GameEntityOption, bool> m_booleanOptions = new Map<GameEntityOption, bool>();
  private Map<GameEntityOption, string> m_stringOptions = new Map<GameEntityOption, string>();

  public GameEntityOptions()
  {
  }

  public GameEntityOptions(
    Map<GameEntityOption, bool> booleanOptions,
    Map<GameEntityOption, string> stringOptions)
  {
    this.AddBooleanOptions(booleanOptions);
    this.AddStringOptions(stringOptions);
  }

  public GameEntityOptions(
    GameEntityOptions source,
    Map<GameEntityOption, bool> booleanOptions,
    Map<GameEntityOption, string> stringOptions)
  {
    this.AddBooleanOptions(source.m_booleanOptions);
    this.AddStringOptions(source.m_stringOptions);
    this.AddBooleanOptions(booleanOptions);
    this.AddStringOptions(stringOptions);
  }

  public void AddOptions(
    Map<GameEntityOption, bool> booleanOptions,
    Map<GameEntityOption, string> stringOptions)
  {
    this.AddBooleanOptions(booleanOptions);
    this.AddStringOptions(stringOptions);
  }

  public void AddBooleanOptions(Map<GameEntityOption, bool> options)
  {
    foreach (KeyValuePair<GameEntityOption, bool> option in options)
      this.SetBooleanOption(option.Key, option.Value);
  }

  public void AddStringOptions(Map<GameEntityOption, string> options)
  {
    foreach (KeyValuePair<GameEntityOption, string> option in options)
      this.SetStringOption(option.Key, option.Value);
  }

  public void SetBooleanOption(GameEntityOption option, bool value)
  {
    if (!this.m_booleanOptions.ContainsKey(option))
      this.m_booleanOptions.Add(option, value);
    else
      this.m_booleanOptions[option] = value;
  }

  public void SetStringOption(GameEntityOption option, string value)
  {
    if (!this.m_stringOptions.ContainsKey(option))
      this.m_stringOptions.Add(option, value);
    else
      this.m_stringOptions[option] = value;
  }

  public bool GetBooleanOption(GameEntityOption option) => this.m_booleanOptions != null && this.m_booleanOptions.ContainsKey(option) && this.m_booleanOptions[option];

  public string GetStringOption(GameEntityOption option) => this.m_stringOptions != null && this.m_stringOptions.ContainsKey(option) ? this.m_stringOptions[option] : (string) null;
}
