using UnityEngine;

public class PlayerLeaderboardIcon : MonoBehaviour
{
  public GameObject m_icon;
  public UberText m_text;
  private const string SHOW_PLAYMAKER_STATE = "Show";

  public void SetText(string text)
  {
    if (text == "")
      this.ClearText();
    else
      this.m_text.gameObject.SetActive(true);
    this.m_text.Text = text;
  }

  public void ClearText()
  {
    this.m_text.Text = "";
    this.m_text.gameObject.SetActive(false);
  }

  public void SetPlaymakerValue(string name, int value)
  {
    PlayMakerFSM component = this.gameObject.GetComponent<PlayMakerFSM>();
    if (!((Object) component != (Object) null) || component.FsmVariables.GetFsmInt(name) == null)
      return;
    component.FsmVariables.GetFsmInt(name).Value = value;
    component.SendEvent("Action");
  }

  public void PlaymakerShow()
  {
    PlayMakerFSM component = this.gameObject.GetComponent<PlayMakerFSM>();
    if ((Object) component != (Object) null)
      component.SetState("Show");
    else
      this.gameObject.SetActive(false);
  }

  public bool PlaymakerIsShowing()
  {
    PlayMakerFSM component = this.gameObject.GetComponent<PlayMakerFSM>();
    return (Object) component != (Object) null && component.ActiveStateName == "Show";
  }
}
