using Hearthstone.DataModels;

public class BattlegroundsFinisherRewardData : RewardData
{
  public BattlegroundsFinisherRewardData()
    : this(0L, new BattlegroundsFinisherDataModel())
  {
  }

  public BattlegroundsFinisherRewardData(long finisherId, BattlegroundsFinisherDataModel dataModel)
    : base(Reward.Type.BATTLEGROUNDS_FINISHER, true)
  {
    this.FinisherId = finisherId;
    this.DataModel = dataModel;
  }

  public BattlegroundsFinisherDataModel DataModel { get; private set; }

  public long FinisherId { get; set; }

  public override string ToString() => string.Format("[BattlegroundsFinisherRewardData: FinisherId={0} Origin={1} OriginData={2}]", (object) this.FinisherId, (object) this.Origin, (object) this.OriginData);

  protected override string GetAssetPath() => "BattlegroundsFinisherReward.prefab:1ccdb05bb23b23648afdd2a9989a7629";
}
