using UnityEngine;

public class BaconCollectionFilterButton : MonoBehaviour
{
  public UIBButton m_activeFilterButton;
  public UIBButton m_inactiveFilterButton;

  protected void Awake()
  {
    if ((Object) this.m_inactiveFilterButton != (Object) null)
      this.m_inactiveFilterButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ToggleFilters()));
    if (!((Object) this.m_activeFilterButton != (Object) null))
      return;
    this.m_activeFilterButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ToggleFilters()));
  }

  public void SetActive(bool active)
  {
    if ((Object) this.m_activeFilterButton != (Object) null && this.m_activeFilterButton.IsEnabled() != active)
    {
      this.m_activeFilterButton.SetEnabled(active);
      this.m_activeFilterButton.Flip(active);
    }
    if (!((Object) this.m_inactiveFilterButton != (Object) null) || this.m_inactiveFilterButton.IsEnabled() == active)
      return;
    this.m_inactiveFilterButton.SetEnabled(active);
    this.m_inactiveFilterButton.Flip(active);
  }

  private void ToggleFilters()
  {
    (CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay).ToggleHeroSkinFilterMode();
    this.FilterUpdated();
  }

  public void FilterUpdated()
  {
    if ((CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay).GetHeroSkinFilterMode() == CollectionUtils.BattlegroundsHeroSkinFilterMode.DEFAULT)
    {
      this.m_activeFilterButton.gameObject.SetActive(false);
      this.m_inactiveFilterButton.gameObject.SetActive(true);
    }
    else
    {
      this.m_activeFilterButton.gameObject.SetActive(true);
      this.m_inactiveFilterButton.gameObject.SetActive(false);
    }
  }
}
