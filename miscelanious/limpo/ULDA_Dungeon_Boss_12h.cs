using System.Collections.Generic;

public class ULDA_Dungeon_Boss_12h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_12h_Pyramad_Death_01 = new AssetReference("VO_ULDA_BOSS_12h_Pyramad_Death_01.prefab:bb52f62014ecf00469de65df3542ed24");
  private static readonly AssetReference VO_ULDA_BOSS_12h_Pyramad_Defeat_01 = new AssetReference("VO_ULDA_BOSS_12h_Pyramad_Defeat_01.prefab:f458e10d636144343a7c2c0bd4b679cf");
  private static readonly AssetReference VO_ULDA_BOSS_12h_Pyramad_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_12h_Pyramad_EmoteResponse_01.prefab:36182e8e51464de4a98dec5e14d53c57");
  private static readonly AssetReference VO_ULDA_BOSS_12h_Pyramad_Intro_01 = new AssetReference("VO_ULDA_BOSS_12h_Pyramad_Intro_01.prefab:5e899eb20b55dd545b6e41eef82efc79");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_12h.VO_ULDA_BOSS_12h_Pyramad_Death_01,
      (string) ULDA_Dungeon_Boss_12h.VO_ULDA_BOSS_12h_Pyramad_Defeat_01,
      (string) ULDA_Dungeon_Boss_12h.VO_ULDA_BOSS_12h_Pyramad_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_12h.VO_ULDA_BOSS_12h_Pyramad_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_12h.VO_ULDA_BOSS_12h_Pyramad_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_12h.VO_ULDA_BOSS_12h_Pyramad_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_12h.VO_ULDA_BOSS_12h_Pyramad_EmoteResponse_01;
  }
}
