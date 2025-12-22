using System.Collections.Generic;
using UnityEngine;

[CustomEditClass(DefaultCollapsed = true)]
internal class TagVisualConfiguration : MonoBehaviour
{
  [CustomEditField(SearchField = "m_Tag")]
  public List<TagVisualConfigurationEntry> m_TagVisuals = new List<TagVisualConfigurationEntry>();
  private static TagVisualConfiguration s_instance;

  public void Awake() => TagVisualConfiguration.s_instance = this;

  private void OnDestroy() => TagVisualConfiguration.s_instance = (TagVisualConfiguration) null;

  public static TagVisualConfiguration Get() => TagVisualConfiguration.s_instance;

  public void ActivateStateSpells(Card card)
  {
    if ((Object) card == (Object) null || (Object) card.GetActor() == (Object) null || card.GetEntity() == null || card.GetEntity().GetController() != null && !card.GetEntity().GetController().IsFriendlySide() && card.GetEntity().IsObfuscated())
      return;
    foreach (TagVisualConfigurationEntry tagVisual in this.m_TagVisuals)
    {
      TagVisualConfigurationEntry entry = tagVisual;
      if (tagVisual.m_ReferenceTag != GAME_TAG.TAG_NOT_SET)
        entry = this.FindTagEntry(tagVisual.m_ReferenceTag);
      if (entry != null && entry.m_IsPlayStateSpell)
      {
        TagDelta change = new TagDelta();
        change.tag = (int) tagVisual.m_Tag;
        change.oldValue = 0;
        change.newValue = card.GetEntity().GetTag(tagVisual.m_Tag);
        if (entry.m_BeforeAlways != null)
        {
          foreach (TagVisualActionConfiguration action in entry.m_BeforeAlways.m_Actions)
            this.ConditionallyExecuteAction(entry, action, card, false, change, false);
        }
        if (change.newValue > 0 && entry.m_TagAdded != null)
        {
          foreach (TagVisualActionConfiguration action in entry.m_TagAdded.m_Actions)
            this.ConditionallyExecuteAction(entry, action, card, false, change, false);
        }
        if (entry.m_AfterAlways != null)
        {
          foreach (TagVisualActionConfiguration action in entry.m_AfterAlways.m_Actions)
            this.ConditionallyExecuteAction(entry, action, card, false, change, false);
        }
      }
    }
  }

  public void ActivateHandStateSpells(Card card, bool forceActivate = false)
  {
    if ((Object) card == (Object) null || (Object) card.GetActor() == (Object) null || card.GetEntity() == null || card.GetEntity().GetController() != null && !card.GetEntity().GetController().IsFriendlySide() && card.GetEntity().IsObfuscated() || ((Object) card.GetZone() != (Object) null ? (int) card.GetZone().m_ServerTag : 6) != 3)
      return;
    foreach (TagVisualConfigurationEntry tagVisual in this.m_TagVisuals)
    {
      TagVisualConfigurationEntry entry = tagVisual;
      if (tagVisual.m_ReferenceTag != GAME_TAG.TAG_NOT_SET)
        entry = this.FindTagEntry(tagVisual.m_ReferenceTag);
      if (entry != null && entry.m_IsHandStateSpell)
      {
        TagDelta change = new TagDelta();
        change.tag = (int) tagVisual.m_Tag;
        change.oldValue = 0;
        change.newValue = card.GetEntity().GetTag(tagVisual.m_Tag);
        if (entry.m_BeforeAlways != null)
        {
          foreach (TagVisualActionConfiguration action in entry.m_BeforeAlways.m_Actions)
            this.ConditionallyExecuteAction(entry, action, card, false, change, false, forceActivate);
        }
        if (change.newValue > 0 && entry.m_TagAdded != null)
        {
          foreach (TagVisualActionConfiguration action in entry.m_TagAdded.m_Actions)
            this.ConditionallyExecuteAction(entry, action, card, false, change, false, forceActivate);
        }
        if (entry.m_AfterAlways != null)
        {
          foreach (TagVisualActionConfiguration action in entry.m_AfterAlways.m_Actions)
            this.ConditionallyExecuteAction(entry, action, card, false, change, false, forceActivate);
        }
      }
    }
  }

