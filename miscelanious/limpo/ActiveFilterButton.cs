using Blizzard.T5.MaterialService.Extensions;
using System;
using UnityEngine;

public class ActiveFilterButton : MonoBehaviour
{
  public SlidingTray m_manaFilterTray;
  public SlidingTray m_setFilterTray;
  public UberText m_searchText;
  public GameObject m_manaFilterIcon;
  public UberText m_manaFilterText;
  public PegUIElement m_activeFilterButton;
  public PegUIElement m_inactiveFilterButton;
  public ManaFilterTabManager m_manaFilter;
  public SetFilterTray m_setFilter;
  public NestedPrefab m_setFilterContainer;
  public CollectionSearch m_search;
  public PegUIElement m_offClickCatcher;
  public UIBButton m_doneButton;
  public Material m_enabledMaterial;
  public Material m_disabledMaterial;
  public MeshRenderer m_inactiveFilterButtonRenderer;
  public GameObject m_inactiveFilterButtonText;
  public Transform m_manaFilterIconCenterBone;
  public Transform m_setFilterIconCenterBone;
  private bool m_filtersShown;
  private bool m_manaFilterActive;
  private string m_manaFilterValue = "";
  private bool m_searchFilterActive;
  private string m_searchFilterValue = "";
  [SerializeField]
  private Transform m_manaFilterIconDefaultBone;
  [SerializeField]
  private Transform m_setFilterIconDefaultBone;

