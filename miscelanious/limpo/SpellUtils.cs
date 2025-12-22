using Blizzard.T5.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellUtils
{
  public static SpellClassTag ConvertClassTagToSpellEnum(TAG_CLASS classTag)
  {
    switch (classTag)
    {
      case TAG_CLASS.DEATHKNIGHT:
        return SpellClassTag.DEATHKNIGHT;
      case TAG_CLASS.DRUID:
        return SpellClassTag.DRUID;
      case TAG_CLASS.HUNTER:
        return SpellClassTag.HUNTER;
      case TAG_CLASS.MAGE:
        return SpellClassTag.MAGE;
      case TAG_CLASS.PALADIN:
        return SpellClassTag.PALADIN;
      case TAG_CLASS.PRIEST:
        return SpellClassTag.PRIEST;
      case TAG_CLASS.ROGUE:
        return SpellClassTag.ROGUE;
      case TAG_CLASS.SHAMAN:
        return SpellClassTag.SHAMAN;
      case TAG_CLASS.WARLOCK:
        return SpellClassTag.WARLOCK;
      case TAG_CLASS.WARRIOR:
        return SpellClassTag.WARRIOR;
      default:
        return SpellClassTag.NONE;
    }
  }

  public static Player.Side ConvertSpellSideToPlayerSide(
    Spell spell,
    SpellPlayerSide spellSide)
  {
    Card sourceCard = spell.GetSourceCard();
    Entity entity = (UnityEngine.Object) sourceCard != (UnityEngine.Object) null ? sourceCard.GetEntity() : (Entity) null;
    switch (spellSide)
    {
      case SpellPlayerSide.FRIENDLY:
        return Player.Side.FRIENDLY;
      case SpellPlayerSide.OPPONENT:
        return Player.Side.OPPOSING;
      case SpellPlayerSide.SOURCE:
        if (entity == null)
        {
          Log.Gameplay.PrintError("sourceEntity null for spell: {0}", (object) spell.name);
          return Player.Side.NEUTRAL;
        }
        return entity.IsControlledByFriendlySidePlayer() ? Player.Side.FRIENDLY : Player.Side.OPPOSING;
      case SpellPlayerSide.TARGET:
        if (entity == null)
        {
          Log.Gameplay.PrintError("sourceEntity null for spell: {0}", (object) spell.name);
          return Player.Side.NEUTRAL;
        }
        return entity.IsControlledByFriendlySidePlayer() ? Player.Side.OPPOSING : Player.Side.FRIENDLY;
      default:
        return Player.Side.NEUTRAL;
    }
  }

  public static List<Zone> FindZonesFromTag(SpellZoneTag zoneTag)
  {
    ZoneMgr zoneMgr = ZoneMgr.Get();
    if ((UnityEngine.Object) zoneMgr == (UnityEngine.Object) null)
      return (List<Zone>) null;
    switch (zoneTag)
    {
      case SpellZoneTag.PLAY:
        return zoneMgr.FindZonesOfType<Zone, ZonePlay>();
      case SpellZoneTag.HERO:
        return zoneMgr.FindZonesOfType<Zone, ZoneHero>();
      case SpellZoneTag.HERO_POWER:
        return zoneMgr.FindZonesOfType<Zone, ZoneHeroPower>();
      case SpellZoneTag.WEAPON:
        return zoneMgr.FindZonesOfType<Zone, ZoneWeapon>();
      case SpellZoneTag.DECK:
        return zoneMgr.FindZonesOfType<Zone, ZoneDeck>();
      case SpellZoneTag.HAND:
        return zoneMgr.FindZonesOfType<Zone, ZoneHand>();
      case SpellZoneTag.GRAVEYARD:
        return zoneMgr.FindZonesOfType<Zone, ZoneGraveyard>();
      case SpellZoneTag.SECRET:
        return zoneMgr.FindZonesOfType<Zone, ZoneSecret>();
      case SpellZoneTag.HERO_BUDDY:
        return zoneMgr.FindZonesOfType<Zone, ZoneBattlegroundHeroBuddy>();
      case SpellZoneTag.QUEST_REWARD:
        return zoneMgr.FindZonesOfType<Zone, ZoneBattlegroundQuestReward>();
      default:
        Debug.LogWarning((object) string.Format("SpellUtils.FindZonesFromTag() - unhandled zoneTag {0}", (object) zoneTag));
        return (List<Zone>) null;
    }
  }

  public static List<Zone> FindZonesFromTag(
    Spell spell,
    SpellZoneTag zoneTag,
    SpellPlayerSide spellSide)
  {
    if ((UnityEngine.Object) ZoneMgr.Get() == (UnityEngine.Object) null)
      return (List<Zone>) null;
    if (spellSide == SpellPlayerSide.NEUTRAL)
      return (List<Zone>) null;
    if (spellSide == SpellPlayerSide.BOTH)
      return SpellUtils.FindZonesFromTag(zoneTag);
    Player.Side playerSide = SpellUtils.ConvertSpellSideToPlayerSide(spell, spellSide);
    switch (zoneTag)
    {
      case SpellZoneTag.PLAY:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZonePlay>(playerSide);
      case SpellZoneTag.HERO:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZoneHero>(playerSide);
      case SpellZoneTag.HERO_POWER:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZoneHeroPower>(playerSide);
      case SpellZoneTag.WEAPON:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZoneWeapon>(playerSide);
      case SpellZoneTag.DECK:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZoneDeck>(playerSide);
      case SpellZoneTag.HAND:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZoneHand>(playerSide);
      case SpellZoneTag.GRAVEYARD:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZoneGraveyard>(playerSide);
      case SpellZoneTag.SECRET:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZoneSecret>(playerSide);
      case SpellZoneTag.HERO_BUDDY:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZoneBattlegroundHeroBuddy>(playerSide);
      case SpellZoneTag.QUEST_REWARD:
        return ZoneMgr.Get().FindZonesOfType<Zone, ZoneBattlegroundQuestReward>(playerSide);
      default:
        Debug.LogWarning((object) string.Format("SpellUtils.FindZonesFromTag() - Unhandled zoneTag {0}. spellSide={1} playerSide={2}", (object) zoneTag, (object) spellSide, (object) playerSide));
        return (List<Zone>) null;
    }
  }

  public static Transform GetLocationTransform(Spell spell)
  {
    GameObject locationObject = SpellUtils.GetLocationObject(spell);
    return !((UnityEngine.Object) locationObject == (UnityEngine.Object) null) ? locationObject.transform : (Transform) null;
  }

  public static GameObject GetLocationObject(Spell spell)
  {
    SpellLocation location = spell.GetLocation();
    return SpellUtils.GetSpellLocationObject(spell, location);
  }

  public static GameObject GetSpellLocationObject(
    Spell spell,
    SpellLocation location,
    string overrideTransformName = null)
  {
    if (location == SpellLocation.NONE)
      return (GameObject) null;
    GameObject parentObject = (GameObject) null;
    if (location == SpellLocation.SOURCE)
      parentObject = spell.GetSource();
    else if (location == SpellLocation.SOURCE_AUTO)
      parentObject = SpellUtils.FindSourceAutoObjectForSpell(spell);
    else if (location == SpellLocation.SOURCE_HERO)
    {
      Card sourceCard = spell.GetSourceCard();
      Card card = sourceCard?.GetEntity().GetLettuceAbilityOwner()?.GetCard();
      if ((UnityEngine.Object) card != (UnityEngine.Object) null)
        parentObject = card.gameObject;
      Card heroCard = SpellUtils.FindHeroCard(sourceCard);
      if ((UnityEngine.Object) heroCard != (UnityEngine.Object) null)
        parentObject = heroCard.gameObject;
    }
    else if (location == SpellLocation.SOURCE_HERO_POWER)
    {
      Card heroPowerCard = SpellUtils.FindHeroPowerCard(spell.GetSourceCard());
      if ((UnityEngine.Object) heroPowerCard == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = heroPowerCard.gameObject;
    }
    else if (location == SpellLocation.SOURCE_PLAY_ZONE)
    {
      Card sourceCard = spell.GetSourceCard();
      if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null)
        return (GameObject) null;
      Player controller = sourceCard.GetEntity().GetController();
      ZonePlay zoneOfType = ZoneMgr.Get().FindZoneOfType<ZonePlay>(controller.GetSide());
      if ((UnityEngine.Object) zoneOfType == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.SOURCE_HAND_ZONE)
    {
      Card sourceCard = spell.GetSourceCard();
      if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null)
        return (GameObject) null;
      Player controller = sourceCard.GetEntity().GetController();
      ZoneHand zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneHand>(controller.GetSide());
      if ((UnityEngine.Object) zoneOfType == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.SOURCE_DECK_ZONE)
    {
      Card sourceCard = spell.GetSourceCard();
      if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null)
        return (GameObject) null;
      Player controller = sourceCard.GetEntity().GetController();
      ZoneDeck zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(controller.GetSide());
      if ((UnityEngine.Object) zoneOfType == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.TARGET)
      parentObject = spell.GetVisualTarget();
    else if (location == SpellLocation.TARGET_AUTO)
      parentObject = SpellUtils.FindTargetAutoObjectForSpell(spell);
    else if (location == SpellLocation.TARGET_HERO)
    {
      Card heroCard = SpellUtils.FindHeroCard(spell.GetVisualTargetCard());
      if ((UnityEngine.Object) heroCard == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = heroCard.gameObject;
    }
    else if (location == SpellLocation.TARGET_HERO_POWER)
    {
      Card heroPowerCard = SpellUtils.FindHeroPowerCard(spell.GetVisualTargetCard());
      if ((UnityEngine.Object) heroPowerCard == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = heroPowerCard.gameObject;
    }
    else if (location == SpellLocation.TARGET_PLAY_ZONE)
    {
      Card visualTargetCard = spell.GetVisualTargetCard();
      if ((UnityEngine.Object) visualTargetCard == (UnityEngine.Object) null)
        return (GameObject) null;
      Player controller = visualTargetCard.GetEntity().GetController();
      ZonePlay zoneOfType = ZoneMgr.Get().FindZoneOfType<ZonePlay>(controller.GetSide());
      if ((UnityEngine.Object) zoneOfType == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.TARGET_HAND_ZONE)
    {
      Card visualTargetCard = spell.GetVisualTargetCard();
      if ((UnityEngine.Object) visualTargetCard == (UnityEngine.Object) null)
        return (GameObject) null;
      Player controller = visualTargetCard.GetEntity().GetController();
      ZoneHand zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneHand>(controller.GetSide());
      if ((UnityEngine.Object) zoneOfType == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.TARGET_DECK_ZONE)
    {
      Card visualTargetCard = spell.GetVisualTargetCard();
      if ((UnityEngine.Object) visualTargetCard == (UnityEngine.Object) null)
        return (GameObject) null;
      Player controller = visualTargetCard.GetEntity().GetController();
      ZoneDeck zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(controller.GetSide());
      if ((UnityEngine.Object) zoneOfType == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.BOARD)
    {
      if ((UnityEngine.Object) Board.Get() == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = Board.Get().gameObject;
    }
    else if (location == SpellLocation.FRIENDLY_HERO)
    {
      Player friendlyPlayer = SpellUtils.FindFriendlyPlayer(spell);
      if (friendlyPlayer == null)
        return (GameObject) null;
      Card heroCard = friendlyPlayer.GetHeroCard();
      if (!(bool) (UnityEngine.Object) heroCard)
        return (GameObject) null;
      parentObject = heroCard.gameObject;
    }
    else if (location == SpellLocation.FRIENDLY_HERO_POWER)
    {
      Player friendlyPlayer = SpellUtils.FindFriendlyPlayer(spell);
      if (friendlyPlayer == null)
        return (GameObject) null;
      Card heroPowerCard = friendlyPlayer.GetHeroPowerCard();
      if (!(bool) (UnityEngine.Object) heroPowerCard)
        return (GameObject) null;
      parentObject = heroPowerCard.gameObject;
    }
    else if (location == SpellLocation.FRIENDLY_PLAY_ZONE)
    {
      ZonePlay friendlyPlayZone = SpellUtils.FindFriendlyPlayZone(spell);
      if (!(bool) (UnityEngine.Object) friendlyPlayZone)
        return (GameObject) null;
      parentObject = friendlyPlayZone.gameObject;
    }
    else if (location == SpellLocation.OPPONENT_HERO)
    {
      Player opponentPlayer = SpellUtils.FindOpponentPlayer(spell);
      if (opponentPlayer == null)
        return (GameObject) null;
      Card heroCard = opponentPlayer.GetHeroCard();
      if (!(bool) (UnityEngine.Object) heroCard)
        return (GameObject) null;
      parentObject = heroCard.gameObject;
    }
    else if (location == SpellLocation.OPPONENT_HERO_POWER)
    {
      Player opponentPlayer = SpellUtils.FindOpponentPlayer(spell);
      if (opponentPlayer == null)
        return (GameObject) null;
      Card heroPowerCard = opponentPlayer.GetHeroPowerCard();
      if (!(bool) (UnityEngine.Object) heroPowerCard)
        return (GameObject) null;
      parentObject = heroPowerCard.gameObject;
    }
    else if (location == SpellLocation.OPPONENT_PLAY_ZONE)
    {
      ZonePlay opponentPlayZone = SpellUtils.FindOpponentPlayZone(spell);
      if (!(bool) (UnityEngine.Object) opponentPlayZone)
        return (GameObject) null;
      parentObject = opponentPlayZone.gameObject;
    }
    else if (location == SpellLocation.CHOSEN_TARGET)
    {
      Card powerTargetCard = spell.GetPowerTargetCard();
      if ((UnityEngine.Object) powerTargetCard == (UnityEngine.Object) null)
        return (GameObject) null;
      parentObject = powerTargetCard.gameObject;
    }
    else if (location == SpellLocation.FRIENDLY_HAND_ZONE)
    {
      Player friendlyPlayer = SpellUtils.FindFriendlyPlayer(spell);
      if (friendlyPlayer == null)
        return (GameObject) null;
      ZoneHand zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneHand>(friendlyPlayer.GetSide());
      if (!(bool) (UnityEngine.Object) zoneOfType)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.OPPONENT_HAND_ZONE)
    {
      Player opponentPlayer = SpellUtils.FindOpponentPlayer(spell);
      if (opponentPlayer == null)
        return (GameObject) null;
      ZoneHand zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneHand>(opponentPlayer.GetSide());
      if (!(bool) (UnityEngine.Object) zoneOfType)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.FRIENDLY_DECK_ZONE)
    {
      Player friendlyPlayer = SpellUtils.FindFriendlyPlayer(spell);
      if (friendlyPlayer == null)
        return (GameObject) null;
      ZoneDeck zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(friendlyPlayer.GetSide());
      if (!(bool) (UnityEngine.Object) zoneOfType)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.OPPONENT_DECK_ZONE)
    {
      Player opponentPlayer = SpellUtils.FindOpponentPlayer(spell);
      if (opponentPlayer == null)
        return (GameObject) null;
      ZoneDeck zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(opponentPlayer.GetSide());
      if (!(bool) (UnityEngine.Object) zoneOfType)
        return (GameObject) null;
      parentObject = zoneOfType.gameObject;
    }
    else if (location == SpellLocation.FRIENDLY_WEAPON)
    {
      Player friendlyPlayer = SpellUtils.FindFriendlyPlayer(spell);
      if (friendlyPlayer == null)
        return (GameObject) null;
      Card weaponCard = friendlyPlayer.GetWeaponCard();
      if (!(bool) (UnityEngine.Object) weaponCard)
      {
        ZoneWeapon zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneWeapon>(friendlyPlayer.GetSide());
        if (!(bool) (UnityEngine.Object) zoneOfType)
          return (GameObject) null;
        parentObject = zoneOfType.gameObject;
      }
      else
        parentObject = weaponCard.gameObject;
    }
    else if (location == SpellLocation.OPPONENT_WEAPON)
    {
      Player opponentPlayer = SpellUtils.FindOpponentPlayer(spell);
      if (opponentPlayer == null)
        return (GameObject) null;
      Card weaponCard = opponentPlayer.GetWeaponCard();
      if (!(bool) (UnityEngine.Object) weaponCard)
      {
        ZoneWeapon zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneWeapon>(opponentPlayer.GetSide());
        if (!(bool) (UnityEngine.Object) zoneOfType)
          return (GameObject) null;
        parentObject = zoneOfType.gameObject;
      }
      else
        parentObject = weaponCard.gameObject;
    }
    else if (location == SpellLocation.FRIENDLY_HERO_BUDDY)
    {
      Player friendlyPlayer = SpellUtils.FindFriendlyPlayer(spell);
      if (friendlyPlayer == null)
        return (GameObject) null;
      Card heroBuddyCard = friendlyPlayer.GetHeroBuddyCard();
      if (!(bool) (UnityEngine.Object) heroBuddyCard)
      {
        ZoneBattlegroundHeroBuddy zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneBattlegroundHeroBuddy>(friendlyPlayer.GetSide());
        if (!(bool) (UnityEngine.Object) zoneOfType)
          return (GameObject) null;
        parentObject = zoneOfType.gameObject;
      }
      else
        parentObject = heroBuddyCard.gameObject;
    }
    else if (location == SpellLocation.OPPONENT_HERO_BUDDY)
    {
      Player opponentPlayer = SpellUtils.FindOpponentPlayer(spell);
      if (opponentPlayer == null)
        return (GameObject) null;
      Card heroBuddyCard = opponentPlayer.GetHeroBuddyCard();
      if (!(bool) (UnityEngine.Object) heroBuddyCard)
      {
        ZoneBattlegroundHeroBuddy zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneBattlegroundHeroBuddy>(opponentPlayer.GetSide());
        if (!(bool) (UnityEngine.Object) zoneOfType)
          return (GameObject) null;
        parentObject = zoneOfType.gameObject;
      }
      else
        parentObject = heroBuddyCard.gameObject;
    }
    else if (location == SpellLocation.FRIENDLY_QUEST_REWARD || location == SpellLocation.FRIENDLY_QUEST_REWARD_HERO_POWER || location == SpellLocation.OPPONENT_QUEST_REWARD || location == SpellLocation.OPPONENT_QUEST_REWARD_HERO_POWER)
    {
      Player player = location == SpellLocation.FRIENDLY_QUEST_REWARD || location == SpellLocation.FRIENDLY_QUEST_REWARD_HERO_POWER ? SpellUtils.FindFriendlyPlayer(spell) : SpellUtils.FindOpponentPlayer(spell);
      if (player == null)
        return (GameObject) null;
      Card card = location == SpellLocation.FRIENDLY_QUEST_REWARD || location == SpellLocation.OPPONENT_QUEST_REWARD ? player.GetQuestRewardCard() : player.GetQuestRewardFromHeroPowerCard();
      if (!(bool) (UnityEngine.Object) card)
      {
        List<ZoneBattlegroundQuestReward> zonesOfType = ZoneMgr.Get().FindZonesOfType<ZoneBattlegroundQuestReward>(player.GetSide());
        if (zonesOfType.Count == 0)
          return (GameObject) null;
        foreach (ZoneBattlegroundQuestReward battlegroundQuestReward in zonesOfType)
        {
          if (battlegroundQuestReward.m_isHeroPower == (location == SpellLocation.FRIENDLY_QUEST_REWARD_HERO_POWER))
          {
            parentObject = battlegroundQuestReward.gameObject;
            break;
          }
        }
      }
      else
        parentObject = card.gameObject;
    }
    if ((UnityEngine.Object) parentObject == (UnityEngine.Object) null)
      return (GameObject) null;
    if (string.IsNullOrEmpty(overrideTransformName))
      overrideTransformName = spell.GetLocationTransformName();
    if (!string.IsNullOrEmpty(overrideTransformName))
    {
      GameObject childBySubstring = GameObjectUtils.FindChildBySubstring(parentObject, overrideTransformName);
      if ((UnityEngine.Object) childBySubstring != (UnityEngine.Object) null)
        return childBySubstring;
    }
    Card component = parentObject.GetComponent<Card>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.GetEntity() != null)
    {
      Entity entity = component.GetEntity();
      if (entity.GetZone() == TAG_ZONE.SETASIDE)
      {
        Zone zoneOfType = (Zone) ZoneMgr.Get().FindZoneOfType<ZoneHero>(entity.GetControllerSide());
        if ((UnityEngine.Object) zoneOfType != (UnityEngine.Object) null)
          parentObject = zoneOfType.gameObject;
      }
    }
    return parentObject;
  }

  public static bool SetPositionFromLocation(Spell spell, bool setParent)
  {
    Transform locationTransform = SpellUtils.GetLocationTransform(spell);
    if ((UnityEngine.Object) locationTransform == (UnityEngine.Object) null)
      return false;
    if (setParent)
      spell.transform.parent = locationTransform;
    spell.transform.position = locationTransform.position;
    return true;
  }

  public static bool SetOrientationFromFacing(Spell spell)
  {
    SpellFacing facing = spell.GetFacing();
    if (facing == SpellFacing.NONE)
      return false;
    SpellFacingOptions options = spell.GetFacingOptions() ?? new SpellFacingOptions();
    switch (facing)
    {
      case SpellFacing.SAME_AS_SOURCE_HERO:
        Card heroCard1 = SpellUtils.FindHeroCard(spell.GetSourceCard());
        if ((UnityEngine.Object) heroCard1 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceSameAs((Component) spell, (Component) heroCard1, options);
        break;
      case SpellFacing.TOWARDS_TARGET:
        GameObject visualTarget = spell.GetVisualTarget();
        if ((UnityEngine.Object) visualTarget == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceTowards((Component) spell, visualTarget, options);
        break;
      case SpellFacing.TOWARDS_TARGET_HERO:
        Card heroCard2 = SpellUtils.FindHeroCard(SpellUtils.FindBestTargetCard(spell));
        if ((UnityEngine.Object) heroCard2 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceTowards((Component) spell, (Component) heroCard2, options);
        break;
      case SpellFacing.TOWARDS_SOURCE_HERO:
        Card heroCard3 = SpellUtils.FindHeroCard(spell.GetSourceCard());
        if ((UnityEngine.Object) heroCard3 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceTowards((Component) spell, (Component) heroCard3, options);
        break;
      case SpellFacing.TOWARDS_SOURCE:
        GameObject source1 = spell.GetSource();
        if ((UnityEngine.Object) source1 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceTowards((Component) spell, source1, options);
        break;
      case SpellFacing.TOWARDS_SOURCE_AUTO:
        GameObject autoObjectForSpell1 = SpellUtils.FindSourceAutoObjectForSpell(spell);
        if ((UnityEngine.Object) autoObjectForSpell1 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceTowards((Component) spell, autoObjectForSpell1, options);
        break;
      case SpellFacing.SAME_AS_SOURCE:
        GameObject source2 = spell.GetSource();
        if ((UnityEngine.Object) source2 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceSameAs((Component) spell, source2, options);
        break;
      case SpellFacing.SAME_AS_SOURCE_AUTO:
        GameObject autoObjectForSpell2 = SpellUtils.FindSourceAutoObjectForSpell(spell);
        if ((UnityEngine.Object) autoObjectForSpell2 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceSameAs((Component) spell, autoObjectForSpell2, options);
        break;
      case SpellFacing.TOWARDS_CHOSEN_TARGET:
        Card powerTargetCard = spell.GetPowerTargetCard();
        if ((UnityEngine.Object) powerTargetCard == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceTowards((Component) spell, (Component) powerTargetCard, options);
        break;
      case SpellFacing.OPPOSITE_OF_SOURCE:
        GameObject source3 = spell.GetSource();
        if ((UnityEngine.Object) source3 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceOppositeOf((Component) spell, source3, options);
        break;
      case SpellFacing.OPPOSITE_OF_SOURCE_AUTO:
        GameObject autoObjectForSpell3 = SpellUtils.FindSourceAutoObjectForSpell(spell);
        if ((UnityEngine.Object) autoObjectForSpell3 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceOppositeOf((Component) spell, autoObjectForSpell3, options);
        break;
      case SpellFacing.OPPOSITE_OF_SOURCE_HERO:
        Card heroCard4 = SpellUtils.FindHeroCard(spell.GetSourceCard());
        if ((UnityEngine.Object) heroCard4 == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceOppositeOf((Component) spell, (Component) heroCard4, options);
        break;
      case SpellFacing.TOWARDS_OPPONENT_HERO:
        Card opponentHeroCard = SpellUtils.FindOpponentHeroCard(spell);
        if ((UnityEngine.Object) opponentHeroCard == (UnityEngine.Object) null)
          return false;
        SpellUtils.FaceTowards((Component) spell, (Component) opponentHeroCard, options);
        break;
      default:
        return false;
    }
    return true;
  }

  public static Player FindFriendlyPlayer(Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return (Player) null;
    Card sourceCard = spell.GetSourceCard();
    return (UnityEngine.Object) sourceCard == (UnityEngine.Object) null ? (Player) null : sourceCard.GetEntity().GetController();
  }

  public static Player FindOpponentPlayer(Spell spell)
  {
    Player friendlyPlayer = SpellUtils.FindFriendlyPlayer(spell);
    return friendlyPlayer == null ? (Player) null : GameState.Get().GetFirstOpponentPlayer(friendlyPlayer);
  }

  public static ZonePlay FindFriendlyPlayZone(Spell spell)
  {
    Player friendlyPlayer = SpellUtils.FindFriendlyPlayer(spell);
    return friendlyPlayer == null ? (ZonePlay) null : ZoneMgr.Get().FindZoneOfType<ZonePlay>(friendlyPlayer.GetSide());
  }

  public static ZonePlay FindOpponentPlayZone(Spell spell)
  {
    Player opponentPlayer = SpellUtils.FindOpponentPlayer(spell);
    return opponentPlayer == null ? (ZonePlay) null : ZoneMgr.Get().FindZoneOfType<ZonePlay>(opponentPlayer.GetSide());
  }

  public static Card FindOpponentHeroCard(Spell spell) => SpellUtils.FindOpponentPlayer(spell)?.GetHeroCard();

  public static Zone FindTargetZone(Spell spell)
  {
    Card targetCard = spell.GetTargetCard();
    if ((UnityEngine.Object) targetCard == (UnityEngine.Object) null)
      return (Zone) null;
    Entity entity = targetCard.GetEntity();
    return ZoneMgr.Get().FindZoneForEntity(entity);
  }

  public static Actor GetParentActor(Spell spell) => GameObjectUtils.FindComponentInThisOrParents<Actor>(spell.gameObject);

  public static GameObject GetParentRootObject(Spell spell)
  {
    Actor parentActor = SpellUtils.GetParentActor(spell);
    return (UnityEngine.Object) parentActor == (UnityEngine.Object) null ? (GameObject) null : parentActor.GetRootObject();
  }

  public static MeshRenderer GetParentRootObjectMesh(Spell spell)
  {
    Actor parentActor = SpellUtils.GetParentActor(spell);
    return (UnityEngine.Object) parentActor == (UnityEngine.Object) null ? (MeshRenderer) null : parentActor.GetMeshRenderer();
  }

  public static bool IsNonMetaTaskListInMetaBlock(PowerTaskList taskList) => taskList.DoesBlockHaveEffectTimingMetaData() && !taskList.HasEffectTimingMetaData();

  public static bool CanAddPowerTargets(PowerTaskList taskList) => !SpellUtils.IsNonMetaTaskListInMetaBlock(taskList) && (taskList.HasTasks() || taskList.IsEndOfBlock());

  public static void SetCustomSpellParent(Spell spell, Component c)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null || (UnityEngine.Object) c == (UnityEngine.Object) null)
      return;
    spell.transform.parent = c.transform;
    spell.transform.localPosition = Vector3.zero;
  }

  public static Spell LoadAndSetupSpell(string spellPath, Component owner)
  {
    Spell spell = SpellManager.Get().GetSpell(spellPath);
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      Error.AddDevFatalUnlessWorkarounds("LoadAndSetupSpell() - \"{0}\" does not have a Spell component.", (object) spellPath);
      return (Spell) null;
    }
    if ((UnityEngine.Object) owner != (UnityEngine.Object) null)
      SpellUtils.SetupSpell(spell, owner);
    return spell;
  }

  public static void SetupSpell(Spell spell, Component c)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null || (UnityEngine.Object) c == (UnityEngine.Object) null)
      return;
    spell.SetSource(c.gameObject);
  }

  public static void SetupSoundSpell(CardSoundSpell spell, Component c)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null || (UnityEngine.Object) c == (UnityEngine.Object) null)
      return;
    spell.SetSource(c.gameObject);
    spell.transform.parent = c.transform;
    TransformUtil.Identity((Component) spell.transform);
  }

  public static bool ActivateStateIfNecessary(Spell spell, SpellStateType state)
  {
    switch (state)
    {
      case SpellStateType.BIRTH:
        return SpellUtils.ActivateBirthIfNecessary(spell);
      case SpellStateType.CANCEL:
        return SpellUtils.ActivateCancelIfNecessary(spell);
      case SpellStateType.DEATH:
        return SpellUtils.ActivateDeathIfNecessary(spell);
      default:
        if (!((UnityEngine.Object) spell != (UnityEngine.Object) null) || spell.GetActiveState() == state)
          return false;
        spell.ActivateState(state);
        return true;
    }
  }

  public static bool ActivateBirthIfNecessary(Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return false;
    switch (spell.GetActiveState())
    {
      case SpellStateType.BIRTH:
        return false;
      case SpellStateType.IDLE:
        return false;
      default:
        spell.ActivateState(SpellStateType.BIRTH);
        return true;
    }
  }

  public static bool ActivateDeathIfNecessary(Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return false;
    switch (spell.GetActiveState())
    {
      case SpellStateType.NONE:
        return false;
      case SpellStateType.DEATH:
        return false;
      default:
        spell.ActivateState(SpellStateType.DEATH);
        return true;
    }
  }

  public static bool ActivateCancelIfNecessary(Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
      return false;
    switch (spell.GetActiveState())
    {
      case SpellStateType.NONE:
        return false;
      case SpellStateType.CANCEL:
        return false;
      default:
        spell.ActivateState(SpellStateType.CANCEL);
        return true;
    }
  }

  public static void PurgeSpell(Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null || !spell.CanPurge())
      return;
    SpellManager.Get()?.ReleaseSpell(spell);
  }

  public static void PurgeSpells<T>(List<T> spells) where T : Spell
  {
    if (spells == null || spells.Count == 0)
      return;
    for (int index = 0; index < spells.Count; ++index)
      SpellUtils.PurgeSpell((Spell) spells[index]);
  }

  private static GameObject FindSourceAutoObjectForSpell(Spell spell)
  {
    GameObject source = spell.GetSource();
    Card sourceCard = spell.GetSourceCard();
    if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null)
      return source;
    Entity entity = sourceCard.GetEntity();
    TAG_CARDTYPE cardType = entity.GetCardType();
    PowerTaskList powerTaskList = spell.GetPowerTaskList();
    if (powerTaskList != null)
    {
      EntityDef effectEntityDef = powerTaskList.GetEffectEntityDef();
      if (effectEntityDef != null)
        cardType = effectEntityDef.GetCardType();
    }
    return SpellUtils.FindAutoObjectForSpell(entity, sourceCard, cardType);
  }

  private static GameObject FindTargetAutoObjectForSpell(Spell spell)
  {
    GameObject visualTarget = spell.GetVisualTarget();
    if ((UnityEngine.Object) visualTarget == (UnityEngine.Object) null)
      return (GameObject) null;
    Card component = visualTarget.GetComponent<Card>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return visualTarget;
    Entity entity = component.GetEntity();
    TAG_CARDTYPE cardType = entity.GetCardType();
    return SpellUtils.FindAutoObjectForSpell(entity, component, cardType);
  }

  private static GameObject FindAutoObjectForSpell(
    Entity entity,
    Card card,
    TAG_CARDTYPE cardType)
  {
    switch (cardType)
    {
      case TAG_CARDTYPE.SPELL:
        Card heroCard = entity.GetController().GetHeroCard();
        if ((UnityEngine.Object) heroCard != (UnityEngine.Object) null)
          return heroCard.gameObject;
        Card card1 = entity.GetLettuceAbilityOwner()?.GetCard();
        return (UnityEngine.Object) card1 != (UnityEngine.Object) null ? card1.gameObject : card.gameObject;
      case TAG_CARDTYPE.ENCHANTMENT:
        Entity entity1 = GameState.Get().GetEntity(entity.GetAttached());
        if (entity1 != null)
        {
          Card card2 = entity1.GetCard();
          if ((UnityEngine.Object) card2 != (UnityEngine.Object) null)
            return card2.gameObject;
          break;
        }
        break;
      case TAG_CARDTYPE.HERO_POWER:
        Card heroPowerCard = entity.GetController().GetHeroPowerCard();
        return (UnityEngine.Object) heroPowerCard == (UnityEngine.Object) null ? card.gameObject : heroPowerCard.gameObject;
    }
    return card.gameObject;
  }

  private static Card FindBestTargetCard(Spell spell)
  {
    Card sourceCard = spell.GetSourceCard();
    if ((UnityEngine.Object) sourceCard == (UnityEngine.Object) null)
      return spell.GetVisualTargetCard();
    Player controller = sourceCard.GetEntity().GetController();
    if (controller == null)
      return spell.GetVisualTargetCard();
    Player.Side side = controller.GetSide();
    List<GameObject> visualTargets = spell.GetVisualTargets();
    for (int index = 0; index < visualTargets.Count; ++index)
    {
      Card component = visualTargets[index].GetComponent<Card>();
      if (!((UnityEngine.Object) component == (UnityEngine.Object) null) && component.GetEntity().GetController().GetSide() != side)
        return component;
    }
    return spell.GetVisualTargetCard();
  }

  private static Card FindHeroCard(Card card)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return (Card) null;
    return card.GetEntity().GetController()?.GetHeroCard();
  }

  private static Card FindHeroPowerCard(Card card)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
      return (Card) null;
    return card.GetEntity().GetController()?.GetHeroPowerCard();
  }

  private static void FaceSameAs(Component source, GameObject target, SpellFacingOptions options) => SpellUtils.FaceSameAs(source.transform, target.transform, options);

  private static void FaceSameAs(Component source, Component target, SpellFacingOptions options) => SpellUtils.FaceSameAs(source.transform, target.transform, options);

  private static void FaceSameAs(Transform source, Transform target, SpellFacingOptions options) => SpellUtils.SetOrientation(source, target.position, target.position + target.forward, options);

  private static void FaceOppositeOf(
    Component source,
    GameObject target,
    SpellFacingOptions options)
  {
    SpellUtils.FaceOppositeOf(source.transform, target.transform, options);
  }

  private static void FaceOppositeOf(
    Component source,
    Component target,
    SpellFacingOptions options)
  {
    SpellUtils.FaceOppositeOf(source.transform, target.transform, options);
  }

  private static void FaceOppositeOf(
    Transform source,
    Transform target,
    SpellFacingOptions options)
  {
    SpellUtils.SetOrientation(source, target.position, target.position - target.forward, options);
  }

  private static void FaceTowards(Component source, GameObject target, SpellFacingOptions options) => SpellUtils.FaceTowards(source.transform, target.transform, options);

  private static void FaceTowards(Component source, Component target, SpellFacingOptions options) => SpellUtils.FaceTowards(source.transform, target.transform, options);

  private static void FaceTowards(Transform source, Transform target, SpellFacingOptions options) => SpellUtils.SetOrientation(source, source.position, target.position, options);

  private static void SetOrientation(
    Transform source,
    Vector3 sourcePosition,
    Vector3 targetPosition,
    SpellFacingOptions options)
  {
    if (!options.m_RotateX || !options.m_RotateY)
    {
      if (options.m_RotateX)
      {
        targetPosition.x = sourcePosition.x;
      }
      else
      {
        if (!options.m_RotateY)
          return;
        targetPosition.y = sourcePosition.y;
      }
    }
    Vector3 forward = targetPosition - sourcePosition;
    if ((double) forward.sqrMagnitude <= (double) Mathf.Epsilon)
      return;
    source.rotation = Quaternion.LookRotation(forward);
  }

  public static T GetAppropriateElementAccordingToRanges<T>(
    T[] elements,
    Func<T, ValueRange> rangeAccessor,
    int desiredValue)
  {
    if (elements.Length == 0)
      return default (T);
    int index1 = -1;
    int num1 = int.MinValue;
    int index2 = -1;
    int num2 = int.MaxValue;
    int index3 = -1;
    int index4 = 0;
    for (int length = elements.Length; index4 < length; ++index4)
    {
      T element = elements[index4];
      int maxValue = rangeAccessor(element).m_maxValue;
      if (maxValue > num1)
      {
        num1 = maxValue;
        index1 = index4;
      }
      int minValue = rangeAccessor(element).m_minValue;
      if (minValue < num2)
      {
        num2 = minValue;
        index2 = index4;
      }
      if (index3 == -1 && desiredValue >= minValue && desiredValue <= maxValue)
        index3 = index4;
    }
    if (desiredValue > num1 && index1 != -1)
      return elements[index1];
    if (desiredValue < num2 && index2 != -1)
      return elements[index2];
    return index3 != -1 ? elements[index3] : default (T);
  }

  public static IEnumerator FlipActorAndReplaceWithCard(
    Actor actor,
    Card card,
    float time)
  {
    float halfTime = time * 0.5f;
    card.HideCard();
    object[] objArray1 = new object[8]
    {
      (object) "z",
      (object) 90,
      (object) nameof (time),
      (object) halfTime,
      (object) "easetype",
      (object) iTween.EaseType.linear,
      (object) "name",
      (object) "SpellUtils.FlipActorAndReplaceWithCard"
    };
    iTween.RotateAdd(actor.gameObject, iTween.Hash(objArray1));
    while (iTween.HasName(actor.gameObject, "SpellUtils.FlipActorAndReplaceWithCard"))
      yield return (object) null;
    TransformUtil.CopyWorld((Component) card, (Component) actor);
    card.transform.rotation *= Quaternion.Euler(0.0f, 0.0f, 180f);
    actor.Hide();
    card.ShowCard();
    object[] objArray2 = new object[8]
    {
      (object) "z",
      (object) 90,
      (object) nameof (time),
      (object) halfTime,
      (object) "easetype",
      (object) iTween.EaseType.linear,
      (object) "name",
      (object) "SpellUtils.FlipActorAndReplaceWithCard"
    };
    iTween.RotateAdd(card.gameObject, iTween.Hash(objArray2));
    while (iTween.HasName(card.gameObject, "SpellUtils.FlipActorAndReplaceWithCard"))
      yield return (object) null;
  }

  public static IEnumerator FlipActorAndReplaceWithOtherActor(
    Actor actor,
    Actor otherActor,
    float time)
  {
    float halfTime = time * 0.5f;
    otherActor.Hide();
    object[] objArray1 = new object[8]
    {
      (object) "z",
      (object) 90,
      (object) nameof (time),
      (object) halfTime,
      (object) "easetype",
      (object) iTween.EaseType.linear,
      (object) "name",
      (object) "SpellUtils.FlipActorAndReplaceWithOtherActor"
    };
    iTween.RotateAdd(actor.gameObject, iTween.Hash(objArray1));
    while (iTween.HasName(actor.gameObject, "SpellUtils.FlipActorAndReplaceWithOtherActor"))
      yield return (object) null;
    TransformUtil.CopyWorld((Component) otherActor, (Component) actor);
    otherActor.transform.rotation *= Quaternion.Euler(0.0f, 0.0f, 180f);
    actor.Hide();
    otherActor.Show();
    object[] objArray2 = new object[8]
    {
      (object) "z",
      (object) 90,
      (object) nameof (time),
      (object) halfTime,
      (object) "easetype",
      (object) iTween.EaseType.linear,
      (object) "name",
      (object) "SpellUtils.FlipActorAndReplaceWithOtherActor"
    };
    iTween.RotateAdd(otherActor.gameObject, iTween.Hash(objArray2));
    while (iTween.HasName(otherActor.gameObject, "SpellUtils.FlipActorAndReplaceWithOtherActor"))
      yield return (object) null;
  }
}
