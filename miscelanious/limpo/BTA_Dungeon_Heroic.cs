using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTA_Dungeon_Heroic : BTA_MissionEntity_Heroic
{
  public List<string> m_BossVOLines = new List<string>();
  public List<string> m_PlayerVOLines = new List<string>();
  public bool m_DisableIdle;
  private static readonly AssetReference Karnuk_Demon_Hunter_Popup_BrassRing = new AssetReference("Karnuk_Demon_Hunter_Popup_BrassRing.prefab:af78f17e1126eef41b6700cad3d1bccb");
  public static readonly AssetReference KarnukBrassRing = new AssetReference("Karnuk_Outcast_Popup_BrassRing.prefab:d097e6294875881488492604e9320e64");
  public static readonly AssetReference KarnukBrassRingDemonHunter = new AssetReference("Karnuk_Demon_Hunter_Popup_BrassRing.prefab:af78f17e1126eef41b6700cad3d1bccb");
  public static readonly AssetReference ShaljaBrassRing = new AssetReference("Shalja_Outcast_Popup_BrassRing.prefab:0425972e057e448458abedcc24797c3a");
  public static readonly AssetReference ShaljaBrassRingDemonHunter = new AssetReference("Shalja_Demon_Hunter_Popup_BrassRing.prefab:08f4bb41a6104a94ca96bb8003fa826f");
  public static readonly AssetReference BaduuBrassRing = new AssetReference("Baduu_Outcast_Popup_BrassRing.prefab:9202d8afcf6e80542ae9dafd691df43f");
  public static readonly AssetReference SklibbBrassRing = new AssetReference("Sklibb_Outcast_Popup_BrassRing.prefab:ec8003f5e3c1c564cb20b106672a8ed4");
  public static readonly AssetReference SklibbBrassRingDemonHunter = new AssetReference("Sklibb_Demon_Hunter_Popup_BrassRing.prefab:6bf5ceddde5f11347bb7df1c1266fb20");
  public static readonly AssetReference IllidanBrassRing = new AssetReference("DemonHunter_Illidan_Popup_BrassRing.prefab:8c007b8e8be417c4fbd9738960e6f7f0");
  public static readonly AssetReference ArannaBrassRing = new AssetReference("Aranna_Explorer_Popup_Banner.prefab:2d1aaedce4ece664680073bf82f191d6");
  public static readonly AssetReference ArannaBrassRingInTraining = new AssetReference("Aranna_Training_Popup_BrassRing.prefab:d2b86b1c51e1f734daee22d98b4abdcf");
  public static readonly AssetReference ArannaBrassRingDemonHunter = new AssetReference("Aranna_Demon_Hunter_Popup_BrassRing.prefab:57c34d7d7bffe1849a85ffbcf95cda3a");
  public string m_introLine;
  public string m_deathLine;
  public string m_standardEmoteResponseLine;
  public List<string> m_BossIdleLines;
  public List<string> m_BossIdleLinesCopy;
  private int m_PlayPlayerVOLineIndex;
  private int m_PlayBossVOLineIndex;

  public static BTA_Dungeon_Heroic InstantiateBTADungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    string opposingHeroCardId = GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame);
    switch (opposingHeroCardId)
    {
      case "BTA_BOSS_18h":
        return (BTA_Dungeon_Heroic) new BTA_Fight_18();
      case "BTA_BOSS_19h":
        return (BTA_Dungeon_Heroic) new BTA_Fight_20();
      case "BTA_BOSS_20h":
        return (BTA_Dungeon_Heroic) new BTA_Fight_19();
      case "BTA_BOSS_21h":
        return (BTA_Dungeon_Heroic) new BTA_Fight_21();
      case "BTA_BOSS_22h":
        return (BTA_Dungeon_Heroic) new BTA_Fight_22();
      case "BTA_BOSS_23h":
        return (BTA_Dungeon_Heroic) new BTA_Fight_23();
      case "BTA_BOSS_24h":
        return (BTA_Dungeon_Heroic) new BTA_Fight_24();
      case "BTA_BOSS_25h":
        return (BTA_Dungeon_Heroic) new BTA_Fight_25();
      case "BTA_BOSS_26h":
        return (BTA_Dungeon_Heroic) new BTA_Fight_26();
      default:
        Log.All.PrintError("BTA_Dungeon.InstantiateBTADungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) opposingHeroCardId);
        return new BTA_Dungeon_Heroic();
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

  protected virtual IEnumerable OnBossHeroPowerPlayed(Entity entity)
  {
    BTA_Dungeon_Heroic btaDungeonHeroic = this;
    float chanceToPlay = btaDungeonHeroic.ChanceToPlayBossHeroPowerVOLine();
    float chanceRoll = Random.Range(0.0f, 1f);
    while (btaDungeonHeroic.m_enemySpeaking)
      yield return (object) null;
    if ((double) chanceToPlay < (double) chanceRoll)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (!((Object) enemyActor == (Object) null))
    {
      List<string> powerRandomLines = btaDungeonHeroic.GetBossHeroPowerRandomLines();
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
        yield return (object) null;
      yield return (object) btaDungeonHeroic.PlayLineAlways(enemyActor, btaDungeonHeroic.m_PlayerVOLines[btaDungeonHeroic.m_PlayPlayerVOLineIndex]);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Dungeon_Heroic btaDungeonHeroic = this;
    if (!btaDungeonHeroic.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.OPPOSING)
      yield return (object) btaDungeonHeroic.OnBossHeroPowerPlayed(entity);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Dungeon_Heroic btaDungeonHeroic = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (btaDungeonHeroic.m_enemySpeaking)
      yield return (object) null;
    yield return (object) btaDungeonHeroic.WaitForEntitySoundToFinish(entity);
    entity.GetCardId();
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
  }

  protected virtual bool GetShouldSuppressDeathTextBubble() => false;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Dungeon_Heroic btaDungeonHeroic = this;
    while (btaDungeonHeroic.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (btaDungeonHeroic.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num = (double) Random.Range(0.0f, 1f);
      btaDungeonHeroic.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      switch (missionEvent)
      {
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (btaDungeonHeroic.m_PlayPlayerVOLineIndex + 1 >= btaDungeonHeroic.m_PlayerVOLines.Count)
            btaDungeonHeroic.m_PlayPlayerVOLineIndex = 0;
          else
            ++btaDungeonHeroic.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(btaDungeonHeroic.m_PlayerVOLines[btaDungeonHeroic.m_PlayPlayerVOLineIndex]);
          yield return (object) btaDungeonHeroic.PlayBossLine(actor, btaDungeonHeroic.m_PlayerVOLines[btaDungeonHeroic.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(btaDungeonHeroic.m_PlayerVOLines[btaDungeonHeroic.m_PlayPlayerVOLineIndex]);
          yield return (object) btaDungeonHeroic.PlayBossLine(actor, btaDungeonHeroic.m_PlayerVOLines[btaDungeonHeroic.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (btaDungeonHeroic.m_PlayBossVOLineIndex + 1 >= btaDungeonHeroic.m_BossVOLines.Count)
            btaDungeonHeroic.m_PlayBossVOLineIndex = 0;
          else
            ++btaDungeonHeroic.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(btaDungeonHeroic.m_BossVOLines[btaDungeonHeroic.m_PlayBossVOLineIndex]);
          yield return (object) btaDungeonHeroic.PlayBossLine(enemyActor, btaDungeonHeroic.m_BossVOLines[btaDungeonHeroic.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(btaDungeonHeroic.m_BossVOLines[btaDungeonHeroic.m_PlayBossVOLineIndex]);
          yield return (object) btaDungeonHeroic.PlayBossLine(enemyActor, btaDungeonHeroic.m_BossVOLines[btaDungeonHeroic.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (btaDungeonHeroic.m_forceAlwaysPlayLine)
          {
            btaDungeonHeroic.m_forceAlwaysPlayLine = false;
            break;
          }
          btaDungeonHeroic.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in btaDungeonHeroic.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) btaDungeonHeroic.PlayLineAlways(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in btaDungeonHeroic.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) btaDungeonHeroic.PlayLineAlways(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in btaDungeonHeroic.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) btaDungeonHeroic.PlayLineAlways(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in btaDungeonHeroic.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) btaDungeonHeroic.PlayLineAlways(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) btaDungeonHeroic.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  public override string GetNameBannerSubtextOverride(Player.Side playerSide)
  {
    GameMgr.Get().GetMissionId();
    return base.GetNameBannerSubtextOverride(playerSide);
  }

  public virtual float GetThinkEmoteBossThinkChancePercentage() => 0.25f;

  public override void OnPlayThinkEmote()
  {
    if (this.m_DisableIdle || this.m_enemySpeaking)
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
}
