using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameToastMgr : MonoBehaviour
{
  private const float FADE_IN_TIME = 0.25f;
  private const float FADE_OUT_TIME = 0.5f;
  private const float HOLD_TIME = 4f;
  private PlatformDependentValue<Vector3> MULTIPLE_TOAST_OFFSET = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.0f, 0.0f, 30f),
    Phone = new Vector3(0.0f, 0.0f, 80f)
  };
  public QuestProgressToast m_questProgressToastPrefab;
  private static GameToastMgr s_instance;
  private List<GameToast> m_toasts = new List<GameToast>();
  private List<GameToastMgr.QuestProgressToastShownListener> m_questProgressToastShownListeners = new List<GameToastMgr.QuestProgressToastShownListener>();

  private void Awake() => GameToastMgr.s_instance = this;

  private void OnDestroy() => GameToastMgr.s_instance = (GameToastMgr) null;

  public static GameToastMgr Get() => GameToastMgr.s_instance;

  public bool RegisterQuestProgressToastShownListener(
    GameToastMgr.QuestProgressToastShownCallback callback)
  {
    if (callback == null)
      return false;
    GameToastMgr.QuestProgressToastShownListener toastShownListener = new GameToastMgr.QuestProgressToastShownListener();
    toastShownListener.SetCallback(callback);
    toastShownListener.SetUserData((object) null);
    if (this.m_questProgressToastShownListeners.Contains(toastShownListener))
      return false;
    this.m_questProgressToastShownListeners.Add(toastShownListener);
    return true;
  }

  public bool RemoveQuestProgressToastShownListener(
    GameToastMgr.QuestProgressToastShownCallback callback)
  {
    if (callback == null)
      return false;
    GameToastMgr.QuestProgressToastShownListener toastShownListener = new GameToastMgr.QuestProgressToastShownListener();
    toastShownListener.SetCallback(callback);
    toastShownListener.SetUserData((object) null);
    return this.m_questProgressToastShownListeners.Remove(toastShownListener);
  }

  private void FireAllQuestProgressListeners(int achieveId, int progress)
  {
    foreach (GameToastMgr.QuestProgressToastShownListener toastShownListener in this.m_questProgressToastShownListeners.ToArray())
      toastShownListener.Fire(achieveId);
  }

  private bool AddToast(GameToast toast)
  {
    toast.transform.parent = OverlayUI.Get().m_QuestProgressToastBone.transform;
    toast.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 180f, 0.0f));
    toast.transform.localScale = new Vector3(110f, 1f, 110f);
    toast.transform.localPosition = Vector3.zero;
    this.m_toasts.Add(toast);
    RenderUtils.SetAlpha(toast.gameObject, 0.0f);
    this.UpdateToastPositions();
    Hashtable args = iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) 0.25f, (object) "delay", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "FadeOutToast", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) toast);
    iTween.FadeTo(toast.gameObject, args);
    return true;
  }

  public bool AreToastsActive() => this.m_toasts.Count > 0;

  private void FadeOutToast(GameToast toast)
  {
    Hashtable args = iTween.Hash((object) "amount", (object) 0.0f, (object) "delay", (object) 4f, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "DeactivateToast", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) toast);
    iTween.FadeTo(toast.gameObject, args);
  }

  private void DeactivateToast(GameToast toast)
  {
    toast.gameObject.SetActive(false);
    this.m_toasts.Remove(toast);
    this.UpdateToastPositions();
  }

  private void UpdateToastPositions()
  {
    int num = 0;
    foreach (GameToast toast in this.m_toasts)
    {
      if (num > 0)
        TransformUtil.SetPoint(toast.gameObject, Anchor.BOTTOM, this.m_toasts[num - 1].gameObject, Anchor.TOP, (Vector3) this.MULTIPLE_TOAST_OFFSET);
      ++num;
    }
  }

  public void UpdateQuestProgressToasts() => this.ShowQuestProgressToasts(AchieveManager.Get().GetNewlyProgressedQuests());

  public void ShowQuestProgressToasts(List<Achievement> achievements)
  {
    foreach (Achievement achievement in achievements)
    {
      this.AddQuestProgressToast(achievement.ID, achievement.Name, achievement.Description, achievement.Progress, achievement.MaxProgress);
      achievement.AckCurrentProgressAndRewardNotices(true);
    }
  }

  public void AddQuestProgressToast(
    int achieveId,
    string questName,
    string questDescription,
    int progress,
    int maxProgress)
  {
    QuestProgressToast toast = Object.Instantiate<QuestProgressToast>(this.m_questProgressToastPrefab);
    toast.UpdateDisplay(questName, questDescription, progress, maxProgress);
    if (!this.AddToast((GameToast) toast))
      return;
    this.FireAllQuestProgressListeners(achieveId, progress);
  }

  public delegate void QuestProgressToastShownCallback(int achieveId);

  private class QuestProgressToastShownListener : 
    EventListener<GameToastMgr.QuestProgressToastShownCallback>
  {
    public void Fire(int achieveId) => this.m_callback(achieveId);
  }
}
