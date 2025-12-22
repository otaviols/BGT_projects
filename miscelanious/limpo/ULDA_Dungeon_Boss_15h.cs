using System.Collections.Generic;

public class ULDA_Dungeon_Boss_15h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_15h_LtHerring_Death = new AssetReference("VO_ULDA_BOSS_15h_LtHerring_Death.prefab:c83a9095f36dc6345a027dda71d4119b");
  private static readonly AssetReference VO_ULDA_BOSS_15h_LtHerring_Defeat = new AssetReference("VO_ULDA_BOSS_15h_LtHerring_Defeat.prefab:b2cf070e6c096e3449670a7e1c3cf9b4");
  private static readonly AssetReference VO_ULDA_BOSS_15h_LtHerring_EmoteResponse = new AssetReference("VO_ULDA_BOSS_15h_LtHerring_EmoteResponse.prefab:d00b38d0eaf13f74da257b97cf48b585");
  private static readonly AssetReference VO_ULDA_BOSS_15h_LtHerring_Intro = new AssetReference("VO_ULDA_BOSS_15h_LtHerring_Intro.prefab:420a245aa0c72d046bf5520b97619fa4");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_15h.VO_ULDA_BOSS_15h_LtHerring_Death,
      (string) ULDA_Dungeon_Boss_15h.VO_ULDA_BOSS_15h_LtHerring_Defeat,
      (string) ULDA_Dungeon_Boss_15h.VO_ULDA_BOSS_15h_LtHerring_EmoteResponse,
      (string) ULDA_Dungeon_Boss_15h.VO_ULDA_BOSS_15h_LtHerring_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_15h.VO_ULDA_BOSS_15h_LtHerring_Intro;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_15h.VO_ULDA_BOSS_15h_LtHerring_Death;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_15h.VO_ULDA_BOSS_15h_LtHerring_EmoteResponse;
  }
}
