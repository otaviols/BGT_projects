using Blizzard.T5.Core.Utils;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusLettuce;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PackOpeningCard : MonoBehaviour
{
  public GameObject m_CardParent;
  public GameObject m_SharedHiddenCardObject;
  public Spell m_ClassNameSpell;
  public Spell m_IsNewSpell;
  public AsyncReference m_packOpeningMercenaryReference;
  public AsyncReference m_packOpeningMercenaryPortraitReference;
  public AsyncReference m_packOpeningCoinReference;
  private const TAG_RARITY FALLBACK_RARITY = TAG_RARITY.COMMON;
  private NetCache.BoosterCard m_boosterCard;
  private LettucePackComponent m_mercenaryPackComponent;
  private TAG_PREMIUM m_premium;
  private EntityDef m_entityDef;
  private Actor m_actor;
  private PackOpeningCardRarityInfo m_rarityInfo;
  private Spell m_spell;
  private PegUIElement m_revealButton;
  private PackOpeningCardMercenary m_packOpeningMercenary;
  private PackOpeningPortrait m_packOpeningMercenaryPortrait;
  private PackOpeningCoin m_packOpeningCoin;
  private bool m_ready;
  private bool m_inputEnabled;
  private bool m_revealEnabled;
  private bool m_revealed;
  private bool m_isNew;
  private List<PackOpeningCard.RevealedListener> m_revealedListeners = new List<PackOpeningCard.RevealedListener>();
  private bool m_showClassName;

  public event EventHandler OnSpellFinishedEvent;

  public event EventHandler<Spell> OnSpellStateFinishedEvent;

  private void Awake()
  {
    this.m_packOpeningMercenaryReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPackOpeningMercenaryReady));
    this.m_packOpeningMercenaryPortraitReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPackOpeningMercenaryPortraitReady));
    this.m_packOpeningCoinReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPackOpeningCoinReady));
  }

  public string GetCardId()
  {
    if (this.m_boosterCard != null)
      return this.m_boosterCard.Def.Name;
    return this.m_mercenaryPackComponent != null ? GameUtils.GetCardIdFromMercenaryId(this.m_mercenaryPackComponent.MercenaryId) : (string) null;
  }

  public EntityDef GetEntityDef() => this.m_entityDef;

  public Actor GetActor() => this.m_actor;

  private void ResetForNewCard()
  {
    this.m_boosterCard = (NetCache.BoosterCard) null;
    this.m_mercenaryPackComponent = (LettucePackComponent) null;
  }

  public void AttachBoosterCard(NetCache.BoosterCard boosterCard)
  {
    if (boosterCard == null)
      return;
    this.ResetForNewCard();
    this.m_boosterCard = boosterCard;
    this.m_premium = this.m_boosterCard.Def.Premium;
    this.m_showClassName = true;
    this.Destroy();
    this.LoadEntityDef(this.m_boosterCard.Def.Name);
  }

  public void AttachBoosterMercenary(LettucePackComponent packComponent)
  {
    if (packComponent == null)
      return;
    this.ResetForNewCard();
    this.m_mercenaryPackComponent = packComponent;
    this.m_premium = TAG_PREMIUM.NORMAL;
    this.m_showClassName = false;
    this.Destroy();
    this.LoadEntityDef(GameUtils.GetCardIdFromMercenaryId(packComponent.MercenaryId));
  }

  public bool IsReady() => this.m_ready;

  public bool IsRevealed() => this.m_revealed;

  public void Destroy()
  {
    this.m_ready = false;
    if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
    {
      this.m_actor.Destroy();
      this.m_actor = (Actor) null;
    }
    this.m_rarityInfo = (PackOpeningCardRarityInfo) null;
    this.m_spell = (Spell) null;
    this.m_revealButton = (PegUIElement) null;
    this.m_revealed = false;
  }

  public bool IsInputEnabled() => this.m_inputEnabled;

  public void EnableInput(bool enable)
  {
    this.m_inputEnabled = enable;
    this.UpdateInput();
  }

  public bool IsRevealEnabled() => this.m_revealEnabled;

  public void EnableReveal(bool enable)
  {
    this.m_revealEnabled = enable;
    this.UpdateActor();
  }

  public void AddRevealedListener(PackOpeningCard.RevealedCallback callback) => this.AddRevealedListener(callback, (object) this);

  public void AddRevealedListener(PackOpeningCard.RevealedCallback callback, object userData)
  {
    PackOpeningCard.RevealedListener revealedListener = new PackOpeningCard.RevealedListener();
    revealedListener.SetCallback(callback);
    revealedListener.SetUserData(userData);
    this.m_revealedListeners.Add(revealedListener);
  }

  public void RemoveRevealedListener(PackOpeningCard.RevealedCallback callback) => this.RemoveRevealedListener(callback, (object) null);

  public void RemoveRevealedListener(PackOpeningCard.RevealedCallback callback, object userData)
  {
    PackOpeningCard.RevealedListener revealedListener = new PackOpeningCard.RevealedListener();
    revealedListener.SetCallback(callback);
    revealedListener.SetUserData(userData);
    this.m_revealedListeners.Remove(revealedListener);
  }

  public void RemoveOnOverWhileFlippedListeners()
  {
    this.m_revealButton.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOverWhileFlipped));
    this.m_revealButton.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOutWhileFlipped));
  }

  public void ForceReveal() => this.OnPress((UIEvent) null);

  public void ShowRarityGlow()
  {
    if (this.IsRevealed())
      return;
    this.OnOver((UIEvent) null);
  }

  public void HideRarityGlow()
  {
    if (this.IsRevealed())
      return;
    this.OnOut((UIEvent) null);
  }

  public void Dissipate()
  {
    CardBackDisplay componentInChildren = this.GetComponentInChildren<CardBackDisplay>();
    if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
      componentInChildren.gameObject.GetComponent<Renderer>().enabled = false;
    Spell classNameSpell = this.m_ClassNameSpell;
    // ISSUE: method pointer
    classNameSpell.AddFinishedCallback(new Spell.FinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellFinishedCallback\u007C56_0)));
    // ISSUE: method pointer
    classNameSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellStateFinishedCallback\u007C56_1)));
    classNameSpell.ActivateState(SpellStateType.DEATH);
    Spell isNewSpell = this.m_IsNewSpell;
    if ((UnityEngine.Object) isNewSpell != (UnityEngine.Object) null)
      isNewSpell.ActivateState(SpellStateType.DEATH);
    Actor actor = this.GetActor();
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
    {
      Spell spell = actor.GetSpell(SpellType.DEATH);
      // ISSUE: method pointer
      spell.AddFinishedCallback(new Spell.FinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellFinishedCallback\u007C56_0)));
      // ISSUE: method pointer
      spell.AddStateFinishedCallback(new Spell.StateFinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellStateFinishedCallback\u007C56_1)));
      spell.Activate();
    }
    else
    {
      PackOpeningType packOpeningType = this.DeterminePackOpeningType();
      if (packOpeningType == PackOpeningType.COIN && (UnityEngine.Object) this.m_packOpeningCoin != (UnityEngine.Object) null)
      {
        // ISSUE: method pointer
        // ISSUE: method pointer
        this.m_packOpeningCoin.ActivateDeathVisuals(new Spell.FinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellFinishedCallback\u007C56_0)), new Spell.StateFinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellStateFinishedCallback\u007C56_1)));
        if ((UnityEngine.Object) this.m_spell != (UnityEngine.Object) null)
          this.m_spell.ActivateState(SpellStateType.DEATH);
      }
      else if (packOpeningType == PackOpeningType.CARD && (UnityEngine.Object) this.m_packOpeningMercenary != (UnityEngine.Object) null)
      {
        // ISSUE: method pointer
        // ISSUE: method pointer
        this.m_packOpeningMercenary.ActivateDeathVisuals(new Spell.FinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellFinishedCallback\u007C56_0)), new Spell.StateFinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellStateFinishedCallback\u007C56_1)));
      }
      else if (packOpeningType == PackOpeningType.MERC_PORTRAIT && (UnityEngine.Object) this.m_packOpeningMercenaryPortrait != (UnityEngine.Object) null)
      {
        // ISSUE: method pointer
        // ISSUE: method pointer
        this.m_packOpeningMercenaryPortrait.ActivateDeathVisuals(new Spell.FinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellFinishedCallback\u007C56_0)), new Spell.StateFinishedCallback((object) this, __methodptr(\u003CDissipate\u003Eg__SpellStateFinishedCallback\u007C56_1)));
      }
    }
    if (!TemporaryAccountManager.IsTemporaryAccount())
      return;
    EntityDef entityDef = this.GetEntityDef();
    if (entityDef == null || entityDef.GetRarity() != TAG_RARITY.EPIC && entityDef.GetRarity() != TAG_RARITY.LEGENDARY)
      return;
    TemporaryAccountManager.Get().ShowEarnCardEventHealUpDialog(TemporaryAccountManager.HealUpReason.OPEN_PACK);
  }

  private void LoadEntityDef(string cardId)
  {
    this.m_entityDef = DefLoader.Get().GetEntityDef(cardId);
    if (this.m_entityDef == null)
    {
      Debug.LogError((object) ("PackOpeningCard.LoadEntityDef() - FAILED to load \"" + cardId + "\""));
    }
    else
    {
      PackOpeningType packOpeningType = this.DeterminePackOpeningType();
      if (packOpeningType == PackOpeningType.NONE)
      {
        Debug.LogError((object) ("PackOpeningCard.OnFullDefLoaded() - FAILED to determine pack reward type for " + this.GetCardId()));
      }
      else
      {
        if (!this.DetermineRarityInfo(packOpeningType))
          return;
        if (this.m_mercenaryPackComponent != null)
        {
          switch (packOpeningType)
          {
            case PackOpeningType.CARD:
              this.SetUpPackOpeningMercenaryDataModel(this.m_entityDef);
              break;
            case PackOpeningType.COIN:
              this.SetUpPackOpeningCoinDataModel(this.m_entityDef);
              break;
            case PackOpeningType.MERC_PORTRAIT:
              this.SetUpPackOpeningMercenaryPortraitDataModel(this.m_entityDef);
              break;
          }
        }
        else if (this.m_boosterCard != null)
        {
          AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(this.m_entityDef, this.m_premium), new PrefabCallback<GameObject>(this.OnActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
          if (!Cheats.Get().IsNewCardInPackOpeningEnabed())
            return;
          CollectibleCard card = CollectionManager.Get().GetCard(this.m_entityDef.GetCardId(), this.m_premium);
          this.m_isNew = card.SeenCount < 1 && card.OwnedCount < 2;
        }
        else
          Debug.LogError((object) string.Format("PackOpeningCard.OnFullDefLoaded() - No card data provided \"{0}\"", (object) cardId));
      }
    }
  }

  private void OnActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("PackOpeningCard.OnActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("PackOpeningCard.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) this.name));
      }
      else
      {
        this.m_actor = component;
        this.m_actor.TurnOffCollider();
        this.SetupActor();
        LayerUtils.SetLayer(component.gameObject, GameLayer.IgnoreFullScreenEffects);
        this.m_ready = true;
        this.UpdateInput();
        this.UpdateActor();
      }
    }
  }

  private PackOpeningType DeterminePackOpeningType()
  {
    if (this.m_mercenaryPackComponent != null)
    {
      if (this.m_mercenaryPackComponent.CurrencyAmount > 0L)
        return PackOpeningType.COIN;
      if (this.IsPortrait())
        return PackOpeningType.MERC_PORTRAIT;
    }
    return PackOpeningType.CARD;
  }

  private bool DetermineRarityInfo(PackOpeningType packRewardType)
  {
    EntityDef entityDef = this.m_entityDef;
    PackOpeningRarity rarity = GameUtils.GetPackOpeningRarity(entityDef != null ? entityDef.GetRarity() : TAG_RARITY.COMMON);
    if (rarity == PackOpeningRarity.NONE)
    {
      Debug.LogError((object) ("PackOpeningCard.DetermineRarityInfo() - FAILED to determine rarity for " + this.GetCardId()));
      return false;
    }
    PackOpening componentInParents = GameObjectUtils.FindComponentInParents<PackOpening>((Component) this);
    GameObject gameObject;
    switch (packRewardType)
    {
      case PackOpeningType.COIN:
        gameObject = componentInParents.GetPackOpeningCoinEffects();
        break;
      case PackOpeningType.MERC_PORTRAIT:
        gameObject = componentInParents.GetPackOpeningPortraitEffects();
        break;
      default:
        gameObject = componentInParents.GetPackOpeningCardEffects();
        break;
    }
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "PackOpeningCard.DetermineRarityInfo() - Fail to get card effect from PackOpening");
      return false;
    }
    PackOpeningCardRarityInfo[] componentsInChildren = gameObject.GetComponentsInChildren<PackOpeningCardRarityInfo>();
    if (componentsInChildren == null)
    {
      Debug.LogError((object) string.Format("PackOpeningCard.DetermineRarityInfo() - {0} has no rarity info list. cardId={1}", (object) this, (object) this.GetCardId()));
      return false;
    }
    PackOpeningCardRarityInfo openingCardRarityInfo = ((IEnumerable<PackOpeningCardRarityInfo>) componentsInChildren).FirstOrDefault<PackOpeningCardRarityInfo>((Func<PackOpeningCardRarityInfo, bool>) (info => rarity == info.m_RarityType));
    if ((UnityEngine.Object) openingCardRarityInfo == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("PackOpeningCard.DetermineRarityInfo() - {0} has no rarity info for {1}, {2}. cardId={3}", (object) this, (object) rarity, (object) packRewardType, (object) this.GetCardId()));
      return false;
    }
    this.m_rarityInfo = openingCardRarityInfo;
    this.SetupRarity();
    return true;
  }

  private void SetupActor()
  {
    this.m_actor.SetEntityDef(this.m_entityDef);
    this.m_actor.SetPremium(this.m_premium);
    this.m_actor.UpdateAllComponents();
  }

  private void UpdateActor()
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    if (!this.IsRevealEnabled())
    {
      this.m_actor.Hide();
    }
    else
    {
      if (!this.IsRevealed())
        this.m_actor.Hide();
      Vector3 localScale = this.m_actor.transform.localScale;
      this.m_actor.transform.parent = this.m_rarityInfo.m_RevealedCardObject.transform;
      this.m_actor.transform.localPosition = Vector3.zero;
      this.m_actor.transform.localRotation = Quaternion.identity;
      this.m_actor.transform.localScale = localScale;
      if (!this.m_isNew)
        return;
      this.m_actor.SetActorState(ActorStateType.CARD_RECENTLY_ACQUIRED);
    }
  }

  private void SetupRarity()
  {
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_rarityInfo.gameObject);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      return;
    gameObject.transform.parent = this.m_CardParent.transform;
    this.m_rarityInfo = gameObject.GetComponent<PackOpeningCardRarityInfo>();
    this.m_rarityInfo.m_RarityObject.SetActive(true);
    this.m_rarityInfo.m_HiddenCardObject.SetActive(true);
    Vector3 localPosition = this.m_rarityInfo.m_HiddenCardObject.transform.localPosition;
    this.m_rarityInfo.m_HiddenCardObject.transform.parent = this.m_CardParent.transform;
    this.m_rarityInfo.m_HiddenCardObject.transform.localPosition = localPosition;
    this.m_rarityInfo.m_HiddenCardObject.transform.localRotation = Quaternion.identity;
    this.m_rarityInfo.m_HiddenCardObject.transform.localScale = new Vector3(7.646f, 7.646f, 7.646f);
    TransformUtil.AttachAndPreserveLocalTransform(this.m_rarityInfo.m_RarityObject.transform, this.m_CardParent.transform);
    this.m_spell = this.m_rarityInfo.m_RarityObject.GetComponent<Spell>();
    this.m_revealButton = this.m_rarityInfo.m_RarityObject.GetComponent<PegUIElement>();
    if (UniversalInputManager.Get().IsTouchMode())
      this.m_revealButton.SetReceiveReleaseWithoutMouseDown(true);
    this.m_SharedHiddenCardObject.transform.parent = this.m_rarityInfo.m_HiddenCardObject.transform;
    TransformUtil.Identity((Component) this.m_SharedHiddenCardObject.transform);
  }

  private void OnOver(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_spell == (UnityEngine.Object) null || !this.IsReady())
      return;
    this.m_spell.ActivateState(SpellStateType.BIRTH);
  }

  private void OnOut(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_spell == (UnityEngine.Object) null || !this.IsReady())
      return;
    this.m_spell.ActivateState(SpellStateType.CANCEL);
  }

  private void OnOverWhileFlipped(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    if (this.m_isNew)
      this.m_actor.SetActorState(ActorStateType.CARD_RECENTLY_ACQUIRED_MOUSE_OVER);
    else
      this.m_actor.SetActorState(ActorStateType.CARD_HISTORY);
    TooltipPanelManager.Get().UpdateKeywordHelpForPackOpening(this.m_actor.GetEntityDef(), this.m_actor);
  }

  private void OnOutWhileFlipped(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    if (this.m_isNew)
      this.m_actor.SetActorState(ActorStateType.CARD_RECENTLY_ACQUIRED);
    else
      this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
    TooltipPanelManager.Get().HideKeywordHelp();
  }

  private void OnPress(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_spell == (UnityEngine.Object) null || !this.IsReady())
      return;
    this.m_revealed = true;
    this.UpdateInput();
    List<GameObject> gameObjectList = new List<GameObject>(3);
    if ((UnityEngine.Object) this.m_packOpeningMercenary != (UnityEngine.Object) null)
      gameObjectList.Add(this.m_packOpeningMercenary.gameObject);
    if ((UnityEngine.Object) this.m_packOpeningMercenaryPortrait != (UnityEngine.Object) null)
      gameObjectList.Add(this.m_packOpeningMercenaryPortrait.gameObject);
    if ((UnityEngine.Object) this.m_packOpeningCoin != (UnityEngine.Object) null)
      gameObjectList.Add(this.m_packOpeningCoin.gameObject);
    foreach (GameObject gameObject in gameObjectList)
    {
      VisualController component = gameObject.GetComponent<VisualController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.SetState("REVEAL");
      else
        Debug.LogError((object) ("PackOpeningCard.OnPress() - Fail to get VisualController from " + gameObject.name));
    }
    this.m_spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished));
    this.m_spell.ActivateState(SpellStateType.ACTION);
    this.PlayCorrectSound();
  }

  private void UpdateInput()
  {
    if (!this.IsReady())
      return;
    bool flag = !this.IsRevealed() && this.IsInputEnabled();
    if (!((UnityEngine.Object) this.m_revealButton != (UnityEngine.Object) null) || (bool) UniversalInputManager.UsePhoneUI)
      return;
    if (flag)
    {
      this.m_revealButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOver));
      this.m_revealButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOut));
      this.m_revealButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPress));
      if (!((UnityEngine.Object) PegUI.Get().FindHitElement() == (UnityEngine.Object) this.m_revealButton))
        return;
      this.OnOver((UIEvent) null);
    }
    else
    {
      this.m_revealButton.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOver));
      this.m_revealButton.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOut));
      this.m_revealButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPress));
    }
  }

  public void OnPackOpeningMercenaryReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "PackOpeningCardMercenary could not be found!");
    else
      this.m_packOpeningMercenary = visualController.GetComponent<PackOpeningCardMercenary>();
  }

  public void OnPackOpeningMercenaryPortraitReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "PackOpeningCardMercenary could not be found!");
    else
      this.m_packOpeningMercenaryPortrait = visualController.GetComponent<PackOpeningPortrait>();
  }

  private LettuceMercenaryDataModel GetPackOpeningMercenaryDataModel()
  {
    Widget owner = (Widget) this.m_packOpeningMercenary.GetComponent<VisualController>().Owner;
    IDataModel model;
    if (!owner.GetDataModel(216, out model))
    {
      model = (IDataModel) MercenaryFactory.CreateEmptyMercenaryDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceMercenaryDataModel;
  }

  private LettuceMercenaryCoinDataModel GetPackOpeningMercenaryCoinDataModel()
  {
    Widget owner = (Widget) this.m_packOpeningMercenary.GetComponent<VisualController>().Owner;
    IDataModel model;
    if (!owner.GetDataModel(238, out model))
    {
      model = (IDataModel) new LettuceMercenaryCoinDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceMercenaryCoinDataModel;
  }

  private void SetUpPackOpeningMercenaryDataModel(EntityDef entityDef)
  {
    LettuceMercenaryDataModel mercenaryDataModel = this.GetPackOpeningMercenaryDataModel();
    LettuceMercenaryCoinDataModel mercenaryCoinDataModel = this.GetPackOpeningMercenaryCoinDataModel();
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) this.m_mercenaryPackComponent.MercenaryId);
    CollectionUtils.PopulateMercenaryCardDataModel(mercenaryDataModel, this.CreateArtVariation());
    CollectionUtils.SetMercenaryStatsByLevel(mercenaryDataModel, mercenary.ID, mercenary.m_level, mercenary.m_isFullyUpgraded);
    bool flag = this.IsPortrait();
    mercenaryDataModel.HideXp = true;
    mercenaryDataModel.HideWatermark = false;
    mercenaryDataModel.HideStats = flag;
    mercenaryDataModel.Label = flag ? GameStrings.Get("GLUE_MERCENARY_LABEL_PORTRAIT") : string.Empty;
    string shortName = entityDef.GetShortName();
    string str = string.IsNullOrEmpty(shortName) ? entityDef.GetName() : shortName;
    mercenaryCoinDataModel.MercenaryId = this.m_mercenaryPackComponent.MercenaryId;
    mercenaryCoinDataModel.MercenaryName = str;
    mercenaryCoinDataModel.Quantity = (int) this.m_mercenaryPackComponent.CurrencyAmount;
    mercenaryCoinDataModel.GlowActive = false;
    mercenaryCoinDataModel.NameActive = true;
    GameObject gameObject = this.m_packOpeningMercenary.gameObject;
    LayerUtils.SetLayer(gameObject, GameLayer.IgnoreFullScreenEffects);
    this.m_SharedHiddenCardObject.gameObject.SetActive(false);
    this.UpdateInput();
    Vector3 localScale = gameObject.transform.localScale;
    gameObject.transform.parent = this.m_rarityInfo.m_RevealedCardObject.transform;
    gameObject.transform.localPosition = Vector3.zero;
    gameObject.transform.localRotation = Quaternion.identity;
    gameObject.transform.localScale = localScale;
    this.m_ready = true;
  }

  private LettuceMercenaryDataModel GetPackOpeningMercenaryPortraitDataModel()
  {
    Widget owner = (Widget) this.m_packOpeningMercenaryPortrait.GetComponent<VisualController>().Owner;
    IDataModel model;
    if (!owner.GetDataModel(216, out model))
    {
      model = (IDataModel) new LettuceMercenaryDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceMercenaryDataModel;
  }

  private void SetUpPackOpeningMercenaryPortraitDataModel(EntityDef entityDef)
  {
    LettuceMercenaryDataModel portraitDataModel = this.GetPackOpeningMercenaryPortraitDataModel();
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) this.m_mercenaryPackComponent.MercenaryId);
    CollectionUtils.PopulateMercenaryCardDataModel(portraitDataModel, this.CreateArtVariation());
    CollectionUtils.SetMercenaryStatsByLevel(portraitDataModel, mercenary.ID, mercenary.m_level, mercenary.m_isFullyUpgraded);
    string shortName = entityDef.GetShortName();
    string str = string.IsNullOrEmpty(shortName) ? entityDef.GetName() : shortName;
    portraitDataModel.HideXp = true;
    portraitDataModel.HideWatermark = false;
    portraitDataModel.MercenaryName = str;
    portraitDataModel.MercenaryShortName = str;
    portraitDataModel.MercenaryRarity = mercenary.m_rarity;
    portraitDataModel.MercenaryRole = mercenary.m_role;
    portraitDataModel.HideStats = true;
    GameObject gameObject = this.m_packOpeningMercenaryPortrait.gameObject;
    LayerUtils.SetLayer(gameObject, GameLayer.IgnoreFullScreenEffects);
    this.m_SharedHiddenCardObject.gameObject.SetActive(false);
    VisualController component = gameObject.GetComponent<VisualController>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.SetState("HIDDEN");
    else
      Debug.LogError((object) "PackOpeningCard.SetUpPackOpeningMercenaryPortraitDataModel() - Fail to get VisualController from m_packOpeningMercenaryPortrait");
    this.UpdateInput();
    Vector3 localScale = gameObject.transform.localScale;
    gameObject.transform.parent = this.m_rarityInfo.m_RevealedCardObject.transform;
    gameObject.transform.localPosition = Vector3.zero;
    gameObject.transform.localRotation = Quaternion.identity;
    gameObject.transform.localScale = localScale;
    this.m_ready = true;
  }

  private bool IsPortrait()
  {
    if (this.m_mercenaryPackComponent.MercenaryArtVariationPremium > 0)
      return true;
    return this.m_mercenaryPackComponent.HasMercenaryArtVariationId && GameDbf.MercenaryArtVariation.HasRecord(this.m_mercenaryPackComponent.MercenaryArtVariationId) && !GameDbf.MercenaryArtVariation.GetRecord(this.m_mercenaryPackComponent.MercenaryArtVariationId).DefaultVariation;
  }

  public void OnPackOpeningCoinReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "PackOpeningCardCoin could not be found!");
    else
      this.m_packOpeningCoin = visualController.GetComponent<PackOpeningCoin>();
  }

  private LettuceMercenaryCoinDataModel GetPackOpeningCoinDataModel()
  {
    Widget owner = (Widget) this.m_packOpeningCoin.GetComponent<VisualController>().Owner;
    IDataModel model;
    if (!owner.GetDataModel(238, out model))
    {
      model = (IDataModel) new LettuceMercenaryCoinDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceMercenaryCoinDataModel;
  }

  private void SetUpPackOpeningCoinDataModel(EntityDef entityDef)
  {
    LettuceMercenaryCoinDataModel openingCoinDataModel = this.GetPackOpeningCoinDataModel();
    string shortName = entityDef.GetShortName();
    string str = string.IsNullOrEmpty(shortName) ? entityDef.GetName() : shortName;
    openingCoinDataModel.MercenaryId = this.m_mercenaryPackComponent.MercenaryId;
    openingCoinDataModel.MercenaryName = str;
    openingCoinDataModel.Quantity = (int) this.m_mercenaryPackComponent.CurrencyAmount;
    openingCoinDataModel.GlowActive = false;
    openingCoinDataModel.NameActive = true;
    GameObject gameObject = this.m_packOpeningCoin.gameObject;
    this.m_SharedHiddenCardObject.gameObject.SetActive(false);
    gameObject.GetComponent<VisualController>().SetState("HIDDEN");
    LayerUtils.SetLayer(gameObject, GameLayer.IgnoreFullScreenEffects);
    this.UpdateInput();
    Vector3 localScale = gameObject.transform.localScale;
    gameObject.transform.parent = this.m_rarityInfo.m_RevealedCardObject.transform;
    gameObject.transform.localPosition = Vector3.zero;
    gameObject.transform.localRotation = Quaternion.identity;
    gameObject.transform.localScale = localScale;
    this.m_ready = true;
  }

  private void FireRevealedEvent()
  {
    foreach (PackOpeningCard.RevealedListener revealedListener in this.m_revealedListeners.ToArray())
      revealedListener.Fire();
  }

  private void OnSpellFinished(Spell spell, object userData)
  {
    this.FireRevealedEvent();
    this.UpdateInput();
    if (this.m_showClassName)
      this.ShowClassName();
    this.ShowIsNew();
    if ((UnityEngine.Object) this.m_packOpeningMercenary != (UnityEngine.Object) null)
    {
      this.m_SharedHiddenCardObject.SetActive(false);
      this.m_packOpeningMercenary.ShowMercenaryNameGlow();
      this.m_packOpeningMercenary.GetComponent<VisualController>().SetState("REVEAL_COMPLETE");
      this.m_packOpeningCoin.GetComponent<VisualController>().SetState("REVEAL_COMPLETE");
    }
    this.m_revealButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOverWhileFlipped));
    this.m_revealButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOutWhileFlipped));
  }

  private void ShowClassName()
  {
    string className = this.GetClassName();
    foreach (UberText componentsInChild in this.m_ClassNameSpell.GetComponentsInChildren<UberText>(true))
    {
      componentsInChild.Text = className;
      if (this.m_entityDef.IsMultiClass())
        componentsInChild.OutlineSize = 3f;
    }
    this.m_ClassNameSpell.ActivateState(SpellStateType.BIRTH);
  }

  private void ShowIsNew()
  {
    if (!this.m_isNew || !((UnityEngine.Object) this.m_IsNewSpell != (UnityEngine.Object) null))
      return;
    this.m_IsNewSpell.ActivateState(SpellStateType.BIRTH);
  }

  private string GetClassName()
  {
    TAG_CLASS tag = this.m_entityDef.GetClass();
    if (this.m_entityDef.IsMultiClass())
      return this.GetFamilyClassNames();
    return tag == TAG_CLASS.NEUTRAL ? GameStrings.Get("GLUE_PACK_OPENING_ALL_CLASSES") : GameStrings.GetClassName(tag);
  }

  private string GetFamilyClassNames()
  {
    if (this.m_entityDef.HasTag(GAME_TAG.GRIMY_GOONS))
      return GameStrings.Get("GLUE_GOONS_CLASS_NAMES");
    if (this.m_entityDef.HasTag(GAME_TAG.JADE_LOTUS))
      return GameStrings.Get("GLUE_LOTUS_CLASS_NAMES");
    if (this.m_entityDef.HasTag(GAME_TAG.KABAL))
      return GameStrings.Get("GLUE_KABAL_CLASS_NAMES");
    List<TAG_CLASS> tagClassList = new List<TAG_CLASS>();
    this.m_entityDef.GetClasses((IList<TAG_CLASS>) tagClassList);
    if (tagClassList.Count<TAG_CLASS>() == 10)
      return GameStrings.Get("GLUE_PACK_OPENING_ALL_CLASSES");
    string familyClassNames = "";
    foreach (TAG_CLASS tag in tagClassList)
    {
      familyClassNames += GameStrings.GetClassName(tag);
      if (tag != tagClassList.Last<TAG_CLASS>())
        familyClassNames += GameStrings.Get("GLOBAL_COMMA_SEPARATOR");
    }
    return familyClassNames;
  }

  private void PlayCorrectSound()
  {
    if (this.DeterminePackOpeningType() == PackOpeningType.COIN)
    {
      SoundManager.Get().LoadAndPlay((AssetReference) "MERC_Coin_FlipOver.prefab:b9874b13c58956341aff2030393b9a4c");
    }
    else
    {
      switch (this.m_rarityInfo.m_RarityType)
      {
        case PackOpeningRarity.COMMON:
          if (this.m_premium == TAG_PREMIUM.SIGNATURE)
          {
            SoundManager.Get().LoadAndPlay((AssetReference) "VO_Innkeeper_Male_Dwarf_Rarity_Signature_Common_03.prefab:2309811536bcc4848abff75ffd6205e2");
            break;
          }
          if (this.m_premium != TAG_PREMIUM.GOLDEN)
            break;
          SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_FOIL_C_29.prefab:69820e4999e4afa439761151e057a526");
          break;
        case PackOpeningRarity.RARE:
          if (this.m_premium == TAG_PREMIUM.SIGNATURE)
          {
            SoundManager.Get().LoadAndPlay((AssetReference) "VO_Innkeeper_Male_Dwarf_Rarity_Signature_Rare_01.prefab:6cebfb7484c0cda48831d0b0dae81689");
            break;
          }
          if (this.m_premium == TAG_PREMIUM.GOLDEN)
          {
            SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_FOIL_R_30.prefab:f5bf5bfd8e5f4d247aa8a6da966969cf");
            break;
          }
          SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_RARE_27.prefab:8ff0de7a4fd144b4b983caea4c54da4d");
          break;
        case PackOpeningRarity.EPIC:
          if (this.m_premium == TAG_PREMIUM.SIGNATURE)
          {
            SoundManager.Get().LoadAndPlay((AssetReference) "VO_Innkeeper_Male_Dwarf_Rarity_Signature_Epic_05.prefab:cb3de39e03a22fb468a4900374d96f02");
            break;
          }
          if (this.m_premium == TAG_PREMIUM.GOLDEN)
          {
            SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_FOIL_E_31.prefab:d419d6eca0e2a72469544bae5f11542f");
            break;
          }
          SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_EPIC_26.prefab:e76d67f55b976104794c3cf73382e82a");
          break;
        case PackOpeningRarity.LEGENDARY:
          if (this.m_premium == TAG_PREMIUM.SIGNATURE)
          {
            SoundManager.Get().LoadAndPlay((AssetReference) "VO_Innkeeper_Male_Dwarf_Rarity_Signature_Legendary_04.prefab:905e5f71e16de48429ac3371d6b26fd4");
            break;
          }
          if (this.m_premium == TAG_PREMIUM.GOLDEN)
          {
            SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_FOIL_L_32.prefab:caefd66acfc4e2b4f858035c274b257e");
            break;
          }
          SoundManager.Get().LoadAndPlay((AssetReference) "VO_ANNOUNCER_LEGENDARY_25.prefab:e015c982aec12bc4893f36396d426750");
          break;
      }
    }
  }

  private LettuceMercenary.ArtVariation CreateArtVariation()
  {
    if (this.m_mercenaryPackComponent == null)
    {
      Error.AddDevWarning("Error", "Mercenary Pack Component was not set!");
      return (LettuceMercenary.ArtVariation) null;
    }
    MercenaryArtVariationDbfRecord record = this.m_mercenaryPackComponent.HasMercenaryArtVariationId ? GameDbf.MercenaryArtVariation.GetRecord(this.m_mercenaryPackComponent.MercenaryArtVariationId) : LettuceMercenary.GetDefaultArtVariationRecord(this.m_mercenaryPackComponent.MercenaryId);
    TAG_PREMIUM premium = this.m_mercenaryPackComponent.HasMercenaryArtVariationPremium ? (TAG_PREMIUM) this.m_mercenaryPackComponent.MercenaryArtVariationPremium : TAG_PREMIUM.NORMAL;
    return new LettuceMercenary.ArtVariation(record, premium, record.DefaultVariation);
  }

  public delegate void RevealedCallback(object userData);

  private class RevealedListener : EventListener<PackOpeningCard.RevealedCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }
}
