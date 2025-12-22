using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_73h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerExplosiveRunes_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerExplosiveRunes_01.prefab:ee2b49761bb03b54bac04eec04d52fc9");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerHeroicStrike_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerHeroicStrike_01.prefab:002ff346bfa849848abf40df1a0ed52c");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerPlayExplosiveRunes_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerPlayExplosiveRunes_01.prefab:06afcc5a0c94cc64b8deb309daf5dfa7");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_Death_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_Death_01.prefab:734e0529efa8cf3429a1262cec87d5ef");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_DefeatPlayer_01.prefab:1b2aae4f91b64a548b9232e789f7bfa5");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_EmoteResponse_01.prefab:b5e892b811ea91c47860068dd1cd52ec");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_01.prefab:62af478601389a246b024c8c58b3619e");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_02.prefab:a103d60ac2181ad4cbe43849cd55846b");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_03.prefab:3436c65cd5de29749b2ca9af4d9bac5e");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_04.prefab:cf78703d684c45b4cbc92ce8b445a1b2");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_05.prefab:4930f45660b454348b9d36cf41444737");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_01.prefab:c4ac919798ee45d4a89329690d8b1fbf");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_02 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_02.prefab:1c64211ff601b794d93512706564a45b");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_03 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_03.prefab:44c14962e4fd80a4c844f7255b159e24");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_Intro_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_Intro_01.prefab:f087080c34f6261438b8052d9a1a5fba");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_IntroSpecialFinley_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_IntroSpecialFinley_01.prefab:07f425bdaca013a4eb2edf5ce59cec7e");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_Cursed_Lieutenant_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_Cursed_Lieutenant_01.prefab:4d29ba82c2bfd7b4e9b364383228e8e6");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_King_Phaoris_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_King_Phaoris_01.prefab:57be555af5fe6004e927eda18d537a1d");
  private static readonly AssetReference VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_Siamat_01 = new AssetReference("VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_Siamat_01.prefab:2db985ecf9ff3db43bec397d9e71652d");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_01,
    (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_02,
    (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_03,
    (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_04,
    (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_01,
    (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_02,
    (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerExplosiveRunes_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerHeroicStrike_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerPlayExplosiveRunes_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Death_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_02,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_03,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_04,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_HeroPower_05,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_02,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Idle_03,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Intro_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_IntroSpecialFinley_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_Cursed_Lieutenant_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_King_Phaoris_01,
      (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_Siamat_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Finley")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_IntroSpecialFinley_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    ULDA_Dungeon_Boss_73h uldaDungeonBoss73h = this;
    while (uldaDungeonBoss73h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) uldaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerExplosiveRunes_01);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss73h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_73h uldaDungeonBoss73h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss73h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss73h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss73h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss73h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss73h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_189"))
      {
        if (!(cardId == "ULD_304"))
        {
          if (cardId == "ULD_178")
            yield return (object) uldaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_Siamat_01);
        }
        else
          yield return (object) uldaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_King_Phaoris_01);
      }
      else
        yield return (object) uldaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_PlayerTrigger_Cursed_Lieutenant_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_73h uldaDungeonBoss73h = this;
    while (uldaDungeonBoss73h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss73h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss73h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss73h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss73h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CS2_105"))
      {
        if (cardId == "LOOT_101")
          yield return (object) uldaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerPlayExplosiveRunes_01);
      }
      else
        yield return (object) uldaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_73h.VO_ULDA_BOSS_73h_Male_NefersetTolvir_BossTriggerHeroicStrike_01);
    }
  }
}
