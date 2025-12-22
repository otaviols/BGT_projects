using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class DropdownControl : PegUIElement
{
  [CustomEditField(Sections = "Buttons")]
  public DropdownMenuItem m_selectedItem;
  [CustomEditField(Sections = "Buttons")]
  public PegUIElement m_cancelCatcher;
  [CustomEditField(Sections = "Buttons")]
  public UIBButton m_button;
  [CustomEditField(Sections = "Menu")]
  public MultiSliceElement m_menu;
  [CustomEditField(Sections = "Menu")]
  public GameObject m_menuMiddle;
  [CustomEditField(Sections = "Menu")]
  public MultiSliceElement m_menuItemContainer;
  [CustomEditField(Sections = "Menu Templates")]
  public DropdownMenuItem m_menuItemTemplate;
  private string m_unselectedItemText = string.Empty;
  private DropdownControl.itemChosenCallback m_itemChosenCallback = (DropdownControl.itemChosenCallback) ((_param1, _param2) => { });
  private DropdownControl.itemTextCallback m_itemTextCallback = new DropdownControl.itemTextCallback(DropdownControl.defaultItemTextCallback);
  private DropdownControl.menuShownCallback m_menuShownCallback = (DropdownControl.menuShownCallback) (_param1 => { });
  private List<DropdownMenuItem> m_items = new List<DropdownMenuItem>();
  private Font m_overrideFont;

  public void Start()
  {
    this.m_button.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.onUserPressedButton()));
    this.m_selectedItem.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.onUserPressedSelection(this.m_selectedItem)));
    this.m_cancelCatcher.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.onUserCancelled()));
    this.hideMenu();
  }

  public void addItem(object value)
  {
    DropdownMenuItem item = (DropdownMenuItem) GameUtils.Instantiate((Component) this.m_menuItemTemplate, this.m_menuItemContainer.gameObject);
    item.gameObject.transform.localRotation = this.m_menuItemTemplate.transform.localRotation;
    item.gameObject.transform.localScale = this.m_menuItemTemplate.transform.localScale;
    this.m_items.Add(item);
    if ((Object) this.m_overrideFont != (Object) null)
      item.m_text.TrueTypeFont = this.m_overrideFont;
    item.SetValue(value, this.m_itemTextCallback(value));
    item.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.onUserItemClicked(item)));
    item.gameObject.SetActive(true);
    this.layoutMenu();
  }

  public bool removeItem(object value)
  {
    int itemIndex = this.findItemIndex(value);
    if (itemIndex < 0)
      return false;
    DropdownMenuItem dropdownMenuItem = this.m_items[itemIndex];
    this.m_items.RemoveAt(itemIndex);
    Object.Destroy((Object) dropdownMenuItem.gameObject);
    this.layoutMenu();
    return true;
  }

  public void clearItems()
  {
    foreach (Component component in this.m_items)
      Object.Destroy((Object) component.gameObject);
    this.layoutMenu();
  }

  public void setSelectionToLastItem()
  {
    this.m_selectedItem.SetValue((object) null, this.m_unselectedItemText);
    if (this.m_items.Count == 0)
      return;
    for (int index = 0; index < this.m_items.Count - 1; ++index)
      this.m_items[index].SetSelected(false);
    DropdownMenuItem dropdownMenuItem = this.m_items[this.m_items.Count - 1];
    dropdownMenuItem.SetSelected(true);
    this.m_selectedItem.SetValue(dropdownMenuItem.GetValue(), this.m_itemTextCallback(dropdownMenuItem.GetValue()));
  }

  public void setSelectionToFirstItem()
  {
    this.m_selectedItem.SetValue((object) null, this.m_unselectedItemText);
    if (this.m_items.Count == 0)
      return;
    for (int index = 0; index < this.m_items.Count - 1; ++index)
      this.m_items[index].SetSelected(false);
    DropdownMenuItem dropdownMenuItem = this.m_items[0];
    dropdownMenuItem.SetSelected(true);
    this.m_selectedItem.SetValue(dropdownMenuItem.GetValue(), this.m_itemTextCallback(dropdownMenuItem.GetValue()));
  }

  public object getSelection() => this.m_selectedItem.GetValue();

  public void setSelection(object val)
  {
    this.m_selectedItem.SetValue((object) null, this.m_unselectedItemText);
    for (int index = 0; index < this.m_items.Count; ++index)
    {
      DropdownMenuItem dropdownMenuItem = this.m_items[index];
      object val1 = dropdownMenuItem.GetValue();
      if (val1 == null && val == null || val1.Equals(val))
      {
        dropdownMenuItem.SetSelected(true);
        this.m_selectedItem.SetValue(val1, this.m_itemTextCallback(val1));
      }
      else
        dropdownMenuItem.SetSelected(false);
    }
  }

  public void onUserPressedButton() => this.showMenu();

  public void onUserPressedSelection(DropdownMenuItem item) => this.showMenu();

  public void onUserItemClicked(DropdownMenuItem item)
  {
    this.hideMenu();
    object selection = this.getSelection();
    object obj = item.GetValue();
    this.setSelection(obj);
    this.m_itemChosenCallback(obj, selection);
  }

  public void onUserCancelled()
  {
    if (SoundManager.Get().IsInitialized())
      SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    this.hideMenu();
  }

  public void setUnselectedItemText(string text) => this.m_unselectedItemText = text;

  public DropdownControl.itemChosenCallback getItemChosenCallback() => this.m_itemChosenCallback;

  public void setItemChosenCallback(DropdownControl.itemChosenCallback callback) => this.m_itemChosenCallback = callback ?? (DropdownControl.itemChosenCallback) ((_param1, _param2) => { });

  public DropdownControl.itemTextCallback getItemTextCallback() => this.m_itemTextCallback;

  public void setItemTextCallback(DropdownControl.itemTextCallback callback) => this.m_itemTextCallback = callback ?? new DropdownControl.itemTextCallback(DropdownControl.defaultItemTextCallback);

  public static string defaultItemTextCallback(object val) => val != null ? val.ToString() : string.Empty;

  public bool isMenuShown() => this.m_menu.gameObject.activeInHierarchy;

  public DropdownControl.menuShownCallback getMenuShownCallback() => this.m_menuShownCallback;

  public void setMenuShownCallback(DropdownControl.menuShownCallback callback) => this.m_menuShownCallback = callback;

  public void setFont(Font font)
  {
    this.m_overrideFont = font;
    this.m_selectedItem.m_text.TrueTypeFont = font;
    this.m_menuItemTemplate.m_text.TrueTypeFont = font;
  }

  private void showMenu()
  {
    this.m_cancelCatcher.gameObject.SetActive(true);
    this.m_menu.gameObject.SetActive(true);
    this.layoutMenu();
    this.m_menuShownCallback(true);
  }

  private void hideMenu()
  {
    this.m_cancelCatcher.gameObject.SetActive(false);
    this.m_menu.gameObject.SetActive(false);
    this.m_menuShownCallback(false);
  }

  private void layoutMenu()
  {
    if (!this.gameObject.activeSelf)
      return;
    this.m_menuItemTemplate.gameObject.SetActive(true);
    OrientedBounds orientedWorldBounds = TransformUtil.ComputeOrientedWorldBounds(this.m_menuItemTemplate.gameObject);
    if (orientedWorldBounds == null)
      return;
    float num = orientedWorldBounds.Extents[1].magnitude * 2f;
    this.m_menuItemTemplate.gameObject.SetActive(false);
    this.m_menuItemContainer.ClearSlices();
    for (int index = 0; index < this.m_items.Count; ++index)
      this.m_menuItemContainer.AddSlice(this.m_items[index].gameObject);
    this.m_menuItemContainer.UpdateSlices();
    if (this.m_items.Count <= 1)
      TransformUtil.SetLocalScaleZ(this.m_menuMiddle, 1f / 1000f);
    else
      TransformUtil.SetLocalScaleToWorldDimension(this.m_menuMiddle, new WorldDimensionIndex(num * (float) (this.m_items.Count - 1), 2));
    this.m_menu.UpdateSlices();
  }

  private int findItemIndex(object value)
  {
    for (int index = 0; index < this.m_items.Count; ++index)
    {
      if (this.m_items[index].GetValue() == value)
        return index;
    }
    return -1;
  }

  private DropdownMenuItem findItem(object value)
  {
    for (int index = 0; index < this.m_items.Count; ++index)
    {
      DropdownMenuItem dropdownMenuItem = this.m_items[index];
      if (dropdownMenuItem.GetValue() == value)
        return dropdownMenuItem;
    }
    return (DropdownMenuItem) null;
  }

  public delegate void itemChosenCallback(object selection, object prevSelection);

  public delegate string itemTextCallback(object val);

  public delegate void menuShownCallback(bool shown);
}
