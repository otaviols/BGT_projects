using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR12_Portals : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private Spell m_introSpellInstance;

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_KarazhanFreeMedivh);

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_Moroes_Male_Human_FinalThirdSequence_01.prefab:c1c97be0950451646bd4803829649485");
    this.PreloadSound("VO_Moroes_Male_Human_FinalAltTurn5_01.prefab:9c414f092ce0ec14d9daf94a7ad6ac1f");
    this.PreloadSound("VO_Moroes_Male_Human_FinalMaidenTurn7_03.prefab:05caa36abf0fa5a47ba8fcbb0f5f9b3d");
    this.PreloadSound("VO_Malchezaar_Male_Demon_FinalThirdSequence_01.prefab:f3d5c93fce92fc8489d31c2472c6215f");
    this.PreloadSound("VO_Malchezaar_Male_Demon_FinalThirdSequence_04.prefab:2fd08853587f35e47898939b971b282c");
    this.PreloadSound("VO_Malchezaar_Male_Demon_FinalMalchezaarTurn7_01.prefab:3ec08fc1a35e79c439b40fdcbcb0db1c");
    this.PreloadSound("VO_Malchezaar_Male_Demon_FInalMalchezaarSacrificialPact_03.prefab:67beaf7b540d3dd46a3c302730ece026");
    this.PreloadSound("VO_Malchezaar_Male_Demon_FinalMalchezaarMedivhSkin_01.prefab:d2ca787691dbb49468911ff1d1a5c1e6");
    this.PreloadSound("VO_Malchezaar_Male_Demon_FinalMalchezaarJaraxxus_01.prefab:c3955f874381c654a84c51134cb7d938");
    this.PreloadSound("VO_Malchezaar_Male_Demon_FinalMalchezaarHeroPower_01.prefab:35d5e0210c690e244830efff7b029b56");
    this.PreloadSound("VO_Malchezaar_Male_Demon_FinalMalchezaarEmoteResponse_01.prefab:80deff75e71de3941ac8b8d4167fb814");
    this.PreloadSound("VO_Malchezaar_Male_Demon_Brawl_06.prefab:7ebc074519c4bee4b9ee2dc3a022cb8a");
    this.PreloadSound("VO_Malchezaar_Male_Demon_FinalAltOpening_01.prefab:9902000abbbc66348b13baaefad5a6ef");
    this.PreloadSound("VO_Malchezaar_Male_Demon_EmoteParty_01.prefab:77b4252ce1451884cb2d1148bdc636a7");
    this.PreloadSound("VO_Medivh_Male_Human_FinalThirdSequence_01.prefab:f7616460d19fffe4682697c0dd03d2b6");
    this.PreloadSound("VO_Medivh_Male_Human_FinalThirdSequence_03.prefab:9a08107f3d877f44bb4ebef53db3c708");
    this.PreloadSound("VO_Medivh_Male_Human_FinalMedivhMedivhSkin_01.prefab:583768d83b20d12469684c50af5db9a0");
    this.PreloadSound("VO_Medivh_Male_Human_FinalMalchezaarTurn5_01.prefab:ce3f76baba16c4640877a9e5ae3e3221");
    this.PreloadSound("VO_Medivh_Male_Human_FinalMalchezaarWin_01.prefab:bf1f3b1d88b8dad42a03581c2e61a0e9");
    this.PreloadSound("VO_Jaraxxus_Male_Demon_FinalMalchezaarJaraxxus_01.prefab:9dc5c97f68e466a45a0e5cd3dafb6a1a");
    this.PreloadSound("VO_Jaraxxus_Male_Demon_FinalMalchezaarJaraxxus_02.prefab:9d151ec830f37f947b6188c3235ea5cc");
    this.PreloadSound("VO_Medivh_Male_Human_FinalThirdSequence_02.prefab:3501e9a3a477db7468459ea6b0c162f6");
    this.PreloadSound("VO_Moroes_Male_Human_FinalSecondSequence_01.prefab:7fafbacc56a622b4385a994f9b231240");
    this.PreloadSound("VO_Nazra_Female_Orc_FinalNazraEmoteResponse_01.prefab:6213bb0b535356b40b360716d732ed83");
    this.PreloadSound("VO_Nazra_Female_Orc_FinalNazraGrom_01.prefab:225efe7172be03148af639919e115b69");
    this.PreloadSound("VO_Nazra_Female_Orc_FinalNazraChogall_02.prefab:52d6f8fa86e142741ad1136a29722be7");
    this.PreloadSound("VO_Moroes_Male_Human_FinalMaidenTurn1_01.prefab:f172c91c0b7f52a47b8d95e4c89a64db");
    this.PreloadSound("VO_Moroes_Male_Human_FinalNazraTurn7_02.prefab:6a6ff647d93cf984a97c07d205013133");
    this.PreloadSound("VO_Nazra_Female_Orc_FinalNazraHeroPower_01.prefab:2e9958d6650ca08469a566b41f9a7df0");
    this.PreloadSound("VO_Moroes_Male_Human_FinalTurn1_02.prefab:edfda4d27ac82974db1cb83c4628a130");
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    if (!(cardId == "KARA_13_01") && !(cardId == "KARA_13_01H"))
    {
      if (!(cardId == "KARA_13_06") && !(cardId == "KARA_13_06H"))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_Malchezaar_Male_Demon_FinalAltOpening_01.prefab:9902000abbbc66348b13baaefad5a6ef", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_Nazra_Female_Orc_FinalNazraEmoteResponse_01.prefab:6213bb0b535356b40b360716d732ed83", Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    KAR12_Portals kaR12Portals = this;
    while (kaR12Portals.m_enemySpeaking)
      yield return (object) null;
    while (GameState.Get().IsBusy())
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!kaR12Portals.m_playedLines.Contains(str))
    {
      kaR12Portals.m_playedLines.Add(str);
      switch (missionEvent)
      {
        case 9:
          Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName("");
          bool playLongSpell = kaR12Portals.ShouldPlayLine("VO_Malchezaar_Male_Demon_FinalThirdSequence_01.prefab:f3d5c93fce92fc8489d31c2472c6215f", new MissionEntity.ShouldPlay(kaR12Portals.ShouldPlayLongCutscene));
          GameState.Get().SetBusy(true);
          yield return (object) new WaitForSeconds(2f);
          if (playLongSpell)
          {
            yield return (object) kaR12Portals.PlayCriticalLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_FinalSecondSequence_01.prefab:7fafbacc56a622b4385a994f9b231240");
            yield return (object) new WaitForSeconds(1f);
          }
          if (!playLongSpell)
            yield return (object) new WaitForSeconds(1.5f);
          Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).UpdateHeroNameBanner();
          yield return (object) kaR12Portals.PlayCriticalLine(enemyActor, "VO_Malchezaar_Male_Demon_FinalThirdSequence_01.prefab:f3d5c93fce92fc8489d31c2472c6215f");
          GameState.Get().SetBusy(false);
          break;
        case 12:
          GameState.Get().SetBusy(true);
          yield return (object) kaR12Portals.PlayMissionFlavorLine(enemyActor, "VO_Malchezaar_Male_Demon_FinalMalchezaarTurn7_01.prefab:3ec08fc1a35e79c439b40fdcbcb0db1c");
          GameState.Get().SetBusy(false);
          break;
        case 13:
          GameState.Get().SetBusy(true);
          yield return (object) kaR12Portals.PlayEasterEggLine(enemyActor, "VO_Malchezaar_Male_Demon_FInalMalchezaarSacrificialPact_03.prefab:67beaf7b540d3dd46a3c302730ece026");
          GameState.Get().SetBusy(false);
          break;
        case 14:
          if (!kaR12Portals.ShouldPlayCriticalLine("VO_Moroes_Male_Human_FinalSecondSequence_01.prefab:7fafbacc56a622b4385a994f9b231240"))
            break;
          GameState.Get().SetBusy(true);
          GameObject gameObject1 = GameObject.Find("Medivh_Hero");
          if ((Object) gameObject1 == (Object) null)
          {
            Log.All.PrintError("Could not find Medivh_Hero gameObject");
            GameState.Get().SetBusy(false);
            break;
          }
          Actor component = gameObject1.GetComponent<Actor>();
          if ((Object) component == (Object) null)
          {
            Log.All.PrintError("Could not find actor component for Medivh_Hero gameObject");
            GameState.Get().SetBusy(false);
            break;
          }
          component.SetEntity(GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetEntity());
          yield return (object) kaR12Portals.PlayCriticalLine(component, "VO_Medivh_Male_Human_FinalThirdSequence_03.prefab:9a08107f3d877f44bb4ebef53db3c708");
          GameState.Get().SetBusy(false);
          break;
        case 16:
          if (!kaR12Portals.ShouldPlayLine("VO_Medivh_Male_Human_FinalThirdSequence_01.prefab:f7616460d19fffe4682697c0dd03d2b6", new MissionEntity.ShouldPlay(kaR12Portals.ShouldPlayLongCutscene)))
            break;
          GameState.Get().SetBusy(true);
          GameObject gameObject2 = GameObject.Find("Medivh_Hero");
          if ((Object) gameObject2 == (Object) null)
          {
            Log.All.PrintError("Could not find Medivh_Hero gameObject");
            GameState.Get().SetBusy(false);
            break;
          }
          Actor medivhActor = gameObject2.GetComponent<Actor>();
          if ((Object) medivhActor == (Object) null)
          {
            Log.All.PrintError("Could not find actor component for Medivh_Hero gameObject");
            GameState.Get().SetBusy(false);
            break;
          }
          medivhActor.SetEntity(GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetEntity());
          yield return (object) new WaitForSeconds(1f);
          yield return (object) kaR12Portals.PlayCriticalLine(medivhActor, "VO_Medivh_Male_Human_FinalThirdSequence_01.prefab:f7616460d19fffe4682697c0dd03d2b6");
          GameState.Get().SetBusy(false);
          medivhActor = (Actor) null;
          break;
        case 17:
          GameState.Get().SetBusy(true);
          yield return (object) kaR12Portals.PlayEasterEggLine("Medivh_BigQuote.prefab:78e18a627031f6c48aef27a0fa1123c1", "VO_Medivh_Male_Human_FinalMedivhMedivhSkin_01.prefab:583768d83b20d12469684c50af5db9a0");
          GameState.Get().SetBusy(false);
          break;
      }
    }
  }

  public override void NotifyOfRealTimeTagChange(Entity entity, Network.HistTagChange tagChange)
  {
    if (tagChange.Tag != 6 || tagChange.Value != 9)
      return;
    if ((Object) TurnTimer.Get() != (Object) null)
      TurnTimer.Get().OnEndTurnRequested();
    EndTurnButton.Get().OnEndTurnRequested();
    GameState.Get().UpdateOptionHighlights();
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR12_Portals kaR12Portals = this;
    while (kaR12Portals.m_enemySpeaking)
      yield return (object) null;
    string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    if (cardId == "KARA_13_01" || cardId == "KARA_13_01H")
    {
      switch (turn)
      {
        case 1:
          yield return (object) kaR12Portals.PlayOpeningLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_FinalMaidenTurn1_01.prefab:f172c91c0b7f52a47b8d95e4c89a64db");
          break;
        case 7:
          yield return (object) kaR12Portals.PlayMissionFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_FinalNazraTurn7_02.prefab:6a6ff647d93cf984a97c07d205013133");
          break;
        case 12:
          GameState.Get().SetBusy(true);
          yield return (object) kaR12Portals.PlayMissionFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_FinalAltTurn5_01.prefab:9c414f092ce0ec14d9daf94a7ad6ac1f");
          yield return (object) kaR12Portals.PlayMissionFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_FinalMaidenTurn7_03.prefab:05caa36abf0fa5a47ba8fcbb0f5f9b3d");
          GameState.Get().SetBusy(false);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    KAR12_Portals kaR12Portals = this;
    while (kaR12Portals.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR12Portals.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Entity hero = GameState.Get().GetOpposingSidePlayer().GetHero();
      if (!(cardId == "KARA_13_11"))
      {
        if (!(cardId == "KARA_13_12"))
        {
          if (!(cardId == "KARA_00_02") && !(cardId == "KARA_13_13H"))
          {
            if (cardId == "EX1_312")
            {
              kaR12Portals.m_playedLines.Add(cardId);
              yield return (object) kaR12Portals.PlayBossLine(actor, "VO_Malchezaar_Male_Demon_FinalThirdSequence_04.prefab:2fd08853587f35e47898939b971b282c");
            }
          }
          else if (hero.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) > 2)
          {
            kaR12Portals.m_playedLines.Add(cardId);
            GameState.Get().SetBusy(true);
            yield return (object) kaR12Portals.PlayBossLine(actor, "VO_Malchezaar_Male_Demon_FinalMalchezaarHeroPower_01.prefab:35d5e0210c690e244830efff7b029b56");
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          kaR12Portals.m_playedLines.Add(cardId);
          yield return (object) kaR12Portals.PlayBossLine(actor, "VO_Malchezaar_Male_Demon_Brawl_06.prefab:7ebc074519c4bee4b9ee2dc3a022cb8a");
        }
      }
      else
      {
        kaR12Portals.m_playedLines.Add(cardId);
        yield return (object) kaR12Portals.PlayBossLine(actor, "VO_Malchezaar_Male_Demon_FinalMalchezaarEmoteResponse_01.prefab:80deff75e71de3941ac8b8d4167fb814");
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    KAR12_Portals kaR12Portals = this;
    while (kaR12Portals.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR12Portals.m_playedLines.Contains(entity.GetCardId()))
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      string cardId1 = entity.GetCardId();
      string cardId2 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      if (cardId2 == "KARA_13_01" || cardId2 == "KARA_13_01H")
      {
        if (!(cardId1 == "EX1_414"))
        {
          if (cardId1 == "OG_121")
          {
            kaR12Portals.m_playedLines.Add(cardId1);
            yield return (object) new WaitForSeconds(3.7f);
            yield return (object) kaR12Portals.PlayEasterEggLine(enemyActor, "VO_Nazra_Female_Orc_FinalNazraChogall_02.prefab:52d6f8fa86e142741ad1136a29722be7");
          }
        }
        else
        {
          kaR12Portals.m_playedLines.Add(cardId1);
          yield return (object) new WaitForSeconds(2.2f);
          yield return (object) kaR12Portals.PlayEasterEggLine(enemyActor, "VO_Nazra_Female_Orc_FinalNazraGrom_01.prefab:225efe7172be03148af639919e115b69");
        }
      }
      else if (cardId2 == "KARA_13_06" || cardId2 == "KARA_13_06H")
      {
        if (!(cardId1 == "EX1_323"))
        {
          if (cardId1 == "CS2_034_H1")
          {
            kaR12Portals.m_playedLines.Add(cardId1);
            yield return (object) kaR12Portals.PlayEasterEggLine(enemyActor, "VO_Malchezaar_Male_Demon_FinalMalchezaarMedivhSkin_01.prefab:d2ca787691dbb49468911ff1d1a5c1e6");
          }
        }
        else
        {
          kaR12Portals.m_playedLines.Add(cardId1);
          if (kaR12Portals.ShouldPlayEasterEggLine("VO_Malchezaar_Male_Demon_FinalMalchezaarJaraxxus_01.prefab:c3955f874381c654a84c51134cb7d938"))
          {
            yield return (object) new WaitForSeconds(5f);
            yield return (object) kaR12Portals.PlayEasterEggLine(enemyActor, "VO_Malchezaar_Male_Demon_FinalMalchezaarJaraxxus_01.prefab:c3955f874381c654a84c51134cb7d938");
            yield return (object) kaR12Portals.PlayEasterEggLine(friendlyActor, "VO_Jaraxxus_Male_Demon_FinalMalchezaarJaraxxus_01.prefab:9dc5c97f68e466a45a0e5cd3dafb6a1a");
            yield return (object) kaR12Portals.PlayEasterEggLine(friendlyActor, "VO_Jaraxxus_Male_Demon_FinalMalchezaarJaraxxus_02.prefab:9d151ec830f37f947b6188c3235ea5cc");
          }
        }
      }
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR12_Portals kaR12Portals = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR12Portals.PlayClosingLine("Medivh_Quote.prefab:423c4a6b7e7a7f643bf0b2992ad3d31b", "VO_Medivh_Male_Human_FinalMalchezaarWin_01.prefab:bf1f3b1d88b8dad42a03581c2e61a0e9");
    }
  }

  private void OnIntroSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if ((Object) this.m_introSpellInstance != (Object) spell || this.m_introSpellInstance.GetActiveState() != SpellStateType.NONE)
      return;
    Object.Destroy((Object) this.m_introSpellInstance.gameObject, 5f);
    this.m_introSpellInstance = (Spell) null;
  }

  protected MissionEntity.ShouldPlayValue ShouldPlayLongCutscene() => MissionEntity.ShouldPlayValue.Once;

  public bool ShouldPlayLongMidmissionCutscene() => this.ShouldPlayLine("VO_Malchezaar_Male_Demon_FinalThirdSequence_01.prefab:f3d5c93fce92fc8489d31c2472c6215f", new MissionEntity.ShouldPlay(this.ShouldPlayLongCutscene));

  public override IEnumerator DoCustomIntro(
    Card friendlyHero,
    Card enemyHero,
    HeroLabel friendlyHeroLabel,
    HeroLabel enemyHeroLabel,
    GameStartVsLetters versusText)
  {
    KAR12_Portals kaR12Portals = this;
    if (kaR12Portals.ShouldPlayLine("VO_Moroes_Male_Human_FinalThirdSequence_01.prefab:c1c97be0950451646bd4803829649485", new MissionEntity.ShouldPlay(kaR12Portals.ShouldPlayLongCutscene)))
    {
      friendlyHeroLabel.SetFade(0.0f);
      enemyHeroLabel.SetFade(0.0f);
      versusText.gameObject.SetActive(false);
      friendlyHero.GetActor().Hide();
      enemyHero.GetActor().Hide();
      GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard().GetActor().Hide();
      yield return (object) new WaitForSeconds(1.5f);
      if (!string.IsNullOrEmpty("Nazra_PreMissionSummon.prefab:22f4f2bf8acd31541b4ce82bab9a1907"))
      {
        kaR12Portals.m_introSpellInstance = SpellManager.Get().GetSpell("Nazra_PreMissionSummon.prefab:22f4f2bf8acd31541b4ce82bab9a1907");
        kaR12Portals.m_introSpellInstance.AddStateFinishedCallback(new Spell.StateFinishedCallback(kaR12Portals.OnIntroSpellStateFinished));
        kaR12Portals.m_introSpellInstance.SetSource(friendlyHero.gameObject);
        kaR12Portals.m_introSpellInstance.AddTarget(enemyHero.gameObject);
        kaR12Portals.m_introSpellInstance.ActivateState(SpellStateType.ACTION);
        yield return (object) Gameplay.Get().StartCoroutine(kaR12Portals.PlayBigCharacterQuoteAndWait("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_FinalThirdSequence_01.prefab:c1c97be0950451646bd4803829649485"));
        Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
        yield return (object) new WaitForSeconds(4f);
        Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.TopRight;
        if ((bool) UniversalInputManager.UsePhoneUI)
          direction = Notification.SpeechBubbleDirection.TopLeft;
        yield return (object) Gameplay.Get().StartCoroutine(kaR12Portals.PlaySoundAndBlockSpeech("VO_Nazra_Female_Orc_FinalNazraHeroPower_01.prefab:2e9958d6650ca08469a566b41f9a7df0", direction, enemyActor, parentBubbleToActor: false, bubbleScale: 0.6f));
        if ((bool) UniversalInputManager.UsePhoneUI)
          yield return (object) Gameplay.Get().StartCoroutine(kaR12Portals.PlayCharacterQuoteAndWait("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_FinalTurn1_02.prefab:edfda4d27ac82974db1cb83c4628a130", "VO_Moroes_Male_Human_FinalTurn1_02", new Vector3(-4f, 0.0f, 0.0f), isBig: true));
        else
          yield return (object) Gameplay.Get().StartCoroutine(kaR12Portals.PlayBigCharacterQuoteAndWait("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_FinalTurn1_02.prefab:edfda4d27ac82974db1cb83c4628a130"));
        while ((Object) kaR12Portals.m_introSpellInstance != (Object) null && !kaR12Portals.m_introSpellInstance.IsFinished())
          yield return (object) null;
        enemyActor = (Actor) null;
      }
      friendlyHeroLabel.FadeIn();
      enemyHeroLabel.FadeIn();
      versusText.gameObject.SetActive(true);
      versusText.FadeIn();
      Gameplay.Get().StartCoroutine(kaR12Portals.FlipInHeroPower());
      yield return (object) new WaitForSeconds(1f);
    }
  }

  public IEnumerator FlipInHeroPower()
  {
    yield return (object) new WaitForSeconds(6f);
    GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard().GetActor().Show();
    GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard().ActivateActorSpell(SpellType.SUMMON_IN);
  }

  public override void OnCustomIntroCancelled(
    Card friendlyHero,
    Card enemyHero,
    HeroLabel friendlyHeroLabel,
    HeroLabel enemyHeroLabel,
    GameStartVsLetters versusText)
  {
    if (!((Object) this.m_introSpellInstance != (Object) null))
      return;
    this.m_introSpellInstance.ActivateState(SpellStateType.CANCEL);
  }
}
