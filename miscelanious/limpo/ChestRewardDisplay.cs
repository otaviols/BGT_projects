using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestRewardDisplay : MonoBehaviour
{
  public const string DEFAULT_PREFAB = "RewardChest_Lock.prefab:06ffa33e82036694e8cacb96aa7b48e8";
  public const string MERCENARIES_PREFAB = "RewardChest_Mercenaries.prefab:7ba36254f98c8914e9b9931bbede3c88";
  public const string MERCENARIES_CONSOLATION_PREFAB = "LettuceConsolationPrize.prefab:8c837b1ecf3fe184eadfca1a3d661f6f";
  public const string MERCENARIES_AUTO_RETIRE_PREFAB = "LettuceAutorunPrize.prefab:05f50ccdbe9c5994e9dd5b2d19860822";
  public const string MERCENARY_FULLY_UPGRADED_PREFAB = "MercenariesMaxedOutReward.prefab:57fbf1dc798a43547b597a5d63e18271";
  public PegUIElement m_rewardChest;
  public PlayMakerFSM m_FSM;
  public Transform m_parent;
  public GameObject m_descText;
  public GameObject m_bannerObject;
  public UberText m_bannerUberText;
  public Transform m_rewardBoxBone;
  public Transform m_rewardBoxBonePackOpening;
  private List<RewardData> m_rewards = new List<RewardData>();
  private List<RewardData> m_bonusRewards = new List<RewardData>();
  private List<RewardData> m_rewardsAfterBoxes = new List<RewardData>();
  private List<Reward> m_rewardsAfterBoxesObjects = new List<Reward>();
  private List<Action> m_doneCallbacks = new List<Action>();
  private bool m_fromNotice;
  private long m_noticeID = -1;
  private int m_wins;
  private int m_leagueId;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public bool ShowRewards_TavernBrawl(
    int wins,
    List<RewardData> rewards,
    Transform rewardBone,
    bool fromNotice = false,
    long noticeID = -1)
  {
    if (rewards == null || rewards.Count < 1)
    {
      Debug.LogErrorFormat("rewards is null!");
      return false;
    }
    this.m_wins = wins;
    this.m_rewards = rewards;
    this.m_fromNotice = fromNotice;
    this.m_noticeID = noticeID;
    this.m_descText.SetActive(fromNotice);
    this.ShowRewardChest_TavernBrawl();
    return true;
  }

  public bool ShowRewards_LeaguePromotion(
    int leagueId,
    List<RewardData> rewards,
    Transform rewardBone,
    bool fromNotice = false,
    long noticeID = -1)
  {
    if (rewards == null || rewards.Count < 1)
    {
      Debug.LogErrorFormat("rewards is null!");
      return false;
    }
    this.m_leagueId = leagueId;
    this.m_rewards = rewards;
    this.m_fromNotice = fromNotice;
    this.m_noticeID = noticeID;
    this.ShowRewardChest_LeaguePromotion();
    return true;
  }

  public bool ShowRewards_Quest(
    List<RewardData> rewards,
    Transform rewardBone,
    string title,
    string desc,
    bool fromNotice,
    int noticeId)
  {
    if (rewards == null || rewards.Count < 1)
    {
      Debug.LogErrorFormat("rewards is null!");
      return false;
    }
    this.m_rewards = rewards;
    this.m_fromNotice = fromNotice;
    this.m_noticeID = (long) noticeId;
    this.m_bannerUberText.Text = title;
    this.m_descText.SetActive(true);
    this.m_descText.GetComponent<UberText>().Text = desc;
    this.ShowRewardChest();
    return true;
  }

  public bool ShowRewards_Mercenaries(
    List<RewardData> rewards,
    List<RewardData> bonusRewards,
    bool autoOpenChest,
    bool fromNotice,
    int noticeId)
  {
    this.m_rewards = rewards;
    this.m_bonusRewards = bonusRewards;
    return this.ShowRewards_MercenariesShared(autoOpenChest, fromNotice, noticeId);
  }

  private bool ShowRewards_MercenariesShared(bool autoOpenChest, bool fromNotice, int noticeId)
  {
    this.m_fromNotice = fromNotice;
    this.m_noticeID = (long) noticeId;
    if ((UnityEngine.Object) this.m_bannerObject != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bannerObject);
    this.m_descText?.SetActive(false);
    this.ShowRewardChest();
    if (autoOpenChest)
      this.ShowRewardBags((UIEvent) null);
    return true;
  }

  public void RegisterDoneCallback(Action action) => this.m_doneCallbacks.Add(action);

  private void Awake() => this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);

  private void ShowRewardChest()
  {
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);
    this.m_FSM.SendEvent("SummonIn");
    LayerUtils.SetLayer(this.m_rewardChest.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.m_rewardChest.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ShowRewardBags));
  }

  private void ShowRewardChest_TavernBrawl()
  {
    this.ShowRewardChest();
    string str;
    if (this.m_wins == 0)
      str = GameStrings.Get("GLUE_BRAWLISEUM_NO_WINS_REWARD_PACK_TEXT");
    else
      str = GameStrings.Format("GLUE_BRAWLISEUM_REWARDS_WIN_BANNER_TEXT", (object) this.m_wins, (object) this.m_wins);
    this.m_bannerUberText.Text = str;
  }

  private void ShowRewardChest_LeaguePromotion()
  {
    this.ShowRewardChest();
    this.m_bannerUberText.Text = GameDbf.LeagueRank.GetRecord((Predicate<LeagueRankDbfRecord>) (r => r.LeagueId == this.m_leagueId && r.StarLevel == 1)).RankName.GetString();
    this.m_descText.GetComponent<UberText>().Text = GameStrings.Get("GLUE_NEW_PLAYER_PROMOTION_CHEST_DESC");
  }

  private void OnRollover(UIEvent e) => this.m_FSM.SendEvent("Hover");

  private void OnRollout(UIEvent e) => this.m_FSM.SendEvent("Idle");

  private void ShowRewardBags(UIEvent e)
  {
    this.m_rewardChest.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ShowRewardBags));
    this.m_rewardChest.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRollover));
    this.m_rewardChest.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRollout));
    this.m_FSM.SendEvent("StartAnim");
  }

  private void OpenRewards()
  {
    if (this.m_rewards == null || this.m_rewards.Count == 0)
    {
      this.OnRewardBoxesDone();
    }
    else
    {
      PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
      {
        if (SoundManager.Get() != null)
          SoundManager.Get().LoadAndPlay((AssetReference) "card_turn_over_legendary.prefab:a8140f686bff601459e954bc23de35e0");
        RewardBoxesDisplay component = go.GetComponent<RewardBoxesDisplay>();
        component.SetRewards(this.m_rewards, this.m_bonusRewards);
        component.m_playBoxFlyoutSound = false;
        component.SetLayer(GameLayer.IgnoreFullScreenEffects);
        component.UseDarkeningClickCatcher(true);
        component.RegisterDoneCallback(new Action(this.OnRewardBoxesDone));
        if (!(bool) UniversalInputManager.UsePhoneUI)
          LayerUtils.SetLayer(this.m_rewardChest.gameObject, GameLayer.Default);
        Transform rewardBoxBoneForScene = this.GetRewardBoxBoneForScene();
        component.transform.position = rewardBoxBoneForScene.position;
        component.transform.localRotation = rewardBoxBoneForScene.localRotation;
        component.transform.localScale = rewardBoxBoneForScene.localScale;
        component.AnimateRewards();
      });
      AssetLoader.Get().InstantiatePrefab((AssetReference) RewardBoxesDisplay.GetPrefab(this.m_rewards), callback);
    }
  }

  private void OnRewardBoxesDone()
  {
    if (this.m_rewardsAfterBoxes.Count == 0)
      this.OnAllChestRewardsDone();
    else
      this.DisplayRewardsAfterRewardBoxes();
  }

  private void DisplayRewardsAfterRewardBoxes() => RewardUtils.LoadAndDisplayRewards(this.m_rewardsAfterBoxes, new Action(this.OnAllChestRewardsDone));

  private void OnAllChestRewardsDone()
  {
    this.m_screenEffectsHandle.StopEffect(RewardUtils.MercRewardEndBlurTime);
    this.m_FSM.SendEvent("SummonOut");
    this.m_descText.SetActive(false);
    if (!this.m_fromNotice)
      return;
    Network.Get().AckNotice(this.m_noticeID);
  }

  public void OnSummonInAnimationDone()
  {
    this.m_rewardChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRollover));
    this.m_rewardChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRollout));
  }

  public void OnSummonOutAnimationDone()
  {
    foreach (Action doneCallback in this.m_doneCallbacks)
    {
      if (doneCallback != null)
        doneCallback();
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_parent.gameObject);
  }

  private Transform GetRewardBoxBoneForScene() => SceneMgr.Get().GetMode() == SceneMgr.Mode.PACKOPENING ? this.m_rewardBoxBonePackOpening : this.m_rewardBoxBone;
}
