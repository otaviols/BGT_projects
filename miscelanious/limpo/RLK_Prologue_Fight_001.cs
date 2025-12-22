using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLK_Prologue_Fight_001 : RLK_Prologue_Dungeon
{
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_A = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_A.prefab:6696de438b2e2a846b2a8257724195ad");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_B = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_B.prefab:ba84510098f174949abc4b58f752a7ae");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_C = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_C.prefab:adf9c48492b0cdc4dad710f9ab6f20ff");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_EmoteResponse_01_A = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_EmoteResponse_01_A.prefab:e688e2ea28b353d458d9ac4f69ab091a");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Introduction_01_A = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Introduction_01_A.prefab:5465cc9d040f805478b954b13c53c0c5");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_LossPreExplosion_01_A = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_LossPreExplosion_01_A.prefab:68f2c44379dafa14c92cfd4efcef66b7");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_PlayerEquipWeapon_01_A = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_PlayerEquipWeapon_01_A.prefab:ca8a9a4b152b50448b57f39729cb597e");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_PlayerEquipWeapon_01_B = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_PlayerEquipWeapon_01_B.prefab:a45fb4257871bc146a4e8ad3996351d0");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_03_01_A = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_03_01_A.prefab:e7f17b0fc90268a4481044a7c9c37814");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_13_01_A = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_13_01_A.prefab:0a68baafa500edd4c86d5e8b6c9468cd");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_17_01_A = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_17_01_A.prefab:f5aa61179fdac1846b4281d47c8ac8a0");
  private static readonly AssetReference VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_17_01_C = new AssetReference("VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_17_01_C.prefab:6677a4be013f37d4890cf3d1de361199");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_BossPlaysCard_01_A = new AssetReference("VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_BossPlaysCard_01_A.prefab:1b504c9cbe0deef4da19ab0f66d10faf");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_BossPlaysCard_01_B = new AssetReference("VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_BossPlaysCard_01_B.prefab:ffecc5b161037b74ab6e039f3d49e88c");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_BossPlaysCard_01_C = new AssetReference("VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_BossPlaysCard_01_C.prefab:4f6d87cff377cff4fb7339c8febe2c3e");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Introduction_01_B = new AssetReference("VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Introduction_01_B.prefab:6b3fa288337068547b4f5083c86d7b5d");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_PlayerEquipWeapon_01_C = new AssetReference("VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_PlayerEquipWeapon_01_C.prefab:2280334433b0c1b41b1153835014fc81");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_03_01_B = new AssetReference("VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_03_01_B.prefab:d68d2a072b2659e44bda07c54ae6d218");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_13_01_B = new AssetReference("VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_13_01_B.prefab:821ca612c0820434ea387d412f37e2bf");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_17_01_B = new AssetReference("VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_17_01_B.prefab:b636bb264791b2a449b8cae4ff370dca");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_VictoryPostExplosion_01_A = new AssetReference("VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_VictoryPostExplosion_01_A.prefab:b8ed793990f3b10419bae56dfb66ca6c");
  private static readonly AssetReference VO_RLK_Prologue_Arthas_002hp_Male_Human_InGame_VictoryPostExplosion_03 = new AssetReference("VO_RLK_Prologue_Arthas_002hp_Male_Human_InGame_VictoryPostExplosion_03.prefab:0f3fec7474be6454d99bc3debd465798");
  private static readonly AssetReference VO_RLK_Prologue_FroznThrn_004hb3_Male_Undead_InGame_VictoryPostExplosion_02 = new AssetReference("VO_RLK_Prologue_FroznThrn_004hb3_Male_Undead_InGame_VictoryPostExplosion_02.prefab:04917d252bc82f648b273d2705820515");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_A.prefab:5556f9ef8e44acc4aa354f6f05e4f330");
  private List<string> m_InGame_BossIdleLines = new List<string>()
  {
    (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_A,
    (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_B,
    (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_C
  };
  private Notification handBounceArrow;
  private bool bhasMinionDeathBeenTriggered;
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_B,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_BossIdle_01_C,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_EmoteResponse_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Introduction_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_LossPreExplosion_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_PlayerEquipWeapon_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_PlayerEquipWeapon_01_B,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_03_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_13_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_17_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_17_01_C,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_BossPlaysCard_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_BossPlaysCard_01_B,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_BossPlaysCard_01_C,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Introduction_01_B,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_PlayerEquipWeapon_01_C,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_03_01_B,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_13_01_B,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_17_01_B,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_VictoryPostExplosion_01_A,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Arthas_002hp_Male_Human_InGame_VictoryPostExplosion_03,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_FroznThrn_004hb3_Male_Undead_InGame_VictoryPostExplosion_02,
      (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_A
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame() => base.OnCreateGame();

  private IEnumerator ShowArrowInSeconds(float seconds, bool bShowInHandZone)
  {
    yield return (object) new WaitForSeconds(seconds);
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    if (cards.Count != 0)
    {
      Card cardInHand = cards[0];
      while (iTween.Count(cardInHand.gameObject) > 0)
        yield return (object) null;
      if (!cardInHand.IsMousedOver() && !((Object) InputManager.Get().GetHeldCard() == (Object) cardInHand))
        this.ShowHandBouncingArrow(bShowInHandZone);
    }
  }

  private void ShowHandBouncingArrow(bool bShowInHandZone)
  {
    if ((Object) this.handBounceArrow != (Object) null)
      return;
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
    if (cards.Count == 0)
      return;
    Card card = cards[0];
    Vector3 position1 = card.transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x + 4.1f, position1.y + 1f, position1.z + 8.75f) : new Vector3(position1.x - 0.08f, position1.y + 0.2f, position1.z + 1.2f);
    this.handBounceArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0.0f, 180f, 0.0f));
    this.handBounceArrow.transform.parent = card.transform;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    RLK_Prologue_Fight_001 prologueFight001 = this;
    while (prologueFight001.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        if (!((Object) prologueFight001.handBounceArrow != (Object) null))
          break;
        NotificationManager.Get().DestroyNotification(prologueFight001.handBounceArrow, 0.0f);
        prologueFight001.handBounceArrow = (Notification) null;
        break;
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) prologueFight001.MissionPlayVO(actor, (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_LossPreExplosion_01_A);
        GameState.Get().SetBusy(false);
        break;
      case 514:
        yield return (object) prologueFight001.MissionPlayVO(actor, (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Introduction_01_A);
        yield return (object) prologueFight001.MissionPlayVO(friendlyActor, (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Introduction_01_B);
        break;
      case 515:
        yield return (object) prologueFight001.MissionPlayVO(actor, (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_EmoteResponse_01_A);
        break;
      case 517:
        yield return (object) prologueFight001.MissionPlayVO(actor, prologueFight001.m_InGame_BossIdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) prologueFight001.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    RLK_Prologue_Fight_001 prologueFight001 = this;
    while (prologueFight001.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!prologueFight001.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) prologueFight001.\u003C\u003En__1(entity);
      yield return (object) prologueFight001.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      prologueFight001.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "RLK_Prologue_503" && !prologueFight001.bhasMinionDeathBeenTriggered)
      {
        Gameplay.Get().StartCoroutine(prologueFight001.ShowArrowInSeconds(0.0f, false));
        prologueFight001.bhasMinionDeathBeenTriggered = true;
        yield return (object) prologueFight001.MissionPlayVO(actor, (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_A);
      }
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    RLK_Prologue_Fight_001 prologueFight001 = this;
    while (prologueFight001.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if ((Object) prologueFight001.handBounceArrow != (Object) null)
    {
      NotificationManager.Get().DestroyNotification(prologueFight001.handBounceArrow, 0.0f);
      prologueFight001.handBounceArrow = (Notification) null;
    }
    switch (turn)
    {
      case 3:
        yield return (object) prologueFight001.MissionPlayVO(actor, (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_03_01_A);
        yield return (object) prologueFight001.MissionPlayVO(friendlyActor, (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_03_01_B);
        break;
      case 13:
        yield return (object) prologueFight001.MissionPlayVO(actor, (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Demon_MalGanis_InGame_Turn_13_01_A);
        yield return (object) prologueFight001.MissionPlayVO(friendlyActor, (string) RLK_Prologue_Fight_001.VO_RLK_Prologue_Male_Human_ArthasMenethil_InGame_Turn_13_01_B);
        break;
    }
  }
}
