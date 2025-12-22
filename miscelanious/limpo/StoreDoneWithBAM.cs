using System.Collections.Generic;

public class StoreDoneWithBAM : UIBPopup
{
  public UIBButton m_okayButton;
  public UberText m_headlineText;
  public UberText m_messageText;
  private List<StoreDoneWithBAM.ButtonPressedListener> m_okayListeners = new List<StoreDoneWithBAM.ButtonPressedListener>();

  protected override void Awake()
  {
    base.Awake();
    this.m_okayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOkayPressed));
  }

  public void RegisterOkayListener(StoreDoneWithBAM.ButtonPressedListener listener)
  {
    if (this.m_okayListeners.Contains(listener))
      return;
    this.m_okayListeners.Add(listener);
  }

  public void RemoveOkayListener(StoreDoneWithBAM.ButtonPressedListener listener) => this.m_okayListeners.Remove(listener);

  private void OnOkayPressed(UIEvent e)
  {
    this.Hide(true);
    foreach (StoreDoneWithBAM.ButtonPressedListener buttonPressedListener in this.m_okayListeners.ToArray())
      buttonPressedListener();
  }

  public delegate void ButtonPressedListener();
}
