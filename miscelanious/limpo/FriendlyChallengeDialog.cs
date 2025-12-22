using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusShared;
using SpectatorProto;
using System;
using System.Collections;
using UnityEngine;

public class FriendlyChallengeDialog : DialogBase
{
  public UberText m_challengeText;
  public UberText m_challengerName;
  public UIBButton m_acceptButton;
  public UIBButton m_denyButton;
  public UberText m_nearbyPlayerNote;
  public float m_friendQuestSliderSoundDelay;
  public string m_friendQuestSliderSound;
  public float m_friendQuestSliderSoundDelay2;
  public string m_friendQuestSliderSound2;
  public GameObject m_friendQuestContainer;
  public GameObject m_dropShadow;
  private FriendlyChallengeDialog.ResponseCallback m_responseCallback;
  private Achievement m_quest;
  private FriendlyChallengeQuestFrame m_friendlyQuestFrame;
  private PartyQuestInfo m_partyQuestInfo;
  private Widget m_questTileWidget;
  private const float NAME_LINE_PADDING = 0.01f;

  private void Start()
  {
    this.m_acceptButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ConfirmButtonPress));
    this.m_denyButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CancelButtonPress));
  }

  public override void Show()
  {
    base.Show();
    BnetBar.Get().DisableButtonsByDialog((DialogBase) this);
    if ((bool) UniversalInputManager.UsePhoneUI && this.m_nearbyPlayerNote.gameObject.activeSelf)
      this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + 50f, this.transform.localPosition.z);
    this.DoShowAnimation();
    UniversalInputManager.Get().SetSystemDialogActive(true);
    SoundManager.Get().LoadAndPlay((AssetReference) "friendly_challenge.prefab:649e070117bcd0d45bac691a03bf2dec");
    if (this.m_partyQuestInfo == null)
      return;
    Processor.ScheduleCallback(this.m_friendQuestSliderSoundDelay, false, (Processor.ScheduledCallback) (u => SoundManager.Get().LoadAndPlay((AssetReference) this.m_friendQuestSliderSound)));
    Processor.ScheduleCallback(this.m_friendQuestSliderSoundDelay2, false, (Processor.ScheduledCallback) (u => SoundManager.Get().LoadAndPlay((AssetReference) this.m_friendQuestSliderSound2)));
  }

  public override void Hide()
  {
    base.Hide();
    SoundManager.Get().LoadAndPlay((AssetReference) "banner_shrink.prefab:d9de7386a7f2017429d126e972232123");
    iTween.FadeTo(this.m_dropShadow, iTween.Hash((object) "amount", (object) 0.0f, (object) "time", (object) 1f));
  }

  public override bool HandleKeyboardInput()
  {
    if (!InputCollection.GetKeyUp(KeyCode.Escape))
      return false;
    this.CancelButtonPress((UIEvent) null);
    return true;
  }

  public void SetInfo(FriendlyChallengeDialog.Info info)
  {
    string key = "GLOBAL_FRIEND_CHALLENGE_BODY1";
    if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
      key = FriendChallengeMgr.Get().GetChallengeBrawlType() != BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING ? "GLOBAL_FRIEND_CHALLENGE_TAVERN_BRAWL_BODY1" : "GLOBAL_FRIEND_CHALLENGE_FIRESIDE_BRAWL_BODY1";
    else if (FriendChallengeMgr.Get().IsChallengeBacon() || info.m_partyType == PartyType.BATTLEGROUNDS_PARTY)
      key = "GLOBAL_FRIEND_CHALLENGE_BODY_BACON";
    else if (CollectionManager.Get().ShouldAccountSeeStandardWild())
    {
      if (info.m_formatType == FormatType.FT_STANDARD)
        key = "GLOBAL_FRIEND_CHALLENGE_BODY1_STANDARD";
      else if (info.m_formatType == FormatType.FT_WILD)
        key = "GLOBAL_FRIEND_CHALLENGE_BODY1_WILD";
      else if (info.m_formatType == FormatType.FT_CLASSIC)
        key = "GLOBAL_FRIEND_CHALLENGE_BODY1_CLASSIC";
    }
    this.m_challengeText.Text = GameStrings.Get(key);
    this.m_challengerName.Text = FriendUtils.GetUniqueName(info.m_challenger);
    this.m_responseCallback = info.m_callback;
    this.m_nearbyPlayerNote.gameObject.SetActive(BnetNearbyPlayerMgr.Get().IsNearbyStranger(info.m_challenger));
    if (info.m_partyType == PartyType.BATTLEGROUNDS_PARTY || info.m_questInfo == null)
      return;
    this.SetQuestInfo(info.m_questInfo);
  }

  public Achievement GetQuest() => this.m_quest;

  public void SetQuestInfo(PartyQuestInfo info)
  {
    if ((UnityEngine.Object) this.m_friendQuestContainer == (UnityEngine.Object) null)
      return;
    this.m_partyQuestInfo = info;
    if (info == null || info.QuestIds.Count == 0)
    {
      this.m_friendQuestContainer.gameObject.SetActive(false);
    }
    else
    {
      bool flag = false;
      foreach (int questId in info.QuestIds)
      {
        AchieveDbfRecord record = GameDbf.Achieve.GetRecord(questId);
        if (record != null && record.SharedAchieveId != 0)
        {
          Achievement achievement = AchieveManager.Get().GetAchievement(record.SharedAchieveId);
          if (achievement != null)
          {
            AchieveRegionDataDbfRecord currentRegionData = achievement.GetCurrentRegionData();
            if (currentRegionData != null && currentRegionData.RewardableLimit > 0 && achievement.IntervalRewardStartDate > 0L && (DateTime.UtcNow - DateTime.FromFileTimeUtc(achievement.IntervalRewardStartDate)).TotalDays < currentRegionData.RewardableInterval && achievement.IntervalRewardCount >= currentRegionData.RewardableLimit)
              flag = true;
          }
        }
      }
      this.m_friendlyQuestFrame = (UnityEngine.Object) this.m_friendQuestContainer != (UnityEngine.Object) null ? this.m_friendQuestContainer.GetComponentInChildren<FriendlyChallengeQuestFrame>() : (FriendlyChallengeQuestFrame) null;
      if ((UnityEngine.Object) this.m_friendlyQuestFrame != (UnityEngine.Object) null && (UnityEngine.Object) this.m_friendlyQuestFrame.m_questTileBone != (UnityEngine.Object) null)
      {
        if (flag)
        {
          this.m_friendlyQuestFrame.m_noGoldRewardText.Text = GameStrings.Get("GLOBAL_FRIENDLYCHALLENGE_QUEST_REWARD_AT_LIMIT");
          this.m_friendlyQuestFrame.m_questName.Hide();
          this.m_friendlyQuestFrame.m_questDesc.Hide();
          this.m_friendlyQuestFrame.m_nameLine.gameObject.SetActive(false);
          this.m_friendlyQuestFrame.m_rewardMesh.gameObject.SetActive(false);
          this.m_friendlyQuestFrame.m_rewardAmountLabel.Hide();
          this.m_friendQuestContainer.gameObject.SetActive(true);
          SlidingTray component = this.m_friendQuestContainer.GetComponent<SlidingTray>();
          if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
            return;
          component.ShowTray();
        }
        else
        {
          this.m_questTileWidget = (Widget) WidgetInstance.Create(Hearthstone.Progression.QuestTile.QUEST_TILE_WIDGET_ASSET);
          GameUtils.SetParent((Component) this.m_questTileWidget, this.m_friendlyQuestFrame.m_questTileBone);
          this.m_questTileWidget.SetLayerOverride((GameLayer) this.gameObject.layer);
          this.StartCoroutine(this.ShowWhenReady(info));
        }
      }
      else
        Debug.LogError((object) "FriendlyChallegeDialog.Start - QuestTileWidget is not set!");
    }
  }

  private void SetQuestInfo_OnLoadRewardObject(Reward reward, object callbackData)
  {
    if ((UnityEngine.Object) this.m_friendlyQuestFrame.m_rewardBone == (UnityEngine.Object) null)
      return;
    reward.transform.SetParent(this.m_friendlyQuestFrame.m_rewardBone.transform);
    reward.transform.localPosition = Vector3.zero;
    bool doubleGold = this.m_quest != null && this.m_quest.IsAffectedByDoubleGold && SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_GOLD_DOUBLED, false);
    float amountToScaleReward;
    RewardUtils.SetupRewardIcon(this.m_quest.Rewards[0], (Renderer) this.m_friendlyQuestFrame.m_rewardMesh, this.m_friendlyQuestFrame.m_rewardAmountLabel, out amountToScaleReward, doubleGold);
    this.m_friendlyQuestFrame.m_rewardMesh.transform.localScale *= amountToScaleReward;
    this.m_friendlyQuestFrame.m_rewardAmountLabel.RenderQueue = RendererExtension.GetMaterial((Renderer) this.m_friendlyQuestFrame.m_rewardMesh).renderQueue;
  }

  private void ConfirmButtonPress(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    if (this.m_responseCallback != null)
      this.m_responseCallback(true);
    this.Hide();
  }

  private void CancelButtonPress(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    if (this.m_responseCallback != null)
      this.m_responseCallback(false);
    this.Hide();
  }

  private IEnumerator ShowWhenReady(PartyQuestInfo info)
  {
    int questId = 114;
    QuestDataModel questDataModelById = QuestManager.Get().CreateQuestDataModelById(questId);
    questDataModelById.RerollCount = 0;
    if ((UnityEngine.Object) this.m_questTileWidget != (UnityEngine.Object) null)
    {
      this.m_questTileWidget.BindDataModel((IDataModel) questDataModelById);
      this.m_questTileWidget.TriggerEvent("DISABLE_INTERACTION");
    }
    this.m_friendQuestContainer.gameObject.SetActive(true);
    while ((UnityEngine.Object) this.m_questTileWidget != (UnityEngine.Object) null && (!this.m_questTileWidget.IsReady || this.m_questTileWidget.IsChangingStates))
      yield return (object) null;
    SlidingTray component = this.m_friendQuestContainer.GetComponent<SlidingTray>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.ShowTray();
    iTween.FadeTo(this.m_dropShadow, iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) 1f));
    if ((UnityEngine.Object) this.m_friendlyQuestFrame != (UnityEngine.Object) null)
    {
      this.m_friendlyQuestFrame.m_nameLine.SetActive(false);
      this.m_friendlyQuestFrame.m_questDesc.Hide();
      this.m_friendlyQuestFrame.m_rewardAmountLabel.Hide();
      this.m_friendlyQuestFrame.m_rewardMesh.gameObject.SetActive(false);
    }
  }

  public delegate void ResponseCallback(bool accept);

  public class Info
  {
    public FormatType m_formatType;
    public BnetPlayer m_challenger;
    public PartyType m_partyType;
    public PartyQuestInfo m_questInfo;
    public FriendlyChallengeDialog.ResponseCallback m_callback;
  }
}
