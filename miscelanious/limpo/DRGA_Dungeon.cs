using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DRGA_Dungeon : DRGA_MissionEntity
{
  public List<string> m_BossVOLines = new List<string>();
  public List<string> m_PlayerVOLines = new List<string>();
  public bool m_Heroic;
  public bool m_Galakrond;
  public static readonly AssetReference BrannBrassRing = new AssetReference("BrannBronzebeard_BrassRing_Quote.prefab:d1f8af47f0917e94289b63f3a42e52f7");
  public static readonly AssetReference EliseBrassRing = new AssetReference("EliseStarseeker_BrassRing_Quote.prefab:7176acaa6d28fa447adbafde663037d3");
  public static readonly AssetReference FinleyBrassRing = new AssetReference("SirFinley_BrassRing_Quote.prefab:5f94953d717142446b348e4d2f3a4ca8");
  public static readonly AssetReference RenoBrassRing = new AssetReference("RenoJackson_BrassRing_Quote.prefab:74a27d2f94ef83744a0a8357dbac2e43");
  public static readonly AssetReference RafaamBrassRing = new AssetReference("Rafaam_BrassRing_Quote.prefab:2d6ab3cc1d153ed4886ff98e47d129c6");
  public string m_introLine;
  public string m_deathLine;
  public string m_standardEmoteResponseLine;
  public List<string> m_BossIdleLines;
  public List<string> m_BossIdleLinesCopy;
  private int m_PlayPlayerVOLineIndex;
  private int m_PlayBossVOLineIndex;
  public int TurnOfPlotTwistLastPlayed;

  public static DRGA_Dungeon InstantiateDRGADungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    string opposingHeroCardId = GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame);
    switch (GameMgr.Get().GetMissionId())
    {
      case 0:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_01();
      case 3469:
      case 3556:
        return (DRGA_Dungeon) new DRGA_Good_Fight_01();
      case 3470:
      case 3583:
        return (DRGA_Dungeon) new DRGA_Good_Fight_02();
      case 3471:
      case 3584:
        return (DRGA_Dungeon) new DRGA_Good_Fight_03();
      case 3472:
      case 3585:
        return (DRGA_Dungeon) new DRGA_Good_Fight_04();
      case 3473:
      case 3586:
        return (DRGA_Dungeon) new DRGA_Good_Fight_05();
      case 3475:
      case 3587:
        return (DRGA_Dungeon) new DRGA_Good_Fight_06();
      case 3477:
      case 3588:
        return (DRGA_Dungeon) new DRGA_Good_Fight_07();
      case 3478:
      case 3589:
        return (DRGA_Dungeon) new DRGA_Good_Fight_08();
      case 3479:
      case 3590:
        return (DRGA_Dungeon) new DRGA_Good_Fight_09();
      case 3480:
      case 3591:
        return (DRGA_Dungeon) new DRGA_Good_Fight_10();
      case 3481:
      case 3592:
        return (DRGA_Dungeon) new DRGA_Good_Fight_11();
      case 3483:
      case 3593:
        return (DRGA_Dungeon) new DRGA_Good_Fight_12();
      case 3484:
      case 3594:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_01();
      case 3488:
      case 3595:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_02();
      case 3489:
      case 3596:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_03();
      case 3490:
      case 3597:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_04();
      case 3491:
      case 3598:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_05();
      case 3493:
      case 3599:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_06();
      case 3494:
      case 3600:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_07();
      case 3495:
      case 3601:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_08();
      case 3497:
      case 3602:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_09();
      case 3498:
      case 3603:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_10();
      case 3499:
      case 3604:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_11();
      case 3500:
      case 3605:
        return (DRGA_Dungeon) new DRGA_Evil_Fight_12();
      default:
        Log.All.PrintError("DRGA_Dungeon.InstantiateDRGADungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) opposingHeroCardId);
        return new DRGA_Dungeon();
    }
  }

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> collection = new List<string>();
    this.m_PlayerVOLines = new List<string>((IEnumerable<string>) collection);
    foreach (string soundPath in collection)
      this.PreloadSound(soundPath);
  }

  public virtual List<string> GetIdleLines() => new List<string>();

  public void SetBossVOLines(List<string> VOLines) => this.m_BossVOLines = new List<string>((IEnumerable<string>) VOLines);

  public virtual List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected virtual float ChanceToPlayBossHeroPowerVOLine() => 1f;

  protected virtual void OnBossHeroPowerPlayed(Entity entity)
  {
    float bossHeroPowerVoLine = this.ChanceToPlayBossHeroPowerVOLine();
    float num = Random.Range(0.0f, 1f);
    if (this.m_enemySpeaking || (double) bossHeroPowerVoLine < (double) num)
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if ((Object) actor == (Object) null)
      return;
    List<string> powerRandomLines = this.GetBossHeroPowerRandomLines();
    string soundPath = "";
    while (powerRandomLines.Count > 0)
    {
      int index = Random.Range(0, powerRandomLines.Count);
      soundPath = powerRandomLines[index];
      powerRandomLines.RemoveAt(index);
      if (!NotificationManager.Get().HasSoundPlayedThisSession(soundPath))
        break;
    }
    if (soundPath == "")
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeechOnce(soundPath, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    DRGA_Dungeon drgaDungeon = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (drgaDungeon.m_enemySpeaking || entity.GetCardType() == TAG_CARDTYPE.INVALID || entity.GetCardType() != TAG_CARDTYPE.HERO_POWER || entity.GetControllerSide() != Player.Side.OPPOSING)
      return false;
    drgaDungeon.OnBossHeroPowerPlayed(entity);
    return false;
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DRGA_Dungeon drgaDungeon = this;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (drgaDungeon.m_enemySpeaking)
      yield return (object) null;
    yield return (object) drgaDungeon.WaitForEntitySoundToFinish(entity);
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (this.m_enemySpeaking || string.IsNullOrEmpty(this.m_deathLine) || gameResult != TAG_PLAYSTATE.WON)
      return;
    if (this.GetShouldSuppressDeathTextBubble())
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_deathLine, Notification.SpeechBubbleDirection.None, actor));
    else
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_deathLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) null;
    this.m_deathLine = (string) null;
    this.m_standardEmoteResponseLine = (string) null;
    this.m_BossIdleLines = new List<string>((IEnumerable<string>) this.GetIdleLines());
    this.m_BossIdleLinesCopy = new List<string>((IEnumerable<string>) this.GetIdleLines());
    this.m_Heroic = this.GetIsHeroic();
  }

  protected virtual bool GetIsHeroic()
  {
    int missionId = GameMgr.Get().GetMissionId();
    return missionId == 3556 || (uint) (missionId - 3583) <= 22U;
  }

  protected virtual bool GetShouldSuppressDeathTextBubble() => false;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DRGA_Dungeon drgaDungeon = this;
    while (drgaDungeon.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 1000:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        if (drgaDungeon.m_PlayPlayerVOLineIndex + 1 >= drgaDungeon.m_PlayerVOLines.Count)
          drgaDungeon.m_PlayPlayerVOLineIndex = 0;
        else
          ++drgaDungeon.m_PlayPlayerVOLineIndex;
        SceneDebugger.Get().AddMessage(drgaDungeon.m_PlayerVOLines[drgaDungeon.m_PlayPlayerVOLineIndex]);
        yield return (object) drgaDungeon.PlayBossLine(actor1, drgaDungeon.m_PlayerVOLines[drgaDungeon.m_PlayPlayerVOLineIndex]);
        break;
      case 1001:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        SceneDebugger.Get().AddMessage(drgaDungeon.m_PlayerVOLines[drgaDungeon.m_PlayPlayerVOLineIndex]);
        yield return (object) drgaDungeon.PlayBossLine(actor1, drgaDungeon.m_PlayerVOLines[drgaDungeon.m_PlayPlayerVOLineIndex]);
        break;
      case 1002:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        if (drgaDungeon.m_PlayBossVOLineIndex + 1 >= drgaDungeon.m_BossVOLines.Count)
          drgaDungeon.m_PlayBossVOLineIndex = 0;
        else
          ++drgaDungeon.m_PlayBossVOLineIndex;
        SceneDebugger.Get().AddMessage(drgaDungeon.m_BossVOLines[drgaDungeon.m_PlayBossVOLineIndex]);
        yield return (object) drgaDungeon.PlayBossLine(actor2, drgaDungeon.m_BossVOLines[drgaDungeon.m_PlayBossVOLineIndex]);
        break;
      case 1003:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        SceneDebugger.Get().AddMessage(drgaDungeon.m_BossVOLines[drgaDungeon.m_PlayBossVOLineIndex]);
        yield return (object) drgaDungeon.PlayBossLine(actor2, drgaDungeon.m_BossVOLines[drgaDungeon.m_PlayBossVOLineIndex]);
        break;
      case 1010:
        if (drgaDungeon.m_forceAlwaysPlayLine)
        {
          drgaDungeon.m_forceAlwaysPlayLine = false;
          break;
        }
        drgaDungeon.m_forceAlwaysPlayLine = true;
        break;
      case 58023:
        SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
        GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
        SceneMgr.Get().SetNextMode(postGameSceneMode);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) drgaDungeon.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override string GetNameBannerSubtextOverride(Player.Side playerSide) => base.GetNameBannerSubtextOverride(playerSide);

  public virtual float GetThinkEmoteBossThinkChancePercentage() => 0.25f;

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
    if ((double) this.GetThinkEmoteBossThinkChancePercentage() > (double) Random.Range(0.0f, 1f) && this.m_BossIdleLines != null && this.m_BossIdleLines.Count != 0)
    {
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      string line = this.PopRandomLine(this.m_BossIdleLinesCopy);
      if (this.m_BossIdleLinesCopy.Count == 0)
        this.m_BossIdleLinesCopy = new List<string>((IEnumerable<string>) this.GetIdleLines());
      Gameplay.Get().StartCoroutine(this.PlayBossLine(actor, line));
    }
    else
    {
      EmoteType emoteType = EmoteType.THINK1;
      switch (Random.Range(1, 4))
      {
        case 1:
          emoteType = EmoteType.THINK1;
          break;
        case 2:
          emoteType = EmoteType.THINK2;
          break;
        case 3:
          emoteType = EmoteType.THINK3;
          break;
      }
      GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType);
    }
  }

  public IEnumerator PlayAndRemoveRandomLineOnlyOnceWithBrassRing(
    Actor actor,
    AssetReference brassRingBackup,
    List<string> lines)
  {
    if ((Object) actor != (Object) null)
      yield return (object) this.PlayAndRemoveRandomLineOnlyOnce(actor, lines);
    else if (brassRingBackup != null)
      yield return (object) this.PlayAndRemoveRandomLineOnlyOnce((string) brassRingBackup, lines);
  }

  protected IEnumerator PlayLineAlwaysWithBrassRing(
    Actor actor,
    AssetReference brassRingBackup,
    string line,
    float duration = 2.5f)
  {
    DRGA_Dungeon drgaDungeon = this;
    if ((Object) actor != (Object) null)
      yield return (object) drgaDungeon.PlayLineAlways(actor, line);
    else if (brassRingBackup != null)
      yield return (object) drgaDungeon.PlayLineAlways((string) brassRingBackup, line);
  }

  public IEnumerator PlayLineInOrderOnceWithBrassRing(
    Actor actor,
    AssetReference brassRingBackup,
    List<string> lines)
  {
    DRGA_Dungeon drgaDungeon = this;
    if ((Object) actor != (Object) null)
      yield return (object) drgaDungeon.PlayLineInOrderOnce(actor, lines);
    else if (brassRingBackup != null)
      yield return (object) drgaDungeon.PlayLineInOrderOnce((string) brassRingBackup, lines);
  }

  public IEnumerator PlayAndRemoveRandomLineOnlyOnce(Actor actor, List<string> lines)
  {
    DRGA_Dungeon drgaDungeon = this;
    string line = drgaDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) drgaDungeon.PlayLineOnlyOnce(actor, line);
  }

  public IEnumerator PlayAndRemoveRandomLineOnlyOnce(string actor, List<string> lines)
  {
    DRGA_Dungeon drgaDungeon = this;
    string line = drgaDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) drgaDungeon.PlayLineOnlyOnce(actor, line);
  }

  public IEnumerator PlayRandomLineAlways(Actor actor, List<string> lines)
  {
    DRGA_Dungeon drgaDungeon = this;
    string line = drgaDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) drgaDungeon.PlayBossLine(actor, line);
  }

  public IEnumerator PlayRandomLineAlways(string actor, List<string> lines)
  {
    DRGA_Dungeon drgaDungeon = this;
    string line = drgaDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) drgaDungeon.PlayBossLine(actor, line);
  }

  protected Actor GetEnemyActorByCardId(string cardId)
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

  protected Actor GetFriendlyActorByCardId(string cardId)
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    foreach (Card card in friendlySidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == friendlySidePlayer.GetPlayerId() && entity.GetCardId() == cardId)
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
  }

  public override void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      return;
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DRGMulligan);
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DRG);
}
