using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_05h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_05h_Male_Kobold_Intro_01.prefab:e1c7c45689b6591498aa76348e5a70f4",
      "VO_LOOTA_BOSS_05h_Male_Kobold_EmoteResponse_01.prefab:2934b191f5c48084f8ca1946cd1c12af",
      "VO_LOOTA_BOSS_05h_Male_Kobold_Death_01.prefab:71eccd84e3ff7c548bbdf11518c4bd77",
      "VO_LOOTA_BOSS_05h_Male_Kobold_HeroPower1_01.prefab:f658526e24d9d9c4a9e69ac192e1b6da",
      "VO_LOOTA_BOSS_05h_Male_Kobold_HeroPower2_01.prefab:767c3f76ae6a5404c9453bee77f002df",
      "VO_LOOTA_BOSS_05h_Male_Kobold_HeroPower3_01.prefab:753ada26bfe00f6419552fc39b17395b",
      "VO_LOOTA_BOSS_05h_Male_Kobold_DefeatPlayer_01.prefab:43c69f56c33d36d4f85a19f69e96f21f",
      "VO_LOOTA_BOSS_05h_Male_Kobold_EventBoomBots_01.prefab:3c88fbc01e9f5554282c392c82596c29",
      "VO_LOOTA_BOSS_05h_Male_Kobold_EventMadBomberDeath_01.prefab:cd159b7aa0612634ebd47d147e729157"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_LOOTA_BOSS_05h_Male_Kobold_HeroPower1_01.prefab:f658526e24d9d9c4a9e69ac192e1b6da",
    "VO_LOOTA_BOSS_05h_Male_Kobold_HeroPower2_01.prefab:767c3f76ae6a5404c9453bee77f002df",
    "VO_LOOTA_BOSS_05h_Male_Kobold_HeroPower3_01.prefab:753ada26bfe00f6419552fc39b17395b"
  };

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_05h_Male_Kobold_Death_01.prefab:71eccd84e3ff7c548bbdf11518c4bd77";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_05h_Male_Kobold_Intro_01.prefab:e1c7c45689b6591498aa76348e5a70f4", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_05h_Male_Kobold_EmoteResponse_01.prefab:2934b191f5c48084f8ca1946cd1c12af", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_05h lootDungeonBoss05h = this;
    while (lootDungeonBoss05h.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!lootDungeonBoss05h.m_playedLines.Contains(str))
    {
      yield return (object) lootDungeonBoss05h.PlayLoyalSideKickBetrayal(missionEvent);
      if (missionEvent == 102)
        yield return (object) lootDungeonBoss05h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_05h_Male_Kobold_EventMadBomberDeath_01.prefab:cd159b7aa0612634ebd47d147e729157");
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    LOOT_Dungeon_BOSS_05h lootDungeonBoss05h = this;
    while (lootDungeonBoss05h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!lootDungeonBoss05h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) lootDungeonBoss05h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      lootDungeonBoss05h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "LOOTA_838")
        yield return (object) lootDungeonBoss05h.PlayEasterEggLine(actor, "VO_LOOTA_BOSS_05h_Male_Kobold_EventBoomBots_01.prefab:3c88fbc01e9f5554282c392c82596c29");
    }
  }
}
