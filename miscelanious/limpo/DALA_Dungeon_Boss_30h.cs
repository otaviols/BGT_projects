using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_30h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_30h_Male_Rat_Death = new AssetReference("VO_DALA_BOSS_30h_Male_Rat_Death.prefab:fb52c37daa34db346a08f66d8c66b8ce");
  private static readonly AssetReference VO_DALA_BOSS_30h_Male_Rat_DefeatPlayer = new AssetReference("VO_DALA_BOSS_30h_Male_Rat_DefeatPlayer.prefab:04d6baeac2746274185b97dfbccc033c");
  private static readonly AssetReference VO_DALA_BOSS_30h_Male_Rat_EmoteResponse = new AssetReference("VO_DALA_BOSS_30h_Male_Rat_EmoteResponse.prefab:5b16504a10dbb4f45af73cb1b4da7553");
  private static readonly AssetReference VO_DALA_BOSS_30h_Male_Rat_Intro = new AssetReference("VO_DALA_BOSS_30h_Male_Rat_Intro.prefab:fb83d73d78d4ce64eaae1db5f7715261");
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_30h.VO_DALA_BOSS_30h_Male_Rat_Death,
      (string) DALA_Dungeon_Boss_30h.VO_DALA_BOSS_30h_Male_Rat_DefeatPlayer,
      (string) DALA_Dungeon_Boss_30h.VO_DALA_BOSS_30h_Male_Rat_EmoteResponse,
      (string) DALA_Dungeon_Boss_30h.VO_DALA_BOSS_30h_Male_Rat_Intro
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_30h.VO_DALA_BOSS_30h_Male_Rat_Intro;
    this.m_deathLine = (string) DALA_Dungeon_Boss_30h.VO_DALA_BOSS_30h_Male_Rat_Death;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_30h.VO_DALA_BOSS_30h_Male_Rat_EmoteResponse;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "DALA_Vessina") || !(cardId != "DALA_Barkeye") || !(cardId != "DALA_Squeamlish"))
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
    DALA_Dungeon_Boss_30h dalaDungeonBoss30h = this;
    while (dalaDungeonBoss30h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss30h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_30h dalaDungeonBoss30h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss30h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss30h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss30h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss30h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss30h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_30h dalaDungeonBoss30h = this;
    while (dalaDungeonBoss30h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss30h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss30h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss30h.m_playedLines.Add(cardId);
    }
  }
}
