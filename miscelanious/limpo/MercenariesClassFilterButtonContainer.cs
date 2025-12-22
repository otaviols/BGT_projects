using Hearthstone.UI;
using System;
using UnityEngine;

public class MercenariesClassFilterButtonContainer : MonoBehaviour
{
  public AsyncReference m_campfireButton;
  private Widget m_campfireButtonWidget;
  public SlidingTray m_slidingTray;
  public MercenariesClassFilterButton[] m_filterButtons;
  private bool m_campfireButtonClicked;
  public static readonly string CAMPFIRE_BUTTON_CLICKED = "Campfire_Button_Clicked";

  public void Start()
  {
    this.m_campfireButton.RegisterReadyListener<Widget>((Action<Widget>) (w => this.OnCampfireButtonReady(w)));
    PegUI.Get().RegisterForRenderPassPriorityHitTest((Component) this);
    this.m_slidingTray.OnTransitionComplete += new Action(this.OnSlidingTrayTransitionCompleted);
  }

  public void OnDestroy()
  {
    this.m_slidingTray.OnTransitionComplete -= new Action(this.OnSlidingTrayTransitionCompleted);
    PegUI.Get().UnregisterFromRenderPassPriorityHitTest((Component) this);
  }

  private void OnCampfireButtonReady(Widget w) => w.RegisterEventListener(new Widget.EventListenerDelegate(this.OnCampfireButtonEvent));

  private void OnCampfireButtonEvent(string eventName)
  {
    if (!(eventName == MercenariesClassFilterButtonContainer.CAMPFIRE_BUTTON_CLICKED))
      return;
    this.m_slidingTray.HideTray();
    this.m_campfireButtonClicked = true;
  }

  public void UpdateRoleButtons()
  {
    LettuceCollectionPageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as LettuceCollectionPageManager;
    foreach (MercenariesClassFilterButton filterButton in this.m_filterButtons)
    {
      filterButton.SetNewCardCount(0);
      if (pageManager.HasRoleCardsAvailable(filterButton.Role))
      {
        int numNewCardsForRole = pageManager.GetNumNewCardsForRole(filterButton.Role);
        filterButton.SetNewCardCount(numNewCardsForRole);
      }
    }
  }

  private void OnSlidingTrayTransitionCompleted()
  {
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if (!this.m_slidingTray.IsShown())
    {
      if (!this.m_campfireButtonClicked)
        collectibleDisplay.TryShowCollectionTips();
    }
    else
      collectibleDisplay.HideAllTips();
    this.m_campfireButtonClicked = false;
  }
}
