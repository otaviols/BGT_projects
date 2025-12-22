using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_70h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_BossTriggerArcaneMissilesSpellPower_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_BossTriggerArcaneMissilesSpellPower_01.prefab:c9540a9ec64f44f409bed15ad68e83da");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_BossVaporizeTrigger_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_BossVaporizeTrigger_01.prefab:89a0bfc086c730441954de3ab364d4e1");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_DeathALT_01.prefab:3dc608dccd0ddfb4ead688a9e2604514");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_DefeatPlayer_01.prefab:eb97396b15ba2bc4da33f03c84e76ca1");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_EmoteResponse_01.prefab:698b1ac4aada5db4f84f112ca995c232");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_01.prefab:e3ef9e9f28778bd418c4f45e97ae942e");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_02.prefab:dee59a73cd6617d45b225bf510e504e3");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_03.prefab:75f80a9bd9ad36d4d831764b03a5326c");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_04.prefab:3aabfd692db3f34428c3031bd4c2a083");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_05.prefab:8bd7a7f82b3b9f447a482cbec4b62b7c");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPowerRare_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPowerRare_01.prefab:c697707f48711bd448a54f6d0932c107");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_01.prefab:f982fe45ee4762b40a9e322e762e05b4");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_02 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_02.prefab:6d059ca0b84dee844aad3249b8e3bb52");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_03 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_03.prefab:fe899f346fe0fbb4e9bf35e4de50e77e");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_Intro_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_Intro_01.prefab:278d7147969332f42915201a4bb3709b");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_IntroSpecialBrann_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_IntroSpecialBrann_01.prefab:cf32fac4c04ad2044b663e3bdfb2094d");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerFreezeBoss_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerFreezeBoss_01.prefab:1455fbb598786ac4a80d2069b36312fb");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerFrostArmorTrigger_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerFrostArmorTrigger_01.prefab:5301ecb285849a841838217c2ffa01dc");
  private static readonly AssetReference VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerPyroblastFace_01 = new AssetReference("VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerPyroblastFace_01.prefab:f9806b9ed78cb734585133632b7751ff");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_01,
    (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_02,
    (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_03,
    (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_04,
    (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_01,
    (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_02,
    (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_BossTriggerArcaneMissilesSpellPower_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_BossVaporizeTrigger_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_DeathALT_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_02,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_03,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_04,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPower_05,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_HeroPowerRare_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_02,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_Idle_03,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_Intro_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_IntroSpecialBrann_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerFreezeBoss_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerFrostArmorTrigger_01,
      (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerPyroblastFace_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Brann")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_IntroSpecialBrann_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_70h uldaDungeonBoss70h = this;
    while (uldaDungeonBoss70h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss70h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerFreezeBoss_01);
        break;
      case 102:
        yield return (object) uldaDungeonBoss70h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerFrostArmorTrigger_01);
        break;
      case 103:
        yield return (object) uldaDungeonBoss70h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_PlayerPyroblastFace_01);
        break;
      case 104:
        yield return (object) uldaDungeonBoss70h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_BossVaporizeTrigger_01);
        break;
      case 105:
        yield return (object) uldaDungeonBoss70h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_70h.VO_ULDA_BOSS_70h_Male_TitanConstruct_BossTriggerArcaneMissilesSpellPower_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss70h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_70h uldaDungeonBoss70h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss70h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss70h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss70h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss70h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss70h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_70h uldaDungeonBoss70h = this;
    while (uldaDungeonBoss70h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss70h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss70h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss70h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss70h.m_playedLines.Add(cardId);
    }
  }
}
