using Hearthstone.DataModels;

public class StaticWidgetCardCache : StaticWidgetCache<CardDataModel>
{
  public override string GetUniqueIdentifier(CardDataModel dataModel) => dataModel.CardId;
}
