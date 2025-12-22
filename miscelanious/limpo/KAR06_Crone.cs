using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR06_Crone : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_Crone_Female_Troll_CroneEmoteResponse_02.prefab:84a22c615324303489aa69ffb9423a7f");
    this.PreloadSound("VO_Crone_Female_Troll_CroneFlyingMonkeys_01.prefab:8f6b0cbfef4a3384286ec890a06d1d10");
    this.PreloadSound("VO_Crone_Female_Troll_CroneHeroPower_02.prefab:d368549c6d3ac224085adc9379623580");
    this.PreloadSound("VO_KARA_04_01_Female_Human_CroneLionTigerBear_02.prefab:3e90aeef2f8f4a243b34e38327499e93");
    this.PreloadSound("VO_KARA_04_01_Female_Human_CroneHuffer_01.prefab:08d4777be932bda4b9c516544e0f6dea");
    this.PreloadSound("VO_KARA_04_01_Female_Human_CroneTurn1_01.prefab:7b284f2e4c3942749a4841ee78d89d9f");
    this.PreloadSound("VO_KARA_04_01_Female_Human_CroneTurn3_01.prefab:48747ac249add864bb4c2e5a28b27205");
    this.PreloadSound("VO_Moroes_Male_Human_CroneTurn5_02.prefab:28a0e089d96edd241a04d24a6d686be5");
    this.PreloadSound("VO_Moroes_Male_Human_CroneWin_01.prefab:b3887f0f8b013314495204d89d64c121");
  }

  protected override void InitEmoteResponses() => this.m_emoteResponseGroups = new List<MissionEntity.EmoteResponseGroup>()
  {
    new MissionEntity.EmoteResponseGroup()
    {
      m_triggers = new List<EmoteType>((IEnumerable<EmoteType>) MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS),
      m_responses = new List<MissionEntity.EmoteResponse>()
      {
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_Crone_Female_Troll_CroneHeroPower_02.prefab:d368549c6d3ac224085adc9379623580",
          m_stringTag = "VO_Crone_Female_Troll_CroneHeroPower_02"
        }
      }
    }
  };

  private Actor GetDorothee()
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    foreach (Card card in friendlySidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == friendlySidePlayer.GetPlayerId() && entity.GetCardId() == "KARA_04_01")
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    KAR06_Crone kaR06Crone = this;
    while (kaR06Crone.m_enemySpeaking)
      yield return (object) null;
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!kaR06Crone.m_playedLines.Contains(str))
    {
      kaR06Crone.m_playedLines.Add(str);
      Actor dorothee = kaR06Crone.GetDorothee();
      if (!((Object) dorothee == (Object) null))
      {
        switch (missionEvent)
        {
          case 1:
            yield return (object) kaR06Crone.PlayEasterEggLine(dorothee, "VO_KARA_04_01_Female_Human_CroneLionTigerBear_02.prefab:3e90aeef2f8f4a243b34e38327499e93");
            break;
          case 2:
            yield return (object) kaR06Crone.PlayEasterEggLine(dorothee, "VO_KARA_04_01_Female_Human_CroneHuffer_01.prefab:08d4777be932bda4b9c516544e0f6dea");
            break;
        }
      }
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR06_Crone kaR06Crone = this;
    while (kaR06Crone.m_enemySpeaking)
      yield return (object) null;
    Actor dorothee = kaR06Crone.GetDorothee();
    if (!((Object) dorothee == (Object) null) || turn >= 7)
    {
      switch (turn)
      {
        case 1:
          yield return (object) kaR06Crone.PlayOpeningLine(dorothee, "VO_KARA_04_01_Female_Human_CroneTurn1_01.prefab:7b284f2e4c3942749a4841ee78d89d9f");
          break;
        case 5:
          yield return (object) kaR06Crone.PlayMissionFlavorLine(dorothee, "VO_KARA_04_01_Female_Human_CroneTurn3_01.prefab:48747ac249add864bb4c2e5a28b27205");
          break;
        case 10:
          GameState.Get().SetBusy(true);
          yield return (object) kaR06Crone.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_CroneTurn5_02.prefab:28a0e089d96edd241a04d24a6d686be5");
          GameState.Get().SetBusy(false);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    KAR06_Crone kaR06Crone = this;
    while (kaR06Crone.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR06Crone.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR06Crone.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "KARA_04_05") && !(cardId == "KARA_04_05h"))
      {
        if (cardId == "KARA_04_02hp")
          yield return (object) kaR06Crone.PlayCriticalLine(actor, "VO_Crone_Female_Troll_CroneEmoteResponse_02.prefab:84a22c615324303489aa69ffb9423a7f");
      }
      else
        yield return (object) kaR06Crone.PlayBossLine(actor, "VO_Crone_Female_Troll_CroneFlyingMonkeys_01.prefab:8f6b0cbfef4a3384286ec890a06d1d10");
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR06_Crone kaR06Crone = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR06Crone.PlayClosingLine("Moroes_Quote.prefab:ea3a21837aab2b0448ce4090103724cf", "VO_Moroes_Male_Human_CroneWin_01.prefab:b3887f0f8b013314495204d89d64c121");
    }
  }
}
