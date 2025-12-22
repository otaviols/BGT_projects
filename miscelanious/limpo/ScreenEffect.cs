using UnityEngine;

public class ScreenEffect : MonoBehaviour
{
  private ScreenEffectsMgr m_ScreenEffectsMgr;

  private void Awake() => this.m_ScreenEffectsMgr = ScreenEffectsMgr.Get();

  private void OnEnable()
  {
    if (this.m_ScreenEffectsMgr == null)
      this.m_ScreenEffectsMgr = ScreenEffectsMgr.Get();
    ScreenEffectsMgr.RegisterScreenEffect(this);
  }

  private void OnDisable()
  {
    if (this.m_ScreenEffectsMgr == null)
      this.m_ScreenEffectsMgr = ScreenEffectsMgr.Get();
    if (this.m_ScreenEffectsMgr == null)
      return;
    ScreenEffectsMgr.UnRegisterScreenEffect(this);
  }
}
