using System.Collections.Generic;
using UnityEngine;

public class CollectionTeamInfo : MonoBehaviour
{
  public GameObject m_root;
  public PegUIElement m_offClicker;
  private bool m_wasTouchModeEnabled;
  protected bool m_shown = true;
  private List<CollectionTeamInfo.ShowListener> m_showListeners = new List<CollectionTeamInfo.ShowListener>();
  private List<CollectionTeamInfo.HideListener> m_hideListeners = new List<CollectionTeamInfo.HideListener>();

  private void Awake() => this.m_wasTouchModeEnabled = true;

  private void Start()
  {
    this.m_offClicker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClosePressed));
    this.m_offClicker.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OverOffClicker));
  }

  private void Update()
  {
    if (this.m_wasTouchModeEnabled == UniversalInputManager.Get().IsTouchMode())
      return;
    this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
    this.m_offClicker.gameObject.SetActive(true);
  }

  public void Show()
  {
    if (this.m_shown)
      return;
    this.m_root.SetActive(true);
    this.m_shown = true;
    if (UniversalInputManager.Get().IsTouchMode())
      Navigation.Push(new Navigation.NavigateBackHandler(this.GoBackImpl));
    foreach (CollectionTeamInfo.ShowListener showListener in this.m_showListeners.ToArray())
      showListener();
  }

  private bool GoBackImpl()
  {
    this.Hide();
    return true;
  }

  public void Hide()
  {
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.GoBackImpl));
    if (!this.m_shown)
      return;
    this.m_root.SetActive(false);
    this.m_shown = false;
    foreach (CollectionTeamInfo.HideListener hideListener in this.m_hideListeners.ToArray())
      hideListener();
  }

  public void RegisterShowListener(CollectionTeamInfo.ShowListener dlg) => this.m_showListeners.Add(dlg);

  public void UnregisterShowListener(CollectionTeamInfo.ShowListener dlg) => this.m_showListeners.Remove(dlg);

  public void RegisterHideListener(CollectionTeamInfo.HideListener dlg) => this.m_hideListeners.Add(dlg);

  public void UnregisterHideListener(CollectionTeamInfo.HideListener dlg) => this.m_hideListeners.Remove(dlg);

  public bool IsShown() => this.m_shown;

  private void OnClosePressed(UIEvent e) => this.Hide();

  private void OverOffClicker(UIEvent e) => this.Hide();

  public delegate void ShowListener();

  public delegate void HideListener();
}
