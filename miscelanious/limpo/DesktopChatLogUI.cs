using UnityEngine;

public class DesktopChatLogUI : IChatLogUI
{
  private QuickChatFrame m_quickChatFrame;

  public bool IsShowing => (Object) this.m_quickChatFrame != (Object) null;

  public GameObject GameObject => !((Object) this.m_quickChatFrame == (Object) null) ? this.m_quickChatFrame.gameObject : (GameObject) null;

  public BnetPlayer Receiver => !((Object) this.m_quickChatFrame == (Object) null) ? this.m_quickChatFrame.GetReceiver() : (BnetPlayer) null;

  public void ShowForPlayer(BnetPlayer player)
  {
    if ((Object) this.m_quickChatFrame != (Object) null)
      return;
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "QuickChatFrame.prefab:a8bbab56b6588e44a8f0d25fc30ae886");
    if (!((Object) gameObject != (Object) null))
      return;
    this.m_quickChatFrame = gameObject.GetComponent<QuickChatFrame>();
    this.m_quickChatFrame.SetReceiver(player);
  }

  public void Hide()
  {
    if ((Object) this.m_quickChatFrame == (Object) null)
      return;
    Object.Destroy((Object) this.m_quickChatFrame.gameObject);
    this.m_quickChatFrame = (QuickChatFrame) null;
  }

  public void GoBack()
  {
  }
}
