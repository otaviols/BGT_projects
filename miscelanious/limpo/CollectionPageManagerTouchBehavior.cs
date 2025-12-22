using Blizzard.T5.Services;
using System.Collections;
using UnityEngine;

public class CollectionPageManagerTouchBehavior : PegUICustomBehavior
{
  private float TurnDist = 0.07f;
  private PegUIElement m_pageLeftRegion;
  private PegUIElement m_pageRightRegion;
  private PegUIElement m_pageDragRegion;
  private CollectionPageManagerTouchBehavior.SwipeState m_swipeState;
  private Vector2 m_swipeStartPosition;

  protected override void Awake()
  {
    base.Awake();
    BookPageManager component = this.GetComponent<BookPageManager>();
    this.m_pageLeftRegion = component.m_pageLeftClickableRegion;
    this.m_pageRightRegion = component.m_pageRightClickableRegion;
    this.m_pageDragRegion = component.m_pageDraggableRegion;
    this.m_pageDragRegion.gameObject.SetActive(true);
    this.m_pageDragRegion.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnPageDraggableRegionDown));
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((Object) collectibleDisplay != (Object) null))
      return;
    collectibleDisplay.OnViewModeChanged += new CollectibleDisplay.ViewModeChangedListener(this.OnViewModeChanged);
  }

  protected override void OnDestroy()
  {
    this.m_pageDragRegion.gameObject.SetActive(false);
    this.m_pageDragRegion.RemoveEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnPageDraggableRegionDown));
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((Object) collectibleDisplay != (Object) null)
      collectibleDisplay.OnViewModeChanged -= new CollectibleDisplay.ViewModeChangedListener(this.OnViewModeChanged);
    base.OnDestroy();
  }

  public override bool UpdateUI()
  {
    if ((Object) CollectionInputMgr.Get() != (Object) null && CollectionInputMgr.Get().HasHeldCard() || (Object) CraftingManager.Get() != (Object) null && CraftingManager.Get().IsCardShowing())
      return false;
    bool flag = false;
    if (InputCollection.GetMouseButtonUp(0))
    {
      flag = this.m_swipeState == CollectionPageManagerTouchBehavior.SwipeState.Success;
      this.m_swipeState = CollectionPageManagerTouchBehavior.SwipeState.None;
    }
    return this.m_swipeState != CollectionPageManagerTouchBehavior.SwipeState.None || flag;
  }

  protected void OnViewModeChanged(
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode mode,
    CollectionUtils.ViewModeData userdata,
    bool triggerResponse)
  {
    this.m_pageDragRegion.gameObject.SetActive(mode != CollectionUtils.ViewMode.HERO_PICKER && mode != CollectionUtils.ViewMode.DECK_TEMPLATE && mode != CollectionUtils.ViewMode.MASS_DISENCHANT);
  }

  private void OnPageDraggableRegionDown(UIEvent e)
  {
    if ((Object) this.gameObject == (Object) null)
      return;
    this.TryStartPageTurnGesture();
  }

  private void TryStartPageTurnGesture()
  {
    if (this.m_swipeState == CollectionPageManagerTouchBehavior.SwipeState.Update)
      return;
    this.StartCoroutine(this.HandlePageTurnGesture());
  }

  private Vector2 GetTouchPosition()
  {
    Vector3 touchPosition = ServiceManager.Get<ITouchScreenService>().GetTouchPosition();
    return new Vector2(touchPosition.x, touchPosition.y);
  }

  private IEnumerator HandlePageTurnGesture()
  {
    if (!UniversalInputManager.Get().IsTouchMode())
      yield return (object) null;
    this.m_swipeStartPosition = this.GetTouchPosition();
    this.m_swipeState = CollectionPageManagerTouchBehavior.SwipeState.Update;
    float pixelTurnDist = Mathf.Clamp(this.TurnDist * (float) Screen.currentResolution.width, 2f, 300f);
    PegUIElement touchDownPageTurnRegion = this.HitTestPageTurnRegions();
    while (!InputCollection.GetMouseButtonUp(0))
    {
      float x = (this.GetTouchPosition() - this.m_swipeStartPosition).x;
      if ((double) x <= -(double) pixelTurnDist && this.m_pageRightRegion.enabled)
      {
        this.m_pageRightRegion.TriggerRelease();
        this.m_swipeState = CollectionPageManagerTouchBehavior.SwipeState.Success;
        yield break;
      }
      else if ((double) x >= (double) pixelTurnDist && this.m_pageLeftRegion.enabled)
      {
        this.m_pageLeftRegion.TriggerRelease();
        this.m_swipeState = CollectionPageManagerTouchBehavior.SwipeState.Success;
        yield break;
      }
      else
        yield return (object) null;
    }
    if ((Object) touchDownPageTurnRegion != (Object) null && (Object) touchDownPageTurnRegion == (Object) this.HitTestPageTurnRegions())
      touchDownPageTurnRegion.TriggerRelease();
    this.m_swipeState = CollectionPageManagerTouchBehavior.SwipeState.None;
  }

  private PegUIElement HitTestPageTurnRegions()
  {
    PegUIElement pegUiElement = (PegUIElement) null;
    Collider component = this.m_pageDragRegion.GetComponent<Collider>();
    component.enabled = false;
    RaycastHit hitInfo;
    if (UniversalInputManager.Get().GetInputHitInfo(out hitInfo))
    {
      pegUiElement = hitInfo.collider.GetComponent<PegUIElement>();
      if ((Object) pegUiElement != (Object) this.m_pageLeftRegion && (Object) pegUiElement != (Object) this.m_pageRightRegion)
        pegUiElement = (PegUIElement) null;
    }
    component.enabled = true;
    return pegUiElement;
  }

  private enum SwipeState
  {
    None,
    Update,
    Success,
  }
}
