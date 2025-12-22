using UnityEngine;

public class RandomTransform : MonoBehaviour
{
  public bool m_applyOnStart;
  public Vector3 positionMin;
  public Vector3 positionMax;
  public Vector3 rotationMin;
  public Vector3 rotationMax;
  public Vector3 scaleMin = Vector3.one;
  public Vector3 scaleMax = Vector3.one;

  public void Start()
  {
    if (!this.m_applyOnStart)
      return;
    this.Apply();
  }

  public void Apply()
  {
    this.transform.localPosition = this.transform.localPosition + new Vector3(Random.Range(this.positionMin.x, this.positionMax.x), Random.Range(this.positionMin.y, this.positionMax.y), Random.Range(this.positionMin.z, this.positionMax.z));
    this.transform.localEulerAngles = this.transform.localEulerAngles + new Vector3(Random.Range(this.rotationMin.x, this.rotationMax.x), Random.Range(this.rotationMin.y, this.rotationMax.y), Random.Range(this.rotationMin.z, this.rotationMax.z));
    Vector3 vector3_1 = new Vector3(Random.Range(this.scaleMin.x, this.scaleMax.x), Random.Range(this.scaleMin.y, this.scaleMax.y), Random.Range(this.scaleMin.z, this.scaleMax.z));
    Vector3 vector3_2 = vector3_1;
    vector3_1.Scale(this.transform.localScale);
    this.transform.localScale = vector3_2;
  }
}
