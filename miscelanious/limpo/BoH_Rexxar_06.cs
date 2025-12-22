using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Rexxar_06 : BoH_Rexxar_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Rexxar_06.InitBooleanOptions();
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Death_01 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Death_01.prefab:7399820ffe8358c4d9e62a99c7f4537a");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6EmoteResponse_01 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6EmoteResponse_01.prefab:e236430c43795544aad7a3cfee0dab7e");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6ExchangeB_01 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6ExchangeB_01.prefab:3b646d2702651c44cb1866c65a8922aa");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6ExchangeC_01 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6ExchangeC_01.prefab:1b569d23197f05742b82066210283a7e");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_01 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_01.prefab:036cc7e9be96f0a44818344b218288fd");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_02 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_02.prefab:c0e26a8cbacc4f9489868d741c6baf14");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_03 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_03.prefab:ed3cab9b40a442440a20ed4f0ed53afb");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_01 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_01.prefab:3fdcbfa6a3f475e46a88e8cacc1222ea");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_02 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_02.prefab:f683936bee3988645bea7c0a56aec9f6");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_03 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_03.prefab:e0ed4fbff1fd0e149b678f6a03b96ac3");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Intro_01 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Intro_01.prefab:61123b7c46905db4cb4e8476c1388d22");
  private static readonly AssetReference VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Loss_01 = new AssetReference("VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Loss_01.prefab:41b4841110cfa3d4eb0aabedc873fc70");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeA_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeA_01.prefab:d1e1dc742d7fec144bbb3d185f8004c8");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeB_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeB_01.prefab:69aca0cd9ccea0548bdadb969652f4c2");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeC_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeC_01.prefab:d5f9a3d8ee2f5b946933655f5059c6cc");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6Intro_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6Intro_01.prefab:8062f8f2b27a4d749a942ae31826cae7");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6Victory_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6Victory_01.prefab:3ae4b33ffab3b864caa9935c5cb61f1b");
  private static readonly AssetReference VO_Story_Minion_Baine_Male_Tauren_Story_Rexxar_Mission6ExchangeA_01 = new AssetReference("VO_Story_Minion_Baine_Male_Tauren_Story_Rexxar_Mission6ExchangeA_01.prefab:70c5f8b2a0d6e924a9189e97232a2344");
  private static readonly AssetReference VO_Story_Minion_Baine_Male_Tauren_Story_Rexxar_Mission6Victory_01 = new AssetReference("VO_Story_Minion_Baine_Male_Tauren_Story_Rexxar_Mission6Victory_01.prefab:1f7a8dd5eae9dfd4187243bf1f26de84");
  private List<string> m_VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPowerLines = new List<string>()
  {
    (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_01,
    (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_02,
    (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_03
  };
  private List<string> m_VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6IdleLines = new List<string>()
  {
    (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_01,
    (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_02,
    (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Rexxar_06() => this.m_gameOptions.AddBooleanOptions(BoH_Rexxar_06.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Death_01,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6EmoteResponse_01,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6ExchangeB_01,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6ExchangeC_01,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_01,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_02,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPower_03,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_01,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_02,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Idle_03,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Intro_01,
      (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Loss_01,
      (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeA_01,
      (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeB_01,
      (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeC_01,
      (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6Intro_01,
      (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6Victory_01,
      (string) BoH_Rexxar_06.VO_Story_Minion_Baine_Male_Tauren_Story_Rexxar_Mission6ExchangeA_01,
      (string) BoH_Rexxar_06.VO_Story_Minion_Baine_Male_Tauren_Story_Rexxar_Mission6Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Rexxar_06 boHRexxar06 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHRexxar06.PlayLineAlways(actor, (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6Intro_01);
    yield return (object) boHRexxar06.PlayLineAlways(enemyActor, (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START || !MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Rexxar_06 boHRexxar06 = this;
    while (boHRexxar06.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) boHRexxar06.PlayLineAlways(actor2, (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6Victory_01);
        Actor enemyActorByCardId = boHRexxar06.GetEnemyActorByCardId("Story_02_Baine");
        if ((Object) enemyActorByCardId != (Object) null)
          yield return (object) boHRexxar06.PlayLineAlways(enemyActorByCardId, (string) BoH_Rexxar_06.VO_Story_Minion_Baine_Male_Tauren_Story_Rexxar_Mission6Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHRexxar06.PlayLineAlways(actor1, (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHRexxar06.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Rexxar_06 boHRexxar06 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHRexxar06.\u003C\u003En__1(entity);
    while (boHRexxar06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHRexxar06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHRexxar06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHRexxar06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Rexxar_06 boHRexxar06 = this;
    while (boHRexxar06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHRexxar06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHRexxar06.\u003C\u003En__2(entity);
      yield return (object) boHRexxar06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHRexxar06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Rexxar_06 boHRexxar06 = this;
    while (boHRexxar06.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        Actor enemyActorByCardId = boHRexxar06.GetEnemyActorByCardId("Story_02_Baine");
        if ((Object) enemyActorByCardId != (Object) null)
          yield return (object) boHRexxar06.PlayLineAlways(enemyActorByCardId, (string) BoH_Rexxar_06.VO_Story_Minion_Baine_Male_Tauren_Story_Rexxar_Mission6ExchangeA_01);
        yield return (object) boHRexxar06.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeA_01);
        break;
      case 3:
        yield return (object) boHRexxar06.PlayLineAlways(actor, (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6ExchangeB_01);
        yield return (object) boHRexxar06.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeB_01);
        break;
      case 13:
        yield return (object) boHRexxar06.PlayLineAlways(actor, (string) BoH_Rexxar_06.VO_Story_02_Centaur_Male_Centaur_Story_Rexxar_Mission6ExchangeC_01);
        yield return (object) boHRexxar06.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_06.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission6ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DRG);
}
