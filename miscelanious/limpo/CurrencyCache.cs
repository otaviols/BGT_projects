using Blizzard.T5.Services;
using Hearthstone.Core;
using Hearthstone.DataModels;
using System;

public class CurrencyCache
{
  private CurrencyCache.StatusFlags m_status;
  private int m_requestAttempts;
  private float m_secondsBetweenRequests;
  private DateTime m_lastGetBalanceRequestTime;
  private bool m_lastIsAvailable;

  public PriceDataModel PriceDataModel { get; }

  public CurrencyType Type { get; }

  public event Action<CurrencyBalanceChangedEventArgs> OnBalanceChanged;

  public event Action OnFirstCache;

  public event Action BalanceAvailabilityChanged;

  public CurrencyCache(CurrencyType type)
  {
    this.Type = type;
    this.PriceDataModel = new PriceDataModel();
    this.PriceDataModel.Currency = type;
    this.PriceDataModel.Amount = 0.0f;
    this.PriceDataModel.DisplayText = string.Empty;
    this.m_status = (CurrencyCache.StatusFlags) 0;
    this.m_requestAttempts = 0;
    this.m_secondsBetweenRequests = 8f;
    this.m_lastGetBalanceRequestTime = DateTime.MinValue;
  }

  public void UpdateDisplayText() => this.UpdateDisplayText(this.PriceDataModel.Amount.ToString());

  public void UpdateDisplayText(string text) => this.PriceDataModel.DisplayText = text;

  public void TryRefresh()
  {
    if (!this.CanRefresh())
      return;
    string currencyCode = ShopUtils.GetCurrencyCode(this.Type);
    HearthstoneCheckout hearthstoneCheckout = ServiceManager.Get<HearthstoneCheckout>();
    if (hearthstoneCheckout == null || !hearthstoneCheckout.IsAvailable())
    {
      Log.Store.PrintError("Cannot request virtual currency balance. Commerce service unavailable");
      this.m_status |= CurrencyCache.StatusFlags.REFRESH_FAILED;
      this.FireAvailabilityChangedIfNeeded();
    }
    else
    {
      ++this.m_requestAttempts;
      this.m_status |= CurrencyCache.StatusFlags.REFRESHING;
      this.m_lastGetBalanceRequestTime = DateTime.UtcNow;
      Log.Store.PrintDebug("Requesting Virtual Currency balance for {0} (attempt #{1})", (object) this.Type, (object) this.m_requestAttempts);
      Processor.RunCoroutine(hearthstoneCheckout.GetVirtualCurrencyBalance(currencyCode, new HearthstoneCheckout.VirtualCurrencyBalanceCallback(this.HandleVirtualCurrencyBalanceCallback), (Action<bool>) (succeeded =>
      {
        if (!succeeded)
        {
          Log.Store.PrintWarning("Failed to send getBalance request");
          this.m_status |= CurrencyCache.StatusFlags.REFRESH_FAILED;
          this.FireAvailabilityChangedIfNeeded();
        }
        else
        {
          if (this.m_requestAttempts <= 0)
            return;
          this.m_secondsBetweenRequests *= 2f;
          if ((double) this.m_secondsBetweenRequests < 64.0)
            return;
          this.m_secondsBetweenRequests = 64f;
          Log.Store.PrintError("Request for virtual currency type {0} is taking a very long time.", (object) this.Type);
        }
      })));
    }
  }

  public void UpdateBalance(long balance)
  {
    int num = this.IsCached() ? 1 : 0;
    this.m_status = CurrencyCache.StatusFlags.CACHED;
    long amount = (long) this.PriceDataModel.Amount;
    this.PriceDataModel.Amount = (float) balance;
    this.UpdateDisplayText();
    if (this.OnBalanceChanged != null && amount != balance)
      this.OnBalanceChanged(new CurrencyBalanceChangedEventArgs(this.Type, amount, balance));
    if (num != 0 || this.OnFirstCache == null)
      return;
    this.OnFirstCache();
  }

  public void MarkDirty() => this.m_status |= CurrencyCache.StatusFlags.DIRTY;

  public bool IsDirty() => (this.m_status & CurrencyCache.StatusFlags.DIRTY) != 0;

  public bool IsCached() => (this.m_status & CurrencyCache.StatusFlags.CACHED) != 0;

  public bool IsRefreshing() => (this.m_status & CurrencyCache.StatusFlags.REFRESHING) != 0;

  public bool HasError() => (this.m_status & CurrencyCache.StatusFlags.REFRESH_FAILED) != 0;

  public bool IsBalanceAvailable() => !this.IsRefreshableCurrency() || (this.m_status & CurrencyCache.StatusFlags.CACHED) != (CurrencyCache.StatusFlags) 0 && ((this.m_status & CurrencyCache.StatusFlags.REFRESH_FAILED) == (CurrencyCache.StatusFlags) 0 || this.m_requestAttempts < 3);

  public bool NeedsRefresh()
  {
    if (!this.IsRefreshableCurrency())
      return false;
    return !this.IsCached() || this.IsDirty() || this.HasError();
  }

  private void FireAvailabilityChangedIfNeeded()
  {
    bool flag = this.IsBalanceAvailable();
    if (this.m_lastIsAvailable == flag)
      return;
    this.m_lastIsAvailable = flag;
    Action availabilityChanged = this.BalanceAvailabilityChanged;
    if (availabilityChanged == null)
      return;
    availabilityChanged();
  }

  private bool IsRefreshableCurrency() => ShopUtils.IsCurrencyVirtual(this.Type) && ShopUtils.IsVirtualCurrencyEnabled();

  private bool CanRefresh() => !ServiceManager.Get<HearthstoneCheckout>().IsClientCreationInProgress() && this.IsRefreshableCurrency() && (!this.IsRefreshing() || (DateTime.UtcNow - this.m_lastGetBalanceRequestTime).TotalSeconds >= (double) this.m_secondsBetweenRequests);

  private void HandleVirtualCurrencyBalanceCallback(
    HearthstoneCheckout.VirtualCurrencyBalanceResult result)
  {
    if (result.isSuccess)
    {
      Log.Store.PrintDebug(string.Format("Virtual Currency balance received for {0}: {1}", (object) this.Type, (object) result.balance));
      this.m_requestAttempts = 0;
      this.m_secondsBetweenRequests = 8f;
      this.UpdateBalance(result.balance);
    }
    else
    {
      this.m_status |= CurrencyCache.StatusFlags.REFRESH_FAILED;
      Log.Store.PrintError(string.Format("Virtual Currency '{0}' balance refresh failed and will retry shortly. Error: {1}", (object) this.Type, (object) result.errorMessage));
    }
    this.FireAvailabilityChangedIfNeeded();
  }

  [Flags]
  private enum StatusFlags
  {
    REFRESHING = 1,
    CACHED = 2,
    DIRTY = 4,
    REFRESH_FAILED = 8,
  }
}
