using UnityEngine;

public class CollectionDraggableCardVisual : MonoBehaviour
{
  public DragRotatorInfo m_CardDragRotatorInfo = new DragRotatorInfo()
  {
    m_PitchInfo = new DragRotatorAxisInfo()
    {
      m_ForceMultiplier = 3f,
      m_MinDegrees = -55f,
      m_MaxDegrees = 55f,
      m_RestSeconds = 2f
    },
    m_RollInfo = new DragRotatorAxisInfo()
    {
      m_ForceMultiplier = 4.5f,
      m_MinDegrees = -60f,
      m_MaxDegrees = 60f,
      m_RestSeconds = 2f
    }
  };
  private static Vector3 DECK_TILE_LOCAL_SCALE;
  private static Vector3 CARD_ACTOR_LOCAL_SCALE;
  private static Vector3 HERO_SKIN_ACTOR_LOCAL_SCALE;
  private static bool s_scaleIsSetup;
  private CollectionDeckSlot m_slot;
  private DeckTrayDeckTileVisual m_deckTileToRemove;
  private Actor m_cardBackActor;
  private CardBack m_currentCardBack;
  private EntityDef m_entityDef;
  private TAG_PREMIUM m_premium;
  private DefLoader.DisposableCardDef m_cardDef;
  private Actor m_activeActor;
  private CollectionDeckTileActor m_deckTile;
  private Actor m_cardActor;
  private HandActorCache m_actorCache = new HandActorCache();
  private bool m_actorCacheInit;
  private CollectionUtils.ViewMode m_visualType;
  private int m_cardBackId;

  private void Awake()
  {
    this.EnsureScaleConstantsExist();
    this.gameObject.SetActive(false);
    this.LoadDeckTile();
    this.LoadCardBack();
    if (!((Object) this.gameObject.GetComponent<AudioSource>() == (Object) null))
      return;
    this.gameObject.AddComponent<AudioSource>();
  }

  private void OnDestroy()
  {
    this.m_cardDef?.Dispose();
    this.m_cardDef = (DefLoader.DisposableCardDef) null;
  }

  private void Update()
  {
    if ((Object) this.m_deckTileToRemove != (Object) null)
      this.m_deckTileToRemove.SetHighlight(false);
    this.m_deckTileToRemove = (DeckTrayDeckTileVisual) null;
    RaycastHit hitInfo;
    if ((Object) this.m_activeActor != (Object) this.m_deckTile || CollectionManager.Get().GetEditedDeck() == null || !UniversalInputManager.Get().GetInputHitInfo((LayerMask) DeckTrayDeckTileVisual.LAYER.LayerBit(), out hitInfo))
      return;
    DeckTrayDeckTileVisual component = hitInfo.collider.gameObject.GetComponent<DeckTrayDeckTileVisual>();
    if ((Object) component == (Object) null || (Object) component == (Object) this.m_deckTileToRemove)
      return;
    this.m_deckTileToRemove = component;
  }

  public void SetCardBackId(int cardBackId) => this.m_cardBackId = cardBackId;

  public int GetCardBackId() => this.m_cardBackId;

  public string GetCardID() => this.m_entityDef.GetCardId();

  public TAG_PREMIUM GetPremium() => this.m_premium;

  public EntityDef GetEntityDef() => this.m_entityDef;

  public CollectionDeckSlot GetSlot() => this.m_slot;

  public void SetSlot(CollectionDeckSlot slot) => this.m_slot = slot;

  public CollectionUtils.ViewMode GetVisualType() => this.m_visualType;

  public void InitActorCache()
  {
    if (this.m_actorCacheInit)
      return;
    this.m_actorCacheInit = true;
    this.m_actorCache.AddActorLoadedListener(new HandActorCache.ActorLoadedCallback(this.OnCardActorLoaded));
    this.m_actorCache.Initialize();
  }

