using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

[RequireComponent(typeof (Widget))]
public class LettuceShowMercHoverCard : MonoBehaviour
{
  [CustomEditField(Sections = "Bones")]
  public Transform m_hoverCardTopBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_hoverCardBottomBone;
  public AsyncReference m_mercHoverReference;
  private Widget m_mercHoverCard;
  private Widget m_widget;

  private void Start()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.EventListener));
    this.m_mercHoverReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnMercHoverCardWidgetReady));
  }

  private void OnDestroy()
  {
    this.m_mercHoverCard.UnbindDataModel(216);
    this.m_widget.RemoveEventListener(new Widget.EventListenerDelegate(this.EventListener));
  }

  private void OnMercHoverCardWidgetReady(Widget widget)
  {
    this.m_mercHoverCard = widget;
    this.m_mercHoverCard.SetLayerOverride(GameLayer.IgnoreFullScreenEffects);
    this.HideMercHoverCard();
  }

  private void EventListener(string eventName)
  {
    if (!(eventName == "MERC_OVER_code"))
    {
      if (!(eventName == "MERC_OUT_code"))
        return;
      this.HideMercHoverCard();
    }
    else
    {
      if (!(this.m_widget.GetDataModel<EventDataModel>().Payload is LettuceMercenaryDataModel payload))
        return;
      this.ShowHoverCard((IDataModel) payload);
    }
  }

  private void ShowHoverCard(IDataModel dataModel)
  {
    this.m_mercHoverCard.BindDataModel(dataModel);
    float z = PegUI.Get().GetMousedOverElement().transform.position.z;
    float max = (UnityEngine.Object) this.m_hoverCardTopBone != (UnityEngine.Object) null ? this.m_hoverCardTopBone.position.z : this.transform.position.z;
    float min = (UnityEngine.Object) this.m_hoverCardBottomBone != (UnityEngine.Object) null ? this.m_hoverCardBottomBone.position.z : this.transform.position.z;
    TransformUtil.SetPosZ((Component) this.m_mercHoverCard.transform, Mathf.Clamp(z, min, max));
    this.m_mercHoverCard.gameObject.SetActive(true);
  }

  private void HideMercHoverCard() => this.m_mercHoverCard.gameObject.SetActive(false);
}
