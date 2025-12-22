using Blizzard.T5.Core;
using UnityEngine;

public class PlayErrors
{
  private static Map<PlayErrors.ErrorType, string> s_playErrorsMessages = new Map<PlayErrors.ErrorType, string>()
  {
    {
      PlayErrors.ErrorType.REQ_MINION_TARGET,
      "GAMEPLAY_PlayErrors_REQ_MINION_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_FRIENDLY_TARGET,
      "GAMEPLAY_PlayErrors_REQ_FRIENDLY_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_ENEMY_TARGET,
      "GAMEPLAY_PlayErrors_REQ_ENEMY_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_DAMAGED_TARGET,
      "GAMEPLAY_PlayErrors_REQ_DAMAGED_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_MAX_SECRETS,
      "GAMEPLAY_PlayErrors_REQ_MAX_SECRETS"
    },
    {
      PlayErrors.ErrorType.REQ_FROZEN_TARGET,
      "GAMEPLAY_PlayErrors_REQ_FROZEN_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_CHARGE_TARGET,
      "GAMEPLAY_PlayErrors_REQ_CHARGE_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_MAX_ATTACK,
      "GAMEPLAY_PlayErrors_REQ_TARGET_MAX_ATTACK"
    },
    {
      PlayErrors.ErrorType.REQ_NONSELF_TARGET,
      "GAMEPLAY_PlayErrors_REQ_NONSELF_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_WITH_RACE,
      "GAMEPLAY_PlayErrors_REQ_TARGET_WITH_RACE"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_TO_PLAY,
      "GAMEPLAY_PlayErrors_REQ_TARGET_TO_PLAY"
    },
    {
      PlayErrors.ErrorType.REQ_NUM_MINION_SLOTS,
      "GAMEPLAY_PlayErrors_REQ_NUM_MINION_SLOTS"
    },
    {
      PlayErrors.ErrorType.REQ_WEAPON_EQUIPPED,
      "GAMEPLAY_PlayErrors_REQ_WEAPON_EQUIPPED"
    },
    {
      PlayErrors.ErrorType.REQ_YOUR_TURN,
      "GAMEPLAY_PlayErrors_REQ_YOUR_TURN"
    },
    {
      PlayErrors.ErrorType.REQ_NONSTEALTH_ENEMY_TARGET,
      "GAMEPLAY_PlayErrors_REQ_NONSTEALTH_ENEMY_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_HERO_TARGET,
      "GAMEPLAY_PlayErrors_REQ_HERO_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_SECRET_ZONE_CAP,
      "GAMEPLAY_PlayErrors_REQ_SECRET_ZONE_CAP"
    },
    {
      PlayErrors.ErrorType.REQ_MINION_CAP_IF_TARGET_AVAILABLE,
      "GAMEPLAY_PlayErrors_REQ_MINION_CAP_IF_TARGET_AVAILABLE"
    },
    {
      PlayErrors.ErrorType.REQ_MINION_CAP,
      "GAMEPLAY_PlayErrors_REQ_MINION_CAP"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_ATTACKED_THIS_TURN,
      "GAMEPLAY_PlayErrors_REQ_TARGET_ATTACKED_THIS_TURN"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE,
      "GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABLE"
    },
    {
      PlayErrors.ErrorType.REQ_MINIMUM_ENEMY_MINIONS,
      "GAMEPLAY_PlayErrors_REQ_MINIMUM_ENEMY_MINIONS"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_FOR_COMBO,
      "GAMEPLAY_PlayErrors_REQ_TARGET_FOR_COMBO"
    },
    {
      PlayErrors.ErrorType.REQ_NOT_EXHAUSTED_ACTIVATE,
      "GAMEPLAY_PlayErrors_REQ_NOT_EXHAUSTED_ACTIVATE"
    },
    {
      PlayErrors.ErrorType.REQ_UNIQUE_SECRET_OR_QUEST,
      "GAMEPLAY_PlayErrors_REQ_UNIQUE_SECRET"
    },
    {
      PlayErrors.ErrorType.REQ_CAN_BE_ATTACKED,
      "GAMEPLAY_PlayErrors_REQ_CAN_BE_ATTACKED"
    },
    {
      PlayErrors.ErrorType.REQ_ACTION_PWR_IS_MASTER_PWR,
      "GAMEPLAY_PlayErrors_REQ_ACTION_PWR_IS_MASTER_PWR"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_MAGNET,
      "GAMEPLAY_PlayErrors_REQ_TARGET_MAGNET"
    },
    {
      PlayErrors.ErrorType.REQ_ATTACK_GREATER_THAN_0,
      "GAMEPLAY_PlayErrors_REQ_ATTACK_GREATER_THAN_0"
    },
    {
      PlayErrors.ErrorType.REQ_ATTACKER_NOT_FROZEN,
      "GAMEPLAY_PlayErrors_REQ_ATTACKER_NOT_FROZEN"
    },
    {
      PlayErrors.ErrorType.REQ_HERO_OR_MINION_TARGET,
      "GAMEPLAY_PlayErrors_REQ_HERO_OR_MINION_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_SPELLS,
      "GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_SPELLS"
    },
    {
      PlayErrors.ErrorType.REQ_SUBCARD_IS_PLAYABLE,
      "GAMEPLAY_PlayErrors_REQ_SUBCARD_IS_PLAYABLE"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_FOR_NO_COMBO,
      "GAMEPLAY_PlayErrors_REQ_TARGET_FOR_NO_COMBO"
    },
    {
      PlayErrors.ErrorType.REQ_NOT_MINION_JUST_PLAYED,
      "GAMEPLAY_PlayErrors_REQ_NOT_MINION_JUST_PLAYED"
    },
    {
      PlayErrors.ErrorType.REQ_NOT_EXHAUSTED_HERO_POWER,
      "GAMEPLAY_PlayErrors_REQ_NOT_EXHAUSTED_HERO_POWER"
    },
    {
      PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_OPPONENTS,
      "GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_OPPONENTS"
    },
    {
      PlayErrors.ErrorType.REQ_ATTACKER_CAN_ATTACK,
      "GAMEPLAY_PlayErrors_REQ_ATTACKER_CAN_ATTACK"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_MIN_ATTACK,
      "GAMEPLAY_PlayErrors_REQ_TARGET_MIN_ATTACK"
    },
    {
      PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_HERO_POWERS,
      "GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_HERO_POWERS"
    },
    {
      PlayErrors.ErrorType.REQ_ENEMY_TARGET_NOT_IMMUNE,
      "GAMEPLAY_PlayErrors_REQ_ENEMY_TARGET_NOT_IMMUNE"
    },
    {
      PlayErrors.ErrorType.REQ_ALL_BASIC_TOTEMS_NOT_IN_PLAY,
      "GAMEPLAY_PlayErrors_REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY"
    },
    {
      PlayErrors.ErrorType.REQ_MINIMUM_TOTAL_MINIONS,
      "GAMEPLAY_PlayErrors_REQ_MINIMUM_TOTAL_MINIONS"
    },
    {
      PlayErrors.ErrorType.REQ_MUST_TARGET_TAUNTER,
      "GAMEPLAY_PlayErrors_REQ_MUST_TARGET_TAUNTER"
    },
    {
      PlayErrors.ErrorType.REQ_UNDAMAGED_TARGET,
      "GAMEPLAY_PlayErrors_REQ_UNDAMAGED_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_BATTLECRIES,
      "GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_BATTLECRIES"
    },
    {
      PlayErrors.ErrorType.REQ_STEADY_SHOT,
      "GAMEPLAY_PlayErrors_REQ_STEADY_SHOT"
    },
    {
      PlayErrors.ErrorType.REQ_MINION_OR_ENEMY_HERO,
      "GAMEPLAY_PlayErrors_REQ_MINION_OR_ENEMY_HERO"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND,
      "GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN,
      "GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN"
    },
    {
      PlayErrors.ErrorType.REQ_LEGENDARY_TARGET,
      "GAMEPLAY_PlayErrors_REQ_LEGENDARY_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_NOT_LEGENDARY_TARGET,
      "GAMEPLAY_PlayErrors_REQ_NOT_LEGENDARY_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_TURN,
      "GAMEPLAY_PlayErrors_REQ_FRIENDLY_MINION_DIED_THIS_TURN"
    },
    {
      PlayErrors.ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME,
      "GAMEPLAY_PlayErrors_REQ_FRIENDLY_MINION_DIED_THIS_GAME"
    },
    {
      PlayErrors.ErrorType.REQ_MINION_DIED_THIS_GAME,
      "GAMEPLAY_PlayErrors_REQ_MINION_DIED_THIS_GAME"
    },
    {
      PlayErrors.ErrorType.REQ_ENEMY_WEAPON_EQUIPPED,
      "GAMEPLAY_PlayErrors_REQ_ENEMY_WEAPON_EQUIPPED"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS,
      "GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_WITH_BATTLECRY,
      "GAMEPLAY_PlayErrors_REQ_TARGET_WITH_BATTLECRY"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_WITH_DEATHRATTLE,
      "GAMEPLAY_PlayErrors_REQ_TARGET_WITH_DEATHRATTLE"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS,
      "GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS"
    },
    {
      PlayErrors.ErrorType.REQ_STEALTHED_TARGET,
      "GAMEPLAY_PlayErrors_REQ_STEALTHED_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_MINION_SLOT_OR_MANA_CRYSTAL_SLOT,
      "GAMEPLAY_PlayErrors_REQ_MINION_SLOT_OR_MANA_CRYSTAL_SLOT"
    },
    {
      PlayErrors.ErrorType.REQ_MAX_QUESTS,
      "GAMEPLAY_PlayErrors_REQ_MAX_QUESTS"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN,
      "GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_NOT_VAMPIRE,
      "GAMEPLAY_PlayErrors_REQ_TARGET_NOT_VAMPIRE"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_NOT_DAMAGEABLE_ONLY_BY_WEAPONS,
      "GAMEPLAY_PlayErrors_REQ_TARGET_NOT_DAMAGEABLE_ONLY_BY_WEAPONS"
    },
    {
      PlayErrors.ErrorType.REQ_NOT_DISABLED_HERO_POWER,
      "GAMEPLAY_PlayErrors_REQ_NOT_DISABLED_HERO_POWER"
    },
    {
      PlayErrors.ErrorType.REQ_MUST_PLAY_OTHER_CARD_FIRST,
      "GAMEPLAY_PlayErrors_REQ_MUST_PLAY_OTHER_CARD_FIRST"
    },
    {
      PlayErrors.ErrorType.REQ_HAND_NOT_FULL,
      "GAMEPLAY_PlayErrors_REQ_HAND_NOT_FULL"
    },
    {
      PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_COMBOS,
      "GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_COMBOS"
    },
    {
      PlayErrors.ErrorType.REQ_CANNOT_PLAY_THIS,
      "GAMEPLAY_PlayErrors_REQ_CANNOT_PLAY_THIS"
    },
    {
      PlayErrors.ErrorType.REQ_FRIENDLY_MINIONS_OF_RACE_DIED_THIS_GAME,
      "GAMEPLAY_PlayErrors_REQ_FRIENDLY_MINIONS_OF_RACE_DIED_THIS_GAME"
    },
    {
      PlayErrors.ErrorType.REQ_OPPONENT_PLAYED_CARDS_THIS_GAME,
      "GAMEPLAY_PlayErrors_REQ_OPPONENT_PLAYED_CARDS_THIS_GAME"
    },
    {
      PlayErrors.ErrorType.REQ_FRIENDLY_MINION_OF_RACE_DIED_THIS_TURN,
      "GAMEPLAY_PlayErrors_REQ_FRIENDLY_MINION_OF_RACE_DIED_THIS_TURN"
    },
    {
      PlayErrors.ErrorType.REQ_FRIENDLY_MINION_OF_RACE_IN_HAND,
      "GAMEPLAY_PlayErrors_REQ_FRIENDLY_MINION_OF_RACE_IN_HAND"
    },
    {
      PlayErrors.ErrorType.REQ_FRIENDLY_DEATHRATTLE_MINION_DIED_THIS_GAME,
      "GAMEPLAY_PlayErrors_REQ_FRIENDLY_DEATHRATTLE_MINION_DIED_THIS_GAME"
    },
    {
      PlayErrors.ErrorType.REQ_FRIENDLY_REBORN_MINION_DIED_THIS_GAME,
      "GAMEPLAY_PlayErrors_REQ_FRIENDLY_REBORN_MINION_DIED_THIS_GAME"
    },
    {
      PlayErrors.ErrorType.REQ_LITERALLY_UNPLAYABLE,
      "GAMEPLAY_PlayErrors_REQ_CANNOT_PLAY_THIS"
    },
    {
      PlayErrors.ErrorType.REQ_BOARD_NOT_COMPLETELY_FULL,
      "GAMEPLAY_PlayErrors_REQ_CANNOT_PLAY_THIS"
    },
    {
      PlayErrors.ErrorType.REQ_NOT_MINION_DORMANT,
      "GAMEPLAY_PlayErrors_REQ_NOT_MINION_DORMANT"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_NOT_DORMANT,
      "GAMEPLAY_PlayErrors_REQ_TARGET_NOT_DORMANT"
    },
    {
      PlayErrors.ErrorType.REQ_TWO_OF_A_KIND,
      "GAMEPLAY_PlayErrors_REQ_TWO_OF_A_KIND"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_NOT_HAVE_TAG,
      "GAMEPLAY_PlayErrors_REQ_TARGET_NOT_HAVE_TAG"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_MUST_HAVE_TAG,
      "GAMEPLAY_PlayErrors_REQ_TARGET_MUST_HAVE_TAG"
    },
    {
      PlayErrors.ErrorType.REQ_HAS_OVERLOADED_MANA,
      "GAMEPLAY_PlayErrors_REQ_HAS_OVERLOADED_MANA"
    },
    {
      PlayErrors.ErrorType.REQ_TRADEABLE,
      "GAMEPLAY_PlayErrors_REQ_TRADEABLE"
    },
    {
      PlayErrors.ErrorType.REQ_MINIMUM_TAVERN_TIER_LEVEL_TO_PLAY,
      "GAMEPLAY_PlayErrors_REQ_MINIMUM_TAVERN_TIER_LEVEL_TO_PLAY"
    },
    {
      PlayErrors.ErrorType.REQ_CARD_TAVERN_TIER_LEVEL_TO_PLAY,
      "GAMEPLAY_PlayErrors_REQ_CARD_TAVERN_TIER_LEVEL_TO_PLAY"
    },
    {
      PlayErrors.ErrorType.REQ_NOT_EXHAUSTED_LOCATION,
      "GAMEPLAY_PlayErrors_REQ_NOT_EXHAUSTED_LOCATION"
    },
    {
      PlayErrors.ErrorType.REQ_LOCATION_TARGET,
      "GAMEPLAY_PlayErrors_REQ_LOCATION_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_TARGET_SILVER_HAND_RECRUIT,
      "GAMEPLAY_PlayErrors_REQ_TARGET_SILVER_HAND_RECRUIT"
    },
    {
      PlayErrors.ErrorType.REQ_LOCATION_OR_MINION_TARGET,
      "GAMEPLAY_PlayErrors_REQ_LOCATION_OR_MINION_TARGET"
    },
    {
      PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_LOCATIONS,
      "GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_LOCATIONS"
    },
    {
      PlayErrors.ErrorType.REQ_DRAG_TO_PLAY,
      "GAMEPLAY_PlayErrors_REQ_DRAG_TO_PLAY"
    }
  };

