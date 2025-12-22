using Blizzard.T5.Core;
using Hearthstone.Progression;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class BOM_06_Cariel_MissionEntity : GenericDungeonMissionEntity
{
  public bool m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_EnemyPlayIdleLines = true;
  public bool m_Mission_EnemyPlayIdleLinesInOrder = true;
  public bool m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_FriendlyPlayIdleLines = true;
  public bool m_Mission_FriendlyPlayIdleLinesUseingEmoteSystem = true;
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
  public float m_lastVOplayFinshtime;
  public float m_BanterVOSilenceTime = 2f;
  private static Map<GameEntityOption, bool> s_booleanOptions = BOM_06_Cariel_MissionEntity.InitBooleanOptions();

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

  public BOM_06_Cariel_MissionEntity() => this.m_gameOptions.AddBooleanOptions(BOM_06_Cariel_MissionEntity.s_booleanOptions);

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
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
    float bossHeroPowerVoLine = carielMissionEntity.ChanceToPlayBossHeroPowerVOLine();
    float num = UnityEngine.Random.Range(0.0f, 1f);
    if (!carielMissionEntity.m_enemySpeaking && !carielMissionEntity.m_MissionDisableAutomaticVO && (double) bossHeroPowerVoLine >= (double) num && !((UnityEngine.Object) GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor() == (UnityEngine.Object) null))
      yield return (object) carielMissionEntity.HandleMissionEventWithTiming(510);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
    if (!carielMissionEntity.m_MissionDisableAutomaticVO && !carielMissionEntity.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.FRIENDLY)
      yield return (object) carielMissionEntity.HandleMissionEventWithTiming(508);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
    if (!carielMissionEntity.m_MissionDisableAutomaticVO && !carielMissionEntity.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.OPPOSING)
      yield return (object) carielMissionEntity.OnBossHeroPowerPlayed(entity);
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
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (!carielMissionEntity.m_enemySpeaking)
    {
      Player currentPlayer = GameState.Get().GetCurrentPlayer();
      if (currentPlayer.IsFriendlySide() && !currentPlayer.GetHeroCard().HasActiveEmoteSound())
      {
        GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
        if ((carielMissionEntity.m_Mission_FriendlyPlayIdleLines || carielMissionEntity.m_Mission_EnemyPlayIdleLines) && (double) carielMissionEntity.GetThinkIdleChancePercentage() >= (double) UnityEngine.Random.Range(0.0f, 1f))
        {
          if (((double) carielMissionEntity.GetThinkEmoteBossIdleChancePercentage() < (double) UnityEngine.Random.Range(0.0f, 1f) || !carielMissionEntity.m_Mission_FriendlyPlayIdleLines) && carielMissionEntity.m_Mission_EnemyPlayIdleLines)
            yield return (object) GameEntity.Coroutines.StartCoroutine(carielMissionEntity.HandleMissionEventWithTiming(517));
          else if (carielMissionEntity.m_Mission_FriendlyPlayIdleLines)
            yield return (object) GameEntity.Coroutines.StartCoroutine(carielMissionEntity.HandleMissionEventWithTiming(518));
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
          if (!string.IsNullOrEmpty(stringOption1))
            SoundManager.Get().LoadAndPlay((AssetReference) stringOption1);
          if (this.m_Mission_EnemyHeroShouldExplodeOnDefeat)
          {
            this.m_enemyBlowUpSpell = this.BlowUpHero(heroCard1, SpellType.ENDGAME_WIN);
            break;
          }
          break;
        case TAG_PLAYSTATE.LOST:
          string stringOption2 = this.GetGameOptions().GetStringOption(GameEntityOption.DEFEAT_AUDIO_PATH);
          if (!string.IsNullOrEmpty(stringOption2))
            SoundManager.Get().LoadAndPlay((AssetReference) stringOption2);
          if (this.m_Mission_FriendlyHeroShouldExplodeOnDefeat)
          {
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

  protected Actor GetActorByCardId(string CardId)
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    foreach (Card card in friendlySidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == friendlySidePlayer.GetPlayerId() && entity.GetCardId() == CardId)
        return entity.GetCard().GetActor();
    }
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    foreach (Card card in opposingSidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == opposingSidePlayer.GetPlayerId() && entity.GetCardId() == CardId)
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
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

  public bool shouldPlayBanterVO() => (double) Time.time > (double) (this.m_lastVOplayFinshtime + this.m_BanterVOSilenceTime);

  protected IEnumerator MissionPlayVO(
    Actor actor,
    string line,
    bool bUseBubble,
    MissionEntity.ShouldPlay shouldPlay)
  {
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
    if (!((UnityEngine.Object) actor == (UnityEngine.Object) null) && line != null)
    {
      Notification.SpeechBubbleDirection speakerDirection = carielMissionEntity.GetDirection(actor);
      if (carielMissionEntity.m_forceAlwaysPlayLine)
        yield return (object) GameEntity.Coroutines.StartCoroutine(carielMissionEntity.PlayLine(actor, line, shouldPlay, 2.5f));
      bool parentBubbleToActor = !((UnityEngine.Object) actor.GetCard() != (UnityEngine.Object) null) || actor.GetCard().GetEntity() == null || !actor.GetCard().GetEntity().IsHeroPower();
      if (shouldPlay() == carielMissionEntity.InternalShouldPlayAlways())
        yield return (object) GameEntity.Coroutines.StartCoroutine(carielMissionEntity.PlaySoundAndBlockSpeech(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
      else if (shouldPlay() == carielMissionEntity.InternalShouldPlayOnlyOnce())
      {
        yield return (object) GameEntity.Coroutines.StartCoroutine(carielMissionEntity.PlaySoundAndBlockSpeechOnce(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
        NotificationManager.Get().ForceAddSoundToPlayedList(line);
      }
      carielMissionEntity.m_lastVOplayFinshtime = Time.time;
    }
  }

  public IEnumerator MissionPlayVO(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) carielMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(carielMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) carielMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(carielMissionEntity.InternalShouldPlayOnce));
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
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
    bool removeLine = false;
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Once && !carielMissionEntity.m_forceAlwaysPlayLine)
      removeLine = true;
    string line = !bPlayOrder ? carielMissionEntity.PopNextLine(lines, removeLine) : carielMissionEntity.PopRandomLine(lines, removeLine);
    if (line != null)
      yield return (object) carielMissionEntity.MissionPlayVO(speaker, line, bUseBubble, shouldPlay);
  }

  public IEnumerator MissionPlayVO(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) carielMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(carielMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) carielMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(((MissionEntity) carielMissionEntity).InternalShouldPlayOnlyOnce));
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
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
    if (carielMissionEntity.m_enemySpeaking)
      yield return (object) null;
    carielMissionEntity.m_enemySpeaking = true;
    if (carielMissionEntity.m_forceAlwaysPlayLine)
      yield return (object) GameEntity.Coroutines.StartCoroutine(carielMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
      yield return (object) GameEntity.Coroutines.StartCoroutine(carielMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    else if (shouldPlay() == MissionEntity.ShouldPlayValue.Once)
    {
      yield return (object) GameEntity.Coroutines.StartCoroutine(carielMissionEntity.PlayBigCharacterQuoteAndWaitOnce(brassRing, line));
      NotificationManager.Get().ForceAddSoundToPlayedList(line);
    }
    carielMissionEntity.m_enemySpeaking = false;
  }

  public IEnumerator MissionPlayVO(AssetReference brassRing, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) carielMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(carielMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(string minionSpeaker, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) carielMissionEntity.MissionPlayVO(carielMissionEntity.GetActorByCardId(minionSpeaker), line, true, new MissionEntity.ShouldPlay(carielMissionEntity.InternalShouldPlayOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) carielMissionEntity.MissionPlayVO(carielMissionEntity.GetActorByCardId(minionSpeaker), lines, new MissionEntity.ShouldPlay(carielMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(string minionSpeaker, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) carielMissionEntity.MissionPlayVO(carielMissionEntity.GetActorByCardId(minionSpeaker), line, true, new MissionEntity.ShouldPlay(carielMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlaySound(string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BOM_06_Cariel_MissionEntity carielMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) GameEntity.Coroutines.StartCoroutine(carielMissionEntity.PlaySoundAndWait(line, (string) null, Notification.SpeechBubbleDirection.None, (Actor) null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlaySound(Actor actor, string line)
  {
    yield return (object) this.MissionPlaySound(line);
  }

  public static class MemberInfoGetting
  {
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression) => ((MemberExpression) memberExpression.Body).Member.Name;
  }
}
