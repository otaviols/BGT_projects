using UnityEngine;

[ExecuteInEditMode]
public class HastyLookat : MonoBehaviour
{
  public Transform target;

  private void Update() => this.transform.LookAt(this.target);
}
