using System;
using UnityEngine;

public class ClassFilterButtonContainer : MonoBehaviour
{
  public TAG_CLASS[] m_classTags;
  public ClassFilterButton[] m_classButtons;
  public Material[] m_classMaterials;
  public Material[] m_runeMaterials;
  public Material m_inactiveMaterial;
  public Material m_templateMaterial;
  public PegUIElement m_cardBacksButton;
  public PegUIElement m_heroSkinsButton;
  public PegUIElement m_coinsButton;
  public GameObject m_cardBacksDisabled;
  public GameObject m_heroSkinsDisabled;
  public GameObject m_coinsDisabled;

  private void OnEnable() => CollectionManagerDisplay.HideLockedRunesCheckboxToggled += new Action<bool>(this.OnHideLockedRunesCheckboxToggled);

  private void OnDisable() => CollectionManagerDisplay.HideLockedRunesCheckboxToggled -= new Action<bool>(this.OnHideLockedRunesCheckboxToggled);

  private void OnHideLockedRunesCheckboxToggled(bool isChecked) => this.UpdateClassButtons();

  private void SetCardBacksEnabled(bool enabled)
  {
    this.m_cardBacksButton.SetEnabled(enabled);
    this.m_cardBacksDisabled.SetActive(!enabled);
  }

  private void SetHeroSkinsEnabled(bool enabled)
  {
    this.m_heroSkinsButton.SetEnabled(enabled);
    this.m_heroSkinsDisabled.SetActive(!enabled);
  }

  private void SetCoinsEnabled(bool enabled)
  {
    this.m_coinsButton.SetEnabled(enabled);
    this.m_coinsDisabled.SetActive(!enabled);
  }

  private void UpdateCosmeticButtons()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    int num;
    bool enabled1 = (num = (collectionManager.GetCollectibleDisplay().GetPageManager() as CollectionPageManager).HasAnyCardsAvailable() ? 1 : 0) != 0;
    bool enabled2 = num != 0;
    bool enabled3 = num != 0;
    CollectionDeck editedDeck = collectionManager.GetEditedDeck();
    if (editedDeck != null)
    {
      int count = CardBackManager.Get().GetCardBacksOwned().Count;
      int ownedHeroesForClass = collectionManager.GetCountOfOwnedHeroesForClass(editedDeck.GetClass());
      bool flag = SceneMgr.Get().IsInDuelsMode() || editedDeck.HasUIHeroOverride();
      enabled1 = count > 1;
      enabled2 = ownedHeroesForClass > 1 && !flag;
      enabled3 = false;
    }
    this.SetCardBacksEnabled(enabled1);
    this.SetHeroSkinsEnabled(enabled2);
    this.SetCoinsEnabled(enabled3);
  }

  private static bool SetupButton(
    ClassFilterButton button,
    CollectionTabInfo tabInfo,
    CollectionPageManager pageManager,
    Material material)
  {
    if (!pageManager.HasClassCardsAvailable(tabInfo.tagClass))
      return false;
    int newCardsForClass = pageManager.GetNumNewCardsForClass(tabInfo.tagClass);
    button.SetTabInfo(tabInfo, material);
    button.SetNewCardCount(newCardsForClass);
    return true;
  }

  private void UpdateClassButtons()
  {
    for (int index = 0; index < this.m_classTags.Length; ++index)
    {
      this.m_classButtons[index].SetTabInfo(new CollectionTabInfo(), this.m_inactiveMaterial);
      this.m_classButtons[index].SetNewCardCount(0);
    }
    CollectionPageManager pageManager = CollectionManager.Get().GetCollectibleDisplay().GetPageManager() as CollectionPageManager;
    if ((UnityEngine.Object) pageManager == (UnityEngine.Object) null)
    {
      Debug.Log((object) "ClassFilterButtonContainer: UpdateClassButtons: pageManager is null");
    }
    else
    {
      int index1 = 0;
      for (int index2 = 0; index2 < this.m_classTags.Length; ++index2)
      {
        if (pageManager.HasClassCardsAvailable(this.m_classTags[index2]))
        {
          CollectionTabInfo tabInfo = new CollectionTabInfo()
          {
            tagClass = this.m_classTags[index2]
          };
          this.m_classButtons[index1].SetTabInfo(tabInfo, this.m_classMaterials[index2]);
          int newCardsForClass = pageManager.GetNumNewCardsForClass(this.m_classTags[index2]);
          this.m_classButtons[index1].SetNewCardCount(newCardsForClass);
          ++index1;
        }
      }
    }
  }

  public void UpdateButtons()
  {
    this.UpdateCosmeticButtons();
    this.UpdateClassButtons();
  }
}
