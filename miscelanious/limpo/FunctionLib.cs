using UnityEngine;

public class FunctionLib : MonoBehaviour
{
  public LightningCtrl lightningScript;
  public GameObject target;
  public GameObject destination;

  private void onAnimaitonEvent() => this.lightningScript.Spawn(this.target.transform, this.destination.transform);
}
