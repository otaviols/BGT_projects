using Assets;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Core;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GeneralStoreAdventureContent : GeneralStoreContent
{
  [CustomEditField(Sections = "General Store")]
  public GeneralStoreAdventureContentDisplay m_adventureDisplay;
  [CustomEditField(Sections = "Animation/Preorder")]
  public GeneralStoreRewardsCardBack m_preorderCardBackReward;
  [CustomEditField(Sections = "General Store")]
  public GameObject m_adventureEmptyDisplay;
  [CustomEditField(Sections = "Rewards")]
  public GameObject m_adventureCardPreviewPanel;
  [CustomEditField(Sections = "Rewards")]
  public UberText m_adventureCardPreviewText;
  [CustomEditField(Sections = "Rewards")]
  public List<GameObject> m_adventureCardPreviewBones;
  [CustomEditField(Sections = "Rewards")]
  public PegUIElement m_adventureCardPreviewOffClicker;
  [CustomEditField(Sections = "General Store/Buttons")]
  public GameObject m_adventureRadioButtonContainer;
  [CustomEditField(Sections = "General Store/Buttons")]
  public UberText m_adventureRadioButtonText;
  [CustomEditField(Sections = "General Store/Buttons")]
  public UberText m_adventureRadioButtonCostText;
  [CustomEditField(Sections = "General Store/Buttons")]
  public RadioButton m_adventureRadioButton;
  [CustomEditField(Sections = "General Store/Buttons")]
  public GameObject m_adventureOwnedCheckmark;
  [CustomEditField(Sections = "Sounds & Music", T = EditType.SOUND_PREFAB)]
  public string m_backgroundFlipSound;
  [CustomEditField(Sections = "Animation")]
  public float m_backgroundFlipAnimTime = 0.5f;
  [CustomEditField(Sections = "Animation")]
  public float m_adventureLogoFadeInTime = 0.5f;
  private bool m_showPreviewCards;
  private Map<string, Actor> m_loadedPreviewCards = new Map<string, Actor>();
  private AdventureDbId m_selectedAdventureId;
  private Map<int, StoreAdventureDef> m_storeAdvDefs = new Map<int, StoreAdventureDef>();
  private int m_currentDisplay = -1;
  private GeneralStoreAdventureContentDisplay m_adventureDisplay1;
  private GeneralStoreAdventureContentDisplay m_adventureDisplay2;
  public static readonly bool REQUIRE_REAL_MONEY_BUNDLE_OPTION = true;

  private void Awake()
  {
    this.m_adventureDisplay1 = this.m_adventureDisplay;
    this.m_adventureDisplay2 = UnityEngine.Object.Instantiate<GeneralStoreAdventureContentDisplay>(this.m_adventureDisplay);
    this.m_adventureDisplay2.transform.parent = this.m_adventureDisplay1.transform.parent;
    this.m_adventureDisplay2.transform.localPosition = this.m_adventureDisplay1.transform.localPosition;
    this.m_adventureDisplay2.transform.localScale = this.m_adventureDisplay1.transform.localScale;
    this.m_adventureDisplay2.transform.localRotation = this.m_adventureDisplay1.transform.localRotation;
    this.m_adventureDisplay2.gameObject.SetActive(false);
    if ((UnityEngine.Object) this.m_adventureDisplay1.m_rewardChest != (UnityEngine.Object) null)
    {
      this.m_adventureDisplay1.m_rewardChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnAdventuresShowPreviewCard));
      this.m_adventureDisplay2.m_rewardChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnAdventuresShowPreviewCard));
      if (!(bool) UniversalInputManager.UsePhoneUI)
      {
        this.m_adventureDisplay1.m_rewardChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnAdventuresHidePreviewCard));
        this.m_adventureDisplay2.m_rewardChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnAdventuresHidePreviewCard));
      }
    }
    AdventureProgressMgr.Get().RegisterProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdated));
    this.m_adventureCardPreviewPanel.SetActive(false);
    this.m_parentStore.SetChooseDescription(GameStrings.Get("GLUE_STORE_CHOOSE_ADVENTURE"));
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_adventureCardPreviewOffClicker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnAdventuresHidePreviewCard));
    foreach (AdventureDbfRecord adventureDbfRecord in GameUtils.GetSortedAdventureRecordsWithStorePrefab())
    {
      string storePrefab = adventureDbfRecord.StorePrefab;
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) storePrefab);
      if (!((UnityEngine.Object) gameObject == (UnityEngine.Object) null))
      {
        StoreAdventureDef component = gameObject.GetComponent<StoreAdventureDef>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          Debug.LogError((object) string.Format("StoreAdventureDef not found in object: {0}", (object) storePrefab));
        else
          this.m_storeAdvDefs.Add(adventureDbfRecord.ID, component);
      }
    }
  }

  private void OnDestroy() => AdventureProgressMgr.Get().RemoveProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdated));

  public void SetAdventureId(AdventureDbId adventureId, bool forceImmediate = false)
  {
    if (this.m_selectedAdventureId == adventureId)
      return;
    this.m_selectedAdventureId = adventureId;
    Network.Bundle bundle = (Network.Bundle) null;
    StoreManager.Get().GetAvailableAdventureBundle(this.m_selectedAdventureId, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
    this.SetCurrentMoneyBundle(bundle);
    this.AnimateAndUpdateDisplay((int) adventureId, forceImmediate);
    this.AnimateAdventureRadioButtonBar();
    this.UpdateAdventureDescription(bundle);
    this.UpdateAdventureTypeMusic();
    this.UpdateRadioButtonText(bundle);
  }

  public AdventureDbId GetAdventureId() => this.m_selectedAdventureId;

  public StoreAdventureDef GetStoreAdventureDef(int advId)
  {
    StoreAdventureDef storeAdventureDef;
    this.m_storeAdvDefs.TryGetValue(advId, out storeAdventureDef);
    return storeAdventureDef;
  }

  public Map<int, StoreAdventureDef> GetStoreAdventureDefs() => this.m_storeAdvDefs;

  public override void PostStoreFlipIn(bool animateIn)
  {
    this.UpdateAdventureTypeMusic();
    Hashtable args = iTween.Hash((object) "amount", (object) 1f, (object) "time", (object) this.m_adventureLogoFadeInTime);
    iTween.FadeTo(this.GetCurrentDisplay().m_logo.gameObject, args);
    if (!((UnityEngine.Object) this.m_preorderCardBackReward != (UnityEngine.Object) null) || !this.IsPreOrder())
      return;
    this.m_preorderCardBackReward.ShowCardBackReward();
  }

  public override void PreStoreFlipIn()
  {
    Hashtable args = iTween.Hash((object) "amount", (object) 0.0f, (object) "time", (object) 0);
    iTween.FadeTo(this.GetCurrentDisplay().m_logo.gameObject, args);
    if (!((UnityEngine.Object) this.m_preorderCardBackReward != (UnityEngine.Object) null))
      return;
    this.m_preorderCardBackReward.HideCardBackReward();
  }

  public override void PreStoreFlipOut()
  {
    if (!((UnityEngine.Object) this.m_preorderCardBackReward != (UnityEngine.Object) null))
      return;
    this.m_preorderCardBackReward.HideCardBackReward();
  }

  public override bool AnimateEntranceEnd()
  {
    this.m_adventureRadioButton.gameObject.SetActive(true);
    return true;
  }

  public override bool AnimateExitStart()
  {
    this.m_adventureRadioButton.gameObject.SetActive(false);
    return true;
  }

  public override bool AnimateExitEnd() => true;

  public override void TryBuyWithMoney(
    Network.Bundle bundle,
    GeneralStoreContent.BuyEvent successBuyCB,
    GeneralStoreContent.BuyEvent failedBuyCB)
  {
    if (this.IsContentActive())
    {
      if (!AchieveManager.Get().HasUnlockedFeature(Achieve.Unlocks.VANILLA_HEROES))
      {
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
        info.m_headerText = GameStrings.Get("GLUE_STORE_ADVENTURE_LOCKED_HEROES_NOT_PURCHASABLE_TITLE");
        info.m_text = GameStrings.Get("GLUE_STORE_ADVENTURE_LOCKED_HEROES_NOT_PURCHASABLE_TEXT");
        info.m_showAlertIcon = true;
        info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
        info.m_responseCallback = (AlertPopup.ResponseCallback) ((response, data) =>
        {
          this.m_parentStore.BlockInterface(false);
          if (failedBuyCB == null)
            return;
          failedBuyCB();
        });
        this.m_parentStore.BlockInterface(true);
        DialogManager.Get().ShowPopup(info);
      }
      else
      {
        if (successBuyCB == null)
          return;
        successBuyCB();
      }
    }
    else
    {
      if (failedBuyCB == null)
        return;
      failedBuyCB();
    }
  }

  public override void TryBuyWithGold(
    GeneralStoreContent.BuyEvent successBuyCB = null,
    GeneralStoreContent.BuyEvent failedBuyCB = null)
  {
    if (successBuyCB == null)
      return;
    successBuyCB();
  }

  protected override void OnRefresh()
  {
    Network.Bundle bundle = (Network.Bundle) null;
    StoreManager.Get().GetAvailableAdventureBundle(this.m_selectedAdventureId, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
    this.SetCurrentMoneyBundle(bundle);
    this.UpdateRadioButtonText(bundle);
    this.UpdateAdventureDescription(bundle);
  }

  protected override void OnBundleChanged(
    NoGTAPPTransactionData goldBundle,
    Network.Bundle moneyBundle)
  {
    this.UpdateRadioButtonText(moneyBundle);
    this.UpdateAdventureDescription(moneyBundle);
  }

  public override void StoreShown(bool isCurrent)
  {
    if (!isCurrent)
      return;
    this.UpdateAdventureTypeMusic();
  }

  public override void StoreHidden(bool isCurrent)
  {
    foreach (KeyValuePair<string, Actor> loadedPreviewCard in this.m_loadedPreviewCards)
      UnityEngine.Object.Destroy((UnityEngine.Object) loadedPreviewCard.Value.gameObject);
    this.m_loadedPreviewCards.Clear();
    if (!isCurrent)
      return;
    this.HidePreviewCardPanel();
  }

  public override bool IsPurchaseDisabled() => this.m_selectedAdventureId == AdventureDbId.INVALID;

  public override string GetMoneyDisplayOwnedText() => GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_COST_OWNED_TEXT");

  private GameObject GetCurrentDisplayContainer() => this.GetCurrentDisplay().gameObject;

  private GameObject GetNextDisplayContainer() => (this.m_currentDisplay + 1) % 2 != 0 ? this.m_adventureDisplay2.gameObject : this.m_adventureDisplay1.gameObject;

  private GeneralStoreAdventureContentDisplay GetCurrentDisplay() => this.m_currentDisplay != 0 ? this.m_adventureDisplay2 : this.m_adventureDisplay1;

  private void OnAdventuresShowPreviewCard(UIEvent e)
  {
    StoreAdventureDef storeAdventureDef = this.GetStoreAdventureDef((int) this.m_selectedAdventureId);
    if ((UnityEngine.Object) storeAdventureDef == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("Unable to find preview cards for {0} adventure.", (object) this.m_selectedAdventureId));
    }
    else
    {
      string[] previewCards = storeAdventureDef.m_previewCards.ToArray();
      if (previewCards.Length == 0)
      {
        Debug.LogError((object) string.Format("No preview cards defined for {0} adventure.", (object) this.m_selectedAdventureId));
      }
      else
      {
        this.m_showPreviewCards = true;
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_card_mouse_over.prefab:0d4e20bc78956bc48b5e2963ec39211c");
        foreach (KeyValuePair<string, Actor> loadedPreviewCard in this.m_loadedPreviewCards)
          loadedPreviewCard.Value.gameObject.SetActive(false);
        int loadedPreviewCards = 0;
        int num = 0;
        foreach (string previewCard in previewCards)
        {
          int cardIndex = num;
          this.LoadAdventurePreviewCard(previewCard, (GeneralStoreAdventureContent.DelOnAdventurePreviewCardLoaded) (cardActor =>
          {
            cardActor.transform.position = this.m_adventureCardPreviewBones[cardIndex].transform.position;
            cardActor.transform.rotation = this.m_adventureCardPreviewBones[cardIndex].transform.rotation;
            cardActor.transform.parent = this.m_adventureCardPreviewBones[cardIndex].transform;
            cardActor.transform.localScale = Vector3.one;
            ++loadedPreviewCards;
            cardActor.gameObject.SetActive(this.m_showPreviewCards);
            if (!this.m_showPreviewCards || loadedPreviewCards != previewCards.Length)
              return;
            this.ShowPreviewCardPanel();
          }));
          ++num;
        }
      }
    }
  }

  private void LoadAdventurePreviewCard(
    string previewCard,
    GeneralStoreAdventureContent.DelOnAdventurePreviewCardLoaded onLoadComplete)
  {
    Actor previewCard1;
    if (this.m_loadedPreviewCards.TryGetValue(previewCard, out previewCard1))
      onLoadComplete(previewCard1);
    else
      DefLoader.Get().LoadFullDef(previewCard, (DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>) ((cardID, fullDef, data) => AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(fullDef.EntityDef), (PrefabCallback<GameObject>) ((actorName, actorObject, data2) =>
      {
        using (fullDef)
        {
          if ((UnityEngine.Object) actorObject == (UnityEngine.Object) null)
          {
            Debug.LogWarning((object) string.Format("FAILED to load actor \"{0}\"", (object) actorName));
            onLoadComplete((Actor) null);
          }
          else
          {
            Actor component = actorObject.GetComponent<Actor>();
            if ((UnityEngine.Object) component == (UnityEngine.Object) null)
            {
              Debug.LogWarning((object) string.Format("ERROR actor \"{0}\" has no Actor component", (object) actorName));
              onLoadComplete((Actor) null);
            }
            else
            {
              component.SetFullDef(fullDef);
              component.UpdateAllComponents();
              LayerUtils.SetLayer(component.gameObject, this.gameObject.layer);
              component.Show();
              this.m_loadedPreviewCards.Add(previewCard, component);
              onLoadComplete(component);
            }
          }
        }
      }), options: AssetLoadingOptions.IgnorePrefabPosition)));
  }

  private void OnAdventuresHidePreviewCard(UIEvent e)
  {
    this.m_showPreviewCards = false;
    SoundManager.Get().LoadAndPlay((AssetReference) "card_shrink.prefab:a4e6170a9f153f94cacee42db7c327fb");
    this.HidePreviewCardPanel();
  }

  private void ShowPreviewCardPanel()
  {
    this.m_adventureCardPreviewPanel.SetActive(true);
    this.m_adventureCardPreviewPanel.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    iTween.StopByName(this.m_adventureCardPreviewPanel, "PreviewCardPanelScale");
    iTween.ScaleTo(this.m_adventureCardPreviewPanel, iTween.Hash((object) "scale", (object) Vector3.one, (object) "time", (object) 0.1f, (object) "name", (object) "PreviewCardPanelScale", (object) "easetype", (object) iTween.EaseType.linear));
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_parentStore.BlockInterface(true);
  }

  private void HidePreviewCardPanel()
  {
    iTween.StopByName(this.m_adventureCardPreviewPanel, "PreviewCardPanelScale");
    iTween.ScaleTo(this.m_adventureCardPreviewPanel, iTween.Hash((object) "scale", (object) new Vector3(0.02f, 0.02f, 0.02f), (object) "time", (object) 0.1f, (object) "name", (object) "PreviewCardPanelScale", (object) "oncomplete", (object) (Action<object>) (o =>
    {
      this.m_adventureCardPreviewPanel.SetActive(false);
      if (!(bool) UniversalInputManager.UsePhoneUI)
        return;
      this.m_parentStore.BlockInterface(false);
    }), (object) "easetype", (object) iTween.EaseType.linear));
  }

  private void UpdateRadioButtonText(Network.Bundle moneyBundle)
  {
    this.m_adventureRadioButton.SetSelected(true);
    if ((Record) moneyBundle == (Record) null)
    {
      this.m_adventureRadioButtonText.Text = GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_TEXT_PURCHASED");
      this.m_adventureRadioButtonText.Anchor = UberText.AnchorOptions.Middle;
      this.m_adventureRadioButtonCostText.Text = string.Empty;
    }
    else
    {
      string key;
      if (this.IsPreOrder())
      {
        AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) this.m_selectedAdventureId);
        key = record == null || string.IsNullOrEmpty((string) record.StorePreorderRadioText) ? "GLUE_STORE_DUNGEON_BUTTON_PREORDER_TEXT" : (string) record.StorePreorderRadioText;
      }
      else
        key = "GLUE_STORE_DUNGEON_BUTTON_TEXT";
      this.m_adventureRadioButtonText.Text = GameStrings.Get(key);
      this.m_adventureRadioButtonText.Anchor = UberText.AnchorOptions.Upper;
      string str = StoreManager.Get().FormatCostBundle(moneyBundle);
      this.m_adventureRadioButtonCostText.Text = GameStrings.Format("GLUE_STORE_DUNGEON_BUTTON_COST_TEXT", (object) StoreManager.Get().GetWingItemCount(moneyBundle.Items), (object) str);
    }
    if (!((UnityEngine.Object) this.m_adventureOwnedCheckmark != (UnityEngine.Object) null))
      return;
    this.m_adventureOwnedCheckmark.SetActive((Record) moneyBundle == (Record) null);
  }

  private void UpdateAdventureDescription(Network.Bundle bundle)
  {
    if (this.m_selectedAdventureId != AdventureDbId.INVALID)
    {
      string title = string.Empty;
      string desc = string.Empty;
      string empty = string.Empty;
      AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) this.m_selectedAdventureId);
      if (record == null)
        Debug.LogError((object) string.Format("Unable to find adventure record ID: {0}", (object) this.m_selectedAdventureId));
      else if ((Record) bundle == (Record) null)
      {
        title = (string) record.StoreOwnedHeadline;
        desc = (string) record.StoreOwnedDesc;
      }
      else if (this.IsPreOrder())
      {
        title = (string) record.StorePreorderHeadline;
        int wingItemCount = StoreManager.Get().GetWingItemCount(bundle.Items);
        desc = !(record.GetVar(string.Format("STORE_PREORDER_WINGS_{0}_DESC", (object) wingItemCount)) is DbfLocValue var) ? "" : var.GetString();
      }
      else
      {
        int wingItemCount = StoreManager.Get().GetWingItemCount(bundle.Items);
        DbfLocValue var1 = record.GetVar(string.Format("STORE_BUY_WINGS_{0}_HEADLINE", (object) wingItemCount)) as DbfLocValue;
        DbfLocValue var2 = record.GetVar(string.Format("STORE_BUY_WINGS_{0}_DESC", (object) wingItemCount)) as DbfLocValue;
        title = var1 == null ? "" : var1.GetString();
        desc = var2 == null ? "" : var2.GetString();
      }
      if (StoreManager.Get().IsKoreanCustomer())
        empty = GameStrings.Get("GLUE_STORE_KOREAN_PRODUCT_DETAILS_ADVENTURE");
      if ((UnityEngine.Object) this.m_adventureCardPreviewText != (UnityEngine.Object) null)
        this.m_adventureCardPreviewText.Text = (string) record.StorePreviewRewardsText;
      this.m_parentStore.SetDescription(title, desc, empty);
      StoreAdventureDef storeAdventureDef = this.GetStoreAdventureDef((int) this.m_selectedAdventureId);
      if (!((UnityEngine.Object) storeAdventureDef != (UnityEngine.Object) null))
        return;
      using (AssetHandle<Texture> texture = AssetLoader.Get().LoadAsset<Texture>((AssetReference) storeAdventureDef.m_accentTextureName))
        this.m_parentStore.SetAccentTexture(texture);
    }
    else
    {
      this.m_parentStore.HideAccentTexture();
      this.m_parentStore.SetChooseDescription(GameStrings.Get("GLUE_STORE_CHOOSE_ADVENTURE"));
    }
  }

  private void UpdateAdventureTypeMusic()
  {
    if (this.m_parentStore.GetMode() == GeneralStoreMode.NONE)
      return;
    StoreAdventureDef storeAdventureDef = this.GetStoreAdventureDef((int) this.m_selectedAdventureId);
    if (!((UnityEngine.Object) storeAdventureDef == (UnityEngine.Object) null) && storeAdventureDef.m_playlist != MusicPlaylistType.Invalid && MusicManager.Get().StartPlaylist(storeAdventureDef.m_playlist))
      return;
    this.m_parentStore.ResumePreviousMusicPlaylist();
  }

  private void AnimateAndUpdateDisplay(int id, bool forceImmediate)
  {
    if ((UnityEngine.Object) this.m_preorderCardBackReward != (UnityEngine.Object) null)
      this.m_preorderCardBackReward.HideCardBackReward();
    GameObject currDisplay = (GameObject) null;
    if (this.m_currentDisplay == -1)
    {
      this.m_currentDisplay = 1;
      currDisplay = this.m_adventureEmptyDisplay;
    }
    else
      currDisplay = this.GetCurrentDisplayContainer();
    GameObject displayContainer = this.GetNextDisplayContainer();
    this.m_currentDisplay = (this.m_currentDisplay + 1) % 2;
    displayContainer.SetActive(true);
    if (!forceImmediate)
    {
      currDisplay.transform.localRotation = Quaternion.identity;
      displayContainer.transform.localEulerAngles = new Vector3(180f, 0.0f, 0.0f);
      iTween.StopByName(currDisplay, "ROTATION_TWEEN");
      iTween.StopByName(displayContainer, "ROTATION_TWEEN");
      iTween.RotateBy(currDisplay, iTween.Hash((object) "amount", (object) new Vector3(0.5f, 0.0f, 0.0f), (object) "time", (object) 0.5f, (object) "name", (object) "ROTATION_TWEEN", (object) "oncomplete", (object) (Action<object>) (o => currDisplay.SetActive(false))));
      iTween.RotateBy(displayContainer, iTween.Hash((object) "amount", (object) new Vector3(0.5f, 0.0f, 0.0f), (object) "time", (object) 0.5f, (object) "name", (object) "ROTATION_TWEEN"));
      if (!string.IsNullOrEmpty(this.m_backgroundFlipSound))
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_backgroundFlipSound);
    }
    else
    {
      displayContainer.transform.localRotation = Quaternion.identity;
      currDisplay.transform.localEulerAngles = new Vector3(180f, 0.0f, 0.0f);
      currDisplay.SetActive(false);
    }
    AdventureDbfRecord record = GameDbf.Adventure.GetRecord(id);
    bool preorder = this.IsPreOrder();
    StoreAdventureDef storeAdventureDef = this.GetStoreAdventureDef(id);
    GeneralStoreAdventureContentDisplay currentDisplay = this.GetCurrentDisplay();
    currentDisplay.UpdateAdventureType(storeAdventureDef, record);
    currentDisplay.SetPreOrder(preorder);
    if (!((UnityEngine.Object) this.m_preorderCardBackReward != (UnityEngine.Object) null & preorder))
      return;
    this.m_preorderCardBackReward.SetCardBack(storeAdventureDef.m_preorderCardBackId);
    this.m_preorderCardBackReward.SetPreorderText(storeAdventureDef.m_preorderCardBackTextName);
    this.m_preorderCardBackReward.ShowCardBackReward();
  }

  private void AnimateAdventureRadioButtonBar()
  {
    if ((UnityEngine.Object) this.m_adventureRadioButtonContainer == (UnityEngine.Object) null)
      return;
    this.m_adventureRadioButtonContainer.SetActive(false);
    if (this.m_selectedAdventureId == AdventureDbId.INVALID)
      return;
    iTween.Stop(this.m_adventureRadioButtonContainer);
    this.m_adventureRadioButtonContainer.transform.localRotation = Quaternion.identity;
    this.m_adventureRadioButtonContainer.SetActive(true);
    this.m_adventureRadioButton.SetSelected(true);
    iTween.RotateBy(this.m_adventureRadioButtonContainer, iTween.Hash((object) "amount", (object) new Vector3(-1f, 0.0f, 0.0f), (object) "time", (object) this.m_backgroundFlipAnimTime, (object) "delay", (object) (1f / 1000f)));
  }

  private void OnAdventureProgressUpdated(
    bool isStartupAction,
    AdventureMission.WingProgress oldProgress,
    AdventureMission.WingProgress newProgress,
    object userData)
  {
    if (newProgress == null || oldProgress != null && oldProgress.IsOwned() || !newProgress.IsOwned())
      return;
    WingDbfRecord record = GameDbf.Wing.GetRecord(newProgress.Wing);
    if (record == null || (AdventureDbId) record.AdventureId != this.m_selectedAdventureId)
      return;
    Network.Bundle bundle = (Network.Bundle) null;
    StoreManager.Get().GetAvailableAdventureBundle(this.m_selectedAdventureId, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
    this.SetCurrentMoneyBundle(bundle);
    if (!((UnityEngine.Object) this.m_parentStore != (UnityEngine.Object) null))
      return;
    this.m_parentStore.RefreshContent();
  }

  private bool IsPreOrder()
  {
    Network.Bundle currentMoneyBundle = this.GetCurrentMoneyBundle();
    return (Record) currentMoneyBundle != (Record) null && StoreManager.Get().IsProductPrePurchase(currentMoneyBundle);
  }

  public delegate void DelOnAdventurePreviewCardLoaded(Actor previewCard);
}
