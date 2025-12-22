using System.Collections.Generic;

public class CraftingPendingTransaction
{
  public string CardID;
  public TAG_PREMIUM Premium;
  public bool CardValueOverridden;
  public int NormalDisenchantCount;
  public int NormalCreateCount;
  public int GoldenDisenchantCount;
  public int GoldenCreateCount;
  public int GoldenUpgradeFromNormalCount;
  public int GoldenUpgradeFromNothingCount;
  private Stack<CraftingPendingTransaction.Operation> m_transactionOrder = new Stack<CraftingPendingTransaction.Operation>();

  public CraftingPendingTransaction.Operation Undo()
  {
    if (this.m_transactionOrder == null || this.m_transactionOrder.Count <= 0)
      return CraftingPendingTransaction.Operation.Invalid;
    CraftingPendingTransaction.Operation operation = this.m_transactionOrder.Pop();
    switch (operation)
    {
      case CraftingPendingTransaction.Operation.NormalDisenchant:
        --this.NormalDisenchantCount;
        break;
      case CraftingPendingTransaction.Operation.NormalCreate:
        --this.NormalCreateCount;
        break;
      case CraftingPendingTransaction.Operation.GoldenDisenchant:
        --this.GoldenDisenchantCount;
        break;
      case CraftingPendingTransaction.Operation.GoldenCreate:
        --this.GoldenCreateCount;
        break;
      case CraftingPendingTransaction.Operation.UpgradeToGoldenFromNormal:
      case CraftingPendingTransaction.Operation.UpgradeToGoldenFromGolden:
        --this.GoldenUpgradeFromNormalCount;
        break;
      case CraftingPendingTransaction.Operation.UpgradeToGoldenFromNothing:
        --this.GoldenUpgradeFromNothingCount;
        break;
    }
    return operation;
  }

  public void Add(CraftingPendingTransaction.Operation transaction)
  {
    if (this.m_transactionOrder == null)
      return;
    switch (transaction)
    {
      case CraftingPendingTransaction.Operation.NormalDisenchant:
        ++this.NormalDisenchantCount;
        break;
      case CraftingPendingTransaction.Operation.NormalCreate:
        ++this.NormalCreateCount;
        break;
      case CraftingPendingTransaction.Operation.GoldenDisenchant:
        ++this.GoldenDisenchantCount;
        break;
      case CraftingPendingTransaction.Operation.GoldenCreate:
        ++this.GoldenCreateCount;
        break;
      case CraftingPendingTransaction.Operation.UpgradeToGoldenFromNormal:
      case CraftingPendingTransaction.Operation.UpgradeToGoldenFromGolden:
        ++this.GoldenUpgradeFromNormalCount;
        break;
      case CraftingPendingTransaction.Operation.UpgradeToGoldenFromNothing:
        ++this.GoldenUpgradeFromNothingCount;
        break;
    }
    this.m_transactionOrder.Push(transaction);
  }

  public CraftingPendingTransaction ShallowCopy() => this.MemberwiseClone() as CraftingPendingTransaction;

  public int GetTransactionAmount(TAG_PREMIUM premium)
  {
    if (premium == TAG_PREMIUM.NORMAL)
      return this.NormalCreateCount - (this.NormalDisenchantCount + this.GoldenUpgradeFromNormalCount);
    return premium == TAG_PREMIUM.GOLDEN ? this.GoldenCreateCount + this.GoldenUpgradeFromNormalCount + this.GoldenUpgradeFromNothingCount - this.GoldenDisenchantCount : 0;
  }

  public bool HasPendingTransactions() => this.m_transactionOrder.Count != 0;

  public void ResetTransactionAmount()
  {
    this.NormalDisenchantCount = 0;
    this.NormalCreateCount = 0;
    this.GoldenDisenchantCount = 0;
    this.GoldenCreateCount = 0;
    this.GoldenUpgradeFromNormalCount = 0;
    this.GoldenUpgradeFromNothingCount = 0;
  }

  public int GetExpectedTransactionCost(
    NetCache.CardValue normalValue,
    NetCache.CardValue goldenValue)
  {
    return -(this.NormalDisenchantCount * normalValue.GetSellValue()) - this.GoldenDisenchantCount * goldenValue.GetSellValue() + this.NormalCreateCount * normalValue.GetBuyValue() + this.GoldenCreateCount * goldenValue.GetBuyValue() + this.GoldenUpgradeFromNormalCount * normalValue.GetUpgradeValue() + this.GoldenUpgradeFromNothingCount * (normalValue.GetBuyValue() + normalValue.GetUpgradeValue());
  }

  public bool GetLastTransactionWasDisenchant()
  {
    if (this.m_transactionOrder == null || this.m_transactionOrder.Count == 0)
      return false;
    CraftingPendingTransaction.Operation lastOperation = this.GetLastOperation();
    return lastOperation == CraftingPendingTransaction.Operation.NormalDisenchant || lastOperation == CraftingPendingTransaction.Operation.GoldenDisenchant;
  }

  public bool GetLastTransactionWasCrafting()
  {
    if (this.m_transactionOrder == null || this.m_transactionOrder.Count == 0)
      return false;
    CraftingPendingTransaction.Operation lastOperation = this.GetLastOperation();
    return lastOperation != CraftingPendingTransaction.Operation.NormalDisenchant && lastOperation != CraftingPendingTransaction.Operation.GoldenDisenchant;
  }

  public bool GetLastTransactionWasUpgrade()
  {
    if (this.m_transactionOrder == null || this.m_transactionOrder.Count == 0)
      return false;
    CraftingPendingTransaction.Operation operation = this.m_transactionOrder.Peek();
    switch (operation)
    {
      case CraftingPendingTransaction.Operation.UpgradeToGoldenFromNormal:
      case CraftingPendingTransaction.Operation.UpgradeToGoldenFromNothing:
        return true;
      default:
        return operation == CraftingPendingTransaction.Operation.UpgradeToGoldenFromGolden;
    }
  }

  public CraftingPendingTransaction.Operation GetLastOperation() => this.m_transactionOrder == null || this.m_transactionOrder.Count == 0 ? CraftingPendingTransaction.Operation.Invalid : this.m_transactionOrder.Peek();

  public enum Operation
  {
    Invalid,
    NormalDisenchant,
    NormalCreate,
    GoldenDisenchant,
    GoldenCreate,
    UpgradeToGoldenFromNormal,
    UpgradeToGoldenFromNothing,
    UpgradeToGoldenFromGolden,
  }
}
