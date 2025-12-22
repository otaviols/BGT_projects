using System.Collections.Generic;
using UnityEngine;

public class ZoneDeck : Zone
{
  public Actor m_ThicknessFull;
  public Transform m_TicknessFullTopOfDeck;
  public Actor m_Thickness75;
  public Transform m_Tickness75TopOfDeck;
  public Actor m_Thickness50;
  public Transform m_Tickness50TopOfDeck;
  public Actor m_Thickness25;
  public Transform m_Tickness25TopOfDeck;
  public Actor m_Thickness1;
  public Transform m_Tickness1TopOfDeck;
  public Spell m_DeckFatigueGlow;
  public Spell m_DeckTradeableGlow;
  public DeckCover m_DeckCover;
  public GameObject m_DeckVisualRootObject;
  public TooltipZone m_deckTooltipZone;
  public TooltipZone m_playerHandTooltipZone;
  public TooltipZone m_playerManaTooltipZone;
  private const int MAX_THICKNESS_CARD_COUNT = 26;
  private bool m_suppressEmotes;
  private bool m_warnedAboutLastCard;
  private bool m_warnedAboutNoCards;
  private bool m_wasFatigued;
  private int m_numCardAnimating;
  private int m_numDefaultHandToDeckAnimation;
  private readonly Dictionary<Actor, Mesh> m_originalDeckMeshes = new Dictionary<Actor, Mesh>();
  private bool m_hasDirtyDeckMeshes;

  public void Awake()
  {
    if ((Object) this.m_deckTooltipZone != (Object) null && (Object) this.m_playerHandTooltipZone != (Object) null)
      this.m_deckTooltipZone.SetTooltipChangeCallback(new TooltipZone.TooltipChangeCallback(this.TooltipChanged));
    if ((Object) this.m_DeckCover != (Object) null)
      this.m_DeckCover.SetDeckVisualRootObject(this.m_DeckVisualRootObject);
    this.CacheOriginalDeckMeshes();
  }

  public void TooltipChanged(bool shown)
  {
    if (shown)
      return;
    this.m_playerHandTooltipZone.HideTooltip();
    if (!((Object) this.m_playerManaTooltipZone != (Object) null))
      return;
    this.m_playerManaTooltipZone.HideTooltip();
  }

  public override bool CanAcceptTags(
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    return gameEntity != null && gameEntity.OverwriteZoneDeckToAcceptEntity(this, controllerId, zoneTag, cardType, entity) || base.CanAcceptTags(controllerId, zoneTag, cardType, entity);
  }

  public override void Reset()
  {
    base.Reset();
    this.UpdateLayout();
  }

  public override void UpdateLayout()
  {
    ++this.m_updatingLayout;
    if (this.IsBlockingLayout())
    {
      this.UpdateLayoutFinished();
    }
    else
    {
      this.UpdateThickness();
      this.UpdateDeckStateEmotes();
      if ((Object) this.m_DeckCover != (Object) null)
        this.m_DeckCover.UpdateVisual(this.m_Side);
      for (int index = 0; index < this.m_cards.Count; ++index)
      {
        Card card = this.m_cards[index];
        if (!card.IsDoNotSort())
        {
          card.HideCard();
          this.SetCardToInDeckState(card);
        }
      }
      this.UpdateLayoutFinished();
    }
  }

  public override void OnHealingDoesDamageEntityEnteredPlay()
  {
  }

  public override void OnHealingDoesDamageEntityMousedOut()
  {
  }

  public override void OnHealingDoesDamageEntityMousedOver()
  {
  }

  public override void OnLifestealDoesDamageEntityEnteredPlay()
  {
  }

  public override void OnLifestealDoesDamageEntityMousedOut()
  {
  }

  public override void OnLifestealDoesDamageEntityMousedOver()
  {
  }

  public void SetVisibility(bool visible) => this.gameObject.SetActive(visible);

  public bool GetVisibility() => this.gameObject.activeSelf;

  public void SetCardToInDeckState(Card card)
  {
    Transform boneForThickness = this.GetTopOfDeckBoneForThickness();
    card.transform.localEulerAngles = new Vector3(275f, 270f, 0.0f);
    card.transform.position = boneForThickness.position;
    card.transform.localScale = new Vector3(0.88f, 0.88f, 0.88f);
    card.EnableTransitioningZones(false);
  }

  public void DoFatigueGlow()
  {
    if ((Object) this.m_DeckFatigueGlow == (Object) null || !GameState.Get().GetBooleanGameOption(GameEntityOption.ALLOW_FATIGUE) || this.m_cards.Count > 0)
      return;
    this.m_DeckFatigueGlow.ActivateState(SpellStateType.ACTION);
  }

