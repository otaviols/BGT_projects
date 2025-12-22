using Hearthstone.UI;
using System;
using UnityEngine;

public class WhatsInsideStorePanel : MonoBehaviour
{
  public UIBButton m_whatsInsideButton;
  public string m_rewardCardId;
  private AssetReference m_whatsInsideWidgetPrefab = new AssetReference("AdventureStorymodeWhatsInsideStore.prefab:099ec2422fde4054495382cee27d8c06");
  private Widget m_whatsInsideWidget;
  private UIBPopup m_whatsInsidePopup;
  private UIBButton m_whatsInsideBackButton;
  private Actor m_legendaryCardActor;

  private void Start() => this.m_whatsInsideButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnWhatsInsideButtonRelease));

  private void OnWhatsInsideButtonRelease(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_whatsInsideWidget == (UnityEngine.Object) null)
      this.m_whatsInsideWidget = (Widget) WidgetInstance.Create((string) this.m_whatsInsideWidgetPrefab);
    this.m_whatsInsideWidget.RegisterReadyListener((Action<object>) (_ =>
    {
      if ((UnityEngine.Object) this.m_whatsInsidePopup == (UnityEngine.Object) null)
        this.m_whatsInsidePopup = this.m_whatsInsideWidget.GetComponentInChildren<UIBPopup>();
      if ((UnityEngine.Object) this.m_whatsInsideBackButton == (UnityEngine.Object) null)
      {
        this.m_whatsInsideBackButton = this.m_whatsInsideWidget.GetComponentInChildren<UIBButton>();
        this.m_whatsInsideBackButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (backEvent => this.m_whatsInsidePopup.Hide()));
      }
      this.m_legendaryCardActor = this.m_whatsInsideWidget.GetComponentInChildren<Actor>(true);
      if ((UnityEngine.Object) this.m_legendaryCardActor != (UnityEngine.Object) null)
      {
        using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(this.m_rewardCardId))
        {
          this.m_legendaryCardActor.SetPremium(TAG_PREMIUM.GOLDEN);
          this.m_legendaryCardActor.SetFullDef(fullDef);
          this.m_legendaryCardActor.UpdateAllComponents();
        }
      }
      this.m_whatsInsidePopup.Show(false);
    }), (object) null, true);
  }
}
