using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_26h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_BossClockworkGnome_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_BossClockworkGnome_01.prefab:3d5a271fdd5626142a9881567ce6ad94");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_BossGatlingWandTreasure_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_BossGatlingWandTreasure_01.prefab:43be4359ee4bc454d83bb067c67e0ea4");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_Death_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_Death_01.prefab:6d892f81258e93f4badfa482a0665765");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_DefeatPlayer_01.prefab:fedcf1cb64a6a3946b33e0a55a21a0ee");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_EmoteResponse_01.prefab:1116f04f4c037884ca39889f23525075");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_01.prefab:8ab9b9f60d05b24409a10c00de2a070e");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_03.prefab:8fd09491add94f348aab77c79bcde414");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_04.prefab:ca9ad22dc7628d64f8c1fd4c026ed67c");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_05.prefab:58693bb4d55776d418c6c5c7914ea8ea");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_01.prefab:d02ab5b480f82f942aa531f4ddce0793");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_02 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_02.prefab:62cb5b2f5510d6045afcabfc3535613d");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_03 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_03.prefab:0d6dca33078df13489fdb7664abeac72");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_Intro_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_Intro_01.prefab:9996636d831af5a479cfd29816b7dd4c");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_IntroReno_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_IntroReno_01.prefab:1ea6a18afc9e9b4499aaa98e23c793f2");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerBlingtron_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerBlingtron_01.prefab:e5bc7c740bc4fab44b3a15c0426a765d");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerGatlingWandTreasure_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerGatlingWandTreasure_01.prefab:9538562cf71a9564b8f05a14eaac5d8e");
  private static readonly AssetReference VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerGnomebliterator_01 = new AssetReference("VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerGnomebliterator_01.prefab:4ca5ab7a76339ac4b876e1eebb5ded0e");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_01,
    (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_03,
    (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_04,
    (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_01,
    (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_02,
    (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_BossClockworkGnome_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_BossGatlingWandTreasure_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Death_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_03,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_04,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_HeroPower_05,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_02,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Idle_03,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Intro_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_IntroReno_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerBlingtron_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerGatlingWandTreasure_01,
      (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerGnomebliterator_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Reno")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_IntroReno_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_26h uldaDungeonBoss26h = this;
    while (uldaDungeonBoss26h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss26h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_26h uldaDungeonBoss26h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss26h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss26h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss26h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss26h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss26h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "GVG_119"))
      {
        if (cardId == "ULDA_115")
          yield return (object) uldaDungeonBoss26h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerGnomebliterator_01);
      }
      else
        yield return (object) uldaDungeonBoss26h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_PlayerBlingtron_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_26h uldaDungeonBoss26h = this;
    while (uldaDungeonBoss26h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss26h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss26h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss26h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss26h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "GVG_082"))
      {
        if (cardId == "ULDA_207")
          yield return (object) uldaDungeonBoss26h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_BossGatlingWandTreasure_01);
      }
      else
        yield return (object) uldaDungeonBoss26h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_26h.VO_ULDA_BOSS_26h_Female_Mechagnome_BossClockworkGnome_01);
    }
  }
}
