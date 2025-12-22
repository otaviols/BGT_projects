using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Hearthstone;
using HutongGames.PlayMaker;
using PegasusGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card : MonoBehaviour
{
  public static readonly Vector3 ABOVE_DECK_OFFSET = new Vector3(-0.3f, 3.6f, 0.0f);
  public static readonly Vector3 IN_DECK_OFFSET = new Vector3(0.0f, 0.0f, 0.1f);
  public static readonly Vector3 IN_DECK_SCALE = new Vector3(0.81f, 0.81f, 0.81f);
  public static readonly Vector3 IN_DECK_ANGLES = new Vector3(-90f, 270f, 0.0f);
  public static readonly Quaternion IN_DECK_ROTATION = Quaternion.Euler(Card.IN_DECK_ANGLES);
  public static readonly Vector3 IN_DECK_HIDDEN_ANGLES = new Vector3(275f, 90f, 0.0f);
  public static readonly Quaternion IN_DECK_HIDDEN_ROTATION = Quaternion.Euler(Card.IN_DECK_HIDDEN_ANGLES);
  public const float DEFAULT_KEYWORD_DEATH_DELAY_SEC = 0.6f;
  protected Entity m_entity;
  protected DefLoader.DisposableCardDef m_cardDef;
  protected CardEffect m_playEffect;
  protected List<CardEffect> m_additionalPlayEffects;
  protected CardEffect m_attackEffect;
  protected CardEffect m_deathEffect;
  protected CardEffect m_lifetimeEffect;
  protected List<CardEffect> m_subOptionEffects;
  protected List<List<CardEffect>> m_additionalSubOptionEffects;
  protected List<CardEffect> m_triggerEffects;
  protected List<CardEffect> m_resetGameEffects;
  private const string BGQuestDescComponent = "Card_Hand_BG_Quest_Text_Tray_Mesh";
  private const string DefaultDescComponent = "Description_mesh";
  private const string NonBGQuestComponent = "NonQuestObjects";
  public const string BGRewardVFX = "BGRewardVFX";
  protected Map<Network.HistBlockStart, CardEffect> m_proxyEffects;
  protected List<CardEffect> m_allEffects;
  protected CardEffect m_customKeywordEffect;
  protected CardEffect m_customChoiceRevealEffect;
  protected CardEffect m_customChoiceConcealEffect;
  protected Map<SpellType, CardEffect> m_spellTableOverrideEffects = new Map<SpellType, CardEffect>();
  protected CardSound[] m_announcerLine = new CardSound[3];
  protected List<EmoteEntry> m_emotes;
  protected Spell m_customSummonSpell;
  protected Spell m_customSpawnSpell;
  protected Spell m_customSpawnSpellOverride;
  protected Spell m_customDeathSpell;
  protected Spell m_customDeathSpellOverride;
  protected Spell m_customDiscardSpell;
  protected Spell m_customDiscardSpellOverride;
  private int m_spellLoadCount;
  protected string m_actorPath;
  protected Actor m_actor;
  protected Actor m_actorWaitingToBeReplaced;
  private bool m_actorReady = true;
  private bool m_actorLoading;
  private bool m_transitioningZones;
  private bool m_hasBeenGrabbedByEnemyActionHandler;
  private Zone m_zone;
  private Zone m_prevZone;
  private bool m_goingThroughDeathrattleReturnfromGraveyard;
  private int m_zonePosition;
  private int m_predictedZonePosition;
  public ZonePositionChange m_minionWasMovedFromSrcToDst;
  private bool m_doNotSort;
  private bool m_beingDrawnByOpponent;
  private bool m_cardStandInInteractive = true;
  private ZoneTransitionStyle m_transitionStyle;
  private bool m_doNotWarpToNewZone;
  private float m_transitionDelay;
  protected bool m_shouldShowTooltip;
  protected bool m_showTooltip;
  protected bool m_overPlayfield;
  protected MoveMinionHoverTarget m_overMoveMinionTarget;
  protected bool m_mousedOver;
  protected bool m_mousedOverByOpponent;
  protected bool m_shown = true;
  private bool m_inputEnabled = true;
  protected bool m_attacking;
  protected bool m_moving;
  private int m_activeDeathEffectCount;
  private bool m_ignoreDeath;
  private bool m_suppressDeathEffects;
  private bool m_suppressDeathSounds;
  private bool m_suppressKeywordDeaths;
  private bool m_suppressHandToDeckTransition;
  private float m_keywordDeathDelaySec = 0.6f;
  private bool m_suppressActorTriggerSpell;
  private int m_suppressPlaySoundCount;
  private bool m_isBattleCrySource;
  private bool m_secretTriggered;
  private bool m_secretSheathed;
  private bool m_isBaubleAnimating;
  private Spell m_activeSpawnSpell;
  private Player.Side? m_playZoneBlockerSide;
  private float m_delayBeforeHideInNullZoneVisuals;
  private DisplayCardsInToolip m_cardsInTooltip;
  private MagneticPlayData m_magneticPlayData;
  private bool m_magneticTarget;
  private ZoneChange m_latestZoneChange;
  private bool m_skipMilling;
  private int m_cardDrawTracker;
  private float? m_drawTimeScale;
  private const int DRAW_FAST_THRESHOLD_START = 3;
  private const int DRAW_FAST_THRESHOLD_MAX = 6;
  private const float NORMAL_DRAW_TIME_SCALE = 1f;
  private const float FAST_DRAW_TIME_SCALE = 0.556f;
  private bool m_disableHeroPowerFlipSoundOnce;
  private int m_lettuceAbilityActionOrder = -1;
  private bool m_lettuceAbilityActionOrderIsTied;
  private Actor m_questRewardActor;
  private bool m_questRewardChanged;

  public event Card.EmotePlayCallback OnEmotePlayCallback;

  public bool IsBeingDragged { get; set; }

  public override string ToString() => this.m_entity != null ? this.m_entity.ToString() : "UNKNOWN CARD";

  public Entity GetEntity() => this.m_entity;

  public void SetEntity(Entity entity) => this.m_entity = entity;

  public void Destroy()
  {
    if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
      this.m_actor.Destroy();
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  public Player GetController() => this.m_entity == null ? (Player) null : this.m_entity.GetController();

  public Player.Side GetControllerSide() => this.m_entity == null ? Player.Side.NEUTRAL : this.m_entity.GetControllerSide();

  public Entity GetHero() => this.GetController()?.GetHero();

  public Card GetHeroCard() => this.GetHero()?.GetCard();

  public Entity GetHeroPower() => this.GetController()?.GetHeroPower();

  public Card GetHeroPowerCard() => this.GetHeroPower()?.GetCard();

  public TAG_PREMIUM GetPremium() => this.m_entity == null ? TAG_PREMIUM.NORMAL : this.m_entity.GetPremiumType();

  public bool IsOverPlayfield() => this.m_overPlayfield;

  public void NotifyOverPlayfield()
  {
    this.m_overPlayfield = true;
    this.UpdateActorState();
  }

  public void NotifyLeftPlayfield()
  {
    this.m_overPlayfield = false;
    this.UpdateActorState();
  }

  public bool IsOverMoveMinionTarget() => (UnityEngine.Object) this.m_overMoveMinionTarget != (UnityEngine.Object) null;

  public void NotifyOverMoveMinionTarget(MoveMinionHoverTarget target)
  {
    this.m_overMoveMinionTarget = target;
    this.UpdateActorState();
  }

  public void NotifyLeftMoveMinionTarget()
  {
    this.m_overMoveMinionTarget = (MoveMinionHoverTarget) null;
    this.UpdateActorState();
  }

  public void NotifyOfWeaponPlayed(Entity source) => this.ActivateLegendaryHeroAnimEvent("OnWeaponCardPlayed");

  public void NotifyOfWeaponDestroyed(Entity source) => this.ActivateLegendaryHeroAnimEvent("OnWeaponCardDestroyed");

  public void NotifyOfWeaponSheathed(Entity source) => this.ActivateLegendaryHeroAnimEvent("OnWeaponSheathed");

  public void NotifyOfWeaponUnsheathed(Entity source) => this.ActivateLegendaryHeroAnimEvent("OnWeaponUnsheathed");

  public void NotifyOfSpellPlayed(Entity source, Entity target) => this.ActivateLegendaryHeroAnimEvent("OnSpellCard");

  public void NotifyOfHeroPowerPlayed(Entity source, Entity target) => this.ActivateLegendaryHeroAnimEvent("OnHeroPower");

  public void OnDestroy()
  {
    this.ReleaseAssets();
    if (!this.m_mousedOver || GameState.Get() == null || (UnityEngine.Object) InputManager.Get() == (UnityEngine.Object) null)
      return;
    InputManager.Get().NotifyCardDestroyed(this);
  }

  public void NotifyMousedOver()
  {
    this.m_mousedOver = true;
    this.UpdateActorState();
    this.UpdateProposedManaUsage();
    if ((bool) (UnityEngine.Object) RemoteActionHandler.Get() && (bool) (UnityEngine.Object) TargetReticleManager.Get())
      RemoteActionHandler.Get().NotifyOpponentOfMouseOverEntity(this.GetEntity().GetCard());
    if (GameState.Get() != null)
      GameState.Get().GetGameEntity().NotifyOfCardMousedOver(this.GetEntity());
    if (this.m_zone is ZoneHand)
    {
      Spell actorSpell1 = this.GetActorSpell(SpellType.SPELL_POWER_HINT_BURST);
      if ((UnityEngine.Object) actorSpell1 != (UnityEngine.Object) null)
        actorSpell1.Deactivate();
      Spell actorSpell2 = this.GetActorSpell(SpellType.SPELL_POWER_HINT_IDLE);
      if ((UnityEngine.Object) actorSpell2 != (UnityEngine.Object) null)
        actorSpell2.Deactivate();
      Spell actorSpell3 = this.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_BURST);
      if ((UnityEngine.Object) actorSpell3 != (UnityEngine.Object) null)
        actorSpell3.Deactivate();
      Spell actorSpell4 = this.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_IDLE);
      if ((UnityEngine.Object) actorSpell4 != (UnityEngine.Object) null)
        actorSpell4.Deactivate();
      this.GetActorSpell(SpellType.LIFESTEAL_DOES_DAMAGE_HINT_IDLE);
      if ((UnityEngine.Object) actorSpell4 != (UnityEngine.Object) null)
        actorSpell4.Deactivate();
      if (GameState.Get() != null && GameState.Get().IsMulliganManagerActive())
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_mouse_over.prefab:0d4e20bc78956bc48b5e2963ec39211c", this.gameObject);
      if (this.ShouldShowCardsInTooltip())
        this.m_cardsInTooltip.NotifyMousedOver();
    }
    if (this.m_entity.IsControlledByFriendlySidePlayer() && (this.m_entity.IsHero() || this.m_zone is ZonePlay) && !this.m_transitioningZones)
    {
      bool flag1 = this.m_entity.HasSpellPower() || this.m_entity.HasSpellPowerDouble();
      bool flag2 = this.m_entity.HasHeroPowerDamage();
      if (flag1 | flag2)
      {
        Spell actorSpell = this.GetActorSpell(SpellType.SPELL_POWER_HINT_BURST);
        if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
          actorSpell.Reactivate();
        if (flag1)
          ZoneMgr.Get().OnSpellPowerEntityMousedOver(this.m_entity.GetSpellPowerSchool());
      }
      if (this.m_entity.HasHealingDoesDamageHint())
      {
        Spell actorSpell = this.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_BURST);
        if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
          actorSpell.Reactivate();
        ZoneMgr.Get().OnHealingDoesDamageEntityMousedOver();
      }
      if (this.m_entity.HasLifestealDoesDamageHint())
      {
        Spell actorSpell = this.GetActorSpell(SpellType.HEALING_DOES_DAMAGE_HINT_BURST);
        if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
          actorSpell.Reactivate();
        ZoneMgr.Get().OnLifestealDoesDamageEntityMousedOver();
      }
    }
    if (this.m_entity.IsControlledByFriendlySidePlayer() && this.m_entity.HasTag(GAME_TAG.BACON_DIED_LAST_COMBAT_HINT))
      ZoneMgr.Get().OnDiedLastCombatMousedOver();
    if (this.m_entity.IsWeapon() && this.m_entity.IsExhausted() && (UnityEngine.Object) this.m_actor != (UnityEngine.Object) null && (UnityEngine.Object) this.m_actor.GetAttackObject() != (UnityEngine.Object) null)
      this.m_actor.GetAttackObject().Enlarge(1f);
    if (this.m_entity.IsQuest() && this.m_zone is ZoneSecret)
    {
      QuestController component = this.m_actor.GetComponent<QuestController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.NotifyMousedOver();
    }
    if (this.m_entity.IsQuestline() && this.m_zone is ZoneSecret)
    {
      QuestlineController component = this.m_actor.GetComponent<QuestlineController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.NotifyMousedOver();
    }
    if (this.m_entity.IsPuzzle() && this.m_zone is ZoneSecret)
    {
      PuzzleController component = this.m_actor.GetComponent<PuzzleController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.NotifyMousedOver();
    }
    if (this.m_entity.IsRulebook() && this.m_zone is ZoneSecret)
    {
      RulebookController component = this.m_actor.GetComponent<RulebookController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.NotifyMousedOver();
    }
    if (!this.m_entity.IsLettuceAbility())
      return;
    Entity abilitiesSourceEntity = ZoneMgr.Get().GetLettuceAbilitiesSourceEntity();
    if (abilitiesSourceEntity == null)
      return;
    if (GameState.Get().IsValidOption(this.m_entity))
    {
      int tag = this.m_entity.GetTag(GAME_TAG.LETTUCE_ABILITY_SUMMONED_MINION);
      if (tag > 0)
      {
        Spell spell = this.m_actor.GetSpell(SpellType.LETTUCE_ABILITY_SUMMON_PREVIEW);
        if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
        {
          int num = abilitiesSourceEntity.GetZonePosition() + 1;
          PlayMakerFSM component = spell.gameObject.GetComponent<PlayMakerFSM>();
          component.FsmVariables.GetFsmInt("FakeMinionZonePosition").Value = num;
          component.FsmVariables.GetFsmInt("FakeMinionDbId").Value = tag;
          component.FsmVariables.GetFsmInt("FakeMinionAttack").Value = this.m_entity.GetATK();
          component.FsmVariables.GetFsmInt("FakeMinionHealth").Value = this.m_entity.GetHealth();
          FsmVector3 fsmVector3 = component.FsmVariables.GetFsmVector3("FakeMinionScale");
          Card card = abilitiesSourceEntity.GetCard();
          Vector3 vector3 = card != null ? card.transform.localScale : Vector3.one;
          fsmVector3.Value = vector3;
          spell.ActivateState(SpellStateType.BIRTH);
        }
      }
    }
    if (!(this.m_actor is LettuceAbilityActor actor))
      return;
    actor.PlayMousedOverSound();
  }

  public void NotifyMousedOut()
  {
    this.m_mousedOver = false;
    this.UpdateActorState();
    this.UpdateProposedManaUsage();
    if ((bool) (UnityEngine.Object) RemoteActionHandler.Get())
      RemoteActionHandler.Get().NotifyOpponentOfMouseOut();
    if ((bool) (UnityEngine.Object) TooltipPanelManager.Get())
      TooltipPanelManager.Get().HideKeywordHelp();
    if ((bool) (UnityEngine.Object) CardTypeBanner.Get())
      CardTypeBanner.Get().Hide(this);
    if (GameState.Get() != null)
      GameState.Get().GetGameEntity().NotifyOfCardMousedOff(this.GetEntity());
    if (this.m_entity.IsControlledByFriendlySidePlayer() && (this.m_entity.IsHero() || this.m_zone is ZonePlay))
    {
      if (this.m_entity.HasSpellPower())
        ZoneMgr.Get().OnSpellPowerEntityMousedOut(this.m_entity.GetSpellPowerSchool());
      if (this.m_entity.HasHealingDoesDamageHint())
        ZoneMgr.Get().OnHealingDoesDamageEntityMousedOut();
      if (this.m_entity.HasLifestealDoesDamageHint())
        ZoneMgr.Get().OnLifestealDoesDamageEntityMousedOut();
    }
    if (this.m_entity.IsControlledByFriendlySidePlayer() && this.m_entity.HasTag(GAME_TAG.BACON_DIED_LAST_COMBAT_HINT))
      ZoneMgr.Get().OnDiedLastCombatMousedOut();
    if (this.m_entity.IsWeapon() && this.m_entity.IsExhausted() && (UnityEngine.Object) this.m_actor != (UnityEngine.Object) null && (UnityEngine.Object) this.m_actor.GetAttackObject() != (UnityEngine.Object) null)
      this.m_actor.GetAttackObject().ScaleToZero();
    if (this.m_entity.IsQuest() && (this.m_zone is ZoneSecret || this.m_prevZone is ZoneSecret))
    {
      QuestController component = this.m_actor.GetComponent<QuestController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.NotifyMousedOut();
    }
    if (this.m_entity.IsQuestline() && this.m_zone is ZoneSecret)
    {
      QuestlineController component = this.m_actor.GetComponent<QuestlineController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.NotifyMousedOut();
    }
    if (this.m_entity.IsPuzzle() && this.m_zone is ZoneSecret && (UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
    {
      PuzzleController component = this.m_actor.GetComponent<PuzzleController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.NotifyMousedOut();
    }
    if (this.m_entity.IsRulebook() && this.m_zone is ZoneSecret && (UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
    {
      RulebookController component = this.m_actor.GetComponent<RulebookController>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.NotifyMousedOut();
    }
    if (this.m_entity.IsLettuceAbility() && this.m_entity.HasTag(GAME_TAG.LETTUCE_ABILITY_SUMMONED_MINION))
      this.m_actor.ActivateSpellDeathState(SpellType.LETTUCE_ABILITY_SUMMON_PREVIEW);
    if (!((UnityEngine.Object) this.m_cardsInTooltip != (UnityEngine.Object) null))
      return;
    this.m_cardsInTooltip.NotifyMousedOut();
  }

  public void ShowWeaknessSplat()
  {
    Spell spell = this.m_actor?.GetSpell(SpellType.WEAKNESS_SPLAT);
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null) || spell.GetActiveState() == SpellStateType.BIRTH)
      return;
    spell.ActivateState(SpellStateType.BIRTH);
  }

  public void HideWeaknessSplat()
  {
    Spell spellIfLoaded = this.m_actor?.GetSpellIfLoaded(SpellType.WEAKNESS_SPLAT);
    if (!((UnityEngine.Object) spellIfLoaded != (UnityEngine.Object) null) || spellIfLoaded.GetActiveState() != SpellStateType.BIRTH)
      return;
    spellIfLoaded.ActivateState(SpellStateType.DEATH);
  }

  public bool IsMousedOver() => this.m_mousedOver;

  public void NotifyOpponentMousedOverThisCard()
  {
    this.m_mousedOverByOpponent = true;
    this.UpdateActorState();
  }

  public void NotifyOpponentMousedOffThisCard()
  {
    this.m_mousedOverByOpponent = false;
    this.UpdateActorState();
  }

  public void NotifyPickedUp()
  {
    this.m_transitioningZones = false;
    if (this.GetZone() is ZoneHand)
      this.CutoffFriendlyCardDraw();
    if (!this.ShouldShowCardsInTooltip())
      return;
    this.m_cardsInTooltip.NotifyPickedUp();
  }

  public void NotifyTargetingCanceled()
  {
    if (this.m_entity.IsCharacter() && !this.IsAttacking())
    {
      Spell attackSpellForInput = this.GetActorAttackSpellForInput();
      if ((UnityEngine.Object) attackSpellForInput != (UnityEngine.Object) null)
      {
        if (!this.ShouldShowImmuneVisuals())
          this.GetActor().ActivateSpellDeathState(SpellType.IMMUNE);
        switch (attackSpellForInput.GetActiveState())
        {
          case SpellStateType.NONE:
          case SpellStateType.CANCEL:
            break;
          default:
            attackSpellForInput.ActivateState(SpellStateType.CANCEL);
            break;
        }
      }
    }
    this.ActivateHandStateSpells();
  }

  public bool IsInputEnabled() => (this.m_entity == null || !this.m_entity.HasQueuedChangeEntity() && (!this.m_entity.IsHeroPower() || !this.m_entity.HasQueuedControllerTagChange())) && this.m_inputEnabled;

  public void SetInputEnabled(bool enabled)
  {
    this.m_inputEnabled = enabled;
    this.UpdateActorState();
  }

  public bool IsAllowedToShowTooltip() => !((UnityEngine.Object) this.m_zone == (UnityEngine.Object) null) && (this.m_zone.m_ServerTag == TAG_ZONE.PLAY || this.m_zone.m_ServerTag == TAG_ZONE.SECRET || this.m_zone.m_ServerTag != TAG_ZONE.HAND || this.m_zone.m_Side == Player.Side.OPPOSING) && (GameState.Get() == null || !this.m_entity.IsHero() || this.m_entity.GetZone() != TAG_ZONE.PLAY || GameState.Get().GetBooleanGameOption(GameEntityOption.SHOW_HERO_TOOLTIPS)) && (this.m_entity.IsBobQuest() || !this.m_entity.IsQuest() && !this.m_entity.IsQuestline() && !this.m_entity.IsPuzzle() && !this.m_entity.IsRulebook());

  public bool IsAbleToShowTooltip() => this.m_entity != null && !((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null) && !((UnityEngine.Object) BigCard.Get() == (UnityEngine.Object) null);

  public bool GetShouldShowTooltip() => this.m_shouldShowTooltip;

  public void SetShouldShowTooltip()
  {
    if (!this.IsAllowedToShowTooltip() || this.m_shouldShowTooltip)
      return;
    this.m_shouldShowTooltip = true;
  }

  public void ShowTooltip()
  {
    if (this.m_showTooltip)
      return;
    this.m_showTooltip = true;
    this.UpdateTooltip();
  }

  public void HideTooltip()
  {
    this.m_shouldShowTooltip = false;
    if (!this.m_showTooltip)
      return;
    this.m_showTooltip = false;
    this.UpdateTooltip();
  }

  public bool IsShowingTooltip() => this.m_showTooltip;

  private void ShowMouseOverSpell()
  {
    if (this.m_entity == null || (UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    if (this.m_entity.HasTag(GAME_TAG.VOODOO_LINK) || this.m_entity.DoEnchantmentsHaveVoodooLink())
    {
      Spell spell = this.m_actor.GetSpell(SpellType.VOODOO_LINK);
      if ((bool) (UnityEngine.Object) spell)
      {
        spell.SetSource(this.gameObject);
        spell.Activate();
      }
    }
    string cardId = this.m_entity.GetCardId();
    if (!(cardId == MagtheridonLinkToHellfireWardersSpell.MagtheridonId) && !(cardId == MagtheridonLinkToHellfireWardersSpell.HellfireWarderId))
      return;
    Spell spell1 = this.m_actor.GetSpell(SpellType.MAGTHERIDON_LINK);
    if (!(bool) (UnityEngine.Object) spell1)
      return;
    spell1.SetSource(this.gameObject);
    spell1.Activate();
  }

  private void HideMouseOverSpell()
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    Spell spellIfLoaded1 = this.m_actor.GetSpellIfLoaded(SpellType.VOODOO_LINK);
    if ((bool) (UnityEngine.Object) spellIfLoaded1)
      spellIfLoaded1.Deactivate();
    Spell spellIfLoaded2 = this.m_actor.GetSpellIfLoaded(SpellType.MAGTHERIDON_LINK);
    if (!(bool) (UnityEngine.Object) spellIfLoaded2)
      return;
    spellIfLoaded2.Deactivate();
  }

  public void UpdateTooltip()
  {
    if ((!this.GetShouldShowTooltip() || !this.IsAllowedToShowTooltip() ? 0 : (this.IsAbleToShowTooltip() ? 1 : 0)) != 0 && this.m_showTooltip)
    {
      this.ShowMouseOverSpell();
      if (!((UnityEngine.Object) BigCard.Get() != (UnityEngine.Object) null))
        return;
      BigCard.Get().Show(this);
    }
    else
    {
      this.m_showTooltip = false;
      this.m_shouldShowTooltip = false;
      this.HideMouseOverSpell();
      if (!((UnityEngine.Object) BigCard.Get() != (UnityEngine.Object) null))
        return;
      BigCard.Get().Hide(this);
    }
  }

  public bool IsAttacking() => this.m_attacking;

  public void EnableAttacking(bool enable) => this.m_attacking = enable;

  public bool IsMoving() => this.m_moving;

  public void EnableMoving(bool enable) => this.m_moving = enable;

  public bool WillIgnoreDeath() => this.m_ignoreDeath;

  public void IgnoreDeath(bool ignore) => this.m_ignoreDeath = ignore;

  public bool WillSuppressDeathEffects() => this.m_suppressDeathEffects;

  public void SuppressDeathEffects(bool suppress) => this.m_suppressDeathEffects = suppress;

  public bool WillSuppressDeathSounds() => this.m_suppressDeathSounds;

  public void SuppressDeathSounds(bool suppress) => this.m_suppressDeathSounds = suppress;

  public bool WillSuppressKeywordDeaths() => this.m_suppressKeywordDeaths;

  public void SuppressKeywordDeaths(bool suppress) => this.m_suppressKeywordDeaths = suppress;

  public float GetKeywordDeathDelaySec() => this.m_keywordDeathDelaySec;

  public void SetKeywordDeathDelaySec(float sec) => this.m_keywordDeathDelaySec = sec;

  public bool WillSuppressActorTriggerSpell() => this.m_suppressActorTriggerSpell;

  public void SuppressActorTriggerSpell(bool suppress) => this.m_suppressActorTriggerSpell = suppress;

  public bool WillSuppressPlaySounds()
  {
    if ((this.GetEntity() == null || !this.GetEntity().HasTag(GAME_TAG.SUPPRESS_ALL_SUMMON_VO)) && !this.GetController().HasTag(GAME_TAG.SUPPRESS_SUMMON_VO_FOR_PLAYER))
      return this.m_suppressPlaySoundCount > 0;
    return this.GetEntity().GetTag(GAME_TAG.DONT_SUPPRESS_SUMMON_VO) != 1;
  }

  public bool WillSuppressCustomSpells() => GameState.Get().GetGameEntity().HasTag(GAME_TAG.FORCE_NO_CUSTOM_SPELLS) || this.GetController().HasTag(GAME_TAG.FORCE_NO_CUSTOM_SPELLS) || this.GetEntity().HasTag(GAME_TAG.FORCE_NO_CUSTOM_SPELLS);

  public bool WillSuppressCustomSummonSpells() => GameState.Get().GetGameEntity().HasTag(GAME_TAG.FORCE_NO_CUSTOM_SUMMON_SPELLS) || this.GetController().HasTag(GAME_TAG.FORCE_NO_CUSTOM_SUMMON_SPELLS) || this.GetEntity().HasTag(GAME_TAG.FORCE_NO_CUSTOM_SUMMON_SPELLS);

  public bool WillSuppressCustomLifetimeSpells() => GameState.Get().GetGameEntity().HasTag(GAME_TAG.FORCE_NO_CUSTOM_LIFETIME_SPELLS) || this.GetController().HasTag(GAME_TAG.FORCE_NO_CUSTOM_LIFETIME_SPELLS) || this.GetEntity().HasTag(GAME_TAG.FORCE_NO_CUSTOM_LIFETIME_SPELLS);

  public bool WillSuppressCustomKeywordSpells() => GameState.Get().GetGameEntity().HasTag(GAME_TAG.FORCE_NO_CUSTOM_KEYWORD_SPELLS) || this.GetController().HasTag(GAME_TAG.FORCE_NO_CUSTOM_KEYWORD_SPELLS) || this.GetEntity().HasTag(GAME_TAG.FORCE_NO_CUSTOM_KEYWORD_SPELLS);

  public void SuppressPlaySounds(bool suppress)
  {
    if (suppress)
    {
      ++this.m_suppressPlaySoundCount;
    }
    else
    {
      if (--this.m_suppressPlaySoundCount >= 0)
        return;
      this.m_suppressPlaySoundCount = 0;
    }
  }

  public void SuppressHandToDeckTransition() => this.m_suppressHandToDeckTransition = true;

  public bool IsShown() => this.m_shown;

  public void ShowCard()
  {
    if (this.m_shown)
      return;
    this.m_shown = true;
    this.ShowImpl();
  }

  private void ShowImpl()
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    this.m_actor.Show();
    if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null)
      this.m_questRewardActor.Show();
    this.RefreshActor();
  }

  public void HideCard()
  {
    if (!this.m_shown || this.m_actorLoading)
      return;
    this.m_shown = false;
    this.HideImpl();
  }

  private void HideImpl()
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    this.m_actor.Hide();
    if (!((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null))
      return;
    this.m_questRewardActor.Hide();
  }

  public void SetBattleCrySource(bool source)
  {
    this.m_isBattleCrySource = source;
    if (!((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null))
      return;
    if (source)
    {
      LayerUtils.SetLayer(this.m_actor.gameObject, GameLayer.IgnoreFullScreenEffects);
    }
    else
    {
      LayerUtils.SetLayer(this.m_actor.gameObject, GameLayer.Default);
      LayerUtils.SetLayer(this.m_actor.GetMeshRenderer().gameObject, GameLayer.CardRaycast);
    }
  }

  public void DoTauntNotification()
  {
    if ((UnityEngine.Object) this.m_activeSpawnSpell != (UnityEngine.Object) null && this.m_activeSpawnSpell.IsActive() || !((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null))
      return;
    iTween.PunchScale(this.m_actor.gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
  }

  public void UpdateProposedManaUsage()
  {
    if (GameState.Get() == null || GameState.Get().GetSelectedOption() != -1)
      return;
    Player player = GameState.Get().GetPlayer(this.GetEntity().GetControllerId());
    if (player == null || !player.IsFriendlySide() || !player.HasTag(GAME_TAG.CURRENT_PLAYER))
      return;
    if (this.m_mousedOver)
    {
      bool flag1 = this.m_entity.GetZone() == TAG_ZONE.HAND;
      bool flag2 = this.m_entity.IsCardButton();
      if (!(flag1 | flag2) || !GameState.Get().IsValidOption(this.m_entity) || this.m_entity.IsSpell() && player.HasTag(GAME_TAG.SPELLS_COST_HEALTH) || this.m_entity.HasTag(GAME_TAG.CARD_COSTS_HEALTH) || this.m_entity.HasTag(GAME_TAG.CARD_COSTS_ARMOR) || !flag1 && this.m_entity.IsLocation())
        return;
      player.ProposeManaCrystalUsage(this.m_entity);
    }
    else
      player.CancelAllProposedMana(this.m_entity);
  }

  public void SetMagneticPlayData(MagneticPlayData data)
  {
    if (data == null)
      return;
    if (this.m_magneticPlayData != null)
      Log.Gameplay.PrintError("{0}.SetMagneticPlayData: m_magneticPlayData is already set! {1}", (object) this, (object) this.m_magneticPlayData);
    this.m_magneticPlayData = data;
  }

  public MagneticPlayData GetMagneticPlayData() => this.m_magneticPlayData;

  public void SetIsMagneticTarget(bool isTarget) => this.m_magneticTarget = isTarget;

  public bool IsMagneticTarget() => this.m_magneticTarget;

  public void DetermineIfOverrideDrawTimeScale()
  {
    if (this.m_drawTimeScale.HasValue)
      return;
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.ALWAYS_USE_FAST_CARD_DRAW_SCALE))
      this.m_drawTimeScale = new float?(0.556f);
    else if (this.m_cardDrawTracker < 3)
      this.m_drawTimeScale = new float?(1f);
    else if (this.m_cardDrawTracker <= 6)
      this.m_drawTimeScale = new float?((float) (1.0 + -0.111000001430511 * (double) (this.m_cardDrawTracker + 1 - 3)));
    else
      this.m_drawTimeScale = new float?(0.556f);
  }

  public void ResetCardDrawTimeScale() => this.m_drawTimeScale = new float?();

  public bool CanPlayHealingDoesDamageHint()
  {
    if (!this.IsShown() || this.m_entity == null || (UnityEngine.Object) this.m_actor == (UnityEngine.Object) null || !this.m_actor.IsShown())
      return false;
    return this.m_entity.HasTag(GAME_TAG.AFFECTED_BY_HEALING_DOES_DAMAGE) || this.m_entity.HasTag(GAME_TAG.LIFESTEAL) || this.m_entity.GetCardTextBuilder().ContainsBonusHealingToken(this.m_entity);
  }

  public bool CanPlayLifestealDoesDamageHint() => this.IsShown() && this.m_entity != null && !((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null) && this.m_actor.IsShown() && this.m_entity.HasTag(GAME_TAG.LIFESTEAL);

  public bool CanPlaySpellPowerHint(TAG_SPELL_SCHOOL spellSchool = TAG_SPELL_SCHOOL.NONE)
  {
    if (!this.IsShown() || (UnityEngine.Object) this.m_actor == (UnityEngine.Object) null || !this.m_actor.IsShown() || this.m_entity == null)
      return false;
    TAG_SPELL_SCHOOL spellSchool1 = this.m_entity.GetSpellSchool();
    Player controller = this.m_entity.GetController();
    if (controller.TotalSpellpower(this.m_entity, spellSchool1) == 0 || (this.m_entity.HasTag(GAME_TAG.SECRET) || this.m_entity.HasTag(GAME_TAG.SIGIL)) && controller.IsSpellpowerTemporary(spellSchool1) || (spellSchool == TAG_SPELL_SCHOOL.NONE ? 1 : (spellSchool == spellSchool1 ? 1 : 0)) == 0)
      return false;
    return this.m_entity.IsAffectedBySpellPower() || this.m_entity.GetCardTextBuilder().ContainsBonusDamageToken(this.m_entity);
  }

  public DefLoader.DisposableCardDef ShareDisposableCardDef() => this.m_cardDef?.Share();

  public void SetCardDef(DefLoader.DisposableCardDef cardDef, bool updateActor)
  {
    if ((UnityEngine.Object) this.m_cardDef?.CardDef == (UnityEngine.Object) cardDef?.CardDef)
      return;
    this.ReleaseCardDef();
    this.m_cardDef = cardDef.Share();
    this.InitCardDefAssets();
    if (!((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null) || updateActor)
      return;
    this.m_actor.SetCardDef(this.m_cardDef);
    this.m_actor.UpdateAllComponents();
  }

  public void PurgeSpells()
  {
    foreach (CardEffect allEffect in this.m_allEffects)
      allEffect.PurgeSpells();
  }

  private bool ShouldPreloadCardAssets() => !HearthstoneApplication.IsPublic() && Options.Get().GetBool(Option.PRELOAD_CARD_ASSETS, false);

  public void OverrideCustomSpawnSpell(Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      Debug.LogErrorFormat("Tried to set OverrideCustomSpawnSpell to null!");
    else
      this.m_customSpawnSpellOverride = this.SetupOverrideSpell(this.m_customSpawnSpellOverride, spell);
  }

  public void OverrideCustomDeathSpell(Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      Debug.LogErrorFormat("Tried to set OverrideCustomDeathSpell to null!");
    else
      this.m_customDeathSpellOverride = this.SetupOverrideSpell(this.m_customDeathSpellOverride, spell);
  }

  public void OverrideCustomDiscardSpell(Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      Debug.LogErrorFormat("Tried to set OverrideCustomDiscardSpell to null!");
    else
      this.m_customDiscardSpellOverride = this.SetupOverrideSpell(this.m_customDiscardSpellOverride, spell);
  }

  public Texture GetPreferredActorPortraitTexture()
  {
    int num = (UnityEngine.Object) this.m_cardDef?.CardDef == (UnityEngine.Object) null ? 1 : this.m_cardDef.CardDef.m_PreferredActorPortraitIndex;
    Texture actorPortraitTexture = (Texture) null;
    switch (num)
    {
      case 0:
        actorPortraitTexture = this.GetPortraitTexture();
        break;
      case 1:
        actorPortraitTexture = this.GetGoldenMaterial().mainTexture;
        break;
    }
    return actorPortraitTexture;
  }

  public Texture GetPortraitTexture(TAG_PREMIUM premium = TAG_PREMIUM.NORMAL) => !((UnityEngine.Object) this.m_cardDef?.CardDef == (UnityEngine.Object) null) ? this.m_cardDef.CardDef.GetPortraitTexture(premium) : (Texture) null;

  public Material GetGoldenMaterial() => !((UnityEngine.Object) this.m_cardDef?.CardDef == (UnityEngine.Object) null) ? this.m_cardDef.CardDef.GetPremiumPortraitMaterial() : (Material) null;

  public CardEffect GetPlayEffect(int index)
  {
    if (index <= 0)
      return this.m_playEffect;
    return --index >= this.m_additionalPlayEffects.Count ? (CardEffect) null : this.m_additionalPlayEffects[index];
  }

  public CardEffect GetOrCreateProxyEffect(
    Network.HistBlockStart blockStart,
    CardEffectDef proxyEffectDef)
  {
    if (this.m_proxyEffects == null)
      this.m_proxyEffects = new Map<Network.HistBlockStart, CardEffect>();
    if (this.m_proxyEffects.ContainsKey(blockStart))
      return this.m_proxyEffects[blockStart];
    CardEffect effect = new CardEffect(proxyEffectDef, this);
    this.InitEffect(proxyEffectDef, ref effect);
    this.m_proxyEffects.Add(blockStart, effect);
    return effect;
  }

  public void DeactivatePlaySpell()
  {
    Entity entity = this.GetEntity();
    Entity parentEntity = entity.GetParentEntity();
    Spell spell = parentEntity != null ? parentEntity.GetCard().GetSubOptionSpell(parentEntity.GetSubCardIndex(entity), 0, false) : this.GetPlaySpell(0, false);
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null) || spell.GetActiveState() == SpellStateType.NONE)
      return;
    spell.SafeActivateState(SpellStateType.CANCEL);
  }

  public Spell GetPlaySpell(int index, bool loadIfNeeded = true) => this.GetPlayEffect(index)?.GetSpell(loadIfNeeded);

  public List<CardSoundSpell> GetPlaySoundSpells(int index, bool loadIfNeeded = true) => this.GetPlayEffect(index)?.GetSoundSpells(loadIfNeeded);

  public Spell GetAttackSpell(bool loadIfNeeded = true) => this.m_attackEffect == null ? (Spell) null : this.m_attackEffect.GetSpell(loadIfNeeded);

  public List<CardSoundSpell> GetAttackSoundSpells(bool loadIfNeeded = true) => this.m_attackEffect == null ? (List<CardSoundSpell>) null : this.m_attackEffect.GetSoundSpells(loadIfNeeded);

  public List<CardSoundSpell> GetDeathSoundSpells(bool loadIfNeeded = true) => this.m_deathEffect == null ? (List<CardSoundSpell>) null : this.m_deathEffect.GetSoundSpells(loadIfNeeded);

  public Spell GetLifetimeSpell(bool loadIfNeeded = true) => this.m_lifetimeEffect == null ? (Spell) null : this.m_lifetimeEffect.GetSpell(loadIfNeeded);

  public List<CardSoundSpell> GetLifetimeSoundSpells(bool loadIfNeeded = true) => this.m_lifetimeEffect == null ? (List<CardSoundSpell>) null : this.m_lifetimeEffect.GetSoundSpells(loadIfNeeded);

  public CardEffect GetSubOptionEffect(int suboption, int index)
  {
    if (suboption < 0)
      return (CardEffect) null;
    if (index > 0)
    {
      if (this.m_additionalSubOptionEffects == null)
        return (CardEffect) null;
      if (suboption >= this.m_additionalSubOptionEffects.Count)
        return (CardEffect) null;
      List<CardEffect> additionalSubOptionEffect = this.m_additionalSubOptionEffects[suboption];
      if (additionalSubOptionEffect == null)
        return (CardEffect) null;
      return --index >= additionalSubOptionEffect.Count ? (CardEffect) null : additionalSubOptionEffect[index];
    }
    if (this.m_subOptionEffects == null)
      return (CardEffect) null;
    return suboption >= this.m_subOptionEffects.Count ? (CardEffect) null : this.m_subOptionEffects[suboption];
  }

  public Spell GetSubOptionSpell(int suboption, int index, bool loadIfNeeded = true) => this.GetSubOptionEffect(suboption, index)?.GetSpell(loadIfNeeded);

  public List<CardSoundSpell> GetSubOptionSoundSpells(
    int suboption,
    int index,
    bool loadIfNeeded = true)
  {
    return this.GetSubOptionEffect(suboption, index)?.GetSoundSpells(loadIfNeeded);
  }

  public CardEffect GetTriggerEffect(int index)
  {
    if (this.m_triggerEffects == null)
      return (CardEffect) null;
    if (index < 0)
      return (CardEffect) null;
    return index >= this.m_triggerEffects.Count ? (CardEffect) null : this.m_triggerEffects[index];
  }

  public CardEffect GetResetGameEffect(int index)
  {
    if (this.m_resetGameEffects == null)
      return (CardEffect) null;
    if (index < 0)
      return (CardEffect) null;
    return index >= this.m_resetGameEffects.Count ? (CardEffect) null : this.m_resetGameEffects[index];
  }

  public Spell GetTriggerSpell(int index, bool loadIfNeeded = true) => this.GetTriggerEffect(index)?.GetSpell(loadIfNeeded);

  public List<CardSoundSpell> GetTriggerSoundSpells(int index, bool loadIfNeeded = true) => this.GetTriggerEffect(index)?.GetSoundSpells(loadIfNeeded);

  public Spell GetCustomKeywordSpell() => this.m_customKeywordEffect == null ? (Spell) null : this.m_customKeywordEffect.GetSpell();

  public Spell GetCustomSummonSpell() => this.m_customSummonSpell;

  public Spell GetCustomSpawnSpell() => this.m_customSpawnSpell;

  public Spell GetCustomSpawnSpellOverride() => this.m_customSpawnSpellOverride;

  public Spell GetCustomDeathSpell() => this.m_customDeathSpell;

  public Spell GetCustomDeathSpellOverride() => this.m_customDeathSpellOverride;

  public Spell GetCustomChoiceRevealSpell()
  {
    if (this.m_customChoiceRevealEffect == null)
      return (Spell) null;
    Spell spell = this.m_customChoiceRevealEffect.GetSpell();
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null) || !spell.IsActive())
      return spell;
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.ReleaseCustomChoiceActiveSpell));
    return this.m_customChoiceRevealEffect.LoadSpell();
  }

  private void ReleaseCustomChoiceActiveSpell(Spell spell, object data)
  {
    SpellManager spellManager = SpellManager.Get();
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null || spellManager == null)
      return;
    spellManager.ReleaseSpell(spell);
  }

  public Spell GetCustomChoiceConcealSpell() => this.m_customChoiceConcealEffect == null ? (Spell) null : this.m_customChoiceConcealEffect.GetSpell();

  public Spell GetSpellTableOverride(SpellType spellType)
  {
    CardEffect cardEffect = (CardEffect) null;
    if (this.m_spellTableOverrideEffects.TryGetValue(spellType, out cardEffect))
      return cardEffect.GetSpell();
    foreach (SpellTableOverride spellTableOverride in this.m_cardDef.CardDef.m_SpellTableOverrides)
    {
      if (spellTableOverride.m_Type == spellType)
      {
        if (!string.IsNullOrEmpty(spellTableOverride.m_SpellPrefabName))
        {
          CardEffect effect = (CardEffect) null;
          this.InitEffect(spellTableOverride.m_SpellPrefabName, ref effect);
          if (effect != null)
          {
            this.m_spellTableOverrideEffects[spellType] = effect;
            return effect.GetSpell();
          }
        }
        else
          break;
      }
    }
    return (Spell) null;
  }

  public AudioSource GetAnnouncerLine(Card.AnnouncerLineType type)
  {
    CardSound cardSound = this.m_announcerLine[(int) type];
    if (cardSound == null || (UnityEngine.Object) cardSound.GetSound() == (UnityEngine.Object) null)
    {
      if (this.m_announcerLine[0] != null)
      {
        cardSound = this.m_announcerLine[0];
      }
      else
      {
        string message = string.Format("Card.GetAnnouncerLine(AnnouncerLineType type) - Failed to load announcer audio source.");
        if (HearthstoneApplication.UseDevWorkarounds())
          Debug.LogError((object) message);
        return SoundManager.Get().GetPlaceholderSource();
      }
    }
    return cardSound.GetSound();
  }

  public EmoteEntry GetEmoteEntry(EmoteType emoteType)
  {
    if (this.m_emotes == null)
      return (EmoteEntry) null;
    bool flag = emoteType == EmoteType.GREETINGS || emoteType == EmoteType.MIRROR_GREETINGS;
    if (SpecialEventManager.Get().IsEventActive(SpecialEventType.LUNAR_NEW_YEAR, false))
    {
      if (flag)
      {
        foreach (EmoteEntry emote in this.m_emotes)
        {
          if (emote.GetEmoteType() == EmoteType.HAPPY_NEW_YEAR_LUNAR)
            return emote;
        }
      }
    }
    else if (SpecialEventManager.Get().IsEventActive(SpecialEventType.FEAST_OF_WINTER_VEIL, false))
    {
      if (flag)
      {
        foreach (EmoteEntry emote in this.m_emotes)
        {
          if (emote.GetEmoteType() == EmoteType.HAPPY_HOLIDAYS)
            return emote;
        }
      }
    }
    else if (SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_FIRE_FESTIVAL_EMOTES_EVERGREEN, false))
    {
      if (flag)
      {
        foreach (EmoteEntry emote in this.m_emotes)
        {
          if (emote.GetEmoteType() == EmoteType.FIRE_FESTIVAL)
            return emote;
        }
      }
      else if (emoteType == EmoteType.WOW)
      {
        foreach (EmoteEntry emote in this.m_emotes)
        {
          if (emote.GetEmoteType() == EmoteType.FIRE_FESTIVAL_FIREWORKS_RANK_THREE)
            return emote;
        }
      }
    }
    else if (SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_HAPPY_NEW_YEAR, false))
    {
      if (flag)
      {
        foreach (EmoteEntry emote in this.m_emotes)
        {
          if (emote.GetEmoteType() == EmoteType.HAPPY_NEW_YEAR)
            return emote;
        }
      }
    }
    else if (SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_PIRATE_DAY, false))
    {
      if (flag)
      {
        foreach (EmoteEntry emote in this.m_emotes)
        {
          if (emote.GetEmoteType() == EmoteType.PIRATE_DAY)
            return emote;
        }
      }
    }
    else if (SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_NOBLEGARDEN, false) && flag)
    {
      foreach (EmoteEntry emote in this.m_emotes)
      {
        if (emote.GetEmoteType() == EmoteType.HAPPY_NOBLEGARDEN)
          return emote;
      }
    }
    foreach (EmoteEntry emote in this.m_emotes)
    {
      if (emote.GetEmoteType() == emoteType)
        return emote;
    }
    return (EmoteEntry) null;
  }

  public Spell GetBestSummonSpell() => this.GetBestSummonSpell(out bool _);

  public Spell GetBestSummonSpell(out bool standard)
  {
    if ((UnityEngine.Object) this.m_customSummonSpell != (UnityEngine.Object) null && this.GetMagneticPlayData() == null && this.GetEntity() != null && !this.GetEntity().HasTag(GAME_TAG.CARD_DOES_NOTHING) && !this.WillSuppressCustomSpells() && !this.WillSuppressCustomSummonSpells())
    {
      standard = false;
      return this.m_customSummonSpell;
    }
    standard = true;
    if ((UnityEngine.Object) this.m_cardDef?.CardDef == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("Cannot determine best summon spell. Missing CardDef");
      return (Spell) null;
    }
    GameState gameState = GameState.Get();
    return this.GetActorSpell(this.m_cardDef.CardDef.DetermineSummonInSpell_HandToPlay(this, gameState != null && gameState.GetGameEntity().HasTag(GAME_TAG.USE_FAST_ACTOR_TRANSITION_ANIMATIONS)));
  }

  public Spell GetBestSpawnSpell() => this.GetBestSpawnSpell(out bool _);

  public Spell GetBestSpawnSpell(out bool standard)
  {
    standard = false;
    if (this.m_entity.HasTag(GAME_TAG.HAS_BEEN_REBORN))
    {
      Spell actorSpell = this.GetActorSpell(SpellType.REBORN_SPAWN);
      if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
        return actorSpell;
    }
    if ((bool) (UnityEngine.Object) this.m_customSpawnSpellOverride)
      return this.m_customSpawnSpellOverride;
    if ((bool) (UnityEngine.Object) this.m_customSpawnSpell)
      return this.m_customSpawnSpell;
    switch (this.m_entity.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE))
    {
      case TAG_ROLE.CASTER:
        return this.GetActorSpell(SpellType.LETTUCE_COME_IN_PLAY_CASTER);
      case TAG_ROLE.FIGHTER:
        return this.GetActorSpell(SpellType.LETTUCE_COME_IN_PLAY_FIGHTER);
      case TAG_ROLE.TANK:
        return this.GetActorSpell(SpellType.LETTUCE_COME_IN_PLAY_PROTECTOR);
      default:
        standard = true;
        return this.m_entity.IsControlledByFriendlySidePlayer() ? this.GetActorSpell(SpellType.FRIENDLY_SPAWN_MINION_OR_LOCATION) : this.GetActorSpell(SpellType.OPPONENT_SPAWN_MINION_OR_LOCATION);
    }
  }

  public Spell GetBestDeathSpell() => this.GetBestDeathSpell(out bool _);

  public Spell GetBestDeathSpell(out bool standard) => this.GetBestDeathSpell(this.m_actor, out standard);

  private Spell GetBestDeathSpell(Actor actor) => this.GetBestDeathSpell(actor, out bool _);

  private Spell GetBestDeathSpell(Actor actor, out bool standard)
  {
    standard = false;
    if (this.m_prevZone is ZoneHand && this.m_zone is ZoneGraveyard)
    {
      if ((bool) (UnityEngine.Object) this.m_customDiscardSpellOverride)
        return this.m_customDiscardSpellOverride;
      if ((bool) (UnityEngine.Object) this.m_customDiscardSpell && !this.m_entity.IsSilenced())
        return this.m_customDiscardSpell;
    }
    else
    {
      if ((bool) (UnityEngine.Object) this.m_customDeathSpellOverride)
        return this.m_customDeathSpellOverride;
      if ((bool) (UnityEngine.Object) this.m_customDeathSpell && !this.m_entity.IsSilenced())
        return this.m_customDeathSpell;
    }
    standard = true;
    return actor.GetSpell(SpellType.DEATH);
  }

  public void ActivateCharacterPlayEffects()
  {
    if (!this.WillSuppressPlaySounds())
      this.ActivateSoundSpellList(this.m_playEffect.GetSoundSpells());
    this.SuppressPlaySounds(false);
    this.ActivateLifetimeEffects();
  }

  public void ActivateCharacterTradeEffects()
  {
    if (this.m_additionalPlayEffects.Count <= 0)
      return;
    this.ActivateSoundSpellList(this.m_additionalPlayEffects[0].GetSoundSpells());
  }

  public void ActivateCharacterAttackEffects() => this.ActivateSoundSpellList(this.m_attackEffect.GetSoundSpells());

  public void ActivateCharacterDeathEffects()
  {
    if (this.m_suppressDeathEffects)
      return;
    if (!this.m_suppressDeathSounds)
    {
      if ((this.m_emotes == null ? -1 : this.m_emotes.FindIndex((Predicate<EmoteEntry>) (e => e != null && e.GetEmoteType() == EmoteType.DEATH_LINE))) >= 0)
        this.PlayEmote(EmoteType.DEATH_LINE);
      else
        this.ActivateSoundSpellList(this.m_deathEffect.GetSoundSpells());
    }
    this.m_suppressDeathSounds = false;
    this.DeactivateLifetimeEffects();
  }

  public void ActivateLifetimeEffects()
  {
    if (this.m_lifetimeEffect == null || this.m_entity.IsSilenced() || this.m_entity.HasTag(GAME_TAG.CARD_DOES_NOTHING) || this.WillSuppressCustomSpells() || this.WillSuppressCustomLifetimeSpells())
      return;
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    if (gameEntity != null && gameEntity.HasTag(GAME_TAG.SQUELCH_LIFETIME_EFFECTS))
      return;
    Spell spell = this.m_lifetimeEffect.GetSpell();
    if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
    {
      spell.Deactivate();
      spell.ActivateState(SpellStateType.BIRTH);
    }
    if (this.m_lifetimeEffect.GetSoundSpells() == null)
      return;
    this.ActivateSoundSpellList(this.m_lifetimeEffect.GetSoundSpells());
  }

  public void DeactivateLifetimeEffects()
  {
    if (this.m_lifetimeEffect == null)
      return;
    Spell spell = this.m_lifetimeEffect.GetSpell();
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null))
      return;
    switch (spell.GetActiveState())
    {
      case SpellStateType.NONE:
        break;
      case SpellStateType.DEATH:
        break;
      default:
        spell.ActivateState(SpellStateType.DEATH);
        break;
    }
  }

  public void ActivateCustomKeywordEffect()
  {
    if (this.m_customKeywordEffect == null || this.GetEntity() != null && (this.GetEntity().HasTag(GAME_TAG.CARD_DOES_NOTHING) || this.WillSuppressCustomSpells() || this.WillSuppressCustomKeywordSpells()))
      return;
    Spell spell = this.m_customKeywordEffect.GetSpell();
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("Card.ActivateCustomKeywordEffect() -- failed to load custom keyword spell for card {0}", (object) ((object) this).ToString()));
    }
    else
    {
      if (spell.DoesBlockServerEvents())
        GameState.Get().AddServerBlockingSpell(spell);
      TransformUtil.AttachAndPreserveLocalTransform(spell.transform, this.m_actor.transform);
      spell.ActivateState(SpellStateType.BIRTH);
    }
  }

  public void DeactivateCustomKeywordEffect()
  {
    if (this.m_customKeywordEffect == null)
      return;
    Spell spell = this.m_customKeywordEffect.GetSpell(false);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null || !spell.IsActive())
      return;
    spell.ActivateState(SpellStateType.DEATH);
  }

  public bool ActivateSoundSpellList(List<CardSoundSpell> soundSpells)
  {
    if (soundSpells == null || soundSpells.Count == 0)
      return false;
    bool flag = false;
    for (int index = 0; index < soundSpells.Count; ++index)
    {
      this.ActivateSoundSpell(soundSpells[index]);
      flag = true;
    }
    return flag;
  }

  public bool ActivateSoundSpell(CardSoundSpell soundSpell)
  {
    if ((UnityEngine.Object) soundSpell == (UnityEngine.Object) null || this.GetEntity().HasTag(GAME_TAG.CARD_DOES_NOTHING))
      return false;
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    if (gameEntity == null)
      return false;
    if (gameEntity.GetGameOptions().GetBooleanOption(GameEntityOption.DELAY_CARD_SOUND_SPELLS))
      this.StartCoroutine(this.WaitThenActivateSoundSpell(soundSpell));
    else
      soundSpell.Reactivate();
    return true;
  }

  public bool HasActiveEmoteSound()
  {
    if (this.m_emotes == null)
      return false;
    foreach (EmoteEntry emote in this.m_emotes)
    {
      CardSoundSpell soundSpell = emote.GetSoundSpell(false);
      if ((UnityEngine.Object) soundSpell != (UnityEngine.Object) null && soundSpell.IsActive())
        return true;
    }
    return false;
  }

  public EmoteEntry GetActiveEmoteSound()
  {
    if (this.m_emotes == null)
      return (EmoteEntry) null;
    foreach (EmoteEntry emote in this.m_emotes)
    {
      CardSoundSpell soundSpell = emote.GetSoundSpell(false);
      if ((UnityEngine.Object) soundSpell != (UnityEngine.Object) null && soundSpell.IsActive())
        return emote;
    }
    return (EmoteEntry) null;
  }

  public bool HasUnfinishedEmoteSpell()
  {
    if (this.m_emotes == null)
      return false;
    foreach (EmoteEntry emote in this.m_emotes)
    {
      Spell spell = emote.GetSpell(false);
      if ((UnityEngine.Object) spell != (UnityEngine.Object) null && !spell.IsFinished())
        return true;
    }
    return false;
  }

  public CardSoundSpell PlayEmote(EmoteType emoteType) => this.PlayEmote(emoteType, Notification.SpeechBubbleDirection.None);

  public CardSoundSpell PlayEmote(
    EmoteType emoteType,
    Notification.SpeechBubbleDirection overrideDirection)
  {
    EmoteEntry emoteEntry = this.GetEmoteEntry(emoteType);
    CardSoundSpell soundSpell1 = emoteEntry?.GetSoundSpell();
    Spell spell = emoteEntry?.GetSpell();
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return (CardSoundSpell) null;
    if ((UnityEngine.Object) soundSpell1 != (UnityEngine.Object) null)
    {
      soundSpell1.Reactivate();
      if (soundSpell1.IsActive())
      {
        for (int index = 0; index < this.m_emotes.Count; ++index)
        {
          EmoteEntry emote = this.m_emotes[index];
          if (emote != emoteEntry)
          {
            Spell soundSpell2 = (Spell) emote.GetSoundSpell(false);
            if ((bool) (UnityEngine.Object) soundSpell2)
              soundSpell2.Deactivate();
          }
        }
      }
      if (this.m_entity.IsHero())
        GameState.Get().GetGameEntity().OnEmotePlayed(this, emoteType, soundSpell1);
    }
    Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.BottomLeft;
    if (this.GetEntity().IsControlledByOpposingSidePlayer())
      direction = Notification.SpeechBubbleDirection.TopRight;
    if (overrideDirection != Notification.SpeechBubbleDirection.None)
      direction = overrideDirection;
    string speechText = (string) null;
    if ((UnityEngine.Object) soundSpell1 != (UnityEngine.Object) null)
    {
      speechText = string.Empty;
      if (soundSpell1 is CardSpecificVoSpell)
      {
        CardSpecificVoData bestVoiceData = ((CardSpecificVoSpell) soundSpell1).GetBestVoiceData();
        if (bestVoiceData != null && !string.IsNullOrEmpty(bestVoiceData.m_GameStringKey))
          speechText = GameStrings.Get(bestVoiceData.m_GameStringKey);
      }
    }
    if (string.IsNullOrEmpty(speechText) && emoteEntry != null && !string.IsNullOrEmpty(emoteEntry.GetGameStringKey()))
      speechText = GameStrings.Get(emoteEntry.GetGameStringKey());
    Notification notification = (Notification) null;
    if (!string.IsNullOrEmpty(speechText))
    {
      notification = NotificationManager.Get().CreateSpeechBubble(speechText, direction, this.m_actor, true);
      float delaySeconds = 1.5f;
      if ((bool) (UnityEngine.Object) soundSpell1)
      {
        AudioSource activeAudioSource = soundSpell1.GetActiveAudioSource();
        if ((bool) (UnityEngine.Object) activeAudioSource && (bool) (UnityEngine.Object) activeAudioSource.clip && (double) delaySeconds < (double) activeAudioSource.clip.length)
          delaySeconds = activeAudioSource.clip.length;
      }
      NotificationManager.Get().DestroyNotification(notification, delaySeconds);
    }
    if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
    {
      VisualEmoteSpell visualEmoteSpell = spell as VisualEmoteSpell;
      if ((UnityEngine.Object) visualEmoteSpell != (UnityEngine.Object) null && visualEmoteSpell.m_PositionOnSpeechBubble && (UnityEngine.Object) notification != (UnityEngine.Object) null)
      {
        visualEmoteSpell.SetSource(notification.gameObject);
        visualEmoteSpell.Reactivate();
      }
      else
        spell.Reactivate();
    }
    Card.EmotePlayCallback emotePlayCallback = this.OnEmotePlayCallback;
    if (emotePlayCallback != null)
      emotePlayCallback(emoteType);
    return soundSpell1;
  }

  private void InitCardDefAssets()
  {
    this.InitEffect(this.m_cardDef.CardDef.m_PlayEffectDef, ref this.m_playEffect);
    this.InitEffectList(this.m_cardDef.CardDef.m_AdditionalPlayEffectDefs, ref this.m_additionalPlayEffects);
    this.InitEffect(this.m_cardDef.CardDef.m_AttackEffectDef, ref this.m_attackEffect);
    this.InitEffect(this.m_cardDef.CardDef.m_DeathEffectDef, ref this.m_deathEffect);
    this.InitEffect(this.m_cardDef.CardDef.m_LifetimeEffectDef, ref this.m_lifetimeEffect);
    this.InitEffect(this.m_cardDef.CardDef.m_CustomKeywordSpellPath, ref this.m_customKeywordEffect);
    this.InitEffect(this.m_cardDef.CardDef.m_CustomChoiceRevealSpellPath, ref this.m_customChoiceRevealEffect);
    this.InitEffect(this.m_cardDef.CardDef.m_CustomChoiceConcealSpellPath, ref this.m_customChoiceConcealEffect);
    this.InitEffectList(this.m_cardDef.CardDef.m_SubOptionEffectDefs, ref this.m_subOptionEffects);
    this.InitEffectListList(this.m_cardDef.CardDef.m_AdditionalSubOptionEffectDefs, ref this.m_additionalSubOptionEffects);
    this.InitEffectList(this.m_cardDef.CardDef.m_TriggerEffectDefs, ref this.m_triggerEffects);
    this.InitEffectList(this.m_cardDef.CardDef.m_ResetGameEffectDefs, ref this.m_resetGameEffects);
    this.InitSound(this.m_cardDef.CardDef.m_AnnouncerLinePath, ref this.m_announcerLine[0], true);
    this.InitSound(this.m_cardDef.CardDef.m_AnnouncerLineBeforeVersusPath, ref this.m_announcerLine[1], false);
    this.InitSound(this.m_cardDef.CardDef.m_AnnouncerLineAfterVersusPath, ref this.m_announcerLine[2], false);
    this.InitEmoteList();
    if (!((UnityEngine.Object) this.m_cardDef.CardDef.m_LegendaryHeroSkinConfig != (UnityEngine.Object) null))
      return;
    if (this.m_entity.GetController() != null && this.m_entity.GetController().IsOpposingSide())
      this.m_cardDef.CardDef.m_LegendaryHeroSkinConfig.InitCombatAssets(this);
    else
      this.m_cardDef.CardDef.m_LegendaryHeroSkinConfig.InitAllAssets(this);
  }

  private void InitEffect(CardEffectDef effectDef, ref CardEffect effect)
  {
    this.DestroyCardEffect(ref effect);
    if (effectDef == null)
      return;
    effect = new CardEffect(effectDef, this);
    if (this.m_allEffects == null)
      this.m_allEffects = new List<CardEffect>();
    this.m_allEffects.Add(effect);
    if (!this.ShouldPreloadCardAssets())
      return;
    effect.LoadAll();
  }

  private void InitEffect(string spellPath, ref CardEffect effect)
  {
    this.DestroyCardEffect(ref effect);
    if (string.IsNullOrEmpty(spellPath))
      return;
    effect = new CardEffect(spellPath, this);
    if (this.m_allEffects == null)
      this.m_allEffects = new List<CardEffect>();
    this.m_allEffects.Add(effect);
    if (!this.ShouldPreloadCardAssets())
      return;
    effect.LoadAll();
  }

  private void InitEffectList(List<CardEffectDef> effectDefs, ref List<CardEffect> effects)
  {
    this.DestroyCardEffectList(ref effects);
    if (effectDefs == null)
      return;
    effects = new List<CardEffect>();
    for (int index = 0; index < effectDefs.Count; ++index)
    {
      CardEffectDef effectDef = effectDefs[index];
      CardEffect cardEffect = (CardEffect) null;
      if (effectDef != null)
      {
        cardEffect = new CardEffect(effectDef, this);
        if (this.m_allEffects == null)
          this.m_allEffects = new List<CardEffect>();
        this.m_allEffects.Add(cardEffect);
        if (this.ShouldPreloadCardAssets())
          cardEffect.LoadAll();
      }
      effects.Add(cardEffect);
    }
  }

  private void InitEffectListList(
    List<List<CardEffectDef>> effectDefs,
    ref List<List<CardEffect>> effects)
  {
    if (effects != null)
    {
      for (int index = 0; index < effects.Count; ++index)
      {
        List<CardEffect> effects1 = effects[index];
        this.DestroyCardEffectList(ref effects1);
      }
      effects = (List<List<CardEffect>>) null;
    }
    if (effectDefs == null)
      return;
    effects = new List<List<CardEffect>>();
    for (int index = 0; index < effectDefs.Count; ++index)
    {
      List<CardEffect> effects2 = effects[index];
      this.InitEffectList(effectDefs[index], ref effects2);
    }
  }

  private void InitSound(string path, ref CardSound cardSound, bool alwaysValid)
  {
    this.DestroyCardSound(ref cardSound);
    if (string.IsNullOrEmpty(path))
      return;
    cardSound = new CardSound(path, this, alwaysValid);
    if (!this.ShouldPreloadCardAssets())
      return;
    cardSound.GetSound();
  }

  private void InitEmoteList()
  {
    this.DestroyEmoteList();
    if (this.m_cardDef.CardDef.m_EmoteDefs == null)
      return;
    this.m_emotes = new List<EmoteEntry>();
    for (int index = 0; index < this.m_cardDef.CardDef.m_EmoteDefs.Count; ++index)
    {
      EmoteEntryDef emoteDef = this.m_cardDef.CardDef.m_EmoteDefs[index];
      EmoteEntry emoteEntry = new EmoteEntry(emoteDef.m_emoteType, emoteDef.m_emoteSpellPath, emoteDef.m_emoteSoundSpellPath, emoteDef.m_emoteGameStringKey, this);
      if (this.ShouldPreloadCardAssets())
      {
        emoteEntry.GetSoundSpell();
        emoteEntry.GetSpell();
      }
      this.m_emotes.Add(emoteEntry);
    }
  }

  private Spell SetupOverrideSpell(Spell existingSpell, Spell spell)
  {
    if ((UnityEngine.Object) existingSpell != (UnityEngine.Object) null)
    {
      if (existingSpell.IsActive())
        Log.Gameplay.PrintError("destroying active spell {0} currently in state {1} with source card {2}.", (object) existingSpell, (object) existingSpell.GetActiveState(), (object) existingSpell.GetSourceCard());
      UnityEngine.Object.Destroy((UnityEngine.Object) existingSpell.gameObject);
    }
    SpellUtils.SetupSpell(spell, (Component) this);
    return spell;
  }

  private void ReleaseAssets()
  {
    this.ReleaseCardDef();
    this.DestroyCardDefAssets();
  }

  private void ReleaseCardDef()
  {
    this.m_cardDef?.Dispose();
    this.m_cardDef = (DefLoader.DisposableCardDef) null;
  }

  private void DestroyCardDefAssets()
  {
    this.DestroyCardEffect(ref this.m_playEffect);
    this.DestroyCardEffect(ref this.m_attackEffect);
    this.DestroyCardEffect(ref this.m_deathEffect);
    this.DestroyCardEffect(ref this.m_lifetimeEffect);
    this.DestroyCardEffectList(ref this.m_subOptionEffects);
    this.DestroyCardEffectList(ref this.m_triggerEffects);
    this.DestroyCardEffectList(ref this.m_resetGameEffects);
    foreach (CardEffect cardEffect in this.m_spellTableOverrideEffects.Values)
      cardEffect.Clear();
    this.m_spellTableOverrideEffects.Clear();
    if (this.m_proxyEffects != null)
    {
      List<CardEffect> effects = new List<CardEffect>((IEnumerable<CardEffect>) this.m_proxyEffects.Values);
      this.DestroyCardEffectList(ref effects);
      this.m_proxyEffects.Clear();
    }
    this.DestroyCardEffect(ref this.m_customKeywordEffect);
    this.DestroyCardEffect(ref this.m_customChoiceRevealEffect);
    this.DestroyCardEffect(ref this.m_customChoiceConcealEffect);
    for (int index = 0; index < ((IEnumerable<CardSound>) this.m_announcerLine).Count<CardSound>(); ++index)
      this.DestroyCardSound(ref this.m_announcerLine[index]);
    this.DestroyEmoteList();
    this.ReleaseCardSpell(ref this.m_customSummonSpell);
    this.ReleaseCardSpell(ref this.m_customSpawnSpell);
    this.ReleaseCardSpell(ref this.m_customSpawnSpellOverride);
    this.ReleaseCardSpell(ref this.m_customDeathSpell);
    this.ReleaseCardSpell(ref this.m_customDeathSpellOverride);
    this.ReleaseCardSpell(ref this.m_customDiscardSpell);
    this.ReleaseCardSpell(ref this.m_customDiscardSpellOverride);
  }

  public void DestroyCardDefAssetsOnEntityChanged()
  {
    this.DeactivateLifetimeEffects();
    this.ReleaseCardSpell(ref this.m_customDeathSpell);
    this.DestroyCardEffect(ref this.m_lifetimeEffect);
  }

  private void DestroyCardEffect(ref CardEffect effect)
  {
    if (effect == null)
      return;
    effect.PurgeSpells();
    effect = (CardEffect) null;
  }

  private void DestroyCardSound(ref CardSound cardSound)
  {
    if (cardSound == null)
      return;
    cardSound.Clear();
    cardSound = (CardSound) null;
  }

  private void DestroyCardEffectList(ref List<CardEffect> effects)
  {
    if (effects == null)
      return;
    foreach (CardEffect cardEffect in effects)
      cardEffect.PurgeSpells();
    effects = (List<CardEffect>) null;
  }

  private void ReleaseCardSpell(ref Spell asset)
  {
    SpellManager spellManager = SpellManager.Get();
    if ((UnityEngine.Object) asset == (UnityEngine.Object) null || spellManager == null)
      return;
    spellManager.ReleaseSpell(asset);
  }

  private void DestroyEmoteList()
  {
    if (this.m_emotes == null)
      return;
    for (int index = 0; index < this.m_emotes.Count; ++index)
      this.m_emotes[index].Clear();
    this.m_emotes = (List<EmoteEntry>) null;
  }

  public void CancelActiveSpells()
  {
    SpellUtils.ActivateCancelIfNecessary(this.GetPlaySpell(0, false));
    if (this.m_subOptionEffects != null)
    {
      foreach (CardEffect subOptionEffect in this.m_subOptionEffects)
        SpellUtils.ActivateCancelIfNecessary(subOptionEffect.GetSpell(false));
    }
    if (this.m_triggerEffects == null)
      return;
    foreach (CardEffect triggerEffect in this.m_triggerEffects)
      SpellUtils.ActivateCancelIfNecessary(triggerEffect.GetSpell(false));
  }

  public void CancelCustomSpells()
  {
    SpellUtils.ActivateCancelIfNecessary(this.m_customSummonSpell);
    SpellUtils.ActivateCancelIfNecessary(this.m_customSpawnSpell);
    SpellUtils.ActivateCancelIfNecessary(this.m_customSpawnSpellOverride);
    SpellUtils.ActivateCancelIfNecessary(this.m_customDeathSpell);
    SpellUtils.ActivateCancelIfNecessary(this.m_customDeathSpellOverride);
    SpellUtils.ActivateCancelIfNecessary(this.m_customDiscardSpell);
    SpellUtils.ActivateCancelIfNecessary(this.m_customDiscardSpellOverride);
  }

  private IEnumerator WaitThenActivateSoundSpell(CardSoundSpell soundSpell)
  {
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    while (gameEntity.GetGameOptions().GetBooleanOption(GameEntityOption.DELAY_CARD_SOUND_SPELLS))
      yield return (object) null;
    soundSpell.Reactivate();
  }

  public void OnTagsChanged(TagDeltaList changeList, bool fromShowEntity)
  {
    bool flag = false;
    for (int index = 0; index < changeList.Count; ++index)
    {
      TagDelta change = changeList[index];
      switch ((GAME_TAG) change.tag)
      {
        case GAME_TAG.HEALTH:
        case GAME_TAG.ATK:
        case GAME_TAG.COST:
        case GAME_TAG.DURABILITY:
        case GAME_TAG.ARMOR:
        case GAME_TAG.HEALTH_DISPLAY:
        case GAME_TAG.ENABLE_HEALTH_DISPLAY:
        case GAME_TAG.HEALTH_DISPLAY_COLOR:
        case GAME_TAG.LETTUCE_ROLE:
        case GAME_TAG.LETTUCE_COOLDOWN_CONFIG:
        case GAME_TAG.LETTUCE_CURRENT_COOLDOWN:
          flag = true;
          break;
        default:
          this.OnTagChanged(change, fromShowEntity);
          break;
      }
    }
    if (!flag || this.m_entity.IsLoadingAssets() || !this.IsActorReady())
      return;
    this.UpdateActorComponents();
  }

  public void OnMetaData(Network.HistMetaData metaData)
  {
    if ((metaData.MetaType == HistoryMeta.Type.DAMAGE || metaData.MetaType == HistoryMeta.Type.HEALING || metaData.MetaType == HistoryMeta.Type.POISONOUS ? 1 : (metaData.MetaType == HistoryMeta.Type.CRITICAL_HIT ? 1 : 0)) == 0 || !this.CanShowActorVisuals() || this.m_entity.GetZone() != TAG_ZONE.PLAY)
      return;
    Spell actorSpell = this.GetActorSpell(SpellType.DAMAGE);
    if ((UnityEngine.Object) actorSpell == (UnityEngine.Object) null)
    {
      this.UpdateActorComponents();
    }
    else
    {
      actorSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_UpdateActorComponents));
      if (this.m_entity.IsCharacter())
      {
        int damage = metaData.MetaType == HistoryMeta.Type.HEALING ? -metaData.Data : metaData.Data;
        DamageSplatSpell damageSplatSpell = (DamageSplatSpell) actorSpell;
        damageSplatSpell.SetDamage(damage);
        if (metaData.MetaType == HistoryMeta.Type.POISONOUS)
        {
          if (damageSplatSpell.IsPoisonous())
            return;
          damageSplatSpell.SetPoisonous(true);
          damageSplatSpell.SetDamageIsCrit(false);
        }
        else if (metaData.MetaType == HistoryMeta.Type.CRITICAL_HIT)
        {
          damageSplatSpell.SetPoisonous(false);
          damageSplatSpell.SetDamageIsCrit(true);
        }
        else if (metaData.MetaType == HistoryMeta.Type.HEALING)
        {
          damageSplatSpell.SetPoisonous(false);
          damageSplatSpell.SetDamageIsCrit(false);
        }
        else
          damageSplatSpell.SetPoisonous(false);
        actorSpell.ActivateState(SpellStateType.ACTION);
        BoardEvents boardEvents = BoardEvents.Get();
        if (!((UnityEngine.Object) boardEvents != (UnityEngine.Object) null))
          return;
        if (metaData.MetaType == HistoryMeta.Type.HEALING)
          boardEvents.HealEvent(this, (float) -metaData.Data);
        else
          boardEvents.DamageEvent(this, (float) metaData.Data);
      }
      else
        actorSpell.Activate();
    }
  }

  public void HandleCardExhaustedTagChanged(TagDelta change)
  {
    if (this.m_entity.IsSecret())
    {
      if (!this.CanShowSecretActorVisuals())
        return;
    }
    else if (!this.CanShowActorVisuals())
      return;
    if (change.tag != 43)
      return;
    if (this.m_entity.IsHeroPower() && this.m_entity.GetController() != null && this.m_entity.GetController().GetTag(GAME_TAG.HERO_POWER_DISABLED) != 0)
      change.newValue = 1;
    if (change.newValue == change.oldValue)
      return;
    if (GameState.Get().IsTurnStartManagerActive() && this.m_entity.IsControlledByFriendlySidePlayer())
      TurnStartManager.Get().NotifyOfExhaustedChange(this, change);
    else
      this.ShowExhaustedChange(change.newValue);
  }

  public void OnTagChanged(TagDelta change, bool fromShowEntity)
  {
    if ((UnityEngine.Object) TagVisualConfiguration.Get() != (UnityEngine.Object) null)
      TagVisualConfiguration.Get().ProcessTagChange((GAME_TAG) change.tag, this, fromShowEntity, change);
    switch ((GAME_TAG) change.tag)
    {
      case GAME_TAG.TAG_SCRIPT_DATA_NUM_1:
        InputManager.Get().ForceRefreshTargetingArrowText();
        break;
      case GAME_TAG.STEALTH:
        if (this.m_entity.HasTaunt() && (UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
        {
          this.m_actor.ActivateTaunt();
          break;
        }
        break;
      case GAME_TAG.LETTUCE_ROLE:
        if (!this.CanShowActorVisuals())
          return;
        this.m_actor.UpdateAllComponents();
        break;
      case GAME_TAG.LETTUCE_IS_COMBAT_ACTION_TAKEN:
      case GAME_TAG.LETTUCE_ABILITY_TILE_VISUAL_SELF_ONLY:
      case GAME_TAG.LETTUCE_ABILITY_TILE_VISUAL_ALL_VISIBLE:
        if (!this.CanShowActorVisuals())
          return;
        if (GameState.Get().GetGameEntity() is LettuceMissionEntity gameEntity)
          gameEntity.UpdateAllMercenaryAbilityOrderBubbleText();
        using (List<int>.Enumerator enumerator = this.m_entity.GetLettuceAbilityEntityIDs().GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            int current = enumerator.Current;
            Card card = GameState.Get().GetEntity(current)?.GetCard();
            if ((UnityEngine.Object) card != (UnityEngine.Object) null && card.CanShowActorVisuals() && card.GetActor() is LettuceAbilityActor actor)
              actor.UpdateCheckMarkObject();
          }
          break;
        }
      case GAME_TAG.FAKE_ZONE:
      case GAME_TAG.FAKE_ZONE_POSITION:
        this.SetPredictedZonePosition(0);
        break;
      case GAME_TAG.TRADE_COST:
        this.DoOptionHighlight(GameState.Get());
        break;
      case GAME_TAG.LOCATION_ACTION_COOLDOWN:
        if (change.oldValue != 0 && change.newValue == 0 && this.m_entity.GetTag(GAME_TAG.EXHAUSTED) == 1)
        {
          this.ShowExhaustedChange(2);
          break;
        }
        break;
    }
    this.m_entity.GetCardTextBuilder().OnTagChange(this, change);
    if (!((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null))
      return;
    this.m_actor.UpdateDiamondCardArt();
  }

  public void ActivateDormantStateVisual()
  {
    this.m_actor.ActivateSpellBirthState(SpellType.DORMANT);
    if (this.m_entity.IsFrozen())
      this.m_actor.ActivateSpellDeathState(SpellType.FROZEN);
    if (this.m_entity.IsSilenced())
      this.m_actor.ActivateSpellDeathState(SpellType.SILENCE);
    this.DeactivateLifetimeEffects();
  }

  public void DeactivateDormantStateVisual()
  {
    this.m_actor.ActivateSpellDeathState(SpellType.DORMANT);
    if (this.m_entity.IsFrozen())
      this.m_actor.ActivateSpellBirthState(SpellType.FROZEN);
    if (this.m_entity.IsSilenced())
      this.m_actor.ActivateSpellBirthState(SpellType.SILENCE);
    this.ActivateLifetimeEffects();
    this.ActivateActorSpell(SpellType.AWAKEN_FROM_DORMANT);
    if (this.m_entity.IsControlledByFriendlySidePlayer())
    {
      if (this.m_entity.GetRealTimeSpellpower() > 0 || this.m_entity.GetRealTimeSpellpowerDouble())
        ZoneMgr.Get().OnSpellPowerEntityEnteredPlay(this.m_entity.GetSpellPowerSchool());
      if (this.m_entity.GetRealTimeHealingDoeDamageHint())
        ZoneMgr.Get().OnHealingDoesDamageEntityEnteredPlay();
      if (this.m_entity.GetRealTimeLifestealDoesDamageHint())
        ZoneMgr.Get().OnLifestealDoesDamageEntityEnteredPlay();
    }
    if (!this.m_entity.IsAsleep())
      return;
    this.m_actor.ActivateSpellBirthState(SpellType.Zzz);
  }

  public void UpdateQuestUI()
  {
    if (this.m_entity == null || !this.m_entity.IsQuest() || (UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    QuestController component = this.m_actor.GetComponent<QuestController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      Log.Gameplay.PrintError("Quest card {0} does not have a QuestController component.", (object) this);
    else
      component.UpdateQuestUI();
  }

  public void UpdateQuestlineUI()
  {
    if (this.m_entity == null || !this.m_entity.IsQuestline() || (UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    QuestlineController component = this.m_actor.GetComponent<QuestlineController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      Log.Gameplay.PrintError("Questline card {0} does not have a QuestlineController component.", (object) this);
    else
      component.UpdateQuestlineUI();
  }

  public void UpdateSideQuestUI(bool allowQuestComplete)
  {
    if (this.m_entity == null || !this.m_entity.IsSideQuest() || (UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    SideQuestController component = this.m_actor.GetComponent<SideQuestController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      Log.Gameplay.PrintError("SideQuest card {0} does not have a SideQuestController component.", (object) this);
    else
      component.UpdateQuestUI(allowQuestComplete);
  }

  public void UpdatePuzzleUI()
  {
    if (this.m_entity == null || !this.m_entity.IsPuzzle() || (UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    PuzzleController component = this.m_actor.GetComponent<PuzzleController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      Log.Gameplay.PrintError("Puzzle card {0} does not have a PuzzleController component.", (object) this);
    else
      component.UpdatePuzzleUI();
  }

  public void UpdateCardCostHealth(TagDelta change)
  {
    if (change.oldValue == change.newValue)
      return;
    if (this.m_entity.IsControlledByFriendlySidePlayer())
    {
      Card mousedOverCard = InputManager.Get().GetMousedOverCard();
      if ((UnityEngine.Object) mousedOverCard != (UnityEngine.Object) null)
      {
        Entity entity = mousedOverCard.GetEntity();
        if (entity == this.m_entity)
        {
          if (change.newValue > 0)
            ManaCrystalMgr.Get().CancelAllProposedMana(entity);
          else
            ManaCrystalMgr.Get().ProposeManaCrystalUsage(entity);
        }
      }
    }
    if (this.CanShowActorVisuals() && change.newValue > 0)
      this.m_actor.ActivateSpellBirthState(SpellType.SPELLS_COST_HEALTH);
    else
      this.m_actor.ActivateSpellDeathState(SpellType.SPELLS_COST_HEALTH);
  }

  public void UpdateCardCostArmor(TagDelta change)
  {
    if (change.oldValue == change.newValue)
      return;
    if (this.m_entity.IsControlledByFriendlySidePlayer())
    {
      Card mousedOverCard = InputManager.Get().GetMousedOverCard();
      if ((UnityEngine.Object) mousedOverCard != (UnityEngine.Object) null)
      {
        Entity entity = mousedOverCard.GetEntity();
        if (entity == this.m_entity)
        {
          if (change.newValue > 0)
            ManaCrystalMgr.Get().CancelAllProposedMana(entity);
          else
            ManaCrystalMgr.Get().ProposeManaCrystalUsage(entity);
        }
      }
    }
    if (this.CanShowActorVisuals() && change.newValue > 0)
      this.m_actor.ActivateSpellBirthState(SpellType.COST_ARMOR);
    else
      this.m_actor.ActivateSpellDeathState(SpellType.COST_ARMOR);
  }

  public bool CanShowActorVisuals() => !this.m_entity.IsLoadingAssets() && !((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null) && this.m_actor.IsShown();

  private bool CanShowSecretActorVisuals() => !this.m_entity.IsLoadingAssets() && !((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null) && (!this.m_actorReady || this.m_actor.IsShown());

  public bool ShouldShowImmuneVisuals() => this.m_entity != null && this.m_entity.HasTag(GAME_TAG.IMMUNE) && !this.m_entity.HasTag(GAME_TAG.DONT_SHOW_IMMUNE);

  public void ActivateStateSpells(bool forceActivate = false)
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null || this.m_entity.GetController() != null && !this.m_entity.GetController().IsFriendlySide() && this.m_entity.IsObfuscated())
      return;
    if (this.m_entity != null && this.m_entity.IsHeroPower())
      this.UpdateHeroPowerRelatedVisual();
    TagVisualConfiguration.Get().ActivateStateSpells(this);
    TAG_ZONE tagZone = (UnityEngine.Object) this.GetZone() != (UnityEngine.Object) null ? this.GetZone().m_ServerTag : TAG_ZONE.SETASIDE;
    if (tagZone == TAG_ZONE.HAND)
      this.ActivateHandStateSpells(forceActivate);
    else if (this.m_entity != null && (tagZone == TAG_ZONE.PLAY || tagZone == TAG_ZONE.SECRET))
    {
      bool exhausted = this.m_entity.IsExhausted();
      if (this.m_entity.IsHeroPower() && this.m_entity.GetController() != null && this.m_entity.GetController().HasTag(GAME_TAG.HERO_POWER_DISABLED))
        exhausted = true;
      this.ShowExhaustedChange(exhausted);
    }
    if (tagZone != TAG_ZONE.PLAY || !this.m_entity.IsLettuceAbility())
      return;
    this.ShowExhaustedChange(false);
  }

  public void UpdateHeroPowerRelatedVisual()
  {
    if (!this.m_entity.IsHeroPower())
      return;
    Player controller = this.m_entity.GetController();
    if (controller == null)
      return;
    if (controller.HasTag(GAME_TAG.STEADY_SHOT_CAN_TARGET) && this.m_entity.HasClass(TAG_CLASS.HUNTER))
      this.m_actor.ActivateSpellBirthState(SpellType.STEADY_SHOT_CAN_TARGET);
    else
      this.m_actor.ActivateSpellDeathState(SpellType.STEADY_SHOT_CAN_TARGET);
    if (controller.HasTag(GAME_TAG.CURRENT_HEROPOWER_DAMAGE_BONUS) && controller.IsHeroPowerAffectedByBonusDamage())
      this.m_actor.ActivateSpellBirthState(SpellType.CURRENT_HEROPOWER_DAMAGE_BONUS);
    else
      this.m_actor.ActivateSpellDeathState(SpellType.CURRENT_HEROPOWER_DAMAGE_BONUS);
  }

  public void ActivateHandStateSpells(bool forceActivate = false)
  {
    this.m_entity.GetController();
    if ((this.m_entity.IsCardButton() || this.m_entity.IsSpell()) && this.m_playEffect != null)
      SpellUtils.ActivateCancelIfNecessary(this.m_playEffect.GetSpell(false));
    if (this.m_entity.IsSpell())
      SpellUtils.ActivateCancelIfNecessary(this.GetActorSpell(SpellType.POWER_UP, false));
    if (!((UnityEngine.Object) TagVisualConfiguration.Get() != (UnityEngine.Object) null))
      return;
    TagVisualConfiguration.Get().ActivateHandStateSpells(this, forceActivate);
  }

  public void DeactivateHandStateSpells(Actor actor = null)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
        return;
      actor = this.m_actor;
    }
    if ((UnityEngine.Object) TagVisualConfiguration.Get() != (UnityEngine.Object) null)
      TagVisualConfiguration.Get().DeactivateHandStateSpells(this, actor);
    if (actor.UseTechLevelManaGem())
      actor.ReleaseSpell(SpellType.TECH_LEVEL_MANA_GEM);
    if (actor.UseCoinManaGem())
      actor.ReleaseSpell(SpellType.COIN_MANA_GEM);
    if (!((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null) || !this.m_questRewardActor.UseCoinManaGem())
      return;
    this.m_questRewardActor.ReleaseSpell(SpellType.COIN_MANA_GEM);
  }

  public void ActivateActorArmsDealingSpell()
  {
    if (this.CardStandInIsInteractive())
    {
      PowerTaskList currentTaskList = GameState.Get().GetPowerProcessor().GetCurrentTaskList();
      if (currentTaskList != null && currentTaskList.IsBlock())
        this.StartCoroutine(this.WaitPowerTaskListAndActivateArmsDealing(currentTaskList));
      else
        this.m_actor.ActivateSpellBirthState(SpellType.ARMS_DEALING);
    }
    else
    {
      Spell spell = this.m_actor.GetSpell(SpellType.ARMS_DEALING);
      if (!((UnityEngine.Object) spell != (UnityEngine.Object) null))
        return;
      spell.ActivateState(SpellStateType.IDLE);
    }
  }

  private IEnumerator WaitPowerTaskListAndActivateArmsDealing(
    PowerTaskList curPowerTaskList)
  {
    while (!curPowerTaskList.IsComplete())
      yield return (object) null;
    if (this.GetZone() is ZoneHand)
      this.m_actor.ActivateSpellBirthState(SpellType.ARMS_DEALING);
  }

  public void ToggleDeathrattle(bool on)
  {
    if (on)
      this.m_actor.ActivateSpellBirthState(SpellType.DEATHRATTLE_IDLE);
    else
      this.m_actor.ActivateSpellDeathState(SpellType.DEATHRATTLE_IDLE);
  }

  public void UpdateBauble()
  {
    if (this.IsBaubleAnimating())
      return;
    this.DeactivateBaubles();
    SpellType prioritizedBaubleSpellType = this.m_entity.GetPrioritizedBaubleSpellType();
    if (prioritizedBaubleSpellType == SpellType.NONE || !((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null))
      return;
    Spell spell = this.m_actor.GetSpell(prioritizedBaubleSpellType);
    if (!((UnityEngine.Object) spell != (UnityEngine.Object) null))
      return;
    spell.ClearPositionDirtyFlag();
    spell.ActivateState(SpellStateType.BIRTH);
    if (prioritizedBaubleSpellType != SpellType.AVENGE)
      return;
    spell.SetSource(this.gameObject);
  }

  public void DeactivateBaubles()
  {
    SpellType prioritizedBaubleSpellType = this.m_entity.GetPrioritizedBaubleSpellType();
    SpellType[] spellTypeArray = new SpellType[11]
    {
      SpellType.TRIGGER,
      SpellType.FAST_TRIGGER,
      SpellType.POISONOUS,
      SpellType.POISONOUS_INSTANT,
      SpellType.INSPIRE,
      SpellType.LIFESTEAL,
      SpellType.OVERKILL,
      SpellType.SPELLBURST,
      SpellType.FRENZY,
      SpellType.AVENGE,
      SpellType.HONORABLEKILL
    };
    foreach (SpellType spellType in spellTypeArray)
    {
      if (prioritizedBaubleSpellType != spellType)
        SpellUtils.ActivateDeathIfNecessary(this.GetActorSpell(spellType, false));
    }
  }

  public bool IsBaubleAnimating() => this.m_isBaubleAnimating;

  public void SetIsBaubleAnimating(bool isAnimating) => this.m_isBaubleAnimating = isAnimating;

  public void ShowExhaustedChange(int val)
  {
    if (this.m_entity.IsLocation())
    {
      if (this.m_entity.GetCurrentHealth() <= 0)
        return;
      if (this.m_entity.GetTag(GAME_TAG.EXHAUSTED) == 1 && this.m_entity.GetTag(GAME_TAG.LOCATION_ACTION_COOLDOWN) == 0)
        val = 2;
      this.StartCoroutine(this.PlayLocationAnimation(val));
    }
    else
      this.ShowExhaustedChange(val == 1);
  }

  public void ShowExhaustedChange(bool exhausted)
  {
    if (this.m_entity.IsHeroPower())
    {
      this.StopCoroutine("PlayHeroPowerAnimation");
      this.StartCoroutine("PlayHeroPowerAnimation", (object) exhausted);
    }
    else if (this.m_entity.IsWeapon() || this.m_entity.IsBattlegroundHeroBuddy())
    {
      if (exhausted)
        this.SheatheWeaponOrHeroBuddy();
      else
        this.UnSheatheWeaponOrHeroBuddy();
    }
    else
    {
      if (!this.m_entity.IsSecret())
        return;
      this.StartCoroutine(this.ShowSecretExhaustedChange(exhausted));
    }
  }

  public void DisableHeroPowerFlipSoundOnce() => this.m_disableHeroPowerFlipSoundOnce = true;

  private IEnumerator PlayHeroPowerAnimation(bool exhausted)
  {
    Card card = this;
    string animationName;
    if (exhausted)
    {
      animationName = (bool) UniversalInputManager.UsePhoneUI ? "HeroPower_Used_phone" : "HeroPower_Used";
      if ((UnityEngine.Object) card.m_actor != (UnityEngine.Object) null && card.m_actor.UseCoinManaGem())
      {
        Spell spellIfLoaded = card.m_actor.GetSpellIfLoaded(SpellType.COIN_MANA_GEM);
        if ((UnityEngine.Object) spellIfLoaded != (UnityEngine.Object) null)
          spellIfLoaded.Deactivate();
      }
    }
    else
    {
      animationName = (bool) UniversalInputManager.UsePhoneUI ? "HeroPower_Restore_phone" : "HeroPower_Restore";
      if ((UnityEngine.Object) card.m_actor != (UnityEngine.Object) null && card.m_actor.UseCoinManaGem())
      {
        Spell spellIfLoaded = card.m_actor.GetSpellIfLoaded(SpellType.COIN_MANA_GEM);
        if ((UnityEngine.Object) spellIfLoaded != (UnityEngine.Object) null)
          spellIfLoaded.Reactivate();
      }
    }
    card.SetInputEnabled(false);
    MinionShake shake = card.m_actor.gameObject.GetComponentInChildren<MinionShake>();
    if (!((UnityEngine.Object) shake == (UnityEngine.Object) null))
    {
      while (shake.isShaking())
        yield return (object) null;
      while ((UnityEngine.Object) card.m_actor.gameObject.transform.parent != (UnityEngine.Object) card.transform)
        yield return (object) null;
      if (card.m_disableHeroPowerFlipSoundOnce)
      {
        card.m_disableHeroPowerFlipSoundOnce = false;
      }
      else
      {
        string assetRef = exhausted ? "hero_power_icon_flip_off.prefab:621ead6ff672f5b4bbfd6578ee217a42" : "hero_power_icon_flip_on.prefab:e1491b367801f6b4395dc63ce0b08f0a";
        SoundManager.Get().LoadAndPlay((AssetReference) assetRef);
      }
      card.m_actor.GetComponent<Animation>().Play(animationName);
      Spell spell = card.GetPlaySpell(0);
      if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
      {
        while (spell.GetActiveState() != SpellStateType.NONE)
          yield return (object) null;
      }
      card.SetInputEnabled(true);
      if (exhausted && GameState.Get().IsValidOption(card.m_entity) && !card.m_entity.HasSubCards() && (UnityEngine.Object) spell != (UnityEngine.Object) null)
        card.SetInputEnabled(false);
    }
  }

  private IEnumerator PlayLocationAnimation(int stateVal)
  {
    Card card = this;
    string animationName;
    switch (stateVal)
    {
      case 0:
        animationName = "Location_AjarToOpen";
        card.PlayFSMSoundEvent("Ajar_To_Open");
        break;
      case 1:
        animationName = "Location_OpenToClose";
        card.PlayFSMSoundEvent("Open_To_Close");
        break;
      default:
        animationName = "Location_ClosedToAjar";
        card.PlayFSMSoundEvent("Close_To_Ajar");
        break;
    }
    card.SetInputEnabled(false);
    MinionShake shake = card.m_actor.gameObject.GetComponentInChildren<MinionShake>();
    if (!((UnityEngine.Object) shake == (UnityEngine.Object) null))
    {
      while (shake.isShaking())
        yield return (object) null;
      while ((UnityEngine.Object) card.m_actor.gameObject.transform.parent != (UnityEngine.Object) card.transform)
        yield return (object) null;
      if (card.m_disableHeroPowerFlipSoundOnce)
        card.m_disableHeroPowerFlipSoundOnce = false;
      card.m_actor.GetComponent<Animation>().Play(animationName);
      Spell spell = card.GetPlaySpell(0);
      if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
      {
        while (spell.GetActiveState() != SpellStateType.NONE)
          yield return (object) null;
      }
      card.SetInputEnabled(true);
      if (stateVal != 1 && GameState.Get().IsValidOption(card.m_entity) && !card.m_entity.HasSubCards() && (UnityEngine.Object) spell != (UnityEngine.Object) null)
        card.SetInputEnabled(false);
    }
  }

  private void PlayFSMSoundEvent(string fsmevent)
  {
    Actor actor = this.GetActor();
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    GameObject rootObject = actor.GetRootObject();
    if ((UnityEngine.Object) rootObject == (UnityEngine.Object) null)
      return;
    PlayMakerFSM component = rootObject.GetComponent<PlayMakerFSM>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    component.SendEvent(fsmevent);
  }

  private void SheatheWeaponOrHeroBuddy()
  {
    if (this.GetZone() is ZoneWeapon)
    {
      this.m_actor.GetAttackObject().ScaleToZero();
      this.ActivateActorSpell(SpellType.SHEATHE);
    }
    else if (this.GetZone() is ZoneBattlegroundHeroBuddy)
    {
      if (!GameState.Get().IsMulliganManagerActive())
        this.ActivateActorSpell(SpellType.SHEATHE);
    }
    else if (!((UnityEngine.Object) this.GetZone() == (UnityEngine.Object) null) && !(this.GetZone() is ZoneGraveyard))
      Log.Gameplay.PrintError("Failed to process Card.SheatheWeapon() card:{0} zone:{1}", (object) this, (object) this.GetZone());
    Player controller = this.GetController();
    if (controller == null)
      return;
    Card heroCard = controller.GetHeroCard();
    if (!((UnityEngine.Object) heroCard != (UnityEngine.Object) null))
      return;
    heroCard.NotifyOfWeaponSheathed(this.m_entity);
  }

  private void UnSheatheWeaponOrHeroBuddy()
  {
    if (this.GetZone() is ZoneWeapon)
    {
      this.m_actor.GetAttackObject().Enlarge(1f);
      this.ActivateActorSpell(SpellType.UNSHEATHE);
    }
    else if (this.GetZone() is ZoneBattlegroundHeroBuddy)
    {
      if (!GameState.Get().IsMulliganManagerActive())
        this.ActivateActorSpell(SpellType.UNSHEATHE);
    }
    else if (!((UnityEngine.Object) this.GetZone() == (UnityEngine.Object) null) && !(this.GetZone() is ZoneGraveyard))
      Log.Gameplay.PrintError("Failed to process Card.UnSheatheWeapon() card:{0} zone:{1}", (object) this, (object) this.GetZone());
    Player controller = this.GetController();
    if (controller == null)
      return;
    Card heroCard = controller.GetHeroCard();
    if (!((UnityEngine.Object) heroCard != (UnityEngine.Object) null))
      return;
    heroCard.NotifyOfWeaponUnsheathed(this.m_entity);
  }

  private IEnumerator ShowSecretExhaustedChange(bool exhausted)
  {
    while (!this.m_actorReady)
      yield return (object) null;
    if (!this.m_entity.IsDarkWandererSecret())
    {
      Spell spell = this.m_actor.GetComponent<Spell>();
      while (spell.GetActiveState() != SpellStateType.NONE)
        yield return (object) null;
      if (this.CanShowSecretZoneCard())
      {
        if (exhausted)
          this.SheatheSecret(spell);
        else
          this.UnSheatheSecret(spell);
      }
    }
  }

  private void SheatheSecret(Spell spell)
  {
    if (this.m_secretSheathed || !this.m_entity.IsExhausted())
      return;
    this.m_secretSheathed = true;
    spell.ActivateState(SpellStateType.IDLE);
  }

  private void UnSheatheSecret(Spell spell)
  {
    if (!this.m_secretSheathed || this.m_entity.IsExhausted())
      return;
    this.m_secretSheathed = false;
    spell.ActivateState(SpellStateType.DEATH);
  }

  public void OnEnchantmentAdded(int oldEnchantmentCount, Entity enchantment)
  {
    if (this.CanShowActorVisuals() && this.IsActorReady())
      this.UpdateBauble();
    Spell spell = (Spell) null;
    if (GameState.Get() != null && GameState.Get().GetGameEntity() != null && GameState.Get().GetBooleanGameOption(GameEntityOption.ALLOW_ENCHANTMENT_SPARKLES))
    {
      switch (enchantment.GetEnchantmentBirthVisual())
      {
        case TAG_ENCHANTMENT_VISUAL.POSITIVE:
          spell = this.GetActorSpell(SpellType.ENCHANT_POSITIVE);
          break;
        case TAG_ENCHANTMENT_VISUAL.NEGATIVE:
          spell = this.GetActorSpell(SpellType.ENCHANT_NEGATIVE);
          break;
        case TAG_ENCHANTMENT_VISUAL.NEUTRAL:
          spell = this.GetActorSpell(SpellType.ENCHANT_NEUTRAL);
          break;
      }
    }
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      this.UpdateEnchantments();
      this.UpdateTooltip();
    }
    else
    {
      spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnEnchantmentSpellStateFinished));
      spell.ActivateState(SpellStateType.BIRTH);
    }
  }

  public void OnEnchantmentRemoved(int oldEnchantmentCount, Entity enchantment)
  {
    if (this.CanShowActorVisuals())
      this.UpdateBauble();
    Spell spell = (Spell) null;
    if (GameState.Get() != null && GameState.Get().GetGameEntity() != null && GameState.Get().GetBooleanGameOption(GameEntityOption.ALLOW_ENCHANTMENT_SPARKLES))
    {
      switch (enchantment.GetEnchantmentBirthVisual())
      {
        case TAG_ENCHANTMENT_VISUAL.POSITIVE:
          spell = this.GetActorSpell(SpellType.ENCHANT_POSITIVE);
          break;
        case TAG_ENCHANTMENT_VISUAL.NEGATIVE:
          spell = this.GetActorSpell(SpellType.ENCHANT_NEGATIVE);
          break;
        case TAG_ENCHANTMENT_VISUAL.NEUTRAL:
          spell = this.GetActorSpell(SpellType.ENCHANT_NEUTRAL);
          break;
      }
    }
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      this.UpdateEnchantments();
      this.UpdateTooltip();
    }
    else
    {
      spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnEnchantmentSpellStateFinished));
      spell.ActivateState(SpellStateType.DEATH);
    }
  }

  private void OnEnchantmentSpellStateFinished(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (prevStateType != SpellStateType.BIRTH && prevStateType != SpellStateType.DEATH)
      return;
    spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnEnchantmentSpellStateFinished));
    this.UpdateEnchantments();
    this.UpdateTooltip();
  }

  public void UpdateEnchantments()
  {
    if (GameState.Get() != null && GameState.Get().GetGameEntity() != null && !GameState.Get().GetBooleanGameOption(GameEntityOption.ALLOW_ENCHANTMENT_SPARKLES))
      return;
    List<Entity> enchantments = this.m_entity.GetEnchantments();
    Spell actorSpell1 = this.GetActorSpell(SpellType.ENCHANT_POSITIVE);
    Spell actorSpell2 = this.GetActorSpell(SpellType.ENCHANT_NEGATIVE);
    Spell actorSpell3 = this.GetActorSpell(SpellType.ENCHANT_NEUTRAL);
    Spell spell1 = (Spell) null;
    if ((UnityEngine.Object) actorSpell1 != (UnityEngine.Object) null && actorSpell1.GetActiveState() == SpellStateType.IDLE)
      spell1 = actorSpell1;
    else if ((UnityEngine.Object) actorSpell2 != (UnityEngine.Object) null && actorSpell2.GetActiveState() == SpellStateType.IDLE)
      spell1 = actorSpell2;
    else if ((UnityEngine.Object) actorSpell3 != (UnityEngine.Object) null && actorSpell3.GetActiveState() == SpellStateType.IDLE)
      spell1 = actorSpell3;
    if (enchantments.Count == 0)
    {
      if (!((UnityEngine.Object) spell1 != (UnityEngine.Object) null))
        return;
      spell1.ActivateState(SpellStateType.DEATH);
    }
    else
    {
      int num = 0;
      bool flag = false;
      foreach (Entity entity in enchantments)
      {
        TAG_ENCHANTMENT_VISUAL enchantmentIdleVisual = entity.GetEnchantmentIdleVisual();
        if (enchantmentIdleVisual == TAG_ENCHANTMENT_VISUAL.POSITIVE)
          ++num;
        else if (enchantmentIdleVisual == TAG_ENCHANTMENT_VISUAL.NEGATIVE)
          --num;
        if (enchantmentIdleVisual != TAG_ENCHANTMENT_VISUAL.INVALID)
          flag = true;
      }
      Spell spell2 = (Spell) null;
      if (num > 0)
        spell2 = actorSpell1;
      else if (num < 0)
        spell2 = actorSpell2;
      else if (flag)
        spell2 = actorSpell3;
      if ((UnityEngine.Object) spell1 != (UnityEngine.Object) null && (UnityEngine.Object) spell1 != (UnityEngine.Object) spell2)
        spell1.Deactivate();
      if (!((UnityEngine.Object) spell2 != (UnityEngine.Object) null))
        return;
      spell2.ActivateState(SpellStateType.BIRTH);
    }
  }

  public Spell GetActorSpell(SpellType spellType, bool loadIfNeeded = true) => (UnityEngine.Object) this.m_actor == (UnityEngine.Object) null ? (Spell) null : (!loadIfNeeded ? this.m_actor.GetSpellIfLoaded(spellType) : this.m_actor.GetSpell(spellType));

  public Spell ActivateActorSpell(SpellType spellType) => this.ActivateActorSpell(this.m_actor, spellType, (Spell.FinishedCallback) null, (Spell.StateFinishedCallback) null);

  public Spell ActivateActorSpell(SpellType spellType, Spell.FinishedCallback finishedCallback) => this.ActivateActorSpell(this.m_actor, spellType, finishedCallback, (Spell.StateFinishedCallback) null);

  public Spell ActivateActorSpell(
    SpellType spellType,
    Spell.FinishedCallback finishedCallback,
    Spell.StateFinishedCallback stateFinishedCallback)
  {
    return this.ActivateActorSpell(this.m_actor, spellType, finishedCallback, stateFinishedCallback);
  }

  private Spell ActivateActorSpell(Actor actor, SpellType spellType) => this.ActivateActorSpell(actor, spellType, (Spell.FinishedCallback) null, (Spell.StateFinishedCallback) null);

  private Spell ActivateActorSpell(
    Actor actor,
    SpellType spellType,
    Spell.FinishedCallback finishedCallback)
  {
    return this.ActivateActorSpell(actor, spellType, finishedCallback, (Spell.StateFinishedCallback) null);
  }

  private Spell ActivateActorSpell(
    Actor actor,
    SpellType spellType,
    Spell.FinishedCallback finishedCallback,
    Spell.StateFinishedCallback stateFinishedCallback)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      Log.Gameplay.Print(string.Format("{0}.ActivateActorSpell() - actor IS NULL spellType={1}", (object) this, (object) spellType));
      return (Spell) null;
    }
    Spell spell = actor.GetSpell(spellType);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      Log.Gameplay.Print(string.Format("{0}.ActivateActorSpell() - spell IS NULL actor={1} spellType={2}", (object) this, (object) actor, (object) spellType));
      return (Spell) null;
    }
    this.ActivateSpell(spell, finishedCallback, stateFinishedCallback);
    return spell;
  }

  private void ActivateSpell(Spell spell, Spell.FinishedCallback finishedCallback) => this.ActivateSpell(spell, finishedCallback, (object) null, (Spell.StateFinishedCallback) null, (object) null);

  private void ActivateSpell(
    Spell spell,
    Spell.FinishedCallback finishedCallback,
    Spell.StateFinishedCallback stateFinishedCallback)
  {
    this.ActivateSpell(spell, finishedCallback, (object) null, stateFinishedCallback, (object) null);
  }

  private void ActivateSpell(
    Spell spell,
    Spell.FinishedCallback finishedCallback,
    object finishedUserData,
    Spell.StateFinishedCallback stateFinishedCallback)
  {
    this.ActivateSpell(spell, finishedCallback, finishedUserData, stateFinishedCallback, (object) null);
  }

  private void ActivateSpell(
    Spell spell,
    Spell.FinishedCallback finishedCallback,
    object finishedUserData,
    Spell.StateFinishedCallback stateFinishedCallback,
    object stateFinishedUserData)
  {
    if (finishedCallback != null)
      spell.AddFinishedCallback(finishedCallback, finishedUserData);
    if (stateFinishedCallback != null)
      spell.AddStateFinishedCallback(stateFinishedCallback, stateFinishedUserData);
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    spell.Activate();
  }

  public Spell GetActorAttackSpellForInput()
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
    {
      Log.Gameplay.Print("{0}.GetActorAttackSpellForInput() - m_actor IS NULL", (object) this);
      return (Spell) null;
    }
    if ((UnityEngine.Object) this.m_zone == (UnityEngine.Object) null)
    {
      Log.Gameplay.Print("{0}.GetActorAttackSpellForInput() - m_zone IS NULL", (object) this);
      return (Spell) null;
    }
    Spell spell = this.m_actor.GetSpell(SpellType.FRIENDLY_ATTACK);
    if (!((UnityEngine.Object) spell == (UnityEngine.Object) null))
      return spell;
    Log.Gameplay.Print("{0}.GetActorAttackSpellForInput() - {1} spell is null", (object) this, (object) SpellType.FRIENDLY_ATTACK);
    return (Spell) null;
  }

  public void FakeDeath()
  {
    if (!this.m_suppressKeywordDeaths)
      this.StartCoroutine(this.WaitAndPrepareForDeathAnimation(this.m_actor));
    this.ActivateDeathSpell(this.m_actor);
  }

  private Spell ActivateDeathSpell(Actor actor)
  {
    bool standard;
    Spell bestDeathSpell = this.GetBestDeathSpell(actor, out standard);
    if ((UnityEngine.Object) bestDeathSpell == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0}.ActivateDeathSpell() - {1} is null", (object) this, (object) SpellType.DEATH));
      return (Spell) null;
    }
    this.CleanUpCustomSpell(bestDeathSpell, ref this.m_customDeathSpell);
    this.CleanUpCustomSpell(bestDeathSpell, ref this.m_customDeathSpellOverride);
    ++this.m_activeDeathEffectCount;
    if (standard)
    {
      if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) actor)
        bestDeathSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
    }
    else
    {
      bestDeathSpell.SetSource(this.gameObject);
      if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) actor)
        bestDeathSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_CustomDeath));
      SpellUtils.SetCustomSpellParent(bestDeathSpell, (Component) actor);
    }
    bestDeathSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_Death));
    bestDeathSpell.Activate();
    BoardEvents boardEvents = BoardEvents.Get();
    if ((UnityEngine.Object) boardEvents != (UnityEngine.Object) null)
      boardEvents.DeathEvent(this);
    return bestDeathSpell;
  }

  private Spell ActivateHandSpawnSpell()
  {
    if ((UnityEngine.Object) this.m_customSpawnSpellOverride == (UnityEngine.Object) null)
      return this.ActivateDefaultSpawnSpell(new Spell.FinishedCallback(this.OnSpellFinished_DefaultHandSpawn));
    Entity creator = this.m_entity.GetCreator();
    Card creatorCard = (Card) null;
    if (creator != null && creator.IsMinion())
      creatorCard = creator.GetCard();
    if ((UnityEngine.Object) creatorCard != (UnityEngine.Object) null)
      TransformUtil.CopyWorld((Component) this.transform, (Component) creatorCard.transform);
    this.ActivateCustomHandSpawnSpell(this.m_customSpawnSpellOverride, creatorCard);
    return this.m_customSpawnSpellOverride;
  }

  private void ActivatePlaySpawnEffects_HeroPowerOrWeaponOrHeroBuddy()
  {
    Spell spell = this.m_customSpawnSpellOverride;
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      spell = this.m_customSpawnSpell;
      if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      {
        this.ActivateDefaultSpawnSpell(new Spell.FinishedCallback(this.OnSpellFinished_DefaultPlaySpawn));
        return;
      }
    }
    if (this.m_zone is ZoneHeroPower)
      this.m_actor.Hide();
    this.ActivateCustomSpawnSpell(spell);
  }

  private Spell ActivateDefaultSpawnSpell(Spell.FinishedCallback finishedCallback)
  {
    this.m_inputEnabled = false;
    this.m_actor.ToggleForceIdle(true);
    int tag = this.m_entity.GetTag(GAME_TAG.PREMIUM);
    SpellType spellType = SpellType.SUMMON_IN;
    if (tag == 2)
      spellType = SpellType.SUMMON_IN_DIAMOND;
    if (this.m_zone is ZoneHand && this.m_entity.HasTag(GAME_TAG.GHOSTLY))
      spellType = SpellType.GHOSTLY_SUMMON_IN;
    else if (this.m_zone is ZoneHand && this.m_entity.HasTag(GAME_TAG.CREATOR))
    {
      Entity entity = GameState.Get().GetEntity(this.m_entity.GetTag(GAME_TAG.CREATOR));
      if (entity != null && entity.IsTwinspell() && entity.GetTag(GAME_TAG.TWINSPELL_COPY) == GameUtils.TranslateCardIdToDbId(this.m_entity.GetCardId()))
        spellType = GameState.Get().GetGameEntity().GetTag(GAME_TAG.USE_FAST_ACTOR_TRANSITION_ANIMATIONS) > 0 ? SpellType.TWINSPELL_SUMMON_IN_FAST : SpellType.TWINSPELL_SUMMON_IN;
    }
    else if (this.m_entity.IsWeapon() && (this.m_zone is ZoneWeapon || this.m_zone is ZoneHeroPower) || this.m_entity.IsBattlegroundHeroBuddy() && this.m_zone is ZoneBattlegroundHeroBuddy || this.m_entity.IsBattlegroundQuestReward() && this.m_zone is ZoneBattlegroundQuestReward)
      spellType = this.m_entity.IsControlledByFriendlySidePlayer() ? SpellType.SUMMON_IN_FRIENDLY : SpellType.SUMMON_IN_OPPONENT;
    if (this.m_zone is ZoneHand && this.m_entity.IsMercenary())
      spellType = SpellType.SUMMON_IN_MERCENARY;
    if (this.m_zone is ZoneHeroPower && this.m_entity.IsHeroPower() && this.m_actor.UseCoinManaGem())
      this.m_actor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
    Spell spell = this.ActivateActorSpell(spellType, finishedCallback);
    if (!((UnityEngine.Object) spell == (UnityEngine.Object) null))
      return spell;
    Debug.LogError((object) string.Format("{0}.ActivateDefaultSpawnSpell() - {1} is null", (object) this, (object) spellType));
    return (Spell) null;
  }

  private void ActivateCustomSpawnSpell(Spell spell)
  {
    spell.SetSource(this.gameObject);
    spell.RemoveAllTargets();
    spell.AddTarget(this.gameObject);
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_ReleaseSpell));
    SpellUtils.SetCustomSpellParent(spell, (Component) this.m_actor);
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_CustomPlaySpawn));
    spell.Activate();
  }

  private void ActivateCustomHandSpawnSpell(Spell spell, Card creatorCard)
  {
    GameObject go = (UnityEngine.Object) creatorCard == (UnityEngine.Object) null ? this.gameObject : creatorCard.gameObject;
    spell.SetSource(go);
    spell.RemoveAllTargets();
    spell.AddTarget(this.gameObject);
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_ReleaseSpell));
    SpellUtils.SetCustomSpellParent(spell, (Component) this.m_actor);
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_CustomHandSpawn));
    spell.Activate();
  }

  private void ActivateMinionSpawnEffects()
  {
    Entity creator = this.m_entity.GetCreator();
    Card creatorCard = (Card) null;
    if (creator != null && creator.IsMinion())
      creatorCard = creator.GetCard();
    if ((UnityEngine.Object) creatorCard != (UnityEngine.Object) null && !(creatorCard.GetZone() is ZonePlay) && !(creatorCard.GetZone() is ZoneGraveyard))
      creatorCard = (Card) null;
    if ((UnityEngine.Object) creatorCard != (UnityEngine.Object) null)
      TransformUtil.CopyWorld((Component) this.transform, (Component) creatorCard.transform);
    bool standard;
    Spell bestSpawnSpell = this.GetBestSpawnSpell(out standard);
    if (standard)
    {
      if ((UnityEngine.Object) creatorCard == (UnityEngine.Object) null)
        this.ActivateStandardSpawnMinionSpell();
      else
        this.StartCoroutine(this.ActivateCreatorSpawnMinionSpell(creator, creatorCard));
    }
    else
      this.ActivateCustomSpawnMinionSpell(bestSpawnSpell, creatorCard);
  }

  private IEnumerator ActivateCreatorSpawnMinionSpell(Entity creator, Card creatorCard)
  {
    while (creator.IsLoadingAssets() || !creatorCard.IsActorReady())
      yield return (object) 0;
    if ((UnityEngine.Object) creatorCard.ActivateCreatorSpawnMinionSpell() != (UnityEngine.Object) null)
      yield return (object) new WaitForSeconds(0.9f);
    this.ActivateStandardSpawnMinionSpell();
  }

  private Spell ActivateCreatorSpawnMinionSpell() => this.m_entity.IsControlledByFriendlySidePlayer() ? this.ActivateActorSpell(SpellType.FRIENDLY_SPAWN_MINION_OR_LOCATION) : this.ActivateActorSpell(SpellType.OPPONENT_SPAWN_MINION_OR_LOCATION);

  private void ActivateStandardSpawnMinionSpell()
  {
    this.m_activeSpawnSpell = !this.m_entity.IsControlledByFriendlySidePlayer() ? this.ActivateActorSpell(SpellType.OPPONENT_SPAWN_MINION_OR_LOCATION, new Spell.FinishedCallback(this.OnSpellFinished_StandardSpawnCharacter)) : this.ActivateActorSpell(SpellType.FRIENDLY_SPAWN_MINION_OR_LOCATION, new Spell.FinishedCallback(this.OnSpellFinished_StandardSpawnCharacter));
    this.ActivateCharacterPlayEffects();
  }

  private void ActivateStandardSpawnHeroSpell()
  {
    if (this.m_entity.IsControlledByFriendlySidePlayer())
      this.m_activeSpawnSpell = this.ActivateActorSpell(SpellType.FRIENDLY_SPAWN_HERO, new Spell.FinishedCallback(this.OnSpellFinished_StandardSpawnCharacter));
    else
      this.m_activeSpawnSpell = this.ActivateActorSpell(SpellType.OPPONENT_SPAWN_HERO, new Spell.FinishedCallback(this.OnSpellFinished_StandardSpawnCharacter));
  }

  private void ActivateCustomSpawnMinionSpell(Spell spell, Card creatorCard)
  {
    this.m_activeSpawnSpell = spell;
    GameObject go = (UnityEngine.Object) creatorCard == (UnityEngine.Object) null ? this.gameObject : creatorCard.gameObject;
    spell.SetSource(go);
    spell.RemoveAllTargets();
    spell.AddTarget(this.gameObject);
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_ReleaseSpell));
    SpellUtils.SetCustomSpellParent(spell, (Component) this.m_actor);
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_CustomSpawnMinion));
    spell.Activate();
  }

  private IEnumerator ActivateReviveSpell()
  {
    while (this.m_activeDeathEffectCount > 0)
      yield return (object) 0;
    this.ActivateStandardSpawnMinionSpell();
  }

  private IEnumerator ActivateActorBattlecrySpell()
  {
    Card card = this;
    if (!card.IsLettuceAbility())
    {
      Spell battlecrySpell = card.GetActorSpell(SpellType.BATTLECRY);
      if (!((UnityEngine.Object) battlecrySpell == (UnityEngine.Object) null) && card.m_zone is ZonePlay && !((UnityEngine.Object) InputManager.Get() == (UnityEngine.Object) null) && !((UnityEngine.Object) InputManager.Get().GetBattlecrySourceCard() != (UnityEngine.Object) card))
      {
        yield return (object) new WaitForSeconds(0.01f);
        if (!((UnityEngine.Object) InputManager.Get() == (UnityEngine.Object) null) && !((UnityEngine.Object) InputManager.Get().GetBattlecrySourceCard() != (UnityEngine.Object) card))
        {
          if (battlecrySpell.GetActiveState() == SpellStateType.NONE)
            battlecrySpell.ActivateState(SpellStateType.BIRTH);
          Spell playSpell = card.GetPlaySpell(0);
          if ((bool) (UnityEngine.Object) playSpell)
            playSpell.ActivateState(SpellStateType.BIRTH);
        }
      }
    }
  }

  private void CleanUpCustomSpell(Spell chosenSpell, ref Spell customSpell)
  {
    if (!(bool) (UnityEngine.Object) customSpell)
      return;
    if ((UnityEngine.Object) chosenSpell == (UnityEngine.Object) customSpell)
      customSpell = (Spell) null;
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) customSpell.gameObject);
  }

  private void OnSpellFinished_StandardSpawnCharacter(Spell spell, object userData)
  {
    this.m_actorReady = true;
    this.m_inputEnabled = true;
    this.m_actor.Show();
    this.ActivateStateSpells();
    this.RefreshActor();
    this.UpdateActorComponents();
    BoardEvents boardEvents = BoardEvents.Get();
    if (!((UnityEngine.Object) boardEvents != (UnityEngine.Object) null))
      return;
    boardEvents.SummonedEvent(this);
  }

  private void OnSpellFinished_CustomSpawnMinion(Spell spell, object userData)
  {
    this.OnSpellFinished_StandardSpawnCharacter(spell, userData);
    this.CleanUpCustomSpell(spell, ref this.m_customSpawnSpell);
    this.CleanUpCustomSpell(spell, ref this.m_customSpawnSpellOverride);
    this.ActivateCharacterPlayEffects();
  }

  private void OnSpellFinished_DefaultHandSpawn(Spell spell, object userData)
  {
    this.m_actor.ToggleForceIdle(false);
    this.m_inputEnabled = true;
    this.ActivateStateSpells();
    this.RefreshActor();
    this.UpdateActorComponents();
  }

  private void OnSpellFinished_CustomHandSpawn(Spell spell, object userData)
  {
    this.OnSpellFinished_DefaultHandSpawn(spell, userData);
    this.CleanUpCustomSpell(spell, ref this.m_customSpawnSpellOverride);
  }

  private void OnSpellFinished_DefaultPlaySpawn(Spell spell, object userData)
  {
    this.m_actor.ToggleForceIdle(false);
    this.m_inputEnabled = true;
    if ((UnityEngine.Object) this.m_zone != (UnityEngine.Object) null)
      this.ActivateStateSpells();
    this.RefreshActor();
    this.UpdateActorComponents();
  }

  private void OnSpellFinished_CustomPlaySpawn(Spell spell, object userData)
  {
    this.OnSpellFinished_DefaultPlaySpawn(spell, userData);
    this.CleanUpCustomSpell(spell, ref this.m_customSpawnSpell);
    this.CleanUpCustomSpell(spell, ref this.m_customSpawnSpellOverride);
  }

  private void OnSpellFinished_StandardCardSummon(Spell spell, object userData)
  {
    this.m_actorReady = true;
    this.m_inputEnabled = true;
    this.ActivateStateSpells();
    this.RefreshActor();
    this.UpdateActorComponents();
  }

  private void OnSpellFinished_UpdateActorComponents(Spell spell, object userData) => this.UpdateActorComponents();

  private void OnSpellFinished_Death(Spell spell, object userData)
  {
    this.m_suppressKeywordDeaths = false;
    this.m_keywordDeathDelaySec = 0.6f;
    --this.m_activeDeathEffectCount;
    GameState.Get()?.ClearCardBeingDrawn(this);
  }

  private void OnSpellStateFinished_DestroyActor(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    if (this.m_zone is ZoneGraveyard)
      this.PurgeSpells();
    Actor componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<Actor>(spell.gameObject);
    if ((UnityEngine.Object) componentInThisOrParents == (UnityEngine.Object) null)
      Debug.LogWarning((object) string.Format("Card.OnSpellStateFinished_DestroyActor() - spell {0} on Card {1} has no Actor ancestor", (object) spell, (object) this));
    else
      componentInThisOrParents.Destroy();
  }

  private void OnSpellStateFinished_ReleaseSpell(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    SpellManager.Get().ReleaseSpell(spell);
  }

  private void OnSpellStateFinished_CustomDeath(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (spell.GetActiveState() != SpellStateType.NONE)
      return;
    Actor componentInThisOrParents = GameObjectUtils.FindComponentInThisOrParents<Actor>(spell.gameObject);
    if ((UnityEngine.Object) componentInThisOrParents == (UnityEngine.Object) null)
      Debug.LogWarning((object) string.Format("Card.OnSpellStateFinished_CustomDeath() - spell {0} on Card {1} has no Actor ancestor", (object) spell, (object) this));
    else
      componentInThisOrParents.Destroy();
  }

  public void UpdateActorState(bool forceHighlightRefresh = false)
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null || !this.m_shown || this.m_entity.IsBusy() || this.m_zone is ZoneGraveyard)
      return;
    if (!this.m_inputEnabled || (UnityEngine.Object) this.m_zone != (UnityEngine.Object) null && !this.m_zone.IsInputEnabled())
    {
      this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
      this.m_actor.SetTradeableHighlightState(TradeableHighlightState.None);
    }
    else
    {
      GameState state = GameState.Get();
      if (state != null && state.IsEntityInputEnabled(this.m_entity))
      {
        if (forceHighlightRefresh)
        {
          this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
          this.m_actor.SetTradeableHighlightState(TradeableHighlightState.None);
        }
        switch (state.GetResponseMode())
        {
          case GameState.ResponseMode.OPTION:
            if (this.DoOptionHighlight(state))
              return;
            break;
          case GameState.ResponseMode.SUB_OPTION:
            if (this.DoSubOptionHighlight(state))
              return;
            break;
          case GameState.ResponseMode.OPTION_TARGET:
            if (this.DoOptionTargetHighlight(state))
              return;
            break;
          case GameState.ResponseMode.CHOICE:
            if (this.DoChoiceHighlight(state))
              return;
            break;
        }
      }
      else
        this.m_actor.SetTradeableHighlightState(TradeableHighlightState.None);
      if (this.m_mousedOver && !(this.m_zone is ZoneHand))
      {
        if (this.m_actor.UseBGQuestSiloutte())
          this.m_actor.SetActorState(ActorStateType.CARD_MOUSE_OVER_BG_QUEST);
        else
          this.m_actor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
      }
      else if (this.m_mousedOverByOpponent)
        this.m_actor.SetActorState(ActorStateType.CARD_OPPONENT_MOUSE_OVER);
      else if (this.ShouldHighlightSelectedLettuceCharacter(state))
        this.m_actor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
      else
        this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
    }
  }

  private bool ShouldHighlightSelectedLettuceCharacter(GameState state) => state != null && state.GetResponseMode() == GameState.ResponseMode.OPTION && ZoneMgr.Get().GetLettuceAbilitiesSourceEntity() == this.m_entity;

  public void UpdateSelectedLettuceCharacterVisual()
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    Spell spell = this.m_actor.GetSpell(SpellType.MERCENARIES_LIFT_UP);
    if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
    {
      if (ZoneMgr.Get().GetLettuceAbilitiesSourceEntity() == this.m_entity)
        spell.ActivateState(SpellStateType.BIRTH);
      else
        spell.ActivateState(SpellStateType.DEATH);
    }
    this.UpdateActorState();
  }

  private bool DoChoiceHighlight(GameState state)
  {
    if (state.GetChosenEntities().Contains(this.m_entity))
    {
      if (this.m_mousedOver)
        this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE_MOUSE_OVER);
      else
        this.m_actor.SetActorState(ActorStateType.CARD_SELECTED);
      return true;
    }
    int entityId = this.m_entity.GetEntityId();
    Network.EntityChoices friendlyEntityChoices = state.GetFriendlyEntityChoices();
    if (!friendlyEntityChoices.Entities.Contains(entityId))
      return false;
    if (GameState.Get().IsMulliganManagerActive())
    {
      if (GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_HAS_HERO_LOBBY))
      {
        if (this.m_mousedOver)
          this.m_actor.SetActorState(GameState.Get().GetGameEntity().GetMulliganChoiceHighlightState());
        else
          this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
      }
      else
        this.m_actor.SetActorState(GameState.Get().GetGameEntity().GetMulliganChoiceHighlightState());
    }
    else if (friendlyEntityChoices.ChoiceType == CHOICE_TYPE.TARGET)
      this.m_actor.SetActorState(ActorStateType.CARD_VALID_TARGET);
    else if (this.m_actor.UseBGQuestSiloutte())
      this.m_actor.SetActorState(ActorStateType.CARD_SELECTABLE_BG_QUEST);
    else
      this.m_actor.SetActorState(ActorStateType.CARD_SELECTABLE);
    return true;
  }

  private bool DoOptionHighlight(GameState state)
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return false;
    bool flag1 = GameState.Get().IsValidOption(this.m_entity, new bool?(true));
    if (flag1)
    {
      if (this.m_overPlayfield)
      {
        if (this.IsInTradeArea())
        {
          this.m_actor.SetTradeableHighlightState(TradeableHighlightState.Blue);
          this.m_actor.SetActorState(ActorStateType.CARD_OVER_PLAYFIELD);
          this.ShowTradeableHover();
          return true;
        }
        this.m_actor.SetTradeableHighlightState(TradeableHighlightState.None);
        this.HideTradeableHover();
      }
      else
        this.m_actor.SetTradeableHighlightState(TradeableHighlightState.Green);
    }
    else
      this.m_actor.SetTradeableHighlightState(TradeableHighlightState.None);
    if (this.m_entity.HasTag(GAME_TAG.FORCE_GREEN_GLOW_ACTIVE) && this.m_entity.IsControlledByFriendlySidePlayer())
    {
      this.m_actor.SetActorState(this.m_mousedOver || this.ShouldHighlightSelectedLettuceCharacter(state) ? ActorStateType.CARD_PLAYABLE_MOUSE_OVER : ActorStateType.CARD_PLAYABLE);
      return true;
    }
    if (GameState.Get().GetGameEntity().ShouldSuppressOptionHighlight(this.m_entity) || !GameState.Get().IsValidOption(this.m_entity, new bool?(false)))
      return false;
    if (this.IsOverMoveMinionTarget())
    {
      this.m_actor.SetActorState(ActorStateType.CARD_OVER_MOVE_MINION_TARGET);
      return true;
    }
    if (this.m_overPlayfield)
    {
      if (this.IsInTradeArea() && !flag1)
        this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
      else
        this.m_actor.SetActorState(ActorStateType.CARD_OVER_PLAYFIELD);
      return true;
    }
    bool flag2 = this.m_entity.GetZone() == TAG_ZONE.HAND;
    bool flag3 = this.m_entity.GetController().IsRealTimeComboActive();
    if (((flag2 ? 1 : (this.m_entity.IsCardButton() ? 1 : 0)) & (flag3 ? 1 : 0)) != 0 && this.m_entity.HasTag(GAME_TAG.COMBO))
    {
      this.m_actor.SetActorState(ActorStateType.CARD_COMBO);
      return true;
    }
    bool realTimePoweredUp = this.m_entity.GetRealTimePoweredUp();
    if (((flag2 ? 1 : (this.m_entity.IsCardButton() ? 1 : 0)) & (realTimePoweredUp ? 1 : 0)) != 0)
    {
      this.m_actor.SetActorState(ActorStateType.CARD_POWERED_UP);
      return true;
    }
    if ((this.m_entity.GetZone() == TAG_ZONE.PLAY ? 1 : (this.m_latestZoneChange == null || !((UnityEngine.Object) this.m_latestZoneChange.GetDestinationZone() != (UnityEngine.Object) null) ? 0 : (this.m_latestZoneChange.GetDestinationZone().m_ServerTag == TAG_ZONE.PLAY ? 1 : 0))) != 0 && state.GetGameEntity().GetTag(GAME_TAG.ALLOW_MOVE_MINION) > 0 && this.m_entity.IsMinion())
    {
      if (!GameState.Get().HasEnoughManaForMoveMinionHoverTarget(this.m_entity))
      {
        if (this.m_mousedOver)
          this.m_actor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
        else
          this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
        return true;
      }
      if (this.m_mousedOver)
        this.m_actor.SetActorState(ActorStateType.CARD_MOVEABLE_MOUSE_OVER);
      else
        this.m_actor.SetActorState(ActorStateType.CARD_MOVEABLE);
      return true;
    }
    if (!flag2 && this.m_mousedOver)
    {
      if (this.m_entity.GetRealTimeAttackableByRush())
        this.m_actor.SetActorState(ActorStateType.CARD_ATTACKABLE_BY_RUSH_MOUSE_OVER);
      else
        this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE_MOUSE_OVER);
      return true;
    }
    if (this.m_entity.GetRealTimeAttackableByRush())
    {
      this.m_actor.SetActorState(ActorStateType.CARD_ATTACKABLE_BY_RUSH);
      return true;
    }
    this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE);
    return true;
  }

  private bool DoSubOptionHighlight(GameState state)
  {
    Network.Options.Option selectedNetworkOption = state.GetSelectedNetworkOption();
    int entityId = this.m_entity.GetEntityId();
    foreach (Network.Options.Option.SubOption sub in selectedNetworkOption.Subs)
    {
      if (entityId == sub.ID)
      {
        if (!sub.PlayErrorInfo.IsValid())
          return false;
        if (this.m_mousedOver)
          this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE_MOUSE_OVER);
        else
          this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE);
        return true;
      }
    }
    return false;
  }

  private bool DoOptionTargetHighlight(GameState state)
  {
    this.m_actor.SetTradeableHighlightState(TradeableHighlightState.None);
    if (!state.GetSelectedNetworkSubOption().IsValidTarget(this.m_entity.GetEntityId()))
      return false;
    if (this.m_mousedOver)
      this.m_actor.SetActorState(ActorStateType.CARD_VALID_TARGET_MOUSE_OVER);
    else
      this.m_actor.SetActorState(ActorStateType.CARD_VALID_TARGET);
    return true;
  }

  public Actor GetActor() => this.m_actor;

  public void SetActor(Actor actor) => this.m_actor = actor;

  public Actor GetQuestRewardActor() => this.m_questRewardActor;

  public string GetActorAssetPath() => this.m_actorPath;

  public void SetActorAssetPath(string actorName) => this.m_actorPath = actorName;

  public bool IsActorReady() => this.m_actorReady;

  public bool IsActorLoading() => this.m_actorLoading;

  public void UpdateActorComponents()
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    this.m_actor.UpdateAllComponents();
  }

  public void UpdateLettuceSpeechBubbleText(bool hideUnselectedBubbles)
  {
    Spell actorSpell = this.GetActorSpell(SpellType.MERCENARIES_SPEECH_BUBBLE);
    if ((UnityEngine.Object) actorSpell == (UnityEngine.Object) null)
      return;
    Entity lettuceAbilityEntity = this.GetPreparedLettuceAbilityEntity();
    PlayMakerFSM component = actorSpell.GetComponent<PlayMakerFSM>();
    if (lettuceAbilityEntity != null)
    {
      bool flag = component.FsmVariables.GetFsmString("Text").Value == string.Empty;
      if (this.m_lettuceAbilityActionOrderIsTied)
        component.FsmVariables.GetFsmString("Text").Value = GameStrings.Format("GAMEPLAY_LETTUCE_ABILITY_ORDER_TIED_TEXT", (object) GameStrings.GetOrdinalNumber(this.m_lettuceAbilityActionOrder));
      else
        component.FsmVariables.GetFsmString("Text").Value = GameStrings.GetOrdinalNumber(this.m_lettuceAbilityActionOrder);
      actorSpell.ActivateState(flag ? SpellStateType.BIRTH : SpellStateType.ACTION);
    }
    else
    {
      component.FsmVariables.GetFsmString("Text").Value = string.Empty;
      actorSpell.ActivateState(hideUnselectedBubbles ? SpellStateType.DEATH : SpellStateType.ACTION);
    }
  }

  public void SetLettuceAbilityActionOrder(int order, bool isTied)
  {
    this.m_lettuceAbilityActionOrder = order;
    this.m_lettuceAbilityActionOrderIsTied = isTied;
  }

  public int GetLettuceAbilityActionOrder() => this.m_lettuceAbilityActionOrder;

  public int GetPreparedLettuceAbilitySpeedValue()
  {
    Entity lettuceAbilityEntity = this.GetPreparedLettuceAbilityEntity();
    if (lettuceAbilityEntity == null)
      return int.MaxValue;
    int cost = lettuceAbilityEntity.GetCost();
    if (lettuceAbilityEntity.HasTag(GAME_TAG.HIDE_COST) && lettuceAbilityEntity.HasSubCards() && this.m_entity != null)
    {
      int tag = this.m_entity.GetTag(GAME_TAG.LETTUCE_SELECTED_SUBCARD_INDEX);
      List<int> subCardIds = lettuceAbilityEntity.GetSubCardIDs();
      if (subCardIds.Count > tag)
      {
        Entity entity = GameState.Get().GetEntity(subCardIds[tag]);
        if (entity != null)
          cost = entity.GetCost();
      }
    }
    return cost;
  }

  public Entity GetPreparedLettuceAbilityEntity()
  {
    if (this.GetEntity() == null)
      return (Entity) null;
    int lettuceAbilityId = this.GetEntity().GetSelectedLettuceAbilityID();
    return GameState.Get().GetEntity(lettuceAbilityId);
  }

  private Color GetLettuceSpeedTextColor(int defNumber, int currentNumber)
  {
    if (defNumber > currentNumber)
      return Color.green;
    if (defNumber >= currentNumber)
      return Color.white;
    return (bool) UniversalInputManager.UsePhoneUI ? new Color(1f, 0.1960784f, 0.1960784f) : Color.red;
  }

  public void RefreshActor()
  {
    this.UpdateActorState();
    if (this.m_entity.IsEnchanted())
      this.UpdateEnchantments();
    this.UpdateTooltip();
  }

  public Zone GetZone() => this.m_zone;

  public Zone GetPrevZone() => this.m_prevZone;

  public void SetZone(Zone zone) => this.m_zone = zone;

  public int GetZonePosition() => this.m_zonePosition;

  public void SetZonePosition(int pos) => this.m_zonePosition = pos;

  public int GetPredictedZonePosition() => this.m_predictedZonePosition;

  public void SetPredictedZonePosition(int pos) => this.m_predictedZonePosition = pos;

  public ZoneTransitionStyle GetTransitionStyle() => this.m_transitionStyle;

  public void SetTransitionStyle(ZoneTransitionStyle style) => this.m_transitionStyle = style;

  public bool IsTransitioningZones() => this.m_transitioningZones;

  public void EnableTransitioningZones(bool enable) => this.m_transitioningZones = enable;

  public bool HasBeenGrabbedByEnemyActionHandler() => this.m_hasBeenGrabbedByEnemyActionHandler;

  public void MarkAsGrabbedByEnemyActionHandler(bool enable)
  {
    Log.FaceDownCard.Print("Card.MarkAsGrabbedByEnemyActionHandler() - card={0} enable={1}", (object) this, (object) enable);
    this.m_hasBeenGrabbedByEnemyActionHandler = enable;
  }

  public bool IsDoNotSort() => this.m_doNotSort;

  public void SetDoNotSort(bool on)
  {
    if (this.m_entity.IsControlledByOpposingSidePlayer())
      Log.FaceDownCard.Print("Card.SetDoNotSort() - card={0} on={1}", (object) this, (object) on);
    this.m_doNotSort = on;
  }

  public bool IsDoNotWarpToNewZone() => this.m_doNotWarpToNewZone;

  public void SetDoNotWarpToNewZone(bool on) => this.m_doNotWarpToNewZone = on;

  public float GetTransitionDelay() => this.m_transitionDelay;

  public void SetTransitionDelay(float delay) => this.m_transitionDelay = delay;

  public void UpdateZoneFromTags()
  {
    this.m_zonePosition = this.m_entity.GetZonePosition();
    Zone zoneForEntity = ZoneMgr.Get().FindZoneForEntity(this.m_entity);
    this.TransitionToZone(zoneForEntity);
    if (!((UnityEngine.Object) zoneForEntity != (UnityEngine.Object) null))
      return;
    zoneForEntity.UpdateLayout();
  }

  public void TransitionToZone(Zone zone, ZoneChange zoneChange = null)
  {
    this.m_latestZoneChange = zoneChange;
    if ((UnityEngine.Object) this.m_zone == (UnityEngine.Object) zone)
      Log.Gameplay.Print("Card.TransitionToZone() - card={0} already in target zone", (object) this);
    else if ((UnityEngine.Object) zone == (UnityEngine.Object) null)
    {
      if (this.m_zone.ContainsCard(this))
        this.m_zone.RemoveCard(this);
      this.m_prevZone = this.m_zone;
      this.m_zone = (Zone) null;
      this.DeactivateLifetimeEffects();
      this.DeactivateCustomKeywordEffect();
      if (this.m_prevZone is ZoneHand)
        this.DeactivateHandStateSpells();
      if (this.m_prevZone is ZoneHeroPower)
      {
        foreach (Card card in this.m_prevZone.GetCards())
        {
          if (!((UnityEngine.Object) card == (UnityEngine.Object) this) && card.GetEntity().GetTag(GAME_TAG.LINKED_ENTITY) == this.m_entity.GetEntityId() && (UnityEngine.Object) card.m_customSpawnSpellOverride != (UnityEngine.Object) null)
          {
            if (!((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null))
              return;
            this.m_actor.DeactivateAllSpells();
            return;
          }
        }
      }
      if (this.m_prevZone is ZoneHero)
      {
        Player controller = this.m_prevZone.GetController();
        if (controller.GetHero() != null && (UnityEngine.Object) controller.GetHero().GetCard() != (UnityEngine.Object) null)
          controller.GetHero().GetCard().ShowCard();
      }
      this.DoNullZoneVisuals();
    }
    else
    {
      if (this.m_zone is ZoneSecret && this.m_entity != null && (this.m_entity.IsQuest() || this.m_entity.IsQuestline()))
        this.NotifyMousedOut();
      this.m_prevZone = this.m_zone;
      this.m_zone = zone;
      if (this.m_prevZone is ZoneDeck && this.m_zone is ZoneHand)
      {
        if (this.m_zone.m_Side == Player.Side.FRIENDLY)
        {
          this.m_cardDrawTracker = GameState.Get().GetFriendlyCardDrawCounter();
          GameState.Get().IncrementFriendlyCardDrawCounter();
        }
        else
        {
          this.m_cardDrawTracker = GameState.Get().GetOpponentCardDrawCounter();
          GameState.Get().IncrementOpponentCardDrawCounter();
        }
      }
      if ((UnityEngine.Object) this.m_prevZone != (UnityEngine.Object) null && this.m_prevZone.ContainsCard(this))
        this.m_prevZone.RemoveCard(this);
      this.m_zone.AddCard(this);
      if ((this.m_zone is ZonePlay || this.m_zone is ZoneHero) && this.m_prevZone is ZoneHand && this.m_entity.IsHero() && GameState.Get().GetBooleanGameOption(GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS) && (UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null && MulliganManager.Get().IsMulliganActive())
      {
        this.m_actorReady = true;
      }
      else
      {
        if (this.m_zone is ZoneGraveyard && (UnityEngine.Object) this.m_actor != (UnityEngine.Object) null && this.m_actor.UseCoinManaGem())
          this.m_actor.ReleaseSpell(SpellType.COIN_MANA_GEM);
        if (this.m_zone is ZoneGraveyard && GameState.Get().IsBeingDrawn(this))
        {
          this.m_actorReady = true;
          this.DiscardCardBeingDrawn();
        }
        else if (this.m_zone is ZoneGraveyard && this.m_ignoreDeath)
          this.m_actorReady = true;
        else if (this.m_zone is ZoneGraveyard && (UnityEngine.Object) this.m_actor != (UnityEngine.Object) null && this.m_actorReady && this.m_entity.IsSpell())
        {
          this.m_actorReady = false;
          this.StartCoroutine(this.LoadActorAndSpellsAfterPowerUpFinishes());
        }
        else
        {
          this.m_actorReady = false;
          this.LoadActorAndSpells();
        }
      }
    }
  }

  public void UpdateActor(bool forceIfNullZone = false, string actorPath = null)
  {
    if (!forceIfNullZone && (UnityEngine.Object) this.m_zone == (UnityEngine.Object) null)
      return;
    TAG_ZONE zone = this.m_entity.GetZone();
    if (actorPath == null)
      actorPath = this.m_cardDef.CardDef.DetermineActorPathForZone(this.m_entity, zone);
    if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null && this.m_actorPath == actorPath && !(this.m_actor is LettuceAbilityActor))
      return;
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) actorPath, AssetLoadingOptions.IgnorePrefabPosition);
    if (!(bool) (UnityEngine.Object) gameObject)
    {
      Debug.LogWarningFormat("Card.UpdateActor() - FAILED to load actor \"{0}\"", (object) actorPath);
    }
    else
    {
      Actor component = gameObject.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("Card.UpdateActor() - ERROR actor \"{0}\" has no Actor component", (object) actorPath);
      }
      else
      {
        if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
          this.m_actor.Destroy();
        this.m_actor = component;
        this.m_actorPath = actorPath;
        this.m_actor.SetEntity(this.m_entity);
        this.m_actor.SetCard(this);
        this.m_actor.SetCardDef(this.m_cardDef);
        this.m_actor.UpdateAllComponents();
        if (this.m_shown)
          this.ShowImpl();
        else
          this.HideImpl();
        this.RefreshActor();
      }
    }
  }

  private IEnumerator LoadActorAndSpellsAfterPowerUpFinishes()
  {
    this.m_actorLoading = true;
    Spell spell = this.m_actor.GetSpell(SpellType.POWER_UP);
    if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
    {
      while (spell.GetActiveState() != SpellStateType.NONE && spell.GetActiveState() != SpellStateType.IDLE)
        yield return (object) null;
    }
    this.LoadActorAndSpells();
  }

  private void LoadActorAndSpells()
  {
    this.m_actorLoading = true;
    List<Card.PrefabLoadRequest> prefabLoadRequestList = new List<Card.PrefabLoadRequest>();
    if (this.m_prevZone is ZoneHand && (this.m_zone is ZonePlay || this.m_zone is ZoneHero || this.m_zone is ZoneWeapon))
    {
      Card.PrefabLoadRequest prefabLoadRequest = this.MakeCustomSpellLoadRequest(this.m_cardDef.CardDef.m_CustomSummonSpellPath, this.m_cardDef.CardDef.m_GoldenCustomSummonSpellPath, new PrefabCallback<GameObject>(this.OnCustomSummonSpellLoaded));
      if (prefabLoadRequest != null)
        prefabLoadRequestList.Add(prefabLoadRequest);
    }
    if (!(bool) (UnityEngine.Object) this.m_customDeathSpell && (this.m_zone is ZoneHand || this.m_zone is ZonePlay))
    {
      Card.PrefabLoadRequest prefabLoadRequest = this.MakeCustomSpellLoadRequest(this.m_cardDef.CardDef.m_CustomDeathSpellPath, this.m_cardDef.CardDef.m_GoldenCustomDeathSpellPath, new PrefabCallback<GameObject>(this.OnCustomDeathSpellLoaded));
      if (prefabLoadRequest != null)
        prefabLoadRequestList.Add(prefabLoadRequest);
    }
    if (!(bool) (UnityEngine.Object) this.m_customDiscardSpell && (this.m_zone is ZoneHand || this.m_zone is ZoneGraveyard))
    {
      Card.PrefabLoadRequest prefabLoadRequest = this.MakeCustomSpellLoadRequest(this.m_cardDef.CardDef.m_CustomDiscardSpellPath, this.m_cardDef.CardDef.m_GoldenCustomDiscardSpellPath, new PrefabCallback<GameObject>(this.OnCustomDiscardSpellLoaded));
      if (prefabLoadRequest != null)
        prefabLoadRequestList.Add(prefabLoadRequest);
    }
    if (!(bool) (UnityEngine.Object) this.m_customSpawnSpell && (this.m_zone is ZonePlay || this.m_zone is ZoneWeapon || this.m_zone is ZoneBattlegroundHeroBuddy || this.m_zone is ZoneBattlegroundQuestReward))
    {
      Card.PrefabLoadRequest prefabLoadRequest = this.MakeCustomSpellLoadRequest(this.m_cardDef.CardDef.m_CustomSpawnSpellPath, this.m_cardDef.CardDef.m_GoldenCustomSpawnSpellPath, new PrefabCallback<GameObject>(this.OnCustomSpawnSpellLoaded));
      if (prefabLoadRequest != null)
        prefabLoadRequestList.Add(prefabLoadRequest);
    }
    this.m_spellLoadCount = prefabLoadRequestList.Count;
    if (prefabLoadRequestList.Count == 0)
    {
      this.LoadActor();
    }
    else
    {
      foreach (Card.PrefabLoadRequest prefabLoadRequest in prefabLoadRequestList)
        AssetLoader.Get().InstantiatePrefab((AssetReference) prefabLoadRequest.m_path, prefabLoadRequest.m_loadCallback);
    }
  }

  private Card.PrefabLoadRequest MakeCustomSpellLoadRequest(
    string customPath,
    string goldenCustomPath,
    PrefabCallback<GameObject> loadCallback)
  {
    string str = customPath;
    if (this.m_entity.GetPremiumType() == TAG_PREMIUM.GOLDEN && !string.IsNullOrEmpty(goldenCustomPath))
      str = goldenCustomPath;
    else if (string.IsNullOrEmpty(str))
      return (Card.PrefabLoadRequest) null;
    return new Card.PrefabLoadRequest()
    {
      m_path = str,
      m_loadCallback = loadCallback
    };
  }

  private void OnCustomSummonSpellLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("Card.OnCustomSummonSpellLoaded() - FAILED to load \"{0}\" for card {1}", (object) assetRef, (object) this);
      this.FinishSpellLoad();
    }
    else
    {
      this.m_customSummonSpell = go.GetComponent<Spell>();
      if ((UnityEngine.Object) this.m_customSummonSpell == (UnityEngine.Object) null)
      {
        this.FinishSpellLoad();
      }
      else
      {
        SpellUtils.SetupSpell(this.m_customSummonSpell, (Component) this);
        this.FinishSpellLoad();
      }
    }
  }

  private void OnCustomDeathSpellLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("Card.OnCustomDeathSpellLoaded() - FAILED to load \"{0}\" for card {1}", (object) assetRef, (object) this);
      this.FinishSpellLoad();
    }
    else
    {
      this.m_customDeathSpell = go.GetComponent<Spell>();
      if ((UnityEngine.Object) this.m_customDeathSpell == (UnityEngine.Object) null)
      {
        this.FinishSpellLoad();
      }
      else
      {
        SpellUtils.SetupSpell(this.m_customDeathSpell, (Component) this);
        this.FinishSpellLoad();
      }
    }
  }

  private void OnCustomDiscardSpellLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("Card.OnCustomDiscardSpellLoaded() - FAILED to load \"{0}\" for card {1}", (object) assetRef, (object) this);
      this.FinishSpellLoad();
    }
    else
    {
      this.m_customDiscardSpell = go.GetComponent<Spell>();
      if ((UnityEngine.Object) this.m_customDiscardSpell == (UnityEngine.Object) null)
      {
        this.FinishSpellLoad();
      }
      else
      {
        SpellUtils.SetupSpell(this.m_customDiscardSpell, (Component) this);
        this.FinishSpellLoad();
      }
    }
  }

  private void OnCustomSpawnSpellLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("Card.OnCustomSpawnSpellLoaded() - FAILED to load \"{0}\" for card {1}", (object) assetRef, (object) this);
      this.FinishSpellLoad();
    }
    else
    {
      this.m_customSpawnSpell = go.GetComponent<Spell>();
      if ((UnityEngine.Object) this.m_customSpawnSpell == (UnityEngine.Object) null)
      {
        this.FinishSpellLoad();
      }
      else
      {
        SpellUtils.SetupSpell(this.m_customSpawnSpell, (Component) this);
        this.FinishSpellLoad();
      }
    }
  }

  private void FinishSpellLoad()
  {
    --this.m_spellLoadCount;
    if (this.m_spellLoadCount > 0)
      return;
    this.LoadActor();
  }

  private void LoadActor()
  {
    this.RefreshCardsInTooltip();
    string actorPathForZone = this.m_cardDef.CardDef.DetermineActorPathForZone(this.m_entity, this.m_zone.m_ServerTag);
    if (this.m_actorPath == actorPathForZone || actorPathForZone == null)
    {
      this.m_actorPath = actorPathForZone;
      this.FinishActorLoad(this.m_actor);
    }
    else
      AssetLoader.Get().InstantiatePrefab((AssetReference) actorPathForZone, new PrefabCallback<GameObject>(this.OnActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
  }

  private bool ShouldShowCardsInTooltip() => (UnityEngine.Object) this.m_cardsInTooltip != (UnityEngine.Object) null && this.m_zone is ZoneHand && this.m_entity.IsControlledByFriendlySidePlayer();

  private void CreateCardsInTooltip()
  {
    if ((UnityEngine.Object) this.m_cardsInTooltip == (UnityEngine.Object) null)
    {
      this.m_cardsInTooltip = this.gameObject.AddComponent<DisplayCardsInToolip>();
      this.m_cardsInTooltip.Setup(this);
    }
    if (this.m_entity.HasTag(GAME_TAG.HERO_POWER))
      this.m_cardsInTooltip.AddCardsInTooltip(this.m_entity.GetTag(GAME_TAG.HERO_POWER));
    if (this.m_entity.HasTag(GAME_TAG.DISPLAY_CARD_ON_MOUSEOVER))
      this.m_cardsInTooltip.AddCardsInTooltip(this.m_entity.GetTag(GAME_TAG.DISPLAY_CARD_ON_MOUSEOVER));
    if (!GameState.Get().BattlegroundAllowBuddies())
      return;
    int heroBuddyCardId = this.m_entity.GetHeroBuddyCardId();
    if (heroBuddyCardId == 0)
      return;
    this.m_cardsInTooltip.AddCardsInTooltip(heroBuddyCardId);
  }

  private void DestroyCardsInTooltip()
  {
    if (!((UnityEngine.Object) this.m_cardsInTooltip != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_cardsInTooltip);
    this.m_cardsInTooltip = (DisplayCardsInToolip) null;
  }

  public void RefreshCardsInTooltip()
  {
    this.DestroyCardsInTooltip();
    if (!(this.m_zone is ZoneHand) || !this.m_entity.IsHero() && !this.m_entity.HasTag(GAME_TAG.DISPLAY_CARD_ON_MOUSEOVER))
      return;
    this.CreateCardsInTooltip();
  }

  private void HideCardsInTooltip()
  {
    if (!((UnityEngine.Object) this.m_cardsInTooltip != (UnityEngine.Object) null))
      return;
    this.m_cardsInTooltip.NotifyMousedOut();
  }

  private void OnActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("Card.OnActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      Actor component = go.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("Card.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        Actor actor = this.m_actor;
        this.m_actor = component;
        this.m_actorPath = assetRef.ToString();
        this.m_actor.SetEntity(this.m_entity);
        this.m_actor.SetCard(this);
        this.m_actor.SetCardDef(this.m_cardDef);
        if ((!GameMgr.Get().IsBattlegrounds() ? 0 : (this.m_entity.IsBobQuest() ? 1 : 0)) != 0)
          this.UseBobQuestComponent();
        this.m_actor.UpdateAllComponents();
        this.FinishActorLoad(actor);
      }
    }
  }

  private void FinishActorLoad(Actor oldActor)
  {
    this.m_actorLoading = false;
    this.OnZoneChanged();
    this.OnActorChanged(oldActor);
    if (this.m_isBattleCrySource)
      LayerUtils.SetLayer(this.m_actor.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.RefreshActor();
  }

  public void ForceLoadHandActor()
  {
    string actorPathForZone = this.m_cardDef.CardDef.DetermineActorPathForZone(this.m_entity, TAG_ZONE.HAND);
    if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null && this.m_actorPath == actorPathForZone)
    {
      this.ShowCard();
      this.m_actor.Show();
      this.RefreshActor();
    }
    else
    {
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) actorPathForZone, AssetLoadingOptions.IgnorePrefabPosition);
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("Card.ForceLoadHandActor() - FAILED to load actor \"{0}\"", (object) actorPathForZone);
      }
      else
      {
        Actor component = gameObject.GetComponent<Actor>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          Debug.LogWarningFormat("Card.ForceLoadHandActor() - ERROR actor \"{0}\" has no Actor component", (object) actorPathForZone);
        }
        else
        {
          if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
            this.m_actor.Destroy();
          this.m_actor = component;
          this.m_actorPath = actorPathForZone;
          this.m_actor.SetEntity(this.m_entity);
          this.m_actor.SetCard(this);
          this.m_actor.SetCardDef(this.m_cardDef);
          if ((!GameMgr.Get().IsBattlegrounds() ? 0 : (this.m_entity.IsQuest() ? 1 : 0)) != 0)
          {
            this.UseBattlegroundQuestComponent();
            this.UpdateRewardActor();
          }
          this.m_actor.UpdateAllComponents();
          if (this.m_shown)
            this.ShowImpl();
          else
            this.HideImpl();
          this.RefreshActor();
        }
      }
    }
  }

  private void UseBattlegroundQuestComponent()
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    GameObjectUtils.FindChildBySubstring(this.gameObject, "Description_mesh")?.SetActive(false);
    GameObjectUtils.FindChildBySubstring(this.gameObject, "Card_Hand_BG_Quest_Text_Tray_Mesh")?.SetActive(true);
    if (this.m_actor.m_isDebuggingBattlegroundQuestReward)
      GameObjectUtils.FindChildBySubstring(this.gameObject, "NonQuestObjects")?.SetActive(false);
    this.m_actor.SetUseBGQuestSiloutte(true);
  }

  private void UseBobQuestComponent()
  {
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
      return;
    this.m_actor.SetUseBGQuestSiloutte(true);
  }

  private void UpdateRewardActor()
  {
    if (!((UnityEngine.Object) this.m_questRewardActor == (UnityEngine.Object) null) && !this.m_questRewardChanged)
      return;
    if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null)
      this.m_questRewardActor.Destroy();
    Entity entity = GameState.Get()?.GetEntity(this.GetEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_1));
    if (entity == null)
    {
      Debug.LogWarning((object) "[UpdateRewardActor] - rewardEnt is null");
    }
    else
    {
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(entity), AssetLoadingOptions.IgnorePrefabPosition);
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      {
        Log.Gameplay.PrintError("[UpdateRewardActor] - Unable to load hand actor for entity {0}.", (object) entity);
      }
      else
      {
        LayerUtils.SetLayer(gameObject, this.m_actor.gameObject.layer);
        gameObject.transform.parent = this.m_actor.gameObject.transform;
        TransformUtil.Identity(gameObject);
        GameObjectUtils.FindChildBySubstring(gameObject, "BGRewardVFX")?.SetActive(true);
        this.m_questRewardActor = gameObject.GetComponentInChildren<Actor>();
        this.m_questRewardActor.SetEntity(entity);
        this.m_questRewardActor.SetCardDefFromEntity(entity);
        this.m_questRewardActor.SetPremium(entity.GetPremiumType());
        this.m_questRewardActor.SetWatermarkCardSetOverride(entity.GetWatermarkCardSetOverride());
        LayerUtils.SetLayer((Component) this.m_questRewardActor, GameLayer.CardRaycast);
        this.m_questRewardActor.SetCard(this);
        if (this.m_questRewardActor.UseCoinManaGem())
        {
          this.m_questRewardActor.m_manaObject.SetActive(false);
          this.m_questRewardActor.m_costTextMesh.gameObject.SetActive(false);
          this.m_questRewardActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
        }
        this.m_questRewardActor.UpdateAllComponents();
        this.m_questRewardActor.gameObject.SetActive(false);
        if (!((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null) || !this.m_actor.m_isDebuggingBattlegroundQuestReward)
          return;
        iTween.MoveTo(this.gameObject, this.gameObject.transform.position + new Vector3(0.0f, 0.0f, 0.2f), 0.2f);
        Vector3 vector3_1 = new Vector3(0.0f, -0.1f, 1.05f);
        Vector3 vector3_2 = new Vector3(0.9f, 0.9f, 0.9f);
        gameObject.transform.localPosition = vector3_1;
        gameObject.transform.localScale = vector3_2;
        this.m_questRewardActor.gameObject.SetActive(true);
      }
    }
  }

  private void OnZoneChanged()
  {
    if (this.m_prevZone is ZoneHand && this.m_zone is ZoneGraveyard)
    {
      if (this.m_mousedOver)
        this.NotifyMousedOut();
      this.DoDiscardAnimation();
      this.HideCardsInTooltip();
    }
    else if (this.m_prevZone is ZoneHand)
    {
      if (this.m_mousedOver)
        this.NotifyMousedOut();
    }
    else if (this.m_zone is ZoneGraveyard)
    {
      if (this.m_entity.IsHero())
        this.m_doNotSort = true;
    }
    else if (this.m_zone is ZoneHand)
    {
      if (!this.m_doNotSort)
        this.ShowCard();
      if (this.m_prevZone is ZoneGraveyard && this.m_entity.IsSpell())
      {
        this.m_actor.Hide();
        this.ActivateActorSpell(SpellType.SUMMON_IN, new Spell.FinishedCallback(this.OnSpellFinished_DefaultHandSpawn));
      }
    }
    else if ((this.m_prevZone is ZoneGraveyard || this.m_prevZone is ZoneDeck) && this.m_zone.m_ServerTag == TAG_ZONE.PLAY)
      this.ShowCard();
    if (this.m_zone is ZonePlay || this.m_magneticPlayData == null)
      return;
    SpellUtils.ActivateDeathIfNecessary(this.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_RIGHT));
    SpellUtils.ActivateDeathIfNecessary(this.m_magneticPlayData.m_targetMech.GetActorSpell(SpellType.MAGNETIC_PLAY_LINKED_LEFT));
    SpellUtils.ActivateDeathIfNecessary((Spell) this.m_magneticPlayData.m_beamSpell);
    if ((UnityEngine.Object) this.m_magneticPlayData.m_targetMech != (UnityEngine.Object) null)
      this.m_magneticPlayData.m_targetMech.SetIsMagneticTarget(false);
    this.m_magneticPlayData = (MagneticPlayData) null;
  }

  private void OnActorChanged(Actor oldActor)
  {
    this.HideTooltip();
    bool flag1 = false;
    bool flag2 = GameState.Get().IsGameCreating();
    bool flag3 = this.m_entity.IsMinion() && this.m_entity.HasTag(GAME_TAG.LETTUCE_CONTROLLER);
    if ((UnityEngine.Object) this.m_prevZone == (UnityEngine.Object) null && this.m_zone is ZoneGraveyard)
    {
      if ((UnityEngine.Object) oldActor != (UnityEngine.Object) null && (UnityEngine.Object) oldActor != (UnityEngine.Object) this.m_actor)
        oldActor.Destroy();
      if (this.IsShown())
        this.HideCard();
      else
        this.HideImpl();
      this.DeactivateHandStateSpells();
      flag1 = true;
      this.m_actorReady = true;
    }
    else if ((UnityEngine.Object) oldActor == (UnityEngine.Object) null)
    {
      bool flag4 = GameState.Get().IsMulliganPhaseNowOrPending();
      if (this.m_zone is ZoneHand && GameState.Get().IsBeginPhase())
      {
        bool flag5 = this.m_entity.GetCardId() == CoinManager.Get().GetFavoriteCoinCardId();
        if (flag4 && !GameState.Get().HasTheCoinBeenSpawned())
        {
          if (flag5)
          {
            GameState.Get().NotifyOfCoinSpawn();
            this.m_actor.TurnOffCollider();
            this.m_actor.Hide();
            this.m_actorReady = true;
            flag1 = true;
            this.transform.position = Vector3.zero;
            this.m_doNotWarpToNewZone = true;
            this.m_doNotSort = true;
          }
          else
          {
            Player controller = this.m_entity.GetController();
            if (controller.IsOpposingSide() && (UnityEngine.Object) this == (UnityEngine.Object) this.m_zone.GetLastCard() && !controller.HasTag(GAME_TAG.FIRST_PLAYER))
            {
              GameState.Get().NotifyOfCoinSpawn();
              this.m_actor.TurnOffCollider();
              this.m_actorReady = true;
              flag1 = true;
            }
          }
        }
        if (!flag5)
          ZoneMgr.Get().FindZoneOfType<ZoneDeck>(this.m_zone.m_Side).SetCardToInDeckState(this);
      }
      else if (flag2)
      {
        TransformUtil.CopyWorld((Component) this.transform, (Component) this.m_zone.transform);
        if (this.m_zone is ZonePlay || this.m_zone is ZoneHero || this.m_zone is ZoneHeroPower || this.m_zone is ZoneWeapon || this.m_zone is ZoneBattlegroundHeroBuddy || this.m_zone is ZoneBattlegroundQuestReward)
          this.ActivateLifetimeEffects();
      }
      else
      {
        if (!this.m_doNotWarpToNewZone)
          TransformUtil.CopyWorld((Component) this.transform, (Component) this.m_zone.transform);
        if (this.m_zone is ZoneHand)
        {
          if (!this.m_doNotWarpToNewZone)
          {
            ZoneHand zone = (ZoneHand) this.m_zone;
            this.transform.localScale = zone.GetCardScale();
            this.transform.localEulerAngles = zone.GetCardRotation(this);
            this.transform.position = zone.GetCardPosition(this);
          }
          Entity entity = GameState.Get().GetEntity(this.m_entity.GetTag(GAME_TAG.CREATOR));
          if (this.m_entity.HasTag(GAME_TAG.CREATOR) && entity != null && entity.IsTwinspell())
          {
            this.m_transitionStyle = ZoneTransitionStyle.INSTANT;
            this.ActivateHandSpawnSpell();
            InputManager.Get().GetFriendlyHand().ActivateTwinspellSpellDeath();
            InputManager.Get().GetFriendlyHand().ClearReservedCard();
          }
          else
          {
            this.m_actorReady = true;
            this.m_shown = true;
            if (!this.m_doNotWarpToNewZone)
            {
              this.m_actor.Hide();
              this.ActivateHandSpawnSpell();
              flag1 = true;
            }
          }
        }
        if ((UnityEngine.Object) this.m_prevZone == (UnityEngine.Object) null && this.m_zone is ZonePlay zone1)
        {
          if (!this.m_doNotWarpToNewZone)
            this.transform.position = zone1.GetCardPosition(this);
          if (this.m_cardDef.CardDef.m_SuppressPlaySoundsDuringMulligan && GameState.Get().IsMulliganPhaseNowOrPending())
            this.SuppressPlaySounds(true);
          if (this.m_entity.HasTag(GAME_TAG.LINKED_ENTITY))
          {
            if ((bool) (UnityEngine.Object) this.m_customSpawnSpellOverride)
            {
              this.ActivateMinionSpawnEffects();
            }
            else
            {
              this.m_transitionStyle = ZoneTransitionStyle.INSTANT;
              this.transform.position = Board.Get().FindBone("SpawnOffscreen").position;
              this.ActivateCharacterPlayEffects();
              this.OnSpellFinished_StandardSpawnCharacter((Spell) null, (object) null);
            }
          }
          else
          {
            this.m_actor.Hide();
            this.ActivateMinionSpawnEffects();
          }
          flag1 = true;
        }
        else if (!flag4 && (this.m_zone is ZoneHeroPower || this.m_zone is ZoneWeapon || this.m_zone is ZoneBattlegroundHeroBuddy || this.m_zone is ZoneBattlegroundQuestReward))
        {
          if (this.IsShown())
          {
            this.ActivatePlaySpawnEffects_HeroPowerOrWeaponOrHeroBuddy();
            flag1 = true;
            this.m_actorReady = true;
          }
        }
        else if ((UnityEngine.Object) this.m_prevZone == (UnityEngine.Object) null && this.m_zone is ZoneHero)
        {
          Entity entity = this.m_entity;
          if (entity.HasTag(GAME_TAG.TREAT_AS_PLAYED_HERO_CARD))
          {
            Card oldHeroCard = HeroCustomSummonSpell.GetOldHeroCard(entity.GetCard());
            if ((UnityEngine.Object) oldHeroCard != (UnityEngine.Object) null)
            {
              entity.GetCard().GetActor().Hide();
              HeroCustomSummonSpell.HideStats(oldHeroCard);
              oldHeroCard.SetDelayBeforeHideInNullZoneVisuals(0.8f);
            }
            this.ActivateStandardSpawnHeroSpell();
            flag1 = true;
          }
        }
      }
    }
    else if ((UnityEngine.Object) this.m_prevZone == (UnityEngine.Object) null && (this.m_zone is ZoneHeroPower || this.m_zone is ZoneWeapon || this.m_zone is ZoneBattlegroundHeroBuddy || this.m_zone is ZoneBattlegroundQuestReward))
    {
      oldActor.Destroy();
      TransformUtil.CopyWorld((Component) this.transform, (Component) this.m_zone.transform);
      this.m_transitionStyle = ZoneTransitionStyle.INSTANT;
      this.ActivatePlaySpawnEffects_HeroPowerOrWeaponOrHeroBuddy();
      flag1 = true;
      this.m_actorReady = true;
    }
    else if ((UnityEngine.Object) this.m_prevZone == (UnityEngine.Object) null && this.m_zone is ZoneHand && (UnityEngine.Object) oldActor == (UnityEngine.Object) this.m_actor && !this.m_goingThroughDeathrattleReturnfromGraveyard)
    {
      this.ActivateHandStateSpells();
      flag1 = true;
      this.m_actorReady = true;
    }
    else if ((UnityEngine.Object) this.m_prevZone == (UnityEngine.Object) null && this.m_zone is ZonePlay && (UnityEngine.Object) oldActor == (UnityEngine.Object) this.m_actor)
    {
      this.ActivateMinionSpawnEffects();
      this.ShowCard();
      flag1 = true;
      this.m_actorReady = true;
    }
    else if (this.m_prevZone is ZoneHand && (this.m_zone is ZonePlay || this.m_zone is ZoneHero))
    {
      if (this.m_entity.IsObfuscated())
      {
        flag1 = true;
        this.m_actorReady = true;
      }
      else
      {
        this.ActivateActorSpells_HandToPlay(oldActor);
        if (this.m_cardDef.CardDef.m_SuppressPlaySoundsOnSummon || this.m_entity.HasTag(GAME_TAG.CARD_DOES_NOTHING))
          this.SuppressPlaySounds(true);
        this.ActivateCharacterPlayEffects();
        this.m_actor.Hide();
        flag1 = true;
        if ((UnityEngine.Object) CardTypeBanner.Get() != (UnityEngine.Object) null && CardTypeBanner.Get().HasCardDef && CardTypeBanner.Get().HasSameCardDef(this.m_cardDef.CardDef))
          CardTypeBanner.Get().Hide();
        if (this.m_entity.IsMinion())
          this.m_prevZone.GetController().GetHeroCard().ActivateLegendaryHeroAnimEvent("OnSummonMinion");
      }
    }
    else if (this.m_prevZone is ZoneHand && this.m_zone is ZoneWeapon)
    {
      if (this.ActivateActorSpells_HandToWeapon(oldActor))
      {
        this.m_actor.Hide();
        flag1 = true;
        if ((UnityEngine.Object) CardTypeBanner.Get() != (UnityEngine.Object) null && CardTypeBanner.Get().HasCardDef && CardTypeBanner.Get().HasSameCardDef(this.m_cardDef.CardDef))
          CardTypeBanner.Get().Hide();
      }
    }
    else if ((this.m_prevZone is ZonePlay || this.m_prevZone is ZoneHero) && this.m_zone is ZoneHand)
    {
      this.DeactivateLifetimeEffects();
      if (this.m_mousedOver && this.m_entity.IsControlledByFriendlySidePlayer())
      {
        if (this.m_entity.HasSpellPower())
          ZoneMgr.Get().OnSpellPowerEntityMousedOut(this.m_entity.GetSpellPowerSchool());
        if (this.m_entity.HasHealingDoesDamageHint())
          ZoneMgr.Get().OnHealingDoesDamageEntityMousedOut();
      }
      bool useFastAnimations = GameState.Get().GetGameEntity().GetTag(GAME_TAG.USE_FAST_ACTOR_TRANSITION_ANIMATIONS) > 0;
      if (this.DoPlayToHandTransition(oldActor, useFastAnimations: useFastAnimations))
        flag1 = true;
    }
    else if (this.m_prevZone is ZoneHero && this.m_zone is ZoneGraveyard)
    {
      oldActor.DoCardDeathVisuals();
      this.DeactivateCustomKeywordEffect();
      flag1 = true;
      this.m_actorReady = true;
    }
    else if ((UnityEngine.Object) this.m_prevZone != (UnityEngine.Object) null && (this.m_prevZone is ZonePlay || this.m_prevZone is ZoneWeapon || this.m_prevZone is ZoneHeroPower || this.m_prevZone is ZoneBattlegroundHeroBuddy || this.m_prevZone is ZoneBattlegroundQuestReward) && this.m_zone is ZoneGraveyard)
    {
      if (this.m_mousedOver && this.m_entity.IsControlledByFriendlySidePlayer() && this.m_prevZone is ZonePlay)
      {
        if (this.m_entity.HasSpellPower())
          ZoneMgr.Get().OnSpellPowerEntityMousedOut(this.m_entity.GetSpellPowerSchool());
        if (this.m_entity.HasHealingDoesDamageHint())
          ZoneMgr.Get().OnHealingDoesDamageEntityMousedOut();
      }
      if (this.m_entity.HasTag(GAME_TAG.DEATHRATTLE_RETURN_ZONE) && this.DoesCardReturnFromGraveyard())
      {
        this.m_playZoneBlockerSide = new Player.Side?(this.m_prevZone.m_Side);
        if (!this.m_entity.IsWeapon())
          this.m_prevZone.AddLayoutBlocker();
        this.m_goingThroughDeathrattleReturnfromGraveyard = true;
        TAG_ZONE tag = this.m_entity.GetTag<TAG_ZONE>(GAME_TAG.DEATHRATTLE_RETURN_ZONE);
        int futureController = this.GetCardFutureController();
        Zone zoneForTags = ZoneMgr.Get().FindZoneForTags(futureController, tag, this.m_entity.GetCardType(), this.m_entity);
        if (zoneForTags is ZoneDeck)
          zoneForTags.AddLayoutBlocker();
        this.m_actorWaitingToBeReplaced = oldActor;
        this.m_actor.Hide();
        flag1 = true;
        this.m_actorReady = true;
      }
      else if (this.HandlePlayActorDeath(oldActor))
        flag1 = true;
    }
    else if (this.m_prevZone is ZoneDeck && this.m_zone is ZoneHand)
    {
      if (this.m_zone.m_Side == Player.Side.FRIENDLY)
      {
        if (GameState.Get().IsPastBeginPhase())
        {
          this.m_actorWaitingToBeReplaced = oldActor;
          this.m_cardStandInInteractive = false;
          if (!TurnStartManager.Get().IsCardDrawHandled(this))
            this.DrawFriendlyCard();
          flag1 = true;
        }
        else
        {
          this.m_actor.TurnOffCollider();
          this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
        }
      }
      else if (GameState.Get().IsPastBeginPhase())
      {
        if ((UnityEngine.Object) oldActor != (UnityEngine.Object) null)
          oldActor.Destroy();
        this.DrawOpponentCard();
        flag1 = true;
      }
    }
    else if (this.m_prevZone is ZoneSecret && this.m_zone is ZoneGraveyard && GameState.Get().GetGameEntity().HasTag(GAME_TAG.COIN_MANA_GEM) && this.m_entity.IsSecretLike())
    {
      flag1 = true;
      this.m_actorReady = true;
      this.m_shown = false;
      this.m_actor.Hide();
    }
    else if (this.m_prevZone is ZoneSecret && this.m_zone is ZoneGraveyard && this.m_entity.IsSecret())
    {
      flag1 = true;
      this.m_actorReady = true;
      if ((bool) UniversalInputManager.UsePhoneUI)
      {
        this.m_shown = false;
        this.m_actor.Hide();
      }
      else
        this.ShowSecretDeath(oldActor);
    }
    else if (this.m_prevZone is ZoneSecret && this.m_zone is ZoneGraveyard && this.m_entity.IsSigil())
    {
      flag1 = true;
      this.m_actorReady = true;
      this.m_shown = false;
      if ((UnityEngine.Object) oldActor != (UnityEngine.Object) null && (UnityEngine.Object) oldActor != (UnityEngine.Object) this.m_actor)
        oldActor.Destroy();
      this.m_actor.Hide();
    }
    else if (this.m_prevZone is ZoneSecret && this.m_zone is ZoneGraveyard && this.m_entity.IsObjective())
    {
      flag1 = true;
      this.m_actorReady = true;
      oldActor.GetComponent<Spell>().SafeActivateState(SpellStateType.DEATH);
    }
    else if (this.m_prevZone is ZoneGraveyard && this.m_zone is ZonePlay)
    {
      this.m_actor.Hide();
      this.StartCoroutine(this.ActivateReviveSpell());
      flag1 = true;
    }
    else if (this.m_prevZone is ZoneDeck && this.m_zone is ZoneGraveyard)
    {
      this.MillCard();
      flag1 = true;
    }
    else if (this.m_prevZone is ZoneDeck && this.m_zone is ZonePlay && !flag3)
    {
      if ((UnityEngine.Object) oldActor != (UnityEngine.Object) null)
        oldActor.Destroy();
      this.AnimateDeckToPlay();
      flag1 = true;
    }
    else if (this.m_prevZone is ZonePlay && this.m_zone is ZoneDeck)
    {
      this.DeactivateLifetimeEffects();
      this.m_playZoneBlockerSide = new Player.Side?(this.m_prevZone.m_Side);
      this.m_prevZone.AddLayoutBlocker();
      ZoneMgr.Get().FindZoneOfType<ZoneDeck>(this.m_zone.m_Side).AddLayoutBlocker();
      this.DoPlayToDeckTransition(oldActor);
      flag1 = true;
    }
    else if (this.m_prevZone is ZoneHand && this.m_zone is ZoneDeck && GameState.Get().IsPastBeginPhase())
    {
      if (!this.m_suppressHandToDeckTransition)
      {
        this.StartCoroutine(this.DoHandToDeckTransition(oldActor));
        if (oldActor.GetEntity() != null && oldActor.GetEntity().HasTag(GAME_TAG.IS_USING_TRADE_OPTION))
          this.ActivateCharacterTradeEffects();
      }
      else
      {
        oldActor.Destroy();
        this.m_actorReady = true;
      }
      this.m_suppressHandToDeckTransition = false;
      flag1 = true;
    }
    else if (this.m_goingThroughDeathrattleReturnfromGraveyard && this.m_zone is ZoneDeck)
    {
      this.m_goingThroughDeathrattleReturnfromGraveyard = false;
      if (this.HandleGraveyardToDeck(oldActor))
        flag1 = true;
    }
    else if (this.m_goingThroughDeathrattleReturnfromGraveyard && this.m_zone is ZoneHand)
    {
      this.m_goingThroughDeathrattleReturnfromGraveyard = false;
      if (this.HandleGraveyardToHand(oldActor))
        flag1 = true;
    }
    else if (this.m_zone is ZoneLettuceAbility)
    {
      this.ActivateStateSpells();
      this.m_actorReady = true;
      flag1 = true;
    }
    if (!flag1 && this.m_entity.IsMercenary() && this.m_zone is ZonePlay)
    {
      if (flag2)
      {
        this.ShowCard();
        this.ActivateStateSpells();
        this.m_actorReady = true;
      }
      else if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) oldActor && (UnityEngine.Object) this.m_prevZone != (UnityEngine.Object) null && this.m_prevZone.m_Side != this.m_zone.m_Side)
      {
        this.m_actorReady = true;
      }
      else
      {
        if ((UnityEngine.Object) oldActor != (UnityEngine.Object) null)
          oldActor.Destroy();
        this.m_actor.Hide();
        this.m_shown = true;
        ZonePlay zone = (ZonePlay) this.m_zone;
        this.SetTransitionStyle(ZoneTransitionStyle.INSTANT);
        zone.UpdateLayout();
        this.ActivateMinionSpawnEffects();
      }
    }
    else if (!flag1 && (UnityEngine.Object) oldActor == (UnityEngine.Object) this.m_actor)
    {
      if ((UnityEngine.Object) this.m_prevZone != (UnityEngine.Object) null && this.m_prevZone.m_Side != this.m_zone.m_Side && this.m_prevZone is ZoneSecret && this.m_zone is ZoneSecret)
      {
        this.StartCoroutine(this.SwitchSecretSides());
        flag1 = true;
      }
      if (flag1)
        return;
      this.m_actorReady = true;
    }
    else
    {
      if (!flag1 && this.m_zone is ZoneSecret)
      {
        this.m_shown = true;
        if ((bool) (UnityEngine.Object) oldActor)
        {
          oldActor.Destroy();
          if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null)
            this.m_questRewardActor.Destroy();
        }
        this.m_transitionStyle = ZoneTransitionStyle.INSTANT;
        this.m_zone.UpdateLayout();
        this.ShowSecretQuestBirth();
        flag1 = true;
        this.m_actorReady = true;
        if (flag2)
          this.ActivateStateSpells();
      }
      if (flag1)
        return;
      if ((bool) (UnityEngine.Object) oldActor)
        oldActor.Destroy();
      bool flag6 = this.m_zone.m_ServerTag == TAG_ZONE.PLAY || this.m_zone.m_ServerTag == TAG_ZONE.SECRET || this.m_zone.m_ServerTag == TAG_ZONE.HAND;
      if (this.IsShown() & flag6)
        this.ActivateStateSpells();
      this.m_actorReady = true;
      if (this.IsShown())
        this.ShowImpl();
      else
        this.HideImpl();
    }
  }

  private bool HandleGraveyardToDeck(Actor oldActor)
  {
    if (!(bool) (UnityEngine.Object) this.m_actorWaitingToBeReplaced)
      return false;
    if ((bool) (UnityEngine.Object) oldActor)
      oldActor.Destroy();
    oldActor = this.m_actorWaitingToBeReplaced;
    this.m_actorWaitingToBeReplaced = (Actor) null;
    this.DoPlayToDeckTransition(oldActor);
    return true;
  }

  private bool HandleGraveyardToHand(Actor oldActor)
  {
    if ((bool) (UnityEngine.Object) this.m_actorWaitingToBeReplaced)
    {
      if ((bool) (UnityEngine.Object) oldActor && (UnityEngine.Object) oldActor != (UnityEngine.Object) this.m_actor)
        oldActor.Destroy();
      oldActor = this.m_actorWaitingToBeReplaced;
      this.m_actorWaitingToBeReplaced = (Actor) null;
      bool useFastAnimations = GameState.Get().GetGameEntity().GetTag(GAME_TAG.USE_FAST_ACTOR_TRANSITION_ANIMATIONS) > 0;
      if (this.DoPlayToHandTransition(oldActor, true, useFastAnimations))
        return true;
    }
    return false;
  }

  public bool CardStandInIsInteractive() => this.m_cardStandInInteractive;

  private void ReadyCardForDraw() => this.GetController().GetDeckZone().SetCardToInDeckState(this);

  public void DrawFriendlyCard() => this.StartCoroutine(this.DrawFriendlyCardWithTiming());

  private IEnumerator DrawFriendlyCardWithTiming()
  {
    Card card = this;
    card.m_doNotSort = true;
    card.m_transitionStyle = ZoneTransitionStyle.SLOW;
    card.m_actor.Hide();
    while ((bool) (UnityEngine.Object) GameState.Get().GetFriendlyCardBeingDrawn())
      yield return (object) null;
    ZoneDeck deck = GameState.Get().GetFriendlySidePlayer().GetDeckZone();
    deck.NotifyCardAnimationStart();
    GameState.Get().SetFriendlyCardBeingDrawn(card);
    card.ReadyCardForDraw();
    Actor cardDrawStandIn = Gameplay.Get().GetCardDrawStandIn();
    cardDrawStandIn.transform.parent = card.m_actor.transform.parent;
    cardDrawStandIn.transform.localPosition = Vector3.zero;
    cardDrawStandIn.transform.localScale = Vector3.one;
    cardDrawStandIn.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180f);
    cardDrawStandIn.Show();
    cardDrawStandIn.GetRootObject().GetComponentInChildren<CardBackDisplay>().SetCardBack(CardBackManager.CardBackSlot.FRIENDLY);
    if ((UnityEngine.Object) card.m_actorWaitingToBeReplaced != (UnityEngine.Object) null)
    {
      card.m_actorWaitingToBeReplaced.Destroy();
      card.m_actorWaitingToBeReplaced = (Actor) null;
    }
    card.DetermineIfOverrideDrawTimeScale();
    Transform bone = Board.Get().FindBone("FriendlyDrawCard");
    Vector3[] vector3Array = new Vector3[3]
    {
      card.gameObject.transform.position,
      card.gameObject.transform.position + Card.ABOVE_DECK_OFFSET,
      bone.position
    };
    float num1 = 1.5f * card.m_drawTimeScale.Value;
    iTween.MoveTo(card.gameObject, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) num1, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
    card.gameObject.transform.localEulerAngles = new Vector3(270f, 270f, 0.0f);
    Vector3 vector3 = new Vector3(0.0f, 0.0f, 357f);
    float num2 = 1.35f * card.m_drawTimeScale.Value;
    float num3 = 0.15f * card.m_drawTimeScale.Value;
    iTween.RotateTo(card.gameObject, iTween.Hash((object) "rotation", (object) vector3, (object) "time", (object) num2, (object) "delay", (object) num3));
    float num4 = 0.75f * card.m_drawTimeScale.Value;
    float num5 = 0.15f * card.m_drawTimeScale.Value;
    iTween.ScaleTo(card.gameObject, iTween.Hash((object) "scale", (object) bone.localScale, (object) "time", (object) num4, (object) "delay", (object) num5));
    SoundManager.Get().LoadAndPlay((AssetReference) "draw_card_1.prefab:19dd221ebfed9754e85ef1f104e0fddb", card.gameObject);
    cardDrawStandIn.transform.parent = (Transform) null;
    cardDrawStandIn.Hide();
    card.m_actor.Show();
    card.m_actor.TurnOffCollider();
    deck.UpdateLayout();
    PowerTask cardDrawBlockingTask = cardDrawBlockingTask = card.GetPowerTaskToBlockCardDraw();
    while (iTween.Count(card.gameObject) > 0)
      yield return (object) null;
    card.m_actorReady = true;
    if (card.ShouldCardDrawWaitForTurnStartSpells())
      yield return (object) card.StartCoroutine(card.WaitForCardDrawBlockingTurnStartSpells());
    else if (cardDrawBlockingTask != null)
    {
      while (!cardDrawBlockingTask.IsCompleted())
        yield return (object) null;
    }
    card.m_doNotSort = false;
    GameState.Get().ClearCardBeingDrawn(card);
    card.ResetCardDrawTimeScale();
    deck.NotifyCardAnimationFinish();
    if ((UnityEngine.Object) card.m_zone != (UnityEngine.Object) null && card.m_zone is ZoneHand)
    {
      ZoneHand handZone = (ZoneHand) card.m_zone;
      SoundManager.Get().LoadAndPlay((AssetReference) "add_card_to_hand_1.prefab:bf6b149b859734c4faf9a96356c53646", card.gameObject);
      card.ActivateStateSpells();
      card.RefreshActor();
      card.m_zone.UpdateLayout();
      yield return (object) new WaitForSeconds(0.3f);
      card.m_cardStandInInteractive = true;
      handZone.MakeStandInInteractive(card);
      handZone = (ZoneHand) null;
    }
  }

  public bool IsBeingDrawnByOpponent() => this.m_beingDrawnByOpponent;

  private void DrawOpponentCard() => this.StartCoroutine(this.DrawOpponentCardWithTiming());

  private IEnumerator DrawOpponentCardWithTiming()
  {
    Card card = this;
    card.m_doNotSort = true;
    card.m_beingDrawnByOpponent = true;
    card.m_actor.Hide();
    while ((bool) (UnityEngine.Object) GameState.Get().GetOpponentCardBeingDrawn())
      yield return (object) null;
    if (card.GetZonePosition() == 0)
      yield return (object) null;
    card.m_actor.Show();
    GameState.Get().SetOpponentCardBeingDrawn(card);
    card.ReadyCardForDraw();
    ZoneHand zone = (ZoneHand) card.m_zone;
    zone.UpdateLayout();
    if (card.m_entity.HasTag(GAME_TAG.REVEALED))
      card.StartCoroutine(card.DrawKnownOpponentCard(zone));
    else
      card.StartCoroutine(card.DrawUnknownOpponentCard(zone));
  }

  private IEnumerator DrawUnknownOpponentCard(ZoneHand handZone)
  {
    Card card = this;
    SoundManager.Get().LoadAndPlay((AssetReference) "draw_card_and_add_to_hand_opp_1.prefab:5a05fbb2c5833a94182e1b454647d5c8", card.gameObject);
    card.gameObject.transform.rotation = Card.IN_DECK_HIDDEN_ROTATION;
    card.DetermineIfOverrideDrawTimeScale();
    Transform bone = Board.Get().FindBone("OpponentDrawCard");
    Vector3[] vector3Array = new Vector3[4]
    {
      card.gameObject.transform.position,
      card.gameObject.transform.position + Card.ABOVE_DECK_OFFSET,
      bone.position,
      handZone.GetCardPosition(card)
    };
    float num1 = 1.75f * card.m_drawTimeScale.Value;
    iTween.MoveTo(card.gameObject, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) num1, (object) "easetype", (object) iTween.EaseType.easeInOutQuart));
    float num2 = 0.7f * card.m_drawTimeScale.Value;
    float num3 = 0.8f * card.m_drawTimeScale.Value;
    iTween.RotateTo(card.gameObject, iTween.Hash((object) "rotation", (object) handZone.GetCardRotation(card), (object) "time", (object) num2, (object) "delay", (object) num3, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
    float num4 = 0.7f * card.m_drawTimeScale.Value;
    float num5 = 0.8f * card.m_drawTimeScale.Value;
    iTween.ScaleTo(card.gameObject, iTween.Hash((object) "scale", (object) handZone.GetCardScale(), (object) "time", (object) num4, (object) "delay", (object) num5, (object) "easetype", (object) iTween.EaseType.easeInOutQuint));
    GameState.Get().GetOpposingSidePlayer().GetDeckZone().UpdateLayout();
    yield return (object) new WaitForSeconds(0.2f);
    card.m_actorReady = true;
    yield return (object) new WaitForSeconds(0.6f);
    GameState.Get().UpdateOptionHighlights();
    while (iTween.Count(card.gameObject) > 0)
      yield return (object) null;
    card.m_doNotSort = false;
    card.m_beingDrawnByOpponent = false;
    GameState.Get().SetOpponentCardBeingDrawn((Card) null);
    card.ResetCardDrawTimeScale();
    handZone.UpdateLayout();
  }

  private IEnumerator DrawKnownOpponentCard(ZoneHand handZone)
  {
    Card card = this;
    Actor handActor = (Actor) null;
    bool loadingActor = true;
    PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      loadingActor = false;
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Error.AddDevFatal("Card.DrawKnownOpponentCard() - failed to load {0}", (object) assetRef);
      }
      else
      {
        handActor = go.GetComponent<Actor>();
        if (!((UnityEngine.Object) handActor == (UnityEngine.Object) null))
          return;
        Error.AddDevFatal("Card.DrawKnownOpponentCard() - instance of {0} has no Actor component", (object) this.name);
      }
    });
    string actorPath = ActorNames.GetHandActor(card.m_entity);
    AssetLoader.Get().InstantiatePrefab((AssetReference) actorPath, callback, options: AssetLoadingOptions.IgnorePrefabPosition);
    while (loadingActor)
      yield return (object) null;
    if ((bool) (UnityEngine.Object) handActor)
    {
      handActor.SetEntity(card.m_entity);
      handActor.SetCardDef(card.m_cardDef);
      handActor.UpdateAllComponents();
      card.StartCoroutine(card.RevealDrawnOpponentCard(actorPath, handActor, handZone));
    }
    else
      card.StartCoroutine(card.DrawUnknownOpponentCard(handZone));
  }

  private IEnumerator RevealDrawnOpponentCard(
    string handActorPath,
    Actor handActor,
    ZoneHand handZone)
  {
    Card card = this;
    SoundManager.Get().LoadAndPlay((AssetReference) "draw_card_1.prefab:19dd221ebfed9754e85ef1f104e0fddb", card.gameObject);
    handActor.transform.parent = card.m_actor.transform.parent;
    TransformUtil.CopyLocal((Component) handActor, (Component) card.m_actor);
    card.m_actor.Hide();
    card.DetermineIfOverrideDrawTimeScale();
    card.gameObject.transform.localEulerAngles = new Vector3(270f, 90f, 0.0f);
    string name = "OpponentDrawCardAndReveal";
    if ((bool) UniversalInputManager.UsePhoneUI)
      name += "_phone";
    Transform bone = Board.Get().FindBone(name);
    Vector3[] vector3Array = new Vector3[3]
    {
      card.gameObject.transform.position,
      card.gameObject.transform.position + Card.ABOVE_DECK_OFFSET,
      bone.position
    };
    float num1 = 1.75f * card.m_drawTimeScale.Value;
    iTween.MoveTo(card.gameObject, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) num1, (object) "easetype", (object) iTween.EaseType.easeInOutQuart));
    float num2 = 0.7f * card.m_drawTimeScale.Value;
    float num3 = 0.8f * card.m_drawTimeScale.Value;
    iTween.RotateTo(card.gameObject, iTween.Hash((object) "rotation", (object) bone.eulerAngles, (object) "time", (object) num2, (object) "delay", (object) num3, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
    float num4 = 0.7f * card.m_drawTimeScale.Value;
    float num5 = 0.8f * card.m_drawTimeScale.Value;
    iTween.ScaleTo(card.gameObject, iTween.Hash((object) "scale", (object) bone.localScale, (object) "time", (object) num4, (object) "delay", (object) num5, (object) "easetype", (object) iTween.EaseType.easeInOutQuint));
    GameState.Get().GetOpposingSidePlayer().GetDeckZone().UpdateLayout();
    yield return (object) new WaitForSeconds(1.75f);
    card.m_actorReady = true;
    card.m_beingDrawnByOpponent = false;
    string actorName = card.m_actorPath;
    card.m_actorWaitingToBeReplaced = card.m_actor;
    card.m_actorPath = handActorPath;
    card.m_actor = handActor;
    PowerTask cardDrawBlockingTask = card.GetPowerTaskToBlockCardDraw();
    if (cardDrawBlockingTask != null)
    {
      while (!cardDrawBlockingTask.IsCompleted())
        yield return (object) null;
      if ((UnityEngine.Object) handActor == (UnityEngine.Object) null)
        handActor = card.m_actor;
    }
    if (card.m_entity.GetZone() != TAG_ZONE.HAND)
    {
      card.m_doNotSort = false;
      GameState.Get().ClearCardBeingDrawn(card);
      card.ResetCardDrawTimeScale();
    }
    else
    {
      card.m_actor = card.m_actorWaitingToBeReplaced;
      card.m_actorPath = actorName;
      card.m_actorWaitingToBeReplaced = (Actor) null;
      card.m_beingDrawnByOpponent = true;
      yield return (object) card.StartCoroutine(card.HideRevealedOpponentCard(handActor));
    }
  }

  private IEnumerator HideRevealedOpponentCard(Actor handActor)
  {
    Card card = this;
    float seconds = 0.5f;
    float num1 = 0.525f * seconds;
    if (!card.GetController().IsRevealed())
    {
      float num2 = 180f;
      TransformUtil.SetEulerAngleZ(card.m_actor.gameObject, -num2);
      if ((UnityEngine.Object) handActor != (UnityEngine.Object) null)
        iTween.RotateAdd(handActor.gameObject, iTween.Hash((object) "z", (object) num2, (object) "time", (object) seconds, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
      iTween.RotateAdd(card.m_actor.gameObject, iTween.Hash((object) "z", (object) num2, (object) "time", (object) seconds, (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
    }
    Action<object> action = (Action<object>) (obj =>
    {
      if ((UnityEngine.Object) handActor != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) handActor.gameObject);
      this.m_actor.Show();
    });
    iTween.Timer(card.m_actor.gameObject, iTween.Hash((object) "time", (object) num1, (object) "oncomplete", (object) action));
    yield return (object) new WaitForSeconds(seconds);
    card.m_doNotSort = false;
    card.m_beingDrawnByOpponent = false;
    GameState.Get().SetOpponentCardBeingDrawn((Card) null);
    card.ResetCardDrawTimeScale();
    SoundManager.Get().LoadAndPlay((AssetReference) "add_card_to_hand_1.prefab:bf6b149b859734c4faf9a96356c53646", card.gameObject);
    card.ActivateStateSpells();
    card.RefreshActor();
    card.m_zone.UpdateLayout();
  }

  private void AnimateDeckToPlay()
  {
    if ((UnityEngine.Object) this.m_customSpawnSpellOverride == (UnityEngine.Object) null)
    {
      this.m_zone.AddLayoutBlocker();
      ZoneDeck zoneDeck = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(this.m_zone.m_Side);
      if (this.m_latestZoneChange != null && this.m_latestZoneChange.GetSourceControllerId() != 0 && this.m_latestZoneChange.GetSourceControllerId() != this.m_latestZoneChange.GetDestinationControllerId() && this.m_latestZoneChange.GetSourceZone() is ZoneDeck)
        zoneDeck = (ZoneDeck) this.m_latestZoneChange.GetSourceZone();
      zoneDeck.SetCardToInDeckState(this);
      this.m_doNotSort = true;
      GameObject actorObject1 = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(this.m_entity), AssetLoadingOptions.IgnorePrefabPosition);
      Actor component1 = actorObject1.GetComponent<Actor>();
      this.SetupDeckToPlayActor(component1, actorObject1);
      SpellType outSpellHandToPlay = this.m_cardDef.CardDef.DetermineSummonOutSpell_HandToPlay(this);
      Spell spell = component1.GetSpell(outSpellHandToPlay);
      GameObject actorObject2 = AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", AssetLoadingOptions.IgnorePrefabPosition);
      Actor component2 = actorObject2.GetComponent<Actor>();
      this.SetupDeckToPlayActor(component2, actorObject2);
      this.StartCoroutine(this.AnimateDeckToPlay(component1, spell, component2));
    }
    else
    {
      this.m_actor.Hide();
      ZonePlay zone = (ZonePlay) this.m_zone;
      this.SetTransitionStyle(ZoneTransitionStyle.INSTANT);
      zone.UpdateLayout();
      this.ActivateMinionSpawnEffects();
    }
  }

  private void SetupDeckToPlayActor(Actor actor, GameObject actorObject)
  {
    actor.SetEntity(this.m_entity);
    actor.SetCardDef(this.m_cardDef);
    actor.UpdateAllComponents();
    actorObject.transform.parent = this.transform;
    actorObject.transform.localPosition = Vector3.zero;
    actorObject.transform.localScale = Vector3.one;
    actorObject.transform.localRotation = Quaternion.identity;
  }

  private IEnumerator AnimateDeckToPlay(
    Actor cardFaceActor,
    Spell outSpell,
    Actor hiddenActor)
  {
    Card card = this;
    ZoneDeck zoneDeck = card.m_prevZone as ZoneDeck;
    zoneDeck?.NotifyCardAnimationStart();
    cardFaceActor.Hide();
    card.m_actor.Hide();
    hiddenActor.Hide();
    card.m_inputEnabled = false;
    SoundManager.Get().LoadAndPlay((AssetReference) "draw_card_into_play.prefab:52139cc25c53e184fab47b23c72df0d1", card.gameObject);
    card.gameObject.transform.localEulerAngles = new Vector3(270f, 90f, 0.0f);
    iTween.MoveTo(card.gameObject, card.gameObject.transform.position + Card.ABOVE_DECK_OFFSET, 0.6f);
    iTween.RotateTo(card.gameObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) 0.7f, (object) "delay", (object) 0.6f, (object) "easetype", (object) iTween.EaseType.easeInOutCubic, (object) "islocal", (object) true));
    hiddenActor.Show();
    yield return (object) new WaitForSeconds(0.4f);
    zoneDeck?.NotifyCardAnimationFinish();
    iTween.MoveTo(hiddenActor.gameObject, iTween.Hash((object) "position", (object) new Vector3(0.0f, 3f, 0.0f), (object) "time", (object) 1f, (object) "delay", (object) 0.0f, (object) "islocal", (object) true));
    card.m_doNotSort = false;
    ZonePlay zone = (ZonePlay) card.m_zone;
    zone.RemoveLayoutBlocker();
    zone.SetTransitionTime(1.6f);
    zone.UpdateLayout();
    yield return (object) new WaitForSeconds(0.2f);
    float cardFlipTime = 0.35f;
    iTween.RotateTo(hiddenActor.gameObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, -90f), (object) "time", (object) cardFlipTime, (object) "delay", (object) 0.0f, (object) "easetype", (object) iTween.EaseType.easeInCubic, (object) "islocal", (object) true));
    yield return (object) new WaitForSeconds(cardFlipTime);
    hiddenActor.Destroy();
    cardFaceActor.Show();
    cardFaceActor.gameObject.transform.localPosition = new Vector3(0.0f, 3f, 0.0f);
    cardFaceActor.gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, 90f));
    iTween.RotateTo(cardFaceActor.gameObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 0.0f), (object) "time", (object) cardFlipTime, (object) "delay", (object) 0.0f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "islocal", (object) true));
    card.m_actor.gameObject.transform.localPosition = new Vector3(0.0f, 2.86f, 0.0f);
    cardFaceActor.gameObject.transform.localPosition = new Vector3(0.0f, 2.86f, 0.0f);
    iTween.MoveTo(hiddenActor.gameObject, iTween.Hash((object) "position", (object) Vector3.zero, (object) "time", (object) 1f, (object) "delay", (object) 0.0f, (object) "islocal", (object) true));
    card.ActivateSpell(outSpell, new Spell.FinishedCallback(card.OnSpellFinished_HandToPlay_SummonOut), (object) null, new Spell.StateFinishedCallback(card.OnSpellStateFinished_DestroyActor));
    card.ActivateCharacterPlayEffects();
    card.m_actor.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
  }

  public void SetSkipMilling(bool skipMilling) => this.m_skipMilling = skipMilling;

  private void MillCard()
  {
    if (this.m_skipMilling)
      this.m_actor.Hide();
    else
      this.StartCoroutine(this.MillCardWithTiming());
  }

  private IEnumerator MillCardWithTiming()
  {
    Card card = this;
    card.SetDoNotSort(true);
    card.ReadyCardForDraw();
    Player cardOwner = card.m_entity.GetController();
    string name;
    if (cardOwner.IsFriendlySide())
    {
      while ((bool) (UnityEngine.Object) GameState.Get().GetFriendlyCardBeingDrawn())
        yield return (object) null;
      GameState.Get().SetFriendlyCardBeingDrawn(card);
      name = "FriendlyMillCard";
    }
    else
    {
      while ((bool) (UnityEngine.Object) GameState.Get().GetOpponentCardBeingDrawn())
        yield return (object) null;
      GameState.Get().SetOpponentCardBeingDrawn(card);
      name = "OpponentMillCard";
    }
    int turn = GameState.Get().GetTurn();
    if (turn != GameState.Get().GetLastTurnRemindedOfFullHand() && cardOwner.GetHandZone().GetCardCount() >= 10)
    {
      GameState.Get().SetLastTurnRemindedOfFullHand(turn);
      cardOwner.GetHeroCard().PlayEmote(EmoteType.ERROR_HAND_FULL);
    }
    card.m_actor.Show();
    card.m_actor.TurnOffCollider();
    Transform bone = Board.Get().FindBone(name);
    Vector3[] vector3Array = new Vector3[3]
    {
      card.gameObject.transform.position,
      card.gameObject.transform.position + Card.ABOVE_DECK_OFFSET,
      bone.position
    };
    iTween.MoveTo(card.gameObject, iTween.Hash((object) "path", (object) vector3Array, (object) "time", (object) 1.5f, (object) "easetype", (object) iTween.EaseType.easeInSineOutExpo));
    card.gameObject.transform.localEulerAngles = new Vector3(270f, 270f, 0.0f);
    iTween.RotateTo(card.gameObject, iTween.Hash((object) "rotation", (object) new Vector3(0.0f, 0.0f, 357f), (object) "time", (object) 1.35f, (object) "delay", (object) 0.15f));
    iTween.ScaleTo(card.gameObject, iTween.Hash((object) "scale", (object) bone.localScale, (object) "time", (object) 0.75f, (object) "delay", (object) 0.15f));
    while (iTween.Count(card.gameObject) > 0)
      yield return (object) null;
    card.m_actorReady = true;
    card.RefreshActor();
    Spell spell = card.m_actor.GetSpell(SpellType.HANDFULL);
    spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(card.OnSpellStateFinished_DestroyActor));
    spell.Activate();
    GameState.Get().ClearCardBeingDrawn(card);
    card.SetDoNotSort(false);
  }

  private void ActivateActorSpells_HandToPlay(Actor oldActor)
  {
    if ((UnityEngine.Object) oldActor == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_HandToPlay() - oldActor=null", (object) this));
    else if (this.m_cardDef == null)
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_HandToPlay() - m_cardDef=null", (object) this));
    else if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_HandToPlay() - m_actor=null", (object) this));
    }
    else
    {
      this.DeactivateHandStateSpells(oldActor);
      SpellType outSpellHandToPlay = this.m_cardDef.CardDef.DetermineSummonOutSpell_HandToPlay(this);
      Spell spell = oldActor.GetSpell(outSpellHandToPlay);
      if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("{0}.ActivateActorSpells_HandToPlay() - outSpell=null outSpellType={1}", (object) this, (object) outSpellHandToPlay));
        this.m_actorReady = true;
      }
      else
      {
        bool standard;
        if ((UnityEngine.Object) this.GetBestSummonSpell(out standard) == (UnityEngine.Object) null)
        {
          Debug.LogError((object) string.Format("{0}.ActivateActorSpells_HandToPlay() - inSpell=null standard={1}", (object) this, (object) standard));
        }
        else
        {
          this.m_inputEnabled = false;
          spell.SetSource(this.gameObject);
          this.ActivateSpell(spell, new Spell.FinishedCallback(this.OnSpellFinished_HandToPlay_SummonOut), (object) oldActor, new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
        }
      }
    }
  }

  private void OnSpellFinished_HandToPlay_SummonOut(Spell spell, object userData)
  {
    Actor actor = userData as Actor;
    this.m_actor.Show();
    if (this.m_magneticPlayData != null)
    {
      SpellUtils.ActivateDeathIfNecessary(actor.GetSpellIfLoaded(SpellType.MAGNETIC_HAND_LINKED_RIGHT));
      this.ActivateActorSpell(SpellType.MAGNETIC_PLAY_LINKED_RIGHT);
    }
    bool standard;
    Spell bestSummonSpell = this.GetBestSummonSpell(out standard);
    if ((UnityEngine.Object) bestSummonSpell == (UnityEngine.Object) null)
    {
      Debug.LogErrorFormat("{0}.OnSpellFinished_HandToPlay_SummonOut() - inSpell=null standard={1}", (object) this, (object) standard);
    }
    else
    {
      if (!standard)
      {
        bestSummonSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_ReleaseSpell));
        SpellUtils.SetCustomSpellParent(bestSummonSpell, (Component) this.m_actor);
      }
      bestSummonSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_HandToPlay_SummonIn));
      bestSummonSpell.Activate();
    }
  }

  private void OnSpellFinished_HandToPlay_SummonIn(Spell spell, object userData)
  {
    Actor actor = this.GetActor();
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
    {
      GameObject rootObject = actor.GetRootObject();
      if ((UnityEngine.Object) rootObject != (UnityEngine.Object) null)
      {
        rootObject.transform.localPosition = Vector3.zero;
        rootObject.transform.localRotation = Quaternion.identity;
        rootObject.transform.localScale = Vector3.one;
      }
      actor.Show();
    }
    this.m_actorReady = true;
    this.m_inputEnabled = true;
    this.ActivateStateSpells();
    this.RefreshActor();
    if (this.m_entity.IsControlledByFriendlySidePlayer() && !this.m_entity.GetRealTimeIsDormant())
    {
      if (this.m_entity.HasSpellPower() || this.m_entity.HasSpellPowerDouble())
        ZoneMgr.Get().OnSpellPowerEntityEnteredPlay(this.m_entity.GetSpellPowerSchool());
      if (this.m_entity.HasHealingDoesDamageHint())
        ZoneMgr.Get().OnHealingDoesDamageEntityEnteredPlay();
      if (this.m_entity.HasLifestealDoesDamageHint())
        ZoneMgr.Get().OnLifestealDoesDamageEntityEnteredPlay();
    }
    if (this.m_entity.HasWindfury())
      this.ActivateActorSpell(SpellType.WINDFURY_BURST);
    this.StartCoroutine(this.ActivateActorBattlecrySpell());
    BoardEvents boardEvents = BoardEvents.Get();
    if (!((UnityEngine.Object) boardEvents != (UnityEngine.Object) null))
      return;
    boardEvents.SummonedEvent(this);
  }

  private bool ActivateActorSpells_HandToWeapon(Actor oldActor)
  {
    if ((UnityEngine.Object) oldActor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_HandToWeapon() - oldActor=null", (object) this));
      return false;
    }
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_HandToWeapon() - m_actor=null", (object) this));
      return false;
    }
    this.DeactivateHandStateSpells(oldActor);
    oldActor.SetActorState(ActorStateType.CARD_IDLE);
    SpellType spellType1 = SpellType.SUMMON_OUT_WEAPON;
    Spell spell = oldActor.GetSpell(spellType1);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_HandToWeapon() - outSpell=null outSpellType={1}", (object) this, (object) spellType1));
      return false;
    }
    Spell finishedUserData = this.m_customSummonSpell;
    if ((UnityEngine.Object) finishedUserData == (UnityEngine.Object) null)
    {
      SpellType spellType2 = this.m_entity.IsControlledByFriendlySidePlayer() ? SpellType.SUMMON_IN_FRIENDLY : SpellType.SUMMON_IN_OPPONENT;
      finishedUserData = this.GetActorSpell(spellType2);
      if ((UnityEngine.Object) finishedUserData == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("{0}.ActivateActorSpells_HandToWeapon() - inSpell=null inSpellType={1}", (object) this, (object) spellType2));
        return false;
      }
    }
    this.m_inputEnabled = false;
    this.ActivateSpell(spell, new Spell.FinishedCallback(this.OnSpellFinished_HandToWeapon_SummonOut), (object) finishedUserData, new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
    return true;
  }

  private void OnSpellFinished_HandToWeapon_SummonOut(Spell spell, object userData)
  {
    this.m_actor.Show();
    Spell spell1 = this.m_customSummonSpell;
    if ((UnityEngine.Object) spell1 == (UnityEngine.Object) null)
    {
      spell1 = (Spell) userData;
    }
    else
    {
      spell1.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_ReleaseSpell));
      SpellUtils.SetCustomSpellParent(spell1, (Component) this.m_actor);
    }
    this.ActivateSpell(spell1, new Spell.FinishedCallback(this.OnSpellFinished_StandardCardSummon));
  }

  private void DiscardCardBeingDrawn()
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) GameState.Get().GetOpponentCardBeingDrawn())
    {
      this.m_actorWaitingToBeReplaced.Destroy();
      this.m_actorWaitingToBeReplaced = (Actor) null;
    }
    if (this.m_actor.IsShown())
      this.ActivateDeathSpell(this.m_actor);
    else
      GameState.Get().ClearCardBeingDrawn(this);
  }

  private void DoDiscardAnimation()
  {
    ZoneHand prevZone = this.m_prevZone as ZoneHand;
    this.m_actor.SetBlockTextComponentUpdate(true);
    this.m_doNotSort = true;
    iTween.Stop(this.gameObject);
    float num = 3f;
    if (this.GetEntity().IsControlledByOpposingSidePlayer())
      num = -num;
    iTween.MoveTo(this.gameObject, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + num), 3f);
    Vector3 vector3 = this.transform.localScale;
    if ((UnityEngine.Object) prevZone != (UnityEngine.Object) null)
      vector3 = prevZone.GetCardScale();
    iTween.ScaleTo(this.gameObject, vector3 * 1.5f, 3f);
    this.StartCoroutine(this.ActivateGraveyardActorDeathSpellAfterDelay(1f, 4f));
  }

  private bool DoPlayToHandTransition(Actor oldActor, bool wasInGraveyard = false, bool useFastAnimations = false)
  {
    int num = this.ActivateActorSpells_PlayToHand(oldActor, wasInGraveyard, useFastAnimations) ? 1 : 0;
    if (num == 0)
      return num != 0;
    this.m_actor.Hide();
    return num != 0;
  }

  private bool ActivateActorSpells_PlayToHand(
    Actor oldActor,
    bool wasInGraveyard,
    bool useFastAnimations)
  {
    if ((UnityEngine.Object) oldActor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_PlayToHand() - oldActor=null", (object) this));
      return false;
    }
    if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_PlayToHand() - m_actor=null", (object) this));
      return false;
    }
    SpellType spellType1 = useFastAnimations ? SpellType.BOUNCE_OUT_FAST : SpellType.BOUNCE_OUT;
    Spell outSpell = oldActor.GetSpell(spellType1);
    if ((UnityEngine.Object) outSpell == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_PlayToHand() - outSpell=null outSpellType={1}", (object) this, (object) spellType1));
      return false;
    }
    SpellType spellType2 = SpellType.BOUNCE_IN;
    if (this.m_actor.UseTechLevelManaGem())
      spellType2 = SpellType.BOUNCE_IN_TECH_LEVEL;
    else if (useFastAnimations)
      spellType2 = SpellType.BOUNCE_IN_FAST;
    Spell inSpell = this.GetActorSpell(spellType2);
    if ((UnityEngine.Object) inSpell == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("{0}.ActivateActorSpells_PlayToHand() - inSpell=null inSpellType={1}", (object) this, (object) spellType2));
      return false;
    }
    this.m_inputEnabled = false;
    outSpell.SetSource(this.gameObject);
    if (this.m_entity.IsControlledByFriendlySidePlayer())
    {
      Spell.FinishedCallback finishedCallback = wasInGraveyard ? new Spell.FinishedCallback(this.OnSpellFinished_PlayToHand_SummonOut_FromGraveyard) : new Spell.FinishedCallback(this.OnSpellFinished_PlayToHand_SummonOut);
      if (!this.CancelCustomSummonSpell((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
      {
        if (prevStateType != SpellStateType.CANCEL)
          return;
        this.ActivateSpell(outSpell, finishedCallback, (object) inSpell, new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
      })))
        this.ActivateSpell(outSpell, finishedCallback, (object) inSpell, new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
    }
    else
    {
      if (this.m_entity.IsControlledByOpposingSidePlayer())
      {
        Log.FaceDownCard.Print("Card.ActivateActorSpells_PlayToHand() - {0} - {1} on {2}", (object) this, (object) spellType1, (object) oldActor);
        Log.FaceDownCard.Print("Card.ActivateActorSpells_PlayToHand() - {0} - {1} on {2}", (object) this, (object) spellType2, (object) this.m_actor);
      }
      Spell.FinishedCallback finishedCallback = wasInGraveyard ? (Spell.FinishedCallback) ((spell, userData) => this.ResumeLayoutForPlayZone()) : (Spell.FinishedCallback) null;
      this.ActivateSpell(outSpell, finishedCallback, (object) null, new Spell.StateFinishedCallback(this.OnSpellStateFinished_PlayToHand_OldActor_SummonOut));
      this.ActivateSpell(inSpell, new Spell.FinishedCallback(this.OnSpellFinished_PlayToHand_SummonIn));
    }
    return true;
  }

  private bool CancelCustomSummonSpell(Spell.StateFinishedCallback callback)
  {
    if ((UnityEngine.Object) this.m_customSummonSpell == (UnityEngine.Object) null || !this.m_customSummonSpell.HasUsableState(SpellStateType.CANCEL) || this.m_customSummonSpell.GetActiveState() == SpellStateType.NONE || this.m_customSummonSpell.GetActiveState() == SpellStateType.CANCEL)
      return false;
    this.m_customSummonSpell.AddStateFinishedCallback(callback);
    this.m_customSummonSpell.ActivateState(SpellStateType.CANCEL);
    return true;
  }

  private void OnSpellFinished_PlayToHand_SummonOut(Spell spell, object userData) => this.ActivateSpell((Spell) userData, new Spell.FinishedCallback(this.OnSpellFinished_StandardCardSummon));

  private void OnSpellFinished_PlayToHand_SummonOut_FromGraveyard(Spell spell, object userData)
  {
    this.OnSpellFinished_PlayToHand_SummonOut(spell, userData);
    this.ResumeLayoutForPlayZone();
  }

  private void ResumeLayoutForPlayZone()
  {
    Player.Side side = this.m_playZoneBlockerSide.HasValue ? this.m_playZoneBlockerSide.Value : this.m_zone.m_Side;
    this.m_playZoneBlockerSide = new Player.Side?();
    ZonePlay zoneOfType = ZoneMgr.Get().FindZoneOfType<ZonePlay>(side);
    zoneOfType.RemoveLayoutBlocker();
    zoneOfType.UpdateLayout();
  }

  private void OnSpellStateFinished_PlayToHand_OldActor_SummonOut(
    Spell spell,
    SpellStateType prevStateType,
    object userData)
  {
    if (this.m_entity.IsControlledByOpposingSidePlayer())
      Log.FaceDownCard.Print("Card.OnSpellStateFinished_PlayToHand_OldActor_SummonOut() - {0} stateType={1}", (object) this, (object) spell.GetActiveState());
    this.OnSpellStateFinished_DestroyActor(spell, prevStateType, userData);
  }

  private void OnSpellFinished_PlayToHand_SummonIn(Spell spell, object userData)
  {
    if (this.m_entity.IsControlledByOpposingSidePlayer())
      Log.FaceDownCard.Print("Card.OnSpellFinished_PlayToHand_SummonIn() - {0}", (object) this);
    this.OnSpellFinished_StandardCardSummon(spell, userData);
  }

  private IEnumerator DoHandToDeckTransition(Actor handActor)
  {
    Card card = this;
    card.m_doNotSort = true;
    card.DeactivateHandStateSpells();
    ZoneDeck deckZone = card.m_zone as ZoneDeck;
    if (!((UnityEngine.Object) deckZone == (UnityEngine.Object) null))
    {
      ZoneHand handZone = card.m_prevZone as ZoneHand;
      if (!((UnityEngine.Object) handZone == (UnityEngine.Object) null))
      {
        int deckAnimationCount = deckZone.GetDefaultHandToDeckAnimationCount();
        deckZone.NotifyCardAnimationStart();
        deckZone.IncrementDefaultHandToDeckAnimationCount();
        deckZone.AddLayoutBlocker();
        if (!card.m_entity.IsTradeable() || !card.m_entity.HasTag(GAME_TAG.IS_USING_TRADE_OPTION))
        {
          float num = handZone.GetController().IsFriendlySide() ? 3f : -3f;
          Vector3 position = new Vector3(card.transform.position.x, card.transform.position.y, handZone.transform.position.z + num);
          iTween.MoveTo(card.gameObject, position, 1.75f);
          iTween.ScaleTo(card.gameObject, handZone.GetCardScale() * 1.5f, 1.75f);
          yield return (object) new WaitForSeconds(1.85f + 0.3f * (float) deckAnimationCount);
        }
        else
          yield return (object) new WaitForSeconds(0.1f);
        yield return (object) card.AnimatePlayToDeck(card.gameObject, deckZone, !handZone.GetController().IsFriendlySide());
        handActor.Destroy();
        card.m_actorReady = true;
        card.m_doNotSort = false;
        deckZone.RemoveLayoutBlocker();
        deckZone.UpdateLayout();
        deckZone.DecrementDefaultHandToDeckAnimationCount();
        deckZone.NotifyCardAnimationFinish();
      }
    }
  }

  private void DoPlayToDeckTransition(Actor playActor)
  {
    this.m_doNotSort = true;
    this.m_actor.Hide();
    this.StartCoroutine(this.AnimatePlayToDeck(playActor));
  }

  private IEnumerator AnimatePlayToDeck(Actor playActor)
  {
    Card card = this;
    Actor handActor = (Actor) null;
    bool loadingActor = true;
    PrefabCallback<GameObject> callback1 = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      loadingActor = false;
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Error.AddDevFatal("Card.AnimatePlayToGraveyardToDeck() - failed to load {0}", (object) assetRef);
      }
      else
      {
        handActor = go.GetComponent<Actor>();
        if (!((UnityEngine.Object) handActor == (UnityEngine.Object) null))
          return;
        Error.AddDevFatal("Card.AnimatePlayToGraveyardToDeck() - instance of {0} has no Actor component", (object) this.name);
      }
    });
    AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(card.m_entity), callback1, options: AssetLoadingOptions.IgnorePrefabPosition);
    while (loadingActor)
      yield return (object) null;
    if ((UnityEngine.Object) handActor == (UnityEngine.Object) null)
    {
      playActor.Destroy();
    }
    else
    {
      handActor.SetEntity(card.m_entity);
      handActor.SetCardDef(card.m_cardDef);
      handActor.UpdateAllComponents();
      handActor.transform.parent = playActor.GetCard().transform;
      TransformUtil.Identity((Component) handActor);
      handActor.Hide();
      SpellType spellType1 = SpellType.SUMMON_OUT;
      Spell spell1 = playActor.GetSpell(spellType1);
      if ((UnityEngine.Object) spell1 == (UnityEngine.Object) null)
      {
        Error.AddDevFatal("{0}.AnimatePlayToGraveyardToDeck() - outSpell=null outSpellType={1}", (object) card, (object) spellType1);
      }
      else
      {
        SpellType spellType2 = SpellType.SUMMON_IN;
        Spell inSpell = handActor.GetSpell(spellType2);
        if ((UnityEngine.Object) inSpell == (UnityEngine.Object) null)
        {
          Error.AddDevFatal("{0}.AnimatePlayToGraveyardToDeck() - inSpell=null inSpellType={1}", (object) card, (object) spellType2);
        }
        else
        {
          bool waitForSpells = true;
          Spell.FinishedCallback callback2 = (Spell.FinishedCallback) ((spell, userData) => waitForSpells = false);
          Spell.StateFinishedCallback callback3 = (Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
          {
            if (spell.GetActiveState() != SpellStateType.NONE)
              return;
            playActor.Destroy();
          });
          Spell.FinishedCallback callback4 = (Spell.FinishedCallback) ((spell, userData) =>
          {
            inSpell.Activate();
            this.ResumeLayoutForPlayZone();
          });
          inSpell.AddFinishedCallback(callback2);
          spell1.AddFinishedCallback(callback4);
          spell1.AddStateFinishedCallback(callback3);
          card.PrepareForDeathAnimation(playActor);
          spell1.Activate();
          while (waitForSpells)
            yield return (object) 0;
          ZoneDeck deckZone = (ZoneDeck) card.m_zone;
          deckZone.NotifyCardAnimationStart();
          yield return (object) card.StartCoroutine(card.AnimatePlayToDeck(card.gameObject, deckZone));
          handActor.Destroy();
          card.m_actorReady = true;
          card.m_doNotSort = false;
          deckZone.RemoveLayoutBlocker();
          deckZone.UpdateLayout();
          deckZone.NotifyCardAnimationFinish();
        }
      }
    }
  }

  public IEnumerator AnimatePlayToDeck(
    GameObject mover,
    ZoneDeck deckZone,
    bool hideBackSide = false,
    float timeScale = 1f)
  {
    Card card = this;
    SoundManager.Get().LoadAndPlay((AssetReference) "MinionToDeck_transition.prefab:8063f1b133f28e34aaeade8fcabe250c");
    Vector3 vector3_1 = deckZone.GetThicknessForLayout().GetMeshRenderer().bounds.center + Card.IN_DECK_OFFSET;
    if (card.m_entity != null && card.m_entity.IsMercenary())
      vector3_1 -= Card.IN_DECK_OFFSET;
    Vector3 vector3_2 = vector3_1 + Card.ABOVE_DECK_OFFSET;
    Vector3 vector3_3 = new Vector3(0.0f, Card.IN_DECK_ANGLES.y, 0.0f);
    Vector3 inDeckAngles = Card.IN_DECK_ANGLES;
    Vector3 inDeckScale = Card.IN_DECK_SCALE;
    float num1 = 0.3f;
    if (hideBackSide)
    {
      vector3_3.y = inDeckAngles.y = -Card.IN_DECK_ANGLES.y;
      num1 = 0.5f;
    }
    float num2 = 1f;
    if ((double) timeScale > 0.0)
      num2 *= 1f / timeScale;
    Actor component = mover.GetComponent<Actor>();
    iTween.MoveTo(mover, iTween.Hash((object) "position", (object) vector3_2, (object) "delay", (object) (float) (0.0 * (double) num2), (object) "time", (object) (float) (0.699999988079071 * (double) num2), (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
    iTween.RotateTo(mover, iTween.Hash((object) "rotation", (object) vector3_3, (object) "delay", (object) (float) (0.0 * (double) num2), (object) "time", (object) (float) (0.200000002980232 * (double) num2), (object) "easetype", (object) iTween.EaseType.easeInOutCubic));
    iTween.MoveTo(mover, iTween.Hash((object) "position", (object) vector3_1, (object) "delay", (object) (float) (0.699999988079071 * (double) num2), (object) "time", (object) (float) (0.699999988079071 * (double) num2), (object) "easetype", (object) iTween.EaseType.easeOutCubic));
    iTween.ScaleTo(mover, iTween.Hash((object) "scale", (object) inDeckScale, (object) "delay", (object) (float) (0.699999988079071 * (double) num2), (object) "time", (object) (float) (0.600000023841858 * (double) num2), (object) "easetype", (object) iTween.EaseType.easeInCubic));
    if ((UnityEngine.Object) card.gameObject != (UnityEngine.Object) null && (UnityEngine.Object) component != (UnityEngine.Object) null)
      iTween.RotateTo(mover, iTween.Hash((object) "rotation", (object) inDeckAngles, (object) "delay", (object) (float) (0.200000002980232 * (double) num2), (object) "time", (object) (float) ((double) num1 * (double) num2), (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "OnCardRotateIntoDeckComplete", (object) "oncompleteparams", (object) component, (object) "oncompletetarget", (object) card.gameObject));
    else
      iTween.RotateTo(mover, iTween.Hash((object) "rotation", (object) inDeckAngles, (object) "delay", (object) (float) (0.200000002980232 * (double) num2), (object) "time", (object) (float) ((double) num1 * (double) num2), (object) "easetype", (object) iTween.EaseType.easeOutCubic));
    while (iTween.HasTween(mover))
      yield return (object) 0;
  }

  private void OnCardRotateIntoDeckComplete(Actor cardActor)
  {
    if (!((UnityEngine.Object) this.gameObject != (UnityEngine.Object) null) || !((UnityEngine.Object) cardActor != (UnityEngine.Object) null))
      return;
    if ((UnityEngine.Object) cardActor.m_eliteObject != (UnityEngine.Object) null)
      cardActor.m_eliteObject.SetActive(false);
    if ((UnityEngine.Object) cardActor.m_portraitMesh != (UnityEngine.Object) null)
      cardActor.m_portraitMesh.SetActive(false);
    if ((UnityEngine.Object) cardActor.m_manaObject != (UnityEngine.Object) null)
      cardActor.m_manaObject.SetActive(false);
    if (!((UnityEngine.Object) cardActor.m_costTextMesh != (UnityEngine.Object) null))
      return;
    cardActor.m_costTextMesh.Hide();
  }

  public void SetSecretTriggered(bool set) => this.m_secretTriggered = set;

  public bool WasSecretTriggered() => this.m_secretTriggered;

  public bool CanShowSecretTrigger() => !(bool) UniversalInputManager.UsePhoneUI || this.m_zone.IsOnlyCard(this);

  public void ShowSecretTrigger() => this.m_actor.GetComponent<Spell>().ActivateState(SpellStateType.ACTION);

  private bool CanShowSecretZoneCard()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return true;
    ZoneSecret zone = this.m_zone as ZoneSecret;
    return !((UnityEngine.Object) zone == (UnityEngine.Object) null) && (this.m_entity != null && this.m_entity.IsQuest() || this.m_entity != null && this.m_entity.IsQuestline() || this.m_entity != null && this.m_entity.IsPuzzle() || this.m_entity != null && this.m_entity.IsRulebook() || this.m_entity != null && this.m_entity.IsSigil() || this.m_entity != null && this.m_entity.IsObjective() || zone.GetSecretCards().IndexOf(this) == 0 || zone.GetSideQuestCards().IndexOf(this) == 0);
  }

  private void ShowSecretQuestBirth()
  {
    Spell component = this.m_actor.GetComponent<Spell>();
    if (!this.CanShowSecretZoneCard())
    {
      Spell.StateFinishedCallback callback = (Spell.StateFinishedCallback) ((thisSpell, prevStateType, userData) =>
      {
        if (thisSpell.GetActiveState() != SpellStateType.NONE || this.CanShowSecretZoneCard())
          return;
        this.HideCard();
      });
      component.AddStateFinishedCallback(callback);
    }
    component.ActivateState(SpellStateType.BIRTH);
  }

  public bool CanShowSecretDeath() => !(bool) UniversalInputManager.UsePhoneUI || this.m_prevZone.GetCardCount() == 0;

  public void ShowSecretDeath(Actor oldActor)
  {
    Spell component = oldActor.GetComponent<Spell>();
    if (this.m_secretTriggered)
    {
      this.m_secretTriggered = false;
      if (component.GetActiveState() == SpellStateType.NONE)
        oldActor.Destroy();
      else
        component.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
    }
    else
    {
      component.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
      component.ActivateState(SpellStateType.ACTION);
      oldActor.transform.parent = (Transform) null;
      this.m_doNotSort = true;
      if ((bool) UniversalInputManager.UsePhoneUI)
        return;
      iTween.Stop(this.gameObject);
      this.m_actor.Hide();
      this.StartCoroutine(this.WaitAndThenShowDestroyedSecret());
    }
  }

  private IEnumerator WaitAndThenShowDestroyedSecret()
  {
    Card card = this;
    yield return (object) new WaitForSeconds(0.5f);
    float num = 2f;
    if (card.GetEntity().IsControlledByOpposingSidePlayer())
      num = -num;
    Vector3 position = new Vector3(card.transform.position.x, card.transform.position.y + 1f, card.transform.position.z + num);
    card.m_actor.Show();
    iTween.MoveTo(card.gameObject, position, 3f);
    card.transform.localScale = new Vector3(1f / 1000f, 1f / 1000f, 1f / 1000f);
    card.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 357f);
    iTween.ScaleTo(card.gameObject, new Vector3(1.25f, 0.2f, 1.25f), 3f);
    card.StartCoroutine(card.ActivateGraveyardActorDeathSpellAfterDelay(1f, 4f));
  }

  private IEnumerator SwitchSecretSides()
  {
    Card card = this;
    card.m_doNotSort = true;
    Actor newActor = (Actor) null;
    bool loadingActor = true;
    PrefabCallback<GameObject> callback1 = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      loadingActor = false;
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Error.AddDevFatal("Card.SwitchSecretSides() - failed to load {0}", (object) assetRef);
      }
      else
      {
        newActor = go.GetComponent<Actor>();
        if (!((UnityEngine.Object) newActor == (UnityEngine.Object) null))
          return;
        Error.AddDevFatal("Card.SwitchSecretSides() - instance of {0} has no Actor component", (object) this.name);
      }
    });
    AssetLoader.Get().InstantiatePrefab((AssetReference) card.m_actorPath, callback1, options: AssetLoadingOptions.IgnorePrefabPosition);
    while (loadingActor)
      yield return (object) null;
    if ((bool) (UnityEngine.Object) newActor)
    {
      Actor oldActor = card.m_actor;
      card.m_actor = newActor;
      card.m_actor.SetEntity(card.m_entity);
      card.m_actor.SetCard(card);
      card.m_actor.SetCardDef(card.m_cardDef);
      card.m_actor.UpdateAllComponents();
      card.m_actor.transform.parent = oldActor.transform.parent;
      TransformUtil.Identity((Component) card.m_actor);
      card.m_actor.Hide();
      if (!card.CanShowSecretDeath())
      {
        oldActor.Destroy();
      }
      else
      {
        oldActor.transform.parent = card.transform.parent;
        card.m_transitionStyle = ZoneTransitionStyle.INSTANT;
        bool oldActorFinished = false;
        Spell.FinishedCallback callback2 = (Spell.FinishedCallback) ((spell, userData) => oldActorFinished = true);
        Spell.StateFinishedCallback callback3 = (Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
        {
          if (spell.GetActiveState() != SpellStateType.NONE)
            return;
          oldActor.Destroy();
        });
        Spell component = oldActor.GetComponent<Spell>();
        component.AddFinishedCallback(callback2);
        component.AddStateFinishedCallback(callback3);
        component.ActivateState(SpellStateType.ACTION);
        while (!oldActorFinished)
          yield return (object) null;
      }
      card.m_shown = true;
      card.m_actor.Show();
      card.ShowSecretQuestBirth();
    }
    card.m_actorReady = true;
    card.m_doNotSort = false;
    card.m_zone.UpdateLayout();
    card.ActivateStateSpells();
  }

  private bool ShouldCardDrawWaitForTurnStartSpells()
  {
    SpellController spellController = TurnStartManager.Get().GetSpellController();
    return !((UnityEngine.Object) spellController == (UnityEngine.Object) null) && (spellController.IsSource(this) || spellController.IsTarget(this));
  }

  private IEnumerator WaitForCardDrawBlockingTurnStartSpells()
  {
    while (this.ShouldCardDrawWaitForTurnStartSpells())
      yield return (object) null;
  }

  private PowerTask GetPowerTaskToBlockCardDraw()
  {
    if (this.m_latestZoneChange == null)
      return (PowerTask) null;
    PowerTaskList taskList = this.m_latestZoneChange.GetParentList().GetTaskList();
    if (taskList == null)
      return (PowerTask) null;
    if (taskList.IsEndOfBlock() && taskList.IsComplete())
      return (PowerTask) null;
    PowerTask blockingTask1 = (PowerTask) null;
    PowerTaskList currentTaskList = GameState.Get().GetPowerProcessor().GetCurrentTaskList();
    if (currentTaskList != null && currentTaskList.IsDescendantOfBlock(taskList))
      this.DoesTaskListBlockCardDraw(currentTaskList, out blockingTask1);
    foreach (PowerTaskList power in (QueueList<PowerTaskList>) GameState.Get().GetPowerProcessor().GetPowerQueue())
    {
      PowerTask blockingTask2;
      if (power.IsDescendantOfBlock(taskList) && this.DoesTaskListBlockCardDraw(power, out blockingTask2))
      {
        if (this.CanPowerTaskListBlockCardDraw(power))
          blockingTask1 = blockingTask2;
        else
          break;
      }
    }
    return blockingTask1;
  }

  private bool CanPowerTaskListBlockCardDraw(PowerTaskList blockingPowerTaskList)
  {
    PowerTaskList currentTaskList = GameState.Get().GetPowerProcessor().GetCurrentTaskList();
    if (currentTaskList != null && (currentTaskList.HasCardDraw() || currentTaskList.HasCardMill() || currentTaskList.HasFatigue()))
      return false;
    foreach (PowerTaskList power in (QueueList<PowerTaskList>) GameState.Get().GetPowerProcessor().GetPowerQueue())
    {
      if (power != blockingPowerTaskList)
      {
        if (power.HasCardDraw() || power.HasCardMill() || power.HasFatigue())
          return false;
      }
      else
        break;
    }
    return true;
  }

  private bool DoesTaskListBlockCardDraw(PowerTaskList taskList, out PowerTask blockingTask)
  {
    blockingTask = this.GetPowerTaskBlockingCardDraw(taskList);
    if (blockingTask == null)
      return false;
    foreach (PowerTask task in taskList.GetTaskList())
    {
      if (task != blockingTask)
      {
        if (task.IsCardDraw() || task.IsCardMill() || task.IsFatigue())
        {
          blockingTask = (PowerTask) null;
          return false;
        }
      }
      else
        break;
    }
    return true;
  }

  private PowerTask GetPowerTaskBlockingCardDraw(PowerTaskList taskList)
  {
    if (taskList == null)
      return (PowerTask) null;
    if (taskList.IsComplete())
      return (PowerTask) null;
    Network.HistBlockStart blockStart = taskList.GetBlockStart();
    if (blockStart == null)
      return (PowerTask) null;
    if (blockStart.BlockType != HistoryBlock.Type.POWER && blockStart.BlockType != HistoryBlock.Type.TRIGGER)
      return (PowerTask) null;
    int entityId = this.m_entity.GetEntityId();
    List<PowerTask> taskList1 = taskList.GetTaskList();
    for (int index = 0; index < taskList1.Count; ++index)
    {
      PowerTask blockingCardDraw = taskList1[index];
      if (!blockingCardDraw.IsCompleted())
      {
        Network.PowerHistory power = blockingCardDraw.GetPower();
        int num = 0;
        switch (power.Type)
        {
          case Network.PowerType.SHOW_ENTITY:
            Network.HistShowEntity histShowEntity = (Network.HistShowEntity) power;
            if (histShowEntity.Entity.ID == entityId)
            {
              Network.Entity.Tag tag = histShowEntity.Entity.Tags.Find((Predicate<Network.Entity.Tag>) (currTag => currTag.Name == 49));
              if (tag != null)
              {
                num = tag.Value;
                break;
              }
              break;
            }
            break;
          case Network.PowerType.HIDE_ENTITY:
            Network.HistHideEntity histHideEntity = (Network.HistHideEntity) power;
            if (histHideEntity.Entity == entityId)
            {
              num = histHideEntity.Zone;
              break;
            }
            break;
          case Network.PowerType.TAG_CHANGE:
            Network.HistTagChange histTagChange = (Network.HistTagChange) power;
            if (histTagChange.Entity == entityId && histTagChange.Tag == 49)
            {
              num = histTagChange.Value;
              break;
            }
            break;
          case Network.PowerType.META_DATA:
            Network.HistMetaData histMetaData = (Network.HistMetaData) power;
            if (histMetaData.MetaType == HistoryMeta.Type.HOLD_DRAWN_CARD && histMetaData.Info.Count == 1 && histMetaData.Info[0] == entityId)
              return blockingCardDraw;
            break;
          case Network.PowerType.CHANGE_ENTITY:
            if (((Network.HistChangeEntity) power).Entity.ID == entityId)
              return blockingCardDraw;
            break;
        }
        if (num != 0 && num != 3)
          return blockingCardDraw;
      }
    }
    return (PowerTask) null;
  }

  private void CutoffFriendlyCardDraw()
  {
    if (this.m_actorReady)
      return;
    if ((UnityEngine.Object) this.m_actorWaitingToBeReplaced != (UnityEngine.Object) null)
    {
      this.m_actorWaitingToBeReplaced.Destroy();
      this.m_actorWaitingToBeReplaced = (Actor) null;
    }
    this.m_actor.Show();
    this.m_actor.TurnOffCollider();
    this.m_doNotSort = false;
    this.m_actorReady = true;
    this.ActivateStateSpells();
    this.RefreshActor();
    GameState.Get().ClearCardBeingDrawn(this);
    this.m_zone.UpdateLayout();
  }

  private IEnumerator WaitAndPrepareForDeathAnimation(Actor dyingActor)
  {
    yield return (object) new WaitForSeconds(this.m_keywordDeathDelaySec);
    this.PrepareForDeathAnimation(dyingActor);
  }

  private void PrepareForDeathAnimation(Actor dyingActor)
  {
    dyingActor.ToggleCollider(false);
    dyingActor.ToggleForceIdle(true);
    dyingActor.SetActorState(ActorStateType.CARD_IDLE);
    dyingActor.DoCardDeathVisuals();
    this.DeactivateCustomKeywordEffect();
  }

  private IEnumerator ActivateGraveyardActorDeathSpellAfterDelay(
    float predelay,
    float postdelay,
    Card.ActivateGraveyardActorDeathSpellAfterDelayCallback finishedCallback = null)
  {
    this.m_actor.DoCardDeathVisuals();
    Spell chosenSpell = this.GetBestDeathSpell();
    if (chosenSpell.DoesBlockServerEvents())
      GameState.Get().AddServerBlockingSpell(chosenSpell);
    yield return (object) new WaitForSeconds(predelay);
    this.ActivateSpell(chosenSpell, (Spell.FinishedCallback) null);
    this.CleanUpCustomSpell(chosenSpell, ref this.m_customDiscardSpell);
    this.CleanUpCustomSpell(chosenSpell, ref this.m_customDiscardSpellOverride);
    yield return (object) new WaitForSeconds(postdelay);
    this.m_doNotSort = false;
    this.m_actor.SetBlockTextComponentUpdate(false);
    if (finishedCallback != null)
      finishedCallback();
  }

  private bool HandlePlayActorDeath(Actor oldActor)
  {
    bool flag = false;
    if (!this.m_cardDef.CardDef.m_SuppressDeathrattleDeath && this.m_entity.HasDeathrattle() && !this.m_entity.IsDeathrattleDisabled())
      this.ActivateActorSpell(oldActor, SpellType.DEATHRATTLE_DEATH);
    if (!this.m_cardDef.CardDef.m_SuppressDeathrattleDeath && this.m_entity.HasTag(GAME_TAG.REBORN))
      this.ActivateActorSpell(oldActor, SpellType.REBORN_DEATH);
    if (this.m_suppressDeathEffects)
    {
      if ((bool) (UnityEngine.Object) oldActor)
        oldActor.Destroy();
      if (this.IsShown())
        this.ShowImpl();
      else
        this.HideImpl();
      flag = true;
      this.m_actorReady = true;
    }
    else
    {
      if (!this.m_suppressKeywordDeaths)
        this.StartCoroutine(this.WaitAndPrepareForDeathAnimation(oldActor));
      if ((UnityEngine.Object) this.ActivateDeathSpell(oldActor) != (UnityEngine.Object) null)
      {
        this.m_actor.Hide();
        flag = true;
        this.m_actorReady = true;
      }
    }
    return flag;
  }

  private bool DoesCardReturnFromGraveyard()
  {
    foreach (PowerTaskList power in (QueueList<PowerTaskList>) GameState.Get().GetPowerProcessor().GetPowerQueue())
    {
      if (this.DoesTaskListReturnCardFromGraveyard(power))
      {
        Log.Gameplay.PrintInfo("Found the task for returning entity {0} from graveyard!", (object) this.m_entity);
        return true;
      }
    }
    return false;
  }

  private bool DoesTaskListReturnCardFromGraveyard(PowerTaskList taskList)
  {
    if (!taskList.IsTriggerBlock())
      return false;
    foreach (PowerTask task in taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = power as Network.HistTagChange;
        if (histTagChange.Tag == 49 && histTagChange.Entity == this.m_entity.GetEntityId())
          return histTagChange.Value != 6;
      }
    }
    return false;
  }

  private int GetCardFutureController()
  {
    foreach (PowerTaskList power in (QueueList<PowerTaskList>) GameState.Get().GetPowerProcessor().GetPowerQueue())
    {
      int controllerFromTaskList = this.GetCardFutureControllerFromTaskList(power);
      if (controllerFromTaskList != this.m_entity.GetControllerId())
        return controllerFromTaskList;
    }
    return this.m_entity.GetControllerId();
  }

  private int GetCardFutureControllerFromTaskList(PowerTaskList taskList)
  {
    foreach (PowerTask task in taskList.GetTaskList())
    {
      Network.PowerHistory power = task.GetPower();
      if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = power as Network.HistTagChange;
        if (histTagChange.Tag == 50 && histTagChange.Entity == this.m_entity.GetEntityId())
          return histTagChange.Value;
      }
    }
    return this.m_entity.GetControllerId();
  }

  public void SetDelayBeforeHideInNullZoneVisuals(float delay) => this.m_delayBeforeHideInNullZoneVisuals = delay;

  private void DoNullZoneVisuals() => this.StartCoroutine(this.DoNullZoneVisualsWithTiming());

  private IEnumerator DoNullZoneVisualsWithTiming()
  {
    if ((double) this.m_delayBeforeHideInNullZoneVisuals > 0.0)
      yield return (object) new WaitForSeconds(this.m_delayBeforeHideInNullZoneVisuals);
    Spell nullZoneSpell = this.GetBestNullZoneSpell();
    if ((UnityEngine.Object) nullZoneSpell != (UnityEngine.Object) null)
    {
      nullZoneSpell.Activate();
      while (nullZoneSpell.GetActiveState() != SpellStateType.NONE)
        yield return (object) null;
    }
    if ((UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
      this.m_actor.DeactivateAllSpells();
    this.HideCard();
  }

  private Spell GetBestNullZoneSpell()
  {
    if (this.m_entity.HasTag(GAME_TAG.GHOSTLY) && this.GetControllerSide() == Player.Side.FRIENDLY && this.m_prevZone is ZoneHand && (UnityEngine.Object) this.m_actor != (UnityEngine.Object) null)
      return this.m_actor.GetSpell(SpellType.GHOSTLY_DEATH);
    return this.m_entity.IsSpell() && this.m_prevZone is ZoneHand && (UnityEngine.Object) this.m_actor != (UnityEngine.Object) null && this.m_zone is ZoneGraveyard ? this.m_actor.GetSpell(SpellType.POWER_UP) : (Spell) null;
  }

  public void SetDrawTimeScale(float scale) => this.m_drawTimeScale = new float?(scale);

  public bool IsInTradeArea() => this.IsInTradeArea(this.gameObject.transform.position);

  public bool IsInTradeArea(Vector3 checkPosition)
  {
    if ((UnityEngine.Object) ZoneMgr.Get() == (UnityEngine.Object) null)
      return false;
    Collider collider = Board.Get().FindCollider("TradeArea");
    return !((UnityEngine.Object) collider == (UnityEngine.Object) null) && collider.bounds.Contains(checkPosition);
  }

  public bool HasEnoughManaToTrade() => this.m_entity != null && this.m_entity.GetController() != null && Math.Max(this.m_entity.GetTag(GAME_TAG.TRADE_COST), 0) <= this.m_entity.GetController().GetNumAvailableResources();

  private void ShowTradeableHover() => SpellUtils.ActivateBirthIfNecessary(this.m_actor.GetSpell(SpellType.TRADEABLE_HOVER));

  public void HideTradeableHover()
  {
    Spell spell = this.m_actor.GetSpell(SpellType.TRADEABLE_HOVER);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null || spell.GetActiveState() == SpellStateType.DEATH)
      return;
    SpellUtils.ActivateCancelIfNecessary(spell);
  }

  public bool HasEnoughManaToPlay() => this.m_entity != null && this.m_entity.GetController() != null && this.m_entity.GetCost() <= this.m_entity.GetController().GetNumAvailableResources();

  public int GetNumberOfMinionsInPlay()
  {
    ZoneMgr zoneMgr = ZoneMgr.Get();
    if ((UnityEngine.Object) zoneMgr == (UnityEngine.Object) null)
      return 0;
    ZonePlay zoneOfType = zoneMgr.FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
    return (UnityEngine.Object) zoneOfType == (UnityEngine.Object) null ? 0 : zoneOfType.GetCards().Count<Card>((Func<Card, bool>) (c => !c.IsBeingDragged));
  }

  public bool IsLettuceAbility() => this.GetEntity().IsLettuceAbility();

  public bool HasCardDef => (UnityEngine.Object) this.m_cardDef?.CardDef != (UnityEngine.Object) null;

  public bool HasSameCardDef(CardDef cardDef) => (UnityEngine.Object) this.m_cardDef?.CardDef == (UnityEngine.Object) cardDef;

  public bool HasHiddenCardDef => this.m_cardDef?.CardDef is HiddenCard;

  public T GetCardDefComponent<T>() => !this.HasCardDef ? default (T) : this.m_cardDef.CardDef.GetComponent<T>();

  public string CustomHeroPhoneManaGem => !this.HasCardDef ? (string) null : this.m_cardDef.CardDef.m_CustomHeroPhoneManaGem;

  public string CustomHeroTray => !this.HasCardDef ? (string) null : this.m_cardDef.CardDef.m_CustomHeroTray;

  public string CustomHeroTrayGolden => !this.HasCardDef ? (string) null : this.m_cardDef.CardDef.m_CustomHeroTrayGolden;

  public string CustomHeroPhoneTray => !this.HasCardDef ? (string) null : this.m_cardDef.CardDef.m_CustomHeroPhoneTray;

  public bool DisablePremiumHeroTray => this.HasCardDef && this.m_cardDef.CardDef.m_DisablePremiumHeroTray;

  public ref string DiamondCustomSpawnSpellPath => ref this.m_cardDef.CardDef.m_DiamondCustomSpawnSpellPath;

  public ref string GoldenCustomSpawnSpellPath => ref this.m_cardDef.CardDef.m_GoldenCustomSpawnSpellPath;

  public ref string CustomSpawnSpellPath => ref this.m_cardDef.CardDef.m_CustomSpawnSpellPath;

  public ref string DiamondCustomSummonSpellPath => ref this.m_cardDef.CardDef.m_DiamondCustomSummonSpellPath;

  public ref string GoldenCustomSummonSpellPath => ref this.m_cardDef.CardDef.m_GoldenCustomSummonSpellPath;

  public ref string CustomSummonSpellPath => ref this.m_cardDef.CardDef.m_CustomSummonSpellPath;

  public BaconLHSConfig LegendaryHeroSkinConfig => !this.HasCardDef ? (BaconLHSConfig) null : this.m_cardDef.CardDef.m_LegendaryHeroSkinConfig;

  public List<Board.CustomTraySettings> CustomHeroTraySettings => !this.HasCardDef ? (List<Board.CustomTraySettings>) null : this.m_cardDef.CardDef.m_CustomHeroTraySettings;

  private class PrefabLoadRequest
  {
    public string m_path;
    public PrefabCallback<GameObject> m_loadCallback;
  }

  public delegate void EmotePlayCallback(EmoteType emoteType);

  public enum AnnouncerLineType
  {
    DEFAULT,
    BEFORE_VERSUS,
    AFTER_VERSUS,
    MAX,
  }

  private delegate void ActivateGraveyardActorDeathSpellAfterDelayCallback();
}
