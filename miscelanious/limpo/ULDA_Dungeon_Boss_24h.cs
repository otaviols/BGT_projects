using System.Collections.Generic;

public class ULDA_Dungeon_Boss_24h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_24h_SuspiciousPalmTree_Death = new AssetReference("VO_ULDA_BOSS_24h_SuspiciousPalmTree_Death.prefab:9ec4a17f58bb4d6459cd0b3d4d180076");
  private static readonly AssetReference VO_ULDA_BOSS_24h_SuspiciousPalmTree_Defeat = new AssetReference("VO_ULDA_BOSS_24h_SuspiciousPalmTree_Defeat.prefab:1ebe6b8b74eb9b141a24ca6ceb8a0bb2");
  private static readonly AssetReference VO_ULDA_BOSS_24h_SuspiciousPalmTree_EmoteResponse = new AssetReference("VO_ULDA_BOSS_24h_SuspiciousPalmTree_EmoteResponse.prefab:8225a92d06879954f8830734c5f973a9");
  private static readonly AssetReference VO_ULDA_BOSS_24h_SuspiciousPalmTree_Intro = new AssetReference("VO_ULDA_BOSS_24h_SuspiciousPalmTree_Intro.prefab:dd27a8380dbcf394ab87ca1bdd26243e");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_24h.VO_ULDA_BOSS_24h_SuspiciousPalmTree_Death,
      (string) ULDA_Dungeon_Boss_24h.VO_ULDA_BOSS_24h_SuspiciousPalmTree_Defeat,
      (string) ULDA_Dungeon_Boss_24h.VO_ULDA_BOSS_24h_SuspiciousPalmTree_EmoteResponse,
      (string) ULDA_Dungeon_Boss_24h.VO_ULDA_BOSS_24h_SuspiciousPalmTree_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_24h.VO_ULDA_BOSS_24h_SuspiciousPalmTree_Intro;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_24h.VO_ULDA_BOSS_24h_SuspiciousPalmTree_Death;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_24h.VO_ULDA_BOSS_24h_SuspiciousPalmTree_EmoteResponse;
  }
}
