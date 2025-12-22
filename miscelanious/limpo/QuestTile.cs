using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class QuestTile : MonoBehaviour
{
  public UberText m_requirement;
  public UberText m_questName;
  public GameObject m_nameLine;
  public GameObject m_progress;
  public UberText m_progressText;
  public NormalButton m_cancelButton;
  public GameObject m_cancelButtonRoot;
  public PlayMakerFSM m_fsmForAutoDestroyQuest;
  public GameObject m_legendaryFX;
  public MeshRenderer m_tileRenderer;
  public Material m_tileNormalMaterial;
  public Material m_tileLegendaryMaterial;
  public GameObject m_rewardIconZone;
  public GameObject m_questTileRewardIconPrefab;
  [CustomEditField(Sections = "Reward Icons")]
  public bool m_rewardIconShrinkToFitEnabled;
  [CustomEditField(Sections = "Reward Icons")]
  public float m_rewardIconPadding;
  [CustomEditField(Sections = "Reward Icons")]
  public float m_rewardIconPaddingPacksOnly = -0.25f;
  [CustomEditField(Sections = "Reward Icons")]
  public float m_rewardIconScaleReductionForEachAdditional;
  [CustomEditField(Sections = "Special Event FX", T = EditType.GAME_OBJECT)]
  public string m_fxPrefabDefault;
  [CustomEditField(Sections = "Special Event FX")]
  public List<QuestTile.SpecialEventFxEntry> m_specialEventFx = new List<QuestTile.SpecialEventFxEntry>();
  private Achievement m_quest;
  private bool m_canShowCancelButton;
  private List<QuestTileRewardIcon> m_rewardIcons = new List<QuestTileRewardIcon>();
  private List<RewardData> m_rewards = new List<RewardData>();
  private PlayMakerFSM m_fsm;
  private bool m_fsmHasBeenSentTerminalEvent;
  private bool m_fsmHasDeathFxFinishedPlaying;
  private bool m_fsmHasPendingQuestRerolledEvent;
  private const float NAME_LINE_PADDING = 0.22f;

  private void Awake()
  {
    this.SetCanShowCancelButton(false);
    this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonReleased));
  }

  public Achievement GetQuest() => this.m_quest;

  public void SetupTile(Achievement quest, QuestTile.FsmEvent fsmEventToPlay = QuestTile.FsmEvent.None)
  {
    quest.AckCurrentProgressAndRewardNotices(true);
    this.m_quest = quest;
    this.m_rewards = this.m_quest.Rewards;
    if (this.m_quest.MaxProgress > 1)
    {
      this.m_progressText.Text = this.m_quest.Progress.ToString() + "/" + (object) this.m_quest.MaxProgress;
      this.m_progress.SetActive(true);
    }
    else
    {
      this.m_progressText.Text = "";
      this.m_progress.SetActive(false);
    }
    if (quest.IsLegendary)
    {
      RendererExtension.SetMaterial((Renderer) this.m_tileRenderer, this.m_tileLegendaryMaterial);
      this.m_legendaryFX.SetActive(true);
    }
    else
    {
      RendererExtension.SetMaterial((Renderer) this.m_tileRenderer, this.m_tileNormalMaterial);
      this.m_legendaryFX.SetActive(false);
    }
    this.m_questName.Text = quest.Name;
    RewardUtils.SetQuestTileNameLinePosition(this.m_nameLine, this.m_questName, 0.22f);
    this.m_requirement.Text = quest.Description;
    this.SetupRewardIcons();
    this.SetVisible(false);
    this.LoadFsmAndPlayFX(fsmEventToPlay);
  }

  public void OnDeathFinishedPlaying()
  {
    this.m_fsmHasDeathFxFinishedPlaying = true;
    if (!this.m_fsmHasPendingQuestRerolledEvent)
      return;
    this.m_fsmHasPendingQuestRerolledEvent = false;
    this.SendFsmEvent(QuestTile.FsmEvent.QuestRerolled);
  }

  public void SetCanShowCancelButton(bool canShowCancel)
  {
    this.m_canShowCancelButton = canShowCancel;
    this.UpdateCancelButtonVisibility();
  }

  public void UpdateCancelButtonVisibility()
  {
    bool flag = false;
    if (this.m_canShowCancelButton && this.m_quest != null)
      flag = AchieveManager.Get().CanCancelQuest(this.m_quest.ID);
    this.m_cancelButtonRoot.gameObject.SetActive(flag);
  }

  public int GetQuestID() => this.m_quest == null ? 0 : this.m_quest.ID;

  public void OnClose()
  {
    foreach (QuestTileRewardIcon rewardIcon in this.m_rewardIcons)
      rewardIcon.OnClose();
    this.SendFsmEvent(QuestTile.FsmEvent.QuestHidden);
  }

  public void CompleteAndAutoDestroyQuest()
  {
    if (this.m_quest == null || !this.m_quest.AutoDestroy || (UnityEngine.Object) this.m_fsmForAutoDestroyQuest == (UnityEngine.Object) null)
      return;
    this.m_fsmForAutoDestroyQuest.SendEvent("Death");
    AchieveManager.Get().CompleteAutoDestroyAchieve(this.m_quest.ID);
  }

  private void ReplaceAutoDestroyQuest()
  {
    if (this.m_quest == null || !this.m_quest.AutoDestroy || (UnityEngine.Object) this.m_fsmForAutoDestroyQuest == (UnityEngine.Object) null)
      return;
    int linkToId = this.m_quest.LinkToId;
    if (linkToId == 0)
      return;
    this.OnClose();
    this.SetupTile(AchieveManager.Get().GetAchievement(linkToId));
    this.m_fsmForAutoDestroyQuest.SendEvent("Birth");
  }

  private void SetVisible(bool visible)
  {
    Renderer[] componentsInChildren1 = this.GetComponentsInChildren<Renderer>();
    if (componentsInChildren1 != null)
    {
      foreach (Renderer renderer in componentsInChildren1)
        renderer.enabled = visible;
    }
    UberText[] componentsInChildren2 = this.GetComponentsInChildren<UberText>();
    if (componentsInChildren2 == null)
      return;
    foreach (UberText uberText in componentsInChildren2)
    {
      if (visible)
        uberText.Show();
      else
        uberText.Hide();
    }
  }

  private void OnCancelButtonReleased(UIEvent e)
  {
    if (!Network.IsLoggedIn())
      DialogManager.Get().ShowReconnectHelperDialog((Action) (() => this.OnCancelButtonReleased(e)));
    else if (this.m_quest.IsLegendary)
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LEGENDARY_QUEST_REROLL_TITLE"),
        m_text = GameStrings.Get("GLUE_LEGENDARY_QUEST_REROLL_BODY"),
        m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
        m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = new AlertPopup.ResponseCallback(this.OnQuestRerolled)
      });
    else
      this.OnQuestRerolled(AlertPopup.Response.CONFIRM, (object) null);
  }

  private void OnQuestRerolled(AlertPopup.Response response, object userData)
  {
    if (response != AlertPopup.Response.CONFIRM || this.m_quest == null)
      return;
    AchieveManager.Get().CancelQuest(this.m_quest.ID);
    foreach (QuestTileRewardIcon rewardIcon in this.m_rewardIcons)
      rewardIcon.OnQuestRerolled();
    this.SendFsmEvent(QuestTile.FsmEvent.Death);
  }

  private void SendFsmEvent(QuestTile.FsmEvent fsmEvent)
  {
    if (fsmEvent == QuestTile.FsmEvent.None || !((UnityEngine.Object) this.m_fsm != (UnityEngine.Object) null))
      return;
    this.m_fsm.SendEvent(fsmEvent.ToString());
    if (fsmEvent != QuestTile.FsmEvent.QuestHidden && fsmEvent != QuestTile.FsmEvent.Death)
      return;
    this.m_fsmHasBeenSentTerminalEvent = true;
    if (fsmEvent != QuestTile.FsmEvent.Death)
      return;
    this.m_fsmHasDeathFxFinishedPlaying = false;
  }

  [ContextMenu("Reset Quest Seen")]
  private void ResetQuestSeen() => AchieveManager.Get().ResetQuestSeenByPlayerThisSession(this.m_quest);

  private void LoadFsmAndPlayFX(QuestTile.FsmEvent fsmEventToPlay)
  {
    string assetRef = this.m_fxPrefabDefault;
    AchieveRegionDataDbfRecord currentRegionData = this.m_quest.GetCurrentRegionData();
    if (currentRegionData != null && currentRegionData.ActivateEvent != SpecialEventType.UNKNOWN)
    {
      foreach (QuestTile.SpecialEventFxEntry specialEventFxEntry in this.m_specialEventFx)
      {
        if (!string.IsNullOrEmpty(specialEventFxEntry.m_fxPrefab) && specialEventFxEntry.m_questActivatedBySpecialEventType != SpecialEventType.UNKNOWN && specialEventFxEntry.m_questActivatedBySpecialEventType == currentRegionData.ActivateEvent)
        {
          assetRef = specialEventFxEntry.m_fxPrefab;
          break;
        }
      }
    }
    AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, new PrefabCallback<GameObject>(this.OnFxPrefabLoaded), (object) fsmEventToPlay, AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void OnFxPrefabLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) go);
    }
    else
    {
      GameUtils.SetParent(go, this.gameObject);
      LayerUtils.SetLayer(go, this.gameObject.layer);
      if ((UnityEngine.Object) this.m_fsm != (UnityEngine.Object) null && !this.m_fsmHasBeenSentTerminalEvent)
      {
        Debug.LogWarning((object) "QuestTile FSM OnFxPrefabLoaded, but existing FSM has not been sent death event!");
      }
      else
      {
        this.m_fsmHasBeenSentTerminalEvent = false;
        this.m_fsm = go.GetComponent<PlayMakerFSM>();
        if ((UnityEngine.Object) this.m_fsm == (UnityEngine.Object) null)
          return;
        this.SendFsmEvent(QuestTile.FsmEvent.Birth);
        bool flag = AchieveManager.Get().MarkQuestAsSeenByPlayerThisSession(this.m_quest);
        this.m_fsm.FsmVariables.GetFsmBool("IsFirstTimeShown").Value = flag;
        QuestTile.FsmEvent outVal;
        if (!EnumUtils.TryCast<QuestTile.FsmEvent>(callbackData, out outVal))
          return;
        if (outVal == QuestTile.FsmEvent.QuestRerolled && !this.m_fsmHasDeathFxFinishedPlaying)
          this.m_fsmHasPendingQuestRerolledEvent = true;
        else
          this.SendFsmEvent(outVal);
      }
    }
  }

  private void SetupRewardIcons()
  {
    foreach (Component rewardIcon in this.m_rewardIcons)
      UnityEngine.Object.Destroy((UnityEngine.Object) rewardIcon.gameObject);
    this.m_rewardIcons.Clear();
    RewardChestContentsDbfRecord contentsDbfRecord = (RewardChestContentsDbfRecord) null;
    if (this.m_quest.DbfRecord.Reward == "generic_reward_chest")
    {
      int rewardData1 = (int) this.m_quest.DbfRecord.RewardData1;
      int rewardData2 = (int) this.m_quest.DbfRecord.RewardData2;
      contentsDbfRecord = RewardUtils.GetRewardChestContents(rewardData1, rewardData2);
      this.m_rewards = RewardUtils.GetRewardDataFromRewardChestAsset(rewardData1, rewardData2);
    }
    if (contentsDbfRecord != null && !string.IsNullOrEmpty(contentsDbfRecord.IconTexture))
    {
      GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.m_questTileRewardIconPrefab, this.m_rewardIconZone.transform);
      LayerUtils.SetLayer(go, this.m_rewardIconZone.gameObject.layer);
      QuestTileRewardIcon component = go.GetComponent<QuestTileRewardIcon>();
      AssetReference iconTextureAssetRef = new AssetReference(contentsDbfRecord.IconTexture);
      Vector2 iconTextureSourceOffset = new Vector2((float) contentsDbfRecord.IconOffsetX, (float) contentsDbfRecord.IconOffsetY);
      int renderQueue = 3000;
      component.InitWithIconParams(renderQueue, iconTextureAssetRef, iconTextureSourceOffset, (string) null);
      this.m_rewardIcons.Add(component);
    }
    else
    {
      this.UnravelPackStacks();
      this.CreateRewardIconsPerReward();
    }
  }

  private void UnravelPackStacks()
  {
    bool flag1 = false;
    bool flag2 = true;
    List<RewardData> rewardDataList = new List<RewardData>();
    for (int index1 = 0; index1 < this.m_rewards.Count; ++index1)
    {
      if (this.m_rewards[index1].RewardType == Reward.Type.BOOSTER_PACK)
      {
        flag1 = true;
        BoosterPackRewardData reward = this.m_rewards[index1] as BoosterPackRewardData;
        for (int index2 = 0; index2 < reward.Count; ++index2)
          rewardDataList.Add(this.m_rewards[index1]);
      }
      else
        flag2 = false;
    }
    if (!flag2)
      Log.Achievements.PrintWarning("Attempted to display a mixture of packs and other rewards without using a specific Reward Chest icon.");
    if (!(flag1 & flag2))
      return;
    this.m_rewards = rewardDataList;
    this.m_rewardIconPadding = this.m_rewardIconPaddingPacksOnly;
    this.m_rewardIconScaleReductionForEachAdditional = 0.0f;
    this.m_rewardIconShrinkToFitEnabled = false;
  }

  private void CreateRewardIconsPerReward()
  {
    bool isDoubleGoldEnabled = this.m_quest.IsAffectedByDoubleGold && SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_GOLD_DOUBLED, false);
    float num1 = this.m_rewardIconZone.GetComponent<BoxCollider>().size.x / (float) this.m_rewards.Count;
    int renderQueue = 3000 + this.m_rewards.Count - 1;
    float num2 = 0.0f;
    float num3 = GeneralUtils.IsEven(this.m_rewards.Count) ? -1f : 1f;
    for (int index = 0; index < this.m_rewards.Count; ++index)
    {
      RewardData reward = this.m_rewards[index];
      GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.m_questTileRewardIconPrefab, this.m_rewardIconZone.transform);
      LayerUtils.SetLayer(go, this.m_rewardIconZone.gameObject.layer);
      QuestTileRewardIcon component = go.GetComponent<QuestTileRewardIcon>();
      component.InitWithRewardData(reward, isDoubleGoldEnabled, renderQueue);
      this.m_rewardIcons.Add(component);
      if (GeneralUtils.IsOdd(this.m_rewards.Count) && GeneralUtils.IsOdd(index) || GeneralUtils.IsEven(this.m_rewards.Count) && GeneralUtils.IsEven(index))
        num2 = num2 + num1 / 2f + this.m_rewardIconPadding;
      --renderQueue;
      num3 *= -1f;
      float x1 = num2 * num3;
      go.transform.localPosition = new Vector3(x1, 0.0f, 0.0f);
      if (index == 0 && this.m_rewards.Count > 1 && GeneralUtils.IsOdd(this.m_rewards.Count))
        num2 += num1 / 2f;
      if (this.m_rewardIconShrinkToFitEnabled)
      {
        float x2 = go.GetComponent<MeshFilter>().mesh.bounds.size.x;
        float num4 = Math.Min(1f, num1 / x2);
        go.transform.localScale *= num4;
      }
      if ((double) this.m_rewardIconScaleReductionForEachAdditional > 0.0)
      {
        float num5 = (float) (1.0 - (double) Math.Max(0, this.m_rewards.Count - 1) * (double) this.m_rewardIconScaleReductionForEachAdditional);
        go.transform.localScale *= num5;
      }
    }
  }

  public enum FsmEvent
  {
    None,
    Birth,
    Death,
    QuestGranted,
    QuestRerolled,
    QuestShownInQuestAlert,
    QuestShownInQuestLog,
    QuestHidden,
  }

  [Serializable]
  public class SpecialEventFxEntry
  {
    public SpecialEventType m_questActivatedBySpecialEventType;
    [CustomEditField(T = EditType.GAME_OBJECT)]
    public string m_fxPrefab;
  }
}