  public static void DisplayPlayError(
    PlayErrors.ErrorType error,
    int? errorParam,
    Entity errorSource)
  {
    Log.PlayErrors.Print("DisplayPlayError: ErrorType = " + (object) error + ", ErrorParam = " + (object) errorParam + ", ErrorSource = " + (object) errorSource);
    if (GameState.Get().GetGameEntity().NotifyOfPlayError(error, errorParam, errorSource))
      return;
    switch (error)
    {
      case PlayErrors.ErrorType.REQ_MINION_TARGET:
      case PlayErrors.ErrorType.REQ_FRIENDLY_TARGET:
      case PlayErrors.ErrorType.REQ_ENEMY_TARGET:
      case PlayErrors.ErrorType.REQ_DAMAGED_TARGET:
      case PlayErrors.ErrorType.REQ_FROZEN_TARGET:
      case PlayErrors.ErrorType.REQ_TARGET_MAX_ATTACK:
      case PlayErrors.ErrorType.REQ_TARGET_WITH_RACE:
      case PlayErrors.ErrorType.REQ_HERO_TARGET:
      case PlayErrors.ErrorType.REQ_HERO_OR_MINION_TARGET:
      case PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_SPELLS:
      case PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_OPPONENTS:
      case PlayErrors.ErrorType.REQ_TARGET_MIN_ATTACK:
      case PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_HERO_POWERS:
      case PlayErrors.ErrorType.REQ_ENEMY_TARGET_NOT_IMMUNE:
      case PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_BATTLECRIES:
      case PlayErrors.ErrorType.REQ_MINION_OR_ENEMY_HERO:
      case PlayErrors.ErrorType.REQ_LEGENDARY_TARGET:
      case PlayErrors.ErrorType.REQ_TARGET_WITH_BATTLECRY:
      case PlayErrors.ErrorType.REQ_TARGET_WITH_DEATHRATTLE:
      case PlayErrors.ErrorType.REQ_TARGET_EXACT_COST:
      case PlayErrors.ErrorType.REQ_STEALTHED_TARGET:
      case PlayErrors.ErrorType.REQ_TARGET_NON_TRIPLED_MINION:
      case PlayErrors.ErrorType.REQ_TWO_OF_A_KIND:
      case PlayErrors.ErrorType.REQ_NOT_LEGENDARY_TARGET:
      case PlayErrors.ErrorType.REQ_LOCATION_TARGET:
      case PlayErrors.ErrorType.REQ_LOCATION_OR_MINION_TARGET:
        Card heroCard1 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
        if (heroCard1 != null)
        {
          heroCard1.PlayEmote(EmoteType.ERROR_TARGET);
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
      case PlayErrors.ErrorType.REQ_TARGET_TO_PLAY:
        if ((errorSource.IsMinion() || errorSource.IsHero()) && errorSource.GetZone() == TAG_ZONE.PLAY)
        {
          Card heroCard2 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
          if (heroCard2 != null)
          {
            heroCard2.PlayEmote(EmoteType.ERROR_GENERIC);
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
          }
          else
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
        {
          Card heroCard3 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
          if (heroCard3 != null)
          {
            heroCard3.PlayEmote(EmoteType.ERROR_PLAY);
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
          }
          else
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
      case PlayErrors.ErrorType.REQ_NUM_MINION_SLOTS:
      case PlayErrors.ErrorType.REQ_MINION_CAP_IF_TARGET_AVAILABLE:
      case PlayErrors.ErrorType.REQ_MINION_CAP:
        Card heroCard4 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
        if (heroCard4 != null)
        {
          heroCard4.PlayEmote(EmoteType.ERROR_FULL_MINIONS);
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
      case PlayErrors.ErrorType.REQ_WEAPON_EQUIPPED:
        Card heroCard5 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
        if (heroCard5 != null)
        {
          heroCard5.PlayEmote(EmoteType.ERROR_NEED_WEAPON);
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
      case PlayErrors.ErrorType.REQ_ENOUGH_MANA:
        if (errorSource.IsSpell() && PlayErrors.DoSpellsCostHealth() || errorSource.HasTag(GAME_TAG.CARD_COSTS_HEALTH))
        {
          Card heroCard6 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
          if (heroCard6 != null)
          {
            heroCard6.PlayEmote(EmoteType.ERROR_PLAY);
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
          }
          else
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else if (errorSource.HasTag(GAME_TAG.CARD_COSTS_ARMOR))
        {
          Card heroCard7 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
          if (heroCard7 != null)
          {
            heroCard7.PlayEmote(EmoteType.ERROR_PLAY);
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
          }
          else
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
        {
          Card heroCard8 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
          if (heroCard8 != null)
          {
            heroCard8.PlayEmote(EmoteType.ERROR_NEED_MANA);
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
          }
          else
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
      case PlayErrors.ErrorType.REQ_YOUR_TURN:
        break;
      case PlayErrors.ErrorType.REQ_NONSTEALTH_ENEMY_TARGET:
        Card heroCard9 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
        if (heroCard9 != null)
        {
          heroCard9.PlayEmote(EmoteType.ERROR_STEALTH);
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
      case PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE:
      case PlayErrors.ErrorType.REQ_TARGET_FOR_COMBO:
      case PlayErrors.ErrorType.REQ_TARGET_FOR_NO_COMBO:
      case PlayErrors.ErrorType.REQ_STEADY_SHOT:
      case PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND:
      case PlayErrors.ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_TURN:
      case PlayErrors.ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME:
      case PlayErrors.ErrorType.REQ_ENEMY_WEAPON_EQUIPPED:
      case PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS:
      case PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS:
      case PlayErrors.ErrorType.REQ_MINION_SLOT_OR_MANA_CRYSTAL_SLOT:
      case PlayErrors.ErrorType.REQ_MUST_PLAY_OTHER_CARD_FIRST:
      case PlayErrors.ErrorType.REQ_CANNOT_PLAY_THIS:
      case PlayErrors.ErrorType.REQ_FRIENDLY_MINIONS_OF_RACE_DIED_THIS_GAME:
      case PlayErrors.ErrorType.REQ_OPPONENT_PLAYED_CARDS_THIS_GAME:
      case PlayErrors.ErrorType.REQ_FRIENDLY_MINION_OF_RACE_DIED_THIS_TURN:
      case PlayErrors.ErrorType.REQ_FRIENDLY_MINION_OF_RACE_IN_HAND:
      case PlayErrors.ErrorType.REQ_FRIENDLY_DEATHRATTLE_MINION_DIED_THIS_GAME:
      case PlayErrors.ErrorType.REQ_FRIENDLY_REBORN_MINION_DIED_THIS_GAME:
      case PlayErrors.ErrorType.REQ_MINION_DIED_THIS_GAME:
      case PlayErrors.ErrorType.REQ_BOARD_NOT_COMPLETELY_FULL:
      case PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN:
        Card heroCard10 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
        if (heroCard10 != null)
        {
          heroCard10.PlayEmote(EmoteType.ERROR_PLAY);
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
      case PlayErrors.ErrorType.REQ_NOT_EXHAUSTED_ACTIVATE:
        if (errorSource.IsHero())
        {
          Card heroCard11 = GameState.Get().GetCurrentPlayer().GetHeroCard();
          if (heroCard11 != null)
          {
            heroCard11.PlayEmote(EmoteType.ERROR_I_ATTACKED);
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
          }
          else
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
        {
          Card heroCard12 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
          if (heroCard12 != null)
          {
            heroCard12.PlayEmote(EmoteType.ERROR_MINION_ATTACKED);
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
          }
          else
            goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
      case PlayErrors.ErrorType.REQ_TARGET_TAUNTER:
        PlayErrors.DisplayTauntErrorEffects();
        goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
      case PlayErrors.ErrorType.REQ_NOT_MINION_JUST_PLAYED:
        Card heroCard13 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
        if (heroCard13 != null)
        {
          heroCard13.PlayEmote(EmoteType.ERROR_SUMMON_SICKNESS);
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
      case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY:
        string errorDescription = PlayErrors.GetErrorDescription(error, errorParam, errorSource);
        if (string.IsNullOrEmpty(errorDescription))
          break;
        GameplayErrorManager.Get().DisplayMessage(errorDescription);
        break;
      default:
        Card heroCard14 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
        if (heroCard14 != null)
        {
          heroCard14.PlayEmote(EmoteType.ERROR_GENERIC);
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
        }
        else
          goto case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY;
    }
  }

  private static bool CanShowMinionTauntError()
  {
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    int minionCount;
    int heroCount;
    GameState.Get().GetTauntCounts(opposingSidePlayer, out minionCount, out heroCount);
    return minionCount > 0 && heroCount == 0;
  }

  private static void DisplayTauntErrorEffects()
  {
    if (PlayErrors.CanShowMinionTauntError())
      GameState.Get().GetFriendlySidePlayer().GetHeroCard()?.PlayEmote(EmoteType.ERROR_TAUNT);
    GameState.Get().ShowEnemyTauntCharacters();
  }

  private static bool DoSpellsCostHealth() => GameState.Get().GetFriendlySidePlayer().HasTag(GAME_TAG.SPELLS_COST_HEALTH);

  private static string GetErrorDescription(
    PlayErrors.ErrorType type,
    int? errorParam,
    Entity errorSource)
  {
    Log.PlayErrors.Print("GetErrorDescription: " + (object) type + " " + (object) errorParam);
    switch (type)
    {
      case PlayErrors.ErrorType.NONE:
        Debug.LogWarning((object) "PlayErrors.GetErrorDescription() - Action is not valid, but no error string found.");
        return "";
      case PlayErrors.ErrorType.REQ_MAX_SECRETS:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_MAX_SECRETS", (object) GameState.Get().GetMaxSecretsPerPlayer());
      case PlayErrors.ErrorType.REQ_TARGET_MAX_ATTACK:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_MAX_ATTACK", (object) errorParam);
      case PlayErrors.ErrorType.REQ_TARGET_WITH_RACE:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_WITH_RACE", (object) GameStrings.GetRaceName((TAG_RACE) errorParam.Value));
      case PlayErrors.ErrorType.REQ_TARGET_TO_PLAY:
        if ((errorSource.IsMinion() || errorSource.IsHero()) && errorSource.GetZone() == TAG_ZONE.PLAY)
          return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_TO_ATTACK");
        break;
      case PlayErrors.ErrorType.REQ_ENOUGH_MANA:
        if (errorSource.IsSpell() && PlayErrors.DoSpellsCostHealth() || errorSource.HasTag(GAME_TAG.CARD_COSTS_HEALTH))
          return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_ENOUGH_HEALTH");
        if (errorSource.HasTag(GAME_TAG.CARD_COSTS_ARMOR))
          return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_ENOUGH_ARMOR");
        return (Object) errorSource.GetCard() != (Object) null && (Object) errorSource.GetCard().GetActor() != (Object) null && errorSource.GetCard().GetActor().UseCoinManaGem() ? GameStrings.Get("GAMEPLAY_PlayErrors_REQ_ENOUGH_COIN") : GameStrings.Get("GAMEPLAY_PlayErrors_REQ_ENOUGH_MANA");
      case PlayErrors.ErrorType.REQ_YOUR_TURN:
        return "";
      case PlayErrors.ErrorType.REQ_SECRET_ZONE_CAP:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_SECRET_ZONE_CAP", (object) GameState.Get().GetMaxSecretZoneSizePerPlayer());
      case PlayErrors.ErrorType.REQ_MINIMUM_ENEMY_MINIONS:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_MINIMUM_ENEMY_MINIONS", (object) errorParam);
      case PlayErrors.ErrorType.REQ_TARGET_TAUNTER:
        return PlayErrors.CanShowMinionTauntError() ? GameStrings.Get("GAMEPLAY_PlayErrors_REQ_TARGET_TAUNTER_MINION") : GameStrings.Get("GAMEPLAY_PlayErrors_REQ_TARGET_TAUNTER_CHARACTER");
      case PlayErrors.ErrorType.REQ_ACTION_PWR_IS_MASTER_PWR:
        return PlayErrors.ErrorInEditorOnly("[Unity Editor] Action power must be master power");
      case PlayErrors.ErrorType.REQ_TARGET_MIN_ATTACK:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_MIN_ATTACK", (object) errorParam);
      case PlayErrors.ErrorType.REQ_MINIMUM_TOTAL_MINIONS:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_MINIMUM_TOTAL_MINIONS", (object) errorParam);
      case PlayErrors.ErrorType.REQ_STEADY_SHOT:
        if (errorSource.IsHeroPower() && errorSource.GetZone() == TAG_ZONE.PLAY)
          return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABLE");
        break;
      case PlayErrors.ErrorType.REQ_SECRET_ZONE_CAP_FOR_NON_SECRET:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_MAX_SECRETS", (object) GameState.Get().GetMaxSecretsPerPlayer());
      case PlayErrors.ErrorType.REQ_TARGET_EXACT_COST:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_EXACT_COST", (object) errorParam);
      case PlayErrors.ErrorType.REQ_MAX_QUESTS:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_MAX_QUESTS", (object) GameState.Get().GetMaxQuestsPerPlayer());
      case PlayErrors.ErrorType.REQ_TARGET_NON_TRIPLED_MINION:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_NON_TRIPLED_MINION", (object) errorParam);
      case PlayErrors.ErrorType.REQ_BOUGHT_MINION_THIS_TURN:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_BOUGHT_MINION_THIS_TURN", (object) errorParam);
      case PlayErrors.ErrorType.REQ_SOLD_MINION_THIS_TURN:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_SOLD_MINION_THIS_TURN", (object) errorParam);
      case PlayErrors.ErrorType.REQ_NOT_MINION_DORMANT:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_NOT_MINION_DORMANT", (object) errorParam);
      case PlayErrors.ErrorType.REQ_NOT_IN_COOLDOWN:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_NOT_IN_COOLDOWN", (object) errorSource.GetTag(GAME_TAG.LETTUCE_CURRENT_COOLDOWN));
      case PlayErrors.ErrorType.REQ_LETTUCE_ABILITY_CANNOT_TARGET_OWNER:
        return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_LETTUCE_ABILITY_CANNOT_TARGET_OWNER");
      case PlayErrors.ErrorType.REQ_MINIMUM_TAVERN_TIER_LEVEL_TO_PLAY:
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_MINIMUM_TAVERN_TIER_LEVEL_TO_PLAY", (object) errorParam);
      case PlayErrors.ErrorType.REQ_CARD_TAVERN_TIER_LEVEL_TO_PLAY:
        return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_CARD_TAVERN_TIER_LEVEL_TO_PLAY");
      case PlayErrors.ErrorType.REQ_NOT_EXHAUSTED_LOCATION:
        int locationCooldown = errorSource.GetLocationCooldown();
        if (locationCooldown == 1)
          return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_NOT_EXHAUSTED_LOCATION");
        return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_NOT_EXHAUSTED_LOCATION_COOLDOWN", (object) locationCooldown);
      case PlayErrors.ErrorType.REQ_MINIMUM_CORPSES:
        Player controller = errorSource.GetController();
        if (controller != null)
        {
          Entity hero = controller.GetHero();
          if (hero != null && !hero.HasClass(TAG_CLASS.DEATHKNIGHT))
            return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_MINIMUM_CORPSES_CLASS");
        }
        return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_MINIMUM_CORPSES");
    }
    string key = (string) null;
    if (PlayErrors.s_playErrorsMessages.TryGetValue(type, out key))
      return GameStrings.Get(key);
    return PlayErrors.ErrorInEditorOnly("[Unity Editor] Unknown play error ({0})", (object) type);
  }

  private static string ErrorInEditorOnly(string format, params object[] args) => "";

  public enum ErrorType
  {
    INVALID = -1, // 0xFFFFFFFF
    NONE = 0,
    REQ_MINION_TARGET = 1,
    REQ_FRIENDLY_TARGET = 2,
    REQ_ENEMY_TARGET = 3,
    REQ_DAMAGED_TARGET = 4,
    REQ_MAX_SECRETS = 5,
    REQ_FROZEN_TARGET = 6,
    REQ_CHARGE_TARGET = 7,
    REQ_TARGET_MAX_ATTACK = 8,
    REQ_NONSELF_TARGET = 9,
    REQ_TARGET_WITH_RACE = 10, // 0x0000000A
    REQ_TARGET_TO_PLAY = 11, // 0x0000000B
    REQ_NUM_MINION_SLOTS = 12, // 0x0000000C
    REQ_WEAPON_EQUIPPED = 13, // 0x0000000D
    REQ_ENOUGH_MANA = 14, // 0x0000000E
    REQ_YOUR_TURN = 15, // 0x0000000F
    REQ_NONSTEALTH_ENEMY_TARGET = 16, // 0x00000010
    REQ_HERO_TARGET = 17, // 0x00000011
    REQ_SECRET_ZONE_CAP = 18, // 0x00000012
    REQ_MINION_CAP_IF_TARGET_AVAILABLE = 19, // 0x00000013
    REQ_MINION_CAP = 20, // 0x00000014
    REQ_TARGET_ATTACKED_THIS_TURN = 21, // 0x00000015
    REQ_TARGET_IF_AVAILABLE = 22, // 0x00000016
    REQ_MINIMUM_ENEMY_MINIONS = 23, // 0x00000017
    REQ_TARGET_FOR_COMBO = 24, // 0x00000018
    REQ_NOT_EXHAUSTED_ACTIVATE = 25, // 0x00000019
    REQ_UNIQUE_SECRET_OR_QUEST = 26, // 0x0000001A
    REQ_TARGET_TAUNTER = 27, // 0x0000001B
    REQ_CAN_BE_ATTACKED = 28, // 0x0000001C
    REQ_ACTION_PWR_IS_MASTER_PWR = 29, // 0x0000001D
    REQ_TARGET_MAGNET = 30, // 0x0000001E
    REQ_ATTACK_GREATER_THAN_0 = 31, // 0x0000001F
    REQ_ATTACKER_NOT_FROZEN = 32, // 0x00000020
    REQ_HERO_OR_MINION_TARGET = 33, // 0x00000021
    REQ_CAN_BE_TARGETED_BY_SPELLS = 34, // 0x00000022
    REQ_SUBCARD_IS_PLAYABLE = 35, // 0x00000023
    REQ_TARGET_FOR_NO_COMBO = 36, // 0x00000024
    REQ_NOT_MINION_JUST_PLAYED = 37, // 0x00000025
    REQ_NOT_EXHAUSTED_HERO_POWER = 38, // 0x00000026
    REQ_CAN_BE_TARGETED_BY_OPPONENTS = 39, // 0x00000027
    REQ_ATTACKER_CAN_ATTACK = 40, // 0x00000028
    REQ_TARGET_MIN_ATTACK = 41, // 0x00000029
    REQ_CAN_BE_TARGETED_BY_HERO_POWERS = 42, // 0x0000002A
    REQ_ENEMY_TARGET_NOT_IMMUNE = 43, // 0x0000002B
    REQ_ALL_BASIC_TOTEMS_NOT_IN_PLAY = 44, // 0x0000002C
    REQ_MINIMUM_TOTAL_MINIONS = 45, // 0x0000002D
    REQ_MUST_TARGET_TAUNTER = 46, // 0x0000002E
    REQ_UNDAMAGED_TARGET = 47, // 0x0000002F
    REQ_CAN_BE_TARGETED_BY_BATTLECRIES = 48, // 0x00000030
    REQ_STEADY_SHOT = 49, // 0x00000031
    REQ_MINION_OR_ENEMY_HERO = 50, // 0x00000032
    REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND = 51, // 0x00000033
    REQ_LEGENDARY_TARGET = 52, // 0x00000034
    REQ_FRIENDLY_MINION_DIED_THIS_TURN = 53, // 0x00000035
    REQ_FRIENDLY_MINION_DIED_THIS_GAME = 54, // 0x00000036
    REQ_ENEMY_WEAPON_EQUIPPED = 55, // 0x00000037
    REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS = 56, // 0x00000038
    REQ_TARGET_WITH_BATTLECRY = 57, // 0x00000039
    REQ_TARGET_WITH_DEATHRATTLE = 58, // 0x0000003A
    REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS = 59, // 0x0000003B
    REQ_SECRET_ZONE_CAP_FOR_NON_SECRET = 60, // 0x0000003C
    REQ_TARGET_EXACT_COST = 61, // 0x0000003D
    REQ_STEALTHED_TARGET = 62, // 0x0000003E
    REQ_MINION_SLOT_OR_MANA_CRYSTAL_SLOT = 63, // 0x0000003F
    REQ_MAX_QUESTS = 64, // 0x00000040
    REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN = 65, // 0x00000041
    REQ_TARGET_NOT_VAMPIRE = 66, // 0x00000042
    REQ_TARGET_NOT_DAMAGEABLE_ONLY_BY_WEAPONS = 67, // 0x00000043
    REQ_NOT_DISABLED_HERO_POWER = 68, // 0x00000044
    REQ_MUST_PLAY_OTHER_CARD_FIRST = 69, // 0x00000045
    REQ_HAND_NOT_FULL = 70, // 0x00000046
    REQ_TARGET_IF_AVAILABLE_AND_NO_3_COST_CARD_IN_DECK = 71, // 0x00000047
    REQ_CAN_BE_TARGETED_BY_COMBOS = 72, // 0x00000048
    REQ_CANNOT_PLAY_THIS = 73, // 0x00000049
    REQ_FRIENDLY_MINIONS_OF_RACE_DIED_THIS_GAME = 74, // 0x0000004A
    REQ_OPPONENT_PLAYED_CARDS_THIS_GAME = 77, // 0x0000004D
    REQ_LITERALLY_UNPLAYABLE = 78, // 0x0000004E
    REQ_TARGET_IF_AVAILABLE_AND_HERO_HAS_ATTACK = 79, // 0x0000004F
    REQ_FRIENDLY_MINION_OF_RACE_DIED_THIS_TURN = 80, // 0x00000050
    REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_SPELLS_PLAYED_THIS_TURN = 81, // 0x00000051
    REQ_FRIENDLY_MINION_OF_RACE_IN_HAND = 82, // 0x00000052
    REQ_FRIENDLY_DEATHRATTLE_MINION_DIED_THIS_GAME = 86, // 0x00000056
    REQ_FRIENDLY_REBORN_MINION_DIED_THIS_GAME = 89, // 0x00000059
    REQ_MINION_DIED_THIS_GAME = 90, // 0x0000005A
    REQ_BOARD_NOT_COMPLETELY_FULL = 92, // 0x0000005C
    REQ_TARGET_IF_AVAILABLE_AND_HAS_OVERLOADED_MANA = 93, // 0x0000005D
    REQ_TARGET_IF_AVAILABLE_AND_HERO_ATTACKED_THIS_TURN = 94, // 0x0000005E
    REQ_TARGET_IF_AVAILABLE_AND_DRAWN_THIS_TURN = 95, // 0x0000005F
    REQ_TARGET_IF_AVAILABLE_AND_NOT_DRAWN_THIS_TURN = 96, // 0x00000060
    REQ_TARGET_NON_TRIPLED_MINION = 97, // 0x00000061
    REQ_BOUGHT_MINION_THIS_TURN = 98, // 0x00000062
    REQ_SOLD_MINION_THIS_TURN = 99, // 0x00000063
    REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN = 100, // 0x00000064
    REQ_TARGET_IF_AVAILABLE_AND_SOUL_FRAGMENT_IN_DECK = 101, // 0x00000065
    REQ_DAMAGED_TARGET_UNLESS_COMBO = 102, // 0x00000066
    REQ_NOT_MINION_DORMANT = 103, // 0x00000067
    REQ_TARGET_NOT_DORMANT = 104, // 0x00000068
    REQ_TARGET_IF_AVAILABLE_AND_BOUGHT_RACE_THIS_TURN = 105, // 0x00000069
    REQ_TARGET_IF_AVAILABLE_AND_SOLD_RACE_THIS_TURN = 106, // 0x0000006A
    REQ_NOT_IN_COOLDOWN = 107, // 0x0000006B
    REQ_TARGET_IS_MERC = 108, // 0x0000006C
    REQ_TARGET_IS_NON_MERC = 109, // 0x0000006D
    REQ_TWO_OF_A_KIND = 110, // 0x0000006E
    REQ_HAS_OVERLOADED_MANA = 111, // 0x0000006F
    REQ_LETTUCE_ABILITY_CANNOT_TARGET_OWNER = 112, // 0x00000070
    REQ_TARGET_NOT_HAVE_TAG = 116, // 0x00000074
    REQ_TARGET_MUST_HAVE_TAG = 117, // 0x00000075
    REQ_TRADEABLE = 119, // 0x00000077
    REQ_NOT_LEGENDARY_TARGET = 123, // 0x0000007B
    REQ_MINIMUM_TAVERN_TIER_LEVEL_TO_PLAY = 128, // 0x00000080
    REQ_CARD_TAVERN_TIER_LEVEL_TO_PLAY = 129, // 0x00000081
    REQ_NOT_EXHAUSTED_LOCATION = 130, // 0x00000082
    REQ_LOCATION_TARGET = 131, // 0x00000083
    REQ_TARGET_SILVER_HAND_RECRUIT = 132, // 0x00000084
    REQ_MINIMUM_CORPSES = 133, // 0x00000085
    REQ_LOCATION_OR_MINION_TARGET = 134, // 0x00000086
    REQ_CAN_BE_TARGETED_BY_LOCATIONS = 135, // 0x00000087
    REQ_DRAG_TO_PLAY = 999, // 0x000003E7
  }
}
