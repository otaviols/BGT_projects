using Hearthstone.DataModels;

public class MiniSetRewardData : RewardData
{
  public MiniSetRewardData(int cardsRewardId, int premium)
    : base(Reward.Type.MINI_SET)
  {
    this.MiniSetID = cardsRewardId;
    this.Premium = premium;
    this.DataModel = new ProductDataModel();
    if (this.Premium != 1)
      return;
    this.DataModel.Tags.Add("golden");
  }

  public ProductDataModel DataModel { get; private set; }

  public int MiniSetID { get; set; }

  public int Premium { get; set; }

  public override string ToString() => string.Format("[MiniSetRewardData: CardsRewardID={0} Origin={1} OriginData={2}]", (object) this.MiniSetID, (object) this.Origin, (object) this.OriginData);

  protected override string GetAssetPath() => "MiniSetReward.prefab:dc43a6807e16eb440a7db978dd95ab1f";
}