  public void ShowTradeableGlow() => SpellUtils.ActivateBirthIfNecessary(this.m_DeckTradeableGlow);

  public void HideTradeableGlow(bool justTraded = false)
  {
    if ((Object) this.m_DeckTradeableGlow == (Object) null)
      return;
    if (justTraded)
    {
      SpellUtils.ActivateDeathIfNecessary(this.m_DeckTradeableGlow);
    }
    else
    {
      if (this.m_DeckTradeableGlow.GetActiveState() == SpellStateType.DEATH)
        return;
      SpellUtils.ActivateCancelIfNecessary(this.m_DeckTradeableGlow);
    }
  }

  public void NotifyCardAnimationStart()
  {
    ++this.m_numCardAnimating;
    if (!((Object) this.m_DeckCover != (Object) null))
      return;
    this.m_DeckCover.OpenDeckCover();
  }

  public void NotifyCardAnimationFinish()
  {
    --this.m_numCardAnimating;
    if (this.m_numCardAnimating > 0 || !((Object) this.m_DeckCover != (Object) null))
      return;
    this.m_DeckCover.CloseDeckCover();
  }

  public void IncrementDefaultHandToDeckAnimationCount() => ++this.m_numDefaultHandToDeckAnimation;

  public void DecrementDefaultHandToDeckAnimationCount() => --this.m_numDefaultHandToDeckAnimation;

  public int GetDefaultHandToDeckAnimationCount() => this.m_numDefaultHandToDeckAnimation;

  public bool IsFatigued() => this.m_cards.Count == 0;

  public Transform GetTopOfDeckBoneForThickness()
  {
    Actor thicknessForLayout = this.GetThicknessForLayout();
    if ((Object) thicknessForLayout == (Object) this.m_ThicknessFull)
      return this.m_TicknessFullTopOfDeck;
    if ((Object) thicknessForLayout == (Object) this.m_Thickness75)
      return this.m_Tickness75TopOfDeck;
    if ((Object) thicknessForLayout == (Object) this.m_Thickness50)
      return this.m_Tickness50TopOfDeck;
    if ((Object) thicknessForLayout == (Object) this.m_Thickness25)
      return this.m_Tickness25TopOfDeck;
    int num = (Object) thicknessForLayout == (Object) this.m_Thickness1 ? 1 : 0;
    return this.m_Tickness1TopOfDeck;
  }

  public Actor GetActiveThickness()
  {
    if (this.m_ThicknessFull.GetMeshRenderer().enabled)
      return this.m_ThicknessFull;
    if (this.m_Thickness75.GetMeshRenderer().enabled)
      return this.m_Thickness75;
    if (this.m_Thickness50.GetMeshRenderer().enabled)
      return this.m_Thickness50;
    if (this.m_Thickness25.GetMeshRenderer().enabled)
      return this.m_Thickness25;
    return this.m_Thickness1.GetMeshRenderer().enabled ? this.m_Thickness1 : (Actor) null;
  }

  public Actor GetThicknessForLayout()
  {
    Actor activeThickness = this.GetActiveThickness();
    return (Object) activeThickness != (Object) null ? activeThickness : this.m_Thickness1;
  }

  public bool AreEmotesSuppressed() => this.m_suppressEmotes;

  public void SetSuppressEmotes(bool suppress) => this.m_suppressEmotes = suppress;

  private void UpdateThickness()
  {
    this.m_ThicknessFull.GetMeshRenderer().enabled = false;
    this.m_Thickness75.GetMeshRenderer().enabled = false;
    this.m_Thickness50.GetMeshRenderer().enabled = false;
    this.m_Thickness25.GetMeshRenderer().enabled = false;
    this.m_Thickness1.GetMeshRenderer().enabled = false;
    int count = this.m_cards.Count;
    if (count == 0)
    {
      if (this.m_wasFatigued || !GameState.Get().GetBooleanGameOption(GameEntityOption.ALLOW_FATIGUE))
        return;
      this.m_DeckFatigueGlow.ActivateState(SpellStateType.BIRTH);
      this.m_wasFatigued = true;
    }
    else
    {
      if (this.m_wasFatigued && GameState.Get().GetBooleanGameOption(GameEntityOption.ALLOW_FATIGUE))
      {
        this.m_DeckFatigueGlow.ActivateState(SpellStateType.DEATH);
        this.m_wasFatigued = false;
      }
      if (count == 1)
      {
        this.m_Thickness1.GetMeshRenderer().enabled = true;
      }
      else
      {
        float num = (float) count / 26f;
        if ((double) num > 0.75)
          this.m_ThicknessFull.GetMeshRenderer().enabled = true;
        else if ((double) num > 0.5)
          this.m_Thickness75.GetMeshRenderer().enabled = true;
        else if ((double) num > 0.25)
        {
          this.m_Thickness50.GetMeshRenderer().enabled = true;
        }
        else
        {
          if ((double) num <= 0.0)
            return;
          this.m_Thickness25.GetMeshRenderer().enabled = true;
        }
      }
    }
  }

