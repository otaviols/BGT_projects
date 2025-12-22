using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CustomEditClass]
public class CheatMenu : MonoBehaviour
{
  [CustomEditField(Sections = "TabGroups")]
  public List<GameObject> groups = new List<GameObject>();
  private int ActiveTabGroupIndex;
  [CustomEditField(Sections = "Arrows")]
  public GameObject LeftArrow;
  [CustomEditField(Sections = "Arrows")]
  public GameObject RightArrow;
  [CustomEditField(Sections = "Tabs")]
  public List<GameObject> tabs = new List<GameObject>();
  [CustomEditField(Sections = "Tabs")]
  public List<GameObject> contents = new List<GameObject>();
  private int ActiveTabIndex;
  private GameObject ActiveTabContents;
  [CustomEditField(Sections = "Tab_00_Contents")]
  public GameObject m_maxManaButton;
  [CustomEditField(Sections = "Tab_00_Contents")]
  public GameObject m_fullHealthButton;
  [CustomEditField(Sections = "Tab_00_Contents")]
  public GameObject m_SetHealthToOneButton;
  [CustomEditField(Sections = "Tab_00_Contents")]
  public GameObject m_ImmuneCheckMark;
  [CustomEditField(Sections = "Tab_00_Contents")]
  public GameObject m_ClearMinionsButton;
  [CustomEditField(Sections = "Tab_00_Contents")]
  public GameObject m_ClearHandButton;
  [CustomEditField(Sections = "Tab_00_Contents")]
  public GameObject m_destroyButton;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject SearchTab;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject PinnedTab;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject SearchTabContents;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject PinnedTabContents;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject SearchInputField;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject PinnedInputField;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject exportCardButton;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject m_GoldenCheckMark;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject m_PinItCheckMark;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject m_SearchResultItem;
  [CustomEditField(Sections = "Tab_01_Contents")]
  public GameObject m_PreviewCard;
  private TAG_PREMIUM m_premiumType;
  [CustomEditField(Sections = "Tab_02_Contents")]
  public GameObject m_runConsoleButton;
  [CustomEditField(Sections = "Tab_02_Contents")]
  public InputField m_scriptContent;
  private int tutorialProgress;
  private int DustInput;
  private int GoldInput;
  private int TicketsInput;
  [CustomEditField(Sections = "Tab_04_General")]
  public GameObject m_HUDcheckMark;
  private bool isHUDactive = true;
  [CustomEditField(Sections = "Tab_04_General")]
  public GameObject m_HideHistorycheckMark;
  private bool isHistoryactive = true;
  [CustomEditField(Sections = "Tab_04_General")]
  public GameObject m_SetboardInputField;
  private Dictionary<string, CardDbfRecord> m_allCardRecords;
  private string m_selectedCard;
  private GameObject m_cardPreview;

  private void Start()
  {
    if (this.ActiveTabGroupIndex > 0)
      this.LeftArrow.SetActive(true);
    else
      this.LeftArrow.SetActive(false);
    if (this.ActiveTabGroupIndex < this.groups.Count - 1)
      this.RightArrow.SetActive(true);
    else
      this.RightArrow.SetActive(false);
    this.ActiveTabContents = this.contents[this.ActiveTabIndex];
    for (int index = 0; index < this.contents.Count; ++index)
    {
      if (index == this.ActiveTabIndex)
      {
        ColorBlock colors = this.tabs[this.ActiveTabIndex].GetComponentInChildren<Button>().colors with
        {
          normalColor = Color.white
        };
        this.tabs[this.ActiveTabIndex].GetComponentInChildren<Button>().colors = colors;
        this.contents[this.ActiveTabIndex].SetActive(true);
      }
      else
      {
        ColorBlock colors = this.tabs[this.ActiveTabIndex].GetComponentInChildren<Button>().colors with
        {
          normalColor = Color.clear
        };
        this.tabs[this.ActiveTabIndex].GetComponentInChildren<Button>().colors = colors;
        this.contents[index].SetActive(false);
      }
    }
    this.ActiveTabContents.SetActive(true);
  }

