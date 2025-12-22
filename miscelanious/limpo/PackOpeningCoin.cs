using Hearthstone.UI;
using System;
using UnityEngine;

public class PackOpeningCoin : MonoBehaviour
{
  public GameObject m_root;
  public AsyncReference m_HiddenCoinWidgetReference;
  private Spell.FinishedCallback m_finishedCallback;
  private Spell.StateFinishedCallback m_stateFinishedCallback;

  private void Start() => this.m_HiddenCoinWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnHiddenCoinWidgetReady));

  private void OnHiddenCoinWidgetReady(Widget widget) => this.RecursiveSetVisibility(widget.gameObject, false);

  private void RecursiveSetVisibility(GameObject go, bool visible)
  {
    Renderer component1 = go.GetComponent<Renderer>();
    if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
      component1.enabled = visible;
    foreach (Component component2 in go.transform)
      this.RecursiveSetVisibility(component2.gameObject, visible);
  }

  public void ActivateDeathVisuals(
    Spell.FinishedCallback finishedCallback,
    Spell.StateFinishedCallback stateFinishedCallback)
  {
    this.m_finishedCallback = finishedCallback;
    this.m_stateFinishedCallback = stateFinishedCallback;
    SendEventUpwardStateAction.SendEventUpward(this.gameObject, "FADE_OUT_COIN");
  }

  public void OnDeathVisualsFadedIn()
  {
    this.m_finishedCallback((Spell) null, (object) null);
    this.m_finishedCallback = (Spell.FinishedCallback) null;
  }

  public void OnDeathVisualsFinished()
  {
    this.m_stateFinishedCallback((Spell) null, SpellStateType.NONE, (object) null);
    this.m_stateFinishedCallback = (Spell.StateFinishedCallback) null;
  }

  public void SetActive(bool active) => this.m_root.SetActive(active);
}
