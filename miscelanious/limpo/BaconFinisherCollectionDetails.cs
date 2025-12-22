using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

public class BaconFinisherCollectionDetails : BaconVideoCollectionDetails
{
  [SerializeField]
  private VisualController m_favoriteButtonController;
  public FinisherVideoCaptionDriver Captions;
  private BattlegroundsFinisherDataModel m_dataModel;
  private BattlegroundsFinisherCollectionPageDataModel m_pageDataModel;

  protected override string DebugTextValue => string.Format("Finisher ID: {0}", (object) this.m_dataModel?.FinisherDbiId);

  public override void AssignDataModels(IDataModel dataModel, IDataModel pageDataModel)
  {
    this.m_dataModel = dataModel as BattlegroundsFinisherDataModel;
    this.m_pageDataModel = pageDataModel as BattlegroundsFinisherCollectionPageDataModel;
    this.m_widget.BindDataModel(dataModel);
  }

  public override void Show()
  {
    base.Show();
    this.ToggleFavoriteButton();
  }

  public override void Hide()
  {
    base.Hide();
    EventFunctions.TriggerEvent(this.m_favoriteButtonController.transform, "DEFAULT_VISIBILITY");
    if ((Object) this.Captions != (Object) null)
      this.Captions.OnClose();
    UIContext.GetRoot().DismissPopup(this.transform.parent.gameObject);
  }

  private void ToggleFavoriteButton()
  {
    if (this.m_dataModel == null)
      return;
    if ((this.m_dataModel.IsFavorite ? 0 : (this.m_dataModel.IsOwned ? 1 : 0)) != 0)
      EventFunctions.TriggerEvent(this.m_favoriteButtonController.transform, "ENABLE_FAVORITE_BUTTON");
    else
      EventFunctions.TriggerEvent(this.m_favoriteButtonController.transform, "DISABLE_FAVORITE_BUTTON");
  }

  private void MakeFavorite()
  {
    if (this.m_dataModel == null)
      return;
    if (this.m_dataModel.IsFavorite || !this.m_dataModel.IsOwned)
    {
      Error.AddDevFatal("BaconFinisherCollectionDetails.MakeFavorite: Should not have been called for already-favorite or un-owned finisher");
    }
    else
    {
      if (BattlegroundsFinisherId.IsDefaultFinisherId(this.m_dataModel.FinisherDbiId))
        Network.Get().ClearBattlegroundsFavoriteFinisher();
      else
        Network.Get().SetBattlegroundsFavoriteFinisher(BattlegroundsFinisherId.FromTrustedValue(this.m_dataModel.FinisherDbiId));
      foreach (BattlegroundsFinisherDataModel finisher in this.m_pageDataModel.FinisherList)
        finisher.IsFavorite = finisher == this.m_dataModel;
    }
  }

  protected override bool ValidateDataModels(IDataModel dataModel, IDataModel pageDataModel) => dataModel is BattlegroundsFinisherDataModel && pageDataModel is BattlegroundsFinisherCollectionPageDataModel;

  protected override void ClearDataModels()
  {
    this.m_dataModel = (BattlegroundsFinisherDataModel) null;
    this.m_pageDataModel = (BattlegroundsFinisherCollectionPageDataModel) null;
  }

  protected override void DetailsEventListener(string eventName)
  {
    if (!(eventName == "OffDialogClick_code"))
    {
      if (eventName == "MakeFavorite_code")
        this.MakeFavorite();
      else
        Debug.LogWarning((object) ("Unrecognized event handled in " + ((object) this).GetType().Name + ": " + eventName));
    }
    else
    {
      if (!this.CanHide())
        return;
      this.Hide();
    }
  }
}
