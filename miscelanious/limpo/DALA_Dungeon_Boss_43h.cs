using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_43h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_BossTimeWarp_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_BossTimeWarp_01.prefab:183d37915f836cf4e9cc5545136e7771");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_Death_02 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_Death_02.prefab:9f007104467bc8b47bddf8814f36a796");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_DefeatPlayer_01.prefab:03602ba877b971942b82211782b024a4");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_EmoteResponse_01.prefab:fd1117caffcf287419549fd83cb9c45a");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_Intro_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_Intro_01.prefab:9eeced40701ab874d9a2224f876db8a3");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_PlayerNozdormu_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_PlayerNozdormu_01.prefab:64d2445dabb900e499ccee0f17b585e7");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_PlayerTemporus_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_PlayerTemporus_01.prefab:70aafb0f704a8774ca8bc4f2c1bd6024");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_PlayerTimeWarp_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_PlayerTimeWarp_01.prefab:825ee80004394b3428f00c3f67def6d9");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_01.prefab:d23bbbe5030cca14b9b6b27122ec31c2");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_02 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_02.prefab:d5e11881cc8e7da4bb5808e5c53d27fa");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_03 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_03.prefab:1bd952c1fcbb3284e8d0f6a4523f22e1");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_04 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_04.prefab:15086e2a34483e142a09f38854c2d840");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_05 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_05.prefab:06821994a69f851468ac0694e344503d");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_06 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_06.prefab:83ca55c537e3df3429ac37a67ffe25f9");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_TurnOne_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_TurnOne_01.prefab:7702bf67f79a89643a49d91745bc0000");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_01 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_01.prefab:7f7f9af17e3db0249ac4b62a87f5681f");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_02 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_02.prefab:da28857aa4b21e348a94d31e5397972a");
  private static readonly AssetReference VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_03 = new AssetReference("VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_03.prefab:46f1b3f800c9f304aa55bf2599315e43");
  private List<string> m_RopeExplodes = new List<string>()
  {
    (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_01,
    (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_02,
    (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_03,
    (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_04,
    (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_05,
    (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_06
  };
  private List<string> m_TurnStart = new List<string>()
  {
    (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_01,
    (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_02,
    (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_BossTimeWarp_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_Death_02,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_Intro_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_PlayerNozdormu_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_PlayerTemporus_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_PlayerTimeWarp_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_02,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_03,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_04,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_05,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_RopeExplode_06,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_TurnOne_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_01,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_02,
      (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_TurnStart_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_Death_02;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_EmoteResponse_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => false;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Rakanishu"))
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
    DALA_Dungeon_Boss_43h dalaDungeonBoss43h = this;
    while (dalaDungeonBoss43h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss43h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss43h.m_TurnStart);
        break;
      case 102:
        yield return (object) dalaDungeonBoss43h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss43h.m_RopeExplodes);
        break;
      case 103:
        yield return (object) dalaDungeonBoss43h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_TurnOne_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss43h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_43h dalaDungeonBoss43h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss43h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss43h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss43h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss43h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss43h.m_playedLines.Add(cardId);
      if (!(cardId == "EX1_560"))
      {
        if (!(cardId == "LOOT_538"))
        {
          if (cardId == "UNG_028t")
            yield return (object) dalaDungeonBoss43h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_PlayerTimeWarp_01);
        }
        else
          yield return (object) dalaDungeonBoss43h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_PlayerTemporus_01);
      }
      else
        yield return (object) dalaDungeonBoss43h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_PlayerNozdormu_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_43h dalaDungeonBoss43h = this;
    while (dalaDungeonBoss43h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss43h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss43h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss43h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "UNG_028t")
        yield return (object) dalaDungeonBoss43h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_43h.VO_DALA_BOSS_43h_Female_BloodElf_BossTimeWarp_01);
    }
  }
}
