using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_19h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_BossCombo_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_BossCombo_01.prefab:148da7870579aa64bad30a0f94ce5d50");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_BossCombo_02 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_BossCombo_02.prefab:4307fbbaea5777642b157165bb52b308");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_BossEdwin_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_BossEdwin_01.prefab:3d2895ec7e3364943b405a8afa8b31ee");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_Death_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_Death_01.prefab:507e0991497a9334182687b0bfd36526");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_DefeatPlayer_01.prefab:79de80e97ed27b64889c5e2881a11372");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_EmoteResponse_01.prefab:356dadca5fdb91844a6b3a65d0f101b4");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_HeroPower_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_HeroPower_01.prefab:57c3195682cd0b8428e697506dc54a09");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_HeroPower_03 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_HeroPower_03.prefab:6b2faed3e88a04543b840434226f3de2");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_HeroPower_04 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_HeroPower_04.prefab:b6e56caae9bddcb41a88b9508c810d26");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_01.prefab:aaf90a661d82ad04aa5bdc72908e3755");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_02 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_02.prefab:1b52a3264a3418849a4670fa9515cb15");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_03 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_03.prefab:65fd4d2daa95df64086baf8446b38ffd");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_Idle_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_Idle_01.prefab:acc43c6c339944a43a542b601e2f43e8");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_Idle_02 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_Idle_02.prefab:278ac34ab6e3f514eb5c31a02c1541d4");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_Idle_03 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_Idle_03.prefab:9a7377bbcad54fa47a29bba0cd701310");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_Intro_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_Intro_01.prefab:137ac6ab94358c643be79969820efde1");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_IntroChu_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_IntroChu_01.prefab:08292dcf1d7b8ac45a5e26d4f36f6bda");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_PlayerEviscerate_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_PlayerEviscerate_01.prefab:ca0f6813553528e42b2eac6126c83313");
  private static readonly AssetReference VO_DALA_BOSS_19h_Female_Gnome_PlayerSmallWeapon_01 = new AssetReference("VO_DALA_BOSS_19h_Female_Gnome_PlayerSmallWeapon_01.prefab:3665f70f588f88442b925746edff3f02");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Idle_01,
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Idle_02,
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Idle_03
  };
  private static List<string> m_BossHeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPower_01,
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPower_03,
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPower_04
  };
  private static List<string> m_BossHeroPowerCombo = new List<string>()
  {
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_01,
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_02,
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_03
  };
  private static List<string> m_BossCombo = new List<string>()
  {
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_BossCombo_01,
    (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_BossCombo_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_BossCombo_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_BossCombo_02,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_BossEdwin_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Death_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPower_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPower_03,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPower_04,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_02,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_HeroPowerCombo_03,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Idle_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Idle_02,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Idle_03,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Intro_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_IntroChu_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_PlayerEviscerate_01,
      (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_PlayerSmallWeapon_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_19h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_Chu")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_IntroChu_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    DALA_Dungeon_Boss_19h dalaDungeonBoss19h = this;
    while (dalaDungeonBoss19h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss19h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_19h.m_BossHeroPower);
        break;
      case 102:
        yield return (object) dalaDungeonBoss19h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_19h.m_BossHeroPowerCombo);
        break;
      case 103:
        yield return (object) dalaDungeonBoss19h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_19h.m_BossCombo);
        break;
      case 104:
        yield return (object) dalaDungeonBoss19h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_PlayerSmallWeapon_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss19h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_19h dalaDungeonBoss19h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss19h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss19h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss19h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss19h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss19h.m_playedLines.Add(cardId);
      if (cardId == "EX1_124")
        yield return (object) dalaDungeonBoss19h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_PlayerEviscerate_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_19h dalaDungeonBoss19h = this;
    while (dalaDungeonBoss19h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss19h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss19h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss19h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "EX1_613")
        yield return (object) dalaDungeonBoss19h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_19h.VO_DALA_BOSS_19h_Female_Gnome_BossEdwin_01);
    }
  }
}
