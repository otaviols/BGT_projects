using System.Collections;
using System.Collections.Generic;

public class DRGA_Evil_Fight_08 : DRGA_Dungeon
{
  private static readonly AssetReference VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_BossStart_01 = new AssetReference("VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_BossStart_01.prefab:ebeac07ad58ae754e8c171ebd6317504");
  private static readonly AssetReference VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_EmoteResponse_01 = new AssetReference("VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_EmoteResponse_01.prefab:e7b356cfaaee40d4e88bc1233435a2ec");
  private static readonly AssetReference VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_01_01 = new AssetReference("VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_01_01.prefab:a40fe4e468a6e7f439e2d3d27ccd347d");
  private static readonly AssetReference VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_02_01 = new AssetReference("VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_02_01.prefab:9cc60e17257bb9f42af260a22e946844");
  private static readonly AssetReference VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_03_01 = new AssetReference("VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_03_01.prefab:bd2012eac94e1284486522a117ce2848");
  private static readonly AssetReference VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_01_01 = new AssetReference("VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_01_01.prefab:23f2fb8953fe11e48a76432fc517c83a");
  private static readonly AssetReference VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_02_01 = new AssetReference("VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_02_01.prefab:062631af973847c46b0c110530af2373");
  private static readonly AssetReference VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_03_01 = new AssetReference("VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_03_01.prefab:773cc7e2beadfad4f9aa5ae0c2c5d212");
  private static readonly AssetReference VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_Death_01 = new AssetReference("VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_Death_01.prefab:8b8fcd0b96015c94c881060522820b6a");
  private List<string> m_VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_IdleLines = new List<string>()
  {
    (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_01_01,
    (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_02_01,
    (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_03_01
  };
  private List<string> m_VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_HeroPowerLines = new List<string>()
  {
    (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_01_01,
    (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_02_01,
    (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_03_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_BossStart_01,
      (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_EmoteResponse_01,
      (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_01_01,
      (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_02_01,
      (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_Idle_03_01,
      (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_01_01,
      (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_02_01,
      (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_HeroPower_03_01,
      (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_HeroPowerLines;

  public override List<string> GetIdleLines() => this.m_VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Good_Fight_09_Hero_Death_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_BossStart_01, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DRGA_Evil_Fight_08.VO_DRGA_BOSS_30h_Male_Human_Evil_Fight_08_EmoteResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DRGA_Evil_Fight_08 drgaEvilFight08 = this;
    while (drgaEvilFight08.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) drgaEvilFight08.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DRGA_Evil_Fight_08 drgaEvilFight08 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) drgaEvilFight08.\u003C\u003En__1(entity);
    while (drgaEvilFight08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!drgaEvilFight08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) drgaEvilFight08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      drgaEvilFight08.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DRGA_Evil_Fight_08 drgaEvilFight08 = this;
    while (drgaEvilFight08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!drgaEvilFight08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) drgaEvilFight08.\u003C\u003En__2(entity);
      yield return (object) drgaEvilFight08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      drgaEvilFight08.m_playedLines.Add(cardId);
    }
  }
}
