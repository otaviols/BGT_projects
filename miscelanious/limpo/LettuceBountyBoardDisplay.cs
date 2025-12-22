using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LettuceBountyBoardDisplay : AbsSceneDisplay
{
  public int m_bountiesPerPage = 6;
  public WidgetTemplate m_widgetTemplate;
  public PlayMakerFSM[] m_fsms;
  public AsyncReference m_PlayButtonReference;
  public AsyncReference m_PlayButtonPhoneReference;
  public AsyncReference m_BackButtonReference;
  public AsyncReference m_BackButtonPhoneReference;
  public AsyncReference[] m_BountyBoardDisplays;
  private PlayButton m_playButton;
  private UIBButton m_backButton;
  private UIBButton m_editTeamButton;
  private VisualController m_bountyBoardVisualController;
  private bool m_playButtonFinishedLoading;
  private bool m_backButtonFinishedLoading;
  private int m_bountyBoardDisplayFinishedLoadingCount;
  private List<LettuceBountyDataModel> m_bountyDataModels = new List<LettuceBountyDataModel>();
  private LettuceBountyBoardDataModel m_bountyBoardDataModel;
  private static int m_lastSelectedBountyRecordIdThisSession;
  private List<DefLoader.DisposableCardDef> m_loadedCardDefs = new List<DefLoader.DisposableCardDef>();
  private List<DefLoader.DisposableFullDef> m_loadedBountyRewardFullDefs = new List<DefLoader.DisposableFullDef>();
  private List<int> m_BountyIdsToAcknowledge = new List<int>();

  public override void Start()
  {
    base.Start();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_PlayButtonPhoneReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
      this.m_BackButtonPhoneReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
    }
    else
    {
      this.m_PlayButtonReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
      this.m_BackButtonReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
    }
    foreach (AsyncReference bountyBoardDisplay in this.m_BountyBoardDisplays)
      bountyBoardDisplay.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnBountyBoardDisplayReady));
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MercenariesSubMenus);
    this.StartCoroutine(this.InitializeWhenReady());
  }

  private void OnDestroy()
  {
    foreach (DefLoader.DisposableCardDef loadedCardDef in this.m_loadedCardDefs)
      loadedCardDef.Dispose();
    foreach (DefLoader.DisposableFullDef bountyRewardFullDef in this.m_loadedBountyRewardFullDefs)
      bountyRewardFullDef.Dispose();
  }

  private void BountyBoardEventListener(string eventName)
  {
    if (!(eventName == "BOUNTY_RELEASED"))
    {
      if (!(eventName == "PAGE_NEXT"))
      {
        if (!(eventName == "PAGE_PREV"))
        {
          if (!(eventName == "BOUNTY_HOVERED_CODE"))
            return;
          this.OnBountyHovered();
        }
        else
        {
          if (this.m_bountyBoardDataModel.PageIndex <= 0)
            return;
          --this.m_bountyBoardDataModel.PageIndex;
          this.m_fsms[this.m_bountyBoardDataModel.PageIndex].FsmVariables.FindFsmBool("PlayAudioOnSlide").Value = this.m_slidingTray.m_playAudioOnSlide;
          this.m_fsms[this.m_bountyBoardDataModel.PageIndex].gameObject.transform.localPosition = this.m_slidingTray.m_trayHiddenBone.localPosition;
          this.m_fsms[this.m_bountyBoardDataModel.PageIndex].SendEvent("Birth");
          this.AcknowledgeBountiesOnCurrentPage();
        }
      }
      else
      {
        if (this.m_bountyBoardDataModel.PageIndex >= this.m_bountyBoardDataModel.PageCount - 1)
          return;
        int index = this.m_bountyBoardDataModel.PageIndex++;
        this.m_fsms[index].FsmVariables.FindFsmBool("PlayAudioOnSlide").Value = this.m_slidingTray.m_playAudioOnSlide;
        this.m_fsms[index].SendEvent("Death");
        this.AcknowledgeBountiesOnCurrentPage();
      }
    }
    else
      this.OnBountySelected();
  }

  public void OnPlayButtonReady(PlayButton playButton)
  {
    this.m_playButtonFinishedLoading = true;
    if ((UnityEngine.Object) playButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButton could not be found! You will not be able to click 'Play'!");
    }
    else
    {
      this.m_playButton = playButton;
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonRelease));
      this.m_playButton.Disable();
    }
  }

  public void OnBackButtonReady(UIBButton backButton)
  {
    this.m_backButtonFinishedLoading = true;
    if ((UnityEngine.Object) backButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "BackButton could not be found! You will not be able to click 'Back'!");
    }
    else
    {
      this.m_backButton = backButton;
      this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonRelease));
    }
  }

  public void OnBountyBoardDisplayReady(VisualController visualController) => ++this.m_bountyBoardDisplayFinishedLoadingCount;

  public EventDataModel GetEventDataModel() => this.m_widgetTemplate.GetDataModel<EventDataModel>();

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if (!this.m_playButtonFinishedLoading)
    {
      failureMessage = "LettuceBountyBoardDisplay - Play button never loaded.";
      return false;
    }
    if (!this.m_backButtonFinishedLoading)
    {
      failureMessage = "LettuceBountyBoardDisplay - Back button never loaded.";
      return false;
    }
    if (this.m_bountyBoardDisplayFinishedLoadingCount != this.m_BountyBoardDisplays.Length)
    {
      failureMessage = string.Format("LettuceBountyBoardDisplay - Display loading count {0} never reached expected count {1}.", (object) this.m_bountyBoardDisplayFinishedLoadingCount, (object) this.m_BountyBoardDisplays.Length);
      return false;
    }
    failureMessage = string.Empty;
    return true;
  }

  private void OnPlayButtonRelease(UIEvent e)
  {
    this.SendAcknowledgeRequestOnExit();
    this.SetNextModeAndHandleTransition(SceneMgr.Mode.LETTUCE_BOUNTY_TEAM_SELECT, this.m_sceneTransitionPayload);
    this.m_playButton.Disable();
  }

  private void OnBackButtonRelease(UIEvent e)
  {
    this.SendAcknowledgeRequestOnExit();
    this.SetNextModeAndHandleTransition(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.CURRENT_SCENE, (object) null);
  }

  protected override bool ShouldStartShown() => SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_BOUNTY_TEAM_SELECT && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_MAP && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_VILLAGE;

  private IEnumerator InitializeWhenReady()
  {
    LettuceBountyBoardDisplay bountyBoardDisplay = this;
    while (!bountyBoardDisplay.IsFinishedLoading(out string _))
      yield return (object) null;
    bountyBoardDisplay.InitializeBountyBoardDataModel();
    bountyBoardDisplay.m_widgetTemplate.RegisterEventListener(new Widget.EventListenerDelegate(bountyBoardDisplay.BountyBoardEventListener));
  }

  private void InitializeBountyBoardDataModel()
  {
    this.m_bountyBoardDataModel = new LettuceBountyBoardDataModel();
    this.m_widgetTemplate.BindDataModel((IDataModel) this.m_bountyBoardDataModel, false);
    if (this.m_sceneTransitionPayload == null)
    {
      Debug.LogError((object) "LettuceBountyBoardDisplay: No scene transition payload was received.");
    }
    else
    {
      NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
      LettuceVillageDisplay.LettuceSceneTransitionPayload payload = (LettuceVillageDisplay.LettuceSceneTransitionPayload) this.m_sceneTransitionPayload;
      LettuceBountySetDbfRecord bountySetRecord = payload.m_SelectedBountySet;
      List<LettuceBountyDbfRecord> list = GameDbf.LettuceBounty.GetRecords((Predicate<LettuceBountyDbfRecord>) (r => r.BountySetId == bountySetRecord.ID && r.Enabled && r.DifficultyMode == payload.m_DifficultyMode)).OrderBy<LettuceBountyDbfRecord, int>((Func<LettuceBountyDbfRecord, int>) (r => r.SortOrder)).ToList<LettuceBountyDbfRecord>();
      this.m_bountyBoardDataModel.Bounties = new DataModelList<LettuceBountyDataModel>();
      this.m_bountyBoardDataModel.PageCount = 1 + (list.Count - 1) / this.m_bountiesPerPage;
      this.m_bountyBoardDataModel.PageIndex = 0;
      this.m_bountyBoardDataModel.AutoSelectedBountyRecordId = list[0].ID;
      int num = 0;
      foreach (LettuceBountyDbfRecord bountyRecord in list)
      {
        Material bossCoinMaterial = (Material) null;
        DefLoader.Get().LoadCardDef(GameUtils.TranslateDbIdToCardId(bountyRecord.FinalBossCardId), (DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>) ((cardId, def, userData) =>
        {
          if (def == null)
            return;
          bossCoinMaterial = def.CardDef.m_MercenaryMapBossCoinPortrait;
          this.m_loadedCardDefs.Add(def);
        }));
        if ((UnityEngine.Object) bossCoinMaterial == (UnityEngine.Object) null)
          bossCoinMaterial = AssetLoader.Get().LoadMaterial((AssetReference) "LOE_08CoinPortrait.mat:b5cdfac2e9672f9479083d73014858c6");
        MercenariesDataUtil.MercenariesBountyLockedReason bountyUnlockStatus = MercenariesDataUtil.GetBountyUnlockStatus(bountyRecord);
        bool flag1 = bountyUnlockStatus != 0;
        bool flag2 = bountyUnlockStatus != MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_NOT_ACTIVE && bountyUnlockStatus != MercenariesDataUtil.MercenariesBountyLockedReason.EVENT_NOT_COMPLETE;
        bool flag3 = bountyUnlockStatus == MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED && !netObject.BountyInfoMap.ContainsKey(bountyRecord.ID) || netObject.BountyInfoMap.ContainsKey(bountyRecord.ID) && !netObject.BountyInfoMap[bountyRecord.ID].IsAcknowledged;
        bool flag4 = MercenariesDataUtil.IsBountyComplete(bountyRecord.ID);
        if (!flag4 & flag2 && num == 0)
          num = bountyRecord.ID;
        this.m_bountyBoardDataModel.Bounties.Add(new LettuceBountyDataModel()
        {
          BountyId = bountyRecord.ID,
          AdventureMission = new AdventureMissionDataModel()
          {
            CoinPortraitMaterial = bossCoinMaterial,
            MissionState = AdventureMissionState.UNLOCKED
          },
          Complete = flag4,
          IsLocked = flag1,
          IsEventLocked = !flag2,
          Available = true,
          ComingSoonText = (string) bountyRecord.ComingSoonText,
          PosterText = GameStrings.Format("GLUE_LETTUCE_BOUNTY_POSTER_TEXT", (object) bountyRecord.BountyLevel),
          IsNew = flag3
        });
      }
      this.m_bountyBoardDataModel.AutoSelectedBountyRecordId = num;
      if (LettuceBountyBoardDisplay.m_lastSelectedBountyRecordIdThisSession != 0)
      {
        this.m_bountyBoardDataModel.AutoSelectedBountyRecordId = LettuceBountyBoardDisplay.m_lastSelectedBountyRecordIdThisSession;
        this.m_bountyBoardDataModel.CurrentSelectedBountyRecordId = LettuceBountyBoardDisplay.m_lastSelectedBountyRecordIdThisSession;
      }
      this.m_bountyBoardDataModel.DifficultyMode = payload.m_DifficultyMode;
      this.m_bountyBoardDataModel.BountySetShortGuid = bountySetRecord.ShortGuid;
      this.m_bountyBoardDataModel.HeaderText = GameStrings.Format("GLUE_LETTUCE_BOUNTY_BOARD_HEADER", (object) bountySetRecord.Name.GetString());
      if (!string.IsNullOrEmpty(bountySetRecord.WatermarkTexture))
        AssetLoader.Get().LoadTexture((AssetReference) bountySetRecord.WatermarkTexture, (ObjectCallback) ((assetRef, obj, callbackData) => this.m_bountyBoardDataModel.BountySetWatermark = obj as Texture));
      this.AcknowledgeBountiesOnCurrentPage();
    }
  }

  private void OnBountyHovered()
  {
    EventDataModel eventDataModel = this.GetEventDataModel();
    if (eventDataModel == null)
      Log.All.PrintError("No bounty attached to the event.");
    else
      this.AcknowledgeBountyAsSeen((int) eventDataModel.Payload);
  }

  private void OnBountySelected()
  {
    EventDataModel eventDataModel = this.GetEventDataModel();
    if (eventDataModel == null)
    {
      Log.All.PrintError("No event data model attached to the LettuceBountyBoardDisplay.");
    }
    else
    {
      LettuceBountyDataModel payload = (LettuceBountyDataModel) eventDataModel.Payload;
      LettuceVillageDisplay.LettuceSceneTransitionPayload transitionPayload = (LettuceVillageDisplay.LettuceSceneTransitionPayload) this.m_sceneTransitionPayload;
      LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord(payload.BountyId);
      LettuceBountyDbfRecord lettuceBountyDbfRecord = record;
      transitionPayload.m_SelectedBounty = lettuceBountyDbfRecord;
      this.AcknowledgeBountyAsSeen(payload.BountyId);
      LettuceBountyBoardDisplay.m_lastSelectedBountyRecordIdThisSession = record.ID;
      this.m_bountyBoardDataModel.CurrentSelectedBountyRecordId = record.ID;
      bool flag = !payload.IsDisabled && !payload.IsLocked && payload.Available;
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      if (flag && netObject.Games.MercenariesAI)
        this.m_playButton.Enable();
      else
        this.m_playButton.Disable();
      this.m_bountyBoardDataModel.IsSelectedBountyLocked = !flag;
      if (payload.IsDisabled)
        this.m_bountyBoardDataModel.BountyLockedText = GameStrings.Get("GLUE_LETTUCE_BOUNTY_BOARD_BOUNTY_DISABLED");
      else if (payload.IsEventLocked)
        this.m_bountyBoardDataModel.BountyLockedText = string.IsNullOrEmpty(payload.ComingSoonText) ? GameStrings.Get("GLUE_LETTUCE_BOUNTY_BOARD_COMING_SOON") : payload.ComingSoonText;
      else if (payload.IsLocked)
        this.m_bountyBoardDataModel.BountyLockedText = GameStrings.Get("GLUE_LETTUCE_BOUNTY_BOARD_BOUNTY_LOCKED");
      this.m_bountyBoardDataModel.BossName = LettuceVillageDataUtil.GetBountyBossName(record);
      int finalBossCardId = record.FinalBossCardId;
      this.m_bountyBoardDataModel.BossCard = new CardDataModel()
      {
        CardId = GameUtils.TranslateDbIdToCardId(finalBossCardId)
      };
      this.m_bountyBoardDataModel.BossDescription = GameStrings.Format("GLUE_LETTUCE_BOUNTY_BOSS_DESCRIPTION", (object) record.BountyLevel);
      foreach (DefLoader.DisposableFullDef bountyRewardFullDef in this.m_loadedBountyRewardFullDefs)
        bountyRewardFullDef.Dispose();
      this.m_loadedBountyRewardFullDefs.Clear();
      this.m_bountyBoardDataModel.SelectedBountyRewardList = new RewardListDataModel();
      foreach (LettuceBountyFinalRewardsDbfRecord finalBossReward in record.FinalBossRewards)
      {
        string idFromMercenaryId = GameUtils.GetCardIdFromMercenaryId(finalBossReward.RewardMercenaryId);
        EntityDef entityDef = DefLoader.Get().GetEntityDef(idFromMercenaryId);
        this.m_bountyBoardDataModel.SelectedBountyRewardList.Items.Add(new RewardItemDataModel()
        {
          ItemType = RewardItemType.MERCENARY_COIN,
          MercenaryCoin = new LettuceMercenaryCoinDataModel()
          {
            MercenaryId = finalBossReward.RewardMercenaryId,
            MercenaryName = entityDef.GetName(),
            Quantity = 0,
            GlowActive = false,
            NameActive = true
          }
        });
      }
    }
  }

  private void AcknowledgeBountiesOnCurrentPage()
  {
    if (this.m_bountyBoardDataModel == null)
      return;
    int num = Math.Min((this.m_bountyBoardDataModel.PageIndex + 1) * this.m_bountiesPerPage, this.m_bountyBoardDataModel.Bounties.Count);
    for (int index = this.m_bountyBoardDataModel.PageIndex * this.m_bountiesPerPage; index < num; ++index)
    {
      LettuceBountyDataModel bounty = this.m_bountyBoardDataModel.Bounties[index];
      if (!this.m_BountyIdsToAcknowledge.Contains(bounty.BountyId) && MercenariesDataUtil.GetBountyUnlockStatus(bounty.BountyId) == MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED)
        this.m_BountyIdsToAcknowledge.Add(bounty.BountyId);
    }
  }

  private void AcknowledgeBountyAsSeen(int bountyRecordID)
  {
    foreach (LettuceBountyDataModel bounty in this.m_bountyBoardDataModel.Bounties)
    {
      if (bounty.BountyId == bountyRecordID && bounty.IsNew)
      {
        if (!this.m_BountyIdsToAcknowledge.Contains(bountyRecordID) && MercenariesDataUtil.GetBountyUnlockStatus(bountyRecordID) == MercenariesDataUtil.MercenariesBountyLockedReason.UNLOCKED)
          this.m_BountyIdsToAcknowledge.Add(bountyRecordID);
        bounty.IsNew = false;
        break;
      }
    }
  }

  private void SendAcknowledgeRequestOnExit()
  {
    if (this.m_BountyIdsToAcknowledge.Count <= 0)
      return;
    Network.Get().AcknowledgeBounties(this.m_BountyIdsToAcknowledge);
    NetCache.Get().UpdateNetCachePlayerInfoAcknowledgedBounties(this.m_BountyIdsToAcknowledge);
  }
}
