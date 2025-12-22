using Hearthstone.Core;
using System;
using System.Collections;
using UnityEngine;

public class ShakePane : MonoBehaviour
{
  [SerializeField]
  private GameObject m_shakeyObject;
  [SerializeField]
  private float m_multipleShakeTolerance = 1.2f;
  [SerializeField]
  private float m_maxRotation = 30f;
  private Coroutine m_shakeyStoreAnimCoroutine;
  private Vector3 m_shakeyObjectOriginalLocalRotation = Vector3.zero;
  private Vector3 m_shakeyObjectOriginalLocalPosition = Vector3.zero;
  private float m_lastShakeAmount;
  private bool m_stillShaking;

  protected void Awake()
  {
    if (!((UnityEngine.Object) this.m_shakeyObject != (UnityEngine.Object) null))
      return;
    this.m_shakeyObjectOriginalLocalRotation = this.m_shakeyObject.transform.localEulerAngles;
    this.m_shakeyObjectOriginalLocalPosition = this.m_shakeyObject.transform.localPosition;
  }

  public void Shake(float xRotationAmount, float shakeTime, float delay = 0.0f, float translateAmount = 0.0f)
  {
    if ((UnityEngine.Object) this.m_shakeyObject == (UnityEngine.Object) null || !this.gameObject.activeInHierarchy)
      return;
    this.m_shakeyStoreAnimCoroutine = this.StartCoroutine(this.AnimateShakeyObjectCoroutine(xRotationAmount, translateAmount, shakeTime, delay));
  }

  public void Reset()
  {
    if (this.m_shakeyStoreAnimCoroutine == null)
      return;
    this.StopCoroutine(this.m_shakeyStoreAnimCoroutine);
  }

  private void OnStopShaking(object obj) => this.m_stillShaking = false;

  private IEnumerator AnimateShakeyObjectCoroutine(
    float xRotationAmount,
    float translationAmount,
    float shakeTime,
    float delay)
  {
    ShakePane shakePane = this;
    xRotationAmount = Mathf.Clamp(xRotationAmount, -shakePane.m_maxRotation, shakePane.m_maxRotation);
    float absRotation = Mathf.Abs(xRotationAmount);
    if ((double) absRotation - (double) shakePane.m_lastShakeAmount >= (double) shakePane.m_multipleShakeTolerance || !shakePane.m_stillShaking)
    {
      if ((double) delay > 0.0)
        yield return (object) new WaitForSeconds(delay);
      shakePane.m_lastShakeAmount = absRotation;
      shakePane.m_stillShaking = true;
      Processor.CancelScheduledCallback(new Processor.ScheduledCallback(shakePane.OnStopShaking));
      Processor.ScheduleCallback(shakeTime * 0.25f, false, new Processor.ScheduledCallback(shakePane.OnStopShaking));
      iTween.Stop(shakePane.m_shakeyObject);
      shakePane.m_shakeyObject.transform.localEulerAngles = shakePane.m_shakeyObjectOriginalLocalRotation;
      Hashtable tweenHashTable1 = iTweenManager.Get().GetTweenHashTable();
      tweenHashTable1.Add((object) "x", (object) xRotationAmount);
      tweenHashTable1.Add((object) "time", (object) shakeTime);
      tweenHashTable1.Add((object) nameof (delay), (object) (1f / 1000f));
      // ISSUE: reference to a compiler-generated method
      tweenHashTable1.Add((object) "oncomplete", (object) new Action<object>(shakePane.\u003CAnimateShakeyObjectCoroutine\u003Eb__12_0));
      iTween.PunchRotation(shakePane.m_shakeyObject, tweenHashTable1, false);
      if ((double) translationAmount != 0.0)
      {
        shakePane.m_shakeyObject.transform.localPosition = shakePane.m_shakeyObjectOriginalLocalPosition;
        Hashtable tweenHashTable2 = iTweenManager.Get().GetTweenHashTable();
        tweenHashTable2.Add((object) "y", (object) translationAmount);
        tweenHashTable2.Add((object) "time", (object) shakeTime);
        tweenHashTable2.Add((object) nameof (delay), (object) (1f / 1000f));
        // ISSUE: reference to a compiler-generated method
        tweenHashTable2.Add((object) "oncomplete", (object) new Action<object>(shakePane.\u003CAnimateShakeyObjectCoroutine\u003Eb__12_1));
        iTween.PunchPosition(shakePane.m_shakeyObject, tweenHashTable2);
      }
    }
  }
}
