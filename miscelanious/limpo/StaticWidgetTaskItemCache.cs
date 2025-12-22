using Hearthstone.DataModels;
using UnityEngine;

public class StaticWidgetTaskItemCache : StaticWidgetCache<MercenaryVillageTaskItemDataModel>
{
  private static StaticWidgetTaskItemCache m_instance;

  public static StaticWidgetTaskItemCache Get() => StaticWidgetTaskItemCache.m_instance;

  private void Awake()
  {
    if ((Object) StaticWidgetTaskItemCache.m_instance == (Object) null)
      StaticWidgetTaskItemCache.m_instance = this;
    else
      Object.DestroyImmediate((Object) this);
  }

  public override string GetUniqueIdentifier(MercenaryVillageTaskItemDataModel dataModel) => dataModel.MercenaryCard.CardId;
}
