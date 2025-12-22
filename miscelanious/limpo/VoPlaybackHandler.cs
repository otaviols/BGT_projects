using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoPlaybackHandler
{
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
  protected List<VoPlaybackHandler.EmoteResponseGroup> m_emoteResponseGroups = new List<VoPlaybackHandler.EmoteResponseGroup>();
  protected Notification m_ActiveSpeechBubble;
  private GameEntity m_GameEntity;
  private MonoBehaviour m_coroutines;
  public const int InGame_BossAttacks = 500;
  public const int InGame_BossAttacksSpecial = 501;
  public const int InGame_BossUsesHeroPower = 510;
  public const int InGame_BossUsesHeroPowerSpecial = 511;
  public const int InGame_BossEquipWeapon = 513;
  public const int InGame_BossDeath = 516;
  public const int InGame_PlayerAttacks = 502;
  public const int InGame_PlayerAttacksSpecial = 503;
  public const int InGame_PlayerUsesHeroPower = 508;
  public const int InGame_PlayerUsesHeroPowerSpecial = 509;
  public const int InGame_PlayerEquipWeapon = 512;
  public const int InGame_PlayerIdle = 518;
  public const int InGame_PlayerDeath = 519;
  public const int InGame_VictoryPreExplosion = 504;
  public const int InGame_VictoryPostExplosion = 505;
  public const int InGame_LossPreExplosion = 506;
  public const int InGame_LossPostExplosion = 507;
  public const int InGame_EmoteResponse = 515;
  public const int TurnOffBossExplodingOnDeath = 600;
  public const int TurnOffPlayerExplodingOnDeath = 601;
  public const int DisableAutomaticVO = 602;
  public const int EnableAutomaticVO = 603;
  public const int TurnOnBossExplodingOnDeath = 610;
  public const int TurnOnPlayerExplodingOnDeath = 611;
  public const int DoEmoteDrivenStart = 612;
  public const int PlayNextPlayerLine = 1000;
  public const int PlayRepeatPlayerLine = 1001;
  public const int PlayNextBossLine = 1002;
  public const int PlayRepeatBossLine = 1003;
  public const int ToggleAlwaysPlayLines = 1010;
  public const int PlayAllVOLines = 1011;
  public const int PlayAllBossVOLines = 1012;
  public const int PlayAllPlayerVOLines = 1013;
  public const int HearthStoneUsed = 58023;
  public int m_PlayPlayerVOLineIndex;
  public int m_PlayBossVOLineIndex;
  public List<string> m_PlayerVOLines = new List<string>();
  public List<string> m_BossVOLines = new List<string>();
  public string m_introLine;
  public string m_deathLine;
  public string m_standardEmoteResponseLine;
  public List<string> m_BossIdleLines;
  public List<string> m_BossIdleLinesCopy;
  public float m_lastVOplayFinshtime;
  public float m_BanterVOSilenceTime = 2f;
  private HashSet<string> m_InOrderPlayedLines = new HashSet<string>();
  public const bool ShowSpeechBubbleTrue = true;
  public const bool ShowSpeechBubbleFalse = false;
  public const bool PlayLinesRandomOrder = true;
  public const bool PlayLinesInOrder = false;
  public const int InGame_Introduction = 514;
  public bool m_MissionDisableAutomaticVO;
  public bool m_forceAlwaysPlayLine;
  public const int InGame_BossIdle = 517;
  private Notification.SpeechBubbleDirection m_LettuceMinionSpeakingDirection = Notification.SpeechBubbleDirection.BottomLeft;

  protected GameEntity GameEntity
  {
    get
    {
      if (this.m_GameEntity == null)
        this.m_GameEntity = GameState.Get().GetGameEntity();
      return this.m_GameEntity;
    }
  }

  public MonoBehaviour Coroutines
  {
    get => this.m_coroutines;
    set => this.m_coroutines = value;
  }

  public virtual IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    yield break;
  }

  public virtual IEnumerator RespondToWillPlayCardWithTiming(
    string cardId,
    Entity playedEntity)
  {
    yield break;
  }

  public virtual IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    yield break;
  }

  public virtual IEnumerator HandleGameOverWithTiming(
    TAG_PLAYSTATE gameResult,
    MissionEntity.OnChangeHandler fallback = null)
  {
    if (fallback != null)
      yield return (object) fallback(gameResult);
  }

  public virtual IEnumerator RespondToResetGameFinishedWithTiming(Entity source)
  {
    yield break;
  }

  public virtual IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    if (!this.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.OPPOSING)
    {
      this.OnBossHeroPowerPlayed(entity);
      yield break;
    }
  }

  public virtual IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    while (this.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (this.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor enemyActorInPlay = this.FindRandomEnemyActorInPlay();
      Actor enemyActor = this.FindRandomFriendlyActorInPlay();
      GameEntity gameEntity = GameState.Get().GetGameEntity();
      gameEntity.GetTag(GAME_TAG.TURN);
      gameEntity.GetTag(GAME_TAG.EXTRA_TURNS_TAKEN_THIS_GAME);
      switch (missionEvent)
      {
        case 602:
          this.m_MissionDisableAutomaticVO = true;
          break;
        case 603:
          this.m_MissionDisableAutomaticVO = false;
          break;
        case 1000:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (this.m_PlayPlayerVOLineIndex + 1 >= this.m_PlayerVOLines.Count)
            this.m_PlayPlayerVOLineIndex = 0;
          else
            ++this.m_PlayPlayerVOLineIndex;
          SceneDebugger.Get().AddMessage(this.m_PlayerVOLines[this.m_PlayPlayerVOLineIndex]);
          yield return (object) this.MissionPlayVO(enemyActorInPlay, this.m_PlayerVOLines[this.m_PlayPlayerVOLineIndex]);
          break;
        case 1001:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(this.m_PlayerVOLines[this.m_PlayPlayerVOLineIndex]);
          yield return (object) this.MissionPlayVO(enemyActorInPlay, this.m_PlayerVOLines[this.m_PlayPlayerVOLineIndex]);
          break;
        case 1002:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          if (this.m_PlayBossVOLineIndex + 1 >= this.m_BossVOLines.Count)
            this.m_PlayBossVOLineIndex = 0;
          else
            ++this.m_PlayBossVOLineIndex;
          SceneDebugger.Get().AddMessage(this.m_BossVOLines[this.m_PlayBossVOLineIndex]);
          yield return (object) this.MissionPlayVO(enemyActor, this.m_BossVOLines[this.m_PlayBossVOLineIndex]);
          break;
        case 1003:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          SceneDebugger.Get().AddMessage(this.m_BossVOLines[this.m_PlayBossVOLineIndex]);
          yield return (object) this.MissionPlayVO(enemyActor, this.m_BossVOLines[this.m_PlayBossVOLineIndex]);
          break;
        case 1010:
          this.m_forceAlwaysPlayLine = !this.m_forceAlwaysPlayLine;
          break;
        case 1011:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in this.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) this.MissionPlayVO(enemyActor, bossVoLine);
          }
          foreach (string playerVoLine in this.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) this.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
        case 1012:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string bossVoLine in this.m_BossVOLines)
          {
            SceneDebugger.Get().AddMessage(bossVoLine);
            yield return (object) this.MissionPlayVO(enemyActor, bossVoLine);
          }
          break;
        case 1013:
          GameState.Get().GetGameEntity().SetTag(GAME_TAG.MISSION_EVENT, 0);
          foreach (string playerVoLine in this.m_PlayerVOLines)
          {
            SceneDebugger.Get().AddMessage(playerVoLine);
            yield return (object) this.MissionPlayVO(enemyActor, playerVoLine);
          }
          break;
      }
    }
  }

  public virtual void NotifyOfEntityAttacked(Entity attacker, Entity defender)
  {
  }

  public virtual void NotifyOfMinionPlayed(Entity minion)
  {
  }

  public virtual void NotifyOfHeroChanged(Entity newHero)
  {
  }

  public virtual void NotifyOfWeaponEquipped(Entity weapon)
  {
  }

  public virtual void NotifyOfSpellPlayed(Entity spell, Entity target)
  {
  }

  public virtual void NotifyOfHeroPowerUsed(Entity heroPower, Entity target)
  {
  }

  public virtual void NotifyOfMinionDied(Entity minion)
  {
  }

  public virtual void NotifyOfHeroDied(Entity hero)
  {
  }

  public virtual void NotifyOfWeaponDestroyed(Entity weapon)
  {
  }

  public virtual void PreloadAssets()
  {
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
    if ((bool) (Object) actor && (Object) MulliganManager.Get() != (Object) null && MulliganManager.Get().IsMulliganActive() && !MulliganManager.Get().IsCustomIntroActive() && actor.GetEntity() != null && actor.GetEntity().IsHero())
    {
      iTween.StopByName(MulliganManager.Get().gameObject, this.GetMulliganHeroFadeItweenName(actor));
      GameState.Get().GetGameEntity().FadeInHeroActor(actor);
    }
    yield return (object) this.Coroutines.StartCoroutine(this.PlaySoundAndWait(soundPath, legacyAssetName, direction, actor, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, testingDuration, bubbleScale));
    if ((bool) (Object) actor && (Object) MulliganManager.Get() != (Object) null && MulliganManager.Get().IsMulliganActive() && !MulliganManager.Get().IsCustomIntroActive() && actor.GetEntity() != null && actor.GetEntity().IsHero())
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
      yield return (object) this.Coroutines.StartCoroutine(this.PlaySoundAndWait(soundPath, legacyAssetName, direction, actor, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, testingDuration, bubbleScale));
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
    AudioSource source = (AudioSource) null;
    bool isJustTesting = false;
    if (string.IsNullOrEmpty(soundPath) || !GameState.Get().GetGameEntity().CheckPreloadedSound(soundPath))
      isJustTesting = true;
    else
      source = GameState.Get().GetGameEntity().GetPreloadedSound(soundPath);
    if (!isJustTesting && ((Object) source == (Object) null || (Object) source.clip == (Object) null))
    {
      if (this.GameEntity.CheckPreloadedSound(soundPath))
      {
        this.GameEntity.RemovePreloadedSound(soundPath);
        this.GameEntity.PreloadSound(soundPath);
        while (this.GameEntity.IsPreloadingAssets())
          yield return (object) null;
        source = this.GameEntity.GetPreloadedSound(soundPath);
      }
      if ((Object) source == (Object) null || (Object) source.clip == (Object) null)
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
      this.Coroutines.StartCoroutine(this.WaitForCardSoundSpellDelay(num));
    if ((Object) actor != (Object) null && direction != Notification.SpeechBubbleDirection.None)
    {
      this.m_ActiveSpeechBubble = this.ShowBubble(gameString, direction, actor, false, num, parentBubbleToActor, bubbleScale);
      seconds += 0.5f;
    }
    yield return (object) new WaitForSeconds(seconds);
  }

  protected IEnumerator WaitForCardSoundSpellDelay(float sec)
  {
    this.GameEntity.GetGameOptions().SetBooleanOption(GameEntityOption.DELAY_CARD_SOUND_SPELLS, true);
    yield return (object) new WaitForSeconds(sec);
    this.GameEntity.GetGameOptions().SetBooleanOption(GameEntityOption.DELAY_CARD_SOUND_SPELLS, false);
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
    if ((Object) speakingActor == (Object) null)
      return (Notification) null;
    NotificationManager notificationManager = NotificationManager.Get();
    Notification speechBubble = notificationManager.CreateSpeechBubble(GameStrings.Get(textKey), direction, speakingActor, destroyOnNewNotification, parentToActor, bubbleScale);
    if ((double) duration > 0.0)
      notificationManager.DestroyNotification(speechBubble, duration);
    return speechBubble;
  }

  protected VoPlaybackHandler.ShouldPlayValue InternalShouldPlayOnlyOnce() => VoPlaybackHandler.ShouldPlayValue.Once;

  protected Notification.SpeechBubbleDirection GetDirection(Actor actor) => actor.GetEntity().IsControlledByFriendlySidePlayer() ? Notification.SpeechBubbleDirection.BottomLeft : Notification.SpeechBubbleDirection.TopRight;

  protected string GetMulliganHeroFadeItweenName(Actor actor) => actor.GetEntity().IsControlledByFriendlySidePlayer() ? "MyHeroLightBlend" : "HisHeroLightBlend";

  protected IEnumerator PlayLine(
    Actor speaker,
    string line,
    VoPlaybackHandler.ShouldPlay shouldPlay,
    float duration)
  {
    if (!this.m_enemySpeaking)
    {
      this.m_enemySpeaking = true;
      Notification.SpeechBubbleDirection direction = this.GetDirection(speaker);
      if (this.m_forceAlwaysPlayLine)
        yield return (object) this.Coroutines.StartCoroutine(this.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
      else if (shouldPlay() == VoPlaybackHandler.ShouldPlayValue.Always)
        yield return (object) this.Coroutines.StartCoroutine(this.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
      else if (shouldPlay() == VoPlaybackHandler.ShouldPlayValue.Once)
        yield return (object) this.Coroutines.StartCoroutine(this.PlaySoundAndBlockSpeechOnce(line, direction, speaker, duration));
      NotificationManager.Get().ForceAddSoundToPlayedList(line);
      this.m_enemySpeaking = false;
    }
  }

  public IEnumerator HandlePlayerEmoteWithTiming(
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
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!VoPlaybackHandler.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
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
      if ((Object) battlefieldZone != (Object) null)
      {
        foreach (Card card in battlefieldZone.GetCards())
        {
          if (card.GetEntity().GetCardId() == designCode)
            return card.GetActor();
        }
      }
      Zone heroZone = (Zone) player.GetHeroZone();
      if ((Object) heroZone != (Object) null)
      {
        foreach (Card card in heroZone.GetCards())
        {
          if (card.GetEntity().GetCardId() == designCode)
            return card.GetActor();
        }
      }
      Card weaponCard = player.GetWeaponCard();
      if ((Object) weaponCard != (Object) null && weaponCard.GetEntity().GetCardId() == designCode)
        return weaponCard.GetActor();
      Card heroPowerCard = player.GetHeroPowerCard();
      if ((Object) heroPowerCard != (Object) null && heroPowerCard.GetEntity().GetCardId() == designCode)
        return heroPowerCard.GetActor();
    }
    return (Actor) null;
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

  public virtual void OnCreateGame()
  {
    this.m_introLine = (string) null;
    this.m_deathLine = (string) null;
    this.m_standardEmoteResponseLine = (string) null;
    this.m_BossIdleLines = new List<string>((IEnumerable<string>) this.GetIdleLines());
    this.m_BossIdleLinesCopy = new List<string>((IEnumerable<string>) this.GetIdleLines());
  }

  protected IEnumerator MissionPlayVO(
    Actor actor,
    string line,
    bool bUseBubble,
    VoPlaybackHandler.ShouldPlay shouldPlay)
  {
    if (!((Object) actor == (Object) null) && line != null)
    {
      Notification.SpeechBubbleDirection speakerDirection = this.m_LettuceMinionSpeakingDirection;
      if (this.m_forceAlwaysPlayLine)
        yield return (object) this.Coroutines.StartCoroutine(this.PlayLine(actor, line, shouldPlay, 2.5f));
      bool parentBubbleToActor = !((Object) actor.GetCard() != (Object) null) || actor.GetCard().GetEntity() == null || !actor.GetCard().GetEntity().IsHeroPower();
      if (shouldPlay() == this.InternalShouldPlayAlways())
        yield return (object) this.Coroutines.StartCoroutine(this.PlaySoundAndBlockSpeech(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
      else if (shouldPlay() == this.InternalShouldPlayOnlyOnce())
      {
        yield return (object) this.Coroutines.StartCoroutine(this.PlaySoundAndBlockSpeechOnce(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
        NotificationManager.Get().ForceAddSoundToPlayedList(line);
      }
      this.m_lastVOplayFinshtime = Time.time;
    }
  }

  public IEnumerator MissionPlayVO(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    VoPlaybackHandler voPlaybackHandler = this;
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
    this.\u003C\u003E2__current = (object) voPlaybackHandler.MissionPlayVO(actor, line, true, new VoPlaybackHandler.ShouldPlay(voPlaybackHandler.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    VoPlaybackHandler voPlaybackHandler = this;
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
    this.\u003C\u003E2__current = (object) voPlaybackHandler.MissionPlayVO(actor, line, true, new VoPlaybackHandler.ShouldPlay(voPlaybackHandler.InternalShouldPlayOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator MissionPlayVO(
    Actor speaker,
    List<string> lines,
    VoPlaybackHandler.ShouldPlay shouldPlay,
    bool bUseBubble = true,
    bool bPlayOrder = true)
  {
    bool removeLine = false;
    if (shouldPlay() == VoPlaybackHandler.ShouldPlayValue.Once && !this.m_forceAlwaysPlayLine)
      removeLine = true;
    string line = !bPlayOrder ? this.PopNextLine(lines, removeLine) : this.PopRandomLine(lines, removeLine);
    if (line != null)
      yield return (object) this.MissionPlayVO(speaker, line, bUseBubble, shouldPlay);
  }

  public IEnumerator MissionPlayVO(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    VoPlaybackHandler voPlaybackHandler = this;
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
    this.\u003C\u003E2__current = (object) voPlaybackHandler.MissionPlayVO(actor, lines, new VoPlaybackHandler.ShouldPlay(voPlaybackHandler.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlaySound(string line)
  {
    float waitTimeScale = 0.0f;
    bool parentBubbleToActor = true;
    bool delayCardSoundSpells = false;
    yield return (object) this.Coroutines.StartCoroutine(this.PlaySoundAndWait(line, (string) null, Notification.SpeechBubbleDirection.None, (Actor) null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells));
  }

  public IEnumerator MissionPlaySound(Actor actor, string line)
  {
    yield return (object) this.MissionPlaySound(line);
  }

  protected VoPlaybackHandler.ShouldPlayValue InternalShouldPlayAlways() => VoPlaybackHandler.ShouldPlayValue.Always;

  protected VoPlaybackHandler.ShouldPlayValue InternalShouldPlayOnce() => VoPlaybackHandler.ShouldPlayValue.Once;

  protected string PopNextLine(List<string> lines, bool removeLine = true)
  {
    string str = (string) null;
    for (int index = 0; index < lines.Count; ++index)
    {
      if (!this.m_InOrderPlayedLines.Contains(lines[index]))
      {
        str = lines[index];
        break;
      }
    }
    if (str == null)
      return (string) null;
    if (removeLine)
      this.m_InOrderPlayedLines.Add(str);
    return str;
  }

  protected string PopRandomLine(List<string> lines, bool removeLine = true)
  {
    if (lines == null)
      return (string) null;
    if (lines.Count == 0)
      return (string) null;
    string line = lines[Random.Range(0, lines.Count)];
    if (removeLine)
      lines.Remove(line);
    return line;
  }

  public IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    this.MissionPause(true);
    yield return (object) this.HandleMissionEventWithTiming(514);
    this.MissionPause(false);
  }

  public void MissionPause(bool pause)
  {
    this.m_MissionDisableAutomaticVO = pause;
    GameState.Get().SetBusy(pause);
  }

  protected Actor FindRandomEnemyActorInPlay()
  {
    using (List<Card>.Enumerator enumerator = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards().GetEnumerator())
    {
      if (enumerator.MoveNext())
        return enumerator.Current.GetActor();
    }
    return (Actor) null;
  }

  protected Actor FindRandomFriendlyActorInPlay()
  {
    using (List<Card>.Enumerator enumerator = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().GetEnumerator())
    {
      if (enumerator.MoveNext())
        return enumerator.Current.GetActor();
    }
    return (Actor) null;
  }

  public IEnumerator UnblockSpeechAgainAfterDuration(float durationInSeconds)
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

  protected class EmoteResponse
  {
    public string m_soundName;
    public string m_stringTag;
  }

  protected class EmoteResponseGroup
  {
    public List<EmoteType> m_triggers = new List<EmoteType>();
    public List<VoPlaybackHandler.EmoteResponse> m_responses = new List<VoPlaybackHandler.EmoteResponse>();
    public int m_responseIndex;
  }

  protected enum ShouldPlayValue
  {
    Never,
    Once,
    Always,
  }

  protected delegate VoPlaybackHandler.ShouldPlayValue ShouldPlay();
}
