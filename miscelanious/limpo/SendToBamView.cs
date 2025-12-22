using System;
using UnityEngine;

public class SendToBamView : ShopView.IComponent
{
  private StoreSendToBAM m_sendToBam;

  public bool IsLoaded => (UnityEngine.Object) this.m_sendToBam != (UnityEngine.Object) null;

  public bool IsShown => this.IsLoaded && this.m_sendToBam.IsShown();

  public event Action OnComponentReady = () => { };

  public event Action<MoneyOrGTAPPTransaction, StoreSendToBAM.BAMReason> OnOkay = (transaction, reason) => { };

  public event Action<MoneyOrGTAPPTransaction> OnCancel = transaction => { };

  public void Load(IAssetLoader assetLoader)
  {
    if (this.IsLoaded)
      return;
    assetLoader.InstantiatePrefab((AssetReference) (string) ShopPrefabs.ShopSendToBamPrefab, new PrefabCallback<GameObject>(this.OnLoaded));
  }

  public void Unload()
  {
    if (!this.IsLoaded)
      return;
    this.m_sendToBam.RemoveOkayListener(new StoreSendToBAM.DelOKListener(this.OkayListener));
    this.m_sendToBam.RemoveCancelListener(new StoreSendToBAM.DelCancelListener(this.CancelListener));
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_sendToBam.gameObject);
    this.m_sendToBam = (StoreSendToBAM) null;
  }

  public void Show(
    MoneyOrGTAPPTransaction transaction,
    StoreSendToBAM.BAMReason reason,
    string errorCode,
    bool fromPreviousPurchase)
  {
    if (!this.IsLoaded)
      return;
    this.m_sendToBam.Show(transaction, reason, errorCode, fromPreviousPurchase);
  }

  public void Hide()
  {
    if (!this.IsShown)
      return;
    this.m_sendToBam.Hide();
  }

  private void OnLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "SendToBAMView.OnLoaded(): go is null!");
    }
    else
    {
      this.m_sendToBam = go.GetComponent<StoreSendToBAM>();
      if ((UnityEngine.Object) this.m_sendToBam == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "SendToBAMView.OnLoaded(): go has no StoreSendToBAM component");
      }
      else
      {
        this.m_sendToBam.Hide();
        this.m_sendToBam.RegisterOkayListener(new StoreSendToBAM.DelOKListener(this.OkayListener));
        this.m_sendToBam.RegisterCancelListener(new StoreSendToBAM.DelCancelListener(this.CancelListener));
        this.OnComponentReady();
      }
    }
  }

  private void OkayListener(MoneyOrGTAPPTransaction transaction, StoreSendToBAM.BAMReason reason) => this.OnOkay(transaction, reason);

  private void CancelListener(MoneyOrGTAPPTransaction transaction) => this.OnCancel(transaction);
}
