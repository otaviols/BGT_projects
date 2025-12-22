using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TB_RumbleDome_Dungeon : BTA_Prologue_MissionEntity
{
  public List<string> m_BossVOLines = new List<string>();
  public List<string> m_PlayerVOLines = new List<string>();
  public string m_introLine;
  public string m_deathLine;
  public string m_standardEmoteResponseLine;
  public List<string> m_BossIdleLines;
  public List<string> m_BossIdleLinesCopy;
  private int m_PlayPlayerVOLineIndex;
  private int m_PlayBossVOLineIndex;

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
    TB_RumbleDome_Dungeon rumbleDomeDungeon = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (rumbleDomeDungeon.m_enemySpeaking || entity.GetCardType() == TAG_CARDTYPE.INVALID || entity.GetCardType() != TAG_CARDTYPE.HERO_POWER || entity.GetControllerSide() != Player.Side.OPPOSING)
      return false;
    rumbleDomeDungeon.OnBossHeroPowerPlayed(entity);
    return false;
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    TB_RumbleDome_Dungeon rumbleDomeDungeon = this;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (rumbleDomeDungeon.m_enemySpeaking)
      yield return (object) null;
    yield return (object) rumbleDomeDungeon.WaitForEntitySoundToFinish(entity);
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
    TB_RumbleDome_Dungeon rumbleDomeDungeon = this;
    while (rumbleDomeDungeon.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 1000:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        if (rumbleDomeDungeon.m_PlayPlayerVOLineIndex + 1 >= rumbleDomeDungeon.m_PlayerVOLines.Count)
          rumbleDomeDungeon.m_PlayPlayerVOLineIndex = 0;
        else
          ++rumbleDomeDungeon.m_PlayPlayerVOLineIndex;
        SceneDebugger.Get().AddMessage(rumbleDomeDungeon.m_PlayerVOLines[rumbleDomeDungeon.m_PlayPlayerVOLineIndex]);
        yield return (object) rumbleDomeDungeon.PlayBossLine(actor1, rumbleDomeDungeon.m_PlayerVOLines[rumbleDomeDungeon.m_PlayPlayerVOLineIndex]);
        break;
      case 1001:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        SceneDebugger.Get().AddMessage(rumbleDomeDungeon.m_PlayerVOLines[rumbleDomeDungeon.m_PlayPlayerVOLineIndex]);
        yield return (object) rumbleDomeDungeon.PlayBossLine(actor1, rumbleDomeDungeon.m_PlayerVOLines[rumbleDomeDungeon.m_PlayPlayerVOLineIndex]);
        break;
      case 1002:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        if (rumbleDomeDungeon.m_PlayBossVOLineIndex + 1 >= rumbleDomeDungeon.m_BossVOLines.Count)
          rumbleDomeDungeon.m_PlayBossVOLineIndex = 0;
        else
          ++rumbleDomeDungeon.m_PlayBossVOLineIndex;
        SceneDebugger.Get().AddMessage(rumbleDomeDungeon.m_BossVOLines[rumbleDomeDungeon.m_PlayBossVOLineIndex]);
        yield return (object) rumbleDomeDungeon.PlayBossLine(actor2, rumbleDomeDungeon.m_BossVOLines[rumbleDomeDungeon.m_PlayBossVOLineIndex]);
        break;
      case 1003:
        GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
        SceneDebugger.Get().AddMessage(rumbleDomeDungeon.m_BossVOLines[rumbleDomeDungeon.m_PlayBossVOLineIndex]);
        yield return (object) rumbleDomeDungeon.PlayBossLine(actor2, rumbleDomeDungeon.m_BossVOLines[rumbleDomeDungeon.m_PlayBossVOLineIndex]);
        break;
      case 1010:
        if (rumbleDomeDungeon.m_forceAlwaysPlayLine)
        {
          rumbleDomeDungeon.m_forceAlwaysPlayLine = false;
          break;
        }
        rumbleDomeDungeon.m_forceAlwaysPlayLine = true;
        break;
      case 58023:
        SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
        GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
        SceneMgr.Get().SetNextMode(postGameSceneMode);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) rumbleDomeDungeon.\u003C\u003En__0(missionEvent);
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

  public override void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      return;
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DHMulligan);
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DHPrologue);
}
