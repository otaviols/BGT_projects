using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollLoader : MonoBehaviour
{
  [SerializeField]
  private AsyncReference m_scrollableReference;
  private UIBScrollable m_scrollable;
  private bool m_isScrollableReady;
  [SerializeField]
  private AsyncReference m_listReference;
  private Listable m_list;
  private bool m_isListReady;
  [SerializeField]
  [Min(0.0f)]
  private float m_itemHeight;
  [Min(0.0f)]
  [SerializeField]
  private int m_itemBuffer;
  [SerializeField]
  private string m_showItemEvent;
  [SerializeField]
  private string m_hideItemEvent;
  private bool m_isReady;
  private bool m_isPaused;
  private bool m_hasChangedStatesOnce;
  private bool _isUpdatingData;
  private Dictionary<string, WidgetInstance> m_visibleAffectedObjects = new Dictionary<string, WidgetInstance>();
  private List<Action> m_startChangingStatesListeners = new List<Action>();
  private List<Action> m_doneChangingStatesListeners = new List<Action>();
  private List<Action<bool>> m_onPausedListeners = new List<Action<bool>>();
  private Coroutine m_waitUntilReadyCoroutine;
  private Coroutine m_refreshDelayCoroutine;

  public bool IsChangingState { get; private set; }

  private bool IsUpdatingData
  {
    get => this._isUpdatingData;
    set
    {
      this._isUpdatingData = value;
      this.RefreshIsChangingState();
    }
  }

  private void Awake()
  {
    this.m_scrollableReference.RegisterReadyListener<UIBScrollable>(new Action<UIBScrollable>(this.OnScrollableReady));
    this.m_listReference.RegisterReadyListener<Listable>(new Action<Listable>(this.OnListReady));
    this.m_waitUntilReadyCoroutine = this.StartCoroutine(this.WaitUntilReady());
  }

  private void OnDestroy()
  {
    if (this.m_waitUntilReadyCoroutine == null)
      return;
    this.StopCoroutine(this.m_waitUntilReadyCoroutine);
    this.m_waitUntilReadyCoroutine = (Coroutine) null;
  }

  private IEnumerator WaitUntilReady()
  {
    ScrollLoader scrollLoader = this;
    while (!scrollLoader.m_isScrollableReady)
      yield return (object) null;
    while (!scrollLoader.m_isListReady)
      yield return (object) null;
    scrollLoader.m_isReady = true;
    scrollLoader.m_list.RegisterDataChangedListener(new Action(scrollLoader.OnListDataChanged));
    scrollLoader.m_list.RegisterDoneChangingStatesListener(new Action<object>(scrollLoader.OnListDoneChangingStates), (object) null, true, false);
    scrollLoader.RefreshVisibleAffectedObjects();
    scrollLoader.m_waitUntilReadyCoroutine = (Coroutine) null;
  }

  public void Pause(bool isPaused)
  {
    bool isPaused1 = this.m_isPaused;
    this.m_isPaused = isPaused;
    if (!isPaused & isPaused1)
    {
      this.RefreshCurrentVisibleStates();
      foreach (Action<bool> onPausedListener in this.m_onPausedListeners)
      {
        if (onPausedListener != null)
          onPausedListener(false);
      }
    }
    else
    {
      if (!isPaused || isPaused1)
        return;
      foreach (Action<bool> onPausedListener in this.m_onPausedListeners)
      {
        if (onPausedListener != null)
          onPausedListener(true);
      }
    }
  }

  private void RefreshVisibleAffectedObjects()
  {
    if (!this.m_isReady)
      return;
    this.m_scrollable.ClearVisibleAffectObjects();
    this.m_visibleAffectedObjects.Clear();
    if (this.m_list.WidgetItemsCount <= 0)
      return;
    Vector3 extents = new Vector3(0.0f, 0.0f, this.m_itemHeight + this.m_itemHeight * (float) this.m_itemBuffer);
    foreach (WidgetInstance widgetItem in this.m_list.WidgetItems)
    {
      this.m_scrollable.AddVisibleAffectedObject(widgetItem.gameObject, extents, this.m_scrollable.IsObjectVisibleInScrollArea(widgetItem.gameObject, extents), new UIBScrollable.VisibleAffected(this.UpdateVisibleState));
      this.m_visibleAffectedObjects.Add(widgetItem.gameObject.name, widgetItem);
    }
  }

  private void RefreshCurrentVisibleStates()
  {
    if (!this.m_isReady || this.m_isPaused)
      return;
    foreach (UIBScrollable.VisibleAffectedObject visibleAffectedObject in this.m_scrollable.GetVisibleAffectedObjects())
      this.UpdateVisibleState(visibleAffectedObject.Obj, visibleAffectedObject.Visible);
  }

  private void UpdateVisibleState(GameObject obj, bool visible)
  {
    WidgetInstance widgetInstance;
    if (!this.m_isReady || this.m_isPaused || !this.m_visibleAffectedObjects.TryGetValue(obj.name, out widgetInstance))
      return;
    if (visible)
      widgetInstance.TriggerEvent(this.m_showItemEvent, new Widget.TriggerEventParameters());
    else
      widgetInstance.TriggerEvent(this.m_hideItemEvent, new Widget.TriggerEventParameters());
  }

  private void OnListDataChanged()
  {
    this.IsUpdatingData = true;
    this.m_scrollable.SetScrollImmediate(0.0f);
    this.RefreshVisibleAffectedObjects();
    this.m_list.UpdatePositions();
  }

  private void OnListDoneChangingStates(object _)
  {
    if (!this.IsUpdatingData)
      return;
    this.IsUpdatingData = false;
    this.RefreshCurrentVisibleStates();
  }

  private void OnScrollableReady(UIBScrollable scrollable)
  {
    this.m_isScrollableReady = true;
    this.m_scrollable = scrollable;
  }

  private void OnListReady(Listable list)
  {
    this.m_isListReady = true;
    this.m_list = list;
  }

  public void RefreshIsChangingState()
  {
    int num1 = this.IsChangingState ? 1 : 0;
    this.IsChangingState = this.IsUpdatingData;
    int num2 = this.IsChangingState ? 1 : 0;
    if (num1 == num2 && !this.m_hasChangedStatesOnce)
      return;
    this.m_hasChangedStatesOnce = true;
    if (this.m_refreshDelayCoroutine != null)
      this.StopCoroutine(this.m_refreshDelayCoroutine);
    if (this.gameObject.activeInHierarchy)
      this.m_refreshDelayCoroutine = this.StartCoroutine(this.RefreshDelay());
    else if (this.IsChangingState)
      this.OnStartChangingStates();
    else
      this.OnDoneChangingStates();
  }

  private IEnumerator RefreshDelay()
  {
    if (this.IsChangingState)
    {
      this.OnStartChangingStates();
    }
    else
    {
      for (int frameCount = 2; frameCount > 0; --frameCount)
      {
        this.m_list.UpdatePositions();
        yield return (object) null;
      }
      this.OnDoneChangingStates();
    }
    this.m_refreshDelayCoroutine = (Coroutine) null;
  }

  private void OnStartChangingStates()
  {
    foreach (Action changingStatesListener in this.m_startChangingStatesListeners)
    {
      if (changingStatesListener != null)
        changingStatesListener();
    }
  }

  private void OnDoneChangingStates()
  {
    foreach (Action changingStatesListener in this.m_doneChangingStatesListeners)
    {
      if (changingStatesListener != null)
        changingStatesListener();
    }
    this.RefreshCurrentVisibleStates();
  }

  public void RegisterStartChangingState(Action del) => this.m_startChangingStatesListeners.Add(del);

  public void UnregisterStartChangingState(Action del) => this.m_startChangingStatesListeners.Remove(del);

  public void RegisterDoneChangingState(Action del) => this.m_doneChangingStatesListeners.Add(del);

  public void UnregisterDoneChangingState(Action del) => this.m_doneChangingStatesListeners.Remove(del);

  public void RegisterOnPausedChanged(Action<bool> del) => this.m_onPausedListeners.Add(del);

  public void UnregisterOnPausedChanged(Action<bool> del) => this.m_onPausedListeners.Remove(del);
}
