using Hearthstone;
using Hearthstone.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

public class CollectibleBattlegroundsEmote : ICollectible, IComparable
{
  private SearchableString m_searchableString;

  public BattlegroundsEmoteDbfRecord DbfRecord { get; }

  public BattlegroundsEmoteId EmoteId { get; }

  public int OwnedCount => !CollectionManager.Get().OwnsBattlegroundsEmote(this.EmoteId) ? 0 : 1;

  public bool IsNewCollectible => CollectionManager.Get().ShouldShowNewBattlegroundsEmoteGlow(this.EmoteId);

  public CollectibleBattlegroundsEmote(BattlegroundsEmoteDbfRecord record)
  {
    if (record == null)
    {
      Error.AddDevFatal("CollectibleBattlegroundsEmote: DBF record unexpectedly null!");
    }
    else
    {
      this.DbfRecord = record;
      this.EmoteId = BattlegroundsEmoteId.FromTrustedValue(this.DbfRecord.ID);
    }
  }

  public int CompareTo(object obj)
  {
    if (obj == null)
      return -1;
    if (obj.Equals((object) this))
      return 0;
    if (!(obj is BattlegroundsEmoteDbfRecord battlegroundsEmoteDbfRecord) && obj is CollectibleBattlegroundsEmote battlegroundsEmote)
      battlegroundsEmoteDbfRecord = battlegroundsEmote.DbfRecord;
    return battlegroundsEmoteDbfRecord == null ? -1 : this.DbfRecord.CollectionShortName.GetString().CompareTo(battlegroundsEmoteDbfRecord.CollectionShortName.GetString());
  }

  public HashSet<string> GetSearchableTokens() => new HashSet<string>()
  {
    (string) this.DbfRecord.CollectionShortName
  };

  public SearchableString GetSearchableString()
  {
    if (this.m_searchableString == null)
      this.m_searchableString = new SearchableString(new StringBuilder().Append((string) this.DbfRecord.CollectionShortName).Append(" ").Append((string) this.DbfRecord.Description).ToString());
    return this.m_searchableString;
  }

  public BattlegroundsEmoteDataModel CreateEmoteDataModel()
  {
    BattlegroundsEmoteDataModel emoteDataModel = new BattlegroundsEmoteDataModel();
    if (this.DbfRecord == null)
    {
      Log.CollectionManager.PrintError("CollectionUtils.CreateEmoteDataModel(): DBF record was null!");
      return emoteDataModel;
    }
    emoteDataModel.EmoteDbiId = this.DbfRecord.ID;
    emoteDataModel.DisplayName = (string) this.DbfRecord.CollectionShortName;
    emoteDataModel.Description = (string) this.DbfRecord.Description;
    emoteDataModel.Animation = this.DbfRecord.AnimationPath;
    emoteDataModel.IsAnimating = this.DbfRecord.IsAnimating;
    emoteDataModel.BorderType = this.DbfRecord.BorderType;
    emoteDataModel.XOffset = (float) this.DbfRecord.XOffset;
    emoteDataModel.ZOffset = (float) this.DbfRecord.ZOffset;
    emoteDataModel.Rarity = GameStrings.Get(GameStrings.GetRarityTextKey((TAG_RARITY) this.DbfRecord.Rarity));
    if (CollectionManager.Get().IsFullyLoaded())
    {
      emoteDataModel.IsOwned = CollectionManager.Get().OwnsBattlegroundsEmote(this.EmoteId);
      emoteDataModel.IsNew = CollectionManager.Get().ShouldShowNewBattlegroundsEmoteGlow(this.EmoteId);
      emoteDataModel.IsEquipped = CollectionManager.Get().IsEquippedBattlegroundsEmote(this.EmoteId);
    }
    return emoteDataModel;
  }
}
