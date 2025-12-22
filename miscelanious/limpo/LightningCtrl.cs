using System.Collections;
using UnityEngine;

public class LightningCtrl : MonoBehaviour
{
  public GameObject mylightning;
  private GameObject lightningObj;
  public float lifetime = 1f;
  public float position_X;
  public float position_Y;
  public float position_Z;
  public float scale = 0.1f;
  public float speed = 1f;
  public GameObject target;
  public GameObject destination;

  private void Update()
  {
    if (!InputCollection.GetMouseButtonDown(0))
      return;
    this.Spawn(this.target.transform, this.destination.transform);
  }

  public void Spawn(Transform targetTransform, Transform destinationTransform)
  {
    this.lightningObj = Object.Instantiate<GameObject>(this.mylightning, new Vector3(this.position_X, this.position_Y, this.position_Z), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
    this.lightningObj.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
    ElectroScript component = this.lightningObj.GetComponent<ElectroScript>();
    component.timers.timeToPowerUp = this.speed;
    component.prefabs.target.position = targetTransform.position;
    component.prefabs.destination.position = destinationTransform.position;
    this.StartCoroutine(this.DestroyLightning());
  }

  private IEnumerator DestroyLightning()
  {
    yield return (object) new WaitForSeconds(this.lifetime);
    Object.Destroy((Object) this.lightningObj);
  }
}
