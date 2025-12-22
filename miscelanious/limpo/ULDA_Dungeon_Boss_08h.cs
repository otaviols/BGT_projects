using System.Collections.Generic;

public class ULDA_Dungeon_Boss_08h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_08h_Sinkhole_Death_01 = new AssetReference("VO_ULDA_BOSS_08h_Sinkhole_Death_01.prefab:9e2b2e5c9540e944bab799821028f45e");
  private static readonly AssetReference VO_ULDA_BOSS_08h_Sinkhole_Defeat_01 = new AssetReference("VO_ULDA_BOSS_08h_Sinkhole_Defeat_01.prefab:b4a60e175c00cc34dabf890eb56ea2f9");
  private static readonly AssetReference VO_ULDA_BOSS_08h_Sinkhole_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_08h_Sinkhole_EmoteResponse_01.prefab:c5d7d9c3b51340f40bad0e4ca7ca46d4");
  private static readonly AssetReference VO_ULDA_BOSS_08h_Sinkhole_Intro_01 = new AssetReference("VO_ULDA_BOSS_08h_Sinkhole_Intro_01.prefab:3190fbf165c4a28419dc3315a20e69f7");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_08h.VO_ULDA_BOSS_08h_Sinkhole_Death_01,
      (string) ULDA_Dungeon_Boss_08h.VO_ULDA_BOSS_08h_Sinkhole_Defeat_01,
      (string) ULDA_Dungeon_Boss_08h.VO_ULDA_BOSS_08h_Sinkhole_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_08h.VO_ULDA_BOSS_08h_Sinkhole_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_08h.VO_ULDA_BOSS_08h_Sinkhole_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_08h.VO_ULDA_BOSS_08h_Sinkhole_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_08h.VO_ULDA_BOSS_08h_Sinkhole_EmoteResponse_01;
  }
}
