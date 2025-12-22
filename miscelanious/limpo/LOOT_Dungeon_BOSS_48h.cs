using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_48h : LOOT_Dungeon
{
  private static readonly AssetReference TrappedRoom_LOOTA_BOSS_48h_Death = new AssetReference("TrappedRoom_LOOTA_BOSS_48h_Death.prefab:a6c6e15236bcc405aafc279d56f13a3d");
  private static readonly AssetReference TrappedRoom_LOOTA_BOSS_48h_EmoteResponse = new AssetReference("TrappedRoom_LOOTA_BOSS_48h_EmoteResponse.prefab:36afbe32da4e24850860d944519508bc");
  private static readonly AssetReference TrappedRoom_LOOTA_BOSS_48h_Intro = new AssetReference("TrappedRoom_LOOTA_BOSS_48h_Intro.prefab:4d478c1f12dc2411d89e3f3b85fbcd85");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LOOT_Dungeon_BOSS_48h.TrappedRoom_LOOTA_BOSS_48h_Death,
      (string) LOOT_Dungeon_BOSS_48h.TrappedRoom_LOOTA_BOSS_48h_EmoteResponse,
      (string) LOOT_Dungeon_BOSS_48h.TrappedRoom_LOOTA_BOSS_48h_Intro
    })
      this.PreloadSound(soundPath);
  }

  protected override string GetBossDeathLine() => (string) LOOT_Dungeon_BOSS_48h.TrappedRoom_LOOTA_BOSS_48h_Death;

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_48h.TrappedRoom_LOOTA_BOSS_48h_Intro, Notification.SpeechBubbleDirection.None, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_48h.TrappedRoom_LOOTA_BOSS_48h_EmoteResponse, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_48h lootDungeonBoss48h = this;
    while (lootDungeonBoss48h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss48h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
