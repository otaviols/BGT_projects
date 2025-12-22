using System.Collections.Generic;

public class ULDA_Dungeon_Boss_46h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_46h_Wildtooth_Death_01 = new AssetReference("VO_ULDA_BOSS_46h_Wildtooth_Death_01.prefab:7d6ad06fd86ea684086550ecf03c5e2d");
  private static readonly AssetReference VO_ULDA_BOSS_46h_Wildtooth_Defeat_01 = new AssetReference("VO_ULDA_BOSS_46h_Wildtooth_Defeat_01.prefab:1765447744be14f479c800f7db28e409");
  private static readonly AssetReference VO_ULDA_BOSS_46h_Wildtooth_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_46h_Wildtooth_EmoteResponse_01.prefab:05176adcc37f2624ababa70c786a6ab8");
  private static readonly AssetReference VO_ULDA_BOSS_46h_Wildtooth_Intro_01 = new AssetReference("VO_ULDA_BOSS_46h_Wildtooth_Intro_01.prefab:0ce9dbdc00dc2ff45b5b79858b68fd91");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_46h.VO_ULDA_BOSS_46h_Wildtooth_Death_01,
      (string) ULDA_Dungeon_Boss_46h.VO_ULDA_BOSS_46h_Wildtooth_Defeat_01,
      (string) ULDA_Dungeon_Boss_46h.VO_ULDA_BOSS_46h_Wildtooth_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_46h.VO_ULDA_BOSS_46h_Wildtooth_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_46h.VO_ULDA_BOSS_46h_Wildtooth_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_46h.VO_ULDA_BOSS_46h_Wildtooth_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_46h.VO_ULDA_BOSS_46h_Wildtooth_EmoteResponse_01;
  }
}
