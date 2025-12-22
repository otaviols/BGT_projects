using System;

public class GoldRewardData : RewardData
{
  public GoldRewardData()
    : this(0L)
  {
  }

  public GoldRewardData(long amount)
    : this(amount, new DateTime?())
  {
  }

  public GoldRewardData(long amount, DateTime? date)
    : this(amount, date, "", "")
  {
  }

  public GoldRewardData(
    long amount,
    DateTime? date,
    string nameOverride,
    string descriptionOverride)
    : base(Reward.Type.GOLD)
  {
    this.Amount = amount;
    this.Date = date;
    this.NameOverride = nameOverride;
    this.DescriptionOverride = descriptionOverride;
  }

  public GoldRewardData(GoldRewardData oldData)
    : base(Reward.Type.GOLD)
  {
    this.Amount = oldData.Amount;
    this.Date = oldData.Date;
    this.NameOverride = oldData.NameOverride;
    this.DescriptionOverride = oldData.DescriptionOverride;
  }

  public long Amount { get; set; }

  public DateTime? Date { get; set; }

  public override string ToString() => string.Format("[GoldRewardData: Amount={0} Origin={1} OriginData={2}]", (object) this.Amount, (object) this.Origin, (object) this.OriginData);

  protected override string GetAssetPath() => "GoldReward.prefab:8e5e9429ae51d8b4bac2a9fb3826e548";
}
