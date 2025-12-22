using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
[CreateAssetMenu(fileName = "BaconLHSConfig", menuName = "ScriptableObjects/BaconLHSConfig", order = 2)]
public class BaconLHSConfig : ScriptableObject
{
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public string m_VOPicked;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public string m_VOStartOfGame;
  [CustomEditField(Sections = "VoiceOver")]
  public List<BaconLHSConfig.CardSpecificLine> m_VOBartenderGreet;
  [CustomEditField(Sections = "VoiceOver")]
  public List<BaconLHSConfig.ValueLine> m_VOWinStreak;
  [CustomEditField(Sections = "VoiceOver")]
  public List<BaconLHSConfig.CardSpecificLine> m_VOHeroGreet;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOGreet;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOKnockout;
  [CustomEditField(Sections = "VFX", T = EditType.SPELL)]
  public string m_VFXSocketInDef;
  [CustomEditField(Sections = "VFX", T = EditType.SPELL)]
  public string m_VFXCombatStartDef;
  public List<BaconLHSConfig.ValueVFXDef> m_VFXWinStreakDef;
  private Card m_heroCard;
  private Spell m_VFXSocketIn;
  private Spell m_VFXCombatStart;
  private List<BaconLHSConfig.ValueVFX> m_VFXWinStreak;
  private Dictionary<string, List<string>> m_VOBartenderGreetDict;
  private Dictionary<string, List<string>> m_VOHeroGreetDict;

  public void InitAllAssets(Card heroCard)
  {
    this.m_heroCard = heroCard;
    this.PreLoadAllVFX();
    this.PreLoadAllVO();
    this.ConfigureAllVO();
  }

  public void InitCombatAssets(Card heroCard)
  {
    this.m_heroCard = heroCard;
    this.PreLoadCombatVFX();
    this.PreLoadCombatVO();
    this.ConfigureCombatVO();
  }

  private void ConfigureAllVO()
  {
    this.ConfigureCombatVO();
    if (this.m_VOBartenderGreet == null || this.m_VOBartenderGreet.Count == 0)
      return;
    this.m_VOBartenderGreetDict = new Dictionary<string, List<string>>();
    foreach (BaconLHSConfig.CardSpecificLine cardSpecificLine in this.m_VOBartenderGreet)
    {
      if (!string.IsNullOrEmpty(cardSpecificLine.m_cardId) && !string.IsNullOrEmpty(cardSpecificLine.m_VOLine))
      {
        if (!this.m_VOBartenderGreetDict.ContainsKey(cardSpecificLine.m_cardId))
          this.m_VOBartenderGreetDict.Add(cardSpecificLine.m_cardId, new List<string>());
        this.m_VOBartenderGreetDict[cardSpecificLine.m_cardId]?.Add(cardSpecificLine.m_VOLine);
      }
    }
  }

  private void ConfigureCombatVO()
  {
    if (this.m_VOHeroGreet == null || this.m_VOHeroGreet.Count == 0)
      return;
    this.m_VOHeroGreetDict = new Dictionary<string, List<string>>();
    foreach (BaconLHSConfig.CardSpecificLine cardSpecificLine in this.m_VOHeroGreet)
    {
      if (!string.IsNullOrEmpty(cardSpecificLine.m_cardId) && !string.IsNullOrEmpty(cardSpecificLine.m_VOLine))
      {
        if (!this.m_VOHeroGreetDict.ContainsKey(cardSpecificLine.m_cardId))
          this.m_VOHeroGreetDict.Add(cardSpecificLine.m_cardId, new List<string>());
        this.m_VOHeroGreetDict[cardSpecificLine.m_cardId]?.Add(cardSpecificLine.m_VOLine);
      }
    }
  }

  private void PreLoadAllVO()
  {
    if (GameState.Get() == null || GameState.Get().GetGameEntity() == null)
      return;
    foreach (string allVoLine in this.GetAllVOLines())
      GameState.Get().GetGameEntity().PreloadSound(allVoLine);
  }

  private void PreLoadCombatVO()
  {
    if (GameState.Get() == null || GameState.Get().GetGameEntity() == null)
      return;
    foreach (string combatVoLine in this.GetCombatVOLines())
      GameState.Get().GetGameEntity().PreloadSound(combatVoLine);
  }

