using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Garrosh_Dungeon : BoH_Garrosh_MissionEntity
{
  public List<string> m_BossVOLines = new List<string>();
  public List<string> m_PlayerVOLines = new List<string>();
  public bool m_Heroic;
  public bool m_Galakrond;
  public string m_introLine;
  public string m_deathLine;
  public string m_standardEmoteResponseLine;
  public List<string> m_BossIdleLines;
  public List<string> m_BossIdleLinesCopy;
  private int m_PlayPlayerVOLineIndex;
  private int m_PlayBossVOLineIndex;
  public int TurnOfPlotTwistLastPlayed;

  public static BoH_Garrosh_Dungeon InstantiateBoH_Garrosh_DungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    string opposingHeroCardId = GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame);
    GameMgr.Get().GetMissionId();
    Log.All.PrintError("BoH_Garrosh_Dungeon.InstantiateBoH_Garrosh_DungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) opposingHeroCardId);
    return new BoH_Garrosh_Dungeon();
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
    if (this.m_enemySpeaking || this.m_MissionDisableAutomaticVO || (double) bossHeroPowerVoLine < (double) num)
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
    BoH_Garrosh_Dungeon boHGarroshDungeon = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (boHGarroshDungeon.m_MissionDisableAutomaticVO || boHGarroshDungeon.m_enemySpeaking || entity.GetCardType() == TAG_CARDTYPE.INVALID || entity.GetCardType() != TAG_CARDTYPE.HERO_POWER || entity.GetControllerSide() != Player.Side.OPPOSING)
      return false;
    boHGarroshDungeon.OnBossHeroPowerPlayed(entity);
    return false;
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_Dungeon boHGarroshDungeon = this;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (boHGarroshDungeon.m_enemySpeaking)
      yield return (object) null;
    yield return (object) boHGarroshDungeon.WaitForEntitySoundToFinish(entity);
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
      if (this.m_MissionDisableAutomaticVO || !MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
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
  }

  protected virtual bool GetShouldSuppressDeathTextBubble() => false;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Garrosh_Dungeon boHGarroshDungeon = this;
    while (boHGarroshDungeon.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 1000:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        if (boHGarroshDungeon.m_PlayPlayerVOLineIndex + 1 >= boHGarroshDungeon.m_PlayerVOLines.Count)
          boHGarroshDungeon.m_PlayPlayerVOLineIndex = 0;
        else
          ++boHGarroshDungeon.m_PlayPlayerVOLineIndex;
        SceneDebugger.Get().AddMessage(boHGarroshDungeon.m_PlayerVOLines[boHGarroshDungeon.m_PlayPlayerVOLineIndex]);
        yield return (object) boHGarroshDungeon.PlayBossLine(actor1, boHGarroshDungeon.m_PlayerVOLines[boHGarroshDungeon.m_PlayPlayerVOLineIndex]);
        break;
      case 1001:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        SceneDebugger.Get().AddMessage(boHGarroshDungeon.m_PlayerVOLines[boHGarroshDungeon.m_PlayPlayerVOLineIndex]);
        yield return (object) boHGarroshDungeon.PlayBossLine(actor1, boHGarroshDungeon.m_PlayerVOLines[boHGarroshDungeon.m_PlayPlayerVOLineIndex]);
        break;
      case 1002:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        if (boHGarroshDungeon.m_PlayBossVOLineIndex + 1 >= boHGarroshDungeon.m_BossVOLines.Count)
          boHGarroshDungeon.m_PlayBossVOLineIndex = 0;
        else
          ++boHGarroshDungeon.m_PlayBossVOLineIndex;
        SceneDebugger.Get().AddMessage(boHGarroshDungeon.m_BossVOLines[boHGarroshDungeon.m_PlayBossVOLineIndex]);
        yield return (object) boHGarroshDungeon.PlayBossLine(actor2, boHGarroshDungeon.m_BossVOLines[boHGarroshDungeon.m_PlayBossVOLineIndex]);
        break;
      case 1003:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        SceneDebugger.Get().AddMessage(boHGarroshDungeon.m_BossVOLines[boHGarroshDungeon.m_PlayBossVOLineIndex]);
        yield return (object) boHGarroshDungeon.PlayBossLine(actor2, boHGarroshDungeon.m_BossVOLines[boHGarroshDungeon.m_PlayBossVOLineIndex]);
        break;
      case 1010:
        if (boHGarroshDungeon.m_forceAlwaysPlayLine)
        {
          boHGarroshDungeon.m_forceAlwaysPlayLine = false;
          break;
        }
        boHGarroshDungeon.m_forceAlwaysPlayLine = true;
        break;
      case 1100:
        GameState.Get().SetBusy(true);
        boHGarroshDungeon.m_MissionDisableAutomaticVO = true;
        GameState.Get().SetBusy(false);
        break;
      case 1101:
        GameState.Get().SetBusy(true);
        boHGarroshDungeon.m_MissionDisableAutomaticVO = false;
        GameState.Get().SetBusy(false);
        break;
      case 58023:
        SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
        GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
        SceneMgr.Get().SetNextMode(postGameSceneMode);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHGarroshDungeon.\u003C\u003En__0(missionEvent);
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
    if (!currentPlayer.IsFriendlySide() || this.m_MissionDisableAutomaticVO || currentPlayer.GetHeroCard().HasActiveEmoteSound())
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
    BoH_Garrosh_Dungeon boHGarroshDungeon = this;
    if ((Object) actor != (Object) null)
      yield return (object) boHGarroshDungeon.PlayLineAlways(actor, line);
    else if (brassRingBackup != null)
      yield return (object) boHGarroshDungeon.PlayLineAlways((string) brassRingBackup, line);
  }

  public IEnumerator PlayLineInOrderOnceWithBrassRing(
    Actor actor,
    AssetReference brassRingBackup,
    List<string> lines)
  {
    BoH_Garrosh_Dungeon boHGarroshDungeon = this;
    if ((Object) actor != (Object) null)
      yield return (object) boHGarroshDungeon.PlayLineInOrderOnce(actor, lines);
    else if (brassRingBackup != null)
      yield return (object) boHGarroshDungeon.PlayLineInOrderOnce((string) brassRingBackup, lines);
  }

  public IEnumerator PlayAndRemoveRandomLineOnlyOnce(Actor actor, List<string> lines)
  {
    BoH_Garrosh_Dungeon boHGarroshDungeon = this;
    string line = boHGarroshDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) boHGarroshDungeon.PlayLineOnlyOnce(actor, line);
  }

  public IEnumerator PlayAndRemoveRandomLineOnlyOnce(string actor, List<string> lines)
  {
    BoH_Garrosh_Dungeon boHGarroshDungeon = this;
    string line = boHGarroshDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) boHGarroshDungeon.PlayLineOnlyOnce(actor, line);
  }

  public IEnumerator PlayRandomLineAlways(Actor actor, List<string> lines)
  {
    BoH_Garrosh_Dungeon boHGarroshDungeon = this;
    string line = boHGarroshDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) boHGarroshDungeon.PlayBossLine(actor, line);
  }

  public IEnumerator PlayRandomLineAlways(string actor, List<string> lines)
  {
    BoH_Garrosh_Dungeon boHGarroshDungeon = this;
    string line = boHGarroshDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) boHGarroshDungeon.PlayBossLine(actor, line);
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
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DHMulligan);
  }
}
