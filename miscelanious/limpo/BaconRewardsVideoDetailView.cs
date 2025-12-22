using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class BaconRewardsVideoDetailView : MonoBehaviour
{
  private BaconBoardCollectionDetails m_boardDetailsDisplay;
  private BaconFinisherCollectionDetails m_finisherDetailsDisplay;
  public VisualController MainVisualController;
  public Widget m_BoardSkinsWidget;
  public AsyncReference m_boardDisplayReference;
  private Widget m_BoardSkinsWidgetInstance;
  public Widget m_FinishersWidget;
  public AsyncReference m_finisherDisplayReference;
  private Widget m_FinisherWidgetInstance;
  private RewardItemDataModel m_CurrentRewardItem;
  private WidgetTemplate m_widget;

  private void Start()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
      Log.Gameplay.PrintError("Video Details View isn't a widget");
    else if ((UnityEngine.Object) this.MainVisualController == (UnityEngine.Object) null)
      Log.Gameplay.PrintError("Main visual controller is null.");
    else if (this.m_boardDisplayReference == null)
      Log.Gameplay.PrintError("Board display reference is null.");
    else if (this.m_finisherDisplayReference == null)
    {
      Log.Gameplay.PrintError("Finisher display reference is null.");
    }
    else
    {
      this.m_boardDisplayReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnBoardDisplayReady));
      this.m_finisherDisplayReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnFinisherDisplayReady));
      this.MainVisualController.RegisterDoneChangingStatesListener(new Action<object>(this.ShowReward), (object) null, true, false);
    }
  }

  private void OnBoardDisplayReady(Widget widget)
  {
    this.m_BoardSkinsWidgetInstance = widget;
    this.m_boardDetailsDisplay = this.m_BoardSkinsWidgetInstance.GetComponentInChildren<BaconBoardCollectionDetails>();
  }

  private void OnFinisherDisplayReady(Widget widget)
  {
    this.m_FinisherWidgetInstance = widget;
    this.m_finisherDetailsDisplay = this.m_FinisherWidgetInstance.GetComponentInChildren<BaconFinisherCollectionDetails>();
  }

  private void ShowReward(object o = null)
  {
    this.m_CurrentRewardItem = this.m_widget.GetDataModel<RewardItemDataModel>();
    if (this.m_CurrentRewardItem == null)
      return;
    switch (this.m_CurrentRewardItem.ItemType)
    {
      case RewardItemType.BATTLEGROUNDS_BOARD_SKIN:
        this.ShowBoardSkinReward();
        break;
      case RewardItemType.BATTLEGROUNDS_FINISHER:
        this.ShowFinisherReward();
        break;
    }
  }

  private void ShowBoardSkinReward()
  {
    if (this.m_CurrentRewardItem.BGBoardSkin == null || !((UnityEngine.Object) this.m_boardDetailsDisplay != (UnityEngine.Object) null))
      return;
    this.m_boardDetailsDisplay.ClearVideo();
    this.m_boardDetailsDisplay.gameObject.SetActive(true);
    this.m_boardDetailsDisplay.AssignDataModels((IDataModel) this.m_CurrentRewardItem.BGBoardSkin, (IDataModel) null);
    this.m_boardDetailsDisplay.Show();
  }

  private void ShowFinisherReward()
  {
    if (this.m_CurrentRewardItem.BGFinisher == null || !((UnityEngine.Object) this.m_finisherDetailsDisplay != (UnityEngine.Object) null))
      return;
    this.m_finisherDetailsDisplay.ClearVideo();
    this.m_finisherDetailsDisplay.gameObject.SetActive(true);
    this.m_finisherDetailsDisplay.AssignDataModels((IDataModel) this.m_CurrentRewardItem.BGFinisher, (IDataModel) null);
    this.m_finisherDetailsDisplay.Show();
  }
}