  public void DeactivateHandStateSpells(Card card, Actor actor)
  {
    if ((Object) card == (Object) null || (Object) actor == (Object) null || card.GetEntity() == null)
      return;
    foreach (TagVisualConfigurationEntry tagVisual in this.m_TagVisuals)
    {
      TagVisualConfigurationEntry entry = tagVisual;
      if (tagVisual.m_ReferenceTag != GAME_TAG.TAG_NOT_SET)
        entry = this.FindTagEntry(tagVisual.m_ReferenceTag);
      if (entry != null && entry.m_IsHandStateSpell)
      {
        TagDelta change = new TagDelta();
        change.tag = (int) tagVisual.m_Tag;
        change.oldValue = 0;
        change.newValue = card.GetEntity().GetTag(tagVisual.m_Tag);
        if (entry.m_BeforeAlways != null)
        {
          foreach (TagVisualActionConfiguration action in entry.m_BeforeAlways.m_Actions)
            this.ConditionallyExecuteAction(entry, action, card, false, change, false, false, actor);
        }
        if (entry.m_TagRemoved != null)
        {
          foreach (TagVisualActionConfiguration action in entry.m_TagRemoved.m_Actions)
            this.ConditionallyExecuteAction(entry, action, card, false, change, false, false, actor);
        }
        if (entry.m_AfterAlways != null)
        {
          foreach (TagVisualActionConfiguration action in entry.m_AfterAlways.m_Actions)
            this.ConditionallyExecuteAction(entry, action, card, false, change, false, false, actor);
        }
      }
    }
  }

  public void ProcessTagChange(GAME_TAG tag, Card card, bool fromShowEntity, TagDelta change)
  {
    TagVisualConfigurationEntry tagEntry = this.FindTagEntry(tag);
    if (tagEntry == null || (Object) card == (Object) null || !card.CanShowActorVisuals() && !tagEntry.m_IgnoreCanShowActorVisuals)
      return;
    if (tagEntry.m_ReferenceTag != GAME_TAG.TAG_NOT_SET)
    {
      tagEntry = this.FindTagEntry(tagEntry.m_ReferenceTag);
      if (tagEntry == null)
        return;
    }
    if (tagEntry.m_BeforeAlways != null)
    {
      foreach (TagVisualActionConfiguration action in tagEntry.m_BeforeAlways.m_Actions)
        this.ConditionallyExecuteAction(tagEntry, action, card, fromShowEntity, change);
    }
    if (change.newValue != 0 && change.oldValue == 0 && tagEntry.m_TagAdded != null)
    {
      foreach (TagVisualActionConfiguration action in tagEntry.m_TagAdded.m_Actions)
        this.ConditionallyExecuteAction(tagEntry, action, card, fromShowEntity, change);
    }
    else if (change.newValue == 0 && change.oldValue != 0 && tagEntry.m_TagRemoved != null)
    {
      foreach (TagVisualActionConfiguration action in tagEntry.m_TagRemoved.m_Actions)
        this.ConditionallyExecuteAction(tagEntry, action, card, fromShowEntity, change);
    }
    if (tagEntry.m_AfterAlways == null)
      return;
    foreach (TagVisualActionConfiguration action in tagEntry.m_AfterAlways.m_Actions)
      this.ConditionallyExecuteAction(tagEntry, action, card, fromShowEntity, change);
  }

  private void ConditionallyExecuteAction(
    TagVisualConfigurationEntry entry,
    TagVisualActionConfiguration actionConfig,
    Card card,
    bool fromShowEntity,
    TagDelta change,
    bool fromTagChange = true,
    bool forceActivate = true,
    Actor overrideActor = null)
  {
    if (actionConfig == null || (Object) card == (Object) null || !this.IsActionConditionMet(actionConfig.m_Condition, card, fromShowEntity, fromTagChange, overrideActor))
      return;
    this.ExecuteAction(actionConfig, card, change, forceActivate, overrideActor);
  }

