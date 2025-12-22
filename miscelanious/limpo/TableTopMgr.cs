using UnityEngine;

public class TableTopMgr : MonoBehaviour
{
  public MeshRenderer m_renderer;
  private bool m_active = true;

  private void Start()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_active = false;
      this.enabled = false;
    }
    else if (GameUtils.CanCheckTutorialCompletion() && GameUtils.IsAnyTutorialComplete())
    {
      this.DisableTableTopRenderer();
      this.m_active = false;
      this.enabled = false;
    }
    else
      Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
  }

  private void OnBoxTransitionFinished(object userData)
  {
    if (!this.m_active)
      return;
    if (GameUtils.CanCheckTutorialCompletion() && GameUtils.IsAnyTutorialComplete())
    {
      Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
      this.DisableTableTopRenderer();
      this.m_active = false;
      this.enabled = false;
    }
    else
    {
      if (Box.Get().GetState() != Box.State.HUB)
        return;
      this.EnableTableTopRenderer();
    }
  }

  public void HideTableTop() => this.DisableTableTopRenderer();

  private void EnableTableTopRenderer() => this.m_renderer.enabled = true;

  private void DisableTableTopRenderer() => this.m_renderer.enabled = false;
}
