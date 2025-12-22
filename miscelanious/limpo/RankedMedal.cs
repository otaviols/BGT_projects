using Blizzard.T5.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RankedMedal : MonoBehaviour
{
  private TooltipZone m_tooltipZone;
  private Widget m_widget;

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.WidgetEventListener));
    this.m_tooltipZone = this.GetComponent<TooltipZone>();
  }

  public void BindRankedPlayDataModel(RankedPlayDataModel dataModel)
  {
    if (dataModel == this.GetRankedPlayDataModel())
      return;
    this.m_widget.BindDataModel((IDataModel) dataModel);
  }

  private RankedPlayDataModel GetRankedPlayDataModel()
  {
    IDataModel model = (IDataModel) null;
    this.m_widget.GetDataModel(123, out model);
    return model as RankedPlayDataModel;
  }

  private void WidgetEventListener(string eventName)
  {
    if (eventName.Equals("RollOver"))
    {
      this.OnRollOver();
    }
    else
    {
      if (!eventName.Equals("RollOut"))
        return;
      this.OnRollOut();
    }
  }

  private void OnRollOver()
  {
    RankedPlayDataModel rankedPlayDataModel = this.GetRankedPlayDataModel();
    if (rankedPlayDataModel == null || !rankedPlayDataModel.IsTooltipEnabled)
      return;
    string bodytext = "";
    string headline = "";
    if (Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE) || SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
    {
      FormatType formatType;
      if (rankedPlayDataModel.IsLegend)
      {
        bodytext = GameStrings.Format("GLOBAL_MEDAL_TOOLTIP_BODY_LEGEND");
      }
      else
      {
        string key;
        if (new Map<FormatType, string>()
        {
          {
            FormatType.FT_STANDARD,
            "GLOBAL_MEDAL_TOOLTIP_BODY_STANDARD"
          },
          {
            FormatType.FT_WILD,
            "GLOBAL_MEDAL_TOOLTIP_BODY_WILD"
          },
          {
            FormatType.FT_CLASSIC,
            "GLOBAL_MEDAL_TOOLTIP_BODY_CLASSIC"
          }
        }.TryGetValue(rankedPlayDataModel.FormatType, out key))
        {
          bodytext = GameStrings.Format(key);
        }
        else
        {
          formatType = rankedPlayDataModel.FormatType;
          bodytext = "UNKNOWN FORMAT TYPE " + formatType.ToString();
        }
      }
      string key1;
      if (new Map<FormatType, string>()
      {
        {
          FormatType.FT_STANDARD,
          "GLOBAL_MEDAL_TOOLTIP_BEST_RANK_STANDARD"
        },
        {
          FormatType.FT_WILD,
          "GLOBAL_MEDAL_TOOLTIP_BEST_RANK_WILD"
        },
        {
          FormatType.FT_CLASSIC,
          "GLOBAL_MEDAL_TOOLTIP_BEST_RANK_CLASSIC"
        }
      }.TryGetValue(rankedPlayDataModel.FormatType, out key1))
      {
        headline = GameStrings.Format(key1, (object) rankedPlayDataModel.RankName);
      }
      else
      {
        formatType = rankedPlayDataModel.FormatType;
        headline = "UNKNOWN FORMAT TYPE " + formatType.ToString();
      }
    }
    this.m_tooltipZone.ShowLayerTooltip(headline, bodytext);
    TooltipPanel tooltipPanel = this.m_tooltipZone.GetTooltipPanel();
    if (!(bool) (Object) tooltipPanel)
      return;
    tooltipPanel.m_name.WordWrap = false;
    tooltipPanel.m_name.Cache = false;
    tooltipPanel.m_name.UpdateNow();
  }

  private void OnRollOut() => this.m_tooltipZone.HideTooltip();

  public enum DisplayMode
  {
    Default,
    Stars,
    Chest,
  }
}
