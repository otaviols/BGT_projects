using UnityEngine;

public class SpotlightConstraint : MonoBehaviour
{
  public GameObject SpotlightSource;
  public GameObject SpotlightTarget;
  public GameObject GroundCircle;
  private float maxLeftPosition = 14f;
  private float maxLeftRotationTarget = 50f;
  private float maxLeftCircleScale = 0.25f;
  private float targetMultiplier;
  private float circleMultiplier;
  private Vector3 eulerRotation;
  private Vector3 originalTargetPosition;

  private void OnEnable()
  {
    if (!(bool) (Object) this.SpotlightSource || !(bool) (Object) this.SpotlightTarget || !(bool) (Object) this.GroundCircle)
      return;
    this.targetMultiplier = (float) ((double) this.maxLeftRotationTarget / (double) this.maxLeftPosition * -1.0);
    this.circleMultiplier = (float) ((double) this.maxLeftCircleScale / (double) this.maxLeftPosition * -1.0);
  }

  private void Update()
  {
    this.originalTargetPosition = this.SpotlightTarget.transform.position;
    this.SpotlightTarget.transform.position = this.originalTargetPosition;
    this.eulerRotation = new Vector3(0.0f, this.targetMultiplier * this.SpotlightTarget.transform.position.x, 0.0f);
    this.SpotlightTarget.transform.localRotation = Quaternion.Euler(this.eulerRotation);
    this.eulerRotation = new Vector3(0.0f, 0.0f, 0.0f);
    this.GroundCircle.transform.rotation = Quaternion.Euler(this.eulerRotation);
    this.GroundCircle.transform.localScale = new Vector3((float) ((double) this.circleMultiplier * (double) this.SpotlightTarget.transform.position.x + 1.0), 1f, 1f);
  }
}
