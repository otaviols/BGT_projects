using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOT_Dungeon_BOSS_25h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_TreasureDestroyedLines = new List<string>()
  {
    "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy1_01.prefab:23e65c02cbddc904cb3eab22458ce562",
    "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy2_01.prefab:f6385398f90fcc04298333ec6aea9d0b",
    "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy3_01.prefab:33e551f7421525d42ab6e70ffa3e3177",
    "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy4_02.prefab:12c9b6c6be374d740b3d1f65ebe63e41",
    "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy5_01.prefab:d1aebeaf8b55f2b4e9780dfdaa0dc47b"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_25h_Male_Dragon_Intro_01.prefab:add998ba4e2e01b44aaac7fba9c2b4cc",
      "VO_LOOTA_BOSS_25h_Male_Dragon_EmoteResponse_01.prefab:e90ae3d9090ead040a3b44f8a4e01c4b",
      "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy1_01.prefab:23e65c02cbddc904cb3eab22458ce562",
      "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy2_01.prefab:f6385398f90fcc04298333ec6aea9d0b",
      "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy3_01.prefab:33e551f7421525d42ab6e70ffa3e3177",
      "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy4_02.prefab:12c9b6c6be374d740b3d1f65ebe63e41",
      "VO_LOOTA_BOSS_25h_Male_Dragon_TreasureDestroy5_01.prefab:d1aebeaf8b55f2b4e9780dfdaa0dc47b",
      "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower1_01.prefab:65776ccd923a75f40bb012264058ca12",
      "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower2_01.prefab:60d746ad07b34e341bc1bc84c7a07dd1",
      "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower3_01.prefab:7de0b19ebe39ca247b8421f748b98525",
      "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower4_01.prefab:7172dc8c3e6a2f94787abc3e997249f0",
      "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower5_01.prefab:8610ec1c1ee27094894ec6f10d8b2f88",
      "VO_LOOTA_BOSS_25h_Male_Dragon_Death_01.prefab:663cc8fb06978b2419904c179be13721",
      "VO_LOOTA_BOSS_25h_Male_Dragon_DefeatPlayer_01.prefab:4cb072ca9af2c1d41b2b50ef57bbd7e2",
      "VO_LOOTA_BOSS_25h_Male_Dragon_EventMarinTheFox_01.prefab:7660408d472cffa498431e1c6bf0b6bb"
    })
      this.PreloadSound(soundPath);
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_LOOTFinalBoss);

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower1_01.prefab:65776ccd923a75f40bb012264058ca12",
    "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower2_01.prefab:60d746ad07b34e341bc1bc84c7a07dd1",
    "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower3_01.prefab:7de0b19ebe39ca247b8421f748b98525",
    "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower4_01.prefab:7172dc8c3e6a2f94787abc3e997249f0",
    "VO_LOOTA_BOSS_25h_Male_Dragon_HeroPower5_01.prefab:8610ec1c1ee27094894ec6f10d8b2f88"
  };

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_25h_Male_Dragon_Death_01.prefab:663cc8fb06978b2419904c179be13721";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_25h_Male_Dragon_Intro_01.prefab:add998ba4e2e01b44aaac7fba9c2b4cc", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_25h_Male_Dragon_EmoteResponse_01.prefab:e90ae3d9090ead040a3b44f8a4e01c4b", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_25h lootDungeonBoss25h = this;
    while (lootDungeonBoss25h.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!lootDungeonBoss25h.m_playedLines.Contains(str))
    {
      yield return (object) lootDungeonBoss25h.PlayLoyalSideKickBetrayal(missionEvent);
      if (missionEvent == 101)
      {
        int num1 = 50;
        int num2 = Random.Range(0, 100);
        if (lootDungeonBoss25h.m_TreasureDestroyedLines.Count != 0 && num1 >= num2)
        {
          string randomLine = lootDungeonBoss25h.m_TreasureDestroyedLines[Random.Range(0, lootDungeonBoss25h.m_TreasureDestroyedLines.Count)];
          yield return (object) lootDungeonBoss25h.PlayLineOnlyOnce(enemyActor, randomLine);
          lootDungeonBoss25h.m_TreasureDestroyedLines.Remove(randomLine);
          yield return (object) null;
          randomLine = (string) null;
        }
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    LOOT_Dungeon_BOSS_25h lootDungeonBoss25h = this;
    while (lootDungeonBoss25h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!lootDungeonBoss25h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) lootDungeonBoss25h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      lootDungeonBoss25h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "LOOT_357")
      {
        yield return (object) lootDungeonBoss25h.PlayEasterEggLine(actor, "VO_LOOTA_BOSS_25h_Male_Dragon_EventMarinTheFox_01.prefab:7660408d472cffa498431e1c6bf0b6bb");
        yield return (object) null;
      }
    }
  }
}
