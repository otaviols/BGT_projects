using Blizzard.T5.AssetManager;
using System;
using System.Collections;
using UnityEngine;

public class BaconCosmeticPreviewManager : MonoBehaviour
{
  public GameObject m_root;
  public GameObject m_boardBase;
  public GameObject m_boardBasePhone;
  public UberText m_displayText;
  public GameObject m_friendlyHeroHolder;
  public GameObject m_opposingHeroHolder;
  public Actor m_friendlyHeroActor;
  public Actor m_opposingHeroActor;
  private BaconCosmeticPreviewManager.BoardSkin m_boardSkin = new BaconCosmeticPreviewManager.BoardSkin();
  private FinisherGameplaySettings m_strikeSettings;
  private int m_pendingLoads;
  private BaconCosmeticPreviewRunnerConfig m_config;
  private int m_currentAction;
  private int m_blockingActionFunctionsInProgress;
  private SpellHandleValueRange[] m_ImpactDefs;
  private string m_DefaultImpactSpellPrefab;
  private const string ATTACK_SPELL_CONTROLLER_PREFAB_PATH = "AttackSpellController_Battlegrounds_Hero.prefab:922da2c91f4cca1458b5901204d1d26c";
  private const string DEFAULT_SHATTER_SPELL = "Bacon_EndRound_HeroImpact.prefab:34d052b6989dcea4c8b7d22adcb31368";

  public void Start()
  {
    AttackSpellController component = AssetLoader.Get().InstantiatePrefab((AssetReference) "AttackSpellController_Battlegrounds_Hero.prefab:922da2c91f4cca1458b5901204d1d26c").GetComponent<AttackSpellController>();
    this.m_ImpactDefs = component.m_ImpactDefHandles;
    this.m_DefaultImpactSpellPrefab = component.m_DefaultImpactSpellPrefabHandle;
  }

  private void Awake()
  {
    this.m_displayText.Hide();
    this.m_config = BaconCosmeticPreviewLoadInfo.s_runnerConfig;
    this.LoadStrikeSettings(this.m_config.strikeId);
    this.LoadBoardSkin(this.m_config.boardId);
    this.LoadHero(this.m_config.friendlyHeroCardId, Player.Side.FRIENDLY);
    this.LoadHero(this.m_config.opposingHeroCardId, Player.Side.OPPOSING);
  }

