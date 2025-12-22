using System.Collections;
using UnityEngine;

public class VillagePackOpeningDisplay : AbsSceneDisplay
{
  public PackOpening m_packOpeningView;

  public override void Start()
  {
    base.Start();
    if ((Object) this.m_packOpeningView != (Object) null)
      this.m_packOpeningView.SetVillageDisplay(this);
    this.StartCoroutine(this.InitializeWhenReady());
  }

  private IEnumerator InitializeWhenReady()
  {
    VillagePackOpeningDisplay packOpeningDisplay = this;
    while (!packOpeningDisplay.IsFinishedLoading(out string _))
      yield return (object) null;
    if ((Object) packOpeningDisplay.m_packOpeningView != (Object) null)
      packOpeningDisplay.m_packOpeningView.Show();
  }

  public void NavigateBack() => this.SetNextModeAndHandleTransition(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.CURRENT_SCENE, (object) null);

  public void PreunloadPackOpeningView()
  {
    if (!((Object) this.m_packOpeningView != (Object) null))
      return;
    this.m_packOpeningView.PreUnload();
  }

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if ((Object) this.m_packOpeningView == (Object) null || !this.m_packOpeningView.IsReady())
    {
      failureMessage = "VillagePackOpeningDisplay - Display never loaded.";
      return false;
    }
    failureMessage = string.Empty;
    return true;
  }

  protected override bool ShouldStartShown() => false;
}
