using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Linq;
using UnityEngine;

public class MiniSetLayout : MonoBehaviour
{
  public Widget m_widget;

  public static MiniSetDbfRecord GetDbfRecord(ProductDataModel product)
  {
    int itemId = product.Items.First<RewardItemDataModel>().ItemId;
    return GameDbf.MiniSet.GetRecord(itemId);
  }

  private void Start()
  {
    DeckDbfRecord deckRecord = MiniSetLayout.GetDbfRecord(this.m_widget.GetDataModel<ProductDataModel>()).DeckRecord;
    DefLoader loader = DefLoader.Get();
    this.m_widget.BindDataModel((IDataModel) new RewardListDataModel()
    {
      Items = deckRecord.Cards.Select<DeckCardDbfRecord, EntityDef>((Func<DeckCardDbfRecord, EntityDef>) (c => loader.GetEntityDef(c.CardId))).Where<EntityDef>((Func<EntityDef, bool>) (ed => ed.GetRarity() == TAG_RARITY.LEGENDARY)).OrderBy<EntityDef, int>((Func<EntityDef, int>) (ed => ed.GetCost())).Select<EntityDef, RewardItemDataModel>((Func<EntityDef, RewardItemDataModel>) (ed => new RewardItemDataModel()
      {
        ItemType = RewardItemType.CARD,
        Card = new CardDataModel()
        {
          CardId = ed.GetCardId()
        }
      })).Append<RewardItemDataModel>(new RewardItemDataModel()
      {
        ItemType = RewardItemType.MINI_SET
      }).ToDataModelList<RewardItemDataModel>()
    });
  }
}
