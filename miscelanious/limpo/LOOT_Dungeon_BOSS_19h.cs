using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_19h : LOOT_Dungeon
{
  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_19h_Male_Trogg_Intro_01.prefab:00df9f15d69d8ce4e8553e579e3ff728",
      "VO_LOOTA_BOSS_19h_Male_Trogg_EmoteResponse_01.prefab:4385254ca60d5d64eb86d9341372d69f",
      "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPower1_01.prefab:5329b61147fc6f94a849ed713935994d",
      "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPower2_01.prefab:a0a22baa0aa03b54b8b9da60451603fa",
      "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPower3_01.prefab:59c9152915ad0d246a3033403363ab4c",
      "VO_LOOTA_BOSS_19h_Male_Trogg_Death_01.prefab:d0fa743934bc7a24db09df3af3ce0b77",
      "VO_LOOTA_BOSS_19h_Male_Trogg_DefeatPlayer_01.prefab:0a0997eeb9130dc4382df8e2f6c23b2d"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPower1_01.prefab:5329b61147fc6f94a849ed713935994d",
    "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPower2_01.prefab:a0a22baa0aa03b54b8b9da60451603fa",
    "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPower3_01.prefab:59c9152915ad0d246a3033403363ab4c"
  };

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_19h_Male_Trogg_Death_01.prefab:d0fa743934bc7a24db09df3af3ce0b77";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_19h_Male_Trogg_Intro_01.prefab:00df9f15d69d8ce4e8553e579e3ff728", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_19h_Male_Trogg_EmoteResponse_01.prefab:4385254ca60d5d64eb86d9341372d69f", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_19h lootDungeonBoss19h = this;
    while (lootDungeonBoss19h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss19h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
