using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using UnityEngine;

public class SelectableMedal : MonoBehaviour
{
  [SerializeField]
  private Widget m_selectableMedalWidget;
  [SerializeField]
  private Widget m_rankedMedalWidget;
  [SerializeField]
  private Widget m_battlegroundsMedalWidget;
  private BattlegroundsMedalDataModel m_battlegroundsDataModel;
  private RankedPlayDataModel m_rankedDataModel;
  private bool m_bgDataModelBound;
  private bool m_rankedDataModelBound;

  private void Awake() => this.m_selectableMedalWidget.WillLoadSynchronously = true;

  public void UpdateWidget(
    BnetPlayer player,
    Action onDisplayBgMedal = null,
    Action onDisplayRankedMedal = null,
    Action onDisplayNoMedal = null)
  {
    MedalInfoTranslator rankPresenceField = RankMgr.Get().GetRankedMedalFromRankPresenceField(player);
    int bgRating;
    if (RankMgr.Get().GetBattlegroundsMedalFromRankPresenceField(player?.GetHearthstoneGameAccount(), out bgRating))
    {
      if (this.m_battlegroundsDataModel == null)
        this.m_battlegroundsDataModel = new BattlegroundsMedalDataModel();
      this.m_battlegroundsDataModel.Rating = bgRating;
      if (this.m_rankedDataModelBound)
      {
        this.m_rankedMedalWidget.gameObject.SetActive(false);
        this.m_selectableMedalWidget.UnbindDataModel(123);
        this.m_rankedDataModelBound = false;
      }
      if (!this.m_bgDataModelBound)
      {
        this.m_battlegroundsMedalWidget.gameObject.SetActive(true);
        this.m_selectableMedalWidget.BindDataModel((IDataModel) this.m_battlegroundsDataModel);
        this.m_bgDataModelBound = true;
      }
      this.m_selectableMedalWidget.Show();
      if (onDisplayBgMedal == null)
        return;
      onDisplayBgMedal();
    }
    else if (rankPresenceField != null && rankPresenceField.IsDisplayable())
    {
      rankPresenceField.CreateOrUpdateDataModel(rankPresenceField.GetBestCurrentRankFormatType(), ref this.m_rankedDataModel, RankedMedal.DisplayMode.Default);
      if (this.m_bgDataModelBound)
      {
        this.m_battlegroundsMedalWidget.gameObject.SetActive(false);
        this.m_selectableMedalWidget.UnbindDataModel(999);
        this.m_bgDataModelBound = false;
      }
      if (!this.m_rankedDataModelBound)
      {
        this.m_rankedMedalWidget.gameObject.SetActive(true);
        this.m_selectableMedalWidget.BindDataModel((IDataModel) this.m_rankedDataModel);
        this.m_rankedDataModelBound = true;
      }
      this.m_selectableMedalWidget.Show();
      if (onDisplayRankedMedal == null)
        return;
      onDisplayRankedMedal();
    }
    else
    {
      this.m_selectableMedalWidget.Hide();
      if (onDisplayNoMedal == null)
        return;
      onDisplayNoMedal();
    }
  }
}
