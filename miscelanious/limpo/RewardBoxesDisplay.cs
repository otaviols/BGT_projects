using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class RewardBoxesDisplay : MonoBehaviour
{
  public bool m_playBoxFlyoutSound = true;
  public GameObject m_Root;
  public GameObject m_ClickCatcher;
  [CustomEditField(Sections = "Reward Panel")]
  public NormalButton m_DoneButton;
  public NormalButton m_BonusLootNextButton;
  public RewardBoxesDisplay.RewardSet m_RewardSet;
  private List<Action> m_doneCallbacks;
  private List<GameObject> m_InstancedObjects;
  private GameObject[] m_RewardObjects;
  private List<RewardBoxesDisplay.RewardPackageData> m_RewardPackages;
  private GameLayer m_layer = GameLayer.IgnoreFullScreenEffects;
  private bool m_useDarkeningClickCatcher;
  private bool m_doneButtonFinishedShown;
  private bool m_destroyed;
  private List<RewardData> m_rewards;
  private List<RewardData> m_bonusRewards;
  private int m_currentPageNum;
  private int m_lastPageNum;
  private int m_numRegularRewardPages;
  private bool m_showingBonusRewards;
  private List<GameObject> m_rewardPackageInstances = new List<GameObject>();
  private bool m_hasFadedFullScreenEffectsOut;
  protected const string DEFAULT_PREFAB = "RewardBoxes.prefab:f136fead3d6a148c6801f1e3bd2e8267";
  protected const string MERCENARY_PREFAB = "RewardBoxes_Mercenary.prefab:3c55d213147b7bb4fbcf50b9145857eb";
  private static RewardBoxesDisplay s_Instance;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private bool IsOnLastPage => this.m_currentPageNum >= this.m_lastPageNum;

  private bool IsOnLastRegularRewardPage => this.m_currentPageNum == this.m_numRegularRewardPages - 1;

  private void Awake()
  {
    RewardBoxesDisplay.s_Instance = this;
    this.m_InstancedObjects = new List<GameObject>();
    this.m_doneCallbacks = new List<Action>();
    RenderUtils.SetAlpha(this.m_ClickCatcher, 0.0f);
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void Start()
  {
    if ((UnityEngine.Object) this.m_RewardSet.m_RewardPackage != (UnityEngine.Object) null)
      this.m_RewardSet.m_RewardPackage.SetActive(false);
    if (!((UnityEngine.Object) this.m_RewardSet.m_BonusRewardPackage != (UnityEngine.Object) null))
      return;
    this.m_RewardSet.m_BonusRewardPackage.SetActive(false);
  }

  private void OnDestroy()
  {
    this.CleanUp();
    this.m_destroyed = true;
  }

  public static RewardBoxesDisplay Get() => RewardBoxesDisplay.s_Instance;

  public static string GetPrefab(List<RewardData> rewards)
  {
    if (rewards != null)
    {
      foreach (RewardData reward in rewards)
      {
        switch (reward.RewardType)
        {
          case Reward.Type.MERCENARY_COIN:
          case Reward.Type.MERCENARY_EXP:
          case Reward.Type.MERCENARY_ABILITY_UNLOCK:
          case Reward.Type.MERCENARY_EQUIPMENT:
          case Reward.Type.MERCENARY_BOOSTER:
          case Reward.Type.MERCENARY_MERCENARY:
          case Reward.Type.MERCENARY_RANDOM_MERCENARY:
          case Reward.Type.MERCENARY_KNOCKOUT:
          case Reward.Type.MERCENARY_RENOWN:
            return "RewardBoxes_Mercenary.prefab:3c55d213147b7bb4fbcf50b9145857eb";
          default:
            continue;
        }
      }
    }
    return "RewardBoxes.prefab:f136fead3d6a148c6801f1e3bd2e8267";
  }

  public void SetRewards(List<RewardData> rewards, List<RewardData> bonusRewards = null)
  {
    this.m_rewards = rewards;
    int result1;
    int num1 = Math.DivRem(this.m_rewards.Count, this.m_RewardSet.m_MaxPackagesPerPage, out result1);
    if (result1 > 0)
      ++num1;
    this.m_numRegularRewardPages = num1;
    int num2 = 0;
    if (bonusRewards != null)
    {
      this.m_bonusRewards = bonusRewards;
      int result2 = 0;
      num2 = Math.DivRem(this.m_bonusRewards.Count, this.m_RewardSet.m_MaxPackagesPerPage, out result2);
      if (result2 > 0)
        ++num2;
    }
    this.m_lastPageNum = num1 + num2 - 1;
  }

  public void UseDarkeningClickCatcher(bool value)
  {
    this.m_useDarkeningClickCatcher = value;
    this.m_ClickCatcher.layer = 0;
  }

  public void RegisterDoneCallback(Action action) => this.m_doneCallbacks.Add(action);

  public List<RewardBoxesDisplay.RewardPackageData> GetPackageData(
    int rewardCount)
  {
    for (int index = 0; index < this.m_RewardSet.m_RewardData.Count; ++index)
    {
      if (this.m_RewardSet.m_RewardData[index].m_PackageData.Count == rewardCount)
        return this.m_RewardSet.m_RewardData[index].m_PackageData;
    }
    Debug.LogError((object) ("RewardBoxesDisplay: GetPackageData - no package data found with a reward count of " + (object) rewardCount));
    return (List<RewardBoxesDisplay.RewardPackageData>) null;
  }

  public void SetLayer(GameLayer layer)
  {
    this.m_layer = layer;
    LayerUtils.SetLayer(this.gameObject, this.m_layer);
  }

  public void ShowAlreadyOpenedRewards()
  {
    List<RewardData> currentPageRewards = this.CurrentPageRewards;
    this.m_RewardPackages = this.GetPackageData(currentPageRewards.Count);
    this.m_RewardObjects = new GameObject[currentPageRewards.Count];
    this.FadeFullscreenEffectsIn();
    this.ShowOpenedRewards(currentPageRewards);
    this.AllDone();
  }

  public void ShowOpenedRewards(List<RewardData> rewardData)
  {
    for (int index = 0; index < this.m_RewardPackages.Count; ++index)
    {
      RewardBoxesDisplay.RewardPackageData rewardPackage = this.m_RewardPackages[index];
      if ((UnityEngine.Object) rewardPackage.m_TargetBone == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "RewardBoxesDisplay: AnimateRewards package target bone is null!");
        break;
      }
      if (index >= this.m_RewardObjects.Length || index >= rewardData.Count)
      {
        Debug.LogWarning((object) "RewardBoxesDisplay: AnimateRewards reward index exceeded!");
        break;
      }
      this.m_RewardObjects[index] = this.CreateRewardInstance(rewardData[index], index, rewardPackage.m_TargetBone.position, true);
    }
  }

  public void AnimateRewards()
  {
    List<RewardData> currentPageRewards = this.CurrentPageRewards;
    int count = currentPageRewards.Count;
    this.m_RewardPackages = this.GetPackageData(count);
    this.m_RewardObjects = new GameObject[count];
    for (int index = 0; index < this.m_RewardPackages.Count; ++index)
    {
      RewardBoxesDisplay.RewardPackageData rewardPackage = this.m_RewardPackages[index];
      if ((UnityEngine.Object) rewardPackage.m_TargetBone == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "RewardBoxesDisplay: AnimateRewards package target bone is null!");
        return;
      }
      if (index >= this.m_RewardObjects.Length || index >= count)
      {
        Debug.LogWarning((object) "RewardBoxesDisplay: AnimateRewards reward index exceeded!");
        return;
      }
      this.m_RewardObjects[index] = this.CreateRewardInstance(currentPageRewards[index], index, rewardPackage.m_TargetBone.position, false);
    }
    this.RewardPackageAnimation();
  }

  public void OpenReward(int rewardIndex, Vector3 rewardPos)
  {
    if (rewardIndex >= this.m_RewardObjects.Length)
    {
      Debug.LogWarning((object) "RewardBoxesDisplay: OpenReward reward index exceeded!");
    }
    else
    {
      GameObject rewardObject = this.m_RewardObjects[rewardIndex];
      if ((UnityEngine.Object) rewardObject == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "RewardBoxesDisplay: OpenReward object is null!");
      }
      else
      {
        if (!rewardObject.activeSelf)
          rewardObject.SetActive(true);
        if (!this.CheckAllRewardsActive())
          return;
        this.AllDone();
      }
    }
  }

  private List<RewardData> CurrentPageRewards
  {
    get
    {
      List<RewardData> list = this.m_rewards.Skip<RewardData>(this.m_RewardSet.m_MaxPackagesPerPage * this.m_currentPageNum).Take<RewardData>(this.m_RewardSet.m_MaxPackagesPerPage).ToList<RewardData>();
      this.m_showingBonusRewards = false;
      if (list.Count == 0 && this.m_bonusRewards != null)
      {
        list = this.m_bonusRewards.Skip<RewardData>(this.m_RewardSet.m_MaxPackagesPerPage * (this.m_currentPageNum - this.m_numRegularRewardPages)).Take<RewardData>(this.m_RewardSet.m_MaxPackagesPerPage).ToList<RewardData>();
        this.m_showingBonusRewards = true;
      }
      return list;
    }
  }

  private void RewardPackageAnimation()
  {
    if (this.m_showingBonusRewards && (UnityEngine.Object) this.m_RewardSet.m_RewardPackage == (UnityEngine.Object) null)
      Debug.LogWarning((object) "RewardBoxesDisplay: missing Bonus Reward Package!");
    else if ((UnityEngine.Object) this.m_RewardSet.m_RewardPackage == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "RewardBoxesDisplay: missing Reward Package!");
    }
    else
    {
      if (this.m_currentPageNum == 0)
        this.FadeFullscreenEffectsIn();
      foreach (GameObject rewardPackageInstance in this.m_rewardPackageInstances)
      {
        if ((UnityEngine.Object) rewardPackageInstance != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) rewardPackageInstance);
      }
      this.m_rewardPackageInstances.Clear();
      for (int index = 0; index < this.m_RewardPackages.Count; ++index)
      {
        RewardBoxesDisplay.RewardPackageData rewardPackage = this.m_RewardPackages[index];
        if ((UnityEngine.Object) rewardPackage.m_TargetBone == (UnityEngine.Object) null || (UnityEngine.Object) rewardPackage.m_StartBone == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) "RewardBoxesDisplay: missing reward target bone!");
        }
        else
        {
          GameObject gameObject = !this.m_showingBonusRewards || !((UnityEngine.Object) this.m_RewardSet.m_BonusRewardPackage != (UnityEngine.Object) null) ? UnityEngine.Object.Instantiate<GameObject>(this.m_RewardSet.m_RewardPackage) : UnityEngine.Object.Instantiate<GameObject>(this.m_RewardSet.m_BonusRewardPackage);
          TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, this.m_Root.transform);
          gameObject.transform.position = rewardPackage.m_StartBone.position;
          gameObject.SetActive(true);
          this.m_InstancedObjects.Add(gameObject);
          this.m_rewardPackageInstances.Add(gameObject);
          Vector3 localScale = gameObject.transform.localScale;
          gameObject.transform.localScale = Vector3.zero;
          RenderUtils.EnableColliders(gameObject, false);
          iTween.ScaleTo(gameObject, iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) this.m_RewardSet.m_AnimationTime, (object) "delay", (object) rewardPackage.m_StartDelay, (object) "easetype", (object) iTween.EaseType.linear));
          PlayMakerFSM component1 = gameObject.GetComponent<PlayMakerFSM>();
          if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
          {
            Debug.LogWarning((object) "RewardBoxesDisplay: missing reward Playmaker FSM!");
          }
          else
          {
            if (!this.m_playBoxFlyoutSound)
              component1.FsmVariables.FindFsmBool("PlayFlyoutSound").Value = false;
            RewardPackage component2 = gameObject.GetComponent<RewardPackage>();
            component2.m_RewardIndex = index;
            RewardBoxesDisplay.RewardBoxData rewardBoxData = new RewardBoxesDisplay.RewardBoxData();
            rewardBoxData.m_GameObject = gameObject;
            rewardBoxData.m_RewardPackage = component2;
            rewardBoxData.m_FSM = component1;
            rewardBoxData.m_Index = index;
            iTween.MoveTo(gameObject, iTween.Hash((object) "position", (object) rewardPackage.m_TargetBone.transform.position, (object) "time", (object) this.m_RewardSet.m_AnimationTime, (object) "delay", (object) rewardPackage.m_StartDelay, (object) "easetype", (object) iTween.EaseType.linear, (object) "onstarttarget", (object) this.gameObject, (object) "onstart", (object) "RewardPackageOnStart", (object) "onstartparams", (object) rewardBoxData, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "RewardPackageOnComplete", (object) "oncompleteparams", (object) rewardBoxData));
          }
        }
      }
    }
  }

  private void RewardPackageOnStart(RewardBoxesDisplay.RewardBoxData boxData) => boxData.m_FSM.SendEvent("Birth");

  private void RewardPackageOnComplete(RewardBoxesDisplay.RewardBoxData boxData) => this.StartCoroutine(this.RewardPackageActivate(boxData));

  private IEnumerator RewardPackageActivate(RewardBoxesDisplay.RewardBoxData boxData)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    RewardBoxesDisplay rewardBoxesDisplay = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      RenderUtils.EnableColliders(boxData.m_GameObject, true);
      boxData.m_RewardPackage.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(rewardBoxesDisplay.RewardPackagePressed));
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(0.5f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private void RewardPackagePressed(UIEvent e) => Log.RewardBox.Print("box clicked!");

  private GameObject CreateRewardInstance(
    RewardData reward,
    int rewardIndex,
    Vector3 rewardPos,
    bool activeOnStart)
  {
    GameObject go = (GameObject) null;
    switch (reward.RewardType)
    {
      case Reward.Type.ARCANE_DUST:
        go = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardSet.m_RewardDust);
        TransformUtil.AttachAndPreserveLocalTransform(go.transform, this.m_Root.transform);
        go.transform.position = rewardPos;
        go.SetActive(true);
        go.GetComponentInChildren<UberText>().Text = ((ArcaneDustRewardData) reward).Amount.ToString();
        go.SetActive(activeOnStart);
        break;
      case Reward.Type.BOOSTER_PACK:
        BoosterPackRewardData boosterPackRewardData = reward as BoosterPackRewardData;
        int id = boosterPackRewardData.Id;
        if (id == 0)
        {
          id = 1;
          Debug.LogWarning((object) "RewardBoxesDisplay - booster reward is not valid. ID = 0");
        }
        Log.RewardBox.Print(string.Format("Booster DB ID: {0}", (object) id));
        string arenaPrefab = GameDbf.Booster.GetRecord(id).ArenaPrefab;
        if (string.IsNullOrEmpty(arenaPrefab))
        {
          Debug.LogError((object) string.Format("RewardBoxesDisplay - no prefab found for booster {0}!", (object) boosterPackRewardData.Id));
          break;
        }
        go = AssetLoader.Get().InstantiatePrefab((AssetReference) arenaPrefab);
        if (boosterPackRewardData.Count > 1)
        {
          UberText componentInChildren = go.GetComponentInChildren<UberText>(true);
          if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
          {
            Debug.LogError((object) string.Format("RewardBoxesDisplay - no uber text found for booster {0}!", (object) boosterPackRewardData.Id));
            break;
          }
          componentInChildren.transform.parent.gameObject.SetActive(true);
          componentInChildren.Text = boosterPackRewardData.Count.ToString();
        }
        TransformUtil.AttachAndPreserveLocalTransform(go.transform, this.m_Root.transform);
        go.transform.position = rewardPos;
        go.SetActive(activeOnStart);
        break;
      case Reward.Type.CARD:
        go = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardSet.m_RewardCard);
        TransformUtil.AttachAndPreserveLocalTransform(go.transform, this.m_Root.transform);
        go.transform.position = rewardPos;
        go.SetActive(true);
        CardRewardData cardData = (CardRewardData) reward;
        go.GetComponentInChildren<RewardCard>().LoadCard(cardData, this.m_layer);
        go.SetActive(activeOnStart);
        break;
      case Reward.Type.CARD_BACK:
        go = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardSet.m_RewardCardBack);
        TransformUtil.AttachAndPreserveLocalTransform(go.transform, this.m_Root.transform);
        go.transform.position = rewardPos;
        go.SetActive(true);
        CardBackRewardData cardbackData = (CardBackRewardData) reward;
        go.GetComponentInChildren<RewardCardBack>().LoadCardBack(cardbackData, this.m_layer);
        go.SetActive(activeOnStart);
        break;
      case Reward.Type.GOLD:
        go = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardSet.m_RewardGold);
        TransformUtil.AttachAndPreserveLocalTransform(go.transform, this.m_Root.transform);
        go.transform.position = rewardPos;
        go.SetActive(true);
        go.GetComponentInChildren<UberText>().Text = ((GoldRewardData) reward).Amount.ToString();
        go.SetActive(activeOnStart);
        break;
      case Reward.Type.MERCENARY_COIN:
        go = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardSet.m_RewardMercenaryCoin);
        TransformUtil.AttachAndPreserveLocalTransform(go.transform, this.m_Root.transform);
        go.transform.position = rewardPos;
        go.SetActive(true);
        MercenaryCoinRewardData rewardData1 = (MercenaryCoinRewardData) reward;
        go.GetComponentInChildren<RewardMercenaryCoin>().Initialize(rewardData1);
        go.SetActive(activeOnStart);
        break;
      case Reward.Type.MERCENARY_EXP:
        go = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardSet.m_RewardMercenaryExp);
        TransformUtil.AttachAndPreserveLocalTransform(go.transform, this.m_Root.transform);
        go.transform.position = rewardPos;
        go.SetActive(true);
        MercenaryExpRewardData rewardData2 = (MercenaryExpRewardData) reward;
        go.GetComponentInChildren<RewardMercenaryExp>().Initialize(rewardData2);
        go.SetActive(activeOnStart);
        break;
      case Reward.Type.MERCENARY_RENOWN:
        go = UnityEngine.Object.Instantiate<GameObject>(this.m_RewardSet.m_RewardRenown);
        TransformUtil.AttachAndPreserveLocalTransform(go.transform, this.m_Root.transform);
        go.transform.position = rewardPos;
        go.SetActive(true);
        MercenaryRenownRewardData rewardData3 = (MercenaryRenownRewardData) reward;
        go.GetComponentInChildren<RewardMercenaryRenown>().Initialize(rewardData3);
        go.SetActive(activeOnStart);
        break;
    }
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "RewardBoxesDisplay: Unable to create reward, object null!");
      return (GameObject) null;
    }
    if (rewardIndex >= this.m_RewardObjects.Length)
    {
      Debug.LogWarning((object) "RewardBoxesDisplay: CreateRewardInstance reward index exceeded!");
      return (GameObject) null;
    }
    LayerUtils.SetLayer(go, this.m_layer);
    this.m_RewardObjects[rewardIndex] = go;
    this.m_InstancedObjects.Add(go);
    return go;
  }

  private void AllDone()
  {
    Vector3 zero = Vector3.zero;
    bool flag = this.IsOnLastRegularRewardPage && this.m_bonusRewards != null && this.m_bonusRewards.Count > 0 && (UnityEngine.Object) this.m_BonusLootNextButton != (UnityEngine.Object) null;
    NormalButton normalButton;
    if (flag)
    {
      normalButton = this.m_BonusLootNextButton;
      this.m_DoneButton.gameObject.SetActive(false);
    }
    else
    {
      normalButton = this.m_DoneButton;
      if ((UnityEngine.Object) this.m_BonusLootNextButton != (UnityEngine.Object) null)
        this.m_BonusLootNextButton.gameObject.SetActive(false);
    }
    if (this.m_RewardPackages.Count > 1)
    {
      for (int index = 0; index < this.m_RewardPackages.Count; ++index)
      {
        RewardBoxesDisplay.RewardPackageData rewardPackage = this.m_RewardPackages[index];
        zero += rewardPackage.m_TargetBone.position;
      }
      normalButton.transform.position = zero / (float) this.m_RewardPackages.Count;
    }
    normalButton.gameObject.SetActive(true);
    if (flag)
      normalButton.SetText(GameStrings.Get("GLUE_MERCENARIES_BONUS_LOOT_BUTTON"));
    else if (this.IsOnLastPage)
      normalButton.SetText(GameStrings.Get("GLOBAL_DONE"));
    else
      normalButton.SetText(GameStrings.Get("GLOBAL_BUTTON_NEXT"));
    Spell component = normalButton.m_button.GetComponent<Spell>();
    if (flag)
      component.AddFinishedCallback(new Spell.FinishedCallback(this.OnBonusLootButtonShown));
    else
      component.AddFinishedCallback(new Spell.FinishedCallback(this.OnDoneButtonShown));
    component.ActivateState(SpellStateType.BIRTH);
    if (!this.IsOnLastPage)
      return;
    NarrativeManager.Get().OnArenaRewardsShown();
  }

  private void OnDoneButtonShown(Spell spell, object userData)
  {
    this.m_doneButtonFinishedShown = true;
    RenderUtils.EnableColliders(this.m_DoneButton.gameObject, true);
    this.m_DoneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));
    this.m_DoneButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDoneButtonRollover));
    if (!this.IsOnLastPage)
      return;
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
  }

  private void OnBonusLootButtonShown(Spell spell, object userData)
  {
    this.m_doneButtonFinishedShown = true;
    RenderUtils.EnableColliders(this.m_BonusLootNextButton.gameObject, true);
    this.m_BonusLootNextButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));
    this.m_BonusLootNextButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnBonusLootButtonRollover));
  }

  private void OnDoneButtonRollover(UIEvent e) => this.m_DoneButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Hover_Wiggle");

  private void OnBonusLootButtonRollover(UIEvent e) => this.m_BonusLootNextButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Hover_Wiggle");

  private void OnDoneButtonPressed(UIEvent e)
  {
    if (this.IsOnLastPage)
    {
      this.FadeFullscreenEffectsOut();
      Navigation.GoBack();
    }
    else
    {
      ++this.m_currentPageNum;
      this.KillRewardObjects();
      this.KillDoneButton();
      this.StartCoroutine(this.AnimateRewardsWhenReady());
    }
  }

  private IEnumerator AnimateRewardsWhenReady()
  {
    while (this.CheckAnyRewardActive())
      yield return (object) null;
    this.AnimateRewards();
  }

  public bool IsClosing { get; private set; }

  public void Close()
  {
    this.IsClosing = true;
    if (this.m_doneButtonFinishedShown)
      this.OnNavigateBack();
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private void KillRewardObjects()
  {
    foreach (GameObject rewardObject in this.m_RewardObjects)
    {
      if (!((UnityEngine.Object) rewardObject == (UnityEngine.Object) null))
      {
        PlayMakerFSM component = rewardObject.GetComponent<PlayMakerFSM>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.SendEvent("Death");
        foreach (Component componentsInChild in rewardObject.GetComponentsInChildren<UberText>())
          iTween.FadeTo(componentsInChild.gameObject, iTween.Hash((object) "alpha", (object) 0.0f, (object) "time", (object) 0.8f, (object) "includechildren", (object) true, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
        RewardCard componentInChildren = rewardObject.GetComponentInChildren<RewardCard>();
        if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
          componentInChildren.Death();
        UnityEngine.Object.Destroy((UnityEngine.Object) rewardObject, 0.8f);
      }
    }
  }

  private void KillDoneButton()
  {
    RenderUtils.EnableColliders(this.m_DoneButton.gameObject, false);
    this.m_DoneButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));
    this.m_DoneButton.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDoneButtonRollover));
    this.m_DoneButton.m_button.GetComponent<Spell>().ActivateState(SpellStateType.DEATH);
    if (!((UnityEngine.Object) this.m_BonusLootNextButton != (UnityEngine.Object) null))
      return;
    RenderUtils.EnableColliders(this.m_BonusLootNextButton.gameObject, false);
    this.m_BonusLootNextButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));
    this.m_BonusLootNextButton.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDoneButtonRollover));
    this.m_BonusLootNextButton.m_button.GetComponent<Spell>().ActivateState(SpellStateType.DEATH);
  }

  private bool OnNavigateBack()
  {
    Debug.Log((object) "navigating back!");
    if (!this.m_DoneButton.m_button.activeSelf)
      return false;
    this.KillRewardObjects();
    this.KillDoneButton();
    return true;
  }

  private void FadeFullscreenEffectsIn()
  {
    if (FullScreenFXMgr.Get() == null)
    {
      Debug.LogWarning((object) "RewardBoxesDisplay: FullScreenFXMgr.Get() returned null!");
    }
    else
    {
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignettePerspective with
      {
        Blur = new BlurParameters(brightness: 0.85f)
      });
      if (!this.m_useDarkeningClickCatcher)
        return;
      iTween.FadeTo(this.m_ClickCatcher, 0.75f, 0.5f);
    }
  }

  private void FadeFullscreenEffectsOut()
  {
    if (this.m_hasFadedFullScreenEffectsOut)
      return;
    this.m_hasFadedFullScreenEffectsOut = true;
    if (FullScreenFXMgr.Get() == null)
    {
      Debug.LogWarning((object) "RewardBoxesDisplay: FullScreenFXMgr.Get() returned null!");
    }
    else
    {
      this.m_screenEffectsHandle.StopEffect(2f, iTween.EaseType.easeOutCirc, new Action(this.FadeFullscreenEffectsOutFinished));
      if (!this.m_useDarkeningClickCatcher)
        return;
      iTween.FadeTo(this.m_ClickCatcher, 0.0f, 0.5f);
    }
  }

  private void FadeVignetteIn()
  {
    this.m_screenEffectsHandle.StopEffect();
    ScreenEffectParameters vignettePerspective = ScreenEffectParameters.VignettePerspective with
    {
      Time = 1.5f,
      EaseType = iTween.EaseType.easeOutCirc
    };
    vignettePerspective.Vignette.Amount = 1.4f;
    this.m_screenEffectsHandle.StartEffect(vignettePerspective);
  }

  private void FadeFullscreenEffectsOutFinished()
  {
    foreach (Action doneCallback in this.m_doneCallbacks)
    {
      if (doneCallback != null)
        doneCallback();
    }
    this.m_doneCallbacks.Clear();
    if (this.m_destroyed)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private bool CheckAllRewardsActive()
  {
    foreach (GameObject rewardObject in this.m_RewardObjects)
    {
      if ((UnityEngine.Object) rewardObject == (UnityEngine.Object) null || !rewardObject.activeSelf)
        return false;
    }
    return true;
  }

  private bool CheckAnyRewardActive()
  {
    foreach (UnityEngine.Object rewardObject in this.m_RewardObjects)
    {
      if (rewardObject != (UnityEngine.Object) null)
        return true;
    }
    return false;
  }

  private void CleanUp()
  {
    foreach (GameObject instancedObject in this.m_InstancedObjects)
    {
      if ((UnityEngine.Object) instancedObject != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) instancedObject);
    }
    this.FadeFullscreenEffectsOut();
    RewardBoxesDisplay.s_Instance = (RewardBoxesDisplay) null;
  }

  public void DebugLogRewards()
  {
    Debug.Log((object) "BOX REWARDS:");
    List<RewardData> currentPageRewards = this.CurrentPageRewards;
    for (int index = 0; index < currentPageRewards.Count; ++index)
    {
      RewardData rewardData = currentPageRewards[index];
      Debug.Log((object) string.Format("  reward {0}={1}", (object) index, (object) rewardData));
    }
  }

  [Serializable]
  public class RewardPackageData
  {
    public Transform m_StartBone;
    public Transform m_TargetBone;
    public float m_StartDelay;
  }

  [Serializable]
  public class RewardSet
  {
    public GameObject m_RewardPackage;
    public GameObject m_BonusRewardPackage;
    public float m_AnimationTime = 1f;
    public GameObject m_RewardCard;
    public GameObject m_RewardCardBack;
    public GameObject m_RewardGold;
    public GameObject m_RewardDust;
    public GameObject m_RewardMercenaryCoin;
    public GameObject m_RewardMercenaryExp;
    public GameObject m_RewardRenown;
    public int m_MaxPackagesPerPage;
    public List<RewardBoxesDisplay.BoxRewardData> m_RewardData;
  }

  [Serializable]
  public class BoxRewardData
  {
    public List<RewardBoxesDisplay.RewardPackageData> m_PackageData;
  }

  public class RewardBoxData
  {
    public GameObject m_GameObject;
    public RewardPackage m_RewardPackage;
    public PlayMakerFSM m_FSM;
    public int m_Index;
  }
}
