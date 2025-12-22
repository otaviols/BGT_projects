using UnityEngine;

public class OptionsMenuPhone : MonoBehaviour
{
  public OptionsMenu m_optionsMenu;
  public UIBButton m_doneButton;
  public GameObject m_mainContentsPanel;

  private void Start() => this.m_doneButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.m_optionsMenu.Hide()));
}
