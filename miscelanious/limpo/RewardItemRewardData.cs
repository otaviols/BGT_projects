using Hearthstone.DataModels;
using System;

public class RewardItemRewardData : RewardData
{
  public RewardItemDataModel DataModel { get; private set; }

  public Action OnDestroyReward { get; private set; }

  public RewardItemRewardData(
    RewardItemDataModel dataModel,
    bool showQuestToast,
    Reward.Type rewardType = Reward.Type.REWARD_ITEM,
    Action onDestroyReward = null)
    : base(rewardType, showQuestToast)
  {
    this.DataModel = dataModel;
    this.OnDestroyReward = onDestroyReward;
  }

  protected override string GetAssetPath() => "RewardItemReward.prefab:dd30749fc49afda46b59f7c48d47522c";
}
