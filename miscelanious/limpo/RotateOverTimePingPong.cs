using System.Collections;
using UnityEngine;

public class RotateOverTimePingPong : MonoBehaviour
{
  public float RotateSpeedX;
  public float RotateSpeedY;
  public float RotateSpeedZ;
  public bool RandomStartX = true;
  public bool RandomStartY = true;
  public bool RandomStartZ = true;
  public float RotateRangeXmin;
  public float RotateRangeXmax = 10f;
  public float RotateRangeYmin;
  public float RotateRangeYmax = 10f;
  public float RotateRangeZmin;
  public float RotateRangeZmax = 10f;

  private void Start()
  {
    if (this.RandomStartX)
      this.transform.Rotate(Vector3.left, Random.Range(this.RotateRangeXmin, this.RotateRangeXmax));
    if (this.RandomStartY)
      this.transform.Rotate(Vector3.up, Random.Range(this.RotateRangeYmin, this.RotateRangeYmax));
    if (!this.RandomStartZ)
      return;
    this.transform.Rotate(Vector3.forward, Random.Range(this.RotateRangeZmin, this.RotateRangeZmax));
  }

  private void Update()
  {
    float z = Mathf.Sin(Time.time) * this.RotateRangeZmax;
    float y = this.gameObject.transform.localRotation.y;
    iTweenManager iTweenManager = iTweenManager.Get();
    Hashtable tweenHashTable = iTweenManager.GetTweenHashTable();
    tweenHashTable.Add((object) "rotation", (object) new Vector3(0.0f, y, z));
    tweenHashTable.Add((object) "isLocal", (object) true);
    tweenHashTable.Add((object) "time", (object) 0.0f);
    iTween.RotateUpdate(this.gameObject, tweenHashTable, false);
    iTweenManager.ReturnTweenHashTable(tweenHashTable);
  }
}
