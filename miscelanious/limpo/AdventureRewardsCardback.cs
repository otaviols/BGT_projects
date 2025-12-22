using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

[CustomEditClass]
public class AdventureRewardsCardback : MonoBehaviour
{
  public GeneralStoreAdventureContent m_parentContent;
  public GameObject m_cardBackContainer;
  public Animation m_cardBackAppearAnimation;
  public UberText m_cardBackText;
  public string m_cardBackAppearAnimationName;
  public float m_cardBackAppearDelay = 0.5f;
  public float m_cardBackAppearTime = 0.5f;
  public int m_cardBackId = -1;
  public float m_driftRadius = 0.1f;
  public float m_driftTime = 10f;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_cardBackAppearSound;
  private GameObject m_cardBackObject;
  private bool m_cardBackObjectLoading;
  private Vector3 m_cardBackTextOrigScale;

  public void ShowCardBackReward()
  {
    this.HideCardBackReward();
    if ((Object) this.m_cardBackAppearAnimation == (Object) null || string.IsNullOrEmpty(this.m_cardBackAppearAnimationName) || !this.gameObject.activeInHierarchy)
      return;
    this.StopCoroutine("AnimateCardBackIn");
    this.StartCoroutine("AnimateCardBackIn");
  }

  public void HideCardBackReward()
  {
    if ((Object) this.m_cardBackObject != (Object) null)
      this.m_cardBackObject.SetActive(false);
    if (!((Object) this.m_cardBackText != (Object) null))
      return;
    this.m_cardBackText.gameObject.SetActive(false);
  }

  private void Awake()
  {
    if (Application.isPlaying)
      this.LoadCardBackWithId();
    this.m_cardBackTextOrigScale = this.m_cardBackText.transform.localScale;
  }

  private void LoadCardBackWithId()
  {
    if ((Object) this.m_cardBackObject != (Object) null)
      Object.Destroy((Object) this.m_cardBackObject);
    if (this.m_cardBackId < 0)
      Debug.LogError((object) "Card back ID must be a positive number");
    else
      this.m_cardBackObjectLoading = CardBackManager.Get().LoadCardBackByIndex(this.m_cardBackId, (CardBackManager.LoadCardBackData.LoadCardBackCallback) (cardBackData =>
      {
        GameObject gameObject = cardBackData.m_GameObject;
        gameObject.transform.parent = this.transform;
        gameObject.name = "CARD_BACK_" + (object) cardBackData.m_CardBackIndex;
        Actor component = gameObject.GetComponent<Actor>();
        if ((Object) component != (Object) null)
        {
          GameObject cardMesh = component.m_cardMesh;
          component.SetCardbackUpdateIgnore(true);
          component.SetUnlit();
          if ((Object) cardMesh != (Object) null)
          {
            Material material = cardMesh.GetComponent<Renderer>().GetMaterial();
            if (material.HasProperty("_SpecularIntensity"))
              material.SetFloat("_SpecularIntensity", 0.0f);
          }
        }
        this.m_cardBackObject = gameObject;
        LayerUtils.SetLayer(this.m_cardBackObject, this.m_cardBackContainer.gameObject.layer);
        GameUtils.SetParent(this.m_cardBackObject, this.m_cardBackContainer);
        this.m_cardBackObject.transform.localPosition = Vector3.zero;
        this.m_cardBackObject.transform.localScale = Vector3.one;
        this.m_cardBackObject.transform.localRotation = Quaternion.identity;
        AnimationUtil.FloatyPosition(this.m_cardBackContainer, this.m_driftRadius, this.m_driftTime);
        this.HideCardBackReward();
        this.m_cardBackObjectLoading = false;
      }));
  }

  private IEnumerator AnimateCardBackIn()
  {
    this.m_cardBackAppearAnimation.Stop(this.m_cardBackAppearAnimationName);
    this.m_cardBackAppearAnimation.Rewind(this.m_cardBackAppearAnimationName);
    yield return (object) new WaitForSeconds(this.m_cardBackAppearDelay);
    this.m_cardBackAppearAnimation.Play(this.m_cardBackAppearAnimationName);
    if (!string.IsNullOrEmpty(this.m_cardBackAppearSound))
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(this.m_cardBackAppearSound));
    yield return (object) new WaitForSeconds(this.m_cardBackAppearTime);
    while (this.m_cardBackObjectLoading)
      yield return (object) null;
    if ((Object) this.m_cardBackObject != (Object) null)
      this.m_cardBackObject.SetActive(true);
    this.m_cardBackText.gameObject.SetActive(true);
    this.m_cardBackText.transform.localScale = Vector3.one * 0.01f;
    iTween.ScaleTo(this.m_cardBackText.gameObject, iTween.Hash((object) "scale", (object) this.m_cardBackTextOrigScale, (object) "time", (object) this.m_cardBackAppearTime));
  }
}
