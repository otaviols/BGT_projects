using System.Collections;
using UnityEngine;

public class Bounce : MonoBehaviour
{
  public float m_BounceSpeed = 3.5f;
  public float m_BounceAmount = 3f;
  public int m_BounceCount = 3;
  public float m_Bounceness = 0.2f;
  public float m_Delay;
  public bool m_PlayOnStart;
  private Vector3 m_StartingPosition;
  private float m_BounceAmountOverTime;

  private void Start()
  {
    if (!this.m_PlayOnStart)
      return;
    this.StartAnimation();
  }

  public void StartAnimation()
  {
    this.m_BounceAmountOverTime = this.m_BounceAmount;
    this.m_StartingPosition = this.transform.position;
    this.StartCoroutine("BounceAnimation");
  }

  private IEnumerator BounceAnimation()
  {
    Bounce bounce = this;
    yield return (object) new WaitForSeconds(bounce.m_Delay);
    for (int c = 0; c < bounce.m_BounceCount; ++c)
    {
      float time = 0.0f;
      for (float i = 0.0f; (double) i < 1.0; i += Time.deltaTime * bounce.m_BounceSpeed)
      {
        time += Time.deltaTime * bounce.m_BounceSpeed;
        Vector3 startingPosition = bounce.m_StartingPosition;
        float num = Mathf.Sin(time * 3.141593f);
        if ((double) num >= 0.0)
        {
          bounce.transform.position = new Vector3(startingPosition.x, startingPosition.y + num * bounce.m_BounceAmountOverTime, startingPosition.z);
          yield return (object) null;
        }
        else
          break;
      }
      bounce.m_BounceAmountOverTime *= bounce.m_Bounceness;
      yield return (object) null;
    }
    bounce.transform.position = bounce.m_StartingPosition;
  }
}
