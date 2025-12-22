using System.Collections;
using System.Collections.Generic;

public class LOOT_Dungeon_BOSS_40h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_40h_Female_Dragon_Intro_01.prefab:ab52d86f1d6df354cabcec7aa0c1d42a",
      "VO_LOOTA_BOSS_40h_Female_Dragon_EmoteResponse_01.prefab:652c16537ece0504592c667443bdd5af",
      "VO_LOOTA_BOSS_40h_Female_Dragon_HeroPower1_01.prefab:e00015e116756b84f9096d8c31f8f82c",
      "VO_LOOTA_BOSS_40h_Female_Dragon_HeroPower2_01.prefab:7174ac501266c4b468a4ab50bdeace74",
      "VO_LOOTA_BOSS_40h_Female_Dragon_HeroPower3_01.prefab:189f71527781ccb4b989b4f7c379c90b",
      "VO_LOOTA_BOSS_40h_Female_Dragon_HeroPower4_01.prefab:83833ba71f55f4a43b35406f6109c06b",
      "VO_LOOTA_BOSS_40h_Female_Dragon_Death_01.prefab:589c4cf8f4789df43a44f335a75a04ca",
      "VO_LOOTA_BOSS_40h_Female_Dragon_DefeatPlayer_01.prefab:d8327e1cd34b37d418c9650e73236ba4",
      "VO_LOOTA_BOSS_40h_Female_Dragon_EventPlayTwilightDrake_01.prefab:c246a514be5600b43a55849f5b024932",
      "VO_LOOTA_BOSS_40h_Female_Dragon_EventPlayTwilightWhelp_01.prefab:452f06d0867a0ef4dbcb4811d9c5f852"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_LOOTA_BOSS_40h_Female_Dragon_HeroPower1_01.prefab:e00015e116756b84f9096d8c31f8f82c",
    "VO_LOOTA_BOSS_40h_Female_Dragon_HeroPower2_01.prefab:7174ac501266c4b468a4ab50bdeace74",
    "VO_LOOTA_BOSS_40h_Female_Dragon_HeroPower3_01.prefab:189f71527781ccb4b989b4f7c379c90b",
    "VO_LOOTA_BOSS_40h_Female_Dragon_HeroPower4_01.prefab:83833ba71f55f4a43b35406f6109c06b"
  };

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_40h_Female_Dragon_Death_01.prefab:589c4cf8f4789df43a44f335a75a04ca";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_40h_Female_Dragon_Intro_01.prefab:ab52d86f1d6df354cabcec7aa0c1d42a", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_40h_Female_Dragon_EmoteResponse_01.prefab:652c16537ece0504592c667443bdd5af", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    LOOT_Dungeon_BOSS_40h lootDungeonBoss40h = this;
    while (lootDungeonBoss40h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!lootDungeonBoss40h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) lootDungeonBoss40h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      lootDungeonBoss40h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_043"))
      {
        if (cardId == "BRM_004")
        {
          yield return (object) lootDungeonBoss40h.PlayEasterEggLine(actor, "VO_LOOTA_BOSS_40h_Female_Dragon_EventPlayTwilightWhelp_01.prefab:452f06d0867a0ef4dbcb4811d9c5f852");
          yield return (object) null;
        }
      }
      else
      {
        yield return (object) lootDungeonBoss40h.PlayEasterEggLine(actor, "VO_LOOTA_BOSS_40h_Female_Dragon_EventPlayTwilightDrake_01.prefab:c246a514be5600b43a55849f5b024932");
        yield return (object) null;
      }
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_40h lootDungeonBoss40h = this;
    while (lootDungeonBoss40h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss40h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
