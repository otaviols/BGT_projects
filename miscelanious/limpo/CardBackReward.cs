using UnityEngine;

public class CardBackReward : Reward
{
  public GameObject m_cardbackBone;
  private int m_numCardBacksLoaded;

  protected override void InitData() => this.SetData((RewardData) new CardBackRewardData(), false);

  protected override void ShowReward(bool updateCacheValues)
  {
    if (!(this.Data is CardBackRewardData data))
    {
      Debug.LogWarning((object) string.Format("CardBackReward.ShowReward() - Data {0} is not CardBackRewardData", (object) this.Data));
    }
    else
    {
      if (!data.IsDummyReward & updateCacheValues)
      {
        CardBackManager.Get().AddNewCardBack(data.CardBackID);
        StoreManager.Get().Catalog.UpdateProductStatus();
      }
      this.m_root.SetActive(true);
      this.m_cardbackBone.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180f);
      iTween.RotateAdd(this.m_cardbackBone.gameObject, iTween.Hash((object) "amount", (object) new Vector3(0.0f, 0.0f, 540f), (object) "time", (object) 1.5f, (object) "easeType", (object) iTween.EaseType.easeOutElastic, (object) "space", (object) Space.Self));
    }
  }

  protected override void HideReward()
  {
    base.HideReward();
    this.m_root.SetActive(false);
  }

  protected override void OnDataSet(bool updateVisuals)
  {
    if (!updateVisuals)
      return;
    CardBackRewardData data = this.Data as CardBackRewardData;
    this.SetRewardText(GameStrings.Get("GLOBAL_REWARD_CARD_BACK_HEADLINE"), string.Empty, string.Empty);
    if (data == null)
    {
      Debug.LogWarning((object) string.Format("CardBackReward.OnDataSet() - Data {0} is not CardBackRewardData", (object) this.Data));
    }
    else
    {
      this.SetReady(false);
      CardBackManager.Get().LoadCardBackByIndex(data.CardBackID, new CardBackManager.LoadCardBackData.LoadCardBackCallback(this.OnFrontCardBackLoaded), true, "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", (object) null);
      CardBackManager.Get().LoadCardBackByIndex(data.CardBackID, new CardBackManager.LoadCardBackData.LoadCardBackCallback(this.OnBackCardBackLoaded), true, "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", (object) null);
    }
  }

  private void OnFrontCardBackLoaded(CardBackManager.LoadCardBackData cardbackData)
  {
    GameObject gameObject = cardbackData.m_GameObject;
    gameObject.transform.parent = this.m_cardbackBone.transform;
    gameObject.transform.localPosition = Vector3.zero;
    gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    gameObject.transform.localScale = Vector3.one;
    LayerUtils.SetLayer(gameObject, this.gameObject.layer);
    ++this.m_numCardBacksLoaded;
    if (2 != this.m_numCardBacksLoaded)
      return;
    this.SetReady(true);
  }

  private void OnBackCardBackLoaded(CardBackManager.LoadCardBackData cardbackData)
  {
    GameObject gameObject = cardbackData.m_GameObject;
    gameObject.transform.parent = this.m_cardbackBone.transform;
    gameObject.transform.localPosition = Vector3.zero;
    gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 180f));
    gameObject.transform.localScale = Vector3.one;
    LayerUtils.SetLayer(gameObject, this.gameObject.layer);
    ++this.m_numCardBacksLoaded;
    if (2 != this.m_numCardBacksLoaded)
      return;
    this.SetReady(true);
  }
}
