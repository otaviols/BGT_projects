using Hearthstone.Progression;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class BoH_Guldan_MissionEntity : GenericDungeonMissionEntity
{
  public bool m_Mission_EnemyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_EnemyPlayIdleLines = true;
  public bool m_Mission_EnemyPlayIdleLinesInOrder = true;
  public bool m_Mission_EnemyPlayHeroPowerLines;
  public bool m_Mission_EnemyPlayHeroPowerLinesInOrder;
  public bool m_Mission_FriendlyHeroShouldExplodeOnDefeat = true;
  public bool m_Mission_FriendlyPlayIdleLines;
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
  public const bool PlayLinesRandomOrder = false;
  public const bool PlayLinesInOrder = true;
  public const int InGame_BossAttacks = 500;
  public const int InGame_BossAttacksSpecial = 501;
  public const int InGame_BossUsesHeroPower = 510;
  public const int InGame_BossUsesHeroPowerSpecial = 511;
  public const int InGame_BossEquipWeapon = 513;
  public const int InGame_PlayerAttacks = 502;
  public const int InGame_PlayerAttacksSpecial = 503;
  public const int InGame_PlayerUsesHeroPower = 508;
  public const int InGame_PlayerUsesHeroPowerSpecial = 509;
  public const int InGame_PlayerEquipWeapon = 512;
  public const int InGame_VictoryPreExplosion = 504;
  public const int InGame_VictoryPostExplosion = 505;
  public const int InGame_LossPreExplosion = 506;
  public const int InGame_LossPostExplosion = 507;
  public const int InGame_Introduction = 514;
  public const int InGame_EmoteResponse = 515;
  public const int InGame_AfterResetGame = 520;
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
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
    float bossHeroPowerVoLine = guldanMissionEntity.ChanceToPlayBossHeroPowerVOLine();
    float num = UnityEngine.Random.Range(0.0f, 1f);
    if (!guldanMissionEntity.m_enemySpeaking && !guldanMissionEntity.m_MissionDisableAutomaticVO && (double) bossHeroPowerVoLine >= (double) num)
    {
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
      {
        List<string> powerRandomLines = guldanMissionEntity.GetBossHeroPowerRandomLines();
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
          yield return (object) guldanMissionEntity.MissionPlayVO(actor, str);
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
    if (!guldanMissionEntity.m_MissionDisableAutomaticVO && !guldanMissionEntity.m_enemySpeaking && entity.GetCardType() != TAG_CARDTYPE.INVALID && entity.GetCardType() == TAG_CARDTYPE.HERO_POWER && entity.GetControllerSide() == Player.Side.OPPOSING)
      yield return (object) guldanMissionEntity.OnBossHeroPowerPlayed(entity);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType) || this.m_enemySpeaking)
      return;
    GameEntity.Coroutines.StartCoroutine(this.HandleMissionEventWithTiming(515));
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    switch (gameResult)
    {
      case TAG_PLAYSTATE.WON:
        GameState.Get().SetBusy(true);
        GameState.Get().SetBusy(false);
        break;
      case TAG_PLAYSTATE.LOST:
        GameState.Get().SetBusy(true);
        GameState.Get().SetBusy(false);
        break;
      default:
        yield break;
    }
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
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
    if (!guldanMissionEntity.m_enemySpeaking)
    {
      Player currentPlayer = GameState.Get().GetCurrentPlayer();
      if (currentPlayer.IsFriendlySide() && !currentPlayer.GetHeroCard().HasActiveEmoteSound())
      {
        GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
        if ((guldanMissionEntity.m_Mission_FriendlyPlayIdleLines || guldanMissionEntity.m_Mission_EnemyPlayIdleLines) && (double) guldanMissionEntity.GetThinkIdleChancePercentage() >= (double) UnityEngine.Random.Range(0.0f, 1f))
        {
          if ((double) guldanMissionEntity.GetThinkEmoteBossIdleChancePercentage() < (double) UnityEngine.Random.Range(0.0f, 1f) || !guldanMissionEntity.m_Mission_FriendlyPlayIdleLines && guldanMissionEntity.m_Mission_EnemyPlayIdleLines)
          {
            Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
            string line = guldanMissionEntity.PopRandomLine(guldanMissionEntity.m_BossIdleLinesCopy);
            if (guldanMissionEntity.m_BossIdleLinesCopy.Count == 0)
              guldanMissionEntity.m_BossIdleLinesCopy = new List<string>((IEnumerable<string>) guldanMissionEntity.m_BossIdleLines);
            yield return (object) guldanMissionEntity.MissionPlayVO(actor, line);
          }
          else if (guldanMissionEntity.m_Mission_FriendlyPlayIdleLines)
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

  protected override IEnumerator RespondToResetGameFinishedWithTiming(Entity entity)
  {
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
    while (guldanMissionEntity.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (GameState.Get().GetGameEntity().GetTag(GAME_TAG.PREVIOUS_PUZZLE_COMPLETED) == 0)
    {
      guldanMissionEntity.MissionPause(true);
      yield return (object) guldanMissionEntity.HandleMissionEventWithTiming(520);
      guldanMissionEntity.MissionPause(false);
    }
  }

  public static bool GetIsFirstBoss()
  {
    AdventureDataDbfRecord adventureDataRecord = BoH_Guldan_MissionEntity.GetAdventureDataRecord(Options.Get().GetInt(Option.SELECTED_ADVENTURE), Options.Get().GetInt(Option.SELECTED_ADVENTURE_MODE));
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

  protected Actor GetEnemyActorByCardId(string cardId)
  {
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    foreach (Card card in opposingSidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == opposingSidePlayer.GetPlayerId() && entity.GetCardId() == cardId)
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
  }

  protected Actor GetFriendlyActorByCardId(string CardId)
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    foreach (Card card in friendlySidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == friendlySidePlayer.GetPlayerId() && entity.GetCardId() == CardId)
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
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

  protected virtual bool GetShouldSuppressDeathTextBubble() => this.m_SupressEnemyDeathTextBubble;

  protected IEnumerator MissionPlayVO(
    Actor actor,
    string line,
    bool bUseBubble,
    MissionEntity.ShouldPlay shouldPlay)
  {
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
    Notification.SpeechBubbleDirection speakerDirection = guldanMissionEntity.GetDirection(actor);
    if (guldanMissionEntity.m_forceAlwaysPlayLine)
      yield return (object) GameEntity.Coroutines.StartCoroutine(guldanMissionEntity.PlayLine(actor, line, shouldPlay, 2.5f));
    if (shouldPlay() == guldanMissionEntity.InternalShouldPlayAlways())
      yield return (object) GameEntity.Coroutines.StartCoroutine(guldanMissionEntity.PlaySoundAndBlockSpeech(line, speakerDirection, actor, 2.5f));
    else if (shouldPlay() == guldanMissionEntity.InternalShouldPlayOnlyOnce())
    {
      yield return (object) GameEntity.Coroutines.StartCoroutine(guldanMissionEntity.PlaySoundAndBlockSpeechOnce(line, speakerDirection, actor, 2.5f));
      NotificationManager.Get().ForceAddSoundToPlayedList(line);
    }
  }

  public IEnumerator MissionPlayVOSound(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(actor, line, false, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(actor, line, true, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator MissionPlayVO(
    Actor speaker,
    List<string> lines,
    MissionEntity.ShouldPlay shouldPlay,
    bool bUseBubble = true,
    bool bPlayOrder = false)
  {
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
    bool removeLine = false;
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Once && !guldanMissionEntity.m_forceAlwaysPlayLine)
      removeLine = true;
    string line = !bPlayOrder ? guldanMissionEntity.PopNextLine(lines, removeLine) : guldanMissionEntity.PopRandomLine(lines, removeLine);
    if (line != null)
      yield return (object) guldanMissionEntity.MissionPlayVO(speaker, line, bUseBubble, shouldPlay);
  }

  public IEnumerator MissionPlaySound(Actor actor, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) GameEntity.Coroutines.StartCoroutine(guldanMissionEntity.PlaySoundAndWait(line, (string) null, Notification.SpeechBubbleDirection.None, (Actor) null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells));
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

  public IEnumerator MissionPlayVO(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(((MissionEntity) guldanMissionEntity).InternalShouldPlayOnlyOnce), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOInOrder(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnceInOrder(Actor actor, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(actor, lines, new MissionEntity.ShouldPlay(((MissionEntity) guldanMissionEntity).InternalShouldPlayOnlyOnce));
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
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
    if (guldanMissionEntity.m_enemySpeaking)
      yield return (object) null;
    guldanMissionEntity.m_enemySpeaking = true;
    if (guldanMissionEntity.m_forceAlwaysPlayLine)
      yield return (object) GameEntity.Coroutines.StartCoroutine(guldanMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
      yield return (object) GameEntity.Coroutines.StartCoroutine(guldanMissionEntity.PlayBigCharacterQuoteAndWait(brassRing, line));
    else if (shouldPlay() == MissionEntity.ShouldPlayValue.Once)
    {
      yield return (object) GameEntity.Coroutines.StartCoroutine(guldanMissionEntity.PlayBigCharacterQuoteAndWaitOnce(brassRing, line));
      NotificationManager.Get().ForceAddSoundToPlayedList(line);
    }
    guldanMissionEntity.m_enemySpeaking = false;
  }

  protected IEnumerator MissionPlayVO(
    AssetReference brassRing,
    List<string> lines,
    MissionEntity.ShouldPlay shouldPlay,
    bool bUseBubble = true,
    bool bPlayOrder = false)
  {
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
    bool removeLine = false;
    if (shouldPlay() == MissionEntity.ShouldPlayValue.Once && !guldanMissionEntity.m_forceAlwaysPlayLine)
      removeLine = true;
    string line = !bPlayOrder ? guldanMissionEntity.PopNextLine(lines, removeLine) : guldanMissionEntity.PopRandomLine(lines, removeLine);
    yield return (object) guldanMissionEntity.MissionPlayVO((string) brassRing, line, bUseBubble, shouldPlay);
  }

  protected IEnumerator MissionPlayVO(
    AssetReference brassRing,
    List<string> lines,
    Actor minionOverride,
    MissionEntity.ShouldPlay shouldPlay,
    bool bUseBubble = true,
    bool bPlayOrder = false)
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
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(AssetReference brassRing, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO((string) brassRing, line, true, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlaySound(AssetReference brassRing, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(AssetReference brassRing, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(AssetReference brassRing, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(((MissionEntity) guldanMissionEntity).InternalShouldPlayOnlyOnce), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOInOrder(AssetReference brassRing, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways));
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
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(brassRing, lines, new MissionEntity.ShouldPlay(((MissionEntity) guldanMissionEntity).InternalShouldPlayOnlyOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOInOrder(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(guldanMissionEntity.GetActorByCardId(minionSpeaker), lines, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(string minionSpeaker, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(guldanMissionEntity.GetActorByCardId(minionSpeaker), line, true, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlaySound(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(guldanMissionEntity.GetActorByCardId(minionSpeaker), lines, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(guldanMissionEntity.GetActorByCardId(minionSpeaker), lines, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnce(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(guldanMissionEntity.GetActorByCardId(minionSpeaker), lines, new MissionEntity.ShouldPlay(((MissionEntity) guldanMissionEntity).InternalShouldPlayOnlyOnce), false);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVO(string minionSpeaker, string line)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(guldanMissionEntity.GetActorByCardId(minionSpeaker), line, true, new MissionEntity.ShouldPlay(guldanMissionEntity.InternalShouldPlayAlways));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public IEnumerator MissionPlayVOOnceInOrder(string minionSpeaker, List<string> lines)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_MissionEntity guldanMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) guldanMissionEntity.MissionPlayVO(guldanMissionEntity.GetActorByCardId(minionSpeaker), lines, new MissionEntity.ShouldPlay(((MissionEntity) guldanMissionEntity).InternalShouldPlayOnlyOnce));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public static class MemberInfoGetting
  {
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression) => ((MemberExpression) memberExpression.Body).Member.Name;
  }
}
