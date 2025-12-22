using System;
using UnityEngine;

public class PurchaseAuthView : ShopView.IComponent
{
  private StorePurchaseAuth m_purchaseAuth;

  public bool IsLoaded => (UnityEngine.Object) this.m_purchaseAuth != (UnityEngine.Object) null;

  public bool IsShown => this.IsLoaded && this.m_purchaseAuth.IsShown();

  public event Action OnComponentReady = () => { };

  public event Action<bool, MoneyOrGTAPPTransaction> OnPurchaseResultAcknowledged = (success, transaction) => { };

  public event Action OnCancelButtonPressed = () => { };

  public event Action OnAuthExit = () => { };

  public void Load(IAssetLoader assetLoader)
  {
    if (this.IsLoaded)
      return;
    assetLoader.InstantiatePrefab((AssetReference) (string) ShopPrefabs.ShopPurchaseAuthPrefab, new PrefabCallback<GameObject>(this.OnLoaded));
  }

  public void Unload()
  {
    if (!this.IsLoaded)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_purchaseAuth.gameObject);
    this.m_purchaseAuth = (StorePurchaseAuth) null;
  }

  public void Show(
    MoneyOrGTAPPTransaction transaction,
    bool isZeroCostLicense,
    StorePurchaseAuth.ButtonStyle waitButtonStyle = StorePurchaseAuth.ButtonStyle.NoButton)
  {
    if (!this.IsLoaded)
      return;
    this.m_purchaseAuth.Show(transaction, isZeroCostLicense, waitButtonStyle);
  }

  public void Hide()
  {
    if (!this.IsShown)
      return;
    this.m_purchaseAuth.Hide();
  }

  public void StartNewTransaction(
    MoneyOrGTAPPTransaction transaction,
    bool isZeroCostLicense,
    StorePurchaseAuth.ButtonStyle waitButtonStyle = StorePurchaseAuth.ButtonStyle.NoButton)
  {
    if (!this.IsLoaded)
      return;
    this.m_purchaseAuth.StartNewTransaction(transaction, isZeroCostLicense, waitButtonStyle);
  }

  public void ShowPreviousPurchaseSuccess(
    MoneyOrGTAPPTransaction transaction,
    StorePurchaseAuth.ButtonStyle waitButtonStyle = StorePurchaseAuth.ButtonStyle.NoButton)
  {
    if (!this.IsLoaded)
      return;
    this.m_purchaseAuth.ShowPreviousPurchaseSuccess(transaction, waitButtonStyle);
  }

  public void ShowPreviousPurchaseFailure(
    MoneyOrGTAPPTransaction transaction,
    string details,
    StorePurchaseAuth.ButtonStyle waitButtonStyle,
    Network.PurchaseErrorInfo.ErrorType error)
  {
    if (!this.IsLoaded)
      return;
    this.m_purchaseAuth.ShowPurchaseMethodFailure(transaction, details, waitButtonStyle, error);
  }

  public bool CompletePurchaseSuccess(MoneyOrGTAPPTransaction transaction) => this.IsLoaded && this.m_purchaseAuth.CompletePurchaseSuccess(transaction);

  public bool CompletePurchaseFailure(
    MoneyOrGTAPPTransaction transaction,
    string details,
    Network.PurchaseErrorInfo.ErrorType error)
  {
    return this.IsLoaded && this.m_purchaseAuth.CompletePurchaseFailure(transaction, details, error);
  }

  private void OnLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "PurchaseAuthView.OnLoaded(): go is null!");
    }
    else
    {
      this.m_purchaseAuth = go.GetComponent<StorePurchaseAuth>();
      if ((UnityEngine.Object) this.m_purchaseAuth == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "PurchaseAuthView.OnLoaded(): go has no StorePurchaseAuth component");
      }
      else
      {
        this.m_purchaseAuth.Hide();
        this.m_purchaseAuth.RegisterAckPurchaseResultListener((StorePurchaseAuth.AckPurchaseResultListener) ((success, transaction) => this.OnPurchaseResultAcknowledged(success, transaction)));
        this.m_purchaseAuth.RegisterCancelButtonListener((Action) (() => this.OnCancelButtonPressed()));
        this.m_purchaseAuth.RegisterExitListener((Action) (() => this.OnAuthExit()));
        this.OnComponentReady();
      }
    }
  }

  public bool HideCancelButton() => this.IsLoaded && !((UnityEngine.Object) this.m_purchaseAuth == (UnityEngine.Object) null) && this.m_purchaseAuth.HideCancelButton();
}
