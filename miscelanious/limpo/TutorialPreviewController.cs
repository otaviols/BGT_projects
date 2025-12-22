using Blizzard.T5.AssetManager;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CustomEditClass]
public class TutorialPreviewController : MonoBehaviour
{
  [CustomEditField(T = EditType.VIDEO)]
  public string m_traditionalVideo;
  [CustomEditField(T = EditType.VIDEO)]
  public string m_battlegroundsVideo;
  [CustomEditField(T = EditType.VIDEO)]
  public string m_mercenariesVideo;
  private AudioSource m_lastPickedVO;
  private Coroutine m_lastCoroutine;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_tutorialTraditionalDescriptionVO = "VO_FTUE_01_Jaina_ModeDescription.prefab:715d99597ad67a14d9dbb371def5526c";
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_tutorialBattlegroundsDescriptionVO = "VO_FTUE_02_Bob_ModeDescription.prefab:96c4ac8907771ff4aa0db0e4136a65c9";
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_tutorialMercenariesDescriptionVO = "VO_FTUE_03_Valeera_ModeDescription.prefab:db54cf1e5b5f0034ba57c75a9c589338";
  private Dictionary<string, AudioSource> m_tutorialDescriptionVOMap;
  public float m_bannerAudioDelay = 0.5f;
  public Spell m_portalSpell;
  public VisualController m_brassRingVisualController;
  public VideoPlayer m_videoPlayer;
  public Spell m_confirmButtonSpell;
  public GameObject m_root;
  private AssetHandle<VideoClip> m_loadedVideo;
  private Widget m_widget;
  private VisualController m_visualController;
  private Action m_onSelectionConfirmedCallback;
  private TutorialPreviewController.TutorialPreviewWidgetState m_currentState;
  private TutorialPreviewDataModel m_tutorialPreviewDataModel;
  private const string CancelClicked = "CANCEL_CLICKED";
  private const string ConfirmClicked = "CONFIRM_CLICKED";
  private const string BannerIntroFinished = "BANNER_INTRO_FINISHED";
  private const string BannerOutroFinished = "BANNER_OUTRO_FINISHED";
  private const string ShowPopupEvent = "SHOW_POPUP";
  private const string DismissPopupEvent = "DISMISS_POPUP";
  private const string StateDeckBattle = "DECKBATTLE";
  private const string StateBattlegrounds = "BATTLEGROUNDS";
  private const string StateMercenaries = "MERCENARIES";
  private const string StateShow = "SHOW";
  private const string StateShowInForeground = "SHOW_IN_FOREGROUND";
  private const string StateHide = "HIDE";
  private const string StateOutro = "OUTRO";
  public const string GameModeTraditional = "traditional";
  public const string GameModeBattlegrounds = "battlegrounds";
  public const string GameModeMercenaries = "mercenaries";
  private bool m_isReopeningPortal;
  private bool m_isPortalAnimating;
  private bool m_isBannerAnimating;

  public static event Action PreviewOpened;

