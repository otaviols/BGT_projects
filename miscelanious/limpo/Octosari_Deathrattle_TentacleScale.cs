using UnityEngine;

public class Octosari_Deathrattle_TentacleScale : MonoBehaviour
{
  public GameObject[] bones;
  public AnimationCurve boneAimingWeights;
  public Vector3 boneStretchingMul = Vector3.up;
  public AnimationCurve boneStretchingWeights;
  public AnimationCurve stretchingByTargetDistance;
  public AnimationCurve deformAnimation;
  public Animation animComponent;
  public Transform tentacleTarget;
  private AnimationState animState;
  private int bonesCount;
  private float boneWeightSampler;
  private float stretchingByDistanceMul;

  private void Start()
  {
    this.animState = this.animComponent[this.animComponent.clip.name];
    this.bonesCount = this.bones.Length;
    this.boneWeightSampler = this.bonesCount < 2 ? 1f : (float) (this.bonesCount - 1);
  }

  private void LateUpdate()
  {
    if ((TrackedReference) this.animState == (TrackedReference) null || !this.animComponent.isPlaying || (double) this.animState.time == 0.0 || (double) this.stretchingByDistanceMul == 0.0)
      return;
    float num1 = this.deformAnimation.Evaluate(this.animState.normalizedTime);
    int num2 = 0;
    foreach (GameObject bone in this.bones)
    {
      float num3 = this.boneStretchingWeights.Evaluate((float) num2 / this.boneWeightSampler);
      float num4 = this.boneAimingWeights.Evaluate((float) num2 / this.boneWeightSampler);
      Transform transform = bone.transform;
      Vector3 localPosition = transform.localPosition;
      Vector3 vector3 = localPosition + Vector3.Scale(localPosition, num3 * num1 * this.boneStretchingMul * this.stretchingByDistanceMul);
      bone.transform.localPosition = vector3;
      Vector3 up = transform.up;
      Vector3 toDirection = this.tentacleTarget.position - transform.position;
      Quaternion rotation = transform.rotation;
      rotation.SetFromToRotation(up, toDirection);
      bone.transform.rotation = Quaternion.Lerp(transform.rotation, rotation * bone.transform.rotation, num1 * num4);
      ++num2;
    }
  }

  public void Setup() => this.stretchingByDistanceMul = this.stretchingByTargetDistance.Evaluate(Vector3.Distance(this.tentacleTarget.position, this.transform.position));
}
