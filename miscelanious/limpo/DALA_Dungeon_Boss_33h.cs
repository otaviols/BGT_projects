using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_33h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_BossCoin_01 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_BossCoin_01.prefab:641e0356fb933da43a0863ff3f4cb97b");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_Death_02 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_Death_02.prefab:3cefb8526164bc44db41c9b7e32506d8");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_DefeatPlayer_01.prefab:238d5ed4853c5b64c9abd46b0019a25a");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_EmoteResponse_01.prefab:cc15f3b773696994483b1e1e643cd50a");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_HeroPower_01 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_HeroPower_01.prefab:301fc70e924dc5e42bb93f221682c37a");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_HeroPower_02 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_HeroPower_02.prefab:e57975c4cda39ad4995879f2dc10fc91");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_HeroPower_03 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_HeroPower_03.prefab:f74b4be1f30a53c4ba84dd2259dc4747");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_Idle_01 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_Idle_01.prefab:0ddebf7b4b38c8b49b9157047a086582");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_Idle_02 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_Idle_02.prefab:af8af91debd524943aa16f3c20280887");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_Idle_03 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_Idle_03.prefab:a97b231d4e03e7a49abe64fef00435b9");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_Idle_04 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_Idle_04.prefab:c0552dd3a2757a440a8a0b0479d26b30");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_Intro_01 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_Intro_01.prefab:cedda85793eb1d74cab0e8ce5d3e3bef");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_PlayerCoin_01 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_PlayerCoin_01.prefab:ab526e51b7c1be7458bc211bb1fc5024");
  private static readonly AssetReference VO_DALA_BOSS_33h_Male_Elemental_PlayerGoldenIdol_01 = new AssetReference("VO_DALA_BOSS_33h_Male_Elemental_PlayerGoldenIdol_01.prefab:765931e3128a256468ea3c0a4ef07246");
  private static List<string> m_HeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_HeroPower_01,
    (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_HeroPower_02,
    (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_HeroPower_03
  };
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Idle_01,
    (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Idle_02,
    (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Idle_03,
    (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Idle_04
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_BossCoin_01,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Death_02,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_HeroPower_01,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_HeroPower_02,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_HeroPower_03,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Idle_01,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Idle_02,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Idle_03,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Idle_04,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Intro_01,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_PlayerCoin_01,
      (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_PlayerGoldenIdol_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_Death_02;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_33h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Eudora") || !(cardId != "DALA_Rakanishu"))
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
    DALA_Dungeon_Boss_33h dalaDungeonBoss33h = this;
    while (dalaDungeonBoss33h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) dalaDungeonBoss33h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_33h.m_HeroPower);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss33h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_33h dalaDungeonBoss33h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss33h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss33h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss33h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss33h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss33h.m_playedLines.Add(cardId);
      if (!(cardId == "GAME_005") && !(cardId == "GVG_028t"))
      {
        if (cardId == "LOOT_998k" || cardId == "DALA_709" || cardId == "LOE_019t2")
          yield return (object) dalaDungeonBoss33h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_PlayerGoldenIdol_01);
      }
      else
        yield return (object) dalaDungeonBoss33h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_PlayerCoin_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_33h dalaDungeonBoss33h = this;
    while (dalaDungeonBoss33h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss33h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      yield return (object) dalaDungeonBoss33h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss33h.m_playedLines.Add(cardId);
      if (cardId == "GAME_005" || cardId == "GVG_028t")
        yield return (object) dalaDungeonBoss33h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_33h.VO_DALA_BOSS_33h_Male_Elemental_BossCoin_01);
    }
  }
}
