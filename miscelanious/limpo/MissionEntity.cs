using Blizzard.T5.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionEntity : GameEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = MissionEntity.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = MissionEntity.InitStringOptions();
  protected const float TIME_TO_WAIT_BEFORE_ENDING_QUOTE = 5f;
  protected const float MINIMUM_DISPLAY_TIME_FOR_BIG_QUOTE = 3f;
  protected const float DEFAULT_VO_DURATION = 2.5f;
  protected static readonly List<EmoteType> STANDARD_EMOTE_RESPONSE_TRIGGERS = new List<EmoteType>()
  {
    EmoteType.GREETINGS,
    EmoteType.WELL_PLAYED,
    EmoteType.OOPS,
    EmoteType.SORRY,
    EmoteType.THANKS,
    EmoteType.THREATEN,
    EmoteType.WOW,
    EmoteType.FIRE_FESTIVAL_FIREWORKS_RANK_ONE,
    EmoteType.FIRE_FESTIVAL_FIREWORKS_RANK_TWO,
    EmoteType.FIRE_FESTIVAL_FIREWORKS_RANK_THREE,
    EmoteType.FROST_FESTIVAL_FIREWORKS_RANK_ONE,
    EmoteType.FROST_FESTIVAL_FIREWORKS_RANK_TWO,
    EmoteType.FROST_FESTIVAL_FIREWORKS_RANK_THREE,
    EmoteType.HAPPY_HALLOWEEN,
    EmoteType.HAPPY_NEW_YEAR
  };
  protected bool m_enemySpeaking;
  protected List<MissionEntity.EmoteResponseGroup> m_emoteResponseGroups = new List<MissionEntity.EmoteResponseGroup>();
  protected Notification m_ActiveSpeechBubble;
  protected VoPlaybackHandler m_voHandler;
  public bool m_forceAlwaysPlayLine;
  private HashSet<string> m_InOrderPlayedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.USE_SECRET_CLASS_NAMES,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public MissionEntity(VoPlaybackHandler voHandler = null)
  {
    this.m_voHandler = voHandler;
    if (voHandler != null)
      voHandler.Coroutines = GameEntity.Coroutines;
    this.m_gameOptions.AddOptions(MissionEntity.s_booleanOptions, MissionEntity.s_stringOptions);
    this.InitEmoteResponses();
  }

  public override void OnCreate()
  {
    base.OnCreate();
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.PreloadAssets();
    this.m_voHandler.OnCreateGame();
  }

  public override void OnTagChanged(TagDelta change)
  {
    switch ((GAME_TAG) change.tag)
    {
      case GAME_TAG.MISSION_EVENT:
        this.HandleMissionEvent(change.newValue);
        break;
      case GAME_TAG.STEP:
        if (change.newValue == 4)
        {
          this.HandleMulliganTagChange();
          break;
        }
        if (change.newValue == 10 && (change.oldValue == 9 || change.oldValue == 19) && !GameState.Get().IsFriendlySidePlayerTurn())
        {
          this.HandleStartOfTurn(this.GetTag(GAME_TAG.TURN));
          break;
        }
        break;
      case GAME_TAG.NEXT_STEP:
        if (change.newValue == 6)
        {
          this.HandleMainReadyStep();
          break;
        }
        if (change.newValue == 10 && (change.oldValue == 9 || change.oldValue == 19) && GameState.Get().IsLocalSidePlayerTurn())
        {
          TurnStartManager.Get().BeginPlayingTurnEvents();
          break;
        }
        break;
    }
    base.OnTagChanged(change);
  }

  public override void NotifyOfStartOfTurnEventsFinished() => this.HandleStartOfTurn(this.GetTag(GAME_TAG.TURN));

  public override void SendCustomEvent(int eventID) => this.HandleMissionEvent(eventID);

  public override void NotifyOfOpponentWillPlayCard(string cardId, Entity playedEntity)
  {
    base.NotifyOfOpponentWillPlayCard(cardId, playedEntity);
    if (this.m_voHandler != null)
      GameEntity.Coroutines.StartCoroutine(this.m_voHandler.RespondToWillPlayCardWithTiming(cardId, playedEntity));
    else
      GameEntity.Coroutines.StartCoroutine(this.RespondToWillPlayCardWithTiming(cardId, playedEntity));
  }

  public override void NotifyOfOpponentPlayedCard(Entity entity)
  {
    base.NotifyOfOpponentPlayedCard(entity);
    if (this.m_voHandler != null)
      GameEntity.Coroutines.StartCoroutine(this.m_voHandler.RespondToPlayedCardWithTiming(entity));
    else
      GameEntity.Coroutines.StartCoroutine(this.RespondToPlayedCardWithTiming(entity));
  }

  public override void NotifyOfFriendlyPlayedCard(Entity entity)
  {
    base.NotifyOfFriendlyPlayedCard(entity);
    if (this.m_voHandler != null)
      GameEntity.Coroutines.StartCoroutine(this.m_voHandler.RespondToFriendlyPlayedCardWithTiming(entity));
    else
      GameEntity.Coroutines.StartCoroutine(this.RespondToFriendlyPlayedCardWithTiming(entity));
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    if (this.m_voHandler != null)
      GameEntity.Coroutines.StartCoroutine(this.m_voHandler.HandleGameOverWithTiming(gameResult, new MissionEntity.OnChangeHandler(this.HandleGameOverWithTiming)));
    else
      GameEntity.Coroutines.StartCoroutine(this.HandleGameOverWithTiming(gameResult));
  }

  public override void NotifyOfResetGameStarted()
  {
    base.NotifyOfResetGameStarted();
    GameEntity.Coroutines.StopAllCoroutines();
  }

  public override void NotifyOfResetGameFinished(Entity source, Entity oldGameEntity)
  {
    base.NotifyOfResetGameFinished(source, oldGameEntity);
    if (this.m_voHandler != null)
      GameEntity.Coroutines.StartCoroutine(this.m_voHandler.RespondToResetGameFinishedWithTiming(source));
    else
      GameEntity.Coroutines.StartCoroutine(this.RespondToResetGameFinishedWithTiming(source));
  }

  public override void OnEmotePlayed(Card card, EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    if (!card.GetEntity().IsControlledByFriendlySidePlayer())
      return;
    if (this.m_voHandler != null)
      GameEntity.Coroutines.StartCoroutine(this.m_voHandler.HandlePlayerEmoteWithTiming(emoteType, emoteSpell));
    else
      GameEntity.Coroutines.StartCoroutine(this.HandlePlayerEmoteWithTiming(emoteType, emoteSpell));
  }

  public override bool DoAlternateMulliganIntro()
  {
    if (!this.ShouldDoAlternateMulliganIntro())
      return false;
    GameEntity.Coroutines.StartCoroutine(this.SkipStandardMulliganWithTiming());
    return true;
  }

  public bool IsHeroic() => GameMgr.Get().IsHeroicMission();

  public bool IsClassChallenge() => GameMgr.Get().IsClassChallengeMission();

  public override void NotifyOfEntityAttacked(Entity attacker, Entity defender)
  {
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.NotifyOfEntityAttacked(attacker, defender);
  }

  public override void NotifyOfMinionPlayed(Entity minion)
  {
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.NotifyOfMinionPlayed(minion);
  }

  public override void NotifyOfHeroChanged(Entity newHero)
  {
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.NotifyOfHeroChanged(newHero);
  }

  public override void NotifyOfWeaponEquipped(Entity weapon)
  {
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.NotifyOfWeaponEquipped(weapon);
  }

  public override void NotifyOfSpellPlayed(Entity spell, Entity target)
  {
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.NotifyOfSpellPlayed(spell, target);
  }

  public override void NotifyOfHeroPowerUsed(Entity heroPower, Entity target)
  {
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.NotifyOfHeroPowerUsed(heroPower, target);
  }

  public override void NotifyOfMinionDied(Entity minion)
  {
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.NotifyOfMinionDied(minion);
  }

  public override void NotifyOfHeroDied(Entity hero)
  {
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.NotifyOfHeroDied(hero);
  }

  public override void NotifyOfWeaponDestroyed(Entity weapon)
  {
    if (this.m_voHandler == null)
      return;
    this.m_voHandler.NotifyOfWeaponDestroyed(weapon);
  }

  protected virtual void HandleMainReadyStep()
  {
    if (GameState.Get() == null)
    {
      Log.Gameplay.PrintError("MissionEntity.HandleMainReadyStep(): GameState is null.");
    }
    else
    {
      GameEntity gameEntity = GameState.Get().GetGameEntity();
      if (gameEntity == null)
      {
        Log.Gameplay.PrintError("MissionEntity.HandleMainReadyStep(): GameEntity is null.");
      }
      else
      {
        if (gameEntity.GetTag(GAME_TAG.TURN) != 1)
          return;
        if (GameState.Get().IsMulliganManagerActive())
        {
          GameState.Get().SetMulliganBusy(true);
        }
        else
        {
          if (this.ShouldDoAlternateMulliganIntro())
            return;
          GameState.Get().SetMulliganBusy(true);
          if (!((UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null))
            return;
          MulliganManager.Get().SkipMulligan();
        }
      }
    }
  }

  public void SetBlockVo(bool shouldBlock, float unblockAfterSeconds = 0.0f)
  {
    if ((double) unblockAfterSeconds < 0.0)
      unblockAfterSeconds = 0.0f;
    if (!shouldBlock)
      this.m_enemySpeaking = shouldBlock;
    if (!shouldBlock)
      return;
    if (this.m_voHandler != null)
      GameEntity.Coroutines.StartCoroutine(this.m_voHandler.UnblockSpeechAgainAfterDuration(unblockAfterSeconds));
    else
      GameEntity.Coroutines.StartCoroutine(this.UnblockSpeechAgainAfterDuration(unblockAfterSeconds));
  }

  private IEnumerator UnblockSpeechAgainAfterDuration(float durationInSeconds)
  {
    if ((double) durationInSeconds <= 0.0)
    {
      this.m_enemySpeaking = false;
    }
    else
    {
      while (this.m_enemySpeaking)
        yield return (object) null;
      this.m_enemySpeaking = true;
      yield return (object) new WaitForSeconds(durationInSeconds);
      this.m_enemySpeaking = false;
    }
  }

  protected virtual void HandleMulliganTagChange() => MulliganManager.Get().BeginMulligan();

  protected void HandleStartOfTurn(int turn)
  {
    if (GameState.Get().GetGameEntity().GetTag(GAME_TAG.IS_CURRENT_TURN_AN_EXTRA_TURN) != 0)
      return;
    int turn1 = turn - GameState.Get().GetGameEntity().GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
    if (this.m_voHandler != null)
      GameEntity.Coroutines.StartCoroutine(this.m_voHandler.HandleStartOfTurnWithTiming(turn1));
    else
      GameEntity.Coroutines.StartCoroutine(this.HandleStartOfTurnWithTiming(turn1));
  }

  protected virtual IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    yield break;
  }

  protected void HandleMissionEvent(int missionEvent) => GameEntity.Coroutines.StartCoroutine(this.HandleVoThenMissionEventWithTiming(missionEvent));

  protected IEnumerator HandleVoThenMissionEventWithTiming(int missionEvent)
  {
    if (this.m_voHandler != null)
      yield return (object) this.m_voHandler.HandleMissionEventWithTiming(missionEvent);
    yield return (object) this.HandleMissionEventWithTiming(missionEvent);
  }

  protected virtual IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    yield break;
  }

  protected virtual IEnumerator RespondToWillPlayCardWithTiming(
    string cardId,
    Entity playedEntity)
  {
    yield break;
  }

  protected virtual IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield break;
  }

  protected virtual IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    yield break;
  }

  protected virtual IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    yield break;
  }

  protected virtual IEnumerator RespondToResetGameFinishedWithTiming(Entity source)
  {
    yield break;
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    if (this.m_voHandler != null)
      yield return (object) this.m_voHandler.DoActionsBeforeDealingBaseMulliganCards();
  }

  protected void PlaySound(
    string soundPath,
    float waitTimeScale = 1f,
    bool parentBubbleToActor = true,
    bool delayCardSoundSpells = false)
  {
    GameEntity.Coroutines.StartCoroutine(this.PlaySoundAndWait(soundPath, (string) null, Notification.SpeechBubbleDirection.None, (Actor) null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells));
  }

  protected IEnumerator PlaySoundAndBlockSpeech(
    string soundPath,
    float waitTimeScale = 1f,
    bool parentBubbleToActor = true,
    bool delayCardSoundSpells = false)
  {
    this.m_enemySpeaking = true;
    yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlaySoundAndWait(soundPath, (string) null, Notification.SpeechBubbleDirection.None, (Actor) null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells));
    this.m_enemySpeaking = false;
  }

  protected IEnumerator PlaySoundAndBlockSpeechWithCustomGameString(
    string soundPath,
    string gameString,
    Notification.SpeechBubbleDirection direction,
    Actor actor,
    float waitTimeScale = 1f,
    bool parentBubbleToActor = true,
    bool delayCardSoundSpells = false)
  {
    this.m_enemySpeaking = true;
    if ((bool) (UnityEngine.Object) actor && (UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null && MulliganManager.Get().IsMulliganActive() && actor.GetEntity() != null && actor.GetEntity().IsHero())
      GameState.Get().GetGameEntity().FadeInHeroActor(actor);
    yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlaySoundAndWait(soundPath, gameString, direction, actor, waitTimeScale, parentBubbleToActor, delayCardSoundSpells));
    if ((bool) (UnityEngine.Object) actor && (UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null && MulliganManager.Get().IsMulliganActive() && actor.GetEntity() != null && actor.GetEntity().IsHero())
      GameState.Get().GetGameEntity().FadeOutHeroActor(actor);
    this.m_enemySpeaking = false;
  }

  protected IEnumerator PlaySoundAndBlockSpeech(
    string soundPath,
    Notification.SpeechBubbleDirection direction,
    Actor actor,
    float testingDuration = 3f,
    float waitTimeScale = 1f,
    bool parentBubbleToActor = true,
    bool delayCardSoundSpells = false,
    float bubbleScale = 0.0f)
  {
    string legacyAssetName = new AssetReference(soundPath).GetLegacyAssetName();
    this.m_enemySpeaking = true;
    if ((bool) (UnityEngine.Object) actor && (UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null && MulliganManager.Get().IsMulliganActive() && !MulliganManager.Get().IsCustomIntroActive() && actor.GetEntity() != null && actor.GetEntity().IsHero())
    {
      iTween.StopByName(MulliganManager.Get().gameObject, this.GetMulliganHeroFadeItweenName(actor));
      GameState.Get().GetGameEntity().FadeInHeroActor(actor);
    }
    yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlaySoundAndWait(soundPath, legacyAssetName, direction, actor, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, testingDuration, bubbleScale));
    if ((bool) (UnityEngine.Object) actor && (UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null && MulliganManager.Get().IsMulliganActive() && !MulliganManager.Get().IsCustomIntroActive() && actor.GetEntity() != null && actor.GetEntity().IsHero())
      GameState.Get().GetGameEntity().FadeOutHeroActor(actor);
    this.m_enemySpeaking = false;
  }

  protected IEnumerator PlaySoundAndBlockSpeechOnce(
    string soundPath,
    Notification.SpeechBubbleDirection direction,
    Actor actor,
    float testingDuration = 3f,
    float waitTimeScale = 1f,
    bool parentBubbleToActor = true,
    bool delayCardSoundSpells = false,
    float bubbleScale = 0.0f)
  {
    if (!NotificationManager.Get().HasSoundPlayedThisSession(soundPath))
    {
      NotificationManager.Get().ForceAddSoundToPlayedList(soundPath);
      string legacyAssetName = new AssetReference(soundPath).GetLegacyAssetName();
      this.m_enemySpeaking = true;
      yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlaySoundAndWait(soundPath, legacyAssetName, direction, actor, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, testingDuration, bubbleScale));
      this.m_enemySpeaking = false;
    }
  }

  protected IEnumerator PlaySoundAndWait(
    string soundPath,
    string gameString,
    Notification.SpeechBubbleDirection direction,
    Actor actor,
    float waitTimeScale = 1f,
    bool parentBubbleToActor = true,
    bool delayCardSoundSpells = false,
    float testingDuration = 3f,
    float bubbleScale = 0.0f)
  {
    MissionEntity missionEntity = this;
    AudioSource source = (AudioSource) null;
    bool isJustTesting = false;
    if (string.IsNullOrEmpty(soundPath) || !missionEntity.CheckPreloadedSound(soundPath))
      isJustTesting = true;
    else
      source = missionEntity.GetPreloadedSound(soundPath);
    if (!isJustTesting && ((UnityEngine.Object) source == (UnityEngine.Object) null || (UnityEngine.Object) source.clip == (UnityEngine.Object) null))
    {
      if (missionEntity.CheckPreloadedSound(soundPath))
      {
        missionEntity.RemovePreloadedSound(soundPath);
        missionEntity.PreloadSound(soundPath);
        while (missionEntity.IsPreloadingAssets())
          yield return (object) null;
        source = missionEntity.GetPreloadedSound(soundPath);
      }
      if ((UnityEngine.Object) source == (UnityEngine.Object) null || (UnityEngine.Object) source.clip == (UnityEngine.Object) null)
      {
        Log.Sound.PrintDebug("MissionEntity.PlaySoundAndWait() - sound error - " + soundPath);
        yield break;
      }
    }
    float num = testingDuration;
    if (!isJustTesting)
      num = source.clip.length;
    float seconds = num * waitTimeScale;
    if (!isJustTesting)
      SoundManager.Get().PlayPreloaded(source);
    if (delayCardSoundSpells)
      GameEntity.Coroutines.StartCoroutine(missionEntity.WaitForCardSoundSpellDelay(num));
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null && direction != Notification.SpeechBubbleDirection.None)
    {
      missionEntity.m_ActiveSpeechBubble = missionEntity.ShowBubble(gameString, direction, actor, false, num, parentBubbleToActor, bubbleScale);
      seconds += 0.5f;
    }
    yield return (object) new WaitForSeconds(seconds);
  }

  protected IEnumerator PlayCharacterQuoteAndWait(
    string prefabPath,
    string soundPath,
    float testingDuration = 0.0f,
    bool allowRepeatDuringSession = true,
    bool delayCardSoundSpells = false)
  {
    string legacyAssetName = new AssetReference(soundPath).GetLegacyAssetName();
    yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlayCharacterQuoteAndWait(prefabPath, soundPath, legacyAssetName, NotificationManager.DEFAULT_CHARACTER_POS, testingDuration: testingDuration, allowRepeatDuringSession: allowRepeatDuringSession, delayCardSoundSpells: delayCardSoundSpells));
  }

  protected IEnumerator PlayCharacterQuoteAndWait(
    string prefabPath,
    string soundPath,
    string gameString,
    Vector3 position,
    float waitTimeScale = 1f,
    float testingDuration = 0.0f,
    bool allowRepeatDuringSession = true,
    bool delayCardSoundSpells = false,
    bool isBig = false,
    Notification.SpeechBubbleDirection bubbleDir = Notification.SpeechBubbleDirection.None,
    bool persistCharacter = false,
    bool skippable = false)
  {
    MissionEntity missionEntity = this;
    AudioSource audioSource = (AudioSource) null;
    bool isJustTesting = false;
    if (string.IsNullOrEmpty(soundPath) || !missionEntity.CheckPreloadedSound(soundPath))
      isJustTesting = true;
    else
      audioSource = missionEntity.GetPreloadedSound(soundPath);
    if (!isJustTesting && ((UnityEngine.Object) audioSource == (UnityEngine.Object) null || (UnityEngine.Object) audioSource.clip == (UnityEngine.Object) null))
    {
      if (missionEntity.CheckPreloadedSound(soundPath))
      {
        missionEntity.RemovePreloadedSound(soundPath);
        missionEntity.PreloadSound(soundPath);
        while (missionEntity.IsPreloadingAssets())
          yield return (object) null;
        audioSource = missionEntity.GetPreloadedSound(soundPath);
      }
      if ((UnityEngine.Object) audioSource == (UnityEngine.Object) null || (UnityEngine.Object) audioSource.clip == (UnityEngine.Object) null)
      {
        Log.Sound.PrintDebug("MissionEntity.PlaySoundAndWait() - sound error - " + soundPath);
        yield break;
      }
    }
    float num = !isJustTesting ? audioSource.clip.length : testingDuration;
    if (!persistCharacter)
      num = Mathf.Max(num, 3f);
    float waitTime = num * waitTimeScale;
    Log.Notifications.Print("PlayCharacterQuoteAndWait() - Playing quote with clipLength {0}.  waitTimeScale: {1}  MINIMUM_DISPLAY_TIME_FOR_BIG_QUOTE: {2}", (object) num, (object) waitTimeScale, (object) 3f);
    if (delayCardSoundSpells)
      GameEntity.Coroutines.StartCoroutine(missionEntity.WaitForCardSoundSpellDelay(num));
    Action<int> finishCallback = skippable ? (Action<int>) (i => waitTime = 0.0f) : (Action<int>) null;
    Notification notification;
    if (isBig)
    {
      notification = NotificationManager.Get().CreateBigCharacterQuoteWithGameString(prefabPath, position, soundPath, gameString, allowRepeatDuringSession, num, finishCallback, bubbleDir: bubbleDir, persistCharacter: persistCharacter);
    }
    else
    {
      if (persistCharacter)
        Log.All.PrintWarning("PersistCharacter is not currently supported for CharacterQuotes that are not big!");
      notification = NotificationManager.Get().CreateCharacterQuote(prefabPath, position, GameStrings.Get(gameString), soundPath, allowRepeatDuringSession, num * 2f, finishCallback);
    }
    if ((bool) (UnityEngine.Object) notification)
    {
      if (!skippable && (UnityEngine.Object) notification.clickOff != (UnityEngine.Object) null)
        notification.clickOff = (PegUIElement) null;
      else if (skippable && (UnityEngine.Object) notification.clickOff == (UnityEngine.Object) null)
        Log.All.PrintWarning("Skippable character quotes require a clickOff reference!");
    }
    else
      Log.All.PrintWarning("PlayCharacterQuoteAndWait: 'notification' is null.");
    for (waitTime += 0.5f; (double) waitTime > 0.0; waitTime -= Time.deltaTime)
      yield return (object) null;
    if (!persistCharacter)
      NotificationManager.Get().DestroyActiveQuote(0.0f);
  }

  protected IEnumerator PlayBigCharacterQuoteAndWait(
    string prefabPath,
    string soundPath,
    Vector3 characterPosition,
    Notification.SpeechBubbleDirection bubbleDir = Notification.SpeechBubbleDirection.None,
    float testingDuration = 3f,
    float waitTimeScale = 1f,
    bool allowRepeatDuringSession = true,
    bool delayCardSoundSpells = false,
    bool persistCharacter = false,
    bool skippable = false)
  {
    string legacyAssetName = new AssetReference(soundPath).GetLegacyAssetName();
    yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlayCharacterQuoteAndWait(prefabPath, soundPath, legacyAssetName, characterPosition, waitTimeScale, testingDuration, allowRepeatDuringSession, delayCardSoundSpells, true, bubbleDir, persistCharacter, skippable));
  }

  protected IEnumerator PlayBigCharacterQuoteAndWait(
    string prefabPath,
    string soundPath,
    float testingDuration = 3f,
    float waitTimeScale = 1f,
    bool allowRepeatDuringSession = true,
    bool delayCardSoundSpells = false)
  {
    string legacyAssetName = new AssetReference(soundPath).GetLegacyAssetName();
    yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlayCharacterQuoteAndWait(prefabPath, soundPath, legacyAssetName, Vector3.zero, waitTimeScale, testingDuration, allowRepeatDuringSession, delayCardSoundSpells, true));
  }

  protected IEnumerator PlayBigCharacterQuoteAndWait(
    string prefabPath,
    string soundPath,
    string gameString,
    float testingDuration = 3f,
    float waitTimeScale = 1f,
    bool allowRepeatDuringSession = true,
    bool delayCardSoundSpells = false)
  {
    yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlayCharacterQuoteAndWait(prefabPath, soundPath, gameString, Vector3.zero, waitTimeScale, testingDuration, allowRepeatDuringSession, delayCardSoundSpells, true));
  }

  protected IEnumerator PlayBigCharacterQuoteAndWaitOnce(
    string prefabPath,
    string soundPath,
    float testingDuration = 3f,
    float waitTimeScale = 1f,
    bool delayCardSoundSpells = false,
    bool persistCharacter = false,
    bool skippable = false)
  {
    bool allowRepeatDuringSession = DemoMgr.Get().IsExpoDemo();
    string legacyAssetName = new AssetReference(soundPath).GetLegacyAssetName();
    yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlayCharacterQuoteAndWait(prefabPath, soundPath, legacyAssetName, Vector3.zero, waitTimeScale, testingDuration, allowRepeatDuringSession, delayCardSoundSpells, true, persistCharacter: persistCharacter, skippable: skippable));
  }

  protected IEnumerator WaitForCardSoundSpellDelay(float sec)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      missionEntity.GetGameOptions().SetBooleanOption(GameEntityOption.DELAY_CARD_SOUND_SPELLS, false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    missionEntity.GetGameOptions().SetBooleanOption(GameEntityOption.DELAY_CARD_SOUND_SPELLS, true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(sec);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected Notification ShowBubble(
    string textKey,
    Notification.SpeechBubbleDirection direction,
    Actor speakingActor,
    bool destroyOnNewNotification,
    float duration,
    bool parentToActor,
    float bubbleScale = 0.0f)
  {
    if ((UnityEngine.Object) speakingActor == (UnityEngine.Object) null)
      return (Notification) null;
    NotificationManager notificationManager = NotificationManager.Get();
    Notification speechBubble = notificationManager.CreateSpeechBubble(GameStrings.Get(textKey), direction, speakingActor, destroyOnNewNotification, parentToActor, bubbleScale);
    if ((double) duration > 0.0)
      notificationManager.DestroyNotification(speechBubble, duration);
    return speechBubble;
  }

  protected MissionEntity.ShouldPlayValue InternalShouldPlayOpeningLine() => MissionEntity.ShouldPlayValue.Always;

  protected MissionEntity.ShouldPlayValue InternalShouldPlayBossLine() => MissionEntity.ShouldPlayValue.Always;

  protected MissionEntity.ShouldPlayValue InternalShouldPlayMissionFlavorLine() => this.IsHeroic() ? MissionEntity.ShouldPlayValue.Once : MissionEntity.ShouldPlayValue.Always;

  protected MissionEntity.ShouldPlayValue InternalShouldPlayOnlyOnce() => MissionEntity.ShouldPlayValue.Once;

  protected MissionEntity.ShouldPlayValue InternalShouldPlayAdventureFlavorLine()
  {
    if (this.IsHeroic())
      return MissionEntity.ShouldPlayValue.Once;
    return this.IsClassChallenge() ? MissionEntity.ShouldPlayValue.Never : MissionEntity.ShouldPlayValue.Always;
  }

  protected MissionEntity.ShouldPlayValue InternalShouldPlayClosingLine() => this.IsClassChallenge() ? MissionEntity.ShouldPlayValue.Never : MissionEntity.ShouldPlayValue.Always;

  protected MissionEntity.ShouldPlayValue InternalShouldPlayEasterEggLine() => MissionEntity.ShouldPlayValue.Always;

  protected MissionEntity.ShouldPlayValue InternalShouldPlayCriticalLine() => MissionEntity.ShouldPlayValue.Always;

  protected Notification.SpeechBubbleDirection GetDirection(Actor actor)
  {
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null && actor.GetEntity() != null)
    {
      if (actor.GetEntity().IsControlledByFriendlySidePlayer())
        return Notification.SpeechBubbleDirection.BottomLeft;
    }
    else
      Log.Gameplay.PrintError("MissionEntity.GetDirection(): actor param is null");
    return Notification.SpeechBubbleDirection.TopRight;
  }

  protected string GetMulliganHeroFadeItweenName(Actor actor) => actor.GetEntity().IsControlledByFriendlySidePlayer() ? "MyHeroLightBlend" : "HisHeroLightBlend";

  protected IEnumerator PlayLittleCharacterLine(
    string speaker,
    string line,
    MissionEntity.ShouldPlay shouldPlay,
    float testingDuration)
  {
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
      yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlayCharacterQuoteAndWait(speaker, line, testingDuration));
  }

  protected IEnumerator PlayLine(
    string speaker,
    string line,
    MissionEntity.ShouldPlay shouldPlay,
    float duration = 2.5f,
    bool persistCharacter = false,
    bool skippable = false)
  {
    yield return (object) this.PlayLine(speaker, line, shouldPlay, Vector3.zero, Notification.SpeechBubbleDirection.None, duration, persistCharacter, skippable);
  }

  protected IEnumerator PlayLine(
    string speaker,
    string line,
    MissionEntity.ShouldPlay shouldPlay,
    Vector3 quotePosition,
    Notification.SpeechBubbleDirection direction,
    float duration,
    bool persistCharacter = false,
    bool skippable = false)
  {
    if (!this.m_enemySpeaking)
    {
      this.m_enemySpeaking = true;
      if (this.m_forceAlwaysPlayLine)
        yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlayBigCharacterQuoteAndWait(speaker, line, quotePosition, direction, duration, persistCharacter: persistCharacter, skippable: skippable));
      else if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
        yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlayBigCharacterQuoteAndWait(speaker, line, quotePosition, direction, duration, persistCharacter: persistCharacter, skippable: skippable));
      else if (shouldPlay() == MissionEntity.ShouldPlayValue.Once)
        yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlayBigCharacterQuoteAndWaitOnce(speaker, line, duration, persistCharacter: persistCharacter, skippable: skippable));
      NotificationManager.Get().ForceAddSoundToPlayedList(line);
      this.m_enemySpeaking = false;
    }
  }

  protected IEnumerator PlayLine(
    Actor speaker,
    string line,
    MissionEntity.ShouldPlay shouldPlay,
    float duration)
  {
    if (!this.m_enemySpeaking)
    {
      this.m_enemySpeaking = true;
      Notification.SpeechBubbleDirection direction = this.GetDirection(speaker);
      if (this.m_forceAlwaysPlayLine)
        yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
      else if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
        yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
      else if (shouldPlay() == MissionEntity.ShouldPlayValue.Once)
        yield return (object) GameEntity.Coroutines.StartCoroutine(this.PlaySoundAndBlockSpeechOnce(line, direction, speaker, duration));
      NotificationManager.Get().ForceAddSoundToPlayedList(line);
      this.m_enemySpeaking = false;
    }
  }

  protected bool ShouldPlayLine(string line, MissionEntity.ShouldPlay shouldPlay)
  {
    bool flag = false;
    switch (shouldPlay())
    {
      case MissionEntity.ShouldPlayValue.Once:
        if (DemoMgr.Get().IsExpoDemo() || !NotificationManager.Get().HasSoundPlayedThisSession(line))
        {
          flag = true;
          break;
        }
        break;
      case MissionEntity.ShouldPlayValue.Always:
        flag = true;
        break;
    }
    return flag;
  }

  protected IEnumerator PlayOpeningLine(string speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayOpeningLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayOpeningLine(Actor speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayOpeningLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayBossLine(string speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayBossLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayBossLine(Actor speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayBossLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayLineOnlyOnce(Actor speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayOnlyOnce), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayLineOnlyOnce(string speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayOnlyOnce), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayMissionFlavorLine(
    string speaker,
    string line,
    float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayMissionFlavorLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayMissionFlavorLine(
    string speaker,
    string line,
    Vector3 quotePosition,
    Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.None,
    float duration = 2.5f,
    bool persistCharacter = false)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayMissionFlavorLine), quotePosition, direction, duration, persistCharacter);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayMissionFlavorLine(
    Actor speaker,
    string line,
    float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayMissionFlavorLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayAdventureFlavorLine(
    string speaker,
    string line,
    float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayAdventureFlavorLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayAdventureFlavorLine(
    Actor speaker,
    string line,
    float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayAdventureFlavorLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayClosingLine(string speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLittleCharacterLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayClosingLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayEasterEggLine(string speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayEasterEggLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayEasterEggLine(Actor speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayEasterEggLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayCriticalLine(string speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayCriticalLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayCriticalLine(Actor speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayCriticalLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected bool ShouldPlayCriticalLine(string line) => this.ShouldPlayLine(line, new MissionEntity.ShouldPlay(this.InternalShouldPlayCriticalLine));

  protected bool ShouldPlayMissionFlavorLine(string line) => this.ShouldPlayLine(line, new MissionEntity.ShouldPlay(this.InternalShouldPlayMissionFlavorLine));

  protected bool ShouldPlayBossLine(string line) => this.ShouldPlayLine(line, new MissionEntity.ShouldPlay(this.InternalShouldPlayBossLine));

  protected bool ShouldPlayEasterEggLine(string line) => this.ShouldPlayLine(line, new MissionEntity.ShouldPlay(this.InternalShouldPlayEasterEggLine));

  protected bool ShouldPlayOpeningLine(string line) => this.ShouldPlayLine(line, new MissionEntity.ShouldPlay(this.InternalShouldPlayOpeningLine));

  protected IEnumerator PlayLineAlways(
    string speaker,
    string line,
    float duration = 2.5f,
    bool persistCharacter = false,
    bool skippable = false)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayBossLine), duration, persistCharacter, skippable);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayLineAlways(Actor speaker, string line, float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MissionEntity missionEntity = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayBossLine), duration);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayLineAlways(
    Actor speaker,
    string backupSpeaker,
    string line,
    float duration = 2.5f)
  {
    MissionEntity missionEntity = this;
    if ((UnityEngine.Object) speaker == (UnityEngine.Object) null)
      yield return (object) missionEntity.PlayLine(backupSpeaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayBossLine), duration);
    else
      yield return (object) missionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(missionEntity.InternalShouldPlayBossLine), duration);
  }

  public IEnumerator PlayLineInOrderOnce(Actor actor, List<string> lines)
  {
    string line = (string) null;
    for (int index = 0; index < lines.Count; ++index)
    {
      if (!this.m_InOrderPlayedLines.Contains(lines[index]))
      {
        line = lines[index];
        break;
      }
    }
    if (line != null)
    {
      this.m_InOrderPlayedLines.Add(line);
      yield return (object) this.PlayLineAlways(actor, line);
    }
  }

  public IEnumerator PlayLineInOrderOnce(string actor, List<string> lines)
  {
    string line = (string) null;
    for (int index = 0; index < lines.Count; ++index)
    {
      if (!this.m_InOrderPlayedLines.Contains(lines[index]))
      {
        line = lines[index];
        break;
      }
    }
    if (line != null)
    {
      this.m_InOrderPlayedLines.Add(line);
      yield return (object) this.PlayLineAlways(actor, line);
    }
  }

  protected virtual void InitEmoteResponses()
  {
  }

  protected IEnumerator HandlePlayerEmoteWithTiming(
    EmoteType emoteType,
    CardSoundSpell emoteSpell)
  {
    while (emoteSpell.IsActive())
      yield return (object) null;
    if (!this.m_enemySpeaking)
      this.PlayEmoteResponse(emoteType, emoteSpell);
  }

  protected virtual void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    foreach (MissionEntity.EmoteResponseGroup emoteResponseGroup in this.m_emoteResponseGroups)
    {
      if (emoteResponseGroup.m_responses.Count != 0 && emoteResponseGroup.m_triggers.Contains(emoteType))
      {
        this.PlayNextEmoteResponse(emoteResponseGroup, actor);
        this.CycleNextResponseGroupIndex(emoteResponseGroup);
      }
    }
  }

  protected void PlayNextEmoteResponse(MissionEntity.EmoteResponseGroup responseGroup, Actor actor)
  {
    int responseIndex = responseGroup.m_responseIndex;
    MissionEntity.EmoteResponse response = responseGroup.m_responses[responseIndex];
    GameEntity.Coroutines.StartCoroutine(this.PlaySoundAndBlockSpeechWithCustomGameString(response.m_soundName, response.m_stringTag, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected virtual void CycleNextResponseGroupIndex(MissionEntity.EmoteResponseGroup responseGroup)
  {
    if (responseGroup.m_responseIndex == responseGroup.m_responses.Count - 1)
      responseGroup.m_responseIndex = 0;
    else
      ++responseGroup.m_responseIndex;
  }

  protected Actor FindEnemyActorInPlayByDesignCode(string designCode) => this.FindActorInPlayByDesignCode(designCode, Player.Side.OPPOSING);

  protected Actor FindActorInPlayByDesignCode(string designCode, Player.Side side = Player.Side.NEUTRAL)
  {
    if (string.IsNullOrEmpty(designCode))
      return (Actor) null;
    List<Player> playerList = new List<Player>();
    GameState gameState = GameState.Get();
    switch (side)
    {
      case Player.Side.NEUTRAL:
        playerList.Add(gameState.GetFriendlySidePlayer());
        playerList.Add(gameState.GetOpposingSidePlayer());
        break;
      case Player.Side.FRIENDLY:
        playerList.Add(gameState.GetFriendlySidePlayer());
        break;
      case Player.Side.OPPOSING:
        playerList.Add(gameState.GetOpposingSidePlayer());
        break;
    }
    foreach (Player player in playerList)
    {
      Zone battlefieldZone = (Zone) player.GetBattlefieldZone();
      if ((UnityEngine.Object) battlefieldZone != (UnityEngine.Object) null)
      {
        foreach (Card card in battlefieldZone.GetCards())
        {
          if (card.GetEntity().GetCardId() == designCode)
            return card.GetActor();
        }
      }
      Zone heroZone = (Zone) player.GetHeroZone();
      if ((UnityEngine.Object) heroZone != (UnityEngine.Object) null)
      {
        foreach (Card card in heroZone.GetCards())
        {
          if (card.GetEntity().GetCardId() == designCode)
            return card.GetActor();
        }
      }
      Card weaponCard = player.GetWeaponCard();
      if ((UnityEngine.Object) weaponCard != (UnityEngine.Object) null && weaponCard.GetEntity().GetCardId() == designCode)
        return weaponCard.GetActor();
      Card heroPowerCard = player.GetHeroPowerCard();
      if ((UnityEngine.Object) heroPowerCard != (UnityEngine.Object) null && heroPowerCard.GetEntity().GetCardId() == designCode)
        return heroPowerCard.GetActor();
    }
    return (Actor) null;
  }

  protected class EmoteResponse
  {
    public string m_soundName;
    public string m_stringTag;
  }

  protected class EmoteResponseGroup
  {
    public List<EmoteType> m_triggers = new List<EmoteType>();
    public List<MissionEntity.EmoteResponse> m_responses = new List<MissionEntity.EmoteResponse>();
    public int m_responseIndex;
  }

  protected enum ShouldPlayValue
  {
    Never,
    Once,
    Always,
  }

  protected delegate MissionEntity.ShouldPlayValue ShouldPlay();

  public delegate IEnumerator OnChangeHandler(TAG_PLAYSTATE gameResult);
}
