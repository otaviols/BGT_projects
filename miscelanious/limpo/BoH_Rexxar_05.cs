using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Rexxar_05 : BoH_Rexxar_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Rexxar_05.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Daelin_Male_Human_Story_Rexxar_Mission5Victory_01 = new AssetReference("VO_Story_Hero_Daelin_Male_Human_Story_Rexxar_Mission5Victory_01.prefab:997fcc78b0cb3fe4fbec72709bcee854");
  private static readonly AssetReference VO_Story_Hero_Daelin_Male_Human_Story_Rexxar_Mission5Victory_02 = new AssetReference("VO_Story_Hero_Daelin_Male_Human_Story_Rexxar_Mission5Victory_02.prefab:b70268c69b6fcfe44a884a2332281f05");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Death_01 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Death_01.prefab:2f1d78ced905b924a8b7e095df3072f6");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5EmoteResponse_01 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5EmoteResponse_01.prefab:728c0896ee6c3ae43991f81dd9cd21fa");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5ExchangeF_01 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5ExchangeF_01.prefab:6fb03e885e5140a47a705acd75232b89");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5ExchangeG_01 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5ExchangeG_01.prefab:6d2ecdfadf3bce347878adecd0b587d0");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_01 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_01.prefab:87e8d2cb6f399ee46a2b5bf1d9500eb0");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_02 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_02.prefab:a1a24e27312657741b588fe92ac39f86");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_03 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_03.prefab:30be6009789d5af41996148c83158ad3");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_01 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_01.prefab:d3baf9d334cc64c4291fdc03ca7082c3");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_02 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_02.prefab:ddc3334a445dc6d49bb758de4924bad4");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_03 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_03.prefab:132abd92e75104748b9f0e53df4e4eb7");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Intro_01.prefab:94bd06ace9e7b2446829a7a780a7f9f0");
  private static readonly AssetReference VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Loss_01 = new AssetReference("VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Loss_01.prefab:6679e11bb7f9d704dbdbd23961d754ef");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeA_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeA_01.prefab:fa0da701f910f7c4b8405a9da4992d55");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeB_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeB_01.prefab:b240c16eba247d248b5d5e1a3e295422");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeC_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeC_01.prefab:3b7fab7a81b8ee941931847b27517c20");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeD_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeD_01.prefab:1432f5c6ded7ab64aa0b200dd057e7f2");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeE_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeE_01.prefab:0228474dc52500d4bb83de6d1295a2c0");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeE_02 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeE_02.prefab:8bc801ed88bd6bf40bfe597daae5f6a0");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Intro_01.prefab:bc321e0aa483b2b4bb0cdcddb89f3ff4");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Victory_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Victory_01.prefab:1a6e39f4d9cd0b849904b3f0e974bb46");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Victory_02 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Victory_02.prefab:a320a54cbadebf44db61d3a51ebbde8b");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeB_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeB_01.prefab:8fe4e45a1df13dc4f94043292557f3b2");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeC_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeC_01.prefab:4da7e895d72989e459030bacd0263096");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeD_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeD_01.prefab:09f65539eb4190a4eb5517a7df375319");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeE_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeE_01.prefab:a281acc43a6ef4d4a9eb83996b93fc28");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5Intro_01.prefab:8f4adabd70a55eb4aaf239e3d5616081");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5Victory_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5Victory_01.prefab:5f8dcfef5a39fa945bef952cc027b04d");
  private static readonly AssetReference VO_Story_Minion_Footman_Male_Human_Story_Footman_Play_01 = new AssetReference("VO_Story_Minion_Footman_Male_Human_Story_Footman_Play_01.prefab:82d107a16b46519499058165c6d0f7f6");
  public static readonly AssetReference DaelinBrassRing = new AssetReference("Daelin_BrassRing_Quote.prefab:8553800b28758a44da69e1cd9bdacf07");
  public static readonly AssetReference JainaBrassRing = new AssetReference("JainaMid_BrassRing_Quote.prefab:7eba171d881f6764e81abddbb125bb19");
  private List<string> m_VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPowerLines = new List<string>()
  {
    (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_01,
    (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_02,
    (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_03
  };
  private List<string> m_VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5IdleLines = new List<string>()
  {
    (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_01,
    (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_02,
    (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Rexxar_05() => this.m_gameOptions.AddBooleanOptions(BoH_Rexxar_05.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Rexxar_05.VO_Story_Hero_Daelin_Male_Human_Story_Rexxar_Mission5Victory_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Daelin_Male_Human_Story_Rexxar_Mission5Victory_02,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Death_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5EmoteResponse_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5ExchangeF_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5ExchangeG_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_02,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPower_03,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_02,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Idle_03,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Intro_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Loss_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeA_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeB_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeC_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeD_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeE_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeE_02,
      (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Intro_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Victory_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Victory_02,
      (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeB_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeC_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeD_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeE_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5Intro_01,
      (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5Victory_01,
      (string) BoH_Rexxar_05.VO_Story_Minion_Footman_Male_Human_Story_Footman_Play_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Rexxar_05 boHRexxar05 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHRexxar05.PlayLineAlways(actor, (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5Intro_01);
    yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.JainaBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Intro_01);
    yield return (object) boHRexxar05.PlayLineAlways(enemyActor, (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5HeroPowerLines;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5EmoteResponse_01;
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
    BoH_Rexxar_05 boHRexxar05 = this;
    while (boHRexxar05.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        GameState.Get().SetBusy(true);
        Actor enemyActorByCardId = boHRexxar05.GetEnemyActorByCardId("Story_02_WoundedFootman");
        if ((Object) enemyActorByCardId != (Object) null)
          yield return (object) boHRexxar05.PlayLineAlways(enemyActorByCardId, (string) BoH_Rexxar_05.VO_Story_Minion_Footman_Male_Human_Story_Footman_Play_01);
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.JainaBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeE_01);
        yield return (object) boHRexxar05.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeE_01);
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.JainaBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeE_02);
        GameState.Get().SetBusy(false);
        break;
      case 201:
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.JainaBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeA_01);
        break;
      case 202:
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.JainaBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeB_01);
        yield return (object) boHRexxar05.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeB_01);
        break;
      case 203:
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.JainaBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeC_01);
        yield return (object) boHRexxar05.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeC_01);
        break;
      case 204:
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.JainaBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5ExchangeD_01);
        yield return (object) boHRexxar05.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5ExchangeD_01);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) boHRexxar05.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_05.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission5Victory_01);
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.DaelinBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Daelin_Male_Human_Story_Rexxar_Mission5Victory_01);
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.JainaBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Victory_01);
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.DaelinBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Daelin_Male_Human_Story_Rexxar_Mission5Victory_02);
        yield return (object) boHRexxar05.PlayLineAlways((string) BoH_Rexxar_05.JainaBrassRing, (string) BoH_Rexxar_05.VO_Story_Hero_Jaina_Female_Human_Story_Rexxar_Mission5Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHRexxar05.PlayLineAlways(actor, (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHRexxar05.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Rexxar_05 boHRexxar05 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHRexxar05.\u003C\u003En__1(entity);
    while (boHRexxar05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHRexxar05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHRexxar05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHRexxar05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Rexxar_05 boHRexxar05 = this;
    while (boHRexxar05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHRexxar05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHRexxar05.\u003C\u003En__2(entity);
      yield return (object) boHRexxar05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHRexxar05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Rexxar_05 boHRexxar05 = this;
    while (boHRexxar05.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 17:
        yield return (object) boHRexxar05.PlayLineAlways(actor, (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5ExchangeF_01);
        break;
      case 19:
        yield return (object) boHRexxar05.PlayLineAlways(actor, (string) BoH_Rexxar_05.VO_Story_Hero_Darkscale_Female_Naga_Story_Rexxar_Mission5ExchangeG_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DRG);
}
