using System;
using System.Collections.Generic;
using UnityEngine;

public class FiresideGatheringSign : MonoBehaviour
{
  public GameObject m_fxMotes;
  public Transform m_shieldContainer;
  private FiresideGatheringSignShield m_shield;
  private List<Action> m_signSocketAnimationCompleteListeners = new List<Action>();

  public event FiresideGatheringSign.OnDestroyCallback OnDestroyEvent;

  public void SetSignShield(FiresideGatheringSignShield shield) => this.m_shield = shield;

  public void SetSignShadowEnabled(bool enabled) => this.m_shield.m_ShieldShadow.SetActive(enabled);

  public MeshRenderer GetShieldMeshRenderer() => this.m_shield.m_ShieldMeshRenderer;

  private void OnDestroy()
  {
    if (this.OnDestroyEvent == null)
      return;
    this.OnDestroyEvent();
  }

  public void RegisterSignSocketAnimationCompleteListener(Action listener)
  {
    if (this.m_signSocketAnimationCompleteListeners.Contains(listener))
      return;
    this.m_signSocketAnimationCompleteListeners.Add(listener);
  }

  public void UnregisterSignSocketAnimationCompleteListener(Action listener)
  {
    if (!this.m_signSocketAnimationCompleteListeners.Contains(listener))
      return;
    this.m_signSocketAnimationCompleteListeners.Remove(listener);
  }

  public void FireSignSocketAnimationCompleteListener()
  {
    foreach (Action completeListener in this.m_signSocketAnimationCompleteListeners)
      completeListener();
  }

  public delegate void OnDestroyCallback();
}
