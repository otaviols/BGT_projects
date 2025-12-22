using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdventureLoadoutTreasuresDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_adventureId;
  [SerializeField]
  private int m_classId;
  [SerializeField]
  private int m_guestHeroId;
  [SerializeField]
  private int m_guestHeroVariantId;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private bool m_isDefault;
  [SerializeField]
  private DbfLocValue m_unlockCriteriaText;
  [SerializeField]
  private DbfLocValue m_unlockedDescriptionText;
  [SerializeField]
  private int m_unlockGameSaveSubkeyId;
  [SerializeField]
  private int m_unlockValue;
  [SerializeField]
  private int m_unlockAchievementId;
  [SerializeField]
  private int m_upgradedCardId;
  [SerializeField]
  private DbfLocValue m_upgradedDescriptionText;
  [SerializeField]
  private int m_upgradeGameSaveSubkeyId;
  [SerializeField]
  private int m_upgradeValue;

  [DbfField("ADVENTURE_ID")]
  public int AdventureId => this.m_adventureId;

  [DbfField("CLASS_ID")]
  public int ClassId => this.m_classId;

  [DbfField("GUEST_HERO_ID")]
  public int GuestHeroId => this.m_guestHeroId;

  [DbfField("GUEST_HERO_VARIANT_ID")]
  public int GuestHeroVariantId => this.m_guestHeroVariantId;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("UNLOCK_CRITERIA_TEXT")]
  public DbfLocValue UnlockCriteriaText => this.m_unlockCriteriaText;

  [DbfField("UNLOCKED_DESCRIPTION_TEXT")]
  public DbfLocValue UnlockedDescriptionText => this.m_unlockedDescriptionText;

  [DbfField("UNLOCK_GAME_SAVE_SUBKEY")]
  public int UnlockGameSaveSubkey => this.m_unlockGameSaveSubkeyId;

  [DbfField("UNLOCK_VALUE")]
  public int UnlockValue => this.m_unlockValue;

  [DbfField("UNLOCK_ACHIEVEMENT")]
  public int UnlockAchievement => this.m_unlockAchievementId;

  [DbfField("UPGRADED_CARD_ID")]
  public int UpgradedCardId => this.m_upgradedCardId;

  [DbfField("UPGRADED_DESCRIPTION_TEXT")]
  public DbfLocValue UpgradedDescriptionText => this.m_upgradedDescriptionText;

  [DbfField("UPGRADE_GAME_SAVE_SUBKEY")]
  public int UpgradeGameSaveSubkey => this.m_upgradeGameSaveSubkeyId;

  [DbfField("UPGRADE_VALUE")]
  public int UpgradeValue => this.m_upgradeValue;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return (object) this.m_adventureId;
      case "CARD_ID":
        return (object) this.m_cardId;
      case "CLASS_ID":
        return (object) this.m_classId;
      case "GUEST_HERO_ID":
        return (object) this.m_guestHeroId;
      case "GUEST_HERO_VARIANT_ID":
        return (object) this.m_guestHeroVariantId;
      case "ID":
        return (object) this.ID;
      case "IS_DEFAULT":
        return (object) this.m_isDefault;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "UNLOCKED_DESCRIPTION_TEXT":
        return (object) this.m_unlockedDescriptionText;
      case "UNLOCK_ACHIEVEMENT":
        return (object) this.m_unlockAchievementId;
      case "UNLOCK_CRITERIA_TEXT":
        return (object) this.m_unlockCriteriaText;
      case "UNLOCK_GAME_SAVE_SUBKEY":
        return (object) this.m_unlockGameSaveSubkeyId;
      case "UNLOCK_VALUE":
        return (object) this.m_unlockValue;
      case "UPGRADED_CARD_ID":
        return (object) this.m_upgradedCardId;
      case "UPGRADED_DESCRIPTION_TEXT":
        return (object) this.m_upgradedDescriptionText;
      case "UPGRADE_GAME_SAVE_SUBKEY":
        return (object) this.m_upgradeGameSaveSubkeyId;
      case "UPGRADE_VALUE":
        return (object) this.m_upgradeValue;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 190718801:
        if (!(name == "ADVENTURE_ID"))
          break;
        this.m_adventureId = (int) val;
        break;
      case 451390141:
        if (!(name == "CARD_ID"))
          break;
        this.m_cardId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1608019912:
        if (!(name == "UNLOCK_GAME_SAVE_SUBKEY"))
          break;
        this.m_unlockGameSaveSubkeyId = (int) val;
        break;
      case 1674937439:
        if (!(name == "UNLOCK_VALUE"))
          break;
        this.m_unlockValue = (int) val;
        break;
      case 1785070112:
        if (!(name == "UPGRADED_CARD_ID"))
          break;
        this.m_upgradedCardId = (int) val;
        break;
      case 1925855082:
        if (!(name == "GUEST_HERO_VARIANT_ID"))
          break;
        this.m_guestHeroVariantId = (int) val;
        break;
      case 1966695012:
        if (!(name == "GUEST_HERO_ID"))
          break;
        this.m_guestHeroId = (int) val;
        break;
      case 2364829034:
        if (!(name == "UPGRADE_GAME_SAVE_SUBKEY"))
          break;
        this.m_upgradeGameSaveSubkeyId = (int) val;
        break;
      case 2401654691:
        if (!(name == "IS_DEFAULT"))
          break;
        this.m_isDefault = (bool) val;
        break;
      case 2477374857:
        if (!(name == "UPGRADE_VALUE"))
          break;
        this.m_upgradeValue = (int) val;
        break;
      case 2795899714:
        if (!(name == "UPGRADED_DESCRIPTION_TEXT"))
          break;
        this.m_upgradedDescriptionText = (DbfLocValue) val;
        break;
      case 3034864917:
        if (!(name == "UNLOCK_ACHIEVEMENT"))
          break;
        this.m_unlockAchievementId = (int) val;
        break;
      case 3710150967:
        if (!(name == "UNLOCK_CRITERIA_TEXT"))
          break;
        this.m_unlockCriteriaText = (DbfLocValue) val;
        break;
      case 4070522309:
        if (!(name == "UNLOCKED_DESCRIPTION_TEXT"))
          break;
        this.m_unlockedDescriptionText = (DbfLocValue) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
      case 4257872637:
        if (!(name == "CLASS_ID"))
          break;
        this.m_classId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return typeof (int);
      case "CARD_ID":
        return typeof (int);
      case "CLASS_ID":
        return typeof (int);
      case "GUEST_HERO_ID":
        return typeof (int);
      case "GUEST_HERO_VARIANT_ID":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "IS_DEFAULT":
        return typeof (bool);
      case "SORT_ORDER":
        return typeof (int);
      case "UNLOCKED_DESCRIPTION_TEXT":
        return typeof (DbfLocValue);
      case "UNLOCK_ACHIEVEMENT":
        return typeof (int);
      case "UNLOCK_CRITERIA_TEXT":
        return typeof (DbfLocValue);
      case "UNLOCK_GAME_SAVE_SUBKEY":
        return typeof (int);
      case "UNLOCK_VALUE":
        return typeof (int);
      case "UPGRADED_CARD_ID":
        return typeof (int);
      case "UPGRADED_DESCRIPTION_TEXT":
        return typeof (DbfLocValue);
      case "UPGRADE_GAME_SAVE_SUBKEY":
        return typeof (int);
      case "UPGRADE_VALUE":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAdventureLoadoutTreasuresDbfRecords loadRecords = new LoadAdventureLoadoutTreasuresDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AdventureLoadoutTreasuresDbfAsset treasuresDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AdventureLoadoutTreasuresDbfAsset)) as AdventureLoadoutTreasuresDbfAsset;
    if ((UnityEngine.Object) treasuresDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AdventureLoadoutTreasuresDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < treasuresDbfAsset.Records.Count; ++index)
      treasuresDbfAsset.Records[index].StripUnusedLocales();
    records = treasuresDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_unlockCriteriaText.StripUnusedLocales();
    this.m_unlockedDescriptionText.StripUnusedLocales();
    this.m_upgradedDescriptionText.StripUnusedLocales();
  }
}