  private void OnEnable()
  {
    Debug.Log((object) "Enabled");
    this.m_allCardRecords = new Dictionary<string, CardDbfRecord>();
    foreach (string allCardId in GameUtils.GetAllCardIds())
      this.m_allCardRecords[allCardId] = GameUtils.GetCardRecord(allCardId);
  }

  public void SetAsActiveTab(int tabIndex)
  {
    Debug.Log((object) ("Tab Index " + (object) tabIndex));
    ColorBlock colors1 = this.tabs[this.ActiveTabIndex].GetComponentInChildren<Button>().colors with
    {
      normalColor = Color.clear
    };
    this.tabs[this.ActiveTabIndex].GetComponentInChildren<Button>().colors = colors1;
    if ((Object) this.ActiveTabContents != (Object) null)
      this.ActiveTabContents.SetActive(false);
    this.ActiveTabIndex = tabIndex;
    this.ActiveTabContents = this.contents[this.ActiveTabIndex];
    this.ActiveTabContents.SetActive(true);
    ColorBlock colors2 = this.tabs[this.ActiveTabIndex].GetComponentInChildren<Button>().colors with
    {
      normalColor = Color.white
    };
    this.tabs[this.ActiveTabIndex].GetComponentInChildren<Button>().colors = colors2;
  }

  public void ShiftGroup(int indexChange)
  {
    this.groups[this.ActiveTabGroupIndex].SetActive(false);
    this.ActiveTabGroupIndex += indexChange;
    this.groups[this.ActiveTabGroupIndex].SetActive(true);
    if (this.ActiveTabGroupIndex > 0)
      this.LeftArrow.SetActive(true);
    else
      this.LeftArrow.SetActive(false);
    if (this.ActiveTabGroupIndex < this.groups.Count - 1)
      this.RightArrow.SetActive(true);
    else
      this.RightArrow.SetActive(false);
  }

  public void MaxMana()
  {
    if (!Network.IsRunning())
      return;
    string command = "maxmana friendly";
    Network.Get().SendDebugConsoleCommand(command);
  }

  public void FullHealth()
  {
    if (!Network.IsRunning())
      return;
    string command = "healhero friendly";
    Network.Get().SendDebugConsoleCommand(command);
  }

  public void SetHealthToOne()
  {
    if (!Network.IsRunning())
      return;
    string command = "spawncard XXX_107 friendly hand 0";
    Network.Get().SendDebugConsoleCommand(command);
  }

  public void SetImmune() => Debug.Log((object) "Cheat: SetImmune function called");

  public void ClearMinions()
  {
    if (!Network.IsRunning())
      return;
    string command = "spawncard XXX_018 friendly hand 0";
    Network.Get().SendDebugConsoleCommand(command);
  }

  public void Discard()
  {
    if (!Network.IsRunning())
      return;
    string command = "cyclehand friendly";
    Network.Get().SendDebugConsoleCommand(command);
  }

  public void DrawCard()
  {
    if (!Network.IsRunning())
      return;
    string command = "drawcard friendly";
    Network.Get().SendDebugConsoleCommand(command);
  }

  public void Destroy() => Debug.Log((object) "Cheat: Destroy function called");

  public void SearchOnValueChanged() => Debug.Log((object) ("Search keyword changed to: " + this.SearchInputField.GetComponent<InputField>().text));

