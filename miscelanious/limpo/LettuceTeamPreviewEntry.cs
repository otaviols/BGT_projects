using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class LettuceTeamPreviewEntry : MonoBehaviour
{
  public Widget m_teamWidget;
  public AsyncReference m_mercenaryReference;
  private VisualController m_mercVisualController;

  private void Start()
  {
    this.m_teamWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.MercenaryEventListener));
    this.m_mercenaryReference.RegisterReadyListener<VisualController>((Action<VisualController>) (vs => this.m_mercVisualController = vs));
  }

  private void OnDestroy() => this.m_teamWidget.RemoveEventListener(new Widget.EventListenerDelegate(this.MercenaryEventListener));

  private void MercenaryEventListener(string eventName)
  {
    if (!(eventName == "MERC_LOADOUT_RELEASED"))
      return;
    this.OpenMercDetailsScreen();
  }

  private void OpenMercDetailsScreen()
  {
    IMercDetailsDisplayProvider componentInParent = this.GetComponentInParent<IMercDetailsDisplayProvider>();
    if (componentInParent == null)
      Debug.LogError((object) "Unable to find IMercDetailsDisplayProvider in parents");
    else if (!(WidgetUtils.GetEventDataModel(this.m_mercVisualController).Payload is LettuceMercenaryDataModel payload))
    {
      Debug.LogError((object) "Unable to find LettuceMercenaryDataModel from latest Widget events");
    }
    else
    {
      LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) payload.MercenaryId);
      if (mercenary == null)
        Debug.LogError((object) string.Format("Unable to find mercenary with ID {0}", (object) payload.MercenaryId));
      else
        componentInParent.ShowMercDetailsDisplay(mercenary);
    }
  }
}
