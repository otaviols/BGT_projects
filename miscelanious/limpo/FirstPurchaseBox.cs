using System;
using System.Collections;
using UnityEngine;

public class FirstPurchaseBox : MonoBehaviour
{
  public GameObject m_BoxBase;
  public GameObject m_BoxLid;
  public GameObject m_CardRootBone;
  public AnimationClip m_RevealCardAnimation;
  public AnimationClip m_GlowOutAnimation;
  private string m_CardId;
  private Actor m_CardActor;
  private PlayMakerFSM m_fsm;
  private PegUIElement m_cardUIElement;
  private GameObject m_InputBlockerPerspectiveUI;
  private GameObject m_InputBlockerCameraMask;

  private void Awake() => this.m_fsm = this.GetComponent<PlayMakerFSM>();

  public void Reset()
  {
    if ((UnityEngine.Object) this.m_BoxLid == (UnityEngine.Object) null)
    {
      this.m_BoxBase.SetActive(true);
    }
    else
    {
      this.m_BoxBase.SetActive(false);
      this.m_BoxLid.SetActive(true);
    }
    if (!((UnityEngine.Object) this.m_CardRootBone != (UnityEngine.Object) null))
      return;
    this.m_CardRootBone.SetActive(false);
    RenderUtils.EnableColliders(this.m_CardRootBone, false);
  }

  public void RevealContents()
  {
    this.m_BoxBase.SetActive(true);
    if (!((UnityEngine.Object) this.m_fsm != (UnityEngine.Object) null))
      return;
    this.m_fsm.SendEvent("Action");
  }

  [ContextMenu("Fake Purchase")]
  public void FakePurchase() => this.PurchaseBundle("NEW1_030");

