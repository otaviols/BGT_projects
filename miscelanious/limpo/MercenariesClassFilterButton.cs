using UnityEngine;

public class MercenariesClassFilterButton : PegUIElement
{
  public GameObject m_newCardCount;
  public UberText m_newCardCountText;
  [SerializeField]
  private TAG_ROLE m_role;

  public TAG_ROLE Role => this.m_role;

  protected override void Awake()
  {
    this.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.HandleRelease()));
    base.Awake();
  }

  public void HandleRelease()
  {
    (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as LettuceCollectionPageManager).SelectRole(this.m_role);
    this.GetComponentInParent<SlidingTray>().HideTray();
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
