using Blizzard.GameService.SDK.Client.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FriendListChatIcon : MonoBehaviour
{
  private BnetPlayer m_player;

  public BnetPlayer GetPlayer() => this.m_player;

  public bool SetPlayer(BnetPlayer player)
  {
    if (this.m_player == player)
      return false;
    this.m_player = player;
    this.UpdateIcon();
    return true;
  }

  public void UpdateIcon()
  {
    if (this.m_player == null)
    {
      this.gameObject.SetActive(false);
    }
    else
    {
      List<BnetWhisper> whispersWithPlayer = BnetWhisperMgr.Get().GetWhispersWithPlayer(this.m_player);
      if (whispersWithPlayer == null)
        this.gameObject.SetActive(false);
      else if (WhisperUtil.IsSpeaker(BnetPresenceMgr.Get().GetMyPlayer(), whispersWithPlayer[whispersWithPlayer.Count - 1]))
      {
        this.gameObject.SetActive(false);
      }
      else
      {
        PlayerChatInfo playerChatInfo = ChatMgr.Get().GetPlayerChatInfo(this.m_player);
        if (playerChatInfo != null && whispersWithPlayer.LastOrDefault<BnetWhisper>((Func<BnetWhisper, bool>) (whisper => WhisperUtil.IsSpeaker(this.m_player, whisper))) == playerChatInfo.GetLastSeenWhisper())
          this.gameObject.SetActive(false);
        else
          this.gameObject.SetActive(true);
      }
    }
  }
}
