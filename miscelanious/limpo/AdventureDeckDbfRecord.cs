using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdventureDeckDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_adventureId;
  [SerializeField]
  private int m_classId;
  [SerializeField]
  private int m_deckId;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private DbfLocValue m_unlockCriteriaText;
  [SerializeField]
  private DbfLocValue m_unlockedDescriptionText;
  [SerializeField]
  private int m_unlockGameSaveSubkeyId;
  [SerializeField]
  private int m_unlockValue;
  [SerializeField]
  private string m_displayTexture;

  [DbfField("ADVENTURE_ID")]
  public int AdventureId => this.m_adventureId;

  [DbfField("CLASS_ID")]
  public int ClassId => this.m_classId;

  [DbfField("DECK_ID")]
  public int DeckId => this.m_deckId;

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

  [DbfField("DISPLAY_TEXTURE")]
  public string DisplayTexture => this.m_displayTexture;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return (object) this.m_adventureId;
      case "CLASS_ID":
        return (object) this.m_classId;
      case "DECK_ID":
        return (object) this.m_deckId;
      case "DISPLAY_TEXTURE":
        return (object) this.m_displayTexture;
      case "ID":
        return (object) this.ID;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "UNLOCKED_DESCRIPTION_TEXT":
        return (object) this.m_unlockedDescriptionText;
      case "UNLOCK_CRITERIA_TEXT":
        return (object) this.m_unlockCriteriaText;
      case "UNLOCK_GAME_SAVE_SUBKEY":
        return (object) this.m_unlockGameSaveSubkeyId;
      case "UNLOCK_VALUE":
        return (object) this.m_unlockValue;
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
      case 771121008:
        if (!(name == "DECK_ID"))
          break;
        this.m_deckId = (int) val;
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
      case 2452245441:
        if (!(name == "DISPLAY_TEXTURE"))
          break;
        this.m_displayTexture = (string) val;
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
      case "CLASS_ID":
        return typeof (int);
      case "DECK_ID":
        return typeof (int);
      case "DISPLAY_TEXTURE":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "SORT_ORDER":
        return typeof (int);
      case "UNLOCKED_DESCRIPTION_TEXT":
        return typeof (DbfLocValue);
      case "UNLOCK_CRITERIA_TEXT":
        return typeof (DbfLocValue);
      case "UNLOCK_GAME_SAVE_SUBKEY":
        return typeof (int);
      case "UNLOCK_VALUE":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAdventureDeckDbfRecords loadRecords = new LoadAdventureDeckDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AdventureDeckDbfAsset adventureDeckDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AdventureDeckDbfAsset)) as AdventureDeckDbfAsset;
    if ((UnityEngine.Object) adventureDeckDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AdventureDeckDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < adventureDeckDbfAsset.Records.Count; ++index)
      adventureDeckDbfAsset.Records[index].StripUnusedLocales();
    records = adventureDeckDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_unlockCriteriaText.StripUnusedLocales();
    this.m_unlockedDescriptionText.StripUnusedLocales();
  }
}
