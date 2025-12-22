using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class MercenariesZoneUnlockDialog : DialogBase
{
  public AsyncReference m_rootWidgetReference;
  public PegUIElement m_clickCatcher;
  private MercenariesZoneUnlockDialog.Info m_info;
  private Widget m_rootWidget;
  private LettuceLobbyChooserButton m_zoneButton;

  private void Start()
  {
    this.m_rootWidgetReference.RegisterReadyListener<Widget>((Action<Widget>) (w => this.m_rootWidget = w));
    this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickCatcherReleased));
  }

  public void SetInfo(MercenariesZoneUnlockDialog.Info info) => this.m_info = info;

  public override void Show() => this.StartCoroutine(this.ShowWhenReady());

  private IEnumerator ShowWhenReady()
  {
    MercenariesZoneUnlockDialog zoneUnlockDialog = this;
    while ((UnityEngine.Object) zoneUnlockDialog.m_rootWidget == (UnityEngine.Object) null || !zoneUnlockDialog.m_rootWidget.IsReady)
      yield return (object) null;
    LettuceBountySetDbfRecord record = GameDbf.LettuceBountySet.GetRecord(zoneUnlockDialog.m_info.m_zoneId);
    if (record != null)
    {
      LettuceZoneUnlockDataModel dataModel = new LettuceZoneUnlockDataModel()
      {
        FooterText = (string) record.UnlockPopupText,
        ZoneNameText = (string) record.Name
      };
      bool textureLoaded = false;
      if (!string.IsNullOrEmpty(record.TileArtTexture))
        AssetLoader.Get().LoadTexture((AssetReference) record.TileArtTexture, (ObjectCallback) ((assetRef, obj, callbackData) =>
        {
          dataModel.ZoneTexture = obj as Texture;
          textureLoaded = true;
        }));
      while (!textureLoaded)
        yield return (object) null;
      zoneUnlockDialog.m_rootWidget.BindDataModel((IDataModel) dataModel);
      while (zoneUnlockDialog.m_rootWidget.IsChangingStates)
        yield return (object) null;
      // ISSUE: reference to a compiler-generated method
      zoneUnlockDialog.\u003C\u003En__0();
      zoneUnlockDialog.DoShowAnimation();
      DialogBase.DoBlur();
      UniversalInputManager.Get().SetGameDialogActive(true);
    }
    else
    {
      Debug.LogError((object) ("Zone unlock dialog attempted to show invalid zone with id: " + (object) zoneUnlockDialog.m_info.m_zoneId));
      // ISSUE: reference to a compiler-generated method
      zoneUnlockDialog.\u003C\u003En__1();
      Action completeCallback = zoneUnlockDialog.m_info.m_onCompleteCallback;
      if (completeCallback != null)
        completeCallback();
    }
  }

  public override void Hide()
  {
    base.Hide();
    DialogBase.EndBlur();
    Action completeCallback = this.m_info.m_onCompleteCallback;
    if (completeCallback != null)
      completeCallback();
    UniversalInputManager.Get().SetGameDialogActive(false);
  }

  private void OnClickCatcherReleased(UIEvent e) => this.Hide();

  public class Info
  {
    public int m_zoneId;
    public Action m_onCompleteCallback;
  }
}
