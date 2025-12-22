using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_12h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_BossKoboldLibrarian_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_BossKoboldLibrarian_01.prefab:c4ea6160f285a8041a7af7a4e94692d5");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_BossWarlockFire_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_BossWarlockFire_01.prefab:a8d5838aa6f449549974afc98124ec66");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_Death_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_Death_01.prefab:56812f5afbd0bf3479fd032f74523365");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_DefeatPlayer_01.prefab:b407121c31baf0841aa622e90ad2c914");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_EmoteResponse_01.prefab:094da1d1c9ee77f42ba6ebe6dc282af8");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_HeroPower_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_HeroPower_01.prefab:3201c16ca8c60824698cea51ac405e5a");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_HeroPower_02 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_HeroPower_02.prefab:3e3ec1e5872235c408ff993576d46954");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_HeroPower_03 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_HeroPower_03.prefab:e4548e1c94e6c634380f596c71023f8a");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_HeroPower_04 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_HeroPower_04.prefab:fc54bdd28cf3e8e44876392635d7f0e4");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_01.prefab:2c88303e921aa214ea51ed3aad08821b");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_02 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_02.prefab:3827b1bc856c8b341815f3fd316fde67");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_03 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_03.prefab:ecfae9b3afa8ce741b11931263741102");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_Idle_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_Idle_01.prefab:cd694d2162815d6479fbfa82dca9e059");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_Idle_02 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_Idle_02.prefab:5105ea76d8a9c5f47b357ebde68f86bf");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_Idle_03 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_Idle_03.prefab:dd44af340b2253144b642b417d967a0a");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_Idle_04 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_Idle_04.prefab:e5795e4bb0f614f40a31037485b6f966");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_Idle_05 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_Idle_05.prefab:f46931357ae078d4cbcffa839727f450");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_Idle_09 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_Idle_09.prefab:7a03d5200365105479c93a3fbbc8ecde");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_Intro_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_Intro_01.prefab:b4ac8ec00c00ff5449c62275a06fc1a8");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_IntroChu_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_IntroChu_01.prefab:468b3754ca20c31438dea75d82a58730");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_PlayerBabblingBook_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_PlayerBabblingBook_01.prefab:2c28a21064da51e44af2d891ef6eb248");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_PlayerBookOfSpectres_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_PlayerBookOfSpectres_01.prefab:6c1198fb7d76d9649b07b71353f54675");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_PlayerBookWyrm_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_PlayerBookWyrm_01.prefab:a5de50bd587defb439d29160811b4c5b");
  private static readonly AssetReference VO_DALA_BOSS_12h_Male_Undead_PlayerCabalistTome_01 = new AssetReference("VO_DALA_BOSS_12h_Male_Undead_PlayerCabalistTome_01.prefab:53700bd93e0b7cc40a2f7b527c28666a");
  private List<string> m_IdleSongLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_01,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_02,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_03,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_04,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_05,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_09
  };
  private List<string> m_HeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPower_01,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPower_02,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPower_03,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPower_04
  };
  private List<string> m_HeroPowerOne = new List<string>()
  {
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_01,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_02,
    (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_BossKoboldLibrarian_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_BossWarlockFire_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Death_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPower_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPower_02,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPower_03,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPower_04,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_02,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_HeroPowerOne_03,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_02,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_03,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_04,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_05,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Idle_09,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Intro_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_IntroChu_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_PlayerBabblingBook_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_PlayerBookOfSpectres_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_PlayerBookWyrm_01,
      (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_PlayerCabalistTome_01
    };
    this.SetBossVOLines(VOLines);
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_EmoteResponse_01;
  }

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (this.m_IdleSongLines.Count == 0)
      return;
    string idleSongLine = this.m_IdleSongLines[0];
    this.m_IdleSongLines.RemoveAt(0);
    Gameplay.Get().StartCoroutine(this.PlayBossLine(actor, idleSongLine));
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_George") || !(cardId != "DALA_Rakanishu") || !(cardId != "DALA_Chu"))
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

  protected override bool GetShouldSuppressDeathTextBubble() => false;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_12h dalaDungeonBoss12h = this;
    while (dalaDungeonBoss12h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss12h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss12h.m_HeroPower);
        break;
      case 102:
        yield return (object) dalaDungeonBoss12h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss12h.m_HeroPowerOne);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss12h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_12h dalaDungeonBoss12h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss12h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss12h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss12h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss12h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss12h.m_playedLines.Add(cardId);
      if (!(cardId == "KAR_009"))
      {
        if (!(cardId == "GIL_548"))
        {
          if (!(cardId == "KAR_033"))
          {
            if (cardId == "OG_090")
              yield return (object) dalaDungeonBoss12h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_PlayerCabalistTome_01);
          }
          else
            yield return (object) dalaDungeonBoss12h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_PlayerBookWyrm_01);
        }
        else
          yield return (object) dalaDungeonBoss12h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_PlayerBookOfSpectres_01);
      }
      else
        yield return (object) dalaDungeonBoss12h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_PlayerBabblingBook_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_12h dalaDungeonBoss12h = this;
    while (dalaDungeonBoss12h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss12h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss12h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss12h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "LOOT_014"))
      {
        if (cardId == "CS2_062" || cardId == "EX1_308")
          yield return (object) dalaDungeonBoss12h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_BossWarlockFire_01);
      }
      else
        yield return (object) dalaDungeonBoss12h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_12h.VO_DALA_BOSS_12h_Male_Undead_BossKoboldLibrarian_01);
    }
  }
}
