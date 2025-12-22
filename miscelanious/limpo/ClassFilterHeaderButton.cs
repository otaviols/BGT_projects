using UnityEngine;

public class ClassFilterHeaderButton : PegUIElement
{
  public SlidingTray m_classFilterTray;
  public UberText m_headerText;
  public Transform m_showTwoRowsBone;
  public ClassFilterButtonContainer m_container;

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
    NotificationManager.Get().DestroyAllPopUps();
    this.m_container.UpdateButtons();
    this.m_classFilterTray.ToggleTraySlider(true, this.m_showTwoRowsBone);
  }

  public void SetMode(CollectionUtils.ViewMode mode, TAG_CLASS? classTag)
  {
    Log.CollectionManager.Print("transitionPageId={0} mode={1} classTag={2}", (object) CollectionManager.Get().GetCollectibleDisplay().GetPageManager().GetTransitionPageId(), (object) mode, (object) classTag);
    switch (mode)
    {
      case CollectionUtils.ViewMode.HERO_SKINS:
      case CollectionUtils.ViewMode.HERO_PICKER:
        this.m_headerText.Text = GameStrings.Get("GLUE_COLLECTION_MANAGER_HERO_SKINS_TITLE");
        break;
      case CollectionUtils.ViewMode.CARD_BACKS:
        this.m_headerText.Text = GameStrings.Get("GLUE_COLLECTION_MANAGER_CARD_BACKS_TITLE");
        break;
      case CollectionUtils.ViewMode.COINS:
        this.m_headerText.Text = GameStrings.Get("GLUE_COLLECTION_MANAGER_COIN_TITLE");
        break;
      default:
        if (classTag.HasValue)
        {
          this.m_headerText.Text = GameStrings.GetClassName(classTag.Value);
          break;
        }
        this.m_headerText.Text = "";
        break;
    }
  }
}
