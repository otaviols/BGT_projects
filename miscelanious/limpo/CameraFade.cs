using UnityEngine;

public class CameraFade : MonoBehaviour
{
  public Color m_Color = Color.black;
  public float m_Fade = 1f;
  private FullScreenEffects m_FullScreenEffects;
  private bool m_hasFullScreenEffects;

  private void Update()
  {
    if (!this.m_hasFullScreenEffects)
    {
      FullScreenFXMgr fullScreenFxMgr = FullScreenFXMgr.Get();
      if (fullScreenFxMgr == null)
        return;
      this.m_FullScreenEffects = fullScreenFxMgr.SecondaryCameraFullScreenEffects;
      this.m_hasFullScreenEffects = true;
    }
    if ((Object) this.m_FullScreenEffects == (Object) null)
      this.m_hasFullScreenEffects = false;
    else if ((double) this.m_Fade <= 0.0)
      this.m_FullScreenEffects.DisableBlendToColorOverride();
    else
      this.m_FullScreenEffects.SetBlendToColorOverride(this.m_Fade, this.m_Color);
  }
}