  protected void Awake()
  {
    if ((UnityEngine.Object) this.m_inactiveFilterButton != (UnityEngine.Object) null)
      this.m_inactiveFilterButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ShowFilters()));
    if ((UnityEngine.Object) this.m_activeFilterButton != (UnityEngine.Object) null)
      this.m_activeFilterButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ActiveFilterPressed()));
    if ((UnityEngine.Object) this.m_doneButton != (UnityEngine.Object) null)
      this.m_doneButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OffClickPressed()));
    CollectionManagerDisplay collectibleDisplay1 = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay1 != (UnityEngine.Object) null)
    {
      collectibleDisplay1.RegisterManaFilterListener(new CollectibleDisplay.FilterStateListener(this.ManaFilterUpdate));
      collectibleDisplay1.RegisterSearchFilterListener(new CollectibleDisplay.FilterStateListener(this.SearchFilterUpdate));
    }
    if ((UnityEngine.Object) this.m_manaFilter != (UnityEngine.Object) null)
      this.m_manaFilter.OnFilterCleared += new Action<bool>(this.ManaFilter_OnFilterCleared);
    CollectibleDisplay collectibleDisplay2 = CollectionManager.Get()?.GetCollectibleDisplay();
    if (!((UnityEngine.Object) collectibleDisplay2 != (UnityEngine.Object) null))
      return;
    collectibleDisplay2.OnViewModeChanged += new CollectibleDisplay.ViewModeChangedListener(this.OnCollectionManagerViewModeChanged);
  }

  protected void Start()
  {
    if ((UnityEngine.Object) this.m_setFilterContainer != (UnityEngine.Object) null)
    {
      this.m_setFilter = this.m_setFilterContainer.PrefabGameObject().GetComponent<SetFilterTray>();
      this.m_setFilter.m_toggleButton.transform.parent = this.transform;
      if (!(bool) UniversalInputManager.UsePhoneUI)
        this.m_setFilterIconDefaultBone = this.m_setFilter.m_toggleButton.transform;
    }
    if (!(bool) UniversalInputManager.UsePhoneUI)
      this.m_manaFilterIconDefaultBone = this.m_manaFilterIcon.transform;
    this.UpdateFilterView();
  }

  protected void OnDestroy()
  {
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
    {
      collectibleDisplay.UnregisterManaFilterListener(new CollectibleDisplay.FilterStateListener(this.ManaFilterUpdate));
      collectibleDisplay.UnregisterSearchFilterListener(new CollectibleDisplay.FilterStateListener(this.SearchFilterUpdate));
    }
    if (!((UnityEngine.Object) this.m_manaFilter != (UnityEngine.Object) null))
      return;
    this.m_manaFilter.OnFilterCleared -= new Action<bool>(this.ManaFilter_OnFilterCleared);
  }

  private void OnCollectionManagerViewModeChanged(
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode mode,
    CollectionUtils.ViewModeData userdata,
    bool triggerResponse)
  {
    if (!triggerResponse)
      return;
    this.UpdateFilterView();
  }

  private void ShowFilters()
  {
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.HideDeckHelpPopup();
    Navigation.Push(new Navigation.NavigateBackHandler(this.HideFilters));
    if ((UnityEngine.Object) this.m_manaFilterTray != (UnityEngine.Object) null)
      this.m_manaFilterTray.ToggleTraySlider(true);
    this.m_setFilterTray.ToggleTraySlider(true);
    this.m_setFilter.Show(true);
    if (!((UnityEngine.Object) this.m_manaFilter != (UnityEngine.Object) null))
      return;
    this.m_manaFilter.m_manaCrystalContainer.UpdateSlices();
  }

  private bool HideFilters()
  {
    if ((UnityEngine.Object) this.m_manaFilterTray != (UnityEngine.Object) null)
      this.m_manaFilterTray.ToggleTraySlider(false);
    this.m_setFilterTray.ToggleTraySlider(false);
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.m_search.Deactivate();
    this.m_setFilter.Show(false);
    return true;
  }

  private void OffClickPressed()
  {
    Navigation.GoBack();
    this.UpdateFilterView();
  }

  public void ActiveFilterPressed()
  {
    bool flag = false;
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      flag = collectibleDisplay.GetHeroSkinClass().HasValue;
    if (flag)
      this.ClearHeroSkinClass();
    else
      this.ClearFilters();
  }

  public void ClearHeroSkinClass()
  {
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
      return;
    collectibleDisplay.SetHeroSkinClass(new TAG_CLASS?());
    if (CollectionManager.Get().IsInEditMode())
      return;
    collectibleDisplay.SetViewMode(CollectionUtils.ViewMode.HERO_PICKER);
  }

  public void ClearFilters()
  {
    if ((UnityEngine.Object) this.m_manaFilter != (UnityEngine.Object) null)
      this.m_manaFilter.ClearFilter(false);
    this.m_setFilter.ClearFilter(false);
    this.m_search.ClearFilter();
  }

  public void SetEnabled(bool enabled)
  {
    this.m_inactiveFilterButton.SetEnabled(enabled);
    this.m_inactiveFilterButtonText.SetActive(enabled);
    this.m_inactiveFilterButtonRenderer.SetSharedMaterial(enabled ? this.m_enabledMaterial : this.m_disabledMaterial);
  }

  private void ManaFilter_OnFilterCleared(bool transitionPage) => this.ManaFilterUpdate(false, (object) string.Empty);

  private void ManaFilterUpdate(bool state, object description)
  {
    this.m_manaFilterActive = state;
    this.m_manaFilterValue = description != null ? (string) description : "";
    this.UpdateFilterView();
  }

  private void SearchFilterUpdate(bool state, object description)
  {
    this.m_searchFilterActive = state;
    this.m_searchFilterValue = description != null ? (string) description : "";
    this.UpdateFilterView();
  }

  public void UpdateFilterView()
  {
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay == (UnityEngine.Object) null)
      return;
    bool flag1 = this.m_searchFilterActive;
    string str1 = this.m_searchFilterValue;
    bool flag2 = collectibleDisplay.GetViewMode() == CollectionUtils.ViewMode.CARDS;
    TAG_CLASS? heroSkinClass = collectibleDisplay.GetHeroSkinClass();
    bool hasValue = heroSkinClass.HasValue;
    bool isShown = this.m_setFilter.HasActiveFilter() & flag2 && !this.m_searchFilterActive && !hasValue;
    bool flag3 = this.m_manaFilterActive & flag2 && !this.m_searchFilterActive && !hasValue;
    string str2 = this.m_manaFilterValue;
    if (hasValue)
    {
      heroSkinClass = collectibleDisplay.GetHeroSkinClass();
      str1 = GameStrings.GetClassName(heroSkinClass.Value);
      flag1 = true;
    }
    else if ((UnityEngine.Object) this.m_manaFilter != (UnityEngine.Object) null && this.m_manaFilter.IsFilterOddOrEvenValues)
    {
      flag3 = false;
      isShown = false;
      flag1 = true;
      str2 = string.Empty;
    }
    bool flag4 = flag3 | flag1 | isShown;
    if ((UnityEngine.Object) this.m_inactiveFilterButton != (UnityEngine.Object) null)
    {
      this.m_activeFilterButton.gameObject.SetActive(flag4);
      this.m_inactiveFilterButton.gameObject.SetActive(!flag4);
    }
    else
    {
      if (this.m_filtersShown != flag4)
      {
        Vector3 euler = flag4 ? new Vector3(180f, 0.0f, 0.0f) : new Vector3(0.0f, 0.0f, 0.0f);
        float num = flag4 ? 0.5f : -0.5f;
        iTween.Stop(this.m_activeFilterButton.gameObject);
        this.m_activeFilterButton.gameObject.transform.localRotation = Quaternion.Euler(euler);
        iTween.RotateBy(this.m_activeFilterButton.gameObject, iTween.Hash((object) "x", (object) num, (object) "time", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeInOutExpo));
      }
      this.m_filtersShown = flag4;
    }
    if (flag1)
    {
      this.m_searchText.gameObject.SetActive(true);
      this.m_searchText.Text = str1;
    }
    else
    {
      this.m_searchText.gameObject.SetActive(false);
      this.m_searchText.Text = string.Empty;
    }
    this.m_manaFilterIcon.SetActive(flag3);
    this.m_manaFilterText.Text = str2;
    this.m_setFilter.SetButtonShown(isShown);
    if (this.m_manaFilterIcon.activeSelf && !isShown)
      this.m_manaFilterIcon.transform.localPosition = this.m_manaFilterIconCenterBone.localPosition;
    else if (!this.m_manaFilterIcon.activeSelf & isShown)
    {
      this.m_setFilter.m_toggleButton.gameObject.transform.localPosition = this.m_setFilterIconCenterBone.localPosition;
    }
    else
    {
      if ((UnityEngine.Object) this.m_manaFilterIconDefaultBone != (UnityEngine.Object) null)
        this.m_manaFilterIcon.transform.localPosition = this.m_manaFilterIconDefaultBone.localPosition;
      if (!((UnityEngine.Object) this.m_setFilterIconDefaultBone != (UnityEngine.Object) null))
        return;
      this.m_setFilter.m_toggleButton.gameObject.transform.localPosition = this.m_setFilterIconDefaultBone.localPosition;
    }
  }
}
