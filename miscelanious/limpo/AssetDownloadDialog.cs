using Hearthstone.Streaming;
using UnityEngine;

public class AssetDownloadDialog : DialogBase
{
  public DownloadStatusView DownloadStatusView;
  public NestedPrefab EnableDownloadButton;
  public NestedPrefab CellularDataButton;
  public PegUIElement ClickCatcher;
  private PegUIElement m_blocker;
  private UIBButton m_enableDownloadButton;
  private UIBButton m_cellularDataButton;

  private IGameDownloadManager DownloadManager => GameDownloadManagerProvider.Get();

  private void Start()
  {
    this.m_enableDownloadButton = this.EnableDownloadButton.GetComponentInChildren<UIBButton>();
    this.m_cellularDataButton = this.CellularDataButton.GetComponentInChildren<UIBButton>();
    this.m_enableDownloadButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDownloadButtonRelease));
    this.m_cellularDataButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => DownloadPermissionManager.CellularEnabled = !DownloadPermissionManager.CellularEnabled));
    this.ClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputCatcherRelease));
    LayerUtils.SetLayer((Component) this.m_enableDownloadButton, GameLayer.HighPriorityUI);
    LayerUtils.SetLayer((Component) this.m_cellularDataButton, GameLayer.HighPriorityUI);
  }

  private void OnDownloadButtonRelease(UIEvent e)
  {
    DownloadPermissionManager.DownloadEnabled = !DownloadPermissionManager.DownloadEnabled;
    if (DownloadPermissionManager.DownloadEnabled)
      this.DownloadManager.StartUpdateProcessForOptional();
    else
      this.DownloadManager.StopOptionalDownloads();
  }

  private void OnInputCatcherRelease(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    this.Hide();
  }

  public override void Show()
  {
    base.Show();
    this.DoShowAnimation();
    DialogBase.DoBlur();
  }

  public override void Hide()
  {
    base.Hide();
    DialogBase.EndBlur();
  }

  private void Update()
  {
    if (this.DownloadManager != null)
    {
      AssetDownloadDialog.SetButtonTextForState(this.m_enableDownloadButton, DownloadPermissionManager.DownloadEnabled);
      AssetDownloadDialog.SetButtonTextForState(this.m_cellularDataButton, DownloadPermissionManager.CellularEnabled);
    }
    if (!((Object) GameMenu.Get() != (Object) null) || !GameMenu.Get().IsShown() || !this.IsShown())
      return;
    this.Hide();
  }

  private static void SetButtonTextForState(UIBButton button, bool enabled)
  {
    if (!((Object) button != (Object) null))
      return;
    button.SetText(GameStrings.Get(enabled ? "GLOBAL_ASSET_DOWNLOAD_ENABLED" : "GLOBAL_ASSET_DOWNLOAD_DISABLED"));
  }

  public class Info
  {
  }
}
