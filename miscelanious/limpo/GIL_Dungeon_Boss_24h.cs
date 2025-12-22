using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_24h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_ZombieLines = new List<string>()
  {
    "VO_GILA_BOSS_24h_Male_Bogbeast_HeroPower_01.prefab:68e1cf7b4e5d11247aba64f927a7e1d6",
    "VO_GILA_BOSS_24h_Male_Bogbeast_HeroPower_02.prefab:c7cde8cf1bff3ae4a9bb1f435a7eb176",
    "VO_GILA_BOSS_24h_Male_Bogbeast_HeroPower_04.prefab:ce0eb5b3f74d4dd4db6d6cbf766724b2"
  };
  private List<string> m_RandomLines = new List<string>()
  {
    "VO_GILA_BOSS_24h_Male_Bogbeast_EventPlaysSmallMinion_01.prefab:2fa04f3ef4d736c479204e71bdd26999",
    "VO_GILA_BOSS_24h_Male_Bogbeast_EventPlaysSmallMinion_02.prefab:0e5a979ec0902f046a1721e40fe035fe",
    "VO_GILA_BOSS_24h_Male_Bogbeast_EventPlaysSmallMinion_03.prefab:45a145dbb9dfc0841ab7d534229729ca",
    "VO_GILA_BOSS_24h_Male_Bogbeast_EventPlaysSmallMinion_04.prefab:3d6f6e82106914044b728e6311bfe1fc"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_24h_Male_Bogbeast_Intro_01.prefab:9a2ea2d619351344dbf3193778af9c76",
      "VO_GILA_BOSS_24h_Male_Bogbeast_EmoteResponse_01.prefab:fba37407e0659ed438a33f8123ae4cf7",
      "VO_GILA_BOSS_24h_Male_Bogbeast_Death_01.prefab:3a813a650b1526e48bb50c4d3859ffe2",
      "VO_GILA_BOSS_24h_Male_Bogbeast_DefeatPlayer_01.prefab:78e984ed197b3024588f7d47bd76bd7a",
      "VO_GILA_BOSS_24h_Male_Bogbeast_EventPlaysSmallMinion_01.prefab:2fa04f3ef4d736c479204e71bdd26999",
      "VO_GILA_BOSS_24h_Male_Bogbeast_EventPlaysSmallMinion_02.prefab:0e5a979ec0902f046a1721e40fe035fe",
      "VO_GILA_BOSS_24h_Male_Bogbeast_EventPlaysSmallMinion_03.prefab:45a145dbb9dfc0841ab7d534229729ca",
      "VO_GILA_BOSS_24h_Male_Bogbeast_EventPlaysSmallMinion_04.prefab:3d6f6e82106914044b728e6311bfe1fc",
      "VO_GILA_BOSS_24h_Male_Bogbeast_HeroPower_01.prefab:68e1cf7b4e5d11247aba64f927a7e1d6",
      "VO_GILA_BOSS_24h_Male_Bogbeast_HeroPower_02.prefab:c7cde8cf1bff3ae4a9bb1f435a7eb176",
      "VO_GILA_BOSS_24h_Male_Bogbeast_HeroPower_04.prefab:ce0eb5b3f74d4dd4db6d6cbf766724b2"
    })
      this.PreloadSound(soundPath);
  }

  protected override float ChanceToPlayRandomVOLine() => 1f;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_24h_Male_Bogbeast_Intro_01.prefab:9a2ea2d619351344dbf3193778af9c76", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_24h_Male_Bogbeast_EmoteResponse_01.prefab:fba37407e0659ed438a33f8123ae4cf7", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_24h_Male_Bogbeast_Death_01.prefab:3a813a650b1526e48bb50c4d3859ffe2";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_24h gilDungeonBoss24h = this;
    while (gilDungeonBoss24h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss24h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss24h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss24h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (entity.GetHealth() == 1 && entity.IsMinion())
      {
        string line = gilDungeonBoss24h.PopRandomLineWithChance(gilDungeonBoss24h.m_RandomLines);
        if (line != null)
          yield return (object) gilDungeonBoss24h.PlayLineOnlyOnce(actor, line);
      }
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_24h gilDungeonBoss24h = this;
    while (gilDungeonBoss24h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!gilDungeonBoss24h.m_playedLines.Contains(str) && missionEvent == 101)
    {
      string line = gilDungeonBoss24h.PopRandomLineWithChance(gilDungeonBoss24h.m_ZombieLines);
      if (line != null)
        yield return (object) gilDungeonBoss24h.PlayLineOnlyOnce(actor, line);
    }
  }
}
