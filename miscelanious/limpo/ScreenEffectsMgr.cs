using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffectsMgr : IService, IHasUpdate
{
  private Camera m_MainCamera;
  private ScreenEffectsRender m_ScreenEffectsRender;
  private bool m_enabled;
  private static List<ScreenEffect> m_ActiveScreenEffects;

  public bool IsActive => this.m_enabled;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    if (ScreenEffectsMgr.m_ActiveScreenEffects == null)
      ScreenEffectsMgr.m_ActiveScreenEffects = new List<ScreenEffect>();
    yield return (IAsyncJobResult) new WaitForMainCamera();
    this.OnEnable();
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (UniversalInputManager)
  };

  public void Shutdown()
  {
    this.OnDisable();
    if (ScreenEffectsMgr.m_ActiveScreenEffects == null)
      return;
    ScreenEffectsMgr.m_ActiveScreenEffects.Clear();
    ScreenEffectsMgr.m_ActiveScreenEffects = (List<ScreenEffect>) null;
  }

  public void Update()
  {
    if ((UnityEngine.Object) this.m_MainCamera == (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) Camera.main == (UnityEngine.Object) null)
        return;
      this.Init();
    }
    if ((UnityEngine.Object) this.m_ScreenEffectsRender == (UnityEngine.Object) null)
      return;
    if (ScreenEffectsMgr.m_ActiveScreenEffects != null && ScreenEffectsMgr.m_ActiveScreenEffects.Count > 0)
    {
      if (this.m_ScreenEffectsRender.enabled)
        return;
      this.m_ScreenEffectsRender.enabled = true;
    }
    else
    {
      if (!this.m_ScreenEffectsRender.enabled)
        return;
      this.m_ScreenEffectsRender.enabled = false;
    }
  }

  public void SetActive(bool enabled)
  {
    if (this.m_enabled == enabled)
      return;
    this.m_enabled = enabled;
    if (this.m_enabled)
      this.OnEnable();
    else
      this.OnDisable();
  }

  private void OnDisable()
  {
    if (!((UnityEngine.Object) this.m_ScreenEffectsRender != (UnityEngine.Object) null))
      return;
    this.m_ScreenEffectsRender.enabled = false;
  }

  private void OnEnable()
  {
    if ((UnityEngine.Object) Camera.main == (UnityEngine.Object) null)
      return;
    this.Init();
  }

  public static ScreenEffectsMgr Get() => ServiceManager.Get<ScreenEffectsMgr>();

  public static void RegisterScreenEffect(ScreenEffect effect)
  {
    if (ScreenEffectsMgr.m_ActiveScreenEffects == null)
      ScreenEffectsMgr.m_ActiveScreenEffects = new List<ScreenEffect>();
    if (ScreenEffectsMgr.m_ActiveScreenEffects.Contains(effect))
      return;
    ScreenEffectsMgr.m_ActiveScreenEffects.Add(effect);
  }

  public static void UnRegisterScreenEffect(ScreenEffect effect)
  {
    if (ScreenEffectsMgr.m_ActiveScreenEffects == null)
      return;
    ScreenEffectsMgr.m_ActiveScreenEffects.Remove(effect);
  }

  public int GetActiveScreenEffectsCount() => ScreenEffectsMgr.m_ActiveScreenEffects == null ? 0 : ScreenEffectsMgr.m_ActiveScreenEffects.Count;

  private void Init()
  {
    this.m_MainCamera = Camera.main;
    if ((UnityEngine.Object) this.m_MainCamera == (UnityEngine.Object) null)
      return;
    this.m_ScreenEffectsRender = this.m_MainCamera.GetComponent<ScreenEffectsRender>();
    if ((UnityEngine.Object) this.m_ScreenEffectsRender == (UnityEngine.Object) null)
    {
      this.m_ScreenEffectsRender = this.m_MainCamera.gameObject.AddComponent<ScreenEffectsRender>();
      this.m_MainCamera.allowHDR = false;
    }
    else
      this.m_ScreenEffectsRender.enabled = true;
  }
}
