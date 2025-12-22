using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICC_05_Lanathel : ICC_MissionEntity
{
  private Notification endTurnNotifier;
  private bool m_hasBiteNoTargetsVOPlayed;
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_BloodEssenceLines = new List<string>()
  {
    "VO_ICC05_Lanathel_Female_Sanlayn_BloodEssence_01.prefab:da59f033f11d565499ec9da44ce6f46b",
    "VO_ICC05_Lanathel_Female_Sanlayn_BloodEssence_02.prefab:e2c7a28dfc86e7a4c94d00b380224e54",
    "VO_ICC05_Lanathel_Female_Sanlayn_BloodEssence_03.prefab:5771ffb67a64672498e2996de625f0bc"
  };

  public override void PreloadAssets()
  {
    foreach (string soundPath in new List<string>()
    {
      "VO_ICC_841_Female_Sanlayn_Death_02.prefab:bcbec586c14d0ea47b294118f9fed020",
      "VO_ICC05_Lanathel_Female_Sanlayn_Intro_01.prefab:a5927ce80194b9d4abee763cc3451c13",
      "VO_ICC05_Lanathel_Female_Sanlayn_BossBite_01.prefab:e847622ab6b89e14b908a5092c681bd6",
      "VO_ICC05_Lanathel_Female_Sanlayn_BiteReminder_01.prefab:e7562028fc82af24e8b5408d70ae79d4",
      "VO_ICC05_Lanathel_Female_Sanlayn_PlayerBiteAcolyte_01.prefab:88ff928135afd344ebabeefc3747231e",
      "VO_ICC05_Lanathel_Female_Sanlayn_BloodEssence_01.prefab:da59f033f11d565499ec9da44ce6f46b",
      "VO_ICC05_Lanathel_Female_Sanlayn_BloodEssence_02.prefab:e2c7a28dfc86e7a4c94d00b380224e54",
      "VO_ICC05_Lanathel_Female_Sanlayn_BloodEssence_03.prefab:5771ffb67a64672498e2996de625f0bc",
      "VO_ICC05_Lanathel_Female_Sanlayn_Turn3_01.prefab:57e21d323fff0954dbcc930fbebb3f03",
      "VO_ICC05_Lanathel_Female_Sanlayn_Turn3_02.prefab:c6d817960f09ec94d842f61fbdd3c26a",
      "VO_ICC05_Lanathel_Female_Sanlayn_Wounded_01.prefab:b0dcbf6d181d5944e9e55b2f5a4b4bb1",
      "VO_ICC05_LichKing_Male_Human_Win_01.prefab:453fe532c40bbfa44a1aac8d99041f7e",
      "VO_ICC05_LichKing_Male_Human_Lose_01.prefab:9978afae17851584795146a392d5cb67",
      "VO_ICC05_Lanathel_Female_Sanlayn_EmoteResponse_01.prefab:8617d4664523cb94aa4847447cbfbb8f",
      "VO_ICC05_Lanathel_Female_Sanlayn_LichKing_01.prefab:2067c9859da805b4faea5fc664f0ada9",
      "VO_ICC05_LichKing_Male_Human_LichKing_02.prefab:a56f85a63706c304786eed0e3c861377",
      "VO_ICC05_Lanathel_Female_Sanlayn_TransformDK_01.prefab:534ffc634d9b4a74fa1a8067ebb0aa85",
      "VO_ICC05_Lanathel_Female_Sanlayn_BiteOoze_01.prefab:b3ff24b5195c4764fb18ef87f48f1fbd",
      "VO_ICC05_Lanathel_Female_Sanlayn_BiteSpikes_01.prefab:4b0a39c8dab009e4cb479e4e8bcd14ea",
      "VO_ICC05_Lanathel_Female_Sanlayn_BiteShell_01.prefab:229be1532f6fe31418f81b56784b4cd4",
      "VO_ICC05_Lanathel_Female_Sanlayn_BitePoisonous_01.prefab:e72be5987d5bb3144b46077aad7e23e7",
      "VO_ICC05_Lanathel_Female_Sanlayn_BloodPrince_01.prefab:c3b8a4ae2da8a4448a74c93f258f85c4",
      "VO_ICC05_Lanathel_Female_Sanlayn_PlayerBloodBite_01.prefab:c66830daa3ed9f045b6c3cbc347514c5",
      "VO_PrinceKeleseth_Male_Vampire_ResponseLanaThel_01.prefab:fddf65c2cd5115745845aa45b32017dc",
      "VO_PrinceTaldaram_Male_Vampire_ResponseLanaThel_01.prefab:ab050ef2ad0f9a4498de5779b3e3d677",
      "VO_PrinceValanar_Male_Vampire_ResponseLanaThel_01.prefab:3c482cfa6dfd9aa4182cfe35b66b4bcc"
    })
      this.PreloadSound(soundPath);
  }

  public override bool NotifyOfEndTurnButtonPushed()
  {
    bool flag1 = true;
    Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
    if (optionsPacket != null && optionsPacket.List != null)
    {
      if (optionsPacket.List.Count == 1)
      {
        NotificationManager.Get().DestroyAllArrows();
        return true;
      }
      for (int index = 0; index < optionsPacket.List.Count; ++index)
      {
        Network.Options.Option option = optionsPacket.List[index];
        if (option.Type == Network.Options.Option.OptionType.POWER && GameState.Get().GetEntity(option.Main.ID).GetCardId() == "ICCA05_002p" && option.Main.PlayErrorInfo.IsValid())
          flag1 = false;
      }
    }
    if (flag1)
      return true;
    bool flag2 = true;
    List<Card> cardList = new List<Card>();
    cardList.AddRange((IEnumerable<Card>) GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards());
    cardList.AddRange((IEnumerable<Card>) GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards());
    foreach (Card card in cardList)
    {
      if (!((Object) card == (Object) null) && card.GetEntity() != null)
      {
        Entity entity = card.GetEntity();
        bool flag3 = false;
        foreach (EntityBase enchantment in entity.GetEnchantments())
        {
          if (enchantment.GetCardId() == "ICCA05_002e")
            flag3 = true;
        }
        if (!flag3)
          flag2 = false;
      }
    }
    if (flag2)
      return true;
    if ((Object) this.endTurnNotifier != (Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.endTurnNotifier);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (!this.m_hasBiteNoTargetsVOPlayed)
    {
      if (this.m_enemySpeaking)
        return false;
      this.m_hasBiteNoTargetsVOPlayed = true;
      Gameplay.Get().StartCoroutine(this.PlayBossLine(actor, "VO_ICC05_Lanathel_Female_Sanlayn_BiteReminder_01.prefab:e7562028fc82af24e8b5408d70ae79d4"));
    }
    else
    {
      Vector3 position1 = EndTurnButton.Get().transform.position;
      Vector3 position2 = new Vector3(position1.x - 3f, position1.y, position1.z);
      string key = "ICC_05_LANATHEL_01";
      this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
      NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
    }
    return false;
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
          m_soundName = "VO_ICC05_Lanathel_Female_Sanlayn_EmoteResponse_01.prefab:8617d4664523cb94aa4847447cbfbb8f",
          m_stringTag = "VO_ICC05_Lanathel_Female_Sanlayn_EmoteResponse_01"
        }
      }
    },
    new MissionEntity.EmoteResponseGroup()
    {
      m_triggers = new List<EmoteType>() { EmoteType.START },
      m_responses = new List<MissionEntity.EmoteResponse>()
      {
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_ICC05_Lanathel_Female_Sanlayn_Intro_01.prefab:a5927ce80194b9d4abee763cc3451c13",
          m_stringTag = "VO_ICC05_Lanathel_Female_Sanlayn_Intro_01"
        }
      }
    }
  };

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    ICC_05_Lanathel icc05Lanathel = this;
    while (icc05Lanathel.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    switch (missionEvent)
    {
      case 101:
        icc05Lanathel.m_playedLines.Add(str);
        yield return (object) icc05Lanathel.PlayBossLine(actor, "VO_ICC05_Lanathel_Female_Sanlayn_BossBite_01.prefab:e847622ab6b89e14b908a5092c681bd6");
        break;
      case 104:
        yield return (object) icc05Lanathel.PlayEasterEggLine(actor, "VO_ICC05_Lanathel_Female_Sanlayn_PlayerBiteAcolyte_01.prefab:88ff928135afd344ebabeefc3747231e");
        break;
      case 105:
        yield return (object) icc05Lanathel.PlayLineOnlyOnce(actor, "VO_ICC05_Lanathel_Female_Sanlayn_Wounded_01.prefab:b0dcbf6d181d5944e9e55b2f5a4b4bb1");
        break;
      case 106:
        if (icc05Lanathel.m_BloodEssenceLines.Count == 0)
          break;
        GameState.Get().SetBusy(true);
        string bloodEssenceLine = icc05Lanathel.m_BloodEssenceLines[Random.Range(0, icc05Lanathel.m_BloodEssenceLines.Count)];
        icc05Lanathel.m_BloodEssenceLines.Remove(bloodEssenceLine);
        yield return (object) icc05Lanathel.PlayLineOnlyOnce(actor, bloodEssenceLine);
        GameState.Get().SetBusy(false);
        break;
      case 107:
        yield return (object) icc05Lanathel.PlayBossLine(actor, "VO_ICC_841_Female_Sanlayn_Death_02.prefab:bcbec586c14d0ea47b294118f9fed020");
        break;
      case 108:
        icc05Lanathel.m_playedLines.Add(str);
        yield return (object) icc05Lanathel.PlayLineOnlyOnce(actor, "VO_ICC05_Lanathel_Female_Sanlayn_BitePoisonous_01.prefab:e72be5987d5bb3144b46077aad7e23e7");
        break;
      case 109:
        icc05Lanathel.m_playedLines.Add(str);
        yield return (object) icc05Lanathel.PlayLineOnlyOnce(actor, "VO_ICC05_Lanathel_Female_Sanlayn_BiteOoze_01.prefab:b3ff24b5195c4764fb18ef87f48f1fbd");
        break;
      case 110:
        icc05Lanathel.m_playedLines.Add(str);
        yield return (object) icc05Lanathel.PlayLineOnlyOnce(actor, "VO_ICC05_Lanathel_Female_Sanlayn_BiteSpikes_01.prefab:4b0a39c8dab009e4cb479e4e8bcd14ea");
        break;
      case 111:
        icc05Lanathel.m_playedLines.Add(str);
        yield return (object) icc05Lanathel.PlayLineOnlyOnce(actor, "VO_ICC05_Lanathel_Female_Sanlayn_BiteShell_01.prefab:229be1532f6fe31418f81b56784b4cd4");
        break;
      case 114:
        icc05Lanathel.m_playedLines.Add(str);
        yield return (object) icc05Lanathel.PlayEasterEggLine(actor, "VO_ICC05_Lanathel_Female_Sanlayn_PlayerBloodBite_01.prefab:c66830daa3ed9f045b6c3cbc347514c5");
        break;
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    ICC_05_Lanathel icc05Lanathel = this;
    while (icc05Lanathel.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (turn == 3)
    {
      GameState.Get().SetBusy(true);
      yield return (object) icc05Lanathel.PlayLineOnlyOnce(enemyActor, "VO_ICC05_Lanathel_Female_Sanlayn_Turn3_01.prefab:57e21d323fff0954dbcc930fbebb3f03");
      yield return (object) icc05Lanathel.PlayLineOnlyOnce(enemyActor, "VO_ICC05_Lanathel_Female_Sanlayn_Turn3_02.prefab:c6d817960f09ec94d842f61fbdd3c26a");
      GameState.Get().SetBusy(false);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ICC_05_Lanathel icc05Lanathel = this;
    while (icc05Lanathel.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!icc05Lanathel.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardID = entity.GetCardId();
      icc05Lanathel.m_playedLines.Add(cardID);
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) icc05Lanathel.WaitForEntitySoundToFinish(entity);
      string str = cardID;
      if (!(str == "ICC_314"))
      {
        if (!(str == "ICC_851"))
        {
          if (!(str == "ICC_852"))
          {
            if (str == "ICC_853")
            {
              yield return (object) icc05Lanathel.PlayEasterEggLine(enemyActor, "VO_ICC05_Lanathel_Female_Sanlayn_BloodPrince_01.prefab:c3b8a4ae2da8a4448a74c93f258f85c4");
              yield return (object) icc05Lanathel.PlayEasterEggLine(icc05Lanathel.GetActorByCardId("ICC_853"), "VO_PrinceValanar_Male_Vampire_ResponseLanaThel_01.prefab:3c482cfa6dfd9aa4182cfe35b66b4bcc");
            }
          }
          else
          {
            yield return (object) icc05Lanathel.PlayEasterEggLine(enemyActor, "VO_ICC05_Lanathel_Female_Sanlayn_BloodPrince_01.prefab:c3b8a4ae2da8a4448a74c93f258f85c4");
            yield return (object) icc05Lanathel.PlayEasterEggLine(icc05Lanathel.GetActorByCardId("ICC_852"), "VO_PrinceTaldaram_Male_Vampire_ResponseLanaThel_01.prefab:ab050ef2ad0f9a4498de5779b3e3d677");
          }
        }
        else
        {
          yield return (object) icc05Lanathel.PlayEasterEggLine(enemyActor, "VO_ICC05_Lanathel_Female_Sanlayn_BloodPrince_01.prefab:c3b8a4ae2da8a4448a74c93f258f85c4");
          yield return (object) icc05Lanathel.PlayEasterEggLine(icc05Lanathel.GetActorByCardId("ICC_851"), "VO_PrinceKeleseth_Male_Vampire_ResponseLanaThel_01.prefab:fddf65c2cd5115745845aa45b32017dc");
        }
      }
      else
      {
        yield return (object) icc05Lanathel.PlayEasterEggLine(enemyActor, "VO_ICC05_Lanathel_Female_Sanlayn_LichKing_01.prefab:2067c9859da805b4faea5fc664f0ada9");
        yield return (object) icc05Lanathel.PlayEasterEggLine(icc05Lanathel.GetLichKingFriendlyMinion(), "VO_ICC05_LichKing_Male_Human_LichKing_02.prefab:a56f85a63706c304786eed0e3c861377");
      }
      yield return (object) icc05Lanathel.IfPlayerPlaysDKHeroVO(entity, enemyActor, "VO_ICC05_Lanathel_Female_Sanlayn_TransformDK_01.prefab:534ffc634d9b4a74fa1a8067ebb0aa85");
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    ICC_05_Lanathel icc05Lanathel = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) icc05Lanathel.PlayClosingLine("LichKing_Banner_Quote.prefab:d42a8f4f69919f449b3dd8ebceaf2a3c", "VO_ICC05_LichKing_Male_Human_Win_01.prefab:453fe532c40bbfa44a1aac8d99041f7e");
    }
    if (gameResult == TAG_PLAYSTATE.LOST)
    {
      yield return (object) new WaitForSeconds(5f);
      string soundPath = "VO_ICC05_LichKing_Male_Human_Lose_01.prefab:9978afae17851584795146a392d5cb67";
      if (!NotificationManager.Get().HasSoundPlayedThisSession(soundPath))
        yield return (object) Gameplay.Get().StartCoroutine(icc05Lanathel.PlayCharacterQuoteAndWait("LichKing_Banner_Quote.prefab:d42a8f4f69919f449b3dd8ebceaf2a3c", soundPath));
    }
  }
}
