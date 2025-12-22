using System;

public class MercenariesClassFilterHeaderButton : PegUIElement
{
  public SlidingTray m_roleFilterTray;
  public UberText m_headerText;
  public MercenariesClassFilterButtonContainer m_container;
  private TAG_ROLE m_displayedRole;

  protected override void Awake()
  {
    base.Awake();
    this.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.HandleRelease()));
    if (!(CollectionManager.Get()?.GetCollectibleDisplay()?.GetPageManager() is LettuceCollectionPageManager collectionPageManager))
      collectionPageManager = (LettuceCollectionPageManager) null;
    LettuceCollectionPageManager pageManager = collectionPageManager;
    if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
      pageManager.PageTransitioned += new EventHandler(this.OnPageTransitioned);
    this.RefreshHeaderText(pageManager);
  }

  protected override void OnDestroy()
  {
    if (!(CollectionManager.Get()?.GetCollectibleDisplay()?.GetPageManager() is LettuceCollectionPageManager collectionPageManager1))
      collectionPageManager1 = (LettuceCollectionPageManager) null;
    LettuceCollectionPageManager collectionPageManager2 = collectionPageManager1;
    if ((UnityEngine.Object) collectionPageManager2 != (UnityEngine.Object) null)
      collectionPageManager2.PageTransitioned -= new EventHandler(this.OnPageTransitioned);
    base.OnDestroy();
  }

  public void HandleRelease()
  {
    this.m_roleFilterTray.ToggleTraySlider(true);
    this.m_container.UpdateRoleButtons();
    NotificationManager.Get().DestroyAllPopUps();
  }

  public void OnPageTransitioned(object sender, EventArgs e) => this.RefreshHeaderText(sender as LettuceCollectionPageManager);

  public void RefreshHeaderText(LettuceCollectionPageManager pageManager)
  {
    TAG_ROLE currentRoleContext = pageManager.CurrentRoleContext;
    if (currentRoleContext == this.m_displayedRole)
      return;
    if (pageManager.CurrentRoleContext == TAG_ROLE.FIGHTER)
      this.m_headerText.Text = GameStrings.Get("GLUE_LETTUCE_MERCENARY_TUTORIAL_INGAME_POPUP_BODY_2");
    else if (pageManager.CurrentRoleContext == TAG_ROLE.CASTER)
      this.m_headerText.Text = GameStrings.Get("GLUE_LETTUCE_MERCENARY_TUTORIAL_INGAME_POPUP_BODY_3");
    else if (pageManager.CurrentRoleContext == TAG_ROLE.TANK)
      this.m_headerText.Text = GameStrings.Get("GLUE_LETTUCE_MERCENARY_TUTORIAL_INGAME_POPUP_BODY_1");
    this.m_displayedRole = currentRoleContext;
  }
}
