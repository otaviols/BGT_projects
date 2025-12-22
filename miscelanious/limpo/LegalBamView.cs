using System;
using UnityEngine;

public class LegalBamView : ShopView.IComponent
{
  private StoreLegalBAMLinks m_legalBam;

  public bool IsLoaded => (UnityEngine.Object) this.m_legalBam != (UnityEngine.Object) null;

  public bool IsShown => this.IsLoaded && this.m_legalBam.IsShown();

  public event Action OnComponentReady = () => { };

  public event Action<StoreLegalBAMLinks.BAMReason> OnOkay = reason => { };

  public event Action OnCancel = () => { };

  public void Load(IAssetLoader assetLoader)
  {
    if (this.IsLoaded)
      return;
    assetLoader.InstantiatePrefab((AssetReference) (string) ShopPrefabs.ShopLegalBamLinksPrefab, new PrefabCallback<GameObject>(this.OnLoaded));
  }

  public void Unload()
  {
    if (!this.IsLoaded)
      return;
    this.m_legalBam.RemoveSendToBAMListener(new StoreLegalBAMLinks.SendToBAMListener(this.OkayListener));
    this.m_legalBam.RemoveCancelListener(new StoreLegalBAMLinks.CancelListener(this.CancelListener));
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_legalBam.gameObject);
    this.m_legalBam = (StoreLegalBAMLinks) null;
  }

  public void Show()
  {
    if (!this.IsLoaded)
      return;
    this.m_legalBam.Show();
  }

  public void Hide()
  {
    if (!this.IsShown)
      return;
    this.m_legalBam.Hide();
  }

  private void OnLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "LegalBamView.OnLoaded(): go is null!");
    }
    else
    {
      this.m_legalBam = go.GetComponent<StoreLegalBAMLinks>();
      if ((UnityEngine.Object) this.m_legalBam == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "LegalBamView.OnLoaded(): go has no StoreLegalBAMLinks component");
      }
      else
      {
        this.m_legalBam.Hide();
        this.m_legalBam.RegisterSendToBAMListener(new StoreLegalBAMLinks.SendToBAMListener(this.OkayListener));
        this.m_legalBam.RegisterCancelListener(new StoreLegalBAMLinks.CancelListener(this.CancelListener));
        this.OnComponentReady();
      }
    }
  }

  private void OkayListener(StoreLegalBAMLinks.BAMReason reason) => this.OnOkay(reason);

  private void CancelListener() => this.OnCancel();
}
