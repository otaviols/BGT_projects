using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Rexxar_08 : BoH_Rexxar_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Rexxar_08.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Death_01 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Death_01.prefab:449307bb2d07fe749b1785dfd53ad27a");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8EmoteResponse_01 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8EmoteResponse_01.prefab:97aa1e86626b3f44f9ea132a9c5d13ed");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeC_01 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeC_01.prefab:d0f6d06584caa3c41816e576f1c5c92a");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeD_01 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeD_01.prefab:a40693b57ecf2df4ea4fffdcd2850b7c");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeE_01 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeE_01.prefab:f8b2f1c0bb3652341b4106644aba629a");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_01 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_01.prefab:a322e6cb04a12a144a85e4f2bd28303d");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_02 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_02.prefab:17babc478105f324fb856bf54ff29b6e");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_03 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_03.prefab:da605a196ac4d3943948f81cafa86b14");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_01 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_01.prefab:e363859d8d7adc34aafccef5da7214de");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_02 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_02.prefab:cdd49e828c8aee743af48a78772e35c5");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_03 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_03.prefab:733e443ef2ca0a34f910c217b4f19650");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Intro_01 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Intro_01.prefab:f9f336cd8c868dc4b8a2f428e4c233ee");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Intro_02 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Intro_02.prefab:bab3445bfc52a6b4aa63866b863e49b8");
  private static readonly AssetReference VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Loss_01 = new AssetReference("VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Loss_01.prefab:03767a3f92025804e86515f0564f0f22");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeA_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeA_01.prefab:183ea9ddf9b75174e994a38aa2577a64");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeB_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeB_01.prefab:f78e10b557e14e24abd5f815bfe6abc6");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeC_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeC_01.prefab:98429fe2e35f24040b410ded98722053");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8Intro_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8Intro_01.prefab:a25ef61d811effd439acd640b194d7ef");
  private List<string> m_VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPowerLines = new List<string>()
  {
    (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_01,
    (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_02,
    (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_03
  };
  private List<string> m_VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8IdleLines = new List<string>()
  {
    (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_01,
    (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_02,
    (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Rexxar_08() => this.m_gameOptions.AddBooleanOptions(BoH_Rexxar_08.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Death_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8EmoteResponse_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeC_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeD_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeE_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_02,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPower_03,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_02,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Idle_03,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Intro_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Intro_02,
      (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Loss_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeA_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeB_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeC_01,
      (string) BoH_Rexxar_08.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Rexxar_08 boHRexxar08 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHRexxar08.PlayLineAlways(enemyActor, (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Intro_01);
    yield return (object) boHRexxar08.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_08.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8Intro_01);
    yield return (object) boHRexxar08.PlayLineAlways(enemyActor, (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Death_01;
    this.m_standardEmoteResponseLine = (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8EmoteResponse_01;
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
    BoH_Rexxar_08 boHRexxar08 = this;
    while (boHRexxar08.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 504)
    {
      GameState.Get().SetBusy(true);
      yield return (object) boHRexxar08.PlayLineAlways(actor, (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8Loss_01);
      GameState.Get().SetBusy(false);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHRexxar08.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Rexxar_08 boHRexxar08 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHRexxar08.\u003C\u003En__1(entity);
    while (boHRexxar08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHRexxar08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHRexxar08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHRexxar08.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_610"))
      {
        if (cardId == "Story_02_GronnTrap")
          yield return (object) boHRexxar08.PlayLineAlways(actor, (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeE_01);
      }
      else
        yield return (object) boHRexxar08.PlayLineAlways(actor, (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeD_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Rexxar_08 boHRexxar08 = this;
    while (boHRexxar08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHRexxar08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHRexxar08.\u003C\u003En__2(entity);
      yield return (object) boHRexxar08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHRexxar08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Rexxar_08 boHRexxar08 = this;
    while (boHRexxar08.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHRexxar08.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_08.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeA_01);
        break;
      case 7:
        yield return (object) boHRexxar08.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_08.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeB_01);
        break;
      case 13:
        yield return (object) boHRexxar08.PlayLineAlways(actor, (string) BoH_Rexxar_08.VO_Story_Hero_Gorgrom_Male_Gronn_Story_Rexxar_Mission8ExchangeC_01);
        yield return (object) boHRexxar08.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_08.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission8ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BT);
}
