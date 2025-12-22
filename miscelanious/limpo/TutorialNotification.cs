using UnityEngine;

public class TutorialNotification : Notification
{
  public UIBButton m_ButtonStart;
  public UberText m_WantedText;

  public void SetWantedText(string txt)
  {
    if (!((Object) this.m_WantedText != (Object) null))
      return;
    this.m_WantedText.Text = txt;
    this.m_WantedText.gameObject.SetActive(true);
  }
}