  private void ExecuteAction(
    TagVisualActionConfiguration actionConfig,
    Card card,
    TagDelta change,
    bool forceActivate,
    Actor overrideActor)
  {
    if ((Object) card == (Object) null)
      return;
    switch (actionConfig.m_Action)
    {
      case TagVisualActorFunction.ACTIVATE_SPELL_STATE:
        this.ActivateSpellState(actionConfig.m_SpellType, actionConfig.m_SpellState, card, forceActivate, overrideActor);
        break;
      case TagVisualActorFunction.PLAY_SOUND_PREFAB:
        AssetReference prefabParameters = (AssetReference) actionConfig.m_PlaySoundPrefabParameters;
        if (prefabParameters == null)
          break;
        SoundManager.Get().LoadAndPlay(prefabParameters);
        break;
      case TagVisualActorFunction.ACTIVATE_STATE_FUNCTION:
        this.ActivateStateFunction(actionConfig.m_StateFunctionParameters, card, true, change);
        break;
      case TagVisualActorFunction.DEACTIVATE_STATE_FUNCTION:
        this.ActivateStateFunction(actionConfig.m_StateFunctionParameters, card, false, change);
        break;
      case TagVisualActorFunction.UPDATE_ACTOR:
        card.UpdateActor();
        break;
      case TagVisualActorFunction.UPDATE_ACTOR_COMPONENTS:
        if ((Object) overrideActor != (Object) null)
        {
          overrideActor.UpdateAllComponents();
          break;
        }
        card.UpdateActorComponents();
        break;
      case TagVisualActorFunction.UPDATE_SIDEQUEST_UI:
        card.UpdateSideQuestUI(false);
        break;
      case TagVisualActorFunction.UPDATE_QUEST_UI:
        card.UpdateQuestUI();
        break;
      case TagVisualActorFunction.UPDATE_PUZZLE_UI:
        card.UpdatePuzzleUI();
        break;
      case TagVisualActorFunction.UPDATE_HERO_POWER_VISUALS:
        card.UpdateHeroPowerRelatedVisual();
        break;
      case TagVisualActorFunction.UPDATE_TEXT_COMPONENTS:
        Actor actor1 = card.GetActor();
        if ((Object) overrideActor != (Object) null)
          actor1 = overrideActor;
        if (!((Object) actor1 != (Object) null))
          break;
        actor1.UpdateTextComponents();
        break;
      case TagVisualActorFunction.UPDATE_BAUBLE:
        card.UpdateBauble();
        break;
      case TagVisualActorFunction.UPDATE_ATTACHED_CARD_BAUBLE:
        if (card.GetEntity() == null)
          break;
        Entity entity1 = GameState.Get().GetEntity(card.GetEntity().GetAttached());
        if (entity1 == null || !((Object) entity1.GetCard() != (Object) null))
          break;
        entity1.GetCard().UpdateBauble();
        break;
      case TagVisualActorFunction.ACTIVATE_LIFETIME_EFFECTS:
        card.ActivateLifetimeEffects();
        break;
      case TagVisualActorFunction.DEACTIVATE_LIFETIME_EFFECTS:
        card.DeactivateLifetimeEffects();
        break;
      case TagVisualActorFunction.CANCEL_ACTIVE_SPELLS:
        card.CancelActiveSpells();
        break;
      case TagVisualActorFunction.ACTIVATE_CUSTOM_KEYWORD_EFFECT:
        card.ActivateCustomKeywordEffect();
        break;
      case TagVisualActorFunction.DEACTIVATE_CUSTOM_KEYWORD_EFFECT:
        card.DeactivateCustomKeywordEffect();
        break;
      case TagVisualActorFunction.ACTIVATE_STATE_SPELLS:
        card.ActivateStateSpells();
        break;
      case TagVisualActorFunction.SPELL_POWER_MOUSE_OVER_EVENT:
        Entity entity2 = card.GetEntity();
        if (entity2 == null)
        {
          ZoneMgr.Get().OnSpellPowerEntityMousedOver();
          break;
        }
        ZoneMgr.Get().OnSpellPowerEntityMousedOver(entity2.GetSpellPowerSchool());
        break;
      case TagVisualActorFunction.SPELL_POWER_MOUSE_OUT_EVENT:
        Entity entity3 = card.GetEntity();
        if (entity3 == null)
        {
          ZoneMgr.Get().OnSpellPowerEntityMousedOut();
          break;
        }
        ZoneMgr.Get().OnSpellPowerEntityMousedOut(entity3.GetSpellPowerSchool());
        break;
      case TagVisualActorFunction.HEALING_DOES_DAMAGE_MOUSE_OVER_EVENT:
        ZoneMgr.Get().OnHealingDoesDamageEntityMousedOver();
        break;
      case TagVisualActorFunction.HEALING_DOES_DAMAGE_MOUSE_OUT_EVENT:
        ZoneMgr.Get().OnHealingDoesDamageEntityMousedOut();
        break;
      case TagVisualActorFunction.LIFESTEAL_DOES_DAMAGE_MOUSE_OVER_EVENT:
        ZoneMgr.Get().OnLifestealDoesDamageEntityMousedOver();
        break;
      case TagVisualActorFunction.LIFESTEAL_DOES_DAMAGE_MOUSE_OUT_EVENT:
        ZoneMgr.Get().OnLifestealDoesDamageEntityMousedOut();
        break;
      case TagVisualActorFunction.UPDATE_WATERMARK:
        Actor actor2 = card.GetActor();
        if ((Object) overrideActor != (Object) null)
          actor2 = overrideActor;
        Entity entity4 = card.GetEntity();
        if (!((Object) actor2 != (Object) null) || entity4 == null)
          break;
        actor2.SetWatermarkCardSetOverride(entity4.GetWatermarkCardSetOverride());
        actor2.UpdateMeshComponents();
        break;
      case TagVisualActorFunction.UPDATE_QUESTLINE_UI:
        card.UpdateQuestlineUI();
        break;
      case TagVisualActorFunction.UPDATE_ACTOR_STATE:
        card.UpdateActorState();
        break;
    }
  }

