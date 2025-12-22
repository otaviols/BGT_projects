using PegasusShared;
using UnityEngine;

public struct RunePattern
{
  public static readonly RuneType[] ValidRuneTypes = new RuneType[3]
  {
    RuneType.RT_BLOOD,
    RuneType.RT_FROST,
    RuneType.RT_UNHOLY
  };
  private int m_blood;
  private int m_frost;
  private int m_unholy;

  public int Blood
  {
    get => this.m_blood;
    private set => this.m_blood = value < 0 ? 0 : value;
  }

  public int Frost
  {
    get => this.m_frost;
    private set => this.m_frost = value < 0 ? 0 : value;
  }

  public int Unholy
  {
    get => this.m_unholy;
    private set => this.m_unholy = value < 0 ? 0 : value;
  }

  public bool HasRunes => this.CombinedValue > 0;

  public int CombinedValue => this.Blood + this.Frost + this.Unholy;

  public bool HasMaxAmountOfOneRuneType => this.Blood == DeckRule_DeathKnightRuneLimit.MaxRuneSlots || this.Frost == DeckRule_DeathKnightRuneLimit.MaxRuneSlots || this.Unholy == DeckRule_DeathKnightRuneLimit.MaxRuneSlots;

  public RunePattern(int blood = 0, int frost = 0, int unholy = 0)
    : this()
  {
    this.Blood = blood;
    this.Frost = frost;
    this.Unholy = unholy;
  }

  public RunePattern(EntityBase entityBase)
    : this()
  {
    if (entityBase == null)
      return;
    this.Blood = entityBase.GetTag(GAME_TAG.COST_BLOOD);
    this.Frost = entityBase.GetTag(GAME_TAG.COST_FROST);
    this.Unholy = entityBase.GetTag(GAME_TAG.COST_UNHOLY);
  }

  public RunePattern(RuneType[] runes)
    : this()
  {
    if (runes == null)
      return;
    foreach (RuneType rune in runes)
      this.AddRunes(rune, 1);
  }

  public void SetCostsFromEntity(EntityBase entityBase)
  {
    if (entityBase == null)
      return;
    this.Blood = entityBase.GetTag(GAME_TAG.COST_BLOOD);
    this.Frost = entityBase.GetTag(GAME_TAG.COST_FROST);
    this.Unholy = entityBase.GetTag(GAME_TAG.COST_UNHOLY);
  }

  public int GetCost(RuneType rune)
  {
    switch (rune)
    {
      case RuneType.RT_BLOOD:
        return this.Blood;
      case RuneType.RT_FROST:
        return this.Frost;
      case RuneType.RT_UNHOLY:
        return this.Unholy;
      default:
        return 0;
    }
  }

  public bool CanAddRunes(RunePattern runesToAdd, int maxRuneSlots)
  {
    int combinedValue = this.CombinedValue;
    foreach (RuneType validRuneType in RunePattern.ValidRuneTypes)
    {
      int cost1 = runesToAdd.GetCost(validRuneType);
      int cost2 = this.GetCost(validRuneType);
      if (cost1 > cost2)
        combinedValue += cost1 - cost2;
      if (combinedValue > maxRuneSlots)
        return false;
    }
    return true;
  }

  public bool Matches(RunePattern other) => other.Blood == this.Blood && other.Frost == this.Frost && other.Unholy == this.Unholy;

  public RunePattern CombineRunes(RunePattern runesToAdd, int maxRuneSlots)
  {
    RunePattern runePattern = new RunePattern();
    int b = maxRuneSlots - this.CombinedValue;
    int a1 = runesToAdd.Blood - this.Blood;
    if (a1 > 0 && b > 0)
    {
      this.Blood = Mathf.Min(runesToAdd.Blood, b);
      runePattern.Blood = Mathf.Min(a1, b);
      b -= this.Blood;
    }
    int a2 = runesToAdd.Frost - this.Frost;
    if (a2 > 0 && b > 0)
    {
      this.Frost = Mathf.Min(runesToAdd.Frost, b);
      runePattern.Frost = Mathf.Min(a2, b);
      b -= this.Frost;
    }
    int a3 = runesToAdd.Unholy - this.Unholy;
    if (a3 > 0 && b > 0)
    {
      this.Unholy = Mathf.Min(runesToAdd.Unholy, b);
      runePattern.Unholy = Mathf.Min(a3, b);
    }
    return runePattern;
  }

  public void AddRunes(RuneType rune, int amount)
  {
    switch (rune)
    {
      case RuneType.RT_BLOOD:
        this.Blood += amount;
        break;
      case RuneType.RT_FROST:
        this.Frost += amount;
        break;
      case RuneType.RT_UNHOLY:
        this.Unholy += amount;
        break;
    }
  }

  public RuneType[] ToArray()
  {
    if (this.CombinedValue <= 0)
      return new RuneType[0];
    RuneType[] array = new RuneType[this.CombinedValue];
    int index1 = 0;
    foreach (RuneType validRuneType in RunePattern.ValidRuneTypes)
    {
      int cost = this.GetCost(validRuneType);
      for (int index2 = 0; index2 < cost; ++index2)
      {
        array[index1] = validRuneType;
        ++index1;
      }
    }
    return array;
  }
}
