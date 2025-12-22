using Assets;
using Shared.Scripts.Util.ValueTypes;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GeneralStoreAdventurePane : GeneralStorePane
{
  [SerializeField]
  private Vector3 m_adventureButtonSpacing;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_adventureSelectionSound;
  private List<GeneralStoreAdventureSelectorButton> m_adventureButtons = new List<GeneralStoreAdventureSelectorButton>();
  private GeneralStoreAdventureContent m_adventureContent;
  private bool m_paneInitialized;

  [CustomEditField(Sections = "Layout")]
  public Vector3 AdventureButtonSpacing
  {
    get => this.m_adventureButtonSpacing;
    set
    {
      this.m_adventureButtonSpacing = value;
      this.UpdateAdventureButtonPositions();
    }
  }

  private void Awake()
  {
    this.m_adventureContent = this.m_parentContent as GeneralStoreAdventureContent;
    if (!((Object) this.m_adventureContent == (Object) null))
      return;
    Debug.LogError((object) "m_adventureContent is not the correct type: GeneralStoreAdventureContent");
  }

  public override void StoreShown(bool isCurrent)
  {
    if (!this.m_paneInitialized)
    {
      this.m_paneInitialized = true;
      this.SetUpAdventureButtons();
    }
    this.UpdateAdventureButtonPositions();
    this.SetupInitialSelectedAdventure();
    if (!AchieveManager.Get().HasUnlockedFeature(Achieve.Unlocks.VANILLA_HEROES))
      return;
    AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_ADVENTURE);
  }

  protected override void OnRefresh()
  {
    foreach (GeneralStoreAdventureSelectorButton adventureButton in this.m_adventureButtons)
      adventureButton.UpdateState();
  }

  private void SetUpAdventureButtons()
  {
    foreach (KeyValuePair<int, StoreAdventureDef> storeAdventureDef in this.m_adventureContent.GetStoreAdventureDefs())
    {
      AdventureDbId adventureId = (AdventureDbId) storeAdventureDef.Key;
      Network.Bundle bundle;
      StoreManager.Get().GetAvailableAdventureBundle(adventureId, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
      if (!((Record) bundle == (Record) null))
      {
        string storeButtonPrefab = storeAdventureDef.Value.m_storeButtonPrefab;
        GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) storeButtonPrefab);
        if (!((Object) gameObject == (Object) null))
        {
          GeneralStoreAdventureSelectorButton advButton = gameObject.GetComponent<GeneralStoreAdventureSelectorButton>();
          if ((Object) advButton == (Object) null)
          {
            Debug.LogError((object) string.Format("{0} does not contain GeneralStoreAdventureSelectorButton component.", (object) storeButtonPrefab));
            Object.Destroy((Object) gameObject);
          }
          else
          {
            GameUtils.SetParent((Component) advButton, this.m_paneContainer, true);
            LayerUtils.SetLayer((Component) advButton, this.m_paneContainer.layer);
            advButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnAdventureSelectorButtonClicked(advButton, adventureId)));
            advButton.SetAdventureId(adventureId);
            this.m_adventureButtons.Add(advButton);
          }
        }
      }
    }
    this.UpdateAdventureButtonPositions();
  }

  private void OnAdventureSelectorButtonClicked(
    GeneralStoreAdventureSelectorButton btn,
    AdventureDbId adventureId)
  {
    if (!this.m_parentContent.IsContentActive() || !btn.IsAvailable())
      return;
    this.m_adventureContent.SetAdventureId(adventureId);
    foreach (GeneralStoreAdventureSelectorButton adventureButton in this.m_adventureButtons)
      adventureButton.Unselect();
    btn.Select();
    Options.Get().SetInt(Option.LAST_SELECTED_STORE_ADVENTURE_ID, (int) btn.GetAdventureId());
    if (string.IsNullOrEmpty(this.m_adventureSelectionSound))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_adventureSelectionSound);
  }

  private void UpdateAdventureButtonPositions()
  {
    GeneralStoreAdventureSelectorButton[] array = this.m_adventureButtons.ToArray();
    int index = 0;
    int num = 0;
    for (; index < array.Length; ++index)
      array[index].transform.localPosition = this.m_adventureButtonSpacing * (float) num++;
  }

  private void SetupInitialSelectedAdventure()
  {
    AdventureDbId adventureId = Options.Get().GetEnum<AdventureDbId>(Option.LAST_SELECTED_STORE_ADVENTURE_ID, AdventureDbId.INVALID);
    Network.Bundle bundle = (Network.Bundle) null;
    StoreManager.Get().GetAvailableAdventureBundle(adventureId, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
    if ((Record) bundle == (Record) null)
      adventureId = AdventureDbId.INVALID;
    foreach (GeneralStoreAdventureSelectorButton adventureButton in this.m_adventureButtons)
    {
      if (adventureButton.GetAdventureId() == adventureId)
      {
        this.m_adventureContent.SetAdventureId(adventureId);
        adventureButton.Select();
      }
      else
        adventureButton.Unselect();
    }
  }
}
