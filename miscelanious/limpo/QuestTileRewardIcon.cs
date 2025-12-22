using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class QuestTileRewardIcon : MonoBehaviour
{
  public UberText m_amountText;
  public NestedPrefab m_goldDoubledFX;
  public NestedPrefab m_goldDoubledFXPhone;
  private RewardData m_rewardData;
  private AssetHandle<Texture> m_loadedTexture;

  private NestedPrefab GoldDoubledFX => !(bool) UniversalInputManager.UsePhoneUI ? this.m_goldDoubledFX : this.m_goldDoubledFXPhone;

  private void OnDestroy() => AssetHandle.SafeDispose<Texture>(ref this.m_loadedTexture);

  public void InitWithRewardData(RewardData rewardData, bool isDoubleGoldEnabled, int renderQueue)
  {
    this.m_rewardData = rewardData;
    RenderUtils.SetRenderQueue(this.gameObject, renderQueue);
    if (isDoubleGoldEnabled && (Object) this.GoldDoubledFX != (Object) null && this.m_rewardData.RewardType == Reward.Type.GOLD)
    {
      GameObject gameObject = this.GoldDoubledFX.PrefabGameObject(true);
      if ((Object) gameObject != (Object) null)
      {
        this.SetDoubleGoldActive(true);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localScale = Vector3.one;
      }
    }
    this.m_amountText.gameObject.SetActive(false);
    this.m_amountText.RenderQueue = renderQueue;
    float amountToScaleReward;
    RewardUtils.SetupRewardIcon(this.m_rewardData, this.GetComponent<Renderer>(), this.m_amountText, out amountToScaleReward, isDoubleGoldEnabled);
    this.SetAmountTextOffset();
    this.transform.localScale *= amountToScaleReward;
  }

  public void InitWithIconParams(
    int renderQueue,
    AssetReference iconTextureAssetRef,
    Vector2 iconTextureSourceOffset,
    string amountText)
  {
    this.m_rewardData = (RewardData) new EventRewardData();
    RenderUtils.SetRenderQueue(this.gameObject, renderQueue);
    this.m_amountText.RenderQueue = renderQueue;
    Material tileMaterial = RendererExtension.GetMaterial(this.GetComponent<Renderer>());
    AssetHandleCallback<Texture> callback = (AssetHandleCallback<Texture>) ((assetRef, texture, loadTextureCbData) =>
    {
      AssetHandle.Take<Texture>(ref this.m_loadedTexture, texture);
      if (!((Object) tileMaterial != (Object) null))
        return;
      tileMaterial.mainTexture = (Texture) this.m_loadedTexture;
    });
    AssetLoader.Get().LoadAsset<Texture>(iconTextureAssetRef, callback);
    tileMaterial.mainTextureOffset = iconTextureSourceOffset;
    if (amountText != null)
    {
      this.m_amountText.Text = amountText;
      this.m_amountText.gameObject.SetActive(true);
    }
    else
      this.m_amountText.gameObject.SetActive(false);
  }

  public void OnClose() => this.SetDoubleGoldActive(false);

  public void OnQuestRerolled() => this.SetDoubleGoldActive(false);

  private void SetDoubleGoldActive(bool active)
  {
    if (!((Object) this.GoldDoubledFX != (Object) null))
      return;
    this.GoldDoubledFX.gameObject.SetActive(active);
  }

  private void SetAmountTextOffset()
  {
    switch (this.m_rewardData.RewardType)
    {
      case Reward.Type.ARCANE_DUST:
        TransformUtil.SetLocalPosX((Component) this.m_amountText, this.m_amountText.transform.localPosition.z + 0.15f);
        TransformUtil.SetLocalPosZ((Component) this.m_amountText, this.m_amountText.transform.localPosition.z + 0.7f);
        break;
      case Reward.Type.GOLD:
        TransformUtil.SetLocalPosZ((Component) this.m_amountText, this.m_amountText.transform.localPosition.z + 0.7f);
        break;
    }
  }
}
