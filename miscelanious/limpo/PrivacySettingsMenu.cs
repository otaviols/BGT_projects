using Hearthstone.Util;
using System;
using UnityEngine;

public class PrivacySettingsMenu : MonoBehaviour
{
  [SerializeField]
  public CheckBox m_chatCheckbox;
  [SerializeField]
  public CheckBox m_personalizedShopOffersCheckbox;
  [SerializeField]
  public CheckBox m_nearbyFriendsCheckbox;
  [SerializeField]
  public CheckBox m_locationSettingsCheckbox;
  [SerializeField]
  public CheckBox m_pushNotificationsCheckbox;
  [SerializeField]
  public UIBButton m_doneButton;
  [SerializeField]
  public UIBButton m_personalizedShopRulesButton;
  private string m_privacyPopupPrefab = "PrivacyPopups.prefab:99a8f571a8a35a54e90790c904bc94f8";
  private PegUIElement m_inputBlocker;
  private Vector3 NORMAL_SCALE;
  private Vector3 HIDDEN_SCALE;
  private static PrivacySettingsMenu s_instance;

  private void Awake()
  {
    PrivacySettingsMenu.s_instance = this;
    this.NORMAL_SCALE = this.transform.localScale;
    this.HIDDEN_SCALE = 0.01f * this.NORMAL_SCALE;
    OverlayUI.Get().AddGameObject(this.gameObject);
    this.CreateInputBlocker();
    this.SetupPrivacySettingsMenu();
  }

  public static PrivacySettingsMenu Get() => PrivacySettingsMenu.s_instance;

  private void OnEnable() => this.UpdateCheckboxes();

