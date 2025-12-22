using System;
using System.Collections.Generic;
using System.Linq;

public class CollectibleBattlegroundsBoardSet : CollectibleFilteredSet<CollectibleBattlegroundsBoard>
{
  private static readonly CollectibleBattlegroundsBoardSet.CollectibleBoardComparer s_BoardsComparer = new CollectibleBattlegroundsBoardSet.CollectibleBoardComparer();

  public CollectibleBattlegroundsBoardSet()
    : base((IComparer<CollectibleBattlegroundsBoard>) CollectibleBattlegroundsBoardSet.s_BoardsComparer)
  {
  }

  public int AddItemsFromDbf() => this.AddItems((IEnumerable<CollectibleBattlegroundsBoard>) CollectibleBattlegroundsBoardSet.GetItemsFromDbf());

  private static List<CollectibleBattlegroundsBoard> GetItemsFromDbf() => GameDbf.BattlegroundsBoardSkin.GetRecords().Select<BattlegroundsBoardSkinDbfRecord, CollectibleBattlegroundsBoard>((Func<BattlegroundsBoardSkinDbfRecord, CollectibleBattlegroundsBoard>) (record => new CollectibleBattlegroundsBoard(record))).ToList<CollectibleBattlegroundsBoard>();

  private class CollectibleBoardComparer : IComparer<CollectibleBattlegroundsBoard>
  {
    public int Compare(CollectibleBattlegroundsBoard board1, CollectibleBattlegroundsBoard board2)
    {
      bool flag1 = board1.BoardSkinId.IsDefaultBoard();
      bool flag2 = board2.BoardSkinId.IsDefaultBoard();
      if (flag1 && !flag2)
        return -1;
      if (flag2 && !flag1)
        return 1;
      bool flag3 = board1.OwnedCount > 0;
      bool flag4 = board2.OwnedCount > 0;
      if (flag3 && !flag4)
        return -1;
      if (flag4 && !flag3)
        return 1;
      string str = GameStrings.Get((string) board1.DbfRecord.CollectionShortName);
      string strB = GameStrings.Get((string) board2.DbfRecord.CollectionShortName);
      return str != strB ? str.CompareTo(strB) : board1.BoardSkinId.ToValue().CompareTo(board2.BoardSkinId.ToValue());
    }
  }
}