  private void ActivateStateFunction(
    TagVisualActorStateFunction stateFunction,
    Card card,
    bool isActive,
    TagDelta change)
  {
    if ((Object) card == (Object) null || (Object) card.GetActor() == (Object) null)
      return;
    switch (stateFunction)
    {
      case TagVisualActorStateFunction.TAUNT:
        if (isActive)
        {
          card.GetActor().ActivateTaunt();
          break;
        }
        card.GetActor().DeactivateTaunt();
        break;
      case TagVisualActorStateFunction.DEATHRATTLE:
        card.ToggleDeathrattle(isActive);
        break;
      case TagVisualActorStateFunction.EXHAUSTED:
        card.HandleCardExhaustedTagChanged(change);
        break;
      case TagVisualActorStateFunction.ARMS_DEALING:
        if (!isActive)
          break;
        card.ActivateActorArmsDealingSpell();
        break;
      case TagVisualActorStateFunction.CARD_COST_HEALTH:
        if (isActive && card.CanShowActorVisuals())
        {
          card.UpdateCardCostHealth(change);
          break;
        }
        SpellUtils.ActivateDeathIfNecessary(card.GetActor().GetSpellIfLoaded(SpellType.SPELLS_COST_HEALTH));
        break;
      case TagVisualActorStateFunction.DORMANT:
        if (isActive)
        {
          card.ActivateDormantStateVisual();
          break;
        }
        card.DeactivateDormantStateVisual();
        break;
      case TagVisualActorStateFunction.TECH_LEVEL_MANA_GEM:
        if (isActive && card.CanShowActorVisuals())
        {
          Spell spell = card.GetActor().GetSpell(SpellType.TECH_LEVEL_MANA_GEM);
          if (!((Object) spell != (Object) null))
            break;
          spell.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("TechLevel").Value = card.GetEntity().GetTechLevel();
          spell.ActivateState(SpellStateType.BIRTH);
          break;
        }
        SpellUtils.ActivateDeathIfNecessary(card.GetActor().GetSpellIfLoaded(SpellType.TECH_LEVEL_MANA_GEM));
        break;
      case TagVisualActorStateFunction.COIN_ON_ENEMY_MINIONS:
        if (isActive)
        {
          Spell spell = card.GetActor().GetSpell(SpellType.BACON_SHOP_MINION_COIN);
          if (!((Object) spell != (Object) null))
            break;
          spell.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("TechLevel").Value = card.GetEntity().GetTechLevel();
          spell.ActivateState(SpellStateType.BIRTH);
          break;
        }
        SpellUtils.ActivateDeathIfNecessary(card.GetActor().GetSpellIfLoaded(SpellType.BACON_SHOP_MINION_COIN));
        break;
      case TagVisualActorStateFunction.DECK_POWER_UP:
        if (isActive)
        {
          Spell spell = card.GetActor().GetSpell(SpellType.DECK_POWER_UP);
          if (!((Object) spell != (Object) null) || !((Object) card.GetHeroCard() != (Object) null) || !((Object) card.GetHeroCard().gameObject != (Object) null))
            break;
          spell.SetSource(card.GetHeroCard().gameObject);
          spell.ForceUpdateTransform();
          SpellUtils.ActivateBirthIfNecessary(spell);
          break;
        }
        SpellUtils.ActivateDeathIfNecessary(card.GetActor().GetSpellIfLoaded(SpellType.DECK_POWER_UP));
        break;
      case TagVisualActorStateFunction.COIN_MANA_GEM:
        if (isActive && card.CanShowActorVisuals() && !(card.GetZone() is ZoneBattlegroundQuestReward))
        {
          Spell spell = card.GetActor().GetSpell(SpellType.COIN_MANA_GEM);
          if (!((Object) spell != (Object) null))
            break;
          spell.ActivateState(SpellStateType.BIRTH);
          break;
        }
        SpellUtils.ActivateDeathIfNecessary(card.GetActor().GetSpellIfLoaded(SpellType.COIN_MANA_GEM));
        break;
      case TagVisualActorStateFunction.EVIL_TWIN_MUSTACHE:
        if (isActive)
        {
          card.GetActor().ActivateEvilTwinMustache();
          break;
        }
        card.GetActor().DeactivateEvilTwinMustache();
        break;
      case TagVisualActorStateFunction.CARD_COST_ARMOR:
        if (isActive && card.CanShowActorVisuals())
        {
          card.UpdateCardCostArmor(change);
          break;
        }
        SpellUtils.ActivateDeathIfNecessary(card.GetActor().GetSpellIfLoaded(SpellType.COST_ARMOR));
        break;
    }
  }

