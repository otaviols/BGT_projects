using Blizzard.T5.Core;
using System.Collections;
using UnityEngine;

public class MobileChatLogUI : IChatLogUI
{
  private ChatFrames m_chatFrames;
  private Map<Renderer, int> m_chatLogOriginalLayers = new Map<Renderer, int>();

  public bool IsShowing => (Object) this.m_chatFrames != (Object) null;

  public GameObject GameObject => !((Object) this.m_chatFrames == (Object) null) ? this.m_chatFrames.gameObject : (GameObject) null;

  public BnetPlayer Receiver => !((Object) this.m_chatFrames == (Object) null) ? this.m_chatFrames.Receiver : (BnetPlayer) null;

  public void ShowForPlayer(BnetPlayer player)
  {
    string assetRef = (bool) UniversalInputManager.UsePhoneUI ? "MobileChatFrames_phone.prefab:044c4b3ec33f4454c9a95d6a9ee52552" : "MobileChatFrames.prefab:1b0605e4925ea4424a53e7b000ad961f";
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef);
    if ((Object) gameObject != (Object) null)
    {
      this.m_chatFrames = gameObject.GetComponent<ChatFrames>();
      this.m_chatFrames.Receiver = player;
    }
    this.m_chatFrames.chatLogFrame.Focus(false);
    this.m_chatFrames.StartCoroutine(this.ShowChatWhenReady(player));
  }

  private IEnumerator ShowChatWhenReady(BnetPlayer player)
  {
    while ((Object) this.m_chatFrames == (Object) null || (Object) this.m_chatFrames.chatLogFrame == (Object) null || this.m_chatFrames.chatLogFrame.IsWaitingOnMedal)
    {
      if ((Object) this.m_chatFrames == (Object) null || (Object) this.m_chatFrames.chatLogFrame == (Object) null)
        yield break;
      else
        yield return (object) null;
    }
    this.m_chatFrames.chatLogFrame.Focus(true);
  }

  public void Hide()
  {
    if (!this.IsShowing)
      return;
    Object.Destroy((Object) this.m_chatFrames.gameObject);
    this.m_chatFrames = (ChatFrames) null;
  }

  public void GoBack()
  {
    if (!this.IsShowing)
      return;
    this.m_chatFrames.Back();
  }
}
