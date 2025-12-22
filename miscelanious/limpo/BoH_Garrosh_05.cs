using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Garrosh_05 : BoH_Garrosh_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5EmoteResponse_01 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5EmoteResponse_01.prefab:8a442b65b8b89104cbfdcbb6d007e943");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeA_01 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeA_01.prefab:bed4da777801956469f487b4871635f4");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeB_01 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeB_01.prefab:01687dfe5439a304685f5e9f36e152ad");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeC_01 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeC_01.prefab:9e77297304529fb46bbad170a005a7a5");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_01 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_01.prefab:346ba20dab46b7b49bca132912144509");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_02 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_02.prefab:d2248704836d7fd47accc6973322bbec");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_03 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_03.prefab:dfd2c6022b3fc2342880ed125ef8d350");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_01 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_01.prefab:87b9604355dd0054a8200353f0096208");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_02 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_02.prefab:03cb5bed9238c62449be0658841e7cbe");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_03 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_03.prefab:11ae308a9a4ace94582da826fbad5a8d");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Intro_01.prefab:b640fe272b39e46449d763cee2151aa8");
  private static readonly AssetReference VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Loss_01 = new AssetReference("VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Loss_01.prefab:e2ddc12b95ad89e4881e66c83038a6b3");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeA_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeA_01.prefab:2b64f188ec70c65489293a74dd9aeee2");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeB_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeB_01.prefab:91ebcabe52f94be48b92c2e26c0c3b2c");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeC_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeC_01.prefab:1da5209a13e850b4da9031bf93ab7976");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Intro_01.prefab:6f05bc3ace322794ca0d7fd33fe3bba5");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Victory_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Victory_01.prefab:3273df884dc30174fae6344cb2c8f327");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Victory_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Victory_02.prefab:1aabae45355658d4abca449ab6e6fe72");
  private static readonly AssetReference VO_Story_Minion_Magatha_Female_Tauren_Story_Minion_Magatha_Play_01 = new AssetReference("VO_Story_Minion_Magatha_Female_Tauren_Story_Minion_Magatha_Play_01.prefab:58f37ce2a8384ff099e289452fe17886");
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      228,
      new string[1]{ "BOH_GARROSH_05" }
    }
  };
  private Player friendlySidePlayer;
  private Entity playerEntity;
  private float popUpScale = 1.25f;
  private Vector3 popUpPos;
  private Notification StartPopup;
  private List<string> m_VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPowerLines = new List<string>()
  {
    (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_01,
    (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_02,
    (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_03
  };
  private List<string> m_VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5IdleLines = new List<string>()
  {
    (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_01,
    (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_02,
    (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5EmoteResponse_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeA_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeB_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeC_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_02,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPower_03,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_02,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Idle_03,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Intro_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Loss_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeA_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeB_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeC_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Intro_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Victory_01,
      (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Victory_02,
      (string) BoH_Garrosh_05.VO_Story_Minion_Magatha_Female_Tauren_Story_Minion_Magatha_Play_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  private void Start() => this.friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();

  private void SetPopupPosition()
  {
    if (this.friendlySidePlayer.IsCurrentPlayer())
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.popUpPos.z = -66f;
      else
        this.popUpPos.z = -44f;
    }
    else if ((bool) UniversalInputManager.UsePhoneUI)
      this.popUpPos.z = 66f;
    else
      this.popUpPos.z = 44f;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Garrosh_05 boHGarrosh05 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHGarrosh05.PlayLineAlways(actor, (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Intro_01);
    yield return (object) boHGarrosh05.PlayLineAlways(enemyActor, (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5HeroPowerLines;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Garrosh_05 boHGarrosh05 = this;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHGarrosh05.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      while (boHGarrosh05.m_enemySpeaking)
        yield return (object) null;
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      boHGarrosh05.popUpPos = new Vector3(0.0f, 0.0f, -40f);
      switch (missionEvent)
      {
        case 101:
          yield return (object) new WaitForSeconds(4f);
          break;
        case 228:
          Notification popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, boHGarrosh05.popUpPos, TutorialEntity.GetTextScale() * boHGarrosh05.popUpScale, GameStrings.Get(boHGarrosh05.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
          yield return (object) new WaitForSeconds(3.5f);
          NotificationManager.Get().DestroyNotification(popup, 0.0f);
          popup = (Notification) null;
          break;
        case 501:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh05.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Victory_01);
          yield return (object) new WaitForSeconds(1f);
          yield return (object) boHGarrosh05.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5Victory_02);
          GameState.Get().SetBusy(false);
          break;
        case 502:
          GameState.Get().SetBusy(true);
          Actor enemyActorByCardId = boHGarrosh05.GetEnemyActorByCardId("Story_03_Magatha");
          if ((Object) enemyActorByCardId != (Object) null)
            yield return (object) boHGarrosh05.PlayLineAlways(enemyActorByCardId, (string) BoH_Garrosh_05.VO_Story_Minion_Magatha_Female_Tauren_Story_Minion_Magatha_Play_01);
          GameState.Get().SetBusy(false);
          break;
        case 504:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh05.PlayLineAlways(actor, (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5Loss_01);
          GameState.Get().SetBusy(false);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHGarrosh05.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_05 boHGarrosh05 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHGarrosh05.\u003C\u003En__1(entity);
    while (boHGarrosh05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHGarrosh05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_05 boHGarrosh05 = this;
    while (boHGarrosh05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHGarrosh05.\u003C\u003En__2(entity);
      yield return (object) boHGarrosh05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Garrosh_05 boHGarrosh05 = this;
    while (boHGarrosh05.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) boHGarrosh05.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeA_01);
        yield return (object) boHGarrosh05.PlayLineAlways(enemyActor, (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeA_01);
        break;
      case 7:
        yield return (object) boHGarrosh05.PlayLineAlways(enemyActor, (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeB_01);
        yield return (object) boHGarrosh05.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeB_01);
        break;
      case 11:
        yield return (object) boHGarrosh05.PlayLineAlways(enemyActor, (string) BoH_Garrosh_05.VO_Story_Hero_Cairne_Male_Tauren_Story_Garrosh_Mission5ExchangeC_01);
        yield return (object) boHGarrosh05.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_05.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission5ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_GILFinalBoss);

  private IEnumerator ShowPopup(string displayString)
  {
    this.StartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.GetTextScale(), GameStrings.Get(displayString), false);
    NotificationManager.Get().DestroyNotification(this.StartPopup, 7f);
    GameState.Get().SetBusy(true);
    yield return (object) new WaitForSeconds(2f);
    GameState.Get().SetBusy(false);
  }
}
