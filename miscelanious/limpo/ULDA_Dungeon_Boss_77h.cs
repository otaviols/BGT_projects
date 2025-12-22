using System.Collections.Generic;

public class ULDA_Dungeon_Boss_77h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_77h_Gorebite_Death = new AssetReference("VO_ULDA_BOSS_77h_Gorebite_Death.prefab:bcf74f8cd0e88da44808ae3d950a1a8e");
  private static readonly AssetReference VO_ULDA_BOSS_77h_Gorebite_Defeat = new AssetReference("VO_ULDA_BOSS_77h_Gorebite_Defeat.prefab:ff8a8435d7634fd449d35e21c17ba1b1");
  private static readonly AssetReference VO_ULDA_BOSS_77h_Gorebite_EmoteResponse = new AssetReference("VO_ULDA_BOSS_77h_Gorebite_EmoteResponse.prefab:99bdf7ab67fea5c4381e1b85c2f56e8d");
  private static readonly AssetReference VO_ULDA_BOSS_77h_Gorebite_Intro = new AssetReference("VO_ULDA_BOSS_77h_Gorebite_Intro.prefab:c12fbd3ed62fc3c4dad6746ee04256d2");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_77h.VO_ULDA_BOSS_77h_Gorebite_Death,
      (string) ULDA_Dungeon_Boss_77h.VO_ULDA_BOSS_77h_Gorebite_Defeat,
      (string) ULDA_Dungeon_Boss_77h.VO_ULDA_BOSS_77h_Gorebite_EmoteResponse,
      (string) ULDA_Dungeon_Boss_77h.VO_ULDA_BOSS_77h_Gorebite_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_77h.VO_ULDA_BOSS_77h_Gorebite_Intro;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_77h.VO_ULDA_BOSS_77h_Gorebite_Death;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_77h.VO_ULDA_BOSS_77h_Gorebite_EmoteResponse;
  }
}
