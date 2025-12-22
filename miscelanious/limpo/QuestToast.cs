using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestToast : MonoBehaviour
{
  public UberText m_questName;
  public GameObject m_nameLine;
  public UberText m_requirement;
  public Transform m_rewardBone;
  public PegUIElement m_clickCatcher;
  public Vector3 m_rewardScale;
  public Vector3_MobileOverride m_boosterRewardRootScale;
  public Vector3_MobileOverride m_boosterRewardPosition;
  public Vector3_MobileOverride m_boosterRewardScale;
  public Vector3_MobileOverride m_cardRewardRootScale;
  public Vector3_MobileOverride m_cardRewardScale;
  public Vector3_MobileOverride m_cardRewardLocation;
  public Vector3_MobileOverride m_signatureCardRewardScale;
  public Vector3_MobileOverride m_signatureCardRewardLocation;
  public Vector3_MobileOverride m_cardDuplicateRewardScale;
  public Vector3_MobileOverride m_cardDuplicateRewardLocation;
  public Vector3_MobileOverride m_cardBackRootScale;
  public Vector3_MobileOverride m_cardbackRewardScale;
  public Vector3_MobileOverride m_cardbackRewardLocation;
  public Vector3_MobileOverride m_goldRewardScale;
  public Vector3_MobileOverride m_goldBannerOffset;
  public Vector3_MobileOverride m_goldBannerScale;
  public Vector3_MobileOverride m_dustRewardScale;
  public Vector3_MobileOverride m_dustRewardOffset;
  public Vector3_MobileOverride m_dustBannerOffset;
  public Vector3_MobileOverride m_dustBannerScale;
  private Achievement m_quest;
  private QuestToast.DelOnCloseQuestToast m_onCloseCallback;
  private object m_onCloseCallbackData;
  private RewardData m_toastReward;
  private string m_toastName = string.Empty;
  private string m_toastDescription = string.Empty;
  private static bool m_showFullscreenEffects = true;
  private static bool m_isToastActiveOrActivating;
  private static QuestToast m_activeToast;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public void Awake()
  {
    OverlayUI.Get().AddGameObject(this.gameObject);
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  public void OnDestroy()
  {
    if (!((UnityEngine.Object) this == (UnityEngine.Object) QuestToast.m_activeToast))
      return;
    if (QuestToast.m_isToastActiveOrActivating)
    {
      this.FadeEffectsOut();
      QuestToast.m_isToastActiveOrActivating = false;
    }
    QuestToast.m_activeToast = (QuestToast) null;
  }

  public static void ShowQuestToast(
    UserAttentionBlocker blocker,
    QuestToast.DelOnCloseQuestToast onClosedCallback,
    bool updateCacheValues,
    Achievement quest)
  {
    QuestToast.ShowQuestToast(blocker, onClosedCallback, updateCacheValues, quest, true);
  }

  public static void ShowQuestToast(
    UserAttentionBlocker blocker,
    QuestToast.DelOnCloseQuestToast onClosedCallback,
    bool updateCacheValues,
    Achievement quest,
    bool fullScreenEffects)
  {
    QuestToast.ShowQuestToast(blocker, onClosedCallback, (object) null, updateCacheValues, quest, fullScreenEffects);
  }

  public static void ShowQuestToast(
    UserAttentionBlocker blocker,
    QuestToast.DelOnCloseQuestToast onClosedCallback,
    object callbackUserData,
    bool updateCacheValues,
    Achievement quest)
  {
    QuestToast.ShowQuestToast(blocker, onClosedCallback, callbackUserData, updateCacheValues, quest, true);
  }

  public static void ShowQuestToast(
    UserAttentionBlocker blocker,
    QuestToast.DelOnCloseQuestToast onClosedCallback,
    object callbackUserData,
    bool updateCacheValues,
    Achievement quest,
    bool fullscreenEffects)
  {
    if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "ShowQuestToast:" + (quest == null ? "null" : quest.ID.ToString())))
    {
      if (onClosedCallback == null)
        return;
      onClosedCallback(callbackUserData);
    }
    else
    {
      Log.Achievements.Print("ShowQuestToast: {0}", (object) quest);
      if (quest.Rewards.Any<RewardData>((Func<RewardData, bool>) (r => r.RewardType == Reward.Type.ARCANE_ORBS)) && (UnityEngine.Object) Shop.Get() != (UnityEngine.Object) null)
        StoreManager.Get().GetCurrencyCache(CurrencyType.CN_ARCANE_ORBS).MarkDirty();
      quest.AckCurrentProgressAndRewardNotices();
      if (quest.ID == 56)
      {
        if (onClosedCallback == null)
          return;
        onClosedCallback(callbackUserData);
      }
      else
        QuestToast.ShowQuestToastPopup(blocker, onClosedCallback, callbackUserData, quest.Rewards == null ? (RewardData) null : quest.Rewards.FirstOrDefault<RewardData>(), quest.Name, quest.Description, fullscreenEffects, updateCacheValues, quest);
    }
  }

  public static void ShowFixedRewardQuestToast(
    UserAttentionBlocker blocker,
    QuestToast.DelOnCloseQuestToast onClosedCallback,
    RewardData rewardData,
    string name,
    string description)
  {
    QuestToast.ShowFixedRewardQuestToast(blocker, onClosedCallback, (object) null, rewardData, name, description, true);
  }

  public static void ShowFixedRewardQuestToast(
    UserAttentionBlocker blocker,
    QuestToast.DelOnCloseQuestToast onClosedCallback,
    object callbackUserData,
    RewardData rewardData,
    string name,
    string description,
    bool fullscreenEffects)
  {
    QuestToast.ShowQuestToastPopup(blocker, onClosedCallback, callbackUserData, rewardData, name, description, fullscreenEffects, true, (Achievement) null);
  }

  public static void ShowGenericRewardQuestToast(
    UserAttentionBlocker blocker,
    QuestToast.DelOnCloseQuestToast onClosedCallback,
    RewardData rewardData,
    string name,
    string description)
  {
    QuestToast.ShowGenericRewardQuestToast(blocker, onClosedCallback, (object) null, rewardData, name, description, true);
  }

  public static void ShowGenericRewardQuestToast(
    UserAttentionBlocker blocker,
    QuestToast.DelOnCloseQuestToast onClosedCallback,
    object callbackUserData,
    RewardData rewardData,
    string name,
    string description,
    bool fullscreenEffects)
  {
    QuestToast.ShowQuestToastPopup(blocker, onClosedCallback, callbackUserData, rewardData, name, description, fullscreenEffects, false, (Achievement) null);
  }

  public static void ShowQuestToastPopup(
    UserAttentionBlocker blocker,
    QuestToast.DelOnCloseQuestToast onClosedCallback,
    object callbackUserData,
    RewardData rewardData,
    string name,
    string description,
    bool fullscreenEffects,
    bool updateCacheValues,
    Achievement quest)
  {
    int attentionCategory = (int) blocker;
    string str;
    if (rewardData != null)
      str = rewardData.Origin.ToString() + ":" + (object) rewardData.OriginData + ":" + (object) rewardData.RewardType;
    else
      str = "null";
    string callerName = "ShowQuestToastPopup:" + str;
    if (!UserAttentionManager.CanShowAttentionGrabber((UserAttentionBlocker) attentionCategory, callerName))
    {
      if (onClosedCallback == null)
        return;
      onClosedCallback(callbackUserData);
    }
    else
    {
      Log.Achievements.Print("ShowQuestToastPopup: name={0} desc={1}", (object) name, (object) description);
      QuestToast.m_showFullscreenEffects = fullscreenEffects;
      QuestToast.m_isToastActiveOrActivating = true;
      QuestToast.ToastCallbackData callbackData = new QuestToast.ToastCallbackData()
      {
        m_toastReward = rewardData,
        m_toastName = name,
        m_toastDescription = description,
        m_onCloseCallback = onClosedCallback,
        m_onCloseCallbackData = callbackUserData,
        m_quest = quest,
        m_updateCacheValues = updateCacheValues
      };
      AssetLoader.Get().InstantiatePrefab((AssetReference) "QuestToast.prefab:ebf10185d03f14f41a367b9a7170c4c4", new PrefabCallback<GameObject>(QuestToast.PositionActor), (object) callbackData);
    }
  }

  private static void PositionActor(AssetReference assetRef, GameObject go, object callbackData)
  {
    go.transform.localPosition = new Vector3(0.0f, 85f, 0.0f);
    Vector3 localScale = go.transform.localScale;
    go.transform.localScale = 0.01f * Vector3.one;
    go.SetActive(true);
    iTween.ScaleTo(go, localScale, 0.5f);
    QuestToast component = go.GetComponent<QuestToast>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "QuestToast.PositionActor(): actor has no QuestToast component");
      QuestToast.m_isToastActiveOrActivating = false;
    }
    else
    {
      QuestToast.m_activeToast = component;
      QuestToast.ToastCallbackData toastCallbackData = callbackData as QuestToast.ToastCallbackData;
      component.m_onCloseCallback = toastCallbackData.m_onCloseCallback;
      component.m_toastReward = toastCallbackData.m_toastReward;
      component.m_toastName = toastCallbackData.m_toastName;
      component.m_toastDescription = toastCallbackData.m_toastDescription;
      component.m_onCloseCallbackData = (object) toastCallbackData;
      component.m_quest = toastCallbackData.m_quest;
      component.SetUpToast(toastCallbackData.m_updateCacheValues);
    }
  }

  private void CloseQuestToast(UIEvent e) => this.CloseQuestToast();

  public void CloseQuestToast()
  {
    if ((UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
      return;
    QuestToast.m_isToastActiveOrActivating = false;
    this.m_clickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseQuestToast));
    SoundManager.Get().LoadAndPlay((AssetReference) "new_quest_click_and_shrink.prefab:601ba6676276eab43947e38f110f7b99");
    this.FadeEffectsOut();
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "time", (object) 0.5f, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "DestroyQuestToast"));
    UIContext.GetRoot().DismissPopup(this.gameObject);
    if (this.m_onCloseCallback == null)
      return;
    if (this.m_onCloseCallbackData is QuestToast.ToastCallbackData closeCallbackData && closeCallbackData.m_quest != null)
      NarrativeManager.Get().OnAchieveDismissed(closeCallbackData.m_quest);
    this.m_onCloseCallback(this.m_onCloseCallbackData);
  }

  public static bool IsQuestActive() => QuestToast.m_isToastActiveOrActivating && (UnityEngine.Object) QuestToast.m_activeToast != (UnityEngine.Object) null;

  public static QuestToast GetCurrentToast() => QuestToast.m_activeToast;

  private void DestroyQuestToast() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

  public void SetUpToast(bool updateCacheValues)
  {
    this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseQuestToast));
    this.m_questName.Text = this.m_toastName;
    this.m_requirement.Text = this.m_toastDescription;
    if (this.m_toastReward != null)
    {
      if ((!SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_GOLD_DOUBLED, false) || this.m_quest == null ? 0 : (this.m_quest.IsAffectedByDoubleGold ? 1 : 0)) != 0 && this.m_toastReward is GoldRewardData)
      {
        GoldRewardData goldRewardData = new GoldRewardData(this.m_toastReward as GoldRewardData);
        goldRewardData.Amount *= 2L;
        this.m_toastReward = (RewardData) goldRewardData;
      }
      this.m_toastReward.LoadRewardObject(new Reward.DelOnRewardLoaded(this.RewardObjectLoaded), (object) updateCacheValues);
    }
    UIContext.GetRoot().ShowPopup(this.gameObject, UIContext.BlurType.None);
    this.FadeEffectsIn();
  }

  private void RewardObjectLoaded(Reward reward, object callbackData)
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null)
      return;
    bool updateCacheValues = (bool) callbackData;
    reward.Hide();
    reward.transform.parent = this.m_rewardBone;
    reward.transform.localEulerAngles = Vector3.zero;
    reward.transform.localScale = this.m_rewardScale;
    reward.transform.localPosition = Vector3.zero;
    BoosterPackReward componentInChildren1 = reward.gameObject.GetComponentInChildren<BoosterPackReward>();
    if ((UnityEngine.Object) componentInChildren1 != (UnityEngine.Object) null)
    {
      reward.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_boosterRewardRootScale;
      reward.m_MeshRoot.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_boosterRewardPosition;
      reward.m_MeshRoot.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_boosterRewardScale;
      componentInChildren1.AllowMultiStack = false;
      componentInChildren1.m_Layer = (GameLayer) this.gameObject.layer;
    }
    CardReward componentInChildren2 = reward.gameObject.GetComponentInChildren<CardReward>();
    if ((UnityEngine.Object) componentInChildren2 != (UnityEngine.Object) null)
    {
      reward.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_cardRewardRootScale;
      if ((componentInChildren2.Data is CardRewardData data ? (data.Premium == TAG_PREMIUM.SIGNATURE ? 1 : 0) : 0) != 0)
      {
        componentInChildren2.m_cardParent.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_signatureCardRewardScale;
        componentInChildren2.m_cardParent.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_signatureCardRewardLocation;
      }
      else
      {
        componentInChildren2.m_cardParent.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_cardRewardScale;
        componentInChildren2.m_cardParent.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_cardRewardLocation;
      }
      componentInChildren2.m_duplicateCardParent.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_cardDuplicateRewardScale;
      componentInChildren2.m_duplicateCardParent.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_cardDuplicateRewardLocation;
    }
    CardBackReward componentInChildren3 = reward.gameObject.GetComponentInChildren<CardBackReward>();
    if ((UnityEngine.Object) componentInChildren3 != (UnityEngine.Object) null)
    {
      reward.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_cardBackRootScale;
      componentInChildren3.m_cardbackBone.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_cardbackRewardScale;
      componentInChildren3.m_cardbackBone.transform.localPosition = (Vector3) (MobileOverrideValue<Vector3>) this.m_cardbackRewardLocation;
    }
    GoldReward componentInChildren4 = reward.gameObject.GetComponentInChildren<GoldReward>();
    if ((UnityEngine.Object) componentInChildren4 != (UnityEngine.Object) null)
    {
      componentInChildren4.m_root.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_goldRewardScale;
      componentInChildren4.m_rewardBannerBone.transform.localPosition += (Vector3) (MobileOverrideValue<Vector3>) this.m_goldBannerOffset;
      componentInChildren4.m_rewardBannerBone.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_goldBannerScale;
    }
    ArcaneDustReward componentInChildren5 = reward.gameObject.GetComponentInChildren<ArcaneDustReward>();
    if ((UnityEngine.Object) componentInChildren5 != (UnityEngine.Object) null)
    {
      componentInChildren5.m_root.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_dustRewardScale;
      componentInChildren5.m_root.transform.localPosition += (Vector3) (MobileOverrideValue<Vector3>) this.m_dustRewardOffset;
      componentInChildren5.m_rewardBannerBone.transform.localPosition += (Vector3) (MobileOverrideValue<Vector3>) this.m_dustBannerOffset;
      componentInChildren5.m_rewardBannerBone.transform.localScale = (Vector3) (MobileOverrideValue<Vector3>) this.m_dustBannerScale;
    }
    this.gameObject.GetComponent<IPopupRoot>()?.ApplyPopupRendering(reward.transform, (HashSet<IPopupRendering>) null, true, 29);
    reward.Show(updateCacheValues);
  }

  private void FadeEffectsIn()
  {
    if (!QuestToast.m_showFullscreenEffects)
      return;
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignettePerspective with
    {
      Time = 0.4f,
      Blur = new BlurParameters(brightness: 1f)
    });
  }

  private void FadeEffectsOut()
  {
    if (!QuestToast.m_showFullscreenEffects || FullScreenFXMgr.Get() == null)
      return;
    this.m_screenEffectsHandle.StopEffect();
  }

  public delegate void DelOnCloseQuestToast(object userData);

  private class ToastCallbackData
  {
    public QuestToast.DelOnCloseQuestToast m_onCloseCallback;
    public object m_onCloseCallbackData;
    public RewardData m_toastReward;
    public string m_toastName = string.Empty;
    public string m_toastDescription = string.Empty;
    public bool m_updateCacheValues;
    public Achievement m_quest;
  }
}
