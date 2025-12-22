using Blizzard.T5.Core;
using Hearthstone.Progression;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class BOM_10_Dawngrasp_MissionEntity : GenericDungeonMissionEntity
{
  public bool m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_EnemyPlayIdleLines = true;
  public bool m_Mission_EnemyPlayIdleLinesUseingEmoteSystem;
  public bool m_Mission_EnemyPlayIdleLinesInOrder = true;
  public bool m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_FriendlyPlayIdleLines = true;
  public bool m_Mission_FriendlyPlayIdleLinesUseingEmoteSystem;
  public bool m_Mission_FriendlylayIdleLinesInOrder = true;
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
  public float m_lastVOplayFinishtime;
  public float m_BanterVOSilenceTime = 2f;
  private static Map<GameEntityOption, bool> s_booleanOptions = BOM_10_Dawngrasp_MissionEntity.InitBooleanOptions();

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

  public BOM_10_Dawngrasp_MissionEntity() => this.m_gameOptions.AddBooleanOptions(BOM_10_Dawngrasp_MissionEntity.s_booleanOptions);

  public virtual List<string> GetBossIdleLines() => new List<string>();

  public virtual float GetThinkEmoteBossIdleChancePercentage() => 0.25f;

  public virtual float GetThinkIdleChancePercentage() => 0.25f;

  protected virtual float ChanceToPlayBossHeroPowerVOLine() => 1f;

  public void MissionPause(bool pause)
  {
    this.m_MissionDisableAutomaticVO = pause;
    GameState.Get().SetBusy(pause);
  }

  protected virtual IEnumerator OnBossHeroPowerPlayed(Entity entity)
  {
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
    float bossHeroPowerVoLine = dawngraspMissionEntity.ChanceToPlayBossHeroPowerVOLine();
    float num = UnityEngine.Random.Range(0.0f, 1f);
    if (!dawngraspMissionEntity.m_enemySpeaking && !dawngraspMissionEntity.m_MissionDisableAutomaticVO && (double) bossHeroPowerVoLine >= (double) num && !((UnityEngine.Object) GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor() == (UnityEngine.Object) null))
      yield return (object) dawngraspMissionEntity.HandleMissionEventWithTiming(510);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
    if (!dawngraspMissionEntity.m_MissionDisableAutomaticVO && !dawngraspMissionEntity.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.FRIENDLY)
      yield return (object) dawngraspMissionEntity.HandleMissionEventWithTiming(508);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
    if (!dawngraspMissionEntity.m_MissionDisableAutomaticVO && !dawngraspMissionEntity.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.OPPOSING)
      yield return (object) dawngraspMissionEntity.OnBossHeroPowerPlayed(entity);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType) || this.m_enemySpeaking)
      return;
    GameEntity.Coroutines.StartCoroutine(this.HandleMissionEventWithTiming(515));
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
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (!dawngraspMissionEntity.m_enemySpeaking)
    {
      Player currentPlayer = GameState.Get().GetCurrentPlayer();
      if (currentPlayer.IsFriendlySide() && !currentPlayer.GetHeroCard().HasActiveEmoteSound())
      {
        GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
        if ((dawngraspMissionEntity.m_Mission_FriendlyPlayIdleLines || dawngraspMissionEntity.m_Mission_EnemyPlayIdleLines) && (double) dawngraspMissionEntity.GetThinkIdleChancePercentage() >= (double) UnityEngine.Random.Range(0.0f, 1f))
        {
          if (((double) dawngraspMissionEntity.GetThinkEmoteBossIdleChancePercentage() < (double) UnityEngine.Random.Range(0.0f, 1f) || !dawngraspMissionEntity.m_Mission_FriendlyPlayIdleLines) && dawngraspMissionEntity.m_Mission_EnemyPlayIdleLines)
          {
            if (dawngraspMissionEntity.m_Mission_EnemyPlayIdleLinesUseingEmoteSystem)
              yield return (object) dawngraspMissionEntity.MissionPlayThinkEmote(actor1);
            else
              yield return (object) GameEntity.Coroutines.StartCoroutine(dawngraspMissionEntity.HandleMissionEventWithTiming(517));
          }
          else if (dawngraspMissionEntity.m_Mission_FriendlyPlayIdleLines)
          {
            if (dawngraspMissionEntity.m_Mission_FriendlyPlayIdleLinesUseingEmoteSystem)
              yield return (object) dawngraspMissionEntity.MissionPlayThinkEmote(actor2);
            else
              yield return (object) GameEntity.Coroutines.StartCoroutine(dawngraspMissionEntity.HandleMissionEventWithTiming(518));
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
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
    while (dawngraspMissionEntity.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    if (gameEntity.GetTag(GAME_TAG.PREVIOUS_PUZZLE_COMPLETED) == 0)
    {
      dawngraspMissionEntity.MissionPause(true);
      yield return (object) dawngraspMissionEntity.HandleMissionEventWithTiming(520);
      dawngraspMissionEntity.MissionPause(false);
    }
    if (gameEntity.GetTag(GAME_TAG.PREVIOUS_PUZZLE_COMPLETED) == 1)
    {
      dawngraspMissionEntity.MissionPause(true);
      yield return (object) dawngraspMissionEntity.HandleMissionEventWithTiming(521);
      dawngraspMissionEntity.MissionPause(false);
    }
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

  protected IEnumerator MissionPlayVO(
    Actor actor,
    string line,
    bool bUseBubble,
    MissionEntity.ShouldPlay shouldPlay)
  {
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
    if (!((UnityEngine.Object) actor == (UnityEngine.Object) null) && line != null)
    {
      Notification.SpeechBubbleDirection speakerDirection = dawngraspMissionEntity.GetDirection(actor);
      if (dawngraspMissionEntity.m_forceAlwaysPlayLine)
        yield return (object) GameEntity.Coroutines.StartCoroutine(dawngraspMissionEntity.PlayLine(actor, line, shouldPlay, 2.5f));
      bool parentBubbleToActor = !((UnityEngine.Object) actor.GetCard() != (UnityEngine.Object) null) || actor.GetCard().GetEntity() == null || !actor.GetCard().GetEntity().IsHeroPower();
      if (shouldPlay() == dawngraspMissionEntity.InternalShouldPlayAlways())
        yield return (object) GameEntity.Coroutines.StartCoroutine(dawngraspMissionEntity.PlaySoundAndBlockSpeech(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
      else if (shouldPlay() == dawngraspMissionEntity.InternalShouldPlayOnlyOnce())
      {
        yield return (object) GameEntity.Coroutines.StartCoroutine(dawngraspMissionEntity.PlaySoundAndBlockSpeechOnce(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
        NotificationManager.Get().ForceAddSoundToPlayedList(line);
      }
    }
  }

  public IEnumerator MissionPlayVO(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) dawngraspMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(dawngraspMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) dawngraspMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(dawngraspMissionEntity.InternalShouldPlayOnce));
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
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
    bool removeLine = false;
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Once && !dawngraspMissionEntity.m_forceAlwaysPlayLine)
      removeLine = true;
    string line = !bPlayOrder ? dawngraspMissionEntity.PopNextLine(lines, removeLine) : dawngraspMissionEntity.PopRandomLine(lines, removeLine);
    if (line != null)
    {
      yield return (object) dawngraspMissionEntity.MissionPlayVO(speaker, line, bUseBubble, shouldPlay);
      dawngraspMissionEntity.m_lastVOplayFinishtime = Time.time;
    }
  }

  public IEnumerator MissionPlayVO(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) dawngraspMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(dawngraspMissionEntity.InternalShouldPlayAlways));
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
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
    if (dawngraspMissionEntity.m_enemySpeaking)
      yield return (object) null;
    dawngraspMissionEntity.m_enemySpeaking = true;
    if (dawngraspMissionEntity.m_forceAlwaysPlayLine)
      yield return (object) GameEntity.Coroutines.StartCoroutine(dawngraspMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
      yield return (object) GameEntity.Coroutines.StartCoroutine(dawngraspMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    else if (shouldPlay() == MissionEntity.ShouldPlayValue.Once)
    {
      yield return (object) GameEntity.Coroutines.StartCoroutine(dawngraspMissionEntity.PlayBigCharacterQuoteAndWaitOnce(brassRing, line));
      NotificationManager.Get().ForceAddSoundToPlayedList(line);
    }
    dawngraspMissionEntity.m_enemySpeaking = false;
  }

  public IEnumerator MissionPlayVO(AssetReference brassRing, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) dawngraspMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(dawngraspMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(AssetReference brassRing, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) dawngraspMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(dawngraspMissionEntity.InternalShouldPlayOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(string minionSpeaker, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) dawngraspMissionEntity.MissionPlayVO(dawngraspMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), line, true, new MissionEntity.ShouldPlay(dawngraspMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(
    string minionSpeaker,
    AssetReference brassRing,
    string line)
  {
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
    if ((UnityEngine.Object) dawngraspMissionEntity.FindActorInPlayByDesignCode(minionSpeaker) == (UnityEngine.Object) null)
      yield return (object) dawngraspMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(dawngraspMissionEntity.InternalShouldPlayAlways));
    else
      yield return (object) dawngraspMissionEntity.MissionPlayVO(dawngraspMissionEntity.FindActorInPlayByDesignCode(minionSpeaker), line, true, new MissionEntity.ShouldPlay(dawngraspMissionEntity.InternalShouldPlayAlways));
  }

  public IEnumerator MissionPlaySound(string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_10_Dawngrasp_MissionEntity dawngraspMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) GameEntity.Coroutines.StartCoroutine(dawngraspMissionEntity.PlaySoundAndWait(line, (string) null, Notification.SpeechBubbleDirection.None, (Actor) null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlaySound(Actor actor, string line)
  {
    yield return (object) this.MissionPlaySound(line);
  }

  public IEnumerator MissionPlaySound(List<string> lines)
  {
    yield return (object) this.MissionPlaySound(this.PopRandomLine(lines));
  }

  public IEnumerator MissionPlaySound(Actor actor, List<string> lines)
  {
    yield return (object) this.MissionPlaySound(lines);
  }

  public static class MemberInfoGetting
  {
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression) => ((MemberExpression) memberExpression.Body).Member.Name;
  }
}
