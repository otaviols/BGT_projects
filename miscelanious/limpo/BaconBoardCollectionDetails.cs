using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using UnityEngine;

public class BaconBoardCollectionDetails : BaconVideoCollectionDetails
{
  [SerializeField]
  private VisualController m_favoriteButtonController;
  private BattlegroundsBoardSkinDataModel m_dataModel;
  private BattlegroundsBoardSkinCollectionPageDataModel m_pageDataModel;

  protected override string DebugTextValue => string.Format("Board ID: {0}", (object) this.m_dataModel?.BoardDbiId);

  public override void AssignDataModels(IDataModel dataModel, IDataModel pageDataModel)
  {
    this.m_dataModel = dataModel as BattlegroundsBoardSkinDataModel;
    this.m_pageDataModel = pageDataModel as BattlegroundsBoardSkinCollectionPageDataModel;
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

  protected void MakeFavorite()
  {
    if (this.m_dataModel == null)
      return;
    if (this.m_dataModel.IsFavorite || !this.m_dataModel.IsOwned)
    {
      Error.AddDevFatal("BaconBoardCollectionDetails.MakeFavorite: Should not have been called for already-favorite or un-owned board");
    }
    else
    {
      if (BattlegroundsBoardSkinId.IsDefaultBoardId(this.m_dataModel.BoardDbiId))
        Network.Get().ClearBattlegroundsFavoriteBoardSkin();
      else
        Network.Get().SetBattlegroundsFavoriteBoardSkin(BattlegroundsBoardSkinId.FromTrustedValue(this.m_dataModel.BoardDbiId));
      foreach (BattlegroundsBoardSkinDataModel boardSkin in this.m_pageDataModel.BoardSkinList)
        boardSkin.IsFavorite = boardSkin == this.m_dataModel;
    }
  }

  protected override bool ValidateDataModels(IDataModel dataModel, IDataModel pageDataModel) => dataModel is BattlegroundsBoardSkinDataModel && pageDataModel is BattlegroundsBoardSkinCollectionPageDataModel;

  protected override void ClearDataModels()
  {
    this.m_dataModel = (BattlegroundsBoardSkinDataModel) null;
    this.m_pageDataModel = (BattlegroundsBoardSkinCollectionPageDataModel) null;
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
