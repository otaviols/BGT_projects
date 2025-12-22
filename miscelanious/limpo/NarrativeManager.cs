using Assets;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using PegasusLettuce;
using PegasusShared;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
  private const float DELAY_TIME_FOR_QUEST_PROGRESS = 1.5f;
  private const float DELAY_TIME_FOR_QUEST_COMPLETE = 1.5f;
  private const float DELAY_TIME_FOR_AUTO_DESTROY_QUEST_RECEIVED = 3.8f;
  private const float DELAY_TIME_BEFORE_QUEST_DESTROY = 0.8f;
  private const float DELAY_TIME_FOR_AUTO_DESTROY_POST_DESTROY = 1.3f;
  private const float DELAY_TIME_BEFORE_SHOW_BANNER = 1f;
  private const float FALLBACK_DURATION_ON_AUDIO_LOADING_FAIL = 3.5f;
  private static NarrativeManager s_instance;
  private Map<string, AudioSource> m_preloadedSounds = new Map<string, AudioSource>();
  private int m_preloadsNeeded;
  private Queue<CharacterDialogSequence> m_characterDialogSequenceToShow = new Queue<CharacterDialogSequence>();
  private Notification m_activeCharacterDialogNotification;
  private bool m_isBannerShowing;
  private bool m_showingBlockingDialog;
  private bool m_isProcessingQueuedDialogSequence;
  private bool m_hasDoneAllPopupsShownEvent;
  private static Map<ScheduledCharacterDialogEvent, Option> m_lastSeenScheduledCharacterDialogOptions = new Map<ScheduledCharacterDialogEvent, Option>()
  {
    {
      ScheduledCharacterDialogEvent.DOUBLE_GOLD_QUEST_GRANTED,
      Option.LATEST_SEEN_SCHEDULED_DOUBLE_GOLD_VO
    },
    {
      ScheduledCharacterDialogEvent.ALL_POPUPS_SHOWN,
      Option.LATEST_SEEN_SCHEDULED_ALL_POPUPS_SHOWN_VO
    },
    {
      ScheduledCharacterDialogEvent.ENTERED_ARENA_DRAFT,
      Option.LATEST_SEEN_SCHEDULED_ENTERED_ARENA_DRAFT
    },
    {
      ScheduledCharacterDialogEvent.LOGIN_FLOW_COMPLETE,
      Option.LATEST_SEEN_SCHEDULED_LOGIN_FLOW_COMPLETE
    },
    {
      ScheduledCharacterDialogEvent.WELCOME_QUESTS_SHOWN,
      Option.LATEST_SEEN_SCHEDULED_WELCOME_QUEST_SHOWN_VO
    },
    {
      ScheduledCharacterDialogEvent.GENERIC_REWARD_SHOWN,
      Option.LATEST_SEEN_SCHEDULED_GENERIC_REWARD_SHOWN_VO
    },
    {
      ScheduledCharacterDialogEvent.ARENA_REWARD_SHOWN,
      Option.LATEST_SEEN_SCHEDULED_ARENA_REWARD_SHOWN_VO
    }
  };
  private static Map<ScheduledCharacterDialogEvent, GameSaveDataManager.GameSaveKeyTuple> m_lastSeenScheduledCharacterDialogKeys = new Map<ScheduledCharacterDialogEvent, GameSaveDataManager.GameSaveKeyTuple>()
  {
    {
      ScheduledCharacterDialogEvent.ENTERED_BATTLEGROUNDS,
      new GameSaveDataManager.GameSaveKeyTuple()
      {
        Key = GameSaveKeyId.CHARACTER_DIALOG,
        Subkey = GameSaveKeySubkeyId.CHARACTER_DIALOG_LAST_SEEN_BACON
      }
    },
    {
      ScheduledCharacterDialogEvent.BATTLEGROUNDS_LUCKY_DRAW_BUTTON_SHOWN,
      new GameSaveDataManager.GameSaveKeyTuple()
      {
        Key = GameSaveKeyId.CHARACTER_DIALOG,
        Subkey = GameSaveKeySubkeyId.CHARACTER_DIALOG_LAST_SEEN_BACON
      }
    },
    {
      ScheduledCharacterDialogEvent.ENTERED_TAVERN_BRAWL,
      new GameSaveDataManager.GameSaveKeyTuple()
      {
        Key = GameSaveKeyId.CHARACTER_DIALOG,
        Subkey = GameSaveKeySubkeyId.CHARACTER_DIALOG_LAST_SEEN_TAVERN_BRAWL
      }
    },
    {
      ScheduledCharacterDialogEvent.PURCHASED_BUNDLE,
      new GameSaveDataManager.GameSaveKeyTuple()
      {
        Key = GameSaveKeyId.CHARACTER_DIALOG,
        Subkey = GameSaveKeySubkeyId.CHARACTER_DIALOG_LAST_SEEN_PURCHASED_BUNDLE
      }
    },
    {
      ScheduledCharacterDialogEvent.ENTERED_LUCKY_DRAW,
      new GameSaveDataManager.GameSaveKeyTuple()
      {
        Key = GameSaveKeyId.CHARACTER_DIALOG,
        Subkey = GameSaveKeySubkeyId.CHARACTER_DIALOG_LAST_SEEN_LUCKY_DRAW
      }
    }
  };
  private Map<ScheduledCharacterDialogEvent, List<ScheduledCharacterDialogDbfRecord>> m_scheduledCharacterDialogData = new Map<ScheduledCharacterDialogEvent, List<ScheduledCharacterDialogDbfRecord>>();

  public void Awake() => NarrativeManager.s_instance = this;

  public void Update()
  {
    if (!this.m_showingBlockingDialog)
      return;
    OverlayUI.Get().RequestActivateClickBlocker();
  }

  public void OnDestroy()
  {
    if (!((UnityEngine.Object) NarrativeManager.s_instance != (UnityEngine.Object) null))
      return;
    this.CleanUpEverything();
    NarrativeManager.s_instance = (NarrativeManager) null;
  }

  public static NarrativeManager Get() => NarrativeManager.s_instance;

  public void Initialize()
  {
    HearthstoneApplication.Get().WillReset += new Action(this.WillReset);
    SceneMgr.Get().RegisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(this.OnScenePreLoad));
    PopupDisplayManager.Get().QuestPopups.RegisterCompletedQuestShownListener(new Action<int>(NarrativeManager.s_instance.OnQuestCompleteShown));
    PopupDisplayManager.Get().RewardPopups.RegisterGenericRewardShownListener(new Action<long>(NarrativeManager.s_instance.OnGenericRewardShown));
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheMercenariesVillageVisitorInfo), new Action(this.OnVillageVisitorStateUpdated));
    StoreManager.Get().RegisterSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnBundlePurchased));
    this.StartCoroutine(this.WaitForAchievesThenInit());
  }

  private void OnScenePreLoad(SceneMgr.Mode prevMode, SceneMgr.Mode mode, object userData)
  {
    if (mode != SceneMgr.Mode.GAMEPLAY)
      return;
    this.CleanUpExceptListeners();
  }

  public void OnQuestCompleteShown(int achieveId)
  {
    Achievement achievement = AchieveManager.Get().GetAchievement(achieveId);
    if (achievement.QuestDialogId == 0 || achievement.OnCompleteDialogSequence == null)
      return;
    if (achievement.OnCompleteDialogSequence.m_deferOnComplete)
      this.EnqueueIfNotPresent(achievement.OnCompleteDialogSequence);
    else
      this.PushDialogSequence(achievement.OnCompleteDialogSequence);
  }

  public void OnGenericRewardShown(long originData) => this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.GENERIC_REWARD_SHOWN, (long) (int) originData);

  private void EnqueueIfNotPresent(CharacterDialogSequence sequence)
  {
    foreach (CharacterDialogSequence characterDialogSequence in this.m_characterDialogSequenceToShow)
    {
      if (characterDialogSequence.m_characterDialogRecord == sequence.m_characterDialogRecord)
        return;
    }
    this.m_characterDialogSequenceToShow.Enqueue(sequence);
  }

  public void ShowOutstandingQuestDialogs() => this.StartCoroutine(this.ShowOutstandingCharacterDialogSequence());

  public void OnWelcomeQuestsShown(
    List<Achievement> questsShown,
    List<Achievement> newlyAvailableQuests)
  {
    this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.WELCOME_QUESTS_SHOWN);
    bool flag = SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_GOLD_DOUBLED, false);
    foreach (Achievement quest in questsShown)
    {
      if (quest.AutoDestroy)
      {
        this.StartCoroutine(this.DestroyAndReplaceQuest(quest));
        break;
      }
      if (quest.QuestDialogId != 0)
      {
        this.StartCoroutine(this.HandleQuestReceived(quest));
        break;
      }
      if (flag && quest.IsAffectedByDoubleGold && newlyAvailableQuests.Contains(quest) && !AchieveManager.Get().HasActiveAutoDestroyQuests() && !AchieveManager.Get().HasActiveUnseenWelcomeQuestDialog() && this.OnDoubleGoldQuestGranted())
        break;
    }
  }

  public bool HasCharacterDialogSequenceToShow() => this.m_characterDialogSequenceToShow.Count > 0;

  public bool IsShowingBlockingDialog() => this.m_showingBlockingDialog;

  public void PushDialogSequence(CharacterDialogSequence sequence)
  {
    this.EnqueueIfNotPresent(sequence);
    this.StartCoroutine(this.ShowOutstandingCharacterDialogSequence());
  }

  public IEnumerator<IAsyncJobResult> Job_WaitForOutstandingCharacterDialog()
  {
    NarrativeManager narrativeManager = this;
    narrativeManager.StartCoroutine(narrativeManager.ShowOutstandingCharacterDialogSequence());
    while (narrativeManager.m_isProcessingQueuedDialogSequence)
      yield return (IAsyncJobResult) null;
  }

  public IEnumerator ShowOutstandingCharacterDialogSequence(
    int villageRecordID = 0,
    bool skipPreDialogueWait = false,
    Action doneCallback = null)
  {
    NarrativeManager narrativeManager = this;
    if (narrativeManager.m_characterDialogSequenceToShow.Count != 0 && !narrativeManager.m_isProcessingQueuedDialogSequence)
    {
      narrativeManager.m_isProcessingQueuedDialogSequence = true;
      if (!skipPreDialogueWait)
        yield return (object) new WaitForSeconds(1.5f);
      int bannerIDToShow = 0;
      while (narrativeManager.m_characterDialogSequenceToShow.Count > 0)
      {
        CharacterDialogSequence dialogSequence = narrativeManager.m_characterDialogSequenceToShow.Peek();
        narrativeManager.SetDialogBlocker(dialogSequence.m_blockInput);
        if (dialogSequence != null && dialogSequence.m_onCompleteBannerId != 0)
          bannerIDToShow = dialogSequence.m_onCompleteBannerId;
        if (dialogSequence.m_onPreShow != null)
          dialogSequence.m_onPreShow(dialogSequence);
        yield return (object) narrativeManager.StartCoroutine(narrativeManager.PlayerCharacterDialogSequence(dialogSequence));
        narrativeManager.m_characterDialogSequenceToShow.Dequeue();
        if (villageRecordID != 0)
          narrativeManager.MarkVillageDialogueAsSeen(villageRecordID);
        if (doneCallback != null)
        {
          doneCallback();
          break;
        }
      }
      if (bannerIDToShow != 0)
      {
        yield return (object) new WaitForSeconds(1f);
        narrativeManager.m_isBannerShowing = true;
        BannerManager.Get().ShowBanner(bannerIDToShow, new BannerManager.DelOnCloseBanner(narrativeManager.OnQuestDialogCompleteBannerClosed));
      }
      narrativeManager.SetDialogBlocker(false);
      while (narrativeManager.m_isBannerShowing)
        yield return (object) null;
      narrativeManager.m_isProcessingQueuedDialogSequence = false;
    }
  }

  public bool OnDoubleGoldQuestGranted() => this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.DOUBLE_GOLD_QUEST_GRANTED);

  public bool OnAllPopupsShown()
  {
    if (this.m_hasDoneAllPopupsShownEvent)
      return false;
    this.m_hasDoneAllPopupsShownEvent = true;
    return this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.ALL_POPUPS_SHOWN);
  }

  public bool OnArenaDraftStarted() => this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.ENTERED_ARENA_DRAFT);

  public bool OnArenaRewardsShown() => this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.ARENA_REWARD_SHOWN);

  public void OnLoginFlowComplete() => this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.LOGIN_FLOW_COMPLETE);

  public bool OnBattlegroundsEntered() => this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.ENTERED_BATTLEGROUNDS);

  public bool OnBattlegroundsLuckyDrawButtonShown() => this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.BATTLEGROUNDS_LUCKY_DRAW_BUTTON_SHOWN);

  public bool OnLuckyDrawEntered() => this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.ENTERED_LUCKY_DRAW);

  public bool OnTavernBrawlEntered() => this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.ENTERED_TAVERN_BRAWL);

  public void OnBundlePurchased(Network.Bundle bundle, PaymentMethod purchaseMethod)
  {
    if (!((Record) bundle != (Record) null))
      return;
    this.TriggerScheduledCharacterDialogEvent(ScheduledCharacterDialogEvent.PURCHASED_BUNDLE, bundle.PMTProductID.Value);
  }

  private void SetDialogBlocker(bool value)
  {
    this.m_showingBlockingDialog = value;
    if (FriendChallengeMgr.Get() == null)
      return;
    FriendChallengeMgr.Get().UpdateMyAvailability();
  }

  private void OnQuestDialogCompleteBannerClosed() => this.m_isBannerShowing = false;

  private IEnumerator WaitForAchievesThenInit()
  {
    while (AchieveManager.Get() == null)
      yield return (object) null;
    while (!AchieveManager.Get().IsReady())
      yield return (object) null;
    this.PreloadActiveQuestDialog();
    this.InitScheduledCharacterDialogData();
    AchieveManager.Get().RegisterAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(NarrativeManager.s_instance.OnAchievesUpdated));
    GameToastMgr.Get().RegisterQuestProgressToastShownListener(new GameToastMgr.QuestProgressToastShownCallback(NarrativeManager.s_instance.OnQuestProgressToastShown));
    TavernBrawlManager.Get().OnTavernBrawlUpdated += new Action(NarrativeManager.s_instance.OnTavernBrawlUpdated);
  }

  private IEnumerator DestroyAndReplaceQuest(Achievement quest)
  {
    NarrativeManager narrativeManager = this;
    yield return (object) new WaitForSeconds(3.8f);
    SoundDucker ducker = (SoundDucker) null;
    ducker = narrativeManager.gameObject.AddComponent<SoundDucker>();
    ducker.m_DuckedCategoryDefs = new List<SoundDuckedCategoryDef>();
    foreach (Global.SoundCategory soundCategory in Enum.GetValues(typeof (Global.SoundCategory)))
    {
      switch (soundCategory)
      {
        case Global.SoundCategory.MUSIC:
        case Global.SoundCategory.AMBIENCE:
          ducker.m_DuckedCategoryDefs.Add(new SoundDuckedCategoryDef()
          {
            m_Category = soundCategory,
            m_BeginSec = 0.0f
          });
          continue;
        default:
          continue;
      }
    }
    ducker.StartDucking();
    CharacterDialog dialog;
    if (quest.QuestDialogId != 0)
    {
      foreach (CharacterDialog characterDialog in quest.OnReceivedDialogSequence)
      {
        dialog = characterDialog;
        if (NarrativeManager.IsCharacterDialogDisplayable(dialog))
        {
          yield return (object) new WaitForSeconds(dialog.waitBefore);
          yield return (object) narrativeManager.StartCoroutine(narrativeManager.PlayCharacterQuoteAndWait(dialog));
          yield return (object) new WaitForSeconds(dialog.waitAfter);
        }
        dialog = new CharacterDialog();
      }
    }
    yield return (object) new WaitForSeconds(0.8f);
    int nextQuestId = WelcomeQuests.Get().CompleteAndReplaceAutoDestroyQuestTile(quest.ID);
    yield return (object) new WaitForSeconds(1.3f);
    Achievement achievement = AchieveManager.Get().GetAchievement(nextQuestId);
    if (achievement.QuestDialogId != 0)
    {
      int numLinesToPlay = achievement.OnReceivedDialogSequence.Count;
      foreach (CharacterDialog characterDialog in achievement.OnReceivedDialogSequence)
      {
        dialog = characterDialog;
        --numLinesToPlay;
        if (NarrativeManager.IsCharacterDialogDisplayable(dialog))
        {
          yield return (object) new WaitForSeconds(dialog.waitBefore);
          if (numLinesToPlay == 0)
            yield return (object) narrativeManager.StartCoroutine(narrativeManager.PlayCharacterQuoteAndWait(dialog, new NarrativeManager.CharacterQuotePlayedCallback(narrativeManager.OnWelcomeQuestNarrativeFinished)));
          else
            yield return (object) narrativeManager.StartCoroutine(narrativeManager.PlayCharacterQuoteAndWait(dialog));
          yield return (object) new WaitForSeconds(dialog.waitAfter);
        }
        dialog = new CharacterDialog();
      }
    }
    if ((UnityEngine.Object) ducker != (UnityEngine.Object) null)
    {
      ducker.StopDucking();
      UnityEngine.Object.Destroy((UnityEngine.Object) ducker);
    }
  }

  private IEnumerator HandleQuestReceived(Achievement quest)
  {
    NarrativeManager narrativeManager = this;
    int numLinesToPlay = quest.OnReceivedDialogSequence.Count;
    if (Options.Get().GetInt(Option.LATEST_SEEN_WELCOME_QUEST_DIALOG) == quest.ID || numLinesToPlay <= 0)
    {
      narrativeManager.OnWelcomeQuestNarrativeFinished();
    }
    else
    {
      SoundDucker ducker = (SoundDucker) null;
      ducker = narrativeManager.gameObject.AddComponent<SoundDucker>();
      ducker.m_DuckedCategoryDefs = new List<SoundDuckedCategoryDef>();
      foreach (Global.SoundCategory soundCategory in Enum.GetValues(typeof (Global.SoundCategory)))
      {
        switch (soundCategory)
        {
          case Global.SoundCategory.MUSIC:
          case Global.SoundCategory.AMBIENCE:
            ducker.m_DuckedCategoryDefs.Add(new SoundDuckedCategoryDef()
            {
              m_Category = soundCategory,
              m_BeginSec = 0.0f
            });
            continue;
          default:
            continue;
        }
      }
      ducker.StartDucking();
      foreach (CharacterDialog characterDialog in quest.OnReceivedDialogSequence)
      {
        CharacterDialog dialog = characterDialog;
        --numLinesToPlay;
        if (NarrativeManager.IsCharacterDialogDisplayable(dialog))
        {
          yield return (object) new WaitForSeconds(dialog.waitBefore);
          if (numLinesToPlay == 0)
            yield return (object) narrativeManager.StartCoroutine(narrativeManager.PlayCharacterQuoteAndWait(dialog, new NarrativeManager.CharacterQuotePlayedCallback(narrativeManager.OnWelcomeQuestNarrativeFinished)));
          else
            yield return (object) narrativeManager.StartCoroutine(narrativeManager.PlayCharacterQuoteAndWait(dialog));
          yield return (object) new WaitForSeconds(dialog.waitAfter);
        }
        dialog = new CharacterDialog();
      }
      Options.Get().SetInt(Option.LATEST_SEEN_WELCOME_QUEST_DIALOG, quest.ID);
      if ((UnityEngine.Object) ducker != (UnityEngine.Object) null)
      {
        ducker.StopDucking();
        UnityEngine.Object.Destroy((UnityEngine.Object) ducker);
      }
    }
  }

  private IEnumerator PlayCharacterQuoteAndWait(
    CharacterDialog dialog,
    NarrativeManager.CharacterQuotePlayedCallback callback = null,
    float waitTimeScale = 1f)
  {
    float minimumDurationSeconds = dialog.minimumDurationSeconds;
    if (Localization.DoesLocaleNeedExtraReadingTime(Localization.GetLocale()))
      minimumDurationSeconds += dialog.localeExtraSeconds;
    AudioSource audioSource = (AudioSource) null;
    bool noSoundSpecified = string.IsNullOrEmpty(dialog.audioName);
    if (!noSoundSpecified)
    {
      audioSource = this.GetPreloadedSound(dialog.audioName);
      if ((UnityEngine.Object) audioSource == (UnityEngine.Object) null || (UnityEngine.Object) audioSource.clip == (UnityEngine.Object) null)
      {
        this.RemovePreloadedSound(dialog.audioName);
        this.PreloadSound(dialog.audioName);
        while (this.IsPreloadingAssets())
          yield return (object) null;
        audioSource = this.GetPreloadedSound(dialog.audioName);
        if ((UnityEngine.Object) audioSource == (UnityEngine.Object) null || (UnityEngine.Object) audioSource.clip == (UnityEngine.Object) null)
        {
          Debug.Log((object) ("NarrativeManager.PlaySoundAndWait() - sound error - " + dialog.audioName));
          yield break;
        }
      }
    }
    float durationSeconds = minimumDurationSeconds;
    if ((UnityEngine.Object) audioSource != (UnityEngine.Object) null)
      durationSeconds = Mathf.Max(minimumDurationSeconds, audioSource.clip.length);
    else if ((UnityEngine.Object) audioSource == (UnityEngine.Object) null && !noSoundSpecified)
      durationSeconds = 3.5f;
    float waitTime = durationSeconds * waitTimeScale;
    waitTime += 0.5f;
    Log.NarrativeManager.Print("PlayCharacterQuoteAndWait - durationSeconds: {0}  waitTimeScale: {1}", (object) durationSeconds, (object) waitTimeScale);
    string str = string.IsNullOrEmpty((string) dialog.bubbleText) ? (string.IsNullOrEmpty(dialog.audioName) ? "***TEXT NOT FOUND***" : GameStrings.Get(new AssetReference(dialog.audioName).GetLegacyAssetName())) : dialog.bubbleText.GetString();
    if (dialog.useInnkeeperQuote)
    {
      this.m_activeCharacterDialogNotification = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, str, dialog.audioName, (Action<int>) null);
      this.m_activeCharacterDialogNotification.ShowWithExistingPopups = true;
    }
    else if (dialog.useBannerStyle && !string.IsNullOrEmpty(dialog.bannerPrefabName))
      this.m_activeCharacterDialogNotification = NotificationManager.Get().CreateCharacterQuote(dialog.bannerPrefabName, NotificationManager.GetDefaultDialogueBannerPos(dialog.canvasAnchor), str, dialog.audioName, durationSeconds: durationSeconds, finishCallback: ((Action<int>) (groupId => waitTime = 0.0f)), anchorPoint: dialog.canvasAnchor);
    else if (!dialog.useBannerStyle && !string.IsNullOrEmpty(dialog.prefabName))
      this.m_activeCharacterDialogNotification = NotificationManager.Get().CreateBigCharacterQuoteWithText(dialog.prefabName, NotificationManager.DEFAULT_CHARACTER_POS, dialog.audioName, str, durationSeconds, (Action<int>) (groupId => waitTime = 0.0f), true, dialog.useAltSpeechBubble ? Notification.SpeechBubbleDirection.TopLeft : Notification.SpeechBubbleDirection.BottomLeft, dialog.persistPrefab, dialog.useAltPosition);
    while ((double) waitTime > 0.0)
    {
      waitTime -= Time.deltaTime;
      yield return (object) null;
    }
    if (callback != null)
      callback();
  }

  private void OnAchievesUpdated(
    List<Achievement> updatedAchieves,
    List<Achievement> completedAchieves,
    object userData)
  {
    this.PreloadQuestDialog(AchieveManager.Get().GetActiveQuests());
  }

  private void OnQuestProgressToastShown(int achieveId) => this.StartCoroutine(this.HandleOnQuestProgressToastShown(achieveId));

  private void OnTavernBrawlUpdated()
  {
    if (!TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
      return;
    foreach (TavernBrawlMission mission in TavernBrawlManager.Get().Missions)
    {
      if (mission.FirstTimeSeenCharacterDialogID > 0)
        this.PreloadDialogSequence(mission.FirstTimeSeenCharacterDialogSequence);
    }
  }

  private IEnumerator HandleOnQuestProgressToastShown(int achieveId)
  {
    yield return (object) new WaitForSeconds(1.5f);
    Achievement achievement = AchieveManager.Get().GetAchievement(achieveId);
    if ((achievement != null ? achievement.QuestDialogId : 0) != 0)
    {
      if (achievement.Progress == 1)
        yield return (object) this.PlayerCharacterDialogSequence(achievement.OnProgress1DialogSequence);
      else if (achievement.Progress == 2)
        yield return (object) this.PlayerCharacterDialogSequence(achievement.OnProgress2DialogSequence);
    }
  }

  public void OnAchieveDismissed(Achievement achieve)
  {
    if (achieve.OnDismissDialogSequence == null)
      return;
    this.StartCoroutine(this.PlayerCharacterDialogSequence(achieve.OnDismissDialogSequence));
  }

  private static bool IsCharacterDialogDisplayable(CharacterDialog dialog)
  {
    if (dialog.useInnkeeperQuote || !string.IsNullOrEmpty(dialog.prefabName))
      return true;
    Log.All.Print("CharacterDialogItem id={0} is not displayable. To be displayable, either USE_INNKEEPER_QUOTE must be true or PREFAB_NAME is not null/empty.", (object) dialog.dbfRecordId);
    return false;
  }

  private IEnumerator PlayerCharacterDialogSequence(
    CharacterDialogSequence dialogSequence)
  {
    NarrativeManager narrativeManager = this;
    if (dialogSequence != null)
    {
      if (!dialogSequence.m_ignorePopups)
        yield return (object) narrativeManager.StartCoroutine(PopupDisplayManager.Get().WaitForAllPopups());
      foreach (CharacterDialog characterDialog in dialogSequence)
      {
        CharacterDialog dialog = characterDialog;
        if (NarrativeManager.IsCharacterDialogDisplayable(dialog))
        {
          yield return (object) new WaitForSeconds(dialog.waitBefore);
          yield return (object) narrativeManager.StartCoroutine(narrativeManager.PlayCharacterQuoteAndWait(dialog));
          yield return (object) new WaitForSeconds(dialog.waitAfter);
        }
        dialog = new CharacterDialog();
      }
    }
  }

  private void OnWelcomeQuestNarrativeFinished()
  {
    if (!((UnityEngine.Object) WelcomeQuests.Get() != (UnityEngine.Object) null))
      return;
    WelcomeQuests.Get().ActivateClickCatcher();
  }

  private void PreloadActiveQuestDialog() => this.PreloadQuestDialog(AchieveManager.Get().GetActiveQuests());

  private void PreloadQuestDialog(Achievement achievement)
  {
    if (achievement.QuestDialogId == 0)
      return;
    this.PreloadDialogSequence(achievement.OnReceivedDialogSequence);
    this.PreloadDialogSequence(achievement.OnCompleteDialogSequence);
    this.PreloadDialogSequence(achievement.OnProgress1DialogSequence);
    this.PreloadDialogSequence(achievement.OnProgress2DialogSequence);
    this.PreloadDialogSequence(achievement.OnDismissDialogSequence);
  }

  private void PreloadQuestDialog(List<Achievement> activeAchievements)
  {
    foreach (Achievement activeAchievement in activeAchievements)
      this.PreloadQuestDialog(activeAchievement);
  }

  private void PreloadDialogSequence(CharacterDialogSequence questDialogSequence)
  {
    foreach (CharacterDialog characterDialog in questDialogSequence)
    {
      if (!string.IsNullOrEmpty(characterDialog.audioName))
        this.PreloadSound(characterDialog.audioName);
    }
  }

  private void PreloadQuestDialog(List<string> audioNames)
  {
    foreach (string audioName in audioNames)
    {
      if (!string.IsNullOrEmpty(audioName))
        this.PreloadSound(audioName);
    }
  }

  private void PreloadSound(string soundPath)
  {
    if (this.CheckPreloadedSound(soundPath))
      return;
    ++this.m_preloadsNeeded;
    SoundLoader.LoadSound((AssetReference) soundPath, new PrefabCallback<GameObject>(this.OnSoundLoaded), fallback: SoundManager.Get().GetPlaceholderSound());
  }

  private void OnSoundLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    --this.m_preloadsNeeded;
    if (assetRef == null)
      Debug.LogWarning((object) string.Format("NarrativeManager.OnSoundLoaded() - Asset ref was null)"));
    else if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("NarrativeManager.OnSoundLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    }
    else
    {
      AudioSource component = go.GetComponent<AudioSource>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("NarrativeManager.OnSoundLoaded() - ERROR \"{0}\" has no Spell component", (object) assetRef));
      }
      else
      {
        if (this.CheckPreloadedSound(assetRef.ToString()))
          return;
        this.m_preloadedSounds.Add(assetRef.ToString(), component);
      }
    }
  }

  private void RemovePreloadedSound(string soundPath) => this.m_preloadedSounds.Remove(soundPath);

  private bool CheckPreloadedSound(string soundPath) => this.m_preloadedSounds.TryGetValue(soundPath, out AudioSource _);

  private AudioSource GetPreloadedSound(string soundPath)
  {
    AudioSource preloadedSound;
    if (this.m_preloadedSounds.TryGetValue(soundPath, out preloadedSound))
      return preloadedSound;
    Debug.LogError((object) string.Format("NarrativeManager.GetPreloadedSound() - \"{0}\" was not preloaded", (object) soundPath));
    return (AudioSource) null;
  }

  private bool IsPreloadingAssets() => this.m_preloadsNeeded > 0;

  private void SetLastSeenScheduledCharacterDialog(
    int scheduledDialogId,
    ScheduledCharacterDialogEvent eventType)
  {
    if (eventType == ScheduledCharacterDialogEvent.INVALID)
      Log.NarrativeManager.PrintError("NarrativeManager.SetLastSeenScheduledCharacterDialog was passed an INVALID ScheduledCharacterDialogEvent");
    else if (NarrativeManager.m_lastSeenScheduledCharacterDialogKeys.ContainsKey(eventType))
      this.SetLastSeenScheduledCharacterDialog_GameSaveData(scheduledDialogId, eventType);
    else if (NarrativeManager.m_lastSeenScheduledCharacterDialogOptions.ContainsKey(eventType))
      this.SetLastSeenScheduledCharacterDialog_ServerOption(scheduledDialogId, eventType);
    else
      Log.NarrativeManager.PrintError("NarrativeManager has no storage mechanism for event {0}", (object) eventType.ToString());
  }

  private int GetLastSeenScheduledCharacterDialog(ScheduledCharacterDialogEvent eventType)
  {
    if (eventType == ScheduledCharacterDialogEvent.INVALID)
    {
      Log.NarrativeManager.PrintError("NarrativeManager.GetLastSeenScheduledCharacterDialog was passed an INVALID ScheduledCharacterDialogEvent");
      return -1;
    }
    if (NarrativeManager.m_lastSeenScheduledCharacterDialogKeys.ContainsKey(eventType))
      return this.GetLastSeenScheduledCharacterDialog_GameSaveData(eventType);
    if (NarrativeManager.m_lastSeenScheduledCharacterDialogOptions.ContainsKey(eventType))
      return this.GetLastSeenScheduledCharacterDialog_ServerOption(eventType);
    Log.NarrativeManager.PrintError("NarrativeManager has no storage mechanism for event {0}", (object) eventType.ToString());
    return -1;
  }

  private void SetLastSeenScheduledCharacterDialog_ServerOption(
    int scheduledDialogId,
    ScheduledCharacterDialogEvent eventType)
  {
    Option option;
    NarrativeManager.m_lastSeenScheduledCharacterDialogOptions.TryGetValue(eventType, out option);
    if (option == Option.INVALID)
      Log.NarrativeManager.PrintError("NarrativeManager.SetLastSeenScheduledCharacterDialog option mapping had no corresponding option for event: {0}", (object) eventType);
    else
      Options.Get().SetInt(option, scheduledDialogId);
  }

  private int GetLastSeenScheduledCharacterDialog_ServerOption(
    ScheduledCharacterDialogEvent eventType)
  {
    Option option;
    NarrativeManager.m_lastSeenScheduledCharacterDialogOptions.TryGetValue(eventType, out option);
    if (option != Option.INVALID)
      return Options.Get().GetInt(option);
    Log.NarrativeManager.PrintError("NarrativeManager.GetLastSeenScheduledCharacterDialog option mapping had no corresponding option for event: {0}", (object) eventType);
    return -1;
  }

  private void SetLastSeenScheduledCharacterDialog_GameSaveData(
    int scheduledDialogId,
    ScheduledCharacterDialogEvent eventType)
  {
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(NarrativeManager.m_lastSeenScheduledCharacterDialogKeys[eventType].Key, NarrativeManager.m_lastSeenScheduledCharacterDialogKeys[eventType].Subkey, new long[1]
    {
      (long) scheduledDialogId
    }));
  }

  private int GetLastSeenScheduledCharacterDialog_GameSaveData(
    ScheduledCharacterDialogEvent eventType)
  {
    long dialogGameSaveData;
    GameSaveDataManager.Get().GetSubkeyValue(NarrativeManager.m_lastSeenScheduledCharacterDialogKeys[eventType].Key, NarrativeManager.m_lastSeenScheduledCharacterDialogKeys[eventType].Subkey, out dialogGameSaveData);
    return (int) dialogGameSaveData;
  }

  private void InitScheduledCharacterDialogData()
  {
    foreach (ScheduledCharacterDialogDbfRecord record in GameDbf.ScheduledCharacterDialog.GetRecords())
    {
      if (GeneralUtils.ForceBool(record.Enabled))
      {
        SpecialEventType eventType = record.Event;
        if ((record.Event == SpecialEventType.UNKNOWN || !SpecialEventManager.Get().HasEventEnded(eventType)) && (record.ShowToNewPlayer || GameUtils.IsAnyTutorialComplete()) && (record.ShowToReturningPlayer || !ReturningPlayerMgr.Get().IsInReturningPlayerMode))
        {
          ScheduledCharacterDialogEvent characterDialogEvent = EnumUtils.GetEnum<ScheduledCharacterDialogEvent>(record.ClientEvent.ToString(), StringComparison.OrdinalIgnoreCase);
          if (this.GetLastSeenScheduledCharacterDialogDisplayOrder(characterDialogEvent) < record.DisplayOrder)
          {
            if (!this.m_scheduledCharacterDialogData.ContainsKey(characterDialogEvent))
              this.m_scheduledCharacterDialogData[characterDialogEvent] = new List<ScheduledCharacterDialogDbfRecord>();
            this.PreloadQuestDialog(CharacterDialogSequence.GetAudioOfCharacterDialogSequence(record.CharacterDialogId));
            this.m_scheduledCharacterDialogData[characterDialogEvent].Add(record);
          }
        }
      }
    }
  }

  private int GetLastSeenScheduledCharacterDialogDisplayOrder(
    ScheduledCharacterDialogEvent dialogEvent)
  {
    int scheduledCharacterDialog = this.GetLastSeenScheduledCharacterDialog(dialogEvent);
    int dialogDisplayOrder = -1;
    ScheduledCharacterDialogDbfRecord record = GameDbf.ScheduledCharacterDialog.GetRecord(scheduledCharacterDialog);
    if (record != null)
      dialogDisplayOrder = record.DisplayOrder;
    return dialogDisplayOrder;
  }

  public void ResetScheduledCharacterDialogEvent_Debug()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    foreach (ScheduledCharacterDialogEvent eventType in Enum.GetValues(typeof (ScheduledCharacterDialogEvent)))
    {
      if (eventType != ScheduledCharacterDialogEvent.INVALID)
        this.SetLastSeenScheduledCharacterDialog(0, eventType);
    }
    this.InitScheduledCharacterDialogData();
  }

  public bool TriggerScheduledCharacterDialogEvent_Debug(ScheduledCharacterDialogEvent eventType) => !HearthstoneApplication.IsPublic() && this.TriggerScheduledCharacterDialogEvent(eventType);

  private bool TriggerScheduledCharacterDialogEvent(
    ScheduledCharacterDialogEvent eventType,
    long eventData = 0)
  {
    if (!this.m_scheduledCharacterDialogData.ContainsKey(eventType) || UserAttentionManager.IsBlockedBy(UserAttentionBlocker.SET_ROTATION_INTRO))
      return false;
    ScheduledCharacterDialogDbfRecord recordToUse = (ScheduledCharacterDialogDbfRecord) null;
    int dialogDisplayOrder = this.GetLastSeenScheduledCharacterDialogDisplayOrder(eventType);
    foreach (ScheduledCharacterDialogDbfRecord characterDialogDbfRecord in this.m_scheduledCharacterDialogData[eventType])
    {
      SpecialEventType eventType1 = characterDialogDbfRecord.Event;
      if ((eventType1 == SpecialEventType.UNKNOWN || SpecialEventManager.Get().IsEventActive(eventType1, false)) && (eventData == 0L || eventData == characterDialogDbfRecord.ClientEventData) && characterDialogDbfRecord.DisplayOrder > dialogDisplayOrder && (recordToUse == null || recordToUse.DisplayOrder > characterDialogDbfRecord.DisplayOrder))
        recordToUse = characterDialogDbfRecord;
    }
    if (recordToUse == null)
      return false;
    CharacterDialogSequence sequence1 = new CharacterDialogSequence(recordToUse.CharacterDialogId);
    if (sequence1 == null)
      return false;
    sequence1.m_onPreShow = (Action<CharacterDialogSequence>) (sequence => this.SetLastSeenScheduledCharacterDialog(recordToUse.ID, eventType));
    this.PushDialogSequence(sequence1);
    return true;
  }

  private void OnVillageVisitorStateUpdated() => this.PreloadDialogForActiveVillageTasks();

  public void PreloadMercenaryTutorialDialogue()
  {
    if (GameUtils.IsMercenariesVillageTutorialComplete())
      return;
    foreach (LettuceTutorialVoDbfRecord record in GameDbf.LettuceTutorialVo.GetRecords())
    {
      if (record.TutorialDialog != 0)
        this.PreloadDialogSequence(new CharacterDialogSequence(record.TutorialDialog));
    }
  }

  private void PreloadDialogForActiveVillageTasks()
  {
    foreach (MercenariesVisitorState visitorState in LettuceVillageDataUtil.VisitorStates)
    {
      if (visitorState.ActiveTaskState != null)
      {
        VisitorTaskDbfRecord taskRecordById = LettuceVillageDataUtil.GetTaskRecordByID(visitorState.ActiveTaskState.TaskId);
        if (taskRecordById != null)
        {
          if (taskRecordById.OnAssignedDialog != 0)
            this.PreloadDialogSequence(new CharacterDialogSequence(taskRecordById.OnAssignedDialog));
          if (taskRecordById.OnCompleteDialog != 0)
            this.PreloadDialogSequence(new CharacterDialogSequence(taskRecordById.OnCompleteDialog));
        }
      }
    }
  }

  public void PreloadDialogForActiveVillageBuildings()
  {
    foreach (BuildingTierDbfRecord buildingTierDbfRecord in LettuceVillageDataUtil.GetTierRecordsThatCanBeBuilt())
    {
      if (buildingTierDbfRecord.OnUpgradedDialog != 0)
        this.PreloadDialogSequence(new CharacterDialogSequence(buildingTierDbfRecord.OnUpgradedDialog));
    }
  }

  public void OnVillageTaskClaimed(VisitorTaskDbfRecord record, Action callback = null)
  {
    if (record == null || record.OnCompleteDialog == 0)
      return;
    this.PlayVillageDialogue(record.OnCompleteDialog, callback);
  }

  public void OnVillageEntered()
  {
    foreach (MercenariesVisitorState visitorState in LettuceVillageDataUtil.VisitorStates)
    {
      if (visitorState.ActiveTaskState != null)
      {
        VisitorTaskDbfRecord taskRecordById = LettuceVillageDataUtil.GetTaskRecordByID(visitorState.ActiveTaskState.TaskId);
        if (taskRecordById != null && taskRecordById.OnAssignedDialog != 0 && this.CanPlayVillageDialogue(taskRecordById.OnAssignedDialog))
          this.PlayVillageDialogue(taskRecordById.OnAssignedDialog);
      }
    }
  }

  public void OnVillageBuildingUpgraded(BuildingTierDbfRecord record, Action callback = null)
  {
    if (record.OnUpgradedDialog <= 0)
      return;
    this.PlayVillageDialogue(record.OnUpgradedDialog, callback);
  }

  private void PlayVillageDialogue(int recordID, Action doneCallback = null)
  {
    this.EnqueueIfNotPresent(new CharacterDialogSequence(recordID));
    this.StartCoroutine(this.ShowOutstandingCharacterDialogSequence(recordID, true, doneCallback));
  }

  public void PlayMercenariesTutorialDialogue(int recordID, Action doneCallback = null)
  {
    this.EnqueueIfNotPresent(new CharacterDialogSequence(recordID));
    this.StartCoroutine(this.ShowOutstandingCharacterDialogSequence(skipPreDialogueWait: true, doneCallback: doneCallback));
  }

  private void MarkVillageDialogueAsSeen(int dialogID)
  {
    GameSaveDataManager.SubkeySaveRequest subkeyIfItExists = GameSaveDataManager.Get().GenerateSaveRequestToRemoveValueFromSubkeyIfItExists(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_VILLAGE_RECENTLY_PLAYED_TASK_DIALOGS, (long) dialogID);
    if (subkeyIfItExists == null)
      return;
    GameSaveDataManager.Get().SaveSubkey(subkeyIfItExists);
  }

  private bool CanPlayVillageDialogue(int dialogID)
  {
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_VILLAGE_RECENTLY_PLAYED_TASK_DIALOGS, out values);
    return !(values ?? new List<long>()).Contains((long) dialogID);
  }

  private void WillReset() => this.CleanUpEverything();

  private void CleanUpEverything()
  {
    this.CleanUpExceptListeners();
    AchieveManager service1;
    if (ServiceManager.TryGet<AchieveManager>(out service1))
      service1.RemoveAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(this.OnAchievesUpdated));
    if ((UnityEngine.Object) GameToastMgr.Get() != (UnityEngine.Object) null)
      GameToastMgr.Get().RemoveQuestProgressToastShownListener(new GameToastMgr.QuestProgressToastShownCallback(this.OnQuestProgressToastShown));
    PopupDisplayManager service2;
    if (ServiceManager.TryGet<PopupDisplayManager>(out service2))
      service2.QuestPopups.RemoveCompletedQuestShownListener(new Action<int>(this.OnQuestCompleteShown));
    TavernBrawlManager service3;
    if (ServiceManager.TryGet<TavernBrawlManager>(out service3))
      service3.OnTavernBrawlUpdated -= new Action(this.OnTavernBrawlUpdated);
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      HearthstoneApplication.Get().WillReset -= new Action(this.WillReset);
    SceneMgr service4;
    if (ServiceManager.TryGet<SceneMgr>(out service4))
      service4.UnregisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(this.OnScenePreLoad));
    LoginManager service5;
    if (ServiceManager.TryGet<LoginManager>(out service5))
      service5.OnFullLoginFlowComplete -= new Action(this.OnLoginFlowComplete);
    if (StoreManager.Get() != null)
      StoreManager.Get().RemoveSuccessfulPurchaseListener(new Action<Network.Bundle, PaymentMethod>(this.OnBundlePurchased));
    if (NetCache.Get() == null)
      return;
    NetCache.Get().RemoveUpdatedListener(typeof (NetCache.NetCacheMercenariesVillageVisitorInfo), new Action(this.OnVillageVisitorStateUpdated));
  }

  private void CleanUpExceptListeners()
  {
    this.StopAllCoroutines();
    this.m_characterDialogSequenceToShow.Clear();
    this.m_preloadedSounds.Clear();
    if ((UnityEngine.Object) NotificationManager.Get() != (UnityEngine.Object) null && (UnityEngine.Object) this.m_activeCharacterDialogNotification != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotification(this.m_activeCharacterDialogNotification, 0.0f);
    this.m_preloadsNeeded = 0;
    this.m_isBannerShowing = false;
    this.m_showingBlockingDialog = false;
    this.m_isProcessingQueuedDialogSequence = false;
    this.m_hasDoneAllPopupsShownEvent = false;
  }

  public List<Option> Cheat_ClearAllSeen()
  {
    List<Option> optionList = new List<Option>();
    optionList.AddRange((IEnumerable<Option>) NarrativeManager.m_lastSeenScheduledCharacterDialogOptions.Values);
    optionList.Add(Option.LATEST_SEEN_WELCOME_QUEST_DIALOG);
    optionList.Add(Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD);
    optionList.Add(Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON_CHALKBOARD);
    optionList.Add(Option.LATEST_SEEN_TAVERNBRAWL_SEASON);
    optionList.Add(Option.LATEST_SEEN_FIRESIDEBRAWL_SEASON);
    foreach (Option option in optionList)
      Options.Get().DeleteOption(option);
    return optionList;
  }

  public delegate void CharacterQuotePlayedCallback();
}
