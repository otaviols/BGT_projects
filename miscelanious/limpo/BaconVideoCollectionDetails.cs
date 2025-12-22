using Hearthstone.UI;
using UnityEngine;

public abstract class BaconVideoCollectionDetails : BaconCollectionDetails
{
  [SerializeField]
  private VisualController m_videoPreviewController;
  [SerializeField]
  private DynamicVideoLoader m_videoPreview;

  public override void Show()
  {
    base.Show();
    EventFunctions.TriggerEvent(this.m_videoPreviewController.transform, "LOAD_VIDEO");
  }

  public void ClearVideo()
  {
    EventFunctions.TriggerEvent(this.m_videoPreviewController.transform, "CLEAR_VIDEO");
    if (!((Object) this.m_videoPreview != (Object) null))
      return;
    this.m_videoPreview.OnClosed();
  }

  public override void Hide()
  {
    base.Hide();
    this.ClearVideo();
  }
}
