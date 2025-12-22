using UnityEngine;

public class PlayerLeaderboardInformationPanel : MonoBehaviour
{
  public UberText m_panelLabel;

  public void SetTitle(string text) => this.m_panelLabel.Text = text;
}
