using System.Collections;
using UnityEngine;

public class TokyoDrift : MonoBehaviour
{
  public float m_DriftPositionAmount = 1f;
  public bool m_DriftPositionAxisX = true;
  public bool m_DriftPositionAxisY = true;
  public bool m_DriftPositionAxisZ = true;
  public float m_DriftSpeed = 0.1f;
  private Vector3 m_originalPosition;
  private Vector3 m_newPosition;
  private float m_posSeedX;
  private float m_posSeedY;
  private float m_posSeedZ;
  private float m_posOffsetX;
  private float m_posOffsetY;
  private float m_posOffsetZ;
  private float m_blend;
  private bool m_blendOut;

  private void Start()
  {
    this.m_originalPosition = this.transform.localPosition;
    this.m_newPosition = new Vector3();
    this.m_posSeedX = (float) Random.Range(1, 10);
    this.m_posSeedY = (float) Random.Range(1, 10);
    this.m_posSeedZ = (float) Random.Range(1, 10);
    this.m_posOffsetX = Random.Range(0.6f, 0.99f);
    this.m_posOffsetY = Random.Range(0.6f, 0.99f);
    this.m_posOffsetZ = Random.Range(0.6f, 0.99f);
  }

  private void OnDisable()
  {
    if ((double) this.m_blend == 0.0)
      return;
    if (!this.gameObject.activeInHierarchy)
    {
      this.m_blend = 0.0f;
      this.m_blendOut = false;
      this.transform.localPosition = this.m_originalPosition;
    }
    else
    {
      if ((double) this.m_blend > 1.0)
        this.m_blend = 1f;
      this.StartCoroutine(this.BlendOut());
    }
  }

  private void Update()
  {
    if (this.m_blendOut)
      return;
    float num1 = Time.timeSinceLevelLoad * this.m_DriftSpeed;
    float num2 = this.m_originalPosition.x;
    float num3 = this.m_originalPosition.y;
    float num4 = this.m_originalPosition.z;
    if (this.m_DriftPositionAxisX)
      num2 = Mathf.Sin(num1 + this.m_posSeedX + Mathf.Cos(num1 * this.m_posOffsetX)) * this.m_DriftPositionAmount;
    if (this.m_DriftPositionAxisY)
      num3 = Mathf.Sin(num1 + this.m_posSeedY + Mathf.Cos(num1 * this.m_posOffsetY)) * this.m_DriftPositionAmount;
    if (this.m_DriftPositionAxisZ)
      num4 = Mathf.Sin(num1 + this.m_posSeedZ + Mathf.Cos(num1 * this.m_posOffsetZ)) * this.m_DriftPositionAmount;
    this.m_newPosition.x = this.m_originalPosition.x + num2;
    this.m_newPosition.y = this.m_originalPosition.y + num3;
    this.m_newPosition.z = this.m_originalPosition.z + num4;
    if ((double) this.m_blend < 1.0)
    {
      this.transform.localPosition = Vector3.Lerp(this.m_originalPosition, this.m_newPosition, this.m_blend);
      this.m_blend += Time.deltaTime * this.m_DriftSpeed;
    }
    else
      this.transform.localPosition = this.m_newPosition;
  }

  private IEnumerator BlendOut()
  {
    TokyoDrift tokyoDrift = this;
    tokyoDrift.m_blendOut = true;
    tokyoDrift.m_blend = 0.0f;
    while ((double) tokyoDrift.m_blend < 1.0)
    {
      tokyoDrift.transform.localPosition = Vector3.Lerp(tokyoDrift.m_newPosition, tokyoDrift.m_originalPosition, tokyoDrift.m_blend);
      tokyoDrift.m_blend += Time.deltaTime * tokyoDrift.m_DriftSpeed;
      yield return (object) null;
    }
    tokyoDrift.m_blend = 0.0f;
    tokyoDrift.m_blendOut = false;
  }
}
