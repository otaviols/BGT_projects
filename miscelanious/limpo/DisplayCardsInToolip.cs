using System.Collections.Generic;
using UnityEngine;

public class DisplayCardsInToolip : MonoBehaviour
{
  public static readonly float APPEARANCE_DELAY = 0.55f;
  public static readonly float APPEARANCE_DURATION = 0.125f;
  public static readonly float CARD_SCALE = 1.3f;
  public static readonly float CARD_SCALE_PHONE_WITH_HERO_BUDDY = 0.94f;
  public static readonly Vector3 s_leftOffset = new Vector3(-3.1f, -0.1f, 0.0f);
  public static readonly Vector3 s_rightOffset = new Vector3(3.1f, -0.1f, 0.0f);
  public static readonly Vector3 s_additionalOffsetDuringMulligan = new Vector3(0.0f, 0.4f, 0.0f);
  public static readonly Vector3 s_offsetExtraCardLeft = new Vector3(-2.9f, 0.0f, 0.0f);
  public static readonly Vector3 s_offsetExtraCardRight = new Vector3(2.9f, 0.0f, 0.0f);
  public static readonly Vector3 s_leftOffsetPhone = new Vector3(-2.6f, -0.1f, 0.0f);
  public static readonly Vector3 s_rightOffsetPhone = new Vector3(2.6f, -0.1f, 0.0f);
  public static readonly Vector3 s_additionalOffsetDuringMulliganPhone = new Vector3(0.0f, 0.4f, 0.0f);
  public static readonly Vector3 s_offsetExtraCardLeftPhone = new Vector3(-2.1f, 0.0f, 0.0f);
  public static readonly Vector3 s_offsetExtraCardRightPhone = new Vector3(2.1f, 0.0f, 0.0f);
  public static readonly Vector3 s_additionalOffsetColossalLeftPhone = new Vector3(-0.4f, 0.0f, 0.0f);
  public static readonly Vector3 s_additionalOffsetColossalRightPhone = new Vector3(0.4f, 0.0f, 0.0f);
  private List<Actor> m_cardActors = new List<Actor>();
  private Card m_ownerCard;

  public void NotifyMousedOver() => this.ShowCardsInTooltipAfterDelay();

  public void NotifyMousedOut() => this.HideCardsInTooltipActor();

  public void NotifyPickedUp() => this.HideCardsInTooltipActor();

  private void OnDestroy()
  {
    foreach (Actor cardActor in this.m_cardActors)
    {
      if ((Object) cardActor != (Object) null && (Object) cardActor.gameObject != (Object) null)
        cardActor.Destroy();
    }
    this.m_cardActors.Clear();
    this.m_ownerCard = (Card) null;
  }

  public void Setup(Card ownerCard)
  {
    if ((Object) ownerCard == (Object) null || ownerCard.GetEntity() == null)
      Log.Spells.PrintError("DisplayCardsInToolip.Setup(): Invalid card was passed in.");
    else
      this.m_ownerCard = ownerCard;
  }

