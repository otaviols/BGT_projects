using UnityEngine;

public class CheatUI : MonoBehaviour
{
  public GameObject m_CheatManagerMenu;

  private void Start() => this.m_CheatManagerMenu.SetActive(false);

  public void CloseCheatMenu() => this.m_CheatManagerMenu.SetActive(false);

  private void Update()
  {
    if (!InputCollection.GetKey(KeyCode.LeftControl) || !InputCollection.GetKey(KeyCode.LeftAlt) || !InputCollection.GetKey(KeyCode.LeftShift) || !InputCollection.GetKeyDown(KeyCode.C))
      return;
    this.m_CheatManagerMenu.SetActive(!this.m_CheatManagerMenu.activeSelf);
    this.SetActiveTabOnOpen();
  }

  private void SetActiveTabOnOpen()
  {
    string str = "Level";
    if (!(str == "Match"))
    {
      if (str == "ClosedBox")
        this.m_CheatManagerMenu.GetComponent<CheatMenu>().SetAsActiveTab(3);
      else
        this.m_CheatManagerMenu.GetComponent<CheatMenu>().SetAsActiveTab(3);
    }
    else
      this.m_CheatManagerMenu.GetComponent<CheatMenu>().SetAsActiveTab(0);
  }
}
