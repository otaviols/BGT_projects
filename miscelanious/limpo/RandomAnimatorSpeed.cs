using UnityEngine;

public class RandomAnimatorSpeed : MonoBehaviour
{
  public float minSpeed = 0.5f;
  public float maxSpeed = 1.5f;

  private void Start()
  {
    Animator component = this.GetComponent<Animator>();
    if ((Object) component == (Object) null)
      return;
    component.speed = Random.Range(this.minSpeed, this.maxSpeed);
  }
}
