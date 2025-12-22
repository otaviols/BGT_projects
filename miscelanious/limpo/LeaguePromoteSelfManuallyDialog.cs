using Hearthstone.UI;
using UnityEngine;

public class LeaguePromoteSelfManuallyDialog : DialogBase
{
  public UIBButton m_cancelButton;
  public UIBButton m_confirmButton;
  private LeaguePromoteSelfManuallyDialog.ResponseCallback m_responseCallback;

  private void Start()
  {
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonPress));
    this.m_confirmButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnConfirmButtonPress));
  }

  protected override void OnDestroy()
  {
    PopupRoot component = this.gameObject.GetComponent<PopupRoot>();
    if ((bool) (Object) component)
      component.DisablePopupRendering();
    base.OnDestroy();
  }

  public override void Show()
  {
    base.Show();
    UIContext.GetRoot().ShowPopup(this.gameObject);
    BnetBar.Get().DisableButtonsByDialog((DialogBase) this);
    SoundManager.Get().LoadAndPlay((AssetReference) "Expand_Up.prefab:775d97ea42498c044897f396362b9db3");
    this.DoShowAnimation();
  }

  public override void Hide()
  {
    base.Hide();
    SoundManager.Get().LoadAndPlay((AssetReference) "Shrink_Down.prefab:a6d5184049ac041418cd5896e7d9a87a");
    UIContext.GetRoot().DismissPopup(this.gameObject);
  }

  public void SetInfo(LeaguePromoteSelfManuallyDialog.Info info) => this.m_responseCallback = info.m_callback;

  private void OnCancelButtonPress(UIEvent e) => this.Hide();

  private void OnConfirmButtonPress(UIEvent e)
  {
    this.m_responseCallback();
    this.Hide();
  }

  public delegate void ResponseCallback();

  public class Info
  {
    public LeaguePromoteSelfManuallyDialog.ResponseCallback m_callback;
  }
}
