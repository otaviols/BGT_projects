using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using UnityEngine;

public class AnimationUtil : MonoBehaviour
{
  public static void ShowWithPunch(
    GameObject go,
    Vector3 startScale,
    Vector3 punchScale,
    Vector3 afterPunchScale,
    string callbackName = "",
    bool noFade = false,
    GameObject callbackGO = null,
    object callbackData = null,
    AnimationUtil.DelOnShownWithPunch onShowPunchCallback = null)
  {
    if (!noFade)
      iTween.FadeTo(go, 1f, 0.25f);
    go.transform.localScale = startScale;
    iTween.ScaleTo(go, iTween.Hash((object) "scale", (object) punchScale, (object) "time", (object) 0.25f));
    iTween.MoveTo(go, iTween.Hash((object) "position", (object) (go.transform.position + new Vector3(0.02f, 0.02f, 0.02f)), (object) "time", (object) 1.5f));
    AnimationUtil.PunchData callbackData1 = new AnimationUtil.PunchData()
    {
      m_gameObject = go,
      m_scale = afterPunchScale,
      m_callbackName = callbackName,
      m_callbackGameObject = callbackGO,
      m_callbackData = callbackData,
      m_onShowPunchCallback = onShowPunchCallback
    };
    go.GetComponent<MonoBehaviour>().StartCoroutine(AnimationUtil.ShowPunchRoutine(callbackData1));
  }

  private static IEnumerator ShowPunchRoutine(AnimationUtil.PunchData callbackData)
  {
    yield return (object) new WaitForSeconds(0.25f);
    AnimationUtil.ShowPunch(callbackData.m_gameObject, callbackData.m_scale, callbackData.m_callbackName, callbackData.m_callbackGameObject, callbackData.m_callbackData);
    if (callbackData.m_onShowPunchCallback != null)
      callbackData.m_onShowPunchCallback(callbackData.m_callbackData);
  }

  public static void ShowPunch(
    GameObject go,
    Vector3 scale,
    string callbackName = "",
    GameObject callbackGO = null,
    object callbackData = null)
  {
    if (string.IsNullOrEmpty(callbackName))
    {
      iTween.ScaleTo(go, scale, 0.15f);
    }
    else
    {
      if ((UnityEngine.Object) callbackGO == (UnityEngine.Object) null)
        callbackGO = go;
      if (callbackData == null)
        callbackData = new object();
      Hashtable args = iTween.Hash((object) nameof (scale), (object) scale, (object) "time", (object) 0.15f, (object) "oncomplete", (object) callbackName, (object) "oncompletetarget", (object) callbackGO, (object) "oncompleteparams", callbackData);
      iTween.ScaleTo(go, args);
    }
  }

  public static void GrowThenDrift(GameObject go, Vector3 origin, Vector3 driftOffset)
  {
    iTween.ScaleFrom(go, iTween.Hash((object) "scale", (object) (Vector3.one * 0.05f), (object) "time", (object) 0.15f, (object) "easeType", (object) iTween.EaseType.easeOutQuart));
    iTween.MoveFrom(go, iTween.Hash((object) "position", (object) origin, (object) "time", (object) 0.15f, (object) "easeType", (object) iTween.EaseType.easeOutQuart));
    go.GetComponent<MonoBehaviour>().StartCoroutine(AnimationUtil.DriftAfterTween(go, 0.15f, driftOffset));
  }

  public static void GrowThenDrift(GameObject go, Vector3 origin, float driftScale)
  {
    Vector3 vector3 = PlatformSettings.IsTablet ? new Vector3(0.0f, 0.1f, 0.1f) : new Vector3(0.1f, 0.1f, 0.1f);
    Vector3 worldScale = TransformUtil.ComputeWorldScale((Component) go.transform.parent);
    double num = (double) driftScale;
    Vector3 driftOffset = Vector3.Scale(vector3 * (float) num, worldScale);
    AnimationUtil.GrowThenDrift(go, origin, driftOffset);
  }

  private static IEnumerator DriftAfterTween(
    GameObject go,
    float delayTime,
    Vector3 driftOffset)
  {
    yield return (object) new WaitForSeconds(delayTime);
    AnimationUtil.DriftObject(go, driftOffset);
  }

