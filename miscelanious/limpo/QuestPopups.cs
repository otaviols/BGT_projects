using Assets;
using Hearthstone.Progression;
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestPopups : IDisposable
{
  private Action OnPopupShown;
  private Action OnPopupClosed;
  private Action<bool> SetIsShowing;

  public Reward.DelOnRewardLoaded OnRewardLoadedCallback { get; private set; }

  private event Action<int> OnQuestCompletedShown = achieveId => { };

  public QuestPopups(
    Action<bool> setIsShowing,
    Action onPopupShown,
    Action onPopupClosed,
    Reward.DelOnRewardLoaded onRewardLoadedCallback)
  {
    this.SetIsShowing = setIsShowing;
    this.OnPopupShown = onPopupShown;
    this.OnPopupClosed = onPopupClosed;
    this.OnRewardLoadedCallback = onRewardLoadedCallback;
  }

  public void Dispose()
  {
  }

  public void RegisterCompletedQuestShownListener(Action<int> callback)
  {
    if (callback == null)
      return;
    this.OnQuestCompletedShown -= callback;
    this.OnQuestCompletedShown += callback;
  }

  public void RemoveCompletedQuestShownListener(Action<int> callback)
  {
    if (callback == null)
      return;
    this.OnQuestCompletedShown -= callback;
  }

  public bool ShowNextCompletedQuest(
    List<Achievement> completedAchieves,
    bool suppressRewardPopupsForNewPlayer)
  {
    if (completedAchieves.Count == 0 || suppressRewardPopupsForNewPlayer)
      return false;
    if (QuestToast.IsQuestActive())
      QuestToast.GetCurrentToast().CloseQuestToast();
    Achievement completedAchieve = completedAchieves[0];
    Action<bool> setIsShowing1 = this.SetIsShowing;
    if (setIsShowing1 != null)
      setIsShowing1(true);
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    UserAttentionBlocker attentionBlocker = completedAchieve.GetUserAttentionBlocker();
    if (ReturningPlayerMgr.Get() != null && ReturningPlayerMgr.Get().IsInReturningPlayerMode && completedAchieve.ShowToReturningPlayer == Achieve.ShowToReturningPlayer.SUPPRESSED)
    {
      completedAchieves.Remove(completedAchieve);
      completedAchieve.AckCurrentProgressAndRewardNotices();
      Action<bool> setIsShowing2 = this.SetIsShowing;
      if (setIsShowing2 != null)
        setIsShowing2(false);
      return true;
    }
    if (AssetLoader.Get() != null && !string.IsNullOrEmpty(completedAchieve.CustomVisualWidget))
      AssetLoader.Get().InstantiatePrefab((AssetReference) completedAchieve.CustomVisualWidget, new PrefabCallback<GameObject>(ONAssetLoad));
    else if (!completedAchieve.UseGenericRewardVisual)
    {
      completedAchieves.Remove(completedAchieve);
      QuestToast.ShowQuestToast(attentionBlocker, (QuestToast.DelOnCloseQuestToast) (userData =>
      {
        Action<bool> setIsShowing3 = this.SetIsShowing;
        if (setIsShowing3 == null)
          return;
        setIsShowing3(false);
      }), false, completedAchieve);
      this.OnQuestCompletedShown(completedAchieve.ID);
    }
    else
    {
      completedAchieves.Remove(completedAchieve);
      completedAchieve.AckCurrentProgressAndRewardNotices();
      completedAchieve.Rewards[0].LoadRewardObject(this.OnRewardLoadedCallback);
    }
    return true;

    void ONAssetLoad(AssetReference assetRef, GameObject go, object callbackData)
    {
      OverlayUI.Get().AddGameObject(go);
      go.GetComponent<CustomVisualReward>().SetCompleteCallback((Action) (() =>
      {
        Action<bool> setIsShowing4 = this.SetIsShowing;
        if (setIsShowing4 != null)
          setIsShowing4(false);
        completedAchieves.Remove(completedAchieve);
        completedAchieve.AckCurrentProgressAndRewardNotices();
      }));
    }
  }

  public bool ShowNextQuestNotification()
  {
    if (JournalPopup.s_isShowing)
      return false;
    QuestManager questManager = QuestManager.Get();
    if (questManager == null || !questManager.HasQuestNotificationToShow() || !questManager.ShowQuestNotification(this.OnPopupClosed))
      return false;
    Action onPopupShown = this.OnPopupShown;
    if (onPopupShown != null)
      onPopupShown();
    Action<bool> setIsShowing = this.SetIsShowing;
    if (setIsShowing != null)
      setIsShowing(true);
    return true;
  }

  public void ShowQuestProgressToasts(List<Achievement> progressedAchieves)
  {
    if (!UserAttentionManager.CanShowAttentionGrabber(nameof (ShowQuestProgressToasts)) || SceneMgr.Get().GetMode() == SceneMgr.Mode.ADVENTURE && (bool) UniversalInputManager.UsePhoneUI)
      return;
    if (QuestManager.Get() != null && QuestToastManager.Get() != null)
    {
      QuestToastManager.Get()?.ShowNextQuestProgress();
    }
    else
    {
      if (progressedAchieves.Count <= 0)
        return;
      GameToastMgr.Get().ShowQuestProgressToasts(progressedAchieves);
      progressedAchieves.Clear();
    }
  }
}
