using PegasusGame;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;

public class Entity : EntityBase
{
  private EntityDef m_staticEntityDef = new EntityDef();
  private EntityDef m_dynamicEntityDef;
  private Card m_card;
  private Entity.LoadState m_loadState;
  private int m_cardAssetLoadCount;
  private bool m_useBattlecryPower;
  private bool m_duplicateForHistory;
  private CardTextHistoryData m_cardTextHistoryData;
  private List<Entity> m_attachments = new List<Entity>();
  private List<int> m_subCardIDs = new List<int>();
  private List<int> m_lettuceAbilityEntityIDs = new List<int>();
  private int m_realTimeCost;
  private int m_realTimeAttack;
  private int m_realTimeHealth;
  private int m_realTimeDamage;
  private int m_realTimeArmor;
  private int m_realTimeZone;
  private int m_realTimeZonePosition;
  private int m_realTimeLinkedEntityId;
  private bool m_realTimePoweredUp;
  private bool m_realTimeDivineShield;
  private bool m_realTimeIsImmune;
  private bool m_realTimeIsImmuneWhileAttacking;
  private bool m_realTimeIsPoisonous;
  private bool m_realTimeIsDormant;
  private int m_realTimeSpellpower;
  private bool m_realTimeSpellpowerDouble;
  private bool m_realTimeHealingDoesDamageHint;
  private bool m_realTimeLifestealDoesDamageHint;
  private bool m_realTimeCardCostsHealth;
  private bool m_realTimeCardCostsArmor;
  private bool m_realTimeAttackableByRush;
  private TAG_CARDTYPE m_realTimeCardType;
  private TAG_PREMIUM m_realTimePremium;
  private int m_realTimePlayerLeaderboardPlace;
  private int m_realTimePlayerTechLevel;
  private int m_queuedRealTimeControllerTagChangeCount;
  private int m_queuedChangeEntityCount;
  private List<Network.HistChangeEntity> m_transformPowersProcessed = new List<Network.HistChangeEntity>();
  private string m_displayedCreatorName;
  private string m_enchantmentCreatorCardIDForPortrait;
  private Entity.CachedDebugName m_cachedDebugName;
  private static ProfilerMarker s_cardInitMarker = new ProfilerMarker("Entity.CardInit");

  public override string ToString() => this.GetDebugName();

  public virtual void OnRealTimeFullEntity(Network.HistFullEntity fullEntity)
  {
    this.SetTags(fullEntity.Entity.Tags);
    this.InitRealTimeValues(fullEntity.Entity.Tags);
    this.InitCard();
    this.LoadEntityDef(fullEntity.Entity.CardID);
  }

  public void OnFullEntity(Network.HistFullEntity fullEntity)
  {
    this.m_loadState = Entity.LoadState.PENDING;
    this.LoadCard(fullEntity.Entity.CardID);
    int tag1 = this.GetTag(GAME_TAG.ATTACHED);
    if (tag1 != 0)
      GameState.Get().GetEntity(tag1).AddAttachment(this);
    int tag2 = this.GetTag(GAME_TAG.PARENT_CARD);
    if (tag2 != 0)
    {
      Entity entity = GameState.Get().GetEntity(tag2);
      if (entity != null)
        entity.AddSubCard(this);
      else
        Log.Gameplay.PrintError("Unable to find parent entity id={0}", (object) tag2);
    }
    int tag3 = this.GetTag(GAME_TAG.LETTUCE_ABILITY_OWNER);
    if (tag3 != 0 && this.IsLettuceAbility())
      GameState.Get().GetEntity(tag3)?.AddLettuceAbilityEntityID(this.GetEntityId());
    if (this.GetZone() == TAG_ZONE.PLAY)
    {
      if (this.IsHero())
        this.GetController().SetHero(this);
      else if (this.IsHeroPower())
        this.GetController().SetHeroPower(this);
    }
    if (fullEntity.Entity.DefTags.Count > 0)
    {
      EntityDef dynamicDefinition = this.GetOrCreateDynamicDefinition();
      for (int index = 0; index < fullEntity.Entity.DefTags.Count; ++index)
        dynamicDefinition.SetTag(fullEntity.Entity.DefTags[index].Name, fullEntity.Entity.DefTags[index].Value);
    }
    if (this.HasTag(GAME_TAG.DISPLAYED_CREATOR))
      this.SetDisplayedCreatorName(this.GetTag(GAME_TAG.DISPLAYED_CREATOR));
    if (this.HasTag(GAME_TAG.CREATOR_DBID))
      this.ResolveEnchantmentPortraitCardID(this.GetTag(GAME_TAG.CREATOR_DBID));
    if (this.HasTag(GAME_TAG.PLAYER_LEADERBOARD_PLACE) && this.GetRealTimeZone() != TAG_ZONE.GRAVEYARD)
    {
      PlayerLeaderboardManager.Get().CreatePlayerTile(this);
      int tag4 = this.GetTag(GAME_TAG.PLAYER_ID);
      if (GameState.Get().GetPlayerInfoMap().ContainsKey(tag4))
        GameState.Get().GetPlayerInfoMap()[tag4].SetPlayerHero(this);
      if (this.HasTag(GAME_TAG.REPLACEMENT_ENTITY))
        PlayerLeaderboardManager.Get().ApplyEntityReplacement(tag4, this);
    }
    if (!this.HasTag(GAME_TAG.BACON_IS_KEL_THUZAD))
      return;
    PlayerLeaderboardManager.Get().SetOddManOutOpponentHero(this);
  }

  public virtual void OnRealTimeShowEntity(Network.HistShowEntity showEntity) => this.HandleRealTimeEntityChange(showEntity.Entity);

  public void OnShowEntity(Network.HistShowEntity showEntity) => this.HandleEntityChange(showEntity.Entity, new Entity.LoadCardData()
  {
    updateActor = false,
    restartStateSpells = false,
    fromChangeEntity = false
  }, true);

  public void OnHideEntity(Network.HistHideEntity hideEntity)
  {
    this.SetTagAndHandleChange<int>(GAME_TAG.ZONE, hideEntity.Zone);
    EntityDef entityDef = this.GetEntityDef();
    this.SetTag(GAME_TAG.ATK, entityDef.GetATK());
    this.SetTag(GAME_TAG.HEALTH, entityDef.GetHealth());
    this.SetTag(GAME_TAG.COST, entityDef.GetCost());
    this.SetTag(GAME_TAG.DAMAGE, 0);
    this.SetCardId((string) null);
  }

  public virtual void OnRealTimeChangeEntity(
    List<Network.PowerHistory> powerList,
    int index,
    Network.HistChangeEntity changeEntity)
  {
    ++this.m_queuedChangeEntityCount;
    this.HandleRealTimeEntityChange(changeEntity.Entity);
    this.CheckRealTimeTransform(powerList, index, changeEntity);
  }

  public void OnChangeEntity(Network.HistChangeEntity changeEntity)
  {
    if (this.m_transformPowersProcessed.Contains(changeEntity))
    {
      this.m_transformPowersProcessed.Remove(changeEntity);
    }
    else
    {
      this.m_subCardIDs.Clear();
      --this.m_queuedChangeEntityCount;
      Entity.LoadCardData data = new Entity.LoadCardData()
      {
        updateActor = this.ShouldUpdateActorOnChangeEntity(changeEntity),
        restartStateSpells = this.ShouldRestartStateSpellsOnChangeEntity(changeEntity),
        fromChangeEntity = true
      };
      this.HandleEntityChange(changeEntity.Entity, data, false);
    }
  }

  private bool IsTagChanged(Network.HistChangeEntity changeEntity, GAME_TAG tag)
  {
    Network.Entity.Tag tag1 = changeEntity.Entity.Tags.Find((Predicate<Network.Entity.Tag>) (currTag => (GAME_TAG) currTag.Name == tag));
    return tag1 != null && this.GetTag(tag) != tag1.Value;
  }

  private bool ShouldUpdateActorOnChangeEntity(Network.HistChangeEntity changeEntity) => this.IsTagChanged(changeEntity, GAME_TAG.CARDTYPE) || (TAG_CARDTYPE) this.GetTag(GAME_TAG.CARDTYPE) != this.m_realTimeCardType || this.IsTagChanged(changeEntity, GAME_TAG.PREMIUM) || (TAG_PREMIUM) this.GetTag(GAME_TAG.PREMIUM) != this.m_realTimePremium || this.IsTagChanged(changeEntity, GAME_TAG.LETTUCE_MERCENARY);

  private bool ShouldRestartStateSpellsOnChangeEntity(Network.HistChangeEntity changeEntity) => this.IsTagChanged(changeEntity, GAME_TAG.ELITE);

