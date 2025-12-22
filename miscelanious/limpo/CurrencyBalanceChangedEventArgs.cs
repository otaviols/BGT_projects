using System;

public class CurrencyBalanceChangedEventArgs : EventArgs
{
  public CurrencyType Currency { get; }

  public CurrencyBalanceChangedEventArgs(CurrencyType type, long oldAmount, long newAmount)
  {
    this.Currency = type;
    // ISSUE: reference to a compiler-generated field
    this.\u003COldAmount\u003Ek__BackingField = oldAmount;
    // ISSUE: reference to a compiler-generated field
    this.\u003CNewAmount\u003Ek__BackingField = newAmount;
  }
}
