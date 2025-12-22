using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.Core.Streaming;
using Hearthstone.Streaming;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BnetBarMenuButton : PegUIElement
{
  [SerializeField]
  private GameObject m_phoneBar;
  [SerializeField]
  private Transform m_phoneBarOneElementBone;
  [SerializeField]
  private Transform m_phoneBarTwoElementBone;
  [Header("Default")]
  [SerializeField]
  private GameObject m_defaultHighlight;
  [SerializeField]
  private MeshRenderer m_defaultBackground;
  [SerializeField]
  [Header("Downloading")]
  private GameObject m_downloadingHighlight;
  [SerializeField]
  private MeshRenderer m_downloadingBackground;
  [SerializeField]
  private MeshRenderer m_downloadingArrow;
  [SerializeField]
  private UberText m_downloadProgressText;
  [SerializeField]
  private float m_normalArrowTextureOffset;
  [SerializeField]
  private float m_inactiveArrowTextureOffset = 0.205f;
  [Header("Offline")]
  [SerializeField]
  private GameObject m_offlineSection;
  [SerializeField]
  private GameObject m_offlineHighlight_OfflineSection;
  [SerializeField]
  private PegUIElement m_offlineSectionButton;
  [SerializeField]
  private TooltipZone m_offlineTooltipZone;
  private bool m_selected;
  private int m_phoneBarStatus = -1;
  private float m_originalLightingBlend;
  private Dictionary<BnetBarMenuButton.State, GameObject> m_highlightsByState;
  private Dictionary<BnetBarMenuButton.State, MeshRenderer> m_backgroundsByState;
  private Material m_backgroundMaterial;
  private Material m_arrowMaterial;
  private Coroutine m_delayedChangeStateCoroutine;
  private int m_progressVal;
  private double m_downloadSpeed;
  private bool m_shouldShowOfflineSection;
  private Vector3 m_offlineSectionStartingPosition;
  public Action StateChanged;
  private const float CURRENCY_FRAME_OFFSET_STATE = -18.75f;
  private readonly PlatformDependentValue<float> CURRENCY_FRAME_OFFSET_DEFAULT = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = -6.25f,
    Tablet = -6.25f,
    Phone = -8.75f
  };
  private readonly PlatformDependentValue<float> CURRENCY_FRAME_OFFSET_STATE_OFFLINE = new PlatformDependentValue<float>(PlatformCategory.Screen)
  {
    PC = -35f,
    Tablet = -35f,
    Phone = -53.75f
  };
  private const float OFFLINE_SECTION_STATE_OFFSET = 21.5f;
  private readonly Vector3 GAME_MENU_TOOLTIP_OFFSET = new Vector3(-24.55692f, 0.0f, 9f);

  public BnetBarMenuButton.State CurrentState { private set; get; }

  public void LockHighlight(bool isLocked) => this.m_highlightsByState[this.CurrentState].SetActive(isLocked);

  private IGameDownloadManager DownloadManager => GameDownloadManagerProvider.Get();

  private GameObject Highlight => this.m_highlightsByState[this.CurrentState];

  private MeshRenderer Background => this.m_backgroundsByState[this.CurrentState];

  protected override void Awake()
  {
    base.Awake();
    this.m_highlightsByState = new Dictionary<BnetBarMenuButton.State, GameObject>()
    {
      {
        BnetBarMenuButton.State.Default,
        this.m_defaultHighlight
      },
      {
        BnetBarMenuButton.State.Downloading,
        this.m_downloadingHighlight
      }
    };
    this.m_backgroundsByState = new Dictionary<BnetBarMenuButton.State, MeshRenderer>()
    {
      {
        BnetBarMenuButton.State.Default,
        this.m_defaultBackground
      },
      {
        BnetBarMenuButton.State.Downloading,
        this.m_downloadingBackground
      }
    };
    foreach (MeshRenderer meshRenderer in this.m_backgroundsByState.Values)
    {
      if ((UnityEngine.Object) meshRenderer != (UnityEngine.Object) null)
        meshRenderer.gameObject.SetActive(false);
    }
    if ((UnityEngine.Object) this.Background != (UnityEngine.Object) null)
    {
      this.m_backgroundMaterial = this.Background.GetMaterial();
      if ((UnityEngine.Object) this.m_backgroundMaterial != (UnityEngine.Object) null)
        this.m_originalLightingBlend = this.m_backgroundMaterial.GetFloat("_LightingBlend");
    }
    if ((UnityEngine.Object) this.m_downloadingArrow != (UnityEngine.Object) null)
      this.m_arrowMaterial = this.m_downloadingArrow.GetMaterial();
    if ((UnityEngine.Object) this.m_offlineSection != (UnityEngine.Object) null)
    {
      this.m_offlineSection.SetActive(false);
      this.m_offlineSectionStartingPosition = this.m_offlineSection.transform.localPosition;
    }
    if ((UnityEngine.Object) this.m_offlineSectionButton != (UnityEngine.Object) null)
    {
      this.m_offlineSectionButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOfflineSectionRelease));
      this.m_offlineSectionButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOfflineSectionRollover));
      this.m_offlineSectionButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOfflineSectionRollout));
    }
    this.UpdateHighlight();
    this.ChangeState(BnetBarMenuButton.State.Default, true);
  }

  private void Update()
  {
    this.SetOfflineSectionActive(BnetBarMenuButton.ShouldShowOfflineSection());
    if (this.DownloadManager == null)
      return;
    TagDownloadStatus currentDownloadStatus = this.DownloadManager.GetCurrentDownloadStatus();
    if (currentDownloadStatus == null || currentDownloadStatus.BytesTotal == 0L)
    {
      this.ChangeState(BnetBarMenuButton.State.Default);
    }
    else
    {
      int num = (int) ((double) currentDownloadStatus.Progress * 100.0);
      double bytesPerSecond = this.DownloadManager.BytesPerSecond;
      if (num == 0 || num == this.m_progressVal && bytesPerSecond == this.m_downloadSpeed && (this.CurrentState != BnetBarMenuButton.State.Downloading || !this.DownloadManager.IsInterrupted))
        return;
      this.m_progressVal = num;
      this.m_downloadSpeed = bytesPerSecond;
      if (!this.DownloadManager.IsAnyDownloadRequestedAndIncomplete)
      {
        this.m_downloadProgressText.Text = "...";
        this.ChangeStateAfterDelay(BnetBarMenuButton.State.Default, 5f);
      }
      else if (this.DownloadManager.IsInterrupted)
      {
        this.m_downloadProgressText.Text = GameStrings.Get("GLOBAL_ASSET_DOWNLOAD_PAUSED");
        this.m_arrowMaterial.mainTextureOffset = (Vector2) new Vector3(this.m_inactiveArrowTextureOffset, 0.0f);
        this.ChangeState(BnetBarMenuButton.State.Downloading);
      }
      else
      {
        this.m_downloadProgressText.Text = string.Format("{0:0.}%", (object) num);
        this.m_arrowMaterial.mainTextureOffset = (Vector2) new Vector3(this.m_normalArrowTextureOffset, 0.0f);
        this.ChangeState(BnetBarMenuButton.State.Downloading);
      }
    }
  }

  private static bool ShouldShowOfflineSection()
  {
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    return !Network.IsLoggedIn() && Network.ShouldBeConnectedToAurora() && mode != SceneMgr.Mode.GAMEPLAY && mode != SceneMgr.Mode.FATAL_ERROR;
  }

  private void ChangeStateAfterDelay(BnetBarMenuButton.State newState, float seconds)
  {
    if (this.m_delayedChangeStateCoroutine != null)
      this.StopCoroutine(this.m_delayedChangeStateCoroutine);
    this.m_delayedChangeStateCoroutine = this.StartCoroutine(this.ChangeStateAfterDelayCoroutine(newState, seconds));
  }

  private IEnumerator ChangeStateAfterDelayCoroutine(
    BnetBarMenuButton.State newState,
    float seconds)
  {
    yield return (object) new WaitForSeconds(seconds);
    this.ChangeState(newState);
  }

  private void ChangeState(BnetBarMenuButton.State nextState, bool force = false)
  {
    if (this.CurrentState == nextState && !force)
      return;
    if (this.m_delayedChangeStateCoroutine != null)
      this.StopCoroutine(this.m_delayedChangeStateCoroutine);
    this.m_backgroundsByState[this.CurrentState].gameObject.SetActive(false);
    this.m_backgroundsByState[nextState].gameObject.SetActive(true);
    this.m_highlightsByState[nextState].gameObject.SetActive(this.m_highlightsByState[this.CurrentState].gameObject.activeSelf);
    this.m_highlightsByState[this.CurrentState].gameObject.SetActive(false);
    this.CurrentState = nextState;
    if (this.CurrentState == BnetBarMenuButton.State.Default)
      this.m_downloadProgressText.Text = "";
    if (this.StateChanged == null)
      return;
    this.StateChanged();
  }

  public bool IsSelected() => this.m_selected;

  public void SetSelected(bool enable)
  {
    if (enable == this.m_selected)
      return;
    this.m_selected = enable;
    this.UpdateHighlight();
  }

  public override void SetEnabled(bool enabled, bool isInternal = false)
  {
    base.SetEnabled(enabled, isInternal);
    if (!((UnityEngine.Object) this.m_backgroundMaterial != (UnityEngine.Object) null))
      return;
    this.m_backgroundMaterial.SetFloat("_LightingBlend", enabled ? this.m_originalLightingBlend : 0.8f);
  }

  public void SetPhoneStatusBarState(int nElements)
  {
    if (nElements == this.m_phoneBarStatus)
      return;
    this.m_phoneBarStatus = nElements;
    switch (nElements)
    {
      case 0:
        this.m_phoneBar.SetActive(false);
        break;
      case 1:
        this.m_phoneBar.SetActive(true);
        iTween.Stop(this.m_phoneBar);
        iTween.MoveTo(this.m_phoneBar, iTween.Hash((object) "position", (object) this.m_phoneBarOneElementBone.position, (object) "time", (object) 1f, (object) "isLocal", (object) false, (object) "easetype", (object) iTween.EaseType.easeOutExpo, (object) "onupdate", (object) "OnStatusBarUpdate", (object) "onupdatetarget", (object) this.gameObject));
        break;
      case 2:
        this.m_phoneBar.SetActive(true);
        iTween.Stop(this.m_phoneBar);
        iTween.MoveTo(this.m_phoneBar, iTween.Hash((object) "position", (object) this.m_phoneBarTwoElementBone.position, (object) "time", (object) 1f, (object) "isLocal", (object) false, (object) "easetype", (object) iTween.EaseType.easeOutExpo, (object) "onupdate", (object) "OnStatusBarUpdate", (object) "onupdatetarget", (object) this.gameObject));
        break;
      default:
        Debug.LogError((object) ("Invalid phone status bar state " + (object) nElements));
        break;
    }
  }

  public void OnStatusBarUpdate() => BnetBar.Get().UpdateLayout();

  public float GetCurrencyFrameOffsetX()
  {
    bool flag = this.CurrentState == BnetBarMenuButton.State.Default;
    if (!flag && this.m_shouldShowOfflineSection)
      return (float) this.CURRENCY_FRAME_OFFSET_STATE_OFFLINE;
    return flag && this.m_shouldShowOfflineSection || !flag && !this.m_shouldShowOfflineSection ? -18.75f : (float) this.CURRENCY_FRAME_OFFSET_DEFAULT;
  }

  private bool ShouldBeHighlighted() => this.m_selected || this.GetInteractionState() == PegUIElement.InteractionState.Over;

  protected virtual void UpdateHighlight()
  {
    bool flag = this.ShouldBeHighlighted();
    if (!flag)
    {
      BnetBar bnetBar = BnetBar.Get();
      if ((UnityEngine.Object) bnetBar != (UnityEngine.Object) null && bnetBar.IsGameMenuShown())
        flag = true;
    }
    if (this.Highlight.activeSelf == flag)
      return;
    this.Highlight.SetActive(flag);
  }

  private void ShowGameMenuTooltip(
    TooltipZone tooltipZone,
    string tooltipHeader,
    string tooltipDescription)
  {
    TooltipPanel src = tooltipZone.ShowTooltip(tooltipHeader, tooltipDescription, 0.7f);
    LayerUtils.SetLayer(src.gameObject, GameLayer.BattleNet);
    src.transform.localEulerAngles = new Vector3(270f, 0.0f, 0.0f);
    src.transform.localScale = new Vector3(82.35294f, 70f, 90.32258f);
    TransformUtil.SetPoint((Component) src, Anchor.BOTTOM, this.gameObject, Anchor.TOP, this.GAME_MENU_TOOLTIP_OFFSET);
  }

  private void SetOfflineSectionActive(bool active)
  {
    if (active == this.m_shouldShowOfflineSection)
      return;
    this.m_shouldShowOfflineSection = active;
    this.m_offlineSection.SetActive(active);
    this.m_offlineSection.transform.localPosition = this.m_offlineSectionStartingPosition;
    if (this.CurrentState != BnetBarMenuButton.State.Default)
      this.m_offlineSection.transform.localPosition -= Vector3.right * 21.5f;
    if (this.StateChanged == null)
      return;
    this.StateChanged();
  }

  private void OnOfflineSectionRelease(UIEvent e)
  {
    if (DialogManager.Get().ShowingDialog())
      return;
    DialogManager.Get().ShowReconnectHelperDialog();
    this.m_offlineTooltipZone.HideTooltip();
    this.m_offlineHighlight_OfflineSection.SetActive(false);
  }

  private void OnOfflineSectionRollover(UIEvent e)
  {
    if (DialogManager.Get().ShowingDialog())
      return;
    this.m_offlineHighlight_OfflineSection.SetActive(true);
    this.ShowGameMenuTooltip(this.m_offlineTooltipZone, GameStrings.Get("GLOBAL_TOOLTIP_MENU_OFFLINE_HEADER"), GameStrings.Get("GLOBAL_TOOLTIP_MENU_OFFLINE_DESC"));
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9"));
  }

  private void OnOfflineSectionRollout(UIEvent e)
  {
    this.m_offlineTooltipZone.HideTooltip();
    this.m_offlineHighlight_OfflineSection.SetActive(false);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    this.ShowGameMenuTooltip(this.GetComponent<TooltipZone>(), GameStrings.Get("GLOBAL_TOOLTIP_MENU_HEADER"), GameStrings.Get("GLOBAL_TOOLTIP_MENU_DESC"));
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("Small_Mouseover.prefab:692610296028713458ea58bc34adb4c9"));
    this.UpdateHighlight();
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    this.GetComponent<TooltipZone>().HideTooltip();
    this.UpdateHighlight();
  }

  public enum State
  {
    Default,
    Downloading,
  }
}
