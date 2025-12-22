using UnityEngine;

[AddComponentMenu("Hearthstone/Gizmos/Transform Locator")]
public class TransformLocator : MonoBehaviour
{
  public float scale = 1f;

  private void OnDrawGizmos()
  {
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Gizmos.color = Color.red;
    Gizmos.DrawLine(Vector3.zero, Vector3.right * this.scale);
    Gizmos.color = Color.red * 0.5f;
    Gizmos.DrawLine(Vector3.zero, Vector3.left * this.scale);
    Gizmos.color = Color.green;
    Gizmos.DrawLine(Vector3.zero, Vector3.up * this.scale);
    Gizmos.color = Color.green * 0.5f;
    Gizmos.DrawLine(Vector3.zero, Vector3.down * this.scale);
    Gizmos.color = Color.blue;
    Gizmos.DrawLine(Vector3.zero, Vector3.forward * this.scale);
    Gizmos.color = Color.blue * 0.5f;
    Gizmos.DrawLine(Vector3.zero, Vector3.back * this.scale);
  }
}
