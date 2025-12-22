using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_08h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_BossTreant_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_BossTreant_01.prefab:6ed67142b9e36174cb5ed1ea0f9833ec");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_BossTreeSpeaker_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_BossTreeSpeaker_01.prefab:49b12f96b9d97954b9de643915195a67");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_BossWitchwoodApple_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_BossWitchwoodApple_01.prefab:adc5a6e72aa03114a911d5fb2e18c943");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_Death_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_Death_01.prefab:736bbdcfae947564f98ff96de06f0c77");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_DefeatPlayer_02 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_DefeatPlayer_02.prefab:f5d38607acec70a47bc28ae0b3d64219");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_EmoteResponse_01.prefab:fb9b7860d65a635478c4bd16b475a2ef");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_HeroPower_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_HeroPower_01.prefab:f562d83ab8499af4b8e7370b52bf558f");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_HeroPower_02 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_HeroPower_02.prefab:a9856797b8e979f4eb35a97c58332c51");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_HeroPower_03 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_HeroPower_03.prefab:78032eb399938b04fac5bfb6969a8f46");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_HeroPower_04 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_HeroPower_04.prefab:13819203da5d83f48826966a14d2bc77");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_HeroPower_05 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_HeroPower_05.prefab:b701bf8041d416b4a81847d217523b63");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_Idle_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_Idle_01.prefab:bb8e872490da7434cb42a13b8f00dd6b");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_Idle_02 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_Idle_02.prefab:2b5a5c1f3451b074e906f705de20ad94");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_Idle_03 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_Idle_03.prefab:20965f6bf0c7c024bbeb0acebc70a4d3");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_Idle_04 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_Idle_04.prefab:a2fd012251a56ed4aba40be68b8f86bc");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_Idle_05 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_Idle_05.prefab:7807ff6818a3dd64fa3745aad25e50d2");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_Intro_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_Intro_01.prefab:1ae0678c9ea0dc8429c920d1612589a7");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_IntroGeorge_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_IntroGeorge_01.prefab:e898a2707ad42ba43bdc1c3eeae1f6e8");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_IntroOlBarkeye_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_IntroOlBarkeye_01.prefab:736fe86dad6e19a4494a39e5f456bab9");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_IntroRakanishu_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_IntroRakanishu_01.prefab:7ed31126427cf2b44aab5160a58acb94");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_IntroSqueamlish_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_IntroSqueamlish_01.prefab:4dd5c5ae31b454b4f9664679eaec2ac5");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_PlayerDruidSpell_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_PlayerDruidSpell_01.prefab:671722509645e5640a5cd6689d234348");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_PlayerFireMageSpell_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_PlayerFireMageSpell_01.prefab:97365f771ae3c0f4f9aca6c5bc019421");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_PlayerTreant_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_PlayerTreant_01.prefab:d76ce6051fbb562489e6e41fd24431cb");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_PlayerTreant_02 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_PlayerTreant_02.prefab:0f4e9f88aef1b9442a9d940ac5d5b3e2");
  private static readonly AssetReference VO_DALA_BOSS_08h_Female_Treant_PlayerTreeOfLife_01 = new AssetReference("VO_DALA_BOSS_08h_Female_Treant_PlayerTreeOfLife_01.prefab:e8b6206b7df036b4e8dadf18f7a4c33b");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_01,
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_02,
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_03,
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_04,
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_05
  };
  private HashSet<string> m_playedLines = new HashSet<string>();
  private static List<string> m_PlayerTreant = new List<string>()
  {
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerTreant_01,
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerTreant_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_BossTreant_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_BossTreeSpeaker_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_BossWitchwoodApple_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Death_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_DefeatPlayer_02,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_02,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_03,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_04,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_05,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_02,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_03,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_04,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Idle_05,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Intro_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_IntroGeorge_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_IntroOlBarkeye_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_IntroRakanishu_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_IntroSqueamlish_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerDruidSpell_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerFireMageSpell_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerTreant_01,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerTreant_02,
      (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerTreeOfLife_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_08h.m_IdleLines;

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_01,
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_02,
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_03,
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_04,
    (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_HeroPower_05
  };

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_08h dalaDungeonBoss08h = this;
    while (dalaDungeonBoss08h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) dalaDungeonBoss08h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerDruidSpell_01);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss08h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_George")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_IntroGeorge_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "DALA_Squeamlish") || !(cardId != "DALA_Barkeye") || !(cardId != "DALA_Rakanishu") || !(cardId != "DALA_Vessina"))
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

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_08h dalaDungeonBoss08h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss08h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss08h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss08h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss08h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss08h.m_playedLines.Add(cardId);
      switch (cardId)
      {
        case "CFM_065":
        case "CS2_029":
        case "CS2_032":
        case "EX1_279":
        case "GIL_147":
        case "ICC_836":
        case "KAR_076":
        case "LOE_002":
        case "LOE_002t":
        case "LOOT_172":
        case "TRL_313":
        case "TRL_317":
        case "UNG_955":
          yield return (object) dalaDungeonBoss08h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerFireMageSpell_01);
          break;
        case "DAL_256t2":
        case "EX1_158t":
        case "FP1_019t":
        case "GIL_663t":
          yield return (object) dalaDungeonBoss08h.PlayAndRemoveRandomLineOnlyOnce(enemyActor, DALA_Dungeon_Boss_08h.m_PlayerTreant);
          break;
        case "GVG_033":
          yield return (object) dalaDungeonBoss08h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_PlayerTreeOfLife_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_08h dalaDungeonBoss08h = this;
    while (dalaDungeonBoss08h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss08h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss08h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss08h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss08h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "GIL_663t") && !(cardId == "FP1_019t") && !(cardId == "EX1_158t") && !(cardId == "DAL_256t2"))
      {
        if (!(cardId == "TRL_341"))
        {
          if (cardId == "GIL_663")
            yield return (object) dalaDungeonBoss08h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_BossWitchwoodApple_01);
        }
        else
          yield return (object) dalaDungeonBoss08h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_BossTreeSpeaker_01);
      }
      else
        yield return (object) dalaDungeonBoss08h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_08h.VO_DALA_BOSS_08h_Female_Treant_BossTreant_01);
    }
  }
}
