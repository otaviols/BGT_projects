using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyFeaturesPopup : DialogBase
{
  [SerializeField]
  private UIBButton m_continueButton;
  [SerializeField]
  private UIBButton m_choiceOneButton;
  [SerializeField]
  private UIBButton m_choiceTwoButton;
  [SerializeField]
  private GameObject m_continueButtonContainer;
  [SerializeField]
  private GameObject m_choiceButtonContainer;
  [SerializeField]
  private Spell m_successRingSpell;
  [SerializeField]
  private GameObject m_successRingContainer;
  [SerializeField]
  private GameObject m_searchPanel;
  [SerializeField]
  private GameObject m_successPanel;
  [SerializeField]
  private UberText m_titleText;
  [SerializeField]
  private UberText m_searchText;
  [SerializeField]
  private UberText m_successText;
  [SerializeField]
  private List<PrivacyFeaturesPopup.FeatureUISettings> m_featureUISettings = new List<PrivacyFeaturesPopup.FeatureUISettings>();
  private PrivacyFeaturesPopup.FeatureUISettings m_currentFeatureUISettings;
  private GameObject m_activePanel;
  private PrivacyFeaturesPopup.DialogState m_activeState;
  private Action m_onAcceptCallback;
  private Action m_onSuccessCallback;
  private Action m_onCancelCallback;
  private Coroutine m_searchCoroutine;
  private Vector3 NORMAL_SCALE;
  private Vector3 HIDDEN_SCALE;
  private PegUIElement m_inputBlocker;
  private const float BLUR_TIME = 0.1f;
  private const float BUTTON_BLOCK_TIME = 0.5f;
  private float m_buttonBlockTimer;
  private bool m_buttonBlocked = true;

  protected override void Awake()
  {
    base.Awake();
    this.NORMAL_SCALE = this.transform.localScale;
    this.HIDDEN_SCALE = 0.01f * this.NORMAL_SCALE;
  }

  private void Update()
  {
    if ((double) this.m_buttonBlockTimer < 0.0)
      return;
    this.m_buttonBlockTimer -= Time.deltaTime;
    if ((double) this.m_buttonBlockTimer >= 0.0)
      return;
    this.m_buttonBlocked = false;
  }

  private void OnEnable()
  {
    this.m_continueButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnContinueButton));
    this.m_choiceOneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChoiceOneButton));
    this.m_choiceTwoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChoiceTwoButton));
  }

  private void OnDisable()
  {
    this.m_continueButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnContinueButton));
    this.m_choiceOneButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChoiceOneButton));
    this.m_choiceTwoButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChoiceTwoButton));
  }

  private void CreateInputBlocker()
  {
    GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(this.gameObject.layer), "PrivacyFeaturesInputBlocker");
    inputBlocker.transform.parent = this.gameObject.transform;
    this.m_inputBlocker = inputBlocker.AddComponent<PegUIElement>();
    this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => { }));
    TransformUtil.SetPosY((Component) this.m_inputBlocker, this.gameObject.transform.position.y - 0.1f);
  }

  private void OnChoiceOneButton(UIEvent e)
  {
    if (this.m_buttonBlocked || this.m_activeState != PrivacyFeaturesPopup.DialogState.START)
      return;
    Action onCancelCallback = this.m_onCancelCallback;
    if (onCancelCallback == null)
      return;
    onCancelCallback();
  }

  private void OnChoiceTwoButton(UIEvent e)
  {
    if (this.m_buttonBlocked || this.m_activeState != PrivacyFeaturesPopup.DialogState.START)
      return;
    Action onAcceptCallback = this.m_onAcceptCallback;
    if (onAcceptCallback != null)
      onAcceptCallback();
    this.m_searchCoroutine = this.StartCoroutine(this.OnSearchState());
  }

  private void OnContinueButton(UIEvent e)
  {
    if (this.m_buttonBlocked)
      return;
    if (this.m_activeState == PrivacyFeaturesPopup.DialogState.SEARCH)
    {
      this.StopCoroutine(this.m_searchCoroutine);
      Action onCancelCallback = this.m_onCancelCallback;
      if (onCancelCallback == null)
        return;
      onCancelCallback();
    }
    else
    {
      if (this.m_activeState != PrivacyFeaturesPopup.DialogState.SUCCESS)
        return;
      Action onSuccessCallback = this.m_onSuccessCallback;
      if (onSuccessCallback == null)
        return;
      onSuccessCallback();
    }
  }

  private void OnStartState(bool isEnabled)
  {
    this.m_activePanel = isEnabled ? this.m_currentFeatureUISettings.disablePanel : this.m_currentFeatureUISettings.enablePanel;
    this.m_activeState = PrivacyFeaturesPopup.DialogState.START;
    this.m_choiceOneButton.SetText(GameStrings.Get("GLOBAL_CANCEL"));
    if (isEnabled)
      this.m_choiceTwoButton.SetText("GLOBAL_DISABLE");
    else
      this.m_choiceTwoButton.SetText("GLOBAL_ENABLE");
    this.m_continueButtonContainer.SetActive(false);
    this.m_choiceButtonContainer.SetActive(true);
    this.SetActivePanel();
  }

  private IEnumerator OnSearchState()
  {
    this.m_activePanel = this.m_searchPanel;
    this.m_activeState = PrivacyFeaturesPopup.DialogState.SEARCH;
    this.m_continueButton.SetText(GameStrings.Get("GLOBAL_CANCEL"));
    this.m_continueButtonContainer.SetActive(false);
    this.m_choiceButtonContainer.SetActive(false);
    this.SetActivePanel();
    this.m_successRingContainer.SetActive(true);
    this.m_successRingSpell.ActivateState(SpellStateType.BIRTH);
    yield return (object) new WaitForSeconds(0.5f);
    this.m_successRingSpell.ActivateState(SpellStateType.ACTION);
    yield return (object) new WaitForSeconds(1.5f);
    this.OnSuccess();
  }

  private void OnSuccess()
  {
    this.m_activePanel = this.m_successPanel;
    this.m_activeState = PrivacyFeaturesPopup.DialogState.SUCCESS;
    this.m_continueButton.SetText(GameStrings.Get("GLOBAL_BUTTON_OK"));
    this.m_continueButtonContainer.SetActive(true);
    this.m_choiceButtonContainer.SetActive(false);
    this.SetActivePanel();
  }

  private bool SetCurrentSettings(PrivacyFeatures privacyFeature)
  {
    PrivacyFeaturesPopup.FeatureUISettings featureUiSettings = this.m_featureUISettings.Find((Predicate<PrivacyFeaturesPopup.FeatureUISettings>) (x => x.privacyFeature == privacyFeature));
    if (featureUiSettings == null)
    {
      Log.Privacy.PrintError("Privacy feature not supported in UI: " + privacyFeature.ToString());
      return false;
    }
    this.m_titleText.Text = featureUiSettings.titleText;
    this.m_searchText.Text = featureUiSettings.searchText;
    this.m_successText.Text = featureUiSettings.successText;
    this.m_currentFeatureUISettings = featureUiSettings;
    return true;
  }

  private void SetActivePanel()
  {
    this.m_currentFeatureUISettings.enablePanel.SetActive((UnityEngine.Object) this.m_currentFeatureUISettings.enablePanel == (UnityEngine.Object) this.m_activePanel);
    this.m_currentFeatureUISettings.disablePanel.SetActive((UnityEngine.Object) this.m_currentFeatureUISettings.disablePanel == (UnityEngine.Object) this.m_activePanel);
    this.m_searchPanel.SetActive((UnityEngine.Object) this.m_searchPanel == (UnityEngine.Object) this.m_activePanel);
    this.m_successPanel.SetActive((UnityEngine.Object) this.m_successPanel == (UnityEngine.Object) this.m_activePanel);
  }

  public void Set(
    PrivacyFeatures privacyFeature,
    bool isEnabled,
    Action acceptCallback,
    Action successCallback,
    Action cancelCallback)
  {
    if (!this.SetCurrentSettings(privacyFeature))
    {
      if (cancelCallback == null)
        return;
      cancelCallback();
    }
    else
    {
      this.m_onAcceptCallback = acceptCallback;
      this.m_onSuccessCallback = successCallback;
      this.m_onCancelCallback = cancelCallback;
      this.OnStartState(isEnabled);
    }
  }

  public override void Show()
  {
    base.Show();
    this.m_buttonBlockTimer = 0.5f;
    this.m_buttonBlocked = true;
    this.CreateInputBlocker();
    AnimationUtil.ShowWithPunch(this.gameObject, this.HIDDEN_SCALE, 1.1f * this.NORMAL_SCALE, this.NORMAL_SCALE, (string) null, true);
    ScreenEffectParameters desaturatePerspective = ScreenEffectParameters.BlurVignetteDesaturatePerspective with
    {
      Time = 0.1f
    };
    DialogBase.m_screenEffectsHandle.StartEffect(desaturatePerspective);
  }

  public override void Hide()
  {
    DialogBase.m_screenEffectsHandle.StopEffect();
    base.Hide();
  }

  private enum DialogState
  {
    START = 1,
    SEARCH = 2,
    SUCCESS = 3,
  }

  [Serializable]
  private class FeatureUISettings
  {
    public PrivacyFeatures privacyFeature = PrivacyFeatures.INVALID;
    public GameObject enablePanel;
    public GameObject disablePanel;
    public string titleText;
    public string searchText;
    public string successText;
  }
}
