using System.Collections;
using UnityEngine;

public class AdventureClassChallengeChestButton : PegUIElement
{
  public GameObject m_RootObject;
  public Transform m_UpBone;
  public Transform m_DownBone;
  public GameObject m_HighlightPlane;
  public GameObject m_RewardBone;
  public GameObject m_RewardCard;
  public bool m_IsRewardLoading;
  private ScreenEffectsHandle m_screenEffectsHandle;

  protected override void Awake()
  {
    base.Awake();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_hero_mouse_over.prefab:653cc8000b988cd468d2210a209adce6", this.gameObject);
    this.ShowHighlight(true);
    this.StartCoroutine(this.ShowRewardCard());
  }

  protected override void OnOut(PegUIElement.InteractionState oldState)
  {
    this.ShowHighlight(false);
    this.HideRewardCard();
  }

  public void Press()
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_hero_mouse_over.prefab:653cc8000b988cd468d2210a209adce6", this.gameObject);
    this.Depress();
    this.ShowHighlight(true);
    this.StartCoroutine(this.ShowRewardCard());
  }

  public void Release()
  {
    this.Raise();
    this.ShowHighlight(false);
    this.HideRewardCard();
  }

  private void Raise() => iTween.MoveTo(this.m_RootObject, iTween.Hash((object) "position", (object) this.m_UpBone.localPosition, (object) "time", (object) 0.1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));

  private void Depress() => iTween.MoveTo(this.m_RootObject, iTween.Hash((object) "position", (object) this.m_DownBone.localPosition, (object) "time", (object) 0.1f, (object) "easeType", (object) iTween.EaseType.linear, (object) "isLocal", (object) true));

  private void ShowHighlight(bool show) => this.m_HighlightPlane.GetComponent<Renderer>().enabled = show;

  private IEnumerator ShowRewardCard()
  {
    AdventureClassChallengeChestButton challengeChestButton = this;
    while (challengeChestButton.m_IsRewardLoading)
      yield return (object) null;
    LayerUtils.SetLayer(challengeChestButton.gameObject, GameLayer.IgnoreFullScreenEffects);
    ScreenEffectParameters vignettePerspective = ScreenEffectParameters.BlurVignettePerspective;
    challengeChestButton.m_screenEffectsHandle.StartEffect(vignettePerspective);
    challengeChestButton.m_RewardBone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    iTween.ScaleTo(challengeChestButton.m_RewardBone, new Vector3(10f, 10f, 10f), 0.2f);
    challengeChestButton.m_RewardCard.SetActive(true);
  }

  private void HideRewardCard()
  {
    iTween.ScaleTo(this.m_RewardBone, new Vector3(0.1f, 0.1f, 0.1f), 0.2f);
    this.m_screenEffectsHandle.StopEffect();
  }

  private void EffectFadeOutFinished()
  {
    if ((Object) this == (Object) null)
      return;
    LayerUtils.SetLayer(this.gameObject, GameLayer.Default);
    if (!((Object) this.m_RewardCard != (Object) null))
      return;
    this.m_RewardCard.SetActive(false);
  }
}
