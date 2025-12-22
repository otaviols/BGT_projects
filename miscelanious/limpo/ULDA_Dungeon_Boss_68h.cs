using System.Collections.Generic;

public class ULDA_Dungeon_Boss_68h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_68h_WeaponizedWasp_Death = new AssetReference("VO_ULDA_BOSS_68h_WeaponizedWasp_Death.prefab:baa76151643f4c549a8e96cf03f40ea1");
  private static readonly AssetReference VO_ULDA_BOSS_68h_WeaponizedWasp_EmoteResponse = new AssetReference("VO_ULDA_BOSS_68h_WeaponizedWasp_EmoteResponse.prefab:e7376ecf5e990ee499abef9a55d0609e");
  private static readonly AssetReference VO_ULDA_BOSS_68h_WeaponizedWasp_Intro = new AssetReference("VO_ULDA_BOSS_68h_WeaponizedWasp_Intro.prefab:8819bc9647b9c2e45bb90e3e929fd7f1");
  private static readonly AssetReference VO_ULDA_BOSS_68h_WeaponizedWasp_PlayerDefeat = new AssetReference("VO_ULDA_BOSS_68h_WeaponizedWasp_PlayerDefeat.prefab:0be271f5e99692c49908ea8b0d93b16a");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_68h.VO_ULDA_BOSS_68h_WeaponizedWasp_Death,
      (string) ULDA_Dungeon_Boss_68h.VO_ULDA_BOSS_68h_WeaponizedWasp_PlayerDefeat,
      (string) ULDA_Dungeon_Boss_68h.VO_ULDA_BOSS_68h_WeaponizedWasp_EmoteResponse,
      (string) ULDA_Dungeon_Boss_68h.VO_ULDA_BOSS_68h_WeaponizedWasp_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_68h.VO_ULDA_BOSS_68h_WeaponizedWasp_Intro;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_68h.VO_ULDA_BOSS_68h_WeaponizedWasp_Death;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_68h.VO_ULDA_BOSS_68h_WeaponizedWasp_EmoteResponse;
  }
}
