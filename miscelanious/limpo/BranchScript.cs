using UnityEngine;

public class BranchScript : MonoBehaviour
{
  public float timeSpawned;

  private void Awake() => this.timeSpawned = Time.time;
}
