using UnityEngine;

public class Tournament
{
  private static Tournament s_instance;

  public static void Init()
  {
    if (Tournament.s_instance != null)
      return;
    Tournament.s_instance = new Tournament();
  }

  public static Tournament Get()
  {
    if (Tournament.s_instance == null)
      Debug.LogError((object) "Trying to retrieve the Tournament without calling Tournament.Init()!");
    return Tournament.s_instance;
  }

  public void NotifyOfBoxTransitionStart() => Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));

  public void OnBoxTransitionFinished(object userData)
  {
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    if (Options.Get().GetBool(Option.HAS_SEEN_TOURNAMENT, false))
      return;
    Options.Get().SetBool(Option.HAS_SEEN_TOURNAMENT, true);
  }
}
