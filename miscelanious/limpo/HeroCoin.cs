using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCoin : PegUIElement
{
  public GameObject m_coin;
  public GameObject m_coinX;
  public GameObject m_cracks;
  public HighlightState m_highlight;
  public UberText m_coinLabel;
  public GameObject m_tooltip;
  public UberText m_tooltipText;
  public GameObject m_leftCap;
  public GameObject m_rightCap;
  public GameObject m_middle;
  public GameObject m_explosionPrefab;
  public bool m_inputEnabled;
  private UnityEngine.Vector2 m_goldTexture;
  private UnityEngine.Vector2 m_grayTexture;
  private UnityEngine.Vector2 m_crackTexture;
  private string m_lessonAsset;
  private UnityEngine.Vector2 m_lessonCoords;
  private int m_missionID;
  private HeroCoin.CoinStatus m_currentStatus;
  private float m_originalMiddleWidth;
  private Vector3 m_originalPosition;
  private Material m_material;
  private Material m_grayMaterial;
  private Vector3 m_originalXPosition;
  private bool m_nextTutorialStarted;
  private HeroCoin.CoinPressCallback m_coinPressCallback;

  protected override void Awake()
  {
    base.Awake();
    this.m_tooltip.SetActive(false);
  }

  private void Start() => this.m_coinLabel.Text = GameStrings.Get("GLOBAL_PLAY");

  protected override void OnDestroy()
  {
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameLoaded));
    base.OnDestroy();
  }

  public int GetMissionId() => this.m_missionID;

  public void SetCoinInfo(
    UnityEngine.Vector2 goldTexture,
    UnityEngine.Vector2 grayTexture,
    UnityEngine.Vector2 crackTexture,
    int missionID)
  {
    List<Material> materials = this.GetComponent<Renderer>().GetMaterials();
    this.m_material = materials[0];
    this.m_grayMaterial = materials[1];
    this.m_goldTexture = goldTexture;
    this.m_material.mainTextureOffset = this.m_goldTexture;
    this.m_grayTexture = grayTexture;
    this.m_grayMaterial.mainTextureOffset = this.m_grayTexture;
    this.m_material.SetColor("_Color", new Color(1f, 1f, 1f, 0.0f));
    this.m_grayMaterial.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
    this.m_crackTexture = crackTexture;
    this.m_cracks.GetComponent<Renderer>().GetMaterial().mainTextureOffset = this.m_crackTexture;
    this.m_missionID = missionID;
    this.m_tooltipText.Text = GameUtils.GetMissionHeroName(missionID);
    this.m_originalPosition = this.transform.localPosition;
    this.m_originalXPosition = this.m_coinX.transform.localPosition;
  }

  public void SetCoinPressCallback(HeroCoin.CoinPressCallback callback) => this.m_coinPressCallback = callback;

  public void SetLessonAsset(string lessonAsset) => this.m_lessonAsset = lessonAsset;

  public string GetLessonAsset() => this.m_lessonAsset;

  public void SetProgress(HeroCoin.CoinStatus status)
  {
    this.gameObject.SetActive(true);
    this.m_currentStatus = status;
    switch (status)
    {
      case HeroCoin.CoinStatus.ACTIVE:
        this.m_cracks.SetActive(false);
        this.m_coinX.SetActive(false);
        this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
        this.m_inputEnabled = true;
        break;
      case HeroCoin.CoinStatus.DEFEATED:
        this.m_material.mainTextureOffset = this.m_grayTexture;
        this.m_cracks.SetActive(true);
        this.m_coinX.SetActive(true);
        this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
        this.m_inputEnabled = false;
        this.m_coinLabel.gameObject.SetActive(false);
        break;
      case HeroCoin.CoinStatus.UNREVEALED_TO_ACTIVE:
        this.m_material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
        this.m_grayMaterial.SetColor("_Color", new Color(1f, 1f, 1f, 0.0f));
        iTween.MoveTo(this.gameObject, iTween.Hash((object) "y", (object) 3f, (object) "z", (object) (float) ((double) this.m_originalPosition.z - 0.200000002980232), (object) "time", (object) 0.5f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutCubic));
        iTween.RotateTo(this.gameObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) 1f, (object) "isLocal", (object) true, (object) "delay", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
        iTween.MoveTo(this.gameObject, iTween.Hash((object) "y", (object) this.m_originalPosition.y, (object) "z", (object) this.m_originalPosition.z, (object) "time", (object) 0.5f, (object) "isLocal", (object) true, (object) "delay", (object) 1.75f, (object) "easetype", (object) iTween.EaseType.easeOutCubic));
        iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) 0, (object) "to", (object) 1, (object) "time", (object) 0.25f, (object) "delay", (object) 1.5f, (object) "easetype", (object) iTween.EaseType.easeOutCirc, (object) "onupdate", (object) "OnUpdateAlphaVal", (object) "oncomplete", (object) "EnableInput", (object) "oncompletetarget", (object) this.gameObject));
        SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("tutorial_mission_hero_coin_rises.prefab:42026163dad364742abef7e15cb2e6cc"), this.gameObject);
        this.StartCoroutine(this.ShowCoinText());
        this.m_inputEnabled = false;
        break;
      case HeroCoin.CoinStatus.ACTIVE_TO_DEFEATED:
        this.m_coinX.transform.localPosition = new Vector3(0.0f, 10f, (bool) UniversalInputManager.UsePhoneUI ? 0.0f : -0.23f);
        this.m_cracks.SetActive(true);
        this.m_coinX.SetActive(true);
        this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
        this.m_inputEnabled = false;
        RenderUtils.SetAlpha(this.m_coinX, 0.0f);
        RenderUtils.SetAlpha(this.m_cracks, 0.0f);
        iTween.MoveTo(this.m_coinX, iTween.Hash((object) "y", (object) this.m_originalXPosition.y, (object) "z", (object) this.m_originalXPosition.z, (object) "time", (object) 0.25f, (object) "isLocal", (object) true, (object) "easetype", (object) iTween.EaseType.easeInCirc));
        iTween.FadeTo(this.m_coinX, iTween.Hash((object) "amount", (object) 1, (object) "delay", (object) 0, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeInCirc));
        iTween.FadeTo(this.m_cracks, iTween.Hash((object) "amount", (object) 1, (object) "delay", (object) 0.15f, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeInCirc));
        SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("tutorial_mission_x_descend.prefab:079781bfd4ce602448860b74693d61bd"), this.gameObject);
        this.StartCoroutine(this.SwitchCoinToGray());
        if (SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
          break;
        GameState.Get().GetGameEntity().NotifyOfDefeatCoinAnimation();
        break;
      default:
        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180f);
        this.m_coinLabel.gameObject.SetActive(false);
        this.m_cracks.SetActive(false);
        this.m_coinX.SetActive(false);
        this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
        this.m_inputEnabled = false;
        break;
    }
  }

  public IEnumerator ShowCoinText()
  {
    yield return (object) new WaitForSeconds(1.5f);
    this.m_coinLabel.gameObject.SetActive(true);
  }

  public IEnumerator SwitchCoinToGray()
  {
    yield return (object) new WaitForSeconds(0.25f);
    this.m_material.SetColor("_Color", new Color(1f, 1f, 1f, 0.0f));
    this.m_grayMaterial.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
    this.m_coinLabel.gameObject.SetActive(false);
    iTween.ShakePosition(Camera.main.gameObject, iTween.Hash((object) "amount", (object) new Vector3(0.2f, 0.2f, 0.2f), (object) "name", (object) nameof (HeroCoin), (object) "time", (object) 0.5f, (object) "delay", (object) 0, (object) "axis", (object) "none"));
  }

  public void HideTooltip() => this.m_tooltip.SetActive(false);

  public void EnableInput()
  {
    this.m_inputEnabled = true;
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }

  public void FinishIntroScaling()
  {
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOver));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOut));
    this.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPress));
    this.m_originalMiddleWidth = this.m_middle.GetComponent<Renderer>().bounds.size.x;
  }

  private void ShowTooltip()
  {
    if (this.m_currentStatus == HeroCoin.CoinStatus.UNREVEALED)
      return;
    this.m_tooltip.SetActive(true);
    float num = 0.0f;
    TransformUtil.SetLocalScaleX(this.m_middle, (this.m_tooltipText.GetTextWorldSpaceBounds().size.x + num) / this.m_originalMiddleWidth);
    float x = this.m_originalMiddleWidth * 0.223f;
    float z = this.m_originalMiddleWidth * 0.01f;
    TransformUtil.SetPoint(this.m_leftCap, Anchor.RIGHT, this.m_middle, Anchor.LEFT, new Vector3(x, 0.0f, z));
    TransformUtil.SetPoint(this.m_rightCap, Anchor.LEFT, this.m_middle, Anchor.RIGHT, new Vector3(-x, 0.0f, z));
  }

  private void OnOver(UIEvent e)
  {
    if (this.m_nextTutorialStarted || iTween.HasTween(this.gameObject))
      return;
    this.ShowTooltip();
    if (!this.m_inputEnabled)
      return;
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_MOUSE_OVER);
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("tutorial_mission_hero_coin_mouse_over.prefab:1fd7d833d7e2ffa469d6572bb08c4582"), this.gameObject);
  }

  private void OnOut(UIEvent e)
  {
    this.HideTooltip();
    if (this.m_nextTutorialStarted || !this.m_inputEnabled)
      return;
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }

  private void OnPress(UIEvent e)
  {
    if (!this.m_inputEnabled || this.m_nextTutorialStarted)
      return;
    this.m_inputEnabled = false;
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("tutorial_mission_hero_coin_play_select.prefab:781c9e3f238e74b4894b5ad9a05c4e35"), this.gameObject);
    this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    if (this.m_coinPressCallback != null)
    {
      this.m_coinPressCallback();
    }
    else
    {
      LoadingScreen.Get().AddTransitionBlocker();
      LoadingScreen.Get().AddTransitionObject(this.gameObject);
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameLoaded));
      this.StartNextTutorial();
      this.GetComponentInChildren<PlayMakerFSM>().SendEvent("Action");
    }
  }

  private void OnGameLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    this.GetComponentInChildren<PlayMakerFSM>().SendEvent("Death");
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameLoaded));
    this.StartCoroutine(this.WaitThenTransition());
  }

  private IEnumerator WaitThenTransition()
  {
    yield return (object) new WaitForSeconds(1.25f);
    LoadingScreen.Get().NotifyTransitionBlockerComplete();
  }

  private void StartNextTutorial()
  {
    this.m_nextTutorialStarted = true;
    GameMgr.Get().FindGame(GameType.GT_TUTORIAL, FormatType.FT_WILD, this.m_missionID);
  }

  public delegate void CoinPressCallback();

  public enum CoinStatus
  {
    ACTIVE,
    DEFEATED,
    UNREVEALED,
    UNREVEALED_TO_ACTIVE,
    ACTIVE_TO_DEFEATED,
  }
}
