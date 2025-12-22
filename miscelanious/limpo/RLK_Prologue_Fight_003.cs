using System.Collections;
using System.Collections.Generic;

public class RLK_Prologue_Fight_003 : RLK_Prologue_Dungeon
{
  private static readonly AssetReference VO_RLK_Prologue_Female_0_SylvanasB_InGame_VictoryPostExplosion_03_D = new AssetReference("VO_RLK_Prologue_Female_0_SylvanasB_InGame_VictoryPostExplosion_03_D.prefab:ae44555335740a84097039bdc175aed1");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossAttacksSpecial_03_A = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossAttacksSpecial_03_A.prefab:71e17eb965c6dcb449032e9e4a642696");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_A = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_A.prefab:ace836842a1262944bcc5557ab332986");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_B = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_B.prefab:81459bd0ad489944fa9ecc12e0cb627d");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_C = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_C.prefab:11066963a1854294682760d085901405");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_EmoteResponse_03_A = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_EmoteResponse_03_A.prefab:b5435a309d76f634db7a8238f8e85672");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Introduction_03_B = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Introduction_03_B.prefab:c264aacc1381d414a9fab4262264a03c");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_LossPreExplosion_03_A = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_LossPreExplosion_03_A.prefab:622366fad4c33f54e9880c8f4195ade1");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_MinionDies_03_B = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_MinionDies_03_B.prefab:be84a1a2512a3564aa0a9b1438474374");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_MinionDies_03_D = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_MinionDies_03_D.prefab:7a6faaf0603bf4d45b88649d2455855f");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_PlayerPlaysCard_03_A = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_PlayerPlaysCard_03_A.prefab:74ea36f41f43ad04c875525e3c1b85fd");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_03_03_B = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_03_03_B.prefab:36dfc8bc743187e48a38c20dcd4183ad");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_11_03_A = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_11_03_A.prefab:a9f0058406ff31747a01a0b0bae0cf96");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_15_03_A = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_15_03_A.prefab:bad93a693439c314fab6717695f425bf");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_VictoryPreExplosion_03_A = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_VictoryPreExplosion_03_A.prefab:44f4800e282acc845981522a2a987076");
  private static readonly AssetReference VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_VictoryPreExplosion_03_C = new AssetReference("VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_VictoryPreExplosion_03_C.prefab:ee7ec7b972fa1f14badcbc0c1a85ddd5");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_BossAttacksSpecial_03_B = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_BossAttacksSpecial_03_B.prefab:d08f020a8b8d2cb4aab0bddb7aafbece");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_Introduction_03_A = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_Introduction_03_A.prefab:c6d519d962d942541978bff85c9de203");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_03 = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_03.prefab:98059650b37618648b06f63ff23f8430");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_03_C = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_03_C.prefab:961f7d5fff7897e42a3484960ae4db0d");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_03_B = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_03_B.prefab:f85fedb54049a524982a8cdbc020235e");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_03_C = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_03_C.prefab:8beb49a59075a9b419d382d2576821e5");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_03_03_A = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_03_03_A.prefab:eb1cdd1cda68da14abdde841cb3fde2d");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_11_03_B = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_11_03_B.prefab:595b83d2da4030c4babc741ff01847e4");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_15_03_B = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_15_03_B.prefab:33073b5982ba062478aa694078fdb959");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_VictoryPreExplosion_03_B = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_VictoryPreExplosion_03_B.prefab:48e8a67d6ee7f494b84804aa8b8d1d07");
  private List<string> m_InGame_BossIdleLines = new List<string>()
  {
    (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_A,
    (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_B,
    (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_C
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_0_SylvanasB_InGame_VictoryPostExplosion_03_D,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossAttacksSpecial_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_B,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_BossIdle_03_C,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_EmoteResponse_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Introduction_03_B,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_LossPreExplosion_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_MinionDies_03_B,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_MinionDies_03_D,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_PlayerPlaysCard_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_03_03_B,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_11_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_15_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_VictoryPreExplosion_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_VictoryPreExplosion_03_C,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_BossAttacksSpecial_03_B,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_Introduction_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_03,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_03_C,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_03_B,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_03_C,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_03_03_A,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_11_03_B,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_15_03_B,
      (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_VictoryPreExplosion_03_B
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame() => base.OnCreateGame();

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    RLK_Prologue_Fight_003 prologueFight003 = this;
    while (prologueFight003.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) prologueFight003.MissionPlayVO(enemyActor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_LossPreExplosion_03_A);
        GameState.Get().SetBusy(false);
        break;
      case 514:
        yield return (object) prologueFight003.MissionPlayVO(actor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_Introduction_03_A);
        yield return (object) prologueFight003.MissionPlayVO(enemyActor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Introduction_03_B);
        break;
      case 515:
        yield return (object) prologueFight003.MissionPlayVO(enemyActor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_EmoteResponse_03_A);
        break;
      case 517:
        yield return (object) prologueFight003.MissionPlayVO(enemyActor, prologueFight003.m_InGame_BossIdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) prologueFight003.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    RLK_Prologue_Fight_003 prologueFight003 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) prologueFight003.\u003C\u003En__1(entity);
    while (prologueFight003.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!prologueFight003.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) prologueFight003.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      prologueFight003.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "RLK_048"))
      {
        int num = cardId == "RLK_122" ? 1 : 0;
      }
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    RLK_Prologue_Fight_003 prologueFight003 = this;
    while (prologueFight003.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) prologueFight003.MissionPlayVO(friendlyActor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_03_03_A);
        yield return (object) prologueFight003.MissionPlayVO(enemyActor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_03_03_B);
        break;
      case 7:
        yield return (object) prologueFight003.MissionPlayVO(enemyActor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_11_03_A);
        yield return (object) prologueFight003.MissionPlayVO(friendlyActor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_11_03_B);
        break;
      case 13:
        yield return (object) prologueFight003.MissionPlayVO(enemyActor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Female_BloodElf_Sylvanas_InGame_Turn_15_03_A);
        yield return (object) prologueFight003.MissionPlayVO(friendlyActor, (string) RLK_Prologue_Fight_003.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_15_03_B);
        break;
    }
  }
}
