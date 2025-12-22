using Hearthstone.UI;
using PegasusShared;
using System;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class RankedIntroPopup : BasicPopup
{
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
      if (!(eventName == "CODE_HIDE_FINISHED") || this.m_readyToDestroyCallback == null)
        return;
      this.m_readyToDestroyCallback((DialogBase) this);
    }));
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
    if (this.m_popupInfo is RankedIntroPopup.RankedIntroPopupInfo popupInfo && popupInfo.m_onHiddenCallback != null)
      popupInfo.m_onHiddenCallback();
    this.m_widget.TriggerEvent("CODE_DIALOGMANAGER_HIDE", new Widget.TriggerEventParameters());
    this.IncrementRankedIntroPopupSeenCount();
  }

  private void IncrementRankedIntroPopupSeenCount()
  {
    if (RankMgr.Get().GetLocalPlayerMedalInfo().GetCurrentMedal(FormatType.FT_STANDARD) == null)
      return;
    long num1 = 0;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_INTRO_SEEN_COUNT, out num1);
    long num2;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_INTRO_SEEN_COUNT, new long[1]
    {
      num2 = num1 + 1L
    }));
  }

  public class RankedIntroPopupInfo : BasicPopup.PopupInfo
  {
    public Action m_onHiddenCallback;
  }
}
