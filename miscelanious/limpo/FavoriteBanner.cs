using UnityEngine;

public class FavoriteBanner : MonoBehaviour
{
  public GameObject m_favoriteBanner;
  private Vector3 m_worldOffset;

  private void Start() => this.m_worldOffset = this.m_favoriteBanner.transform.position - this.m_favoriteBanner.transform.parent.position;

  public void PinToActor(Actor actor)
  {
    if ((Object) this.m_favoriteBanner == (Object) null)
      return;
    this.m_favoriteBanner.transform.position = this.m_worldOffset + actor.transform.position;
    this.m_favoriteBanner.SetActive(true);
  }

  public void SetActive(bool enable) => this.m_favoriteBanner.SetActive(enable);
}
