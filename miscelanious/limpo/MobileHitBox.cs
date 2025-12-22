using UnityEngine;

public class MobileHitBox : MonoBehaviour
{
  public BoxCollider m_boxCollider;
  public float m_scaleX = 1f;
  public float m_scaleY = 1f;
  public float m_scaleZ;
  public Vector3 m_offset;
  public bool m_phoneOnly;
  private bool m_hasExecuted;
  private PlatformDependentValue<bool> m_isMobile = new PlatformDependentValue<bool>(PlatformCategory.Screen)
  {
    Tablet = true,
    MiniTablet = true,
    Phone = true,
    PC = false
  };

  private void Start()
  {
    if (!((Object) this.m_boxCollider != (Object) null) || !(bool) this.m_isMobile || this.m_phoneOnly && !(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_boxCollider.size = new Vector3()
    {
      x = this.m_boxCollider.size.x * this.m_scaleX,
      y = this.m_boxCollider.size.y * this.m_scaleY,
      z = (double) this.m_scaleZ != 0.0 ? this.m_boxCollider.size.z * this.m_scaleZ : this.m_boxCollider.size.z * this.m_scaleY
    };
    this.m_boxCollider.center += this.m_offset;
    this.m_hasExecuted = true;
  }

  public bool HasExecuted() => this.m_hasExecuted;
}
