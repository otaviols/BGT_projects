using Assets;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTray : CraftingTrayBase
{
  public UIBButton m_doneButton;
  public PegUIElement m_massDisenchantButton;
  public UberText m_potentialDustAmount;
  public UberText m_massDisenchantText;
  public CheckBox m_normalOwnedCheckbox;
  public CheckBox m_normalMissingCheckbox;
  public CheckBox m_premiumOwnedCheckbox;
  public CheckBox m_premiumMissingCheckbox;
  public CheckBox m_includeUncraftableCheckbox;
  public HighlightState m_highlight;
  public GameObject m_massDisenchantMesh;
  public Material m_massDisenchantMaterial;
  public Material m_massDisenchantDisabledMaterial;
  private int m_dustAmount;
  private bool m_shown;
  private CollectionUtils.ViewMode m_previousViewMode;
  private List<CollectibleCard> m_disenchantCards = new List<CollectibleCard>();
  private static CraftingTray s_instance;
  private static PlatformDependentValue<int> MASS_DISENCHANT_MATERIAL_TO_SWITCH = new PlatformDependentValue<int>(PlatformCategory.Screen)
  {
    PC = 0,
    Phone = 1
  };

  public static event Action CraftingTrayShown;

  public static event Action CraftingTrayHidden;

  private void Awake() => CraftingTray.s_instance = this;

  private void Start()
  {
    this.m_doneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonReleased));
    this.m_massDisenchantButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnMassDisenchantButtonReleased));
    this.m_massDisenchantButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnMassDisenchantButtonOver));
    this.m_massDisenchantButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnMassDisenchantButtonOut));
    this.SetMassDisenchantAmount();
    this.m_normalOwnedCheckbox.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.CheckboxChanged(this.m_normalOwnedCheckbox.IsChecked())));
    this.m_normalOwnedCheckbox.SetChecked(true);
    this.m_normalMissingCheckbox.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.CheckboxChanged(this.m_normalMissingCheckbox.IsChecked())));
    this.m_normalMissingCheckbox.SetChecked(true);
    this.m_premiumOwnedCheckbox.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.CheckboxChanged(this.m_premiumOwnedCheckbox.IsChecked())));
    this.m_premiumOwnedCheckbox.SetChecked(true);
    this.m_premiumMissingCheckbox.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.CheckboxChanged(this.m_premiumMissingCheckbox.IsChecked())));
    this.m_premiumMissingCheckbox.SetChecked(false);
    this.m_includeUncraftableCheckbox.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.CheckboxChanged(this.m_includeUncraftableCheckbox.IsChecked())));
    this.m_includeUncraftableCheckbox.SetChecked(true);
  }

  private void OnDestroy() => CraftingTray.s_instance = (CraftingTray) null;

  public static CraftingTray Get() => CraftingTray.s_instance;

  public void UpdateMassDisenchantAmount() => this.SetMassDisenchantEnabled(this.m_dustAmount > 0 && !GameUtils.AtPrereleaseEvent());

  private void SetMassDisenchantEnabled(bool enabled)
  {
    this.m_massDisenchantButton.SetEnabled(enabled);
    this.m_massDisenchantText.gameObject.SetActive(enabled);
    this.m_potentialDustAmount.gameObject.SetActive(enabled);
    this.m_highlight.gameObject.SetActive(enabled);
    Renderer component = this.m_massDisenchantMesh.GetComponent<Renderer>();
    if (enabled)
    {
      component.SetMaterial((int) CraftingTray.MASS_DISENCHANT_MATERIAL_TO_SWITCH, this.m_massDisenchantMaterial);
      this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    }
    else
      component.SetMaterial((int) CraftingTray.MASS_DISENCHANT_MATERIAL_TO_SWITCH, this.m_massDisenchantDisabledMaterial);
  }

  public void SetMassDisenchantAmount()
  {
    if (!this.gameObject.activeSelf)
      return;
    this.StartCoroutine(this.SetMassDisenchantAmountWhenReady());
  }

  private IEnumerator SetMassDisenchantAmountWhenReady()
  {
    while ((UnityEngine.Object) MassDisenchant.Get() == (UnityEngine.Object) null)
      yield return (object) null;
    CollectionManager.Get().GetMassDisenchantCards(this.m_disenchantCards);
    MassDisenchant.Get().UpdateContents(this.m_disenchantCards);
    int totalAmount = MassDisenchant.Get().GetTotalAmount();
    this.m_dustAmount = totalAmount;
    this.m_potentialDustAmount.Text = totalAmount.ToString();
    this.UpdateMassDisenchantAmount();
  }

  public override void Show(
    bool? overrideIncludeUncraftable = null,
    bool? overrideNormalOwned = null,
    bool? overrideNormalMissing = null,
    bool? overridePremiumOwned = null,
    bool? overridePremiumMissing = null,
    bool updatePage = true)
  {
    if (this.m_shown)
      return;
    this.m_shown = true;
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.CRAFTING);
    if (overrideIncludeUncraftable.HasValue)
      this.m_includeUncraftableCheckbox.SetChecked(overrideIncludeUncraftable.Value);
    if (overrideNormalOwned.HasValue)
      this.m_normalOwnedCheckbox.SetChecked(overrideNormalOwned.Value);
    if (overrideNormalMissing.HasValue)
      this.m_normalMissingCheckbox.SetChecked(overrideNormalMissing.Value);
    if (overridePremiumOwned.HasValue)
      this.m_premiumOwnedCheckbox.SetChecked(overridePremiumOwned.Value);
    if (overridePremiumMissing.HasValue)
      this.m_premiumMissingCheckbox.SetChecked(overridePremiumMissing.Value);
    this.SetMassDisenchantAmount();
    (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as CollectionPageManager).ShowCraftingModeCards(showUncraftable: this.m_includeUncraftableCheckbox.IsChecked(), showNormalOwned: this.m_normalOwnedCheckbox.IsChecked(), showNormalMissing: this.m_normalMissingCheckbox.IsChecked(), showPremiumOwned: this.m_premiumOwnedCheckbox.IsChecked(), showPremiumMissing: this.m_premiumMissingCheckbox.IsChecked(), updatePage: updatePage);
    Action craftingTrayShown = CraftingTray.CraftingTrayShown;
    if (craftingTrayShown == null)
      return;
    craftingTrayShown();
  }

  public override void Hide()
  {
    this.Hide(true);
    Action craftingTrayHidden = CraftingTray.CraftingTrayHidden;
    if (craftingTrayHidden == null)
      return;
    craftingTrayHidden();
  }

  public void Hide(bool updatePage = true)
  {
    if (!this.m_shown)
      return;
    this.m_shown = false;
    PresenceMgr.Get().SetPrevStatus();
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    collectibleDisplay.HideCraftingTray();
    if (!updatePage)
      return;
    int num = CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.MASS_DISENCHANT ? 1 : 0;
    BookPageManager.PageTransitionType transitionType = num != 0 ? BookPageManager.PageTransitionType.MANY_PAGE_LEFT : BookPageManager.PageTransitionType.NONE;
    collectibleDisplay.GetPageManager().HideCraftingModeCards(transitionType);
    if (num == 0)
      return;
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(this.m_previousViewMode);
  }

  public override bool IsShown() => this.m_shown;

  public void EnableCraftingInBackground(bool enable = true)
  {
    CollectionPageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as CollectionPageManager;
    if (enable)
      pageManager.ShowCraftingModeCards(showUncraftable: this.m_includeUncraftableCheckbox.IsChecked(), showNormalOwned: this.m_normalOwnedCheckbox.IsChecked(), showNormalMissing: this.m_normalMissingCheckbox.IsChecked(), showPremiumOwned: this.m_premiumOwnedCheckbox.IsChecked(), showPremiumMissing: this.m_premiumMissingCheckbox.IsChecked(), updatePage: false);
    else
      pageManager.HideCraftingModeCards(updatePage: false);
  }

  private void OnDoneButtonReleased(UIEvent e) => this.Hide(true);

  private void OnMassDisenchantButtonReleased(UIEvent e)
  {
    if (CollectionManager.Get().GetCollectibleDisplay().GetPageManager().ArePagesTurning())
      return;
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.MASS_DISENCHANT)
    {
      CollectionManager.Get().GetCollectibleDisplay().SetViewMode(this.m_previousViewMode);
      this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    }
    else
    {
      this.m_previousViewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
      CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.MASS_DISENCHANT);
      this.StartCoroutine(MassDisenchant.Get().StartHighlight());
    }
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("Hub_Click.prefab:cc2cf2b5507827149b13d12210c0f323"));
  }

  private void OnMassDisenchantButtonOver(UIEvent e)
  {
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("Hub_Mouseover.prefab:40130da7b734190479c527d6bca1a4a8"));
  }

  private void OnMassDisenchantButtonOut(UIEvent e)
  {
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.MASS_DISENCHANT)
      return;
    int num = 0;
    try
    {
      num = int.Parse(this.m_potentialDustAmount.Text);
    }
    catch (Exception ex)
    {
      Log.All.PrintWarning("Exception when attempting to parse CraftingTray's m_potentialDustAmount! Exception: {0}", (object) ex);
    }
    if (num > 0)
      this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
    else
      this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
  }

  private void CheckboxChanged(bool isChecked)
  {
    bool updatePage = CollectionManager.Get().GetCollectibleDisplay().GetViewMode() == CollectionUtils.ViewMode.CARDS;
    (CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as CollectionPageManager).ShowCraftingModeCards(showUncraftable: this.m_includeUncraftableCheckbox.IsChecked(), showNormalOwned: this.m_normalOwnedCheckbox.IsChecked(), showNormalMissing: this.m_normalMissingCheckbox.IsChecked(), showPremiumOwned: this.m_premiumOwnedCheckbox.IsChecked(), showPremiumMissing: this.m_premiumMissingCheckbox.IsChecked(), updatePage: updatePage, toggleChanged: true);
    if (isChecked)
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("checkbox_toggle_on.prefab:8be4c59e7387600468ac88787943da8b"), this.gameObject);
    else
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("checkbox_toggle_off.prefab:fa341d119cee1d14c941b63dba112af3"), this.gameObject);
  }
}
