using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

public class RuneSlotVisual : MonoBehaviour
{
  public GameObject m_deckRunesContainer;
  public List<Rune> m_deckRuneSlots;

  public void Show(RuneType[] runes)
  {
    this.m_deckRunesContainer.gameObject.SetActive(true);
    this.UpdateRuneSlots(runes);
  }

  public void Show(RunePattern runes)
  {
    this.m_deckRunesContainer.gameObject.SetActive(true);
    this.UpdateRuneSlots(runes);
  }

  public void Hide() => this.m_deckRunesContainer.gameObject.SetActive(false);

  private void UpdateRuneSlots(RunePattern runes)
  {
    if (runes.CombinedValue <= 0)
      return;
    RuneType[] runes1 = new RuneType[runes.CombinedValue];
    int index1 = 0;
    foreach (RuneType validRuneType in RunePattern.ValidRuneTypes)
    {
      int cost = runes.GetCost(validRuneType);
      for (int index2 = 0; index2 < cost; ++index2)
      {
        runes1[index1] = validRuneType;
        ++index1;
      }
    }
    this.UpdateRuneSlots(runes1);
  }

  private void UpdateRuneSlots(RuneType[] runes)
  {
    if (runes == null)
      return;
    int index = 0;
    foreach (Rune deckRuneSlot in this.m_deckRuneSlots)
    {
      if (index >= runes.Length)
      {
        deckRuneSlot.ShowRune(RuneType.RT_NONE, RuneState.Empty);
      }
      else
      {
        deckRuneSlot.ShowRune(runes[index], RuneState.Default);
        ++index;
      }
    }
  }
}
