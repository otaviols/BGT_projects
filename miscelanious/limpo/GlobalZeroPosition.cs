using UnityEngine;

public class GlobalZeroPosition : MonoBehaviour
{
  private void LateUpdate()
  {
    this.transform.position = Vector3.zero;
    this.transform.rotation = Quaternion.identity;
    this.transform.localScale = Vector3.one;
  }
}
