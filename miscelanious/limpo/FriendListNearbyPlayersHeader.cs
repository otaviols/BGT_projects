using System;
using UnityEngine;

public class FriendListNearbyPlayersHeader : FriendListItemHeader
{
  public GameObject m_arrowRight;
  public GameObject m_disableNearbyPlayersPanel;
  public GameObject m_enableNearbyPlayersPanel;
  public UIBButton m_disableButton;
  public UIBButton m_enableButton;
  private PegUIElement m_PanelInputBlocker;
  private bool m_PanelOpen;
  private int m_StoredNearbyPlayerCount;
  private string m_privacyPopupPrefab = "PrivacyPopups.prefab:99a8f571a8a35a54e90790c904bc94f8";
  private PrivacyFeaturesPopup m_privacyFeaturesPopup;

  public event Action OnPanelOpened;

  private bool NearbyPlayersEnabled => Options.Get().GetBool(Option.NEARBY_PLAYERS);

  private GameObject Panel => !this.NearbyPlayersEnabled ? this.m_enableNearbyPlayersPanel : this.m_disableNearbyPlayersPanel;

  protected override void Awake()
  {
    base.Awake();
    this.m_disableButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDisableRelease));
    this.m_enableButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnEnableRelease));
  }

  protected override void OnDestroy()
  {
    this.OnPanelOpened = (Action) null;
    base.OnDestroy();
  }

  public void SetText(int nearbyPlayerCount)
  {
    string text;
    if (!this.NearbyPlayersEnabled)
      text = GameStrings.Format("GLOBAL_FRIENDLIST_NEARBY_PLAYERS_DISABLED_HEADER");
    else
      text = GameStrings.Format("GLOBAL_FRIENDLIST_NEARBY_PLAYERS_HEADER", (object) nearbyPlayerCount);
    this.SetText(text);
    this.m_StoredNearbyPlayerCount = nearbyPlayerCount;
  }

  protected override void OnHeaderButtonReleased(UIEvent e)
  {
    if (this.m_PanelOpen)
      this.ClosePanel();
    else
      this.OpenPanel();
  }

  private void OpenPanel()
  {
    if ((UnityEngine.Object) this.Panel == (UnityEngine.Object) null || this.m_PanelOpen)
      return;
    if ((UnityEngine.Object) ChatMgr.Get() != (UnityEngine.Object) null && (UnityEngine.Object) ChatMgr.Get().FriendListFrame != (UnityEngine.Object) null && (UnityEngine.Object) ChatMgr.Get().FriendListFrame.items != (UnityEngine.Object) null)
      ChatMgr.Get().FriendListFrame.items.GetComponent<TouchList>().SetScrollingEnabled(false);
    this.m_PanelOpen = true;
    this.Panel.gameObject.SetActive(true);
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.Panel.layer);
    Bounds bounds1 = this.Panel.GetComponent<BoxCollider>().bounds;
    Bounds bounds2 = this.GetComponent<BoxCollider>().bounds;
    Vector3 vector3_1 = bounds1.size / 2f - bounds2.size / 2f;
    Vector3 vector3_2 = bounds2.center + (this.Panel.transform.position - bounds1.center);
    Vector3 vector3_3 = vector3_2 - vector3_1;
    Vector3 vector3_4 = vector3_2 + vector3_1;
    Vector3 position1 = new Vector3(vector3_3.x, 0.0f, (bounds2.center - bounds1.size).z);
    float z = (double) firstByLayer.WorldToViewportPoint(position1).y < 0.0 ? vector3_4.z : vector3_3.z;
    Vector3 position2 = this.Panel.transform.position;
    this.Panel.transform.position = new Vector3(position2.x, position2.y, z);
    UIBHighlight component = this.GetComponent<UIBHighlight>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.AlwaysOver = true;
    this.InitPanelInputBlocker();
    if (this.OnPanelOpened == null)
      return;
    this.OnPanelOpened();
  }

  private void ClosePanel()
  {
    if ((UnityEngine.Object) this.Panel == (UnityEngine.Object) null || !this.m_PanelOpen)
      return;
    if ((UnityEngine.Object) ChatMgr.Get() != (UnityEngine.Object) null && (UnityEngine.Object) ChatMgr.Get().FriendListFrame != (UnityEngine.Object) null && (UnityEngine.Object) ChatMgr.Get().FriendListFrame.items != (UnityEngine.Object) null)
      ChatMgr.Get().FriendListFrame.items.GetComponent<TouchList>().SetScrollingEnabled(true);
    this.m_PanelOpen = false;
    this.Panel.gameObject.SetActive(false);
    UIBHighlight component = this.GetComponent<UIBHighlight>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.AlwaysOver = false;
    if (!((UnityEngine.Object) this.m_PanelInputBlocker != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_PanelInputBlocker.gameObject);
    this.m_PanelInputBlocker = (PegUIElement) null;
  }

  private void InitPanelInputBlocker()
  {
    if ((UnityEngine.Object) this.m_PanelInputBlocker != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_PanelInputBlocker.gameObject);
      this.m_PanelInputBlocker = (PegUIElement) null;
    }
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.Panel.layer), "NearbyPlayerPanelInputBlocker");
    inputBlocker.transform.parent = this.Panel.transform;
    this.m_PanelInputBlocker = inputBlocker.AddComponent<PegUIElement>();
    this.m_PanelInputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPanelInputBlockerReleased));
    TransformUtil.SetPosZ((Component) this.m_PanelInputBlocker, this.Panel.transform.position.z + 1f);
  }

  private void OnPanelInputBlockerReleased(UIEvent e) => this.ClosePanel();

  private void OnDisableRelease(UIEvent e)
  {
    this.ClosePanel();
    this.ShowPrivacyPopup(false, (Action) (() => this.SetText(this.m_StoredNearbyPlayerCount)));
  }

  private void OnEnableRelease(UIEvent e)
  {
    this.ClosePanel();
    this.ShowPrivacyPopup(true, (Action) (() => this.SetText(this.m_StoredNearbyPlayerCount)));
  }

  private void ShowPrivacyPopup(bool doEnable, Action onActionSuccess)
  {
    this.m_privacyFeaturesPopup = AssetLoader.Get().InstantiatePrefab((AssetReference) this.m_privacyPopupPrefab).GetComponent<PrivacyFeaturesPopup>();
    PrivacyFeatures privacyFeature = PrivacyFeatures.NEARBY_FRIENDS;
    Action acceptCallback;
    if (!PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.NEARBY_FRIENDS) && !PrivacyGate.Get().FeatureEnabled(PrivacyFeatures.GEOLOCATION))
    {
      privacyFeature = PrivacyFeatures.GEOLOCATION;
      acceptCallback = (Action) (() =>
      {
        PrivacyGate.Get().SetFeature(PrivacyFeatures.NEARBY_FRIENDS, doEnable);
        PrivacyGate.Get().SetFeature(PrivacyFeatures.GEOLOCATION, doEnable);
      });
    }
    else
      acceptCallback = (Action) (() => PrivacyGate.Get().SetFeature(privacyFeature, doEnable));
    this.m_privacyFeaturesPopup.Set(privacyFeature, !doEnable, acceptCallback, (Action) (() =>
    {
      Action action = onActionSuccess;
      if (action != null)
        action();
      this.OnPopupSuccess();
    }), new Action(this.OnCancelPopup));
    this.m_privacyFeaturesPopup.Show();
  }

  private void OnPopupSuccess()
  {
    this.m_privacyFeaturesPopup.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_privacyFeaturesPopup.gameObject, 1f);
  }

  private void OnCancelPopup()
  {
    this.m_privacyFeaturesPopup.Hide();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_privacyFeaturesPopup.gameObject, 1f);
  }
}
