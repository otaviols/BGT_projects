using Blizzard.T5.Core;
using Hearthstone.Progression;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class BoM_01_Rokara_MissionEntity : GenericDungeonMissionEntity
{
  public bool m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_EnemyPlayIdleLines = true;
  public bool m_Mission_EnemyPlayIdleLinesInOrder = true;
  public bool m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_FriendlyPlayIdleLines = true;
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
  private static Map<GameEntityOption, bool> s_booleanOptions = BoM_01_Rokara_MissionEntity.InitBooleanOptions();

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

  public BoM_01_Rokara_MissionEntity() => this.m_gameOptions.AddBooleanOptions(BoM_01_Rokara_MissionEntity.s_booleanOptions);

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
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
    float bossHeroPowerVoLine = rokaraMissionEntity.ChanceToPlayBossHeroPowerVOLine();
    float num = UnityEngine.Random.Range(0.0f, 1f);
    if (!rokaraMissionEntity.m_enemySpeaking && !rokaraMissionEntity.m_MissionDisableAutomaticVO && (double) bossHeroPowerVoLine >= (double) num)
    {
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
      {
        List<string> powerRandomLines = rokaraMissionEntity.GetBossHeroPowerRandomLines();
        string str = "";
        while (powerRandomLines.Count > 0)
        {
          int index = UnityEngine.Random.Range(0, powerRandomLines.Count);
          str = powerRandomLines[index];
          powerRandomLines.RemoveAt(index);
          if (!NotificationManager.Get().HasSoundPlayedThisSession(str))
            break;
        }
        if (!(str == ""))
          yield return (object) rokaraMissionEntity.MissionPlayVO(actor, str);
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
    if (!rokaraMissionEntity.m_MissionDisableAutomaticVO && !rokaraMissionEntity.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.FRIENDLY)
      yield return (object) rokaraMissionEntity.HandleMissionEventWithTiming(508);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
    if (!rokaraMissionEntity.m_MissionDisableAutomaticVO && !rokaraMissionEntity.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.OPPOSING)
      yield return (object) rokaraMissionEntity.OnBossHeroPowerPlayed(entity);
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
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
    if (!rokaraMissionEntity.m_enemySpeaking)
    {
      Player currentPlayer = GameState.Get().GetCurrentPlayer();
      if (currentPlayer.IsFriendlySide() && !currentPlayer.GetHeroCard().HasActiveEmoteSound())
      {
        GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
        if ((rokaraMissionEntity.m_Mission_FriendlyPlayIdleLines || rokaraMissionEntity.m_Mission_EnemyPlayIdleLines) && (double) rokaraMissionEntity.GetThinkIdleChancePercentage() >= (double) UnityEngine.Random.Range(0.0f, 1f))
        {
          double chancePercentage = (double) rokaraMissionEntity.GetThinkEmoteBossIdleChancePercentage();
          float num1 = UnityEngine.Random.Range(0.0f, 1f);
          if (rokaraMissionEntity.m_BossIdleLinesCopy.Count == 0)
          {
            rokaraMissionEntity.m_BossIdleLinesCopy = new List<string>((IEnumerable<string>) rokaraMissionEntity.m_BossIdleLines);
            rokaraMissionEntity.m_Mission_EnemyPlayIdleLines = rokaraMissionEntity.m_BossIdleLinesCopy.Count != 0;
          }
          double num2 = (double) num1;
          if ((chancePercentage < num2 || !rokaraMissionEntity.m_Mission_FriendlyPlayIdleLines) && rokaraMissionEntity.m_Mission_EnemyPlayIdleLines)
          {
            Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
            string line = rokaraMissionEntity.PopRandomLine(rokaraMissionEntity.m_BossIdleLinesCopy);
            yield return (object) rokaraMissionEntity.MissionPlayVO(actor, line);
          }
          else if (rokaraMissionEntity.m_Mission_FriendlyPlayIdleLines)
          {
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
            AudioSource activeAudioSource = GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType).GetActiveAudioSource();
            yield return (object) GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType);
            yield return (object) new WaitForSeconds(activeAudioSource.clip.length);
            activeAudioSource = (AudioSource) null;
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

  protected IEnumerator MissionPlayVO(
    Actor actor,
    string line,
    bool bUseBubble,
    MissionEntity.ShouldPlay shouldPlay)
  {
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
    if (!((UnityEngine.Object) actor == (UnityEngine.Object) null) && line != null)
    {
      Notification.SpeechBubbleDirection speakerDirection = rokaraMissionEntity.GetDirection(actor);
      if (rokaraMissionEntity.m_forceAlwaysPlayLine)
        yield return (object) GameEntity.Coroutines.StartCoroutine(rokaraMissionEntity.PlayLine(actor, line, shouldPlay, 2.5f));
      bool parentBubbleToActor = !((UnityEngine.Object) actor.GetCard() != (UnityEngine.Object) null) || actor.GetCard().GetEntity() == null || !actor.GetCard().GetEntity().IsHeroPower();
      if (shouldPlay() == rokaraMissionEntity.InternalShouldPlayAlways())
        yield return (object) GameEntity.Coroutines.StartCoroutine(rokaraMissionEntity.PlaySoundAndBlockSpeech(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
      else if (shouldPlay() == rokaraMissionEntity.InternalShouldPlayOnlyOnce())
      {
        yield return (object) GameEntity.Coroutines.StartCoroutine(rokaraMissionEntity.PlaySoundAndBlockSpeechOnce(line, speakerDirection, actor, 2.5f, parentBubbleToActor: parentBubbleToActor));
        NotificationManager.Get().ForceAddSoundToPlayedList(line);
      }
    }
  }

  public IEnumerator MissionPlayVO(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) rokaraMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(rokaraMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) rokaraMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(rokaraMissionEntity.InternalShouldPlayOnce));
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
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
    bool removeLine = false;
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Once && !rokaraMissionEntity.m_forceAlwaysPlayLine)
      removeLine = true;
    string line = !bPlayOrder ? rokaraMissionEntity.PopNextLine(lines, removeLine) : rokaraMissionEntity.PopRandomLine(lines, removeLine);
    if (line != null)
      yield return (object) rokaraMissionEntity.MissionPlayVO(speaker, line, bUseBubble, shouldPlay);
  }

  public IEnumerator MissionPlaySound(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
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
    float waitTimeScale = 1f;
    bool parentBubbleToActor = true;
    bool delayCardSoundSpells = false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) GameEntity.Coroutines.StartCoroutine(rokaraMissionEntity.PlaySoundAndWait(line, (string) null, Notification.SpeechBubbleDirection.None, (Actor) null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlaySound(Actor actor, List<string> lines)
  {
    bool removeLine = false;
    string line = this.PopRandomLine(lines, removeLine);
    yield return (object) this.MissionPlaySound(actor, line);
  }

  public IEnumerator MissionPlayVOOnceInOrder(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) rokaraMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(((MissionEntity) rokaraMissionEntity).InternalShouldPlayOnlyOnce), false);
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
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
    if (rokaraMissionEntity.m_enemySpeaking)
      yield return (object) null;
    rokaraMissionEntity.m_enemySpeaking = true;
    if (rokaraMissionEntity.m_forceAlwaysPlayLine)
      yield return (object) GameEntity.Coroutines.StartCoroutine(rokaraMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
      yield return (object) GameEntity.Coroutines.StartCoroutine(rokaraMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    else if (shouldPlay() == MissionEntity.ShouldPlayValue.Once)
    {
      yield return (object) GameEntity.Coroutines.StartCoroutine(rokaraMissionEntity.PlayBigCharacterQuoteAndWaitOnce(brassRing, line));
      NotificationManager.Get().ForceAddSoundToPlayedList(line);
    }
    rokaraMissionEntity.m_enemySpeaking = false;
  }

  public IEnumerator MissionPlayVO(AssetReference brassRing, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) rokaraMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(rokaraMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) rokaraMissionEntity.MissionPlayVO(rokaraMissionEntity.GetActorByCardId(minionSpeaker), lines, new MissionEntity.ShouldPlay(rokaraMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) rokaraMissionEntity.MissionPlayVO(rokaraMissionEntity.GetActorByCardId(minionSpeaker), lines, new MissionEntity.ShouldPlay(((MissionEntity) rokaraMissionEntity).InternalShouldPlayOnlyOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(
    string minionSpeaker,
    AssetReference brassRing,
    string line)
  {
    BoM_01_Rokara_MissionEntity rokaraMissionEntity = this;
    if ((UnityEngine.Object) rokaraMissionEntity.GetActorByCardId(minionSpeaker) == (UnityEngine.Object) null)
      yield return (object) rokaraMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(rokaraMissionEntity.InternalShouldPlayAlways));
    else
      yield return (object) rokaraMissionEntity.MissionPlayVO(rokaraMissionEntity.GetActorByCardId(minionSpeaker), line, true, new MissionEntity.ShouldPlay(rokaraMissionEntity.InternalShouldPlayAlways));
  }

  public static class MemberInfoGetting
  {
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression) => ((MemberExpression) memberExpression.Body).Member.Name;
  }
}
