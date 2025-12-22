using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_38h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_PlayerBirds = new List<string>()
  {
    "VO_GILA_BOSS_38h_Male_Goblin_EventPlayerPlayBird_01.prefab:c4ee445c7f3a05543b3d0a52a153fc4c",
    "VO_GILA_BOSS_38h_Male_Goblin_EventPlayerPlayBird_02.prefab:b74522c99d50c7743a2018b4339a4e43"
  };
  private List<string> m_BossBirds = new List<string>()
  {
    "VO_GILA_BOSS_38h_Male_Goblin_EventPlayBird_01.prefab:87fb7cb30ce446c42b2f94c7550f507f",
    "VO_GILA_BOSS_38h_Male_Goblin_EventPlayBird_02.prefab:9e41bc5cc0f12774bbb04ad8766d91a1",
    "VO_GILA_BOSS_38h_Male_Goblin_EventPlayBird_03.prefab:8cb03c80c4dc16f40a1bd685465fcfbb",
    "VO_GILA_BOSS_38h_Male_Goblin_EventPlayBird_04.prefab:83eb53ae594d4814d9ee125c175f7a24"
  };
  private List<string> m_NestingRoc = new List<string>()
  {
    "VO_GILA_BOSS_38h_Male_Goblin_EventPlayNestingRoc_01.prefab:1e538ce6838c7cb44bb42fbe7b5e4f94",
    "VO_GILA_BOSS_38h_Male_Goblin_EventPlayNestingRoc_02.prefab:08ff0ef9bfafb3c4eafcc03bb4b2a2dd"
  };
  private List<string> m_DeadBirds = new List<string>()
  {
    "VO_GILA_BOSS_38h_Male_Goblin_EventBirdDead_01.prefab:77653c50d328f8f4fb86dca2670da780",
    "VO_GILA_BOSS_38h_Male_Goblin_EventBirdDead_02.prefab:2f0df9f435f01ca4d860834b69b1fc18",
    "VO_GILA_BOSS_38h_Male_Goblin_EventBirdDead_03.prefab:cb381c7fe1966e84fa08f512e92d9122"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_38h_Male_Goblin_Intro_01.prefab:952991d4799b8014086abf0a8eb3a4ba",
      "VO_GILA_BOSS_38h_Male_Goblin_EmoteResponse_01.prefab:31f5ac41aa6b846419f4080611dfaf79",
      "VO_GILA_BOSS_38h_Male_Goblin_Death_01.prefab:cc33cd36cbe954e40b849f9e5123a58d",
      "VO_GILA_BOSS_38h_Male_Goblin_DefeatPlayer_01.prefab:c16741a44637661469202e067ca63c6b",
      "VO_GILA_BOSS_38h_Male_Goblin_HeroPower_01.prefab:037e6d75fb60c3c4bb81645ced779df7",
      "VO_GILA_BOSS_38h_Male_Goblin_HeroPower_02.prefab:d2d33eeb557d01649b17fc85b6c25811",
      "VO_GILA_BOSS_38h_Male_Goblin_HeroPower_03.prefab:65762f713cbb9a243b3ab042da2b4176",
      "VO_GILA_BOSS_38h_Male_Goblin_EventPlayNestingRoc_01.prefab:1e538ce6838c7cb44bb42fbe7b5e4f94",
      "VO_GILA_BOSS_38h_Male_Goblin_EventPlayNestingRoc_02.prefab:08ff0ef9bfafb3c4eafcc03bb4b2a2dd",
      "VO_GILA_BOSS_38h_Male_Goblin_EventPlayBird_01.prefab:87fb7cb30ce446c42b2f94c7550f507f",
      "VO_GILA_BOSS_38h_Male_Goblin_EventPlayBird_02.prefab:9e41bc5cc0f12774bbb04ad8766d91a1",
      "VO_GILA_BOSS_38h_Male_Goblin_EventPlayBird_03.prefab:8cb03c80c4dc16f40a1bd685465fcfbb",
      "VO_GILA_BOSS_38h_Male_Goblin_EventPlayBird_04.prefab:83eb53ae594d4814d9ee125c175f7a24",
      "VO_GILA_BOSS_38h_Male_Goblin_EventBirdDead_01.prefab:77653c50d328f8f4fb86dca2670da780",
      "VO_GILA_BOSS_38h_Male_Goblin_EventBirdDead_02.prefab:2f0df9f435f01ca4d860834b69b1fc18",
      "VO_GILA_BOSS_38h_Male_Goblin_EventBirdDead_03.prefab:cb381c7fe1966e84fa08f512e92d9122",
      "VO_GILA_BOSS_38h_Male_Goblin_EventPlayerPlayBird_01.prefab:c4ee445c7f3a05543b3d0a52a153fc4c",
      "VO_GILA_BOSS_38h_Male_Goblin_EventPlayerPlayBird_02.prefab:b74522c99d50c7743a2018b4339a4e43"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_38h gilDungeonBoss38h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) gilDungeonBoss38h.\u003C\u003En__0(entity);
    while (gilDungeonBoss38h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss38h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss38h.WaitForEntitySoundToFinish(entity);
      if (entity.GetControllerSide() == Player.Side.OPPOSING)
      {
        Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
        switch (entity.GetCardId())
        {
          case "CS2_169":
          case "CS2_203":
          case "CS2_237":
          case "EX1_009":
          case "GIL_664":
          case "ICC_023":
          case "KAR_037":
          case "KAR_300":
          case "LOOT_170":
          case "NEW1_016":
          case "UNG_027":
          case "UNG_912":
            string line1 = gilDungeonBoss38h.PopRandomLineWithChance(gilDungeonBoss38h.m_BossBirds);
            if (line1 == null)
              break;
            yield return (object) gilDungeonBoss38h.PlayLineOnlyOnce(actor, line1);
            break;
          case "UNG_801":
            string line2 = gilDungeonBoss38h.PopRandomLineWithChance(gilDungeonBoss38h.m_NestingRoc);
            if (line2 == null)
              break;
            yield return (object) gilDungeonBoss38h.PlayLineOnlyOnce(actor, line2);
            break;
        }
      }
    }
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_GILA_BOSS_38h_Male_Goblin_HeroPower_01.prefab:037e6d75fb60c3c4bb81645ced779df7",
    "VO_GILA_BOSS_38h_Male_Goblin_HeroPower_02.prefab:d2d33eeb557d01649b17fc85b6c25811",
    "VO_GILA_BOSS_38h_Male_Goblin_HeroPower_03.prefab:65762f713cbb9a243b3ab042da2b4176"
  };

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_38h_Male_Goblin_Death_01.prefab:cc33cd36cbe954e40b849f9e5123a58d";

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_38h_Male_Goblin_Intro_01.prefab:952991d4799b8014086abf0a8eb3a4ba", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_38h_Male_Goblin_EmoteResponse_01.prefab:31f5ac41aa6b846419f4080611dfaf79", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_38h gilDungeonBoss38h = this;
    while (gilDungeonBoss38h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 101)
    {
      string line = gilDungeonBoss38h.PopRandomLineWithChance(gilDungeonBoss38h.m_DeadBirds);
      if (line != null)
        yield return (object) gilDungeonBoss38h.PlayLineOnlyOnce(actor, line);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_38h gilDungeonBoss38h = this;
    while (gilDungeonBoss38h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss38h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss38h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss38h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "CS2_169":
        case "CS2_203":
        case "CS2_237":
        case "EX1_009":
        case "GIL_664":
        case "ICC_023":
        case "KAR_037":
        case "KAR_300":
        case "LOOT_170":
        case "NEW1_016":
        case "UNG_027":
        case "UNG_912":
          if (gilDungeonBoss38h.m_PlayerBirds.Count == 0)
            break;
          string line = gilDungeonBoss38h.PopRandomLineWithChance(gilDungeonBoss38h.m_PlayerBirds);
          if (line == null)
            break;
          yield return (object) gilDungeonBoss38h.PlayLineOnlyOnce(actor, line);
          break;
      }
    }
  }
}