  public static event Action PreviewClosed;

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_visualController = this.GetComponent<VisualController>();
    this.m_tutorialPreviewDataModel = new TutorialPreviewDataModel();
    this.m_widget.BindDataModel((IDataModel) this.m_tutorialPreviewDataModel);
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnVideoPreviewEvent));
    this.m_tutorialDescriptionVOMap = new Dictionary<string, AudioSource>();
  }

  private void OnDestroy() => AssetHandle.SafeDispose<VideoClip>(ref this.m_loadedVideo);

  public bool IsPlayingPreview => (UnityEngine.Object) this.m_videoPlayer != (UnityEngine.Object) null && this.m_videoPlayer.isPlaying;

  public bool IsAnimating => this.m_isPortalAnimating || this.m_isBannerAnimating || this.m_isReopeningPortal;

  public void StartTraditionalTutorialPreviewVideo(Action OnPlayerConfirmedSelection)
  {
    if (this.m_currentState == TutorialPreviewController.TutorialPreviewWidgetState.TRADITIONAL_SELECTED)
      return;
    this.m_onSelectionConfirmedCallback = OnPlayerConfirmedSelection;
    this.ShowTutorialVideo(TutorialPreviewController.TutorialPreviewWidgetState.TRADITIONAL_SELECTED);
  }

  public void StartBattleGroundsTutorialPreviewVideo(Action OnPlayerConfirmedSelection)
  {
    if (this.m_currentState == TutorialPreviewController.TutorialPreviewWidgetState.BATTLEGROUNDS_SELECTED)
      return;
    this.m_onSelectionConfirmedCallback = OnPlayerConfirmedSelection;
    this.ShowTutorialVideo(TutorialPreviewController.TutorialPreviewWidgetState.BATTLEGROUNDS_SELECTED);
  }

  public void StartMercenariesTutorialPreviewVideo(Action OnPlayerConfirmedSelection)
  {
    if (this.m_currentState == TutorialPreviewController.TutorialPreviewWidgetState.MERCENARIES_SELECTED)
      return;
    this.m_onSelectionConfirmedCallback = OnPlayerConfirmedSelection;
    this.ShowTutorialVideo(TutorialPreviewController.TutorialPreviewWidgetState.MERCENARIES_SELECTED);
  }

  private void ResetTutorialPreview(bool isReopening = false)
  {
    this.StopVideo();
    this.m_visualController.SetState("HIDE");
    SoundManager.Get().Stop(this.m_lastPickedVO);
    this.m_isPortalAnimating = false;
    this.m_isBannerAnimating = false;
    if (isReopening)
      return;
    this.m_isReopeningPortal = false;
    this.m_currentState = TutorialPreviewController.TutorialPreviewWidgetState.NOTHING_SELECTED;
  }

  private void ShowBrassRingBanner()
  {
    if (this.m_lastCoroutine != null)
      this.StopCoroutine(this.m_lastCoroutine);
    switch (this.m_currentState)
    {
      case TutorialPreviewController.TutorialPreviewWidgetState.TRADITIONAL_SELECTED:
        this.m_brassRingVisualController.SetState("DECKBATTLE");
        this.m_lastCoroutine = this.StartCoroutine(this.PlayTutorialPreviewVO(this.m_tutorialTraditionalDescriptionVO));
        this.m_isBannerAnimating = true;
        break;
      case TutorialPreviewController.TutorialPreviewWidgetState.BATTLEGROUNDS_SELECTED:
        this.m_brassRingVisualController.SetState("BATTLEGROUNDS");
        this.m_lastCoroutine = this.StartCoroutine(this.PlayTutorialPreviewVO(this.m_tutorialBattlegroundsDescriptionVO));
        this.m_isBannerAnimating = true;
        break;
      case TutorialPreviewController.TutorialPreviewWidgetState.MERCENARIES_SELECTED:
        this.m_brassRingVisualController.SetState("MERCENARIES");
        this.m_lastCoroutine = this.StartCoroutine(this.PlayTutorialPreviewVO(this.m_tutorialMercenariesDescriptionVO));
        this.m_isBannerAnimating = true;
        break;
      default:
        Debug.LogError((object) ("TutorialPreviewController:PlayTutorialPreviewVO: Unknown state " + this.m_currentState.ToString()));
        break;
    }
  }

  private IEnumerator OpenPortal(
    TutorialPreviewController.TutorialPreviewWidgetState nextState)
  {
    TutorialPreviewController previewController = this;
    string nextVideoRef;
    switch (nextState)
    {
      case TutorialPreviewController.TutorialPreviewWidgetState.TRADITIONAL_SELECTED:
        previewController.m_tutorialPreviewDataModel.SelectedMode = "traditional";
        nextVideoRef = previewController.m_traditionalVideo;
        break;
      case TutorialPreviewController.TutorialPreviewWidgetState.BATTLEGROUNDS_SELECTED:
        previewController.m_tutorialPreviewDataModel.SelectedMode = "battlegrounds";
        nextVideoRef = previewController.m_battlegroundsVideo;
        break;
      case TutorialPreviewController.TutorialPreviewWidgetState.MERCENARIES_SELECTED:
        previewController.m_tutorialPreviewDataModel.SelectedMode = "mercenaries";
        nextVideoRef = previewController.m_mercenariesVideo;
        break;
      default:
        yield break;
    }
    previewController.m_currentState = nextState;
    previewController.m_isPortalAnimating = true;
    Action previewOpened = TutorialPreviewController.PreviewOpened;
    if (previewOpened != null)
      previewOpened();
    previewController.PrepareVideo(nextVideoRef);
    previewController.m_tutorialPreviewDataModel.IsNewPlayer = !GameUtils.IsAnyTutorialComplete();
    float timer = 0.0f;
    float videoTimeout = GameUtils.TutorialPreviewVideosTimeout();
    while (!previewController.m_videoPlayer.isPrepared && (double) timer < (double) videoTimeout)
    {
      timer += Time.unscaledDeltaTime;
      yield return (object) null;
    }
    if (!previewController.m_videoPlayer.isPrepared)
    {
      previewController.StartCoroutine(previewController.StartSelectedTutorialOnPrepareFailure());
    }
    else
    {
      previewController.ShowBrassRingBanner();
      previewController.m_visualController.SetState(GameUtils.IsAnyTutorialComplete() ? "SHOW_IN_FOREGROUND" : "SHOW");
      previewController.m_portalSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(previewController.OnSpellStateFinished));
      previewController.m_portalSpell.Activate();
      previewController.m_confirmButtonSpell.Activate();
      previewController.PlayVideo();
    }
  }

  private IEnumerator ReOpenPortal(
    TutorialPreviewController.TutorialPreviewWidgetState nextState)
  {
    this.m_isReopeningPortal = true;
    this.ClosePortal();
    while (this.m_isPortalAnimating || this.m_isBannerAnimating)
      yield return (object) null;
    yield return (object) this.OpenPortal(nextState);
  }

  public void ClosePortal()
  {
    this.m_isPortalAnimating = true;
    this.m_portalSpell.ActivateState(SpellStateType.DEATH);
    this.m_isBannerAnimating = true;
    this.m_brassRingVisualController.SetState("OUTRO");
    this.m_confirmButtonSpell.ActivateState(SpellStateType.DEATH);
    SoundManager.Get().Stop(this.m_lastPickedVO);
  }

  private void PlayVideo()
  {
    if (!(bool) (UnityEngine.Object) this.m_videoPlayer.clip)
      return;
    this.m_videoPlayer.Play();
  }

  private void StopVideo()
  {
    if (!(bool) (UnityEngine.Object) this.m_videoPlayer.clip)
      return;
    this.m_videoPlayer.Stop();
  }

  private IEnumerator StartSelectedTutorial()
  {
    if (this.m_onSelectionConfirmedCallback == null)
    {
      Debug.LogError((object) "Confirmed tutorial Selection with null Callback");
    }
    else
    {
      this.SendConfirmationToTelemetry();
      this.ClosePortal();
      while (this.IsAnimating)
        yield return (object) null;
      this.ResetTutorialPreview();
      this.m_onSelectionConfirmedCallback();
    }
  }

  private IEnumerator StartSelectedTutorialOnPrepareFailure()
  {
    Debug.LogWarning((object) "VideoPlayer.Prepare() failed to prepare video skipping tutorial preview movie.");
    if (this.m_onSelectionConfirmedCallback == null)
    {
      Debug.LogError((object) "Failed VideoPlayer.Prepare() with null Callback");
    }
    else
    {
      this.SendTimeoutToTelemetry();
      this.ResetTutorialPreview();
      this.m_onSelectionConfirmedCallback();
      yield break;
    }
  }

  private IEnumerator PlayTutorialPreviewVO(string clipReference)
  {
    SoundManager.Get().Stop(this.m_lastPickedVO);
    yield return (object) new WaitForSeconds(this.m_bannerAudioDelay);
    if (!this.m_tutorialDescriptionVOMap.ContainsKey(clipReference))
    {
      GameObject gameObject = SoundLoader.LoadSound((AssetReference) clipReference);
      if ((bool) (UnityEngine.Object) gameObject)
      {
        AudioSource component = gameObject.GetComponent<AudioSource>();
        if ((bool) (UnityEngine.Object) component)
        {
          this.m_tutorialDescriptionVOMap.Add(clipReference, component);
          SoundManager.Get().Play(component);
          this.m_lastPickedVO = component;
        }
      }
    }
    else
    {
      AudioSource tutorialDescriptionVo = this.m_tutorialDescriptionVOMap[clipReference];
      SoundManager.Get().Play(tutorialDescriptionVo);
      this.m_lastPickedVO = tutorialDescriptionVo;
    }
  }

  private void ShowPopup()
  {
    OverlayUI.Get().AddGameObject(this.gameObject, scaleMode: ((bool) UniversalInputManager.UsePhoneUI ? CanvasScaleMode.WIDTH : CanvasScaleMode.HEIGHT));
    UIContext.GetRoot().ShowPopup(this.gameObject);
  }

  private void DismissPopup() => UIContext.GetRoot().DismissPopup(this.gameObject);

  private void OnVideoPreviewEvent(string eventName)
  {
    if (!(eventName == "CONFIRM_CLICKED"))
    {
      if (!(eventName == "CANCEL_CLICKED"))
      {
        if (!(eventName == "BANNER_INTRO_FINISHED"))
        {
          if (!(eventName == "BANNER_OUTRO_FINISHED"))
          {
            if (!(eventName == "SHOW_POPUP"))
            {
              if (!(eventName == "DISMISS_POPUP"))
                return;
              this.DismissPopup();
            }
            else
              this.ShowPopup();
          }
          else
            this.m_isBannerAnimating = false;
        }
        else
          this.m_isBannerAnimating = false;
      }
      else
      {
        if (this.IsAnimating)
          return;
        this.ClosePortal();
      }
    }
    else
    {
      if (this.IsAnimating)
        return;
      this.StartCoroutine(this.StartSelectedTutorial());
    }
  }

  private void OnSpellStateFinished(Spell spell, SpellStateType type, object data)
  {
    switch (type)
    {
      case SpellStateType.BIRTH:
        this.m_isPortalAnimating = false;
        this.m_isReopeningPortal = false;
        break;
      case SpellStateType.DEATH:
        this.m_portalSpell.Deactivate();
        this.ResetTutorialPreview(this.m_isReopeningPortal);
        Action previewClosed = TutorialPreviewController.PreviewClosed;
        if (previewClosed == null)
          break;
        previewClosed();
        break;
    }
  }

  private void SendConfirmationToTelemetry()
  {
    string gameMode;
    switch (this.m_currentState)
    {
      case TutorialPreviewController.TutorialPreviewWidgetState.TRADITIONAL_SELECTED:
        gameMode = "traditional";
        break;
      case TutorialPreviewController.TutorialPreviewWidgetState.BATTLEGROUNDS_SELECTED:
        gameMode = "battlegrounds";
        break;
      case TutorialPreviewController.TutorialPreviewWidgetState.MERCENARIES_SELECTED:
        gameMode = "mercenaries";
        break;
      default:
        return;
    }
    TelemetryManager.Client().SendFTUELetsGoButtonClicked(gameMode);
  }

  private void SendTimeoutToTelemetry()
  {
    string gameMode;
    switch (this.m_currentState)
    {
      case TutorialPreviewController.TutorialPreviewWidgetState.TRADITIONAL_SELECTED:
        gameMode = "traditional";
        break;
      case TutorialPreviewController.TutorialPreviewWidgetState.BATTLEGROUNDS_SELECTED:
        gameMode = "battlegrounds";
        break;
      case TutorialPreviewController.TutorialPreviewWidgetState.MERCENARIES_SELECTED:
        gameMode = "mercenaries";
        break;
      default:
        return;
    }
    TelemetryManager.Client().SendFTUEVideoTimeout(gameMode);
  }

  private void ShowTutorialVideo(
    TutorialPreviewController.TutorialPreviewWidgetState nextState)
  {
    if ((this.m_visualController.State == "SHOW" ? 1 : (this.m_visualController.State == "SHOW_IN_FOREGROUND" ? 1 : 0)) != 0)
      this.StartCoroutine(this.ReOpenPortal(nextState));
    else
      this.StartCoroutine(this.OpenPortal(nextState));
  }

  private void PrepareVideo(string nextVideoRef)
  {
    this.m_root.SetActive(true);
    this.StopVideo();
    AssetLoader.Get().LoadAsset<VideoClip>(ref this.m_loadedVideo, (AssetReference) nextVideoRef);
    if (!(bool) this.m_loadedVideo)
    {
      Debug.LogError((object) "Tutorial video failed to load.");
    }
    else
    {
      this.m_videoPlayer.clip = (VideoClip) this.m_loadedVideo;
      this.m_videoPlayer.Prepare();
    }
  }

  private enum TutorialPreviewWidgetState
  {
    NOTHING_SELECTED,
    TRADITIONAL_SELECTED,
    BATTLEGROUNDS_SELECTED,
    MERCENARIES_SELECTED,
  }
}
