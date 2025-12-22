using UnityEngine;

public class RewardCardBack : MonoBehaviour
{
  public GameObject m_cardbackBone;
  public UberText m_cardbackTitle;
  public UberText m_cardbackName;
  public int m_CardBackID = -1;
  private bool m_Ready;
  private Actor m_actor;
  private GameLayer m_layer = GameLayer.IgnoreFullScreenEffects;

  private void OnDestroy() => this.m_Ready = false;

  public bool IsReady() => this.m_Ready;

  public void LoadCardBack(CardBackRewardData cardbackData, GameLayer layer = GameLayer.IgnoreFullScreenEffects)
  {
    this.m_layer = layer;
    this.m_CardBackID = cardbackData.CardBackID;
    CardBackManager.Get().LoadCardBackByIndex(this.m_CardBackID, new CardBackManager.LoadCardBackData.LoadCardBackCallback(this.OnCardBackLoaded));
  }

  public void Death() => this.m_actor.ActivateSpellBirthState(SpellType.DEATH);

  private void OnCardBackLoaded(CardBackManager.LoadCardBackData cardbackData)
  {
    GameObject gameObject = cardbackData.m_GameObject;
    gameObject.transform.parent = this.m_cardbackBone.transform;
    gameObject.transform.localPosition = Vector3.zero;
    gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    gameObject.transform.localScale = Vector3.one;
    LayerUtils.SetLayer(gameObject, this.m_layer);
    this.m_actor = gameObject.GetComponent<Actor>();
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_cardbackTitle.Text = "GLOBAL_SEASON_END_NEW_CARDBACK_TITLE_PHONE";
    this.m_cardbackName.Text = cardbackData.m_Name;
    this.m_Ready = true;
  }
}
