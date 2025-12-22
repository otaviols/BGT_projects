using Hearthstone.UI;

public class LuckyDrawEmoteDetails : BaconCollectionDetails
{
  protected override string DebugTextValue => "Debug Text";

  protected override void Start()
  {
    base.Start();
    this.gameObject.SetActive(true);
  }

  public override void AssignDataModels(IDataModel dataModel, IDataModel pageDataModel)
  {
  }

  protected override void ClearDataModels()
  {
  }

  protected override void DetailsEventListener(string eventName)
  {
  }

  protected override bool ValidateDataModels(IDataModel dataModel, IDataModel pageDataModel) => true;
}
