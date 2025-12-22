using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelsPlayMat : MonoBehaviour
{
  public AsyncReference m_livesReference;
  public AsyncReference m_vaultReference;
  public AsyncReference m_vaultLeverReference;
  private Clickable m_leverButton;
  private Widget m_livesWidget;
  private Widget m_vaultWidget;
  private bool m_livesWidgetLoaded;
  private bool m_vaultWidgetLoaded;
  private List<Action> m_vaultDoorOpenedListeners;
  private List<Action> m_vaultDoorClickedListeners;

  public void Start()
  {
    this.m_livesReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnLivesWidgetReady));
    this.m_vaultReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnVaultWidgetReady));
    this.m_vaultLeverReference.RegisterReadyListener<Clickable>(new Action<Clickable>(this.OnLeverClickableReady));
    this.m_vaultDoorOpenedListeners = new List<Action>();
    this.m_vaultDoorClickedListeners = new List<Action>();
  }

  public bool IsReady() => this.m_livesWidgetLoaded && this.m_vaultWidgetLoaded;

  public void SetLeverButtonEnabled(bool enabled)
  {
    this.m_leverButton.enabled = enabled;
    if (!enabled)
      return;
    this.m_leverButton.GetComponent<VisualController>().SetState(DuelsConfig.LEVER_GLOW_STATE);
  }

  public void RegisterVaultDoorOpenedListener(Action a)
  {
    if (this.m_vaultDoorOpenedListeners.Contains(a))
      return;
    this.m_vaultDoorOpenedListeners.Add(a);
  }

  public void RemoveVaultDoorOpenedListener(Action a) => this.m_vaultDoorOpenedListeners.Remove(a);

  public void OnVaultDoorOpened()
  {
    for (int index = 0; index < this.m_vaultDoorOpenedListeners.Count; ++index)
      this.m_vaultDoorOpenedListeners[index]();
  }

  public void RegisterVaultDoorClickedListener(Action a)
  {
    if (this.m_vaultDoorClickedListeners.Contains(a))
      return;
    this.m_vaultDoorClickedListeners.Add(a);
  }

  public void RemoveVaultDoorClickedListener(Action a) => this.m_vaultDoorClickedListeners.Remove(a);

  public void OnVaultDoorClicked()
  {
    for (int index = 0; index < this.m_vaultDoorClickedListeners.Count; ++index)
      this.m_vaultDoorClickedListeners[index]();
  }

  private void OnLivesWidgetReady(Widget w)
  {
    this.m_livesWidget = w;
    this.m_livesWidget.RegisterDoneChangingStatesListener(new Action<object>(this.OnLivesWidgetDoneChangingStates), (object) null, true, false);
  }

  private void OnLivesWidgetDoneChangingStates(object obj)
  {
    this.m_livesWidgetLoaded = true;
    this.m_livesWidget.RemoveStartChangingStatesListener(new Action<object>(this.OnLivesWidgetDoneChangingStates));
  }

  private void OnVaultWidgetReady(Widget w)
  {
    this.m_vaultWidget = w;
    this.m_vaultWidget.RegisterDoneChangingStatesListener(new Action<object>(this.OnLivesWidgetDoneChangingStates), (object) null, true, false);
    this.m_vaultWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnVaultEvent));
  }

  private void OnVaultWidgetDoneChangingStates(object obj)
  {
    this.m_vaultWidgetLoaded = true;
    this.m_vaultWidget.RemoveStartChangingStatesListener(new Action<object>(this.OnLivesWidgetDoneChangingStates));
  }

  private void OnVaultEvent(string eventName)
  {
    if (eventName == DuelsConfig.DOOR_OPENED_EVENT)
    {
      this.OnVaultDoorOpened();
    }
    else
    {
      if (!(eventName == DuelsConfig.DOOR_LEVEL_CLICKED))
        return;
      this.OnVaultDoorClicked();
    }
  }

  private void OnLeverClickableReady(Clickable c) => this.m_leverButton = c;
}
