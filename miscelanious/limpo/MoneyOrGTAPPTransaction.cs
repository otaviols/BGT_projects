using PegasusShared;

public class MoneyOrGTAPPTransaction
{
  public static readonly BattlePayProvider? UNKNOWN_PROVIDER;
  public const int LOCKED_THIRD_PARTY_QUANTITY = 1;

  public long ID { get; }

  public long? PMTProductID { get; }

  public bool IsGTAPP { get; }

  public BattlePayProvider? Provider { get; }

  public bool ClosedStore { get; set; }

  public MoneyOrGTAPPTransaction(
    long id,
    long? pmtProductID,
    BattlePayProvider? provider,
    bool isGTAPP)
  {
    this.ID = id;
    this.PMTProductID = pmtProductID;
    this.IsGTAPP = isGTAPP;
    this.Provider = provider;
    this.ClosedStore = false;
  }

  public override int GetHashCode() => this.ID.GetHashCode() * this.PMTProductID.GetHashCode();

  public override bool Equals(object obj)
  {
    if (!(obj is MoneyOrGTAPPTransaction gtappTransaction))
      return false;
    bool flag = !this.Provider.HasValue || !gtappTransaction.Provider.HasValue || this.Provider.Value == gtappTransaction.Provider.Value;
    int num1;
    if (gtappTransaction.ID == this.ID)
    {
      long? pmtProductId1 = gtappTransaction.PMTProductID;
      long? pmtProductId2 = this.PMTProductID;
      num1 = pmtProductId1.GetValueOrDefault() == pmtProductId2.GetValueOrDefault() & pmtProductId1.HasValue == pmtProductId2.HasValue ? 1 : 0;
    }
    else
      num1 = 0;
    int num2 = flag ? 1 : 0;
    return (num1 & num2) != 0;
  }

  public override string ToString() => string.Format("[MoneyOrGTAPPTransaction: ID={0}, PmtProductID='{1}', IsGTAPP={2}, Provider={3}]", (object) this.ID, (object) this.PMTProductID, (object) this.IsGTAPP, this.Provider.HasValue ? (object) this.Provider.Value.ToString() : (object) "UNKNOWN");

  public bool ShouldShowMiniSummary() => StoreManager.HasExternalStore || this.ClosedStore;
}
