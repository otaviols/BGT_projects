using Blizzard.T5.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RankedPlayDisplay : MonoBehaviour
{
  [SerializeField]
  private Transform m_medalBone;
  [SerializeField]
  private VisualController m_rankContainerVisualController;
  [SerializeField]
  private Widget m_rewardsContainerWidget;
  [SerializeField]
  private AsyncReference m_rankedMedalWidgetReference;
  [SerializeField]
  private AsyncReference m_starMultiplierWidgetReference;
  [SerializeField]
  private TooltipZone m_starMultiplierTooltipZone;
  [SerializeField]
  private Vector3 m_rewardListPos;
  [SerializeField]
  private float m_rewardListDeviceScale = 1f;
  [SerializeField]
  private float m_rewardListScaleSmall = 1f;
  [SerializeField]
  private float m_rewardListScaleWide = 1f;
  [SerializeField]
  private float m_rewardListScaleExtraWide = 1f;
  [SerializeField]
  private List<PlayMakerFSM> formatChangeGlowFSMs;
  [SerializeField]
  private List<PlayMakerFSM> newDeckFormatChangeGlowFSMs;
  private bool m_inSetRotationTutorial;
  private VisualsFormatType m_currentVisualsFormatType;
  private RankedPlayDataModel m_rankedChestDataModel;
  private Widget m_starMultiplierWidget;
  private Widget m_rankedMedalWidget;
  private Widget m_widget;
  private WidgetInstance m_rankedRewardListWidget;
  private RankedRewardList m_rankedRewardList;
  private bool m_isShowingRewardsList;
  private bool m_isDesiredHidden;
  private Coroutine m_delayedVisibilityChange;
  private const string MEDAL_BUTTON_CLICKED = "MEDAL_BUTTON_CLICKED";
  private const string SHOW_MEDAL_TOOLTIP = "SHOW_MEDAL_TOOLTIP";
  private const string HIDE_MEDAL_TOOLTIP = "HIDE_MEDAL_TOOLTIP";
  private const string CHEST_BUTTON_CLICKED = "CHEST_BUTTON_CLICKED";
  private const string SHOW_CHEST_TOOLTIP = "SHOW_CHEST_TOOLTIP";
  private const string HIDE_CHEST_TOOLTIP = "HIDE_CHEST_TOOLTIP";
  private const string ENABLE_CLICKABLES = "POPUP_CLOSED_ENABLE_CLICKABLES";
  private const string DISABLE_CLICKABLES = "POPUP_OPEN_DISABLE_CLICKABLES";

  private void Awake()
  {
    this.m_currentVisualsFormatType = VisualsFormatTypeExtensions.ToVisualsFormatType(Options.GetFormatType(), Options.GetInRankedPlayMode());
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnRankedPlayDisplayEvent));
  }

  private void Start()
  {
    this.UpdateRankContainerVisualController();
    this.m_rewardsContainerWidget.RegisterReadyListener((Action<object>) (_ => this.UpdateRewardsContainerWidget()), (object) null, true);
    this.m_rankedMedalWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnRankedMedalWidgetReady));
    this.m_starMultiplierWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnStarMultiplierWidgetReady));
  }

  private void OnDestroy() => this.DestroyRankedRewardsList();

  public void UpdateMode(VisualsFormatType newVisualsFormatType)
  {
    DeckPickerTrayDisplay.Get().UpdateRankedClassWinsPlate();
    if ((bool) UniversalInputManager.UsePhoneUI)
      DeckPickerTrayDisplay.Get().ToggleRankedDetailsTray(newVisualsFormatType.IsRanked());
    DeckPickerTrayDisplay.Get().SetPlayButtonText(GameStrings.Get("GLOBAL_PLAY"));
  }

  public void StartSetRotationTutorial()
  {
    this.m_inSetRotationTutorial = true;
    if ((bool) UniversalInputManager.UsePhoneUI)
      DeckPickerTrayDisplay.Get().ToggleRankedDetailsTray(true);
    this.m_currentVisualsFormatType = VisualsFormatType.VFT_STANDARD;
    this.Hide();
    DeckPickerTrayDisplay.Get().SetPlayButtonText(GameStrings.Get("GLOBAL_PLAY"));
    DeckPickerTrayDisplay.Get().SetPlayButtonTextAlpha(0.0f);
    DeckPickerTrayDisplay.Get().UpdateRankedClassWinsPlate();
    this.OnMedalChanged(TournamentDisplay.Get().GetCurrentMedalInfo());
  }

  public void OnMedalChanged(NetCache.NetCacheMedalInfo medalInfo)
  {
    MedalInfoTranslator medalInfoTranslator = new MedalInfoTranslator(medalInfo);
    bool isTooltipEnabled = false;
    bool hasEarnedCardBack = medalInfoTranslator.HasEarnedSeasonCardBack();
    this.m_rankedChestDataModel = medalInfoTranslator.CreateDataModel(this.m_currentVisualsFormatType.ToFormatType(), RankedMedal.DisplayMode.Chest, isTooltipEnabled, hasEarnedCardBack);
    this.UpdateRankContainerVisualController();
    this.UpdateRewardsContainerWidget();
  }

  public void UpdateRankContainerVisualController()
  {
    NetCache.NetCacheMedalInfo currentMedalInfo = TournamentDisplay.Get().GetCurrentMedalInfo();
    if (currentMedalInfo == null)
      return;
    MedalInfoTranslator medalInfoTranslator = new MedalInfoTranslator(currentMedalInfo);
    bool isTooltipEnabled = false;
    bool hasEarnedCardBack = medalInfoTranslator.HasEarnedSeasonCardBack();
    RankedPlayDataModel dataModel = medalInfoTranslator.CreateDataModel(this.m_currentVisualsFormatType.ToFormatType(), RankedMedal.DisplayMode.Stars, isTooltipEnabled, hasEarnedCardBack);
    this.m_rankContainerVisualController.BindDataModel((IDataModel) dataModel);
    if (!(bool) UniversalInputManager.UsePhoneUI || !((UnityEngine.Object) this.m_rankedMedalWidget != (UnityEngine.Object) null))
      return;
    this.m_rankedMedalWidget.BindDataModel((IDataModel) dataModel);
  }

  public void UpdateRewardsContainerWidget()
  {
    if (!this.m_rewardsContainerWidget.IsReady || this.m_rankedChestDataModel == null)
      return;
    this.m_rewardsContainerWidget.BindDataModel((IDataModel) this.m_rankedChestDataModel);
    if (this.m_isDesiredHidden)
    {
      this.Hide();
    }
    else
    {
      if (!(bool) UniversalInputManager.UsePhoneUI)
        return;
      this.m_rewardsContainerWidget.SetLayerOverride(GameLayer.IgnoreFullScreenEffects);
    }
  }

  public void OnSwitchFormat(VisualsFormatType newVisualsFormatType)
  {
    if (this.m_inSetRotationTutorial)
      return;
    if (this.m_currentVisualsFormatType != newVisualsFormatType)
    {
      this.m_currentVisualsFormatType = newVisualsFormatType;
      this.OnMedalChanged(TournamentDisplay.Get().GetCurrentMedalInfo());
    }
    this.UpdateMode(newVisualsFormatType);
  }

  public void Show(float delay = 0.0f)
  {
    if (!this.m_isDesiredHidden)
      return;
    this.m_isDesiredHidden = false;
    this.StopAndClearCoroutine(ref this.m_delayedVisibilityChange);
    if ((double) delay > 0.0)
      this.m_delayedVisibilityChange = this.StartCoroutine(this.WaitThenSetVisibility(delay, true));
    else
      this.SetVisibility(true);
  }

  public void Hide(float delay = 0.0f)
  {
    if (this.m_isDesiredHidden)
      return;
    this.m_isDesiredHidden = true;
    this.StopAndClearCoroutine(ref this.m_delayedVisibilityChange);
    if ((double) delay > 0.0)
      this.StartCoroutine(this.WaitThenSetVisibility(delay, false));
    else
      this.SetVisibility(false);
  }

  private IEnumerator WaitThenSetVisibility(float delay, bool visible)
  {
    yield return (object) new WaitForSeconds(delay);
    this.SetVisibility(visible);
  }

  private void SetVisibility(bool visible)
  {
    if (visible)
    {
      this.m_widget.Show();
      this.m_rewardsContainerWidget.Show();
    }
    else
    {
      this.m_rewardsContainerWidget.Hide();
      this.m_widget.Hide();
    }
  }

  private void StopAndClearCoroutine(ref Coroutine co)
  {
    if (co == null)
      return;
    this.StopCoroutine(co);
    co = (Coroutine) null;
  }

  public void PlayTransitionGlowBurstsForNonNewDeckFSMs(string fxEvent)
  {
    foreach (PlayMakerFSM formatChangeGlowFsM in this.formatChangeGlowFSMs)
    {
      if ((UnityEngine.Object) formatChangeGlowFsM != (UnityEngine.Object) null)
        formatChangeGlowFsM.SendEvent(fxEvent);
    }
  }

  public void PlayTransitionGlowBurstsForNewDeckFSMs(string fxEvent)
  {
    if (string.IsNullOrEmpty(fxEvent))
      return;
    foreach (PlayMakerFSM formatChangeGlowFsM in this.newDeckFormatChangeGlowFSMs)
    {
      if ((UnityEngine.Object) formatChangeGlowFsM != (UnityEngine.Object) null)
        formatChangeGlowFsM.SendEvent(fxEvent);
    }
  }

  private void OnRankedMedalWidgetReady(Widget widget)
  {
    this.m_rankedMedalWidget = widget;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      widget.transform.parent = DeckPickerTrayDisplay.Get().m_medalBone_phone;
      widget.SetLayerOverride(GameLayer.IgnoreFullScreenEffects);
    }
    else
      widget.transform.parent = this.m_medalBone;
    widget.transform.localScale = Vector3.one;
    widget.transform.localPosition = Vector3.zero;
    this.OnMedalChanged(TournamentDisplay.Get().GetCurrentMedalInfo());
  }

  private void OnStarMultiplierWidgetReady(Widget widget) => this.m_starMultiplierWidget = widget;

  private void OnRankedPlayDisplayEvent(string eventName)
  {
    if (RankMgr.Get().IsNewPlayer())
      return;
    if (eventName == "MEDAL_BUTTON_CLICKED")
    {
      this.m_widget.TriggerEvent("POPUP_OPEN_DISABLE_CLICKABLES");
      DialogManager.Get().ShowRankedIntroPopUp((Action) (() => this.m_widget.TriggerEvent("POPUP_CLOSED_ENABLE_CLICKABLES")));
    }
    else if (eventName == "SHOW_MEDAL_TOOLTIP")
      this.ShowMedalTooltip();
    else if (eventName == "HIDE_MEDAL_TOOLTIP")
      this.HideMedalTooltip();
    if (eventName == "CHEST_BUTTON_CLICKED")
      this.StartCoroutine(this.ShowRankedRewardList());
    else if (eventName == "SHOW_CHEST_TOOLTIP")
    {
      this.ShowChestTooltip();
    }
    else
    {
      if (!(eventName == "HIDE_CHEST_TOOLTIP"))
        return;
      this.HideChestTooltip();
    }
  }

  private void ShowMedalTooltip()
  {
    FormatType formatType = Options.GetFormatType();
    string bodytext1;
    if (this.m_rankedChestDataModel.IsLegend)
    {
      bodytext1 = GameStrings.Format("GLOBAL_MEDAL_TOOLTIP_BODY_LEGEND");
    }
    else
    {
      string key;
      bodytext1 = !new Map<FormatType, string>()
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
      }.TryGetValue(formatType, out key) ? "UNKNOWN FORMAT TYPE " + formatType.ToString() : GameStrings.Format(key);
    }
    string key1;
    string headline1;
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
    }.TryGetValue(formatType, out key1))
      headline1 = GameStrings.Format(key1, (object) this.m_rankedChestDataModel.RankName);
    else
      headline1 = "UNKNOWN FORMAT TYPE " + this.m_rankedChestDataModel.FormatType.ToString();
    TooltipZone component = this.m_rankContainerVisualController.GetComponent<TooltipZone>();
    component.ShowTooltip(headline1, bodytext1, 5f);
    int starsPerWin = RankMgr.Get().GetLocalPlayerMedalInfo().GetCurrentMedal(formatType).starsPerWin;
    if (starsPerWin <= 1)
      return;
    string headline2 = GameStrings.Format("GLUE_TOURNAMENT_STAR_MULT_HEAD", (object) starsPerWin);
    string bodytext2 = GameStrings.Format("GLUE_TOURNAMENT_STAR_MULT_BODY", (object) starsPerWin);
    if (!((UnityEngine.Object) this.m_starMultiplierTooltipZone != (UnityEngine.Object) null))
      return;
    this.m_starMultiplierTooltipZone.ShowTooltip(headline2, bodytext2, 5f);
    this.m_starMultiplierTooltipZone.AnchorTooltipTo(component.GetTooltipObject(), Anchor.BOTTOM_XZ, Anchor.TOP_XZ);
  }

  private void HideMedalTooltip()
  {
    this.m_rankContainerVisualController.GetComponent<TooltipZone>().HideTooltip();
    if (!((UnityEngine.Object) this.m_starMultiplierTooltipZone != (UnityEngine.Object) null))
      return;
    this.m_starMultiplierTooltipZone.HideTooltip();
  }

  private IEnumerator ShowRankedRewardList()
  {
    RankedPlayDisplay rankedPlayDisplay = this;
    if (!rankedPlayDisplay.m_isShowingRewardsList)
    {
      rankedPlayDisplay.m_widget.TriggerEvent("POPUP_OPEN_DISABLE_CLICKABLES");
      rankedPlayDisplay.m_isShowingRewardsList = true;
      if ((UnityEngine.Object) rankedPlayDisplay.m_rankedRewardListWidget == (UnityEngine.Object) null)
      {
        rankedPlayDisplay.m_rankedRewardListWidget = WidgetInstance.Create((string) RankMgr.RANKED_REWARD_LIST_POPUP);
        // ISSUE: reference to a compiler-generated method
        rankedPlayDisplay.m_rankedRewardListWidget.RegisterReadyListener(new Action<object>(rankedPlayDisplay.\u003CShowRankedRewardList\u003Eb__53_0), (object) null, true);
        rankedPlayDisplay.m_rankedRewardListWidget.WillLoadSynchronously = true;
        rankedPlayDisplay.m_rankedRewardListWidget.Initialize();
      }
      while ((UnityEngine.Object) rankedPlayDisplay.m_rankedRewardList == (UnityEngine.Object) null || rankedPlayDisplay.m_rankedRewardListWidget.IsChangingStates)
        yield return (object) null;
      UIContext.GetRoot().ShowPopup(rankedPlayDisplay.m_rankedRewardListWidget.gameObject);
      rankedPlayDisplay.m_rankedRewardListWidget.Show();
      rankedPlayDisplay.m_rankedRewardListWidget.TriggerEvent("SHOW", new Widget.TriggerEventParameters());
      yield return (object) new WaitForSeconds(0.25f);
    }
  }

  private void OnRankedRewardListPopupWidgetReady()
  {
    OverlayUI.Get().AddGameObject(this.m_rankedRewardListWidget.gameObject);
    this.m_rankedRewardListWidget.transform.localPosition = this.m_rewardListPos;
    this.m_rankedRewardListWidget.transform.localScale = Vector3.one * TransformUtil.GetAspectRatioDependentValue(this.m_rewardListScaleSmall, this.m_rewardListScaleWide, this.m_rewardListScaleExtraWide) * this.m_rewardListDeviceScale;
    this.m_rankedRewardListWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.WidgetEventListener_RewardsList));
    this.m_rankedRewardList = this.m_rankedRewardListWidget.GetComponentInChildren<RankedRewardList>();
    this.m_rankedRewardListWidget.Hide();
    this.UpdateRankedRewardList();
  }

  private void WidgetEventListener_RewardsList(string eventName)
  {
    if (!eventName.Equals("HIDE"))
      return;
    this.HideRankedRewardsList();
  }

  private void HideRankedRewardsList()
  {
    this.m_widget.TriggerEvent("POPUP_CLOSED_ENABLE_CLICKABLES");
    UIContext.GetRoot().DismissPopup(this.m_rankedRewardListWidget.gameObject);
    this.m_isShowingRewardsList = false;
  }

  private void UpdateRankedRewardList()
  {
    if (!((UnityEngine.Object) this.m_rankedRewardList != (UnityEngine.Object) null))
      return;
    this.m_rankedRewardList.Initialize(new MedalInfoTranslator(TournamentDisplay.Get().GetCurrentMedalInfo()));
  }

  private void DestroyRankedRewardsList()
  {
    if ((UnityEngine.Object) this.m_rankedRewardListWidget != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_rankedRewardListWidget.gameObject);
    this.m_isShowingRewardsList = false;
  }

  private void ShowChestTooltip() => this.m_rewardsContainerWidget.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get("GLOBAL_PROGRESSION_RANKED_REWARDS_TOOLTIP_TITLE"), GameStrings.Get("GLOBAL_PROGRESSION_RANKED_REWARDS_TOOLTIP"), 5f);

  private void HideChestTooltip() => this.m_rewardsContainerWidget.GetComponent<TooltipZone>().HideTooltip();
}
