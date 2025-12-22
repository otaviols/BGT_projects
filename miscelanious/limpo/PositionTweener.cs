using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class PositionTweener
{
  [SerializeField]
  private string m_name = "position";
  [SerializeField]
  private bool m_isLocal = true;
  [SerializeField]
  private Vector3 m_initialPosition = Vector3.zero;
  [SerializeField]
  private Vector3 m_finalPosition = Vector3.zero;
  [SerializeField]
  private float m_time = 1f;
  [SerializeField]
  private iTween.EaseType m_easeType = iTween.EaseType.easeOutQuad;
  [SerializeField]
  [Header("Callback")]
  private string m_onCompleteCallbackMethodName;
  [SerializeField]
  private GameObject m_onCompleteGameObject;

  public bool IsLocal => this.m_isLocal;

  public Vector3 InitialPosition => this.m_initialPosition;

  public Vector3 FinalPosition => this.m_finalPosition;

  public void Play(GameObject target, bool forward)
  {
    Hashtable args = iTween.Hash((object) "name", (object) this.m_name, (object) "position", (object) (forward ? this.m_finalPosition : this.m_initialPosition), (object) "isLocal", (object) this.m_isLocal, (object) "time", (object) this.m_time, (object) "easetype", (object) this.m_easeType);
    if (!string.IsNullOrWhiteSpace(this.m_onCompleteCallbackMethodName) && (UnityEngine.Object) this.m_onCompleteGameObject != (UnityEngine.Object) null)
    {
      args.Add((object) "oncomplete", (object) this.m_onCompleteCallbackMethodName);
      args.Add((object) "oncompletetarget", (object) this.m_onCompleteGameObject);
    }
    iTween.MoveTo(target, args);
  }

  public PositionTweener SetInitialPosition(Vector3 initialPosition)
  {
    this.m_initialPosition = initialPosition;
    return this;
  }

  public PositionTweener SetFinalPosition(Vector3 targetPosition)
  {
    this.m_finalPosition = targetPosition;
    return this;
  }
}
