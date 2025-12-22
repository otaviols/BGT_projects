using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeQuests : MonoBehaviour
{
  public QuestTile m_questTilePrefab;
  public Collider m_placementCollider;
  public GameObject m_placementColliderPhoneNoIksBone;
  public Banner m_headlineBanner;
  public PegUIElement m_clickCatcher;
  public UberText m_questCaption;
  public UberText m_allCompletedCaption;
  public GameObject m_friendWeekReminderContainer;
  public UberText m_friendWeekReminderCaption;
  public GameObject m_friendWeekReminderGlow;
  public Transform m_phoneNoIksCaptionBone;
  public Animation m_bannerFX;
  public GameObject m_Root;
  public GameObject[] m_normalFXs;
  public GameObject[] m_legendaryFXs;
  private static WelcomeQuests s_instance;
  private static bool s_fullScreenFXActive;
  private WelcomeQuests.ShowRequestData m_showRequestData;
  private List<QuestTile> m_currentQuests;
  private Vector3 m_originalScale;
  private float m_loginQuestShownTime;
  private bool m_bnetButtonsLocked;
  private static ScreenEffectsHandle m_screenEffectsHandle;
  private const float SPECIAL_QUEST_DISMISS_DELAY = 2.5f;

  public static bool Show(
    UserAttentionBlocker blocker,
    bool fromLogin,
    WelcomeQuests.DelOnWelcomeQuestsClosed onCloseCallback = null,
    bool keepRichPresence = false)
  {
    if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "WelcomeQuests.Show:" + fromLogin.ToString()))
    {
      if (onCloseCallback != null)
        onCloseCallback();
      return false;
    }
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.WELCOMEQUESTS);
    WelcomeQuests.ShowRequestData showRequestData = new WelcomeQuests.ShowRequestData()
    {
      m_fromLogin = fromLogin,
      m_onCloseCallback = onCloseCallback,
      m_keepRichPresence = keepRichPresence,
      m_achievement = (Achievement) null
    };
    if ((UnityEngine.Object) WelcomeQuests.s_instance != (UnityEngine.Object) null)
    {
      Debug.Log((object) "WelcomeQuests.Show(): requested to show welcome quests while it was already active!");
      WelcomeQuests.s_instance.ReinitAndShow(showRequestData);
      return true;
    }
    AssetLoader.Get().InstantiatePrefab((AssetReference) "WelcomeQuests.prefab:c1b288441ca1a05419dcb2bd498b8830", new PrefabCallback<GameObject>(WelcomeQuests.OnWelcomeQuestsLoaded), (object) showRequestData);
    return true;
  }

  public static void ShowSpecialQuest(
    UserAttentionBlocker blocker,
    Achievement achievement,
    WelcomeQuests.DelOnWelcomeQuestsClosed onCloseCallback = null,
    bool keepRichPresence = false)
  {
    if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "WelcomeQuests.ShowSpecialQuest:" + (achievement == null ? "null" : achievement.ID.ToString())))
    {
      if (onCloseCallback == null)
        return;
      onCloseCallback();
    }
    else
    {
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.WELCOMEQUESTS);
      WelcomeQuests.ShowRequestData showRequestData = new WelcomeQuests.ShowRequestData()
      {
        m_fromLogin = false,
        m_onCloseCallback = onCloseCallback,
        m_keepRichPresence = keepRichPresence,
        m_achievement = achievement
      };
      if ((UnityEngine.Object) WelcomeQuests.s_instance != (UnityEngine.Object) null)
      {
        Debug.Log((object) "WelcomeQuests.Show(): requested to show welcome quests while it was already active!");
        WelcomeQuests.s_instance.ReinitAndShow(showRequestData);
      }
      else
        AssetLoader.Get().InstantiatePrefab((AssetReference) "WelcomeQuests.prefab:c1b288441ca1a05419dcb2bd498b8830", new PrefabCallback<GameObject>(WelcomeQuests.OnWelcomeQuestsLoaded), (object) showRequestData);
    }
  }

  public static void Hide()
  {
    if ((UnityEngine.Object) WelcomeQuests.s_instance == (UnityEngine.Object) null)
      return;
    WelcomeQuests.s_instance.Close();
  }

  public static WelcomeQuests Get() => WelcomeQuests.s_instance;

  public QuestTile GetFirstQuestTile() => this.m_currentQuests[0];

  public int CompleteAndReplaceAutoDestroyQuestTile(int achieveId)
  {
    foreach (QuestTile currentQuest in this.m_currentQuests)
    {
      if (currentQuest.GetQuestID() == achieveId)
      {
        currentQuest.CompleteAndAutoDestroyQuest();
        return AchieveManager.Get().GetAchievement(achieveId).LinkToId;
      }
    }
    return 0;
  }

  public void ActivateClickCatcher()
  {
    this.m_clickCatcher.gameObject.SetActive(true);
    this.RegisterClickCatcher();
  }

  private void Awake()
  {
    this.m_originalScale = this.transform.localScale;
    this.m_headlineBanner.gameObject.SetActive(false);
    this.m_friendWeekReminderContainer.SetActive(false);
    this.m_questCaption.gameObject.SetActive(false);
    this.m_clickCatcher.gameObject.SetActive(false);
    this.m_allCompletedCaption.gameObject.SetActive(false);
    SoundManager.Get().Load((AssetReference) "new_quest_pop_up.prefab:5ef0d42842220a648bdebd874ba716e4");
    SoundManager.Get().Load((AssetReference) "existing_quest_pop_up.prefab:9b4dcb4e8233104409605a8bd5f3095d");
    SoundManager.Get().Load((AssetReference) "new_quest_click_and_shrink.prefab:601ba6676276eab43947e38f110f7b99");
    SceneMgr.Get().RegisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(this.OnPreLoadScene));
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    WelcomeQuests.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnDestroy()
  {
    WelcomeQuests.FadeEffectsOut();
    if (!((UnityEngine.Object) WelcomeQuests.s_instance != (UnityEngine.Object) null))
      return;
    this.CleanUpEventListeners();
    WelcomeQuests.s_instance = (WelcomeQuests) null;
    this.UnlockBnetButtons();
    if ((UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null && (bool) UniversalInputManager.UsePhoneUI)
      DeckPickerTrayDisplay.Get().SetHeroDetailsTrayToIgnoreFullScreenEffects(true);
    InnKeepersSpecial.Close();
  }

  private static void OnWelcomeQuestsLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if (SceneMgr.Get() != null && SceneMgr.Get().IsInGame())
    {
      if (!((UnityEngine.Object) WelcomeQuests.s_instance != (UnityEngine.Object) null))
        return;
      WelcomeQuests.s_instance.Close();
    }
    else if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("WelcomeQuests.OnWelcomeQuestsLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      WelcomeQuests.s_instance = go.GetComponent<WelcomeQuests>();
      if ((UnityEngine.Object) WelcomeQuests.s_instance == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("WelcomeQuests.OnWelcomeQuestsLoaded() - ERROR object \"{0}\" has no WelcomeQuests component", (object) assetRef));
      }
      else
      {
        WelcomeQuests.ShowRequestData showRequestData = callbackData as WelcomeQuests.ShowRequestData;
        WelcomeQuests.s_instance.InitAndShow(showRequestData);
      }
    }
  }

  private List<Achievement> GetQuestsToShow(
    WelcomeQuests.ShowRequestData showRequestData)
  {
    List<Achievement> questsToShow;
    if (showRequestData.m_achievement == null)
    {
      questsToShow = AchieveManager.Get().GetActiveQuests();
    }
    else
    {
      questsToShow = new List<Achievement>();
      questsToShow.Add(showRequestData.m_achievement);
    }
    return questsToShow;
  }

  private void InitAndShow(WelcomeQuests.ShowRequestData showRequestData)
  {
    OverlayUI.Get().AddGameObject(this.gameObject);
    this.m_showRequestData = showRequestData;
    this.LockBnetButtons();
    List<Achievement> questsToShow = this.GetQuestsToShow(this.m_showRequestData);
    if (questsToShow.Count < 1 && !InnKeepersSpecial.Get().LoadedSuccessfully())
    {
      Log.InnKeepersSpecial.Print("Skipping IKS! loadedSucsesfully={0}", (object) InnKeepersSpecial.Get().LoadedSuccessfully());
      this.Close();
    }
    else
    {
      List<Achievement> newlyAvailableQuests = new List<Achievement>();
      foreach (Achievement achievement in questsToShow)
      {
        if (achievement.IsNewlyActive())
          newlyAvailableQuests.Add(achievement);
      }
      this.m_clickCatcher.gameObject.SetActive(true);
      if (this.m_showRequestData.IsSpecialQuestRequest())
        this.Invoke("RegisterClickCatcher", 2.5f);
      else if (!AchieveManager.Get().HasActiveAutoDestroyQuests() && !AchieveManager.Get().HasActiveUnseenWelcomeQuestDialog())
        this.RegisterClickCatcher();
      this.CheckShowInnkeepersSpecial();
      this.ShowQuests();
      WelcomeQuests.FadeEffectsIn();
      if ((UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null && (bool) UniversalInputManager.UsePhoneUI)
        DeckPickerTrayDisplay.Get().SetHeroDetailsTrayToIgnoreFullScreenEffects(false);
      this.transform.localScale = new Vector3(1f / 1000f, 1f / 1000f, 1f / 1000f);
      iTween.ScaleTo(this.gameObject, this.m_originalScale, 0.5f);
      Navigation.PushUnique(new Navigation.NavigateBackHandler(WelcomeQuests.OnNavigateBack));
      NarrativeManager.Get().OnWelcomeQuestsShown(questsToShow, newlyAvailableQuests);
    }
  }

  private void ReinitAndShow(WelcomeQuests.ShowRequestData showRequestData)
  {
    WelcomeQuests.FadeEffectsOut();
    this.UnlockBnetButtons();
    this.InitAndShow(showRequestData);
  }

  private void RegisterClickCatcher()
  {
    if (!((UnityEngine.Object) WelcomeQuests.s_instance != (UnityEngine.Object) null))
      return;
    this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickCatcherClicked));
  }

  private void ShowQuests()
  {
    List<Achievement> questsToShow = this.GetQuestsToShow(this.m_showRequestData);
    if (questsToShow.Count < 1)
    {
      this.m_allCompletedCaption.gameObject.SetActive(true);
    }
    else
    {
      this.m_headlineBanner.gameObject.SetActive(true);
      if (this.m_showRequestData.IsSpecialQuestRequest())
        this.m_headlineBanner.SetText(GameStrings.Get("GLUE_SPECIAL_QUEST_NOTIFICATION_HEADER"));
      else if (this.m_showRequestData.m_fromLogin)
        this.m_headlineBanner.SetText(GameStrings.Get("GLUE_QUEST_NOTIFICATION_HEADER"));
      else
        this.m_headlineBanner.SetText(GameStrings.Get("GLUE_QUEST_NOTIFICATION_HEADER_NEW_ONLY"));
      bool flag1 = SpecialEventManager.Get().IsEventActive(SpecialEventType.FRIEND_WEEK, false) && !string.IsNullOrEmpty(GameStrings.Get("GLUE_QUEST_NOTIFICATION_CAPTION_FRIEND_WEEK"));
      bool flag2 = !flag1;
      if (!AchieveManager.Get().HasUnlockedFeature(Achieve.Unlocks.DAILY) || this.m_showRequestData.IsSpecialQuestRequest() || ReturningPlayerMgr.Get().IsInReturningPlayerMode)
      {
        flag1 = false;
        flag2 = false;
      }
      this.m_friendWeekReminderContainer.SetActive(flag1);
      this.m_questCaption.gameObject.SetActive(flag2);
      if (flag1)
        this.m_friendWeekReminderGlow.transform.localScale = this.m_friendWeekReminderGlow.transform.localScale with
        {
          x = this.m_friendWeekReminderCaption.GetTextBounds().extents.x * 2f
        };
      bool flag3 = true;
      foreach (Achievement achievement in questsToShow)
      {
        if (!achievement.IsLegendary)
        {
          flag3 = false;
          break;
        }
      }
      foreach (GameObject normalFx in this.m_normalFXs)
        normalFx.SetActive(!flag3);
      foreach (GameObject legendaryFx in this.m_legendaryFXs)
        legendaryFx.SetActive(flag3);
      this.m_currentQuests = new List<QuestTile>();
      GameObject gameObject = this.m_placementCollider.gameObject;
      PlatformDependentValue<float> platformDependentValue = new PlatformDependentValue<float>(PlatformCategory.Screen)
      {
        PC = 0.4408684f,
        Phone = 0.4208684f
      };
      if (!InnKeepersSpecial.Get().IsShown)
      {
        platformDependentValue = new PlatformDependentValue<float>(PlatformCategory.Screen)
        {
          PC = 0.4408684f,
          Phone = 0.4408684f
        };
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          if ((UnityEngine.Object) this.m_placementColliderPhoneNoIksBone != (UnityEngine.Object) null)
            gameObject = this.m_placementColliderPhoneNoIksBone;
          if ((UnityEngine.Object) this.m_phoneNoIksCaptionBone != (UnityEngine.Object) null)
          {
            this.m_friendWeekReminderContainer.transform.position = this.m_phoneNoIksCaptionBone.transform.position;
            this.m_questCaption.transform.position = this.m_phoneNoIksCaptionBone.transform.position;
          }
        }
      }
      float num1 = this.m_placementCollider.transform.position.x - this.m_placementCollider.GetComponent<Collider>().bounds.extents.x;
      float num2 = this.m_placementCollider.bounds.size.x / (float) questsToShow.Count;
      float num3 = num2 / 2f;
      for (int index = 0; index < questsToShow.Count; ++index)
      {
        Achievement quest = questsToShow[index];
        int num4 = quest.IsNewlyActive() ? 1 : 0;
        if (num4 != 0)
          this.DoInnkeeperLine(quest);
        GameObject go;
        if (quest.AutoDestroy && !string.IsNullOrEmpty(quest.QuestTilePrefabName))
        {
          go = GameUtils.LoadGameObjectWithComponent<QuestTile>(quest.QuestTilePrefabName).gameObject;
          if ((UnityEngine.Object) go == (UnityEngine.Object) null)
            go = UnityEngine.Object.Instantiate<GameObject>(this.m_questTilePrefab.gameObject);
        }
        else
          go = UnityEngine.Object.Instantiate<GameObject>(this.m_questTilePrefab.gameObject);
        LayerUtils.SetLayer(go, GameLayer.UI);
        go.transform.position = new Vector3(num1 + num3, gameObject.transform.position.y, gameObject.transform.position.z);
        go.transform.parent = this.transform;
        go.transform.localEulerAngles = new Vector3(90f, 180f, 0.0f);
        go.transform.localScale = new Vector3((float) platformDependentValue, (float) platformDependentValue, (float) platformDependentValue);
        QuestTile component = go.GetComponent<QuestTile>();
        QuestTile.FsmEvent fsmEventToPlay = num4 != 0 ? QuestTile.FsmEvent.QuestGranted : QuestTile.FsmEvent.QuestShownInQuestAlert;
        component.SetupTile(quest, fsmEventToPlay);
        this.m_currentQuests.Add(component);
        num3 += num2;
      }
      if (!this.m_showRequestData.m_fromLogin)
        return;
      this.m_loginQuestShownTime = Time.realtimeSinceStartup;
      this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.SendTelemetry));
    }
  }

  private void CheckShowInnkeepersSpecial()
  {
    int val = Options.Get().GetInt(Option.IKS_VIEW_ATTEMPTS, 0) + 1;
    Options.Get().SetInt(Option.IKS_VIEW_ATTEMPTS, val);
    bool flag1 = val > 3;
    int num = 0;
    bool flag2 = Options.Get().GetBool(Option.FORCE_SHOW_IKS);
    if (this.m_showRequestData.m_fromLogin && !ReturningPlayerMgr.Get().SuppressOldPopups)
    {
      if (flag1 | flag2)
      {
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          Vector3 localPosition = this.transform.localPosition;
          localPosition.y += 2f;
          this.transform.localPosition = localPosition;
        }
        Log.InnKeepersSpecial.Print("Showing IKS!");
        InnKeepersSpecial.Get().ShowAdAndIncrementViewCountWhenReady();
      }
      else
        Log.InnKeepersSpecial.Print("Skipping IKS! views={0} lastShownViews={1}", (object) val, (object) num);
    }
    else
      Log.InnKeepersSpecial.Print("Skipping IKS! login={0}, ReturningPlayerMgr.Get().SuppressOldPopups={1}!", (object) this.m_showRequestData.m_fromLogin, (object) ReturningPlayerMgr.Get().SuppressOldPopups);
  }

  private void DoInnkeeperLine(Achievement quest)
  {
    if (quest.ID == 11 || quest.ID != 568)
      return;
    NotificationManager.Get().CreateCharacterQuote("DemonHunter_Illidan_Popup_Banner.prefab:c2b08a2b89af02e4bb9e80b08526df7a", GameStrings.Get("VO_ILLIDAN_RETURNING_PLAYER_QUEST1"), "VO_TB_Hero_Illidan2_Male_NightElf_RP_Intro02_01.prefab:85586365c070ded4bb713703951d6bd5");
  }

  private static void FadeEffectsIn()
  {
    if (WelcomeQuests.s_fullScreenFXActive)
      return;
    WelcomeQuests.s_fullScreenFXActive = true;
    ScreenEffectParameters vignettePerspective = ScreenEffectParameters.BlurVignettePerspective with
    {
      Blur = new BlurParameters(brightness: 1f)
    };
    WelcomeQuests.m_screenEffectsHandle.StartEffect(vignettePerspective);
  }

  private static void FadeEffectsOut()
  {
    if (!WelcomeQuests.s_fullScreenFXActive || FullScreenFXMgr.Get() == null)
      return;
    WelcomeQuests.s_fullScreenFXActive = false;
    WelcomeQuests.m_screenEffectsHandle.StopEffect();
  }

  private void OnFatalError(FatalErrorMessage message, object userData) => this.Close();

  private void Close()
  {
    this.CleanUpEventListeners();
    this.UnlockBnetButtons();
    WelcomeQuests.s_instance = (WelcomeQuests) null;
    WelcomeQuests.FadeEffectsOut();
    if ((UnityEngine.Object) DeckPickerTrayDisplay.Get() != (UnityEngine.Object) null && (bool) UniversalInputManager.UsePhoneUI)
      DeckPickerTrayDisplay.Get().SetHeroDetailsTrayToIgnoreFullScreenEffects(true);
    if (this.m_currentQuests != null)
    {
      foreach (QuestTile currentQuest in this.m_currentQuests)
        currentQuest.OnClose();
    }
    if ((UnityEngine.Object) this.gameObject != (UnityEngine.Object) null)
    {
      iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "time", (object) 0.5f, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "DestroyWelcomeQuests"));
      SoundManager.Get().LoadAndPlay((AssetReference) "new_quest_click_and_shrink.prefab:601ba6676276eab43947e38f110f7b99");
      this.m_bannerFX.Play("BannerClose");
    }
    if (this.m_showRequestData != null)
    {
      if (!this.m_showRequestData.m_keepRichPresence)
        PresenceMgr.Get().SetPrevStatus();
      if (this.m_showRequestData.m_onCloseCallback != null)
        this.m_showRequestData.m_onCloseCallback();
    }
    InnKeepersSpecial.Close();
  }

  public static bool OnNavigateBack()
  {
    if ((UnityEngine.Object) WelcomeQuests.s_instance != (UnityEngine.Object) null)
      WelcomeQuests.s_instance.Close();
    return true;
  }

  private void OnClickCatcherClicked(UIEvent e) => this.Close();

  private void DestroyWelcomeQuests() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

  private void OnPreLoadScene(SceneMgr.Mode prevMode, SceneMgr.Mode nextMode, object userData)
  {
    if (nextMode != SceneMgr.Mode.GAMEPLAY)
      return;
    this.Close();
  }

  private void SendTelemetry(UIEvent e)
  {
    float questAckDuration = Time.realtimeSinceStartup - this.m_loginQuestShownTime;
    TelemetryManager.Client().SendWelcomeQuestsAcknowledged(questAckDuration);
    this.m_clickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.SendTelemetry));
  }

  private void CleanUpEventListeners()
  {
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(WelcomeQuests.OnNavigateBack));
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(this.OnPreLoadScene));
    this.m_clickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickCatcherClicked));
    this.m_clickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.SendTelemetry));
    FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
  }

  private void LockBnetButtons()
  {
    if ((UnityEngine.Object) BaseUI.Get() == (UnityEngine.Object) null || this.m_bnetButtonsLocked)
      return;
    BaseUI.Get().m_BnetBar.RequestDisableButtons();
    this.m_bnetButtonsLocked = true;
  }

  private void UnlockBnetButtons()
  {
    if ((UnityEngine.Object) BaseUI.Get() == (UnityEngine.Object) null || !this.m_bnetButtonsLocked)
      return;
    BaseUI.Get().m_BnetBar.CancelRequestToDisableButtons();
    this.m_bnetButtonsLocked = false;
  }

  public delegate void DelOnWelcomeQuestsClosed();

  private class ShowRequestData
  {
    public bool m_fromLogin;
    public WelcomeQuests.DelOnWelcomeQuestsClosed m_onCloseCallback;
    public bool m_keepRichPresence;
    public Achievement m_achievement;

    public bool IsSpecialQuestRequest() => this.m_achievement != null;
  }
}
