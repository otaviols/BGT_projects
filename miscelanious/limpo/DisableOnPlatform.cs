using UnityEngine;

public class DisableOnPlatform : MonoBehaviour
{
  public ScreenCategory m_screenCategory;

  private void OnEnable() => this.UpdateState();

  private void Update() => this.UpdateState();

  private void UpdateState()
  {
    if (!Application.IsPlaying((Object) this) || PlatformSettings.Screen != this.m_screenCategory)
      return;
    this.gameObject.SetActive(false);
  }
}
