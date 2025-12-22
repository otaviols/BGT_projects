using Hearthstone.DataModels;

public class BattlegroundsBoardSkinRewardData : RewardData
{
  public BattlegroundsBoardSkinRewardData()
    : this(0L, new BattlegroundsBoardSkinDataModel())
  {
  }

  public BattlegroundsBoardSkinRewardData(
    long boardSkinId,
    BattlegroundsBoardSkinDataModel dataModel)
    : base(Reward.Type.BATTLEGROUNDS_BOARD_SKIN, true)
  {
    this.BoardSkinId = boardSkinId;
    this.DataModel = dataModel;
  }

  public BattlegroundsBoardSkinDataModel DataModel { get; private set; }

  public long BoardSkinId { get; set; }

  public override string ToString() => string.Format("[BattlegroundsBoardSkinRewardData: BoardSkinId={0} Origin={1} OriginData={2}]", (object) this.BoardSkinId, (object) this.Origin, (object) this.OriginData);

  protected override string GetAssetPath() => "BattlegroundsBoardSkinReward.prefab:f9cb78c7c8244924cac09826e6310962";
}
