using Hearthstone.DataModels;
using System;

public class MercenariesKnockoutRewardData : RewardData
{
  public RewardItemDataModel MercenaryDataModel { get; private set; }

  public RewardItemDataModel KnockoutDataModel { get; private set; }

  private Action OnDestroyReward
  {
    set => this.\u003COnDestroyReward\u003Ek__BackingField = value;
  }

  public MercenariesKnockoutRewardData(
    RewardItemDataModel mercenaryDataModel,
    RewardItemDataModel knockoutDataModel,
    Action onDestroyReward = null)
    : base(Reward.Type.MERCENARY_KNOCKOUT, true)
  {
    this.MercenaryDataModel = mercenaryDataModel;
    this.KnockoutDataModel = knockoutDataModel;
    this.OnDestroyReward = onDestroyReward;
  }

  protected override string GetAssetPath() => "MercenariesKnockoutReward.prefab:5d024a3773d16e44da415b6693266fdc";
}
