using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using UnityEngine;

public class DeckTrayHeroSkinContent : DeckTrayContent
{
  [SerializeField]
  private UberText m_currentHeroSkinName;
  [Header("Positioning")]
  [SerializeField]
  private GameObject m_root;
  [SerializeField]
  private Vector3 m_trayHiddenOffset;
  [SerializeField]
  private GameObject m_heroSkinContainer;
  [SerializeField]
  private Vector3 m_missingCardEffectScale;
  [SerializeField]
  [Header("Animation")]
  private iTween.EaseType m_traySlideSlideInAnimation = iTween.EaseType.easeOutBounce;
  [SerializeField]
  private iTween.EaseType m_traySlideSlideOutAnimation;
  [SerializeField]
  private float m_traySlideAnimationTime = 0.25f;
  [SerializeField]
  private SpellType m_removalSpellType;
  [SerializeField]
  [Header("Sound")]
  private WeakAssetReference m_appearanceSound;
  [SerializeField]
  private WeakAssetReference m_socketSound;
  [SerializeField]
  private WeakAssetReference m_addSound;
  [SerializeField]
  private WeakAssetReference m_unsocketSound;
  [SerializeField]
  private WeakAssetReference m_pickUpSound;
  private const string ADD_CARD_TO_DECK_SOUND = "collection_manager_card_add_to_deck_instant.prefab:06df359c4026d7e47b06a4174f33e3ef";
  private Widget m_rootWidget;
  private DeckDataModel m_deckDataModel;
  private string m_currentHeroCardId;
  private CollectionDeck m_currentDeck;
  private Actor m_heroSkinActor;
  private Vector3 m_originalLocalPosition;
  private bool m_animating;
  private bool m_waitingToLoadHeroDef;
  private bool m_shouldUpdateLimitToFavoritesSetting;
  private const string CODE_CHECKBOX_TOGGLED = "CODE_CHECKBOX_TOGGLED";
  private const string CODE_TRAY_CLICKED = "CODE_TRAY_CLICKED";
  private const string CODE_TRAY_DRAG_START = "CODE_TRAY_DRAG_START";
  private DeckTrayHeroSkinContent.AnimatedHeroSkin m_animData;

  public event Action<string> OnHeroChanged;

