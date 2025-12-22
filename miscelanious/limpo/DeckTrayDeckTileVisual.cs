using UnityEngine;

public class DeckTrayDeckTileVisual : PegUIElement
{
  public static readonly GameLayer LAYER = GameLayer.CardRaycast;
  private readonly Vector3 BOX_COLLIDER_SIZE = new Vector3(25.34f, 2.14f, 3.68f);
  private readonly Vector3 BOX_COLLIDER_CENTER = new Vector3(-1.4f, 0.0f, 0.0f);
  protected const int DEFAULT_PORTRAIT_QUALITY = 1;
  protected CollectionDeck m_deck;
  protected CollectionDeckSlot m_slot;
  protected BoxCollider m_collider;
  protected CollectionDeckTileActor m_actor;
  protected bool m_isInUse;
  protected bool m_useSliderAnimations;
  protected bool m_inArena;
  protected bool m_offsetCardNameForRunes;
  private bool m_pendingRemoval;

  public void Initialize(bool useFullScaleDeckTileActor)
  {
    string assetRef = useFullScaleDeckTileActor ? "DeckCardBar.prefab:c2bab6eea6c3a3a4d90dcd7572075291" : "DeckCardBar_phone.prefab:bd1c5e767f791984e851553bc5cb3b07";
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject == (Object) null)
    {
      Debug.LogWarning((object) string.Format("DeckTrayDeckTileVisual.OnDeckTileActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_actor = gameObject.GetComponent<CollectionDeckTileActor>();
      if ((Object) this.m_actor == (Object) null)
      {
        Debug.LogWarning((object) string.Format("DeckTrayDeckTileVisual.OnDeckTileActorLoaded() - ERROR game object \"{0}\" has no CollectionDeckTileActor component", (object) assetRef));
      }
      else
      {
        GameUtils.SetParent((Component) this.m_actor, (Component) this);
        this.m_actor.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
        UIBScrollableItem component = this.m_actor.GetComponent<UIBScrollableItem>();
        if ((Object) component != (Object) null)
        {
          component.SetCustomActiveState(new UIBScrollableItem.ActiveStateCallback(this.IsInUse));
          component.UpdateScrollableParent();
        }
        this.SetUpActor();
        if ((Object) this.gameObject.GetComponent<BoxCollider>() == (Object) null)
        {
          this.m_collider = this.gameObject.AddComponent<BoxCollider>();
          this.m_collider.size = this.BOX_COLLIDER_SIZE;
          this.m_collider.center = this.BOX_COLLIDER_CENTER;
        }
        this.Hide();
        LayerUtils.SetLayer(this.gameObject, DeckTrayDeckTileVisual.LAYER);
        this.SetDragTolerance(5f);
      }
    }
  }

  public string GetCardID() => this.m_actor.GetEntityDef().GetCardId();

  public TAG_PREMIUM GetPremium() => this.m_actor.GetPremium();

  public CollectionDeckSlot GetSlot() => this.m_slot;

  public void SetSlot(
    CollectionDeck deck,
    CollectionDeckSlot s,
    bool useSliderAnimations,
    bool offsetCardNameForRunes)
  {
    this.m_deck = deck;
    this.m_slot = s;
    this.m_useSliderAnimations = useSliderAnimations;
    this.m_offsetCardNameForRunes = offsetCardNameForRunes;
    this.SetUpActor();
  }

  public CollectionDeckTileActor GetActor() => this.m_actor;

  public Bounds GetBounds() => this.m_collider.bounds;

  public void Show() => this.gameObject.SetActive(true);

  public void ShowAndSetupActor()
  {
    this.Show();
    this.SetUpActor();
  }

  public void Hide() => this.gameObject.SetActive(false);

  public void MarkAsUsed() => this.m_isInUse = true;

  public void MarkAsUnused()
  {
    this.m_isInUse = false;
    if ((Object) this.m_actor == (Object) null)
      return;
    this.m_actor.UpdateDeckCardProperties(CollectionDeckTileActor.TileIconState.CARD_COUNT, 1, false);
  }

  public bool IsInUse() => this.m_isInUse;

  public void SetInArena(bool inArena) => this.m_inArena = inArena;

  public void SetHighlight(bool highlight)
  {
    if ((Object) this.m_actor.m_highlight != (Object) null)
      this.m_actor.m_highlight.SetActive(highlight);
    if (!((Object) this.m_actor.m_highlightGlow != (Object) null))
      return;
    if (this.GetGhostedState() == CollectionDeckTileActor.GhostedState.RED)
      this.m_actor.m_highlightGlow.SetActive(highlight);
    else
      this.m_actor.m_highlightGlow.SetActive(false);
  }

  public void UpdateGhostedState()
  {
    this.m_actor.SetGhosted(this.GetGhostedState());
    this.m_actor.UpdateGhostTileEffect();
  }

  private CollectionDeckTileActor.GhostedState GetGhostedState()
  {
    CollectionDeckTileActor.GhostedState ghostedState = CollectionDeckTileActor.GhostedState.NONE;
    if (this.m_deck != null)
    {
      switch (this.m_deck.GetSlotStatus(this.m_slot))
      {
        case CollectionDeck.SlotStatus.NOT_VALID:
          ghostedState = CollectionDeckTileActor.GhostedState.RED;
          break;
        case CollectionDeck.SlotStatus.MISSING:
          ghostedState = CollectionDeckTileActor.GhostedState.BLUE;
          break;
      }
      bool flag = SceneMgr.Get().IsInDuelsMode() && !PvPDungeonRunScene.IsEditingDeck();
      if (this.m_deck.HasClass(TAG_CLASS.DEATHKNIGHT) && !flag && !SceneMgr.Get().IsInArenaDraftMode() && !this.m_deck.Runes.CanAddRunes(this.m_slot.GetEntityDef().GetRuneCost(), this.m_deck.Runes.CombinedValue))
        ghostedState = CollectionDeckTileActor.GhostedState.RED;
      if (this.m_pendingRemoval)
        ghostedState = CollectionDeckTileActor.GhostedState.BLUE;
    }
    return ghostedState;
  }

  private void SetUpActor()
  {
    if ((Object) this.m_actor == (Object) null || this.m_slot == null || string.IsNullOrEmpty(this.m_slot.CardID))
      return;
    this.m_actor.GetEntityDef();
    EntityDef entityDef = this.m_slot.GetEntityDef();
    this.m_actor.SetSlot(this.m_slot);
    TAG_PREMIUM tagPremium = this.m_slot.PreferredPremium;
    if (this.m_inArena && Options.Get().GetBool(Option.HAS_DISABLED_PREMIUMS_THIS_DRAFT))
      tagPremium = TAG_PREMIUM.NORMAL;
    this.m_actor.SetPremium(tagPremium);
    this.m_actor.SetEntityDef(entityDef);
    this.m_actor.SetGhosted(this.GetGhostedState());
    this.m_actor.UpdateCardRuneBanner(entityDef);
    this.m_actor.UpdateNameTextForRuneBar(this.m_offsetCardNameForRunes);
    bool isUnique = entityDef != null && entityDef.IsElite();
    if (isUnique && this.m_inArena && this.m_slot.Count > 1)
      isUnique = false;
    this.m_actor.UpdateDeckCardProperties(isUnique, false, this.m_slot.Count, this.m_useSliderAnimations);
    DefLoader.Get().LoadCardDef(entityDef.GetCardId(), (DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>) ((cardID, cardDef, data) =>
    {
      using (cardDef)
      {
        if ((Object) this.m_actor == (Object) null || !cardID.Equals(this.m_actor.GetEntityDef().GetCardId()))
          return;
        this.m_actor.SetCardDef(cardDef);
        this.m_actor.UpdateAllComponents(true);
        this.m_actor.UpdateGhostTileEffect();
      }
    }), quality: new CardPortraitQuality(1, tagPremium));
  }

  public void SetPendingRemoval(bool pendingRemoval)
  {
    this.m_pendingRemoval = pendingRemoval;
    this.UpdateGhostedState();
  }

  public bool HasRuneCost()
  {
    if ((Object) this.m_actor == (Object) null)
      return false;
    EntityDef entityDef = this.m_actor.GetEntityDef();
    return entityDef != null && entityDef.HasRuneCost;
  }
}
