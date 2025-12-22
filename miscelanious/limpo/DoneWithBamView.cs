using System;
using UnityEngine;

public class DoneWithBamView : ShopView.IComponent
{
  private StoreDoneWithBAM m_doneWithBam;

  public bool IsLoaded => (UnityEngine.Object) this.m_doneWithBam != (UnityEngine.Object) null;

  public bool IsShown => this.IsLoaded && this.m_doneWithBam.IsShown();

  public event Action OnComponentReady = () => { };

  public event Action OnOkay = () => { };

  public void Load(IAssetLoader assetLoader)
  {
    if (this.IsLoaded)
      return;
    assetLoader.InstantiatePrefab((AssetReference) (string) ShopPrefabs.ShopDoneWithBamPrefab, new PrefabCallback<GameObject>(this.OnLoaded));
  }

  public void Unload()
  {
    if (!this.IsLoaded)
      return;
    this.m_doneWithBam.RemoveOkayListener(new StoreDoneWithBAM.ButtonPressedListener(this.OnOkayListener));
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_doneWithBam.gameObject);
    this.m_doneWithBam = (StoreDoneWithBAM) null;
  }

  public void Show()
  {
    if (!this.IsLoaded)
      return;
    this.m_doneWithBam.Show();
  }

  public void Hide()
  {
    if (!this.IsShown)
      return;
    this.m_doneWithBam.Hide();
  }

  private void OnLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "DoneWithBamView.OnLoaded(): go is null!");
    }
    else
    {
      this.m_doneWithBam = go.GetComponent<StoreDoneWithBAM>();
      if ((UnityEngine.Object) this.m_doneWithBam == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "DoneWithBamView.OnLoaded(): go has no StoreDoneWithBAM component");
      }
      else
      {
        this.m_doneWithBam.Hide();
        this.m_doneWithBam.RegisterOkayListener(new StoreDoneWithBAM.ButtonPressedListener(this.OnOkayListener));
        this.OnComponentReady();
      }
    }
  }

  private void OnOkayListener() => this.OnOkay();
}
