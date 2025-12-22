using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_51h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerAnubisathDefender_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerAnubisathDefender_01.prefab:3e2dfe63028792941a30fd3c818caba6");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerEmbalmingRitual_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerEmbalmingRitual_01.prefab:08aef91a3a8b66244bd306daea060474");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerPsychopomp_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerPsychopomp_01.prefab:2b2bc6b4297687f43bde99e100bb5ceb");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_Death_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_Death_01.prefab:60891177533b5934ea4d499152eef7c9");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_DefeatPlayer_01.prefab:c34d561cc66605d4b95fe836ccc523b0");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_EmoteResponse_01.prefab:32ba78c9a0364de48b8abf823ef603aa");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_01.prefab:49940cd11cadb5041b6431367c43d164");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_02.prefab:a384f835f507a0349b2ac39ef70e2e34");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_03.prefab:a146aca8a334e684c873d0f3da19a73e");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_05.prefab:587a74d18825d9a4bbdc6b0349cd9210");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_Idle_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_Idle_01.prefab:49fd34f7c4ba8f54e91406a4a38ac49c");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_Idle_03 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_Idle_03.prefab:5d0d8db1387ffdd458f6fad072550935");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_Intro_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_Intro_01.prefab:ecb6ded326712904889601681e0a5f60");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_IntroSpecial_Elise_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_IntroSpecial_Elise_01.prefab:54ea30eff03a410458187db325b3ba34");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Anubisath_Defender_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Anubisath_Defender_01.prefab:ba414294e85df514990c7a501dbb0ed7");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Enslaved_Guardian_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Enslaved_Guardian_01.prefab:12e1b3782b0072843858d63157d2b12a");
  private static readonly AssetReference VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Pharaoh_Cat_01 = new AssetReference("VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Pharaoh_Cat_01.prefab:0c73fa01c1b00d14f82f99c0e09039d3");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_01,
    (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_02,
    (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_03,
    (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_Idle_01,
    (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerAnubisathDefender_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerEmbalmingRitual_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerPsychopomp_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_Death_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_02,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_03,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_HeroPower_05,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_Idle_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_Idle_03,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_Intro_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_IntroSpecial_Elise_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Anubisath_Defender_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Enslaved_Guardian_01,
      (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Pharaoh_Cat_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START && cardId != "ULDA_Elise")
    {
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
    ULDA_Dungeon_Boss_51h uldaDungeonBoss51h = this;
    while (uldaDungeonBoss51h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss51h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_51h uldaDungeonBoss51h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss51h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss51h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss51h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss51h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss51h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_186"))
      {
        if (!(cardId == "ULD_138"))
        {
          if (cardId == "ULD_271")
            yield return (object) uldaDungeonBoss51h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Enslaved_Guardian_01);
        }
        else
          yield return (object) uldaDungeonBoss51h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Anubisath_Defender_01);
      }
      else
        yield return (object) uldaDungeonBoss51h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_PlayerTrigger_Pharaoh_Cat_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_51h uldaDungeonBoss51h = this;
    while (uldaDungeonBoss51h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss51h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss51h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss51h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss51h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_138"))
      {
        if (!(cardId == "ULD_265"))
        {
          if (cardId == "ULD_268")
            yield return (object) uldaDungeonBoss51h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerPsychopomp_01);
        }
        else
          yield return (object) uldaDungeonBoss51h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerEmbalmingRitual_01);
      }
      else
        yield return (object) uldaDungeonBoss51h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_51h.VO_ULDA_BOSS_51h_Female_Anubisath_BossTriggerAnubisathDefender_01);
    }
  }
}