  public bool ChangeActor(Actor actor, CollectionUtils.ViewMode vtype, TAG_PREMIUM premium)
  {
    this.InitActorCache();
    if (this.m_actorCache.IsInitializing())
      return false;
    this.m_visualType = vtype;
    if (this.m_visualType != CollectionUtils.ViewMode.CARD_BACKS)
    {
      EntityDef entityDef = actor.GetEntityDef();
      bool flag1 = entityDef != this.m_entityDef;
      bool flag2 = premium != this.m_premium;
      if (!flag1 && !flag2)
        return true;
      this.m_entityDef = entityDef;
      this.m_premium = premium;
      this.m_cardActor = this.m_actorCache.GetActor(entityDef, premium);
      if ((Object) this.m_cardActor == (Object) null)
        return false;
      if (flag1 | flag2)
      {
        DefLoader.Get().LoadCardDef(this.m_entityDef.GetCardId(), new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnCardDefLoaded), (object) new CardPortraitQuality(1, this.m_premium));
      }
      else
      {
        this.InitDeckTileActor();
        this.InitCardActor();
      }
      return true;
    }
    if (!((Object) actor != (Object) null))
      return false;
    this.m_entityDef = (EntityDef) null;
    this.m_premium = TAG_PREMIUM.NORMAL;
    this.m_currentCardBack = actor.GetComponentInChildren<CardBack>();
    this.m_cardActor = this.m_cardBackActor;
    this.m_cardBackActor.SetCardbackUpdateIgnore(true);
    return true;
  }

  public void UpdateVisual(bool isOverDeck)
  {
    Actor activeActor = this.m_activeActor;
    SpellType spellType;
    if (this.m_visualType == CollectionUtils.ViewMode.CARDS)
    {
      if (isOverDeck && this.m_entityDef != null && !this.m_entityDef.IsHeroSkin())
      {
        if ((Object) this.m_deckTile != (Object) null && this.m_entityDef != null)
          this.m_deckTile.UpdateNameTextForRuneBar(this.m_entityDef.HasRuneCost);
        this.m_activeActor = (Actor) this.m_deckTile;
        spellType = SpellType.SUMMON_IN;
      }
      else
      {
        this.m_activeActor = this.m_cardActor;
        spellType = SpellType.DEATHREVERSE;
      }
    }
    else
    {
      this.m_activeActor = this.m_cardActor;
      spellType = SpellType.DEATHREVERSE;
      if ((Object) this.m_deckTileToRemove != (Object) null)
        this.m_deckTileToRemove.SetHighlight(false);
    }
    if ((Object) activeActor == (Object) this.m_activeActor)
      return;
    if ((Object) activeActor != (Object) null)
    {
      activeActor.Hide();
      activeActor.gameObject.SetActive(false);
    }
    if ((Object) this.m_activeActor == (Object) null)
      return;
    this.m_activeActor.gameObject.SetActive(true);
    this.m_activeActor.Show();
    if (this.m_visualType == CollectionUtils.ViewMode.CARD_BACKS && (Object) this.m_currentCardBack != (Object) null)
      CardBackManager.Get().UpdateCardBack(this.m_activeActor, this.m_currentCardBack);
    Spell spell = this.m_activeActor.GetSpell(spellType);
    if ((Object) spell != (Object) null)
      spell.ActivateState(SpellStateType.BIRTH);
    if (this.m_entityDef == null || !this.m_entityDef.IsHeroSkin())
      return;
    CollectionHeroSkin component = this.m_activeActor.gameObject.GetComponent<CollectionHeroSkin>();
    if (!((Object) component != (Object) null))
      return;
    component.SetClass(this.m_entityDef.GetClass());
    component.ShowSocketFX();
    component.ShowName = false;
  }

  public bool IsShown() => !((Object) this.gameObject == (Object) null) && this.gameObject.activeSelf;

  public void Show(bool isOverDeck)
  {
    this.gameObject.SetActive(true);
    this.UpdateVisual(isOverDeck);
    if (!((Object) this.m_deckTile != (Object) null) || this.m_entityDef == null)
      return;
    this.m_deckTile.UpdateDeckCardProperties(this.m_entityDef.IsElite(), false, 1, false);
  }

  public void Hide()
  {
    if ((Object) this.m_activeActor != (Object) null && this.m_entityDef != null && this.m_entityDef.IsHeroSkin())
    {
      CollectionHeroSkin component = this.m_activeActor.gameObject.GetComponent<CollectionHeroSkin>();
      if ((Object) component != (Object) null)
        component.HideSocketFX();
    }
    this.gameObject.SetActive(false);
    if (!((Object) this.m_activeActor != (Object) null))
      return;
    this.m_activeActor.Hide();
    this.m_activeActor.gameObject.SetActive(false);
    this.m_activeActor = (Actor) null;
  }

  public DeckTrayDeckTileVisual GetDeckTileToRemove() => this.m_deckTileToRemove;

  private void LoadDeckTile()
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) "DeckCardBar.prefab:c2bab6eea6c3a3a4d90dcd7572075291", AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject == (Object) null)
    {
      Debug.LogWarning((object) string.Format("CollectionDraggableCardVisual.OnDeckTileActorLoaded() - FAILED to load actor \"{0}\"", (object) "DeckCardBar.prefab:c2bab6eea6c3a3a4d90dcd7572075291"));
    }
    else
    {
      this.m_deckTile = gameObject.GetComponent<CollectionDeckTileActor>();
      if ((Object) this.m_deckTile == (Object) null)
      {
        Debug.LogWarning((object) string.Format("CollectionDraggableCardVisual.OnDeckTileActorLoaded() - ERROR game object \"{0}\" has no CollectionDeckTileActor component", (object) "DeckCardBar.prefab:c2bab6eea6c3a3a4d90dcd7572075291"));
      }
      else
      {
        this.m_deckTile.Hide();
        this.m_deckTile.transform.parent = this.transform;
        this.m_deckTile.transform.localPosition = new Vector3(2.194931f, 0.0f, 0.0f);
        this.m_deckTile.transform.localScale = CollectionDraggableCardVisual.DECK_TILE_LOCAL_SCALE;
        this.m_deckTile.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
      }
    }
  }

  private void LoadCardBack()
  {
    GameObject child = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", AssetLoadingOptions.IgnorePrefabPosition);
    GameUtils.SetParent(child, (Component) this);
    this.m_cardBackActor = child.GetComponent<Actor>();
    this.m_cardBackActor.transform.localScale = CollectionDraggableCardVisual.CARD_ACTOR_LOCAL_SCALE;
    this.m_cardBackActor.TurnOffCollider();
    this.m_cardBackActor.Hide();
    child.AddComponent<DragRotator>().SetInfo(this.m_CardDragRotatorInfo);
  }

  private void OnCardActorLoaded(string assetName, Actor actor, object callbackData)
  {
    if ((Object) actor == (Object) null)
    {
      Debug.LogWarning((object) string.Format("CollectionDraggableCardVisual.OnCardActorLoaded() - FAILED to load {0}", (object) assetName));
    }
    else
    {
      ((object) actor).GetType();
      actor.TurnOffCollider();
      actor.Hide();
      if (this.name == "Card_Hero_Skin")
        actor.transform.localScale = CollectionDraggableCardVisual.HERO_SKIN_ACTOR_LOCAL_SCALE;
      else
        actor.transform.localScale = CollectionDraggableCardVisual.CARD_ACTOR_LOCAL_SCALE;
      actor.transform.parent = this.transform;
      actor.transform.localPosition = Vector3.zero;
      actor.gameObject.AddComponent<DragRotator>().SetInfo(this.m_CardDragRotatorInfo);
    }
  }

  private void OnCardDefLoaded(
    string cardID,
    DefLoader.DisposableCardDef cardDef,
    object callbackData)
  {
    if (this.m_entityDef == null || this.m_entityDef.GetCardId() != cardID)
    {
      cardDef?.Dispose();
    }
    else
    {
      this.m_cardDef?.Dispose();
      this.m_cardDef = cardDef;
      this.InitDeckTileActor();
      this.InitCardActor();
    }
  }

  private void InitDeckTileActor()
  {
    this.InitActor((Actor) this.m_deckTile);
    this.m_deckTile.SetSlot((CollectionDeckSlot) null);
    this.m_deckTile.SetCardDef(this.m_cardDef);
    this.m_deckTile.UpdateAllComponents(true);
    this.m_deckTile.UpdateDeckCardProperties(this.m_entityDef.IsElite(), false, 1, false);
  }

  private void InitCardActor()
  {
    this.InitActor(this.m_cardActor);
    this.m_cardActor.transform.localRotation = Quaternion.identity;
  }

  private void InitActor(Actor actor)
  {
    actor.SetEntityDef(this.m_entityDef);
    actor.SetCardDef(this.m_cardDef);
    actor.SetPremium(this.m_premium);
    actor.UpdateAllComponents();
  }

  private void EnsureScaleConstantsExist()
  {
    if (CollectionDraggableCardVisual.s_scaleIsSetup)
      return;
    CollectionDraggableCardVisual.s_scaleIsSetup = true;
    CollectionDraggableCardVisual.DECK_TILE_LOCAL_SCALE = (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
    {
      PC = new Vector3(0.6f, 0.6f, 0.6f),
      Phone = new Vector3(0.9f, 0.9f, 0.9f)
    };
    CollectionDraggableCardVisual.CARD_ACTOR_LOCAL_SCALE = (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
    {
      PC = new Vector3(6f, 6f, 6f),
      Phone = new Vector3(6.9f, 6.9f, 6.9f)
    };
    CollectionDraggableCardVisual.HERO_SKIN_ACTOR_LOCAL_SCALE = (Vector3) new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
    {
      PC = new Vector3(7.5f, 7.5f, 7.5f),
      Phone = new Vector3(8.2f, 8.2f, 8.2f)
    };
  }
}
