using UnityEngine;

public class OnAnimationExitEvent : StateMachineBehaviour
{
  public string animationName;
  public float exitOffset;
  private float timeElapsed;
  private bool exitEventInvoked;

  public override void OnStateUpdate(
    Animator animator,
    AnimatorStateInfo stateInfo,
    int layerIndex)
  {
    base.OnStateUpdate(animator, stateInfo, layerIndex);
    this.timeElapsed += Time.deltaTime;
    if ((double) stateInfo.length - (double) this.timeElapsed > (double) this.exitOffset || this.exitEventInvoked)
      return;
    GameUtils.OnAnimationExitEvent.Invoke(this.animationName);
    this.exitEventInvoked = true;
  }

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    if (this.exitEventInvoked)
      return;
    GameUtils.OnAnimationExitEvent.Invoke(this.animationName);
    this.exitEventInvoked = true;
  }
}
