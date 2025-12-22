using Hearthstone.UI;
using UnityEngine;

public class PackOpeningPortrait : MonoBehaviour
{
  public GameObject m_root;
  public Renderer[] m_CardBackRenderers;
  private Spell.FinishedCallback m_finishedCallback;
  private Spell.StateFinishedCallback m_stateFinishedCallback;

  private void Start() => this.SetCardbackVisability(false);

  private void SetCardbackVisability(bool visible)
  {
    foreach (Renderer cardBackRenderer in this.m_CardBackRenderers)
      cardBackRenderer.enabled = visible;
  }

  public void ActivateDeathVisuals(
    Spell.FinishedCallback finishedCallback,
    Spell.StateFinishedCallback stateFinishedCallback)
  {
    this.m_finishedCallback = finishedCallback;
    this.m_stateFinishedCallback = stateFinishedCallback;
    SendEventUpwardStateAction.SendEventUpward(this.gameObject, "FADE_OUT");
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
