using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class ClassFilterButton : PegUIElement
{
  public GameObject m_newCardCount;
  public UberText m_newCardCountText;
  public GameObject m_disabled;
  public CollectionUtils.ViewMode m_tabViewMode;
  private CollectionTabInfo m_tabInfo;

  protected override void Awake()
  {
    this.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.HandleRelease()));
    base.Awake();
  }

  public void HandleRelease()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    CollectibleDisplay collectibleDisplay = collectionManager.GetCollectibleDisplay();
    CollectionPageManager pageManager = collectibleDisplay.GetPageManager() as CollectionPageManager;
    switch (this.m_tabViewMode)
    {
      case CollectionUtils.ViewMode.CARDS:
        if (this.m_tabInfo.tagClass != TAG_CLASS.INVALID)
        {
          if ((Object) pageManager == (Object) null)
          {
            Debug.Log((object) "ClassFilterButton: HandleRelease: pageManager is null");
            return;
          }
          pageManager.JumpToCollectionClassPage(this.m_tabInfo);
          break;
        }
        break;
      case CollectionUtils.ViewMode.CARD_BACKS:
        collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.CARD_BACKS);
        break;
      case CollectionUtils.ViewMode.COINS:
        collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.COINS);
        break;
      case CollectionUtils.ViewMode.HERO_PICKER:
        if ((Object) pageManager == (Object) null)
        {
          Debug.Log((object) "ClassFilterButton: HandleRelease: pageManager is null");
          return;
        }
        CollectionUtils.ViewMode mode = (pageManager.IsSearching() ? 1 : (collectionManager.GetEditedDeck() != null ? 1 : 0)) != 0 ? CollectionUtils.ViewMode.HERO_SKINS : CollectionUtils.ViewMode.HERO_PICKER;
        collectibleDisplay.SetViewMode(mode);
        break;
    }
    this.GetComponentInParent<SlidingTray>().HideTray();
  }

  public void SetTabInfo(CollectionTabInfo tabInfo, Material material)
  {
    this.m_tabInfo = tabInfo;
    Renderer component = this.GetComponent<Renderer>();
    component.SetMaterial(material);
    bool flag = this.m_tabInfo.tagClass == TAG_CLASS.INVALID;
    this.GetComponent<BoxCollider>().enabled = !flag;
    component.enabled = !flag;
    if ((Object) this.m_newCardCount != (Object) null)
      this.m_newCardCount.SetActive(!flag);
    if (!((Object) this.m_disabled != (Object) null))
      return;
    this.m_disabled.SetActive(flag);
  }

  public void SetNewCardCount(int count)
  {
    if ((Object) this.m_newCardCount != (Object) null)
      this.m_newCardCount.SetActive(count > 0);
    if (count <= 0 || !((Object) this.m_newCardCountText != (Object) null))
      return;
    this.m_newCardCountText.Text = GameStrings.Format("GLUE_COLLECTION_NEW_CARD_CALLOUT", (object) count);
  }
}
