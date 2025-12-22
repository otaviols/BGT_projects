using System.Collections;
using UnityEngine;

public class AnimPositionResetter : MonoBehaviour
{
  private Vector3 m_initialPosition;
  private float m_endTimestamp;
  private float m_delay;

  private void Awake() => this.m_initialPosition = this.transform.position;

  public static AnimPositionResetter OnAnimStarted(
    GameObject go,
    float animTime)
  {
    if ((double) animTime <= 0.0)
      return (AnimPositionResetter) null;
    AnimPositionResetter positionResetter = AnimPositionResetter.RegisterResetter(go);
    positionResetter.OnAnimStarted(animTime);
    return positionResetter;
  }

  public Vector3 GetInitialPosition() => this.m_initialPosition;

  public float GetEndTimestamp() => this.m_endTimestamp;

  public float GetDelay() => this.m_delay;

  private static AnimPositionResetter RegisterResetter(GameObject go)
  {
    if ((Object) go == (Object) null)
      return (AnimPositionResetter) null;
    AnimPositionResetter component = go.GetComponent<AnimPositionResetter>();
    return (Object) component != (Object) null ? component : go.AddComponent<AnimPositionResetter>();
  }

  private void OnAnimStarted(float animTime)
  {
    float num = Time.realtimeSinceStartup + animTime;
    float a = num - this.m_endTimestamp;
    if ((double) a <= 0.0)
      return;
    this.m_delay = Mathf.Min(a, animTime);
    this.m_endTimestamp = num;
    this.StopCoroutine("ResetPosition");
    this.StartCoroutine("ResetPosition");
  }

  private IEnumerator ResetPosition()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    AnimPositionResetter positionResetter = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      positionResetter.transform.position = positionResetter.m_initialPosition;
      Object.Destroy((Object) positionResetter);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(positionResetter.m_delay);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
