using UnityEngine;

public class FollowObject : MonoBehaviour
{
  public Transform targetObj;

  private void LateUpdate() => this.transform.position = this.targetObj.position;
}