  protected override void Awake()
  {
    base.Awake();
    this.m_rootWidget = (Widget) this.GetComponent<WidgetTemplate>();
    if ((UnityEngine.Object) this.m_rootWidget != (UnityEngine.Object) null)
    {
      this.m_rootWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.WidgetEventListener));
      this.m_rootWidget.RegisterReadyListener(new Action<object>(this.WidgetReadyListener), (object) null, true);
    }
    this.m_originalLocalPosition = this.transform.localPosition;
    this.transform.localPosition = this.m_originalLocalPosition + this.m_trayHiddenOffset;
    this.m_root.SetActive(false);
    this.LoadHeroSkinActor();
  }

  private void WidgetReadyListener(object unused)
  {
    this.m_deckDataModel = new DeckDataModel();
    this.m_deckDataModel.RandomHeroFavoritesOnly = true;
    this.m_rootWidget.BindDataModel((IDataModel) this.m_deckDataModel);
  }

  private void WidgetEventListener(string eventName)
  {
    if (!(eventName == "CODE_CHECKBOX_TOGGLED"))
    {
      if (!(eventName == "CODE_TRAY_CLICKED"))
      {
        if (!(eventName == "CODE_TRAY_DRAG_START"))
          return;
        this.GrabHeroSkin();
      }
      else
        this.RemoveHeroSkin();
    }
    else
      this.ToggleFavoritesOnly(!this.m_deckDataModel.RandomHeroFavoritesOnly);
  }

  private void ToggleFavoritesOnly(bool enabled)
  {
    this.m_currentDeck.RandomHeroUseFavorite = enabled;
    this.UpdateDatamodel();
    Action<string> onHeroChanged = this.OnHeroChanged;
    if (onHeroChanged != null)
      onHeroChanged(string.Empty);
    this.m_shouldUpdateLimitToFavoritesSetting = true;
  }

  public void UpdateHeroSkin(string cardId, TAG_PREMIUM premium, bool assigning, Actor baseActor = null)
  {
    if (this.m_currentDeck == null)
      return;
    this.ToggleSparkleEffects(false);
    if (assigning)
    {
      if (!string.IsNullOrEmpty(this.m_socketSound.AssetString))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_socketSound.AssetString);
      if (this.m_currentDeck.HeroCardID == cardId && this.m_currentDeck.HeroOverridden)
      {
        this.ShowSocketFX();
        return;
      }
      this.m_currentDeck.HeroOverridden = true;
      this.m_currentDeck.HeroCardID = cardId;
    }
    if ((UnityEngine.Object) baseActor != (UnityEngine.Object) null)
    {
      using (DefLoader.DisposableCardDef cardDef = baseActor.ShareDisposableCardDef())
        this.UpdateHeroSkinVisual(baseActor.GetEntityDef(), cardDef, baseActor.GetPremium(), assigning);
    }
    else
    {
      this.m_waitingToLoadHeroDef = true;
      DefLoader.Get().LoadFullDef(cardId, (DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>) ((cardID, fullDef, callbackData) =>
      {
        using (fullDef)
        {
          this.m_waitingToLoadHeroDef = false;
          this.UpdateHeroSkinVisual(fullDef.EntityDef, fullDef.DisposableCardDef, premium, assigning);
        }
      }));
    }
    this.UpdateHeroSkinObject();
    this.UpdateDatamodel();
  }

  private void GrabHeroSkin()
  {
    if (!((UnityEngine.Object) this.m_heroSkinActor != (UnityEngine.Object) null))
      return;
    int dbId = GameUtils.TranslateCardIdToDbId(this.m_currentDeck.HeroCardID);
    if (!this.m_currentDeck.HeroOverridden || !CollectionInputMgr.Get().GrabHeroSkinFromSlot(this.m_heroSkinActor, dbId))
      return;
    this.m_currentDeck.HeroOverridden = false;
    this.UpdateDatamodel();
    this.UpdateHeroSkinObject();
    this.ToggleSparkleEffects(true);
    Action<string> onHeroChanged = this.OnHeroChanged;
    if (onHeroChanged == null)
      return;
    onHeroChanged(string.Empty);
  }

  public void ToggleSparkleEffects(bool enabled)
  {
    if (this.m_deckDataModel == null)
      return;
    this.m_deckDataModel.DraggingDeckAssignment = enabled;
  }

  private void RemoveHeroSkin()
  {
    if (!((UnityEngine.Object) this.m_heroSkinActor != (UnityEngine.Object) null) || !this.m_currentDeck.HeroOverridden)
      return;
    Spell spell1 = this.m_heroSkinActor.GetSpell(this.m_removalSpellType);
    this.m_currentDeck.HeroOverridden = false;
    this.UpdateDatamodel();
    Action<string> onHeroChanged = this.OnHeroChanged;
    if (onHeroChanged != null)
      onHeroChanged(string.Empty);
    if (!string.IsNullOrEmpty(this.m_unsocketSound.AssetString))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_unsocketSound.AssetString, this.gameObject);
    if ((UnityEngine.Object) spell1 == (UnityEngine.Object) null)
    {
      this.UpdateHeroSkinObject();
    }
    else
    {
      spell1.AddFinishedCallback((Spell.FinishedCallback) ((spell, userData) => this.UpdateHeroSkinObject()));
      spell1.ActivateState(SpellStateType.BIRTH);
    }
  }

  private void LoadHeroSkinActor()
  {
    string heroSkinOrHandActor = ActorNames.GetHeroSkinOrHandActor(TAG_CARDTYPE.HERO, TAG_PREMIUM.NORMAL);
    AssetLoader.Get().InstantiatePrefab((AssetReference) heroSkinOrHandActor, (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("DeckTrayHeroSkinContent.LoadHeroSkinActor - FAILED to load \"{0}\"", (object) assetRef));
      }
      else
      {
        Actor component1 = go.GetComponent<Actor>();
        if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) string.Format("HandActorCache.OnActorLoaded() - ERROR \"{0}\" has no Actor component", (object) assetRef));
        }
        else
        {
          GameUtils.SetParent((Component) component1, this.m_heroSkinContainer);
          this.m_heroSkinActor = component1;
          CollectionHeroSkin component2 = this.m_heroSkinActor.GetComponent<CollectionHeroSkin>();
          if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
            return;
          component2.ShowName = false;
        }
      }
    }), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void UpdateHeroSkinVisual(
    EntityDef entityDef,
    DefLoader.DisposableCardDef cardDef,
    TAG_PREMIUM premium,
    bool assigning)
  {
    if ((UnityEngine.Object) this.m_heroSkinActor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Hero skin object not loaded yet! Cannot set portrait!");
    }
    else
    {
      this.m_heroSkinActor.SetEntityDef(entityDef);
      this.m_heroSkinActor.SetCardDef(cardDef);
      this.m_heroSkinActor.SetPremium(premium);
      GameObject rootObject = this.m_heroSkinActor.GetRootObject();
      if ((UnityEngine.Object) rootObject != (UnityEngine.Object) null && !rootObject.activeSelf)
        rootObject.SetActive(true);
      this.m_heroSkinActor.UpdateAllComponents();
      CollectionHeroSkin component = this.m_heroSkinActor.GetComponent<CollectionHeroSkin>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.SetClass(entityDef.GetClass());
      if (assigning)
      {
        Action<string> onHeroChanged = this.OnHeroChanged;
        if (onHeroChanged != null)
          onHeroChanged(entityDef.GetCardId());
      }
      if (assigning && (UnityEngine.Object) cardDef?.CardDef != (UnityEngine.Object) null)
        GameUtils.LoadCardDefEmoteSound(cardDef.CardDef.m_EmoteDefs, EmoteType.PICKED, new GameUtils.EmoteSoundLoaded(this.OnPickEmoteLoaded));
      if ((UnityEngine.Object) this.m_currentHeroSkinName != (UnityEngine.Object) null)
        this.m_currentHeroSkinName.Text = entityDef.GetName();
      this.ShowSocketFX();
    }
  }

  private void OnPickEmoteLoaded(CardSoundSpell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return;
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnPickEmoteFinished));
    spell.Reactivate();
  }

  private void OnPickEmoteFinished(Spell spell, object userData) => UnityEngine.Object.Destroy((UnityEngine.Object) spell.gameObject);

  private void ShowSocketFX()
  {
    CollectionHeroSkin component = this.m_heroSkinActor.GetComponent<CollectionHeroSkin>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.ShowSocketFX();
  }

  private void UpdateHeroSkinObject()
  {
    Spell spell = this.m_heroSkinActor.GetSpell(this.m_removalSpellType);
    SpellManager.Get().ReleaseSpell(spell, true);
    this.m_heroSkinActor.UpdateAllComponents();
    this.m_heroSkinActor.gameObject.transform.position = this.m_heroSkinContainer.transform.position;
    this.m_heroSkinActor.gameObject.SetActive(this.m_currentDeck.HeroOverridden);
  }

  private void UpdateDatamodel()
  {
    this.m_deckDataModel.HeroOverride = this.m_currentDeck.HeroOverridden;
    this.m_deckDataModel.RandomHeroFavoritesOnly = this.m_currentDeck.RandomHeroUseFavorite;
    if (this.m_currentDeck.HeroOverridden)
    {
      int dbId = GameUtils.TranslateCardIdToDbId(this.m_currentDeck.HeroCardID);
      CardDbfRecord record = GameDbf.Card.GetRecord(dbId);
      if (this.m_deckDataModel.Hero == null)
        this.m_deckDataModel.Hero = new HeroDataModel();
      this.m_deckDataModel.Hero.CardID = dbId;
      this.m_deckDataModel.Hero.Name = (string) record.Name;
    }
    else
      this.m_deckDataModel.Hero = (HeroDataModel) null;
  }

  private void SaveRandomHeroSelectionPreference()
  {
    if (!this.m_shouldUpdateLimitToFavoritesSetting)
      return;
    int num1 = GameUtils.IsGSDFlagSet(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_RANDOM_HERO_USE_ALL_OWNED) ? 1 : 0;
    bool enableFlag = !this.m_deckDataModel.RandomHeroFavoritesOnly;
    int num2 = enableFlag ? 1 : 0;
    if (num1 != num2)
      GameUtils.SetGSDFlag(GameSaveKeyId.COLLECTION_MANAGER, GameSaveKeySubkeyId.COLLECTION_MANAGER_RANDOM_HERO_USE_ALL_OWNED, enableFlag);
    this.m_shouldUpdateLimitToFavoritesSetting = false;
  }

  public void AnimateHeroAssignmentFromPageVisual(Actor actor)
  {
    GameObject gameObject = actor.gameObject;
    DeckTrayHeroSkinContent.AnimatedHeroSkin animatedHeroSkin = new DeckTrayHeroSkinContent.AnimatedHeroSkin();
    animatedHeroSkin.Actor = actor;
    animatedHeroSkin.GameObject = gameObject;
    animatedHeroSkin.OriginalScale = gameObject.transform.localScale;
    animatedHeroSkin.OriginalPosition = gameObject.transform.position;
    this.m_animData = animatedHeroSkin;
    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z);
    gameObject.transform.localScale = this.m_heroSkinContainer.transform.lossyScale;
    Hashtable args = iTween.Hash((object) "from", (object) 0.0f, (object) "to", (object) 1f, (object) "time", (object) 0.6f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "onupdate", (object) "AnimateNewHeroSkinUpdate", (object) "onupdatetarget", (object) this.gameObject, (object) "oncomplete", (object) "AnimateNewHeroSkinFinished", (object) "oncompleteparams", (object) animatedHeroSkin, (object) "oncompletetarget", (object) this.gameObject);
    iTween.ValueTo(gameObject, args);
    CollectionHeroSkin component = actor.gameObject.GetComponent<CollectionHeroSkin>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.ShowSocketFX();
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_addSound.AssetString);
  }

  private void AnimateNewHeroSkinFinished()
  {
    this.m_heroSkinActor.gameObject.SetActive(true);
    Actor actor = this.m_animData.Actor;
    this.UpdateHeroSkin(actor.GetEntityDef().GetCardId(), actor.GetPremium(), true, actor);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_animData.GameObject);
    this.m_animData = (DeckTrayHeroSkinContent.AnimatedHeroSkin) null;
  }

  private void AnimateNewHeroSkinUpdate(float val)
  {
    GameObject gameObject = this.m_animData.GameObject;
    Vector3 originalPosition = this.m_animData.OriginalPosition;
    Vector3 position = this.m_heroSkinContainer.transform.position;
    if ((double) val <= 0.850000023841858)
    {
      val /= 0.85f;
      gameObject.transform.position = new Vector3(Mathf.Lerp(originalPosition.x, position.x, val), (float) ((double) Mathf.Lerp(originalPosition.y, position.y, val) + (double) Mathf.Sin(val * 3.141593f) * 15.0 + (double) val * 4.0), Mathf.Lerp(originalPosition.z, position.z, val));
    }
    else
    {
      this.m_heroSkinActor.gameObject.SetActive(false);
      val = (float) (((double) val - 0.850000023841858) / 0.149999976158142);
      gameObject.transform.position = new Vector3(position.x, position.y + Mathf.Lerp(4f, 0.0f, val), position.z);
    }
  }

  public void AnimateInHeroSkin(Actor actor)
  {
    if (this.m_animData != null)
      return;
    Actor actor1 = actor.Clone();
    actor1.SetCardDefFromActor(actor);
    CollectionHeroSkin component = actor1.GetComponent<CollectionHeroSkin>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      component.ShowFavoriteBanner(false);
      component.ShowName = false;
    }
    this.AnimateHeroAssignmentFromPageVisual(actor1);
  }

  public override bool PreAnimateContentEntrance()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null)
      return true;
    this.m_currentDeck = editedDeck;
    TAG_PREMIUM heroPremium = CollectionManager.Get().GetHeroPremium(editedDeck.GetClass());
    this.UpdateHeroSkin(editedDeck.HeroCardID, heroPremium, false);
    return true;
  }

  public override bool AnimateContentEntranceStart()
  {
    if (this.m_waitingToLoadHeroDef)
      return false;
    this.m_root.SetActive(true);
    this.UpdateDatamodel();
    if (!string.IsNullOrEmpty(this.m_appearanceSound.AssetString))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_appearanceSound.AssetString, this.gameObject);
    this.transform.localPosition = this.m_originalLocalPosition;
    this.m_animating = true;
    iTween.MoveFrom(this.gameObject, iTween.Hash((object) "position", (object) (this.m_originalLocalPosition + this.m_trayHiddenOffset), (object) "islocal", (object) true, (object) "time", (object) this.m_traySlideAnimationTime, (object) "easetype", (object) this.m_traySlideSlideInAnimation, (object) "oncomplete", (object) (Action<object>) (o => this.m_animating = false)));
    return true;
  }

  public override bool AnimateContentEntranceEnd() => !this.m_animating;

  public override bool AnimateContentExitStart()
  {
    this.SaveRandomHeroSelectionPreference();
    this.transform.localPosition = this.m_originalLocalPosition;
    this.m_animating = true;
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "position", (object) (this.m_originalLocalPosition + this.m_trayHiddenOffset), (object) "islocal", (object) true, (object) "time", (object) this.m_traySlideAnimationTime, (object) "easetype", (object) this.m_traySlideSlideOutAnimation, (object) "oncomplete", (object) (Action<object>) (o =>
    {
      this.m_animating = false;
      this.m_root.SetActive(false);
    })));
    return true;
  }

  public override bool AnimateContentExitEnd() => !this.m_animating;

  private class AnimatedHeroSkin
  {
    public Actor Actor;
    public GameObject GameObject;
    public Vector3 OriginalScale;
    public Vector3 OriginalPosition;
  }
}
