using UnityEngine;

public class BounceScale : MonoBehaviour
{
  public float m_Time;

  public void BounceyScale()
  {
    Vector3 localScale = this.transform.localScale;
    this.transform.localScale = Vector3.zero;
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) this.m_Time, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
  }
}
