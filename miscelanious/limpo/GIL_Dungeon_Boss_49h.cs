using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_49h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_RandomPlayFaithfulLines = new List<string>()
  {
    "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_01.prefab:d9d094e9e67fac3458521751d4feb5b2",
    "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_02.prefab:0289abf9ce7d95949ae8b9c0c18412bb",
    "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_03.prefab:5fda1ce79f9a98442bcfb238e214937e",
    "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_04.prefab:0b1588ab70f395f4fb37eae0b35aec0d",
    "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_05.prefab:34d5190a3edea1745bec766025bee71e"
  };
  private List<string> m_RandomPlayPactLines = new List<string>()
  {
    "VO_GILA_BOSS_49h_Female_Undead_EventPact_01.prefab:ccdcf80420c1f234bbf1ce63c976ea7e",
    "VO_GILA_BOSS_49h_Female_Undead_EventPact_02.prefab:31395c54f2db7504e8843c6dfa385a96",
    "VO_GILA_BOSS_49h_Female_Undead_EventPact_03.prefab:22836152465e0b240aad0e8121f2d88f",
    "VO_GILA_BOSS_49h_Female_Undead_EventPact_05.prefab:9ce174ea9bda3044994468a83d6b0e04"
  };
  private List<string> m_RandomPlayFaithfulDeathLines = new List<string>()
  {
    "VO_GILA_BOSS_49h_Female_Undead_EventFaithfulDies_01.prefab:bbda8addb74a4d64e88928a14faec44e",
    "VO_GILA_BOSS_49h_Female_Undead_EventFaithfulDies_02.prefab:9bd5404fb376f8f4d93325153663c8ae",
    "VO_GILA_BOSS_49h_Female_Undead_EventFaithfulDies_03.prefab:988dd4ecb5ffd534d9d5ba128736c330"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_49h_Female_Undead_Intro_02.prefab:2f0acde3738fe4446865336c303d12a8",
      "VO_GILA_BOSS_49h_Female_Undead_EmoteResponse_01.prefab:bc5c2c4ceee01194fa3a2008208b7bd4",
      "VO_GILA_BOSS_49h_Female_Undead_Death_01.prefab:ca4b279f54ee8db4f9ec1907a85fd73e",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_01.prefab:d9d094e9e67fac3458521751d4feb5b2",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_02.prefab:0289abf9ce7d95949ae8b9c0c18412bb",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_03.prefab:5fda1ce79f9a98442bcfb238e214937e",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_04.prefab:0b1588ab70f395f4fb37eae0b35aec0d",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayFaithful_05.prefab:34d5190a3edea1745bec766025bee71e",
      "VO_GILA_BOSS_49h_Female_Undead_EventPact_01.prefab:ccdcf80420c1f234bbf1ce63c976ea7e",
      "VO_GILA_BOSS_49h_Female_Undead_EventPact_02.prefab:31395c54f2db7504e8843c6dfa385a96",
      "VO_GILA_BOSS_49h_Female_Undead_EventPact_03.prefab:22836152465e0b240aad0e8121f2d88f",
      "VO_GILA_BOSS_49h_Female_Undead_EventPact_05.prefab:9ce174ea9bda3044994468a83d6b0e04",
      "VO_GILA_BOSS_49h_Female_Undead_EventFaithfulDies_01.prefab:bbda8addb74a4d64e88928a14faec44e",
      "VO_GILA_BOSS_49h_Female_Undead_EventFaithfulDies_02.prefab:9bd5404fb376f8f4d93325153663c8ae",
      "VO_GILA_BOSS_49h_Female_Undead_EventFaithfulDies_03.prefab:988dd4ecb5ffd534d9d5ba128736c330",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayTreant_01.prefab:e92c0d40ff6129a47abcfc29f57b6e7b",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayBird_01.prefab:82defd64034acef4ab92df6b337f4562",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayCauldron_01.prefab:6a62fc2214d9c9c46acd777d8965cba2",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayHex_01.prefab:62c66cae6890a5a448a7f2840c80290a",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlaySnake_01.prefab:2d3b11843f5f807488ef0167c5dd158d",
      "VO_GILA_BOSS_49h_Female_Undead_EventPlayDragon_01.prefab:4e0a2fcb4940e5248acf3f7c6cde7b72",
      "VO_GILA_BOSS_49h_Female_Undead_EventTurn02_01.prefab:a52e60d47c36dac4bba54bde6e75c7c8",
      "VO_GILA_400h_Male_Human_EVENT_NEMESIS_TURN2_01.prefab:aa59fb8f1a646c042b1b22355393a9a3"
    })
      this.PreloadSound(soundPath);
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_GILFinalBoss);

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_49h_Female_Undead_Intro_02.prefab:2f0acde3738fe4446865336c303d12a8", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_49h_Female_Undead_EmoteResponse_01.prefab:bc5c2c4ceee01194fa3a2008208b7bd4", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_49h_Female_Undead_Death_01.prefab:ca4b279f54ee8db4f9ec1907a85fd73e";

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_49h gilDungeonBoss49h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) gilDungeonBoss49h.\u003C\u003En__0(entity);
    while (gilDungeonBoss49h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss49h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss49h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "GILA_BOSS_49t"))
      {
        if (cardId == "GILA_BOSS_49t2")
        {
          string line = gilDungeonBoss49h.PopRandomLineWithChance(gilDungeonBoss49h.m_RandomPlayPactLines);
          if (line != null)
            yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(actor, line);
        }
      }
      else
      {
        string line = gilDungeonBoss49h.PopRandomLineWithChance(gilDungeonBoss49h.m_RandomPlayFaithfulLines);
        if (line != null)
          yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(actor, line);
      }
    }
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_49h gilDungeonBoss49h = this;
    while (gilDungeonBoss49h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss49h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss49h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss49h.m_playedLines.Add(cardId);
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
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
          yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(enemyActor, "VO_GILA_BOSS_49h_Female_Undead_EventPlayBird_01.prefab:82defd64034acef4ab92df6b337f4562");
          break;
        case "EX1_158t":
        case "EX1_573t":
        case "EX1_tk9":
        case "FB_Champs_EX1_tk9":
        case "FP1_019t":
        case "GIL_663t":
        case "UNG_111t1":
          yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(enemyActor, "VO_GILA_BOSS_49h_Female_Undead_EventPlayTreant_01.prefab:e92c0d40ff6129a47abcfc29f57b6e7b");
          break;
        case "EX1_170":
        case "EX1_554t":
        case "LOE_010":
          yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(enemyActor, "VO_GILA_BOSS_49h_Female_Undead_EventPlaySnake_01.prefab:2d3b11843f5f807488ef0167c5dd158d");
          break;
        case "EX1_246":
          yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(enemyActor, "VO_GILA_BOSS_49h_Female_Undead_EventPlayHex_01.prefab:62c66cae6890a5a448a7f2840c80290a");
          break;
        case "GIL_819":
          yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(enemyActor, "VO_GILA_BOSS_49h_Female_Undead_EventPlayCauldron_01.prefab:6a62fc2214d9c9c46acd777d8965cba2");
          break;
      }
      if (entity.HasRace(TAG_RACE.DRAGON))
        yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(enemyActor, "VO_GILA_BOSS_49h_Female_Undead_EventPlayDragon_01.prefab:4e0a2fcb4940e5248acf3f7c6cde7b72");
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_49h gilDungeonBoss49h = this;
    while (gilDungeonBoss49h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!gilDungeonBoss49h.m_playedLines.Contains(str) && missionEvent == 101)
    {
      string line = gilDungeonBoss49h.PopRandomLineWithChance(gilDungeonBoss49h.m_RandomPlayFaithfulDeathLines);
      if (line != null)
        yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(actor, line);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    GIL_Dungeon_Boss_49h gilDungeonBoss49h = this;
    while (gilDungeonBoss49h.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    if (turn == 2)
    {
      GameState.Get().SetBusy(true);
      yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(actor, "VO_GILA_400h_Male_Human_EVENT_NEMESIS_TURN2_01.prefab:aa59fb8f1a646c042b1b22355393a9a3");
      yield return (object) gilDungeonBoss49h.PlayLineOnlyOnce(enemyActor, "VO_GILA_BOSS_49h_Female_Undead_EventTurn02_01.prefab:a52e60d47c36dac4bba54bde6e75c7c8");
      GameState.Get().SetBusy(false);
    }
  }
}
