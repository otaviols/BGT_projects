using UnityEngine;

public class ShowoffRotate : MonoBehaviour
{
  public float Speed = 1f;

  private void Update() => this.transform.Rotate(0.0f, this.Speed, 0.0f);
}