  public List<string> GetAllVOLines()
  {
    List<string> ret = this.GetCombatVOLines();
    Action<string> action = (Action<string>) (x =>
    {
      if (string.IsNullOrEmpty(x))
        return;
      ret.Add(x);
    });
    foreach (BaconLHSConfig.ValueLine valueLine in this.m_VOWinStreak)
      action(valueLine.m_VOLine);
    foreach (BaconLHSConfig.CardSpecificLine cardSpecificLine in this.m_VOBartenderGreet)
      action(cardSpecificLine.m_VOLine);
    action(this.m_VOStartOfGame);
    return ret;
  }

  private List<string> GetCombatVOLines()
  {
    List<string> ret = new List<string>();
    Action<string> action1 = (Action<string>) (x =>
    {
      if (string.IsNullOrEmpty(x))
        return;
      ret.Add(x);
    });
    Action<List<string>> action2 = (Action<List<string>>) (x =>
    {
      if (x == null)
        return;
      ret.AddRange((IEnumerable<string>) x);
    });
    foreach (BaconLHSConfig.CardSpecificLine cardSpecificLine in this.m_VOHeroGreet)
      action1(cardSpecificLine.m_VOLine);
    action2(this.m_VOGreet);
    action2(this.m_VOKnockout);
    return ret;
  }

