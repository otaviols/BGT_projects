using System.Collections.Generic;
using UnityEngine;

public class DraftManaCurve : MonoBehaviour
{
  public List<ManaCostBar> m_bars;
  private List<int> m_manaCosts;
  private const int MAX_CARDS = 10;
  private const float SIZE_PER_CARD = 0.1f;

  private void Awake() => this.ResetBars();

  public void UpdateBars()
  {
    int num = 0;
    foreach (int manaCost in this.m_manaCosts)
    {
      if (manaCost > num)
        num = manaCost;
    }
    if (num < 10)
      num = 10;
    for (int index = 0; index < this.m_bars.Count; ++index)
    {
      this.m_bars[index].m_maxValue = (float) num;
      this.m_bars[index].AnimateBar((float) this.m_manaCosts[index]);
    }
  }

  public void AddCardOfCost(int cost)
  {
    if (this.m_manaCosts == null)
      return;
    cost = Mathf.Clamp(cost, 0, this.m_manaCosts.Count - 1);
    this.m_manaCosts[cost]++;
    this.UpdateBars();
  }

  public void ResetBars()
  {
    this.m_manaCosts = new List<int>();
    for (int index = 0; index < this.m_bars.Count; ++index)
      this.m_manaCosts.Add(0);
    this.UpdateBars();
  }

  public void AddCardToManaCurve(EntityDef entityDef)
  {
    if (entityDef == null)
      Debug.LogWarning((object) "DraftManaCurve.AddCardToManaCurve() - entityDef is null");
    else
      this.AddCardOfCost(entityDef.GetCost());
  }
}
