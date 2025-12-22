using Blizzard.T5.Core;
using Hearthstone;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOTA_MissionEntity : GenericDungeonMissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BOTA_MissionEntity.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = BOTA_MissionEntity.InitStringOptions();
  private static readonly AssetReference PuzzleIntroUI_Mirror = new AssetReference("PuzzleIntroUI_Mirror.prefab:d1c537160881d574f9ec948c60f7053a");
  private static readonly AssetReference PuzzleIntroUI_Lethal = new AssetReference("PuzzleIntroUI_Lethal.prefab:2991b0a18a580eb4dac344255b615563");
  private static readonly AssetReference PuzzleIntroUI_Survival = new AssetReference("PuzzleIntroUI_Survival.prefab:0ffd8ff37cf93e844b58b5babbba9e02");
  private static readonly AssetReference PuzzleIntroUI_Clear = new AssetReference("PuzzleIntroUI_Clear.prefab:47371bd3bd83eda48af01e1f9e4be1ee");
  private static bool s_shownEndTurnReminder = false;
  private Notification m_endTurnReminder;
  private Coroutine m_endTurnReminderCoroutine;
  public bool m_waitingForTurnStartIndicatorAfterReset;
  private PuzzleIntroSpell m_introSpell;
  private NormalButton m_confirmButton;
  private bool m_entranceFinished;
  private bool m_confirmButtonPressed;
  public static string s_introLine = (string) null;
  public static string s_returnLine = (string) null;
  public bool s_returnLineOverride;
  public List<string> s_emoteLines = new List<string>();
  protected List<string> m_randomEmoteLines = new List<string>();
  public List<string> s_idleLines = new List<string>();
  protected List<string> m_randomIdleLines = new List<string>();
  public List<string> s_restartLines = new List<string>();
  protected List<string> m_randomRestartLines = new List<string>();
  public string s_victoryLine_1;
  public string s_victoryLine_2;
  public string s_victoryLine_3;
  public string s_victoryLine_4;
  public string s_victoryLine_5;
  public string s_victoryLine_6;
  public string s_victoryLine_7;
  public string s_victoryLine_8;
  public string s_victoryLine_9;
  public List<string> s_lethalCompleteLines = new List<string>();
  private bool lethalLineUsed;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public BOTA_MissionEntity() => this.m_gameOptions.AddOptions(BOTA_MissionEntity.s_booleanOptions, BOTA_MissionEntity.s_stringOptions);

  public override void OnCreateGame()
  {
    if (BOTA_MissionEntity.s_shownEndTurnReminder)
      return;
    GameState.Get().RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
    GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
  }

  public override void OnDecommissionGame()
  {
    GameState.Get().UnregisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
    GameState.Get().UnregisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver));
    base.OnDecommissionGame();
  }

  public override float? GetThinkEmoteDelayOverride() => new float?(50f + UnityEngine.Random.Range(0.0f, 20f));

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BOT);

  public override void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      return;
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BOTMulligan);
  }

  private IEnumerator ShowEndTurnReminderIfNeeded()
  {
    BOTA_MissionEntity botaMissionEntity = this;
    yield return (object) new WaitForSeconds(1f);
    Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
    if (optionsPacket != null && !optionsPacket.HasValidOption() && !BOTA_MissionEntity.s_shownEndTurnReminder)
    {
      BOTA_MissionEntity.s_shownEndTurnReminder = true;
      GameState.Get().UnregisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(botaMissionEntity.OnOptionsReceived));
      Vector3 position = EndTurnButton.Get().transform.position;
      position.x -= 3.1f;
      botaMissionEntity.m_endTurnReminder = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.GetTextScale(), GameStrings.Get("BOTA_PUZZLE_END_TURN_REMINDER"));
      botaMissionEntity.m_endTurnReminder.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
      botaMissionEntity.m_endTurnReminderCoroutine = (Coroutine) null;
    }
  }

  private void OnOptionsReceived(object userData)
  {
    if (SpectatorManager.Get().IsInSpectatorMode())
      return;
    if (this.m_endTurnReminderCoroutine != null)
    {
      Gameplay.Get().StopCoroutine(this.m_endTurnReminderCoroutine);
      this.m_endTurnReminderCoroutine = (Coroutine) null;
    }
    Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
    if (optionsPacket == null)
    {
      Log.Gameplay.PrintError("BOTA_MissionEntity wants options packet but option packet is null.");
    }
    else
    {
      if (BOTA_MissionEntity.s_shownEndTurnReminder || optionsPacket.HasValidOption())
        return;
      this.m_endTurnReminderCoroutine = Gameplay.Get().StartCoroutine(this.ShowEndTurnReminderIfNeeded());
    }
  }

  private void OnGameOver(TAG_PLAYSTATE playState, object userData) => this.DestroyEndTurnReminder();

  public override void NotifyOfResetGameStarted()
  {
    base.NotifyOfResetGameStarted();
    this.DestroyEndTurnReminder();
  }

  public override void NotifyOfResetGameFinished(Entity source, Entity oldGameEntity)
  {
    this.m_waitingForTurnStartIndicatorAfterReset = true;
    BOTA_MissionEntity botaMissionEntity = oldGameEntity as BOTA_MissionEntity;
    this.s_lethalCompleteLines = botaMissionEntity.s_lethalCompleteLines;
    this.lethalLineUsed = botaMissionEntity.lethalLineUsed;
    this.m_randomEmoteLines = botaMissionEntity.m_randomEmoteLines;
    this.m_randomIdleLines = botaMissionEntity.m_randomIdleLines;
    this.m_randomRestartLines = botaMissionEntity.m_randomRestartLines;
    base.NotifyOfResetGameFinished(source, oldGameEntity);
  }

  public override void OnTurnStartManagerFinished()
  {
    if (this.m_waitingForTurnStartIndicatorAfterReset && GameState.Get().GetGameEntity().GetTag(GAME_TAG.PREVIOUS_PUZZLE_COMPLETED) == 0)
      return;
    Gameplay.Get().StartCoroutine(this.OnTurnStartManagerFinishedWithTiming());
  }

  public virtual IEnumerator OnTurnStartManagerFinishedWithTiming()
  {
    BOTA_MissionEntity botaMissionEntity = this;
    while (botaMissionEntity.m_enemySpeaking)
      yield return (object) null;
    yield return (object) botaMissionEntity.RespondToPuzzleStartWithTiming();
    while (botaMissionEntity.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    int tag = GameState.Get().GetFriendlySidePlayer().GetSecretZone().GetPuzzleEntity().GetTag(GAME_TAG.PUZZLE_PROGRESS);
    string puzzleVictoryLine = botaMissionEntity.GetPuzzleVictoryLine(tag);
    if (puzzleVictoryLine != null)
      yield return (object) botaMissionEntity.PlayBossLine(actor, puzzleVictoryLine);
  }

  protected virtual IEnumerator RespondToPuzzleStartWithTiming()
  {
    yield break;
  }

  private void DestroyEndTurnReminder()
  {
    if (this.m_endTurnReminderCoroutine != null)
    {
      Gameplay.Get().StopCoroutine(this.m_endTurnReminderCoroutine);
      this.m_endTurnReminderCoroutine = (Coroutine) null;
    }
    if (!((UnityEngine.Object) this.m_endTurnReminder != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotification(this.m_endTurnReminder, 0.0f);
  }

  public override bool NotifyOfEndTurnButtonPushed()
  {
    this.DestroyEndTurnReminder();
    return true;
  }

  public override IEnumerator DoGameSpecificPostIntroActions()
  {
    BOTA_MissionEntity botaMissionEntity = this;
    botaMissionEntity.m_entranceFinished = false;
    botaMissionEntity.m_confirmButtonPressed = false;
    int currentPuzzleProgress = 0;
    int totalPuzzleProgress = 0;
    string puzzleName = "";
    string puzzleText = "";
    TAG_PUZZLE_TYPE puzzleType = TAG_PUZZLE_TYPE.INVALID;
    int maxNumAttempts = 2;
    if (HearthstoneApplication.IsPublic())
      maxNumAttempts = 10;
    bool puzzleInfoFound = false;
    for (int i = 0; i < maxNumAttempts; ++i)
    {
      puzzleInfoFound = botaMissionEntity.LookUpPuzzleInfoFromFutureTaskLists(out currentPuzzleProgress, out totalPuzzleProgress, out puzzleName, out puzzleText, out puzzleType);
      if (!puzzleInfoFound)
        yield return (object) new WaitForSeconds(1f);
      else
        break;
    }
    if (!puzzleInfoFound)
    {
      Log.Spells.PrintError("BOTA_MissionEntity.DoGameSpecificPostIntroActions(): puzzle info could not be found in the task lists - most likely the script for this game entity is not setting up a puzzle entity correctly.");
      if (puzzleType == TAG_PUZZLE_TYPE.INVALID)
        yield break;
    }
    GameObject gameObject = botaMissionEntity.LoadIntroUIForPuzzleType(puzzleType);
    PuzzleProgressUI component = gameObject.GetComponent<PuzzleProgressUI>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Log.Spells.PrintError("BOTA_MissionEntity.DoGameSpecificPostIntroActions(): No PuzzleProgressUI found on puzzle intro spell {0}.", (object) gameObject.gameObject.name);
    }
    else
    {
      component.UpdateNameAndText(puzzleName, puzzleText);
      component.UpdateProgressValues(currentPuzzleProgress, totalPuzzleProgress);
      botaMissionEntity.m_introSpell = gameObject.GetComponent<PuzzleIntroSpell>();
      if ((UnityEngine.Object) botaMissionEntity.m_introSpell == (UnityEngine.Object) null)
        Log.Spells.PrintError("BOTA_MissionEntity.DoGameSpecificPostIntroActions(): No PuzzleIntroSpell found on puzzle intro spell {0}.", (object) gameObject.gameObject.name);
      else if ((UnityEngine.Object) botaMissionEntity.m_introSpell.GetConfirmButton() == (UnityEngine.Object) null)
      {
        Log.Spells.PrintError("BOTA_MissionEntity.DoGameSpecificPostIntroActions(): No confirmButton found on puzzle intro spell {0}.", (object) gameObject.gameObject.name);
      }
      else
      {
        botaMissionEntity.m_confirmButton = botaMissionEntity.m_introSpell.GetConfirmButton().GetComponentInChildren<NormalButton>();
        if ((UnityEngine.Object) botaMissionEntity.m_confirmButton == (UnityEngine.Object) null)
        {
          Log.Spells.PrintError(string.Format("BOTA_MissionEntity.DoGameSpecificPostIntroActions() - ERROR \"{0}\" has no {1} component", (object) botaMissionEntity.m_introSpell.GetConfirmButton(), (object) typeof (NormalButton)));
        }
        else
        {
          botaMissionEntity.m_introSpell.AddSpellEventCallback(new Spell.SpellEventCallback(botaMissionEntity.OnSpellEvent));
          botaMissionEntity.m_confirmButton.SetText(GameStrings.Get("GLOBAL_CONFIRM"));
          botaMissionEntity.m_confirmButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(botaMissionEntity.OnConfirmButtonReleased));
          botaMissionEntity.m_confirmButton.GetComponent<Collider>().enabled = true;
          botaMissionEntity.m_confirmButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Birth");
          botaMissionEntity.m_introSpell.ActivateState(SpellStateType.BIRTH);
          while ((UnityEngine.Object) botaMissionEntity.m_introSpell != (UnityEngine.Object) null && !botaMissionEntity.m_introSpell.IsFinished())
          {
            if (GameState.Get().WasConcedeRequested())
            {
              if (botaMissionEntity.m_confirmButtonPressed || !botaMissionEntity.m_entranceFinished)
              {
                yield break;
              }
              else
              {
                botaMissionEntity.m_confirmButton.SetEnabled(false);
                botaMissionEntity.ProgressPastConfirmButton();
                yield break;
              }
            }
            else
              yield return (object) null;
          }
          Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
          if (currentPuzzleProgress == 1)
            Gameplay.Get().StartCoroutine(botaMissionEntity.PlayBossLine(actor, BOTA_MissionEntity.s_introLine));
          else if (botaMissionEntity.s_returnLineOverride)
          {
            GameEntity gameEntity = GameState.Get().GetGameEntity();
            gameEntity.SetTag(GAME_TAG.MISSION_EVENT, 77);
            gameEntity.SetTag(GAME_TAG.MISSION_EVENT, 0);
          }
          else
            Gameplay.Get().StartCoroutine(botaMissionEntity.PlayBossLine(actor, BOTA_MissionEntity.s_returnLine));
        }
      }
    }
  }

  private GameObject LoadIntroUIForPuzzleType(TAG_PUZZLE_TYPE puzzleType)
  {
    switch (puzzleType)
    {
      case TAG_PUZZLE_TYPE.INVALID:
        Log.Spells.PrintError(string.Format("BOTA_MissionEntity.LoadIntroUIForPuzzleType() - invalid puzzle type"));
        return (GameObject) null;
      case TAG_PUZZLE_TYPE.MIRROR:
        return AssetLoader.Get().InstantiatePrefab(BOTA_MissionEntity.PuzzleIntroUI_Mirror);
      case TAG_PUZZLE_TYPE.LETHAL:
        return AssetLoader.Get().InstantiatePrefab(BOTA_MissionEntity.PuzzleIntroUI_Lethal);
      case TAG_PUZZLE_TYPE.SURVIVAL:
        return AssetLoader.Get().InstantiatePrefab(BOTA_MissionEntity.PuzzleIntroUI_Survival);
      case TAG_PUZZLE_TYPE.CLEAR:
        return AssetLoader.Get().InstantiatePrefab(BOTA_MissionEntity.PuzzleIntroUI_Clear);
      default:
        return (GameObject) null;
    }
  }

  private bool LookUpPuzzleInfoFromFutureTaskLists(
    out int currentPuzzleProgress,
    out int totalPuzzleProgress,
    out string puzzleName,
    out string puzzleText,
    out TAG_PUZZLE_TYPE puzzleType)
  {
    int currentPuzzleProgressFound = 0;
    int totalPuzzleProgressFound = 0;
    string puzzleNameFound = "";
    string puzzleTextFound = "";
    TAG_PUZZLE_TYPE puzzleTypeFound = TAG_PUZZLE_TYPE.INVALID;
    bool puzzleInfoFound = false;
    GameState.Get().GetPowerProcessor().ForEachTaskList((Action<int, PowerTaskList>) ((index, taskList) =>
    {
      if (currentPuzzleProgressFound != 0 && totalPuzzleProgressFound != 0)
        return;
      foreach (PowerTask task in taskList.GetTaskList())
      {
        Network.PowerHistory power = task.GetPower();
        if (power.Type == Network.PowerType.FULL_ENTITY)
        {
          Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
          Network.Entity.Tag tag1 = histFullEntity.Entity.Tags.Find((Predicate<Network.Entity.Tag>) (tag => tag.Name == 982));
          if (tag1 != null)
          {
            puzzleTypeFound = (TAG_PUZZLE_TYPE) tag1.Value;
            CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(histFullEntity.Entity.CardID);
            if (cardRecord != null && cardRecord.Name != null && cardRecord.TextInHand != null)
            {
              puzzleNameFound = (string) cardRecord.Name;
              puzzleTextFound = (string) cardRecord.TextInHand;
            }
          }
        }
        if (power.Type == Network.PowerType.TAG_CHANGE)
        {
          Network.HistTagChange histTagChange = power as Network.HistTagChange;
          if (histTagChange.Tag == 980 && histTagChange.Value != 0)
            currentPuzzleProgressFound = histTagChange.Value;
          if (histTagChange.Tag == 981 && histTagChange.Value != 0)
            totalPuzzleProgressFound = histTagChange.Value;
          if (currentPuzzleProgressFound != 0 && totalPuzzleProgressFound != 0)
          {
            puzzleInfoFound = true;
            break;
          }
        }
      }
    }));
    currentPuzzleProgress = currentPuzzleProgressFound;
    totalPuzzleProgress = totalPuzzleProgressFound;
    puzzleName = puzzleNameFound;
    puzzleText = puzzleTextFound;
    puzzleType = puzzleTypeFound;
    return puzzleInfoFound;
  }

  private void OnConfirmButtonReleased(UIEvent e)
  {
    if (GameMgr.Get().IsSpectator())
      return;
    e.GetElement().SetEnabled(false);
    this.m_confirmButtonPressed = true;
    if (!(this.m_entranceFinished | GameState.Get().WasConcedeRequested()))
      return;
    this.ProgressPastConfirmButton();
  }

  private void ProgressPastConfirmButton()
  {
    this.m_introSpell.ActivateState(SpellStateType.DEATH);
    this.m_confirmButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Death");
  }

  private void OnSpellEvent(string eventName, object eventData, object userData)
  {
    if (!(eventName == "EntranceFinished"))
      return;
    bool flag = GameState.Get().WasConcedeRequested();
    this.m_entranceFinished = true;
    if (!(this.m_confirmButtonPressed | flag))
      return;
    this.ProgressPastConfirmButton();
  }

  protected virtual List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected virtual string GetBossDeathLine() => (string) null;

  protected virtual bool GetShouldSupressDeathTextBubble() => false;

  protected virtual float ChanceToPlayBossHeroPowerVOLine() => 0.5f;

  protected override float ChanceToPlayRandomVOLine() => 1f;

  protected virtual void OnBossHeroPowerPlayed(Entity entity)
  {
    float bossHeroPowerVoLine = this.ChanceToPlayBossHeroPowerVOLine();
    float num = UnityEngine.Random.Range(0.0f, 1f);
    if (this.m_enemySpeaking || (double) bossHeroPowerVoLine < (double) num)
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    List<string> powerRandomLines = this.GetBossHeroPowerRandomLines();
    string soundPath = "";
    while (powerRandomLines.Count > 0)
    {
      int index = UnityEngine.Random.Range(0, powerRandomLines.Count);
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
    BOTA_MissionEntity botaMissionEntity = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    if (botaMissionEntity.m_enemySpeaking || entity.GetCardType() == TAG_CARDTYPE.INVALID || entity.GetCardType() != TAG_CARDTYPE.HERO_POWER || entity.GetControllerSide() != Player.Side.OPPOSING)
      return false;
    botaMissionEntity.OnBossHeroPowerPlayed(entity);
    return false;
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string bossDeathLine = this.GetBossDeathLine();
    if (this.m_enemySpeaking || string.IsNullOrEmpty(bossDeathLine) || gameResult != TAG_PLAYSTATE.WON)
      return;
    if (this.GetShouldSupressDeathTextBubble())
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(bossDeathLine, Notification.SpeechBubbleDirection.None, actor));
    else
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(bossDeathLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    if (this.m_randomEmoteLines.Count == 0)
      this.m_randomEmoteLines = new List<string>((IEnumerable<string>) this.s_emoteLines);
    string soundPath = this.PopRandomLineWithChance(this.m_randomEmoteLines);
    if (soundPath == null)
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(soundPath, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (this.m_randomIdleLines.Count == 0)
      this.m_randomIdleLines = new List<string>((IEnumerable<string>) this.s_idleLines);
    string line = this.PopRandomLineWithChance(this.m_randomIdleLines);
    if (line == null)
      return;
    Gameplay.Get().StartCoroutine(this.PlayBossLine(actor, line));
  }

  protected override IEnumerator RespondToResetGameFinishedWithTiming(Entity entity)
  {
    BOTA_MissionEntity botaMissionEntity = this;
    while (botaMissionEntity.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (GameState.Get().GetGameEntity().GetTag(GAME_TAG.PREVIOUS_PUZZLE_COMPLETED) == 0)
    {
      if (botaMissionEntity.m_randomRestartLines.Count == 0)
        botaMissionEntity.m_randomRestartLines = new List<string>((IEnumerable<string>) botaMissionEntity.s_restartLines);
      string line = botaMissionEntity.PopRandomLineWithChance(botaMissionEntity.m_randomRestartLines);
      if (line != null)
        Gameplay.Get().StartCoroutine(botaMissionEntity.PlayBossLine(actor, line));
    }
  }

  private string GetPuzzleVictoryLine(int puzzleProgress)
  {
    switch (puzzleProgress)
    {
      case 1:
        return this.s_victoryLine_1;
      case 2:
        return this.s_victoryLine_2;
      case 3:
        return this.s_victoryLine_3;
      case 4:
        return this.s_victoryLine_4;
      case 5:
        return this.s_victoryLine_5;
      case 6:
        return this.s_victoryLine_6;
      case 7:
        return this.s_victoryLine_7;
      case 8:
        return this.s_victoryLine_8;
      case 9:
        return this.s_victoryLine_9;
      default:
        return (string) null;
    }
  }

  protected string GetLethalCompleteLine()
  {
    if (this.s_lethalCompleteLines.Count == 0)
      return (string) null;
    if (this.m_enemySpeaking)
      return (string) null;
    if (this.lethalLineUsed && UnityEngine.Random.Range(0, 100) >= 85)
      return (string) null;
    this.lethalLineUsed = true;
    string lethalCompleteLine = this.s_lethalCompleteLines[UnityEngine.Random.Range(0, this.s_lethalCompleteLines.Count)];
    this.s_lethalCompleteLines.Remove(lethalCompleteLine);
    return lethalCompleteLine;
  }
}
