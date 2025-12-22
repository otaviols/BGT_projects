using UnityEngine;

public class BaconClassFilterHeaderButton : PegUIElement
{
  public SlidingTray m_classFilterTray;
  public UberText m_headerText;
  public Transform m_showTwoRowsBone;
  private ClassFilterButton[] m_buttons;

  protected override void Awake()
  {
    this.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.HandleRelease()));
    base.Awake();
  }

  public void HandleRelease()
  {
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((Object) collectibleDisplay != (Object) null)
      collectibleDisplay.HideDeckHelpPopup();
    CollectionManager.Get().GetEditedDeck();
    if (this.m_buttons == null)
      this.m_buttons = this.m_classFilterTray.GetComponentsInChildren<ClassFilterButton>();
    this.m_classFilterTray.ToggleTraySlider(true, this.m_showTwoRowsBone);
    NotificationManager.Get().DestroyAllPopUps();
  }

  public void SetMode(CollectionUtils.ViewMode mode)
  {
    Log.CollectionManager.Print("transitionPageId={0} mode={1}", (object) CollectionManager.Get().GetCollectibleDisplay().GetPageManager().GetTransitionPageId(), (object) mode);
    switch (mode)
    {
      case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
        this.m_headerText.Text = GameStrings.Get("GLUE_BACON_COLLECTION_MANAGER_GUIDE_SKINS_TITLE");
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
        this.m_headerText.Text = GameStrings.Get("GLUE_COLLECTION_MANAGER_HERO_SKINS_TITLE");
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS:
        this.m_headerText.Text = GameStrings.Get("GLUE_BACON_COLLECTION_MANAGER_BOARD_SKINS_TITLE");
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS:
        this.m_headerText.Text = GameStrings.Get("GLUE_BACON_COLLECTION_MANAGER_FINISHERS_TITLE");
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES:
        this.m_headerText.Text = GameStrings.Get("GLUE_BACON_COLLECTION_MANAGER_EMOTES_TITLE");
        break;
      default:
        this.m_headerText.Text = "";
        break;
    }
  }
}
