using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_01h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_BossLackey_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_BossLackey_01.prefab:210378a566be5d64cb7e1cb9831be00b");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_BossMarkedShot_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_BossMarkedShot_01.prefab:53d7f7f6fd7148148b170560acee7859");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_BossPressurePlate_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_BossPressurePlate_01.prefab:8247465a3f19e8c40a60cceec85dd836");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_BossTriggerRapidFire_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_BossTriggerRapidFire_01.prefab:b090f8036c62d81428590821605aefc9");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_Death_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_Death_01.prefab:0667acc72b6264c41a5378f7dae7a25e");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_DefeatPlayer_01.prefab:6e605d1820c14124cbf0ad98f9b6ad93");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_EmoteResponse_01.prefab:004b2084937f4d24b93c0e86a2ed985e");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_HeroPower_01.prefab:cf017339da5e28248b7b5405a15fde10");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_HeroPower_02.prefab:d9469ab5e488c85409d301facb1a0f3f");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_HeroPower_04.prefab:52e3323c9a5fb3e42b7461a5332265b8");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_HeroPower_05.prefab:cabd559ba2df7ff41af8f0bbe3033bc6");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_Idle_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_Idle_01.prefab:055e09a2b533b62469c79d12c7553766");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_Idle_02 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_Idle_02.prefab:aca575db3a6bdd041a0e7864e9d07cd9");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_Idle_03 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_Idle_03.prefab:dfcd00f746d57644482e276d355450a9");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_Intro_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_Intro_01.prefab:0ceccd4cc4c695f499d9afe387bb2094");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_IntroFinley_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_IntroFinley_01.prefab:4179dc8ffc9623446a904c6854f75d4b");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_PlayerBlunderbuss_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_PlayerBlunderbuss_01.prefab:86979e14b68e54f46b3a712b6f429af9");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_PlayerGatlingWand_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_PlayerGatlingWand_01.prefab:321bcdb0194dbf142ab39539f8500276");
  private static readonly AssetReference VO_ULDA_BOSS_01h_Male_Human_PlayerGoblinBomb_01 = new AssetReference("VO_ULDA_BOSS_01h_Male_Human_PlayerGoblinBomb_01.prefab:4af7a4406d320e4418a87bcd21f6618e");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_HeroPower_01,
    (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_HeroPower_02,
    (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_HeroPower_04,
    (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Idle_01,
    (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Idle_02,
    (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_BossLackey_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_BossMarkedShot_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_BossPressurePlate_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_BossTriggerRapidFire_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Death_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_HeroPower_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_HeroPower_02,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_HeroPower_04,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_HeroPower_05,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Idle_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Idle_02,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Idle_03,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Intro_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_IntroFinley_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_PlayerBlunderbuss_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_PlayerGatlingWand_01,
      (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_PlayerGoblinBomb_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Finley")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_IntroFinley_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_01h uldaDungeonBoss01h = this;
    while (uldaDungeonBoss01h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss01h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_01h uldaDungeonBoss01h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss01h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss01h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss01h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss01h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss01h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULDA_401"))
      {
        if (!(cardId == "ULDA_207"))
        {
          if (cardId == "BOT_031")
            yield return (object) uldaDungeonBoss01h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_PlayerGoblinBomb_01);
        }
        else
          yield return (object) uldaDungeonBoss01h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_PlayerGatlingWand_01);
      }
      else
        yield return (object) uldaDungeonBoss01h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_PlayerBlunderbuss_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_01h uldaDungeonBoss01h = this;
    while (uldaDungeonBoss01h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss01h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss01h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss01h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss01h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "DAL_371":
          yield return (object) uldaDungeonBoss01h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_BossMarkedShot_01);
          break;
        case "DAL_373":
          yield return (object) uldaDungeonBoss01h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_BossTriggerRapidFire_01);
          break;
        case "DAL_613":
        case "DAL_614":
        case "DAL_615":
        case "DAL_739":
        case "DAL_741":
        case "ULD_616":
          yield return (object) uldaDungeonBoss01h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_BossLackey_01);
          break;
        case "ULD_152":
          yield return (object) uldaDungeonBoss01h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_01h.VO_ULDA_BOSS_01h_Male_Human_BossPressurePlate_01);
          break;
      }
    }
  }
}
