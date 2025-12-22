using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class StaticWidgetCacheLender : MonoBehaviour
{
  [SerializeField]
  private GameObject m_handlerObject;
  [SerializeField]
  private GameLayer m_layerOverride;
  public const string REQUEST_WIDGET = "REQUEST_WIDGET";
  public const string RETURN_WIDGETS = "RETURN_WIDGETS";
  private Widget m_widget;
  private IStaticWidgetCacheOwner m_cacheOwner;
  private string m_dataModelUniqueId;

  private StaticWidgetCacheBase Cache => this.m_cacheOwner.Cache;

  private void Awake()
  {
    this.m_widget = this.GetComponent<Widget>();
    if ((Object) this.m_widget == (Object) null)
      return;
    this.m_cacheOwner = this.GetComponentInParent<IStaticWidgetCacheOwner>();
    if (this.m_cacheOwner == null)
      return;
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleEvent));
  }

  private void OnDestroy() => this.ReturnWidgets();

  private void RequestWidget(IDataModel dataModel)
  {
    if (this.m_cacheOwner == null || dataModel == null)
      return;
    if (!string.IsNullOrEmpty(this.m_dataModelUniqueId))
    {
      if (this.m_dataModelUniqueId == this.Cache.GetUniqueIdentifier(dataModel))
        return;
      this.ReturnWidgets();
    }
    this.Cache.RequestWidget(this, dataModel, this.m_handlerObject, this.m_layerOverride);
    this.m_dataModelUniqueId = this.Cache.GetUniqueIdentifier(dataModel);
  }

  private void ReturnWidgets()
  {
    if (this.m_cacheOwner == null || string.IsNullOrEmpty(this.m_dataModelUniqueId))
      return;
    this.Cache.ReturnWidgets(this);
    this.m_dataModelUniqueId = (string) null;
  }

  private void HandleEvent(string eventName)
  {
    if (eventName == "REQUEST_WIDGET")
    {
      EventDataModel dataModel = this.m_widget.GetDataModel<EventDataModel>();
      if (dataModel == null || dataModel.Payload == null || !(dataModel.Payload is IDataModel payload))
        return;
      this.RequestWidget(payload);
    }
    else
    {
      if (!(eventName == "RETURN_WIDGETS"))
        return;
      this.ReturnWidgets();
    }
  }
}
