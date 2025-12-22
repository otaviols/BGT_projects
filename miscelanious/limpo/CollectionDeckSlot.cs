using Blizzard.T5.Core.Utils;
using System.Collections.Generic;

public class CollectionDeckSlot
{
  public CollectionDeckSlot.DelOnSlotEmptied OnSlotEmptied;
  private int m_index;
  private List<int> m_count = new List<int>((IEnumerable<int>) new int[EnumUtils.Length<TAG_PREMIUM>()]);
  private string m_cardId;
  private bool m_owned = true;
  public EntityDef m_entityDefOverride;

  public override string ToString() => string.Format("[CollectionDeckSlot: Index={0}, PreferredPremium={1}, Count={2}, CardID={3}]", (object) this.Index, (object) this.PreferredPremium, (object) this.Count, (object) this.CardID);

  public int Index
  {
    get => this.m_index;
    set => this.m_index = value;
  }

  public TAG_PREMIUM PreferredPremium
  {
    get
    {
      TAG_PREMIUM preferredPremium1 = CollectionManager.Get().GetPreferredPremium();
      if (this.m_count[(int) preferredPremium1] > 0)
        return preferredPremium1;
      TAG_PREMIUM preferredPremium2 = TAG_PREMIUM.NORMAL;
      for (int index = this.m_count.Count - 1; index > 0; --index)
      {
        if (this.m_count[index] > 0)
          return (TAG_PREMIUM) index;
      }
      return preferredPremium2;
    }
  }

  public TAG_PREMIUM UnPreferredPremium
  {
    get
    {
      TAG_PREMIUM preferredPremium = CollectionManager.Get().GetPreferredPremium();
      for (int index = 0; index < this.m_count.Count; ++index)
      {
        if (this.m_count[index] > 0 && (TAG_PREMIUM) index != preferredPremium)
          return (TAG_PREMIUM) index;
      }
      return preferredPremium;
    }
  }

  public int Count
  {
    get
    {
      int count = 0;
      foreach (int num in this.m_count)
        count += num;
      return count;
    }
  }

  public string CardID
  {
    get => this.m_cardId;
    set => this.m_cardId = value;
  }

  public bool Owned
  {
    get => this.m_owned;
    set => this.m_owned = value;
  }

  public int GetCount(TAG_PREMIUM premium) => this.m_count[(int) premium];

  public void SetCount(int count, TAG_PREMIUM premium)
  {
    this.m_count[(int) premium] = count;
    if (this.Count > 0 || this.OnSlotEmptied == null)
      return;
    this.OnSlotEmptied(this);
  }

  public void RemoveCard(int count, TAG_PREMIUM premium)
  {
    this.m_count[(int) premium] -= count;
    if (this.Count > 0 || this.OnSlotEmptied == null)
      return;
    this.OnSlotEmptied(this);
  }

  public void AddCard(int count, TAG_PREMIUM premium) => this.m_count[(int) premium] += count;

  public void CreateEntityDefOverride()
  {
    if (this.m_entityDefOverride != null)
      return;
    this.m_entityDefOverride = DefLoader.Get().GetEntityDef(this.m_cardId).Clone();
  }

  public EntityDef GetEntityDef() => this.m_entityDefOverride != null ? this.m_entityDefOverride : DefLoader.Get().GetEntityDef(this.m_cardId);

  public void CopyFrom(CollectionDeckSlot otherSlot)
  {
    this.Index = otherSlot.Index;
    this.m_count = new List<int>((IEnumerable<int>) otherSlot.m_count);
    this.CardID = otherSlot.CardID;
    this.Owned = otherSlot.Owned;
  }

  public delegate void DelOnSlotEmptied(CollectionDeckSlot slot);
}
