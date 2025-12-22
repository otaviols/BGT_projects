using UnityEngine;

public class PhasedMissionEntity : MissionEntity
{
  public string m_PopupPrefabNameAndGUID = "PhaseProgress_Next.prefab:7013b28700033444c9f20897a59edaa0";
  public string m_PopupText = "GAMEPLAY_RESTART_PUZZLES";
  private ScreenEffectsHandle m_screenEffectsHandle;

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    if (GameState.Get().GetGameEntity().GetTag(GAME_TAG.PHASED_RESTART) == 1)
    {
      this.PhaseComplete();
      GameState.Get().Restart();
    }
    else
      base.NotifyOfGameOver(gameResult);
  }

  public virtual void PhaseComplete()
  {
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.DesaturatePerspective);
    foreach (UberText componentsInChild in AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_PopupPrefabNameAndGUID, AssetLoadingOptions.IgnorePrefabPosition).GetComponentsInChildren<UberText>())
      componentsInChild.SetText(GameStrings.Get(this.m_PopupText));
    GameObject go = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_PopupPrefabNameAndGUID, AssetLoadingOptions.IgnorePrefabPosition);
    foreach (UberText componentsInChild in go.GetComponentsInChildren<UberText>())
      componentsInChild.SetText(GameStrings.Get(this.m_PopupText));
    LayerUtils.SetLayer(go, 0);
  }

  public PhasedMissionEntity()
    : base()
  {
  }
}
