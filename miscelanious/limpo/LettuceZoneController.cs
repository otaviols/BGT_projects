using System.Collections.Generic;
using UnityEngine;

public class LettuceZoneController
{
  private const string ABILITY_TRAY_PREFAB = "MercenariesAbilityTray.prefab:bf65ec0d425616a40a16734ff75c32b1";
  private const string ABILITY_TRAY_BONE = "MercenariesAbilityTray";
  private GameState m_gameState;
  private InputManager m_inputManager;
  private Entity m_lettuceAbilitiesSourceEntity;
  private Card m_previouslySelectedMercenaryCard;
  private List<Card> m_displayedAbilityCards = new List<Card>();
  private MercenariesAbilityTray m_abilityTray;

  public LettuceZoneController(GameState gameState, InputManager inputManager)
  {
    this.m_gameState = gameState;
    this.m_inputManager = inputManager;
  }

  public Entity GetLettuceAbilitiesSourceEntity() => this.m_lettuceAbilitiesSourceEntity;

  public List<Card> GetDisplayedLettuceAbilityCards() => this.m_displayedAbilityCards;

  public void DisplayLettuceAbilitiesForPreviouslySelectedCard()
  {
    if (!((Object) this.m_previouslySelectedMercenaryCard != (Object) null))
      return;
    this.DisplayLettuceAbilitiesForEntity(this.m_previouslySelectedMercenaryCard.GetEntity());
  }

  public void DisplayLettuceAbilitiesForEntity(Entity entity)
  {
    if (this.m_lettuceAbilitiesSourceEntity == entity || !(this.m_gameState.GetGameEntity() is LettuceMissionEntity gameEntity))
      return;
    if (entity != null)
      gameEntity.SetPrevSelectedCharacterZonePosition(entity.GetZonePosition());
    this.ClearDisplayedLettuceAbilities();
    if (entity == null || !this.IsAllowedToShowAbilityTray())
      return;
    gameEntity.ShowWeaknessSplatsForMercenary(entity);
    Card card1 = (Card) null;
    foreach (int lettuceAbilityEntityId in entity.GetLettuceAbilityEntityIDs())
    {
      Card card2 = this.m_gameState.GetEntity(lettuceAbilityEntityId)?.GetCard();
      if (entity.GetSelectedLettuceAbilityID() == card2.GetEntity().GetEntityId())
      {
        card1 = card2;
        break;
      }
    }
    this.m_displayedAbilityCards = new List<Card>();
    foreach (int lettuceAbilityEntityId in entity.GetLettuceAbilityEntityIDs())
    {
      Entity entity1 = this.m_gameState.GetEntity(lettuceAbilityEntityId);
      if (entity1 != null && !entity1.IsLettuceEquipment())
      {
        Card card3 = entity1.GetCard();
        if ((Object) card3 != (Object) null)
          this.m_displayedAbilityCards.Add(card3);
      }
    }
    this.ShowAbilityTray(entity, this.m_displayedAbilityCards);
    if ((Object) card1 != (Object) null)
    {
      Entity entity2 = this.m_gameState.GetEntity(entity.GetTag(GAME_TAG.LETTUCE_SELECTED_TARGET));
      if (entity2 != null)
      {
        TargetReticleManager.Get().CreateStaticTargetArrow(entity, entity2);
        TargetReticleManager.Get().SetTargetArrowLinkLayer(GameLayer.Default);
        TargetReticleManager.Get().SetParabolaHeight(0.4f);
      }
    }
    this.m_lettuceAbilitiesSourceEntity = entity;
    entity.GetCard().UpdateSelectedLettuceCharacterVisual();
  }

  public void ClearDisplayedLettuceAbilities(bool hideWeaknessSplats = true, bool cachePreviouslySelected = false)
  {
    Card card = (Card) null;
    this.m_previouslySelectedMercenaryCard = (Card) null;
    if (this.m_lettuceAbilitiesSourceEntity != null)
    {
      card = this.m_lettuceAbilitiesSourceEntity.GetCard();
      if (cachePreviouslySelected)
        this.m_previouslySelectedMercenaryCard = card;
      this.HideAbilityTray();
      TargetReticleManager.Get().DestroyStaticTargetArrow();
    }
    if (hideWeaknessSplats && this.m_gameState.GetGameEntity() is LettuceMissionEntity gameEntity)
      gameEntity.HideWeaknessSplats();
    this.m_lettuceAbilitiesSourceEntity = (Entity) null;
    card?.UpdateSelectedLettuceCharacterVisual();
  }

  private void CreateAbilityTray()
  {
    this.m_abilityTray = AssetLoader.Get().InstantiatePrefab((AssetReference) "MercenariesAbilityTray.prefab:bf65ec0d425616a40a16734ff75c32b1").GetComponent<MercenariesAbilityTray>();
    Transform bone = Gameplay.Get().GetBoardLayout().FindBone("MercenariesAbilityTray");
    if (!((Object) bone != (Object) null))
      return;
    this.m_abilityTray.transform.position = bone.position;
  }

  private void ShowAbilityTray(Entity entity, List<Card> abilityCards)
  {
    if ((Object) this.m_abilityTray == (Object) null)
      this.CreateAbilityTray();
    this.m_abilityTray.SetupForMercenary(entity, abilityCards);
    if (this.m_gameState.GetGameEntity() is LettuceMissionEntity gameEntity)
    {
      gameEntity.UpdateAllMercenaryAbilityOrderBubbleText(true);
      gameEntity.OnAbilityTrayShown(entity);
    }
    this.m_abilityTray.Show();
  }

  private void HideAbilityTray()
  {
    if ((Object) this.m_abilityTray == (Object) null)
      return;
    if (this.m_gameState.GetGameEntity() is LettuceMissionEntity gameEntity)
    {
      gameEntity.ShowAllMercenaryAbilityOrderBubbles();
      gameEntity.UpdateAllMercenaryAbilityOrderBubbleText();
      gameEntity.OnAbilityTrayDismissed();
    }
    this.m_abilityTray.Hide();
  }

  private bool IsAllowedToShowAbilityTray() => !this.m_gameState.IsResponsePacketBlocked();

  public MercenariesAbilityTray GetAbilityTray() => this.m_abilityTray;
}
