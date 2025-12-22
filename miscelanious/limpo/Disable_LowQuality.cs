using Blizzard.T5.Services;
using UnityEngine;

public class Disable_LowQuality : MonoBehaviour
{
  private IGraphicsManager m_graphicsManager;

  private void Awake()
  {
    this.m_graphicsManager = ServiceManager.Get<IGraphicsManager>();
    this.m_graphicsManager.RegisterLowQualityDisableObject(this.gameObject);
    if (this.m_graphicsManager.RenderQualityLevel != GraphicsQuality.Low)
      return;
    this.gameObject.SetActive(false);
  }

  private void OnDestroy()
  {
    if (this.m_graphicsManager == null)
      return;
    this.m_graphicsManager.DeregisterLowQualityDisableObject(this.gameObject);
  }
}
