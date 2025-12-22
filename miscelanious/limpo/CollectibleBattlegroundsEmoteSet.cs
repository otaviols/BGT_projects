using System;
using System.Collections.Generic;
using System.Linq;

public class CollectibleBattlegroundsEmoteSet : CollectibleFilteredSet<CollectibleBattlegroundsEmote>
{
  private static readonly CollectibleBattlegroundsEmoteSet.CollectibleEmoteComparer s_EmoteComparer = new CollectibleBattlegroundsEmoteSet.CollectibleEmoteComparer();

  public CollectibleBattlegroundsEmoteSet()
    : base((IComparer<CollectibleBattlegroundsEmote>) CollectibleBattlegroundsEmoteSet.s_EmoteComparer)
  {
  }

  public int AddItemsFromDbf() => this.AddItems((IEnumerable<CollectibleBattlegroundsEmote>) CollectibleBattlegroundsEmoteSet.GetItemsFromDbf());

  private static List<CollectibleBattlegroundsEmote> GetItemsFromDbf() => GameDbf.BattlegroundsEmote.GetRecords().Select<BattlegroundsEmoteDbfRecord, CollectibleBattlegroundsEmote>((Func<BattlegroundsEmoteDbfRecord, CollectibleBattlegroundsEmote>) (record => new CollectibleBattlegroundsEmote(record))).ToList<CollectibleBattlegroundsEmote>();

  private class CollectibleEmoteComparer : IComparer<CollectibleBattlegroundsEmote>
  {
    public int Compare(CollectibleBattlegroundsEmote emote1, CollectibleBattlegroundsEmote emote2)
    {
      bool flag1 = emote1.EmoteId.IsDefaultEmote();
      bool flag2 = emote2.EmoteId.IsDefaultEmote();
      if (flag1 && !flag2)
        return -1;
      if (flag2 && !flag1)
        return 1;
      bool flag3 = emote1.OwnedCount > 0;
      bool flag4 = emote2.OwnedCount > 0;
      if (flag3 && !flag4)
        return -1;
      if (flag4 && !flag3)
        return 1;
      string str = GameStrings.Get((string) emote1.DbfRecord.CollectionShortName);
      string strB = GameStrings.Get((string) emote2.DbfRecord.CollectionShortName);
      return str != strB ? str.CompareTo(strB) : emote1.EmoteId.ToValue().CompareTo(emote2.EmoteId.ToValue());
    }
  }
}
