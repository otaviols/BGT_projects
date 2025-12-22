using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TabbedBookPageManager : BookPageManager
{
  public GameObject m_tabContainer;
  public BookTab m_tabPrefab;
  public float m_spaceBetweenTabs;
  protected BookTab m_currentTab;
  protected List<BookTab> m_allTabs = new List<BookTab>();
  protected Map<BookTab, bool> m_tabVisibility = new Map<BookTab, bool>();
  protected bool m_tabsAreAnimating;

  protected override void Start()
  {
    this.SetUpBookTabs();
    base.Start();
  }

  public void UpdateVisibleTabs()
  {
    if (!SceneMgr.Get().IsInLettuceMode() && (bool) UniversalInputManager.UsePhoneUI)
      return;
    bool flag1 = false;
    foreach (BookTab allTab in this.m_allTabs)
    {
      int num1 = this.m_tabVisibility[allTab] ? 1 : 0;
      bool flag2 = this.ShouldShowTab(allTab);
      int num2 = flag2 ? 1 : 0;
      if (num1 != num2)
      {
        flag1 = true;
        this.m_tabVisibility[allTab] = flag2;
      }
    }
    if (!flag1)
      return;
    this.PositionBookTabs(true);
  }

  protected abstract bool ShouldShowTab(BookTab tab);

  protected abstract void SetUpBookTabs();

  protected abstract void PositionBookTabs(bool animate);

  protected void DeselectCurrentTab()
  {
    if ((Object) this.m_currentTab == (Object) null)
      return;
    this.m_currentTab.SetSelected(false);
    this.m_currentTab.SetLargeTab(false);
    this.m_currentTab = (BookTab) null;
  }

  protected void OnTabOver(UIEvent e)
  {
    BookTab element = e.GetElement() as BookTab;
    if ((Object) element == (Object) null)
      return;
    element.SetGlowActive(true);
  }

  protected void OnTabOut(UIEvent e)
  {
    BookTab element = e.GetElement() as BookTab;
    if ((Object) element == (Object) null)
      return;
    element.SetGlowActive(false);
  }

  protected void OnTabOver_Touch(UIEvent e)
  {
    if (!UniversalInputManager.Get().IsTouchMode())
      return;
    (e.GetElement() as BookTab).SetLargeTab(true);
  }

  protected void OnTabOut_Touch(UIEvent e)
  {
    if (!UniversalInputManager.Get().IsTouchMode())
      return;
    BookTab element = e.GetElement() as BookTab;
    if (!((Object) element != (Object) this.m_currentTab))
      return;
    element.SetLargeTab(false);
  }

  protected override void TransitionPage(object callbackData)
  {
    base.TransitionPage(callbackData);
    this.UpdateVisibleTabs();
  }

  protected override void HandleTouchModeChanged()
  {
    base.HandleTouchModeChanged();
    foreach (PegUIElement allTab in this.m_allTabs)
      allTab.SetReceiveReleaseWithoutMouseDown(UniversalInputManager.Get().IsTouchMode());
  }

  protected void PositionFixedTab(bool showTab, BookTab tab, Vector3 originalPos, bool animate)
  {
    if (!showTab)
      originalPos.z -= 0.5f;
    tab.SetTargetVisibility(showTab);
    tab.SetTargetLocalPosition(originalPos);
    if (animate)
    {
      tab.AnimateToTargetPosition(0.4f, iTween.EaseType.easeOutQuad);
    }
    else
    {
      tab.SetIsVisible(tab.ShouldBeVisible());
      tab.transform.localPosition = originalPos;
    }
  }

  protected IEnumerator SelectTabWhenReady(BookTab tab)
  {
    while (this.m_tabsAreAnimating)
      yield return (object) 0;
    if (!((Object) this.m_currentTab != (Object) tab))
    {
      tab.SetSelected(true);
      tab.SetLargeTab(true);
    }
  }
}
