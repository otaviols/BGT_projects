using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_03h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_01.prefab:16976478bdca1bb41a4cd3903a2e502e");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_02 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_02.prefab:87b79e63418b6f24e827268842a28e2f");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_03 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_03.prefab:b8211fca6a3cd614a84cd09b6044449d");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossFireSpell_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossFireSpell_01.prefab:708c0acd0b75eef43b67a7f1efe0f43f");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossPresto_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossPresto_01.prefab:b069918550be0a24e8a85dd04fe847b3");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossPresto_02 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossPresto_02.prefab:b4ebbcbf9f919984590b27ef49262623");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossPresto_03 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossPresto_03.prefab:f44f15adb95d7cb4c812671be1cc1c7a");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossPresto_04 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossPresto_04.prefab:ed5f44d3a5b90764f8bfa3fbb5926eeb");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_01.prefab:5c0002137d68f17468f48f752e4cae6e");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_02 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_02.prefab:8297447eaf8711845ac24407433e3e2e");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_03 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_03.prefab:0f0347be7d6966c4d9877b8d42912837");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossSpell_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossSpell_01.prefab:a7b9ab72aa814264b913486991e71b10");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossSpell_02 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossSpell_02.prefab:cc6765c289562e7478459df57a36dddc");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_01.prefab:6b4ecb20342992c49a31676700e13988");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_02 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_02.prefab:bdf4e0ddd4e025645ac57c780b747e8e");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_03 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_03.prefab:cd6099343fd020544968c54145c893e8");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_Death_03 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_Death_03.prefab:62438176772786e4383069d604e42766");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_DefeatPlayer_02 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_DefeatPlayer_02.prefab:7194f7ddc4463524984926fe6e687a0f");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_EmoteResponse_01.prefab:209c31fdaf50f4949871aed09e282485");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_HeroPower_02 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_HeroPower_02.prefab:8c0d04d7904b2024096f83ee6ced3091");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_HeroPower_03 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_HeroPower_03.prefab:5559e68af0ac45e46a14efb8d91c970c");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_HeroPower_04 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_HeroPower_04.prefab:38fb3ef64c2e2b94cbc73a5a4cee61eb");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_Idle_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_Idle_01.prefab:f93ca7e9021783d42a38c42d5a07a094");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_Idle_02 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_Idle_02.prefab:99d3be1976028a8439a4bddba50c0b6d");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_Idle_04 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_Idle_04.prefab:99c4f5047113f9247be1ce60fff837fa");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_Intro_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_Intro_01.prefab:75457983cd77c824d8fefa9b5196ee9e");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_IntroGeorge_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_IntroGeorge_01.prefab:8dd82ee0ffe129a4aa4ae109391ba793");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_PlayerBossSpell_01 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_PlayerBossSpell_01.prefab:94fce89c59818ef4d85fbf51b1bdf314");
  private static readonly AssetReference VO_DALA_BOSS_03h_Male_Goblin_PlayerBossSpell_02 = new AssetReference("VO_DALA_BOSS_03h_Male_Goblin_PlayerBossSpell_02.prefab:ff5ccb4f4aa2b35499b0419a47610fa6");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Idle_01,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Idle_02,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Idle_04
  };
  private List<string> m_BossReductomara = new List<string>()
  {
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_01,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_02,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_03
  };
  private List<string> m_BossBunnifitronus = new List<string>()
  {
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_01,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_02,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_03
  };
  private List<string> m_BossPresto = new List<string>()
  {
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossPresto_01,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossPresto_02,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossPresto_03
  };
  private List<string> m_BossYoggers = new List<string>()
  {
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_01,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_02,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_03
  };
  private List<string> m_BossSpell = new List<string>()
  {
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossSpell_01,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossSpell_02
  };
  private List<string> m_PlayerBossSpell = new List<string>()
  {
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_PlayerBossSpell_01,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_PlayerBossSpell_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_02,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossBunnifitronus_03,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossFireSpell_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossPresto_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossPresto_02,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossPresto_03,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossPresto_04,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_02,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossReductomara_03,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossSpell_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossSpell_02,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_02,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossYoggers_03,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Death_03,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_DefeatPlayer_02,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_HeroPower_02,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_HeroPower_03,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_HeroPower_04,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Idle_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Idle_02,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Idle_04,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Intro_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_IntroGeorge_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_PlayerBossSpell_01,
      (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_PlayerBossSpell_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_HeroPower_02,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_HeroPower_03,
    (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_HeroPower_04
  };

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_03h.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_Death_03;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_George")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_IntroGeorge_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "DALA_Chu") || !(cardId != "DALA_Vessina"))
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

  protected override bool GetShouldSuppressDeathTextBubble() => false;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_03h dalaDungeonBoss03h = this;
    while (dalaDungeonBoss03h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss03h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss03h.m_BossSpell);
        break;
      case 102:
        GameState.Get().SetBusy(true);
        yield return (object) dalaDungeonBoss03h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossPresto_04);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss03h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_03h dalaDungeonBoss03h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss03h.\u003C\u003En__1(entity);
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    while (dalaDungeonBoss03h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss03h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss03h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss03h.m_playedLines.Add(cardId);
      if (cardId == "DALA_BOSS_03t" || cardId == "DALA_BOSS_03t2" || cardId == "DALA_BOSS_03t3" || cardId == "DALA_BOSS_03t4")
        yield return (object) dalaDungeonBoss03h.PlayAndRemoveRandomLineOnlyOnce(enemyActor, dalaDungeonBoss03h.m_PlayerBossSpell);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_03h dalaDungeonBoss03h = this;
    while (dalaDungeonBoss03h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss03h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss03h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss03h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss03h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "DALA_BOSS_03t"))
      {
        if (!(cardId == "DALA_BOSS_03t2"))
        {
          if (!(cardId == "DALA_BOSS_03t3"))
          {
            if (!(cardId == "DALA_BOSS_03t4"))
            {
              if (cardId == "GIL_147")
                yield return (object) dalaDungeonBoss03h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_03h.VO_DALA_BOSS_03h_Male_Goblin_BossFireSpell_01);
            }
            else
              yield return (object) dalaDungeonBoss03h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss03h.m_BossYoggers);
          }
          else
            yield return (object) dalaDungeonBoss03h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss03h.m_BossPresto);
        }
        else
          yield return (object) dalaDungeonBoss03h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss03h.m_BossBunnifitronus);
      }
      else
        yield return (object) dalaDungeonBoss03h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss03h.m_BossReductomara);
    }
  }
}
