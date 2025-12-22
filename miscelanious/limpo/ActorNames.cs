using Assets;
using Blizzard.T5.Core;
using UnityEngine;

public class ActorNames
{
  public static readonly Map<ActorNames.ACTOR_ASSET, string> s_actorAssets = new Map<ActorNames.ACTOR_ASSET, string>()
  {
    {
      ActorNames.ACTOR_ASSET.HAND_MINION,
      "Card_Hand_Ally.prefab:d00eb0f79080e0749993fe4619e9143d"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_SPELL,
      "Card_Hand_Ability.prefab:3c3f5189f0d0b3745a1c1ca21d41efe0"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_WEAPON,
      "Card_Hand_Weapon.prefab:30888a1fdca5c6c43abcc5d9dca55783"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_HERO,
      "Card_Hand_Hero.prefab:a977c49edb5fb5d4c8dee4d2344d1395"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_MERCENARY,
      "Card_Hand_Mercenary.prefab:f9e5a62d0cf1f4b4db131efcf1a082c0"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_LOCATION,
      "Card_Hand_Location.prefab:bc312fcf691884a40967dae38d4d8b79"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_HERO,
      "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_MINION,
      "Card_Play_Ally.prefab:23b7de16184fa8042bf6b734e7ca4d60"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_WEAPON,
      "Card_Play_Weapon.prefab:71f767d4f10681a45ac853936d1db800"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_HERO_POWER,
      "Card_Play_HeroPower.prefab:a3794839abb947146903a26be13e09af"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_GAME_MODE_BUTTON,
      "Card_Play_GameModeButton.prefab:6d260d8912ac3f945a4177ba5882eaf2"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_MOVE_MINION_HOVER_TARGET,
      "Card_Play_MoveMinionHoverTarget.prefab:1f57541a9fdc77344810e84b76693bc4"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_BATTLEGROUND_HERO_BUDDY,
      "Card_Play_BG_Hero_Buddy.prefab:ad7824941d570f545a0afc6b9927e5c2"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_MERCENARY,
      "Card_Play_Mercenary.prefab:7c4e1f3052ce6e545a018b7131dad5ad"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_LETTUCE_ABILITY_SPELL,
      "Card_Play_LettuceAbility.prefab:c580722c24bcdbd4d9125352d1275e69"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_LETTUCE_ABILITY_MINION,
      "Card_Play_LettuceAbility_Minion.prefab:9820a8900603e844fb08fcb5493f0334"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_LETTUCE_EQUIPMENT,
      "Card_Play_LettuceEquipment.prefab:029c966daebb81343ad4c07bc85deaad"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_LOCATION,
      "Card_Play_Location.prefab:f4ee385d6c5c2a54cb83c817676b3e96"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_BATTLEGROUND_QUEST_REWARD,
      "Card_Play_Bacon_HeroPower_Quests.prefab:7aeb77ca586b96449b626fc2284d8e7e"
    },
    {
      ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER,
      "History_HeroPower.prefab:e73edf8ccea2b11429093f7a448eef53"
    },
    {
      ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER_OPPONENT,
      "History_HeroPower_Opponent.prefab:a99d23d6e8630f94b96a8e096fffb16f"
    },
    {
      ActorNames.ACTOR_ASSET.BIG_CARD_LETTUCE_ABILITY_SPELL,
      "BigCard_LettuceAbility.prefab:53cf859e0a512a240b9a6b1f8ad524b1"
    },
    {
      ActorNames.ACTOR_ASSET.BIG_CARD_LETTUCE_ABILITY_MINION,
      "BigCard_LettuceAbility_Minion.prefab:cd2ed854b5e5ef542806802188fb40d5"
    },
    {
      ActorNames.ACTOR_ASSET.BIG_CARD_LETTUCE_EQUIPMENT,
      "BigCard_LettuceEquipment.prefab:2b360077e2dc4ec4299908d851b32a5b"
    },
    {
      ActorNames.ACTOR_ASSET.BIG_CARD_BG_HERO_BUDDY,
      "Big_Card_BG_Hero_Buddy.prefab:4d5862fe52114a8419a76466756a9bce"
    }
  };
  public static readonly Map<ActorNames.ACTOR_ASSET, string> s_premiumActorAssets = new Map<ActorNames.ACTOR_ASSET, string>()
  {
    {
      ActorNames.ACTOR_ASSET.HAND_MINION,
      "Card_Hand_Ally_Premium.prefab:b0f0a4abee3293540830967b829f2bec"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_SPELL,
      "Card_Hand_Ability_Premium.prefab:5105f461bc4a48e4c8bf452b93cfd772"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_WEAPON,
      "Card_Hand_Weapon_Premium.prefab:c7736007f7a350942bbe40e466ac357c"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_HERO,
      "Card_Hand_Hero_Premium.prefab:aca669662daf766449cd351fe4691f8f"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_MERCENARY,
      "Card_Hand_Mercenary_Tier_2.prefab:9c9adc8aa105ac24296f1e1538faf951"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_LOCATION,
      "Card_Hand_Location_Premium.prefab:7de3ab2e9ed39f84fae0b6376494577b"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_MINION,
      "Card_Play_Ally_Premium.prefab:99bd268ec3a056d4795110a141c6fd75"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_WEAPON,
      "Card_Play_Weapon_Premium.prefab:66cbba9ed8f300c43834ab519327f094"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_HERO_POWER,
      "Card_Play_HeroPower_Premium.prefab:015ad985f9ec49e4db327d131fd79901"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_GAME_MODE_BUTTON,
      "Card_Play_GameModeButton.prefab:6d260d8912ac3f945a4177ba5882eaf2"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_MOVE_MINION_HOVER_TARGET,
      "Card_Play_MoveMinionHoverTarget.prefab:1f57541a9fdc77344810e84b76693bc4"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_BATTLEGROUND_HERO_BUDDY,
      "Card_Play_BG_Hero_Buddy.prefab:ad7824941d570f545a0afc6b9927e5c2"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_MERCENARY,
      "Card_Play_Mercenary_Tier_2.prefab:c8dcb22e4703ddd4a8584b5dade8b924"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_LETTUCE_ABILITY_SPELL,
      "Card_Play_LettuceAbility.prefab:c580722c24bcdbd4d9125352d1275e69"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_LETTUCE_ABILITY_MINION,
      "Card_Play_LettuceAbility_Minion.prefab:9820a8900603e844fb08fcb5493f0334"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_LETTUCE_EQUIPMENT,
      "Card_Play_LettuceEquipment.prefab:029c966daebb81343ad4c07bc85deaad"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_LOCATION,
      "Card_Play_Location_Premium.prefab:b7fc72340ca46464699682c3a9758343"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_BATTLEGROUND_QUEST_REWARD,
      "Card_Play_Bacon_HeroPower_Quests.prefab:7aeb77ca586b96449b626fc2284d8e7e"
    },
    {
      ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER,
      "History_HeroPower_Premium.prefab:081da807b95b8495e9f16825c5164787"
    },
    {
      ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER_OPPONENT,
      "History_HeroPower_Opponent_Premium.prefab:82e1456f33aae4b3d9b2dac73aaa3ffa"
    },
    {
      ActorNames.ACTOR_ASSET.BIG_CARD_LETTUCE_ABILITY_SPELL,
      "BigCard_LettuceAbility.prefab:53cf859e0a512a240b9a6b1f8ad524b1"
    },
    {
      ActorNames.ACTOR_ASSET.BIG_CARD_LETTUCE_ABILITY_MINION,
      "BigCard_LettuceAbility_Minion.prefab:cd2ed854b5e5ef542806802188fb40d5"
    },
    {
      ActorNames.ACTOR_ASSET.BIG_CARD_LETTUCE_EQUIPMENT,
      "BigCard_LettuceEquipment.prefab:2b360077e2dc4ec4299908d851b32a5b"
    },
    {
      ActorNames.ACTOR_ASSET.BIG_CARD_BG_HERO_BUDDY,
      "Big_Card_BG_Hero_Buddy.prefab:4d5862fe52114a8419a76466756a9bce"
    }
  };
  public static readonly Map<ActorNames.ACTOR_ASSET, string> s_diamondActorAssets = new Map<ActorNames.ACTOR_ASSET, string>()
  {
    {
      ActorNames.ACTOR_ASSET.HAND_MINION,
      "Card_Hand_Ally_Diamond.prefab:5fdbef3fa7e0c05419050d01202a85d3"
    },
    {
      ActorNames.ACTOR_ASSET.HAND_MERCENARY,
      "Card_Hand_Mercenary_Tier_3.prefab:1d00ef78a06433d4eb7eb52b9cccfc3a"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_MINION,
      "Card_Play_Ally_Diamond.prefab:42fb12461ed7d0142a34f9b72399421c"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_MERCENARY,
      "Card_Play_Mercenary_Tier_3.prefab:bf967a38c2a6edf4c9b64d49f0ce41df"
    }
  };
  public static readonly Map<ActorNames.ACTOR_ASSET, string> s_signatureActorAssets = new Map<ActorNames.ACTOR_ASSET, string>()
  {
    {
      ActorNames.ACTOR_ASSET.HAND_MINION,
      "Card_Hand_Ally_Signature_25.prefab:44c7a553fe181c840b0e700652af7caa"
    },
    {
      ActorNames.ACTOR_ASSET.PLAY_MINION,
      "Card_Play_Ally_Signature_25.prefab:11170861f6a754f43b227f61faa8e121"
    }
  };

