using UnityEngine;

public class RotateAnim : MonoBehaviour
{
  private Quaternion targetRotation;
  private bool gogogo;
  private float timeValue;
  private float timePassed;
  private float startingAngle;

  private void Update()
  {
    if (!this.gogogo)
      return;
    this.timePassed += Time.deltaTime;
    float timePassed = this.timePassed;
    float startingAngle = this.startingAngle;
    double num1 = (double) startingAngle - (double) Quaternion.Angle(this.transform.rotation, this.targetRotation);
    float timeValue = this.timeValue;
    double num2 = -(double) Mathf.Pow(2f, -10f * timePassed / timeValue) + 1.0;
    this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, this.targetRotation, ((float) (num1 * num2) + startingAngle) * Time.deltaTime);
    if ((double) Quaternion.Angle(this.transform.rotation, this.targetRotation) > (double) Mathf.Epsilon)
      return;
    this.gogogo = false;
    Object.Destroy((Object) this);
  }

  public void SetTargetRotation(Vector3 target, float timeValueInput)
  {
    this.targetRotation = Quaternion.Euler(target);
    this.gogogo = true;
    this.timeValue = timeValueInput;
    this.startingAngle = Quaternion.Angle(this.transform.rotation, this.targetRotation);
  }
}
