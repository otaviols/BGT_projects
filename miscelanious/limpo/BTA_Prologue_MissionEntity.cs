using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class BTA_Prologue_MissionEntity : GenericDungeonMissionEntity
{
  public override sealed AdventureDbId GetAdventureID() => AdventureDbId.BTP;

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
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ULDMulligan);
  }

  protected string PopRandomLine(List<string> lines)
  {
    if (lines.Count == 0 || lines == null)
      return (string) null;
    string line = lines[UnityEngine.Random.Range(0, lines.Count)];
    lines.Remove(line);
    return line;
  }

  public static class MemberInfoGetting
  {
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression) => ((MemberExpression) memberExpression.Body).Member.Name;
  }
}