  public virtual void OnRealTimeTagChanged(Network.HistTagChange change)
  {
    switch ((GAME_TAG) change.Tag)
    {
      case GAME_TAG.PREMIUM:
        this.SetRealTimePremium((TAG_PREMIUM) change.Value);
        break;
      case GAME_TAG.DAMAGE:
        this.SetRealTimeDamage(change.Value);
        break;
      case GAME_TAG.HEALTH:
      case GAME_TAG.DURABILITY:
        this.SetRealTimeHealth(change.Value);
        break;
      case GAME_TAG.ATK:
        this.SetRealTimeAttack(change.Value);
        break;
      case GAME_TAG.COST:
        this.SetRealTimeCost(change.Value);
        break;
      case GAME_TAG.ZONE:
        this.SetRealTimeZone(change.Value);
        break;
      case GAME_TAG.CONTROLLER:
        ++this.m_queuedRealTimeControllerTagChangeCount;
        break;
      case GAME_TAG.SPELLPOWER:
        this.SetRealTimeHasSpellpower(change.Value);
        break;
      case GAME_TAG.DIVINE_SHIELD:
        this.SetRealTimeDivineShield(change.Value);
        break;
      case GAME_TAG.CARDTYPE:
        this.SetRealTimeCardType((TAG_CARDTYPE) change.Value);
        break;
      case GAME_TAG.IMMUNE:
        this.SetRealTimeIsImmune(change.Value);
        break;
      case GAME_TAG.LINKED_ENTITY:
        this.SetRealTimeLinkedEntityId(change.Value);
        break;
      case GAME_TAG.ZONE_POSITION:
        this.SetRealTimeZonePosition(change.Value);
        ZoneMgr.Get().OnRealTimeZonePosChange(this);
        break;
      case GAME_TAG.ARMOR:
        this.SetRealTimeArmor(change.Value);
        break;
      case GAME_TAG.SPELLPOWER_DOUBLE:
        this.SetRealTimeSpellpowerDouble(change.Value);
        break;
      case GAME_TAG.POISONOUS:
      case GAME_TAG.NON_KEYWORD_POISONOUS:
        this.SetRealTimeIsPoisonous(change.Value);
        break;
      case GAME_TAG.IMMUNE_WHILE_ATTACKING:
        this.SetRealTimeIsImmuneWhileAttacking(change.Value);
        break;
      case GAME_TAG.POWERED_UP:
        this.SetRealTimePoweredUp(change.Value);
        break;
      case GAME_TAG.CARD_COSTS_HEALTH:
        this.SetRealTimeCardCostsHealth(change.Value);
        break;
      case GAME_TAG.ATTACKABLE_BY_RUSH:
        this.SetRealTimeAttackableByRush(change.Value);
        break;
      case GAME_TAG.PUZZLE_COMPLETED:
        this.OnRealTimePuzzleCompleted(change.Value);
        break;
      case GAME_TAG.HEALING_DOES_DAMAGE_HINT:
        this.SetRealTimeHealingDoesDamageHint(change.Value);
        break;
      case GAME_TAG.PLAYER_LEADERBOARD_PLACE:
        this.SetRealTimePlayerLeaderboardPlace(change.Value);
        this.UpdateSharedPlayer();
        break;
      case GAME_TAG.PLAYER_TECH_LEVEL:
        this.SetRealTimePlayerTechLevel(change.Value);
        PlayerLeaderboardManager.Get().NotifyPlayerTileEvent(this.GetTag(GAME_TAG.PLAYER_ID), PlayerLeaderboardManager.PlayerTileEvent.TECH_LEVEL);
        break;
      case GAME_TAG.PLAYER_TRIPLES:
        if (change.Value == 0)
          break;
        PlayerLeaderboardManager.Get().NotifyPlayerTileEvent(this.GetTag(GAME_TAG.PLAYER_ID), PlayerLeaderboardManager.PlayerTileEvent.TRIPLE);
        break;
      case GAME_TAG.DORMANT:
        this.SetRealTimeIsDormant(change.Value);
        break;
      case GAME_TAG.BACON_MUKLA_BANANA_SPAWN_COUNT:
        PlayerLeaderboardManager.Get().NotifyPlayerTileEvent(this.GetTag(GAME_TAG.PLAYER_ID), PlayerLeaderboardManager.PlayerTileEvent.BANANA);
        break;
      case GAME_TAG.LIFESTEAL_DOES_DAMAGE_HINT:
        this.SetRealTimeLifestealDoesDamageHint(change.Value);
        break;
      case GAME_TAG.BACON_PLAYER_NUM_HERO_BUDDIES_GAINED:
        switch (change.Value)
        {
          case 0:
            return;
          case 1:
            PlayerLeaderboardManager.Get().NotifyPlayerTileEvent(this.GetTag(GAME_TAG.PLAYER_ID), PlayerLeaderboardManager.PlayerTileEvent.HERO_BUDDY);
            return;
          case 2:
            PlayerLeaderboardManager.Get().NotifyPlayerTileEvent(this.GetTag(GAME_TAG.PLAYER_ID), PlayerLeaderboardManager.PlayerTileEvent.DOUBLE_HERO_BUDDY);
            return;
          default:
            PlayerLeaderboardManager.Get().NotifyPlayerTileEvent(this.GetTag(GAME_TAG.PLAYER_ID), PlayerLeaderboardManager.PlayerTileEvent.DOUBLE_HERO_BUDDY);
            Debug.LogWarning((object) string.Format("Unexpected Number of Hero Buddies gained: {0}", (object) change.Value));
            return;
        }
      case GAME_TAG.BACON_QUEST_COMPLETED:
        PlayerLeaderboardManager.Get().NotifyPlayerTileEvent(this.GetTag(GAME_TAG.PLAYER_ID), PlayerLeaderboardManager.PlayerTileEvent.QUEST_COMPLETE);
        break;
      case GAME_TAG.CARD_COSTS_ARMOR:
        this.SetRealTimeCardCostsArmor(change.Value);
        break;
    }
  }

  private void UpdateSharedPlayer()
  {
    PlayerLeaderboardManager.Get().CreatePlayerTile(this);
    int tag = this.GetTag(GAME_TAG.PLAYER_ID);
    if (tag == 0)
      tag = this.GetTag(GAME_TAG.CONTROLLER);
    if (!GameState.Get().GetPlayerInfoMap().ContainsKey(tag) || GameState.Get().GetPlayerInfoMap()[tag].GetPlayerHero() != null)
      return;
    GameState.Get().GetPlayerInfoMap()[tag].SetPlayerHero(this);
  }

  public void OnRealTimePuzzleCompleted(int newValue)
  {
    if (!this.IsPuzzle() || (UnityEngine.Object) this.m_card == (UnityEngine.Object) null || (UnityEngine.Object) this.m_card.GetActor() == (UnityEngine.Object) null)
      return;
    PuzzleController component = this.m_card.GetActor().GetComponent<PuzzleController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      Log.Gameplay.PrintError("Puzzle card {0} does not have a PuzzleController component.", (object) this);
    else
      component.OnRealTimePuzzleCompleted(newValue);
  }

  public virtual void HandlePreTransformTagChanges(TagDeltaList changeList)
  {
    if (!((UnityEngine.Object) this.m_card != (UnityEngine.Object) null))
      return;
    this.m_card.DeactivateCustomKeywordEffect();
  }

  public virtual void OnTagsChanged(TagDeltaList changeList, bool fromShowEntity)
  {
    bool flag = false;
    for (int index = 0; index < changeList.Count; ++index)
    {
      TagDelta change = changeList[index];
      if (this.IsNameChange(change))
        flag = true;
      this.HandleTagChange(change);
    }
    if ((UnityEngine.Object) this.m_card == (UnityEngine.Object) null)
      return;
    if (flag)
      this.UpdateCardName();
    this.m_card.OnTagsChanged(changeList, fromShowEntity);
  }

  public virtual void OnTagChanged(TagDelta change)
  {
    this.HandleTagChange(change);
    if ((UnityEngine.Object) this.m_card == (UnityEngine.Object) null)
      return;
    if (this.IsNameChange(change))
      this.UpdateCardName();
    this.m_card.OnTagChanged(change, false);
  }

  public virtual void OnCachedTagForDormantChanged(TagDelta change) => this.SetCachedTagForDormant(change.tag, change.newValue);

  protected override void OnUpdateCardId() => this.UpdateCardName();

  public virtual void OnMetaData(Network.HistMetaData metaData)
  {
    if ((UnityEngine.Object) this.m_card == (UnityEngine.Object) null)
      return;
    this.m_card.OnMetaData(metaData);
  }

  private void HandleRealTimeEntityChange(Network.Entity netEntity) => this.InitRealTimeValues(netEntity.Tags);

  private bool HasRealTimeTransformTag(Network.Entity netEntity)
  {
    foreach (Network.Entity.Tag tag in netEntity.Tags)
    {
      if (tag.Name == 859 && tag.Value == 1)
        return true;
    }
    return false;
  }

  private void CheckRealTimeTransform(
    List<Network.PowerHistory> powerList,
    int index,
    Network.HistChangeEntity changeEntity)
  {
    if (!this.HasRealTimeTransformTag(changeEntity.Entity) || !this.CanRealTimeTransform(powerList, index))
      return;
    this.OnChangeEntity(changeEntity);
    this.m_transformPowersProcessed.Add(changeEntity);
  }

  private bool CanRealTimeTransform(List<Network.PowerHistory> powerList, int index)
  {
    for (int index1 = 0; index1 < index; ++index1)
    {
      if (!this.CheckPowerHistoryForRealTimeTransform(powerList[index1]))
        return false;
    }
    foreach (PowerTaskList power in (QueueList<PowerTaskList>) GameState.Get().GetPowerProcessor().GetPowerQueue())
    {
      if (!this.CheckPowerTaskListForRealTimeTransform(power))
        return false;
    }
    return this.CheckPowerTaskListForRealTimeTransform(GameState.Get().GetPowerProcessor().GetCurrentTaskList());
  }

