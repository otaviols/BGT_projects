using System;
using UnityEngine;

public class TrailFromAnimation : MonoBehaviour
{
  [Tooltip("Can output node pathing to multiple line renderers")]
  public LineRenderer[] TargetLineRenderers;
  [Tooltip("The maximum number of divisions at full length. Number of divisions scales as trail gets shorter.")]
  public int MaxNumDivisions = 50;
  [Tooltip("Maximum length of trail in terms of time. Trail automatically shortens at beginning and end of lifetime.")]
  public float TrailLengthTime = 0.5f;
  [Tooltip("Scales length of trail over lifetime.")]
  public AnimationCurve LengthScaleOverTime;
  [Tooltip("The animation clip to use for trail pathing.")]
  public AnimationClip SourceAnimationClip;
  [Tooltip("The dummy gameobject tree to use for animation simulations. Need to match heirarchy of original animated object. Top level of heirarchy should correlate to the parent object that original animator & animation clip are attached to.")]
  public GameObject NodeAvatarParent;
  [Tooltip("The specific gameobject in the heirarchy that the trail pathing should follow")]
  public GameObject NodeAvatar;
  private float _animTime;
  private Vector3[] _nodeArray;
  [Tooltip("Output pathing nodes in reverse")]
  public bool ReverseNodeArray;
  [Tooltip("Output pathing nodes to world space instead of local space")]
  public bool OutputWorldSpace;

  private void Update()
  {
    this._animTime += Time.deltaTime;
    this._nodeArray = this.RefreshNodeArray(this.MaxNumDivisions, this.SourceAnimationClip, this.NodeAvatarParent, this.NodeAvatar, this._animTime, this.TrailLengthTime);
    this.RestructureLines(this.TargetLineRenderers, this._nodeArray);
  }

  private Vector3[] RefreshNodeArray(
    int numDivisions,
    AnimationClip sourceAnimClip,
    GameObject nodeAvatarParent,
    GameObject nodeAvatar,
    float animTime,
    float trailMaxTimeLag)
  {
    float length1 = sourceAnimClip.length;
    double num1 = (double) animTime * (double) length1;
    float num2 = Mathf.Clamp((float) num1, 0.0f, length1);
    float num3 = Mathf.Clamp((float) num1 - trailMaxTimeLag, 0.0f, length1);
    double num4 = (double) num2 - (double) num3;
    float num5 = (float) num4 / trailMaxTimeLag;
    int length2 = Mathf.RoundToInt((float) numDivisions * num5);
    float num6 = (float) num4 / (float) length2;
    Vector3[] vector3Array = new Vector3[length2];
    for (int index = 0; index < length2; ++index)
    {
      float time = num2 - num6 * (float) index;
      sourceAnimClip.SampleAnimation(nodeAvatarParent, time);
      vector3Array[index] = !this.OutputWorldSpace ? nodeAvatar.transform.localPosition : nodeAvatar.transform.position;
    }
    if (this.ReverseNodeArray)
      Array.Reverse((Array) vector3Array);
    return vector3Array;
  }

  private void RestructureLines(LineRenderer[] lineRenderers, Vector3[] nodeArray)
  {
    for (int index = 0; index < lineRenderers.Length; ++index)
    {
      lineRenderers[index].SetPositions(nodeArray);
      lineRenderers[index].positionCount = nodeArray.Length;
    }
  }
}
