using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class GeneralStoreAdventureContentDisplay : MonoBehaviour
{
  public PegUIElement m_rewardChest;
  public GameObject m_rewardsFrame;
  public GameObject m_preorderFrame;
  public GameObject m_leavingSoonBanner;
  public UIBButton m_leavingSoonButton;
  public MeshRenderer m_logo;
  public MeshRenderer m_keyArt;
  private string m_leavingSoonInfoText;
  private AssetHandle<Texture> m_logoTexture;

  private void Awake()
  {
    if (!((Object) this.m_leavingSoonButton != (Object) null))
      return;
    this.m_leavingSoonButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnLeavingSoonButtonClicked()));
  }

  private void OnDestroy() => AssetHandle.SafeDispose<Texture>(ref this.m_logoTexture);

  public void UpdateAdventureType(StoreAdventureDef advDef, AdventureDbfRecord advRecord)
  {
    if ((Object) advDef == (Object) null)
      return;
    AssetLoader.Get().LoadAsset<Texture>((AssetReference) advDef.m_logoTextureName, (AssetHandleCallback<Texture>) ((assetRef, loadedTexture, data) =>
    {
      if (!(bool) loadedTexture)
      {
        Debug.LogError((object) string.Format("Failed to load texture {0}!", (object) assetRef));
      }
      else
      {
        AssetHandle.Take<Texture>(ref this.m_logoTexture, loadedTexture);
        RendererExtension.GetMaterial((Renderer) this.m_logo).mainTexture = (Texture) this.m_logoTexture;
      }
    }));
    RendererExtension.SetMaterial((Renderer) this.m_keyArt, advDef.m_keyArt);
    if (!((Object) this.m_leavingSoonBanner != (Object) null))
      return;
    this.m_leavingSoonBanner.SetActive(advRecord.LeavingSoon);
    if (!advRecord.LeavingSoon)
      return;
    this.m_leavingSoonInfoText = (string) advRecord.LeavingSoonText;
  }

  public void SetPreOrder(bool preorder)
  {
    if ((Object) this.m_rewardChest != (Object) null && !(bool) UniversalInputManager.UsePhoneUI)
      this.m_rewardChest.gameObject.SetActive(!preorder);
    if ((Object) this.m_rewardsFrame != (Object) null)
      this.m_rewardsFrame.SetActive(!preorder);
    if (!((Object) this.m_preorderFrame != (Object) null))
      return;
    this.m_preorderFrame.SetActive(preorder);
  }

  private void OnLeavingSoonButtonClicked() => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLUE_STORE_ADVENTURE_LEAVING_SOON"),
    m_text = this.m_leavingSoonInfoText,
    m_showAlertIcon = true,
    m_responseDisplay = AlertPopup.ResponseDisplay.OK
  });
}