  private bool CheckPowerHistoryForRealTimeTransform(Network.PowerHistory power)
  {
    switch (power.Type)
    {
      case Network.PowerType.FULL_ENTITY:
        if (((Network.HistFullEntity) power).Entity.ID == this.GetEntityId())
          return false;
        break;
      case Network.PowerType.SHOW_ENTITY:
        if (((Network.HistShowEntity) power).Entity.ID == this.GetEntityId())
          return false;
        break;
      case Network.PowerType.HIDE_ENTITY:
        if (((Network.HistHideEntity) power).Entity == this.GetEntityId())
          return false;
        break;
      case Network.PowerType.TAG_CHANGE:
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        if (histTagChange.Entity == this.GetEntityId() && histTagChange.Tag != 263 && histTagChange.Tag != 385 && histTagChange.Tag != 466)
          return false;
        break;
      case Network.PowerType.META_DATA:
        if (!this.CheckPowerHistoryMetaDataForRealTimeTransform((Network.HistMetaData) power))
          return false;
        break;
      case Network.PowerType.CHANGE_ENTITY:
        Network.HistChangeEntity histChangeEntity = (Network.HistChangeEntity) power;
        if (histChangeEntity.Entity.ID == this.GetEntityId() && !this.m_transformPowersProcessed.Contains(histChangeEntity))
          return false;
        break;
    }
    return true;
  }

