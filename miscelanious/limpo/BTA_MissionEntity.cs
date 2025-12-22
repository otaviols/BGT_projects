using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class BTA_MissionEntity : GenericDungeonMissionEntity
{
  public override sealed AdventureDbId GetAdventureID() => AdventureDbId.BTA;

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.LOST)
      yield return (object) new WaitForSeconds(5f);
  }

  public override bool ShouldShowHeroClassDuringMulligan(Player.Side playerSide) => false;

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
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
    GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType);
  }

  public override void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      return;
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DHMulligan);
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BT);

  public IEnumerator PlayAndRemoveRandomLineOnlyOnce(Actor actor, List<string> lines)
  {
    BTA_MissionEntity btaMissionEntity = this;
    string line = btaMissionEntity.PopRandomLine(lines);
    if (line != null)
      yield return (object) btaMissionEntity.PlayLineOnlyOnce(actor, line);
  }

  public IEnumerator PlayAndRemoveRandomLineOnlyOnce(string actor, List<string> lines)
  {
    BTA_MissionEntity btaMissionEntity = this;
    string line = btaMissionEntity.PopRandomLine(lines);
    if (line != null)
      yield return (object) btaMissionEntity.PlayLineOnlyOnce(actor, line);
  }

  protected string PopRandomLine(List<string> lines)
  {
    if (lines == null || lines.Count == 0)
      return (string) null;
    string line = lines[UnityEngine.Random.Range(0, lines.Count)];
    lines.Remove(line);
    return line;
  }

  protected IEnumerator PlayLineAlways(
    Actor speaker,
    string line,
    Notification.SpeechBubbleDirection direction,
    float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BTA_MissionEntity btaMissionEntity = this;
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
    this.\u003C\u003E2__current = (object) btaMissionEntity.PlayLine(speaker, line, new MissionEntity.ShouldPlay(((MissionEntity) btaMissionEntity).InternalShouldPlayBossLine), duration, direction);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayLine(
    Actor speaker,
    string line,
    MissionEntity.ShouldPlay shouldPlay,
    float duration,
    Notification.SpeechBubbleDirection direction)
  {
    BTA_MissionEntity btaMissionEntity = this;
    if (btaMissionEntity.m_enemySpeaking)
      yield return (object) null;
    btaMissionEntity.m_enemySpeaking = true;
    if (btaMissionEntity.m_forceAlwaysPlayLine)
      yield return (object) GameEntity.Coroutines.StartCoroutine(btaMissionEntity.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
    else if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
      yield return (object) GameEntity.Coroutines.StartCoroutine(btaMissionEntity.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
    else if (shouldPlay() == MissionEntity.ShouldPlayValue.Once)
      yield return (object) GameEntity.Coroutines.StartCoroutine(btaMissionEntity.PlaySoundAndBlockSpeechOnce(line, direction, speaker, duration));
    NotificationManager.Get().ForceAddSoundToPlayedList(line);
    btaMissionEntity.m_enemySpeaking = false;
  }

  public static class MemberInfoGetting
  {
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression) => ((MemberExpression) memberExpression.Body).Member.Name;
  }
}
