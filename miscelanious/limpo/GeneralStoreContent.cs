using Shared.Scripts.Util.ValueTypes;
using System.Collections.Generic;
using UnityEngine;

public class GeneralStoreContent : MonoBehaviour
{
  protected GeneralStore m_parentStore;
  private bool m_isContentActive;
  private NoGTAPPTransactionData m_currentGoldBundle;
  private Network.Bundle m_currentMoneyBundle;
  private List<GeneralStoreContent.BundleChanged> m_bundleChangedListeners = new List<GeneralStoreContent.BundleChanged>();

  public void SetParentStore(GeneralStore parentStore) => this.m_parentStore = parentStore;

  public void SetContentActive(bool active) => this.m_isContentActive = active;

  public bool IsContentActive()
  {
    if (!this.m_isContentActive)
      return false;
    return (Object) this.m_parentStore == (Object) null || !this.m_parentStore.IsCovered();
  }

  public void SetCurrentGoldBundle(NoGTAPPTransactionData bundle)
  {
    if (this.m_currentGoldBundle == bundle)
      return;
    this.m_currentMoneyBundle = (Network.Bundle) null;
    this.m_currentGoldBundle = bundle;
    this.OnBundleChanged(this.m_currentGoldBundle, this.m_currentMoneyBundle);
    this.FireBundleChangedEvent();
  }

  public NoGTAPPTransactionData GetCurrentGoldBundle() => this.m_currentGoldBundle;

  public void SetCurrentMoneyBundle(Network.Bundle bundle, bool force = false)
  {
    if (!force && (Record) this.m_currentMoneyBundle == (Record) bundle && (Record) bundle != (Record) null)
      return;
    this.m_currentGoldBundle = (NoGTAPPTransactionData) null;
    this.m_currentMoneyBundle = bundle;
    this.OnBundleChanged(this.m_currentGoldBundle, this.m_currentMoneyBundle);
    this.FireBundleChangedEvent();
  }

  public Network.Bundle GetCurrentMoneyBundle() => this.m_currentMoneyBundle;

  public void Refresh() => this.OnRefresh();

  public bool HasBundleSet() => (Record) this.m_currentMoneyBundle != (Record) null || this.m_currentGoldBundle != null;

  public void RegisterCurrentBundleChanged(GeneralStoreContent.BundleChanged dlg) => this.m_bundleChangedListeners.Add(dlg);

  public void UnregisterCurrentBundleChanged(GeneralStoreContent.BundleChanged dlg) => this.m_bundleChangedListeners.Remove(dlg);

  public virtual bool AnimateEntranceStart() => true;

  public virtual bool AnimateEntranceEnd() => true;

  public virtual bool AnimateExitStart() => true;

  public virtual bool AnimateExitEnd() => true;

  public virtual void PreStoreFlipIn()
  {
  }

  public virtual void PostStoreFlipIn(bool animatedFlipIn)
  {
  }

  public virtual void PreStoreFlipOut()
  {
  }

  public virtual void PostStoreFlipOut()
  {
  }

  public virtual void TryBuyWithMoney(
    Network.Bundle bundle,
    GeneralStoreContent.BuyEvent successBuyCB,
    GeneralStoreContent.BuyEvent failedBuyCB)
  {
    if (successBuyCB == null)
      return;
    successBuyCB();
  }

  public virtual void TryBuyWithGold(
    GeneralStoreContent.BuyEvent successBuyCB = null,
    GeneralStoreContent.BuyEvent failedBuyCB = null)
  {
    if (successBuyCB == null)
      return;
    successBuyCB();
  }

  public virtual void StoreShown(bool isCurrent)
  {
  }

  public virtual void StoreHidden(bool isCurrent)
  {
  }

  public virtual void OnCoverStateChanged(bool coverActive)
  {
  }

  public virtual bool IsPurchaseDisabled() => false;

  public virtual string GetMoneyDisplayOwnedText() => string.Empty;

  protected virtual void OnBundleChanged(
    NoGTAPPTransactionData goldBundle,
    Network.Bundle moneyBundle)
  {
  }

  protected virtual void OnRefresh()
  {
  }

  private void FireBundleChangedEvent()
  {
    if (!this.IsContentActive())
      return;
    foreach (GeneralStoreContent.BundleChanged bundleChanged in this.m_bundleChangedListeners.ToArray())
      bundleChanged(this.m_currentGoldBundle, this.m_currentMoneyBundle);
  }

  public delegate void BuyEvent();

  public delegate void BundleChanged(
    NoGTAPPTransactionData newGoldBundle,
    Network.Bundle newMoneyBundle);
}
