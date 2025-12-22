using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class BaconGuideConfig : MonoBehaviour
{
  [CustomEditField(Sections = "Guide ID")]
  public string m_GuideCardId;
  [CustomEditField(Sections = "VoiceOver")]
  public List<BaconGuideConfig.VOHeroSpecificLine> m_VOHeroSpecificLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public string m_VOAFK;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public string m_VOHighestTier;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOFreezingLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOSellingLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOShopUpgradeLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOTripleLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOPostShopWinLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOPostShopLoseLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOPostCombatLoseLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOPostCombatWinLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOPostShopIsFirstLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOSpecialIdleLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOPostCombatGeneralLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VORefreshLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOIdleLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VONewGameLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VORecruitLargeLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VORecruitMediumLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VORecruitSmallLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOPostShopGeneralLines;
  [CustomEditField(Sections = "VoiceOver", T = EditType.SOUND_PREFAB)]
  public List<string> m_VOPossibleTripleLines;

  public List<string> GetAllVOLines()
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
    foreach (BaconGuideConfig.VOHeroSpecificLine heroSpecificLine in this.m_VOHeroSpecificLines)
      action1(heroSpecificLine.m_VOLine);
    action1(this.m_VOAFK);
    action1(this.m_VOHighestTier);
    action2(this.m_VOFreezingLines);
    action2(this.m_VOSellingLines);
    action2(this.m_VOShopUpgradeLines);
    action2(this.m_VOTripleLines);
    action2(this.m_VOPostShopWinLines);
    action2(this.m_VOPostShopLoseLines);
    action2(this.m_VOPostCombatLoseLines);
    action2(this.m_VOPostCombatWinLines);
    action2(this.m_VOPostShopIsFirstLines);
    action2(this.m_VOSpecialIdleLines);
    action2(this.m_VOPostCombatGeneralLines);
    action2(this.m_VORefreshLines);
    action2(this.m_VOIdleLines);
    action2(this.m_VONewGameLines);
    action2(this.m_VORecruitLargeLines);
    action2(this.m_VORecruitMediumLines);
    action2(this.m_VORecruitSmallLines);
    action2(this.m_VOPostShopGeneralLines);
    action2(this.m_VOPossibleTripleLines);
    return ret;
  }

  public string PopRandomSpecialIdleLine()
  {
    if (this.m_VOSpecialIdleLines.Count == 0 || this.m_VOSpecialIdleLines == null)
      return (string) null;
    string randomLine = this.TryGetRandomLine(this.m_VOSpecialIdleLines);
    if (this.m_VOSpecialIdleLines != null && randomLine != null)
      this.m_VOSpecialIdleLines.Remove(randomLine);
    return randomLine;
  }

  public bool CheckHeroSpecificLine(string heroCardId, out string voHeroSpecificLine)
  {
    foreach (BaconGuideConfig.VOHeroSpecificLine heroSpecificLine in this.m_VOHeroSpecificLines)
    {
      if (heroSpecificLine.m_HeroCardId == heroCardId)
      {
        voHeroSpecificLine = heroSpecificLine.m_VOLine;
        return true;
      }
    }
    voHeroSpecificLine = (string) null;
    return false;
  }

  public string GetHighestTierLine() => this.m_VOHighestTier;

  public string GetAFKLine() => this.m_VOAFK;

  public string GetRandomFreezingLine() => this.TryGetRandomLine(this.m_VOFreezingLines);

  public string GetRandomSellingLine() => this.TryGetRandomLine(this.m_VOSellingLines);

  public string GetRandomShopUpgradeLine() => this.TryGetRandomLine(this.m_VOShopUpgradeLines);

  public string GetRandomTripleLine() => this.TryGetRandomLine(this.m_VOTripleLines);

  public string GetRandomPostShopWinLine() => this.TryGetRandomLine(this.m_VOPostShopWinLines);

  public string GetRandomPostShopLoseLine() => this.TryGetRandomLine(this.m_VOPostShopLoseLines);

  public string GetRandomPostCombatLoseLine() => this.TryGetRandomLine(this.m_VOPostCombatLoseLines);

  public string GetRandomPostCombatWinLine() => this.TryGetRandomLine(this.m_VOPostCombatWinLines);

  public string GetRandomPostShopIsFirstLine() => this.TryGetRandomLine(this.m_VOPostShopIsFirstLines);

  public string GetRandomSpecialIdleLine() => this.TryGetRandomLine(this.m_VOSpecialIdleLines);

  public string GetRandomPostCombatGeneralLine() => this.TryGetRandomLine(this.m_VOPostCombatGeneralLines);

  public string GetRandomRefreshLine() => this.TryGetRandomLine(this.m_VORefreshLines);

  public string GetRandomIdleLine() => this.TryGetRandomLine(this.m_VOIdleLines);

  public string GetRandomNewGameLine() => this.TryGetRandomLine(this.m_VONewGameLines);

  public string GetRandomRecruitLargeLine() => this.TryGetRandomLine(this.m_VORecruitLargeLines);

  public string GetRandomRecruitMediumLine() => this.TryGetRandomLine(this.m_VORecruitMediumLines);

  public string GetRandomRecruitSmallLine() => this.TryGetRandomLine(this.m_VORecruitSmallLines);

  public string GetRandomPostShopGeneralLine() => this.TryGetRandomLine(this.m_VOPostShopGeneralLines);

  public string GetRandomPossibleTripleLine() => this.TryGetRandomLine(this.m_VOPossibleTripleLines);

  protected string TryGetRandomLine(List<string> lines)
  {
    if (lines == null)
      return (string) null;
    return lines.Count == 0 ? (string) null : lines[UnityEngine.Random.Range(0, lines.Count)];
  }

  public List<string> GetLinesByHumanReadableName(string humanReadableName)
  {
    switch (EnumUtils.SafeParse<BaconGuideConfig.HumanReadableVOLineCategory>(humanReadableName))
    {
      case BaconGuideConfig.HumanReadableVOLineCategory.InvalidCategory:
        Log.Gameplay.PrintError("BaconGuideConfig.GetLinesByHumanReadableName() - Invalid category name given: " + humanReadableName);
        return new List<string>();
      case BaconGuideConfig.HumanReadableVOLineCategory.All:
        return this.GetAllVOLines();
      case BaconGuideConfig.HumanReadableVOLineCategory.HeroSpecific:
        List<string> humanReadableName1 = new List<string>();
        foreach (BaconGuideConfig.VOHeroSpecificLine heroSpecificLine in this.m_VOHeroSpecificLines)
          humanReadableName1.Add(heroSpecificLine.m_VOLine);
        return humanReadableName1;
      case BaconGuideConfig.HumanReadableVOLineCategory.AFK:
        return new List<string>() { this.m_VOAFK };
      case BaconGuideConfig.HumanReadableVOLineCategory.HighestShopTier:
        return new List<string>() { this.m_VOHighestTier };
      case BaconGuideConfig.HumanReadableVOLineCategory.AfterFreezing:
        return this.m_VOFreezingLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.AfterSelling:
        return this.m_VOSellingLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.AfterShopUpgrade:
        return this.m_VOShopUpgradeLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.AfterTriple:
        return this.m_VOTripleLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.Ahead:
        return this.m_VOPostShopWinLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.Behind:
        return this.m_VOPostShopLoseLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.CombatLoss:
        return this.m_VOPostCombatLoseLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.CombatWin:
        return this.m_VOPostCombatWinLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.FirstPlace:
        return this.m_VOPostShopIsFirstLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.Flavor:
        return this.m_VOSpecialIdleLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.General:
        return this.m_VOPostCombatGeneralLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.Hire:
        return this.m_VORefreshLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.Idle:
        return this.m_VOIdleLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.NewGame:
        return this.m_VONewGameLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.RecruitLargeMinion:
        return this.m_VORecruitLargeLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.RecruitMediumMinion:
        return this.m_VORecruitMediumLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.RecruitSmallMinion:
        return this.m_VORecruitSmallLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.ShopToCombat:
        return this.m_VOPostShopGeneralLines;
      case BaconGuideConfig.HumanReadableVOLineCategory.PossibleTriple:
        return this.m_VOPossibleTripleLines;
      default:
        Log.Gameplay.PrintError("BaconGuideConfig.GetLinesByHumanReadableName() - Unable to parse category name given: " + humanReadableName);
        return new List<string>();
    }
  }

  [Serializable]
  public class VOHeroSpecificLine
  {
    public string m_HeroCardId;
    [CustomEditField(T = EditType.SOUND_PREFAB)]
    public string m_VOLine;
  }

  public enum HumanReadableVOLineCategory
  {
    InvalidCategory,
    All,
    HeroSpecific,
    AFK,
    HighestShopTier,
    AfterFreezing,
    AfterSelling,
    AfterShopUpgrade,
    AfterTriple,
    Ahead,
    Behind,
    CombatLoss,
    CombatWin,
    FirstPlace,
    Flavor,
    General,
    Hire,
    Idle,
    NewGame,
    RecruitLargeMinion,
    RecruitMediumMinion,
    RecruitSmallMinion,
    ShopToCombat,
    PossibleTriple,
  }
}
