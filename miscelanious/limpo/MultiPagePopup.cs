using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class MultiPagePopup : DialogBase
{
  private readonly Map<MultiPagePopup.PageType, string> m_pagePrefabRefs = new Map<MultiPagePopup.PageType, string>()
  {
    {
      MultiPagePopup.PageType.CARD_LIST,
      "CardListPage.prefab:e48c89787318c4d49bd21abc51901bf8"
    },
    {
      MultiPagePopup.PageType.DUST_JAR,
      "DustJarPage.prefab:9d96713c54a11764691eb73236976680"
    }
  };
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_showAnimationSound = "Expand_Up.prefab:775d97ea42498c044897f396362b9db3";
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_hideAnimationSound = "Shrink_Down_Quicker.prefab:2fe963b171811ca4b8d544fa53e3330c";
  private MultiPagePopup.Info m_info = new MultiPagePopup.Info();
  private int m_currentPageIdx;
  private Map<int, GameObject> m_pageObjects = new Map<int, GameObject>();
  private int m_numPagesLoaded;

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (UniversalInputManager.Get() == null)
      return;
    UniversalInputManager.Get().SetSystemDialogActive(false);
  }

  public void SetInfo(MultiPagePopup.Info info)
  {
    this.m_info = info;
    if (this.m_info.m_callbackOnHide == null)
      return;
    this.AddHideListener(this.m_info.m_callbackOnHide);
  }

  public override void Show()
  {
    if (this.m_info.m_blurWhenShown)
      DialogBase.DoBlur();
    UniversalInputManager.Get().SetSystemDialogActive(true);
    int num = 0;
    foreach (MultiPagePopup.PageInfo page in this.m_info.m_pages)
    {
      string pageAssetRef = this.GetPageAssetRef(page);
      this.m_pageObjects[num] = (GameObject) null;
      AssetLoader.Get().InstantiatePrefab((AssetReference) pageAssetRef, new PrefabCallback<GameObject>(this.OnPageLoaded), (object) num, AssetLoadingOptions.IgnorePrefabPosition);
      ++num;
    }
    this.StartCoroutine(this.ShowWhenReady());
  }

  public override void Hide()
  {
    base.Hide();
    if (!this.m_info.m_blurWhenShown)
      return;
    DialogBase.EndBlur();
  }

  private string GetPageAssetRef(MultiPagePopup.PageInfo pageInfo)
  {
    if (!string.IsNullOrEmpty(pageInfo.m_customPrefabAssetRef))
      return pageInfo.m_customPrefabAssetRef;
    string pageAssetRef;
    this.m_pagePrefabRefs.TryGetValue(pageInfo.m_pageType, out pageAssetRef);
    return pageAssetRef;
  }

  private void OnPageLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_pageObjects[(int) callbackData] = go;
    GameUtils.SetParent(go, this.gameObject);
    LayerUtils.SetLayer(go, this.gameObject.layer);
    go.SetActive(false);
    ++this.m_numPagesLoaded;
  }

  private IEnumerator ShowWhenReady()
  {
    MultiPagePopup multiPagePopup = this;
    while (multiPagePopup.m_numPagesLoaded < multiPagePopup.m_pageObjects.Count)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    multiPagePopup.\u003C\u003En__0();
    if (!string.IsNullOrEmpty(multiPagePopup.m_showAnimationSound))
      SoundManager.Get().LoadAndPlay((AssetReference) multiPagePopup.m_showAnimationSound);
    Vector3 localScale = multiPagePopup.transform.localScale;
    multiPagePopup.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    Hashtable args = iTween.Hash((object) "scale", (object) localScale, (object) "time", (object) 0.3f, (object) "easetype", (object) iTween.EaseType.easeOutBack);
    iTween.ScaleTo(multiPagePopup.gameObject, args);
    UniversalInputManager.Get().SetSystemDialogActive(true);
    if (!multiPagePopup.ShowPage(multiPagePopup.m_currentPageIdx))
      multiPagePopup.Hide();
  }

  protected override void DoHideAnimation()
  {
    if (!string.IsNullOrEmpty(this.m_hideAnimationSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_hideAnimationSound);
    base.DoHideAnimation();
  }

  private void PressNext()
  {
    GameObject gameObject = (GameObject) null;
    if (this.m_pageObjects.TryGetValue(this.m_currentPageIdx, out gameObject))
      gameObject.gameObject.SetActive(false);
    ++this.m_currentPageIdx;
    if (this.ShowPage(this.m_currentPageIdx))
      return;
    this.Hide();
  }

  private bool ShowPage(int pageIdx)
  {
    if (pageIdx >= this.m_info.m_pages.Count)
      return false;
    MultiPagePopup.PageInfo page = this.m_info.m_pages[pageIdx];
    if (page == null)
      return false;
    GameObject gameObject = (GameObject) null;
    if (!this.m_pageObjects.TryGetValue(pageIdx, out gameObject))
      return false;
    MultiPagePopupPage component = gameObject.GetComponent<MultiPagePopupPage>();
    if ((Object) component == (Object) null)
      return false;
    gameObject.SetActive(true);
    component.m_button.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.PressNext()));
    component.m_buttonText.Text = pageIdx != this.m_info.m_pages.Count - 1 ? GameStrings.Get("GLOBAL_BUTTON_NEXT") : GameStrings.Get("GLOBAL_DONE");
    if ((Object) component.m_headerText != (Object) null && page.m_headerText != null)
      component.m_headerText.Text = page.m_headerText;
    if ((Object) component.m_bodyText != (Object) null && page.m_bodyText != null)
      component.m_bodyText.Text = page.m_bodyText;
    if ((Object) component.m_footerText != (Object) null && page.m_footerText != null)
      component.m_footerText.Text = page.m_footerText;
    CardListPanel componentInChildren1 = gameObject.GetComponentInChildren<CardListPanel>();
    if ((Object) componentInChildren1 != (Object) null)
      componentInChildren1.Show(page.m_cards);
    if (page.m_dustAmount > 0)
    {
      DustJarPanel componentInChildren2 = gameObject.GetComponentInChildren<DustJarPanel>();
      if ((Object) componentInChildren2 != (Object) null)
        componentInChildren2.Show(page.m_dustAmount);
    }
    return true;
  }

  public enum PageType
  {
    CARD_LIST,
    DUST_JAR,
  }

  public class PageInfo
  {
    public MultiPagePopup.PageType m_pageType;
    public string m_customPrefabAssetRef;
    public string m_headerText;
    public string m_bodyText;
    public string m_footerText;
    public List<int> m_cards;
    public int m_dustAmount;
  }

  public class Info
  {
    public DialogBase.HideCallback m_callbackOnHide;
    public bool m_blurWhenShown;
    public List<MultiPagePopup.PageInfo> m_pages = new List<MultiPagePopup.PageInfo>();
  }
}