  private bool IsActionConditionMet(
    bool invertCondition,
    TagVisualActorCondition condition,
    GAME_TAG tag,
    TagVisualActorConditionComparisonOperator tagComparisonOperator,
    int tagValue,
    TagVisualActorConditionEntity tagComparisonEntity,
    SpellType spellType,
    SpellStateType spellState,
    Card card,
    bool fromShowEntity,
    bool fromTagChange,
    Actor overrideActor)
  {
    bool flag = false;
    if ((Object) card == (Object) null)
      return false;
    Actor actor = card.GetActor();
    if ((Object) overrideActor != (Object) null)
      actor = overrideActor;
    switch (condition)
    {
      case TagVisualActorCondition.ALWAYS:
        flag = true;
        break;
      case TagVisualActorCondition.DOES_SPELL_HAVE_STATE:
        flag = this.CompareSpellState(spellType, spellState, card, overrideActor);
        break;
      case TagVisualActorCondition.DOES_TAG_HAVE_VALUE:
        Entity entity1 = card.GetEntity();
        switch (tagComparisonEntity)
        {
          case TagVisualActorConditionEntity.HERO:
            entity1 = card.GetHero();
            break;
          case TagVisualActorConditionEntity.CONTROLLER:
            entity1 = (Entity) card.GetController();
            break;
          case TagVisualActorConditionEntity.GAME:
            entity1 = GameState.Get() != null ? (Entity) GameState.Get().GetGameEntity() : (Entity) null;
            break;
        }
        flag = this.CompareTagValue(tagComparisonOperator, tag, tagValue, entity1);
        break;
      case TagVisualActorCondition.IS_ENRAGED:
        flag = card.GetEntity() != null && card.GetEntity().IsEnraged();
        break;
      case TagVisualActorCondition.IS_ASLEEP:
        flag = card.GetEntity() != null && card.GetEntity().IsAsleep();
        break;
      case TagVisualActorCondition.IS_FRIENDLY:
        flag = card.GetEntity() != null && card.GetEntity().GetController() != null && card.GetEntity().GetController().IsFriendlySide();
        break;
      case TagVisualActorCondition.IS_MOUSED_OVER:
        flag = card.IsMousedOver();
        break;
      case TagVisualActorCondition.IS_ENCHANTMENT:
        flag = card.GetEntity() != null && card.GetEntity().IsEnchantment();
        break;
      case TagVisualActorCondition.IS_DISABLED_HERO_POWER:
        flag = card.GetEntity() != null && card.GetEntity().GetController() != null && card.GetEntity().GetController().HasTag(GAME_TAG.HERO_POWER_DISABLED);
        break;
      case TagVisualActorCondition.IS_FROM_SHOW_ENTITY:
        flag = fromShowEntity;
        break;
      case TagVisualActorCondition.SHOULD_SHOW_IMMUNE_VISUALS:
        flag = card.ShouldShowImmuneVisuals();
        break;
      case TagVisualActorCondition.CAN_SHOW_ACTOR_VISUALS:
        flag = card.CanShowActorVisuals();
        break;
      case TagVisualActorCondition.ATTACHED_CARD_CAN_SHOW_ACTOR_VISUALS:
        if (card.GetEntity() != null)
        {
          Entity entity2 = GameState.Get().GetEntity(card.GetEntity().GetAttached());
          flag = entity2 != null && (Object) entity2.GetCard() != (Object) null && entity2.GetCard().CanShowActorVisuals();
          break;
        }
        break;
      case TagVisualActorCondition.SHOULD_USE_TECH_LEVEL_MANA_GEM:
        flag = (Object) actor != (Object) null && actor.UseTechLevelManaGem();
        break;
      case TagVisualActorCondition.IS_REAL_TIME_DORMANT:
        flag = card.GetEntity() != null && card.GetEntity().GetRealTimeIsDormant();
        break;
      case TagVisualActorCondition.IS_AI_CONTROLLER:
        flag = card.GetEntity() != null && card.GetEntity().GetController() != null && card.GetEntity().GetController().IsAI();
        break;
      case TagVisualActorCondition.IS_FROM_TAG_CHANGE:
        flag = fromTagChange;
        break;
      case TagVisualActorCondition.SHOULD_USE_COIN_ON_ENEMY_MINIONS:
        flag = (Object) actor != (Object) null && !actor.GetEntity().IsControlledByFriendlySidePlayer() && GameState.Get() != null && GameState.Get().GetGameEntity().HasTag(GAME_TAG.BACON_COIN_ON_ENEMY_MINIONS);
        break;
      case TagVisualActorCondition.IS_VALID_OPTION:
        flag = GameState.Get() != null && GameState.Get().IsValidOption(card.GetEntity());
        break;
      case TagVisualActorCondition.IS_SPELL:
        flag = card.GetEntity() != null && card.GetEntity().IsSpell();
        break;
    }
    if (invertCondition)
      flag = !flag;
    return flag;
  }