  private bool CheckPowerHistoryMetaDataForRealTimeTransform(Network.HistMetaData metaDataEntity)
  {
    switch (metaDataEntity.MetaType)
    {
      case HistoryMeta.Type.TARGET:
      case HistoryMeta.Type.DAMAGE:
      case HistoryMeta.Type.HEALING:
      case HistoryMeta.Type.JOUST:
      case HistoryMeta.Type.HISTORY_TARGET:
        using (List<int>.Enumerator enumerator = metaDataEntity.Info.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            if (enumerator.Current == this.GetEntityId())
              return false;
          }
          break;
        }
      case HistoryMeta.Type.SHOW_BIG_CARD:
      case HistoryMeta.Type.EFFECT_TIMING:
      case HistoryMeta.Type.OVERRIDE_HISTORY:
      case HistoryMeta.Type.HISTORY_TARGET_DONT_DUPLICATE_UNTIL_END:
      case HistoryMeta.Type.BEGIN_ARTIFICIAL_HISTORY_TILE:
      case HistoryMeta.Type.BEGIN_ARTIFICIAL_HISTORY_TRIGGER_TILE:
      case HistoryMeta.Type.BURNED_CARD:
        if (metaDataEntity.Info.Count > 0 && metaDataEntity.Info[0] == this.GetEntityId())
          return false;
        break;
    }
    return true;
  }

  private bool CheckPowerTaskListForRealTimeTransform(PowerTaskList powerTaskList)
  {
    if (powerTaskList == null)
      return true;
    foreach (PowerTask task in powerTaskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (!task.IsCompleted() && !this.CheckPowerHistoryForRealTimeTransform(power))
        return false;
    }
    return true;
  }

  private void HandleEntityChange(
    Network.Entity netEntity,
    Entity.LoadCardData data,
    bool fromShowEntity)
  {
    TagDeltaList deltas = this.m_tags.CreateDeltas(netEntity.Tags);
    this.SetTags(netEntity.Tags);
    this.HandlePreTransformTagChanges(deltas);
    if ((UnityEngine.Object) this.m_card != (UnityEngine.Object) null)
      this.m_card.DestroyCardDefAssetsOnEntityChanged();
    this.LoadCard(netEntity.CardID, data);
    if (this.GetZone() == TAG_ZONE.HAND && (UnityEngine.Object) this.GetCard() != (UnityEngine.Object) null && (UnityEngine.Object) this.GetCard().GetZone() != (UnityEngine.Object) null)
    {
      if (data.updateActor)
        this.GetCard().GetZone().UpdateLayout();
      this.GetCard().UpdateActorState(true);
    }
    if (netEntity.DefTags.Count > 0)
    {
      EntityDef dynamicDefinition = this.GetOrCreateDynamicDefinition();
      for (int index = 0; index < netEntity.DefTags.Count; ++index)
        dynamicDefinition.SetTag(netEntity.DefTags[index].Name, netEntity.DefTags[index].Value);
    }
    this.OnTagsChanged(deltas, fromShowEntity);
  }

  private void HandleTagChange(TagDelta change)
  {
    switch ((GAME_TAG) change.tag)
    {
      case GAME_TAG.ATTACHED:
        GameState.Get().GetEntity(change.oldValue)?.RemoveAttachment(this);
        GameState.Get().GetEntity(change.newValue)?.AddAttachment(this);
        break;
      case GAME_TAG.ZONE:
        this.UpdateUseBattlecryFlag(false);
        if (GameState.Get().IsTurnStartManagerActive() && change.oldValue == 2 && change.newValue == 3)
        {
          PowerTaskList currentTaskList = GameState.Get().GetPowerProcessor().GetCurrentTaskList();
          if (currentTaskList != null && currentTaskList.GetSourceEntity() == GameState.Get().GetFriendlySidePlayer())
            TurnStartManager.Get().NotifyOfCardDrawn(this);
        }
        if (change.newValue == 1)
        {
          if (this.IsHero())
            this.GetController().SetHero(this);
          else if (this.IsHeroPower())
            this.GetController().SetHeroPower(this);
        }
        if (change.newValue == 4 && this.IsLettuceAbility())
          this.GetLettuceAbilityOwner()?.RemoveLettuceAbilityEntityID(this.GetEntityId());
        this.CheckZoneChangeForEnchantment(change);
        if (change.newValue != 5)
          break;
        GameState.Get().GetGameEntity().QueueEntityForRemoval(this);
        break;
      case GAME_TAG.CONTROLLER:
        Entity parentEntity = this.GetParentEntity();
        if (parentEntity != null)
        {
          if (GameState.Get().GetFriendlyPlayerId() != change.newValue)
            parentEntity.RemoveSubCard(this);
          else
            parentEntity.AddSubCard(this);
        }
        if (this.IsHeroPower())
          this.GetController().SetHeroPower(this);
        --this.m_queuedRealTimeControllerTagChangeCount;
        break;
      case GAME_TAG.PARENT_CARD:
        GameState.Get().GetEntity(change.oldValue)?.RemoveSubCard(this);
        GameState.Get().GetEntity(change.newValue)?.AddSubCard(this);
        break;
      case GAME_TAG.HERO_POWER:
      case GAME_TAG.HERO_POWER_ENTITY:
        PlayerLeaderboardManager leaderboardManager = PlayerLeaderboardManager.Get();
        if (!((UnityEngine.Object) leaderboardManager != (UnityEngine.Object) null) || !leaderboardManager.IsEnabled())
          break;
        leaderboardManager.UpdatePlayerTileHeroPower(this, change.newValue);
        break;
      case GAME_TAG.DISPLAYED_CREATOR:
        this.SetDisplayedCreatorName(change.newValue);
        break;
      case GAME_TAG.CREATOR_DBID:
        this.ResolveEnchantmentPortraitCardID(change.newValue);
        break;
      case GAME_TAG.LETTUCE_ABILITY_OWNER:
        GameState.Get().GetEntity(change.oldValue)?.RemoveLettuceAbilityEntityID(this.GetEntityId());
        Entity entity = GameState.Get().GetEntity(change.newValue);
        if (entity == null || !this.IsLettuceAbility())
          break;
        entity.AddLettuceAbilityEntityID(this.GetEntityId());
        break;
      case GAME_TAG.LETTUCE_SELECTED_ABILITY_QUEUE_ORDER:
        if (!(GameState.Get().GetGameEntity() is LettuceMissionEntity gameEntity))
          break;
        gameEntity.UpdateAllMercenaryAbilityOrderBubbleText();
        break;
      case GAME_TAG.BACON_PLAYER_NUM_HERO_BUDDIES_GAINED:
        Actor actor = TB_BaconShop.GetHeroBuddyCard(this.IsControlledByFriendlySidePlayer() ? Player.Side.FRIENDLY : Player.Side.OPPOSING)?.GetActor();
        if (!((UnityEngine.Object) actor != (UnityEngine.Object) null) || !actor.IsShown())
          break;
        switch (change.newValue)
        {
          case 0:
            return;
          case 1:
            SpellUtils.ActivateBirthIfNecessary(actor.GetLoadedSpell(SpellType.HERO_BUDDY_SINGLE));
            return;
          case 2:
            SpellUtils.ActivateBirthIfNecessary(actor.GetLoadedSpell(SpellType.HERO_BUDDY_DOUBLE));
            return;
          default:
            PlayerLeaderboardManager.Get().NotifyPlayerTileEvent(this.GetTag(GAME_TAG.PLAYER_ID), PlayerLeaderboardManager.PlayerTileEvent.DOUBLE_HERO_BUDDY);
            Debug.LogWarning((object) string.Format("Unexpected Number of Hero Buddies gained: {0}", (object) change.newValue));
            return;
        }
      case GAME_TAG.BACON_HERO_BUDDY_PROGRESS:
        HeroBuddyWidget component = TB_BaconShop.GetHeroBuddyCard(this.IsControlledByFriendlySidePlayer() ? Player.Side.FRIENDLY : Player.Side.OPPOSING)?.GetActor()?.GetComponent<HeroBuddyWidget>();
        Entity hero = this.GetController()?.GetHero();
        int num1 = 0;
        if (hero != null)
          num1 = hero.GetTag(GAME_TAG.BACON_PLAYER_NUM_HERO_BUDDIES_GAINED);
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
          break;
        int num2 = 100 * num1 + change.newValue;
        if (num2 > 200)
          num2 = 200;
        component.UpdateProgressBar((float) num2 / 200f);
        break;
      case GAME_TAG.BACON_HERO_QUEST_REWARD_DATABASE_ID:
      case GAME_TAG.BACON_HERO_HEROPOWER_QUEST_REWARD_DATABASE_ID:
      case GAME_TAG.BACON_HERO_QUEST_REWARD_COMPLETED:
      case GAME_TAG.BACON_HERO_HEROPOWER_QUEST_REWARD_COMPLETED:
        PlayerLeaderboardManager.Get()?.NotifyPlayerTileEvent(this.GetTag(GAME_TAG.PLAYER_ID), PlayerLeaderboardManager.PlayerTileEvent.QUEST_UPDATE);
        break;
    }
  }

  private void SetDisplayedCreatorName(int entityID)
  {
    Entity entity = GameState.Get().GetEntity(entityID);
    if (entity == null)
      this.m_displayedCreatorName = (string) null;
    else if (string.IsNullOrEmpty(entity.m_cardId))
      this.m_displayedCreatorName = GameStrings.Get("GAMEPLAY_UNKNOWN_CREATED_BY");
    else
      this.m_displayedCreatorName = entity.GetName();
  }

  private bool HasEnchantmentPortrait(string enchantmentPortraitCardID)
  {
    if (string.IsNullOrEmpty(enchantmentPortraitCardID))
      return false;
    using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(enchantmentPortraitCardID))
    {
      if (cardDef == null)
        return false;
      EntityDef entityDef = DefLoader.Get().GetEntityDef(enchantmentPortraitCardID);
      if (entityDef == null)
        return false;
      TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
      Material material;
      if (entityDef.GetCardType() == TAG_CARDTYPE.ENCHANTMENT)
      {
        if (cardDef.CardDef.TryGetEnchantmentPortrait(out material) || (UnityEngine.Object) cardDef.CardDef.GetPortraitTexture(premium) != (UnityEngine.Object) null)
          return true;
      }
      else if (cardDef.CardDef.TryGetHistoryTileFullPortrait(premium, out material) || (UnityEngine.Object) cardDef.CardDef.GetPortraitTexture(premium) != (UnityEngine.Object) null)
        return true;
      return false;
    }
  }

  public string GetEnchantmentCreatorCardIDForPortrait() => this.m_enchantmentCreatorCardIDForPortrait;

  private void ResolveEnchantmentPortraitCardID(int creatorDBID)
  {
    this.m_enchantmentCreatorCardIDForPortrait = (string) null;
    if (!this.IsEnchantment())
      return;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(creatorDBID);
    if (entityDef == null)
      return;
    this.m_enchantmentCreatorCardIDForPortrait = entityDef.GetCardId();
    Entity creator = this.GetCreator();
    EntityDef creatorDef;
    for (; !this.HasEnchantmentPortrait(this.m_enchantmentCreatorCardIDForPortrait); this.m_enchantmentCreatorCardIDForPortrait = creatorDef.GetCardId())
    {
      if (creator == null || !creator.IsEnchantment() && creator.GetCardType() != TAG_CARDTYPE.INVALID)
      {
        this.m_enchantmentCreatorCardIDForPortrait = (string) null;
        return;
      }
      creatorDef = creator.GetCreatorDef();
      creator = creator.GetCreator();
      if (creatorDef == null)
      {
        this.m_enchantmentCreatorCardIDForPortrait = (string) null;
        return;
      }
    }
    Entity entity = GameState.Get().GetEntity(this.GetAttached());
    if (entity == null || !((UnityEngine.Object) entity.m_card != (UnityEngine.Object) null))
      return;
    entity.m_card.UpdateTooltip();
  }

  private void CheckZoneChangeForEnchantment(TagDelta change)
  {
    if (change.tag != 49 || !this.IsEnchantment() || change.oldValue == change.newValue || change.newValue != 5 && change.newValue != 4)
      return;
    GameState.Get().GetEntity(this.GetAttached())?.RemoveAttachment(this);
    if (!((UnityEngine.Object) this.m_card != (UnityEngine.Object) null))
      return;
    this.m_card.Destroy();
  }

  private bool IsNameChange(TagDelta change)
  {
    switch ((GAME_TAG) change.tag)
    {
      case GAME_TAG.ZONE:
      case GAME_TAG.CONTROLLER:
      case GAME_TAG.ENTITY_ID:
      case GAME_TAG.CARDTYPE:
      case GAME_TAG.ZONE_POSITION:
      case GAME_TAG.OVERRIDECARDNAME:
        return true;
      default:
        return false;
    }
  }

  public EntityDef GetEntityDef() => this.m_dynamicEntityDef == null ? this.m_staticEntityDef : this.m_dynamicEntityDef;

  public EntityDef GetOrCreateDynamicDefinition()
  {
    if (this.m_dynamicEntityDef == null)
    {
      this.m_dynamicEntityDef = this.m_staticEntityDef.Clone();
      this.m_staticEntityDef = (EntityDef) null;
    }
    return this.m_dynamicEntityDef;
  }

  public Card InitCard()
  {
    using (Entity.s_cardInitMarker.Auto())
    {
      this.m_card = AssetLoader.Get().InstantiatePrefab((AssetReference) "BaseCard.prefab:465d44bb92c351f48ba03163aa012389").GetComponent<Card>();
      this.m_card.SetEntity(this);
      this.UpdateCardName();
      return this.m_card;
    }
  }

  public DefLoader.DisposableCardDef ShareDisposableCardDef()
  {
    if (this.m_duplicateForHistory)
      return this.GetCardDefForHistory();
    if ((UnityEngine.Object) this.m_card != (UnityEngine.Object) null)
      return this.m_card.ShareDisposableCardDef();
    return !string.IsNullOrEmpty(this.m_cardId) ? DefLoader.Get().GetCardDef(this.m_cardId, this.m_card.GetPremium()) : (DefLoader.DisposableCardDef) null;
  }

  private DefLoader.DisposableCardDef GetCardDefForHistory()
  {
    if ((UnityEngine.Object) this.m_card != (UnityEngine.Object) null)
    {
      if (this.IsHidden() && !this.m_card.HasHiddenCardDef)
        return DefLoader.Get().GetCardDef("HiddenCard");
      if (this.m_cardId == this.m_card.GetEntity().GetCardId())
        return this.m_card.ShareDisposableCardDef();
    }
    return !string.IsNullOrEmpty(this.m_cardId) ? DefLoader.Get().GetCardDef(this.m_cardId) : DefLoader.Get().GetCardDef("HiddenCard");
  }

  public Card GetCard() => this.m_card;

  public void Destroy()
  {
    if (!((UnityEngine.Object) this.m_card != (UnityEngine.Object) null))
      return;
    this.m_card.Destroy();
  }

  public Entity.LoadState GetLoadState() => this.m_loadState;

  public bool IsLoadingAssets() => this.m_loadState == Entity.LoadState.LOADING;

  public bool IsBusy() => this.IsLoadingAssets() || (UnityEngine.Object) this.m_card != (UnityEngine.Object) null && !this.m_card.IsActorReady();

  public bool IsHidden() => string.IsNullOrEmpty(this.m_cardId);

  public bool HasQueuedChangeEntity() => this.m_queuedChangeEntityCount > 0;

  public bool HasQueuedControllerTagChange() => this.m_queuedRealTimeControllerTagChangeCount > 0;

  public void SetTagAndHandleChange<TagEnum>(GAME_TAG tag, TagEnum tagValue) => this.SetTagAndHandleChange((int) tag, Convert.ToInt32((object) tagValue));

  public TagDelta SetTagAndHandleChange(int tag, int tagValue)
  {
    int tag1 = this.m_tags.GetTag(tag);
    this.SetTag(tag, tagValue);
    TagDelta change = new TagDelta();
    change.tag = tag;
    change.oldValue = tag1;
    change.newValue = tagValue;
    this.OnTagChanged(change);
    return change;
  }

  public override int GetReferencedTag(int tag) => this.GetEntityDef().GetReferencedTag(tag);

  public int GetDefCost() => this.GetEntityDef().GetCost();

  public int GetDefATK() => this.GetEntityDef().GetATK();

  public int GetDefHealth() => this.GetEntityDef().GetHealth();

  public int GetDefDurability() => this.GetEntityDef().GetDurability();

  public bool HasRace(TAG_RACE race) => (this.HasTag(GAME_TAG.CARDRACE) ? (int) this.GetTag<TAG_RACE>(GAME_TAG.CARDRACE) : (int) this.GetEntityDef().GetTag<TAG_RACE>(GAME_TAG.CARDRACE)) == 26 && race != TAG_RACE.INVALID || this.GetRaces().Contains(race);

  public override TAG_CLASS GetClass() => this.IsSecret() ? base.GetClass() : this.GetEntityDef().GetClass();

  public override void GetClasses(IList<TAG_CLASS> classes)
  {
    if (this.IsSecret())
      base.GetClasses(classes);
    else
      this.GetEntityDef().GetClasses(classes);
  }

  public TAG_ENCHANTMENT_VISUAL GetEnchantmentBirthVisual() => this.GetEntityDef().GetEnchantmentBirthVisual();

  public TAG_ENCHANTMENT_VISUAL GetEnchantmentIdleVisual() => this.GetEntityDef().GetEnchantmentIdleVisual();

  public TAG_RARITY GetRarity() => this.GetEntityDef().GetRarity();

  public new TAG_CARD_SET GetCardSet() => this.GetEntityDef().GetCardSet();

  public TAG_PREMIUM GetPremiumType()
  {
    TAG_PREMIUM premiumType = (TAG_PREMIUM) this.GetTag(GAME_TAG.PREMIUM);
    if (premiumType == TAG_PREMIUM.DIAMOND && !this.HasTag(GAME_TAG.HAS_DIAMOND_QUALITY))
      premiumType = TAG_PREMIUM.SIGNATURE;
    if (premiumType == TAG_PREMIUM.SIGNATURE && !this.HasTag(GAME_TAG.HAS_SIGNATURE_QUALITY))
      premiumType = TAG_PREMIUM.GOLDEN;
    return premiumType;
  }

  public bool CanBeDamagedRealTime() => !this.GetRealTimeDivineShield() && !this.GetRealTimeIsImmune() && (!this.GetRealTimeIsImmuneWhileAttacking() || !(bool) (UnityEngine.Object) TargetReticleManager.Get() || TargetReticleManager.Get().ArrowSourceEntityID != this.GetEntityId());

  public int GetCurrentHealth() => this.GetTag(GAME_TAG.HEALTH) - this.GetTag(GAME_TAG.DAMAGE) - this.GetTag(GAME_TAG.PREDAMAGE);

  public int GetCurrentDurability() => this.GetTag(GAME_TAG.DURABILITY) - this.GetTag(GAME_TAG.DAMAGE) - this.GetTag(GAME_TAG.PREDAMAGE);

  public int GetCurrentDefense() => this.GetCurrentHealth() + this.GetArmor();

  public int GetCurrentVitality()
  {
    if (this.IsCharacter())
      return this.GetCurrentDefense();
    if (this.IsWeapon())
      return this.GetCurrentDurability();
    Error.AddDevFatal("Entity.GetCurrentVitality() should not be called on {0}. This entity is neither a character nor a weapon.", (object) this);
    return 0;
  }

  public virtual Player GetController() => GameState.Get()?.GetPlayer(this.GetControllerId());

  public Player.Side GetControllerSide()
  {
    Player controller = this.GetController();
    return controller == null ? Player.Side.NEUTRAL : controller.GetSide();
  }

  public bool IsControlledByLocalUser() => this.GetController().IsLocalUser();

  public bool IsControlledByFriendlySidePlayer() => this.GetController().IsFriendlySide();

  public bool IsControlledByOpposingSidePlayer() => this.GetController().IsOpposingSide();

  public bool IsControlledByRevealedPlayer() => this.GetController().IsRevealed();

  public bool IsControlledByConcealedPlayer() => !this.IsControlledByRevealedPlayer();

  public Entity GetCreator() => GameState.Get().GetEntity(this.GetCreatorId());

  public EntityDef GetCreatorDef() => DefLoader.Get().GetEntityDef(this.GetCreatorDBID());

  public string GetDisplayedCreatorName() => this.m_displayedCreatorName;

  public virtual Entity GetHero()
  {
    if (this.IsHero())
      return this;
    return this.GetController()?.GetHero();
  }

  public virtual Card GetHeroCard()
  {
    if (this.IsHero())
      return this.GetCard();
    return this.GetController()?.GetHeroCard();
  }

  public virtual Entity GetHeroPower()
  {
    if (this.IsHeroPower())
      return this;
    return this.GetController()?.GetHeroPower();
  }

  public virtual Card GetHeroPowerCard()
  {
    if (this.IsHeroPower())
      return this.GetCard();
    return this.GetController()?.GetHeroPowerCard();
  }

  public virtual Card GetWeaponCard()
  {
    if (this.IsWeapon())
      return this.GetCard();
    return this.GetController()?.GetWeaponCard();
  }

  public virtual Card GetHeroBuddyCard()
  {
    if (this.IsBattlegroundHeroBuddy())
      return this.GetCard();
    return this.GetController()?.GetHeroBuddyCard();
  }

  public virtual Card GetQuestRewardFromHeroPowerCard()
  {
    if (this.IsBattlegroundQuestReward())
      return this.GetCard();
    return this.GetController()?.GetQuestRewardFromHeroPowerCard();
  }

  public virtual Card GetQuestRewardCard()
  {
    if (this.IsBattlegroundQuestReward())
      return this.GetCard();
    return this.GetController()?.GetQuestRewardCard();
  }

  public virtual List<Card> GetQuestRewardCards()
  {
    if (this.IsBattlegroundQuestReward())
      return new List<Card>() { this.GetCard() };
    Player controller = this.GetController();
    return controller == null ? new List<Card>() : controller.GetQuestRewardCards();
  }

  public virtual int GetHeroBuddyCardId()
  {
    if (!this.HasTag(GAME_TAG.BACON_SKIN) || !this.HasTag(GAME_TAG.BACON_SKIN_PARENT_ID))
      return this.GetTag(GAME_TAG.BACON_COMPANION_ID);
    int tag = this.GetTag(GAME_TAG.BACON_SKIN_PARENT_ID);
    using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(tag))
    {
      if (fullDef?.EntityDef != null && !((UnityEngine.Object) fullDef?.CardDef == (UnityEngine.Object) null))
        return fullDef.EntityDef.GetTag(GAME_TAG.BACON_COMPANION_ID);
      Log.Gameplay.PrintError("GetHeroBuddyId(): Unable to load def for card ID {0}.", (object) tag);
      return 0;
    }
  }

  public virtual bool HasValidDisplayName() => this.GetEntityDef().HasValidDisplayName();

  public virtual string GetName()
  {
    int tag = this.GetTag(GAME_TAG.OVERRIDECARDNAME);
    if (tag > 0)
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
      if (entityDef != null)
        return entityDef.GetName();
    }
    EntityDef entityDef1 = this.GetEntityDef();
    if (entityDef1 != null && entityDef1.GetCardTextBuilder() != null)
      return entityDef1.GetCardTextBuilder().BuildCardName(this);
    if (!string.IsNullOrEmpty(this.m_cardId))
      Debug.LogWarning((object) string.Format("Entity.GetName: No textbuilder found for {0}, returning default name", (object) this.m_cardId));
    return CardTextBuilder.GetDefaultCardName(this.GetEntityDef());
  }

  public virtual string GetDebugName()
  {
    if (this.m_cachedDebugName.Name == null)
      this.m_cachedDebugName.Dirty = true;
    if (this.m_cachedDebugName.Dirty)
    {
      string name = this.GetEntityDef().GetName();
      if (name != null)
        this.m_cachedDebugName.Name = string.Format("[entityName={0} id={1} zone={2} zonePos={3} cardId={4} player={5}]", (object) name, (object) this.GetEntityId(), (object) this.GetZone(), (object) this.GetZonePosition(), (object) this.m_cardId, (object) this.GetControllerId());
      else if (this.m_cardId != null)
        this.m_cachedDebugName.Name = string.Format("[id={0} cardId={1} type={2} zone={3} zonePos={4} player={5}]", (object) this.GetEntityId(), (object) this.m_cardId, (object) this.GetCardType(), (object) this.GetZone(), (object) this.GetZonePosition(), (object) this.GetControllerId());
      else
        this.m_cachedDebugName.Name = string.Format("UNKNOWN ENTITY [id={0} type={1} zone={2} zonePos={3}]", (object) this.GetEntityId(), (object) this.GetCardType(), (object) this.GetZone(), (object) this.GetZonePosition());
      this.m_cachedDebugName.Dirty = false;
    }
    return this.m_cachedDebugName.Name;
  }

  public void UpdateCardName()
  {
    this.m_cachedDebugName.Dirty = true;
    if ((UnityEngine.Object) this.m_card == (UnityEngine.Object) null)
      return;
    string name = this.GetEntityDef().GetName();
    if (name != null)
    {
      if (string.IsNullOrEmpty(this.m_cardId))
        this.m_card.gameObject.name = string.Format("{0} [id={1} zone={2} zonePos={3}]", (object) name, (object) this.GetEntityId(), (object) this.GetZone(), (object) this.GetZonePosition());
      else
        this.m_card.gameObject.name = string.Format("{0} [id={1} cardId={2} zone={3} zonePos={4} player={5}]", (object) name, (object) this.GetEntityId(), (object) this.GetCardId(), (object) this.GetZone(), (object) this.GetZonePosition(), (object) this.GetControllerId());
    }
    else
      this.m_card.gameObject.name = string.Format("Hidden Entity [id={0} zone={1} zonePos={2}]", (object) this.GetEntityId(), (object) this.GetZone(), (object) this.GetZonePosition());
    if (!((UnityEngine.Object) this.m_card.GetActor() != (UnityEngine.Object) null))
      return;
    this.m_card.GetActor().UpdateNameText();
  }

  public string GetCardTextInHand()
  {
    using (DefLoader.DisposableCardDef disposableCardDef = this.ShareDisposableCardDef())
    {
      if (!((UnityEngine.Object) disposableCardDef?.CardDef == (UnityEngine.Object) null))
        return this.GetCardTextBuilder().BuildCardTextInHand(this);
      Log.All.PrintError("Entity.GetCardTextInHand(): entity {0} does not have a CardDef", (object) this.GetEntityId());
      return string.Empty;
    }
  }

  public string GetCardTextInHistory()
  {
    using (DefLoader.DisposableCardDef disposableCardDef = this.ShareDisposableCardDef())
    {
      if (!((UnityEngine.Object) disposableCardDef?.CardDef == (UnityEngine.Object) null))
        return this.GetCardTextBuilder().BuildCardTextInHistory(this);
      Log.All.PrintError("Entity.GetCardTextInHand(): entity {0} does not have a CardDef", (object) this.GetEntityId());
      return string.Empty;
    }
  }

  public string GetTargetingArrowText()
  {
    using (DefLoader.DisposableCardDef disposableCardDef = this.ShareDisposableCardDef())
    {
      if (!((UnityEngine.Object) disposableCardDef?.CardDef == (UnityEngine.Object) null))
        return this.GetCardTextBuilder().GetTargetingArrowText(this);
      Log.All.PrintError("Entity.GetTargetingArrowText(): entity {0} does not have a CardDef", (object) this.GetEntityId());
      return string.Empty;
    }
  }

  public string GetRaceText() => this.GetEntityDef().GetRaceText();

  public void AddAttachment(Entity entity)
  {
    int count = this.m_attachments.Count;
    if (this.m_attachments.Contains(entity))
    {
      Log.Gameplay.Print(string.Format("Entity.AddAttachment() - {0} is already an attachment of {1}", (object) entity, (object) this));
    }
    else
    {
      this.m_attachments.Add(entity);
      if ((UnityEngine.Object) this.m_card == (UnityEngine.Object) null)
        return;
      this.m_card.OnEnchantmentAdded(count, entity);
    }
  }

  public void RemoveAttachment(Entity entity)
  {
    int count = this.m_attachments.Count;
    if (!this.m_attachments.Remove(entity))
    {
      Log.Gameplay.Print("Entity.RemoveAttachment() - {0} is not an attachment of {1}", (object) entity, (object) this);
    }
    else
    {
      if ((UnityEngine.Object) this.m_card == (UnityEngine.Object) null)
        return;
      this.m_card.OnEnchantmentRemoved(count, entity);
    }
  }

  private void AddSubCard(Entity entity)
  {
    if (this.m_subCardIDs.Contains(entity.GetEntityId()))
      return;
    this.m_subCardIDs.Add(entity.GetEntityId());
  }

  private void RemoveSubCard(Entity entity) => this.m_subCardIDs.Remove(entity.GetEntityId());

  private void RemoveLettuceAbilityEntityID(int entityID) => this.m_lettuceAbilityEntityIDs.Remove(entityID);

  private void AddLettuceAbilityEntityID(int entityID)
  {
    if (this.m_lettuceAbilityEntityIDs.Contains(entityID))
      return;
    this.m_lettuceAbilityEntityIDs.Add(entityID);
  }

  public List<int> GetLettuceAbilityEntityIDs() => this.m_lettuceAbilityEntityIDs;

  public int GetSelectedLettuceAbilityID()
  {
    int tag = this.GetTag(GAME_TAG.LETTUCE_ABILITY_TILE_VISUAL_SELF_ONLY);
    return tag > 0 ? tag : this.GetTag(GAME_TAG.LETTUCE_ABILITY_TILE_VISUAL_ALL_VISIBLE);
  }

  public List<Entity> GetAttachments() => this.m_attachments;

  public bool DoEnchantmentsHaveVoodooLink()
  {
    foreach (EntityBase attachment in this.m_attachments)
    {
      if (attachment.HasTag(GAME_TAG.VOODOO_LINK))
        return true;
    }
    return false;
  }

  public bool DoEnchantmentsHaveTriggerVisuals()
  {
    foreach (EntityBase attachment in this.m_attachments)
    {
      if (attachment.HasTriggerVisual())
        return true;
    }
    return false;
  }

  public bool DoEnchantmentsHaveOverKill()
  {
    foreach (EntityBase attachment in this.m_attachments)
    {
      if (attachment.HasTag(GAME_TAG.OVERKILL))
        return true;
    }
    return false;
  }

  public bool DoEnchantmentsHaveSpellburst()
  {
    foreach (EntityBase attachment in this.m_attachments)
    {
      if (attachment.HasSpellburst())
        return true;
    }
    return false;
  }

  public bool DoEnchantmentsHaveCounter()
  {
    foreach (EntityBase attachment in this.m_attachments)
    {
      if (attachment.HasCounter())
        return true;
    }
    return false;
  }

  public bool DoEnchantmentsHaveHonorableKill()
  {
    foreach (EntityBase attachment in this.m_attachments)
    {
      if (attachment.HasTag(GAME_TAG.HONORABLEKILL))
        return true;
    }
    return false;
  }

  public bool IsEnchanted() => this.m_attachments.Count > 0;

  public bool IsEnchantment() => this.GetRealTimeCardType() == TAG_CARDTYPE.ENCHANTMENT;

  public bool IsDarkWandererSecret() => this.IsSecret() && this.GetClass() == TAG_CLASS.WARRIOR;

  public bool IsDeathrattleDisabled() => this.HasTag(GAME_TAG.CANT_TRIGGER_DEATHRATTLE);

  public List<Entity> GetEnchantments() => this.GetAttachments();

  public List<Entity> GetDisplayedEnchantments(bool unique = false)
  {
    List<Entity> source = new List<Entity>((IEnumerable<Entity>) this.GetAttachments());
    source.RemoveAll((Predicate<Entity>) (enchant => enchant.HasTag(GAME_TAG.ENCHANTMENT_INVISIBLE)));
    return !unique ? source : source.Distinct<Entity>((IEqualityComparer<Entity>) new Entity.EnchantmentComparer()).ToList<Entity>();
  }

  public bool HasSubCards() => this.m_subCardIDs != null && this.m_subCardIDs.Count > 0;

  public List<int> GetSubCardIDs() => this.m_subCardIDs;

  public int GetSubCardIndex(Entity entity)
  {
    if (entity == null)
      return -1;
    int entityId = entity.GetEntityId();
    for (int index = 0; index < this.m_subCardIDs.Count; ++index)
    {
      if (this.m_subCardIDs[index] == entityId)
        return index;
    }
    return -1;
  }

  public Entity GetParentEntity()
  {
    int tag = this.GetTag(GAME_TAG.PARENT_CARD);
    return GameState.Get().GetEntity(tag);
  }

  public CardTextBuilder GetCardTextBuilder()
  {
    if (this.GetEntityDef() != null && this.GetEntityDef().GetCardTextBuilder() != null)
      return this.GetEntityDef().GetCardTextBuilder();
    if (!string.IsNullOrEmpty(this.m_cardId))
      Debug.LogWarning((object) string.Format("Entity.GetCardTextBuilder: No textbuilder found for {0}, returning fallback text builder", (object) this.m_cardId));
    return CardTextBuilder.GetFallbackCardTextBuilder();
  }

  public Entity CloneForZoneMgr()
  {
    Entity entity = new Entity();
    entity.m_staticEntityDef = this.GetEntityDef();
    entity.m_dynamicEntityDef = (EntityDef) null;
    entity.m_card = this.m_card;
    entity.m_cardId = this.m_cardId;
    entity.ReplaceTags(this.m_tags);
    entity.m_loadState = this.m_loadState;
    entity.m_cachedDebugName.Dirty = true;
    return entity;
  }

  public Entity CloneForHistory(HistoryInfo historyInfo)
  {
    Entity entity = new Entity();
    entity.m_duplicateForHistory = true;
    entity.m_staticEntityDef = this.GetEntityDef();
    entity.m_dynamicEntityDef = (EntityDef) null;
    entity.m_card = this.m_card;
    entity.m_cardId = this.m_cardId;
    entity.ReplaceTags(this.m_tags);
    entity.m_cardTextHistoryData = this.GetCardTextBuilder().CreateCardTextHistoryData();
    entity.m_cardTextHistoryData.SetHistoryData(this, historyInfo);
    entity.m_subCardIDs = this.m_subCardIDs;
    if (!this.IsHero())
      entity.SetTag<TAG_ZONE>(GAME_TAG.ZONE, TAG_ZONE.HAND);
    entity.m_loadState = this.m_loadState;
    entity.m_displayedCreatorName = this.m_displayedCreatorName;
    entity.m_enchantmentCreatorCardIDForPortrait = this.m_enchantmentCreatorCardIDForPortrait;
    return entity;
  }

  public bool IsHistoryDupe() => this.m_duplicateForHistory;

  public int GetJadeGolem() => Mathf.Min(this.GetController().GetTag(GAME_TAG.JADE_GOLEM) + 1, 30);

  private bool IsEnchantmentAffectedBySpellPower() => this.IsEnchantment() && this.IsAffectedBySpellPower();

  private Player GetControllerForDamageOrHealingBonus()
  {
    if (this.HasTag(GAME_TAG.SOURCE_OVERRIDE_FOR_MODIFIER_TEXT))
      return GameState.Get().GetEntity(this.GetTag(GAME_TAG.SOURCE_OVERRIDE_FOR_MODIFIER_TEXT)).GetController();
    if (this.IsLettuceAbility())
    {
      Entity lettuceAbilityOwner = this.GetLettuceAbilityOwner();
      if (lettuceAbilityOwner != null)
        return lettuceAbilityOwner.GetController();
    }
    Entity parentEntity1 = this.GetParentEntity();
    if ((parentEntity1 != null ? (parentEntity1.IsLettuceAbility() ? 1 : 0) : 0) != 0)
    {
      Entity parentEntity2 = this.GetParentEntity();
      if (parentEntity2 != null)
      {
        Entity lettuceAbilityOwner = parentEntity2.GetLettuceAbilityOwner();
        if (lettuceAbilityOwner != null)
          return lettuceAbilityOwner.GetController();
      }
    }
    return this.GetController();
  }

  public int GetDamageBonus()
  {
    Player damageOrHealingBonus = this.GetControllerForDamageOrHealingBonus();
    if (damageOrHealingBonus == null)
      return 0;
    if (this.IsSpell() || this.IsMinion() || this.IsLettuceAbilitySpellCasting() || this.IsEnchantmentAffectedBySpellPower())
    {
      int damageBonus = damageOrHealingBonus.TotalSpellpower(this, this.GetSpellSchool());
      if (this.HasTag(GAME_TAG.RECEIVES_DOUBLE_SPELLDAMAGE_BONUS))
        damageBonus *= 2;
      return damageBonus;
    }
    if (!this.IsHeroPower())
      return 0;
    int tag = damageOrHealingBonus.GetTag(GAME_TAG.CURRENT_HEROPOWER_DAMAGE_BONUS);
    if (this.GetCardTextBuilder() is SpellDamageOnlyCardTextBuilder)
    {
      int num = damageOrHealingBonus.TotalSpellpower(this, this.GetSpellSchool());
      if (this.HasTag(GAME_TAG.RECEIVES_DOUBLE_SPELLDAMAGE_BONUS))
        num *= 2;
      tag += num;
    }
    return tag;
  }

  public int GetDamageBonusDouble()
  {
    Player damageOrHealingBonus = this.GetControllerForDamageOrHealingBonus();
    if (damageOrHealingBonus == null)
      return 0;
    if (this.IsSpell() || this.IsLettuceAbilitySpellCasting() || this.IsEnchantmentAffectedBySpellPower())
      return damageOrHealingBonus.GetTag(GAME_TAG.SPELLPOWER_DOUBLE);
    return this.IsHeroPower() ? damageOrHealingBonus.GetTag(GAME_TAG.HERO_POWER_DOUBLE) : 0;
  }

  public int GetHealingBonus()
  {
    Player damageOrHealingBonus = this.GetControllerForDamageOrHealingBonus();
    return damageOrHealingBonus == null || !this.IsSpell() && !this.IsLettuceAbilitySpellCasting() && !this.IsEnchantmentAffectedBySpellPower() ? 0 : damageOrHealingBonus.GetTag(GAME_TAG.CURRENT_HEALING_POWER);
  }

  public int GetHealingDouble()
  {
    Player damageOrHealingBonus = this.GetControllerForDamageOrHealingBonus();
    if (damageOrHealingBonus == null)
      return 0;
    int tag = damageOrHealingBonus.GetTag(GAME_TAG.ALL_HEALING_DOUBLE);
    if (this.IsSpell() || this.IsLettuceAbilitySpellCasting() || this.IsEnchantmentAffectedBySpellPower())
      return damageOrHealingBonus.GetTag(GAME_TAG.SPELL_HEALING_DOUBLE) + tag;
    return this.IsHeroPower() ? damageOrHealingBonus.GetTag(GAME_TAG.HERO_POWER_DOUBLE) + tag : tag;
  }

  public void ClearBattlecryFlag() => this.m_useBattlecryPower = false;

  public void UpdateUseBattlecryFlag(bool fromGameState)
  {
    if (!this.IsMinion())
      return;
    bool flag = fromGameState || GameState.Get().EntityHasTargets(this);
    if (!(TAG_ZONE.HAND == this.GetZone() & flag))
      return;
    this.m_useBattlecryPower = true;
  }

  public virtual void InitRealTimeValues(List<Network.Entity.Tag> tags)
  {
    foreach (Network.Entity.Tag tag in tags)
    {
      switch ((GAME_TAG) tag.Name)
      {
        case GAME_TAG.PREMIUM:
          this.SetRealTimePremium((TAG_PREMIUM) tag.Value);
          continue;
        case GAME_TAG.DAMAGE:
          this.SetRealTimeDamage(tag.Value);
          continue;
        case GAME_TAG.HEALTH:
        case GAME_TAG.DURABILITY:
          this.SetRealTimeHealth(tag.Value);
          continue;
        case GAME_TAG.ATK:
          this.SetRealTimeAttack(tag.Value);
          continue;
        case GAME_TAG.COST:
          this.SetRealTimeCost(tag.Value);
          continue;
        case GAME_TAG.ZONE:
          this.SetRealTimeZone(tag.Value);
          continue;
        case GAME_TAG.DIVINE_SHIELD:
          this.SetRealTimeDivineShield(tag.Value);
          continue;
        case GAME_TAG.CARDTYPE:
          this.SetRealTimeCardType((TAG_CARDTYPE) tag.Value);
          continue;
        case GAME_TAG.IMMUNE:
          this.SetRealTimeIsImmune(tag.Value);
          continue;
        case GAME_TAG.LINKED_ENTITY:
          this.SetRealTimeLinkedEntityId(tag.Value);
          continue;
        case GAME_TAG.ZONE_POSITION:
          this.SetRealTimeZonePosition(tag.Value);
          continue;
        case GAME_TAG.ARMOR:
          this.SetRealTimeArmor(tag.Value);
          continue;
        case GAME_TAG.POISONOUS:
        case GAME_TAG.NON_KEYWORD_POISONOUS:
          this.SetRealTimeIsPoisonous(tag.Value);
          continue;
        case GAME_TAG.IMMUNE_WHILE_ATTACKING:
          this.SetRealTimeIsImmuneWhileAttacking(tag.Value);
          continue;
        case GAME_TAG.POWERED_UP:
          this.SetRealTimePoweredUp(tag.Value);
          continue;
        case GAME_TAG.CARD_COSTS_HEALTH:
          this.SetRealTimeCardCostsHealth(tag.Value);
          continue;
        case GAME_TAG.ATTACKABLE_BY_RUSH:
          this.SetRealTimeAttackableByRush(tag.Value);
          continue;
        case GAME_TAG.PLAYER_LEADERBOARD_PLACE:
          this.SetRealTimePlayerLeaderboardPlace(tag.Value);
          continue;
        case GAME_TAG.PLAYER_TECH_LEVEL:
          this.SetRealTimePlayerTechLevel(tag.Value);
          continue;
        case GAME_TAG.CARD_COSTS_ARMOR:
          this.SetRealTimeCardCostsArmor(tag.Value);
          continue;
        default:
          continue;
      }
    }
  }

  public void SetRealTimeCost(int newCost) => this.m_realTimeCost = newCost;

  public int GetRealTimeCost() => this.m_realTimeCost == -1 ? this.GetCost() : this.m_realTimeCost;

  public void SetRealTimeAttack(int newAttack) => this.m_realTimeAttack = newAttack;

  public int GetRealTimeAttack() => this.m_realTimeAttack;

  public void SetRealTimeHealth(int newHealth) => this.m_realTimeHealth = newHealth;

  public void SetRealTimeDamage(int newDamage) => this.m_realTimeDamage = newDamage;

  public void SetRealTimeArmor(int newArmor) => this.m_realTimeArmor = newArmor;

  public int GetRealTimeRemainingHP() => this.m_realTimeHealth + this.m_realTimeArmor - this.m_realTimeDamage;

  public void SetRealTimeZone(int zone) => this.m_realTimeZone = zone;

  public TAG_ZONE GetRealTimeZone() => (TAG_ZONE) this.m_realTimeZone;

  public void SetRealTimeZonePosition(int zonePosition) => this.m_realTimeZonePosition = zonePosition;

  public int GetRealTimeZonePosition() => this.m_realTimeZonePosition;

  public void SetRealTimeLinkedEntityId(int linkedEntityId) => this.m_realTimeLinkedEntityId = linkedEntityId;

  public int GetRealTimeLinkedEntityId() => this.m_realTimeLinkedEntityId;

  public void SetRealTimePoweredUp(int poweredUp) => this.m_realTimePoweredUp = poweredUp > 0;

  public bool GetRealTimePoweredUp() => this.m_realTimePoweredUp;

  public void SetRealTimeDivineShield(int divineShield) => this.m_realTimeDivineShield = divineShield > 0;

  public bool GetRealTimeDivineShield() => this.m_realTimeDivineShield;

  public void SetRealTimeIsImmune(int immune) => this.m_realTimeIsImmune = immune > 0;

  public bool GetRealTimeIsImmune() => this.m_realTimeIsImmune;

  public void SetRealTimeIsImmuneWhileAttacking(int immune) => this.m_realTimeIsImmuneWhileAttacking = immune > 0;

  public bool GetRealTimeIsImmuneWhileAttacking() => this.m_realTimeIsImmuneWhileAttacking;

  public void SetRealTimeIsPoisonous(int poisonous) => this.m_realTimeIsPoisonous = poisonous > 0;

  public bool GetRealTimeIsPoisonous() => this.m_realTimeIsPoisonous;

  public void SetRealTimeIsDormant(int dormant) => this.m_realTimeIsDormant = dormant > 0;

  public bool GetRealTimeIsDormant() => this.m_realTimeIsDormant;

  public void SetRealTimeHasSpellpower(int spellpower) => this.m_realTimeSpellpower = spellpower;

  public int GetRealTimeSpellpower() => this.m_realTimeSpellpower;

  public void SetRealTimeSpellpowerDouble(int powerDouble) => this.m_realTimeSpellpowerDouble = powerDouble > 0;

  public bool GetRealTimeSpellpowerDouble() => this.m_realTimeSpellpowerDouble;

  public void SetRealTimeHealingDoesDamageHint(int healingDoesDamageHint) => this.m_realTimeHealingDoesDamageHint = healingDoesDamageHint > 0;

  public bool GetRealTimeHealingDoeDamageHint() => this.m_realTimeHealingDoesDamageHint;

  public void SetRealTimeLifestealDoesDamageHint(int lifestealDoesDamageHint) => this.m_realTimeLifestealDoesDamageHint = lifestealDoesDamageHint > 0;

  public bool GetRealTimeLifestealDoesDamageHint() => this.m_realTimeLifestealDoesDamageHint;

  public void SetRealTimeCardCostsHealth(int value) => this.m_realTimeCardCostsHealth = value > 0;

  public bool GetRealTimeCardCostsHealth() => this.m_realTimeCardCostsHealth;

  public void SetRealTimeCardCostsArmor(int value) => this.m_realTimeCardCostsArmor = value > 0;

  public bool GetRealTimeCardCostsArmor() => this.m_realTimeCardCostsArmor;

  public void SetRealTimeAttackableByRush(int value) => this.m_realTimeAttackableByRush = value > 0;

  public bool GetRealTimeAttackableByRush() => this.m_realTimeAttackableByRush;

  public void SetRealTimeCardType(TAG_CARDTYPE cardType) => this.m_realTimeCardType = cardType;

  public TAG_CARDTYPE GetRealTimeCardType() => this.m_realTimeCardType;

  public void SetRealTimePremium(TAG_PREMIUM premium) => this.m_realTimePremium = premium;

  public void SetRealTimePlayerLeaderboardPlace(int playerLeaderboardPlace) => this.m_realTimePlayerLeaderboardPlace = playerLeaderboardPlace;

  public int GetRealTimePlayerLeaderboardPlace() => this.m_realTimePlayerLeaderboardPlace;

  public void SetRealTimePlayerTechLevel(int playerTechLevel) => this.m_realTimePlayerTechLevel = playerTechLevel;

  public int GetRealTimePlayerTechLevel() => this.m_realTimePlayerTechLevel;

  public CardTextHistoryData GetCardTextHistoryData() => this.m_cardTextHistoryData;

  private void LoadEntityDef(string cardId)
  {
    if (this.m_cardId != cardId)
      this.m_cardId = cardId;
    if (string.IsNullOrEmpty(cardId))
      return;
    this.m_dynamicEntityDef = (EntityDef) null;
    this.m_staticEntityDef = DefLoader.Get().GetEntityDef(cardId);
    if (this.m_staticEntityDef == null)
      Error.AddDevFatal("Failed to load a card xml for {0}", (object) cardId);
    else
      this.UpdateCardName();
  }

  public void LoadCard(string cardId, Entity.LoadCardData data = null)
  {
    this.LoadEntityDef(cardId);
    this.m_loadState = Entity.LoadState.LOADING;
    if (string.IsNullOrEmpty(cardId))
    {
      DefLoader.Get().LoadCardDef("HiddenCard", new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnCardDefLoaded));
    }
    else
    {
      CardPortraitQuality quality = CardPortraitQuality.GetDefault();
      quality.PremiumType = this.m_realTimePremium;
      DefLoader.Get().LoadCardDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>(this.OnCardDefLoaded), (object) data, quality);
    }
  }

  private void OnCardDefLoaded(
    string cardId,
    DefLoader.DisposableCardDef cardDef,
    object callbackData)
  {
    using (cardDef)
    {
      if (cardDef == null)
      {
        Debug.LogErrorFormat("Entity.OnCardDefLoaded() - {0} does not have an asset!", (object) cardId);
        this.m_loadState = Entity.LoadState.DONE;
      }
      else
      {
        Entity.LoadCardData loadCardData = new Entity.LoadCardData()
        {
          updateActor = false,
          restartStateSpells = false,
          fromChangeEntity = false
        };
        if (callbackData is Entity.LoadCardData)
          loadCardData = (Entity.LoadCardData) callbackData;
        if ((UnityEngine.Object) this.m_card != (UnityEngine.Object) null)
        {
          this.m_card.SetCardDef(cardDef, loadCardData.updateActor);
          if (loadCardData.updateActor)
          {
            this.m_card.UpdateActor();
            this.m_card.ActivateStateSpells();
          }
          else if (loadCardData.restartStateSpells)
            this.m_card.ActivateStateSpells(true);
          this.m_card.RefreshCardsInTooltip();
          if (loadCardData.fromChangeEntity && this.IsMinion() && this.m_card.GetZone() is ZonePlay)
            this.m_card.ActivateCharacterPlayEffects();
        }
        this.UpdateUseBattlecryFlag(false);
        this.m_loadState = Entity.LoadState.DONE;
        if (!((UnityEngine.Object) this.m_card != (UnityEngine.Object) null))
          return;
        this.m_card.RefreshActor();
      }
    }
  }

  public SpellType GetTriggerSpellType()
  {
    GameState gameState = GameState.Get();
    GameMgr gameMgr = GameMgr.Get();
    return gameMgr != null && gameMgr.IsBattlegrounds() && gameState != null && gameState.IsUsingFastActorTriggers() && !this.IsHeroPower() ? SpellType.FAST_TRIGGER : SpellType.TRIGGER;
  }

  public SpellType GetPrioritizedBaubleSpellType()
  {
    if (this.IsPoisonous())
      return SpellType.POISONOUS;
    if (this.HasTriggerVisual() || this.DoEnchantmentsHaveTriggerVisuals())
      return this.GetTriggerSpellType();
    if (this.HasLifesteal())
      return SpellType.LIFESTEAL;
    if (this.HasInspire())
      return SpellType.INSPIRE;
    if (this.HasOverKill() || this.DoEnchantmentsHaveOverKill())
      return SpellType.OVERKILL;
    if (this.HasSpellburst() || this.DoEnchantmentsHaveSpellburst() || this.DoEnchantmentsHaveCounter())
      return SpellType.SPELLBURST;
    if (this.HasFrenzy())
      return SpellType.FRENZY;
    if (this.HasAvenge())
      return SpellType.AVENGE;
    return this.HasHonorableKill() || this.DoEnchantmentsHaveHonorableKill() ? SpellType.HONORABLEKILL : SpellType.NONE;
  }

  public TAG_CARD_SET GetWatermarkCardSetOverride()
  {
    if (this.HasTag(GAME_TAG.WATERMARK_OVERRIDE_CARD_SET))
      return (TAG_CARD_SET) this.GetTag(GAME_TAG.WATERMARK_OVERRIDE_CARD_SET);
    EntityDef entityDef = this.GetEntityDef();
    return entityDef != null && entityDef.HasTag(GAME_TAG.WATERMARK_OVERRIDE_CARD_SET) ? (TAG_CARD_SET) entityDef.GetTag(GAME_TAG.WATERMARK_OVERRIDE_CARD_SET) : TAG_CARD_SET.INVALID;
  }

  public bool IsTauntIgnored() => GameState.Get().GetFirstOpponentPlayer(this.GetController()).HasTag(GAME_TAG.IGNORE_TAUNT);

  public Entity GetLettuceAbilityOwner() => GameState.Get().GetEntity(this.GetTag(GAME_TAG.LETTUCE_ABILITY_OWNER));

  public bool IsMyLettuceRoleStrongAgainst(Entity otherEntity)
  {
    if (otherEntity == null)
      return false;
    TAG_ROLE tag1 = this.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);
    TAG_ROLE tag2 = otherEntity.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);
    return tag1 == TAG_ROLE.CASTER && tag2 == TAG_ROLE.TANK || tag1 == TAG_ROLE.TANK && tag2 == TAG_ROLE.FIGHTER || tag1 == TAG_ROLE.FIGHTER && tag2 == TAG_ROLE.CASTER;
  }

  public bool HasSelectedLettuceAbility() => this.HasTag(GAME_TAG.LETTUCE_ABILITY_TILE_VISUAL_ALL_VISIBLE) || this.HasTag(GAME_TAG.LETTUCE_ABILITY_TILE_VISUAL_SELF_ONLY);

  public bool IsMercenary() => this.HasTag(GAME_TAG.LETTUCE_MERCENARY);

  public bool ShouldShowEquipmentTextOnMerc()
  {
    int tag = this.GetTag(GAME_TAG.LETTUCE_EQUIPMENT_ID);
    if (tag == 0)
      return false;
    LettuceEquipmentTierDbfRecord equipmentTierFromCardId = GameDbf.GetIndex().GetEquipmentTierFromCardID(tag);
    return equipmentTierFromCardId != null && equipmentTierFromCardId.ShowTextOnMerc;
  }

  public Entity GetEquipmentEntity()
  {
    int tag = this.GetTag(GAME_TAG.LETTUCE_EQUIPMENT_ID);
    if (tag == 0)
      return (Entity) null;
    foreach (int lettuceAbilityEntityId in this.GetLettuceAbilityEntityIDs())
    {
      Entity entity = GameState.Get().GetEntity(lettuceAbilityEntityId);
      if (GameUtils.TranslateCardIdToDbId(entity.GetCardId()) == tag)
        return entity;
    }
    return (Entity) null;
  }

  public enum LoadState
  {
    INVALID,
    PENDING,
    LOADING,
    DONE,
  }

  public class LoadCardData
  {
    public bool updateActor;
    public bool restartStateSpells;
    public bool fromChangeEntity;
  }

  private struct CachedDebugName
  {
    public bool Dirty;
    public string Name;
  }

  private class EnchantmentComparer : IEqualityComparer<Entity>
  {
    public bool Equals(Entity a, Entity b)
    {
      if (a == b)
        return true;
      return a != null && b != null && !(a.GetCardId() != b.GetCardId()) && a.GetCardTextInHand() == b.GetCardTextInHand();
    }

    public int GetHashCode(Entity entity) => entity == null || entity.GetCardId() == null ? 0 : entity.GetCardId().GetHashCode();
  }
}
