using System;
using UnityEngine;

public class BigCardBoneLayout : MonoBehaviour
{
  public GameObject m_OuterLeftBone;
  public GameObject m_InnerLeftBone;
  public GameObject m_InnerRightBone;
  public GameObject m_OuterRightBone;
  public BigCardBoneLayout.ScaleSettings m_scaleSettings = new BigCardBoneLayout.ScaleSettings();

  private void Awake()
  {
    if ((double) this.m_scaleSettings.m_BigCardScale_Minion <= 0.0)
    {
      GameObject gameObject = this.gameObject;
      while ((UnityEngine.Object) this.gameObject.transform.parent.gameObject != (UnityEngine.Object) null)
        gameObject = this.gameObject.transform.parent.gameObject;
      Debug.LogError((object) (string.Format("{0} on object \"{1}\" is an invalid value for the scale of a big card. Parent-most object is called \"{2}\". ", (object) this.m_scaleSettings.m_BigCardScale_Minion, (object) this.gameObject.name, (object) gameObject.name) + "This should be a positive number.  Value is being set to 1."));
      this.m_scaleSettings.m_BigCardScale_Minion = 1f;
    }
    if ((double) this.m_scaleSettings.m_BigCardScale_LettuceAbility > 0.0)
      return;
    GameObject gameObject1 = this.gameObject;
    while ((UnityEngine.Object) this.gameObject.transform.parent.gameObject != (UnityEngine.Object) null)
      gameObject1 = this.gameObject.transform.parent.gameObject;
    Debug.LogError((object) (string.Format("{0} on object \"{1}\" is an invalid value for the scale of a big card. Parent-most object is called \"{2}\". ", (object) this.m_scaleSettings.m_BigCardScale_LettuceAbility, (object) this.gameObject.name, (object) gameObject1.name) + "This should be a positive number.  Value is being set to 1."));
    this.m_scaleSettings.m_BigCardScale_LettuceAbility = 1f;
  }

  public bool HasAllBones() => (UnityEngine.Object) this.m_OuterLeftBone != (UnityEngine.Object) null && (UnityEngine.Object) this.m_InnerLeftBone != (UnityEngine.Object) null && (UnityEngine.Object) this.m_InnerRightBone != (UnityEngine.Object) null && (UnityEngine.Object) this.m_OuterRightBone != (UnityEngine.Object) null;

  [Serializable]
  public class ScaleSettings
  {
    [Range(0.1f, 5f)]
    public float m_BigCardScale_Minion = 1f;
    [Range(0.1f, 5f)]
    public float m_BigCardScale_LettuceAbility = 1f;
  }
}
