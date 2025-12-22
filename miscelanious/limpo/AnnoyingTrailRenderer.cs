using System;
using UnityEngine;

public class AnnoyingTrailRenderer : MonoBehaviour
{
  public GameObject GameObjectWithLineRenderer;
  public LineRenderer TargetLineRenderer;
  public int NumDivisions = 50;
  public float TrailMaxTimeLag = 0.5f;
  public GameObject GameObjectWithAnimator;
  public Animator _runningAnimator;
  public bool MoveToAnimParent;
  public AnimationClip SourceAnimationClip;
  public GameObject NodeAvatarParent;
  public GameObject NodeAvatar;
  private float _animTime;
  private Vector3[] _nodeArray;
  public bool ReverseNodeArray;
  public bool OutputWorldSpace = true;
  private float _cutoffTime;

  private void OnEnable()
  {
    if ((bool) (UnityEngine.Object) this.GameObjectWithAnimator)
      this._runningAnimator = this.GameObjectWithAnimator.GetComponent<Animator>();
    if (!(bool) (UnityEngine.Object) this.TargetLineRenderer)
      this.TargetLineRenderer = this.GameObjectWithLineRenderer.GetComponent<LineRenderer>();
    this.TargetLineRenderer.sortingOrder = 1000;
    if (!this.MoveToAnimParent)
      return;
    this.gameObject.transform.parent = this.GameObjectWithAnimator.transform.parent.transform;
    this.gameObject.transform.localPosition = this.GameObjectWithAnimator.transform.localPosition;
    this.gameObject.transform.localScale = this.GameObjectWithAnimator.transform.localScale;
    this.gameObject.transform.rotation = this.GameObjectWithAnimator.transform.rotation;
  }

  private void Update()
  {
    if ((bool) (UnityEngine.Object) this._runningAnimator)
      this._animTime = this.GetAnimationPosition(this._runningAnimator);
    else if ((double) this._cutoffTime == 0.0)
      this._cutoffTime = Time.time;
    this._nodeArray = this.RefreshNodeArray(this.NumDivisions, this.SourceAnimationClip, this.NodeAvatarParent, this.NodeAvatar, this._animTime, this.TrailMaxTimeLag, this._cutoffTime);
    this.RestructureLine(this.TargetLineRenderer, this._nodeArray);
  }

  private Vector3[] RefreshNodeArray(
    int numDivisions,
    AnimationClip sourceAnimClip,
    GameObject nodeAvatarParent,
    GameObject nodeAvatar,
    float animTime,
    float trailMaxTimeLag,
    float cutoffTime)
  {
    float length1 = sourceAnimClip.length;
    float num1 = !(bool) (UnityEngine.Object) this._runningAnimator ? (float) ((double) animTime * (double) length1 + ((double) Time.time - (double) cutoffTime)) : animTime * length1;
    float num2 = Mathf.Clamp(num1, 0.0f, length1);
    float num3 = Mathf.Clamp(num1 - trailMaxTimeLag, 0.0f, length1);
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

  private void RestructureLine(LineRenderer lineRenderer, Vector3[] nodeArray)
  {
    lineRenderer.positionCount = nodeArray.Length;
    lineRenderer.SetPositions(nodeArray);
  }

  private float GetAnimationPosition(Animator runningAnimator)
  {
    float animationPosition = 0.0f;
    if (runningAnimator.GetCurrentAnimatorClipInfoCount(0) > 0)
      animationPosition = runningAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    return animationPosition;
  }
}
