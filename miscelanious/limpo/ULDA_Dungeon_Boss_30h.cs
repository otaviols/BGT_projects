using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_30h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFirefly_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFirefly_01.prefab:7f4b2bcd0a897154781b45da0945666f");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFlamestrike_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFlamestrike_01.prefab:90d1ffbe274c5c74d896cf53c8cc4e8b");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFlamewaker_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFlamewaker_01.prefab:be690996d7ba7114893560caf4a89f4b");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_Death_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_Death_01.prefab:786ab936edab4ca4f9ec25ef1043ad5f");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_DefeatPlayer_01.prefab:195ab34934b1e4c4190b00a57628d8b4");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_EmoteResponse_01.prefab:388d6dc2e5873f34baef5edbf2f4e45c");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_01.prefab:d3da526643216d3469cb4affd279b486");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_03.prefab:4d51832d05d371e47b6858dac38b0b7b");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_04.prefab:efc05fffb80f51c4bb1d0e9422cbeb9e");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_05.prefab:6b91551cde887824a88ad5bb7d0339ae");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_01.prefab:8564c53c5c5ded248b8f960b770edc3d");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_02 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_02.prefab:ae9737b3f3f15444c9bf8aace76dc766");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_03 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_03.prefab:4e5103210c9e69140a919c27c17ed2ad");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_Intro_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_Intro_01.prefab:c0cb28dc94ff7af4a8d1a72181307912");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_PlayerTrigger_Fireball_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_PlayerTrigger_Fireball_01.prefab:8dd701ead2a23324ab6cb65105829b13");
  private static readonly AssetReference VO_ULDA_BOSS_30h_Female_NefersetTolvir_PlayerTrigger_Pharaoh_Cat_01 = new AssetReference("VO_ULDA_BOSS_30h_Female_NefersetTolvir_PlayerTrigger_Pharaoh_Cat_01.prefab:cbadc943d6f8d4847869588fc6b42dd9");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_01,
    (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_03,
    (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_04,
    (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_01,
    (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_02,
    (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFirefly_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFlamestrike_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFlamewaker_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Death_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_03,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_04,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_HeroPower_05,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_02,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Idle_03,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Intro_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_PlayerTrigger_Fireball_01,
      (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_PlayerTrigger_Pharaoh_Cat_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
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
    ULDA_Dungeon_Boss_30h uldaDungeonBoss30h = this;
    while (uldaDungeonBoss30h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) uldaDungeonBoss30h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss30h.m_HeroPowerLines);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss30h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_30h uldaDungeonBoss30h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss30h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss30h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss30h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss30h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss30h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_186"))
      {
        if (cardId == "CS2_029")
          yield return (object) uldaDungeonBoss30h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_PlayerTrigger_Fireball_01);
      }
      else
        yield return (object) uldaDungeonBoss30h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_PlayerTrigger_Pharaoh_Cat_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_30h uldaDungeonBoss30h = this;
    while (uldaDungeonBoss30h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss30h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss30h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss30h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss30h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "UNG_809"))
      {
        if (!(cardId == "CS2_032"))
        {
          if (cardId == "BRM_002")
            yield return (object) uldaDungeonBoss30h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFlamewaker_01);
        }
        else
          yield return (object) uldaDungeonBoss30h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFlamestrike_01);
      }
      else
        yield return (object) uldaDungeonBoss30h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_30h.VO_ULDA_BOSS_30h_Female_NefersetTolvir_BossTriggerFirefly_01);
    }
  }
}
