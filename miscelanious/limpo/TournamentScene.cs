using UnityEngine;

public class TournamentScene : PlayGameScene
{
  private static TournamentScene s_instance;

  protected override void Awake()
  {
    base.Awake();
    TournamentScene.s_instance = this;
  }

  private void OnDestroy() => TournamentScene.s_instance = (TournamentScene) null;

  public static TournamentScene Get() => TournamentScene.s_instance;

  public override string GetScreenPath() => "Tournament.prefab:e6cb7fa773178834ebff4e16c3847ede";

  public override void Unload()
  {
    base.Unload();
    if (!((Object) TournamentDisplay.Get() != (Object) null))
      return;
    TournamentDisplay.Get().SceneUnload();
  }
}
