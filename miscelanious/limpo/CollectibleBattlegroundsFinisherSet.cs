using System;
using System.Collections.Generic;
using System.Linq;

public class CollectibleBattlegroundsFinisherSet : 
  CollectibleFilteredSet<CollectibleBattlegroundsFinisher>
{
  private static readonly CollectibleBattlegroundsFinisherSet.CollectibleFinisherComparer s_FinisherComparer = new CollectibleBattlegroundsFinisherSet.CollectibleFinisherComparer();

  public CollectibleBattlegroundsFinisherSet()
    : base((IComparer<CollectibleBattlegroundsFinisher>) CollectibleBattlegroundsFinisherSet.s_FinisherComparer)
  {
  }

  public int AddItemsFromDbf() => this.AddItems((IEnumerable<CollectibleBattlegroundsFinisher>) CollectibleBattlegroundsFinisherSet.GetItemsFromDbf());

  private static List<CollectibleBattlegroundsFinisher> GetItemsFromDbf() => GameDbf.BattlegroundsFinisher.GetRecords().Select<BattlegroundsFinisherDbfRecord, CollectibleBattlegroundsFinisher>((Func<BattlegroundsFinisherDbfRecord, CollectibleBattlegroundsFinisher>) (record => new CollectibleBattlegroundsFinisher(record))).ToList<CollectibleBattlegroundsFinisher>();

  private class CollectibleFinisherComparer : IComparer<CollectibleBattlegroundsFinisher>
  {
    public int Compare(
      CollectibleBattlegroundsFinisher finisher1,
      CollectibleBattlegroundsFinisher finisher2)
    {
      bool flag1 = finisher1.FinisherId.IsDefaultFinisher();
      bool flag2 = finisher2.FinisherId.IsDefaultFinisher();
      if (flag1 && !flag2)
        return -1;
      if (flag2 && !flag1)
        return 1;
      bool flag3 = finisher1.OwnedCount > 0;
      bool flag4 = finisher2.OwnedCount > 0;
      if (flag3 && !flag4)
        return -1;
      if (flag4 && !flag3)
        return 1;
      string str = GameStrings.Get((string) finisher1.DbfRecord.CollectionShortName);
      string strB = GameStrings.Get((string) finisher2.DbfRecord.CollectionShortName);
      return str != strB ? str.CompareTo(strB) : finisher1.FinisherId.ToValue().CompareTo(finisher2.FinisherId.ToValue());
    }
  }
}
