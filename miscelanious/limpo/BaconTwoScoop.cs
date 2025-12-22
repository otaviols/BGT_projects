using System.Collections;
using UnityEngine;

public class BaconTwoScoop : VictoryTwoScoop
{
  public GameObject m_Root;
  public GameObject m_RatingBanner;
  public GameObject m_Top1Visual;
  public GameObject m_Top4Visual;
  public GameObject m_Bottom4Visual;
  public AudioSource m_Top1Sound;
  public AudioSource m_Top4Sound;
  public AudioSource m_Bottom4Sound;
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

  protected override void ShowImpl() => this.StartCoroutine(this.ShowWhenReady());

  protected override string GetActorName() => "Card_Play_Bacon_Hero.prefab:227eb40f91281fa429c48c8a730c982f";

  private IEnumerator ShowWhenReady()
  {
    BaconTwoScoop baconTwoScoop = this;
    baconTwoScoop.m_Root.SetActive(false);
    baconTwoScoop.m_heroActor.gameObject.SetActive(false);
    while (GameState.Get() == null || GameState.Get().GetGameEntity() == null)
      yield return (object) null;
    TB_BaconShop baconGameEntity = (TB_BaconShop) null;
    if (GameState.Get().GetGameEntity() is TB_BaconShop)
      baconGameEntity = (TB_BaconShop) GameState.Get().GetGameEntity();
    bool ratingChangeDisabled = GameMgr.Get().IsFriendlyBattlegrounds();
    if (GameState.Get().GetBooleanGameOption(GameEntityOption.WAIT_FOR_RATING_INFO))
    {
      while (baconGameEntity != null && baconGameEntity.RatingChangeData == null && !ratingChangeDisabled && (double) baconTwoScoop.m_waitForRatingTimeoutTimer < 5.0)
      {
        baconTwoScoop.m_waitForRatingTimeoutTimer += Time.unscaledDeltaTime;
        yield return (object) null;
      }
    }
    baconTwoScoop.m_Root.SetActive(true);
    baconTwoScoop.m_heroActor.gameObject.SetActive(true);
    CustomHeroFrameBehaviour component = baconTwoScoop.m_heroActor.gameObject.GetComponent<CustomHeroFrameBehaviour>();
    if ((Object) component != (Object) null)
      component.UpdateFrame();
    baconTwoScoop.SetupHeroActor();
    baconTwoScoop.SetupBannerText();
    baconTwoScoop.SetupTwoScoopForPlace();
    if (GameMgr.Get().IsSpectator() || baconGameEntity == null || baconGameEntity.RatingChangeData == null)
    {
      baconTwoScoop.m_RatingBanner.SetActive(false);
    }
    else
    {
      baconTwoScoop.m_newRating = baconGameEntity.RatingChangeData.NewRating;
      baconTwoScoop.m_ratingChange = baconGameEntity.RatingChangeData.RatingChange;
      baconTwoScoop.m_RatingBanner.SetActive(true);
      yield return (object) baconTwoScoop.PlayRatingChangeAnimation();
    }
  }

  private void SetupTwoScoopForPlace()
  {
    this.m_Top1Visual.SetActive(false);
    this.m_Top4Visual.SetActive(false);
    this.m_Bottom4Visual.SetActive(false);
    int leaderboardPlace = GameState.Get().GetFriendlySidePlayer().GetHero().GetRealTimePlayerLeaderboardPlace();
    if (leaderboardPlace <= 1)
    {
      this.m_Top1Visual.SetActive(true);
      SoundManager.Get().Play(this.m_Top1Sound);
    }
    else if (leaderboardPlace <= 4)
    {
      this.m_Top4Visual.SetActive(true);
      SoundManager.Get().Play(this.m_Top4Sound);
    }
    else
    {
      this.m_Bottom4Visual.SetActive(true);
      SoundManager.Get().Play(this.m_Bottom4Sound);
    }
  }

  private IEnumerator PlayRatingChangeAnimation()
  {
    int oldRating = this.m_newRating - this.m_ratingChange;
    this.m_RatingChangeText.Text = "";
    this.m_RatingText.Text = oldRating.ToString();
    Animator ratingChangeAnimator = this.m_RatingChangeText.GetComponent<Animator>();
    ratingChangeAnimator.enabled = false;
    Animator ratingTextAnimator = this.m_RatingText.GetComponent<Animator>();
    ratingTextAnimator.enabled = false;
    yield return (object) new WaitForSeconds(this.m_DelayBeforeRatingChangeSeconds);
    this.m_RatingChangeText.Text = (this.m_ratingChange >= 0 ? "+" : "") + this.m_ratingChange.ToString();
    this.m_RatingChangeText.TextColor = this.m_ratingChange >= 0 ? this.m_RatingChangeTextColorPositive : this.m_RatingChangeTextColorNegative;
    ratingChangeAnimator.enabled = true;
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