  public void SearchOnEndEdit()
  {
    if (this.m_allCardRecords.Count == 0)
    {
      DefLoader.Get().Clear();
      Localization.SetLocale(Locale.enUS);
      GameDbf.Load();
      GameStrings.ReloadAll();
      foreach (string allCardId in GameUtils.GetAllCardIds())
        this.m_allCardRecords[allCardId] = GameUtils.GetCardRecord(allCardId);
      DefLoader.Get().LoadAllEntityDefs();
    }
    string lower = this.SearchInputField.GetComponent<InputField>().text.ToLower();
    Debug.Log((object) ("User pressed 'enter'. Keyword: " + lower));
    Transform transform = this.SearchTabContents.transform.Find("Search Results List").transform.Find("Search Result Items").transform;
    for (int index = 0; index < transform.childCount; ++index)
      Object.Destroy((Object) transform.GetChild(index).gameObject);
    if (string.IsNullOrEmpty(lower) || this.m_allCardRecords.Count <= 0)
      return;
    Vector3 vector3 = new Vector3(0.0f, 0.0f, -73f);
    Vector3 one = Vector3.one;
    foreach (KeyValuePair<string, CardDbfRecord> allCardRecord in this.m_allCardRecords)
    {
      if ((allCardRecord.Key + allCardRecord.Value.Name.GetString(Locale.enUS).ToLower()).Contains(lower))
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.m_SearchResultItem);
        SearchResultItem result = gameObject.GetComponent<SearchResultItem>();
        result.m_text = allCardRecord.Value.Name.GetString(Locale.enUS);
        result.m_card = allCardRecord.Key;
        gameObject.name = "Item";
        gameObject.transform.SetParent(transform);
        gameObject.transform.localPosition = vector3;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localScale = one;
        gameObject.GetComponent<Button>().onClick.AddListener((UnityAction) (() => this.CardSelectedHandler(result)));
      }
    }
  }

  public void CardSelectedHandler(SearchResultItem item)
  {
    Debug.Log((object) item.m_text);
    this.m_selectedCard = item.m_card;
    this.PreviewCard();
  }

  private void PreviewCard()
  {
    if ((Object) this.m_cardPreview != (Object) null)
      Object.Destroy((Object) this.m_cardPreview);
    this.m_cardPreview = this.LoadCard(this.m_selectedCard, this.m_premiumType);
  }

  private GameObject LoadCard(string cardID, TAG_PREMIUM premium)
  {
    using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardID, new CardPortraitQuality(3, premium)))
    {
      string handActor = ActorNames.GetHandActor(fullDef.EntityDef, premium);
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) handActor, AssetLoadingOptions.IgnorePrefabPosition);
      Actor component = gameObject.GetComponent<Actor>();
      if ((Object) component == (Object) null)
      {
        Debug.LogWarning((object) string.Format("Error getting Actor for: {0}", (object) cardID));
        return (GameObject) null;
      }
      this.m_PreviewCard.SetActive(false);
      component.SetPremium(premium);
      component.SetEntityDef(fullDef.EntityDef);
      component.SetCardDef(fullDef.DisposableCardDef);
      component.UpdateAllComponents();
      component.SetUnlit();
      gameObject.transform.SetParent(this.contents[1].transform, false);
      gameObject.transform.localPosition = this.m_PreviewCard.transform.localPosition;
      gameObject.transform.localRotation = Quaternion.identity;
      gameObject.transform.localScale = this.m_PreviewCard.transform.localScale;
      foreach (Component componentsInChild in gameObject.GetComponentsInChildren<Transform>())
        componentsInChild.gameObject.layer = LayerMask.NameToLayer("UI");
      gameObject.layer = LayerMask.NameToLayer("UI");
      return gameObject;
    }
  }

  public void PinnedOnValueChanged() => Debug.Log((object) ("Pinned keyword changed to: " + this.PinnedInputField.GetComponent<InputField>().text));

  public void PinnedOnEndEdit() => Debug.Log((object) ("User pressed 'enter'. Keyword: " + this.PinnedInputField.GetComponent<InputField>().text));

  public void ShowSearchTab()
  {
    Debug.Log((object) "Showing Search");
    this.SearchTabContents.SetActive(true);
    this.PinnedTabContents.SetActive(false);
    RectTransform component1 = this.SearchTab.GetComponent<RectTransform>();
    Vector3 localPosition1 = component1.localPosition;
    component1.localPosition = new Vector3(localPosition1.x, localPosition1.y, 0.109f);
    RectTransform component2 = this.PinnedTab.GetComponent<RectTransform>();
    Vector3 localPosition2 = component2.localPosition;
    component2.localPosition = new Vector3(localPosition2.x, localPosition2.y, 0.095f);
  }

  public void ShowPinnedTab()
  {
    Debug.Log((object) "Showing Pinned Items");
    this.SearchTabContents.SetActive(false);
    this.PinnedTabContents.SetActive(true);
    RectTransform component1 = this.SearchTab.GetComponent<RectTransform>();
    Vector3 localPosition1 = component1.localPosition;
    component1.localPosition = new Vector3(localPosition1.x, localPosition1.y, 0.095f);
    RectTransform component2 = this.PinnedTab.GetComponent<RectTransform>();
    Vector3 localPosition2 = component2.localPosition;
    component2.localPosition = new Vector3(localPosition2.x, localPosition2.y, 0.109f);
  }

  public void PreviewCard(GameObject textObj) => Debug.Log((object) ("Search Result Click. Previewing: " + textObj.GetComponent<Text>().text));

  public void ToggleGolden()
  {
    Debug.Log((object) "Cheat: ToggleGolden function called");
    this.m_GoldenCheckMark.SetActive(!this.m_GoldenCheckMark.activeSelf);
    this.m_premiumType = this.m_premiumType == TAG_PREMIUM.GOLDEN || this.m_premiumType == TAG_PREMIUM.DIAMOND ? TAG_PREMIUM.NORMAL : TAG_PREMIUM.GOLDEN;
    this.PreviewCard();
  }

  public void ExportCard() => Debug.Log((object) "Export Card function called");

  public void AddCardTo(string location)
  {
    if (!(location == "opponentHand"))
    {
      if (!(location == "opponentField"))
      {
        if (!(location == "opponentDeck"))
        {
          if (!(location == "yourField"))
          {
            if (!(location == "yourHand"))
            {
              if (!(location == "yourDeck"))
                return;
              Debug.Log((object) "AddCardTo function called. Adding card to Your Deck");
              if (!Network.IsRunning())
                return;
              string command = string.Format("spawncard {0} friendly deck 0", (object) this.m_selectedCard);
              Network.Get().SendDebugConsoleCommand(command);
            }
            else
            {
              Debug.Log((object) "AddCardTo function called. Adding card to Your Hand");
              if (!Network.IsRunning())
                return;
              string command = string.Format("spawncard {0} friendly hand 0", (object) this.m_selectedCard);
              Network.Get().SendDebugConsoleCommand(command);
            }
          }
          else
          {
            Debug.Log((object) "AddCardTo function called. Adding card to Your Field");
            if (!Network.IsRunning())
              return;
            string command = string.Format("spawncard {0} friendly play 0", (object) this.m_selectedCard);
            Network.Get().SendDebugConsoleCommand(command);
          }
        }
        else
        {
          Debug.Log((object) "AddCardTo function called. Adding card to Opponent's Deck");
          if (!Network.IsRunning())
            return;
          string command = string.Format("spawncard {0} opponent deck 0", (object) this.m_selectedCard);
          Network.Get().SendDebugConsoleCommand(command);
        }
      }
      else
      {
        Debug.Log((object) "AddCardTo function called. Adding card to Opponent's Field");
        if (!Network.IsRunning())
          return;
        string command = string.Format("spawncard {0} opponent play 0", (object) this.m_selectedCard);
        Network.Get().SendDebugConsoleCommand(command);
      }
    }
    else
    {
      Debug.Log((object) "AddCardTo function called. Adding card to Opponent's Hand");
      if (!Network.IsRunning())
        return;
      string command = string.Format("spawncard {0} opponent hand 0", (object) this.m_selectedCard);
      Network.Get().SendDebugConsoleCommand(command);
    }
  }

  public void RunConsole() => Debug.Log((object) "Cheat: RunConsole function called");

  public void ClearConsole() => this.m_scriptContent.text = "";

  public void DustValueInput(InputField input)
  {
    this.DustInput = int.Parse(input.text);
    Debug.Log((object) ("Arcane Dust input field changed to: " + (object) this.DustInput));
  }

  public void GoldValueInput(InputField input)
  {
    this.GoldInput = int.Parse(input.text);
    Debug.Log((object) ("Gold input field changed to: " + (object) this.GoldInput));
  }

  public void TicketValueInput(InputField input)
  {
    this.TicketsInput = int.Parse(input.text);
    Debug.Log((object) ("Tickets input field changed to: " + (object) this.TicketsInput));
  }

  public void TutorialDropdownValueChanged(int value)
  {
    this.tutorialProgress = value;
    Debug.Log((object) ("Tut: " + (object) this.tutorialProgress));
  }

  public void SetTutorialProgress()
  {
    switch (this.tutorialProgress)
    {
      case 0:
        Debug.Log((object) ("Tutorial Progress set to: " + (object) this.tutorialProgress + " : Hogger"));
        break;
      case 1:
        Debug.Log((object) ("Tutorial Progress set to: " + (object) this.tutorialProgress + " : Manastorm"));
        break;
      case 2:
        Debug.Log((object) ("Tutorial Progress set to: " + (object) this.tutorialProgress + " : Lorewalker"));
        break;
      case 3:
        Debug.Log((object) ("Tutorial Progress set to: " + (object) this.tutorialProgress + " : King Mukla"));
        break;
      case 4:
        Debug.Log((object) ("Tutorial Progress set to: " + (object) this.tutorialProgress + " : Nesingwary"));
        break;
      case 5:
        Debug.Log((object) ("Tutorial Progress set to: " + (object) this.tutorialProgress + " : Stormrage"));
        break;
      case 6:
        Debug.Log((object) ("Tutorial Progress set to: " + (object) this.tutorialProgress + " : Tutorial Complete"));
        break;
    }
  }

  public void SetArcaneDust() => Debug.Log((object) ("Cheat: SetArcaneDust function called to add " + (object) this.DustInput + " Arcane Dust to account"));

  public void SetGoldBalance() => Debug.Log((object) ("Cheat: SetGoldBalance function called to add " + (object) this.GoldInput + " Gold to account"));

  public void OpenArena() => Debug.Log((object) "Cheat: OpenArena function called");

  public void SetTickets() => Debug.Log((object) ("Cheat: SetTickets function called to add " + (object) this.TicketsInput + " Tickets to account"));

  public void BuyAllAdventures() => Debug.Log((object) "Cheat: BuyAllAdventures function called");

  public void DefeatAllAdventures() => Debug.Log((object) "Cheat: DefeatAllAdventures function called");

  public void MaxLevelAllHeroes() => Debug.Log((object) "Cheat: MaxLevelAllHeroes function called");

  public void CloneAccount() => Debug.Log((object) "Cheat: CloneAccount function called");

  public void ResetAccount() => Debug.Log((object) "Cheat: ResetAccount function called");

  public void GiveMeEverything() => Debug.Log((object) "Cheat: GiveMeEverything function called");

  public void ToggleHUD()
  {
    Debug.Log((object) "Cheat: ToggleHUD function called");
    this.m_HUDcheckMark.SetActive(!this.m_HUDcheckMark.activeSelf);
    this.isHUDactive = !this.isHUDactive;
  }

  public void ToggleHideHistory()
  {
    Debug.Log((object) "Cheat: ToggleHideHistory function called");
    this.m_HideHistorycheckMark.SetActive(!this.m_HideHistorycheckMark.activeSelf);
    this.isHistoryactive = !this.isHistoryactive;
  }

  public void RenameInnkeeper(Text name) => Debug.Log((object) ("Cheat: RenameInnkeeper function called. Renaming to: " + name.GetComponent<Text>().text));

  public void ResetClient() => Debug.Log((object) "Cheat: ResetClient function called");

  public void ExportCardsTool() => Debug.Log((object) "Cheat: ExportCardsTool function called");

  public void BoardOnValueChanged() => Debug.Log((object) ("Pinned keyword changed to: " + this.m_SetboardInputField.GetComponent<InputField>().text));

  public void BoardOnEndEdit() => Debug.Log((object) ("User pressed 'enter'. Keyword: " + this.m_SetboardInputField.GetComponent<InputField>().text));
}
