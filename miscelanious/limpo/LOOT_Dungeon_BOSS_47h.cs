using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_47h : LOOT_Dungeon
{
  private static readonly AssetReference LOOTA_BOSS_47h_Lava_FilledRoom_Death = new AssetReference("LOOTA_BOSS_47h_Lava_FilledRoom_Death.prefab:2753d2ebd9bd40b458a33c552832df00");
  private static readonly AssetReference LOOTA_BOSS_47h_Lava_FilledRoom_Emote = new AssetReference("LOOTA_BOSS_47h_Lava_FilledRoom_Emote.prefab:ceb20cb5ceec4a549886c63442ac8b93");
  private static readonly AssetReference LOOTA_BOSS_47h_Lava_FilledRoom_Intro = new AssetReference("LOOTA_BOSS_47h_Lava_FilledRoom_Intro.prefab:79a4d695d025efc4a82d056a338932e5");

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) LOOT_Dungeon_BOSS_47h.LOOTA_BOSS_47h_Lava_FilledRoom_Death,
      (string) LOOT_Dungeon_BOSS_47h.LOOTA_BOSS_47h_Lava_FilledRoom_Emote,
      (string) LOOT_Dungeon_BOSS_47h.LOOTA_BOSS_47h_Lava_FilledRoom_Intro
    })
      this.PreloadSound(soundPath);
  }

  protected override string GetBossDeathLine() => (string) LOOT_Dungeon_BOSS_47h.LOOTA_BOSS_47h_Lava_FilledRoom_Death;

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_47h.LOOTA_BOSS_47h_Lava_FilledRoom_Intro, Notification.SpeechBubbleDirection.None, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) LOOT_Dungeon_BOSS_47h.LOOTA_BOSS_47h_Lava_FilledRoom_Emote, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_47h lootDungeonBoss47h = this;
    while (lootDungeonBoss47h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss47h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
