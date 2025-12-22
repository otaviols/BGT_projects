using Hearthstone.UI;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticWidgetCache<T> : StaticWidgetCacheBase where T : class, IDataModel
{
  [SerializeField]
  private GameObject m_cacheHolder;
  [SerializeField]
  private WeakAssetReference m_cachedWidget;
  private Dictionary<string, List<StaticWidgetCache<T>.StaticWidgetData>> m_widgets = new Dictionary<string, List<StaticWidgetCache<T>.StaticWidgetData>>();
  private Dictionary<string, Stack<Widget>> m_freeWidgets = new Dictionary<string, Stack<Widget>>();
  private Dictionary<StaticWidgetCacheLender, List<Widget>> m_lenders = new Dictionary<StaticWidgetCacheLender, List<Widget>>();
  private Dictionary<StaticWidgetCacheLender, List<StaticWidgetCache<T>.RequestData>> m_lenderRequests = new Dictionary<StaticWidgetCacheLender, List<StaticWidgetCache<T>.RequestData>>();
  private bool m_isPaused;
  private HashSet<string> m_pauseRequestIds = new HashSet<string>();

  public override string GetUniqueIdentifier(IDataModel dataModelBase) => !(dataModelBase is T dataModel) ? (string) null : this.GetUniqueIdentifier(dataModel);

  public abstract string GetUniqueIdentifier(T dataModel);

  public override void RequestWidget(
    StaticWidgetCacheLender lender,
    IDataModel dataModelBase,
    GameObject handler = null,
    GameLayer overrideLayer = GameLayer.Default)
  {
    if (!(dataModelBase is T inputDataModel))
      return;
    T dataModel = inputDataModel.CloneDataModel<T>();
    if (this.m_isPaused)
    {
      List<StaticWidgetCache<T>.RequestData> requestDataList;
      if (!this.m_lenderRequests.TryGetValue(lender, out requestDataList))
      {
        requestDataList = new List<StaticWidgetCache<T>.RequestData>();
        this.m_lenderRequests.Add(lender, requestDataList);
      }
      bool flag = false;
      foreach (StaticWidgetCache<T>.RequestData requestData in requestDataList)
      {
        if (this.GetUniqueIdentifier(requestData.requestedData) == this.GetUniqueIdentifier(dataModel))
          flag = true;
      }
      if (flag)
        return;
      requestDataList.Add(new StaticWidgetCache<T>.RequestData()
      {
        requestedData = dataModel,
        handler = handler,
        overrideLayer = overrideLayer
      });
    }
    else
    {
      List<Widget> widgetList;
      if (!this.m_lenders.TryGetValue(lender, out widgetList))
      {
        widgetList = new List<Widget>();
        this.m_lenders.Add(lender, widgetList);
      }
      string uniqueIdentifier = this.GetUniqueIdentifier(dataModel);
      Stack<Widget> widgetStack;
      Widget widget = !this.m_freeWidgets.TryGetValue(uniqueIdentifier, out widgetStack) || widgetStack.Count <= 0 ? (Widget) this.GetNewWidgetInstance(dataModel) : widgetStack.Pop();
      if ((Object) widget == (Object) null)
        return;
      this.UpdateWidgetList(uniqueIdentifier, new StaticWidgetCache<T>.StaticWidgetData()
      {
        widget = widget,
        currentOwner = lender
      });
      widgetList.Add(widget);
      widget.Show();
      GameUtils.SetParent(widget.gameObject, (Object) handler != (Object) null ? handler.gameObject : lender.gameObject, true);
      widget.SetLayerOverride(overrideLayer);
      widget.transform.localPosition = Vector3.zero;
    }
  }

  public override void ReturnWidgets(StaticWidgetCacheLender lender)
  {
    if (this.m_isPaused)
      this.m_lenderRequests.Remove(lender);
    List<Widget> widgetList;
    if (!this.m_lenders.TryGetValue(lender, out widgetList))
      return;
    foreach (WidgetInstance widgetInstance in widgetList)
    {
      T dataModel = widgetInstance.GetDataModel<T>();
      if ((object) dataModel == null)
        return;
      string uniqueIdentifier = this.GetUniqueIdentifier(dataModel);
      Stack<Widget> widgetStack;
      if (!this.m_freeWidgets.TryGetValue(uniqueIdentifier, out widgetStack))
      {
        widgetStack = new Stack<Widget>();
        this.m_freeWidgets[uniqueIdentifier] = widgetStack;
      }
      this.UpdateWidgetList(uniqueIdentifier, new StaticWidgetCache<T>.StaticWidgetData()
      {
        widget = (Widget) widgetInstance,
        currentOwner = (StaticWidgetCacheLender) null
      });
      widgetStack.Push((Widget) widgetInstance);
      GameUtils.SetParent(widgetInstance.gameObject, this.m_cacheHolder, true);
      widgetInstance.Hide();
    }
    widgetList.Clear();
  }

  public override void Preload(IEnumerable<IDataModel> dataModels, bool createNew = false)
  {
    using (IEnumerator<IDataModel> enumerator = dataModels.GetEnumerator())
    {
      while (enumerator.MoveNext() && enumerator.Current is T current)
      {
        string uniqueIdentifier = this.GetUniqueIdentifier(current);
        List<StaticWidgetCache<T>.StaticWidgetData> staticWidgetDataList;
        if (createNew || !this.m_widgets.TryGetValue(uniqueIdentifier, out staticWidgetDataList) || staticWidgetDataList.Count <= 0)
        {
          Stack<Widget> widgetStack;
          if (!this.m_freeWidgets.TryGetValue(uniqueIdentifier, out widgetStack))
          {
            widgetStack = new Stack<Widget>();
            this.m_freeWidgets[uniqueIdentifier] = widgetStack;
          }
          WidgetInstance newWidgetInstance = this.GetNewWidgetInstance(current);
          this.UpdateWidgetList(uniqueIdentifier, new StaticWidgetCache<T>.StaticWidgetData()
          {
            widget = (Widget) newWidgetInstance,
            currentOwner = (StaticWidgetCacheLender) null
          });
          widgetStack.Push((Widget) newWidgetInstance);
          GameUtils.SetParent(newWidgetInstance.gameObject, this.m_cacheHolder, true);
          newWidgetInstance.Hide();
        }
      }
    }
  }

  public override void Rebind(IDataModel dataModelBase)
  {
    List<StaticWidgetCache<T>.StaticWidgetData> staticWidgetDataList;
    if (!(dataModelBase is T dataModel) || !this.m_widgets.TryGetValue(this.GetUniqueIdentifier(dataModel), out staticWidgetDataList))
      return;
    foreach (StaticWidgetCache<T>.StaticWidgetData staticWidgetData in staticWidgetDataList)
    {
      if ((Object) staticWidgetData.widget != (Object) null)
        staticWidgetData.widget.BindDataModel((IDataModel) dataModel);
    }
  }

  public override void Pause(bool pause, string pauseRequestId)
  {
    if (pause)
      this.m_pauseRequestIds.Add(pauseRequestId);
    else
      this.m_pauseRequestIds.Remove(pauseRequestId);
    int num = this.m_isPaused ? 1 : 0;
    this.m_isPaused = this.m_pauseRequestIds.Count > 0;
    if (num == 0 || this.m_isPaused)
      return;
    foreach (KeyValuePair<StaticWidgetCacheLender, List<StaticWidgetCache<T>.RequestData>> lenderRequest in this.m_lenderRequests)
    {
      if (!((Object) lenderRequest.Key == (Object) null))
      {
        foreach (StaticWidgetCache<T>.RequestData requestData in lenderRequest.Value)
          this.RequestWidget(lenderRequest.Key, (IDataModel) requestData.requestedData, requestData.handler, requestData.overrideLayer);
      }
    }
    this.m_lenderRequests.Clear();
  }

  private WidgetInstance GetNewWidgetInstance(T dataModel)
  {
    WidgetInstance newWidgetInstance = WidgetInstance.Create(this.m_cachedWidget.AssetString);
    newWidgetInstance.BindDataModel((IDataModel) dataModel, false);
    return newWidgetInstance;
  }

  private void UpdateWidgetList(string uniqueId, StaticWidgetCache<T>.StaticWidgetData updateData)
  {
    List<StaticWidgetCache<T>.StaticWidgetData> staticWidgetDataList;
    if (this.m_widgets.TryGetValue(uniqueId, out staticWidgetDataList))
    {
      bool flag = false;
      for (int index = 0; index < staticWidgetDataList.Count; ++index)
      {
        if ((Object) staticWidgetDataList[index].widget == (Object) updateData.widget)
        {
          staticWidgetDataList[index] = updateData;
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      staticWidgetDataList.Add(updateData);
    }
    else
      this.m_widgets.Add(uniqueId, new List<StaticWidgetCache<T>.StaticWidgetData>()
      {
        updateData
      });
  }

  private struct StaticWidgetData
  {
    public Widget widget;
    public StaticWidgetCacheLender currentOwner;
  }

  private struct RequestData
  {
    public T requestedData;
    public GameObject handler;
    public GameLayer overrideLayer;
  }
}
