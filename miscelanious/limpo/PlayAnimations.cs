using UnityEngine;

public class PlayAnimations : MonoBehaviour
{
  private Animation m_coinDropAnimation;

  public void Awake() => this.m_coinDropAnimation = this.GetComponent<Animation>();

  public void Update()
  {
    this.m_coinDropAnimation.PlayQueued("CoinDropA");
    this.m_coinDropAnimation.PlayQueued("CoinDropB");
  }
}
