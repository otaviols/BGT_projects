using System;
using System.Collections.Generic;
using UnityEngine;

public class ManaFilterTabManager : MonoBehaviour
{
  public ManaFilterTab m_singleManaFilterPrefab;
  public ManaFilterTab m_dynamicManaFilterPrefab;
  public MultiSliceElement m_manaCrystalContainer;
  private bool m_tabsActive;
  private List<ManaFilterTab> m_tabs = new List<ManaFilterTab>();
  private HashSet<int> m_currentFilterExactValues = new HashSet<int>();
  private int? m_currentFilterMinValue;
  private int? m_currentFilterMaxValue;
  private bool m_currentFilterIsEven;
  private bool m_currentFilterIsOdd;

  public event Action<bool> OnFilterCleared;

  public event Action<int, bool> OnManaValueActivated;

  public void ClearFilter(bool transitionPage = true) => this.UpdateCurrentFilterToSingleValue(-1, transitionPage);

  public bool IsFilterActive => this.IsFilterOnExactValues || this.IsFilterOddOrEvenValues || this.IsFilterRange;

  public bool IsFilterOnExactValues => this.m_currentFilterExactValues.Count > 0;

  public bool IsFilterOddOrEvenValues => this.IsFilterEvenValues || this.IsFilterOddValues;

  public bool IsFilterEvenValues => this.m_currentFilterIsEven;

  public bool IsFilterOddValues => this.m_currentFilterIsOdd;

  public bool IsFilterRange => this.m_currentFilterMinValue.HasValue || this.m_currentFilterMaxValue.HasValue;

  public bool IsManaValueActive(int manaValue)
  {
    if (this.m_currentFilterExactValues.Contains(manaValue))
      return true;
    if (this.IsFilterRange)
      return (!this.m_currentFilterMinValue.HasValue || manaValue >= this.m_currentFilterMinValue.Value) && (!this.m_currentFilterMaxValue.HasValue || manaValue <= this.m_currentFilterMaxValue.Value);
    if (this.IsFilterEvenValues)
      return manaValue % 2 == 0;
    return this.IsFilterOddValues && manaValue % 2 == 1;
  }

  public void SetFilter_Range(int minCost, int maxCost)
  {
    if ((!this.m_currentFilterMinValue.HasValue || this.m_currentFilterMinValue.Value != minCost || !this.m_currentFilterMaxValue.HasValue ? 1 : (this.m_currentFilterMaxValue.Value != maxCost ? 1 : 0)) != 0)
      SoundManager.Get().LoadAndPlay((AssetReference) "mana_crystal_refresh.prefab:ea5c456dd852f904e9828db66636f54d");
    this.m_currentFilterExactValues.Clear();
    this.m_currentFilterMinValue = new int?(minCost);
    this.m_currentFilterMaxValue = new int?(maxCost);
    this.m_currentFilterIsEven = false;
    this.m_currentFilterIsOdd = false;
    this.UpdateFilterStates();
  }

  public void SetFilter_EvenOdd(bool isOdd)
  {
    if ((!this.IsFilterOddOrEvenValues ? 1 : (isOdd != this.IsFilterOddValues ? 1 : 0)) != 0)
      SoundManager.Get().LoadAndPlay((AssetReference) "mana_crystal_refresh.prefab:ea5c456dd852f904e9828db66636f54d");
    this.m_currentFilterExactValues.Clear();
    this.m_currentFilterMinValue = this.m_currentFilterMaxValue = new int?();
    this.m_currentFilterIsEven = !isOdd;
    this.m_currentFilterIsOdd = isOdd;
    this.UpdateFilterStates();
  }

  public void SetUpTabs()
  {
    for (int index = 0; index <= 6; ++index)
      this.CreateNewTab(this.m_singleManaFilterPrefab, index);
    this.CreateNewTab(this.m_dynamicManaFilterPrefab, 7);
    this.m_manaCrystalContainer.UpdateSlices();
  }

  public void ActivateTabs(bool active)
  {
    this.m_tabsActive = active;
    this.UpdateFilterStates();
    if (!active)
      return;
    this.m_manaCrystalContainer.UpdateSlices();
  }

  public bool Enabled
  {
    get
    {
      foreach (PegUIElement tab in this.m_tabs)
      {
        if (!tab.IsEnabled())
          return false;
      }
      return true;
    }
    set
    {
      foreach (ManaFilterTab tab in this.m_tabs)
      {
        tab.SetEnabled(value);
        ManaFilterTab.FilterState state = ManaFilterTab.FilterState.DISABLED;
        if (tab.IsEnabled() && this.m_tabsActive)
          state = this.GetTabFilterState(tab.GetManaID());
        tab.SetFilterState(state);
        if ((UnityEngine.Object) tab.m_costText != (UnityEngine.Object) null)
          tab.m_costText.gameObject.SetActive(value);
      }
    }
  }

