using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLK_Prologue_Fight_002 : RLK_Prologue_Dungeon
{
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_02_A.prefab:f459120a7b592ae44a3e5342c00596f0");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_A.prefab:5556f9ef8e44acc4aa354f6f05e4f330");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_C = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_C.prefab:30b9ca815425fd746a044aa9121396dc");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_03_02_B = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_03_02_B.prefab:d7026c60516dd014c97317e519ab9d53");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_09_02_B = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_09_02_B.prefab:d563cc61e01f1904c94a6b771e39802c");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_17_02_B = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_17_02_B.prefab:6b0d8cc0cfee3c747ad5e35b42a7f5d6");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Arthas_InGame_VictoryPreExplosion_02_B = new AssetReference("VO_RLK_Prologue_Male_Human_Arthas_InGame_VictoryPreExplosion_02_B.prefab:eec2da0f199898f44b4baf5328a47e97");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_A.prefab:88b7986d4ce97904792fb49a38a29038");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_B = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_B.prefab:2200d0bd065e939499112ef4905c1855");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_C = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_C.prefab:6f12e912a75174e4cbcdc698d0a6c0e7");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_EmoteResponse_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_EmoteResponse_02_A.prefab:f8bce042a8aa21d49b145e41abc4b225");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_Introduction_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_Introduction_02_A.prefab:9b690cc86354cb14799eb1ca9b6fc15c");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_LossPostExplosion_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_LossPostExplosion_02_A.prefab:03f6e340032402c4284f1b10452c9703");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_PlayerPlaysCard_02_B = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_PlayerPlaysCard_02_B.prefab:37e16093e92adbb42a2b669fd9a5ab7e");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_PlayerPlaysCard_02_C = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_PlayerPlaysCard_02_C.prefab:4acb69a96b646994f91bab872e108844");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_PlayerPlaysCard_02_D = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_PlayerPlaysCard_02_D.prefab:eb2fffa18c634e44ab9b3dbe53bd7118");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_03_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_03_02_A.prefab:1cb0764cda4524a4bb3e54fdeeeea420");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_09_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_09_02_A.prefab:c64156a39fd91cb4a9c25cee034080ab");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_17_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_17_02_A.prefab:da9db0582f511b94785d0f6a7a0a4e5e");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_VictoryPostExplosion_02_C = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_VictoryPostExplosion_02_C.prefab:6a4eb15653c8e26498da73a6c68d38a3");
  private static readonly AssetReference VO_RLK_Prologue_Male_Human_Uther_InGame_VictoryPreExplosion_02_A = new AssetReference("VO_RLK_Prologue_Male_Human_Uther_InGame_VictoryPreExplosion_02_A.prefab:7b2fc20b094baf64fbdc0c27d0dfc78f");
  private List<string> m_InGame_BossIdleLines = new List<string>()
  {
    (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_A,
    (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_B,
    (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_C
  };
  private Notification m_popup;
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      1,
      new string[1]
      {
        nameof (VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_C)
      }
    }
  };
  private float popUpScale = 1f;
  private Vector3 popUpPos;
  private Notification handBounceArrow;
  private bool bhasMinionDeathBeenTriggered;
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_02_A,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_A,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_C,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_03_02_B,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_09_02_B,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_17_02_B,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_VictoryPreExplosion_02_B,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_A,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_B,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_BossIdle_02_C,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_EmoteResponse_02_A,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_Introduction_02_A,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_LossPostExplosion_02_A,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_PlayerPlaysCard_02_B,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_PlayerPlaysCard_02_C,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_PlayerPlaysCard_02_D,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_03_02_A,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_09_02_A,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_17_02_A,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_VictoryPostExplosion_02_C,
      (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_VictoryPreExplosion_02_A
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame() => base.OnCreateGame();

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  protected Actor GetEnemyActorByCardId(string cardId)
  {
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    foreach (Card card in opposingSidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == opposingSidePlayer.GetPlayerId() && entity.GetCardId() == cardId)
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
  }

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
    Vector3 position2 = !bShowInHandZone ? (!(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x + 4.9f, position1.y + 1f, position1.z + 1f) : new Vector3(position1.x - 0.08f, position1.y + 0.2f, position1.z + 1.2f)) : (!(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x + 3.9f, position1.y + 1f, position1.z + 1f) : new Vector3(position1.x - 0.08f, position1.y + 0.2f, position1.z + 1.2f));
    this.handBounceArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0.0f, 0.0f, 0.0f));
    this.handBounceArrow.transform.parent = card.transform;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    RLK_Prologue_Fight_002 prologueFight002 = this;
    while (prologueFight002.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) prologueFight002.MissionPlayVO(actor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_LossPostExplosion_02_A);
        GameState.Get().SetBusy(false);
        break;
      case 514:
        yield return (object) prologueFight002.MissionPlayVO(actor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_Introduction_02_A);
        break;
      case 515:
        yield return (object) prologueFight002.MissionPlayVO(actor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_EmoteResponse_02_A);
        break;
      case 517:
        yield return (object) prologueFight002.MissionPlayVO(actor, prologueFight002.m_InGame_BossIdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) prologueFight002.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    RLK_Prologue_Fight_002 prologueFight002 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) prologueFight002.\u003C\u003En__1(entity);
    while (prologueFight002.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!prologueFight002.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) prologueFight002.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      prologueFight002.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "RLK_Prologue_RLK_079") && !(cardId == "RLK_Prologue_RLK_730") && !(cardId == "RLK_Prologue_RLK_012"))
      {
        int num = cardId == "RLK_Prologue_RLK_071" ? 1 : 0;
      }
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    RLK_Prologue_Fight_002 prologueFight002 = this;
    if ((Object) prologueFight002.handBounceArrow != (Object) null)
    {
      NotificationManager.Get().DestroyNotification(prologueFight002.handBounceArrow, 0.0f);
      prologueFight002.handBounceArrow = (Notification) null;
    }
    while (prologueFight002.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        GameState gameState = GameState.Get();
        if (gameState == null)
        {
          Debug.LogError((object) string.Format("RLK_Prologue.HandleMissionEventWithTiming(): GameState is null"));
          break;
        }
        if ((Object) prologueFight002.m_popup != (Object) null)
          break;
        if (prologueFight002.m_popUpInfo == null)
        {
          Debug.LogError((object) string.Format("RLK_Prologue.HandleMissionEventWithTiming(): m_popUpInfo is null"));
          break;
        }
        string[] strArray = (string[]) null;
        if (!prologueFight002.m_popUpInfo.TryGetValue(turn, out strArray))
        {
          Debug.LogError((object) string.Format("RLK_Prologue.HandleMissionEventWithTiming(): gameStringKeys is null"));
          break;
        }
        if (strArray.Length == 0)
        {
          Debug.LogError((object) string.Format("RLK_Prologue.HandleMissionEventWithTiming(): gameStringKeys is empty"));
          break;
        }
        NotificationManager notificationManager = NotificationManager.Get();
        if ((Object) notificationManager == (Object) null)
        {
          Debug.LogError((object) string.Format("RLK_Prologue.HandleMissionEventWithTiming(): notificationManager is null"));
          break;
        }
        gameState.SetBusy(true);
        Gameplay.Get().StartCoroutine(prologueFight002.ShowArrowInSeconds(0.0f, true));
        string key = strArray[0];
        prologueFight002.m_popup = notificationManager.CreatePopupText(UserAttentionBlocker.NONE, prologueFight002.popUpPos, TutorialEntity.GetTextScale() * prologueFight002.popUpScale, GameStrings.Get(key), false, NotificationManager.PopupTextType.FANCY);
        yield return (object) GameEntity.Coroutines.StartCoroutine(prologueFight002.PlaySoundAndBlockSpeech((string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_PlayerPlaysCard_02_C, Notification.SpeechBubbleDirection.None, prologueFight002.GetEnemyActorByCardId("RLK_Prologue_Arthas_002p"), 2.5f));
        notificationManager.DestroyNotification(prologueFight002.m_popup, 0.0f);
        prologueFight002.m_popup = (Notification) null;
        NotificationManager.Get().DestroyNotification(prologueFight002.handBounceArrow, 0.0f);
        prologueFight002.handBounceArrow = (Notification) null;
        gameState.SetBusy(false);
        gameState = (GameState) null;
        notificationManager = (NotificationManager) null;
        break;
      case 3:
        GameState.Get().SetBusy(true);
        yield return (object) prologueFight002.MissionPlayVO(actor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_03_02_A);
        yield return (object) prologueFight002.MissionPlayVO(friendlyActor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_03_02_B);
        GameState.Get().SetBusy(false);
        break;
      case 9:
        yield return (object) prologueFight002.MissionPlayVO(actor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_09_02_A);
        yield return (object) prologueFight002.MissionPlayVO(friendlyActor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_09_02_B);
        break;
      case 15:
        yield return (object) prologueFight002.MissionPlayVO(actor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Uther_InGame_Turn_17_02_A);
        yield return (object) prologueFight002.MissionPlayVO(friendlyActor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_Turn_17_02_B);
        break;
    }
  }

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    RLK_Prologue_Fight_002 prologueFight002 = this;
    string cardId = entity.GetCardId();
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (cardId)
    {
      case "HERO_11bpt":
      case "RLK_Prologue_066":
      case "RLK_Prologue_503":
      case "RLK_Prologue_RLK_071":
      case "RLK_Prologue_RLK_079":
      case "RLK_Prologue_RLK_082":
      case "RLK_Prologue_RLK_506":
      case "RLK_Prologue_RLK_708":
      case "RLK_Prologue_RLK_711":
      case "RLK_Prologue_RLK_720":
      case "RLK_Prologue_RLK_731":
      case "RLK_Prologue_RLK_741":
        if (prologueFight002.bhasMinionDeathBeenTriggered)
          break;
        GameState.Get().SetBusy(true);
        Gameplay.Get().StartCoroutine(prologueFight002.ShowArrowInSeconds(0.0f, false));
        prologueFight002.bhasMinionDeathBeenTriggered = true;
        yield return (object) prologueFight002.MissionPlayVO(actor, (string) RLK_Prologue_Fight_002.VO_RLK_Prologue_Male_Human_Arthas_InGame_MinionDies_02_A);
        GameState.Get().SetBusy(false);
        break;
    }
  }
}
