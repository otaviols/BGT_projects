using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureSubScene : MonoBehaviour
{
  [CustomEditField(Sections = "Animation Settings")]
  public float m_TransitionAnimationTime = 1f;
  [CustomEditField(Sections = "Bounds Settings")]
  public Vector3_MobileOverride m_SubSceneBounds;
  [CustomEditField(Sections = "Transition Settings")]
  public bool m_reverseTransitionAfterThisSubscene;
  public bool m_reverseTransitionBeforeThisSubscene;
  private bool m_IsLoaded;
  private List<AdventureSubScene.SubSceneTransitionFinished> m_SubSceneTransitionListeners = new List<AdventureSubScene.SubSceneTransitionFinished>();

  public void SetIsLoaded(bool loaded) => this.m_IsLoaded = loaded;

  public bool IsLoaded() => this.m_IsLoaded;

  public void AddSubSceneTransitionFinishedListener(AdventureSubScene.SubSceneTransitionFinished dlg) => this.m_SubSceneTransitionListeners.Add(dlg);

  public void RemoveSubSceneTransitionFinishedListener(
    AdventureSubScene.SubSceneTransitionFinished dlg)
  {
    this.m_SubSceneTransitionListeners.Remove(dlg);
  }

  public void NotifyTransitionComplete() => this.FireSubSceneTransitionFinishedEvent();

  private void FireSubSceneTransitionFinishedEvent()
  {
    foreach (AdventureSubScene.SubSceneTransitionFinished transitionFinished in this.m_SubSceneTransitionListeners.ToArray())
      transitionFinished();
  }

  public delegate void SubSceneTransitionFinished();
}
