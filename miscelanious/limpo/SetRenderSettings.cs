using UnityEngine;

public class SetRenderSettings : MonoBehaviour
{
  public Color m_ambient;
  private bool m_ambient_shouldUpdate;
  private Color m_lastSavedAmbient;

  private void enableAmbientUpdates()
  {
    this.m_ambient_shouldUpdate = true;
    if ((Object) LoadingScreen.Get() != (Object) null && LoadingScreen.Get().IsPreviousSceneActive())
    {
      this.m_lastSavedAmbient = this.m_ambient;
      LoadingScreen.Get().RegisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnPreviousSceneDestroyed));
    }
    else
      RenderSettings.ambientLight = this.m_ambient;
  }

  private void disableAmbientUpdates() => this.m_ambient_shouldUpdate = false;

  private void Update()
  {
    if (!this.m_ambient_shouldUpdate)
      return;
    this.m_lastSavedAmbient = this.m_ambient;
    if (!((Object) LoadingScreen.Get() == (Object) null) && LoadingScreen.Get().IsPreviousSceneActive())
      return;
    RenderSettings.ambientLight = this.m_ambient;
  }

  public void SetColor(Color newColor)
  {
    this.m_ambient = newColor;
    this.m_lastSavedAmbient = newColor;
  }

  private void OnPreviousSceneDestroyed(object userData)
  {
    LoadingScreen.Get().UnregisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnPreviousSceneDestroyed));
    RenderSettings.ambientLight = this.m_lastSavedAmbient;
  }
}
