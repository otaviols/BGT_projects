using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Uther_08 : BoH_Uther_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Uther_08.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8EmoteResponse_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8EmoteResponse_01.prefab:1c863967bbf51124b91a032af52bc611");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeA_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeA_01.prefab:5228d5240d7332b499320186d0c22a08");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeB_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeB_01.prefab:17a4c7b01980e0643981c80486d6b203");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeC_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeC_01.prefab:d5d9b45756b579647980236ed457695c");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_01.prefab:a1f4a52d68e2f1b42a051fac5f244663");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_02.prefab:b6b6233ceeb482147a88ac462f7c0b4c");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_03 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_03.prefab:e920578aa0df881489de3cd1c7ecd1a1");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_01.prefab:f9365764ca569b1458f61865e91d0b8f");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_02.prefab:b2809a7f35d86604e9e8371f58b19749");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_03 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_03.prefab:e0414ff298743f34296b99c08bd63703");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Loss_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Loss_01.prefab:fa3924befd4c9044999c773b893437cf");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeA_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeA_01.prefab:5ae0bef312acadf4b9728831e17f5f63");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeC_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeC_01.prefab:482d703e8500cd54a9a42cd579e87525");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8Intro_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8Intro_01.prefab:d0e1fba54e9156446bdd8e9f72578615");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Intro_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Intro_01.prefab:16e9a108765122f4e88b7f5dec76918a");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeB_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeB_01.prefab:0e2fd2dbe0efe79429e611b614c92d06");
  private static readonly AssetReference VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Victory_01 = new AssetReference("VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Victory_01.prefab:2c5b3ea5536f97f499f1d5edc20a6a25");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_01,
    (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_02,
    (string) BoH_Uther_08.VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Victory_01
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_01,
    (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_02,
    (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();
  private Notification m_turnCounter;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8EmoteResponse_01,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeA_01,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeB_01,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeC_01,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_01,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_02,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8HeroPower_03,
      (string) BoH_Uther_08.VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Victory_01,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_01,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_02,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Idle_03,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Intro_01,
      (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Loss_01,
      (string) BoH_Uther_08.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeA_01,
      (string) BoH_Uther_08.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeB_01,
      (string) BoH_Uther_08.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeC_01,
      (string) BoH_Uther_08.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Uther_08 boHUther08 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHUther08.PlayLineAlways(actor, (string) BoH_Uther_08.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8Intro_01);
    yield return (object) boHUther08.PlayLineAlways(enemyActor, (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8EmoteResponse_01;
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
    BoH_Uther_08 boHUther08 = this;
    while (boHUther08.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 504)
    {
      yield return (object) boHUther08.PlayLineAlways(actor, (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8Loss_01);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHUther08.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Uther_08 boHUther08 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHUther08.\u003C\u003En__1(entity);
    while (boHUther08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHUther08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHUther08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHUther08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Uther_08 boHUther08 = this;
    while (boHUther08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHUther08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHUther08.\u003C\u003En__2(entity);
      yield return (object) boHUther08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHUther08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Uther_08 boHUther08 = this;
    while (boHUther08.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 5:
        yield return (object) boHUther08.PlayLineAlways(actor, (string) BoH_Uther_08.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeB_01);
        yield return (object) boHUther08.PlayLineAlways(enemyActor, (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeB_01);
        break;
      case 13:
        yield return (object) boHUther08.PlayLineAlways(actor, (string) BoH_Uther_08.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission8ExchangeC_01);
        yield return (object) boHUther08.PlayLineAlways(enemyActor, (string) BoH_Uther_08.VO_Story_Hero_Arthas_Male_Human_Story_Uther_Mission8ExchangeC_01);
        break;
    }
  }

  public override void NotifyOfMulliganEnded()
  {
    base.NotifyOfMulliganEnded();
    this.InitVisuals();
  }

  private void InitVisuals() => this.InitTurnCounter(this.GetCost());

  public override void OnTagChanged(TagDelta change)
  {
    base.OnTagChanged(change);
    if (change.tag != 48 || change.newValue == change.oldValue)
      return;
    this.UpdateVisuals(change.newValue);
  }

  private void InitTurnCounter(int cost)
  {
    this.m_turnCounter = AssetLoader.Get().InstantiatePrefab((AssetReference) "LOE_Turn_Timer.prefab:b05530aa55868554fb8f0c66632b3c22").GetComponent<Notification>();
    PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
    component.FsmVariables.GetFsmBool("RunningMan").Value = true;
    component.FsmVariables.GetFsmBool("MineCart").Value = false;
    component.FsmVariables.GetFsmBool("Airship").Value = false;
    component.FsmVariables.GetFsmBool("Destroyer").Value = false;
    component.SendEvent("Birth");
    this.m_turnCounter.transform.parent = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor().gameObject.transform;
    this.m_turnCounter.transform.localPosition = new Vector3(-1.4f, 0.187f, -0.11f);
    this.m_turnCounter.transform.localScale = Vector3.one * 0.52f;
    this.UpdateTurnCounterText(cost);
  }

  private void UpdateVisuals(int cost) => this.UpdateTurnCounter(cost);

  private void UpdateTurnCounter(int cost)
  {
    this.m_turnCounter.GetComponent<PlayMakerFSM>().SendEvent("Action");
    if (cost <= 0)
      Object.Destroy((Object) this.m_turnCounter.gameObject);
    else
      this.UpdateTurnCounterText(cost);
  }

  private void UpdateTurnCounterText(int cost) => this.m_turnCounter.ChangeDialogText(GameStrings.FormatPlurals("BOH_UTHER_08", new GameStrings.PluralNumber[1]
  {
    new GameStrings.PluralNumber()
    {
      m_index = 0,
      m_number = cost
    }
  }), cost.ToString(), "", "");

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ICCLichKing);
}
