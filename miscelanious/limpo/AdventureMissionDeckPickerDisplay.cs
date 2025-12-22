using UnityEngine;

[CustomEditClass]
public class AdventureMissionDeckPickerDisplay : MonoBehaviour
{
  public GameObject m_deckPickerTrayContainer;
  private DeckPickerTrayDisplay m_deckPickerTray;

  private void Awake()
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) ((bool) UniversalInputManager.UsePhoneUI ? "DeckPickerTray_phone.prefab:a30124f640b5b92459bf820a4e3b1ca7" : "DeckPickerTray.prefab:3e13b59cdca14074bbce2b7d903ed895"), AssetLoadingOptions.IgnorePrefabPosition);
    if ((Object) gameObject == (Object) null)
    {
      Debug.LogError((object) "Unable to load DeckPickerTray.");
    }
    else
    {
      this.m_deckPickerTray = gameObject.GetComponent<DeckPickerTrayDisplay>();
      if ((Object) this.m_deckPickerTray == (Object) null)
      {
        Debug.LogError((object) "DeckPickerTrayDisplay component not found in DeckPickerTray object.");
      }
      else
      {
        if ((Object) this.m_deckPickerTrayContainer != (Object) null)
          GameUtils.SetParent((Component) this.m_deckPickerTray, this.m_deckPickerTrayContainer);
        this.m_deckPickerTray.AddDeckTrayLoadedListener(new AbsDeckPickerTrayDisplay.DeckTrayLoaded(this.OnTrayLoaded));
        this.m_deckPickerTray.InitAssets();
        this.m_deckPickerTray.SetPlayButtonText(GameStrings.Get("GLOBAL_PLAY"));
        AdventureConfig adventureConfig = AdventureConfig.Get();
        this.m_deckPickerTray.SetHeaderText((string) GameUtils.GetAdventureDataRecord((int) adventureConfig.GetSelectedAdventure(), (int) adventureConfig.GetSelectedMode()).Name);
      }
    }
  }

  private void OnTrayLoaded()
  {
    AdventureSubScene component = this.GetComponent<AdventureSubScene>();
    if (!((Object) component != (Object) null))
      return;
    component.SetIsLoaded(true);
  }

  public DeckPickerTrayDisplay GetDeckPickerTrayDisplay() => this.m_deckPickerTray;
}
