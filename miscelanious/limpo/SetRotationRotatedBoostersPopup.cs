using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class SetRotationRotatedBoostersPopup : BasicPopup
{
  private Widget m_widget;
  private const int NUM_DISPLAY_PACKS = 3;
  private const string SHOW_EVENT_NAME = "CODE_DIALOGMANAGER_SHOW";
  private const string HIDE_EVENT_NAME = "CODE_DIALOGMANAGER_HIDE";
  private const string HIDE_FINISHED_EVENT_NAME = "CODE_HIDE_FINISHED";
  private SetRotationRotatedBoostersPopup.SetRotationRotatedBoostersPopupInfo m_info;

  protected override void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (eventName == "Button_Framed_Clicked")
        this.Hide();
      if (!(eventName == "CODE_HIDE_FINISHED"))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }));
    this.BindRankedPackListDataModel();
  }

  public override void Show()
  {
    if ((UnityEngine.Object) this.m_widget == (UnityEngine.Object) null)
      return;
    OverlayUI.Get().AddGameObject(this.m_widget.gameObject);
    UIContext.GetRoot().ShowPopup(this.m_widget.gameObject);
    Vector3 localScale = this.transform.localScale;
    this.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    this.m_widget.TriggerEvent("CODE_DIALOGMANAGER_SHOW");
    if (!string.IsNullOrEmpty(this.m_showAnimationSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_showAnimationSound);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.3f, (object) "easetype", (object) iTween.EaseType.easeOutBack));
  }

  public override void Hide()
  {
    if (this.m_popupInfo is SetRotationRotatedBoostersPopup.SetRotationRotatedBoostersPopupInfo popupInfo && popupInfo.m_onHiddenCallback != null)
      popupInfo.m_onHiddenCallback();
    this.m_widget.TriggerEvent("CODE_DIALOGMANAGER_HIDE");
  }

  private void BindRankedPackListDataModel()
  {
    PackListDataModel packListDataModel = new PackListDataModel();
    SpecialEventManager events = SpecialEventManager.Get();
    List<BoosterDbfRecord> records = GameDbf.Booster.GetRecords((Predicate<BoosterDbfRecord>) (r => events.IsEventActive(r.BuyWithGoldEvent, false)));
    records.Sort((Comparison<BoosterDbfRecord>) ((a, b) => b.LatestExpansionOrder.CompareTo(a.LatestExpansionOrder)));
    foreach (BoosterDbfRecord boosterDbfRecord in records)
    {
      if (GameUtils.IsBoosterRotated((BoosterDbId) boosterDbfRecord.ID, DateTime.UtcNow))
      {
        packListDataModel.Packs.Insert(0, new PackDataModel()
        {
          Type = (BoosterDbId) boosterDbfRecord.ID,
          BoosterName = (string) boosterDbfRecord.Name
        });
        if (packListDataModel.Packs.Count >= 3)
          break;
      }
    }
    this.m_widget.BindDataModel((IDataModel) packListDataModel);
  }

  public class SetRotationRotatedBoostersPopupInfo : BasicPopup.PopupInfo
  {
    public Action m_onHiddenCallback;
  }
}
