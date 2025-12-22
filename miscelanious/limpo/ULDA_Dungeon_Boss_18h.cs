using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_18h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_BossCurseofRafaam_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_BossCurseofRafaam_01.prefab:8d3aabd960041cb449ccfbb70c8c1909");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_BossCurseofWeakness_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_BossCurseofWeakness_01.prefab:985a74422cace944dadb7c1222ebae4c");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_BossLeperGnome_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_BossLeperGnome_01.prefab:7c6cca069aee4a44ca055568ef8402a3");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_Death_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_Death_01.prefab:15179da5009a1524991a7b1d094940cc");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_DefeatPlayer_01.prefab:508e5a159c9be324a9a2686ee50db98c");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_EmoteResponse_01.prefab:0d17d0519ec7f9e449e0766d3cd90036");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_02.prefab:9ff0d01cf24d228498180fc49cd7f388");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_03.prefab:aa72ac465697c3d4395ece51b5d18267");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_04.prefab:1f6f035fa5adf254985140ece7fadc31");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_Idle_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_Idle_01.prefab:dd295f7b1d6ef2b42a3dbce95159f796");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_Idle_02 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_Idle_02.prefab:f2027a387beaba340becba9af5641a5f");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_Idle_03 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_Idle_03.prefab:9048b74a696b9a74cac55fe27089b2e2");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_Intro_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_Intro_01.prefab:f1417663b85d9294eaa192946a4de224");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_IntroFinleyResponse_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_IntroFinleyResponse_01.prefab:d18c0430263ac0a41b8c7200fadfc309");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_PlayerPilotedShredder_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_PlayerPilotedShredder_01.prefab:1a18075e95afd3749ac1e39240cb8acc");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_PlayerPlague_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_PlayerPlague_01.prefab:b4531194ef377fd47a862204938dbda8");
  private static readonly AssetReference VO_ULDA_BOSS_18h_Male_Gnome_PlayerThermaplugg_01 = new AssetReference("VO_ULDA_BOSS_18h_Male_Gnome_PlayerThermaplugg_01.prefab:5f848ea8ad1536e4fbba109d53afdff6");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_02,
    (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_03,
    (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_04
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Idle_01,
    (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Idle_02,
    (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_BossCurseofRafaam_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_BossCurseofWeakness_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_BossLeperGnome_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Death_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_02,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_03,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_HeroPower_04,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Idle_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Idle_02,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Idle_03,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Intro_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_IntroFinleyResponse_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_PlayerPilotedShredder_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_PlayerPlague_01,
      (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_PlayerThermaplugg_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Finley")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_IntroFinleyResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "ULDA_Reno") || !(cardId != "ULDA_Brann"))
          return;
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
      }
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
    ULDA_Dungeon_Boss_18h uldaDungeonBoss18h = this;
    while (uldaDungeonBoss18h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss18h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_18h uldaDungeonBoss18h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss18h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss18h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss18h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss18h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss18h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "GVG_096":
          yield return (object) uldaDungeonBoss18h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_PlayerPilotedShredder_01);
          break;
        case "GVG_116":
          yield return (object) uldaDungeonBoss18h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_PlayerThermaplugg_01);
          break;
        case "ULD_172":
        case "ULD_707":
        case "ULD_715":
        case "ULD_717":
        case "ULD_718":
          yield return (object) uldaDungeonBoss18h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_PlayerPlague_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_18h uldaDungeonBoss18h = this;
    while (uldaDungeonBoss18h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss18h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss18h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss18h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss18h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "LOE_007"))
      {
        if (!(cardId == "GIL_665"))
        {
          if (cardId == "EX1_029")
            yield return (object) uldaDungeonBoss18h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_BossLeperGnome_01);
        }
        else
          yield return (object) uldaDungeonBoss18h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_BossCurseofWeakness_01);
      }
      else
        yield return (object) uldaDungeonBoss18h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_18h.VO_ULDA_BOSS_18h_Male_Gnome_BossCurseofRafaam_01);
    }
  }
}
