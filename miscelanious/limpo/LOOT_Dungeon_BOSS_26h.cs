using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_26h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_26h_Female_Demon_Intro_01.prefab:c703633103d735549b99de1f634199ca",
      "VO_LOOTA_BOSS_26h_Female_Demon_EmoteResponse_01.prefab:4e149016fdfa580488c77b1a96a3b591",
      "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkFrost_01.prefab:993f89ae525d2d04aa9feafca8a81c28",
      "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkDeath_01.prefab:6961bc705a859874693e7954322b011e",
      "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkConfuse_01.prefab:2c97b65ba728edc4c987d06fcdbccca3",
      "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkFear_01.prefab:7606ae781cbd88e49a460898e82c4ad7",
      "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkFire_01.prefab:d5b30fd8422782d428f4a77e6384e808",
      "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkDevitalize_01.prefab:0f45daf1f2aee9a489d3f5553bc440c1",
      "VO_LOOTA_BOSS_26h_Female_Demon_Death_01.prefab:bdf3d6fb4e212144b8d02428f5c09520",
      "VO_LOOTA_BOSS_26h_Female_Demon_DefeatPlayer_01.prefab:50208e375f2b0804b814051af470f108"
    })
      this.PreloadSound(soundPath);
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_LOOTFinalBoss);

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_26h_Female_Demon_Death_01.prefab:bdf3d6fb4e212144b8d02428f5c09520";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_26h_Female_Demon_Intro_01.prefab:c703633103d735549b99de1f634199ca", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_26h_Female_Demon_EmoteResponse_01.prefab:4e149016fdfa580488c77b1a96a3b591", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_26h lootDungeonBoss26h = this;
    while (lootDungeonBoss26h.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!lootDungeonBoss26h.m_playedLines.Contains(str))
    {
      yield return (object) lootDungeonBoss26h.PlayLoyalSideKickBetrayal(missionEvent);
      switch (missionEvent)
      {
        case 102:
          yield return (object) lootDungeonBoss26h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkFrost_01.prefab:993f89ae525d2d04aa9feafca8a81c28");
          break;
        case 103:
          yield return (object) lootDungeonBoss26h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkDeath_01.prefab:6961bc705a859874693e7954322b011e");
          break;
        case 104:
          yield return (object) lootDungeonBoss26h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkConfuse_01.prefab:2c97b65ba728edc4c987d06fcdbccca3");
          break;
        case 105:
          yield return (object) lootDungeonBoss26h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkFear_01.prefab:7606ae781cbd88e49a460898e82c4ad7");
          break;
        case 106:
          yield return (object) lootDungeonBoss26h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkFire_01.prefab:d5b30fd8422782d428f4a77e6384e808");
          break;
        case 107:
          yield return (object) lootDungeonBoss26h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_26h_Female_Demon_EyeStalkDevitalize_01.prefab:0f45daf1f2aee9a489d3f5553bc440c1");
          break;
      }
    }
  }
}
