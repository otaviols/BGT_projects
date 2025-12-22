using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_24h : LOOT_Dungeon
{
  private static readonly AssetReference TheMothergloop_LOOTA_BOSS_24h_Death = new AssetReference("TheMothergloop_LOOTA_BOSS_24h_Death.prefab:b6895bc5ad7734ebd92fbaa3c2f85743");
  private static readonly AssetReference TheMothergloop_LOOTA_BOSS_24h_EmoteResponse = new AssetReference("TheMothergloop_LOOTA_BOSS_24h_EmoteResponse.prefab:4ca93dcbf4ae74178bf3f5e74d667045");
  private static readonly AssetReference TheMothergloop_LOOTA_BOSS_24h_Intro = new AssetReference("TheMothergloop_LOOTA_BOSS_24h_Intro.prefab:b2cb09bd0fcd84227848236790d3d9ae");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LOOT_Dungeon_BOSS_24h.TheMothergloop_LOOTA_BOSS_24h_Death,
      (string) LOOT_Dungeon_BOSS_24h.TheMothergloop_LOOTA_BOSS_24h_EmoteResponse,
      (string) LOOT_Dungeon_BOSS_24h.TheMothergloop_LOOTA_BOSS_24h_Intro
    })
      this.PreloadSound(soundPath);
  }

  protected override string GetBossDeathLine() => (string) LOOT_Dungeon_BOSS_24h.TheMothergloop_LOOTA_BOSS_24h_Death;

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_24h.TheMothergloop_LOOTA_BOSS_24h_Intro, Notification.SpeechBubbleDirection.None, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_24h.TheMothergloop_LOOTA_BOSS_24h_EmoteResponse, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_24h lootDungeonBoss24h = this;
    while (lootDungeonBoss24h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss24h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