  private bool IsActionConditionMet(
    TagVisualActorConditionConfiguration condition,
    Card card,
    bool fromShowEntity,
    bool fromTagChange,
    Actor overrideActor)
  {
    bool flag;
    switch (condition.m_Condition)
    {
      case TagVisualActorCondition.ALWAYS:
        flag = true;
        break;
      case TagVisualActorCondition.AND:
        flag = this.IsActionConditionMet(condition.m_Parameters.m_InvertConditionLHS, condition.m_Parameters.m_ConditionLHS, condition.m_Parameters.m_TagLHS, condition.m_Parameters.m_ComparisonOperatorLHS, condition.m_Parameters.m_ValueLHS, condition.m_Parameters.m_TagComparisonEntityLHS, condition.m_Parameters.m_SpellTypeLHS, condition.m_Parameters.m_SpellStateLHS, card, fromShowEntity, fromTagChange, overrideActor) && this.IsActionConditionMet(condition.m_Parameters.m_InvertConditionRHS, condition.m_Parameters.m_ConditionRHS, condition.m_Parameters.m_TagRHS, condition.m_Parameters.m_ComparisonOperatorRHS, condition.m_Parameters.m_ValueRHS, condition.m_Parameters.m_TagComparisonEntityRHS, condition.m_Parameters.m_SpellTypeRHS, condition.m_Parameters.m_SpellStateRHS, card, fromShowEntity, fromTagChange, overrideActor);
        if (condition.m_InvertCondition)
        {
          flag = !flag;
          break;
        }
        break;
      case TagVisualActorCondition.OR:
        flag = this.IsActionConditionMet(condition.m_Parameters.m_InvertConditionLHS, condition.m_Parameters.m_ConditionLHS, condition.m_Parameters.m_TagLHS, condition.m_Parameters.m_ComparisonOperatorLHS, condition.m_Parameters.m_ValueLHS, condition.m_Parameters.m_TagComparisonEntityLHS, condition.m_Parameters.m_SpellTypeLHS, condition.m_Parameters.m_SpellStateLHS, card, fromShowEntity, fromTagChange, overrideActor) || this.IsActionConditionMet(condition.m_Parameters.m_InvertConditionRHS, condition.m_Parameters.m_ConditionRHS, condition.m_Parameters.m_TagRHS, condition.m_Parameters.m_ComparisonOperatorRHS, condition.m_Parameters.m_ValueRHS, condition.m_Parameters.m_TagComparisonEntityRHS, condition.m_Parameters.m_SpellTypeRHS, condition.m_Parameters.m_SpellStateRHS, card, fromShowEntity, fromTagChange, overrideActor);
        if (condition.m_InvertCondition)
        {
          flag = !flag;
          break;
        }
        break;
      default:
        flag = this.IsActionConditionMet(condition.m_InvertCondition, condition.m_Condition, condition.m_Parameters.m_Tag, condition.m_Parameters.m_ComparisonOperator, condition.m_Parameters.m_Value, condition.m_Parameters.m_TagComparisonEntity, condition.m_Parameters.m_SpellType, condition.m_Parameters.m_SpellState, card, fromShowEntity, fromTagChange, overrideActor);
        break;
    }
    return flag;
  }

