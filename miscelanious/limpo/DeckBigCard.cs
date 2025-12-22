using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBigCard : MonoBehaviour
{
  public GameObject m_topPosition;
  public GameObject m_bottomPosition;
  public Material m_missingCardMaterial;
  public Material m_ghostCardMaterial;
  public Material m_invalidCardMaterial;
  public bool m_disableCollidersOnHeroPower;
  public bool m_flipHeroPowerHorizontalPosition;
  public bool m_showTooltipsForAdventure;
  public UberText m_createdByText;
  public Vector3 m_positionOffsetWithCreatorBanner;
  public float m_scaleMultiplierWithCreatorBanner = 1f;
  private HandActorCache m_actorCache = new HandActorCache();
  private bool m_actorCacheInit;
  private bool m_hideBigHeroPower;
  private bool m_shown;
  private EntityDef m_entityDef;
  private TAG_PREMIUM m_premium;
  private DefLoader.DisposableCardDef m_cardDef;
  private Actor m_shownActor;
  private Actor m_shownHeroPowerActor;
  private GhostCard.Type m_ghosted;
  private int m_firstShowFrame;
  private Vector3 m_defaultLocalPosition;
  private Vector3 m_defaultLocalScale;
  private static readonly PlatformDependentValue<Vector3> HERO_POWER_START_POSITION = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.0f, -0.5f, 0.0f)
  };
  private static readonly PlatformDependentValue<Vector3> HERO_POWER_START_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.0f, 0.0f, 0.0f)
  };
  private static readonly PlatformDependentValue<Vector3> HERO_POWER_POSITION = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(-2.11f, 0.0f, -0.12f),
    Phone = new Vector3(-2.05f, 0.0f, -0.12f)
  };
  private static readonly PlatformDependentValue<Vector3> HERO_POWER_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(0.8117157f, 0.8117157f, 0.8117157f),
    Phone = new Vector3(0.8117157f, 0.8117157f, 0.8117157f)
  };
  private static readonly float HERO_POWER_TWEEN_TIME = 0.5f;
  private static readonly int GHOST_CARD_RENDER_QUEUE = 72;

  public event DeckBigCard.OnBigCardShownHandler OnBigCardShown;

  private void Awake()
  {
    this.m_firstShowFrame = 0;
    this.m_defaultLocalScale = this.transform.localScale;
    this.m_defaultLocalPosition = this.transform.localPosition;
  }

  private void OnDestroy()
  {
    this.m_cardDef?.Dispose();
    this.m_cardDef = (DefLoader.DisposableCardDef) null;
  }

  public void Show(
    EntityDef entityDef,
    TAG_PREMIUM premium,
    DefLoader.DisposableCardDef cardDef,
    Vector3 sourcePosition,
    GhostCard.Type ghosted,
    float delay = 0.0f)
  {
    if (false)
    {
      int frameCount = Time.frameCount;
      if (this.m_firstShowFrame == 0)
        this.m_firstShowFrame = frameCount;
      else if (frameCount - this.m_firstShowFrame <= 1)
        return;
    }
    this.StopCoroutine("ShowWithDelayInternal");
    this.m_shown = true;
    this.m_entityDef = entityDef;
    this.m_premium = premium;
    this.m_cardDef?.Dispose();
    this.m_cardDef = cardDef?.Share();
    this.m_ghosted = ghosted;
    if ((double) delay > 0.0)
    {
      this.StartCoroutine("ShowWithDelayInternal", (object) new KeyValuePair<float, Action>(delay, (Action) (() => this.Show(entityDef, premium, cardDef, sourcePosition, ghosted))));
    }
    else
    {
      if (!(bool) UniversalInputManager.UsePhoneUI)
      {
        float z1 = this.m_bottomPosition.transform.position.z;
        float z2 = this.m_topPosition.transform.position.z;
        TransformUtil.SetPosZ((Component) this.transform, Mathf.Clamp(sourcePosition.z, z1, z2));
        this.m_defaultLocalPosition = this.transform.localPosition;
      }
      if (!this.m_actorCacheInit)
      {
        this.m_actorCacheInit = true;
        this.m_actorCache.AddActorLoadedListener(new HandActorCache.ActorLoadedCallback(this.OnActorLoaded));
        this.m_actorCache.Initialize();
      }
      if (this.m_actorCache.IsInitializing())
        return;
      this.Show(sourcePosition.z);
    }
  }

  public void Hide(EntityDef entityDef, TAG_PREMIUM premium)
  {
    if (this.m_entityDef != entityDef || this.m_premium != premium)
      return;
    this.Hide();
  }

  public void ForceHide() => this.Hide();

  public void SetCreatorName(string creatorName)
  {
    if ((UnityEngine.Object) this.m_createdByText == (UnityEngine.Object) null)
      return;
    if (string.IsNullOrEmpty(creatorName))
    {
      this.m_createdByText.Text = string.Empty;
      this.transform.localPosition = this.m_defaultLocalPosition;
      this.transform.localScale = this.m_defaultLocalScale;
    }
    else
    {
      this.m_createdByText.Text = GameStrings.Format("GAMEPLAY_HISTORY_CREATED_BY", (object) creatorName);
      this.transform.localPosition = this.m_defaultLocalPosition + this.m_positionOffsetWithCreatorBanner;
      this.transform.localScale = this.m_defaultLocalScale * this.m_scaleMultiplierWithCreatorBanner;
    }
  }

  public void OffsetByVector(Vector3 offset)
  {
    this.m_defaultLocalPosition += offset;
    this.transform.localPosition = this.m_defaultLocalPosition;
  }

  public void SetHideBigHeroPower(bool hide) => this.m_hideBigHeroPower = hide;

  private IEnumerator ShowWithDelayInternal(KeyValuePair<float, Action> args)
  {
    yield return (object) new WaitForSeconds(args.Key);
    args.Value();
  }

  private void OnActorLoaded(string assetName, Actor actor, object callbackData)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("DeckBigCard.OnActorLoaded() - FAILED to load {0}", (object) assetName));
    }
    else
    {
      actor.TurnOffCollider();
      actor.Hide();
      actor.transform.parent = this.transform;
      TransformUtil.Identity((Component) actor.transform);
      LayerUtils.SetLayer((Component) actor, this.gameObject.layer);
      if (this.m_actorCache.IsInitializing() || !this.m_shown)
        return;
      this.Show();
    }
  }

  private void Show(float sourceZ = 0.0f)
  {
    this.m_shownActor = this.m_actorCache.GetActor(this.m_entityDef, this.m_premium);
    if ((UnityEngine.Object) this.m_shownActor == (UnityEngine.Object) null)
      return;
    this.m_shownActor.SetEntityDef(this.m_entityDef);
    this.m_shownActor.SetPremium(this.m_premium);
    this.m_cardDef?.Dispose();
    this.m_cardDef = DefLoader.Get().GetCardDef(this.m_entityDef.GetCardId(), new CardPortraitQuality(3, this.m_premium));
    this.m_shownActor.SetCardDef(this.m_cardDef);
    this.m_shownActor.GhostCardEffect(this.m_ghosted, this.m_premium);
    if (this.m_shownActor.isGhostCard())
    {
      GhostCard component = this.m_shownActor.m_ghostCardGameObject.GetComponent<GhostCard>();
      component.SetRenderQueue(DeckBigCard.GHOST_CARD_RENDER_QUEUE);
      component.SetBigCard(true);
    }
    if (this.m_showTooltipsForAdventure && !this.m_shownActor.GetEntityDef().IsHero())
      TooltipPanelManager.Get().UpdateKeywordHelpForAdventure(this.m_shownActor.GetEntityDef(), this.m_shownActor);
    this.m_shownActor.CreateBannedRibbon();
    this.m_shownActor.UpdateAllComponents();
    if ((UnityEngine.Object) this.m_missingCardMaterial != (UnityEngine.Object) null)
      this.m_shownActor.SetMissingCardMaterial(this.m_missingCardMaterial);
    this.m_shownActor.Show();
    CollectibleDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay();
    bool flag = (UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck != null && editedDeck.Locked)
      return;
    if (this.m_ghosted != GhostCard.Type.NONE)
    {
      TooltipPanelManager.Orientation orientation = TooltipPanelManager.Orientation.LeftMiddle;
      if ((bool) UniversalInputManager.UsePhoneUI & flag)
        orientation = (double) sourceZ > 0.0 ? TooltipPanelManager.Orientation.RightBottom : TooltipPanelManager.Orientation.RightTop;
      TooltipPanelManager.Get().UpdateGhostCardHelpForCollectionManager(this.m_shownActor, this.m_ghosted, orientation);
    }
    if (this.m_shownActor.GetEntityDef().IsHero() && !this.m_hideBigHeroPower)
      this.ShowHeroPowerCard(GameUtils.GetHeroPowerCardIdFromHero(this.m_shownActor.GetEntityDef().GetCardId()), this.m_shownActor.GetPremium());
    if ((UnityEngine.Object) this.m_createdByText != (UnityEngine.Object) null && !string.IsNullOrEmpty(this.m_createdByText.Text))
      this.m_createdByText.gameObject.SetActive(true);
    if (this.OnBigCardShown == null)
      return;
    this.OnBigCardShown(this.m_shownActor, this.m_entityDef);
  }

  private void Hide()
  {
    this.StopCoroutine("ShowWithDelayInternal");
    if (this.m_showTooltipsForAdventure)
      TooltipPanelManager.Get().HideKeywordHelp();
    if ((UnityEngine.Object) this.m_createdByText != (UnityEngine.Object) null)
      this.m_createdByText.gameObject.SetActive(false);
    this.m_shown = false;
    if ((UnityEngine.Object) this.m_shownActor == (UnityEngine.Object) null)
      return;
    this.m_shownActor.Hide();
    this.m_shownActor = (Actor) null;
    if (!this.m_hideBigHeroPower)
      this.HideHeroPowerCard();
    TooltipPanelManager.Get().HideTooltipPanels();
  }

  private void ShowHeroPowerCard(string heroPowerCardId, TAG_PREMIUM premium) => AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER, premium), (PrefabCallback<GameObject>) ((actorName, actorGameObject, data) => this.OnHeroPowerActorLoaded(actorName, actorGameObject, heroPowerCardId, premium)), (object) heroPowerCardId, AssetLoadingOptions.IgnorePrefabPosition);

  private void OnHeroPowerActorLoaded(
    AssetReference assetRef,
    GameObject actorGameObject,
    string heroPowerCardId,
    TAG_PREMIUM premium)
  {
    if ((UnityEngine.Object) actorGameObject == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("CollectionDeckTray.OnDeckBigHeroPowerActorLoaded: Unable to load actor for hero power: {0}", (object) assetRef));
    }
    else
    {
      if ((UnityEngine.Object) this.m_shownHeroPowerActor != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_shownHeroPowerActor.gameObject);
      this.m_shownHeroPowerActor = actorGameObject.GetComponent<Actor>();
      if (!this.m_shown)
      {
        this.HideHeroPowerCard();
      }
      else
      {
        this.m_shownHeroPowerActor.Show();
        if (this.m_disableCollidersOnHeroPower)
          this.m_shownHeroPowerActor.TurnOffCollider();
        using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(heroPowerCardId, this.m_shownHeroPowerActor.CardPortraitQuality))
        {
          this.m_shownHeroPowerActor.SetCardDef(fullDef.DisposableCardDef);
          this.m_shownHeroPowerActor.SetEntityDef(fullDef.EntityDef);
          this.m_shownHeroPowerActor.SetPremium(premium);
          this.m_shownHeroPowerActor.GhostCardEffect(this.m_ghosted, premium);
          if (this.m_shownActor.isGhostCard())
            this.m_shownHeroPowerActor.m_ghostCardGameObject.GetComponent<GhostCard>().SetRenderQueue(DeckBigCard.GHOST_CARD_RENDER_QUEUE);
          this.m_shownHeroPowerActor.SetUnlit();
          this.m_shownHeroPowerActor.UpdateAllComponents();
          this.m_shownHeroPowerActor.gameObject.transform.parent = this.transform;
          this.m_shownHeroPowerActor.transform.localPosition = (Vector3) DeckBigCard.HERO_POWER_START_POSITION;
          this.m_shownHeroPowerActor.transform.localScale = (Vector3) DeckBigCard.HERO_POWER_START_SCALE;
          Vector3 heroPowerPosition = (Vector3) DeckBigCard.HERO_POWER_POSITION;
          Vector3 heroPowerScale = (Vector3) DeckBigCard.HERO_POWER_SCALE;
          if (this.m_flipHeroPowerHorizontalPosition)
            heroPowerPosition.x = -heroPowerPosition.x;
          iTween.MoveTo(this.m_shownHeroPowerActor.gameObject, iTween.Hash((object) "position", (object) heroPowerPosition, (object) "isLocal", (object) true, (object) "time", (object) DeckBigCard.HERO_POWER_TWEEN_TIME));
          iTween.ScaleTo(this.m_shownHeroPowerActor.gameObject, iTween.Hash((object) "scale", (object) heroPowerScale, (object) "isLocal", (object) true, (object) "time", (object) DeckBigCard.HERO_POWER_TWEEN_TIME));
        }
      }
    }
  }

  private void HideHeroPowerCard()
  {
    if (!((UnityEngine.Object) this.m_shownHeroPowerActor != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_shownHeroPowerActor.gameObject);
  }

  public delegate void OnBigCardShownHandler(Actor shownActor, EntityDef entityDef);
}