  public static string GetZoneActor(
    EntityBase entityBase,
    TAG_ZONE zoneTag,
    Player controller,
    TAG_PREMIUM premium)
  {
    TAG_CARDTYPE cardType = entityBase.GetCardType();
    TAG_CLASS tagClass = entityBase.GetClass();
    bool flag1 = entityBase.IsQuest();
    bool flag2 = entityBase.IsSideQuest();
    bool flag3 = entityBase.IsQuestline();
    bool flag4 = entityBase.IsSigil();
    bool flag5 = entityBase.IsObjective();
    bool flag6 = entityBase.HasTag(GAME_TAG.GHOSTLY);
    bool flag7 = entityBase.IsPuzzle();
    TAG_PUZZLE_TYPE puzzleType = entityBase.GetPuzzleType();
    bool flag8 = entityBase.IsRulebook();
    switch (zoneTag)
    {
      case TAG_ZONE.PLAY:
      case TAG_ZONE.LETTUCE_ABILITY:
        string playActorByTags = ActorNames.GetPlayActorByTags(entityBase, premium);
        if (!"Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9".Equals(playActorByTags))
          return playActorByTags;
        break;
      case TAG_ZONE.DECK:
      case TAG_ZONE.REMOVEDFROMGAME:
      case TAG_ZONE.SETASIDE:
        return "Card_Invisible.prefab:579b3b9a80234754593f24582f9cb93b";
      case TAG_ZONE.HAND:
        if (controller == null || !controller.IsRevealed())
          return "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9";
        string handActorByTags = ActorNames.GetHandActorByTags(entityBase, premium);
        if (!"Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9".Equals(handActorByTags))
          return handActorByTags;
        break;
      case TAG_ZONE.GRAVEYARD:
        if (flag6 && controller.GetSide() == Player.Side.OPPOSING)
          return "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9";
        switch (cardType)
        {
          case TAG_CARDTYPE.HERO:
            return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_HERO, premium);
          case TAG_CARDTYPE.MINION:
            return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_MINION, premium);
          case TAG_CARDTYPE.SPELL:
            return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_SPELL, premium);
          case TAG_CARDTYPE.WEAPON:
            return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_WEAPON, premium);
          case TAG_CARDTYPE.BATTLEGROUND_HERO_BUDDY:
            return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_SPELL, premium);
          case TAG_CARDTYPE.LOCATION:
            return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_LOCATION, premium);
          case TAG_CARDTYPE.BATTLEGROUND_QUEST_REWARD:
            return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_SPELL, premium);
        }
        break;
      case TAG_ZONE.SECRET:
        if (entityBase != null && entityBase.IsBobQuest())
          return "Card_Play_Bacon_Bob_Quest.prefab:b179261da0ff34e4390103a43c4d46dc";
        if (flag1)
          return GameMgr.Get() != null && GameMgr.Get().IsBattlegrounds() ? "Card_Play_Bacon_Quest.prefab:30e69416dada17e43b7f2722ffb25f0e" : "Card_Play_Quest.prefab:321b6d1ad558ebd46996c1f4eeaccb0c";
        if (flag3)
          return "Card_Play_Questline.prefab:47f7e5eb9be22de42813a3b32660a1d0";
        if (flag7)
        {
          switch (puzzleType)
          {
            case TAG_PUZZLE_TYPE.MIRROR:
              return "Card_Play_Puzzle_Mirror.prefab:4583d6e2b04fad74986ef47b4ff00c79";
            case TAG_PUZZLE_TYPE.LETHAL:
              return "Card_Play_Puzzle_Lethal.prefab:00d669c10a286e84cb91df1d40312d4b";
            case TAG_PUZZLE_TYPE.SURVIVAL:
              return "Card_Play_Puzzle_Survival.prefab:036a2c2eee552fc4db25051107a0b797";
            case TAG_PUZZLE_TYPE.CLEAR:
              return "Card_Play_Puzzle_BoardClear.prefab:fd9eec17f48c319468f103336095ad7b";
          }
        }
        if (flag8)
          return "Card_Play_Rulebook.prefab:a8fbb8b315f4a3244be82718c1606858";
        if (flag2)
        {
          switch (tagClass)
          {
            case TAG_CLASS.DRUID:
              return "Card_Play_SideQuest_Druid.prefab:d1430dc4bc9786640a02f4b178b59393";
            case TAG_CLASS.HUNTER:
              return "Card_Play_SideQuest_Hunter.prefab:c9ed37b5a056d4e4885dc882d9d37664";
            case TAG_CLASS.MAGE:
              return "Card_Play_SideQuest_Mage.prefab:39faefe5a4f9cf54ba9d85deb7627acb";
            case TAG_CLASS.PALADIN:
              return "Card_Play_SideQuest_Paladin.prefab:396bf10a7c7da404ea3624e009861780";
            case TAG_CLASS.ROGUE:
              return "Card_Play_SideQuest_Rogue.prefab:e805c70aa076e6743925e8d06a4be247";
          }
        }
        if (flag4 && tagClass == TAG_CLASS.DEMONHUNTER)
          return "Card_Play_Sigil_DemonHunter.prefab:b1ee048f6f0150e4ebd512208fb6a707";
        if (flag5)
          return entityBase.HasTag(GAME_TAG.OBJECTIVE_AURA) ? "Card_Play_Hero_Trigger_Aura.prefab:cf92394f0897f4443a5593d3b30be4af" : "Card_Play_Hero_Trigger.prefab:61b3b672a79aecf46a40b7d88e2e1637";
        switch (tagClass)
        {
          case TAG_CLASS.HUNTER:
            return "Card_Play_Secret_Hunter.prefab:fdf71d0657e17a7428a43c1a8f319818";
          case TAG_CLASS.MAGE:
            return "Card_Play_Secret_Mage.prefab:ffc78954f637f6f4d8b8bb7ec0b936ca";
          case TAG_CLASS.PALADIN:
            return "Card_Play_Secret_Paladin.prefab:b0f3901ff0fad674bb7c72faa7966e73";
          case TAG_CLASS.ROGUE:
            return "Card_Play_Secret_Rogue.prefab:1b224ad272f03724c9bc0aa802456c3e";
          case TAG_CLASS.WARRIOR:
            return "Card_Play_Secret_Wanderer.prefab:9eaa9bf6015f05f4e9bbe9ba5e42b20f";
          default:
            return "Card_Play_Secret_Mage.prefab:ffc78954f637f6f4d8b8bb7ec0b936ca";
        }
    }
    Debug.LogWarningFormat("ActorNames.GetZoneActor() - Can't determine actor for {0}. Returning {1} instead.", (object) cardType, (object) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9");
    return "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9";
  }

  private static bool ShouldObfuscate(Entity entity) => entity.GetController() != null && !entity.GetController().IsFriendlySide() && entity.IsObfuscated();

  public static string GetZoneActor(Entity entity, TAG_ZONE zoneTag)
  {
    if (ActorNames.ShouldObfuscate(entity) && zoneTag == TAG_ZONE.PLAY)
      return "Card_Play_Obfuscated.prefab:682f46c64054e9948875d38245cbacae";
    return entity.IsHero() && zoneTag == TAG_ZONE.GRAVEYARD ? ActorNames.GetGraveyardActorForHero(entity) : ActorNames.GetZoneActor((EntityBase) entity, zoneTag, entity.GetController(), entity.GetPremiumType());
  }

  public static string GetZoneActor(EntityDef entityDef, TAG_ZONE zoneTag) => ActorNames.GetZoneActor((EntityBase) entityDef, zoneTag, (Player) null, TAG_PREMIUM.NORMAL);

  public static string GetZoneActor(EntityDef entityDef, TAG_ZONE zoneTag, TAG_PREMIUM premium) => ActorNames.GetZoneActor((EntityBase) entityDef, zoneTag, (Player) null, premium);

  private static string GetGraveyardActorForHero(Entity entity)
  {
    Card card = entity.GetCard();
    return entity.IsHero() && (Object) card != (Object) null && card.GetPrevZone() is ZoneHero ? ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_HERO, TAG_PREMIUM.NORMAL) : ActorNames.GetZoneActor((EntityBase) entity, TAG_ZONE.GRAVEYARD, entity.GetController(), entity.GetPremiumType());
  }

  public static string GetHandActor(TAG_CARDTYPE cardType, TAG_PREMIUM premiumType)
  {
    switch (cardType)
    {
      case TAG_CARDTYPE.HERO:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_HERO, premiumType);
      case TAG_CARDTYPE.MINION:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_MINION, premiumType);
      case TAG_CARDTYPE.SPELL:
      case TAG_CARDTYPE.BATTLEGROUND_QUEST_REWARD:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_SPELL, premiumType);
      case TAG_CARDTYPE.WEAPON:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_WEAPON, premiumType);
      case TAG_CARDTYPE.HERO_POWER:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER, premiumType);
      case TAG_CARDTYPE.BATTLEGROUND_HERO_BUDDY:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.BIG_CARD_BG_HERO_BUDDY, premiumType);
      case TAG_CARDTYPE.LOCATION:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_LOCATION, premiumType);
      default:
        return "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9";
    }
  }

  public static string GetHandActorByTags(EntityBase entityBase, TAG_PREMIUM premiumType)
  {
    if (entityBase.IsLettuceMercenary())
      return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_MERCENARY, premiumType);
    if (entityBase.IsLettuceEquipment())
      return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.BIG_CARD_LETTUCE_EQUIPMENT, premiumType);
    if (!entityBase.IsLettuceAbility())
      return ActorNames.GetHandActor(entityBase.GetCardType(), premiumType);
    return entityBase.IsLettuceAbilityMinionSummoning() ? ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.BIG_CARD_LETTUCE_ABILITY_MINION, premiumType) : ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.BIG_CARD_LETTUCE_ABILITY_SPELL, premiumType);
  }

  public static string GetHandActor(TAG_CARDTYPE cardType) => ActorNames.GetHandActor(cardType, TAG_PREMIUM.NORMAL);

  public static string GetHandActor(Entity entity) => ActorNames.GetHandActorByTags((EntityBase) entity, entity.GetPremiumType());

  public static string GetHandActor(EntityDef entityDef) => ActorNames.GetHandActorByTags((EntityBase) entityDef, TAG_PREMIUM.NORMAL);

  public static string GetHandActor(EntityDef entityDef, TAG_PREMIUM premiumType) => ActorNames.GetHandActorByTags((EntityBase) entityDef, premiumType);

  public static string GetHeroSkinOrHandActor(EntityDef entityDef, TAG_PREMIUM premium) => entityDef.GetCardType() == TAG_CARDTYPE.HERO ? "Card_Hero_Skin.prefab:ed2af57fa6b571741ab047c2c3e0e663" : ActorNames.GetHandActorByTags((EntityBase) entityDef, premium);

  public static string GetHeroSkinOrHandActor(TAG_CARDTYPE type, TAG_PREMIUM premium) => type == TAG_CARDTYPE.HERO ? "Card_Hero_Skin.prefab:ed2af57fa6b571741ab047c2c3e0e663" : ActorNames.GetHandActor(type, premium);

  public static string GetPlayActorByTags(EntityBase entityBase, TAG_PREMIUM premiumType)
  {
    switch (entityBase.GetCardType())
    {
      case TAG_CARDTYPE.HERO:
        if (entityBase.HasTag(GAME_TAG.BACON_IS_KEL_THUZAD) || entityBase.HasTag(GAME_TAG.BACON_PLAYER_RESULTS_HERO_OVERRIDE))
          return "Card_Play_Bacon_Hero.prefab:227eb40f91281fa429c48c8a730c982f";
        CardHero.HeroType? heroType = GameUtils.GetHeroType(entityBase.GetCardId());
        if (heroType.HasValue)
        {
          switch (heroType.GetValueOrDefault())
          {
            case CardHero.HeroType.BATTLEGROUNDS_HERO:
              return "Card_Play_Bacon_Hero.prefab:227eb40f91281fa429c48c8a730c982f";
            case CardHero.HeroType.BATTLEGROUNDS_GUIDE:
              return "Card_Play_Bacon_Guide.prefab:6cf6c56b1ef6f4c4db7210533b95f4ac";
          }
        }
        return "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d";
      case TAG_CARDTYPE.MINION:
        return entityBase.IsLettuceMercenary() ? ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_MERCENARY, premiumType) : ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_MINION, premiumType);
      case TAG_CARDTYPE.SPELL:
        return "Card_Invisible.prefab:579b3b9a80234754593f24582f9cb93b";
      case TAG_CARDTYPE.ENCHANTMENT:
        return "Card_Play_Enchantment.prefab:cc1eafed24951ee4c92ad007507b1b69";
      case TAG_CARDTYPE.WEAPON:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_WEAPON, premiumType);
      case TAG_CARDTYPE.HERO_POWER:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_HERO_POWER, premiumType);
      case TAG_CARDTYPE.GAME_MODE_BUTTON:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_GAME_MODE_BUTTON, premiumType);
      case TAG_CARDTYPE.MOVE_MINION_HOVER_TARGET:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_MOVE_MINION_HOVER_TARGET, premiumType);
      case TAG_CARDTYPE.LETTUCE_ABILITY:
        if (entityBase.IsLettuceEquipment())
          return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_LETTUCE_EQUIPMENT, premiumType);
        return entityBase.IsLettuceAbilityMinionSummoning() ? ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_LETTUCE_ABILITY_MINION, premiumType) : ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_LETTUCE_ABILITY_SPELL, premiumType);
      case TAG_CARDTYPE.BATTLEGROUND_HERO_BUDDY:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_BATTLEGROUND_HERO_BUDDY, premiumType);
      case TAG_CARDTYPE.LOCATION:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_LOCATION, premiumType);
      case TAG_CARDTYPE.BATTLEGROUND_QUEST_REWARD:
        return ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.PLAY_BATTLEGROUND_QUEST_REWARD, premiumType);
      default:
        return "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9";
    }
  }

  public static string GetBigCardActor(Entity entity) => ActorNames.GetHistoryActor(entity, HistoryInfoType.NONE);

  public static bool ShouldDisplayTooltipInsteadOfBigCard(Entity entity) => entity.GetCardType() == TAG_CARDTYPE.GAME_MODE_BUTTON || entity.IsBobQuest();

  public static string GetHistoryActor(Entity entity, HistoryInfoType historyTileType)
  {
    if (entity.IsSecret() && entity.IsHidden())
      return ActorNames.GetHistorySecretActor(entity);
    if (ActorNames.ShouldObfuscate(entity))
      return "History_Obfuscated.prefab:d620dfa4ff929274d8805efec62fc096";
    if (string.IsNullOrEmpty(entity.GetCardId()))
      return "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9";
    TAG_CARDTYPE cardType = entity.GetCardType();
    TAG_PREMIUM premiumType = entity.GetPremiumType();
    switch (cardType)
    {
      case TAG_CARDTYPE.HERO:
        return (entity.GetZone() != TAG_ZONE.PLAY || historyTileType == HistoryInfoType.CARD_PLAYED) && entity.GetEntityDef().GetCardSet() != TAG_CARD_SET.HERO_SKINS ? ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HAND_HERO, premiumType) : "History_Hero.prefab:a040b63fa76fd4348b2a41b3bdc9789c";
      case TAG_CARDTYPE.HERO_POWER:
        return entity.GetController().IsFriendlySide() ? ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER, premiumType) : ActorNames.GetNameWithPremiumType(ActorNames.ACTOR_ASSET.HISTORY_HERO_POWER_OPPONENT, premiumType);
      default:
        return ActorNames.GetHandActor(entity);
    }
  }

  public static string GetHistorySecretActor(Entity entity)
  {
    TAG_CLASS tagClass = entity.GetClass();
    switch (tagClass)
    {
      case TAG_CLASS.HUNTER:
        return "History_Secret_Hunter.prefab:5e8dcf274b20d714abaec2a80904d83e";
      case TAG_CLASS.MAGE:
        return "History_Secret_Mage.prefab:6efbdae2809ad704ab794654d8bf2156";
      case TAG_CLASS.PALADIN:
        return "History_Secret_Paladin.prefab:158dc4838feed994db5c6d8e6cb7792b";
      case TAG_CLASS.ROGUE:
        return "History_Secret_Rogue.prefab:c827cbea9c33b7c45967ec3281c012cf";
      default:
        if (entity.IsDarkWandererSecret())
          return "History_Secret_Wanderer.prefab:7b140cf72c157604899f60f60bb37bd8";
        Debug.LogWarning((object) string.Format("ActorNames.GetHistorySecretActor() - No actor for class {0}. Returning {1} instead.", (object) tagClass, (object) "History_Secret_Mage.prefab:6efbdae2809ad704ab794654d8bf2156"));
        return "History_Secret_Mage.prefab:6efbdae2809ad704ab794654d8bf2156";
    }
  }

  public static string GetNameWithPremiumType(
    ActorNames.ACTOR_ASSET actorName,
    TAG_PREMIUM premiumType)
  {
    string nameWithPremiumType = (string) null;
    switch (premiumType)
    {
      case TAG_PREMIUM.GOLDEN:
        if (ActorNames.s_premiumActorAssets.TryGetValue(actorName, out nameWithPremiumType))
          return nameWithPremiumType;
        break;
      case TAG_PREMIUM.DIAMOND:
        if (ActorNames.s_diamondActorAssets.TryGetValue(actorName, out nameWithPremiumType))
          return nameWithPremiumType;
        goto case TAG_PREMIUM.SIGNATURE;
      case TAG_PREMIUM.SIGNATURE:
        if (ActorNames.s_signatureActorAssets.TryGetValue(actorName, out nameWithPremiumType))
          return nameWithPremiumType;
        goto case TAG_PREMIUM.GOLDEN;
    }
    return ActorNames.s_actorAssets.TryGetValue(actorName, out nameWithPremiumType) ? nameWithPremiumType : (string) null;
  }

  public enum ACTOR_ASSET
  {
    HAND_MINION,
    HAND_SPELL,
    HAND_WEAPON,
    HAND_HERO,
    HAND_MERCENARY,
    HAND_LOCATION,
    PLAY_MINION,
    PLAY_WEAPON,
    PLAY_HERO,
    PLAY_HERO_POWER,
    PLAY_GAME_MODE_BUTTON,
    PLAY_MOVE_MINION_HOVER_TARGET,
    PLAY_MERCENARY,
    PLAY_LETTUCE_ABILITY_SPELL,
    PLAY_LETTUCE_ABILITY_MINION,
    PLAY_LETTUCE_EQUIPMENT,
    PLAY_BATTLEGROUND_HERO_BUDDY,
    PLAY_LOCATION,
    PLAY_BATTLEGROUND_QUEST_REWARD,
    HISTORY_HERO_POWER,
    HISTORY_HERO_POWER_OPPONENT,
    BIG_CARD_LETTUCE_ABILITY_SPELL,
    BIG_CARD_LETTUCE_ABILITY_MINION,
    BIG_CARD_LETTUCE_EQUIPMENT,
    BIG_CARD_BG_HERO_BUDDY,
  }
}
