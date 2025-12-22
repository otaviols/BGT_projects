using System.Collections.Generic;

public class ULDA_Dungeon_Boss_05h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_05h_GlacktheScorpid_Death_01 = new AssetReference("VO_ULDA_BOSS_05h_GlacktheScorpid_Death_01.prefab:80f4e0c700dc8ae4a9aaa1aee5c5f3d8");
  private static readonly AssetReference VO_ULDA_BOSS_05h_GlacktheScorpid_Defeat_01 = new AssetReference("VO_ULDA_BOSS_05h_GlacktheScorpid_Defeat_01.prefab:2c2aabfc5d4923449933ca8f1e85fae2");
  private static readonly AssetReference VO_ULDA_BOSS_05h_GlacktheScorpid_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_05h_GlacktheScorpid_EmoteResponse_01.prefab:272e4dfd2fe844e499eca3388e20d193");
  private static readonly AssetReference VO_ULDA_BOSS_05h_GlacktheScorpid_Intro_01 = new AssetReference("VO_ULDA_BOSS_05h_GlacktheScorpid_Intro_01.prefab:96e665030602e2640a4af2a3e40701db");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_05h.VO_ULDA_BOSS_05h_GlacktheScorpid_Death_01,
      (string) ULDA_Dungeon_Boss_05h.VO_ULDA_BOSS_05h_GlacktheScorpid_Defeat_01,
      (string) ULDA_Dungeon_Boss_05h.VO_ULDA_BOSS_05h_GlacktheScorpid_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_05h.VO_ULDA_BOSS_05h_GlacktheScorpid_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_05h.VO_ULDA_BOSS_05h_GlacktheScorpid_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_05h.VO_ULDA_BOSS_05h_GlacktheScorpid_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_05h.VO_ULDA_BOSS_05h_GlacktheScorpid_EmoteResponse_01;
  }
}