  private void ActivateSpellState(
    SpellType spellType,
    SpellStateType spellState,
    Card card,
    bool forceActivate,
    Actor overrideActor)
  {
    if ((Object) card == (Object) null)
      return;
    Actor actor = card.GetActor();
    if ((Object) overrideActor != (Object) null)
      actor = overrideActor;
    if (!((Object) actor != (Object) null))
      return;
    Spell spell = spellState == SpellStateType.BIRTH ? actor.GetSpell(spellType) : actor.GetSpellIfLoaded(spellType);
    if (!((Object) spell != (Object) null))
      return;
    if (forceActivate)
    {
      spell.ActivateState(spellState);
      if (!((Object) spell.GetSource() == (Object) null) || !((Object) card != (Object) null))
        return;
      spell.SetSource(card.gameObject);
    }
    else
    {
      if (!SpellUtils.ActivateStateIfNecessary(spell, spellState) || !((Object) spell.GetSource() == (Object) null) || !((Object) card != (Object) null))
        return;
      spell.SetSource(card.gameObject);
    }
  }

  private bool CompareSpellState(
    SpellType spellType,
    SpellStateType spellState,
    Card card,
    Actor overrideActor)
  {
    bool flag = false;
    if ((Object) card == (Object) null)
      return false;
    Actor actor = card.GetActor();
    if ((Object) overrideActor != (Object) null)
      actor = overrideActor;
    if (!((Object) actor != (Object) null))
      return flag;
    Spell spellIfLoaded = actor.GetSpellIfLoaded(spellType);
    return (Object) spellIfLoaded != (Object) null ? spellIfLoaded.GetActiveState() == spellState : spellState == SpellStateType.NONE;
  }

  private bool CompareTagValue(
    TagVisualActorConditionComparisonOperator op,
    GAME_TAG tag,
    int value,
    Entity entity)
  {
    bool flag = false;
    if (entity == null)
      return false;
    switch (op)
    {
      case TagVisualActorConditionComparisonOperator.EQUAL:
        flag = entity.GetTag(tag) == value;
        break;
      case TagVisualActorConditionComparisonOperator.GREATER_THAN:
        flag = entity.GetTag(tag) > value;
        break;
      case TagVisualActorConditionComparisonOperator.GREATER_THAN_OR_EQUAL:
        flag = entity.GetTag(tag) >= value;
        break;
      case TagVisualActorConditionComparisonOperator.LESS_THAN:
        flag = entity.GetTag(tag) < value;
        break;
      case TagVisualActorConditionComparisonOperator.LESS_THAN_OR_EQUAL:
        flag = entity.GetTag(tag) <= value;
        break;
    }
    return flag;
  }

  private TagVisualConfigurationEntry FindTagEntry(GAME_TAG tag)
  {
    foreach (TagVisualConfigurationEntry tagVisual in this.m_TagVisuals)
    {
      if (tagVisual.m_Tag == tag)
        return tagVisual;
    }
    return (TagVisualConfigurationEntry) null;
  }
}
