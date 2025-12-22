using UnityEngine;

public class ShowAllCardsTab : MonoBehaviour
{
  public CheckBox m_showAllCardsCheckBox;
  public CheckBox m_includePremiumsCheckBox;

  private void Awake()
  {
    this.m_showAllCardsCheckBox.SetButtonText(GameStrings.Get("GLUE_COLLECTION_SHOW_ALL_CARDS"));
    this.m_includePremiumsCheckBox.SetButtonText(GameStrings.Get("GLUE_COLLECTION_INCLUDE_PREMIUMS"));
  }

  private void Start()
  {
    this.m_includePremiumsCheckBox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleIncludePremiums));
    this.m_showAllCardsCheckBox.SetChecked(false);
    this.m_includePremiumsCheckBox.SetChecked(false);
    this.m_includePremiumsCheckBox.gameObject.SetActive(false);
  }

  public bool IsShowAllChecked() => this.m_showAllCardsCheckBox.IsChecked();

  private void ToggleIncludePremiums(UIEvent e)
  {
    bool show = this.m_includePremiumsCheckBox.IsChecked();
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((Object) collectibleDisplay != (Object) null)
      collectibleDisplay.ShowPremiumCardsNotOwned(show);
    if (show)
      SoundManager.Get().LoadAndPlay((AssetReference) "checkbox_toggle_on.prefab:8be4c59e7387600468ac88787943da8b", this.gameObject);
    else
      SoundManager.Get().LoadAndPlay((AssetReference) "checkbox_toggle_off.prefab:fa341d119cee1d14c941b63dba112af3", this.gameObject);
  }
}
