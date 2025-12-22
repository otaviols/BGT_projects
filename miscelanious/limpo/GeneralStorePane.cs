using UnityEngine;

[CustomEditClass]
public class GeneralStorePane : MonoBehaviour
{
  public GeneralStoreContent m_parentContent;
  public GameObject m_paneContainer;

  public void Refresh() => this.OnRefresh();

  public virtual bool AnimateEntranceStart() => true;

  public virtual bool AnimateEntranceEnd() => true;

  public virtual bool AnimateExitStart() => true;

  public virtual bool AnimateExitEnd() => true;

  public virtual void PrePaneSwappedIn()
  {
  }

  public virtual void PostPaneSwappedIn()
  {
  }

  public virtual void PrePaneSwappedOut()
  {
  }

  public virtual void PostPaneSwappedOut()
  {
  }

  public virtual void OnPurchaseFinished()
  {
  }

  public virtual void StoreShown(bool isCurrent)
  {
  }

  public virtual void StoreHidden(bool isCurrent)
  {
  }

  protected virtual void OnRefresh()
  {
  }
}
