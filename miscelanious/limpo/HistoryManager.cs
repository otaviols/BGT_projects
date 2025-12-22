using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : CardTileListDisplay
{
  public Texture m_mageSecretTexture;
  public Texture m_paladinSecretTexture;
  public Texture m_hunterSecretTexture;
  public Texture m_rogueSecretTexture;
  public Texture m_wandererSecretTexture;
  public Texture m_FatigueTexture;
  public Texture m_BurnedCardsTexture;
  public Spell[] m_TransformSpells;
  private const float BIG_CARD_POWER_PROCESSOR_DELAY_TIME = 1f;
  private const float BIG_CARD_SPELL_DISPLAY_TIME = 4f;
  private const float BIG_CARD_MINION_DISPLAY_TIME = 3f;
  private const float BIG_CARD_LETTUCE_ABILITY_DISPLAY_TIME = 1.65f;
  private const float BIG_CARD_HERO_POWER_DISPLAY_TIME = 4f;
  private const float BIG_CARD_SECRET_DISPLAY_TIME = 4f;
  private const float BIG_CARD_POST_TRANSFORM_DISPLAY_TIME = 2f;
  private const float BIG_CARD_META_DATA_DEFAULT_DISPLAY_TIME = 1.5f;
  private const float BIG_CARD_META_DATA_FAST_DISPLAY_TIME = 1f;
  private const float SPACE_BETWEEN_TILES = 0.15f;
  private static HistoryManager s_instance;
  private bool m_historyDisabled;
  private List<HistoryCard> m_historyTiles = new List<HistoryCard>();
  private HistoryCard m_currentlyMousedOverTile;
  private List<HistoryManager.TileEntry> m_queuedEntries = new List<HistoryManager.TileEntry>();
  private HistoryManager.TileEntryBuffer m_queuedEntriesPrevious = new HistoryManager.TileEntryBuffer();
  private Vector3[] m_bigCardPath;
  private Vector3[] m_lettuceAbilityBigCardPath;
  private HistoryManager.BigCardEntry m_pendingBigCardEntry;
  private HistoryCard m_currentBigCard;
  private bool m_showingBigCard;
  private bool m_bigCardWaitingForSecret;
  private bool m_bigCardWaitingForLettuceSpeedTile;
  private HistoryManager.BigCardTransformState m_bigCardTransformState;
  private Spell m_bigCardTransformSpell;

  protected override void Awake()
  {
    base.Awake();
    HistoryManager.s_instance = this;
    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.15f, this.transform.position.z);
    this.m_queuedEntriesPrevious.Clear();
  }

  protected override void OnDestroy()
  {
    HistoryManager.s_instance = (HistoryManager) null;
    base.OnDestroy();
  }

  protected override void Start()
  {
    base.Start();
    this.StartCoroutine(this.WaitForBoardLoadedAndSetPaths());
  }

  public static HistoryManager Get() => HistoryManager.s_instance;

  public bool IsHistoryEnabled() => !this.m_historyDisabled;

  public void DisableHistory()
  {
    this.m_historyDisabled = true;
    this.GetComponent<Collider>().enabled = false;
  }

  public void EnableHistory()
  {
    this.m_historyDisabled = false;
    this.GetComponent<Collider>().enabled = true;
  }

  private Entity CreatePreTransformedEntity(Entity entity)
  {
    int tag = entity.GetTag(GAME_TAG.TRANSFORMED_FROM_CARD);
    if (tag == 0)
      return (Entity) null;
    string cardId = GameUtils.TranslateDbIdToCardId(tag);
    if (string.IsNullOrEmpty(cardId))
      return (Entity) null;
    Entity transformedEntity = new Entity();
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
    transformedEntity.InitCard();
    transformedEntity.ReplaceTags(entityDef.GetTags());
    transformedEntity.LoadCard(cardId);
    transformedEntity.SetTag(GAME_TAG.CONTROLLER, entity.GetControllerId());
    transformedEntity.SetTag<TAG_ZONE>(GAME_TAG.ZONE, TAG_ZONE.HAND);
    transformedEntity.SetTag<TAG_PREMIUM>(GAME_TAG.PREMIUM, entity.GetPremiumType());
    transformedEntity.SetTag<TAG_CARD_SET>(GAME_TAG.WATERMARK_OVERRIDE_CARD_SET, entity.GetWatermarkCardSetOverride());
    return transformedEntity;
  }

  private Entity CreatePostTransformedEntity(Entity entity)
  {
    string cardId = entity.GetCardId();
    if (string.IsNullOrEmpty(cardId))
      return (Entity) null;
    Entity transformedEntity = new Entity();
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
    transformedEntity.InitCard();
    transformedEntity.ReplaceTags(entityDef.GetTags());
    transformedEntity.LoadCard(cardId);
    transformedEntity.SetTag(GAME_TAG.CONTROLLER, entity.GetControllerId());
    transformedEntity.SetTag<TAG_ZONE>(GAME_TAG.ZONE, TAG_ZONE.HAND);
    transformedEntity.SetTag<TAG_PREMIUM>(GAME_TAG.PREMIUM, entity.GetPremiumType());
    transformedEntity.SetTag<TAG_CARD_SET>(GAME_TAG.WATERMARK_OVERRIDE_CARD_SET, entity.GetWatermarkCardSetOverride());
    return transformedEntity;
  }

  private Entity CreateSecretDeathrattleEntity(Entity entity)
  {
    if (!entity.HasSecretDeathrattle())
      return (Entity) null;
    string cardId = "GIL_222t";
    if (string.IsNullOrEmpty(cardId))
      return (Entity) null;
    Entity deathrattleEntity = new Entity();
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
    deathrattleEntity.InitCard();
    deathrattleEntity.ReplaceTags(entityDef.GetTags());
    deathrattleEntity.LoadCard(cardId);
    deathrattleEntity.SetTag(GAME_TAG.CONTROLLER, entity.GetControllerId());
    deathrattleEntity.SetTag<TAG_ZONE>(GAME_TAG.ZONE, TAG_ZONE.HAND);
    deathrattleEntity.SetTag<TAG_PREMIUM>(GAME_TAG.PREMIUM, entity.GetPremiumType());
    deathrattleEntity.SetTag<TAG_CARD_SET>(GAME_TAG.WATERMARK_OVERRIDE_CARD_SET, entity.GetWatermarkCardSetOverride());
    return deathrattleEntity;
  }

  public void CreatePlayedTile(Entity playedEntity, Entity targetedEntity)
  {
    if (this.m_historyDisabled)
      return;
    HistoryManager.TileEntry tileEntry = new HistoryManager.TileEntry();
    this.m_queuedEntries.Add(tileEntry);
    tileEntry.SetCardPlayed(playedEntity);
    tileEntry.SetCardTargeted(targetedEntity);
    if (tileEntry.m_lastCardPlayed.GetDuplicatedEntity() != null)
      return;
    this.StartCoroutine("WaitForCardLoadedAndDuplicateInfo", (object) tileEntry.m_lastCardPlayed);
  }

  public void CreateTriggerTile(Entity triggeredEntity)
  {
    if (this.m_historyDisabled)
      return;
    HistoryManager.TileEntry tileEntry = new HistoryManager.TileEntry();
    this.m_queuedEntries.Add(tileEntry);
    tileEntry.SetCardTriggered(triggeredEntity);
  }

  public void CreateAttackTile(Entity attacker, Entity defender, PowerTaskList taskList)
  {
    if (this.m_historyDisabled)
      return;
    HistoryManager.TileEntry tileEntry = new HistoryManager.TileEntry();
    this.m_queuedEntries.Add(tileEntry);
    tileEntry.SetAttacker(attacker);
    tileEntry.SetDefender(defender);
    Entity duplicatedEntity1 = tileEntry.m_lastAttacker.GetDuplicatedEntity();
    Entity duplicatedEntity2 = tileEntry.m_lastDefender.GetDuplicatedEntity();
    int entityId1 = attacker.GetEntityId();
    int entityId2 = defender.GetEntityId();
    int num = -1;
    List<PowerTask> taskList1 = taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      Network.PowerHistory power = taskList1[index].GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.DAMAGE && histMetaData.Info.Contains(entityId2))
        {
          num = index;
          break;
        }
      }
    }
    for (int index = 0; index < num; ++index)
    {
      Network.PowerHistory power = taskList1[index].GetPower();
      switch (power.Type)
      {
        case Network.PowerType.SHOW_ENTITY:
          Network.HistShowEntity showEntity = (Network.HistShowEntity) power;
          if (entityId1 == showEntity.Entity.ID)
            GameUtils.ApplyShowEntity(duplicatedEntity1, showEntity);
          if (entityId2 == showEntity.Entity.ID)
          {
            GameUtils.ApplyShowEntity(duplicatedEntity2, showEntity);
            break;
          }
          break;
        case Network.PowerType.HIDE_ENTITY:
          Network.HistHideEntity hideEntity = (Network.HistHideEntity) power;
          if (entityId1 == hideEntity.Entity)
            GameUtils.ApplyHideEntity(duplicatedEntity1, hideEntity);
          if (entityId2 == hideEntity.Entity)
          {
            GameUtils.ApplyHideEntity(duplicatedEntity2, hideEntity);
            break;
          }
          break;
        case Network.PowerType.TAG_CHANGE:
          Network.HistTagChange tagChange = (Network.HistTagChange) power;
          if (entityId1 == tagChange.Entity)
            GameUtils.ApplyTagChange(duplicatedEntity1, tagChange);
          if (entityId2 == tagChange.Entity)
          {
            GameUtils.ApplyTagChange(duplicatedEntity2, tagChange);
            break;
          }
          break;
      }
    }
  }

  public void CreateFatigueTile()
  {
    if (this.m_historyDisabled)
      return;
    HistoryManager.TileEntry tileEntry = new HistoryManager.TileEntry();
    this.m_queuedEntries.Add(tileEntry);
    tileEntry.SetFatigue();
  }

  public void CreateBurnedCardsTile()
  {
    if (this.m_historyDisabled)
      return;
    HistoryManager.TileEntry tileEntry = new HistoryManager.TileEntry();
    this.m_queuedEntries.Add(tileEntry);
    tileEntry.SetBurnedCards();
  }

  public void MarkCurrentHistoryEntryAsCompleted()
  {
    if (this.m_historyDisabled)
      return;
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry == null)
    {
      Log.Power.Print("HistoryManager.MarkCurrentHistoryEntryAsCompleted: There is no current History Entry!");
    }
    else
    {
      currentHistoryEntry.m_complete = true;
      this.m_queuedEntriesPrevious.AddHistoryEntry(ref currentHistoryEntry);
      this.LoadNextHistoryEntry();
    }
  }

  public bool HasHistoryEntry() => this.GetCurrentHistoryEntry() != null;

  public void NotifyDamageChanged(Entity entity, int damage)
  {
    if (entity == null || this.m_historyDisabled)
      return;
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry == null)
      Log.Power.Print("HistoryManager.NotifyDamageChanged: There is no current History Entry!");
    else if (this.IsEntityTheLastCardPlayed(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastCardPlayed.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int num = damage - duplicatedEntity.GetDamage();
      currentHistoryEntry.m_lastCardPlayed.m_damageChangeAmount = num;
    }
    else if (this.IsEntityTheLastAttacker(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastAttacker.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int num = damage - duplicatedEntity.GetDamage();
      currentHistoryEntry.m_lastAttacker.m_damageChangeAmount = num;
    }
    else if (this.IsEntityTheLastDefender(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastDefender.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int num = damage - duplicatedEntity.GetDamage();
      currentHistoryEntry.m_lastDefender.m_damageChangeAmount = num;
    }
    else if (this.IsEntityTheLastCardTargeted(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastCardTargeted.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int num = damage - duplicatedEntity.GetDamage();
      currentHistoryEntry.m_lastCardTargeted.m_damageChangeAmount = num;
    }
    else
    {
      for (int index = 0; index < currentHistoryEntry.m_affectedCards.Count; ++index)
      {
        if (this.IsEntityTheAffectedCard(entity, index))
        {
          Entity duplicatedEntity = currentHistoryEntry.m_affectedCards[index].GetDuplicatedEntity();
          if (duplicatedEntity == null)
            return;
          int num = damage - duplicatedEntity.GetDamage();
          currentHistoryEntry.m_affectedCards[index].m_damageChangeAmount = num;
          return;
        }
      }
      if (!this.NotifyEntityAffected(entity, false, false))
        return;
      this.NotifyDamageChanged(entity, damage);
    }
  }

  public void NotifyArmorChanged(Entity entity, int newArmor)
  {
    if (entity == null || this.m_historyDisabled || entity.GetArmor() - newArmor <= 0 || this.IsEntityTheLastCardPlayed(entity))
      return;
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry == null)
      Log.Power.Print("HistoryManager.NotifyArmorChanged: There is no current History Entry!");
    else if (this.IsEntityTheLastAttacker(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastAttacker.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int b = duplicatedEntity.GetArmor() - newArmor;
      currentHistoryEntry.m_lastAttacker.m_armorChangeAmount = Mathf.Max(currentHistoryEntry.m_lastAttacker.m_armorChangeAmount, b);
    }
    else if (this.IsEntityTheLastDefender(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastDefender.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int b = duplicatedEntity.GetArmor() - newArmor;
      currentHistoryEntry.m_lastDefender.m_armorChangeAmount = Mathf.Max(currentHistoryEntry.m_lastDefender.m_armorChangeAmount, b);
    }
    else if (this.IsEntityTheLastCardTargeted(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastCardTargeted.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int b = duplicatedEntity.GetArmor() - newArmor;
      currentHistoryEntry.m_lastCardTargeted.m_armorChangeAmount = Mathf.Max(currentHistoryEntry.m_lastCardTargeted.m_armorChangeAmount, b);
    }
    else
    {
      for (int index = 0; index < currentHistoryEntry.m_affectedCards.Count; ++index)
      {
        if (this.IsEntityTheAffectedCard(entity, index))
        {
          Entity duplicatedEntity = currentHistoryEntry.m_affectedCards[index].GetDuplicatedEntity();
          if (duplicatedEntity == null)
            return;
          int b = duplicatedEntity.GetArmor() - newArmor;
          currentHistoryEntry.m_affectedCards[index].m_armorChangeAmount = Mathf.Max(currentHistoryEntry.m_affectedCards[index].m_armorChangeAmount, b);
          return;
        }
      }
      if (!this.NotifyEntityAffected(entity, false, false))
        return;
      this.NotifyArmorChanged(entity, newArmor);
    }
  }

  public void NotifyHealthChanged(Entity entity, int health)
  {
    if (entity == null || this.m_historyDisabled)
      return;
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry == null)
      Log.Power.Print("HistoryManager.NotifyHealthChanged: There is no current History Entry!");
    else if (this.IsEntityTheLastCardPlayed(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastCardPlayed.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int num = health - duplicatedEntity.GetHealth();
      currentHistoryEntry.m_lastCardPlayed.m_maxHealthChangeAmount = num;
    }
    else if (this.IsEntityTheLastAttacker(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastAttacker.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int num = health - duplicatedEntity.GetHealth();
      currentHistoryEntry.m_lastAttacker.m_maxHealthChangeAmount = num;
    }
    else if (this.IsEntityTheLastDefender(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastDefender.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int num = health - duplicatedEntity.GetHealth();
      currentHistoryEntry.m_lastDefender.m_maxHealthChangeAmount = num;
    }
    else if (this.IsEntityTheLastCardTargeted(entity))
    {
      Entity duplicatedEntity = currentHistoryEntry.m_lastCardTargeted.GetDuplicatedEntity();
      if (duplicatedEntity == null)
        return;
      int num = health - duplicatedEntity.GetHealth();
      currentHistoryEntry.m_lastCardTargeted.m_maxHealthChangeAmount = num;
    }
    else
    {
      for (int index = 0; index < currentHistoryEntry.m_affectedCards.Count; ++index)
      {
        if (this.IsEntityTheAffectedCard(entity, index))
        {
          Entity duplicatedEntity = currentHistoryEntry.m_affectedCards[index].GetDuplicatedEntity();
          if (duplicatedEntity == null)
            return;
          int num = health - duplicatedEntity.GetHealth();
          currentHistoryEntry.m_affectedCards[index].m_maxHealthChangeAmount = num;
          return;
        }
      }
      if (!this.NotifyEntityAffected(entity, false, false))
        return;
      this.NotifyHealthChanged(entity, health);
    }
  }

  public void OverrideCurrentHistoryEntryWithMetaData()
  {
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry == null || currentHistoryEntry.m_usingMetaDataOverride)
      return;
    currentHistoryEntry.m_usingMetaDataOverride = true;
    currentHistoryEntry.m_affectedCards.Clear();
  }

  public void OverrideCurrentHistoryTriggerSource(Entity entity)
  {
    if (entity == null)
      return;
    this.GetCurrentHistoryEntry()?.SetCardTriggered(entity);
  }

  public void OverrideCurrentHistorySourceOwner(Entity entity)
  {
    if (entity == null)
      return;
    this.GetCurrentHistoryEntry()?.SetSourceOwner(entity);
  }

  private HistoryInfo GetHistoryInfoForEntity(
    HistoryManager.TileEntry entry,
    Entity entity)
  {
    if (this.IsEntityTheLastAttacker(entity))
      return entry.m_lastAttacker;
    if (this.IsEntityTheLastDefender(entity))
      return entry.m_lastDefender;
    if (this.IsEntityTheLastCardTargeted(entity))
      return entry.m_lastCardTargeted;
    if (entry.m_lastCardPlayed != null && entity == entry.m_lastCardPlayed.GetOriginalEntity())
      return entry.m_lastCardPlayed;
    for (int index = 0; index < entry.m_affectedCards.Count; ++index)
    {
      if (this.IsEntityTheAffectedCard(entry, entity, index))
        return entry.m_affectedCards[index];
    }
    return (HistoryInfo) null;
  }

  public bool NotifyEntityAffected(
    int entityId,
    bool allowDuplicates,
    bool fromMetaData,
    bool dontDuplicateUntilEnd = false,
    bool isBurnedCard = false,
    bool isPoisonous = false,
    bool isCriticalHit = false)
  {
    return this.NotifyEntityAffected(GameState.Get().GetEntity(entityId), allowDuplicates, fromMetaData, dontDuplicateUntilEnd, isBurnedCard, isPoisonous, isCriticalHit);
  }

  public bool NotifyEntityAffected(
    Entity entity,
    bool allowDuplicates,
    bool fromMetaData,
    bool dontDuplicateUntilEnd = false,
    bool isBurnedCard = false,
    bool isPoisonous = false,
    bool isCriticalHit = false)
  {
    if (entity == null || this.m_historyDisabled || entity.IsEnchantment())
      return false;
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry != null)
    {
      if (!fromMetaData && currentHistoryEntry.m_usingMetaDataOverride)
        return false;
      if (!allowDuplicates)
      {
        HistoryInfo historyInfoForEntity = this.GetHistoryInfoForEntity(currentHistoryEntry, entity);
        if (historyInfoForEntity != null)
        {
          if (dontDuplicateUntilEnd)
            historyInfoForEntity.m_dontDuplicateUntilEnd = dontDuplicateUntilEnd;
          if (isBurnedCard)
            historyInfoForEntity.m_isBurnedCard = isBurnedCard;
          if (isPoisonous)
            historyInfoForEntity.m_isPoisonous = isPoisonous;
          if (isCriticalHit)
            historyInfoForEntity.m_isCriticalHit = isCriticalHit;
          return false;
        }
      }
      HistoryInfo historyInfo = new HistoryInfo();
      historyInfo.m_dontDuplicateUntilEnd = dontDuplicateUntilEnd;
      historyInfo.m_isBurnedCard = isBurnedCard;
      historyInfo.m_isPoisonous = isPoisonous;
      historyInfo.m_isCriticalHit = isCriticalHit;
      historyInfo.SetOriginalEntity(entity);
      currentHistoryEntry.m_affectedCards.Add(historyInfo);
      return true;
    }
    for (int index = 0; index < this.m_queuedEntriesPrevious.Length; ++index)
    {
      HistoryManager.TileEntry historyEntry = this.m_queuedEntriesPrevious.GetHistoryEntry(index);
      if (historyEntry == null)
      {
        Log.Power.Print("HistoryManager.NotifyEntityAffected(): There is no current History Entry!");
        return false;
      }
      if ((fromMetaData || !historyEntry.m_usingMetaDataOverride) && !allowDuplicates)
      {
        HistoryInfo historyInfoForEntity = this.GetHistoryInfoForEntity(historyEntry, entity);
        if (historyInfoForEntity != null)
        {
          if (dontDuplicateUntilEnd)
            historyInfoForEntity.m_dontDuplicateUntilEnd = dontDuplicateUntilEnd;
          if (isBurnedCard)
            historyInfoForEntity.m_isBurnedCard = isBurnedCard;
          if (isPoisonous)
            historyInfoForEntity.m_isPoisonous = isPoisonous;
          return false;
        }
      }
    }
    return false;
  }

  public void NotifyEntityDied(int entityId) => this.NotifyEntityDied(GameState.Get().GetEntity(entityId));

  public void NotifyEntityDied(Entity entity)
  {
    if (this.m_historyDisabled || entity.IsEnchantment() || this.IsEntityTheLastCardPlayed(entity))
      return;
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (this.IsEntityTheLastAttacker(entity))
      currentHistoryEntry.m_lastAttacker.SetDied(true);
    else if (this.IsEntityTheLastDefender(entity))
      currentHistoryEntry.m_lastDefender.SetDied(true);
    else if (this.IsEntityTheLastCardTargeted(entity))
    {
      currentHistoryEntry.m_lastCardTargeted.SetDied(true);
    }
    else
    {
      if (currentHistoryEntry != null)
      {
        for (int index = 0; index < currentHistoryEntry.m_affectedCards.Count; ++index)
        {
          if (this.IsEntityTheAffectedCard(entity, index))
          {
            currentHistoryEntry.m_affectedCards[index].SetDied(true);
            return;
          }
        }
      }
      if (this.IsDeadInLaterHistoryEntry(entity) || !this.NotifyEntityAffected(entity, false, false))
        return;
      this.NotifyEntityDied(entity);
    }
  }

  public void NotifyOfInput(float zPosition)
  {
    if (this.m_historyTiles.Count == 0)
      this.CheckForMouseOff();
    else if (GameState.Get().GetGameEntity().ShouldSuppressHistoryMouseOver())
    {
      this.CheckForMouseOff();
    }
    else
    {
      float num1 = 1000f;
      float num2 = -1000f;
      float num3 = 1000f;
      HistoryCard historyCard = (HistoryCard) null;
      foreach (HistoryCard historyTile in this.m_historyTiles)
      {
        if (historyTile.HasBeenShown())
        {
          Collider tileCollider = historyTile.GetTileCollider();
          if (!((Object) tileCollider == (Object) null))
          {
            Bounds bounds = tileCollider.bounds;
            double z1 = (double) bounds.center.z;
            bounds = tileCollider.bounds;
            double z2 = (double) bounds.extents.z;
            float num4 = (float) (z1 - z2);
            bounds = tileCollider.bounds;
            double z3 = (double) bounds.center.z;
            bounds = tileCollider.bounds;
            double z4 = (double) bounds.extents.z;
            float num5 = (float) (z3 + z4);
            if ((double) num4 < (double) num1)
              num1 = num4;
            if ((double) num5 > (double) num2)
              num2 = num5;
            float num6 = Mathf.Abs(zPosition - num4);
            if ((double) num6 < (double) num3)
            {
              num3 = num6;
              historyCard = historyTile;
            }
            float num7 = Mathf.Abs(zPosition - num5);
            if ((double) num7 < (double) num3)
            {
              num3 = num7;
              historyCard = historyTile;
            }
          }
        }
      }
      if ((double) zPosition < (double) num1 || (double) zPosition > (double) num2)
        this.CheckForMouseOff();
      else if ((Object) historyCard == (Object) null)
      {
        this.CheckForMouseOff();
      }
      else
      {
        this.m_SoundDucker.StartDucking();
        if ((Object) historyCard == (Object) this.m_currentlyMousedOverTile)
          return;
        if ((Object) this.m_currentlyMousedOverTile != (Object) null)
          this.m_currentlyMousedOverTile.NotifyMousedOut();
        else
          this.FadeVignetteIn();
        this.m_currentlyMousedOverTile = historyCard;
        historyCard.NotifyMousedOver();
      }
    }
  }

  public void NotifyOfMouseOff() => this.CheckForMouseOff();

  public void UpdateLayout()
  {
    if (this.UserIsMousedOverAHistoryTile())
      return;
    float num1 = 0.0f;
    Vector3 topTilePosition = this.GetTopTilePosition();
    for (int index = this.m_historyTiles.Count - 1; index >= 0; --index)
    {
      int num2 = 0;
      if (this.m_historyTiles[index].IsHalfSize())
        num2 = 1;
      Collider tileCollider = this.m_historyTiles[index].GetTileCollider();
      float num3 = 0.0f;
      Bounds bounds;
      if ((Object) tileCollider != (Object) null)
      {
        bounds = tileCollider.bounds;
        num3 = bounds.size.z / 2f;
      }
      Vector3 position = new Vector3(topTilePosition.x, topTilePosition.y, (float) ((double) topTilePosition.z - (double) num1 + (double) num2 * (double) num3));
      this.m_historyTiles[index].MarkAsShown();
      iTween.MoveTo(this.m_historyTiles[index].gameObject, position, 1f);
      if ((Object) tileCollider != (Object) null)
      {
        double num4 = (double) num1;
        bounds = tileCollider.bounds;
        double num5 = (double) bounds.size.z + 0.150000005960464;
        num1 = (float) (num4 + num5);
      }
    }
    this.DestroyHistoryTilesThatFallOffTheEnd();
  }

  public int GetNumHistoryTiles() => this.m_historyTiles.Count;

  public int GetIndexForTile(HistoryCard tile)
  {
    for (int index = 0; index < this.m_historyTiles.Count; ++index)
    {
      if ((Object) this.m_historyTiles[index] == (Object) tile)
        return index;
    }
    Debug.LogWarning((object) "HistoryManager.GetIndexForTile() - that Tile doesn't exist!");
    return -1;
  }

  public void OnEntityRevealed() => this.GetCurrentHistoryEntry()?.DuplicateAllEntities(false);

  private void LoadNextHistoryEntry()
  {
    if (this.m_queuedEntries.Count == 0 || !this.m_queuedEntries[0].m_complete)
      return;
    this.StartCoroutine(this.LoadNextHistoryEntryWhenLoaded());
  }

  private IEnumerator LoadNextHistoryEntryWhenLoaded()
  {
    HistoryManager historyManager = this;
    HistoryManager.TileEntry currentEntry = historyManager.m_queuedEntries[0];
    historyManager.m_queuedEntries.RemoveAt(0);
    while (!currentEntry.CanDuplicateAllEntities(true, true))
      yield return (object) null;
    if (currentEntry.GetSourceInfo() != null && currentEntry.GetSourceInfo().GetOriginalEntity() != null && currentEntry.GetSourceInfo().GetOriginalEntity().IsEnchantment())
    {
      historyManager.LoadNextHistoryEntry();
    }
    else
    {
      currentEntry.DuplicateAllEntities(true, true);
      HistoryInfo sourceInfo = currentEntry.GetSourceInfo();
      if (sourceInfo == null || !sourceInfo.HasValidDisplayEntity())
      {
        historyManager.LoadNextHistoryEntry();
      }
      else
      {
        historyManager.CreateTransformTile(sourceInfo);
        HistoryManager.TileLoadedCallbackData callbackData = new HistoryManager.TileLoadedCallbackData()
        {
          m_sourceInfo = sourceInfo,
          m_sourceOwnerInfo = currentEntry.m_sourceOwner
        };
        HistoryInfo targetInfo = currentEntry.GetTargetInfo();
        if (targetInfo != null)
          callbackData.m_childInfos.Add(targetInfo);
        if (currentEntry.m_affectedCards.Count > 0)
          callbackData.m_childInfos.AddRange((IEnumerable<HistoryInfo>) currentEntry.m_affectedCards);
        AssetLoader.Get().InstantiatePrefab((AssetReference) "HistoryCard.prefab:f8193c3e146b62342b8fb2c0494ec447", new PrefabCallback<GameObject>(historyManager.TileLoadedCallback), (object) callbackData, AssetLoadingOptions.IgnorePrefabPosition);
      }
    }
  }

  private void CreateTransformTile(HistoryInfo sourceInfo)
  {
    if (sourceInfo.m_infoType == HistoryInfoType.FATIGUE || sourceInfo.m_infoType == HistoryInfoType.BURNED_CARDS)
      return;
    Entity duplicatedEntity = sourceInfo.GetDuplicatedEntity();
    Entity originalEntity = sourceInfo.GetOriginalEntity();
    if (duplicatedEntity == null || originalEntity == null)
      return;
    int tag = duplicatedEntity.GetTag(GAME_TAG.TRANSFORMED_FROM_CARD);
    if (tag == 0 || string.IsNullOrEmpty(GameUtils.TranslateDbIdToCardId(tag)))
      return;
    Entity transformedEntity1 = this.CreatePreTransformedEntity(duplicatedEntity);
    HistoryInfo historyInfo1 = new HistoryInfo();
    historyInfo1.SetOriginalEntity(transformedEntity1);
    historyInfo1.DuplicateEntity(true, true);
    Entity transformedEntity2 = this.CreatePostTransformedEntity(originalEntity);
    HistoryInfo historyInfo2 = new HistoryInfo();
    historyInfo2.SetOriginalEntity(transformedEntity2);
    historyInfo2.DuplicateEntity(true, true);
    HistoryManager.TileLoadedCallbackData callbackData = new HistoryManager.TileLoadedCallbackData()
    {
      m_sourceInfo = historyInfo1
    };
    callbackData.m_childInfos.Add(historyInfo2);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "HistoryCard.prefab:f8193c3e146b62342b8fb2c0494ec447", new PrefabCallback<GameObject>(this.TileLoadedCallback), (object) callbackData, AssetLoadingOptions.IgnorePrefabPosition);
  }

  private IEnumerator WaitForCardLoadedAndDuplicateInfo(HistoryInfo info)
  {
    while (!info.CanDuplicateEntity(false))
      yield return (object) null;
    info.DuplicateEntity(false, false);
  }

  private bool IsEntityTheLastCardTargeted(Entity entity)
  {
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry == null)
    {
      Log.Power.Print("HistoryManager.IsEntityTheLastCardTargeted: There is no current History Entry!");
      return false;
    }
    return currentHistoryEntry.m_lastCardTargeted != null && entity == currentHistoryEntry.m_lastCardTargeted.GetOriginalEntity();
  }

  private bool IsEntityTheLastAttacker(Entity entity)
  {
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry == null)
    {
      Log.Power.Print("HistoryManager.IsEntityTheLastAttacker: There is no current History Entry!");
      return false;
    }
    return currentHistoryEntry.m_lastAttacker != null && entity == currentHistoryEntry.m_lastAttacker.GetOriginalEntity();
  }

  private bool IsEntityTheLastCardPlayed(Entity entity)
  {
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry == null)
    {
      Log.Power.Print("HistoryManager.IsEntityTheLastCardPlayed: There is no current History Entry!");
      return false;
    }
    return currentHistoryEntry.m_lastCardPlayed != null && entity == currentHistoryEntry.m_lastCardPlayed.GetOriginalEntity();
  }

  private bool IsEntityTheLastDefender(Entity entity)
  {
    HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
    if (currentHistoryEntry == null)
    {
      Log.Power.Print("HistoryManager.IsEntityTheLastDefender: There is no current History Entry!");
      return false;
    }
    return currentHistoryEntry.m_lastDefender != null && entity == currentHistoryEntry.m_lastDefender.GetOriginalEntity();
  }

  private bool IsEntityTheAffectedCard(Entity entity, int index) => this.IsEntityTheAffectedCard(this.GetCurrentHistoryEntry(), entity, index);

  private bool IsEntityTheAffectedCard(HistoryManager.TileEntry entry, Entity entity, int index)
  {
    if (entry == null)
    {
      Log.Power.Print("HistoryManager.IsEntityTheAffectedCard: There is no current History Entry!");
      return false;
    }
    return entry.m_affectedCards[index] != null && entity == entry.m_affectedCards[index].GetOriginalEntity();
  }

  private HistoryManager.TileEntry GetCurrentHistoryEntry()
  {
    if (this.m_queuedEntries.Count == 0)
      return (HistoryManager.TileEntry) null;
    for (int index = this.m_queuedEntries.Count - 1; index >= 0; --index)
    {
      if (!this.m_queuedEntries[index].m_complete)
        return this.m_queuedEntries[index];
    }
    return (HistoryManager.TileEntry) null;
  }

  private bool IsDeadInLaterHistoryEntry(Entity entity)
  {
    bool flag = false;
    for (int index1 = this.m_queuedEntries.Count - 1; index1 >= 0; --index1)
    {
      HistoryManager.TileEntry queuedEntry = this.m_queuedEntries[index1];
      if (!queuedEntry.m_complete)
        return flag;
      for (int index2 = 0; index2 < queuedEntry.m_affectedCards.Count; ++index2)
      {
        HistoryInfo affectedCard = queuedEntry.m_affectedCards[index2];
        if (affectedCard.GetOriginalEntity() == entity && affectedCard.HasDied())
          flag = true;
      }
    }
    return false;
  }

  private void TileLoadedCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    HistoryManager.TileLoadedCallbackData loadedCallbackData = (HistoryManager.TileLoadedCallbackData) callbackData;
    HistoryInfo mSourceInfo = loadedCallbackData.m_sourceInfo;
    HistoryTileInitInfo info = new HistoryTileInitInfo()
    {
      m_type = mSourceInfo.m_infoType,
      m_ownerInfo = loadedCallbackData.m_sourceOwnerInfo,
      m_childInfos = loadedCallbackData.m_childInfos
    };
    if (info.m_type == HistoryInfoType.FATIGUE)
      info.m_fatigueTexture = this.m_FatigueTexture;
    else if (info.m_type == HistoryInfoType.BURNED_CARDS)
    {
      info.m_burnedCardsTexture = this.m_BurnedCardsTexture;
    }
    else
    {
      Entity duplicatedEntity = mSourceInfo.GetDuplicatedEntity();
      info.m_cardDef = duplicatedEntity.ShareDisposableCardDef();
      info.m_entity = duplicatedEntity;
      info.m_portraitTexture = this.DeterminePortraitTextureForTiles(duplicatedEntity, info.m_cardDef.CardDef);
      info.m_portraitGoldenMaterial = info.m_cardDef.CardDef.GetPremiumPortraitMaterial();
      info.m_cardDef.CardDef.TryGetHistoryTileFullPortrait(duplicatedEntity.GetPremiumType(), out info.m_fullTileMaterial);
      info.m_cardDef.CardDef.TryGetHistoryTileHalfPortrait(duplicatedEntity.GetPremiumType(), out info.m_halfTileMaterial);
      info.m_splatAmount = mSourceInfo.GetSplatAmount();
      info.m_dead = mSourceInfo.HasDied();
      info.m_burned = mSourceInfo.m_isBurnedCard;
      info.m_isPoisonous = mSourceInfo.m_isPoisonous;
      info.m_isCriticalHit = mSourceInfo.m_isCriticalHit;
    }
    using (info.m_cardDef)
    {
      HistoryCard component = go.GetComponent<HistoryCard>();
      this.m_historyTiles.Add(component);
      component.LoadTile(info);
      this.SetAsideTileAndTryToUpdate(component);
      this.LoadNextHistoryEntry();
    }
  }

  public Texture DeterminePortraitTextureForTiles(Entity entity, CardDef cardDef) => !entity.IsSecret() || !entity.IsHidden() || !entity.IsControlledByConcealedPlayer() ? (entity.GetController() == null || entity.GetController().IsFriendlySide() || !entity.IsObfuscated() ? cardDef.GetPortraitTexture(entity.GetPremiumType()) : this.m_paladinSecretTexture) : (entity.GetClass() != TAG_CLASS.PALADIN ? (entity.GetClass() != TAG_CLASS.HUNTER ? (entity.GetClass() != TAG_CLASS.ROGUE ? (!entity.IsDarkWandererSecret() ? this.m_mageSecretTexture : this.m_wandererSecretTexture) : this.m_rogueSecretTexture) : this.m_hunterSecretTexture) : this.m_paladinSecretTexture);

  private void CheckForMouseOff()
  {
    if ((Object) this.m_currentlyMousedOverTile == (Object) null)
      return;
    this.m_currentlyMousedOverTile.NotifyMousedOut();
    this.m_currentlyMousedOverTile = (HistoryCard) null;
    this.m_SoundDucker.StopDucking();
    this.FadeVignetteOut();
  }

  private void DestroyHistoryTilesThatFallOffTheEnd()
  {
    if (this.m_historyTiles.Count == 0)
      return;
    float num1 = 0.0f;
    float z = this.GetComponent<Collider>().bounds.size.z;
    for (int index = 0; index < this.m_historyTiles.Count; ++index)
      num1 += this.m_historyTiles[index].GetTileSize();
    float num2 = num1 + 0.15f * (float) (this.m_historyTiles.Count - 1);
    while ((double) num2 > (double) z)
    {
      num2 = num2 - this.m_historyTiles[0].GetTileSize() - 0.15f;
      Object.Destroy((Object) this.m_historyTiles[0].gameObject);
      this.m_historyTiles.RemoveAt(0);
    }
  }

  private void SetAsideTileAndTryToUpdate(HistoryCard tile)
  {
    Vector3 topTilePosition = this.GetTopTilePosition();
    tile.transform.position = new Vector3(topTilePosition.x - 20f, topTilePosition.y, topTilePosition.z);
    this.UpdateLayout();
  }

  private Vector3 GetTopTilePosition() => new Vector3(this.transform.position.x, this.transform.position.y - 0.15f, this.transform.position.z);

  private bool UserIsMousedOverAHistoryTile()
  {
    RaycastHit hitInfo;
    if (UniversalInputManager.Get().IsTouchMode() && !InputCollection.GetMouseButton(0) || UniversalInputManager.Get().GetInputHitInfo((LayerMask) GameLayer.Default.LayerBit(), out hitInfo) && (Object) hitInfo.transform.GetComponentInChildren<HistoryManager>() == (Object) null && (Object) hitInfo.transform.GetComponentInChildren<HistoryCard>() == (Object) null)
      return false;
    float z = hitInfo.point.z;
    float num1 = 1000f;
    float num2 = -1000f;
    foreach (HistoryCard historyTile in this.m_historyTiles)
    {
      if (historyTile.HasBeenShown())
      {
        Collider tileCollider = historyTile.GetTileCollider();
        if (!((Object) tileCollider == (Object) null))
        {
          float num3 = tileCollider.bounds.center.z - tileCollider.bounds.extents.z;
          float num4 = tileCollider.bounds.center.z + tileCollider.bounds.extents.z;
          if ((double) num3 < (double) num1)
            num1 = num3;
          if ((double) num4 > (double) num2)
            num2 = num4;
        }
      }
    }
    return (double) z >= (double) num1 && (double) z <= (double) num2;
  }

  private void FadeVignetteIn()
  {
    foreach (HistoryCard historyTile in this.m_historyTiles)
    {
      if (!((Object) historyTile.m_tileActor == (Object) null))
        LayerUtils.SetLayer(historyTile.m_tileActor.gameObject, GameLayer.IgnoreFullScreenEffects);
    }
    LayerUtils.SetLayer(this.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.AnimateVignetteIn();
  }

  private void FadeVignetteOut()
  {
    foreach (HistoryCard historyTile in this.m_historyTiles)
    {
      if (!((Object) historyTile.m_tileActor == (Object) null))
        LayerUtils.SetLayer(historyTile.GetTileCollider().gameObject, GameLayer.Default);
    }
    LayerUtils.SetLayer(this.gameObject, GameLayer.CardRaycast);
    this.AnimateVignetteOut();
  }

  protected override void OnFullScreenEffectOutFinished()
  {
    foreach (HistoryCard historyTile in this.m_historyTiles)
    {
      if (!((Object) historyTile.m_tileActor == (Object) null))
        LayerUtils.SetLayer(historyTile.m_tileActor.gameObject, GameLayer.Default);
    }
  }

  public bool IsShowingBigCard() => this.m_showingBigCard;

  public bool HasBigCard() => (Object) this.m_currentBigCard != (Object) null;

  public HistoryCard GetCurrentBigCard() => this.m_currentBigCard;

  public Entity GetPendingBigCardEntity() => this.m_pendingBigCardEntry == null ? (Entity) null : this.m_pendingBigCardEntry.m_info.GetOriginalEntity();

  public void CreateFastBigCardFromMetaData(Entity entity)
  {
    int displayTimeMS = 1000;
    this.CreatePlayedBigCard(entity, (HistoryManager.BigCardStartedCallback) (() => { }), (HistoryManager.BigCardFinishedCallback) (() => { }), true, false, displayTimeMS);
  }

  public void CreatePlayedBigCard(
    Entity entity,
    HistoryManager.BigCardStartedCallback startedCallback,
    HistoryManager.BigCardFinishedCallback finishedCallback,
    bool fromMetaData,
    bool countered,
    int displayTimeMS)
  {
    if (!GameState.Get().GetGameEntity().ShouldShowBigCard())
    {
      finishedCallback();
    }
    else
    {
      this.m_showingBigCard = true;
      this.StopCoroutine("WaitForCardLoadedAndCreateBigCard");
      HistoryManager.BigCardEntry bigCardEntry = new HistoryManager.BigCardEntry();
      bigCardEntry.m_info = new HistoryInfo();
      bigCardEntry.m_info.SetOriginalEntity(entity);
      bigCardEntry.m_info.m_infoType = !entity.IsWeapon() ? HistoryInfoType.CARD_PLAYED : HistoryInfoType.WEAPON_PLAYED;
      bigCardEntry.m_startedCallback = startedCallback;
      bigCardEntry.m_finishedCallback = finishedCallback;
      bigCardEntry.m_fromMetaData = fromMetaData;
      bigCardEntry.m_countered = countered;
      bigCardEntry.m_displayTimeMS = displayTimeMS;
      this.StartCoroutine("WaitForCardLoadedAndCreateBigCard", (object) bigCardEntry);
    }
  }

  public void CreateTriggeredBigCard(
    Entity entity,
    HistoryManager.BigCardStartedCallback startedCallback,
    HistoryManager.BigCardFinishedCallback finishedCallback,
    bool fromMetaData,
    bool isSecret)
  {
    if (!GameState.Get().GetGameEntity().ShouldShowBigCard() || entity.IsBobQuest())
    {
      finishedCallback();
    }
    else
    {
      this.m_showingBigCard = true;
      this.StopCoroutine("WaitForCardLoadedAndCreateBigCard");
      HistoryManager.BigCardEntry bigCardEntry = new HistoryManager.BigCardEntry();
      bigCardEntry.m_info = new HistoryInfo();
      bigCardEntry.m_info.SetOriginalEntity(entity);
      bigCardEntry.m_info.m_infoType = HistoryInfoType.TRIGGER;
      bigCardEntry.m_fromMetaData = fromMetaData;
      bigCardEntry.m_startedCallback = startedCallback;
      bigCardEntry.m_finishedCallback = finishedCallback;
      bigCardEntry.m_waitForSecretSpell = isSecret;
      this.StartCoroutine("WaitForCardLoadedAndCreateBigCard", (object) bigCardEntry);
    }
  }

  public void NotifyOfSecretSpellFinished() => this.m_bigCardWaitingForSecret = false;

  public void NotifyOfLettuceSpeedTileSpellFinished() => this.m_bigCardWaitingForLettuceSpeedTile = false;

  public void HandleClickOnBigCard(HistoryCard card)
  {
    if (!((Object) this.m_currentBigCard != (Object) null) || !((Object) this.m_currentBigCard == (Object) card))
      return;
    this.OnCurrentBigCardClicked();
  }

  public string GetBigCardBoneName()
  {
    string bigCardBoneName = "BigCardPosition";
    if ((bool) UniversalInputManager.UsePhoneUI)
      bigCardBoneName += "_phone";
    return bigCardBoneName;
  }

  private IEnumerator WaitForBoardLoadedAndSetPaths()
  {
    while ((Object) ZoneMgr.Get() == (Object) null)
      yield return (object) null;
    while ((Object) Gameplay.Get()?.GetBoardLayout() == (Object) null)
      yield return (object) null;
    Transform bone = Board.Get()?.FindBone("BigCardPathPoint");
    if (!((Object) bone == (Object) null))
    {
      Vector3 bigCardPosition = this.GetBigCardPosition();
      this.m_bigCardPath = new Vector3[3];
      this.m_bigCardPath[1] = bone.position;
      this.m_bigCardPath[2] = bigCardPosition;
      this.m_lettuceAbilityBigCardPath = new Vector3[2];
      this.m_lettuceAbilityBigCardPath[1] = bigCardPosition;
    }
  }

  private Vector3 GetBigCardPosition()
  {
    if (PlatformSettings.IsTablet)
    {
      Transform bone = Board.Get().FindBone("BigCardPosition_tablet");
      if ((Object) bone != (Object) null)
        return bone.position;
    }
    return Board.Get().FindBone(this.GetBigCardBoneName()).position;
  }

  private IEnumerator WaitForCardLoadedAndCreateBigCard(
    HistoryManager.BigCardEntry bigCardEntry)
  {
    HistoryManager historyManager = this;
    historyManager.m_pendingBigCardEntry = bigCardEntry;
    HistoryInfo info = bigCardEntry.m_info;
    while (!info.CanDuplicateEntity(false))
      yield return (object) null;
    bigCardEntry.m_startedCallback();
    info.DuplicateEntity(false, false);
    historyManager.m_pendingBigCardEntry = (HistoryManager.BigCardEntry) null;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "HistoryCard.prefab:f8193c3e146b62342b8fb2c0494ec447", new PrefabCallback<GameObject>(historyManager.BigCardLoadedCallback), (object) bigCardEntry, AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void BigCardLoadedCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    HistoryManager.BigCardEntry bigCardEntry = (HistoryManager.BigCardEntry) callbackData;
    Entity entity1 = bigCardEntry.m_info.GetDuplicatedEntity();
    Card card1 = entity1.GetCard();
    DefLoader.DisposableCardDef disposableCardDef = card1.ShareDisposableCardDef();
    if (entity1.IsLettuceAbility())
    {
      Card card2 = entity1.GetLettuceAbilityOwner()?.GetCard();
      if ((Object) card2 != (Object) null)
      {
        go.transform.position = card2.transform.position;
        go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
      }
      else
        go.transform.position = this.GetBigCardPosition();
    }
    else if (entity1.IsSpell() || entity1.IsHeroPower() || bigCardEntry.m_fromMetaData)
    {
      go.transform.position = card1.transform.position;
      go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
    }
    else
      go.transform.position = this.GetBigCardPosition();
    Entity transformedEntity = this.CreatePreTransformedEntity(entity1);
    Entity entity2 = (Entity) null;
    if (transformedEntity != null)
    {
      entity2 = entity1;
      entity1 = transformedEntity;
      Card card3 = entity1.GetCard();
      disposableCardDef?.Dispose();
      disposableCardDef = card3.ShareDisposableCardDef();
    }
    Entity deathrattleEntity = this.CreateSecretDeathrattleEntity(entity1);
    if (deathrattleEntity != null)
    {
      entity1 = deathrattleEntity;
      Card card4 = entity1.GetCard();
      disposableCardDef?.Dispose();
      disposableCardDef = card4.ShareDisposableCardDef();
    }
    using (disposableCardDef)
    {
      HistoryBigCardInitInfo info = new HistoryBigCardInitInfo();
      info.m_historyInfoType = bigCardEntry.m_info.m_infoType;
      info.m_entity = entity1;
      info.m_portraitTexture = disposableCardDef.CardDef.GetPortraitTexture(entity1.GetPremiumType());
      info.m_portraitGoldenMaterial = disposableCardDef.CardDef.GetPremiumPortraitMaterial();
      info.m_cardDef = disposableCardDef;
      info.m_finishedCallback = bigCardEntry.m_finishedCallback;
      info.m_countered = bigCardEntry.m_countered;
      info.m_waitForSecretSpell = bigCardEntry.m_waitForSecretSpell;
      info.m_fromMetaData = bigCardEntry.m_fromMetaData;
      info.m_postTransformedEntity = entity2;
      info.m_displayTimeMS = bigCardEntry.m_displayTimeMS;
      HistoryCard component = go.GetComponent<HistoryCard>();
      component.LoadBigCard(info);
      if ((bool) (Object) this.m_currentBigCard)
        this.InterruptCurrentBigCard();
      this.m_currentBigCard = component;
      this.StartCoroutine("WaitThenShowBigCard");
    }
  }

  private IEnumerator WaitThenShowBigCard()
  {
    HistoryManager historyManager = this;
    if (historyManager.m_currentBigCard.IsBigCardWaitingForSecret())
    {
      historyManager.m_bigCardWaitingForSecret = true;
      historyManager.m_currentBigCard.transform.localScale = new Vector3(1f / 1000f, 1f / 1000f, 1f / 1000f);
      while (historyManager.m_bigCardWaitingForSecret)
        yield return (object) null;
      if (historyManager.m_currentBigCard.HasBigCardPostTransformedEntity())
        historyManager.m_bigCardTransformState = HistoryManager.BigCardTransformState.PRE_TRANSFORM;
      historyManager.m_currentBigCard.ShowBigCard(historyManager.m_bigCardPath);
      historyManager.StartCoroutine("WaitThenDestroyBigCard");
      if (historyManager.m_currentBigCard.HasBigCardPostTransformedEntity())
      {
        while (historyManager.m_bigCardTransformState == HistoryManager.BigCardTransformState.PRE_TRANSFORM || historyManager.m_bigCardTransformState == HistoryManager.BigCardTransformState.TRANSFORM)
          yield return (object) null;
      }
    }
    else if (historyManager.m_currentBigCard.HasBigCardPostTransformedEntity())
    {
      historyManager.m_bigCardTransformState = HistoryManager.BigCardTransformState.PRE_TRANSFORM;
      historyManager.m_currentBigCard.ShowBigCard(historyManager.m_bigCardPath);
      historyManager.StartCoroutine("WaitThenDestroyBigCard");
      while (historyManager.m_bigCardTransformState == HistoryManager.BigCardTransformState.PRE_TRANSFORM || historyManager.m_bigCardTransformState == HistoryManager.BigCardTransformState.TRANSFORM)
        yield return (object) null;
    }
    else if (historyManager.m_currentBigCard.IsCastedByLettuceCharacter())
    {
      if (!historyManager.m_currentBigCard.IsBigCardFromMetaData())
      {
        historyManager.m_bigCardWaitingForLettuceSpeedTile = true;
        while (historyManager.m_bigCardWaitingForLettuceSpeedTile)
          yield return (object) null;
      }
      historyManager.m_currentBigCard.ShowBigCard(historyManager.m_lettuceAbilityBigCardPath);
      historyManager.StartCoroutine("WaitThenDestroyBigCard");
    }
    else
    {
      historyManager.m_currentBigCard.ShowBigCard(historyManager.m_bigCardPath);
      historyManager.StartCoroutine("WaitThenDestroyBigCard");
    }
    Entity entity = historyManager.m_currentBigCard.GetEntity();
    if (entity.HasSubCards() && !entity.IsLettuceAbility())
    {
      Network.HistBlockStart blockStart = GameState.Get().GetPowerProcessor().GetHistoryBlockingTaskList()?.GetBlockStart();
      if (blockStart.SubOption != -1)
      {
        Card card = entity.GetCard();
        ChoiceCardMgr.Get().ShowSubOptions(card);
        historyManager.StartCoroutine(historyManager.FinishSpectatorSubOption(entity, blockStart.SubOption));
      }
    }
    if (!GameState.Get().GetBooleanGameOption(GameEntityOption.DISABLE_DELAY_BETWEEN_BIG_CARD_DISPLAY_AND_POWER_PROCESSING))
      yield return (object) new WaitForSeconds(1f);
    historyManager.m_currentBigCard.RunBigCardFinishedCallback();
  }

  private IEnumerator FinishSpectatorSubOption(Entity mainEntity, int chosenSubOption)
  {
    while (ChoiceCardMgr.Get().IsWaitingToShowSubOptions())
    {
      yield return (object) null;
      if ((Object) ChoiceCardMgr.Get() == (Object) null || !ChoiceCardMgr.Get().HasSubOption())
        yield break;
    }
    List<Card> friendlyCards = ChoiceCardMgr.Get().GetFriendlyCards();
    List<Card> choiceCards;
    if (friendlyCards == null)
    {
      Log.All.PrintError("actualChoiceCards is NULL. Attempting workaround.");
      choiceCards = new List<Card>();
    }
    else
      choiceCards = new List<Card>((IEnumerable<Card>) friendlyCards);
    Card subCard = chosenSubOption < choiceCards.Count ? choiceCards[chosenSubOption] : (Card) null;
    Entity subEntity = (bool) (Object) subCard ? subCard.GetEntity() : (Entity) null;
    if ((Object) subCard != (Object) null)
      subCard.SetInputEnabled(false);
    yield return (object) new WaitForSeconds(1f);
    if ((Object) subCard != (Object) null)
      subCard.SetInputEnabled(true);
    GameState gameState = GameState.Get();
    if (gameState == null || gameState.IsGameOver())
    {
      foreach (Card card in choiceCards)
        card.HideCard();
    }
    else
      InputManager.Get().HandleClickOnSubOption(subEntity, true);
  }

  private IEnumerator WaitThenDestroyBigCard()
  {
    float seconds = (float) this.m_currentBigCard.GetDisplayTimeMS() / 1000f;
    if ((double) seconds <= 0.0)
    {
      if (this.m_currentBigCard.IsBigCardFromMetaData())
      {
        seconds = 1.5f;
      }
      else
      {
        if (this.m_currentBigCard.GetEntity() != null)
        {
          switch (this.m_currentBigCard.GetEntity().GetCardType())
          {
            case TAG_CARDTYPE.SPELL:
              seconds = 4f + GameState.Get().GetGameEntity().GetAdditionalTimeToWaitForSpells();
              break;
            case TAG_CARDTYPE.HERO_POWER:
              seconds = 4f + GameState.Get().GetGameEntity().GetAdditionalTimeToWaitForSpells();
              break;
            case TAG_CARDTYPE.LETTUCE_ABILITY:
              seconds = 1.65f;
              break;
            default:
              seconds = 3f;
              break;
          }
        }
        else
          seconds = 4f;
        if (this.m_currentBigCard.HasBigCardPostTransformedEntity())
          seconds *= 0.5f;
      }
    }
    yield return (object) new WaitForSeconds(seconds);
    this.DestroyBigCard();
  }

  private void DestroyBigCard()
  {
    if ((Object) this.m_currentBigCard == (Object) null)
      return;
    if ((Object) this.m_currentBigCard.m_mainCardActor == (Object) null)
      this.RunFinishedCallbackAndDestroyBigCard();
    else if (this.m_currentBigCard.HasBigCardPostTransformedEntity())
      this.PlayBigCardTransformEffects();
    else if (this.m_currentBigCard.WasBigCardCountered())
      this.PlayBigCardCounteredEffects();
    else
      this.RunFinishedCallbackAndDestroyBigCard();
  }

  private void RunFinishedCallbackAndDestroyBigCard()
  {
    if ((Object) this.m_currentBigCard == (Object) null)
      return;
    this.m_currentBigCard.RunBigCardFinishedCallback();
    this.m_showingBigCard = false;
    Object.Destroy((Object) this.m_currentBigCard.gameObject);
  }

  private void PlayBigCardCounteredEffects()
  {
    Spell.StateFinishedCallback callback = (Spell.StateFinishedCallback) ((s, prevStateType, userData) =>
    {
      if (s.GetActiveState() != SpellStateType.NONE)
        return;
      HistoryCard historyCard = (HistoryCard) userData;
      this.m_showingBigCard = false;
      Object.Destroy((Object) historyCard.gameObject);
    });
    Spell spell = this.m_currentBigCard.m_mainCardActor.GetSpell(SpellType.DEATH);
    if ((Object) spell == (Object) null)
    {
      this.RunFinishedCallbackAndDestroyBigCard();
    }
    else
    {
      spell.AddStateFinishedCallback(callback, (object) this.m_currentBigCard);
      this.m_currentBigCard.RunBigCardFinishedCallback();
      this.m_currentBigCard = (HistoryCard) null;
      spell.Activate();
    }
  }

  private void PlayBigCardTransformEffects() => this.StartCoroutine("PlayBigCardTransformEffectsWithTiming");

  private IEnumerator PlayBigCardTransformEffectsWithTiming()
  {
    HistoryManager historyManager = this;
    if (historyManager.m_bigCardTransformState == HistoryManager.BigCardTransformState.INVALID)
    {
      historyManager.RunFinishedCallbackAndDestroyBigCard();
    }
    else
    {
      if (historyManager.m_bigCardTransformState == HistoryManager.BigCardTransformState.PRE_TRANSFORM)
      {
        historyManager.m_bigCardTransformState = HistoryManager.BigCardTransformState.TRANSFORM;
        yield return (object) historyManager.StartCoroutine(historyManager.PlayBigCardTransformSpell());
      }
      if (historyManager.m_bigCardTransformState == HistoryManager.BigCardTransformState.TRANSFORM)
      {
        historyManager.m_bigCardTransformState = HistoryManager.BigCardTransformState.POST_TRANSFORM;
        yield return (object) historyManager.StartCoroutine(historyManager.WaitForBigCardPostTransform());
      }
      if (historyManager.m_bigCardTransformState == HistoryManager.BigCardTransformState.POST_TRANSFORM)
      {
        historyManager.m_bigCardTransformState = HistoryManager.BigCardTransformState.INVALID;
        historyManager.RunFinishedCallbackAndDestroyBigCard();
      }
    }
  }

  private IEnumerator PlayBigCardTransformSpell()
  {
    if (this.m_TransformSpells != null && this.m_TransformSpells.Length != 0)
    {
      Entity entity = this.m_currentBigCard.GetEntity();
      int index = entity.GetTag(GAME_TAG.TRANSFORMED_FROM_CARD_VISUAL_TYPE);
      if (index < 0 || index >= this.m_TransformSpells.Length)
        index = 0;
      this.m_bigCardTransformSpell = SpellManager.Get().GetSpell(this.m_TransformSpells[index]);
      if (!((Object) this.m_bigCardTransformSpell == (Object) null))
      {
        Card card = entity.GetCard();
        this.m_bigCardTransformSpell.SetSource(card.gameObject);
        this.m_bigCardTransformSpell.AddTarget(card.gameObject);
        this.m_bigCardTransformSpell.m_SetParentToLocation = true;
        this.m_bigCardTransformSpell.UpdateTransform();
        this.m_bigCardTransformSpell.SetPosition(this.m_currentBigCard.m_mainCardActor.transform.position);
        this.m_bigCardTransformSpell.AddStateFinishedCallback((Spell.StateFinishedCallback) ((s, prevStateType, userData) =>
        {
          if (s.GetActiveState() != SpellStateType.NONE)
            return;
          SpellManager.Get().ReleaseSpell(s);
        }));
        this.m_bigCardTransformSpell.Activate();
        while ((bool) (Object) this.m_bigCardTransformSpell && !this.m_bigCardTransformSpell.IsFinished())
          yield return (object) null;
      }
    }
  }

  private IEnumerator WaitForBigCardPostTransform()
  {
    Actor mainCardActor = this.m_currentBigCard.m_mainCardActor;
    mainCardActor.Hide(true);
    this.m_currentBigCard.LoadBigCardPostTransformedEntity();
    TransformUtil.CopyLocal((Component) this.m_currentBigCard.m_mainCardActor, (Component) mainCardActor);
    yield return (object) new WaitForSeconds(2f);
  }

  private void OnCurrentBigCardClicked()
  {
    if (this.m_currentBigCard.HasBigCardPostTransformedEntity())
      this.ForceNextBigCardTransformState();
    else
      this.InterruptCurrentBigCard();
  }

  private void ForceNextBigCardTransformState()
  {
    switch (this.m_bigCardTransformState)
    {
      case HistoryManager.BigCardTransformState.PRE_TRANSFORM:
        this.m_bigCardTransformState = HistoryManager.BigCardTransformState.TRANSFORM;
        this.StopWaitingThenDestroyBigCard();
        break;
      case HistoryManager.BigCardTransformState.TRANSFORM:
        if (!(bool) (Object) this.m_bigCardTransformSpell)
          break;
        Object.Destroy((Object) this.m_bigCardTransformSpell.gameObject);
        break;
      case HistoryManager.BigCardTransformState.POST_TRANSFORM:
        this.InterruptCurrentBigCard();
        break;
    }
  }

  private void StopWaitingThenDestroyBigCard()
  {
    this.StopCoroutine("WaitThenDestroyBigCard");
    this.DestroyBigCard();
  }

  private void InterruptCurrentBigCard()
  {
    this.StopCoroutine("WaitThenShowBigCard");
    if (this.m_currentBigCard.HasBigCardPostTransformedEntity())
      this.CutoffBigCardTransformEffects();
    else
      this.StopWaitingThenDestroyBigCard();
  }

  private void CutoffBigCardTransformEffects()
  {
    if ((bool) (Object) this.m_bigCardTransformSpell)
      Object.Destroy((Object) this.m_bigCardTransformSpell.gameObject);
    this.StopCoroutine("PlayBigCardTransformEffectsWithTiming");
    this.m_bigCardTransformState = HistoryManager.BigCardTransformState.INVALID;
    this.RunFinishedCallbackAndDestroyBigCard();
  }

  public delegate void BigCardStartedCallback();

  public delegate void BigCardFinishedCallback();

  private class BigCardEntry
  {
    public HistoryInfo m_info;
    public HistoryManager.BigCardStartedCallback m_startedCallback;
    public HistoryManager.BigCardFinishedCallback m_finishedCallback;
    public bool m_fromMetaData;
    public bool m_countered;
    public bool m_waitForSecretSpell;
    public int m_displayTimeMS;
  }

  private enum BigCardTransformState
  {
    INVALID,
    PRE_TRANSFORM,
    TRANSFORM,
    POST_TRANSFORM,
  }

  private class TileEntry
  {
    public HistoryInfo m_lastAttacker;
    public HistoryInfo m_lastDefender;
    public HistoryInfo m_lastCardPlayed;
    public HistoryInfo m_sourceOwner;
    public HistoryInfo m_lastCardTriggered;
    public HistoryInfo m_lastCardTargeted;
    public List<HistoryInfo> m_affectedCards = new List<HistoryInfo>();
    public HistoryInfo m_fatigueInfo;
    public HistoryInfo m_burnedCardsInfo;
    public bool m_usingMetaDataOverride;
    public bool m_complete;

    public void SetAttacker(Entity attacker)
    {
      this.m_lastAttacker = new HistoryInfo();
      this.m_lastAttacker.m_infoType = HistoryInfoType.ATTACK;
      this.m_lastAttacker.SetOriginalEntity(attacker);
    }

    public void SetDefender(Entity defender)
    {
      this.m_lastDefender = new HistoryInfo();
      this.m_lastDefender.SetOriginalEntity(defender);
    }

    public void SetCardPlayed(Entity entity)
    {
      this.m_lastCardPlayed = new HistoryInfo();
      this.m_lastCardPlayed.m_infoType = !entity.IsWeapon() ? HistoryInfoType.CARD_PLAYED : HistoryInfoType.WEAPON_PLAYED;
      this.m_lastCardPlayed.SetOriginalEntity(entity);
      Entity lettuceAbilityOwner = entity.GetLettuceAbilityOwner();
      if (lettuceAbilityOwner == null)
        return;
      this.SetSourceOwner(lettuceAbilityOwner);
    }

    public void SetSourceOwner(Entity entity)
    {
      if (entity == null)
        return;
      this.m_sourceOwner = new HistoryInfo();
      this.m_sourceOwner.SetOriginalEntity(entity);
    }

    public void SetCardTargeted(Entity entity)
    {
      if (entity == null)
        return;
      this.m_lastCardTargeted = new HistoryInfo();
      this.m_lastCardTargeted.SetOriginalEntity(entity);
    }

    public void SetCardTriggered(Entity entity)
    {
      if (entity.IsGame() || entity.IsPlayer())
        return;
      this.m_lastCardTriggered = new HistoryInfo();
      this.m_lastCardTriggered.m_infoType = HistoryInfoType.TRIGGER;
      this.m_lastCardTriggered.SetOriginalEntity(entity);
    }

    public void SetFatigue()
    {
      this.m_fatigueInfo = new HistoryInfo();
      this.m_fatigueInfo.m_infoType = HistoryInfoType.FATIGUE;
    }

    public void SetBurnedCards()
    {
      this.m_burnedCardsInfo = new HistoryInfo();
      this.m_burnedCardsInfo.m_infoType = HistoryInfoType.BURNED_CARDS;
    }

    public bool CanDuplicateAllEntities(bool duplicateHiddenNonSecrets, bool isEndOfHistory = false)
    {
      HistoryInfo sourceInfo = this.GetSourceInfo();
      if (this.ShouldDuplicateEntity(sourceInfo) && !sourceInfo.CanDuplicateEntity(duplicateHiddenNonSecrets, isEndOfHistory))
        return false;
      HistoryInfo targetInfo = this.GetTargetInfo();
      if (this.ShouldDuplicateEntity(targetInfo) && !targetInfo.CanDuplicateEntity(duplicateHiddenNonSecrets, isEndOfHistory))
        return false;
      for (int index = 0; index < this.m_affectedCards.Count; ++index)
      {
        HistoryInfo affectedCard = this.m_affectedCards[index];
        if (this.ShouldDuplicateEntity(affectedCard) && !affectedCard.CanDuplicateEntity(duplicateHiddenNonSecrets, isEndOfHistory))
          return false;
      }
      return true;
    }

    public void DuplicateAllEntities(bool duplicateHiddenNonSecrets, bool isEndOfHistory = false)
    {
      HistoryInfo sourceInfo = this.GetSourceInfo();
      if (this.ShouldDuplicateEntity(sourceInfo))
        sourceInfo.DuplicateEntity(duplicateHiddenNonSecrets, isEndOfHistory);
      HistoryInfo targetInfo = this.GetTargetInfo();
      if (this.ShouldDuplicateEntity(targetInfo))
        targetInfo.DuplicateEntity(duplicateHiddenNonSecrets, isEndOfHistory);
      for (int index = 0; index < this.m_affectedCards.Count; ++index)
      {
        HistoryInfo affectedCard = this.m_affectedCards[index];
        if (this.ShouldDuplicateEntity(affectedCard))
          affectedCard.DuplicateEntity(duplicateHiddenNonSecrets, isEndOfHistory);
      }
    }

    public bool ShouldDuplicateEntity(HistoryInfo info) => info != null && info != this.m_fatigueInfo && info != this.m_burnedCardsInfo;

    public HistoryInfo GetSourceInfo()
    {
      if (this.m_lastCardPlayed != null)
        return this.m_lastCardPlayed;
      if (this.m_lastAttacker != null)
        return this.m_lastAttacker;
      if (this.m_lastCardTriggered != null)
        return this.m_lastCardTriggered;
      if (this.m_fatigueInfo != null)
        return this.m_fatigueInfo;
      if (this.m_burnedCardsInfo != null)
        return this.m_burnedCardsInfo;
      Debug.LogError((object) "HistoryEntry.GetSourceInfo() - no source info");
      return (HistoryInfo) null;
    }

    public HistoryInfo GetTargetInfo()
    {
      if (this.m_lastCardPlayed != null && this.m_lastCardTargeted != null)
        return this.m_lastCardTargeted;
      return this.m_lastAttacker != null && this.m_lastDefender != null ? this.m_lastDefender : (HistoryInfo) null;
    }
  }

  private class TileEntryBuffer
  {
    private const int MAX_PREVIOUS_TILE_ENTRIES = 5;
    private int m_queuedEntriesBufferIndex;
    private HistoryManager.TileEntry[] m_queuedEntriesBuffer = new HistoryManager.TileEntry[5];

    public int Length => this.m_queuedEntriesBuffer.Length;

    public void Clear()
    {
      for (int index = 0; index < 5; ++index)
        this.m_queuedEntriesBuffer[index] = (HistoryManager.TileEntry) null;
    }

    public void AddHistoryEntry(ref HistoryManager.TileEntry newEntry)
    {
      this.m_queuedEntriesBuffer[this.m_queuedEntriesBufferIndex] = newEntry;
      ++this.m_queuedEntriesBufferIndex;
      this.m_queuedEntriesBufferIndex %= 5;
    }

    public HistoryManager.TileEntry GetHistoryEntry(int index)
    {
      int index1 = (this.m_queuedEntriesBufferIndex - 1 - index) % 5;
      if (index1 < 0)
        index1 += 5;
      return this.m_queuedEntriesBuffer[index1];
    }
  }

  private class TileLoadedCallbackData
  {
    public HistoryInfo m_sourceInfo { get; set; }

    public List<HistoryInfo> m_childInfos { get; } = new List<HistoryInfo>();

    public HistoryInfo m_sourceOwnerInfo { get; set; }
  }
}
