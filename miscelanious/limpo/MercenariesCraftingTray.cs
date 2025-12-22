using Assets;
using System;

public class MercenariesCraftingTray : CraftingTrayBase
{
  public UIBButton m_doneButton;
  public UberText m_mercenaryCount;
  public CheckBox m_showOnlyPromotableCheckbox;
  public CheckBox m_showCraftableCheckbox;
  private bool m_shown;
  private static MercenariesCraftingTray s_instance;

  private void Awake() => MercenariesCraftingTray.s_instance = this;

  private void Start()
  {
    this.m_doneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonReleased));
    this.m_showOnlyPromotableCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleShowOnlyPromotable));
    this.m_showOnlyPromotableCheckbox.SetChecked(false);
    this.m_showCraftableCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleShowCraftable));
    this.m_showCraftableCheckbox.SetChecked(false);
  }

  private void OnDestroy() => MercenariesCraftingTray.s_instance = (MercenariesCraftingTray) null;

  public static MercenariesCraftingTray Get() => MercenariesCraftingTray.s_instance;

  public override void Show(
    bool? overrideShowCraftable = null,
    bool? overrideShowOnlyPromotable = null,
    bool? unused1 = null,
    bool? unused2 = null,
    bool? unused3 = null,
    bool updatePage = true)
  {
    this.m_shown = true;
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.CRAFTING);
    if (overrideShowCraftable.HasValue)
      this.m_showCraftableCheckbox.SetChecked(overrideShowCraftable.Value);
    if (overrideShowOnlyPromotable.HasValue)
      this.m_showOnlyPromotableCheckbox.SetChecked(overrideShowOnlyPromotable.Value);
    this.SetMercenaryTotalCount();
    (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as LettuceCollectionPageManager).ShowCraftingModeMercs(showCraftableMercs: this.m_showCraftableCheckbox.IsChecked(), showOnlyPromotableMercs: this.m_showOnlyPromotableCheckbox.IsChecked(), updatePage: updatePage);
  }

  public override void Hide()
  {
    this.m_shown = false;
    PresenceMgr.Get().SetPrevStatus();
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    collectibleDisplay.HideCraftingTray();
    BookPageManager.PageTransitionType transitionType = BookPageManager.PageTransitionType.NONE;
    collectibleDisplay.GetPageManager().HideCraftingModeCards(transitionType);
  }

  public override bool IsShown() => this.m_shown;

  private void OnDoneButtonReleased(UIEvent e) => this.Hide();

  private void SetMercenaryTotalCount() => this.m_mercenaryCount.Text = GameStrings.Format("GLUE_DECK_TRAY_COUNT", (object) CollectionManager.Get().GetOwnedMercenaryCount(), (object) CollectionManager.Get().GetTotalMercenaryCount());

  private void ToggleShowOnlyPromotable(UIEvent e)
  {
    bool showOnlyPromotableMercs = this.m_showOnlyPromotableCheckbox.IsChecked();
    (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as LettuceCollectionPageManager).ShowCraftingModeMercs(showCraftableMercs: this.m_showCraftableCheckbox.IsChecked(), showOnlyPromotableMercs: showOnlyPromotableMercs, toggleChanged: true);
    if (showOnlyPromotableMercs)
      SoundManager.Get().LoadAndPlay((AssetReference) "checkbox_toggle_on.prefab:8be4c59e7387600468ac88787943da8b", this.gameObject);
    else
      SoundManager.Get().LoadAndPlay((AssetReference) "checkbox_toggle_off.prefab:fa341d119cee1d14c941b63dba112af3", this.gameObject);
  }

  private void ToggleShowCraftable(UIEvent e)
  {
    bool showCraftableMercs = this.m_showCraftableCheckbox.IsChecked();
    (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as LettuceCollectionPageManager).ShowCraftingModeMercs(showCraftableMercs: showCraftableMercs, showOnlyPromotableMercs: this.m_showOnlyPromotableCheckbox.IsChecked(), toggleChanged: true);
    if (showCraftableMercs)
      SoundManager.Get().LoadAndPlay((AssetReference) "checkbox_toggle_on.prefab:8be4c59e7387600468ac88787943da8b", this.gameObject);
    else
      SoundManager.Get().LoadAndPlay((AssetReference) "checkbox_toggle_off.prefab:fa341d119cee1d14c941b63dba112af3", this.gameObject);
  }
}
