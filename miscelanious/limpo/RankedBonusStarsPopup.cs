using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RankedBonusStarsPopup : BasicPopup
{
  public UberText m_descriptionText;
  public UberText m_finePrintText;
  private const string SHOW_EVENT_NAME = "CODE_DIALOGMANAGER_SHOW";
  private const string HIDE_EVENT_NAME = "CODE_DIALOGMANAGER_HIDE";
  private const string HIDE_FINISHED_EVENT_NAME = "CODE_HIDE_FINISHED";
  private const string SETUP_SCENE_LOGIN = "SetUp_Scene_Login";
  private const string SETUP_SCENE_PLAYSCREEN = "SetUp_Scene_PlayScreen";
  private WidgetTemplate m_widget;

  protected override void Awake()
  {
    this.m_widget = this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (eventName == "Button_Framed_Clicked")
        this.Hide();
      if (!(eventName == "CODE_HIDE_FINISHED"))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }));
    this.m_widget.RegisterReadyListener((Action<object>) (_ => this.OnWidgetReady()), (object) null, true);
  }

  protected override void OnDestroy()
  {
    GameObject gameObject = this.transform.parent.gameObject;
    if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null && (UnityEngine.Object) gameObject.GetComponent<WidgetInstance>() != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.transform.parent.gameObject);
    base.OnDestroy();
  }

  public override void Show()
  {
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
      return;
    OverlayUI.Get().AddGameObject(this.gameObject, scaleMode: ((bool) UniversalInputManager.UsePhoneUI ? CanvasScaleMode.WIDTH : CanvasScaleMode.HEIGHT));
    UIContext.GetRoot().ShowPopup(this.gameObject);
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.LOGIN || SceneMgr.Get().GetMode() == SceneMgr.Mode.HUB)
      this.m_widget.TriggerEvent("SetUp_Scene_Login", new Widget.TriggerEventParameters());
    else if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT)
      this.m_widget.TriggerEvent("SetUp_Scene_PlayScreen", new Widget.TriggerEventParameters());
    this.m_widget.TriggerEvent("CODE_DIALOGMANAGER_SHOW", new Widget.TriggerEventParameters());
  }

  public override void Hide()
  {
    if (this.m_popupInfo is RankedBonusStarsPopup.BonusStarsPopupInfo popupInfo && popupInfo.m_onHiddenCallback != null)
      popupInfo.m_onHiddenCallback();
    this.m_widget.TriggerEvent("CODE_DIALOGMANAGER_HIDE", new Widget.TriggerEventParameters());
    this.IncrementBonusStarsPopupSeenCount();
  }

  private void OnWidgetReady()
  {
    IDataModel model = (IDataModel) null;
    this.m_widget.GetDataModel(123, out model);
    if (!(model is RankedPlayDataModel rankedPlayDataModel) || !((UnityEngine.Object) this.m_descriptionText != (UnityEngine.Object) null))
      return;
    this.m_descriptionText.Text = GameStrings.Format("GLUE_RANKED_BONUS_STARS_DESCRIPTION", (object) rankedPlayDataModel.StarMultiplier);
  }

  private void IncrementBonusStarsPopupSeenCount()
  {
    TranslatedMedalInfo currentMedal = RankMgr.Get().GetLocalPlayerMedalInfo().GetCurrentMedal(FormatType.FT_STANDARD);
    if (currentMedal == null)
      return;
    long num = 0;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_BONUS_STARS_POPUP_SEEN_COUNT, out num);
    GameSaveDataManager.Get().SaveSubkeys(new List<GameSaveDataManager.SubkeySaveRequest>()
    {
      new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_LAST_SEASON_BONUS_STARS_POPUP_SEEN, new long[1]
      {
        (long) currentMedal.seasonId
      }),
      new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_BONUS_STARS_POPUP_SEEN_COUNT, new long[1]
      {
        ++num
      })
    });
  }

  public class BonusStarsPopupInfo : BasicPopup.PopupInfo
  {
    public Action m_onHiddenCallback;
  }
}
