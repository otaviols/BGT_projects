using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroicBrawlRewardDisplay : MonoBehaviour
{
  public GameObject m_Root;
  public float m_FirewarksRewardDelayMin = 0.45f;
  public float m_FirewarksRewardDelayMax = 0.75f;
  public float m_FirewarksRewardHold = 0.7f;
  public float m_FireworksRewardRanRot = 30f;
  public float m_CardRewardDelay = 0.5f;
  public float m_CardRewardBurstDelay = 0.2f;
  public float m_EndScaleAwayTime = 0.3f;
  public float m_CardAnimationTime = 0.5f;
  public PlayMakerFSM m_PackFireworkFSM;
  public PlayMakerFSM m_GoldFireworkFSM;
  public PlayMakerFSM m_DustFireworkFSM;
  public PlayMakerFSM m_CardFireworkFSM;
  public NormalButton m_DoneButton;
  public PlayMakerFSM m_FSM;
  public GameObject m_RewardFireworksRoot;
  public HeroicBrawlRewardDisplay.FireworkRewardZone[] m_RewardZones;
  public GameObject m_FinalRewardsRoot;
  public UberText m_BannerUberText;
  public HeroicBrawlRewardDisplay.RewardVisuals[] m_RewardVisuals = new HeroicBrawlRewardDisplay.RewardVisuals[12];
  public GameObject m_CardsRoot;
  public HeroicBrawlRewardDisplay.CardVisuals[] m_CardVisuals = new HeroicBrawlRewardDisplay.CardVisuals[3];
  public int m_DebugWins;
  public const string DEFAULT_PREFAB = "HeroicBrawlReward.prefab:8f49f1fcb5ca4485d9b6b22993e1b1ab";
  public PegUIElement m_HeroicRewardChest;
  public GameObject m_DescText;
  private List<RewardData> m_Rewards = new List<RewardData>();
  private HeroicBrawlRewardDisplay.RewardsReceivedData m_RewardsReceived;
  private List<Reward> m_finalRewards = new List<Reward>();
  private int m_finalRewardsLoadedCount;
  private int m_lastZone = 1;
  private List<Action> m_doneCallbacks;
  private int m_wins;
  private bool m_fromNotice;
  private long m_noticeID = -1;
  private static HeroicBrawlRewardDisplay s_instance;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    HeroicBrawlRewardDisplay.s_instance = this;
    this.m_doneCallbacks = new List<Action>();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void Start() => this.Init();

  private void OnDestroy() => HeroicBrawlRewardDisplay.s_instance = (HeroicBrawlRewardDisplay) null;

  public static HeroicBrawlRewardDisplay Get() => HeroicBrawlRewardDisplay.s_instance;

  public void ShowRewards(int wins, List<RewardData> rewards, bool fromNotice = false, long noticeID = -1)
  {
    if ((UnityEngine.Object) this.m_FSM == (UnityEngine.Object) null)
      Debug.LogErrorFormat("FSM is null!");
    else if (rewards == null && rewards.Count < 1)
    {
      Debug.LogErrorFormat("rewards is null!");
    }
    else
    {
      this.m_Rewards = rewards;
      this.m_wins = wins;
      this.m_fromNotice = fromNotice;
      this.m_noticeID = noticeID;
      this.m_DescText.SetActive(fromNotice);
      this.ShowRewardChest();
    }
  }

  [ContextMenu("Debug Show Rewards")]
  public void DebugShowRewards() => this.ShowRewards(this.m_DebugWins, this.DebugRewards(this.m_DebugWins));

  public void RegisterDoneCallback(Action action) => this.m_doneCallbacks.Add(action);

  private void Init()
  {
    for (int index = 0; index < this.m_RewardZones.Length; ++index)
    {
      this.m_RewardZones[index].goldReward = this.m_RewardZones[index].GoldGameObject.GetComponentInChildren<GoldReward>();
      this.m_RewardZones[index].dustReward = this.m_RewardZones[index].DustGameObject.GetComponentInChildren<ArcaneDustReward>();
    }
    this.m_FinalRewardsRoot.SetActive(false);
    this.m_RewardFireworksRoot.SetActive(false);
    this.m_PackFireworkFSM.gameObject.SetActive(false);
    this.m_GoldFireworkFSM.gameObject.SetActive(false);
    this.m_DustFireworkFSM.gameObject.SetActive(false);
    this.m_CardFireworkFSM.gameObject.SetActive(false);
  }

  private void ShowRewardChest()
  {
    this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective);
    this.LoadFinalRewards();
    this.m_HeroicRewardChest.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ShowRewardsCeremony));
  }

  private void ShowRewardsCeremony(UIEvent e) => this.StartCoroutine(this.AnimateRewardsCeremony());

  private IEnumerator AnimateRewardsCeremony()
  {
    HeroicBrawlRewardDisplay brawlRewardDisplay = this;
    brawlRewardDisplay.m_HeroicRewardChest.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(brawlRewardDisplay.ShowRewardsCeremony));
    HeroicBrawlRewardDisplay.RewardVisuals visuals = brawlRewardDisplay.m_RewardVisuals[brawlRewardDisplay.m_wins];
    brawlRewardDisplay.m_DescText.SetActive(false);
    if (visuals.DropBox)
    {
      brawlRewardDisplay.m_FSM.FsmVariables.GetFsmBool("ShatterDialog").Value = visuals.ShatterDialog;
      brawlRewardDisplay.m_FSM.SendEvent("DropBox");
    }
    else
      brawlRewardDisplay.m_FSM.SendEvent("OpenBoxOnly");
    while (!brawlRewardDisplay.m_FSM.FsmVariables.GetFsmBool("isChestAnimationDone").Value)
      yield return (object) null;
    if (visuals.DropBox)
      brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.ShowRewardsFireworks(brawlRewardDisplay.m_wins));
    else
      brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.ShowRewardsSimple(brawlRewardDisplay.m_wins));
  }

  private IEnumerator ShowRewardsSimple(int wins)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    HeroicBrawlRewardDisplay brawlRewardDisplay = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.ShowFinalRewards(wins, true));
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private IEnumerator ShowRewardsFireworks(int wins)
  {
    HeroicBrawlRewardDisplay brawlRewardDisplay = this;
    HeroicBrawlRewardDisplay.RewardVisuals rewardvisuals = brawlRewardDisplay.m_RewardVisuals[wins];
    brawlRewardDisplay.m_RewardFireworksRoot.SetActive(true);
    brawlRewardDisplay.LoadBoosterReward();
    brawlRewardDisplay.InitRewardsReceived();
    int remainingPacks = brawlRewardDisplay.m_RewardsReceived.PackCount;
    int remainingGold = brawlRewardDisplay.m_RewardsReceived.GoldCount;
    int remainingDust = brawlRewardDisplay.m_RewardsReceived.DustCount;
    int lastType = 0;
    while (remainingPacks > 0 || remainingGold > 0 || remainingDust > 0)
    {
      int zone = brawlRewardDisplay.NextRewardZone();
      int num1 = UnityEngine.Random.Range(0, 4);
      if (num1 == lastType)
      {
        if (lastType == 0 || lastType == 1)
          num1 = UnityEngine.Random.Range(2, 4);
        else if (lastType == 2)
        {
          num1 = UnityEngine.Random.Range(0, 3);
          if (num1 == 2)
            num1 = 3;
        }
        else
          num1 = UnityEngine.Random.Range(0, 3);
      }
      if ((num1 == 0 || num1 == 1) && remainingPacks <= 0)
        num1 = 2;
      if (num1 == 2 && remainingGold <= 0)
        num1 = 3;
      if (num1 == 3 && remainingDust <= 0)
        num1 = 0;
      if ((num1 == 0 || num1 == 1) && remainingPacks <= 0)
        num1 = 2;
      if (num1 == 2 && remainingGold <= 0)
        num1 = 3;
      Vector3 localPosition = brawlRewardDisplay.ZoneRandomLocalPosition(zone);
      if ((num1 == 0 || num1 == 1) && remainingPacks > 0)
      {
        lastType = num1;
        --remainingPacks;
        brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.DisplayFireworkPack(zone, localPosition));
        yield return (object) new WaitForSeconds(brawlRewardDisplay.GetFireworkRewardDelay());
      }
      else if (num1 == 2 && remainingGold > 0)
      {
        lastType = num1;
        int num2 = UnityEngine.Random.Range(rewardvisuals.GoldPerBagMin, rewardvisuals.GoldPerBagMax);
        int amount = brawlRewardDisplay.m_RewardsReceived.GoldCount - num2;
        if (amount > num2)
          amount = num2;
        remainingGold -= amount;
        brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.DisplayFireworkGold(zone, localPosition, amount));
        yield return (object) new WaitForSeconds(brawlRewardDisplay.GetFireworkRewardDelay());
      }
      else if (num1 == 3 && remainingDust > 0)
      {
        lastType = num1;
        int num3 = UnityEngine.Random.Range(rewardvisuals.DustPerBottleMin, rewardvisuals.DustPerBottleMax);
        int amount = brawlRewardDisplay.m_RewardsReceived.DustCount - num3;
        if (amount > num3)
          amount = num3;
        remainingDust -= amount;
        brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.DisplayFireworkDust(zone, localPosition, amount));
        yield return (object) new WaitForSeconds(brawlRewardDisplay.GetFireworkRewardDelay());
      }
      else
      {
        Debug.LogWarningFormat("No reward found: Packs: {0}, Gold: {1}, Dust: {2}", (object) remainingPacks, (object) remainingGold, (object) remainingDust);
        break;
      }
    }
    yield return (object) new WaitForSeconds(1f);
    if (brawlRewardDisplay.m_RewardsReceived.CardsCount > 0)
    {
      brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.ShowCards(wins));
      yield return (object) new WaitForSeconds((brawlRewardDisplay.m_CardRewardDelay + brawlRewardDisplay.m_CardRewardBurstDelay) * (float) brawlRewardDisplay.m_RewardsReceived.CardsCount);
    }
    else
      brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.ShowFinalRewards(wins));
  }

  private float GetFireworkRewardDelay() => UnityEngine.Random.Range(this.m_FirewarksRewardDelayMin, this.m_FirewarksRewardDelayMax);

  private float DisplayFirework(PlayMakerFSM fsm, Vector3 targetPosition)
  {
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fsm.gameObject);
    gameObject.transform.parent = this.transform;
    gameObject.transform.position = fsm.transform.position;
    gameObject.SetActive(true);
    gameObject.gameObject.layer = fsm.gameObject.layer;
    PlayMakerFSM component = gameObject.GetComponent<PlayMakerFSM>();
    component.FsmVariables.FindFsmVector3("TargetPosition").Value = targetPosition;
    component.SendEvent("Firework");
    this.m_FSM.SendEvent("BounceBox");
    return component.FsmVariables.GetFsmFloat("FireworkTime").Value;
  }

  private IEnumerator DisplayFireworkPack(int zone, Vector3 localPosition)
  {
    yield return (object) new WaitForSeconds(this.DisplayFirework(this.m_PackFireworkFSM, this.m_RewardZones[zone].ZoneRoot.transform.TransformPoint(localPosition)));
    GameObject packGO = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardZones[zone].packReward.gameObject);
    packGO.layer = this.m_RewardZones[zone].PackGameObject.layer;
    packGO.transform.parent = this.m_RewardZones[zone].ZoneRoot.transform;
    packGO.transform.localPosition = localPosition;
    packGO.transform.localEulerAngles = new Vector3(0.0f, UnityEngine.Random.Range(-this.m_FireworksRewardRanRot, this.m_FireworksRewardRanRot), 0.0f);
    packGO.transform.localScale = this.m_RewardZones[zone].PackGameObject.transform.localScale;
    BoosterPackReward packReward = packGO.GetComponent<BoosterPackReward>();
    packReward.SetData((RewardData) new BoosterPackRewardData()
    {
      Count = 1,
      Id = this.m_RewardsReceived.PackID
    }, true);
    packReward.m_RotateIn = false;
    packReward.m_showBanner = false;
    packReward.m_playSounds = false;
    yield return (object) null;
    packReward.Show(true);
    yield return (object) new WaitForSeconds(this.m_FirewarksRewardHold);
    packReward.HideWithFX();
    this.m_RewardZones[zone].packReward.Hide(true);
    yield return (object) new WaitForSeconds(3f);
    if ((UnityEngine.Object) packGO != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) packGO);
  }

  private IEnumerator DisplayFireworkGold(int zone, Vector3 localPosition, int amount)
  {
    yield return (object) new WaitForSeconds(this.DisplayFirework(this.m_GoldFireworkFSM, this.m_RewardZones[zone].ZoneRoot.transform.TransformPoint(localPosition)));
    GoldRewardData data = new GoldRewardData();
    data.Amount = (long) amount;
    GameObject goldGO = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardZones[zone].goldReward.gameObject);
    goldGO.layer = this.m_RewardZones[zone].GoldGameObject.layer;
    goldGO.transform.parent = this.m_RewardZones[zone].ZoneRoot.transform;
    goldGO.transform.localPosition = localPosition;
    goldGO.transform.localEulerAngles = new Vector3(0.0f, UnityEngine.Random.Range(-this.m_FireworksRewardRanRot, this.m_FireworksRewardRanRot), 0.0f);
    goldGO.transform.localScale = this.m_RewardZones[zone].GoldGameObject.transform.localScale;
    GoldReward goldReward = goldGO.GetComponent<GoldReward>();
    yield return (object) null;
    goldReward.SetData((RewardData) data, true);
    goldReward.m_RotateIn = false;
    goldReward.m_showBanner = false;
    goldReward.m_playSounds = false;
    goldReward.Show(true);
    yield return (object) new WaitForSeconds(this.m_FirewarksRewardHold);
    goldReward.HideWithFX();
    yield return (object) new WaitForSeconds(3f);
    if ((UnityEngine.Object) goldGO != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) goldGO);
  }

  private IEnumerator DisplayFireworkDust(int zone, Vector3 localPosition, int amount)
  {
    yield return (object) new WaitForSeconds(this.DisplayFirework(this.m_DustFireworkFSM, this.m_RewardZones[zone].ZoneRoot.transform.TransformPoint(localPosition)));
    ArcaneDustRewardData data = new ArcaneDustRewardData();
    data.Amount = amount;
    data.MarkAsDummyReward();
    GameObject dustGO = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardZones[zone].dustReward.gameObject);
    dustGO.layer = this.m_RewardZones[zone].DustGameObject.layer;
    dustGO.transform.parent = this.m_RewardZones[zone].ZoneRoot.transform;
    dustGO.transform.localPosition = localPosition;
    dustGO.transform.localEulerAngles = new Vector3(0.0f, UnityEngine.Random.Range(-this.m_FireworksRewardRanRot, this.m_FireworksRewardRanRot), 0.0f);
    dustGO.transform.localScale = this.m_RewardZones[zone].DustGameObject.transform.localScale;
    ArcaneDustReward dustReward = dustGO.GetComponent<ArcaneDustReward>();
    yield return (object) null;
    dustReward.SetData((RewardData) data, true);
    dustReward.m_showBanner = false;
    dustReward.m_playSounds = false;
    dustReward.Show(true);
    yield return (object) new WaitForSeconds(this.m_FirewarksRewardHold);
    dustReward.HideWithFX();
    yield return (object) new WaitForSeconds(3f);
    if ((UnityEngine.Object) dustGO != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) dustGO);
  }

  private Vector3 ZoneRandomLocalPosition(int zone)
  {
    Bounds bounds = this.m_RewardZones[zone].Collider.bounds;
    return new Vector3(UnityEngine.Random.Range(-bounds.extents.x, bounds.extents.x), 0.0f, UnityEngine.Random.Range(-bounds.extents.z, bounds.extents.z));
  }

  private IEnumerator ShowCards(int wins)
  {
    HeroicBrawlRewardDisplay brawlRewardDisplay = this;
    if (brawlRewardDisplay.m_RewardsReceived.CardsCount == 0)
    {
      brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.ShowFinalRewards(wins));
    }
    else
    {
      int cardNum = 0;
      for (int i = 0; i < brawlRewardDisplay.m_Rewards.Count; ++i)
      {
        if (brawlRewardDisplay.m_Rewards[i].RewardType == Reward.Type.CARD)
        {
          GameObject cardRoot = brawlRewardDisplay.m_CardVisuals[brawlRewardDisplay.m_RewardsReceived.CardsCount - 1].m_Cards[cardNum];
          if ((UnityEngine.Object) cardRoot == (UnityEngine.Object) null)
          {
            Debug.LogWarningFormat("ShowCards() m_CardVisuals[{0}].m_Cards[{1}] is null!", (object) brawlRewardDisplay.m_RewardsReceived.CardsCount, (object) cardNum);
          }
          else
          {
            Vector3 position = cardRoot.transform.position;
            yield return (object) new WaitForSeconds(brawlRewardDisplay.DisplayFirework(brawlRewardDisplay.m_CardFireworkFSM, position));
            cardRoot.SetActive(true);
            yield return (object) null;
            cardRoot.GetComponentInChildren<CardBurstLegendary>().Activate();
            yield return (object) new WaitForSeconds(brawlRewardDisplay.m_CardRewardBurstDelay);
            CardReward componentInChildren = cardRoot.GetComponentInChildren<CardReward>();
            componentInChildren.SetData(brawlRewardDisplay.m_Rewards[i], true);
            componentInChildren.m_showBanner = false;
            componentInChildren.m_showCardCount = false;
            componentInChildren.m_RotateIn = false;
            componentInChildren.Show(false);
            yield return (object) new WaitForSeconds(brawlRewardDisplay.m_CardRewardDelay);
            ++cardNum;
            cardRoot = (GameObject) null;
          }
        }
      }
      if (brawlRewardDisplay.m_RewardsReceived.CardsCount >= 3)
      {
        for (int index = 0; index < brawlRewardDisplay.m_CardVisuals[brawlRewardDisplay.m_RewardsReceived.CardsCount - 1].m_Cards.Length; ++index)
        {
          UberFloaty componentInChildren = brawlRewardDisplay.m_CardVisuals[brawlRewardDisplay.m_RewardsReceived.CardsCount - 1].m_Cards[index].GetComponentInChildren<UberFloaty>();
          if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
            componentInChildren.enabled = false;
          Hashtable args1 = iTween.Hash((object) "position", (object) brawlRewardDisplay.m_CardVisuals[brawlRewardDisplay.m_RewardsReceived.CardsCount - 1].m_CardTargets[index].transform.position, (object) "time", (object) brawlRewardDisplay.m_CardAnimationTime, (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "islocal", (object) false);
          iTween.MoveTo(brawlRewardDisplay.m_CardVisuals[brawlRewardDisplay.m_RewardsReceived.CardsCount - 1].m_Cards[index], args1);
          Hashtable args2 = iTween.Hash((object) "rotation", (object) brawlRewardDisplay.m_CardVisuals[brawlRewardDisplay.m_RewardsReceived.CardsCount - 1].m_CardTargets[index].transform.localEulerAngles, (object) "time", (object) brawlRewardDisplay.m_CardAnimationTime, (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "islocal", (object) true);
          iTween.RotateTo(brawlRewardDisplay.m_CardVisuals[brawlRewardDisplay.m_RewardsReceived.CardsCount - 1].m_Cards[index], args2);
        }
      }
      yield return (object) new WaitForSeconds(brawlRewardDisplay.m_CardAnimationTime * 0.5f);
      brawlRewardDisplay.StartCoroutine(brawlRewardDisplay.ShowFinalRewards(wins));
      if (brawlRewardDisplay.m_RewardsReceived.CardsCount >= 3)
      {
        yield return (object) new WaitForSeconds(brawlRewardDisplay.m_CardAnimationTime * 0.5f);
        for (int index = 0; index < brawlRewardDisplay.m_CardVisuals[brawlRewardDisplay.m_RewardsReceived.CardsCount - 1].m_Cards.Length; ++index)
        {
          UberFloaty componentInChildren = brawlRewardDisplay.m_CardVisuals[brawlRewardDisplay.m_RewardsReceived.CardsCount - 1].m_Cards[index].GetComponentInChildren<UberFloaty>();
          if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
            componentInChildren.enabled = true;
        }
      }
      PlayMakerFSM component = brawlRewardDisplay.m_CardsRoot.GetComponent<PlayMakerFSM>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.SendEvent("Birth");
    }
  }

  private void InitRewardsReceived()
  {
    this.m_RewardsReceived.PackID = 1;
    this.m_RewardsReceived.PackCount = 0;
    this.m_RewardsReceived.DustCount = 0;
    this.m_RewardsReceived.GoldCount = 0;
    this.m_RewardsReceived.CardsCount = 0;
    this.m_RewardsReceived.Cards = new List<HeroicBrawlRewardDisplay.RewardCardReceived>();
    for (int index = 0; index < this.m_finalRewards.Count; ++index)
    {
      Reward finalReward = this.m_finalRewards[index];
      switch (finalReward.RewardType)
      {
        case Reward.Type.ARCANE_DUST:
          this.m_RewardsReceived.DustCount = ((ArcaneDustRewardData) finalReward.Data).Amount;
          break;
        case Reward.Type.BOOSTER_PACK:
          BoosterPackRewardData data1 = (BoosterPackRewardData) finalReward.Data;
          this.m_RewardsReceived.PackID = data1.Id;
          this.m_RewardsReceived.PackCount = data1.Count;
          break;
        case Reward.Type.CARD:
          CardRewardData data2 = (CardRewardData) finalReward.Data;
          HeroicBrawlRewardDisplay.RewardCardReceived rewardCardReceived = new HeroicBrawlRewardDisplay.RewardCardReceived();
          rewardCardReceived.CardID = data2.CardID;
          rewardCardReceived.Premium = data2.Premium;
          EntityDef entityDef = DefLoader.Get().GetEntityDef(rewardCardReceived.CardID);
          if (entityDef == null)
          {
            Debug.LogWarningFormat("InitRewardsReceived() - entityDef for Card ID {0} is null", (object) rewardCardReceived.CardID);
            return;
          }
          rewardCardReceived.CardEntityDef = entityDef;
          this.m_RewardsReceived.Cards.Add(rewardCardReceived);
          ++this.m_RewardsReceived.CardsCount;
          break;
        case Reward.Type.GOLD:
          this.m_RewardsReceived.GoldCount = (int) ((GoldRewardData) finalReward.Data).Amount;
          break;
      }
    }
  }

  private int NextRewardZone()
  {
    int num = UnityEngine.Random.Range(0, this.m_RewardZones.Length);
    if (num == this.m_lastZone)
    {
      num = UnityEngine.Random.Range(0, this.m_RewardZones.Length);
      if (num == this.m_lastZone)
      {
        ++num;
        if (num >= this.m_RewardZones.Length)
          num = 0;
      }
    }
    this.m_lastZone = num;
    return num;
  }

  private IEnumerator ShowFinalRewards(int wins, bool simpleRewards = false)
  {
    while (!this.IsFinalRewardsLoaded())
      yield return (object) null;
    this.m_FSM.SendEvent("HideBox");
    this.m_FinalRewardsRoot.SetActive(true);
    string str;
    if (wins == 0)
      str = GameStrings.Get("GLUE_HEROIC_BRAWL_NO_WINS_REWARD_PACK_TEXT");
    else
      str = GameStrings.Format("GLUE_HEROIC_BRAWL_REWARDS_WIN_BANNER_TEXT", (object) wins, (object) wins);
    this.m_BannerUberText.Text = str;
    HeroicBrawlRewardDisplay.RewardVisuals rewardVisual = this.m_RewardVisuals[wins];
    for (int index = 0; index < this.m_finalRewards.Count; ++index)
    {
      Reward finalReward = this.m_finalRewards[index];
      finalReward.m_playSounds = false;
      finalReward.m_showBanner = false;
      Transform transform = (Transform) null;
      switch (finalReward.RewardType)
      {
        case Reward.Type.ARCANE_DUST:
          finalReward.transform.parent = rewardVisual.m_FinalDustBone;
          finalReward.Show(false);
          transform = rewardVisual.m_FinalDustBone;
          break;
        case Reward.Type.BOOSTER_PACK:
          finalReward.transform.parent = rewardVisual.m_FinalPacksBone;
          finalReward.Show(false);
          transform = rewardVisual.m_FinalPacksBone;
          break;
        case Reward.Type.GOLD:
          finalReward.transform.parent = rewardVisual.m_FinalGoldBone;
          finalReward.Show(false);
          transform = rewardVisual.m_FinalGoldBone;
          break;
      }
      if (simpleRewards && (UnityEngine.Object) transform != (UnityEngine.Object) null)
      {
        PlayMakerFSM component = transform.GetComponent<PlayMakerFSM>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.SendEvent("Birth");
      }
      if (!simpleRewards)
      {
        PlayMakerFSM component = this.m_FinalRewardsRoot.GetComponent<PlayMakerFSM>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.SendEvent("Birth");
      }
      finalReward.transform.localPosition = Vector3.zero;
      finalReward.transform.localRotation = Quaternion.identity;
      finalReward.transform.localScale = Vector3.one;
    }
    this.AllDone();
  }

  private void LoadFinalRewards()
  {
    this.m_finalRewardsLoadedCount = 0;
    for (int index = 0; index < this.m_Rewards.Count; ++index)
      this.m_Rewards[index].LoadRewardObject(new Reward.DelOnRewardLoaded(this.FinalRewardLoaded));
  }

  private bool IsFinalRewardsLoaded() => this.m_finalRewardsLoadedCount >= this.m_Rewards.Count;

  private void FinalRewardLoaded(Reward reward, object callbackData)
  {
    ++this.m_finalRewardsLoadedCount;
    if ((UnityEngine.Object) reward == (UnityEngine.Object) null)
      Debug.LogWarningFormat("HeroicBrawlRewardDisplay.FinalRewardLoaded() - FAILED to load reward");
    else if ((UnityEngine.Object) reward.gameObject == (UnityEngine.Object) null)
    {
      Debug.LogWarningFormat("HeroicBrawlRewardDisplay.FinalRewardLoaded() - reward GameObject is null");
    }
    else
    {
      reward.gameObject.layer = this.gameObject.layer;
      this.m_finalRewards.Add(reward);
    }
  }

  private void AllDone()
  {
    this.m_DoneButton.gameObject.SetActive(true);
    Spell component = this.m_DoneButton.m_button.GetComponent<Spell>();
    component.AddFinishedCallback(new Spell.FinishedCallback(this.OnDoneButtonShown));
    component.ActivateState(SpellStateType.BIRTH);
  }

  private void OnDoneButtonShown(Spell spell, object userData) => this.m_DoneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));

  private void OnDoneButtonPressed(UIEvent e)
  {
    this.m_DoneButton.m_button.GetComponent<Spell>().ActivateState(SpellStateType.DEATH);
    iTween.ScaleTo(this.m_Root, Vector3.zero, this.m_EndScaleAwayTime);
    this.m_screenEffectsHandle.StopEffect();
    if (this.m_fromNotice)
      Network.Get().AckNotice(this.m_noticeID);
    foreach (Action doneCallback in this.m_doneCallbacks)
    {
      if (doneCallback != null)
        doneCallback();
    }
    this.StartCoroutine(this.OnDone());
  }

  private IEnumerator OnDone()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    HeroicBrawlRewardDisplay brawlRewardDisplay = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      UnityEngine.Object.Destroy((UnityEngine.Object) brawlRewardDisplay.gameObject);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(brawlRewardDisplay.m_EndScaleAwayTime);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void LoadRewardCards()
  {
    for (int index = 0; index < this.m_RewardsReceived.Cards.Count; ++index)
    {
      HeroicBrawlRewardDisplay.RewardCardReceived card = this.m_RewardsReceived.Cards[index];
      string handActor = ActorNames.GetHandActor(card.CardEntityDef);
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) handActor, AssetLoadingOptions.IgnorePrefabPosition);
      this.m_RewardsReceived.Cards[index] = new HeroicBrawlRewardDisplay.RewardCardReceived()
      {
        CardGameObject = gameObject,
        CardID = card.CardID,
        CardEntityDef = card.CardEntityDef,
        Premium = card.Premium
      };
    }
  }

  private void LoadBoosterReward()
  {
    string assetRef = "BoosterPackReward.prefab:b3f2b69bf55efe2419ca6d55c46f7fa7";
    for (int index = 0; index < this.m_RewardZones.Length; ++index)
    {
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, AssetLoadingOptions.IgnorePrefabPosition);
      gameObject.transform.parent = this.m_RewardZones[index].PackGameObject.transform;
      gameObject.transform.localPosition = Vector3.zero;
      gameObject.transform.localRotation = Quaternion.identity;
      gameObject.transform.localScale = Vector3.one;
      this.m_RewardZones[index].packReward = this.m_RewardZones[index].PackGameObject.GetComponentInChildren<BoosterPackReward>();
    }
  }

  private List<RewardData> DebugRewards(int wins)
  {
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    int num4 = 0;
    switch (wins)
    {
      case 0:
        num1 = 1;
        break;
      case 1:
        num1 = 2;
        break;
      case 2:
        num1 = 3;
        break;
      case 3:
        num1 = 4;
        num2 = 200;
        num3 = 200;
        break;
      case 4:
        num1 = 5;
        num2 = 350;
        num3 = 350;
        break;
      case 5:
        num1 = 6;
        num2 = 400;
        num3 = 400;
        break;
      case 6:
        num1 = 7;
        num2 = 450;
        num3 = 450;
        break;
      case 7:
        num1 = 8;
        num2 = 500;
        num3 = 500;
        break;
      case 8:
        num1 = 9;
        num2 = 550;
        num3 = 550;
        break;
      case 9:
        num1 = 14;
        num2 = 800;
        num3 = 800;
        break;
      case 10:
        num1 = 15;
        num2 = 950;
        num3 = 950;
        num4 = 1;
        break;
      case 11:
        num1 = 20;
        num2 = 1000;
        num3 = 1000;
        num4 = 2;
        break;
      case 12:
        num1 = 50;
        num2 = 2300;
        num3 = 2300;
        num4 = 3;
        break;
    }
    List<RewardData> rewardDataList = new List<RewardData>();
    if (num1 > 0)
    {
      BoosterPackRewardData boosterPackRewardData = new BoosterPackRewardData();
      boosterPackRewardData.Count = num1;
      boosterPackRewardData.Id = 11;
      boosterPackRewardData.MarkAsDummyReward();
      rewardDataList.Add((RewardData) boosterPackRewardData);
    }
    if (num3 > 0)
    {
      GoldRewardData goldRewardData = new GoldRewardData();
      goldRewardData.Amount = (long) num3;
      goldRewardData.MarkAsDummyReward();
      rewardDataList.Add((RewardData) goldRewardData);
    }
    if (num2 > 0)
    {
      ArcaneDustRewardData arcaneDustRewardData = new ArcaneDustRewardData();
      arcaneDustRewardData.Amount = num2;
      arcaneDustRewardData.MarkAsDummyReward();
      rewardDataList.Add((RewardData) arcaneDustRewardData);
    }
    if (num4 > 0)
    {
      string[] strArray = new string[3]
      {
        "NEW1_030",
        "NEW1_030",
        "NEW1_030"
      };
      for (int index = 0; index < num4; ++index)
      {
        CardRewardData cardRewardData = new CardRewardData();
        cardRewardData.CardID = strArray[index];
        cardRewardData.Premium = TAG_PREMIUM.GOLDEN;
        cardRewardData.MarkAsDummyReward();
        rewardDataList.Add((RewardData) cardRewardData);
      }
    }
    return rewardDataList;
  }

  [Serializable]
  public class RewardVisuals
  {
    public bool DropBox;
    public bool ShatterDialog;
    public int DustPerBottleMin = 50;
    public int DustPerBottleMax = 100;
    public int GoldPerBagMin = 50;
    public int GoldPerBagMax = 100;
    public Transform m_FinalPacksBone;
    public Transform m_FinalGoldBone;
    public Transform m_FinalDustBone;
  }

  [Serializable]
  public struct FireworkRewardZone
  {
    public GameObject ZoneRoot;
    public BoxCollider Collider;
    public GameObject PackGameObject;
    public GameObject GoldGameObject;
    public GameObject DustGameObject;
    public BoosterPackReward packReward;
    public ArcaneDustReward dustReward;
    public GoldReward goldReward;
  }

  [Serializable]
  public struct CardVisuals
  {
    public GameObject[] m_Cards;
    public GameObject[] m_CardTargets;
  }

  public struct RewardCardReceived
  {
    public string CardID;
    public TAG_PREMIUM Premium;
    public EntityDef CardEntityDef;
    public GameObject CardGameObject;
  }

  public struct RewardsReceivedData
  {
    public int PackID;
    public int PackCount;
    public int GoldCount;
    public int DustCount;
    public int CardsCount;
    public List<HeroicBrawlRewardDisplay.RewardCardReceived> Cards;
  }
}
