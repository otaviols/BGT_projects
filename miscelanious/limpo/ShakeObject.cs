using UnityEngine;

public class ShakeObject : MonoBehaviour
{
  public float amount = 1f;
  private Vector3 orgPos;

  private void Start() => this.orgPos = this.transform.position;

  private void Update() => this.transform.position = this.orgPos + new Vector3(Random.value * this.amount * this.amount, Random.value * this.amount * this.amount, Random.value * this.amount * this.amount);
}