  public void PurchaseBundle(string cardID)
  {
    if (string.IsNullOrEmpty(cardID))
    {
      Debug.LogWarningFormat("PurchaseBundle() - CardID is empty");
    }
    else
    {
      if ((UnityEngine.Object) this.m_BoxLid != (UnityEngine.Object) null)
        this.m_BoxLid.SetActive(false);
      this.m_BoxBase.SetActive(true);
      if (!((UnityEngine.Object) this.m_CardRootBone != (UnityEngine.Object) null))
        return;
      GameObject userData = AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(TAG_CARDTYPE.MINION), AssetLoadingOptions.IgnorePrefabPosition);
      userData.transform.parent = this.m_CardRootBone.transform;
      userData.transform.localPosition = Vector3.zero;
      userData.transform.localScale = Vector3.one;
      userData.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
      this.m_CardActor = userData.GetComponent<Actor>();
      DefLoader.Get().LoadFullDef(cardID, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnCardDefLoaded), (object) userData, new CardPortraitQuality(3, TAG_PREMIUM.GOLDEN));
    }
  }

  private void OnCardDefLoaded(string cardID, DefLoader.DisposableFullDef def, object callbackData)
  {
    using (def)
    {
      if (def == null)
      {
        Debug.LogWarningFormat("OnCardDefLoaded() - def for CardID {0} is null", (object) cardID);
      }
      else
      {
        this.m_CardId = cardID;
        GameObject go = (GameObject) callbackData;
        this.m_CardActor.gameObject.SetActive(true);
        this.m_CardActor.SetFullDef(def);
        this.m_CardActor.SetPremium(TAG_PREMIUM.NORMAL);
        this.m_CardActor.UpdateAllComponents();
        int layer = this.m_CardRootBone.layer;
        int? ignoredLayer = new int?();
        LayerUtils.SetLayer(go, layer, ignoredLayer);
        if ((UnityEngine.Object) this.m_fsm != (UnityEngine.Object) null)
          this.m_fsm.SendEvent("Birth");
        this.StartCoroutine(this.RevealCardAndSetupWaitForClick());
      }
    }
  }

  private IEnumerator RevealCardAndSetupWaitForClick()
  {
    FirstPurchaseBox firstPurchaseBox = this;
    if ((UnityEngine.Object) firstPurchaseBox.m_CardRootBone != (UnityEngine.Object) null)
      RenderUtils.EnableColliders(firstPurchaseBox.m_CardRootBone, true);
    firstPurchaseBox.m_cardUIElement = firstPurchaseBox.m_CardRootBone.GetComponent<PegUIElement>();
    if ((UnityEngine.Object) firstPurchaseBox.m_cardUIElement == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "SetupWaitForClick: PegUIElement missing!");
    }
    else
    {
      Camera firstByLayer = CameraUtils.FindFirstByLayer(GameLayer.PerspectiveUI);
      firstPurchaseBox.m_InputBlockerPerspectiveUI = CameraUtils.CreateInputBlocker(firstByLayer, "FirstPurchaseBoxInputBlocker", (Component) firstPurchaseBox.transform);
      firstPurchaseBox.m_InputBlockerPerspectiveUI.AddComponent<PegUIElement>();
      firstPurchaseBox.m_InputBlockerPerspectiveUI.layer = firstPurchaseBox.gameObject.layer;
      Vector3 localPosition = firstPurchaseBox.m_InputBlockerPerspectiveUI.transform.localPosition;
      firstPurchaseBox.m_InputBlockerPerspectiveUI.transform.localPosition = new Vector3(localPosition.x, 10f, localPosition.z);
      firstPurchaseBox.m_InputBlockerCameraMask = UnityEngine.Object.Instantiate<GameObject>(firstPurchaseBox.m_InputBlockerPerspectiveUI);
      LayerUtils.SetLayer(firstPurchaseBox.m_InputBlockerCameraMask, GameLayer.CameraMask);
      firstPurchaseBox.m_InputBlockerCameraMask.transform.parent = firstPurchaseBox.m_InputBlockerPerspectiveUI.transform;
      firstPurchaseBox.m_InputBlockerCameraMask.transform.localPosition = Vector3.zero;
      firstPurchaseBox.m_InputBlockerCameraMask.transform.localRotation = Quaternion.identity;
      firstPurchaseBox.m_InputBlockerCameraMask.transform.localScale = Vector3.one;
      yield return (object) new WaitForSeconds(firstPurchaseBox.m_RevealCardAnimation.length / 2f);
      TAG_CLASS cardClass = DefLoader.Get().GetEntityDef(firstPurchaseBox.m_CardId).GetClass();
      NotificationManager.Get().PlayBundleInnkeeperLineForClass(cardClass);
      UnityEngine.Object.Destroy((UnityEngine.Object) firstPurchaseBox.m_InputBlockerPerspectiveUI);
      if ((UnityEngine.Object) firstPurchaseBox.m_cardUIElement != (UnityEngine.Object) null)
        firstPurchaseBox.m_cardUIElement.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(firstPurchaseBox.OnCardClicked));
    }
  }

  private void PlayInnkeeperLineForClass(TAG_CLASS cardClass)
  {
    bool usePhoneUi = (bool) UniversalInputManager.UsePhoneUI;
    string empty = string.Empty;
    string soundPath = string.Empty;
    switch (cardClass)
    {
      case TAG_CLASS.DRUID:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_DRUID");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryDruid_01.prefab:2c4672cdfe2a96a45a7ac4f29c17d5b7";
        break;
      case TAG_CLASS.HUNTER:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_HUNTER");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryHunter_01.prefab:77302a32e0268f845a97992117241577";
        break;
      case TAG_CLASS.MAGE:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_MAGE");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryMage_01.prefab:2059ede4ae6efab489ecb4240a08d5bb";
        break;
      case TAG_CLASS.PALADIN:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_PALADIN");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryPaladin_01.prefab:21b7870188f66714b9707961d833b26a";
        break;
      case TAG_CLASS.PRIEST:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_PRIEST");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryPriest_01.prefab:fe9cd14401fd7f14f80950fb99864ce7";
        break;
      case TAG_CLASS.ROGUE:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_ROGUE");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryRogue_01.prefab:aa4c71ab99a240a4885e4a8d034adb1b";
        break;
      case TAG_CLASS.SHAMAN:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_SHAMAN");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryShaman_01.prefab:1101d9f890551164791f277babaa25d9";
        break;
      case TAG_CLASS.WARLOCK:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_WARLOCK");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryWarlock_01.prefab:5eaf5c883b0310e4d91bcfd3debc6eff";
        break;
      case TAG_CLASS.WARRIOR:
        empty = GameStrings.Get("GLUE_INKEEPER_RANDOM_CARD_DECK_RECIPE_WARRIOR");
        soundPath = "VO_INKEEPER_Male_Dwarf_ClassLegendaryWarrior_01.prefab:41b4581beb2dae945843ed164a6ec710";
        break;
    }
    NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, empty, soundPath, (Action<int>) null, usePhoneUi);
  }

  private void OnCardClicked(UIEvent e) => this.OnCardClicked();

  private void OnCardClicked()
  {
    if ((UnityEngine.Object) this.m_fsm != (UnityEngine.Object) null)
      this.m_fsm.SendEvent("Death");
    if ((UnityEngine.Object) this.m_CardRootBone != (UnityEngine.Object) null)
      RenderUtils.EnableColliders(this.m_CardRootBone, false);
    if ((UnityEngine.Object) this.m_cardUIElement != (UnityEngine.Object) null)
      this.m_cardUIElement.RemoveEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnCardClicked));
    this.ReturnToStore();
  }

  private void ReturnToStore() => ((GeneralStorePacksPane) ((GeneralStore) StoreManager.Get().GetCurrentStore()).GetCurrentPane()).RemoveFirstPurchaseBundle(this.m_GlowOutAnimation.length);
}
