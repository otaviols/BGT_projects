using UnityEngine;

public class BaconClassFilterButton : PegUIElement
{
  public GameObject m_newItemCount;
  public UberText m_newItemCountText;
  public CollectionUtils.ViewMode m_tabViewMode = CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS;
  protected int m_numNewItems;

  protected override void Awake()
  {
    this.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.HandleRelease()));
    base.Awake();
  }

  public void HandleRelease()
  {
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(this.m_tabViewMode);
    this.GetComponentInParent<SlidingTray>().HideTray();
  }

  public void UpdateNewItemCount(int numNewItems)
  {
    this.m_numNewItems = numNewItems;
    this.UpdateNewItemCountVisuals();
  }

  private void UpdateNewItemCountVisuals()
  {
    if ((Object) this.m_newItemCountText != (Object) null)
      this.m_newItemCountText.Text = GameStrings.Format("GLUE_COLLECTION_NEW_CARD_CALLOUT", (object) this.m_numNewItems);
    if (!((Object) this.m_newItemCount != (Object) null))
      return;
    this.m_newItemCount.SetActive(this.m_numNewItems > 0);
  }
}
