using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOT_Dungeon_BOSS_15h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_GeorgeTaggedInLines = new List<string>()
  {
    "VO_LOOTA_BOSS_15h_Male_Human_TaggedIn1_01.prefab:2f55e8f736f9f98458fb8f08f58c5a36",
    "VO_LOOTA_BOSS_15h_Male_Human_TaggedIn2_01.prefab:18063202c6aa5c749bf231bfa74ed27c",
    "VO_LOOTA_BOSS_15h_Male_Human_TaggedIn3_01.prefab:239aa3d876cb06b4eab89fc00fd64c22"
  };
  private List<string> m_KarlTaggedInLines = new List<string>()
  {
    "VO_LOOTA_BOSS_32h_Male_Human_TaggedIn1_01.prefab:700ba1a448e654044ac8f9af7982b78f",
    "VO_LOOTA_BOSS_32h_Male_Human_TaggedIn2_01.prefab:3efa9593adabf4c4eb74275eebb477bb",
    "VO_LOOTA_BOSS_32h_Male_Human_TaggedIn3_01.prefab:992d77a6eeaa7f643b22870a0efa0870"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_15h_Male_Human_Intro_01.prefab:eac2e5310989138459bcaccb9cf1e262",
      "VO_LOOTA_BOSS_15h_Male_Human_EmoteResponse_01.prefab:418380d3659570c4c97e6c1c118a9f6f",
      "VO_LOOTA_BOSS_15h_Male_Human_TaggedIn1_01.prefab:2f55e8f736f9f98458fb8f08f58c5a36",
      "VO_LOOTA_BOSS_15h_Male_Human_TaggedIn2_01.prefab:18063202c6aa5c749bf231bfa74ed27c",
      "VO_LOOTA_BOSS_15h_Male_Human_TaggedIn3_01.prefab:239aa3d876cb06b4eab89fc00fd64c22",
      "VO_LOOTA_BOSS_15h_Male_Human_Death_01.prefab:708c935fa2ec89b42bc095e8bf8a221c",
      "VO_LOOTA_BOSS_15h_Male_Human_PartnerDeath_01.prefab:e4fe992eef19428439e269f5ac37b08b",
      "VO_LOOTA_BOSS_15h_Male_Human_DefeatPlayer_01.prefab:83840107361ae9c479e52940ce8edf52",
      "VO_LOOTA_BOSS_32h_Male_Human_EmoteResponse_01.prefab:8ef89068a08ab764d9edd2ffd51fecca",
      "VO_LOOTA_BOSS_32h_Male_Human_TaggedIn1_01.prefab:700ba1a448e654044ac8f9af7982b78f",
      "VO_LOOTA_BOSS_32h_Male_Human_TaggedIn2_01.prefab:3efa9593adabf4c4eb74275eebb477bb",
      "VO_LOOTA_BOSS_32h_Male_Human_TaggedIn3_01.prefab:992d77a6eeaa7f643b22870a0efa0870",
      "VO_LOOTA_BOSS_32h_Male_Human_Death_01.prefab:a79d401349083e54597da55c1b21e511",
      "VO_LOOTA_BOSS_32h_Male_Human_PartnerDeath_01.prefab:9b8fa70ea9b43244e832cc445cada336"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine()
  {
    string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    if (cardId == "LOOTA_BOSS_15h")
      return "VO_LOOTA_BOSS_15h_Male_Human_Death_01.prefab:708c935fa2ec89b42bc095e8bf8a221c";
    return cardId == "LOOTA_BOSS_32h" ? "VO_LOOTA_BOSS_32h_Male_Human_Death_01.prefab:a79d401349083e54597da55c1b21e511" : (string) null;
  }

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_15h_Male_Human_Intro_01.prefab:eac2e5310989138459bcaccb9cf1e262", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      if (cardId == "LOOTA_BOSS_15h")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_15h_Male_Human_EmoteResponse_01.prefab:418380d3659570c4c97e6c1c118a9f6f", Notification.SpeechBubbleDirection.TopRight, actor));
      if (!(cardId == "LOOTA_BOSS_32h"))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_32h_Male_Human_EmoteResponse_01.prefab:8ef89068a08ab764d9edd2ffd51fecca", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_15h lootDungeonBoss15h = this;
    while (lootDungeonBoss15h.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!lootDungeonBoss15h.m_playedLines.Contains(str))
    {
      yield return (object) lootDungeonBoss15h.PlayLoyalSideKickBetrayal(missionEvent);
      Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName("");
      Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).UpdateHeroNameBanner();
      string randomLine;
      switch (missionEvent)
      {
        case 101:
          int num1 = 100;
          int num2 = Random.Range(0, 100);
          if (lootDungeonBoss15h.m_GeorgeTaggedInLines.Count == 0 || num1 < num2)
            break;
          randomLine = lootDungeonBoss15h.m_GeorgeTaggedInLines[Random.Range(0, lootDungeonBoss15h.m_GeorgeTaggedInLines.Count)];
          yield return (object) lootDungeonBoss15h.PlayLineOnlyOnce(enemyActor, randomLine);
          lootDungeonBoss15h.m_GeorgeTaggedInLines.Remove(randomLine);
          yield return (object) null;
          randomLine = (string) null;
          break;
        case 102:
          int num3 = 100;
          int num4 = Random.Range(0, 100);
          if (lootDungeonBoss15h.m_KarlTaggedInLines.Count == 0 || num3 < num4)
            break;
          randomLine = lootDungeonBoss15h.m_KarlTaggedInLines[Random.Range(0, lootDungeonBoss15h.m_KarlTaggedInLines.Count)];
          yield return (object) lootDungeonBoss15h.PlayLineOnlyOnce(enemyActor, randomLine);
          lootDungeonBoss15h.m_KarlTaggedInLines.Remove(randomLine);
          yield return (object) null;
          randomLine = (string) null;
          break;
        case 103:
          yield return (object) lootDungeonBoss15h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_32h_Male_Human_PartnerDeath_01.prefab:9b8fa70ea9b43244e832cc445cada336");
          break;
        case 104:
          yield return (object) lootDungeonBoss15h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_15h_Male_Human_PartnerDeath_01.prefab:e4fe992eef19428439e269f5ac37b08b");
          break;
      }
    }
  }
}