  private void PreLoadAllVFX()
  {
    this.PreLoadCombatVFX();
    if (!string.IsNullOrEmpty(this.m_VFXSocketInDef))
      AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_VFXSocketInDef, new PrefabCallback<GameObject>(this.OnSpellLoaded_SocketIn));
    this.m_VFXWinStreak = new List<BaconLHSConfig.ValueVFX>();
    foreach (BaconLHSConfig.ValueVFXDef callbackData in this.m_VFXWinStreakDef)
    {
      if (!string.IsNullOrEmpty(callbackData.m_vfxAsset))
        AssetLoader.Get().InstantiatePrefab((AssetReference) callbackData.m_vfxAsset, new PrefabCallback<GameObject>(this.OnSpellLoaded_WinStreak), (object) callbackData);
    }
  }

  private void PreLoadCombatVFX()
  {
    if (string.IsNullOrEmpty(this.m_VFXCombatStartDef))
      return;
    AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_VFXCombatStartDef, new PrefabCallback<GameObject>(this.OnSpellLoaded_CombatStart));
  }

  private void OnSpellLoaded_SocketIn(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      return;
    this.m_VFXSocketIn = go.GetComponent<Spell>();
    SpellUtils.SetupSpell(this.m_VFXSocketIn, (Component) this.m_heroCard);
    this.m_VFXSocketIn.transform.parent = this.m_heroCard.transform;
  }

  private void OnSpellLoaded_CombatStart(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      return;
    this.m_VFXCombatStart = go.GetComponent<Spell>();
    SpellUtils.SetupSpell(this.m_VFXCombatStart, (Component) this.m_heroCard);
  }

  private void OnSpellLoaded_WinStreak(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null || !(callbackData is BaconLHSConfig.ValueVFXDef def))
      return;
    BaconLHSConfig.ValueVFX valueVfx = new BaconLHSConfig.ValueVFX(def);
    valueVfx.m_vfxSpell = go.GetComponent<Spell>();
    SpellUtils.SetupSpell(valueVfx.m_vfxSpell, (Component) this.m_heroCard);
    this.m_VFXWinStreak.Add(valueVfx);
  }

  public bool TryGetAllBartenderGreet(string bartenderId, out List<string> voLines)
  {
    if (this.m_VOBartenderGreetDict.ContainsKey(bartenderId))
    {
      voLines = this.m_VOBartenderGreetDict[bartenderId];
      return true;
    }
    voLines = (List<string>) null;
    return false;
  }

  public bool TryGetAllHeroGreet(string heroId, out List<string> voLines)
  {
    if (this.m_VOHeroGreetDict.ContainsKey(heroId))
    {
      voLines = this.m_VOHeroGreetDict[heroId];
      return true;
    }
    voLines = (List<string>) null;
    return false;
  }

  public bool CheckBartenderGreetLine(string bartenderId, out string voLine)
  {
    if (this.m_VOBartenderGreetDict != null && this.m_VOBartenderGreetDict.ContainsKey(bartenderId) && this.m_VOBartenderGreetDict[bartenderId] != null && this.m_VOBartenderGreetDict[bartenderId].Count > 0)
    {
      voLine = this.TryGetRandomLine(this.m_VOBartenderGreetDict[bartenderId]);
      return !string.IsNullOrEmpty(voLine);
    }
    voLine = (string) null;
    return false;
  }

  public bool CheckStartGameLine(out string line)
  {
    line = this.m_VOStartOfGame;
    return !string.IsNullOrEmpty(line);
  }

  public bool CheckWinStreakLine(int streak, out string line)
  {
    line = (string) null;
    if (this.m_VOWinStreak == null || this.m_VOWinStreak.Count == 0)
      return false;
    int num = -1;
    foreach (BaconLHSConfig.ValueLine valueLine in this.m_VOWinStreak)
    {
      if (valueLine.m_onlyExactMatch)
      {
        if (valueLine.m_value == streak)
        {
          line = valueLine.m_VOLine;
          break;
        }
      }
      else if (valueLine.m_value > num && valueLine.m_value <= streak)
      {
        num = valueLine.m_value;
        line = valueLine.m_VOLine;
      }
    }
    return !string.IsNullOrEmpty(line);
  }

  public bool CheckGreetLine(string heroCardId, out string line)
  {
    if (heroCardId == null)
    {
      line = "";
      return false;
    }
    if (this.m_VOHeroGreetDict != null && this.m_VOHeroGreetDict.ContainsKey(heroCardId) && this.m_VOHeroGreetDict[heroCardId] != null && this.m_VOHeroGreetDict[heroCardId].Count > 0)
    {
      line = this.TryGetRandomLine(this.m_VOHeroGreetDict[heroCardId]);
      return !string.IsNullOrEmpty(line);
    }
    line = this.TryGetRandomLine(this.m_VOGreet);
    return !string.IsNullOrEmpty(line);
  }

  public string GetPickedLine() => this.m_VOPicked;

  public bool CheckKnockoutLine(out string line)
  {
    line = this.TryGetRandomLine(this.m_VOKnockout);
    return !string.IsNullOrEmpty(line);
  }

  public bool TryActivateVFX_SocketIn()
  {
    if ((UnityEngine.Object) this.m_VFXSocketIn == (UnityEngine.Object) null || (UnityEngine.Object) this.m_heroCard == (UnityEngine.Object) null)
      return false;
    this.m_VFXSocketIn.Activate();
    return true;
  }

  public bool TryActivateVFX_CombatStart()
  {
    if ((UnityEngine.Object) this.m_VFXCombatStart == (UnityEngine.Object) null || (UnityEngine.Object) this.m_heroCard == (UnityEngine.Object) null)
      return false;
    this.m_VFXCombatStart.Activate();
    return true;
  }

  public bool TryActivateVFX_WinStreak(int currentStreak)
  {
    Spell spell = (Spell) null;
    if (this.m_VFXWinStreak == null || this.m_VFXWinStreak.Count == 0)
      return false;
    int num1 = -1;
    foreach (BaconLHSConfig.ValueVFX valueVfx in this.m_VFXWinStreak)
    {
      if (valueVfx.m_onlyExactMatch)
      {
        if (valueVfx.m_value == currentStreak)
        {
          int num2 = valueVfx.m_value;
          spell = valueVfx.m_vfxSpell;
          break;
        }
      }
      else if (valueVfx.m_value > num1 && valueVfx.m_value <= currentStreak)
      {
        num1 = valueVfx.m_value;
        spell = valueVfx.m_vfxSpell;
      }
    }
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_heroCard != (UnityEngine.Object) null))
      return false;
    spell.Activate();
    return true;
  }

  protected string TryGetRandomLine(List<string> lines)
  {
    if (lines == null)
      return (string) null;
    return lines.Count == 0 ? (string) null : lines[UnityEngine.Random.Range(0, lines.Count)];
  }

  [Serializable]
  public class ValueVFXDef
  {
    public int m_value;
    public bool m_onlyExactMatch = true;
    [CustomEditField(T = EditType.SPELL)]
    public string m_vfxAsset;
  }

  public class ValueVFX
  {
    public int m_value;
    public bool m_onlyExactMatch = true;
    public Spell m_vfxSpell;

    public ValueVFX(BaconLHSConfig.ValueVFXDef def)
    {
      this.m_value = def.m_value;
      this.m_onlyExactMatch = def.m_onlyExactMatch;
    }
  }

  [Serializable]
  public class ValueLine
  {
    public int m_value;
    public bool m_onlyExactMatch = true;
    [CustomEditField(T = EditType.SOUND_PREFAB)]
    public string m_VOLine;
  }

  [Serializable]
  public class CardSpecificLine
  {
    public string m_cardId;
    [CustomEditField(T = EditType.SOUND_PREFAB)]
    public string m_VOLine;
  }
}
