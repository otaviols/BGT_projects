using System.Collections;
using UnityEngine;

public class NAX15_KelThuzad : NAX_MissionEntity
{
  private bool m_frostHeroPowerLinePlayed;
  private bool m_bigglesLinePlayed;
  private bool m_hurryLinePlayed;
  private int m_numTimesMindControlPlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX15_01_SUMMON_ADDS_12.prefab:94bc769aa09d4234fb3ec6e6012b2304");
    this.PreloadSound("VO_NAX15_01_PHASE2_10.prefab:ad7357857f8cb904fad0280a4ed2d988");
    this.PreloadSound("VO_NAX15_01_HP_07.prefab:a56d0bb88cc42014fa9c5d53903faf15");
    this.PreloadSound("VO_NAX15_01_HP2_05.prefab:ee4e46ad9b4206146ab439ccfad4e59e");
    this.PreloadSound("VO_NAX15_01_HP3_06.prefab:41d3bd9b7963d5f41a0b3614df6074aa");
    this.PreloadSound("VO_NAX15_01_PHASE2_ALT_11.prefab:2f066d9f9a49df94cafa065e79d7ebdf");
    this.PreloadSound("VO_NAX15_01_EMOTE_HELLO_26.prefab:9ed2ae3873b199146819291cfaa396e5");
    this.PreloadSound("VO_NAX15_01_EMOTE_WP_25.prefab:57f0f617dc85a1441a6fe68fe570347c");
    this.PreloadSound("VO_NAX15_01_EMOTE_OOPS_29.prefab:0d497df4f2aced741bbba13ac2912d58");
    this.PreloadSound("VO_NAX15_01_EMOTE_SORRY_28.prefab:c7086f87dd8a03e489d1f19339942794");
    this.PreloadSound("VO_NAX15_01_EMOTE_THANKS_27.prefab:72955d7b668a26d4581ac52bf0ed03d0");
    this.PreloadSound("VO_NAX15_01_EMOTE_THREATEN_30.prefab:983b1fb96a8525041945d5b41475599f");
    this.PreloadSound("VO_KT_HEIGAN2_55.prefab:f465a1b0b2312764f92f4d86160c9dac");
    this.PreloadSound("VO_NAX15_01_RESPOND_GARROSH_15.prefab:48cc88124901a3447b86a466a761f3a9");
    this.PreloadSound("VO_NAX15_01_RESPOND_THRALL_17.prefab:ccc75bb0ed1ff104bbd9a85820ff5afe");
    this.PreloadSound("VO_NAX15_01_RESPOND_VALEERA_18.prefab:c9de6754f5d117a4d8fbdb6c7b7871e9");
    this.PreloadSound("VO_NAX15_01_RESPOND_UTHER_14.prefab:1079fbad87857364a95f558df2e47102");
    this.PreloadSound("VO_NAX15_01_RESPOND_REXXAR_19.prefab:5b09a5dd8bedd5d4b854e38878b48e80");
    this.PreloadSound("VO_NAX15_01_RESPOND_MALFURION_ALT_21.prefab:609de8d4162f0894da2c05b14473b6e7");
    this.PreloadSound("VO_NAX15_01_RESPOND_GULDAN_22.prefab:b28117d69d646014bb3a8ec39d5cc388");
    this.PreloadSound("VO_NAX15_01_RESPOND_JAINA_23.prefab:0434b865495ab2f45a36cef7be6b4ebc");
    this.PreloadSound("VO_NAX15_01_RESPOND_ANDUIN_24.prefab:10a05fc478fe371419a859b464b13b3e");
    this.PreloadSound("VO_NAX15_01_BIGGLES_32.prefab:1c0b11f45e9af1547ac5db34be687f9e");
    this.PreloadSound("VO_NAX15_01_HURRY_31.prefab:552de9d45281f0a47a4a9cb9645c98f6");
  }

  public override void OnPlayThinkEmote()
  {
    if (this.m_hurryLinePlayed || this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
    this.m_hurryLinePlayed = true;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_HURRY_31.prefab:552de9d45281f0a47a4a9cb9645c98f6", Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.LOST)
    {
      int KTgloat = Options.Get().GetInt(Option.KELTHUZADTAUNTS);
      yield return (object) new WaitForSeconds(5f);
      switch (KTgloat)
      {
        case 0:
          NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT1_33", "VO_NAX15_01_GLOAT1_33.prefab:6afb33fab639f1f43a7f33f17ef4d7d4");
          break;
        case 1:
          NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT2_34", "VO_NAX15_01_GLOAT2_34.prefab:ee23015fccf6cce44a21420f7ca0c8e6");
          break;
        case 2:
          NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT3_35", "VO_NAX15_01_GLOAT3_35.prefab:c7a207b5224015747a321ac520a02b9c");
          break;
        case 3:
          NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT4_36", "VO_NAX15_01_GLOAT4_36.prefab:8c432d06dd4a9254a9b415621fe22539");
          break;
        case 4:
          NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT5_37", "VO_NAX15_01_GLOAT5_37.prefab:e6821e5c9b4225441912e23add8b17f4");
          break;
      }
      if (KTgloat >= 4)
        Options.Get().SetInt(Option.KELTHUZADTAUNTS, 0);
      else
        Options.Get().SetInt(Option.KELTHUZADTAUNTS, KTgloat + 1);
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    NAX15_KelThuzad naX15KelThuzad = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) Gameplay.Get().StartCoroutine(naX15KelThuzad.\u003C\u003En__0(missionEvent));
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 1:
        GameState.Get().SetBusy(true);
        while (naX15KelThuzad.m_enemySpeaking)
          yield return (object) null;
        GameState.Get().SetBusy(false);
        yield return (object) Gameplay.Get().StartCoroutine(naX15KelThuzad.PlaySoundAndBlockSpeech("VO_NAX15_01_SUMMON_ADDS_12.prefab:94bc769aa09d4234fb3ec6e6012b2304", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 2:
        naX15KelThuzad.m_enemySpeaking = true;
        GameState.Get().SetBusy(true);
        yield return (object) Gameplay.Get().StartCoroutine(naX15KelThuzad.PlaySoundAndBlockSpeech("VO_NAX15_01_PHASE2_10.prefab:ad7357857f8cb904fad0280a4ed2d988", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        GameState.Get().SetBusy(false);
        yield return (object) Gameplay.Get().StartCoroutine(naX15KelThuzad.PlaySoundAndBlockSpeech("VO_NAX15_01_SUMMON_ADDS_12.prefab:94bc769aa09d4234fb3ec6e6012b2304", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 3:
        if (naX15KelThuzad.m_frostHeroPowerLinePlayed)
          break;
        naX15KelThuzad.m_frostHeroPowerLinePlayed = true;
        yield return (object) Gameplay.Get().StartCoroutine(naX15KelThuzad.PlaySoundAndBlockSpeech("VO_NAX15_01_HP_07.prefab:a56d0bb88cc42014fa9c5d53903faf15", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
      case 4:
        if (naX15KelThuzad.m_numTimesMindControlPlayed == 0)
          yield return (object) Gameplay.Get().StartCoroutine(naX15KelThuzad.PlaySoundAndBlockSpeech("VO_NAX15_01_HP2_05.prefab:ee4e46ad9b4206146ab439ccfad4e59e", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        else if (naX15KelThuzad.m_numTimesMindControlPlayed == 1)
          yield return (object) Gameplay.Get().StartCoroutine(naX15KelThuzad.PlaySoundAndBlockSpeech("VO_NAX15_01_HP3_06.prefab:41d3bd9b7963d5f41a0b3614df6074aa", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        ++naX15KelThuzad.m_numTimesMindControlPlayed;
        break;
      case 5:
        if (naX15KelThuzad.m_bigglesLinePlayed)
          break;
        naX15KelThuzad.m_bigglesLinePlayed = true;
        Gameplay.Get().StartCoroutine(naX15KelThuzad.PlaySoundAndBlockSpeech("VO_NAX15_01_BIGGLES_32.prefab:1c0b11f45e9af1547ac5db34be687f9e", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        break;
    }
  }

  public override void HandleRealTimeMissionEvent(int missionEvent)
  {
    if (missionEvent != 1)
      return;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "KelThuzad_StealTurn.prefab:7630c436593404790a4a948dc219f537", new PrefabCallback<GameObject>(this.OnStealTurnSpellLoaded));
  }

  private void OnStealTurnSpellLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
    {
      if ((Object) TurnTimer.Get() != (Object) null)
        TurnTimer.Get().OnEndTurnRequested();
      EndTurnButton.Get().OnEndTurnRequested();
    }
    else
    {
      go.transform.position = EndTurnButton.Get().transform.position;
      Spell component = go.GetComponent<Spell>();
      if ((Object) component == (Object) null)
      {
        if ((Object) TurnTimer.Get() != (Object) null)
          TurnTimer.Get().OnEndTurnRequested();
        EndTurnButton.Get().OnEndTurnRequested();
      }
      else
      {
        Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
        component.ActivateState(SpellStateType.ACTION);
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_PHASE2_ALT_11.prefab:2f066d9f9a49df94cafa065e79d7ebdf", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (emoteType)
    {
      case EmoteType.GREETINGS:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_HELLO_26.prefab:9ed2ae3873b199146819291cfaa396e5", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.WELL_PLAYED:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_WP_25.prefab:57f0f617dc85a1441a6fe68fe570347c", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.OOPS:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_OOPS_29.prefab:0d497df4f2aced741bbba13ac2912d58", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.THREATEN:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_THREATEN_30.prefab:983b1fb96a8525041945d5b41475599f", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.THANKS:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_THANKS_27.prefab:72955d7b668a26d4581ac52bf0ed03d0", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.SORRY:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_SORRY_28.prefab:c7086f87dd8a03e489d1f19339942794", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.START:
        string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(cardId))
        {
          case 1352851680:
            if (!(cardId == "HERO_07c"))
              return;
            goto label_53;
          case 1638365393:
            if (!(cardId == "HERO_05b"))
              return;
            goto label_51;
          case 1757780202:
            if (!(cardId == "HERO_09c"))
              return;
            goto label_55;
          case 1772983703:
            if (!(cardId == "HERO_03b"))
              return;
            goto label_49;
          case 1889588393:
            if (!(cardId == "HERO_06c"))
              return;
            goto label_52;
          case 1973770678:
            if (!(cardId == "HERO_04d"))
              return;
            goto label_50;
          case 2041719797:
            if (!(cardId == "HERO_01b"))
              return;
            break;
          case 2111415856:
            if (!(cardId == "HERO_06"))
              return;
            goto label_52;
          case 2128193475:
            if (!(cardId == "HERO_07"))
              return;
            goto label_53;
          case 2144971094:
            if (!(cardId == "HERO_04"))
              return;
            goto label_50;
          case 2160295963:
            if (!(cardId == "HERO_08c"))
              return;
            goto label_54;
          case 2161748713:
            if (!(cardId == "HERO_05"))
              return;
            goto label_51;
          case 2175396296:
            if (!(cardId == "HERO_02d"))
              return;
            goto label_48;
          case 2178526332:
            if (!(cardId == "HERO_02"))
              return;
            goto label_48;
          case 2195303951:
            if (!(cardId == "HERO_03"))
              return;
            goto label_49;
          case 2228859189:
            if (!(cardId == "HERO_01"))
              return;
            break;
          case 2346302522:
            if (!(cardId == "HERO_08"))
              return;
            goto label_54;
          case 2363080141:
            if (!(cardId == "HERO_09"))
              return;
            goto label_55;
          default:
            return;
        }
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_GARROSH_15.prefab:48cc88124901a3447b86a466a761f3a9", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
label_48:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_THRALL_17.prefab:ccc75bb0ed1ff104bbd9a85820ff5afe", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
label_49:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_VALEERA_18.prefab:c9de6754f5d117a4d8fbdb6c7b7871e9", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
label_50:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_UTHER_14.prefab:1079fbad87857364a95f558df2e47102", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
label_51:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_REXXAR_19.prefab:5b09a5dd8bedd5d4b854e38878b48e80", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
label_52:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_MALFURION_ALT_21.prefab:609de8d4162f0894da2c05b14473b6e7", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
label_53:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_GULDAN_22.prefab:b28117d69d646014bb3a8ec39d5cc388", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
label_54:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_JAINA_23.prefab:0434b865495ab2f45a36cef7be6b4ebc", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
label_55:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_ANDUIN_24.prefab:10a05fc478fe371419a859b464b13b3e", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case EmoteType.WOW:
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_KT_HEIGAN2_55.prefab:f465a1b0b2312764f92f4d86160c9dac", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
    }
  }
}
