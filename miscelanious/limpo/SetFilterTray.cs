using Blizzard.T5.AssetManager;
using Blizzard.T5.Services;
using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetFilterTray : MonoBehaviour
{
  public UIBScrollable m_scroller;
  public GameObject m_contents;
  public CollectionSetFilterDropdownToggle m_toggleButton;
  public PegUIElement m_hideArea;
  public GameObject m_trayObject;
  public GameObject m_contentsBone;
  public GameObject m_headerPrefab;
  public GameObject m_itemPrefab;
  public GameObject m_showBone;
  public GameObject m_hideBone;
  public GameObject m_setFilterButtonGlow;
  private bool m_shown;
  private FormatType m_formatType = FormatType.FT_WILD;
  private bool m_editingDeck;
  private bool m_showUnownedSets;
  private bool m_isAnimating;
  private bool m_glowEnabled;
  private List<SetFilterItem> m_items = new List<SetFilterItem>();
  private float m_lastCollectionQueryTime;
  private HashSet<TAG_CARD_SET> m_setsWithOwnedCards = new HashSet<TAG_CARD_SET>();
  private SetFilterItem m_selected;
  private SetFilterItem m_lastSelected;

  private void Awake()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_toggleButton.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.Show(true)));
      this.m_hideArea.AddEventListener(UIEventType.PRESS, (UIEvent.Handler) (e => this.Show(false)));
      this.m_trayObject.SetActive(false);
    }
    else
      this.m_hideArea.gameObject.SetActive(false);
    this.m_toggleButton.gameObject.SetActive(false);
  }

  public void SetButtonShown(bool isShown) => this.m_toggleButton.gameObject.SetActive(isShown);

  public void SetButtonEnabled(bool isEnabled)
  {
    this.m_toggleButton.SetEnabled(isEnabled);
    this.m_toggleButton.SetEnabledVisual(isEnabled);
    if (!((UnityEngine.Object) this.m_setFilterButtonGlow != (UnityEngine.Object) null))
      return;
    this.m_setFilterButtonGlow.SetActive(this.m_glowEnabled & isEnabled);
  }

  public void SetFilterButtonGlowActive(bool active)
  {
    this.m_glowEnabled = active;
    if (!((UnityEngine.Object) this.m_setFilterButtonGlow != (UnityEngine.Object) null))
      return;
    this.m_setFilterButtonGlow.SetActive(active);
  }

  public void AddHeader(string headerName, FormatType formatType)
  {
    GameObject child = UnityEngine.Object.Instantiate<GameObject>(this.m_headerPrefab);
    GameUtils.SetParent(child, this.m_contents);
    child.SetActive(false);
    SetFilterItem component1 = child.GetComponent<SetFilterItem>();
    UIBScrollableItem component2 = child.GetComponent<UIBScrollableItem>();
    component1.IsHeader = true;
    component1.Text = headerName;
    component1.Height = component2.m_size.z;
    component1.FormatType = formatType;
    this.m_items.Add(component1);
  }

  public void AddItem(
    string itemName,
    string iconTextureAssetRef,
    UnityEngine.Vector2? iconOffset,
    SetFilterItem.ItemSelectedCallback callback,
    List<TAG_CARD_SET> data,
    FormatType formatType,
    bool isAllStandard = false)
  {
    SetFilterItem callbackData = this.AddItemUsingTexture(itemName, (Texture) null, iconOffset, callback, data, (List<int>) null, formatType, isAllStandard);
    if (string.IsNullOrEmpty(iconTextureAssetRef))
      return;
    AssetHandleCallback<Texture> callback1 = (AssetHandleCallback<Texture>) ((assetRef, texture, loadTextureCbData) =>
    {
      SetFilterItem setFilterItem = loadTextureCbData as SetFilterItem;
      if ((UnityEngine.Object) setFilterItem == (UnityEngine.Object) null)
      {
        texture?.Dispose();
      }
      else
      {
        ServiceManager.Get<DisposablesCleaner>()?.Attach((Component) setFilterItem, (IDisposable) texture);
        setFilterItem.IconTexture = (Texture) texture;
        setFilterItem.IconOffset = iconOffset;
      }
    });
    AssetLoader.Get().LoadAsset<Texture>((AssetReference) iconTextureAssetRef, callback1, (object) callbackData);
  }

  public SetFilterItem AddItemUsingTexture(
    string itemName,
    Texture iconTexture,
    UnityEngine.Vector2? iconOffset,
    SetFilterItem.ItemSelectedCallback callback,
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    FormatType formatType,
    bool isAllStandard = false,
    bool tooltipActive = false,
    string tooltipHeadline = null,
    string tooltipDescription = null)
  {
    GameObject child = UnityEngine.Object.Instantiate<GameObject>(this.m_itemPrefab);
    SetFilterItem item = child.GetComponent<SetFilterItem>();
    GameUtils.SetParent(child, this.m_contents);
    child.SetActive(false);
    UIBScrollableItem component = child.GetComponent<UIBScrollableItem>();
    item.IsHeader = false;
    item.Text = itemName;
    item.Height = component.m_size.z;
    item.FormatType = formatType;
    item.IsAllStandard = isAllStandard;
    item.CardSets = cardSets;
    item.SpecificCards = specificCards;
    item.Callback = callback;
    item.IconTexture = iconTexture;
    item.IconOffset = iconOffset;
    item.TooltipHeadline = tooltipHeadline;
    item.TooltipDescription = tooltipDescription;
    item.Tooltip.ScreenConstraintLayerOverride = GameLayer.Default;
    item.ShowTooltip = tooltipActive;
    this.m_items.Add(item);
    item.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.Select(item)));
    return item;
  }

  public void SelectFirstItem(bool transitionPage = true)
  {
    foreach (SetFilterItem setFilterItem in this.m_items)
    {
      if (!setFilterItem.IsHeader)
      {
        UIBScrollableItem component = setFilterItem.GetComponent<UIBScrollableItem>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.m_active == UIBScrollableItem.ActiveState.Active)
        {
          this.Select(setFilterItem, transitionPage: transitionPage);
          break;
        }
      }
    }
  }

  public bool SelectFirstItemWithFormat(FormatType formatType, bool transitionPage = true)
  {
    SetFilterItem setFilterItem = this.m_items.Where<SetFilterItem>((Func<SetFilterItem, bool>) (item =>
    {
      UIBScrollableItem component = item.GetComponent<UIBScrollableItem>();
      return !item.IsHeader && item.FormatType == formatType && (UnityEngine.Object) component != (UnityEngine.Object) null && component.m_active == UIBScrollableItem.ActiveState.Active;
    })).FirstOrDefault<SetFilterItem>();
    if ((UnityEngine.Object) setFilterItem == (UnityEngine.Object) null)
      return false;
    this.Select(setFilterItem, transitionPage);
    return true;
  }

  public bool HasActiveFilter()
  {
    foreach (SetFilterItem setFilterItem in this.m_items)
    {
      if (!setFilterItem.IsHeader && setFilterItem.GetComponent<UIBScrollableItem>().m_active != UIBScrollableItem.ActiveState.Inactive)
        return !((UnityEngine.Object) setFilterItem == (UnityEngine.Object) this.m_selected);
    }
    return false;
  }

  public void Select(SetFilterItem item, bool callCallback = true, bool transitionPage = true)
  {
    if ((UnityEngine.Object) item == (UnityEngine.Object) this.m_selected)
      return;
    if ((UnityEngine.Object) this.m_selected != (UnityEngine.Object) null)
    {
      this.m_selected.SetSelected(false);
      this.m_lastSelected = this.m_selected;
    }
    this.m_selected = item;
    item.SetSelected(true);
    if (callCallback)
      item.Callback(item.CardSets, item.SpecificCards, item.FormatType, item, transitionPage);
    this.m_toggleButton.SetToggleIcon(item.IconTexture, item.IconOffset.Value);
  }

  public void SelectPreviouslySelectedItem() => this.Select(this.m_lastSelected, false);

  public void UpdateSetFilters(FormatType formatType, bool editingDeck, bool showUnownedSets)
  {
    if (this.m_formatType == formatType && this.m_editingDeck == editingDeck && this.m_showUnownedSets == showUnownedSets)
      return;
    this.m_formatType = formatType;
    this.m_editingDeck = editingDeck;
    this.m_showUnownedSets = showUnownedSets;
    this.Arrange();
  }

  public void ClearFilter(bool transitionPage = true)
  {
    this.SelectFirstItem(transitionPage);
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.SetButtonShown(false);
  }

  public void Show(bool show)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
    {
      if (this.m_isAnimating)
        return;
      this.m_shown = show;
      this.m_trayObject.SetActive(true);
      this.m_hideArea.gameObject.SetActive(true);
      UIBHighlight component = this.m_toggleButton.GetComponent<UIBHighlight>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.AlwaysOver = show;
      this.m_isAnimating = true;
      if (show)
      {
        this.Arrange();
        this.m_trayObject.transform.localPosition = this.m_hideBone.transform.localPosition;
        iTween.MoveTo(this.m_trayObject, iTween.Hash((object) "position", (object) this.m_showBone.transform.localPosition, (object) "time", (object) 0.35f, (object) "easeType", (object) iTween.EaseType.easeOutCubic, (object) "isLocal", (object) true, (object) "oncomplete", (object) "FinishFilterShown", (object) "oncompletetarget", (object) this.gameObject));
        SoundManager.Get().LoadAndPlay((AssetReference) "choose_opponent_panel_slide_on.prefab:66491d3d01ed663429ab80daf6a5e880", this.gameObject);
      }
      else
      {
        this.m_trayObject.transform.localPosition = this.m_showBone.transform.localPosition;
        iTween.MoveTo(this.m_trayObject, iTween.Hash((object) "position", (object) this.m_hideBone.transform.localPosition, (object) "time", (object) 0.25f, (object) "easeType", (object) iTween.EaseType.easeOutCubic, (object) "isLocal", (object) true, (object) "oncomplete", (object) "FinishFilterHidden", (object) "oncompletetarget", (object) this.gameObject));
        SoundManager.Get().LoadAndPlay((AssetReference) "choose_opponent_panel_slide_off.prefab:3139d09eb94899d41b9bf612649f47bf", this.gameObject);
      }
      this.m_hideArea.gameObject.SetActive(this.m_shown);
    }
    else
    {
      this.m_shown = show;
      if (show)
        this.Arrange();
    }
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (!((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null))
      return;
    collectibleDisplay.HideSetFilterTutorial();
  }

  public bool IsShown() => this.m_shown;

  private void FinishFilterShown() => this.m_isAnimating = false;

  private void FinishFilterHidden()
  {
    this.m_isAnimating = false;
    this.m_trayObject.SetActive(false);
    this.m_hideArea.gameObject.SetActive(false);
  }

  private void Arrange()
  {
    this.m_scroller.ClearVisibleAffectObjects();
    if (!this.m_showUnownedSets)
      this.EvaluateOwnership();
    Vector3 position = this.m_contentsBone.transform.position;
    bool flag1 = false;
    foreach (SetFilterItem setFilterItem in this.m_items)
    {
      UIBScrollableItem component = setFilterItem.GetComponent<UIBScrollableItem>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "SetFilterItem has no UIBScrollableItem component!");
      }
      else
      {
        bool flag2 = false;
        if (setFilterItem.FormatType == FormatType.FT_WILD && this.m_formatType != FormatType.FT_WILD)
          flag2 = true;
        else if (setFilterItem.FormatType == FormatType.FT_CLASSIC && this.m_formatType == FormatType.FT_STANDARD)
          flag2 = true;
        else if (setFilterItem.FormatType != FormatType.FT_CLASSIC && this.m_formatType == FormatType.FT_CLASSIC)
          flag2 = true;
        else if (this.m_editingDeck && setFilterItem.IsAllStandard && this.m_formatType == FormatType.FT_WILD)
          flag2 = true;
        else if (this.m_editingDeck && setFilterItem.FormatType == FormatType.FT_CLASSIC && this.m_formatType != FormatType.FT_CLASSIC)
          flag2 = true;
        else if (!this.m_showUnownedSets && !this.OwnCardInSetsForItem(setFilterItem))
          flag2 = true;
        if (flag2)
        {
          if ((UnityEngine.Object) setFilterItem == (UnityEngine.Object) this.m_selected)
            flag1 = true;
          setFilterItem.gameObject.SetActive(false);
          component.m_active = UIBScrollableItem.ActiveState.Inactive;
        }
        else
        {
          setFilterItem.gameObject.SetActive(true);
          component.m_active = UIBScrollableItem.ActiveState.Active;
          setFilterItem.gameObject.transform.position = position;
          position.z -= setFilterItem.Height;
          this.m_scroller.AddVisibleAffectedObject(setFilterItem.gameObject, new Vector3(setFilterItem.Height, setFilterItem.Height, setFilterItem.Height), true);
        }
      }
    }
    if (flag1)
      this.SelectFirstItem();
    this.m_scroller.UpdateAndFireVisibleAffectedObjects();
  }

  private void EvaluateOwnership()
  {
    if ((double) this.m_lastCollectionQueryTime > (double) CollectionManager.Get().CollectionLastModifiedTime())
      return;
    this.m_setsWithOwnedCards.Clear();
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    List<CollectibleCard> allCards = CollectionManager.Get().GetAllCards();
    for (int index = 0; index < allCards.Count; ++index)
    {
      CollectibleCard collectibleCard = allCards[index];
      if (collectibleCard.OwnedCount > 0)
        this.m_setsWithOwnedCards.Add(collectibleCard.Set);
    }
    Log.Performance.Print("SetFilterTray - Evaluating Ownership took {0} seconds.", (object) (float) ((double) Time.realtimeSinceStartup - (double) realtimeSinceStartup));
    this.m_lastCollectionQueryTime = Time.realtimeSinceStartup;
  }

  private bool OwnCardInSetsForItem(SetFilterItem item)
  {
    if (item.CardSets == null)
      return true;
    for (int index = 0; index < item.CardSets.Count; ++index)
    {
      if (this.m_setsWithOwnedCards.Contains(item.CardSets[index]))
        return true;
    }
    return false;
  }

  public void RemoveAllItems()
  {
    foreach (Component component in this.m_items)
      component.gameObject.SetActive(false);
    this.m_items.Clear();
  }
}
