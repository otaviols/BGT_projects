using System.Collections;
using System.Collections.Generic;

public class BTA_Fight_21 : BTA_Dungeon_Heroic
{
  private static readonly AssetReference BTA_BOSS_21h_Supremus_Play = new AssetReference("BTA_BOSS_21h_Supremus_Play.prefab:e267c28d7ceda6442adfc092a8f825a1");
  private static readonly AssetReference BTA_BOSS_21h_Supremus_EmoteResponse = new AssetReference("BTA_BOSS_21h_Supremus_EmoteResponse.prefab:7878bf624db617843952fd7b939287ec");
  private static readonly AssetReference BTA_BOSS_21h_Supremus_Death = new AssetReference("BTA_BOSS_21h_Supremus_Death.prefab:a432f18252006904eaffffee747f5647");
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_21.BTA_BOSS_21h_Supremus_Play,
      (string) BTA_Fight_21.BTA_BOSS_21h_Supremus_EmoteResponse,
      (string) BTA_Fight_21.BTA_BOSS_21h_Supremus_Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_21.BTA_BOSS_21h_Supremus_Death;
    this.m_standardEmoteResponseLine = (string) BTA_Fight_21.BTA_BOSS_21h_Supremus_EmoteResponse;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_21.BTA_BOSS_21h_Supremus_Play, Notification.SpeechBubbleDirection.TopRight, actor));
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
    BTA_Fight_21 btaFight21 = this;
    while (btaFight21.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight21.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_21 btaFight21 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight21.\u003C\u003En__1(entity);
    while (btaFight21.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight21.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight21.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight21.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_21 btaFight21 = this;
    while (btaFight21.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight21.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight21.\u003C\u003En__2(entity);
      yield return (object) btaFight21.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight21.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_21 btaFight21 = this;
    while (btaFight21.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }
}