  private void LoadHero(string heroId, Player.Side side)
  {
    if (string.IsNullOrEmpty(heroId))
    {
      if (side == Player.Side.FRIENDLY)
        this.m_friendlyHeroActor.gameObject.SetActive(false);
      else
        this.m_opposingHeroActor.gameObject.SetActive(false);
    }
    else
    {
      ++this.m_pendingLoads;
      DefLoader.Get().LoadFullDef(heroId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroLoaded), (object) side);
    }
  }

  private void LoadStrikeSettings(int id)
  {
    BattlegroundsFinisherDbfRecord record = GameDbf.BattlegroundsFinisher.GetRecord(id);
    if (record == null)
      return;
    AssetReference fromAssetString = AssetReference.CreateFromAssetString(record.GameplaySettings);
    if (fromAssetString == null)
      return;
    ++this.m_pendingLoads;
    AssetLoader.Get().LoadAsset<FinisherGameplaySettings>(fromAssetString, new AssetHandleCallback<FinisherGameplaySettings>(this.OnStrikeLoaded));
  }

  private void LoadBoardSkin(int id)
  {
    BattlegroundsBoardSkinDbfRecord boardSkinDbfRecord = GameDbf.BattlegroundsBoardSkin.GetRecord(id) ?? GameDbf.BattlegroundsBoardSkin.GetRecord(1);
    string assetRef1;
    string assetRef2;
    if (PlatformSettings.Screen == ScreenCategory.Phone)
    {
      assetRef1 = boardSkinDbfRecord.FullBoardPrefabPhone;
      assetRef2 = boardSkinDbfRecord.FullTavernBoardPrefabPhone;
    }
    else
    {
      assetRef1 = boardSkinDbfRecord.FullBoardPrefab;
      assetRef2 = boardSkinDbfRecord.FullTavernBoardPrefab;
    }
    this.m_pendingLoads += 2;
    AssetLoader.Get().LoadAsset<GameObject>((AssetReference) assetRef1, new AssetHandleCallback<GameObject>(this.OnSkinLoaded), (object) TAG_BOARD_VISUAL_STATE.COMBAT);
    AssetLoader.Get().LoadAsset<GameObject>((AssetReference) assetRef2, new AssetHandleCallback<GameObject>(this.OnSkinLoaded), (object) TAG_BOARD_VISUAL_STATE.SHOP);
  }

  private void OnSkinLoaded(
    AssetReference assetRef,
    AssetHandle<GameObject> asset,
    object callbackData)
  {
    --this.m_pendingLoads;
    switch ((TAG_BOARD_VISUAL_STATE) callbackData)
    {
      case TAG_BOARD_VISUAL_STATE.SHOP:
        this.m_boardSkin.m_AssetHandleTavern = asset;
        this.m_boardSkin.m_TavernPrefab = asset.Asset;
        GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_boardSkin.m_TavernPrefab, this.m_root.transform);
        if (!gameObject1.TryGetComponent<BaconBoardSkinBehaviour>(out this.m_boardSkin.m_TavernInstance))
        {
          Debug.LogError((object) ("Attempting to get component BaconBoardSkinBehaviour but not found on " + (object) gameObject1));
          return;
        }
        this.m_boardSkin.m_TavernInstance.SetBoardState(this.m_config.initialState);
        break;
      case TAG_BOARD_VISUAL_STATE.COMBAT:
        this.m_boardSkin.m_AssetHandleCombat = asset;
        this.m_boardSkin.m_CombatPrefab = asset.Asset;
        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_boardSkin.m_CombatPrefab, this.m_root.transform);
        if (!gameObject2.TryGetComponent<BaconBoardSkinBehaviour>(out this.m_boardSkin.m_CombatInstance))
        {
          Debug.LogError((object) ("Attempting to get component BaconBoardSkinBehaviour but not found on " + (object) gameObject2));
          return;
        }
        this.m_boardSkin.m_CombatInstance.SetBoardState(this.m_config.initialState);
        break;
    }
    if (this.m_pendingLoads != 0)
      return;
    this.StartRunning();
  }

  private void OnHeroLoaded(
    string cardId,
    DefLoader.DisposableFullDef fullDef,
    object callbackData)
  {
    --this.m_pendingLoads;
    if ((Player.Side) callbackData == Player.Side.FRIENDLY)
    {
      this.m_friendlyHeroActor.SetCardDef(fullDef.DisposableCardDef);
      this.m_friendlyHeroActor.UpdateAllComponents();
    }
    else
    {
      this.m_opposingHeroActor.SetCardDef(fullDef.DisposableCardDef);
      this.m_opposingHeroActor.UpdateAllComponents();
    }
    if (this.m_pendingLoads != 0)
      return;
    this.StartRunning();
  }

  private void OnStrikeLoaded(
    AssetReference assetRef,
    AssetHandle<FinisherGameplaySettings> asset,
    object callbackData)
  {
    --this.m_pendingLoads;
    this.m_strikeSettings = asset.Asset;
    if (this.m_pendingLoads != 0)
      return;
    this.StartRunning();
  }

  private void StartRunning() => this.StartCoroutine(this.RunPreview());

  private IEnumerator RunPreview()
  {
    BaconCosmeticPreviewAction action = this.m_config.actions[this.m_currentAction];
    yield return (object) new WaitForSeconds(action.delay);
    if (string.IsNullOrEmpty(action.displayText))
    {
      this.m_displayText.Hide();
    }
    else
    {
      this.m_displayText.Show();
      this.m_displayText.Text = action.displayText;
    }
    switch (action.actionType)
    {
      case BaconCosmeticPreviewActionType.SWAP_BOARD_STATE:
        this.m_boardBase.GetComponentInChildren<BaconBoard>().ChangeBoardVisualStateForPreview(action.boardState, this.m_boardSkin.m_CombatInstance, this.m_boardSkin.m_TavernInstance);
        break;
      case BaconCosmeticPreviewActionType.TRIGGER_FSM_EVENT:
        if (action.boardState == TAG_BOARD_VISUAL_STATE.COMBAT)
        {
          this.m_boardSkin.m_CombatInstance.DebugTriggerFSMState(action.fsmParameter);
          break;
        }
        this.m_boardSkin.m_TavernInstance.DebugTriggerFSMState(action.fsmParameter);
        break;
      case BaconCosmeticPreviewActionType.LAUNCH_STRIKE:
        ++this.m_blockingActionFunctionsInProgress;
        this.LoadAndLaunchStrike(action);
        break;
    }
    yield return (object) new WaitForSeconds(action.duration);
    this.m_currentAction = (this.m_currentAction + 1) % this.m_config.actions.Count;
    yield return (object) this.RunPreview();
  }

  private void LoadAndLaunchStrike(BaconCosmeticPreviewAction action)
  {
    string assetRef = action.strikeLethalLevel != KeyboardFinisherSettings.LethalLevel.Lethal || string.IsNullOrEmpty(this.m_strikeSettings.LethalPrefab) ? (action.strikeLethalLevel != KeyboardFinisherSettings.LethalLevel.FirstPlaceVictory || string.IsNullOrEmpty(this.m_strikeSettings.FirstPlaceVictoryPrefab) ? (action.strikeDamageLevel != KeyboardFinisherSettings.DamageLevel.Small ? this.m_strikeSettings.LargePrefab : this.m_strikeSettings.SmallPrefab) : this.m_strikeSettings.FirstPlaceVictoryPrefab) : this.m_strikeSettings.LethalPrefab;
    if (string.IsNullOrEmpty(assetRef))
    {
      Log.CosmeticPreview.PrintError("Tried to play an empty finisher spell prefab for finisher " + this.m_strikeSettings.name + ": " + (object) action.strikeDamageLevel + ", " + (object) action.strikeLethalLevel);
      --this.m_blockingActionFunctionsInProgress;
    }
    else
      AssetLoader.Get().LoadAsset<GameObject>((AssetReference) assetRef, new AssetHandleCallback<GameObject>(this.OnFinisherSpellLoaded), (object) action);
  }

  private void OnFinisherSpellLoaded(
    AssetReference assetRef,
    AssetHandle<GameObject> asset,
    object callbackData)
  {
    BaconCosmeticPreviewAction userData = (BaconCosmeticPreviewAction) callbackData;
    Spell component = UnityEngine.Object.Instantiate<GameObject>(asset.Asset).GetComponent<Spell>();
    component.SetSource(this.m_friendlyHeroHolder);
    component.AddTarget(this.m_opposingHeroHolder);
    component.transform.parent = this.m_friendlyHeroHolder.transform;
    component.AddFinishedCallback(new Spell.FinishedCallback(this.OnFinisherFinished), (object) userData);
    if (userData.strikeLethalLevel != KeyboardFinisherSettings.LethalLevel.Nonlethal)
    {
      ++this.m_blockingActionFunctionsInProgress;
      component.AddFinishedCallback(new Spell.FinishedCallback(this.DestroyOpposingPlayerHero), (object) userData);
    }
    SuperSpell superSpell = component as SuperSpell;
    if ((UnityEngine.Object) superSpell == (UnityEngine.Object) null)
      component.Activate();
    else
      superSpell.ActivateFinisher();
  }

  private void DestroyOpposingPlayerHero(Spell spell, object callbackData)
  {
    string spellAssetRef = ((BaconCosmeticPreviewAction) callbackData).strikeLethalLevel != KeyboardFinisherSettings.LethalLevel.FirstPlaceVictory || string.IsNullOrEmpty(this.m_strikeSettings.FirstPlaceVictoryDestroyOpponentPrefab) ? this.m_strikeSettings.DestroyOpponentPrefab : this.m_strikeSettings.FirstPlaceVictoryDestroyOpponentPrefab;
    Spell spell1 = string.IsNullOrEmpty(spellAssetRef) ? SpellManager.Get().GetSpell("Bacon_EndRound_HeroImpact.prefab:34d052b6989dcea4c8b7d22adcb31368") : SpellManager.Get().GetSpell(spellAssetRef);
    spell1.AddFinishedCallback(new Spell.FinishedCallback(this.OnFinishedCallback));
    spell1.SetSource(this.m_opposingHeroHolder);
    spell1.AddTarget(this.m_friendlyHeroHolder);
    spell1.transform.parent = this.m_opposingHeroHolder.transform;
    spell1.Activate();
  }

  private void OnFinisherFinished(Spell spell, object userData) => this.ActivateImpactEffects(spell, (BaconCosmeticPreviewAction) userData);

  private void OnFinished(Spell spell)
  {
    --this.m_blockingActionFunctionsInProgress;
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) spell, 5f);
  }

  private void OnFinishedCallback(Spell spell, object userData) => this.OnFinished(spell);

  private void ActivateImpactEffects(Spell spell, BaconCosmeticPreviewAction action)
  {
    ++this.m_blockingActionFunctionsInProgress;
    if (!this.m_strikeSettings.ShowImpactEffects)
    {
      this.OnFinished(spell);
    }
    else
    {
      string impactSpellPrefab = this.DetermineImpactSpellPrefab(action.strikeImpactDamage);
      if (string.IsNullOrEmpty(impactSpellPrefab))
      {
        this.OnFinished(spell);
      }
      else
      {
        Spell spell1 = SpellManager.Get().GetSpell(impactSpellPrefab);
        spell1.SetSource(this.m_friendlyHeroHolder);
        spell1.AddTarget(this.m_opposingHeroHolder);
        Vector3 position = this.m_opposingHeroHolder.transform.position;
        spell1.SetPosition(position);
        spell1.SetOrientation(Quaternion.LookRotation(position - this.m_friendlyHeroHolder.transform.position));
        spell1.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnImpactSpellStateFinished), (object) spell);
        spell1.Activate();
      }
    }
  }

  private string DetermineImpactSpellPrefab(int impactDamage)
  {
    SpellHandleValueRange accordingToRanges = SpellUtils.GetAppropriateElementAccordingToRanges<SpellHandleValueRange>(this.m_ImpactDefs, (Func<SpellHandleValueRange, ValueRange>) (x => x.m_range), impactDamage);
    return accordingToRanges != null && !string.IsNullOrEmpty(accordingToRanges.m_spellPrefabName) ? accordingToRanges.m_spellPrefabName : this.m_DefaultImpactSpellPrefab;
  }

  private void OnImpactSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    this.OnFinished((Spell) userData);
    this.OnFinished(spell);
  }

  public void OnDestroy() => this.StopAllCoroutines();

  private class BoardSkin
  {
    public GameObject m_CombatPrefab;
    public GameObject m_TavernPrefab;
    public AssetHandle<GameObject> m_AssetHandleCombat;
    public AssetHandle<GameObject> m_AssetHandleTavern;
    public BaconBoardSkinBehaviour m_CombatInstance;
    public BaconBoardSkinBehaviour m_TavernInstance;
  }
}
