using System.Collections;
using System.Collections.Generic;

public class BOTA_Clear_Boom : BOTA_MissionEntity
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
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Clear_02 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Clear_02.prefab:9fb97d28f74b0e44a8f1d4650b753e05");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_01 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_01.prefab:fac7082fdc86c6f4b88f2541be028b90");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_UI_Boom_Coin1_End_01 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_UI_Boom_Coin1_End_01.prefab:a1877ed45c945ae4dbef3c731515cade");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_15 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_15.prefab:bc29bc9c1caa41e49a3f209c8688164d");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_13 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_13.prefab:37a96caf5c628f048b251cbd9891237d");
  private static readonly AssetReference VO_BOTA_BOSS_20h_Male_Goblin_Trigger_Thermaplug_01 = new AssetReference("VO_BOTA_BOSS_20h_Male_Goblin_Trigger_Thermaplug_01.prefab:9532f6101d8da4044bd980f4fe0aa7bc");
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_01,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_02,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_04,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_05,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_06,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_01,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_02,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_03,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_04,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_05,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_06,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_02,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_04,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_11,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_18,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_12,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_17,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Clear_02,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_01,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_UI_Boom_Coin1_End_01,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Trigger_Thermaplug_01,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_15,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_13
    })
      this.PreloadSound(soundPath);
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BOTFinalBoss);

  protected override float ChanceToPlayRandomVOLine() => 1f;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    BOTA_MissionEntity.s_introLine = (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_01;
    BOTA_MissionEntity.s_returnLine = (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_13;
    this.s_victoryLine_1 = (string) null;
    this.s_victoryLine_2 = (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Clear_02;
    this.s_victoryLine_3 = (string) null;
    this.s_victoryLine_4 = (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_15;
    this.s_victoryLine_5 = (string) null;
    this.s_victoryLine_6 = (string) null;
    this.s_victoryLine_7 = (string) null;
    this.s_victoryLine_8 = (string) null;
    this.s_victoryLine_9 = (string) null;
    this.s_emoteLines = new List<string>()
    {
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_12,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_17
    };
    this.s_idleLines = new List<string>()
    {
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_01,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_02,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_03,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_04,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_05,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Idle_06
    };
    this.s_restartLines = new List<string>()
    {
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_01,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_02,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_04,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_05,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Failure_06,
      (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_One_Liner_18
    };
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    yield break;
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOTA_Clear_Boom botaClearBoom = this;
    while (botaClearBoom.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!botaClearBoom.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) botaClearBoom.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      botaClearBoom.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "GVG_116")
        yield return (object) botaClearBoom.PlayEasterEggLine(actor, (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_Trigger_Thermaplug_01);
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    BOTA_Clear_Boom botaClearBoom = this;
    if (gameResult == TAG_PLAYSTATE.WON)
      yield return (object) botaClearBoom.PlayClosingLine("DrBoom_Banner_Quote.prefab:ff8653a27a00c464ea5552e3c6b6dc5c", (string) BOTA_Clear_Boom.VO_BOTA_BOSS_20h_Male_Goblin_UI_Boom_Coin1_End_01);
  }
}
