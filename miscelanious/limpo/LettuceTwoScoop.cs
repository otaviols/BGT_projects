using System.Collections;
using UnityEngine;

public class LettuceTwoScoop : EndGameTwoScoop
{
  public GameObject m_Root;
  public GameObject m_RatingBanner;
  public UberText m_RatingText;
  public UberText m_RatingChangeText;
  public Color m_RatingChangeTextColorPositive;
  public Color m_RatingChangeTextColorNegative;
  public float m_RatingTextUpdateTimeSeconds = 0.5f;
  public float m_DelayBeforeRatingChangeSeconds = 0.5f;
  private const float WAIT_FOR_RATING_TIMEOUT_SECONDS = 5f;
  private float m_waitForRatingTimeoutTimer;
  private int m_newRating;
  private int m_ratingChange;

  public override void Awake() => this.gameObject.SetActive(false);

  public override bool IsLoaded() => true;

  protected override void ShowImpl() => this.StartCoroutine(this.ShowWhenReady());

  public IEnumerator ShowWhenReady()
  {
    LettuceTwoScoop lettuceTwoScoop = this;
    lettuceTwoScoop.m_Root.SetActive(false);
    while (GameState.Get() == null || GameState.Get().GetGameEntity() == null)
      yield return (object) null;
    LettuceMissionEntity lettuceGameEntity = (LettuceMissionEntity) null;
    if (GameState.Get().GetGameEntity() is LettuceMissionEntity)
      lettuceGameEntity = (LettuceMissionEntity) GameState.Get().GetGameEntity();
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.WAIT_FOR_RATING_INFO))
    {
      while (lettuceGameEntity != null && lettuceGameEntity.RatingChangeData == null && (double) lettuceTwoScoop.m_waitForRatingTimeoutTimer < 5.0)
      {
        lettuceTwoScoop.m_waitForRatingTimeoutTimer += Time.unscaledDeltaTime;
        yield return (object) null;
      }
    }
    lettuceTwoScoop.m_Root.SetActive(true);
    lettuceTwoScoop.GetComponent<PlayMakerFSM>().SendEvent("Action");
    iTween.FadeTo(lettuceTwoScoop.gameObject, 1f, 0.25f);
    lettuceTwoScoop.gameObject.transform.localScale = new Vector3(EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL);
    Hashtable args1 = iTween.Hash((object) "scale", (object) new Vector3(EndGameTwoScoop.END_SCALE_VAL, EndGameTwoScoop.END_SCALE_VAL, EndGameTwoScoop.END_SCALE_VAL), (object) "time", (object) 0.5f, (object) "oncomplete", (object) "PunchEndGameTwoScoop", (object) "oncompletetarget", (object) lettuceTwoScoop.gameObject);
    iTween.ScaleTo(lettuceTwoScoop.gameObject, args1);
    Hashtable args2 = iTween.Hash((object) "position", (object) (lettuceTwoScoop.gameObject.transform.position + new Vector3(0.005f, 0.005f, 0.005f)), (object) "time", (object) 1.5f);
    iTween.MoveTo(lettuceTwoScoop.gameObject, args2);
    if (GameMgr.Get().IsSpectator() || lettuceGameEntity == null || lettuceGameEntity.RatingChangeData == null)
    {
      lettuceTwoScoop.m_RatingBanner.SetActive(false);
    }
    else
    {
      lettuceTwoScoop.m_newRating = lettuceGameEntity.RatingChangeData.PvpRating;
      lettuceTwoScoop.m_ratingChange = lettuceGameEntity.RatingChangeData.Delta;
      lettuceTwoScoop.m_RatingBanner.SetActive(true);
      yield return (object) lettuceTwoScoop.PlayRatingChangeAnimation();
    }
  }

  private IEnumerator PlayRatingChangeAnimation()
  {
    int oldRating = this.m_newRating - this.m_ratingChange;
    this.m_RatingChangeText.Text = "";
    this.m_RatingText.Text = oldRating.ToString();
    Animator ratingChangeTextAnimator = this.m_RatingChangeText.GetComponent<Animator>();
    ratingChangeTextAnimator.enabled = false;
    Animator ratingTextAnimator = this.m_RatingText.GetComponent<Animator>();
    ratingTextAnimator.enabled = false;
    yield return (object) new WaitForSeconds(this.m_DelayBeforeRatingChangeSeconds);
    this.m_RatingChangeText.Text = (this.m_ratingChange >= 0 ? "+" : "") + this.m_ratingChange.ToString();
    this.m_RatingChangeText.TextColor = this.m_ratingChange >= 0 ? this.m_RatingChangeTextColorPositive : this.m_RatingChangeTextColorNegative;
    ratingChangeTextAnimator.enabled = true;
    ratingTextAnimator.enabled = true;
    float timer = 0.0f;
    while ((double) timer < (double) this.m_RatingTextUpdateTimeSeconds)
    {
      float t = Mathf.Clamp01(timer / this.m_RatingTextUpdateTimeSeconds);
      this.m_RatingText.Text = Mathf.FloorToInt(Mathf.Lerp((float) oldRating, (float) this.m_newRating, t)).ToString();
      timer += Time.deltaTime;
      yield return (object) null;
    }
    this.m_RatingText.Text = this.m_newRating.ToString();
  }
}
