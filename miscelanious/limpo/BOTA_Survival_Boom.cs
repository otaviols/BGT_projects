using System.Collections;
using System.Collections.Generic;

public class BOTA_Survival_Boom : BOTA_MissionEntity
{
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Failure_01 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Failure_01.prefab:b20d52247bbde0d42bbefc64782157b5");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Failure_02 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Failure_02.prefab:52e836691533e3c4088fdb10776b729b");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Failure_04 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Failure_04.prefab:2e4544fa8c22f884bac0ac39c8f532c2");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Failure_05 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Failure_05.prefab:fe6e5edff239439468bbb9010eacd983");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Failure_06 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Failure_06.prefab:e790bfa98d7ddb74c98291acd203a3aa");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Idle_01 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Idle_01.prefab:828de7f730eb81b46888d3b574abbd08");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Idle_02 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Idle_02.prefab:b6a9def10b3457f49b9af0e8a1a77a60");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Idle_03 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Idle_03.prefab:f42a1a17fb2fde249a467e44d4ad212a");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Idle_04 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Idle_04.prefab:649dafe9e6a4b4842a43754f6e28a5ef");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Idle_05 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Idle_05.prefab:2c5259eedbae90d4783c3fec86f31445");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Idle_06 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Idle_06.prefab:baa804a050242ce458f50095a1dae149");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_02 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_02.prefab:96fa445d8b983f14a8411a2d6d34f5d8");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_04 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_04.prefab:189f72a1be840df43a88b50b971327ee");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_11 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_11.prefab:8adaa50a0eb7df349bc61382eb1c059c");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_18 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_18.prefab:51395490635b9e5479b827925921d3fb");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_12 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_12.prefab:d8a8edc6248318147bfd375b0204ee96");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_17 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_17.prefab:3772b50d815a43c4bbeaf0f9f8204687");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Survival_01 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Survival_01.prefab:89dfe02d2bbcdf94291beb2d0ed29c10");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Survival_02 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Survival_02.prefab:34fb848d6a6c0cd47bc6886ec765948a");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_UI_Boom_Coin4_Victory_01 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_UI_Boom_Coin4_Victory_01.prefab:214eb2193a8e2f74c939bc6660323069");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_08 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_08.prefab:456ba8208dd03b14bbaddb494d71b925");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_01 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_01.prefab:fac7082fdc86c6f4b88f2541be028b90");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_01,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_02,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_04,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_05,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_06,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_01,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_02,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_03,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_04,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_05,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_06,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_02,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_04,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_08,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_11,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_18,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_12,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_17,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Survival_01,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Survival_02,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_UI_Boom_Coin4_Victory_01,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_01
    })
      this.PreloadSound(soundPath);
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BOTFinalBoss);

  protected override float ChanceToPlayRandomVOLine() => 1f;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    BOTA_MissionEntity.s_introLine = (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_01;
    BOTA_MissionEntity.s_returnLine = (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_08;
    this.s_victoryLine_1 = (string) null;
    this.s_victoryLine_2 = (string) null;
    this.s_victoryLine_3 = (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Survival_02;
    this.s_victoryLine_4 = (string) null;
    this.s_victoryLine_5 = (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Survival_01;
    this.s_victoryLine_6 = (string) null;
    this.s_victoryLine_7 = (string) null;
    this.s_victoryLine_8 = (string) null;
    this.s_victoryLine_9 = (string) null;
    this.s_emoteLines = new List<string>()
    {
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_12,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_17
    };
    this.s_idleLines = new List<string>()
    {
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_01,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_02,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_03,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_04,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_05,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_06,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_18
    };
    this.s_restartLines = new List<string>()
    {
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_01,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_02,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_04,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_05,
      (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_06
    };
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    yield break;
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    BOTA_Survival_Boom botaSurvivalBoom = this;
    if (gameResult == TAG_PLAYSTATE.WON)
      yield return (object) botaSurvivalBoom.PlayClosingLine("DrBoom_Banner_Quote.prefab:ff8653a27a00c464ea5552e3c6b6dc5c", (string) BOTA_Survival_Boom.VO_BOTA_BOSS_20h_Male_Goblin_UI_Boom_Coin4_Victory_01);
  }
}
