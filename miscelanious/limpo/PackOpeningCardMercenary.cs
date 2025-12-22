using Hearthstone.UI;
using System;
using UnityEngine;

public class PackOpeningCardMercenary : MonoBehaviour
{
  public GameObject m_root;
  public Renderer[] m_CardBackRenderers;
  public AsyncReference m_mercenaryNameGlowReference;
  private ParticleSystem m_mercenaryNameGlow;
  private Spell.FinishedCallback m_finishedCallback;
  private Spell.StateFinishedCallback m_stateFinishedCallback;

  private void Start()
  {
    this.m_mercenaryNameGlowReference.RegisterReadyListener<ParticleSystem>(new Action<ParticleSystem>(this.OnMercenaryNameGlowReady));
    this.SetCardbackVisability(false);
  }

  private void SetCardbackVisability(bool visible)
  {
    foreach (Renderer cardBackRenderer in this.m_CardBackRenderers)
      cardBackRenderer.enabled = visible;
  }

  private void OnMercenaryNameGlowReady(ParticleSystem nameGlow)
  {
    if ((UnityEngine.Object) nameGlow == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "MercenaryNameGlowReference could not be found!");
    else
      this.m_mercenaryNameGlow = nameGlow;
  }

  public void ShowMercenaryNameGlow()
  {
    if ((UnityEngine.Object) this.m_mercenaryNameGlow == (UnityEngine.Object) null)
      return;
    this.m_mercenaryNameGlow.Play();
    LayerUtils.SetLayer(this.m_mercenaryNameGlow.gameObject, GameLayer.Default);
  }

  public void ActivateDeathVisuals(
    Spell.FinishedCallback finishedCallback,
    Spell.StateFinishedCallback stateFinishedCallback)
  {
    this.m_finishedCallback = finishedCallback;
    this.m_stateFinishedCallback = stateFinishedCallback;
    SendEventUpwardStateAction.SendEventUpward(this.gameObject, "FADE_OUT_MERC");
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
