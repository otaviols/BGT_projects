using Hearthstone;
using Hearthstone.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

public class CollectibleBattlegroundsBoard : ICollectible, IComparable
{
  private SearchableString m_searchableString;

  public BattlegroundsBoardSkinDbfRecord DbfRecord { get; }

  public BattlegroundsBoardSkinId BoardSkinId { get; }

  public int OwnedCount => !CollectionManager.Get().OwnsBattlegroundsBoardSkin(this.BoardSkinId) ? 0 : 1;

  public bool IsNewCollectible => CollectionManager.Get().ShouldShowNewBattlegroundsBoardSkinGlow(this.BoardSkinId);

  public CollectibleBattlegroundsBoard(BattlegroundsBoardSkinDbfRecord record)
  {
    if (record == null)
    {
      Error.AddDevFatal("CollectibleBattlegroundsBoard: DBF record unexpectedly null!");
    }
    else
    {
      this.DbfRecord = record;
      this.BoardSkinId = BattlegroundsBoardSkinId.FromTrustedValue(this.DbfRecord.ID);
    }
  }

  public int CompareTo(object obj)
  {
    if (obj == null)
      return -1;
    if (obj.Equals((object) this))
      return 0;
    if (!(obj is BattlegroundsBoardSkinDbfRecord boardSkinDbfRecord) && obj is CollectibleBattlegroundsBoard battlegroundsBoard)
      boardSkinDbfRecord = battlegroundsBoard.DbfRecord;
    return boardSkinDbfRecord == null ? -1 : this.DbfRecord.CollectionName.GetString().CompareTo(boardSkinDbfRecord.CollectionName.GetString());
  }

  public HashSet<string> GetSearchableTokens() => new HashSet<string>()
  {
    (string) this.DbfRecord.CollectionName,
    (string) this.DbfRecord.CollectionShortName
  };

  public SearchableString GetSearchableString()
  {
    if (this.m_searchableString == null)
      this.m_searchableString = new SearchableString(new StringBuilder().Append((string) this.DbfRecord.CollectionName).Append(" ").Append((string) this.DbfRecord.CollectionShortName).Append(" ").Append((string) this.DbfRecord.Description).ToString());
    return this.m_searchableString;
  }

  public BattlegroundsBoardSkinDataModel CreateBoardDataModel()
  {
    BattlegroundsBoardSkinDataModel boardDataModel = new BattlegroundsBoardSkinDataModel();
    if (this.DbfRecord == null)
    {
      Log.CollectionManager.PrintError("CollectionUtils.CreateBoardDataModel(): DBF record was null!");
      return boardDataModel;
    }
    boardDataModel.BoardDbiId = this.DbfRecord.ID;
    boardDataModel.DisplayName = (string) this.DbfRecord.CollectionShortName;
    boardDataModel.DetailsDisplayName = (string) this.DbfRecord.CollectionName;
    boardDataModel.Description = (string) this.DbfRecord.Description;
    boardDataModel.BorderType = this.DbfRecord.BorderType;
    boardDataModel.ShopDetailsTexture = PlatformSettings.Screen == ScreenCategory.Phone ? this.DbfRecord.DetailsTexturePhone : this.DbfRecord.DetailsTexture;
    boardDataModel.ShopDetailsMovie = PlatformSettings.Screen == ScreenCategory.Phone ? this.DbfRecord.DetailsMoviePhone : this.DbfRecord.DetailsMovie;
    boardDataModel.Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) this.DbfRecord.Rarity));
    boardDataModel.IsForCollectionPage = true;
    if (CollectionManager.Get().IsFullyLoaded())
    {
      boardDataModel.IsFavorite = CollectionManager.Get().IsFavoriteBattlegroundsBoardSkin(this.BoardSkinId);
      boardDataModel.IsOwned = CollectionManager.Get().OwnsBattlegroundsBoardSkin(this.BoardSkinId);
      boardDataModel.IsNew = CollectionManager.Get().ShouldShowNewBattlegroundsBoardSkinGlow(this.BoardSkinId);
    }
    return boardDataModel;
  }
}
