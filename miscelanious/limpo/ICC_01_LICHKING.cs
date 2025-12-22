using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICC_01_LICHKING : ICC_MissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = ICC_01_LICHKING.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = ICC_01_LICHKING.InitStringOptions();
  private static readonly string TEXT_TIRION_TURN_1 = "ICC_01_TIRIONTURNS_01";
  private static readonly string TEXT_TIRION_TURN_2 = "ICC_01_TIRIONTURNS_02";
  private static readonly string TEXT_TIRION_TURN_3 = "ICC_01_TIRIONTURNS_03";
  private static readonly float TIRION_POPUP_DISPLAY_TIME = 2.5f;
  private Notification TirionTurnPopup;
  private Vector3 popUpPos = new Vector3(0.0f, 0.0f, 4f);
  private float popUpScale = 1f;
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>();

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>()
  {
    {
      GameEntityOption.VICTORY_SCREEN_PREFAB_PATH,
      "VictoryTwoScoop_ICCPrologue.prefab:a9e377ed0578dc14aa0029dc4af183cb"
    },
    {
      GameEntityOption.VICTORY_AUDIO_PATH,
      (string) null
    }
  };

  public ICC_01_LICHKING() => this.m_gameOptions.AddOptions(ICC_01_LICHKING.s_booleanOptions, ICC_01_LICHKING.s_stringOptions);

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ICCLichKing);

  public override string GetNameBannerSubtextOverride(Player.Side playerSide)
  {
    if (playerSide != Player.Side.OPPOSING)
      return base.GetNameBannerSubtextOverride(playerSide);
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    return opposingSidePlayer.GetHero() == opposingSidePlayer.GetStartingHero() ? GameStrings.Get("ICC_01_LICH_KING_SUBTEXT") : GameStrings.Get("ICC_01_TIRION_SUBTEXT");
  }

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_ICC01_Jaina_Female_Human_PostDeath_02.prefab:32db8cb82e111fd4c8bb56f5db507858");
    this.PreloadSound("VO_ICC01_Jaina_Female_Human_JainaDKIntro_05.prefab:e2b9b4bad3d006d45a1ed347b0cf6662");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_EndOfTurn2_01.prefab:bf54a25aa869a9d4cafd7aca65c262c0");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_EndOfTurn2_02.prefab:d5fe6eeb5fb997848acead28c516fa06");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_PlaysFrostmourne_01.prefab:7a4d0659ac92e394c8f25378fa9283c5");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_JainaDKIntro_01.prefab:31e70b7a1cd5d61498f391d40b4f7d43");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_JainaDKIntro_02.prefab:4e7678cce8c778941bdec0e493bc9129");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_JainaDKIntro_03.prefab:fb89ff5c830365347ae6494f74ad45a6");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Idle_01.prefab:4b9c6ba7bb584d04db2142a51282dd19");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Intro_01.prefab:7febb86d30cc95342823c0d6e9881573");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Turn1_01.prefab:e56daff85a5f2b840bdd5b24e8ea4dbf");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Turn11_01.prefab:663695085edbf034cba88620470622a7");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Turn13_01.prefab:d0d218202afbefd44ae379a95fe32383");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Turn15_01.prefab:49adda6df4448ac4cbb3bf3c7a3788bd");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Turn2_02.prefab:d7d93710894c71e46baa401198b75f8b");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Turn4_01.prefab:f9efdb3e91190e24fa143ab0f169e06a");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Turn4_02.prefab:e518e35bf465166488721f9ea11fdfc3");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_Turn5_02.prefab:2b02682c870a30d479674a47f24c112f");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_EndOfTurn4_01.prefab:36458bc5f111ecf47b5c23d6cb88eb5c");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_EndOfTurn4_03.prefab:6f98e2b658702824d926f0d448bfe537");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_JainaDKintro_05.prefab:81291d917a3f01c44a87c67f82fd2f6c");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_TerribleTank_01.prefab:9ea7d69e8eaf4ca47a17e8e7fd55af70");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_AFKay_01.prefab:1f645b92bb8cdac46b1580bf24bb895b");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_PauseDeath_02.prefab:8fcc6731e21286e4f8c2147d85bce59f");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_PostDeath_01.prefab:baaf7891e78de8343ae7fca845615ed0");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_Turn2_01.prefab:139c12e7eb223cf468f2663f68ae48da");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_Turn3_01.prefab:3c5498e7af98dbb4b8bb48a28fc8d2df");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_Turn6_01.prefab:cc757390f0689184f8fdfcb0f8b51c41");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_Turn6_02.prefab:600bfa0e8bb7f6b4f8467e0ebc5aeb74");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_Turn8_01.prefab:bd2fe6fcbfe02b340a11339ebe9f1bd6");
    this.PreloadSound("VO_ICC01_Jaina_Female_Human_DrawsRager_01.prefab:e8af0892cde840d41bf498a45573303f");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_JainaDKintro_06.prefab:4a0038f435eb069408b4231c55a712aa");
    this.PreloadSound("VO_ICC01_LichKing_Male_Human_EmoteResponse_01.prefab:e99b3bedcc63f8248bd0e47d26947a41");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_EmoteResponse_02.prefab:472ecefae2bb58d40b80e893ab4960af");
    this.PreloadSound("VO_ICC01_Tirion_Male_Human_Flavor01_01.prefab:fbf6ddd4207473d428897980504f0c54");
  }

  public IEnumerator ShowTurnCounter(string text)
  {
    Notification popup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.GetTextScale() * this.popUpScale, GameStrings.Get(text), false, NotificationManager.PopupTextType.FANCY);
    yield return (object) new WaitForSeconds(ICC_01_LICHKING.TIRION_POPUP_DISPLAY_TIME);
    NotificationManager.Get().DestroyNotification(popup, 0.0f);
  }

  public override void OnPlayThinkEmote()
  {
    if (this.m_playedLines.Contains("VO_ICC01_LichKing_Male_Human_Idle_01") || this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (GameState.Get().GetTurn())
    {
      case 1:
        Gameplay.Get().StartCoroutine(this.PlayBossLine(actor, "VO_ICC01_LichKing_Male_Human_Idle_01.prefab:4b9c6ba7bb584d04db2142a51282dd19"));
        this.m_playedLines.Add("VO_ICC01_LichKing_Male_Human_Idle_01");
        break;
      case 5:
        Gameplay.Get().StartCoroutine(this.PlayBossLine(actor, "VO_ICC01_LichKing_Male_Human_Idle_01.prefab:4b9c6ba7bb584d04db2142a51282dd19"));
        this.m_playedLines.Add("VO_ICC01_LichKing_Male_Human_Idle_01");
        break;
    }
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_ICC01_LichKing_Male_Human_Intro_01.prefab:7febb86d30cc95342823c0d6e9881573", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      if (!(cardId == "ICCA01_001"))
      {
        if (!(cardId == "ICCA01_013"))
          return;
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_ICC01_Tirion_Male_Human_EmoteResponse_02.prefab:472ecefae2bb58d40b80e893ab4960af", Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_ICC01_LichKing_Male_Human_EmoteResponse_01.prefab:e99b3bedcc63f8248bd0e47d26947a41", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    ICC_01_LICHKING icc01Lichking = this;
    while (icc01Lichking.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!icc01Lichking.m_playedLines.Contains(str))
    {
      switch (missionEvent)
      {
        case 101:
          GameState.Get().SetBusy(true);
          yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_LichKing_Male_Human_EndOfTurn2_01.prefab:bf54a25aa869a9d4cafd7aca65c262c0");
          yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_LichKing_Male_Human_EndOfTurn2_02.prefab:d5fe6eeb5fb997848acead28c516fa06");
          GameState.Get().SetBusy(false);
          break;
        case 102:
          GameState.Get().SetBusy(true);
          yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_LichKing_Male_Human_PlaysFrostmourne_01.prefab:7a4d0659ac92e394c8f25378fa9283c5");
          GameState.Get().SetBusy(false);
          break;
        case 103:
          GameState.Get().SetBusy(true);
          yield return (object) icc01Lichking.PlayBossLine("Tirion_BigQuote.prefab:878fcebc1cddaf24f828c44edb07f7f8", "VO_ICC01_Tirion_Male_Human_EndOfTurn4_01.prefab:36458bc5f111ecf47b5c23d6cb88eb5c");
          yield return (object) icc01Lichking.PlayBossLine("Tirion_BigQuote.prefab:878fcebc1cddaf24f828c44edb07f7f8", "VO_ICC01_Tirion_Male_Human_EndOfTurn4_03.prefab:6f98e2b658702824d926f0d448bfe537");
          GameState.Get().SetBusy(false);
          yield return (object) icc01Lichking.ShowTurnCounter(ICC_01_LICHKING.TEXT_TIRION_TURN_1);
          break;
        case 105:
          Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName("");
          Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).UpdateHeroNameBanner();
          GameState.Get().SetBusy(true);
          yield return (object) icc01Lichking.PlayBossLine(enemyActor, "VO_ICC01_Tirion_Male_Human_JainaDKintro_05.prefab:81291d917a3f01c44a87c67f82fd2f6c");
          GameState.Get().SetBusy(false);
          yield return (object) icc01Lichking.PlayBossLine("LichKing_BigQuote.prefab:6d0439b386dc3cc41a591f989cbb93ed", "VO_ICC01_LichKing_Male_Human_JainaDKintro_06.prefab:4a0038f435eb069408b4231c55a712aa");
          break;
        case 107:
          icc01Lichking.m_playedLines.Add(str);
          yield return (object) new WaitForSeconds(0.7f);
          GameState.Get().SetBusy(true);
          yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_Tirion_Male_Human_TerribleTank_01.prefab:9ea7d69e8eaf4ca47a17e8e7fd55af70");
          GameState.Get().SetBusy(false);
          break;
        case 108:
          icc01Lichking.m_playedLines.Add(str);
          yield return (object) new WaitForSeconds(5.7f);
          GameState.Get().SetBusy(true);
          yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_Tirion_Male_Human_AFKay_01.prefab:1f645b92bb8cdac46b1580bf24bb895b");
          GameState.Get().SetBusy(false);
          break;
        case 109:
          yield return (object) new WaitForSeconds(0.75f);
          yield return (object) icc01Lichking.PlayBossLine(enemyActor, "VO_ICC01_Tirion_Male_Human_PauseDeath_02.prefab:8fcc6731e21286e4f8c2147d85bce59f");
          break;
        case 111:
          GameState.Get().SetBusy(true);
          yield return (object) icc01Lichking.PlayBossLine(actor, "VO_ICC01_Jaina_Female_Human_PostDeath_02.prefab:32db8cb82e111fd4c8bb56f5db507858");
          GameState.Get().SetBusy(false);
          break;
        case 112:
          icc01Lichking.m_playedLines.Add(str);
          yield return (object) new WaitForSeconds(2.2f);
          GameState.Get().SetBusy(true);
          yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_Tirion_Male_Human_Flavor01_01.prefab:fbf6ddd4207473d428897980504f0c54");
          GameState.Get().SetBusy(false);
          break;
        case 114:
          yield return (object) icc01Lichking.PlayLineOnlyOnce("LichKing_BigQuote.prefab:6d0439b386dc3cc41a591f989cbb93ed", "VO_ICC01_LichKing_Male_Human_Turn11_01.prefab:663695085edbf034cba88620470622a7");
          break;
        case 115:
          yield return (object) icc01Lichking.PlayLineOnlyOnce("LichKing_BigQuote.prefab:6d0439b386dc3cc41a591f989cbb93ed", "VO_ICC01_LichKing_Male_Human_Turn13_01.prefab:d0d218202afbefd44ae379a95fe32383");
          break;
        case 116:
          yield return (object) icc01Lichking.PlayLineOnlyOnce("LichKing_BigQuote.prefab:6d0439b386dc3cc41a591f989cbb93ed", "VO_ICC01_LichKing_Male_Human_Turn15_01.prefab:49adda6df4448ac4cbb3bf3c7a3788bd");
          break;
      }
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    ICC_01_LICHKING icc01Lichking = this;
    while (icc01Lichking.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    switch (turn)
    {
      case 1:
        GameState.Get().SetBusy(true);
        yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_LichKing_Male_Human_Turn1_01.prefab:e56daff85a5f2b840bdd5b24e8ea4dbf");
        GameState.Get().SetBusy(false);
        break;
      case 2:
        GameState.Get().SetBusy(true);
        yield return (object) icc01Lichking.PlayBossLine("Tirion_BigQuote.prefab:878fcebc1cddaf24f828c44edb07f7f8", "VO_ICC01_Tirion_Male_Human_Turn2_01.prefab:139c12e7eb223cf468f2663f68ae48da");
        yield return (object) icc01Lichking.ShowTurnCounter(ICC_01_LICHKING.TEXT_TIRION_TURN_3);
        yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_LichKing_Male_Human_Turn2_02.prefab:d7d93710894c71e46baa401198b75f8b");
        GameState.Get().SetBusy(false);
        break;
      case 3:
        yield return (object) icc01Lichking.PlayLineOnlyOnce("Tirion_BigQuote.prefab:878fcebc1cddaf24f828c44edb07f7f8", "VO_ICC01_Tirion_Male_Human_Turn3_01.prefab:3c5498e7af98dbb4b8bb48a28fc8d2df");
        yield return (object) icc01Lichking.ShowTurnCounter(ICC_01_LICHKING.TEXT_TIRION_TURN_2);
        break;
      case 4:
        yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_LichKing_Male_Human_Turn4_01.prefab:f9efdb3e91190e24fa143ab0f169e06a");
        yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_LichKing_Male_Human_Turn4_02.prefab:e518e35bf465166488721f9ea11fdfc3");
        break;
      case 5:
        GameState.Get().SetBusy(true);
        yield return (object) icc01Lichking.PlayLineOnlyOnce(actor, "VO_ICC01_Jaina_Female_Human_DrawsRager_01.prefab:e8af0892cde840d41bf498a45573303f");
        yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_LichKing_Male_Human_Turn5_02.prefab:2b02682c870a30d479674a47f24c112f");
        GameState.Get().SetBusy(false);
        break;
      case 6:
        yield return (object) icc01Lichking.PlayLineOnlyOnce("Tirion_BigQuote.prefab:878fcebc1cddaf24f828c44edb07f7f8", "VO_ICC01_Tirion_Male_Human_Turn6_01.prefab:cc757390f0689184f8fdfcb0f8b51c41");
        yield return (object) icc01Lichking.PlayLineOnlyOnce("Tirion_BigQuote.prefab:878fcebc1cddaf24f828c44edb07f7f8", "VO_ICC01_Tirion_Male_Human_Turn6_02.prefab:600bfa0e8bb7f6b4f8467e0ebc5aeb74");
        break;
      case 7:
        yield return (object) icc01Lichking.PlayLineOnlyOnce(actor, "VO_ICC01_Jaina_Female_Human_JainaDKIntro_05.prefab:e2b9b4bad3d006d45a1ed347b0cf6662");
        break;
      case 8:
        GameState.Get().SetBusy(true);
        yield return (object) icc01Lichking.PlayLineOnlyOnce(enemyActor, "VO_ICC01_Tirion_Male_Human_Turn8_01.prefab:bd2fe6fcbfe02b340a11339ebe9f1bd6");
        GameState.Get().SetBusy(false);
        break;
    }
  }

  private Actor GetActorbyCardId(string cardId)
  {
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    foreach (Card card in opposingSidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == opposingSidePlayer.GetPlayerId() && entity.GetCardId() == cardId)
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
  }

  public override string GetDefeatScreenBannerText() => !GameState.Get().IsGameOver() ? GameStrings.Get("GAMEPLAY_END_OF_GAME_DEFEAT_MAYBE") : base.GetDefeatScreenBannerText();

  public IEnumerator PlayLichKingRezLines()
  {
    ICC_01_LICHKING icc01Lichking = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) icc01Lichking.PlayBossLine(enemyActor, "VO_ICC01_LichKing_Male_Human_JainaDKIntro_01.prefab:31e70b7a1cd5d61498f391d40b4f7d43");
    yield return (object) icc01Lichking.PlayBossLine(enemyActor, "VO_ICC01_LichKing_Male_Human_JainaDKIntro_02.prefab:4e7678cce8c778941bdec0e493bc9129");
    GameState.Get().SetBusy(false);
  }

  public IEnumerator PlayTirionVictoryScreenLine()
  {
    AudioSource preloadedSound = this.GetPreloadedSound("VO_ICC01_Tirion_Male_Human_PostDeath_01.prefab:baaf7891e78de8343ae7fca845615ed0");
    float seconds = 6.8f;
    if ((Object) preloadedSound != (Object) null && (Object) preloadedSound.clip != (Object) null)
      seconds = preloadedSound.clip.length;
    else
      Log.Gameplay.PrintError("ICC_01_Lichking.PlayTirionVictoryScreenLine() - failed to find Preloaded Sound \"VO_ICC01_Tirion_Male_Human_PostDeath_01\"");
    PlayMakerFSM fsm = NotificationManager.Get().CreateBigCharacterQuoteWithText("Tirion_BigQuote.prefab:878fcebc1cddaf24f828c44edb07f7f8", NotificationManager.DEFAULT_CHARACTER_POS, "VO_ICC01_Tirion_Male_Human_PostDeath_01.prefab:baaf7891e78de8343ae7fca845615ed0", GameStrings.Get("VO_ICC01_Tirion_Male_Human_PostDeath_01"), seconds + 1f, bubbleDir: Notification.SpeechBubbleDirection.BottomLeft).GetComponentInChildren<PlayMakerFSM>();
    if ((Object) fsm == (Object) null)
    {
      Log.Gameplay.PrintError("ICC_01_Lichking.PlayTirionVictoryScreenLine(): Tirion_BigQuote prefab does not have a PlayMakerFSM in its children!");
    }
    else
    {
      yield return (object) new WaitForSeconds(seconds);
      fsm.SendEvent("DoEffect");
      yield return (object) new WaitForSeconds(1f);
    }
  }

  public IEnumerator PlayJainaVictoryScreenLine(Actor jaina)
  {
    ICC_01_LICHKING icc01Lichking = this;
    GameState.Get().SetBusy(true);
    float bubbleScale = (bool) UniversalInputManager.UsePhoneUI ? 0.5f : 0.75f;
    yield return (object) icc01Lichking.PlaySoundAndBlockSpeech("VO_ICC01_Jaina_Female_Human_PostDeath_02.prefab:32db8cb82e111fd4c8bb56f5db507858", Notification.SpeechBubbleDirection.BottomLeft, jaina, 2f, parentBubbleToActor: false, bubbleScale: bubbleScale);
    GameState.Get().SetBusy(false);
  }
}