  private void CreateNewTab(ManaFilterTab tabPrefab, int index)
  {
    ManaFilterTab manaFilterTab = (ManaFilterTab) GameUtils.Instantiate((Component) tabPrefab, this.m_manaCrystalContainer.gameObject);
    manaFilterTab.SetManaID(index);
    manaFilterTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTabPressed));
    manaFilterTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabMousedOver));
    manaFilterTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabMousedOut));
    manaFilterTab.SetFilterState(ManaFilterTab.FilterState.DISABLED);
    if (UniversalInputManager.Get().IsTouchMode())
      manaFilterTab.SetReceiveReleaseWithoutMouseDown(true);
    this.m_tabs.Add(manaFilterTab);
    this.m_manaCrystalContainer.AddSlice(manaFilterTab.gameObject);
  }

  private void OnTabPressed(UIEvent e)
  {
    if (!this.m_tabsActive)
      return;
    ManaFilterTab element = (ManaFilterTab) e.GetElement();
    if (!(bool) UniversalInputManager.UsePhoneUI && !Options.Get().GetBool(Option.HAS_CLICKED_MANA_TAB, false) && UserAttentionManager.CanShowAttentionGrabber("ManaFilterTabManager.OnTabPressed:" + (object) Option.HAS_CLICKED_MANA_TAB))
    {
      Options.Get().SetBool(Option.HAS_CLICKED_MANA_TAB, true);
      this.ShowManaTabHint(element);
    }
    if (this.IsManaValueActive(element.GetManaID()))
    {
      TelemetryManager.Client().SendManaFilterToggleOff();
      this.UpdateCurrentFilterToSingleValue(-1);
    }
    else
      this.UpdateCurrentFilterToSingleValue(element.GetManaID());
  }

  private void OnTabMousedOver(UIEvent e)
  {
    if (!this.m_tabsActive)
      return;
    ((ManaFilterTab) e.GetElement()).NotifyMousedOver();
  }

  private void OnTabMousedOut(UIEvent e)
  {
    if (!this.m_tabsActive)
      return;
    ((ManaFilterTab) e.GetElement()).NotifyMousedOut();
  }

  private ManaFilterTab.FilterState GetTabFilterState(int manaValue) => !this.IsManaValueActive(manaValue) ? ManaFilterTab.FilterState.OFF : ManaFilterTab.FilterState.ON;

  private void UpdateCurrentFilterToSingleValue(int manaValue, bool transitionPage = true)
  {
    int num = this.m_currentFilterIsEven || this.m_currentFilterIsOdd || this.m_currentFilterExactValues.Count != 1 ? 1 : (!this.m_currentFilterExactValues.Contains(manaValue) ? 1 : 0);
    if (num != 0)
      SoundManager.Get().LoadAndPlay((AssetReference) "mana_crystal_refresh.prefab:ea5c456dd852f904e9828db66636f54d");
    this.m_currentFilterExactValues.Clear();
    if (manaValue != -1)
      this.m_currentFilterExactValues.Add(manaValue);
    this.m_currentFilterMinValue = this.m_currentFilterMaxValue = new int?();
    this.m_currentFilterIsEven = false;
    this.m_currentFilterIsOdd = false;
    this.UpdateFilterStates();
    if (num == 0)
      return;
    if (manaValue == -1)
    {
      if (this.OnFilterCleared == null)
        return;
      this.OnFilterCleared(transitionPage);
    }
    else
    {
      if (this.OnManaValueActivated == null)
        return;
      this.OnManaValueActivated(manaValue, transitionPage);
    }
  }

  private void UpdateFilterStates()
  {
    foreach (ManaFilterTab tab in this.m_tabs)
    {
      ManaFilterTab.FilterState state = ManaFilterTab.FilterState.DISABLED;
      if (tab.IsEnabled() && this.m_tabsActive)
        state = this.GetTabFilterState(tab.GetManaID());
      tab.SetFilterState(state);
    }
  }

  private void ShowManaTabHint(ManaFilterTab tabButton)
  {
    Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tabButton.transform.position + new Vector3(0.0f, 0.0f, 7f), TutorialEntity.GetTextScale(), GameStrings.Get("GLUE_COLLECTION_MANAGER_MANA_TAB_FIRST_CLICK"));
    if ((UnityEngine.Object) popupText == (UnityEngine.Object) null)
      return;
    popupText.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
    NotificationManager.Get().DestroyNotification(popupText, 3f);
  }
}
