using System;
using System.Collections.Generic;

public static class SpellPreloadManifest
{
  private static SpellConfiguration[] s_defaultGameplay = new SpellConfiguration[44]
  {
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ability_SpellPower_Hint.prefab:2186c8aba42f6426e90d58a224ef5f12", 7, 14),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ability_SpellPower_HintBurst.prefab:e960faeeb2cae44b7bdc33aa93adec9c", 7, 14),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ability_PowerUp.prefab:8be816f383a6c40deaa6c23ae5a561ed", 3, 10),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ally_SummonOut_Medium_PM.prefab:7106ac4b00cca44a6a998fd151491481", 3, 7),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Opponent_Attack.prefab:8c29d9367bf1247c88264a794fab8d4c", 7, 8),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Stealth.prefab:cc58b513fb7cc4942b923350e0a148f2", 2, 7),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_DamageSplat.prefab:3011e9bf869a34d05b66e084f8880b11", 7, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Death.prefab:0d12844e0f5834ae084290dc98ded3af", 7, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_SummonOut.prefab:caa660e18377d44339006998f2c1aefe", 2, 7),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Frozen.prefab:30dfeba50160a49888aa75e91fddb9e0", 7, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Friendly_Attack.prefab:ba0932f04af91400fbe0a59dea76d9be", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Opponent_Attack.prefab:8c29d9367bf1247c88264a794fab8d4c", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_SummonIn_Medium_PM.prefab:b66403dbb17e349b7a74ccac69286f92", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_SummonIn.prefab:786da6b44c25c46d39459e873d237292", 2, 10),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_BattleCry.prefab:1822746d34c264f9db24796a47d0c087", 7, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_SummonIn_Opponent.prefab:b68ecaea3f52740beac5429761699e02", 3, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Enchantment_Positive.prefab:a0c71c708090e4189a7dc8ae8c1244a6", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Enchantment_Negative.prefab:8335efea889f5498ba153192e281d1ae", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Enchantment_Neutral.prefab:6da3acecfec27488fbc69edc11ddfc1c", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Taunt_Stealth.prefab:61720198cd6951942b4ce2d281b24d4e", 3, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Trigger_Quick.prefab:cdefa967acc47ef488fe442ebf319c0e", 3, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Zzz.prefab:734e352ca20a1494e8ec18226bf49f4c", 14, 14),
    SpellPreloadManifest.PreloadConfig("Card_Hidden_SummonIn.prefab:532602057e71b4030b3a56adced8cf23", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Hidden_SummonOut.prefab:7293a69287e574d4ea07c6b8a9c189d6", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Hidden_SummonOut_Weapon.prefab:bdca7270f20ff40a8b89c2fe6a837ba7", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Hero_DamageSplat.prefab:2ecc84d6a2db7461799ad55067409307", 2, 4),
    SpellPreloadManifest.PreloadConfig("Card_Play_Hero_Opponent_Attack.prefab:b5a6efe735fce40968bf2e833f86abfc", 2, 4),
    SpellPreloadManifest.PreloadConfig("Card_Play_Hero_Friendly_Attack.prefab:12d4f45fae1f44384b4c5c70aa3baa64", 2, 4),
    SpellPreloadManifest.PreloadConfig("Card_Play_Hero_Frozen.prefab:173ef382e0e354aa0b8590d8be21f977", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_Weapon_Death.prefab:9583bd3b2ad5a4b9c90ed680d460a12e", 2, 7),
    SpellPreloadManifest.PreloadConfig("Card_Play_Weapon_Damage.prefab:f5881061a688d4095b81ca3d7a8c9cb7", 2, 3),
    SpellPreloadManifest.PreloadConfig("Card_Play_Weapon_Enchantment_Positive.prefab:409419cac9e154843a5aa827caa36b11", 2, 7),
    SpellPreloadManifest.PreloadConfig("Card_Play_Weapon_Enchantment_Negative.prefab:941cf10fc356f43e8ad3ec3bd224c2c2", 2, 7),
    SpellPreloadManifest.PreloadConfig("Card_Play_Weapon_Enchantment_Neutral.prefab:b2c0b3650165048e0816754408a39947", 2, 7),
    SpellPreloadManifest.PreloadConfig("Card_Play_Weapon_SummonIn_Friendly.prefab:b142a0cfb212145268a4ec77aeb2912f", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_Weapon_SummonIn_Opponent.prefab:eafae527ed9d24df3b7b34367d2361bc", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_Weapon_Sheathe.prefab:5d62059174796457aac31e2196e084e6", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_Weapon_Unsheathe.prefab:8fa4c811ddb5542f086dc7a64902bb83", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Taunt.prefab:2230bb169367d4c92b909c02f33a01a4", 5, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_DeathSplat.prefab:abea9449432314da1a71c67f549c71b0", 7, 14),
    SpellPreloadManifest.PreloadConfig("ReuseFX_Ally_Attack_Impact_Small_PREFAB.prefab:d507b4e4c8918fa4fb96a889f5181c00", 7, 14),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_PREFAB.prefab:6c71198e5d017044d904c9095071972f", 7, 14),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Large_PREFAB.prefab:9de87064a0d9231479ec94c43c218741", 7, 14),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Mega_PREFAB.prefab:17d6b20302c03fb4f810aefb2df1b5ef", 7, 14)
  };
  private static SpellConfiguration[] s_defaultCollectionManager = new SpellConfiguration[6]
  {
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ally_DeathReverse.prefab:d7c4b47e25fdd41e6b59c2942ee944a3", 0, 2),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ally_GhostCard.prefab:c68c7b7f4056f498583b28436f050861", 0, 2),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ability_DeathReverse.prefab:577ad3ccf627b4bb0bb0d72572d395a5", 0, 2),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ability_GhostCard.prefab:8fbe2bc8bc6bd4f708774c4b82f633d0", 0, 2),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Weapon_DeathReverse.prefab:628f5b107db4941e2a1153125167d43d", 0, 2),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Weapon_GhostCard.prefab:d43f59784a83244378656d8cd6de0672", 0, 2)
  };
  private static SpellConfiguration[] s_battlegrounds = new SpellConfiguration[18]
  {
    SpellPreloadManifest.PreloadConfig("Card_Play_BG_Buddy_Death.prefab:fdcf0bb90600e54469d03a66da5d1237", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_Hero_Buddy_Sheathe.prefab:efdceca0d8f3c75479fbe6d48f62489a", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_Hero_Buddy_Unsheathe.prefab:8d598a2b910d2bb4bba6d7308af6b9a5", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_BG_Buddy_SummonIn_Opponent.prefab:6612232626db3134fa5cb5c9aca74f37", 1, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_BG_Buddy_SummonIn_Friendly.prefab:62a144060acf7ec4a93945cc31d92721", 1, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_BG_Buddy_Single.prefab:ea99c4edf3cfa054a9b625263eda1db9", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_BG_Buddy_Double.prefab:a5c7d10349b3c50468e95da1e941c96f", 2, 2),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_BaconShopCoin.prefab:86026e8c80800724cb94ba0a3c731b80", 7, 14),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ally_TechLevelManaGem.prefab:fafee4fdb205ce84984f1c7f2147cdab", 2, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Spawn_Minion.prefab:fd6035c3a26bd4d69817ff4f21436fe9", 7, 14),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Spawn_Minion_Opponent.prefab:ff1a0c7b2b7534ea883711094aa170e5", 7, 14),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ability_HealingDoesDamage_HintBurst.prefab:499e60ba4c2b7d44f93dc8ca059aca2a", 4, 7),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ability_HealingDoesDamage_Hint.prefab:b3c4902dbb1799840b95299e293b34dd", 4, 7),
    SpellPreloadManifest.PreloadConfig("Card_Play_GameModeButton_CoinManaGem.prefab:7a47d9b4337ceed4283916a2e3ae3bea", 3, 7),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Battlegrounds_Large_PREFAB.prefab:dea2b75d99e630e47ab4b0d0bba38a41", 7, 14),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Battlegrounds_Mega_PREFAB.prefab:43a5c5d7f0793154ba5aa84181da34ed", 7, 14),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Battlegrounds_PREFAB.prefab:c5edc82f4cab6e94e8f222cb0644687a", 7, 14),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Battlegrounds_Small_PREFAB.prefab:d46308838cab6d44da610c9e92bcf24e", 7, 14)
  };
  private static SpellConfiguration[] s_mercenaries = new SpellConfiguration[13]
  {
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ally_SummonIn_Mercenary.prefab:e37ea6a441aff134682f92688f0b8a06", 6, 12),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_MercenariesTransition_Down.prefab:7f5fd54d47de72f44a93b21917654ade", 6, 12),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_MercenariesTransition_Up.prefab:8e88900d7fbd7d74c95ef2d38c31f13d", 6, 12),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Lettuce_Speech_Bubble.prefab:e04b1268540961d44b573a1c7204e51e", 6, 12),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ally_SummonOut_Mercenary.prefab:5a538a4e9282a27469993ffcd9e53796", 6, 12),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Mercenary_CombatBoosh.prefab:e3335696ab1d97b438c242bf9a4b7e2f", 6, 12),
    SpellPreloadManifest.PreloadConfig("Card_Play_Ally_Friendly_Spell_With_Highlight.prefab:39f6314771fe279429b7fedca46a3386", 6, 12),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ability_HealingDoesDamage_HintBurst.prefab:499e60ba4c2b7d44f93dc8ca059aca2a", 3, 12),
    SpellPreloadManifest.PreloadConfig("Card_Hand_Ability_HealingDoesDamage_Hint.prefab:b3c4902dbb1799840b95299e293b34dd", 3, 12),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Mercenaries_Critical_Hit.prefab:a5d9ceeaabd114247acb6c4cbe231a7f", 3, 12),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Mercenaries_Critical_Hit_Large.prefab:bce2cc5bb87c78c4cb75ed1e8f0fda15", 3, 12),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Mercenaries_Critical_Hit_Mega.prefab:14decebd6a519d143bfebb51e17333bc", 3, 12),
    SpellPreloadManifest.PreloadConfig("FX_Ally_Attack_Impact_Mercenaries_Critical_Hit_Small.prefab:84550a8ea683c5444acdd9a49664a335", 3, 12)
  };
  public static Dictionary<SpellPreloadMode, SpellPreloadConfiguration> PreloadManifest = new Dictionary<SpellPreloadMode, SpellPreloadConfiguration>()
  {
    {
      SpellPreloadMode.DEFAULT_GAMEPLAY,
      new SpellPreloadConfiguration(SpellPreloadManifest.s_defaultGameplay, Array.Empty<SpellPreloadMode>())
    },
    {
      SpellPreloadMode.DEFAULT_COLLECTION,
      new SpellPreloadConfiguration(SpellPreloadManifest.s_defaultCollectionManager, Array.Empty<SpellPreloadMode>())
    },
    {
      SpellPreloadMode.BATTLEGROUNDS,
      new SpellPreloadConfiguration(SpellPreloadManifest.s_battlegrounds, new SpellPreloadMode[1]
      {
        SpellPreloadMode.DEFAULT_GAMEPLAY
      })
    },
    {
      SpellPreloadMode.MERCENARIES,
      new SpellPreloadConfiguration(SpellPreloadManifest.s_mercenaries, new SpellPreloadMode[1]
      {
        SpellPreloadMode.DEFAULT_GAMEPLAY
      })
    }
  };

  private static SpellConfiguration PreloadConfig(
    string name,
    int prepopulateCount,
    int maxSize)
  {
    return new SpellConfiguration()
    {
      SpellAssetRef = name,
      PoolPrepopulateCount = prepopulateCount,
      MaxPoolSize = maxSize
    };
  }
}
