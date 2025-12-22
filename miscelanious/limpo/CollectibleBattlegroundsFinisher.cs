using Hearthstone;
using Hearthstone.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

public class CollectibleBattlegroundsFinisher : ICollectible, IComparable
{
  private SearchableString m_searchableString;

  public BattlegroundsFinisherDbfRecord DbfRecord { get; }

  public BattlegroundsFinisherId FinisherId { get; }

  public int OwnedCount => !CollectionManager.Get().OwnsBattlegroundsFinisher(this.FinisherId) ? 0 : 1;

  public bool IsNewCollectible => CollectionManager.Get().ShouldShowNewBattlegroundsFinisherGlow(this.FinisherId);

  public CollectibleBattlegroundsFinisher(BattlegroundsFinisherDbfRecord record)
  {
    if (record == null)
    {
      Error.AddDevFatal("CollectibleBattlegroundsFinisher: DBF record unexpectedly null!");
    }
    else
    {
      this.DbfRecord = record;
      this.FinisherId = BattlegroundsFinisherId.FromTrustedValue(this.DbfRecord.ID);
    }
  }

  public int CompareTo(object obj)
  {
    if (obj == null)
      return -1;
    if (obj.Equals((object) this))
      return 0;
    if (!(obj is BattlegroundsFinisherDbfRecord finisherDbfRecord) && obj is CollectibleBattlegroundsFinisher battlegroundsFinisher)
      finisherDbfRecord = battlegroundsFinisher.DbfRecord;
    return finisherDbfRecord == null ? -1 : this.DbfRecord.CollectionName.GetString().CompareTo(finisherDbfRecord.CollectionName.GetString());
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

  public BattlegroundsFinisherDataModel CreateFinisherDataModel()
  {
    BattlegroundsFinisherDataModel finisherDataModel = new BattlegroundsFinisherDataModel();
    if (this.DbfRecord == null)
    {
      Log.CollectionManager.PrintError("CollectionUtils.CreateFinisherDataModel(): DBF record was null!");
      return finisherDataModel;
    }
    finisherDataModel.FinisherDbiId = this.DbfRecord.ID;
    finisherDataModel.DisplayName = (string) this.DbfRecord.CollectionShortName;
    finisherDataModel.DetailsDisplayName = (string) this.DbfRecord.CollectionName;
    finisherDataModel.Description = (string) this.DbfRecord.Description;
    finisherDataModel.ShopDetailsTexture = this.DbfRecord.DetailsTexture;
    finisherDataModel.ShopDetailsMovie = this.DbfRecord.DetailsMovie;
    finisherDataModel.BodyMaterial = this.DbfRecord.MiniBodyMaterial;
    finisherDataModel.ArtMaterial = this.DbfRecord.MiniArtMaterial;
    finisherDataModel.CapsuleType = this.DbfRecord.CapsuleType;
    finisherDataModel.Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) this.DbfRecord.Rarity));
    finisherDataModel.IsForCollectionPage = true;
    if (CollectionManager.Get().IsFullyLoaded())
    {
      finisherDataModel.IsFavorite = CollectionManager.Get().IsFavoriteBattlegroundsFinisher(this.FinisherId);
      finisherDataModel.IsOwned = CollectionManager.Get().OwnsBattlegroundsFinisher(this.FinisherId);
      finisherDataModel.IsNew = CollectionManager.Get().ShouldShowNewBattlegroundsFinisherGlow(this.FinisherId);
    }
    return finisherDataModel;
  }
}
