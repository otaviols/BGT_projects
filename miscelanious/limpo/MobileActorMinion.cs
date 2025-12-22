using UnityEngine;

public class MobileActorMinion : MonoBehaviour
{
  public Vector3 m_minionScaleFactor = Vector3.one;

  private void Awake()
  {
    if (!PlatformSettings.IsMobile() || !(bool) UniversalInputManager.UsePhoneUI)
      return;
    Vector3 localScale = this.gameObject.transform.localScale;
    localScale.x *= this.m_minionScaleFactor.x;
    localScale.y *= this.m_minionScaleFactor.y;
    localScale.z *= this.m_minionScaleFactor.z;
    this.gameObject.transform.localScale = localScale;
  }
}
