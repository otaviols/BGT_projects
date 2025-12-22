using Blizzard.T5.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPopupDisplay : TransitionPopup
{
  public UberText m_tipOfTheDay;
  public ProgressBar m_progressBar;
  public GameObject m_loadingTile;
  public GameObject m_cancelButtonParent;
  public List<LoadingPopupDisplay.LoadingbarTexture> m_barTextures = new List<LoadingPopupDisplay.LoadingbarTexture>();
  private Map<AdventureDbId, List<string>> m_adventureTaskNameMap = new Map<AdventureDbId, List<string>>();
  private List<string> m_mercenariesTaskNameList = new List<string>();
  private Map<AdventureDbId, List<string>> m_adventureTipOfTheDayNameMap = new Map<AdventureDbId, List<string>>();
  private List<string> m_spectatorTaskNameMap = new List<string>();
  private bool m_stopAnimating;
  private bool m_animationStopped;
  private AudioSource m_loopSound;
  private bool m_barAnimating;
  public static readonly Vector3 START_POS = new Vector3(-0.0152f, -0.0894f, -0.0837f);
  public static readonly Vector3 MID_POS = new Vector3(-0.0152f, -0.0894f, 0.0226f);
  public static readonly Vector3 END_POS = new Vector3(-0.0152f, 0.0368f, 0.0226f);
  public static readonly Vector3 OFFSCREEN_POS = new Vector3(-0.0152f, -0.0894f, 0.13f);
  private const int TASK_DURATION_VARIATION = 2;
  private const float ROTATION_DURATION = 0.5f;
  private const float ROTATION_DELAY = 0.5f;
  private const float SLIDE_IN_TIME = 0.5f;
  private const float SLIDE_OUT_TIME = 0.25f;
  private const float RAISE_TIME = 0.5f;
  private const float LOWER_TIME = 0.25f;
  private const string SHOW_CANCEL_BUTTON_TWEEN_NAME = "ShowCancelButton";
  private const float SHOW_CANCEL_BUTTON_THRESHOLD = 30f;

  protected override void Awake()
  {
    base.Awake();
    this.GenerateStringNameMaps();
    this.m_title.Text = GameStrings.Get("GLUE_STARTING_GAME");
    this.gameObject.transform.localPosition = new Vector3(-0.05f, 9f, 3.908f);
    SoundManager.Get().Load((AssetReference) "StartGame_window_expand_up.prefab:1989383da054858489f420f8e2ac43d4");
    SoundManager.Get().Load((AssetReference) "StartGame_window_shrink_down.prefab:07b3273ed29d9df479442d93caa07799");
    SoundManager.Get().Load((AssetReference) "StartGame_window_loading_bar_move_down_and_forward.prefab:0b04e30939289024dadd8292dbfd7fef");
    SoundManager.Get().Load((AssetReference) "StartGame_window_loading_bar_flip.prefab:fa63e6a9075dba24fae4fddc2fd32a39");
    SoundManager.Get().Load((AssetReference) "StartGame_window_bar_filling_loop.prefab:4e8350e37c1218a4cbbd9e93b394cd48");
    SoundManager.Get().Load((AssetReference) "StartGame_window_loading_bar_drop.prefab:899774ec312ca8241a5a5e4e300c5d93");
    this.DisableCancelButton();
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (SoundManager.Get() == null)
      return;
    this.StopLoopingSound();
  }

  public override void Hide()
  {
    if (!this.m_shown)
      return;
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnNavigateBack));
    base.Hide();
  }

  protected override bool EnableCancelButtonIfPossible()
  {
    if (!base.EnableCancelButtonIfPossible())
      return false;
    TransformUtil.SetLocalPosX((Component) this.m_queueTab, -0.3234057f);
    return true;
  }

  protected override void EnableCancelButton()
  {
    this.m_cancelButtonParent.SetActive(true);
    base.EnableCancelButton();
  }

  protected override void DisableCancelButton()
  {
    base.DisableCancelButton();
    this.m_cancelButtonParent.SetActive(false);
  }

  protected override void AnimateShow()
  {
    iTween.Timer(this.gameObject, iTween.Hash((object) "name", (object) "ShowCancelButton", (object) "time", (object) 30f, (object) "ignoretimescale", (object) true, (object) "oncomplete", (object) new Action<object>(this.OnCancelButtonShowTimerCompleted), (object) "oncompletetarget", (object) this.gameObject));
    this.SetTipOfTheDay();
    this.SetLoadingBarTexture();
    SoundManager.Get().LoadAndPlay((AssetReference) "StartGame_window_expand_up.prefab:1989383da054858489f420f8e2ac43d4");
    base.AnimateShow();
    this.m_stopAnimating = false;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
  }

  protected void OnCancelButtonShowTimerCompleted(object userData) => this.EnableCancelButtonIfPossible();

  protected override void OnGameEntered(FindGameEventData eventData)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    base.OnGameEntered(eventData);
  }

  protected override void OnGameUpdated(FindGameEventData eventData)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    base.OnGameUpdated(eventData);
  }

  protected override void AnimateHide()
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "StartGame_window_shrink_down.prefab:07b3273ed29d9df479442d93caa07799");
    iTween.StopByName(this.gameObject, "ShowCancelButton");
    if (this.m_barAnimating)
    {
      this.StopCoroutine("AnimateBar");
      this.m_barAnimating = false;
      this.StopLoopingSound();
    }
    base.AnimateHide();
  }

  protected override void OnAnimateShowFinished()
  {
    base.OnAnimateShowFinished();
    this.AnimateInLoadingTile();
  }

  private void AnimateInLoadingTile()
  {
    if (this.m_stopAnimating)
    {
      this.m_animationStopped = true;
    }
    else
    {
      this.m_loadingTile.transform.localEulerAngles = new Vector3(180f, 0.0f, 0.0f);
      this.m_loadingTile.transform.localPosition = LoadingPopupDisplay.START_POS;
      this.m_progressBar.SetProgressBar(0.0f);
      iTween.MoveTo(this.m_loadingTile, iTween.Hash((object) "position", (object) LoadingPopupDisplay.MID_POS, (object) "isLocal", (object) true, (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutBounce));
      SoundManager.Get().LoadAndPlay((AssetReference) "StartGame_window_loading_bar_move_down_and_forward.prefab:0b04e30939289024dadd8292dbfd7fef");
      iTween.MoveTo(this.m_loadingTile, iTween.Hash((object) "position", (object) LoadingPopupDisplay.END_POS, (object) "isLocal", (object) true, (object) "time", (object) 0.5f, (object) "delay", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutCubic));
      iTween.RotateAdd(this.m_loadingTile, iTween.Hash((object) "amount", (object) new Vector3(180f, 0.0f, 0.0f), (object) "time", (object) 0.5f, (object) "delay", (object) 0.8f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self, (object) "name", (object) "flip"));
      this.m_progressBar.SetLabel(this.GetRandomTaskName());
      this.StartCoroutine("AnimateBar");
    }
  }

  private void AnimateOutLoadingTile()
  {
    iTween.MoveTo(this.m_loadingTile, iTween.Hash((object) "position", (object) LoadingPopupDisplay.MID_POS, (object) "isLocal", (object) true, (object) "time", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeOutBounce));
    SoundManager.Get().LoadAndPlay((AssetReference) "StartGame_window_loading_bar_drop.prefab:899774ec312ca8241a5a5e4e300c5d93");
    iTween.MoveTo(this.m_loadingTile, iTween.Hash((object) "position", (object) LoadingPopupDisplay.OFFSCREEN_POS, (object) "isLocal", (object) true, (object) "time", (object) 0.25f, (object) "delay", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "AnimateInLoadingTile", (object) "oncompletetarget", (object) this.gameObject));
  }

  private float GetRandomTaskDuration() => (float) (1.0 + (double) UnityEngine.Random.value * 2.0);

  private string GetRandomTaskName()
  {
    List<string> stringList;
    if (GameMgr.Get().IsSpectator())
      stringList = this.m_spectatorTaskNameMap;
    else if (!this.m_adventureTaskNameMap.TryGetValue(this.m_adventureId, out stringList))
      stringList = !GameUtils.IsMercenariesMission(this.m_scenarioId) ? this.m_adventureTaskNameMap[AdventureDbId.INVALID] : this.m_mercenariesTaskNameList;
    if (stringList.Count == 0)
      return "ERROR - OUT OF TASK NAMES!!!";
    int index = UnityEngine.Random.Range(0, stringList.Count);
    return stringList[index];
  }

  private IEnumerator AnimateBar()
  {
    LoadingPopupDisplay loadingPopupDisplay = this;
    loadingPopupDisplay.m_barAnimating = true;
    yield return (object) new WaitForSeconds(0.8f);
    SoundManager.Get().LoadAndPlay((AssetReference) "StartGame_window_loading_bar_flip.prefab:fa63e6a9075dba24fae4fddc2fd32a39");
    yield return (object) new WaitForSeconds(0.2f);
    float randomTaskDuration = loadingPopupDisplay.GetRandomTaskDuration();
    loadingPopupDisplay.m_progressBar.m_increaseAnimTime = randomTaskDuration;
    loadingPopupDisplay.m_progressBar.AnimateProgress(0.0f, 1f);
    SoundManager.Get().LoadAndPlay((AssetReference) "StartGame_window_bar_filling_loop.prefab:4e8350e37c1218a4cbbd9e93b394cd48", (GameObject) null, 1f, new SoundManager.LoadedCallback(loadingPopupDisplay.LoopingSoundLoadedCallback));
    yield return (object) new WaitForSeconds(randomTaskDuration);
    loadingPopupDisplay.StopLoopingSound();
    loadingPopupDisplay.AnimateOutLoadingTile();
    loadingPopupDisplay.m_barAnimating = false;
  }

  private void LoopingSoundLoadedCallback(AudioSource source, object userData)
  {
    this.StopLoopingSound();
    if (this.m_barAnimating)
      this.m_loopSound = source;
    else
      SoundManager.Get().Stop(source);
  }

  protected override void OnGameplaySceneLoaded()
  {
    this.StartCoroutine(this.StopLoading());
    Navigation.Clear();
  }

  private IEnumerator StopLoading()
  {
    LoadingPopupDisplay loadingPopupDisplay = this;
    loadingPopupDisplay.m_stopAnimating = true;
    while (!loadingPopupDisplay.m_animationStopped)
      yield return (object) null;
    if (loadingPopupDisplay.m_adventureId == AdventureDbId.PRACTICE)
    {
      int num = Options.Get().GetInt(Option.TIP_PRACTICE_PROGRESS, 0);
      Options.Get().SetInt(Option.TIP_PRACTICE_PROGRESS, num + 1);
    }
    loadingPopupDisplay.Hide();
  }

  private void StopLoopingSound()
  {
    SoundManager.Get().Stop(this.m_loopSound);
    this.m_loopSound = (AudioSource) null;
  }

  private bool OnNavigateBack()
  {
    if (!this.m_cancelButtonParent.gameObject.activeSelf)
      return false;
    this.StartCoroutine(this.StopLoading());
    this.FireMatchCanceledEvent();
    return true;
  }

  protected override void OnCancelButtonReleased(UIEvent e)
  {
    base.OnCancelButtonReleased(e);
    Navigation.GoBack();
  }

  private void GenerateStringNameMaps()
  {
    this.GenerateTaskNamesForAdventure(AdventureDbId.INVALID, "GLUE_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.NAXXRAMAS, "GLUE_NAXX_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.BRM, "GLUE_BRM_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.LOE, "GLUE_LOE_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.KARA, "GLUE_KARA_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.ICC, "GLUE_ICC_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.LOOT, "GLUE_LOOT_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.GIL, "GLUE_GIL_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.BOT, "GLUE_BOT_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.TRL, "GLUE_TRL_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.DALARAN, "GLUE_DAL_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.ULDUM, "GLUE_ULD_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.DRAGONS, "GLUE_DRG_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.BTA, "GLUE_BTA_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.BOH, "GLUE_BOH_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.BOM, "GLUE_BOH_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForAdventure(AdventureDbId.ROTLK, "GLUE_BOH_LOADING_BAR_TASK_");
    this.GenerateTaskNamesForPrefix(this.m_mercenariesTaskNameList, "GLUE_LET_LOADING_BAR_TASK_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.LOOT, "GLUE_TIP_ADVENTURE_LOOT_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.GIL, "GLUE_TIP_ADVENTURE_GIL_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.BOT, "GLUE_TIP_ADVENTURE_BOT_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.TRL, "GLUE_TIP_ADVENTURE_TRL_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.DALARAN, "GLUE_TIP_ADVENTURE_DAL_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.ULDUM, "GLUE_TIP_ADVENTURE_ULD_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.DRAGONS, "GLUE_TIP_ADVENTURE_DRG_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.BTP, "GLUE_TIP_ADVENTURE_BTP_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.BTA, "GLUE_TIP_ADVENTURE_BTA_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.BOH, "GLUE_TIP_ADVENTURE_BOH_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.BOM, "GLUE_TIP_ADVENTURE_BOM_");
    this.GenerateTipOfTheDayNamesForAdventure(AdventureDbId.ROTLK, "GLUE_TIP_ADVENTURE_RLK_");
    this.GenerateTaskNamesForPrefix(this.m_spectatorTaskNameMap, "GLUE_SPECTATOR_LOADING_BAR_TASK_");
  }

  private void GenerateTaskNamesForAdventure(AdventureDbId adventureId, string prefix)
  {
    List<string> taskNames = new List<string>();
    this.GenerateTaskNamesForPrefix(taskNames, prefix);
    this.m_adventureTaskNameMap[adventureId] = taskNames;
  }

  private void GenerateTipOfTheDayNamesForAdventure(AdventureDbId adventureId, string prefix)
  {
    List<string> taskNames = new List<string>();
    this.GenerateTaskNamesForPrefix(taskNames, prefix);
    this.m_adventureTipOfTheDayNameMap[adventureId] = taskNames;
  }

  private void GenerateTaskNamesForPrefix(List<string> taskNames, string prefix)
  {
    taskNames.Clear();
    for (int index = 1; index < 100; ++index)
    {
      string key = prefix + (object) index;
      string str = GameStrings.Get(key);
      if (str == key)
        break;
      taskNames.Add(str);
    }
  }

  private void SetTipOfTheDay()
  {
    if (GameUtils.IsMercenariesMission(this.m_scenarioId))
      this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.LETTUCE);
    else if (this.m_adventureId == AdventureDbId.PRACTICE)
      this.m_tipOfTheDay.Text = GameStrings.GetTip(TipCategory.PRACTICE, new int?(Options.Get().GetInt(Option.TIP_PRACTICE_PROGRESS, 0)));
    else if (GameUtils.IsExpansionAdventure(this.m_adventureId))
    {
      List<string> stringList;
      if (this.m_adventureTipOfTheDayNameMap.TryGetValue(this.m_adventureId, out stringList) && stringList != null && stringList.Count > 0)
      {
        int index = UnityEngine.Random.Range(0, stringList.Count);
        this.m_tipOfTheDay.Text = stringList[index];
      }
      else
        this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.ADVENTURE);
    }
    else if (this.m_scenarioId == 3539 || this.m_scenarioId == 3459)
      this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.BACON);
    else
      this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.DEFAULT);
  }

  private void SetLoadingBarTexture()
  {
    Texture texture = this.m_barTextures[0].texture;
    foreach (LoadingPopupDisplay.LoadingbarTexture barTexture in this.m_barTextures)
    {
      if (barTexture.adventureID == this.m_adventureId || barTexture.scenarioId == (ScenarioDbId) this.m_scenarioId)
      {
        texture = barTexture.texture;
        this.m_progressBar.m_barIntensity = barTexture.m_barIntensity;
        this.m_progressBar.m_barIntensityIncreaseMax = barTexture.m_barIntensityIncreaseMax;
        break;
      }
    }
    this.m_progressBar.SetBarTexture(texture);
  }

  [Serializable]
  public class LoadingbarTexture
  {
    public AdventureDbId adventureID;
    public ScenarioDbId scenarioId;
    public Texture texture;
    public float m_barIntensity = 1.2f;
    public float m_barIntensityIncreaseMax = 3f;
  }
}
