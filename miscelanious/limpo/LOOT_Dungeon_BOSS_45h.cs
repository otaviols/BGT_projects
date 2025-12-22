using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_45h : LOOT_Dungeon
{
  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_45h_Male_Gnome_Intro_01.prefab:bfdb9f8e2c3f0494083ce5aa230c7f56",
      "VO_LOOTA_BOSS_45h_Male_Gnome_EmoteResponse_01.prefab:77e7d3d50d4efce4b87b1cad34b67048",
      "VO_LOOTA_BOSS_45h_Male_Gnome_HeroPower1_01.prefab:ddfb713f6b7cd284daf2a96f21f731d8",
      "VO_LOOTA_BOSS_45h_Male_Gnome_HeroPower2_01.prefab:005a46770cc21444cbc8c05853a88481",
      "VO_LOOTA_BOSS_45h_Male_Gnome_HeroPower3_01.prefab:35be5eca8eae03342a5018b40c7741ba",
      "VO_LOOTA_BOSS_45h_Male_Gnome_HeroPower4_01.prefab:43f0b3d950d0bce4281f30f83c11cae2",
      "VO_LOOTA_BOSS_45h_Male_Gnome_Death_01.prefab:84f3bb48aefd92846aadc833a1928aeb",
      "VO_LOOTA_BOSS_45h_Male_Gnome_DefeatPlayer_01.prefab:bad2261cb6735c145a71f6fe50cf427a"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_LOOTA_BOSS_45h_Male_Gnome_HeroPower1_01.prefab:ddfb713f6b7cd284daf2a96f21f731d8",
    "VO_LOOTA_BOSS_45h_Male_Gnome_HeroPower2_01.prefab:005a46770cc21444cbc8c05853a88481",
    "VO_LOOTA_BOSS_45h_Male_Gnome_HeroPower3_01.prefab:35be5eca8eae03342a5018b40c7741ba",
    "VO_LOOTA_BOSS_45h_Male_Gnome_HeroPower4_01.prefab:43f0b3d950d0bce4281f30f83c11cae2"
  };

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_45h_Male_Gnome_Death_01.prefab:84f3bb48aefd92846aadc833a1928aeb";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_45h_Male_Gnome_Intro_01.prefab:bfdb9f8e2c3f0494083ce5aa230c7f56", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_45h_Male_Gnome_EmoteResponse_01.prefab:77e7d3d50d4efce4b87b1cad34b67048", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_45h lootDungeonBoss45h = this;
    while (lootDungeonBoss45h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss45h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
