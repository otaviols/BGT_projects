using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTA_Dungeon : BTA_MissionEntity
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

  public static BTA_Dungeon InstantiateBTADungeonMissionEntityForBoss(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    string opposingHeroCardId = GenericDungeonMissionEntity.GetOpposingHeroCardID(powerList, createGame);
    switch (opposingHeroCardId)
    {
      case "BTA_BOSS_01h":
        return (BTA_Dungeon) new BTA_Fight_01();
      case "BTA_BOSS_02h":
        return (BTA_Dungeon) new BTA_Fight_02();
      case "BTA_BOSS_03h":
        return (BTA_Dungeon) new BTA_Fight_03();
      case "BTA_BOSS_04h":
        return (BTA_Dungeon) new BTA_Fight_04();
      case "BTA_BOSS_05h":
        return (BTA_Dungeon) new BTA_Fight_05();
      case "BTA_BOSS_06h":
        return (BTA_Dungeon) new BTA_Fight_06();
      case "BTA_BOSS_07h":
        return (BTA_Dungeon) new BTA_Fight_07();
      case "BTA_BOSS_08h":
        return (BTA_Dungeon) new BTA_Fight_08();
      case "BTA_BOSS_09h":
        return (BTA_Dungeon) new BTA_Fight_09();
      case "BTA_BOSS_10h":
        return (BTA_Dungeon) new BTA_Fight_10();
      case "BTA_BOSS_11h":
        return (BTA_Dungeon) new BTA_Fight_11();
      case "BTA_BOSS_12h":
        return (BTA_Dungeon) new BTA_Fight_12();
      case "BTA_BOSS_13h":
        return (BTA_Dungeon) new BTA_Fight_13();
      case "BTA_BOSS_14h":
        return (BTA_Dungeon) new BTA_Fight_14();
      case "BTA_BOSS_15h":
        return (BTA_Dungeon) new BTA_Fight_15();
      case "BTA_BOSS_16h":
        return (BTA_Dungeon) new BTA_Fight_16();
      case "BTA_BOSS_17h":
        return (BTA_Dungeon) new BTA_Fight_17();
      default:
        Log.All.PrintError("BTA_Dungeon.InstantiateBTADungeonMissionEntityForBoss() - Found unsupported enemy Boss {0}.", (object) opposingHeroCardId);
        return new BTA_Dungeon();
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
    BTA_Dungeon btaDungeon = this;
    float chanceToPlay = btaDungeon.ChanceToPlayBossHeroPowerVOLine();
    float chanceRoll = Random.Range(0.0f, 1f);
    while (btaDungeon.m_enemySpeaking)
      yield return (object) null;
    if ((double) chanceToPlay < (double) chanceRoll)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (!((Object) enemyActor == (Object) null))
    {
      List<string> powerRandomLines = btaDungeon.GetBossHeroPowerRandomLines();
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
      yield return (object) btaDungeon.PlayLineAlways(enemyActor, btaDungeon.m_PlayerVOLines[btaDungeon.m_PlayPlayerVOLineIndex]);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Dungeon btaDungeon = this;
    if (!btaDungeon.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.OPPOSING)
      yield return (object) btaDungeon.OnBossHeroPowerPlayed(entity);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Dungeon btaDungeon = this;
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    while (btaDungeon.m_enemySpeaking)
      yield return (object) null;
    yield return (object) btaDungeon.WaitForEntitySoundToFinish(entity);
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
    BTA_Dungeon btaDungeon = this;
    while (btaDungeon.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (btaDungeon.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
      double num = (double) Random.Range(0.0f, 1f);
      btaDungeon.GetTag(GAME_TAG.TURN);
      GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      switch (missionEvent)
      {
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (btaDungeon.m_PlayPlayerVOLineIndex + 1 >= btaDungeon.m_PlayerVOLines.Count)
            btaDungeon.m_PlayPlayerVOLineIndex = 0;
          else
            ++btaDungeon.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(btaDungeon.m_PlayerVOLines[btaDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) btaDungeon.PlayBossLine(actor, btaDungeon.m_PlayerVOLines[btaDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(btaDungeon.m_PlayerVOLines[btaDungeon.m_PlayPlayerVOLineIndex]);
          yield return (object) btaDungeon.PlayBossLine(actor, btaDungeon.m_PlayerVOLines[btaDungeon.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (btaDungeon.m_PlayBossVOLineIndex + 1 >= btaDungeon.m_BossVOLines.Count)
            btaDungeon.m_PlayBossVOLineIndex = 0;
          else
            ++btaDungeon.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(btaDungeon.m_BossVOLines[btaDungeon.m_PlayBossVOLineIndex]);
          yield return (object) btaDungeon.PlayBossLine(enemyActor, btaDungeon.m_BossVOLines[btaDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(btaDungeon.m_BossVOLines[btaDungeon.m_PlayBossVOLineIndex]);
          yield return (object) btaDungeon.PlayBossLine(enemyActor, btaDungeon.m_BossVOLines[btaDungeon.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          if (btaDungeon.m_forceAlwaysPlayLine)
          {
            btaDungeon.m_forceAlwaysPlayLine = false;
            break;
          }
          btaDungeon.m_forceAlwaysPlayLine = true;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in btaDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) btaDungeon.PlayLineAlways(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in btaDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) btaDungeon.PlayLineAlways(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in btaDungeon.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) btaDungeon.PlayLineAlways(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in btaDungeon.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) btaDungeon.PlayLineAlways(enemyActor, playerVoLine);
          }
          break;
        case 58023:
          SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
          GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
          SceneMgr.Get().SetNextMode(postGameSceneMode);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) btaDungeon.\u003C\u003En__0(missionEvent);
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

  public IEnumerator PlayRandomLineAlways(Actor actor, List<string> lines)
  {
    BTA_Dungeon btaDungeon = this;
    string line = btaDungeon.PopRandomLine(lines);
    if (line != null)
      yield return (object) btaDungeon.PlayBossLine(actor, line);
  }

  public IEnumerator PlayAndRemoveRandomLineOnlyOnceWithBrassRing(
    Actor actor,
    AssetReference brassRingBackup,
    List<string> lines)
  {
    BTA_Dungeon btaDungeon = this;
    if ((Object) actor != (Object) null)
      yield return (object) btaDungeon.PlayAndRemoveRandomLineOnlyOnce(actor, lines);
    else if (brassRingBackup != null)
      yield return (object) btaDungeon.PlayAndRemoveRandomLineOnlyOnce((string) brassRingBackup, lines);
  }

  protected IEnumerator PlayLineAlwaysWithBrassRing(
    Actor actor,
    AssetReference brassRingBackup,
    string line,
    float duration = 2.5f)
  {
    BTA_Dungeon btaDungeon = this;
    if ((Object) actor != (Object) null)
      yield return (object) btaDungeon.PlayLineAlways(actor, line);
    else if (brassRingBackup != null)
      yield return (object) btaDungeon.PlayLineAlways((string) brassRingBackup, line);
  }

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