  public void AddCardsInTooltip(int cardID)
  {
    Entity entity = this.m_ownerCard.GetEntity();
    using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardID))
    {
      if (fullDef?.EntityDef == null || (Object) fullDef?.CardDef == (Object) null)
      {
        Log.Spells.PrintError("CardsInTooltip.Setup(): Unable to load def for card ID {0}.", (object) cardID);
      }
      else
      {
        GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(fullDef.EntityDef, entity.GetPremiumType()), AssetLoadingOptions.IgnorePrefabPosition);
        if ((Object) gameObject == (Object) null)
        {
          Log.Spells.PrintError("AddCardsInTooltip(): Unable to load Hand Actor for entity def {0}.", (object) fullDef.EntityDef);
        }
        else
        {
          Actor componentInChildren = gameObject.GetComponentInChildren<Actor>();
          LayerUtils.SetLayer((Component) componentInChildren, GameLayer.Tooltip);
          componentInChildren.SetFullDef(fullDef);
          componentInChildren.SetPremium(entity.GetPremiumType());
          componentInChildren.SetCardBackSideOverride(new Player.Side?(entity.GetControllerSide()));
          componentInChildren.SetWatermarkCardSetOverride(entity.GetWatermarkCardSetOverride());
          componentInChildren.UpdateAllComponents();
          if (componentInChildren.UseCoinManaGem())
            componentInChildren.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
          if (fullDef.EntityDef.GetTag(GAME_TAG.BACON_BUDDY) != 0)
            componentInChildren.ActivateSpellBirthState(SpellType.GHOSTMODE);
          componentInChildren.Hide();
          componentInChildren.gameObject.SetActive(false);
          this.m_cardActors.Add(componentInChildren);
        }
      }
    }
  }

  private void Update()
  {
    if (iTween.HasName(this.gameObject, "Appearing"))
      return;
    int offsetIndex = 0;
    foreach (Actor cardActor in this.m_cardActors)
    {
      if ((Object) cardActor != (Object) null && cardActor.IsShown() && cardActor.gameObject.activeSelf)
      {
        cardActor.transform.position = this.gameObject.transform.position + this.GetDesiredOffset(offsetIndex);
        ++offsetIndex;
      }
    }
  }

  private bool HasRelatedCards()
  {
    Entity entity = this.m_ownerCard.GetEntity();
    if (entity == null)
      return false;
    return entity.HasTag(GAME_TAG.COLOSSAL) || entity.HasTag(GAME_TAG.DISPLAY_CARD_ON_MOUSEOVER);
  }

  private bool IsColossalLimbOnTheLeft()
  {
    foreach (Actor cardActor in this.m_cardActors)
    {
      Entity entity = cardActor.GetEntity();
      EntityDef entityDef = cardActor.GetEntityDef();
      if (entityDef != null && entityDef.HasTag(GAME_TAG.COLOSSAL_LIMB_ON_LEFT) || entity != null && entity.HasTag(GAME_TAG.COLOSSAL_LIMB_ON_LEFT))
        return true;
    }
    return false;
  }

  private Vector3 GetDesiredOffset(int offsetIndex)
  {
    int num = this.HasRelatedCards() ? 1 : 0;
    bool flag = this.IsColossalLimbOnTheLeft();
    Vector3 vector3_1 = Vector3.zero;
    Vector3 vector3_2 = Vector3.zero;
    if (GameState.Get().IsMulliganManagerActive() && GameState.Get().GetBooleanGameOption(GameEntityOption.CARDS_IN_TOOLTIP_SHIFTED_DURING_MULLIGAN))
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        vector3_1 = DisplayCardsInToolip.s_additionalOffsetDuringMulliganPhone + new Vector3(-1f, 0.0f, 0.0f);
        vector3_2 = DisplayCardsInToolip.s_additionalOffsetDuringMulliganPhone + new Vector3(1f, 0.0f, 0.0f);
      }
      else
      {
        vector3_1 = DisplayCardsInToolip.s_additionalOffsetDuringMulligan + new Vector3(-1f, 0.0f, 0.0f);
        vector3_2 = DisplayCardsInToolip.s_additionalOffsetDuringMulligan + new Vector3(1f, 0.0f, 0.0f);
      }
    }
    if (num != 0)
      return flag ? ((bool) UniversalInputManager.UsePhoneUI ? DisplayCardsInToolip.s_leftOffsetPhone + vector3_2 + DisplayCardsInToolip.s_additionalOffsetColossalLeftPhone + (float) offsetIndex * DisplayCardsInToolip.s_offsetExtraCardLeftPhone : DisplayCardsInToolip.s_leftOffset + vector3_2 + (float) offsetIndex * DisplayCardsInToolip.s_offsetExtraCardLeft) : ((bool) UniversalInputManager.UsePhoneUI ? DisplayCardsInToolip.s_rightOffsetPhone + vector3_1 + DisplayCardsInToolip.s_additionalOffsetColossalRightPhone + (float) offsetIndex * DisplayCardsInToolip.s_offsetExtraCardRightPhone : DisplayCardsInToolip.s_rightOffset + vector3_1 + (float) offsetIndex * DisplayCardsInToolip.s_offsetExtraCardRight);
    ZoneHand zone = this.m_ownerCard.GetZone() as ZoneHand;
    return (Object) zone != (Object) null && !zone.ShouldShowCardTooltipOnRight(this.m_ownerCard) ? ((bool) UniversalInputManager.UsePhoneUI ? DisplayCardsInToolip.s_leftOffsetPhone + vector3_2 + (float) offsetIndex * DisplayCardsInToolip.s_offsetExtraCardLeftPhone : DisplayCardsInToolip.s_leftOffset + vector3_2 + (float) offsetIndex * DisplayCardsInToolip.s_offsetExtraCardLeft) : ((bool) UniversalInputManager.UsePhoneUI ? DisplayCardsInToolip.s_rightOffsetPhone + vector3_1 + (float) offsetIndex * DisplayCardsInToolip.s_offsetExtraCardRightPhone : DisplayCardsInToolip.s_rightOffset + vector3_1 + (float) offsetIndex * DisplayCardsInToolip.s_offsetExtraCardRight);
  }

  private void OnCardsInToolipAppearUpdate(float newValue)
  {
    int offsetIndex = 0;
    bool flag = false;
    foreach (Actor cardActor in this.m_cardActors)
    {
      Entity entity = cardActor.GetEntity();
      EntityDef entityDef = cardActor.GetEntityDef();
      if (entityDef != null && entityDef.GetTag(GAME_TAG.BACON_BUDDY) != 0 || entity != null && entity.GetTag(GAME_TAG.BACON_BUDDY) != 0)
        flag = true;
    }
    float num1 = DisplayCardsInToolip.CARD_SCALE;
    if ((bool) UniversalInputManager.UsePhoneUI & flag)
      num1 = DisplayCardsInToolip.CARD_SCALE_PHONE_WITH_HERO_BUDDY;
    foreach (Actor cardActor in this.m_cardActors)
    {
      if ((Object) cardActor == (Object) null)
        break;
      if (!cardActor.gameObject.activeSelf)
        cardActor.gameObject.SetActive(true);
      if (!cardActor.IsShown())
        cardActor.Show();
      float num2 = num1 * newValue;
      cardActor.transform.localScale = new Vector3(num2, num2, num2);
      cardActor.transform.position = this.gameObject.transform.position + this.GetDesiredOffset(offsetIndex) * newValue;
      ++offsetIndex;
    }
  }

  private void ShowCardsInTooltipAfterDelay()
  {
    foreach (Actor cardActor in this.m_cardActors)
    {
      if ((Object) cardActor == (Object) null)
        return;
      if (!cardActor.gameObject.activeSelf)
        cardActor.gameObject.SetActive(true);
      if ((cardActor.GetEntityDef() == null || cardActor.GetEntityDef().GetTag(GAME_TAG.BACON_BUDDY) == 0 ? (cardActor.GetEntity() == null ? 0 : (cardActor.GetEntity().GetTag(GAME_TAG.BACON_BUDDY) != 0 ? 1 : 0)) : 1) != 0)
        cardActor.ActivateSpellBirthState(SpellType.GHOSTMODE);
      if (cardActor.UseTechLevelManaGem())
      {
        Spell spell = cardActor.GetSpell(SpellType.TECH_LEVEL_MANA_GEM);
        if ((Object) spell != (Object) null)
        {
          spell.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("TechLevel").Value = cardActor.GetEntityDef().GetTechLevel();
          spell.ActivateState(SpellStateType.BIRTH);
        }
      }
      else if (cardActor.UseCoinManaGem())
        cardActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
      cardActor.SetUnlit();
    }
    iTween.StopByName(this.gameObject, "Appearing");
    iTween.ValueTo(this.gameObject, iTween.Hash((object) "onupdatetarget", (object) this.gameObject, (object) "onupdate", (object) "OnCardsInToolipAppearUpdate", (object) "time", (object) DisplayCardsInToolip.APPEARANCE_DURATION, (object) "delay", (object) (float) (GameState.Get().GetGameEntity().ShouldDelayShowingCardInTooltip() ? (double) DisplayCardsInToolip.APPEARANCE_DELAY : 0.0), (object) "to", (object) 1f, (object) "from", (object) 0.0f, (object) "name", (object) "Appearing"));
  }

  private void HideCardsInTooltipActor()
  {
    foreach (Actor cardActor in this.m_cardActors)
    {
      if ((Object) cardActor == (Object) null)
        return;
      cardActor.Hide();
      cardActor.gameObject.SetActive(false);
    }
    this.OnCardsInToolipAppearUpdate(0.0f);
    iTween.StopByName(this.gameObject, "Appearing");
  }
}
