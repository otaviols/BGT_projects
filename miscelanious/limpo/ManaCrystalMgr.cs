using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class ManaCrystalMgr : MonoBehaviour
{
  public Texture redCrystalTexture;
  [CustomEditField(T = EditType.GAME_OBJECT)]
  public String_MobileOverride manaLockPrefab;
  public ManaCrystalEventSpells m_eventSpells;
  public SlidingTray manaTrayPhone;
  public Transform manaGemBone;
  public GameObject friendlyManaCounter;
  public GameObject opposingManaCounter;
  public List<ManaCrystalAssetPaths> m_ManaCrystalAssetTable = new List<ManaCrystalAssetPaths>();
  public int maxManaCrystalToDisplay = 10;
  private const float SECS_BETW_MANA_SPAWNS = 0.2f;
  private const float SECS_BETW_MANA_READIES = 0.2f;
  private const float SECS_BETW_MANA_SPENDS = 0.2f;
  private const float GEM_FLIP_TEXT_FADE_TIME = 0.1f;
  private readonly string GEM_FLIP_ANIM_NAME = "Resource_Large_phone_Flip";
  private static ManaCrystalMgr s_instance;
  private ManaCrystalType m_manaCrystalType;
  private List<ManaCrystal> m_permanentCrystals;
  private List<ManaCrystal> m_temporaryCrystals;
  private int m_proposedManaSourceEntID = -1;
  private int m_numCrystalsLoading;
  private int m_numQueuedToSpawn;
  private int m_numQueuedToReady;
  private int m_numQueuedToSpend;
  private int m_additionalOverloadedCrystalsOwedNextTurn;
  private int m_additionalOverloadedCrystalsOwedThisTurn;
  private bool m_overloadLocksAreShowing;
  private float m_manaCrystalWidth;
  private GameObject m_friendlyManaGem;
  private UberText m_friendlyManaText;
  private AssetHandle<Texture> m_friendlyManaGemTexture;

  private void Awake()
  {
    ManaCrystalMgr.s_instance = this;
    if (!((UnityEngine.Object) this.gameObject.GetComponent<AudioSource>() == (UnityEngine.Object) null))
      return;
    this.gameObject.AddComponent<AudioSource>();
  }

  private void OnDestroy()
  {
    ManaCrystalMgr.s_instance = (ManaCrystalMgr) null;
    AssetHandle.SafeDispose<Texture>(ref this.m_friendlyManaGemTexture);
  }

  private void Start()
  {
    this.m_permanentCrystals = new List<ManaCrystal>();
    this.m_temporaryCrystals = new List<ManaCrystal>();
    this.InitializePhoneManaGems();
  }

  public static ManaCrystalMgr Get() => ManaCrystalMgr.s_instance;

  public void Reset()
  {
    this.StopAllCoroutines();
    this.DestroyManaCrystals(this.m_permanentCrystals.Count);
    this.DestroyTempManaCrystals(this.m_temporaryCrystals.Count);
    this.UnlockCrystals(this.m_additionalOverloadedCrystalsOwedThisTurn);
    this.ReclaimCrystalsOwedForOverload(this.m_additionalOverloadedCrystalsOwedNextTurn);
    this.m_manaCrystalType = ManaCrystalType.DEFAULT;
  }

  public void ResetUnresolvedManaToBeReadied()
  {
    if (this.m_numQueuedToReady >= 0)
      return;
    this.m_numQueuedToReady = 0;
  }

  public void SetManaCrystalType(ManaCrystalType type)
  {
    this.m_manaCrystalType = type;
    this.InitializePhoneManaGems();
  }

  public Vector3 GetManaCrystalSpawnPosition() => this.transform.position;

  public void AddManaCrystals(int numCrystals, bool isTurnStart)
  {
    for (int index = 0; index < numCrystals; ++index)
    {
      GameState.Get().GetGameEntity().NotifyOfManaCrystalSpawned();
      this.StartCoroutine(this.WaitThenAddManaCrystal(false, isTurnStart));
    }
  }

  public void AddTempManaCrystals(int numCrystals)
  {
    for (int index = 0; index < numCrystals; ++index)
      this.StartCoroutine(this.WaitThenAddManaCrystal(true, false));
  }

  public void DestroyManaCrystals(int numCrystals) => this.StartCoroutine(this.WaitThenDestroyManaCrystals(false, numCrystals));

  public void DestroyTempManaCrystals(int numCrystals) => this.StartCoroutine(this.WaitThenDestroyManaCrystals(true, numCrystals));

  public void UpdateSpentMana(int shownChangeAmount)
  {
    if (shownChangeAmount > 0)
      this.SpendManaCrystals(shownChangeAmount);
    else if (GameState.Get().IsTurnStartManagerActive())
      TurnStartManager.Get().NotifyOfManaCrystalFilled(-shownChangeAmount);
    else
      this.ReadyManaCrystals(-shownChangeAmount);
  }

  public void SpendManaCrystals(int numCrystals)
  {
    ManaCrystalAssetPaths crystalAssetPaths = this.GetManaCrystalAssetPaths(this.m_manaCrystalType);
    SoundManager.Get().LoadAndPlay((AssetReference) crystalAssetPaths.m_SoundOnSpendPath, this.gameObject);
    for (int index = 0; index < numCrystals; ++index)
      this.SpendManaCrystal();
  }

  public void ReadyManaCrystals(int numCrystals)
  {
    for (int index = 0; index < numCrystals; ++index)
      this.ReadyManaCrystal();
  }

  public int GetSpendableManaCrystals()
  {
    int spendableManaCrystals = 0;
    for (int index = 0; index < this.m_temporaryCrystals.Count; ++index)
    {
      if (this.m_temporaryCrystals[index].state == ManaCrystal.State.READY)
        ++spendableManaCrystals;
    }
    for (int index = 0; index < this.m_permanentCrystals.Count; ++index)
    {
      ManaCrystal permanentCrystal = this.m_permanentCrystals[index];
      if (permanentCrystal.state == ManaCrystal.State.READY && !permanentCrystal.IsOverloaded())
        ++spendableManaCrystals;
    }
    return spendableManaCrystals;
  }

  public void CancelAllProposedMana(Entity entity)
  {
    if (entity == null || this.m_proposedManaSourceEntID != entity.GetEntityId())
      return;
    this.m_proposedManaSourceEntID = -1;
    this.m_eventSpells.m_proposeUsageSpell.ActivateState(SpellStateType.DEATH);
    for (int index = 0; index < this.m_temporaryCrystals.Count; ++index)
    {
      if (this.m_temporaryCrystals[index].state == ManaCrystal.State.PROPOSED)
        this.m_temporaryCrystals[index].state = ManaCrystal.State.READY;
    }
    for (int index = this.m_permanentCrystals.Count - 1; index >= 0; --index)
    {
      if (this.m_permanentCrystals[index].state == ManaCrystal.State.PROPOSED)
        this.m_permanentCrystals[index].state = ManaCrystal.State.READY;
    }
  }

  public void ProposeManaCrystalUsage(Entity entity)
  {
    if (entity == null)
      return;
    this.m_proposedManaSourceEntID = entity.GetEntityId();
    int cost = entity.GetCost();
    this.m_eventSpells.m_proposeUsageSpell.ActivateState(SpellStateType.BIRTH);
    int num = 0;
    for (int index = this.m_temporaryCrystals.Count - 1; index >= 0; --index)
    {
      if (this.m_temporaryCrystals[index].state == ManaCrystal.State.USED)
        Log.Gameplay.Print("Found a SPENT temporary mana crystal... this shouldn't happen!");
      else if (num < cost)
      {
        this.m_temporaryCrystals[index].state = ManaCrystal.State.PROPOSED;
        ++num;
      }
      else
        this.m_temporaryCrystals[index].state = ManaCrystal.State.READY;
    }
    for (int index = 0; index < this.m_permanentCrystals.Count; ++index)
    {
      if (this.m_permanentCrystals[index].state != ManaCrystal.State.USED && !this.m_permanentCrystals[index].IsOverloaded())
      {
        if (num < cost)
        {
          this.m_permanentCrystals[index].state = ManaCrystal.State.PROPOSED;
          ++num;
        }
        else
          this.m_permanentCrystals[index].state = ManaCrystal.State.READY;
      }
    }
  }

  public void HandleSameTurnOverloadChanged(int crystalsChanged)
  {
    if (crystalsChanged > 0)
    {
      this.MarkCrystalsOwedForOverload(crystalsChanged);
    }
    else
    {
      if (crystalsChanged >= 0)
        return;
      this.ReclaimCrystalsOwedForOverload(-crystalsChanged);
    }
  }

  public void SetCrystalsLockedForOverload(int numCrystals) => this.StartCoroutine(this.WaitForCrystalsToLoadThenLockThem(numCrystals));

  private IEnumerator WaitForCrystalsToLoadThenLockThem(int numCrystals)
  {
    while (this.m_numCrystalsLoading > 0)
      yield return (object) null;
    for (int index = 0; index < numCrystals; ++index)
    {
      if (index < this.m_permanentCrystals.Count)
        this.m_permanentCrystals[index].PayOverload();
    }
  }

  public void MarkCrystalsOwedForOverload(int numCrystals)
  {
    if (numCrystals > 0)
      this.m_overloadLocksAreShowing = true;
    int num = 0;
    int index = 0;
    while (numCrystals != num)
    {
      if (index == this.m_permanentCrystals.Count)
      {
        this.m_additionalOverloadedCrystalsOwedNextTurn += numCrystals - num;
        break;
      }
      ManaCrystal permanentCrystal = this.m_permanentCrystals[index];
      if (!permanentCrystal.IsOwedForOverload())
      {
        permanentCrystal.MarkAsOwedForOverload();
        ++num;
      }
      ++index;
    }
  }

  public void ReclaimCrystalsOwedForOverload(int numCrystals)
  {
    int num = 0;
    int lastIndex;
    for (lastIndex = this.m_permanentCrystals.FindLastIndex((Predicate<ManaCrystal>) (crystal => crystal.IsOwedForOverload())); num < numCrystals && lastIndex >= 0; ++num)
    {
      this.m_permanentCrystals[lastIndex].ReclaimOverload();
      --lastIndex;
    }
    this.m_additionalOverloadedCrystalsOwedNextTurn -= numCrystals - num;
    this.m_overloadLocksAreShowing = lastIndex >= 0 || this.m_additionalOverloadedCrystalsOwedNextTurn > 0;
  }

  public void UnlockCrystals(int numCrystals)
  {
    int num = 0;
    int lastIndex;
    for (lastIndex = this.m_permanentCrystals.FindLastIndex((Predicate<ManaCrystal>) (crystal => crystal.IsOverloaded())); num < numCrystals && lastIndex >= 0; ++num)
    {
      this.m_permanentCrystals[lastIndex].UnlockOverload();
      --lastIndex;
    }
    this.m_additionalOverloadedCrystalsOwedThisTurn -= numCrystals - num;
    this.m_overloadLocksAreShowing = lastIndex >= 0 || this.m_additionalOverloadedCrystalsOwedThisTurn > 0;
  }

  public void TurnCrystalsRed(int previous, int current)
  {
    for (int index = previous; index < current && index < this.m_permanentCrystals.Count; ++index)
      RendererExtension.GetMaterial(this.m_permanentCrystals[index].gem.gameObject.GetComponent<Renderer>()).mainTexture = this.redCrystalTexture;
  }

  public void OnCurrentPlayerChanged()
  {
    this.m_additionalOverloadedCrystalsOwedThisTurn = this.m_additionalOverloadedCrystalsOwedNextTurn;
    this.m_additionalOverloadedCrystalsOwedNextTurn = 0;
    this.m_overloadLocksAreShowing = this.m_additionalOverloadedCrystalsOwedThisTurn > 0;
    for (int index = 0; index < this.m_permanentCrystals.Count; ++index)
    {
      ManaCrystal permanentCrystal = this.m_permanentCrystals[index];
      if (permanentCrystal.IsOverloaded())
        permanentCrystal.UnlockOverload();
      if (permanentCrystal.IsOwedForOverload())
      {
        this.m_overloadLocksAreShowing = true;
        permanentCrystal.PayOverload();
      }
      else if (this.m_additionalOverloadedCrystalsOwedThisTurn > 0)
      {
        permanentCrystal.PayOverload();
        --this.m_additionalOverloadedCrystalsOwedThisTurn;
      }
    }
  }

  public bool ShouldShowTooltip(ManaCrystalType type) => this.m_manaCrystalType == type;

  public bool ShouldShowOverloadTooltip() => this.m_overloadLocksAreShowing;

  public void SetFriendlyManaGemTexture(AssetHandle<Texture> texture)
  {
    AssetHandle.Set<Texture>(ref this.m_friendlyManaGemTexture, texture);
    this.ApplyFriendlyManaGemTexture();
  }

  public void SetFriendlyManaGemTint(Color tint)
  {
    if ((UnityEngine.Object) this.m_friendlyManaGem == (UnityEngine.Object) null)
      return;
    RendererExtension.GetMaterial((Renderer) this.m_friendlyManaGem.GetComponentInChildren<MeshRenderer>()).SetColor("_TintColor", tint);
  }

  public void ShowPhoneManaTray()
  {
    if ((UnityEngine.Object) this.manaTrayPhone == (UnityEngine.Object) null)
      return;
    Animation component = this.m_friendlyManaGem.GetComponent<Animation>();
    component[this.GEM_FLIP_ANIM_NAME].speed = 1f;
    component.Play(this.GEM_FLIP_ANIM_NAME);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) this.m_friendlyManaText.TextAlpha, (object) "to", (object) 0.0f, (object) "time", (object) 0.1f, (object) "onupdate", (object) (Action<object>) (newVal => this.m_friendlyManaText.TextAlpha = (float) newVal)));
    this.manaTrayPhone.ToggleTraySlider(true);
    CorpseCounter.ShowPhoneManaTray();
  }

  public void HidePhoneManaTray()
  {
    if ((UnityEngine.Object) this.manaTrayPhone == (UnityEngine.Object) null)
      return;
    Animation component = this.m_friendlyManaGem.GetComponent<Animation>();
    component[this.GEM_FLIP_ANIM_NAME].speed = -1f;
    if ((double) component[this.GEM_FLIP_ANIM_NAME].time == 0.0)
      component[this.GEM_FLIP_ANIM_NAME].time = component[this.GEM_FLIP_ANIM_NAME].length;
    component.Play(this.GEM_FLIP_ANIM_NAME);
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) this.m_friendlyManaText.TextAlpha, (object) "to", (object) 1f, (object) "time", (object) 0.1f, (object) "onupdate", (object) (Action<object>) (newVal => this.m_friendlyManaText.TextAlpha = (float) newVal)));
    this.manaTrayPhone.ToggleTraySlider(false);
    CorpseCounter.HidePhoneManaTray();
  }

  public Material GetTemporaryManaCrystalMaterial() => this.m_ManaCrystalAssetTable[(int) this.m_manaCrystalType].m_tempManaCrystalMaterial;

  public Material GetTemporaryManaCrystalProposedQuadMaterial() => this.m_ManaCrystalAssetTable[(int) this.m_manaCrystalType].m_tempManaCrystalProposedQuadMaterial;

  public void SetEnemyManaCounterActive(bool active)
  {
    this.opposingManaCounter.GetComponent<ManaCounter>().enabled = active;
    this.opposingManaCounter.SetActive(active);
  }

  private void UpdateLayout()
  {
    Vector3 position = this.transform.position;
    if ((bool) UniversalInputManager.UsePhoneUI)
      position = this.manaGemBone.transform.position;
    int num = 0;
    for (int index = this.m_permanentCrystals.Count - 1; index >= 0; --index)
    {
      this.m_permanentCrystals[index].Show();
      if (num >= this.maxManaCrystalToDisplay)
      {
        this.m_permanentCrystals[index].Hide();
      }
      else
      {
        this.m_permanentCrystals[index].transform.position = position;
        if ((bool) UniversalInputManager.UsePhoneUI)
          position.z += this.m_manaCrystalWidth;
        else
          position.x += this.m_manaCrystalWidth;
        ++num;
      }
    }
    for (int index = 0; index < this.m_temporaryCrystals.Count; ++index)
    {
      this.m_temporaryCrystals[index].Show();
      if (this.m_permanentCrystals.Count + index >= this.maxManaCrystalToDisplay)
      {
        this.m_temporaryCrystals[index].Hide();
      }
      else
      {
        this.m_temporaryCrystals[index].transform.position = position;
        if ((bool) UniversalInputManager.UsePhoneUI)
          position.z += this.m_manaCrystalWidth;
        else
          position.x += this.m_manaCrystalWidth;
      }
    }
  }

  private IEnumerator UpdatePermanentCrystalStates()
  {
    while (this.m_numQueuedToReady > 0 || this.m_numCrystalsLoading > 0 || this.m_numQueuedToSpend > 0)
      yield return (object) null;
    int tag1 = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.RESOURCES_USED);
    int tag2 = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.OVERLOAD_OWED);
    int index1;
    for (index1 = 0; index1 < tag1 && index1 != this.m_permanentCrystals.Count; ++index1)
    {
      if (this.m_permanentCrystals[index1].state != ManaCrystal.State.USED)
        this.m_permanentCrystals[index1].state = ManaCrystal.State.USED;
    }
    for (int index2 = index1; index2 < this.m_permanentCrystals.Count; ++index2)
    {
      if (this.m_permanentCrystals[index2].state != ManaCrystal.State.READY)
        this.m_permanentCrystals[index2].state = ManaCrystal.State.READY;
    }
    for (int index3 = 0; index3 < Math.Min(this.m_permanentCrystals.Count, tag2); ++index3)
    {
      if (!this.m_permanentCrystals[index3].IsOwedForOverload())
        this.m_permanentCrystals[index3].MarkAsOwedForOverload();
    }
  }

  private void LoadCrystalCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    --this.m_numCrystalsLoading;
    if ((double) this.m_manaCrystalWidth <= 0.0)
      this.m_manaCrystalWidth = !(bool) UniversalInputManager.UsePhoneUI ? go.transform.Find("Gem_Mana").GetComponent<Renderer>().bounds.size.x : 0.33f;
    ManaCrystalMgr.LoadCrystalCallbackData crystalCallbackData = callbackData as ManaCrystalMgr.LoadCrystalCallbackData;
    ManaCrystal component = go.GetComponent<ManaCrystal>();
    if (crystalCallbackData.IsTempCrystal)
    {
      component.MarkAsTemp();
      this.m_temporaryCrystals.Add(component);
    }
    else
    {
      this.m_permanentCrystals.Add(component);
      if (crystalCallbackData.IsTurnStart)
      {
        if (this.m_additionalOverloadedCrystalsOwedThisTurn > 0)
        {
          component.PayOverload();
          --this.m_additionalOverloadedCrystalsOwedThisTurn;
        }
      }
      else if (this.m_additionalOverloadedCrystalsOwedNextTurn > 0)
      {
        component.state = ManaCrystal.State.USED;
        component.MarkAsOwedForOverload();
        --this.m_additionalOverloadedCrystalsOwedNextTurn;
      }
      this.StartCoroutine(this.UpdatePermanentCrystalStates());
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      component.transform.parent = this.manaGemBone.transform.parent;
      component.transform.localRotation = this.manaGemBone.transform.localRotation;
      component.transform.localScale = this.manaGemBone.transform.localScale;
    }
    else
      component.transform.parent = this.transform;
    component.transform.localPosition = Vector3.zero;
    component.PlayCreateAnimation();
    ManaCrystalAssetPaths crystalAssetPaths = this.GetManaCrystalAssetPaths(this.m_manaCrystalType);
    SoundManager.Get().LoadAndPlay((AssetReference) crystalAssetPaths.m_SoundOnAddPath, this.gameObject);
    this.UpdateLayout();
  }

  public float GetWidth() => this.m_permanentCrystals.Count == 0 ? 0.0f : this.m_permanentCrystals[0].transform.Find("Gem_Mana").GetComponent<Renderer>().bounds.size.x * (float) this.m_permanentCrystals.Count * (float) this.m_temporaryCrystals.Count;

  private ManaCrystalAssetPaths GetManaCrystalAssetPaths(ManaCrystalType type)
  {
    foreach (ManaCrystalAssetPaths crystalAssetPaths in this.m_ManaCrystalAssetTable)
    {
      if (crystalAssetPaths.m_Type == type)
        return crystalAssetPaths;
    }
    return this.m_ManaCrystalAssetTable[0];
  }

  private IEnumerator WaitThenAddManaCrystal(bool isTemp, bool isTurnStart)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    ManaCrystalMgr manaCrystalMgr = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      ManaCrystalAssetPaths crystalAssetPaths = manaCrystalMgr.GetManaCrystalAssetPaths(manaCrystalMgr.m_manaCrystalType);
      ManaCrystalMgr.LoadCrystalCallbackData callbackData = new ManaCrystalMgr.LoadCrystalCallbackData(isTemp, isTurnStart);
      AssetLoader.Get().InstantiatePrefab((AssetReference) crystalAssetPaths.m_ResourcePath, new PrefabCallback<GameObject>(manaCrystalMgr.LoadCrystalCallback), (object) callbackData, AssetLoadingOptions.IgnorePrefabPosition);
      --manaCrystalMgr.m_numQueuedToSpawn;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    ++manaCrystalMgr.m_numCrystalsLoading;
    ++manaCrystalMgr.m_numQueuedToSpawn;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds((float) manaCrystalMgr.m_numQueuedToSpawn * 0.2f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private IEnumerator WaitThenDestroyManaCrystals(bool isTemp, int numCrystals)
  {
    while (this.m_numCrystalsLoading > 0)
      yield return (object) null;
    for (int index = 0; index < numCrystals; ++index)
    {
      if (isTemp)
        this.DestroyTempManaCrystal();
      else
        this.DestroyManaCrystal();
    }
  }

  private IEnumerator WaitThenReadyManaCrystal()
  {
    ManaCrystalMgr manaCrystalMgr = this;
    ++manaCrystalMgr.m_numQueuedToReady;
    yield return (object) new WaitForSeconds((float) manaCrystalMgr.m_numQueuedToReady * 0.2f);
    if (manaCrystalMgr.m_numQueuedToReady > 0)
    {
      if (manaCrystalMgr.m_permanentCrystals.Count > 0)
      {
        for (int index = manaCrystalMgr.m_permanentCrystals.Count - 1; index >= 0; --index)
        {
          if (manaCrystalMgr.m_permanentCrystals[index].state == ManaCrystal.State.USED)
          {
            ManaCrystalAssetPaths crystalAssetPaths = manaCrystalMgr.GetManaCrystalAssetPaths(manaCrystalMgr.m_manaCrystalType);
            SoundManager.Get().LoadAndPlay((AssetReference) crystalAssetPaths.m_SoundOnRefreshPath, manaCrystalMgr.gameObject);
            manaCrystalMgr.m_permanentCrystals[index].state = ManaCrystal.State.READY;
            break;
          }
        }
      }
      --manaCrystalMgr.m_numQueuedToReady;
    }
  }

  private IEnumerator WaitThenSpendManaCrystal()
  {
    ++this.m_numQueuedToSpend;
    yield return (object) new WaitForSeconds((float) (this.m_numQueuedToSpend - 1) * 0.2f);
    if (this.m_numQueuedToSpend > 0)
    {
      bool flag = false;
      for (int index = 0; index < this.m_permanentCrystals.Count; ++index)
      {
        if (this.m_permanentCrystals[index].state != ManaCrystal.State.USED)
        {
          this.m_permanentCrystals[index].state = ManaCrystal.State.USED;
          flag = true;
          break;
        }
      }
      if (!flag)
        --this.m_numQueuedToReady;
      --this.m_numQueuedToSpend;
      if (this.m_numQueuedToSpend <= 0)
        InputManager.Get().OnManaCrystalMgrManaSpent();
    }
  }

  private void DestroyManaCrystal()
  {
    if (this.m_permanentCrystals.Count <= 0)
      return;
    int index = 0;
    ManaCrystal permanentCrystal = this.m_permanentCrystals[index];
    this.m_permanentCrystals.RemoveAt(index);
    permanentCrystal.GetComponent<ManaCrystal>().Destroy();
    this.UpdateLayout();
    this.StartCoroutine(this.UpdatePermanentCrystalStates());
  }

  private void DestroyTempManaCrystal()
  {
    if (this.m_temporaryCrystals.Count <= 0)
      return;
    int index = this.m_temporaryCrystals.Count - 1;
    ManaCrystal temporaryCrystal = this.m_temporaryCrystals[index];
    this.m_temporaryCrystals.RemoveAt(index);
    temporaryCrystal.GetComponent<ManaCrystal>().Destroy();
    this.UpdateLayout();
  }

  private void SpendManaCrystal() => this.StartCoroutine(this.WaitThenSpendManaCrystal());

  private void ReadyManaCrystal() => this.StartCoroutine(this.WaitThenReadyManaCrystal());

  private void InitializePhoneManaGems()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_friendlyManaText = this.friendlyManaCounter.GetComponent<UberText>();
    ManaCounter component = this.friendlyManaCounter.GetComponent<ManaCounter>();
    string phoneLargeResource = this.m_ManaCrystalAssetTable[(int) this.m_manaCrystalType].m_phoneLargeResource;
    component.InitializeLargeResourceGameObject(phoneLargeResource);
    if (this.opposingManaCounter.activeInHierarchy)
      this.opposingManaCounter.GetComponent<ManaCounter>().InitializeLargeResourceGameObject(phoneLargeResource);
    this.m_friendlyManaGem = component.GetPhoneGem();
    this.ApplyFriendlyManaGemTexture();
  }

  private void ApplyFriendlyManaGemTexture()
  {
    if ((UnityEngine.Object) this.m_friendlyManaGem == (UnityEngine.Object) null || this.m_friendlyManaGemTexture == null)
      return;
    RendererExtension.GetMaterial((Renderer) this.m_friendlyManaGem.GetComponentInChildren<MeshRenderer>()).mainTexture = (Texture) this.m_friendlyManaGemTexture;
  }

  private class LoadCrystalCallbackData
  {
    private bool m_isTempCrystal;
    private bool m_isTurnStart;

    public bool IsTempCrystal => this.m_isTempCrystal;

    public bool IsTurnStart => this.m_isTurnStart;

    public LoadCrystalCallbackData(bool isTempCrystal, bool isTurnStart)
    {
      this.m_isTempCrystal = isTempCrystal;
      this.m_isTurnStart = isTurnStart;
    }
  }
}
