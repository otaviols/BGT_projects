using System.Collections.Generic;

public class ULDA_Dungeon_Boss_56h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_56h_SandPufferFrog_Death = new AssetReference("VO_ULDA_BOSS_56h_SandPufferFrog_Death.prefab:e5742d5456598d749bdb29f175198b88");
  private static readonly AssetReference VO_ULDA_BOSS_56h_SandPufferFrog_Defeat = new AssetReference("VO_ULDA_BOSS_56h_SandPufferFrog_Defeat.prefab:9f51d3d202e6a3649aa997c823a07a3a");
  private static readonly AssetReference VO_ULDA_BOSS_56h_SandPufferFrog_EmoteResponse = new AssetReference("VO_ULDA_BOSS_56h_SandPufferFrog_EmoteResponse.prefab:cd4e9275be8335b4a9d59199b2bd1529");
  private static readonly AssetReference VO_ULDA_BOSS_56h_SandPufferFrog_Intro = new AssetReference("VO_ULDA_BOSS_56h_SandPufferFrog_Intro.prefab:9abcb5b8aa24dab4aa82e517520983f0");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_56h.VO_ULDA_BOSS_56h_SandPufferFrog_Death,
      (string) ULDA_Dungeon_Boss_56h.VO_ULDA_BOSS_56h_SandPufferFrog_Defeat,
      (string) ULDA_Dungeon_Boss_56h.VO_ULDA_BOSS_56h_SandPufferFrog_EmoteResponse,
      (string) ULDA_Dungeon_Boss_56h.VO_ULDA_BOSS_56h_SandPufferFrog_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_56h.VO_ULDA_BOSS_56h_SandPufferFrog_Intro;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_56h.VO_ULDA_BOSS_56h_SandPufferFrog_Death;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_56h.VO_ULDA_BOSS_56h_SandPufferFrog_EmoteResponse;
  }
}
