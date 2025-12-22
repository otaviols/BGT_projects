using Blizzard.T5.Core;
using Hearthstone.Progression;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class TB_MagicalGuardians_MissionEntity : GenericDungeonMissionEntity
{
  public bool m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_EnemyPlayIdleLines = true;
  public bool m_Mission_EnemyPlayIdleLinesUseingEmoteSystem;
  public bool m_Mission_EnemyPlayIdleLinesInOrder = true;
  public bool m_Mission_EnemyPlayHeroPowerLines;
  public bool m_Mission_EnemyPlayHeroPowerLinesInOrder;
  public bool m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_FriendlyPlayIdleLines = true;
  public bool m_Mission_FriendlyPlayIdleLinesUseingEmoteSystem;
  public bool m_Mission_FriendlylayIdleLinesInOrder = true;
  public bool m_Mission_FriendlyPlayHeroPowerLines;
  public bool m_Mission_FriendlyPlayHeroPowerLinesInOrder;
  public bool m_MissionDisableAutomaticVO;
  private HashSet<string> m_InOrderPlayedLines = new HashSet<string>();
  public List<string> m_BossVOLines = new List<string>();
  public List<string> m_PlayerVOLines = new List<string>();
  public List<string> m_BossIdleLines;
  public List<string> m_BossIdleLinesCopy;
  public int m_PlayPlayerVOLineIndex;
  public int m_PlayBossVOLineIndex;
  public string m_introLine;
  public string m_deathLine;
  public string m_standardEmoteResponseLine;
  public bool m_DoEmoteDrivenStart;
  public MusicPlaylistType m_OverrideMulliganMusicTrack;
  public MusicPlaylistType m_OverrideMusicTrack;
  public string m_OverrideBossSubtext;
  public string m_OverridePlayerSubtext;
  public bool m_SupressEnemyDeathTextBubble;
  private Spell m_enemyBlowUpSpell;
  private Spell m_friendlyBlowUpSpell;
  public const bool ShowSpeechBubbleTrue = true;
  public const bool ShowSpeechBubbleFalse = false;
  public const bool PlayLinesRandomOrder = true;
  public const bool PlayLinesInOrder = false;
  public float m_lastVOplayFinishtime;
  public float m_BanterVOSilenceTime = 2f;
  private static Map<GameEntityOption, bool> s_booleanOptions = TB_MagicalGuardians_MissionEntity.InitBooleanOptions();
  public const int InGame_BossAttacks = 500;
  public const int InGame_BossAttacksSpecial = 501;
  public const int InGame_BossUsesHeroPower = 510;
  public const int InGame_BossUsesHeroPowerSpecial = 511;
  public const int InGame_BossEquipWeapon = 513;
  public const int InGame_BossDeath = 516;
  public const int InGame_BossIdle = 517;
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
  public const int InGame_Introduction = 514;
  public const int InGame_EmoteResponse = 515;
  public const int InGame_AfterResetGame = 520;
  public const int InGame_AfterPuzzleComplete = 521;
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

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> collection = new List<string>();
    this.m_PlayerVOLines = new List<string>((IEnumerable<string>) collection);
    foreach (string soundPath in collection)
      this.PreloadSound(soundPath);
  }

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public TB_MagicalGuardians_MissionEntity() => this.m_gameOptions.AddBooleanOptions(TB_MagicalGuardians_MissionEntity.s_booleanOptions);

  public virtual List<string> GetBossIdleLines() => new List<string>();

  public virtual float GetThinkEmoteBossIdleChancePercentage() => 0.25f;

  public virtual float GetThinkIdleChancePercentage() => 0.25f;

  public virtual List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected virtual float ChanceToPlayBossHeroPowerVOLine() => 1f;

  public void MissionPause(bool pause)
  {
    this.m_MissionDisableAutomaticVO = pause;
    GameState.Get().SetBusy(pause);
  }

  protected virtual IEnumerator OnBossHeroPowerPlayed(Entity entity)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    float bossHeroPowerVoLine = guardiansMissionEntity.ChanceToPlayBossHeroPowerVOLine();
    float num = UnityEngine.Random.Range(0.0f, 1f);
    if (!guardiansMissionEntity.m_enemySpeaking && !guardiansMissionEntity.m_MissionDisableAutomaticVO && (double) bossHeroPowerVoLine >= (double) num && !((UnityEngine.Object) GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor() == (UnityEngine.Object) null))
      yield return (object) guardiansMissionEntity.HandleMissionEventWithTiming(510);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    if (!guardiansMissionEntity.m_MissionDisableAutomaticVO && !guardiansMissionEntity.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.FRIENDLY)
      yield return (object) guardiansMissionEntity.HandleMissionEventWithTiming(508);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    if (!guardiansMissionEntity.m_MissionDisableAutomaticVO && !guardiansMissionEntity.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.OPPOSING)
      yield return (object) guardiansMissionEntity.OnBossHeroPowerPlayed(entity);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType) || this.m_enemySpeaking)
      return;
    Gameplay.Get().StartCoroutine(this.PlayCustomEmoteResponse(emoteType, emoteSpell));
  }

  public virtual IEnumerator PlayCustomEmoteResponse(
    EmoteType emoteType,
    CardSoundSpell emoteSpell)
  {
    yield return (object) null;
  }

  protected IEnumerator DelayAndPlayInGameTrigger(int VOTriggerID)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    yield return (object) new WaitForSeconds(3f);
    yield return (object) guardiansMissionEntity.HandleMissionEventWithTiming(VOTriggerID);
  }

  public override bool ShouldShowHeroClassDuringMulligan(Player.Side playerSide) => false;

  public override void StartGameplaySoundtracks()
  {
    if (this.m_OverrideMusicTrack == MusicPlaylistType.Invalid)
      base.StartGameplaySoundtracks();
    else
      MusicManager.Get().StartPlaylist(this.m_OverrideMusicTrack);
  }

  public override void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      return;
    if (this.m_OverrideMulliganMusicTrack == MusicPlaylistType.Invalid)
      base.StartMulliganSoundtracks(soft);
    else
      MusicManager.Get().StartPlaylist(this.m_OverrideMulliganMusicTrack);
  }

  public override string GetNameBannerSubtextOverride(Player.Side playerSide)
  {
    if (playerSide == Player.Side.OPPOSING && this.m_OverrideBossSubtext != null)
      return GameStrings.Get(this.m_OverrideBossSubtext);
    return playerSide == Player.Side.FRIENDLY && this.m_OverridePlayerSubtext != null ? GameStrings.Get(this.m_OverridePlayerSubtext) : base.GetNameBannerSubtextOverride(playerSide);
  }

  public override void OnPlayThinkEmote() => Gameplay.Get().StartCoroutine(this.OnPlayThinkEmoteWithTiming());

  public override IEnumerator OnPlayThinkEmoteWithTiming()
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (!guardiansMissionEntity.m_enemySpeaking)
    {
      Player currentPlayer = GameState.Get().GetCurrentPlayer();
      if (currentPlayer.IsFriendlySide() && !currentPlayer.GetHeroCard().HasActiveEmoteSound())
      {
        GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
        if ((guardiansMissionEntity.m_Mission_FriendlyPlayIdleLines || guardiansMissionEntity.m_Mission_EnemyPlayIdleLines) && (double) guardiansMissionEntity.GetThinkIdleChancePercentage() >= (double) UnityEngine.Random.Range(0.0f, 1f))
        {
          if (((double) guardiansMissionEntity.GetThinkEmoteBossIdleChancePercentage() < (double) UnityEngine.Random.Range(0.0f, 1f) || !guardiansMissionEntity.m_Mission_FriendlyPlayIdleLines) && guardiansMissionEntity.m_Mission_EnemyPlayIdleLines)
          {
            if (guardiansMissionEntity.m_Mission_EnemyPlayIdleLinesUseingEmoteSystem)
              yield return (object) guardiansMissionEntity.MissionPlayThinkEmote(actor1);
            else
              yield return (object) GameEntity.Coroutines.StartCoroutine(guardiansMissionEntity.HandleMissionEventWithTiming(517));
          }
          else if (guardiansMissionEntity.m_Mission_FriendlyPlayIdleLines)
          {
            if (guardiansMissionEntity.m_Mission_FriendlyPlayIdleLinesUseingEmoteSystem)
              yield return (object) guardiansMissionEntity.MissionPlayThinkEmote(actor2);
            else
              yield return (object) GameEntity.Coroutines.StartCoroutine(guardiansMissionEntity.HandleMissionEventWithTiming(518));
          }
        }
      }
    }
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE playState)
  {
    PegCursor.Get().SetMode(PegCursor.Mode.STOPWAITING);
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_EndGameScreen);
    Card heroCard1 = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
    Card heroCard2 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    Gameplay.Get().SaveOriginalTimeScale();
    AchievementManager.Get()?.PauseToastNotifications();
    if (this.ShouldPlayHeroBlowUpSpells(playState))
    {
      switch (playState)
      {
        case TAG_PLAYSTATE.WON:
          string stringOption1 = this.GetGameOptions().GetStringOption(GameEntityOption.VICTORY_AUDIO_PATH);
          if (this.m_Mission_EnemyHeroShouldExplodeOnDefeat)
          {
            if (!string.IsNullOrEmpty(stringOption1))
              SoundManager.Get().LoadAndPlay((AssetReference) stringOption1);
            this.m_enemyBlowUpSpell = this.BlowUpHero(heroCard1, SpellType.ENDGAME_WIN);
            break;
          }
          break;
        case TAG_PLAYSTATE.LOST:
          string stringOption2 = this.GetGameOptions().GetStringOption(GameEntityOption.DEFEAT_AUDIO_PATH);
          if (this.m_Mission_FriendlyHeroShouldExplodeOnDefeat)
          {
            if (!string.IsNullOrEmpty(stringOption2))
              SoundManager.Get().LoadAndPlay((AssetReference) stringOption2);
            this.m_friendlyBlowUpSpell = this.BlowUpHero(heroCard2, SpellType.ENDGAME_LOSE);
            break;
          }
          break;
        case TAG_PLAYSTATE.TIED:
          string stringOption3 = this.GetGameOptions().GetStringOption(GameEntityOption.DEFEAT_AUDIO_PATH);
          if (!string.IsNullOrEmpty(stringOption3))
            SoundManager.Get().LoadAndPlay((AssetReference) stringOption3);
          if (this.m_Mission_EnemyHeroShouldExplodeOnDefeat)
            this.m_enemyBlowUpSpell = this.BlowUpHero(heroCard1, SpellType.ENDGAME_DRAW);
          if (this.m_Mission_FriendlyHeroShouldExplodeOnDefeat)
          {
            this.m_friendlyBlowUpSpell = this.BlowUpHero(heroCard2, SpellType.ENDGAME_LOSE);
            break;
          }
          break;
      }
    }
    this.ShowEndGameScreen(playState, this.m_enemyBlowUpSpell, this.m_friendlyBlowUpSpell);
    GameEntity.Coroutines.StartCoroutine(this.HandleGameOverWithTiming(playState));
  }

  protected override IEnumerator RespondToResetGameFinishedWithTiming(Entity entity)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    while (guardiansMissionEntity.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    if (gameEntity.GetTag(GAME_TAG.PREVIOUS_PUZZLE_COMPLETED) == 0)
    {
      guardiansMissionEntity.MissionPause(true);
      yield return (object) guardiansMissionEntity.HandleMissionEventWithTiming(520);
      guardiansMissionEntity.MissionPause(false);
    }
    if (gameEntity.GetTag(GAME_TAG.PREVIOUS_PUZZLE_COMPLETED) == 1)
    {
      guardiansMissionEntity.MissionPause(true);
      yield return (object) guardiansMissionEntity.HandleMissionEventWithTiming(521);
      guardiansMissionEntity.MissionPause(false);
    }
  }

  public static bool GetIsFirstBoss()
  {
    AdventureDataDbfRecord adventureDataRecord = TB_MagicalGuardians_MissionEntity.GetAdventureDataRecord(Options.Get().GetInt(Option.SELECTED_ADVENTURE), Options.Get().GetInt(Option.SELECTED_ADVENTURE_MODE));
    if (adventureDataRecord == null)
      return true;
    GameSaveKeyId saveDataServerKey = (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey;
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_RUN_ACTIVE, out num);
    return num == 0L;
  }

  public static AdventureDataDbfRecord GetAdventureDataRecord(
    int adventureId,
    int modeId)
  {
    foreach (AdventureDataDbfRecord record in GameDbf.AdventureData.GetRecords())
    {
      if (record.AdventureId == adventureId && record.ModeId == modeId)
        return record;
    }
    return (AdventureDataDbfRecord) null;
  }

  protected IEnumerator MissionPlayThinkEmote(Actor thinkingActor)
  {
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    EmoteType emoteType = EmoteType.THINK1;
    switch (UnityEngine.Random.Range(1, 4))
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
    AudioSource activeAudioSource;
    if ((UnityEngine.Object) thinkingActor == (UnityEngine.Object) actor1)
    {
      CardSoundSpell cardSoundSpell = GameState.Get().GetOpposingSidePlayer().GetHeroCard().PlayEmote(emoteType);
      if (!((UnityEngine.Object) cardSoundSpell == (UnityEngine.Object) null))
      {
        activeAudioSource = cardSoundSpell.GetActiveAudioSource();
        yield return (object) GameState.Get().GetOpposingSidePlayer().GetHeroCard().PlayEmote(emoteType);
        yield return (object) new WaitForSeconds(activeAudioSource.clip.length);
        activeAudioSource = (AudioSource) null;
      }
    }
    else if ((UnityEngine.Object) thinkingActor == (UnityEngine.Object) actor2)
    {
      CardSoundSpell cardSoundSpell = GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(emoteType);
      if (!((UnityEngine.Object) cardSoundSpell == (UnityEngine.Object) null))
      {
        activeAudioSource = cardSoundSpell.GetActiveAudioSource();
        yield return (object) GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(emoteType);
        yield return (object) new WaitForSeconds(activeAudioSource.clip.length);
        activeAudioSource = (AudioSource) null;
      }
    }
  }

  protected string PopRandomLine(List<string> lines, bool removeLine = true)
  {
    if (lines == null)
      return (string) null;
    if (lines.Count == 0)
      return (string) null;
    string line = lines[UnityEngine.Random.Range(0, lines.Count)];
    if (removeLine)
      lines.Remove(line);
    return line;
  }

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

  public void SetBossVOLines(List<string> VOLines) => this.m_BossVOLines = new List<string>((IEnumerable<string>) VOLines);

  protected MissionEntity.ShouldPlayValue InternalShouldPlayAlways() => MissionEntity.ShouldPlayValue.Always;

  protected MissionEntity.ShouldPlayValue InternalShouldPlayOnce() => MissionEntity.ShouldPlayValue.Once;

  public bool shouldPlayBanterVO() => (double) Time.time > (double) (this.m_lastVOplayFinishtime + this.m_BanterVOSilenceTime);

  protected virtual bool GetShouldSuppressDeathTextBubble() => this.m_SupressEnemyDeathTextBubble;

  protected IEnumerator MissionPlayVO(
    Actor actor,
    string line,
    bool bUseBubble,
    MissionEntity.ShouldPlay shouldPlay)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    if (!((UnityEngine.Object) actor == (UnityEngine.Object) null) && line != null)
    {
      Notification.SpeechBubbleDirection speakerDirection = guardiansMissionEntity.GetDirection(actor);
      if (guardiansMissionEntity.m_forceAlwaysPlayLine)
        yield return (object) GameEntity.Coroutines.StartCoroutine(guardiansMissionEntity.PlayLine(actor, line, shouldPlay, 2.5f));
      bool parentBubbleToActor = !((UnityEngine.Object) actor.GetCard() != (UnityEngine.Object) null) || actor.GetCard().GetEntity() == null || !actor.GetCard().GetEntity().IsHeroPower();
      if (shouldPlay() == guardiansMissionEntity.InternalShouldPlayAlways())
        yield return (object) GameEntity.Coroutines.StartCoroutine(guardiansMissionEntity.PlaySoundAndBlockSpeech(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
      else if (shouldPlay() == guardiansMissionEntity.InternalShouldPlayOnlyOnce())
      {
        yield return (object) GameEntity.Coroutines.StartCoroutine(guardiansMissionEntity.PlaySoundAndBlockSpeechOnce(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
        NotificationManager.Get().ForceAddSoundToPlayedList(line);
      }
    }
  }

  public IEnumerator MissionPlayVOSound(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(actor, line, false, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator MissionPlayVO(
    Actor speaker,
    List<string> lines,
    MissionEntity.ShouldPlay shouldPlay,
    bool bUseBubble = true,
    bool bPlayOrder = true)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    bool removeLine = false;
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Once && !guardiansMissionEntity.m_forceAlwaysPlayLine)
      removeLine = true;
    string line = !bPlayOrder ? guardiansMissionEntity.PopNextLine(lines, removeLine) : guardiansMissionEntity.PopRandomLine(lines, removeLine);
    if (line != null)
    {
      yield return (object) guardiansMissionEntity.MissionPlayVO(speaker, line, bUseBubble, shouldPlay);
      guardiansMissionEntity.m_lastVOplayFinishtime = Time.time;
    }
  }

  public IEnumerator MissionPlayVO(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(((MissionEntity) guardiansMissionEntity).InternalShouldPlayOnlyOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOInOrder(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnceInOrder(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(((MissionEntity) guardiansMissionEntity).InternalShouldPlayOnlyOnce), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator MissionPlayVO(
    string brassRing,
    string line,
    bool bUseBubble,
    MissionEntity.ShouldPlay shouldPlay)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    if (guardiansMissionEntity.m_enemySpeaking)
      yield return (object) null;
    guardiansMissionEntity.m_enemySpeaking = true;
    if (guardiansMissionEntity.m_forceAlwaysPlayLine)
      yield return (object) GameEntity.Coroutines.StartCoroutine(guardiansMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
      yield return (object) GameEntity.Coroutines.StartCoroutine(guardiansMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    else if (shouldPlay() == MissionEntity.ShouldPlayValue.Once)
    {
      yield return (object) GameEntity.Coroutines.StartCoroutine(guardiansMissionEntity.PlayBigCharacterQuoteAndWaitOnce(brassRing, line));
      NotificationManager.Get().ForceAddSoundToPlayedList(line);
    }
    guardiansMissionEntity.m_enemySpeaking = false;
  }

  protected IEnumerator MissionPlayVO(
    AssetReference brassRing,
    List<string> lines,
    MissionEntity.ShouldPlay shouldPlay,
    bool bUseBubble = true,
    bool bPlayOrder = true)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    bool removeLine = false;
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Once && !guardiansMissionEntity.m_forceAlwaysPlayLine)
      removeLine = true;
    string line = !bPlayOrder ? guardiansMissionEntity.PopRandomLine(lines, removeLine) : guardiansMissionEntity.PopNextLine(lines, removeLine);
    yield return (object) guardiansMissionEntity.MissionPlayVO((string) brassRing, line, bUseBubble, shouldPlay);
  }

  protected IEnumerator MissionPlayVO(
    AssetReference brassRing,
    List<string> lines,
    Actor minionOverride,
    MissionEntity.ShouldPlay shouldPlay,
    bool bUseBubble = true,
    bool bPlayOrder = true)
  {
    if ((UnityEngine.Object) minionOverride == (UnityEngine.Object) null)
      yield return (object) this.MissionPlayVO(brassRing, lines, shouldPlay, bUseBubble, bPlayOrder);
    else
      yield return (object) this.MissionPlayVO(minionOverride, lines, shouldPlay, bUseBubble, bPlayOrder);
  }

  public IEnumerator MissionPlayVO(AssetReference brassRing, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(AssetReference brassRing, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(AssetReference brassRing, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(AssetReference brassRing, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(((MissionEntity) guardiansMissionEntity).InternalShouldPlayOnlyOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOInOrder(AssetReference brassRing, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnceInOrder(
    AssetReference brassRing,
    List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(((MissionEntity) guardiansMissionEntity).InternalShouldPlayOnlyOnce), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOInOrder(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(string minionSpeaker, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), lines, new MissionEntity.ShouldPlay(((MissionEntity) guardiansMissionEntity).InternalShouldPlayOnlyOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(string minionSpeaker, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnceInOrder(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), lines, new MissionEntity.ShouldPlay(((MissionEntity) guardiansMissionEntity).InternalShouldPlayOnlyOnce), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOInOrder(
    string minionSpeaker,
    AssetReference brassRing,
    List<string> lines)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    if ((UnityEngine.Object) guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker) == (UnityEngine.Object) null)
      yield return (object) guardiansMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways), false);
    else
      yield return (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways), false);
  }

  public IEnumerator MissionPlayVOOnce(
    string minionSpeaker,
    AssetReference brassRing,
    string line)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    if ((UnityEngine.Object) guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker) == (UnityEngine.Object) null)
      yield return (object) guardiansMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayOnce));
    else
      yield return (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayOnce));
  }

  public IEnumerator MissionPlayVO(
    string minionSpeaker,
    AssetReference brassRing,
    List<string> lines)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    if ((UnityEngine.Object) guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker) == (UnityEngine.Object) null)
      yield return (object) guardiansMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways), false);
    else
      yield return (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
  }

  public IEnumerator MissionPlayVOOnce(
    string minionSpeaker,
    AssetReference brassRing,
    List<string> lines)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    if ((UnityEngine.Object) guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker) == (UnityEngine.Object) null)
      yield return (object) guardiansMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways), false);
    else
      yield return (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), lines, new MissionEntity.ShouldPlay(((MissionEntity) guardiansMissionEntity).InternalShouldPlayOnlyOnce));
  }

  public IEnumerator MissionPlayVO(
    string minionSpeaker,
    AssetReference brassRing,
    string line)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    if ((UnityEngine.Object) guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker) == (UnityEngine.Object) null)
      yield return (object) guardiansMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
    else
      yield return (object) guardiansMissionEntity.MissionPlayVO(guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), line, true, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways));
  }

  public IEnumerator MissionPlayVOOnceInOrder(
    string minionSpeaker,
    AssetReference brassRing,
    List<string> lines)
  {
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
    Actor playByDesignCode = guardiansMissionEntity.FindActorInPlayByDesignCode(minionSpeaker);
    if ((UnityEngine.Object) playByDesignCode == (UnityEngine.Object) null)
      yield return (object) guardiansMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(guardiansMissionEntity.InternalShouldPlayAlways), false);
    else
      yield return (object) guardiansMissionEntity.MissionPlayVO(playByDesignCode, lines, new MissionEntity.ShouldPlay(((MissionEntity) guardiansMissionEntity).InternalShouldPlayOnlyOnce), false);
  }

  public IEnumerator MissionPlaySound(string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_MagicalGuardians_MissionEntity guardiansMissionEntity = this;
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
    float waitTimeScale = 0.0f;
    bool parentBubbleToActor = true;
    bool delayCardSoundSpells = false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) GameEntity.Coroutines.StartCoroutine(guardiansMissionEntity.PlaySoundAndWait(line, (string) null, Notification.SpeechBubbleDirection.None, (Actor) null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlaySound(Actor actor, string line)
  {
    yield return (object) this.MissionPlaySound(line);
  }

  public IEnumerator MissionPlaySound(AssetReference brassRing, string line)
  {
    yield return (object) this.MissionPlaySound(line);
  }

  public IEnumerator MissionPlaySound(string minionSpeaker, string line)
  {
    yield return (object) this.MissionPlaySound(line);
  }

  public IEnumerator MissionPlaySound(
    string minionSpeaker,
    AssetReference brassRing,
    string line)
  {
    yield return (object) this.MissionPlaySound(line);
  }

  public IEnumerator MissionPlaySound(List<string> lines)
  {
    yield return (object) this.MissionPlaySound(this.PopRandomLine(lines));
  }

  public IEnumerator MissionPlaySound(string minionSpeaker, List<string> lines)
  {
    yield return (object) this.MissionPlaySound(lines);
  }

  public IEnumerator MissionPlaySound(
    string minionSpeaker,
    AssetReference brassRing,
    List<string> lines)
  {
    yield return (object) this.MissionPlaySound(lines);
  }

  public IEnumerator MissionPlaySound(Actor actor, List<string> lines)
  {
    yield return (object) this.MissionPlaySound(lines);
  }

  public IEnumerator MissionPlaySound(AssetReference brassRing, List<string> lines)
  {
    yield return (object) this.MissionPlaySound(lines);
  }

  public static class MemberInfoGetting
  {
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression) => ((MemberExpression) memberExpression.Body).Member.Name;
  }
}
