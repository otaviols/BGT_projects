using Blizzard.T5.MaterialService.Extensions;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureWing : MonoBehaviour
{
  [CustomEditField(Sections = "Wing Event Table")]
  public AdventureWingEventTable m_WingEventTable;
  [CustomEditField(Sections = "Containers & Bones")]
  public GameObject m_ContentsContainer;
  [CustomEditField(Sections = "Containers & Bones")]
  public GameObject m_CoinContainer;
  [CustomEditField(Sections = "Containers & Bones")]
  public GameObject m_WallAccentContainer;
  [CustomEditField(Sections = "Containers & Bones")]
  public GameObject m_PlateAccentContainer;
  [CustomEditField(Sections = "Lock Plate")]
  public GameObject m_LockPlate;
  [CustomEditField(Sections = "Lock Plate")]
  public GameObject m_LockPlateFXContainer;
  [CustomEditField(Sections = "UI")]
  public List<UberText> m_WingTitles = new List<UberText>();
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_UnlockButton;
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_BuyButton;
  [CustomEditField(Sections = "UI")]
  public MeshRenderer m_BuyButtonMesh;
  [CustomEditField(Sections = "UI")]
  public UberText m_BuyButtonText;
  [CustomEditField(Sections = "UI")]
  public UberText m_ReleaseLabelText;
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_RewardsPreviewButton;
  [CustomEditField(Sections = "UI")]
  public GameObject m_PurchasedBanner;
  [CustomEditField(Sections = "Wing Rewards")]
  public PegUIElement m_BigChest;
  [CustomEditField(ListTable = true, Sections = "Random Background Properties")]
  public List<AdventureWing.BackgroundRandomization> m_BackgroundRenderers = new List<AdventureWing.BackgroundRandomization>();
  [CustomEditField(Sections = "Random Background Properties")]
  public List<float> m_BackgroundOffsets = new List<float>();
  [CustomEditField(Sections = "Special UI")]
  public bool m_BuyButtonOnOppositeSideOfKey;
  [CustomEditField(Sections = "Special UI/LOE")]
  public MeshRenderer m_UnlockButtonHighlightMesh_LOE;
  [CustomEditField(Sections = "Special UI/LOE")]
  public float m_UnlockButtonHighlightIntensityOut = 1.52f;
  [CustomEditField(Sections = "Special UI/LOE")]
  public float m_UnlockButtonHighlightIntensityOver = 2f;
  [CustomEditField(Sections = "Special UI/KARA")]
  public PlayMakerFSM m_prologueLoadingPlayMakerFSM_KARA;
  [SerializeField]
  private float m_CoinSpacing = 25f;
  [SerializeField]
  private Vector3 m_CoinsOffset = Vector3.zero;
  [SerializeField]
  private Vector3 m_CoinsChestOffset = Vector3.zero;
  protected AdventureWingDef m_WingDef;
  private Spell m_UnlockSpell;
  private GameObject m_WallAccentObject;
  private GameObject m_PlateAccentObject;
  private List<AdventureWing.Boss> m_BossCoins = new List<AdventureWing.Boss>();
  private AdventureWing.BringToFocusCallback m_BringToFocusCallback;
  private bool m_Owned;
  private bool m_Playable;
  private bool m_Locked;
  private bool m_EventStartDetected;
  private bool m_HasJustAckedProgress;
  private List<AdventureWing.BossSelected> m_BossSelectedListeners = new List<AdventureWing.BossSelected>();
  private List<AdventureWing.OpenPlateStart> m_OpenPlateStartListeners = new List<AdventureWing.OpenPlateStart>();
  private List<AdventureWing.OpenPlateEnd> m_OpenPlateEndListeners = new List<AdventureWing.OpenPlateEnd>();
  private List<AdventureWing.ShowRewards> m_ShowRewardsListeners = new List<AdventureWing.ShowRewards>();
  private List<AdventureWing.HideRewards> m_HideRewardsListeners = new List<AdventureWing.HideRewards>();
  private List<AdventureWing.ShowRewardsPreview> m_ShowRewardsPreviewListeners = new List<AdventureWing.ShowRewardsPreview>();
  private List<AdventureWing.TryPurchaseWing> m_TryPurchaseWingListeners = new List<AdventureWing.TryPurchaseWing>();
  private static List<int> s_LastRandomNumbers = new List<int>();

  public float CoinSpacing
  {
    get => this.m_CoinSpacing;
    set
    {
      this.m_CoinSpacing = value;
      this.UpdateCoinPositions();
    }
  }

  public Vector3 CoinsOffset
  {
    get => this.m_CoinsOffset;
    set
    {
      this.m_CoinsOffset = value;
      this.UpdateCoinPositions();
    }
  }

  public Vector3 CoinsChestOffset
  {
    get => this.m_CoinsChestOffset;
    set
    {
      this.m_CoinsChestOffset = value;
      this.UpdateCoinPositions();
    }
  }

  public bool IsDevMode { get; set; }

  protected virtual void Awake() => this.IsDevMode = (UnityEngine.Object) AdventureScene.Get() == (UnityEngine.Object) null || AdventureScene.Get().IsDevMode;

  private void Start()
  {
    if ((UnityEngine.Object) this.m_UnlockButton != (UnityEngine.Object) null)
    {
      this.m_UnlockButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.UnlockButtonPressed));
      this.m_UnlockButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnUnlockButtonOut));
      this.m_UnlockButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnUnlockButtonOver));
    }
    if ((UnityEngine.Object) this.m_BuyButton != (UnityEngine.Object) null)
      this.m_BuyButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
      {
        if (this.IsDevMode)
          this.SetPlateKeyEvent(false);
        else
          this.FireTryPurchaseWingEvent();
      }));
    if ((UnityEngine.Object) this.m_RewardsPreviewButton != (UnityEngine.Object) null)
      this.m_RewardsPreviewButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.FireShowRewardsPreviewEvent()));
    if (!((UnityEngine.Object) this.m_BigChest != (UnityEngine.Object) null))
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_BigChest.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ShowBigChestRewards));
    }
    else
    {
      this.m_BigChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowBigChestRewards));
      this.m_BigChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideBigChestRewards));
    }
  }

  private void OnDestroy()
  {
    if (StoreManager.Get() == null)
      return;
    StoreManager.Get().RemoveStatusChangedListener(new Action<bool>(this.UpdateBuyButton));
  }

  private void Update()
  {
    if (this.m_WingEventTable.IsPlateInOrGoingToAnActiveState() && !this.IsDevMode)
    {
      if (!this.m_EventStartDetected && AdventureProgressMgr.IsWingEventActive((int) this.m_WingDef.GetWingId()))
        this.UpdatePlateState();
      else if (!this.m_Owned && AdventureProgressMgr.Get().OwnsWing((int) this.m_WingDef.GetWingId()))
        this.UpdatePlateState();
    }
    if (!this.IsDevMode)
      return;
    if (InputCollection.GetKeyDown(KeyCode.Alpha1))
    {
      this.m_LockPlate.SetActive(true);
      this.SetPlateKeyEvent(true);
    }
    if (InputCollection.GetKeyDown(KeyCode.Alpha2))
    {
      this.m_LockPlate.SetActive(true);
      this.m_WingEventTable.DoStatePlateBuy(true);
    }
    if (InputCollection.GetKeyDown(KeyCode.Alpha3))
    {
      this.m_LockPlate.SetActive(true);
      this.m_WingEventTable.DoStatePlateInitialText();
    }
    if (InputCollection.GetKeyDown(KeyCode.Alpha4))
      this.m_WingEventTable.DoStatePlateDeactivate();
    if (InputCollection.GetKeyDown(KeyCode.Alpha5))
    {
      this.m_LockPlate.SetActive(true);
      this.UnlockPlate();
    }
    if (InputCollection.GetKeyDown(KeyCode.Alpha6))
    {
      this.m_WingEventTable.DoStatePlateDeactivate();
      foreach (AdventureWing.Boss bossCoin in this.m_BossCoins)
        bossCoin.m_Chest.SlamInCheckmark();
    }
    if (InputCollection.GetKeyDown(KeyCode.Alpha7))
    {
      this.m_WingEventTable.DoStatePlateDeactivate();
      this.OpenBigChest();
    }
    if (InputCollection.GetKeyDown(KeyCode.Alpha8))
    {
      this.m_WingEventTable.DoStatePlateDeactivate();
      this.StartCoroutine(this.AnimateCoinsAndChests(this.m_BossCoins, 0.0f, (AdventureWing.DelOnCoinAnimateCallback) null));
    }
    if (!InputCollection.GetKeyDown(KeyCode.Alpha9))
      return;
    this.m_LockPlate.SetActive(true);
    this.m_WingEventTable.DoStatePlateReset();
  }

  public void Initialize(AdventureWingDef wingDef)
  {
    this.m_WingDef = wingDef;
    this.gameObject.name = string.Format("{0}_{1}", (object) this.gameObject.name, (object) (int) wingDef.GetWingId());
    foreach (UberText wingTitle in this.m_WingTitles)
    {
      if ((UnityEngine.Object) wingTitle != (UnityEngine.Object) null)
        wingTitle.Text = this.m_WingDef.GetWingName();
    }
    if (!string.IsNullOrEmpty(wingDef.m_UnlockSpellPrefab) && (UnityEngine.Object) this.m_LockPlateFXContainer != (UnityEngine.Object) null)
    {
      this.m_UnlockSpell = SpellManager.Get().GetSpell(wingDef.m_UnlockSpellPrefab);
      GameUtils.SetParent((Component) this.m_UnlockSpell, this.m_LockPlateFXContainer);
      this.m_UnlockSpell.gameObject.SetActive(false);
    }
    this.SetAccent(wingDef.m_AccentPrefab);
    this.m_Owned = AdventureProgressMgr.Get().OwnsWing((int) wingDef.GetWingId());
    this.m_EventStartDetected = AdventureProgressMgr.IsWingEventActive((int) wingDef.GetWingId());
    this.m_Playable = this.m_Owned && this.m_EventStartDetected;
    this.m_Locked = AdventureProgressMgr.Get().IsWingLocked(wingDef);
    this.UpdatePurchasedBanner();
    bool flag1 = AdventureConfig.Get().GetSelectedMode() == AdventureModeDbId.LINEAR_HEROIC;
    bool flag2 = this.HasAckedAllPlateOpenEvents();
    AdventureWingKarazhanHelper component = this.GetComponent<AdventureWingKarazhanHelper>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.Initialize();
    if (this.IsDevMode)
      return;
    if (this.m_Playable && flag2 | flag1)
    {
      this.m_WingEventTable.DoStatePlateDeactivate();
    }
    else
    {
      this.UpdateBuyButton(StoreManager.Get().IsOpen());
      StoreManager.Get().RegisterStatusChangedListener(new Action<bool>(this.UpdateBuyButton));
      this.m_WingEventTable.DoStatePlateActivate();
      if (this.m_Locked && this.m_EventStartDetected)
      {
        this.m_WingEventTable.DoStatePlateInitialText();
        if (!((UnityEngine.Object) this.m_ReleaseLabelText != (UnityEngine.Object) null))
          return;
        this.m_ReleaseLabelText.Text = this.m_Owned ? GameStrings.Get(this.m_WingDef.m_LockedLocString) : GameStrings.Get(this.m_WingDef.m_LockedPurchaseLocString);
      }
      else
      {
        bool flag3 = AdventureProgressMgr.Get().OwnershipPrereqWingIsOwned(this.m_WingDef);
        if (!this.m_EventStartDetected)
        {
          this.m_WingEventTable.DoStatePlateInitialText();
          if (!((UnityEngine.Object) this.m_ReleaseLabelText != (UnityEngine.Object) null))
            return;
          this.m_ReleaseLabelText.Text = this.m_WingDef.GetComingSoonLabel();
        }
        else if (!this.m_Owned & flag3)
        {
          if ((UnityEngine.Object) this.m_prologueLoadingPlayMakerFSM_KARA != (UnityEngine.Object) null)
            this.m_prologueLoadingPlayMakerFSM_KARA.SendEvent("on");
          this.m_WingEventTable.DoStatePlateBuy(true);
        }
        else if (this.m_Owned && !flag2)
        {
          this.SetPlateKeyEvent(true);
        }
        else
        {
          this.m_WingEventTable.DoStatePlateInitialText();
          if (!((UnityEngine.Object) this.m_ReleaseLabelText != (UnityEngine.Object) null))
            return;
          this.m_ReleaseLabelText.Text = this.m_WingDef.GetRequiresLabel();
        }
      }
    }
  }

  public void InitializeDevMode()
  {
    if (!this.IsDevMode)
      return;
    int devModeSetting = AdventureScene.Get().DevModeSetting;
    this.m_WingEventTable.DoStatePlateActivate();
    switch (devModeSetting)
    {
      case 1:
        this.SetPlateKeyEvent(true);
        break;
      case 2:
        this.m_WingEventTable.DoStatePlateInitialText();
        break;
    }
    if (!((UnityEngine.Object) this.m_ReleaseLabelText != (UnityEngine.Object) null))
      return;
    this.m_ReleaseLabelText.Text = this.m_WingDef.GetComingSoonLabel();
  }

  public AdventureWingDef GetWingDef() => this.m_WingDef;

  public WingDbId GetWingId() => this.m_WingDef.GetWingId();

  public List<AdventureRewardsChest> GetChests()
  {
    List<AdventureRewardsChest> chests = new List<AdventureRewardsChest>();
    foreach (AdventureWing.Boss bossCoin in this.m_BossCoins)
      chests.Add(bossCoin.m_Chest);
    return chests;
  }

  public AdventureDbId GetAdventureId() => this.m_WingDef.GetAdventureId();

  public ProductType GetProductType() => StoreManager.GetAdventureProductType(this.GetAdventureId());

  public int GetProductData() => (int) this.GetWingId();

  public string GetWingName() => this.m_WingDef.GetWingName();

  public void AddBossSelectedListener(AdventureWing.BossSelected dlg) => this.m_BossSelectedListeners.Add(dlg);

  public void AddOpenPlateStartListener(AdventureWing.OpenPlateStart dlg) => this.m_OpenPlateStartListeners.Add(dlg);

  public void AddOpenPlateEndListener(AdventureWing.OpenPlateEnd dlg) => this.m_OpenPlateEndListeners.Add(dlg);

  public void AddShowRewardsListener(AdventureWing.ShowRewards dlg) => this.m_ShowRewardsListeners.Add(dlg);

  public void AddHideRewardsListener(AdventureWing.HideRewards dlg) => this.m_HideRewardsListeners.Add(dlg);

  public void AddShowRewardsPreviewListeners(AdventureWing.ShowRewardsPreview dlg) => this.m_ShowRewardsPreviewListeners.Add(dlg);

  public void AddTryPurchaseWingListener(AdventureWing.TryPurchaseWing dlg) => this.m_TryPurchaseWingListeners.Add(dlg);

  public bool ContainsBossCoin(AdventureBossCoin coin)
  {
    foreach (AdventureWing.Boss bossCoin in this.m_BossCoins)
    {
      if ((UnityEngine.Object) bossCoin.m_Coin == (UnityEngine.Object) coin)
        return true;
    }
    return false;
  }

  public AdventureBossCoin CreateBoss(
    string coinPrefab,
    string rewardsPrefab,
    ScenarioDbId mission,
    bool enabled)
  {
    AdventureBossCoin newcoin = GameUtils.LoadGameObjectWithComponent<AdventureBossCoin>(coinPrefab);
    AdventureRewardsChest newchest = GameUtils.LoadGameObjectWithComponent<AdventureRewardsChest>(rewardsPrefab);
    newcoin.gameObject.name = string.Format("{0}_{1}", (object) newcoin.gameObject.name, (object) mission);
    if ((UnityEngine.Object) newchest != (UnityEngine.Object) null)
    {
      newchest.gameObject.name = string.Format("{0}_{1}", (object) newchest.gameObject.name, (object) mission);
      this.UpdateBossChest(newchest, mission);
    }
    if ((UnityEngine.Object) this.m_CoinContainer != (UnityEngine.Object) null)
    {
      GameUtils.SetParent((Component) newcoin, this.m_CoinContainer);
      if ((UnityEngine.Object) newchest != (UnityEngine.Object) null)
      {
        GameUtils.SetParent((Component) newchest, this.m_CoinContainer);
        TransformUtil.SetLocalPosY((Component) newchest.transform, 0.01f);
      }
    }
    newcoin.Enable(enabled, false);
    newcoin.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.FireBossSelectedEvent(newcoin, mission)));
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      newchest.Enable(false);
      if ((UnityEngine.Object) newcoin.m_DisabledCollider != (UnityEngine.Object) null)
        newcoin.m_DisabledCollider.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.ShowBossRewards(mission, newcoin.transform.position)));
    }
    else
    {
      newchest.AddChestEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.ShowBossRewards(mission, newchest.transform.position)));
      newchest.AddChestEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.HideBossRewards(mission)));
    }
    if (this.m_BossCoins.Count == 0)
      newcoin.ShowConnector(false);
    this.m_BossCoins.Add(new AdventureWing.Boss()
    {
      m_MissionId = mission,
      m_Coin = newcoin,
      m_Chest = newchest
    });
    this.UpdateCoinPositions();
    return newcoin;
  }

  public void UpdateAllBossCoinChests()
  {
    foreach (AdventureWing.Boss bossCoin in this.m_BossCoins)
      this.UpdateBossChest(bossCoin.m_Chest, bossCoin.m_MissionId);
  }

  public void SetAccent(string accentPrefab)
  {
    if ((UnityEngine.Object) this.m_WallAccentObject != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_WallAccentObject);
    if ((UnityEngine.Object) this.m_PlateAccentObject != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_PlateAccentObject);
    if (string.IsNullOrEmpty(accentPrefab))
      return;
    if ((UnityEngine.Object) this.m_WallAccentContainer != (UnityEngine.Object) null)
    {
      this.m_WallAccentObject = AssetLoader.Get().InstantiatePrefab((AssetReference) accentPrefab);
      GameUtils.SetParent(this.m_WallAccentObject, this.m_WallAccentContainer);
    }
    if (!((UnityEngine.Object) this.m_PlateAccentContainer != (UnityEngine.Object) null))
      return;
    this.m_PlateAccentObject = UnityEngine.Object.Instantiate<GameObject>(this.m_WallAccentObject);
    GameUtils.SetParent(this.m_PlateAccentObject, this.m_PlateAccentContainer);
  }

  public void SetBringToFocusCallback(AdventureWing.BringToFocusCallback dlg) => this.m_BringToFocusCallback = dlg;

  public void OpenBigChest()
  {
    if (!((UnityEngine.Object) this.m_BigChest != (UnityEngine.Object) null))
      return;
    this.m_WingEventTable.DoStateBigChestOpen();
    this.BringToFocus();
    this.m_BigChest.RemoveEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ShowBigChestRewards));
    this.m_BigChest.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowBigChestRewards));
    this.m_BigChest.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideBigChestRewards));
  }

  public void HideBigChest() => this.m_WingEventTable.DoStateBigChestCover();

  public void BigChestStayOpen() => this.m_WingEventTable.DoStateBigChestStayOpen();

  public void SetBigChestRewards(WingDbId wingId)
  {
    if (AdventureConfig.Get().GetSelectedMode() != AdventureModeDbId.LINEAR)
      return;
    HashSet<Assets.Achieve.RewardTiming> rewardTimings = new HashSet<Assets.Achieve.RewardTiming>()
    {
      Assets.Achieve.RewardTiming.ADVENTURE_CHEST
    };
    List<RewardData> rewardsForWing = AdventureProgressMgr.GetRewardsForWing((int) wingId, rewardTimings);
    if ((UnityEngine.Object) this.m_BigChest != (UnityEngine.Object) null)
      this.m_BigChest.SetData((object) rewardsForWing);
    AdventureWingFrozenThroneHelper component = this.GetComponent<AdventureWingFrozenThroneHelper>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.SetBigChestRewards(wingId);
  }

  public List<RewardData> GetBigChestRewards() => !((UnityEngine.Object) this.m_BigChest != (UnityEngine.Object) null) ? (List<RewardData>) null : (List<RewardData>) this.m_BigChest.GetData();

  public bool HasBigChestRewards() => (UnityEngine.Object) this.m_BigChest != (UnityEngine.Object) null && this.m_BigChest.GetData() != null;

  public bool UpdateAndAnimateCoinsAndChests(
    float startDelay,
    bool forceCoinAnimation,
    AdventureWing.DelOnCoinAnimateCallback dlg)
  {
    if (this.m_WingEventTable.IsPlateInOrGoingToAnActiveState())
      return false;
    List<AdventureWing.Boss> thingsToFlip = new List<AdventureWing.Boss>();
    List<KeyValuePair<int, int>> keyValuePairList = new List<KeyValuePair<int, int>>();
    foreach (AdventureWing.Boss bossCoin in this.m_BossCoins)
    {
      int wingId = 0;
      int missionReqProgress = 0;
      bool reqs = AdventureConfig.IsMissionNewlyAvailableAndGetReqs((int) bossCoin.m_MissionId, ref wingId, ref missionReqProgress);
      if ((forceCoinAnimation || reqs) && (!forceCoinAnimation || AdventureProgressMgr.Get().CanPlayScenario((int) bossCoin.m_MissionId)) && !AdventureProgressMgr.Get().HasDefeatedScenario((int) bossCoin.m_MissionId))
      {
        keyValuePairList.Add(new KeyValuePair<int, int>(wingId, missionReqProgress));
        AdventureWing.Boss boss = new AdventureWing.Boss();
        boss.m_MissionId = bossCoin.m_MissionId;
        boss.m_Coin = bossCoin.m_Coin;
        if (AdventureProgressMgr.Get().ScenarioHasRewardData((int) bossCoin.m_MissionId))
          boss.m_Chest = bossCoin.m_Chest;
        thingsToFlip.Add(boss);
      }
    }
    foreach (KeyValuePair<int, int> keyValuePair in keyValuePairList)
    {
      if (AdventureConfig.SetWingAckIfGreater(keyValuePair.Key, keyValuePair.Value))
        AdventureMissionDisplay.Get().SetWingHasJustAckedProgress(keyValuePair.Key, true);
    }
    if (thingsToFlip.Count <= 0)
      return false;
    this.StartCoroutine(this.AnimateCoinsAndChests(thingsToFlip, startDelay, dlg));
    return true;
  }

  public void UpdatePlateState()
  {
    this.UpdatePurchasedBanner();
    bool flag1 = AdventureProgressMgr.Get().IsWingLocked(this.m_WingDef);
    bool flag2 = AdventureProgressMgr.Get().OwnsWing((int) this.m_WingDef.GetWingId());
    bool flag3 = AdventureProgressMgr.IsWingEventActive((int) this.m_WingDef.GetWingId());
    bool flag4 = flag2 & flag3;
    this.TryToUnlockAutomatically();
    if (flag4 && this.m_Playable && !flag1 && !this.m_Locked)
      return;
    if (flag4 && !flag1 && !this.m_WingEventTable.IsPlateKey())
    {
      if (this.m_BuyButtonOnOppositeSideOfKey && !this.m_WingEventTable.IsPlateBuy())
        this.m_WingEventTable.DoStatePlateBuy();
      if ((UnityEngine.Object) this.m_prologueLoadingPlayMakerFSM_KARA != (UnityEngine.Object) null)
        this.m_prologueLoadingPlayMakerFSM_KARA.SendEvent("off");
      this.SetPlateKeyEvent(false);
    }
    else if (!this.m_WingEventTable.IsPlateBuy())
    {
      if (flag1 & flag3)
      {
        this.m_WingEventTable.DoStatePlateInitialText();
        if ((UnityEngine.Object) this.m_ReleaseLabelText != (UnityEngine.Object) null)
          this.m_ReleaseLabelText.Text = flag2 ? GameStrings.Get(this.m_WingDef.m_LockedLocString) : GameStrings.Get(this.m_WingDef.m_LockedPurchaseLocString);
      }
      else
      {
        bool flag5 = AdventureProgressMgr.Get().OwnershipPrereqWingIsOwned(this.m_WingDef);
        if (!flag3)
        {
          this.m_WingEventTable.DoStatePlateInitialText();
          if ((UnityEngine.Object) this.m_ReleaseLabelText != (UnityEngine.Object) null)
            this.m_ReleaseLabelText.Text = this.m_WingDef.GetComingSoonLabel();
        }
        else if (!flag2 & flag5)
        {
          this.m_WingEventTable.DoStatePlateBuy();
        }
        else
        {
          this.m_WingEventTable.DoStatePlateInitialText();
          if ((UnityEngine.Object) this.m_ReleaseLabelText != (UnityEngine.Object) null)
            this.m_ReleaseLabelText.Text = this.m_WingDef.GetRequiresLabel();
        }
      }
    }
    this.m_EventStartDetected = flag3;
    this.m_Playable = flag4;
    this.m_Owned = flag2;
    this.m_Locked = flag1;
  }

  public void UpdateRewardsPreviewCover()
  {
    if (this.HasRewards())
      return;
    this.m_WingEventTable.DoStatePlateCoverPreviewChest();
  }

  public bool HasRewards()
  {
    List<RewardData> bigChestRewards = this.GetBigChestRewards();
    if (bigChestRewards != null && bigChestRewards.Count > 0)
      return true;
    foreach (AdventureWing.Boss bossCoin in this.m_BossCoins)
    {
      if (AdventureProgressMgr.Get().ScenarioHasRewardData((int) bossCoin.m_MissionId))
        return true;
    }
    return false;
  }

  public void RandomizeBackground()
  {
    if (this.m_BackgroundOffsets.Count == 0)
      return;
    int index;
    do
    {
      index = UnityEngine.Random.Range(0, this.m_BackgroundOffsets.Count);
    }
    while (AdventureWing.s_LastRandomNumbers.Contains(index));
    AdventureWing.s_LastRandomNumbers.Add(index);
    if (AdventureWing.s_LastRandomNumbers.Count >= this.m_BackgroundOffsets.Count)
      AdventureWing.s_LastRandomNumbers.RemoveAt(0);
    foreach (AdventureWing.BackgroundRandomization backgroundRenderer in this.m_BackgroundRenderers)
    {
      if (!((UnityEngine.Object) backgroundRenderer.m_backgroundRenderer == (UnityEngine.Object) null) && !string.IsNullOrEmpty(backgroundRenderer.m_materialTextureName))
      {
        Material material = RendererExtension.GetMaterial((Renderer) backgroundRenderer.m_backgroundRenderer);
        Vector2 textureOffset = material.GetTextureOffset(backgroundRenderer.m_materialTextureName) with
        {
          y = this.m_BackgroundOffsets[index]
        };
        material.SetTextureOffset(backgroundRenderer.m_materialTextureName, textureOffset);
      }
    }
  }

  public void BringToFocus()
  {
    if (this.m_BringToFocusCallback == null)
      return;
    this.m_BringToFocusCallback(this);
  }

  public void HideBossChests()
  {
    foreach (AdventureWing.Boss bossCoin in this.m_BossCoins)
    {
      if ((UnityEngine.Object) bossCoin.m_Chest != (UnityEngine.Object) null)
        bossCoin.m_Chest.FadeOutChestImmediate();
    }
  }

  public void NavigateBackCleanup()
  {
    if (!((UnityEngine.Object) this.m_prologueLoadingPlayMakerFSM_KARA != (UnityEngine.Object) null))
      return;
    this.m_prologueLoadingPlayMakerFSM_KARA.SendEvent("cancel");
  }

  public void GetCompleteQuoteAssetsFromTargetWingEventTiming(
    int targetWingId,
    out string completeQuotePrefab,
    out string completeQuoteVOLine)
  {
    completeQuotePrefab = this.m_WingDef.m_CompleteQuotePrefab;
    completeQuoteVOLine = this.m_WingDef.m_CompleteQuoteVOLine;
    if (targetWingId == 0 || AdventureProgressMgr.IsWingEventActive(targetWingId) || string.IsNullOrEmpty(this.m_WingDef.m_CompleteQuoteNextWingLockedPrefab) || string.IsNullOrEmpty(this.m_WingDef.m_CompleteQuoteNextWingLockedVOLine))
      return;
    completeQuotePrefab = this.m_WingDef.m_CompleteQuoteNextWingLockedPrefab;
    completeQuoteVOLine = this.m_WingDef.m_CompleteQuoteNextWingLockedVOLine;
  }

  private IEnumerator AnimateCoinsAndChests(
    List<AdventureWing.Boss> thingsToFlip,
    float delaySeconds,
    AdventureWing.DelOnCoinAnimateCallback dlg)
  {
    AdventureWing adventureWing = this;
    if ((double) delaySeconds > 0.0)
      yield return (object) new WaitForSeconds(delaySeconds);
    if (dlg != null)
      dlg(thingsToFlip[0].m_Coin.transform.position);
    for (int i = 0; i < thingsToFlip.Count; ++i)
    {
      AdventureWing.Boss boss = thingsToFlip[i];
      adventureWing.StartCoroutine(adventureWing.AnimateOneCoinAndChest(boss));
      yield return (object) new WaitForSeconds(0.2f);
    }
    yield return (object) new WaitForSeconds(0.5f);
  }

  private IEnumerator AnimateOneCoinAndChest(AdventureWing.Boss boss)
  {
    if ((UnityEngine.Object) boss.m_Chest != (UnityEngine.Object) null && !AdventureProgressMgr.Get().HasDefeatedScenario((int) boss.m_MissionId))
      boss.m_Chest.BlinkChest();
    yield return (object) new WaitForSeconds(0.5f);
    boss.m_Coin.Enable(true);
    yield return (object) new WaitForSeconds(1f);
    if ((UnityEngine.Object) boss.m_Chest != (UnityEngine.Object) null && boss.m_Chest.m_fadedOut)
      boss.m_Chest.FadeInChest();
    boss.m_Coin.ShowNewLookGlow();
  }

  private void UpdateCoinPositions()
  {
    int num = 0;
    foreach (AdventureWing.Boss bossCoin in this.m_BossCoins)
    {
      bossCoin.m_Coin.transform.localPosition = this.m_CoinsOffset;
      TransformUtil.SetLocalPosX((Component) bossCoin.m_Coin, this.m_CoinsOffset.x + (float) num * this.m_CoinSpacing);
      if ((UnityEngine.Object) bossCoin.m_Chest != (UnityEngine.Object) null)
      {
        bossCoin.m_Chest.transform.localPosition = this.m_CoinsOffset;
        TransformUtil.SetLocalPosX((Component) bossCoin.m_Chest, this.m_CoinsOffset.x + (float) num * this.m_CoinSpacing);
        bossCoin.m_Chest.transform.localPosition += this.m_CoinsChestOffset;
      }
      ++num;
    }
  }

  private void FireBossSelectedEvent(AdventureBossCoin coin, ScenarioDbId mission)
  {
    foreach (AdventureWing.BossSelected bossSelected in this.m_BossSelectedListeners.ToArray())
      bossSelected(coin, mission);
  }

  private void FireOpenPlateStartEvent()
  {
    foreach (AdventureWing.OpenPlateStart openPlateStart in this.m_OpenPlateStartListeners.ToArray())
      openPlateStart(this);
  }

  protected void FireOpenPlateEndEvent(Spell s)
  {
    if ((UnityEngine.Object) this.m_UnlockSpell != (UnityEngine.Object) null)
      this.m_UnlockSpell.gameObject.SetActive(false);
    foreach (AdventureWing.OpenPlateEnd openPlateEnd in this.m_OpenPlateEndListeners.ToArray())
      openPlateEnd(this);
  }

  private void OnUnlockButtonOut(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_UnlockButtonHighlightMesh_LOE == (UnityEngine.Object) null)
      return;
    RendererExtension.GetMaterial((Renderer) this.m_UnlockButtonHighlightMesh_LOE).SetFloat("_Intensity", this.m_UnlockButtonHighlightIntensityOut);
  }

  private void OnUnlockButtonOver(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_UnlockButtonHighlightMesh_LOE == (UnityEngine.Object) null)
      return;
    RendererExtension.GetMaterial((Renderer) this.m_UnlockButtonHighlightMesh_LOE).SetFloat("_Intensity", this.m_UnlockButtonHighlightIntensityOver);
  }

  private void UnlockButtonPressed(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_WingDef != (UnityEngine.Object) null && !string.IsNullOrEmpty(this.m_WingDef.GetOpeningNotRecommendedWarning()) && !this.IsWingRecommendedToOpen())
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_text = this.m_WingDef.GetOpeningNotRecommendedWarning(),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = new AlertPopup.ResponseCallback(this.OnConfirmWingUnlockResponse)
      });
    else
      this.UnlockPlate();
  }

  private void OnConfirmWingUnlockResponse(AlertPopup.Response response, object userData)
  {
    if (response != AlertPopup.Response.CONFIRM)
      return;
    this.UnlockPlate();
  }

  public bool TryToUnlockAutomatically()
  {
    if (this.IsDevMode || !this.m_WingDef.GetUnlocksAutomatically() || !this.m_WingEventTable.IsPlateKey() && (!this.m_WingEventTable.IsPlatePartiallyOpen() || !this.HasDependentWingJustAckedRequiredProgress()) || AdventureProgressMgr.Get().IsWingLocked(this.m_WingDef) || !AdventureProgressMgr.Get().OwnsWing((int) this.m_WingDef.GetWingId()) || !AdventureProgressMgr.IsWingEventActive((int) this.m_WingDef.GetWingId()) || !this.CanPlayAtLeastOneScenario())
      return false;
    this.UnlockPlate();
    return true;
  }

  public void UnlockPlate()
  {
    if ((UnityEngine.Object) this.m_UnlockButton != (UnityEngine.Object) null)
      this.m_UnlockButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.UnlockButtonPressed));
    float startDelay = 0.0f;
    if (this.m_BringToFocusCallback != null)
    {
      startDelay = 0.5f;
      this.m_BringToFocusCallback(this);
    }
    this.StartCoroutine(this.DoUnlockPlate(startDelay));
  }

  private IEnumerator DoUnlockPlate(float startDelay)
  {
    AdventureWing adventureWing = this;
    adventureWing.FireOpenPlateStartEvent();
    if ((double) startDelay > 0.0)
      yield return (object) new WaitForSeconds(startDelay);
    adventureWing.m_WingEventTable.AddOpenPlateEndEventListener(new StateEventTable.StateEventTrigger(adventureWing.FireOpenPlateEndEvent), true);
    if ((UnityEngine.Object) adventureWing.m_UnlockButton != (UnityEngine.Object) null)
      adventureWing.m_UnlockButton.GetComponent<Collider>().enabled = false;
    float unlockDelay = 0.0f;
    if ((UnityEngine.Object) adventureWing.m_UnlockSpell != (UnityEngine.Object) null)
    {
      AdventureWingUnlockSpell component = adventureWing.m_UnlockSpell.GetComponent<AdventureWingUnlockSpell>();
      unlockDelay = (UnityEngine.Object) component != (UnityEngine.Object) null ? component.m_UnlockDelay : 0.0f;
    }
    adventureWing.DoOpenPlate(unlockDelay);
    adventureWing.m_ContentsContainer.SetActive(true);
    if ((UnityEngine.Object) adventureWing.m_UnlockSpell != (UnityEngine.Object) null)
    {
      adventureWing.m_UnlockSpell.gameObject.SetActive(true);
      adventureWing.m_UnlockSpell.AddFinishedCallback(new Spell.FinishedCallback(adventureWing.OnUnlockSpellFinished));
      adventureWing.m_UnlockSpell.Activate();
    }
    else
      adventureWing.OnUnlockSpellFinished((Spell) null, (object) null);
    if ((UnityEngine.Object) adventureWing.m_UnlockButton != (UnityEngine.Object) null)
      adventureWing.m_UnlockButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(adventureWing.UnlockButtonPressed));
  }

  protected virtual void DoOpenPlate(float unlockDelay)
  {
    int progressValueForWing = AdventureProgressMgr.Get().GetProgressValueForWing((int) this.m_WingDef.GetWingId());
    int plateOpenEventIndex = progressValueForWing - 1;
    int ack;
    AdventureProgressMgr.Get().GetWingAck((int) this.m_WingDef.GetWingId(), out ack);
    if (ack < progressValueForWing || this.m_HasJustAckedProgress || this.IsDevMode)
    {
      if (!this.m_WingEventTable.SupportsIncrementalOpening() && ack != 0)
        return;
      if (this.IsDevMode)
        plateOpenEventIndex = 0;
      this.m_WingEventTable.DoStatePlateOpen(plateOpenEventIndex, unlockDelay);
    }
    else
      this.FireOpenPlateEndEvent((Spell) null);
  }

  protected bool HasDependentWingJustAckedRequiredProgress()
  {
    foreach (AdventureMissionDbfRecord record in GameDbf.AdventureMission.GetRecords((Predicate<AdventureMissionDbfRecord>) (r => (WingDbId) r.GrantsWingId == this.m_WingDef.GetWingId())))
    {
      if (AdventureMissionDisplay.Get().HasWingJustAckedRequiredProgress(record.ReqWingId, record.ReqProgress))
        return true;
    }
    return false;
  }

  protected bool HasDependentWingJustAckedRequiredProgress(AdventureMissionDbfRecord record) => AdventureMissionDisplay.Get().HasWingJustAckedRequiredProgress(record.ReqWingId, record.ReqProgress);

  public bool HasJustAckedRequiredProgress(int requiredProgress)
  {
    int progressValueForWing = AdventureProgressMgr.Get().GetProgressValueForWing((int) this.m_WingDef.GetWingId());
    return this.m_HasJustAckedProgress && progressValueForWing == requiredProgress;
  }

  public void SetHasJustAckedProgress(bool hasJustAckedProgress) => this.m_HasJustAckedProgress = hasJustAckedProgress;

  private bool CanPlayAtLeastOneScenario()
  {
    foreach (AdventureWing.Boss bossCoin in this.m_BossCoins)
    {
      if (AdventureProgressMgr.Get().CanPlayScenario((int) bossCoin.m_MissionId))
        return true;
    }
    return false;
  }

  protected virtual bool InitializePlateOpenState()
  {
    int ack;
    AdventureProgressMgr.Get().GetWingAck((int) this.m_WingDef.GetWingId(), out ack);
    int plateAlreadyOpenEventIndex = ack - 1;
    if (ack < 1)
      return false;
    this.m_WingEventTable.DoStatePlateAlreadyOpen(plateAlreadyOpenEventIndex);
    return true;
  }

  private void OnUnlockSpellFinished(Spell spell, object userData)
  {
    if (!AdventureUtils.CanPlayWingOpenQuote(this.m_WingDef))
      return;
    this.StartCoroutine(this.PlayOpenQuoteAfterDelay());
  }

  private IEnumerator PlayOpenQuoteAfterDelay()
  {
    if (!((UnityEngine.Object) this.m_WingDef == (UnityEngine.Object) null))
    {
      yield return (object) new WaitForSeconds(this.m_WingDef.m_OpenQuoteDelay);
      string prefabPath = this.m_WingDef.m_OpenQuotePrefab;
      if (string.IsNullOrEmpty(prefabPath))
        prefabPath = AdventureScene.Get().GetAdventureDef(this.GetAdventureId()).m_DefaultQuotePrefab;
      Vector3 position = (bool) UniversalInputManager.UsePhoneUI ? NotificationManager.PHONE_CHARACTER_POS : NotificationManager.ALT_ADVENTURE_SCREEN_POS;
      string legacyAssetName = new AssetReference(this.m_WingDef.m_OpenQuoteVOLine).GetLegacyAssetName();
      NotificationManager.Get().CreateCharacterQuote(prefabPath, position, GameStrings.Get(legacyAssetName), this.m_WingDef.m_OpenQuoteVOLine, this.IsDevMode);
    }
  }

  protected void ShowBigChestRewards(UIEvent e)
  {
    List<RewardData> bigChestRewards = this.GetBigChestRewards();
    if (bigChestRewards == null)
      return;
    this.FireShowRewardsEvent(bigChestRewards, this.m_BigChest.transform.position);
  }

  protected void HideBigChestRewards(UIEvent e)
  {
    List<RewardData> bigChestRewards = this.GetBigChestRewards();
    if (bigChestRewards == null)
      return;
    this.FireHideRewardsEvent(bigChestRewards);
  }

  private void ShowBossRewards(ScenarioDbId mission, Vector3 origin) => this.FireShowRewardsEvent(AdventureProgressMgr.Get().GetImmediateRewardsForDefeatingScenario((int) mission), origin);

  private void HideBossRewards(ScenarioDbId mission) => this.FireHideRewardsEvent(AdventureProgressMgr.Get().GetImmediateRewardsForDefeatingScenario((int) mission));

  public void FireShowRewardsEvent(List<RewardData> rewards, Vector3 origin)
  {
    foreach (AdventureWing.ShowRewards showRewards in this.m_ShowRewardsListeners.ToArray())
      showRewards(rewards, origin);
  }

  public void FireHideRewardsEvent(List<RewardData> rewards)
  {
    foreach (AdventureWing.HideRewards hideRewards in this.m_HideRewardsListeners.ToArray())
      hideRewards(rewards);
  }

  private void FireShowRewardsPreviewEvent()
  {
    foreach (AdventureWing.ShowRewardsPreview showRewardsPreview in this.m_ShowRewardsPreviewListeners.ToArray())
      showRewardsPreview();
  }

  private void FireTryPurchaseWingEvent()
  {
    foreach (AdventureWing.TryPurchaseWing tryPurchaseWing in this.m_TryPurchaseWingListeners.ToArray())
      tryPurchaseWing();
  }

  private void UpdateBossChest(AdventureRewardsChest chest, ScenarioDbId mission)
  {
    AdventureConfig adventureConfig = AdventureConfig.Get();
    if (adventureConfig.IsScenarioDefeatedAndInitCache(mission))
    {
      if (adventureConfig.IsScenarioJustDefeated(mission))
        chest.SlamInCheckmark();
      else
        chest.ShowCheckmark();
    }
    else if (AdventureProgressMgr.ScenarioUsesGameSaveDataProgress((int) mission) && AdventureProgressMgr.Get().CanPlayScenario((int) mission))
    {
      int progress;
      int maxProgress;
      if (!AdventureProgressMgr.GetGameSaveDataProgressForScenario((int) mission, out progress, out maxProgress))
        return;
      chest.ShowGameSaveDataProgress(progress, maxProgress);
    }
    else
    {
      if (AdventureProgressMgr.Get().ScenarioHasRewardData((int) mission))
        return;
      chest.HideAll();
    }
  }

  private void UpdatePurchasedBanner()
  {
    if (!((UnityEngine.Object) this.m_PurchasedBanner != (UnityEngine.Object) null))
      return;
    bool flag1 = AdventureProgressMgr.Get().OwnsWing((int) this.m_WingDef.GetWingId());
    bool flag2 = AdventureProgressMgr.IsWingEventActive((int) this.m_WingDef.GetWingId());
    this.m_PurchasedBanner.SetActive(flag1 && !flag2);
  }

  private void UpdateBuyButton(bool isStoreOpen)
  {
    if ((UnityEngine.Object) this.m_BuyButton == (UnityEngine.Object) null)
      return;
    float num = 0.0f;
    bool flag = true;
    string key = "GLUE_STORE_MONEY_BUTTON_TOOLTIP_HEADLINE";
    if (!isStoreOpen)
    {
      num = 1f;
      flag = false;
      key = "GLUE_ADVENTURE_LABEL_SHOP_CLOSED";
    }
    RendererExtension.GetMaterial((Renderer) this.m_BuyButtonMesh).SetFloat("_Desaturate", num);
    this.m_BuyButton.GetComponent<Collider>().enabled = flag;
    this.m_BuyButtonText.SetText(GameStrings.Get(key));
  }

  private bool HasAckedAllPlateOpenEvents()
  {
    int ack;
    AdventureProgressMgr.Get().GetWingAck((int) this.m_WingDef.GetWingId(), out ack);
    return !this.m_WingEventTable.SupportsIncrementalOpening() ? ack >= 1 : this.m_WingEventTable.m_PlateOpenEvents.Count == ack;
  }

  private bool IsWingRecommendedToOpen()
  {
    if (this.m_WingDef.GetOpenPrereqId() == WingDbId.INVALID)
      return true;
    AdventureWingDef wingDef = AdventureScene.Get().GetWingDef(this.m_WingDef.GetOpenPrereqId());
    return (UnityEngine.Object) wingDef == (UnityEngine.Object) null || AdventureProgressMgr.Get().IsWingComplete(wingDef.GetAdventureId(), AdventureConfig.Get().GetSelectedMode(), wingDef.GetWingId());
  }

  private void SetPlateKeyEvent(bool initial)
  {
    bool open = this.IsWingRecommendedToOpen();
    this.m_WingEventTable.DoStatePlateKey(open, initial);
    if ((UnityEngine.Object) this.m_ReleaseLabelText != (UnityEngine.Object) null)
      this.m_ReleaseLabelText.Text = open || string.IsNullOrEmpty(this.m_WingDef.GetOpeningNotRecommendedLabel()) ? (!this.m_WingDef.GetUnlocksAutomatically() ? GameStrings.Get("GLUE_ADVENTURE_READY_TO_OPEN") : this.m_WingDef.GetWingName()) : this.m_WingDef.GetOpeningNotRecommendedLabel();
    this.InitializePlateOpenState();
    this.TryToUnlockAutomatically();
  }

  [Serializable]
  public class BackgroundRandomization
  {
    public MeshRenderer m_backgroundRenderer;
    public string m_materialTextureName = "_MainTex";
  }

  public delegate void BossSelected(AdventureBossCoin coin, ScenarioDbId mission);

  public delegate void OpenPlateStart(AdventureWing wing);

  public delegate void OpenPlateEnd(AdventureWing wing);

  public delegate void ShowRewards(List<RewardData> rewards, Vector3 origin);

  public delegate void HideRewards(List<RewardData> rewards);

  public delegate void ShowRewardsPreview();

  public delegate void TryPurchaseWing();

  public delegate void DelOnCoinAnimateCallback(Vector3 coinPosition);

  public delegate void BringToFocusCallback(AdventureWing wing);

  protected class Boss
  {
    public ScenarioDbId m_MissionId;
    public AdventureBossCoin m_Coin;
    public AdventureRewardsChest m_Chest;
  }
}
