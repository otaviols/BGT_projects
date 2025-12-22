using Blizzard.T5.Services;
using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using UnityEngine;

public class UiTooltip : MonoBehaviour, IWidgetEventListener
{
  [SerializeField]
  private string m_title;
  [SerializeField]
  private string m_description;
  [SerializeField]
  private float m_scale = 0.5f;
  private TooltipZone m_tooltipZone;
  private WidgetTemplate m_widget;
  private IGameStringsService m_gameStringsService;
  private const string CODE_SHOW_TOOLTIP = "CODE_SHOW_TOOLTIP";
  private const string CODE_HIDE_TOOLTIP = "CODE_HIDE_TOOLTIP";

  [Overridable]
  public string Title
  {
    get => this.m_title;
    set => this.m_title = value;
  }

  [Overridable]
  public string Description
  {
    get => this.m_description;
    set => this.m_description = value;
  }

  [Overridable]
  public float Scale
  {
    get => this.m_scale;
    set => this.m_scale = value;
  }

  public WidgetTemplate OwningWidget => this.m_widget;

  private void Awake()
  {
    this.m_tooltipZone = this.GetComponent<TooltipZone>();
    this.m_widget = this.GetComponentInParent<WidgetTemplate>();
    this.m_widget.RegisterDeactivatedListener(new Action<object>(this.OnDeactivate), (object) null);
  }

  public WidgetEventListenerResponse EventReceived(string eventName)
  {
    WidgetEventListenerResponse listenerResponse = new WidgetEventListenerResponse();
    if (!(eventName == "CODE_SHOW_TOOLTIP"))
    {
      if (eventName == "CODE_HIDE_TOOLTIP")
      {
        this.HideTooltip();
        listenerResponse.Consumed = true;
      }
    }
    else
    {
      this.ShowTooltip();
      listenerResponse.Consumed = true;
    }
    return listenerResponse;
  }

  private void OnDeactivate(object unused) => this.HideTooltip();

  private void ShowTooltip()
  {
    if ((UnityEngine.Object) this.m_tooltipZone == (UnityEngine.Object) null)
      return;
    if (this.m_gameStringsService == null)
      this.m_gameStringsService = ServiceManager.Get<IGameStringsService>();
    this.m_tooltipZone.ShowLayerTooltip(this.m_gameStringsService.Get(this.m_title.Trim()), this.m_gameStringsService.Get(this.m_description.Trim()), this.m_scale);
  }

  private void HideTooltip()
  {
    if ((UnityEngine.Object) this.m_tooltipZone == (UnityEngine.Object) null)
      return;
    this.m_tooltipZone.HideTooltip();
  }
}
