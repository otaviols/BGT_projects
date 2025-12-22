using Assets;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class BaconEmoteShopPile : MonoBehaviour
{
  [SerializeField]
  private Widget m_topEmote;
  [SerializeField]
  private Widget m_widget;

  private void Start() => this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
  {
    if (!(eventName == "ORDER_PILE"))
      return;
    DataModelList<BattlegroundsEmoteDataModel> bgEmotePile = this.m_widget.GetDataModel<RewardItemDataModel>().BGEmotePile;
    bgEmotePile.Sort(new Comparison<BattlegroundsEmoteDataModel>(BaconEmoteShopPile.CompareBorders));
    this.m_topEmote.BindDataModel((IDataModel) bgEmotePile[0]);
    this.m_topEmote.TriggerEvent("DEFAULT_BOTTOM_LEFT");
    this.m_widget.TriggerEvent("2_NO_BUBBLE");
    if (bgEmotePile.Count > 1)
    {
      switch (bgEmotePile[1].BorderType)
      {
        case BattlegroundsEmote.Bordertype.NONE:
          this.m_widget.TriggerEvent("2_NO_BORDER");
          break;
        case BattlegroundsEmote.Bordertype.PURPLE:
          this.m_widget.TriggerEvent("2_PURPLE");
          break;
        default:
          Debug.LogError((object) "Unimplemented border selected.");
          this.m_widget.TriggerEvent("2_NO_BORDER");
          break;
      }
    }
    this.m_widget.TriggerEvent("3_NO_BUBBLE");
    if (bgEmotePile.Count <= 2)
      return;
    switch (bgEmotePile[2].BorderType)
    {
      case BattlegroundsEmote.Bordertype.NONE:
        this.m_widget.TriggerEvent("3_NO_BORDER");
        break;
      case BattlegroundsEmote.Bordertype.PURPLE:
        this.m_widget.TriggerEvent("3_PURPLE");
        break;
      default:
        Debug.LogError((object) "Unimplemented border selected.");
        this.m_widget.TriggerEvent("3_NO_BORDER");
        break;
    }
  }));

  private static int CompareBorders(BattlegroundsEmoteDataModel x, BattlegroundsEmoteDataModel y) => y.BorderType.CompareTo((object) x.BorderType);
}