  public void UpdateToCustomDeckMeshes(CardBack.CustomDeckMeshes customMeshes)
  {
    ZoneDeck.TryUpdateDeckMesh(this.m_ThicknessFull, customMeshes.ThicknessFull);
    ZoneDeck.TryUpdateDeckMesh(this.m_Thickness75, customMeshes.Thickness75);
    ZoneDeck.TryUpdateDeckMesh(this.m_Thickness50, customMeshes.Thickness50);
    ZoneDeck.TryUpdateDeckMesh(this.m_Thickness25, customMeshes.Thickness25);
    ZoneDeck.TryUpdateDeckMesh(this.m_Thickness1, customMeshes.Thickness1);
    this.m_hasDirtyDeckMeshes = true;
  }

  public void TryRestoreOriginalDeckMeshes()
  {
    if (!this.m_hasDirtyDeckMeshes)
      return;
    foreach (KeyValuePair<Actor, Mesh> originalDeckMesh in this.m_originalDeckMeshes)
      ZoneDeck.TryUpdateDeckMesh(originalDeckMesh.Key, originalDeckMesh.Value);
    this.m_hasDirtyDeckMeshes = false;
  }

  private void UpdateDeckStateEmotes()
  {
    if (!GameState.Get().IsPastBeginPhase() || this.m_suppressEmotes || GameState.Get().GetGameEntity().HasTag(GAME_TAG.HIDE_OUT_OF_CARDS_WARNING))
      return;
    int count = this.m_cards.Count;
    if (count <= 0 && !this.m_warnedAboutNoCards)
    {
      this.m_warnedAboutNoCards = true;
      this.m_warnedAboutLastCard = true;
      this.m_controller.GetHeroCard()?.PlayEmote(EmoteType.NOCARDS);
    }
    else if (count == 1 && !this.m_warnedAboutLastCard)
    {
      this.m_warnedAboutLastCard = true;
      this.m_controller.GetHeroCard()?.PlayEmote(EmoteType.LOWCARDS);
    }
    else
    {
      if (this.m_warnedAboutLastCard && count > 1)
        this.m_warnedAboutLastCard = false;
      if (!this.m_warnedAboutNoCards || count <= 0)
        return;
      this.m_warnedAboutNoCards = false;
    }
  }

  private void CacheOriginalDeckMeshes()
  {
    ZoneDeck.TryAppendDeckMeshToCache(this.m_ThicknessFull, this.m_originalDeckMeshes);
    ZoneDeck.TryAppendDeckMeshToCache(this.m_Thickness75, this.m_originalDeckMeshes);
    ZoneDeck.TryAppendDeckMeshToCache(this.m_Thickness50, this.m_originalDeckMeshes);
    ZoneDeck.TryAppendDeckMeshToCache(this.m_Thickness25, this.m_originalDeckMeshes);
    ZoneDeck.TryAppendDeckMeshToCache(this.m_Thickness1, this.m_originalDeckMeshes);
  }

  private static void TryUpdateDeckMesh(Actor meshActor, Mesh newMesh)
  {
    if ((Object) meshActor == (Object) null)
      return;
    if ((Object) newMesh == (Object) null)
    {
      Debug.LogWarning((object) "ZoneDeck failed to update deck mesh as new mesh was null!");
    }
    else
    {
      MeshFilter componentInChildren = meshActor.GetComponentInChildren<MeshFilter>();
      if ((Object) componentInChildren == (Object) null)
        Debug.LogWarning((object) ("ZoneDeck failed to update deck mesh for " + meshActor.name + " as it couldn't find original mesh!"));
      else
        componentInChildren.mesh = newMesh;
    }
  }

  private static void TryAppendDeckMeshToCache(Actor actor, Dictionary<Actor, Mesh> collection)
  {
    MeshFilter componentInChildren = actor.GetComponentInChildren<MeshFilter>();
    if ((Object) componentInChildren == (Object) null)
      return;
    collection[actor] = componentInChildren.mesh;
  }
}
