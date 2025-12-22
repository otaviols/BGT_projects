using Blizzard.GameService.SDK.Client.Integration;
using PegasusClient;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
  private const string DEFAULT_AI_OPPONENT_NAME = "GAMEPLAY_AI_OPPONENT_NAME";
  private const string POPUP_CONTROLLER_PREFAB = "PopUpProgressBar.prefab:1e74ef51d3388674792ddf7d6233f5d7";
  private BnetGameAccountId m_gameAccountId;
  private bool m_waitingForHeroEntity;
  private string m_name;
  private bool m_local;
  private Player.Side m_side;
  private int m_cardBackId;
  private int m_initialCardBackId;
  private ManaCounter m_manaCounter;
  private Entity m_hero;
  private Entity m_heroPower;
  private int m_queuedSpentMana;
  private int m_usedTempMana;
  private int m_realtimeTempMana;
  private bool m_realTimeComboActive;
  private bool m_realTimeSpellsCostHealth;
  private MedalInfoTranslator m_medalInfo;
  private uint m_arenaWins;
  private uint m_arenaLoss;
  private uint m_tavernBrawlWins;
  private uint m_tavernBrawlLoss;
  private uint m_duelsWins;
  private uint m_duelsLoss;
  private bool m_concedeEmotePlayed;
  private TAG_PLAYSTATE m_preGameOverPlayState;
  private HashSet<EntityDef> m_seenStartOfGameSpells = new HashSet<EntityDef>();
  private MarkOfEvilCounter m_markOfEvilCounter;

  public static Player.Side GetOppositePlayerSide(Player.Side side)
  {
    if (side == Player.Side.FRIENDLY)
      return Player.Side.OPPOSING;
    return side == Player.Side.OPPOSING ? Player.Side.FRIENDLY : side;
  }

  public void OnShuffleDeck()
  {
    ZoneDeck deckZone = this.GetDeckZone();
    if ((UnityEngine.Object) deckZone == (UnityEngine.Object) null)
      return;
    deckZone.UpdateLayout();
    Actor activeThickness = deckZone.GetActiveThickness();
    if ((UnityEngine.Object) activeThickness == (UnityEngine.Object) null)
      return;
    activeThickness.ActivateSpellBirthState(SpellType.SHUFFLE_DECK);
  }

  public void InitPlayer(Network.HistCreateGame.PlayerData netPlayer)
  {
    this.SetPlayerId(netPlayer.ID);
    this.SetGameAccountId(netPlayer.GameAccountId);
    this.SetCardBackId(netPlayer.CardBackID);
    this.SetTags(netPlayer.Player.Tags);
    this.InitRealTimeValues(netPlayer.Player.Tags);
    if (this.IsLocalUser())
    {
      foreach (Network.Entity.Tag tag in netPlayer.Player.Tags)
      {
        if (tag.Name == 1048)
          GameMgr.Get().LastGameData.WhizbangDeckID = tag.Value;
      }
    }
    if (this.HasTag(GAME_TAG.CARD_BACK_OVERRIDE))
      this.SetOverrideCardBackId(this.GetTag(GAME_TAG.CARD_BACK_OVERRIDE));
    Network.Entity.Tag tag1 = netPlayer.Player.Tags.Find((Predicate<Network.Entity.Tag>) (tag => tag.Name == 994));
    if (tag1 != null)
      this.GetOrCreateMarkOfEvilCounter().OnMarksChanged(tag1.Value);
    GameState.Get().RegisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
  }

  public override bool HasValidDisplayName() => !string.IsNullOrEmpty(this.m_name);

  public override string GetName() => this.m_name;

  public MedalInfoTranslator GetRank() => this.m_medalInfo;

  public override string GetDebugName()
  {
    if (this.m_name != null)
      return this.m_name;
    return this.IsAI() ? GameStrings.Get("GAMEPLAY_AI_OPPONENT_NAME") : "UNKNOWN HUMAN PLAYER";
  }

  public void SetGameAccountId(BnetGameAccountId id)
  {
    this.m_gameAccountId = id;
    this.UpdateLocal();
    if (this.IsDisplayable())
    {
      this.UpdateDisplayInfo();
    }
    else
    {
      this.UpdateRank();
      this.UpdateSessionRecord();
      if (!this.IsBnetPlayer())
        return;
      BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnBnetPlayersChanged));
      if (BnetFriendMgr.Get().IsFriend(this.m_gameAccountId))
        return;
      GameUtils.RequestPlayerPresence(this.m_gameAccountId);
    }
  }

  public bool IsLocalUser() => this.m_local;

  public bool IsAI() => GameUtils.IsAIPlayer(this.m_gameAccountId);

  public bool IsHuman() => GameUtils.IsHumanPlayer(this.m_gameAccountId);

  public bool IsBnetPlayer() => GameUtils.IsBnetPlayer(this.m_gameAccountId);

  public Player.Side GetSide() => this.m_side;

  public bool IsFriendlySide() => this.m_side == Player.Side.FRIENDLY;

  public bool IsOpposingSide() => this.m_side == Player.Side.OPPOSING;

  public bool IsSpellpowerTemporary(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    int tag1 = this.GetTag(GAME_TAG.CURRENT_SPELLPOWER);
    int tag2 = this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER);
    switch (spellSchool)
    {
      case TAG_SPELL_SCHOOL.ARCANE:
        tag1 += this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_ARCANE);
        tag2 += this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_ARCANE);
        break;
      case TAG_SPELL_SCHOOL.FIRE:
        tag1 += this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_FIRE);
        tag2 += this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_FIRE);
        break;
      case TAG_SPELL_SCHOOL.FROST:
        tag1 += this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_FROST);
        tag2 += this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_FROST);
        break;
      case TAG_SPELL_SCHOOL.NATURE:
        tag1 += this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_NATURE);
        tag2 += this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_NATURE);
        break;
      case TAG_SPELL_SCHOOL.HOLY:
        tag1 += this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_HOLY);
        tag2 += this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_HOLY);
        break;
      case TAG_SPELL_SCHOOL.SHADOW:
        tag1 += this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_SHADOW);
        tag2 += this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_SHADOW);
        break;
      case TAG_SPELL_SCHOOL.FEL:
        tag1 += this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_FEL);
        tag2 += this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_FEL);
        break;
      case TAG_SPELL_SCHOOL.PHYSICAL_COMBAT:
        tag1 += this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_PHYSICAL);
        tag2 += this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_PHYSICAL);
        break;
    }
    Debug.Log((object) ("Current spell power: " + (object) tag1));
    Debug.Log((object) ("Current temp spell power: " + (object) tag2));
    return tag1 == 0 && tag2 > 0;
  }

  public int TotalSpellpower(Entity ent, TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    int num1 = 1;
    if (ent.HasTag(GAME_TAG.SECRET) || ent.HasTag(GAME_TAG.SIGIL))
      num1 = 0;
    int num2 = 0;
    switch (spellSchool)
    {
      case TAG_SPELL_SCHOOL.ARCANE:
        num2 = num2 + this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_ARCANE) + this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_ARCANE) * num1;
        break;
      case TAG_SPELL_SCHOOL.FIRE:
        num2 = num2 + this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_FIRE) + this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_FIRE) * num1;
        break;
      case TAG_SPELL_SCHOOL.FROST:
        num2 = num2 + this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_FROST) + this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_FROST) * num1;
        break;
      case TAG_SPELL_SCHOOL.NATURE:
        num2 = num2 + this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_NATURE) + this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_NATURE) * num1;
        break;
      case TAG_SPELL_SCHOOL.HOLY:
        num2 = num2 + this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_HOLY) + this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_HOLY) * num1;
        break;
      case TAG_SPELL_SCHOOL.SHADOW:
        num2 = num2 + this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_SHADOW) + this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_SHADOW) * num1;
        break;
      case TAG_SPELL_SCHOOL.FEL:
        num2 = num2 + this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_FEL) + this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_FEL) * num1;
        break;
      case TAG_SPELL_SCHOOL.PHYSICAL_COMBAT:
        num2 = num2 + this.GetTag(GAME_TAG.CURRENT_SPELLPOWER_PHYSICAL) + this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER_PHYSICAL) * num1;
        break;
    }
    int num3 = this.GetTag(GAME_TAG.CURRENT_TEMP_SPELLPOWER) * num1;
    int tag = ent.GetTag(GAME_TAG.CURRENT_SPELLPOWER);
    return this.GetTag(GAME_TAG.CURRENT_SPELLPOWER) - this.GetTag(GAME_TAG.CURRENT_NEGATIVE_SPELLPOWER) + num2 + num3 + tag;
  }

  public new bool IsRevealed() => this.IsFriendlySide() || SpectatorManager.Get().IsSpectatingPlayer(this.m_gameAccountId) || this.HasTag(GAME_TAG.ZONES_REVEALED);

  public int GetCardBackId() => this.m_cardBackId;

  public int GetOriginalCardBackId() => this.m_initialCardBackId;

  public void SetCardBackId(int id)
  {
    this.m_cardBackId = id;
    this.m_initialCardBackId = id;
  }

  public void SetOverrideCardBackId(int id)
  {
    if (id > 0)
      this.m_cardBackId = id;
    else
      this.m_cardBackId = this.m_initialCardBackId;
  }

  public int GetPlayerId() => this.GetTag(GAME_TAG.PLAYER_ID);

  public void SetPlayerId(int playerId) => this.SetTag(GAME_TAG.PLAYER_ID, playerId);

  public int GetTeamId() => this.GetTag(GAME_TAG.TEAM_ID);

  public bool IsTeamLeader() => this.GetPlayerId() == this.GetTeamId();

  public bool IsCurrentPlayer()
  {
    bool isCurrentPlayer;
    return GameState.Get().GetGameEntity().OverwriteCurrentPlayer(this, out isCurrentPlayer) ? isCurrentPlayer : this.HasTag(GAME_TAG.CURRENT_PLAYER);
  }

  public bool IsComboActive() => this.HasTag(GAME_TAG.COMBO_ACTIVE);

  public bool IsRealTimeComboActive() => this.m_realTimeComboActive;

  public void SetRealTimeComboActive(int tagValue) => this.SetRealTimeComboActive(tagValue == 1);

  public void SetRealTimeComboActive(bool active) => this.m_realTimeComboActive = active;

  public void SetRealTimeSpellsCostHealth(int value) => this.m_realTimeSpellsCostHealth = value > 0;

  public bool GetRealTimeSpellsCostHealth() => this.m_realTimeSpellsCostHealth;

  public override void InitRealTimeValues(List<Network.Entity.Tag> tags)
  {
    base.InitRealTimeValues(tags);
    foreach (Network.Entity.Tag tag in tags)
    {
      switch ((GAME_TAG) tag.Name)
      {
        case GAME_TAG.COMBO_ACTIVE:
          this.SetRealTimeComboActive(tag.Value);
          continue;
        case GAME_TAG.TEMP_RESOURCES:
          this.SetRealTimeTempMana(tag.Value);
          continue;
        case GAME_TAG.SPELLS_COST_HEALTH:
          this.SetRealTimeSpellsCostHealth(tag.Value);
          continue;
        default:
          continue;
      }
    }
  }

  public int GetNumAvailableResources()
  {
    int tag1 = this.GetTag(GAME_TAG.TEMP_RESOURCES);
    int tag2 = this.GetTag(GAME_TAG.RESOURCES);
    int tag3 = this.GetTag(GAME_TAG.RESOURCES_USED);
    int num1 = tag1;
    int num2 = tag2 + num1 - tag3 - this.m_queuedSpentMana - this.m_usedTempMana;
    return num2 >= 0 ? num2 : 0;
  }

  public int GetNumAvailableCorpses()
  {
    Entity hero = this.GetHero();
    if (hero == null || !hero.HasClass(TAG_CLASS.DEATHKNIGHT))
      return 0;
    int num = this.GetTag(GAME_TAG.CORPSES) - this.GetTag(GAME_TAG.CORPSES_SPENT_THIS_GAME);
    return num >= 0 ? num : 0;
  }

  public bool HasWeapon()
  {
    foreach (Zone zone in ZoneMgr.Get().GetZones())
    {
      if (zone is ZoneWeapon && zone.m_Side == this.m_side)
        return zone.GetCards().Count > 0;
    }
    return false;
  }

  public void SetHero(Entity hero)
  {
    this.m_hero = hero;
    if (this.ShouldUseHeroName())
      this.UpdateDisplayInfo();
    foreach (Card card in this.GetHandZone().GetCards())
    {
      if (card.GetEntity().IsMultiClass())
        card.UpdateActorComponents();
    }
    if (this.IsFriendlySide())
      GameState.Get().FireHeroChangedEvent(this);
    CorpseCounter.UpdateTextAll();
  }

  public Entity GetStartingHero()
  {
    Entity startingHero = this.GetHero();
    if (startingHero == null)
      return startingHero;
    Entity entity;
    for (; startingHero.HasTag(GAME_TAG.LINKED_ENTITY); startingHero = entity)
    {
      int tag = startingHero.GetTag(GAME_TAG.LINKED_ENTITY);
      entity = GameState.Get().GetEntity(tag);
      if (entity == null || !entity.IsHero())
      {
        Log.Gameplay.PrintError("Player.GetStartingHero() - Hero entity {0} has a LINKED_ENTITY tag value of {1} which corresponds to invalid Entity {2}.", (object) startingHero, (object) tag, (object) entity);
        break;
      }
    }
    return startingHero;
  }

  public override Entity GetHero() => this.m_hero;

  public EntityDef GetHeroEntityDef() => this.m_hero == null ? (EntityDef) null : this.m_hero.GetEntityDef() ?? (EntityDef) null;

  public override Card GetHeroCard() => this.m_hero == null ? (Card) null : this.m_hero.GetCard();

  public void SetHeroPower(Entity heroPower) => this.m_heroPower = heroPower;

  public override Entity GetHeroPower() => this.m_heroPower;

  public override Card GetHeroPowerCard() => this.m_heroPower == null ? (Card) null : this.m_heroPower.GetCard();

  public bool IsHeroPowerAffectedByBonusDamage()
  {
    Card heroPowerCard = this.GetHeroPowerCard();
    if ((UnityEngine.Object) heroPowerCard == (UnityEngine.Object) null)
      return false;
    Entity entity = heroPowerCard.GetEntity();
    return entity.IsHeroPower() && entity.GetCardTextBuilder().ContainsBonusDamageToken(entity);
  }

  public override Card GetWeaponCard() => ZoneMgr.Get().FindZoneOfType<ZoneWeapon>(this.GetSide()).GetFirstCard();

  public override Card GetHeroBuddyCard() => ZoneMgr.Get().FindZoneOfType<ZoneBattlegroundHeroBuddy>(this.GetSide()).GetFirstCard();

  public override Card GetQuestRewardFromHeroPowerCard()
  {
    foreach (ZoneBattlegroundQuestReward battlegroundQuestReward in ZoneMgr.Get().FindZonesOfType<ZoneBattlegroundQuestReward>(this.GetSide()))
    {
      if (battlegroundQuestReward.m_isHeroPower)
        return battlegroundQuestReward.GetFirstCard();
    }
    return (Card) null;
  }

  public override Card GetQuestRewardCard()
  {
    foreach (ZoneBattlegroundQuestReward battlegroundQuestReward in ZoneMgr.Get().FindZonesOfType<ZoneBattlegroundQuestReward>(this.GetSide()))
    {
      if (!battlegroundQuestReward.m_isHeroPower)
        return battlegroundQuestReward.GetFirstCard();
    }
    return (Card) null;
  }

  public override List<Card> GetQuestRewardCards()
  {
    List<Card> questRewardCards = new List<Card>();
    foreach (ZoneBattlegroundQuestReward battlegroundQuestReward in ZoneMgr.Get().FindZonesOfType<ZoneBattlegroundQuestReward>(this.GetSide()))
    {
      if ((UnityEngine.Object) battlegroundQuestReward.GetFirstCard() != (UnityEngine.Object) null)
        questRewardCards.Add(battlegroundQuestReward.GetFirstCard());
    }
    return questRewardCards;
  }

  public ZoneHand GetHandZone() => ZoneMgr.Get().FindZoneOfType<ZoneHand>(this.GetSide());

  public ZonePlay GetBattlefieldZone() => ZoneMgr.Get().FindZoneOfType<ZonePlay>(this.GetSide());

  public ZoneDeck GetDeckZone() => ZoneMgr.Get().FindZoneOfType<ZoneDeck>(this.GetSide());

  public ZoneGraveyard GetGraveyardZone() => ZoneMgr.Get().FindZoneOfType<ZoneGraveyard>(this.GetSide());

  public ZoneSecret GetSecretZone() => ZoneMgr.Get().FindZoneOfType<ZoneSecret>(this.GetSide());

  public ZoneHero GetHeroZone() => ZoneMgr.Get().FindZoneOfType<ZoneHero>(this.GetSide());

  public ZoneLettuceAbility GetLettuceAbilityZone() => ZoneMgr.Get().FindZoneOfType<ZoneLettuceAbility>(this.GetSide());

  public bool HasReadyAttackers()
  {
    List<Card> cards = this.GetBattlefieldZone().GetCards();
    for (int index = 0; index < cards.Count; ++index)
    {
      if (GameState.Get().HasResponse(cards[index].GetEntity()))
        return true;
    }
    return false;
  }

  public bool HasATauntMinion()
  {
    List<Card> cards = this.GetBattlefieldZone().GetCards();
    for (int index = 0; index < cards.Count; ++index)
    {
      if (cards[index].GetEntity().HasTaunt())
        return true;
    }
    return false;
  }

  public uint GetArenaWins() => this.m_arenaWins;

  public uint GetArenaLosses() => this.m_arenaLoss;

  public uint GetTavernBrawlWins() => this.m_tavernBrawlWins;

  public uint GetTavernBrawlLosses() => this.m_tavernBrawlLoss;

  public uint GetDuelsWins() => this.m_duelsWins;

  public uint GetDuelsLosses() => this.m_duelsLoss;

  public void PlayConcedeEmote()
  {
    if (this.m_concedeEmotePlayed)
      return;
    Card heroCard = this.GetHeroCard();
    if ((UnityEngine.Object) heroCard == (UnityEngine.Object) null)
      return;
    heroCard.PlayEmote(EmoteType.CONCEDE);
    this.m_concedeEmotePlayed = true;
  }

  public BnetGameAccountId GetGameAccountId() => this.m_gameAccountId;

  public BnetPlayer GetBnetPlayer() => BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);

  public bool IsDisplayable()
  {
    if ((BnetEntityId) this.m_gameAccountId == (BnetEntityId) null)
      return false;
    if (!this.IsBnetPlayer())
      return !this.ShouldUseHeroName() || this.GetHeroEntityDef() != null;
    BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
    if (player == null || !player.IsDisplayable())
      return false;
    if (GameUtils.IsGameTypeRanked())
    {
      BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
      if (hearthstoneGameAccount == (BnetGameAccount) null || !hearthstoneGameAccount.HasGameField(18U))
        return false;
    }
    return true;
  }

  public void WipeZzzs()
  {
    foreach (Card card in this.GetBattlefieldZone().GetCards())
    {
      Spell actorSpell = card.GetActorSpell(SpellType.Zzz);
      if (!((UnityEngine.Object) actorSpell == (UnityEngine.Object) null))
        actorSpell.ActivateState(SpellStateType.DEATH);
    }
  }

  public TAG_PLAYSTATE GetPreGameOverPlayState() => this.m_preGameOverPlayState;

  public bool HasSeenStartOfGameSpell(EntityDef entityDef) => this.m_seenStartOfGameSpells.Contains(entityDef);

  public void MarkStartOfGameSpellAsSeen(EntityDef entityDef) => this.m_seenStartOfGameSpells.Add(entityDef);

  public bool IsEarlyConcedePopupAvailable() => this.HasTag(GAME_TAG.EARLY_CONCEDE_POPUP_AVAILABLE);

  public void AddManaCrystal(int numCrystals, bool isTurnStart)
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().AddManaCrystals(numCrystals, isTurnStart);
  }

  public void AddManaCrystal(int numCrystals) => this.AddManaCrystal(numCrystals, false);

  public void DestroyManaCrystal(int numCrystals)
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().DestroyManaCrystals(numCrystals);
  }

  public void AddTempManaCrystal(int numCrystals)
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().AddTempManaCrystals(numCrystals);
  }

  public void DestroyTempManaCrystal(int numCrystals)
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().DestroyTempManaCrystals(numCrystals);
  }

  public void ReadyManaCrystal(int numCrystals)
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().ReadyManaCrystals(numCrystals);
  }

  public void HandleSameTurnOverloadChanged(int crystalsChanged)
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().HandleSameTurnOverloadChanged(crystalsChanged);
  }

  public void UnlockCrystals(int numCrystals)
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().UnlockCrystals(numCrystals);
  }

  public void CancelAllProposedMana(Entity entity)
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().CancelAllProposedMana(entity);
  }

  public void ProposeManaCrystalUsage(Entity entity)
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().ProposeManaCrystalUsage(entity);
  }

  public void ResetUnresolvedManaToBeReadied()
  {
    if (!this.IsFriendlySide())
      return;
    ManaCrystalMgr.Get().ResetUnresolvedManaToBeReadied();
  }

  public void UpdateManaCounter()
  {
    if ((UnityEngine.Object) this.m_manaCounter == (UnityEngine.Object) null)
      return;
    this.m_manaCounter.UpdateText();
  }

  public void NotifyOfSpentMana(int spentMana) => this.m_queuedSpentMana += spentMana;

  public void NotifyOfUsedTempMana(int usedMana) => this.m_usedTempMana += usedMana;

  public void SetRealTimeTempMana(int tempMana) => this.m_realtimeTempMana = tempMana;

  public int GetRealTimeTempMana() => this.m_realtimeTempMana;

  public void OnBoardLoaded() => this.AssignPlayerBoardObjects();

  public override void OnRealTimeTagChanged(Network.HistTagChange change)
  {
    switch ((GAME_TAG) change.Tag)
    {
      case GAME_TAG.PLAYSTATE:
        TAG_PLAYSTATE playState = (TAG_PLAYSTATE) change.Value;
        if (!GameUtils.IsPreGameOverPlayState(playState))
          break;
        this.m_preGameOverPlayState = playState;
        break;
      case GAME_TAG.COMBO_ACTIVE:
        this.SetRealTimeComboActive(change.Value);
        break;
      case GAME_TAG.TEMP_RESOURCES:
        this.SetRealTimeTempMana(change.Value);
        break;
      case GAME_TAG.SPELLS_COST_HEALTH:
        this.SetRealTimeSpellsCostHealth(change.Value);
        break;
      case GAME_TAG.BACON_NUMBER_HERO_REFRESH_AVAILABLE:
        if (!this.IsFriendlySide() || !((UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null))
          break;
        MulliganManager.Get().OnFriendlyPlayerNumberRefreshAvailableChanged(change.Value);
        break;
    }
  }

  public override void OnTagsChanged(TagDeltaList changeList, bool fromShowEntity)
  {
    for (int index = 0; index < changeList.Count; ++index)
      this.OnTagChanged(changeList[index]);
  }

  public override void OnTagChanged(TagDelta change)
  {
    if (this.IsFriendlySide())
      this.OnFriendlyPlayerTagChanged(change);
    else
      this.OnOpposingPlayerTagChanged(change);
    switch ((GAME_TAG) change.tag)
    {
      case GAME_TAG.PLAYSTATE:
        if (change.newValue != 8)
          break;
        this.PlayConcedeEmote();
        break;
      case GAME_TAG.CURRENT_PLAYER:
        if (change.newValue != 1 || !GameState.Get().IsLocalSidePlayerTurn())
          break;
        ManaCrystalMgr.Get().OnCurrentPlayerChanged();
        this.m_queuedSpentMana = 0;
        if (!GameState.Get().IsMainPhase())
          break;
        TurnStartManager.Get().BeginListeningForTurnEvents();
        break;
      case GAME_TAG.RESOURCES_USED:
      case GAME_TAG.RESOURCES:
      case GAME_TAG.TEMP_RESOURCES:
        if (!GameState.Get().IsTurnStartManagerActive() || !this.IsFriendlySide())
          this.UpdateManaCounter();
        GameState.Get().UpdateOptionHighlights();
        break;
      case GAME_TAG.COMBO_ACTIVE:
        foreach (Card card in this.GetHandZone().GetCards())
          card.UpdateActorState();
        this.GetHeroPower()?.GetCard().UpdateActorState();
        break;
      case GAME_TAG.MULLIGAN_STATE:
        if (change.newValue != 4 || !((UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null))
          break;
        MulliganManager.Get().ServerHasDealtReplacementCards(this.IsFriendlySide());
        break;
      case GAME_TAG.STEADY_SHOT_CAN_TARGET:
        this.ToggleActorSpellOnCard(this.GetHeroPowerCard(), change, SpellType.STEADY_SHOT_CAN_TARGET);
        break;
      case GAME_TAG.CURRENT_HEROPOWER_DAMAGE_BONUS:
        if (!this.IsHeroPowerAffectedByBonusDamage())
          break;
        this.ToggleActorSpellOnCard(this.GetHeroPowerCard(), change, SpellType.CURRENT_HEROPOWER_DAMAGE_BONUS);
        break;
      case GAME_TAG.LOCK_AND_LOAD:
        this.ToggleActorSpellOnCard(this.GetHeroCard(), change, SpellType.LOCK_AND_LOAD);
        break;
      case GAME_TAG.CHOOSE_BOTH:
        this.UpdateChooseBoth();
        break;
      case GAME_TAG.SPELLS_COST_HEALTH:
        this.UpdateSpellsCostHealth(change);
        break;
      case GAME_TAG.IGNORE_TAUNT:
        using (List<Card>.Enumerator enumerator = GameState.Get().GetFirstOpponentPlayer(this.GetController()).GetBattlefieldZone().GetCards().GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Card current = enumerator.Current;
            if (current.CanShowActorVisuals())
            {
              Entity entity = current.GetEntity();
              if (entity != null && entity.HasTaunt())
              {
                Actor actor = current.GetActor();
                if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
                  actor.ActivateTaunt();
              }
            }
          }
          break;
        }
      case GAME_TAG.EMBRACE_THE_SHADOW:
        this.ToggleActorSpellOnCard(this.GetHeroCard(), change, SpellType.EMBRACE_THE_SHADOW);
        break;
      case GAME_TAG.DEATH_KNIGHT:
        this.ToggleActorSpellOnCard(this.GetHeroCard(), change, SpellType.DEATH_KNIGHT);
        break;
      case GAME_TAG.STAMPEDE:
        this.ToggleActorSpellOnCard(this.GetHeroCard(), change, SpellType.STAMPEDE);
        break;
      case GAME_TAG.IS_VAMPIRE:
        this.ToggleActorSpellOnCard(this.GetHeroCard(), change, SpellType.IS_VAMPIRE);
        break;
      case GAME_TAG.OVERRIDE_EMOTE_0:
      case GAME_TAG.OVERRIDE_EMOTE_1:
      case GAME_TAG.OVERRIDE_EMOTE_2:
      case GAME_TAG.OVERRIDE_EMOTE_3:
      case GAME_TAG.OVERRIDE_EMOTE_4:
      case GAME_TAG.OVERRIDE_EMOTE_5:
        if (!((UnityEngine.Object) EmoteHandler.Get() != (UnityEngine.Object) null))
          break;
        EmoteHandler.Get().ChangeAvailableEmotes();
        break;
      case GAME_TAG.HERO_POWER_DISABLED:
        Card heroPowerCard = this.GetHeroPowerCard();
        if (!((UnityEngine.Object) heroPowerCard != (UnityEngine.Object) null) || heroPowerCard.GetEntity() == null || heroPowerCard.GetEntity().GetTag(GAME_TAG.EXHAUSTED) != 0)
          break;
        heroPowerCard.HandleCardExhaustedTagChanged(change);
        break;
      case GAME_TAG.MARK_OF_EVIL:
        this.GetOrCreateMarkOfEvilCounter().OnMarksChanged(change.newValue);
        break;
      case GAME_TAG.GLORIOUSGLOOP:
        this.ToggleActorSpellOnCard(this.GetHeroCard(), change, SpellType.GLORIOUSGLOOP);
        break;
      case GAME_TAG.WHIZBANG_DECK_ID:
        if (!this.IsLocalUser())
          break;
        GameMgr.Get().LastGameData.WhizbangDeckID = change.newValue;
        break;
      case GAME_TAG.DECK_POWER_UP:
        Spell spell = this.ToggleActorSpellOnCard(this.GetHeroCard(), change, SpellType.DECK_POWER_UP);
        if (!((UnityEngine.Object) spell != (UnityEngine.Object) null) || !((UnityEngine.Object) this.GetHeroCard() != (UnityEngine.Object) null) || !((UnityEngine.Object) this.GetHeroCard().gameObject != (UnityEngine.Object) null))
          break;
        spell.SetSource(this.GetHeroCard().gameObject);
        spell.ForceUpdateTransform();
        break;
      case GAME_TAG.SPELLS_CAST_TWICE:
        this.ToggleActorSpellOnCard(this.GetHeroCard(), change, SpellType.SPELLS_CAST_TWICE);
        break;
      case GAME_TAG.PROGRESSBAR_SHOW:
        if (change.newValue != 1)
          break;
        AssetLoader.Get().InstantiatePrefab((AssetReference) "PopUpProgressBar.prefab:1e74ef51d3388674792ddf7d6233f5d7").GetComponent<PopUpController>().Populate(this.GetTag(GAME_TAG.PROGRESSBAR_PROGRESS), this.GetTag(GAME_TAG.PROGRESSBAR_TOTAL), this.GetTag(GAME_TAG.PROGRESSBAR_CARDID), CollectionManager.Get().GetHeroPremium(this.m_hero.GetClass()));
        break;
      case GAME_TAG.CARD_BACK_OVERRIDE:
        this.SetOverrideCardBackId(change.newValue);
        CardBackManager.Get().SetGameCardBackIDs(GameState.Get().GetFriendlySidePlayer().GetCardBackId(), GameState.Get().GetOpposingSidePlayer().GetCardBackId());
        break;
    }
  }

  public MarkOfEvilCounter GetOrCreateMarkOfEvilCounter()
  {
    if ((UnityEngine.Object) this.m_markOfEvilCounter == (UnityEngine.Object) null)
    {
      GameObject destination = AssetLoader.Get().InstantiatePrefab((AssetReference) "MarkOfEvilCounter.prefab:ff08f2e19826b354bb37bb25bf81471d", AssetLoadingOptions.IgnorePrefabPosition);
      this.m_markOfEvilCounter = destination.GetComponent<MarkOfEvilCounter>();
      string name = this.GetSide() == Player.Side.FRIENDLY ? "MarkOfEvil" : "MarkOfEvil_Opponent";
      Transform bone = Board.Get().FindBone(name);
      TransformUtil.CopyWorld(destination, (Component) bone);
    }
    return this.m_markOfEvilCounter;
  }

  private void OnFriendlyPlayerTagChanged(TagDelta change)
  {
    switch ((GAME_TAG) change.tag)
    {
      case GAME_TAG.RESOURCES_USED:
        int num1 = change.oldValue + this.m_queuedSpentMana;
        int num2 = change.newValue - change.oldValue;
        if (num2 > 0)
          this.m_queuedSpentMana -= num2;
        if (this.m_queuedSpentMana < 0)
          this.m_queuedSpentMana = 0;
        int shownChangeAmount = change.newValue - num1 + this.m_queuedSpentMana;
        ManaCrystalMgr.Get().UpdateSpentMana(shownChangeAmount);
        break;
      case GAME_TAG.RESOURCES:
        if (change.newValue > change.oldValue)
        {
          if (GameState.Get().IsTurnStartManagerActive() && this.IsFriendlySide())
          {
            TurnStartManager.Get().NotifyOfManaCrystalGained(change.newValue - change.oldValue);
            break;
          }
          this.AddManaCrystal(change.newValue - change.oldValue);
          break;
        }
        this.DestroyManaCrystal(change.oldValue - change.newValue);
        break;
      case GAME_TAG.NUM_TURNS_LEFT:
        TurnStartManager turnStartManager = TurnStartManager.Get();
        if (!((UnityEngine.Object) turnStartManager != (UnityEngine.Object) null))
          break;
        Spell extraTurnSpell = turnStartManager.GetExtraTurnSpell();
        if (change.oldValue >= 2 && change.newValue == 1)
          turnStartManager.NotifyOfExtraTurn(extraTurnSpell, true);
        if (change.newValue < 2 || change.newValue <= change.oldValue)
          break;
        turnStartManager.NotifyOfExtraTurn(extraTurnSpell);
        break;
      case GAME_TAG.CURRENT_SPELLPOWER:
      case GAME_TAG.SPELLPOWER_DOUBLE:
      case GAME_TAG.SPELL_HEALING_DOUBLE:
      case GAME_TAG.CURRENT_NEGATIVE_SPELLPOWER:
      case GAME_TAG.CURRENT_SPELLPOWER_ARCANE:
      case GAME_TAG.CURRENT_SPELLPOWER_FIRE:
      case GAME_TAG.CURRENT_SPELLPOWER_FROST:
      case GAME_TAG.CURRENT_SPELLPOWER_NATURE:
      case GAME_TAG.CURRENT_SPELLPOWER_HOLY:
      case GAME_TAG.CURRENT_SPELLPOWER_SHADOW:
      case GAME_TAG.CURRENT_SPELLPOWER_FEL:
      case GAME_TAG.CURRENT_SPELLPOWER_PHYSICAL:
      case GAME_TAG.CURRENT_TEMP_SPELLPOWER_ARCANE:
      case GAME_TAG.CURRENT_TEMP_SPELLPOWER_FEL:
      case GAME_TAG.CURRENT_TEMP_SPELLPOWER_FIRE:
      case GAME_TAG.CURRENT_TEMP_SPELLPOWER_FROST:
      case GAME_TAG.CURRENT_TEMP_SPELLPOWER_NATURE:
      case GAME_TAG.CURRENT_TEMP_SPELLPOWER_HOLY:
      case GAME_TAG.CURRENT_TEMP_SPELLPOWER_PHYSICAL:
      case GAME_TAG.CURRENT_TEMP_SPELLPOWER_SHADOW:
      case GAME_TAG.CURRENT_TEMP_SPELLPOWER:
        this.UpdateHandCardPowersText(false);
        break;
      case GAME_TAG.TEMP_RESOURCES:
        int num3 = change.oldValue - this.m_usedTempMana;
        int num4 = change.newValue - change.oldValue;
        if (num4 < 0)
          this.m_usedTempMana += num4;
        if (this.m_usedTempMana < 0)
          this.m_usedTempMana = 0;
        if (num3 < 0)
          num3 = 0;
        int numCrystals = change.newValue - num3 - this.m_usedTempMana;
        if (numCrystals > 0)
        {
          this.AddTempManaCrystal(numCrystals);
          break;
        }
        this.DestroyTempManaCrystal(-numCrystals);
        break;
      case GAME_TAG.OVERLOAD_OWED:
        this.HandleSameTurnOverloadChanged(change.newValue - change.oldValue);
        break;
      case GAME_TAG.MULLIGAN_STATE:
        if (change.newValue == 4)
        {
          if (!((UnityEngine.Object) MulliganManager.Get() == (UnityEngine.Object) null))
            break;
          using (List<Card>.Enumerator enumerator = this.GetHandZone().GetCards().GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.GetActor().TurnOnCollider();
            break;
          }
        }
        else
        {
          if (change.newValue != 1 || change.oldValue != 5 || !((UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null))
            break;
          MulliganManager.Get().ServerHasDealtReplacementCards(this.IsFriendlySide());
          break;
        }
      case GAME_TAG.OVERLOAD_LOCKED:
        if (change.newValue >= change.oldValue || GameState.Get().IsTurnStartManagerActive())
          break;
        this.UnlockCrystals(change.oldValue - change.newValue);
        break;
      case GAME_TAG.JADE_GOLEM:
      case GAME_TAG.AMOUNT_HEALED_THIS_GAME:
      case GAME_TAG.NUM_HERO_POWER_DAMAGE_THIS_GAME:
      case GAME_TAG.ALL_HEALING_DOUBLE:
      case GAME_TAG.ARMOR_GAINED_THIS_GAME:
        this.UpdateHandCardPowersText(false);
        break;
      case GAME_TAG.RED_MANA_CRYSTALS:
        ManaCrystalMgr.Get().TurnCrystalsRed(change.oldValue, change.newValue);
        break;
      case GAME_TAG.CORPSES:
      case GAME_TAG.CORPSES_SPENT_THIS_GAME:
        CorpseCounter.UpdateTextAll();
        break;
    }
  }

  private void OnOpposingPlayerTagChanged(TagDelta change)
  {
    switch ((GAME_TAG) change.tag)
    {
      case GAME_TAG.PLAYSTATE:
        if (change.newValue != 7)
          break;
        NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_ANNOUNCER_DISCONNECT_45"), "VO_ANNOUNCER_DISCONNECT_45.prefab:911a83eb9ad91fc41acf1aca808c5e5a");
        break;
      case GAME_TAG.RESOURCES:
        if (change.newValue <= change.oldValue)
          break;
        GameState.Get().GetGameEntity().NotifyOfEnemyManaCrystalSpawned();
        break;
      case GAME_TAG.NUM_TURNS_LEFT:
        TurnStartManager turnStartManager = TurnStartManager.Get();
        if (!((UnityEngine.Object) turnStartManager != (UnityEngine.Object) null))
          break;
        Spell extraTurnSpell = turnStartManager.GetExtraTurnSpell(false);
        if (change.oldValue >= 2 && change.newValue == 1)
          TurnStartManager.Get().NotifyOfExtraTurn(extraTurnSpell, true, false);
        if (change.newValue < 2 || change.newValue <= change.oldValue)
          break;
        TurnStartManager.Get().NotifyOfExtraTurn(extraTurnSpell, isFriendly: false);
        break;
      case GAME_TAG.CORPSES:
      case GAME_TAG.CORPSES_SPENT_THIS_GAME:
        CorpseCounter.UpdateTextAll();
        break;
    }
  }

  private void UpdateName()
  {
    GameEntity gameEntity = GameState.Get()?.GetGameEntity();
    if (gameEntity != null && gameEntity.ShouldUseAlternateNameForPlayer(this.GetSide()))
      this.m_name = gameEntity.GetNameBannerOverride(this.GetSide());
    else if (this.ShouldUseHeroName())
      this.UpdateNameWithHeroName();
    else if (this.IsAI())
    {
      if (GameUtils.IsMatchmadeGameType(GameMgr.Get().GetGameType()))
        this.m_name = this.GetRandomName();
      else
        this.m_name = GameStrings.Get("GAMEPLAY_AI_OPPONENT_NAME");
    }
    else if (this.IsBnetPlayer())
    {
      BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
      if (player != null)
        this.m_name = player.GetBestName();
      if (string.IsNullOrEmpty(this.m_name))
        return;
      GameMgr.Get().SetLastDisplayedPlayerName(this.GetPlayerId(), this.m_name);
    }
    else
      Debug.LogError((object) string.Format("Player.UpdateName() - unable to determine player name"));
  }

  private bool ShouldUseHeroName() => !this.IsBnetPlayer() && (!this.IsAI() || !GameMgr.Get().IsPractice() && !GameUtils.IsMatchmadeGameType(GameMgr.Get().GetGameType()));

  private string GetRandomName()
  {
    string[] strArray = ExternalUrlService.Get().GetRandomNamesText().Split(',');
    if (strArray.Length == 0)
      return GameStrings.Get("GAMEPLAY_AI_OPPONENT_NAME");
    System.Random random = new System.Random(GameMgr.Get().GetGameHandle());
    return strArray[random.Next(0, strArray.Length - 1)];
  }

  private void UpdateNameWithHeroName()
  {
    if (this.m_hero == null)
      return;
    EntityDef entityDef = this.m_hero.GetEntityDef();
    if (entityDef == null)
      return;
    this.m_name = entityDef.GetName();
  }

  private bool ShouldUseBogusRank() => !this.IsBnetPlayer();

  private void UpdateRank()
  {
    MedalInfoTranslator medalInfoTranslator = (MedalInfoTranslator) null;
    if (this.ShouldUseBogusRank())
      medalInfoTranslator = new MedalInfoTranslator();
    else if ((BnetEntityId) this.m_gameAccountId == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())
      medalInfoTranslator = RankMgr.Get().GetLocalPlayerMedalInfo();
    if (medalInfoTranslator == null)
    {
      BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
      if (player != null)
        medalInfoTranslator = RankMgr.Get().GetRankedMedalFromRankPresenceField(player);
    }
    this.m_medalInfo = medalInfoTranslator;
  }

  public void UpdateDisplayInfo()
  {
    this.UpdateName();
    this.UpdateRank();
    this.UpdateSessionRecord();
    if (!this.IsBnetPlayer() || this.IsLocalUser())
      return;
    BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
    if (player == null || !BnetFriendMgr.Get().IsFriend(player))
      return;
    ChatMgr.Get().AddRecentWhisperPlayerToBottom(player);
  }

  private void UpdateSessionRecord()
  {
    BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
    if (player == null)
      return;
    BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
    if (hearthstoneGameAccount == (BnetGameAccount) null)
      return;
    SessionRecord sessionRecord = hearthstoneGameAccount.GetSessionRecord();
    if (sessionRecord == null)
      return;
    if (sessionRecord.SessionRecordType == SessionRecordType.ARENA)
    {
      this.m_arenaWins = sessionRecord.Wins;
      this.m_arenaLoss = sessionRecord.Losses;
    }
    else if (sessionRecord.SessionRecordType == SessionRecordType.TAVERN_BRAWL)
    {
      this.m_tavernBrawlWins = sessionRecord.Wins;
      this.m_tavernBrawlLoss = sessionRecord.Losses;
    }
    else
    {
      if (sessionRecord.SessionRecordType != SessionRecordType.DUELS)
        return;
      this.m_duelsWins = sessionRecord.Wins;
      this.m_duelsLoss = sessionRecord.Losses;
    }
  }

  private void OnBnetPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    if (changelist.FindChange(this.m_gameAccountId) == null || !this.IsDisplayable())
      return;
    BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnBnetPlayersChanged));
    this.UpdateDisplayInfo();
  }

  private void UpdateLocal()
  {
    if (GameMgr.Get() != null && SpectatorManager.Get().IsSpectatingOrWatching)
      this.m_local = false;
    else if (this.IsBnetPlayer())
      this.m_local = (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId() == (BnetEntityId) this.m_gameAccountId;
    else
      this.m_local = this.m_gameAccountId.Low == 1UL;
  }

  public void UpdateSide(int friendlySideTeamId)
  {
    if (this.GetTeamId() == friendlySideTeamId)
    {
      this.m_side = Player.Side.FRIENDLY;
      GameState.Get().RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnFriendlyOptionsReceived));
      GameState.Get().RegisterOptionsSentListener(new GameState.OptionsSentCallback(this.OnFriendlyOptionsSent));
      GameState.Get().RegisterFriendlyTurnStartedListener(new GameState.FriendlyTurnStartedCallback(this.OnFriendlyTurnStarted));
    }
    else
      this.m_side = Player.Side.OPPOSING;
  }

  private void AssignPlayerBoardObjects()
  {
    if (!this.IsTeamLeader())
      return;
    foreach (ManaCounter componentsInChild in Gameplay.Get().GetBoardLayout().gameObject.GetComponentsInChildren<ManaCounter>(true))
    {
      if (componentsInChild.m_Side == this.m_side)
      {
        this.m_manaCounter = componentsInChild;
        this.m_manaCounter.SetPlayer(this);
        this.m_manaCounter.UpdateText();
        break;
      }
    }
    this.InitManaCrystalMgr();
    this.InitCorpseCounter();
    foreach (Zone zone in ZoneMgr.Get().GetZones())
    {
      if (zone.m_Side == this.m_side)
      {
        if (this.IsFriendlySide() && zone.m_ServerTag == TAG_ZONE.HAND)
          zone.SetController(GameState.Get().GetLocalSidePlayer());
        else
          zone.SetController(this);
      }
    }
  }

  private void InitManaCrystalMgr()
  {
    if (!this.IsFriendlySide())
      return;
    int tag1 = this.GetTag(GAME_TAG.TEMP_RESOURCES);
    int tag2 = this.GetTag(GAME_TAG.RESOURCES);
    int tag3 = this.GetTag(GAME_TAG.RESOURCES_USED);
    int tag4 = this.GetTag(GAME_TAG.OVERLOAD_OWED);
    int tag5 = this.GetTag(GAME_TAG.OVERLOAD_LOCKED);
    ManaCrystalMgr.Get().AddManaCrystals(tag2, false);
    ManaCrystalMgr.Get().AddTempManaCrystals(tag1);
    ManaCrystalMgr.Get().UpdateSpentMana(tag3);
    ManaCrystalMgr.Get().MarkCrystalsOwedForOverload(tag4);
    ManaCrystalMgr.Get().SetCrystalsLockedForOverload(tag5);
    ManaCrystalMgr.Get().ResetUnresolvedManaToBeReadied();
  }

  private void InitCorpseCounter()
  {
    if (!this.IsFriendlySide())
      return;
    CorpseCounter.InitializeAll();
  }

  private void OnTurnChanged(int oldTurn, int newTurn, object userData)
  {
    this.WipeZzzs();
    this.UpdateChooseBoth();
  }

  private void OnFriendlyOptionsReceived(object userData) => this.UpdateChooseBoth();

  private void OnFriendlyOptionsSent(Network.Options.Option option, object userData)
  {
    this.UpdateChooseBoth();
    this.CancelAllProposedMana(GameState.Get().GetEntity(option.Main.ID));
  }

  private void OnFriendlyTurnStarted(object userData) => this.UpdateChooseBoth();

  private Spell ToggleActorSpellOnCard(Card card, TagDelta change, SpellType spellType)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return (Spell) null;
    if (!card.CanShowActorVisuals())
      return (Spell) null;
    Actor actor = card.GetActor();
    if (change.newValue > 0)
      return actor.ActivateSpellBirthState(spellType);
    actor.ActivateSpellDeathState(spellType);
    return (Spell) null;
  }

  private void UpdateHandCardPowersText(bool onlySpells)
  {
    List<Card> cards = this.GetHandZone().GetCards();
    for (int index = 0; index < cards.Count; ++index)
    {
      Card card = cards[index];
      if (!((UnityEngine.Object) card.GetActor() == (UnityEngine.Object) null) && (!onlySpells || card.GetEntity().IsSpell()))
        card.GetActor().UpdatePowersText();
    }
  }

  private void UpdateSpellsCostHealth(TagDelta change)
  {
    if (change.oldValue == change.newValue)
      return;
    if (this.IsFriendlySide())
    {
      Card mousedOverCard = InputManager.Get().GetMousedOverCard();
      if ((UnityEngine.Object) mousedOverCard != (UnityEngine.Object) null)
      {
        Entity entity = mousedOverCard.GetEntity();
        if (entity.IsSpell())
        {
          if (change.newValue > 0)
            ManaCrystalMgr.Get().CancelAllProposedMana(entity);
          else
            ManaCrystalMgr.Get().ProposeManaCrystalUsage(entity);
        }
      }
    }
    List<Card> cards = this.GetHandZone().GetCards();
    for (int index = 0; index < cards.Count; ++index)
    {
      Card card = cards[index];
      if (card.CanShowActorVisuals())
      {
        Entity entity = card.GetEntity();
        if (entity.IsSpell() && !entity.HasTag(GAME_TAG.CARD_COSTS_HEALTH))
        {
          Actor actor = card.GetActor();
          if (change.newValue > 0)
            actor.ActivateSpellBirthState(SpellType.SPELLS_COST_HEALTH);
          else
            actor.ActivateSpellDeathState(SpellType.SPELLS_COST_HEALTH);
        }
      }
    }
  }

  private void UpdateChooseBoth()
  {
    List<Card> cards = this.GetHandZone().GetCards();
    for (int index = 0; index < cards.Count; ++index)
      this.UpdateChooseBoth(cards[index]);
    this.UpdateChooseBoth(this.GetHeroPowerCard());
  }

  private void UpdateChooseBoth(Card card)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null || !card.CanShowActorVisuals())
      return;
    Entity entity = card.GetEntity();
    if (!entity.HasTag(GAME_TAG.CHOOSE_ONE))
      return;
    Actor actor = card.GetActor();
    SpellType spellType = SpellType.CHOOSE_BOTH;
    if ((entity.HasTag(GAME_TAG.CHOOSE_BOTH) || this.HasTag(GAME_TAG.CHOOSE_BOTH)) && GameState.Get().IsValidOption(entity))
      SpellUtils.ActivateBirthIfNecessary(actor.GetSpell(spellType));
    else
      SpellUtils.ActivateDeathIfNecessary(actor.GetSpellIfLoaded(spellType));
  }

  public enum Side
  {
    NEUTRAL,
    FRIENDLY,
    OPPOSING,
  }
}