  private void SetupPrivacySettingsMenu()
  {
    this.InitializeOptIn(PrivacyFeatures.CHAT);
    this.InitializeOptIn(PrivacyFeatures.PERSONALIZED_STORE_ITEMS);
    this.InitializeOptIn(PrivacyFeatures.NEARBY_FRIENDS);
    this.InitializeOptIn(PrivacyFeatures.GEOLOCATION);
    this.InitializeOptIn(PrivacyFeatures.PUSH_NOTIFICATIONS);
    if ((UnityEngine.Object) this.m_doneButton != (UnityEngine.Object) null)
      this.m_doneButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (_ => this.Hide()));
    if (!((UnityEngine.Object) this.m_personalizedShopRulesButton != (UnityEngine.Object) null) || !RegionUtils.IsCNLegalRegion)
      return;
    this.m_personalizedShopRulesButton.gameObject.SetActive(true);
    this.m_personalizedShopRulesButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPersonalizedShopRulesButtonReleased));
  }

  private void UpdateCheckboxes()
  {
    if ((UnityEngine.Object) this.m_chatCheckbox != (UnityEngine.Object) null)
      this.m_chatCheckbox.SetChecked(PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.CHAT));
    if ((UnityEngine.Object) this.m_personalizedShopOffersCheckbox != (UnityEngine.Object) null)
      this.m_personalizedShopOffersCheckbox.SetChecked(PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.PERSONALIZED_STORE_ITEMS));
    if ((UnityEngine.Object) this.m_nearbyFriendsCheckbox != (UnityEngine.Object) null)
      this.m_nearbyFriendsCheckbox.SetChecked(PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.NEARBY_FRIENDS));
    if ((UnityEngine.Object) this.m_locationSettingsCheckbox != (UnityEngine.Object) null)
      this.m_locationSettingsCheckbox.SetChecked(PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.GEOLOCATION));
    if (!((UnityEngine.Object) this.m_pushNotificationsCheckbox != (UnityEngine.Object) null))
      return;
    this.m_pushNotificationsCheckbox.SetChecked(PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.PUSH_NOTIFICATIONS));
  }

  private void InitializeOptIn(PrivacyFeatures privacyFeature)
  {
    CheckBox checkbox = (CheckBox) null;
    switch (privacyFeature)
    {
      case PrivacyFeatures.CHAT:
        checkbox = this.m_chatCheckbox;
        break;
      case PrivacyFeatures.GEOLOCATION:
        checkbox = this.m_locationSettingsCheckbox;
        break;
      case PrivacyFeatures.PERSONALIZED_STORE_ITEMS:
        checkbox = this.m_personalizedShopOffersCheckbox;
        break;
      case PrivacyFeatures.PUSH_NOTIFICATIONS:
        if (PlatformSettings.IsMobile())
        {
          checkbox = this.m_pushNotificationsCheckbox;
          break;
        }
        if ((UnityEngine.Object) this.m_pushNotificationsCheckbox != (UnityEngine.Object) null)
        {
          this.m_pushNotificationsCheckbox.gameObject.SetActive(false);
          break;
        }
        break;
      case PrivacyFeatures.NEARBY_FRIENDS:
        checkbox = this.m_nearbyFriendsCheckbox;
        break;
    }
    if ((UnityEngine.Object) checkbox == (UnityEngine.Object) null)
      return;
    checkbox.SetChecked(PrivacyGate.Get().FeatureEnabled(privacyFeature));
    checkbox.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (eventHandler => this.OnCheckboxReleasedEvent(checkbox, privacyFeature)));
  }

  private void OnCheckboxReleasedEvent(CheckBox checkbox, PrivacyFeatures privacyFeature)
  {
    PrivacyFeaturesPopup privacyPopup = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_privacyPopupPrefab).GetComponent<PrivacyFeaturesPopup>();
    bool isBoxChecked = checkbox.IsChecked();
    Action acceptCallback;
    if (privacyFeature == PrivacyFeatures.NEARBY_FRIENDS && !PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.NEARBY_FRIENDS) && !PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.GEOLOCATION))
    {
      privacyFeature = PrivacyFeatures.GEOLOCATION;
      acceptCallback = (Action) (() =>
      {
        PrivacyGate.Get().SetFeature(PrivacyFeatures.NEARBY_FRIENDS, isBoxChecked);
        PrivacyGate.Get().SetFeature(PrivacyFeatures.GEOLOCATION, isBoxChecked);
      });
    }
    else
      acceptCallback = (Action) (() => PrivacyGate.Get().SetFeature(privacyFeature, isBoxChecked));
    privacyPopup.Set(privacyFeature, !isBoxChecked, acceptCallback, (Action) (() => this.OnPopupSuccess(privacyPopup)), (Action) (() => this.OnCancelPopup(privacyPopup, checkbox)));
    privacyPopup.Show();
  }

  private void OnPopupSuccess(PrivacyFeaturesPopup privacyPopup)
  {
    this.UpdateCheckboxes();
    privacyPopup.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) privacyPopup.gameObject, 1f);
  }

  private void OnCancelPopup(PrivacyFeaturesPopup privacyPopup, CheckBox checkbox)
  {
    checkbox.SetChecked(!checkbox.IsChecked());
    privacyPopup.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) privacyPopup.gameObject, 1f);
  }

  public void Show()
  {
    this.gameObject.SetActive(true);
    AnimationUtil.ShowWithPunch(this.gameObject, this.HIDDEN_SCALE, 1.1f * this.NORMAL_SCALE, this.NORMAL_SCALE, (string) null, true);
  }

  public bool IsShown() => this.gameObject.activeSelf;

  public void Hide() => this.gameObject.SetActive(false);

  private void CreateInputBlocker()
  {
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "OptionMenuInputBlocker", (Component) this, (Component) this.transform, 10f);
    inputBlocker.layer = this.gameObject.layer;
    this.m_inputBlocker = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Hide()));
  }

  private void OnPersonalizedShopRulesButtonReleased(UIEvent e) => Application.OpenURL(ExternalUrlService.Get().GetPersonalizedShopRulesLink());
}
