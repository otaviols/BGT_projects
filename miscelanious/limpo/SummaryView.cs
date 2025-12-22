using Hearthstone.Commerce;
using System;
using UnityEngine;

public class SummaryView : ShopView.IComponent
{
  private StoreSummary m_summary;

  public bool IsLoaded => (UnityEngine.Object) this.m_summary != (UnityEngine.Object) null;

  public bool IsShown => this.IsLoaded && this.m_summary.IsShown();

  public event Action OnComponentReady = () => { };

  public event Action<int, object> OnSummaryConfirm = (quantity, userData) => { };

  public event Action<object> OnSummaryCancel = userData => { };

  public event Action<object> OnSummaryInfo = userData => { };

  public event Action<object> OnSummaryPaymentAndTos = userData => { };

  public void Load(IAssetLoader assetLoader)
  {
    if (this.IsLoaded)
      return;
    assetLoader.InstantiatePrefab((AssetReference) (string) ShopPrefabs.ShopSummaryPrefab, new PrefabCallback<GameObject>(this.OnLoaded));
  }

  public void Unload()
  {
    if (!this.IsLoaded)
      return;
    this.m_summary.RemoveConfirmListener(new StoreSummary.ConfirmCallback(this.ConfirmListener));
    this.m_summary.RemoveCancelListener(new StoreSummary.CancelCallback(this.CancelListener));
    this.m_summary.RemoveInfoListener(new StoreSummary.InfoCallback(this.InfoListener));
    this.m_summary.RemovePaymentAndTOSListener(new StoreSummary.PaymentAndTOSCallback(this.PaymentAndTosListener));
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_summary.gameObject);
    this.m_summary = (StoreSummary) null;
  }

  public void Show(ProductId productID, int quantity, string paymentMethodName)
  {
    if (!this.IsLoaded)
      return;
    this.m_summary.Show(productID, quantity, paymentMethodName);
  }

  public void Hide()
  {
    if (!this.IsShown)
      return;
    this.m_summary.Hide();
  }

  private void OnLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "SummaryView.OnLoaded(): go is null!");
    }
    else
    {
      this.m_summary = go.GetComponent<StoreSummary>();
      if ((UnityEngine.Object) this.m_summary == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "SummaryView.OnLoaded(): go has no StoreSummary component");
      }
      else
      {
        this.m_summary.Hide();
        this.m_summary.RegisterConfirmListener(new StoreSummary.ConfirmCallback(this.ConfirmListener));
        this.m_summary.RegisterCancelListener(new StoreSummary.CancelCallback(this.CancelListener));
        this.m_summary.RegisterInfoListener(new StoreSummary.InfoCallback(this.InfoListener));
        this.m_summary.RegisterPaymentAndTOSListener(new StoreSummary.PaymentAndTOSCallback(this.PaymentAndTosListener));
        this.OnComponentReady();
      }
    }
  }

  private void ConfirmListener(int quantity, object userData) => this.OnSummaryConfirm(quantity, userData);

  private void CancelListener(object userData) => this.OnSummaryCancel(userData);

  private void InfoListener(object userData) => this.OnSummaryInfo(userData);

  private void PaymentAndTosListener(object userData) => this.OnSummaryPaymentAndTos(userData);
}
