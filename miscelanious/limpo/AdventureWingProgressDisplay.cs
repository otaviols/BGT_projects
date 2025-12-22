using UnityEngine;

[CustomEditClass]
public class AdventureWingProgressDisplay : MonoBehaviour
{
  public virtual void UpdateProgress(WingDbId wingDbId, bool linearComplete)
  {
  }

  public virtual bool HasProgressAnimationToPlay() => false;

  public virtual void PlayProgressAnimation(
    AdventureWingProgressDisplay.OnAnimationComplete onAnimComplete = null)
  {
    if (onAnimComplete == null)
      return;
    onAnimComplete();
  }

  public delegate void OnAnimationComplete();
}
