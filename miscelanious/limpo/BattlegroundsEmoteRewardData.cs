using Hearthstone.DataModels;

public class BattlegroundsEmoteRewardData : RewardData
{
  public BattlegroundsEmoteRewardData()
    : this(0L, new BattlegroundsEmoteDataModel())
  {
  }

  public BattlegroundsEmoteRewardData(long emoteID, BattlegroundsEmoteDataModel dataModel)
    : base(Reward.Type.BATTLEGROUNDS_EMOTE, true)
  {
    this.EmoteID = emoteID;
    this.DataModel = dataModel;
  }

  public BattlegroundsEmoteDataModel DataModel { get; private set; }

  public long EmoteID { get; set; }

  public override string ToString() => string.Format("[BattlegroundsEmoteRewardData: EmoteId={0} Origin={1} OriginData={2}]", (object) this.EmoteID, (object) this.Origin, (object) this.OriginData);

  protected override string GetAssetPath() => "BattlegroundsEmoteReward.prefab:e199e14ef82bfc045bc4e4ed939daa08";
}