  public static void FloatyPosition(
    GameObject go,
    Vector3 startPos,
    float localRadius,
    float loopTime)
  {
    Vector3[] vector3Array = new Vector3[5]
    {
      startPos,
      startPos + new Vector3(localRadius, 0.0f, localRadius),
      startPos + new Vector3(localRadius * 2f, 0.0f, 0.0f),
      startPos + new Vector3(localRadius, 0.0f, -localRadius),
      startPos + Vector3.zero
    };
    iTween.StopByName("DriftingTween");
    iTween.MoveTo(go, iTween.Hash((object) "name", (object) "DriftingTween", (object) "path", (object) vector3Array, (object) "time", (object) loopTime, (object) "islocal", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "looptype", (object) iTween.LoopType.loop, (object) "movetopath", (object) false));
  }

  public static void FloatyPosition(GameObject go, float radius, float loopTime) => AnimationUtil.FloatyPosition(go, go.transform.localPosition, radius, loopTime);

  public static void ScaleFade(GameObject go, Vector3 scale) => AnimationUtil.ScaleFade(go, scale, (string) null);

  public static void ScaleFade(GameObject go, Vector3 scale, string callbackName)
  {
    iTween.FadeTo(go, 0.0f, 0.25f);
    Hashtable args;
    if (string.IsNullOrEmpty(callbackName))
      args = iTween.Hash((object) nameof (scale), (object) scale, (object) "time", (object) 0.25f);
    else
      args = iTween.Hash((object) nameof (scale), (object) scale, (object) "time", (object) 0.25f, (object) "oncomplete", (object) callbackName, (object) "oncompletetarget", (object) go);
    iTween.ScaleTo(go, args);
  }

  public static int GetLayerIndexFromName(Animator animator, string layerName)
  {
    if (layerName == null)
      return -1;
    layerName = layerName.Trim();
    for (int layerIndex = 0; layerIndex < animator.layerCount; ++layerIndex)
    {
      string layerName1 = animator.GetLayerName(layerIndex);
      if (layerName1 != null && layerName1.Trim().Equals(layerName, StringComparison.OrdinalIgnoreCase))
        return layerIndex;
    }
    return -1;
  }

  public static void DriftObject(GameObject go, Vector3 driftOffset)
  {
    iTween.StopByName(go, "DRIFT_MOVE_OBJECT_ITWEEN");
    iTween.MoveBy(go, iTween.Hash((object) "amount", (object) driftOffset, (object) "time", (object) 10f, (object) "name", (object) "DRIFT_MOVE_OBJECT_ITWEEN", (object) "easeType", (object) iTween.EaseType.easeOutQuart));
  }

  public static void FadeTexture(
    MeshRenderer mesh,
    float fromAlpha,
    float toAlpha,
    float fadeTime,
    float delay,
    AnimationUtil.DelOnFade onCompleteCallback = null)
  {
    iTween.StopByName(mesh.gameObject, "FADE_TEXTURE");
    Material logoMaterial = mesh.GetMaterial();
    Color currentColor = logoMaterial.GetColor("_Color") with
    {
      a = fromAlpha
    };
    logoMaterial.SetColor("_Color", currentColor);
    Hashtable args = iTween.Hash((object) "from", (object) fromAlpha, (object) "to", (object) toAlpha, (object) "time", (object) fadeTime, (object) "onupdate", (object) (Action<object>) (val =>
    {
      currentColor.a = (float) val;
      logoMaterial.SetColor("_Color", currentColor);
    }), (object) "name", (object) "FADE_TEXTURE");
    if ((double) delay > 0.0)
      args.Add((object) nameof (delay), (object) delay);
    if (onCompleteCallback != null)
      args.Add((object) "oncomplete", (object) (Action<object>) (o => onCompleteCallback()));
    iTween.ValueTo(mesh.gameObject, args);
  }

  public static void DelayedActivate(GameObject go, float time, bool activate) => go.GetComponent<MonoBehaviour>().StartCoroutine(AnimationUtil.DelayedActivation(go, time, activate));

  private static IEnumerator DelayedActivation(GameObject go, float time, bool activate)
  {
    yield return (object) new WaitForSeconds(time);
    go.SetActive(activate);
  }

  public delegate void DelOnShownWithPunch(object callbackData);

  public delegate void DelOnFade();

  private class PunchData
  {
    public GameObject m_gameObject;
    public Vector3 m_scale;
    public string m_callbackName;
    public GameObject m_callbackGameObject;
    public object m_callbackData;
    public AnimationUtil.DelOnShownWithPunch m_onShowPunchCallback;
  }
}
