using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_49h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_01 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_01.prefab:aa25b431c7ba1d14aaa63a88ddcdbf2b");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_02 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_02.prefab:14f7e34a19ab2e845b07cf758a53489e");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_04 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_04.prefab:bf876403f6721c544a737b056e253dac");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_Death_01 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_Death_01.prefab:72fd230d6c65a5045ba066c3834a5115");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_DefeatPlayer_02 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_DefeatPlayer_02.prefab:ef721f400b460524aa4429a2009b5139");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_EmoteResponse_01.prefab:ebb6ec89f444ee94bba1bb9266265dfc");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_HeroPower_01 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_HeroPower_01.prefab:ada4fbddd58fe2a43b0bbeb44dc8b164");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_HeroPower_02 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_HeroPower_02.prefab:4c209315128c1b1459fea1a9f99c6a46");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_HeroPower_03 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_HeroPower_03.prefab:5617b5b8a9906e043a99945db0d6183d");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_HeroPower_04 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_HeroPower_04.prefab:71a2407a5cf96a2439009baaa3626eff");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_Idle_01 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_Idle_01.prefab:62f21d400545c114980599b44ba8950e");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_Idle_02 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_Idle_02.prefab:a1654986a0955354297712d8d5feb989");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_Idle_03 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_Idle_03.prefab:97f40a086caae7844bcafc1bfd36a42a");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_Intro_01 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_Intro_01.prefab:f4777e851af7f004fa6d70ee6f1ca6fb");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_Misc_01 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_Misc_01.prefab:be8fd937104d1d940b40ad0f2d385e0a");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_PlayerArcaneExplosion_01 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_PlayerArcaneExplosion_01.prefab:6c686f3a65947894ea0c155198d18f67");
  private static readonly AssetReference VO_DALA_BOSS_49h_Female_Dragon_PlayerMalygos_01 = new AssetReference("VO_DALA_BOSS_49h_Female_Dragon_PlayerMalygos_01.prefab:19af92f5f76ad414a9f1cd6f050a6b7e");
  private static List<string> m_BossHeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_HeroPower_01,
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_HeroPower_02,
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_HeroPower_03,
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_HeroPower_04
  };
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Idle_01,
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Idle_02,
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Idle_03
  };
  private static List<string> m_BossFrostSpell = new List<string>()
  {
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_01,
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_02,
    (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_04
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_01,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_02,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_BossFrostSpell_04,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Death_01,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_DefeatPlayer_02,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_HeroPower_01,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_HeroPower_02,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_HeroPower_03,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_HeroPower_04,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Idle_01,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Idle_02,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Idle_03,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Intro_01,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Misc_01,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_PlayerArcaneExplosion_01,
      (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_PlayerMalygos_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_49h.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_EmoteResponse_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Tekahn") || !(cardId != "DALA_Squeamlish"))
        return;
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
    DALA_Dungeon_Boss_49h dalaDungeonBoss49h = this;
    while (dalaDungeonBoss49h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss49h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_Misc_01);
        break;
      case 102:
        yield return (object) dalaDungeonBoss49h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_49h.m_BossHeroPower);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss49h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_49h dalaDungeonBoss49h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss49h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss49h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss49h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss49h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss49h.m_playedLines.Add(cardId);
      if (!(cardId == "CS2_025"))
      {
        if (cardId == "EX1_563")
          yield return (object) dalaDungeonBoss49h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_PlayerMalygos_01);
      }
      else
        yield return (object) dalaDungeonBoss49h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_49h.VO_DALA_BOSS_49h_Female_Dragon_PlayerArcaneExplosion_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_49h dalaDungeonBoss49h = this;
    while (dalaDungeonBoss49h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss49h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss49h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss49h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "CS2_024" || cardId == "CS2_026" || cardId == "CS2_037" || cardId == "DAL_577" || cardId == "EX1_179" || cardId == "CS2_031")
        yield return (object) dalaDungeonBoss49h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_49h.m_BossFrostSpell);
    }
  }
}
