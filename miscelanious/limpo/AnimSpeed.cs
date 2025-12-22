using UnityEngine;

public class AnimSpeed : MonoBehaviour
{
  public float animspeed = 1f;

  private void Awake()
  {
    foreach (AnimationState animationState in this.GetComponent<Animation>())
      animationState.speed = this.animspeed;
  }
}
