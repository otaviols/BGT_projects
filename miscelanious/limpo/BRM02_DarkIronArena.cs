using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM02_DarkIronArena : BRM_MissionEntity
{
  private HashSet<string> m_linesPlayed = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA02_1_RESPONSE_04.prefab:f5c965be447046543ad83c07cf2bcd0c");
    this.PreloadSound("VO_BRMA02_1_HERO_POWER_05.prefab:c00acb0a43035e24aa86b12856b7af44");
    this.PreloadSound("VO_BRMA02_1_TURN1_02.prefab:ef65f9070bf2aa6489a58c705d21b1d9");
    this.PreloadSound("VO_BRMA02_1_TURN1_PT2_03.prefab:b500c91480d36e045aece029d3646acc");
    this.PreloadSound("VO_BRMA02_1_ALAKIR_34.prefab:da389ff1794394e46bae7929a2db7452");
    this.PreloadSound("VO_BRMA02_1_ALEXSTRAZA_32.prefab:912653e742620bb4185dc691ba8380d7");
    this.PreloadSound("VO_BRMA02_1_BEAST_22.prefab:15d2fbd54c2845741af4946fd489ee0a");
    this.PreloadSound("VO_BRMA02_1_BOOM_28.prefab:b6075c33b877a14409eafb2f91ca9a6e");
    this.PreloadSound("VO_BRMA02_1_CAIRNE_20.prefab:b43fe34d1ec6a424abe41b02dc0e4196");
    this.PreloadSound("VO_BRMA02_1_CHO_07.prefab:ac3d8b3ee38366140b30299bee846f39");
    this.PreloadSound("VO_BRMA02_1_DEATHWING_35.prefab:c5f1030779827c643bfb810f919819fa");
    this.PreloadSound("VO_BRMA02_1_ETC_18.prefab:1533c18bd7e1b204ebed7b6fac30fa9a");
    this.PreloadSound("VO_BRMA02_1_FEUGEN_15.prefab:04b5933629beae54da3357341132b94a");
    this.PreloadSound("VO_BRMA02_1_FOEREAPER_29.prefab:356b9c85ddd8ad340a71d4c9c7c9d58c");
    this.PreloadSound("VO_BRMA02_1_GEDDON_13.prefab:c4a73265eab005b47bb7d51c74d7b41d");
    this.PreloadSound("VO_BRMA02_1_GELBIN_21.prefab:c9710a6ef19cdba46a7c51404bd73851");
    this.PreloadSound("VO_BRMA02_1_GRUUL_31.prefab:4fdd03655b03494488e8453de69a3c34");
    this.PreloadSound("VO_BRMA02_1_HOGGER_27.prefab:ec226470424912547a6971e727116826");
    this.PreloadSound("VO_BRMA02_1_LEVIATHAN_12.prefab:998e9da05f77b3945ac21f8a7c245738");
    this.PreloadSound("VO_BRMA02_1_LOATHEB_16.prefab:cb44878eabffc304db3080dd9571018b");
    this.PreloadSound("VO_BRMA02_1_MAEXXNA_24.prefab:4d37c3fd5de32014bb70b071c47e6491");
    this.PreloadSound("VO_BRMA02_1_MILLHOUSE_09.prefab:5d1b5f555fe70e344afce98d443d9730");
    this.PreloadSound("VO_BRMA02_1_MOGOR_25.prefab:13e09894e4d3edc4a95ccf97a4865fa3");
    this.PreloadSound("VO_BRMA02_1_MUKLA_10.prefab:1bcf6267b4e82b841a48d2a094f99618");
    this.PreloadSound("VO_BRMA02_1_NOZDORMU_36.prefab:890229ac317e19d45bd1fe7fb67af713");
    this.PreloadSound("VO_BRMA02_1_ONYXIA_33.prefab:a963754f9a1ffbe489bddca145214f19");
    this.PreloadSound("VO_BRMA02_1_PAGLE_08.prefab:4681ed30e0c08544c8f67ee5ee64e05c");
    this.PreloadSound("VO_BRMA02_1_SNEED_30.prefab:4e86ebca4343b2e49841d0afa38719cf");
    this.PreloadSound("VO_BRMA02_1_STALAGG_14.prefab:f86dfd95c5c043f40becb7def0ec2b5b");
    this.PreloadSound("VO_BRMA02_1_SYLVANAS_19.prefab:c81b1205ba2c977489aaa357f60879ba");
    this.PreloadSound("VO_BRMA02_1_THALNOS_06.prefab:dfdd1db2a3b8e31439ffb9609a5a686d");
    this.PreloadSound("VO_BRMA02_1_THAURISSAN_37.prefab:dc3a267f6d3fafd4196cbd9dd7c145e2");
    this.PreloadSound("VO_BRMA02_1_TINKMASTER_11.prefab:398799c5e8b1ef649af23857a270ac05");
    this.PreloadSound("VO_BRMA02_1_TOSHLEY_26.prefab:07c3b6a0fd3b6174dadba657f6248d8f");
    this.PreloadSound("VO_BRMA02_1_VOLJIN_17.prefab:319230fe533b551469286ecb9fe97dde");
    this.PreloadSound("VO_NEFARIAN_GRIMSTONE_DEAD1_30.prefab:cb4444f1726517043ad2997a282b7863");
    this.PreloadSound("VO_RAGNAROS_GRIMSTONE_DEAD2_66.prefab:5494ef65549f1b449b6132b3ef620f0f");
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
          m_soundName = "VO_BRMA02_1_RESPONSE_04.prefab:f5c965be447046543ad83c07cf2bcd0c",
          m_stringTag = "VO_BRMA02_1_RESPONSE_04"
        }
      }
    }
  };

  protected override IEnumerator RespondToWillPlayCardWithTiming(
    string cardId,
    Entity playedEntity)
  {
    BRM02_DarkIronArena m02DarkIronArena = this;
    if (!m02DarkIronArena.m_linesPlayed.Contains(cardId))
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "BRMA02_2" || cardId == "BRMA02_2H")
      {
        if (!m02DarkIronArena.m_enemySpeaking)
        {
          GameState.Get().SetBusy(true);
          m02DarkIronArena.m_linesPlayed.Add(cardId);
          yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_HERO_POWER_05.prefab:c00acb0a43035e24aa86b12856b7af44", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        while (m02DarkIronArena.m_enemySpeaking)
          yield return (object) null;
        switch (cardId)
        {
          case "BRM_028":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_THAURISSAN_37.prefab:dc3a267f6d3fafd4196cbd9dd7c145e2", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_012":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_THALNOS_06.prefab:dfdd1db2a3b8e31439ffb9609a5a686d", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_014":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_MUKLA_10.prefab:1bcf6267b4e82b841a48d2a094f99618", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_016":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_SYLVANAS_19.prefab:c81b1205ba2c977489aaa357f60879ba", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_083":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_TINKMASTER_11.prefab:398799c5e8b1ef649af23857a270ac05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_100":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_CHO_07.prefab:ac3d8b3ee38366140b30299bee846f39", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_110":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_CAIRNE_20.prefab:b43fe34d1ec6a424abe41b02dc0e4196", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_112":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_GELBIN_21.prefab:c9710a6ef19cdba46a7c51404bd73851", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_249":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_GEDDON_13.prefab:c4a73265eab005b47bb7d51c74d7b41d", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_557":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_PAGLE_08.prefab:4681ed30e0c08544c8f67ee5ee64e05c", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_560":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_NOZDORMU_36.prefab:890229ac317e19d45bd1fe7fb67af713", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_561":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_ALEXSTRAZA_32.prefab:912653e742620bb4185dc691ba8380d7", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_562":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_ONYXIA_33.prefab:a963754f9a1ffbe489bddca145214f19", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "EX1_577":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_BEAST_22.prefab:15d2fbd54c2845741af4946fd489ee0a", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "FP1_010":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_MAEXXNA_24.prefab:4d37c3fd5de32014bb70b071c47e6491", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "FP1_014":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_STALAGG_14.prefab:f86dfd95c5c043f40becb7def0ec2b5b", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "FP1_015":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_FEUGEN_15.prefab:04b5933629beae54da3357341132b94a", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "FP1_030":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_LOATHEB_16.prefab:cb44878eabffc304db3080dd9571018b", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "GVG_007":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_LEVIATHAN_12.prefab:998e9da05f77b3945ac21f8a7c245738", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "GVG_014":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_VOLJIN_17.prefab:319230fe533b551469286ecb9fe97dde", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "GVG_110":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_BOOM_28.prefab:b6075c33b877a14409eafb2f91ca9a6e", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "GVG_112":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_MOGOR_25.prefab:13e09894e4d3edc4a95ccf97a4865fa3", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "GVG_113":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_FOEREAPER_29.prefab:356b9c85ddd8ad340a71d4c9c7c9d58c", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "GVG_114":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_SNEED_30.prefab:4e86ebca4343b2e49841d0afa38719cf", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "GVG_115":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_TOSHLEY_26.prefab:07c3b6a0fd3b6174dadba657f6248d8f", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "NEW1_010":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_ALAKIR_34.prefab:da389ff1794394e46bae7929a2db7452", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "NEW1_029":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_MILLHOUSE_09.prefab:5d1b5f555fe70e344afce98d443d9730", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "NEW1_030":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_DEATHWING_35.prefab:c5f1030779827c643bfb810f919819fa", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "NEW1_038":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_GRUUL_31.prefab:4fdd03655b03494488e8453de69a3c34", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "NEW1_040":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_HOGGER_27.prefab:ec226470424912547a6971e727116826", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
          case "PRO_001":
            m02DarkIronArena.m_linesPlayed.Add(cardId);
            yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_ETC_18.prefab:1533c18bd7e1b204ebed7b6fac30fa9a", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, delayCardSoundSpells: true));
            break;
        }
        GameState.Get().SetBusy(false);
      }
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BRM02_DarkIronArena m02DarkIronArena = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (turn == 1)
    {
      yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_TURN1_02.prefab:ef65f9070bf2aa6489a58c705d21b1d9", Notification.SpeechBubbleDirection.TopRight, enemyActor));
      Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndBlockSpeech("VO_BRMA02_1_TURN1_PT2_03.prefab:b500c91480d36e045aece029d3646acc", Notification.SpeechBubbleDirection.TopRight, enemyActor));
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    BRM02_DarkIronArena m02DarkIronArena = this;
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", GameStrings.Get("VO_NEFARIAN_GRIMSTONE_DEAD1_30"), "", durationSeconds: 5f);
      yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndWait("VO_NEFARIAN_GRIMSTONE_DEAD1_30.prefab:cb4444f1726517043ad2997a282b7863", "", Notification.SpeechBubbleDirection.None, (Actor) null));
      NotificationManager.Get().DestroyActiveQuote(0.0f);
      NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote.prefab:c9e0154894cd1a946b90ebefeb481a51", NotificationManager.ALT_ADVENTURE_SCREEN_POS, GameStrings.Get("VO_RAGNAROS_GRIMSTONE_DEAD2_66"), "", durationSeconds: 7f);
      yield return (object) Gameplay.Get().StartCoroutine(m02DarkIronArena.PlaySoundAndWait("VO_RAGNAROS_GRIMSTONE_DEAD2_66.prefab:5494ef65549f1b449b6132b3ef620f0f", "", Notification.SpeechBubbleDirection.None, (Actor) null));
      NotificationManager.Get().DestroyActiveQuote(0.0f);
    }
  }
}
