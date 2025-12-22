using System.Collections;
using UnityEngine;

[CustomEditClass]
public class AnimatedLowPolyPack : MonoBehaviour
{
  public Vector3 PUNCH_POSITION_AMOUNT = new Vector3(0.0f, 5f, 0.0f);
  public float PUNCH_POSITION_TIME = 0.25f;
  public ParticleSystem m_DustParticle;
  public FirstPurchaseBox m_FirstPurchaseBox;
  public GameObject m_AmountBanner;
  public UberText m_AmountBannerText;
  public GameObject m_amountFlash;
  public Animator m_amountFlashAnimController;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_FlyOutSound = "purchase_pack_lift_whoosh_1.prefab:5e1611f00212a1f43beb26b37be32eee";
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_FlyInSound = "purchase_pack_drop_impact_1.prefab:45f550c45ed7b5645a2d7f493df17489";
  public bool m_isLeavingSoonBanner;
  public GameObject m_shadow;
  private Vector3 m_flyInLocalAngles = Vector3.zero;
  private Vector3 m_flyOutLocalAngles = Vector3.zero;
  private Vector3 m_targetOffScreenLocalPos = Vector3.zero;
  private Vector3 m_targetLocalPos = Vector3.zero;
  private AnimatedLowPolyPack.State m_state;
  private int m_lastVisibleBannerCount;
  private bool m_amountBannerFlashing;
  private bool m_changeActivation = true;

  public int Column { get; private set; }

  public bool IsShowingShadow
  {
    get => (Object) this.m_shadow != (Object) null && this.m_shadow.activeSelf;
    set
    {
      if (!((Object) this.m_shadow != (Object) null) || value == this.m_shadow.activeSelf)
        return;
      this.m_shadow.SetActive(value);
    }
  }

  public void Init(
    int column,
    Vector3 targetLocalPos,
    Vector3 offScreenOffset,
    bool ignoreFullscreenEffects = true,
    bool changeActivation = true)
  {
    this.m_targetLocalPos = targetLocalPos;
    this.m_targetOffScreenLocalPos = targetLocalPos + offScreenOffset;
    this.m_changeActivation = changeActivation;
    this.Column = column;
    if (ignoreFullscreenEffects)
      LayerUtils.SetLayer(this.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.PositionOffScreen();
  }

  public void FlyInImmediate()
  {
    iTween.Stop(this.gameObject);
    this.transform.localEulerAngles = this.m_flyInLocalAngles;
    this.transform.localPosition = this.m_targetLocalPos;
    this.m_state = AnimatedLowPolyPack.State.FLOWN_IN;
    if (this.m_changeActivation)
      this.gameObject.SetActive(true);
    if (!((Object) this.m_FirstPurchaseBox != (Object) null))
      return;
    this.m_FirstPurchaseBox.RevealContents();
  }

  public bool FlyIn(float animTime, float delay)
  {
    if (this.m_state == AnimatedLowPolyPack.State.FLOWN_IN || this.m_state == AnimatedLowPolyPack.State.FLYING_IN)
      return false;
    this.m_state = AnimatedLowPolyPack.State.FLYING_IN;
    if (this.m_changeActivation)
      this.gameObject.SetActive(true);
    this.transform.localEulerAngles = this.m_flyInLocalAngles;
    if ((Object) this.m_FirstPurchaseBox != (Object) null)
      this.m_FirstPurchaseBox.Reset();
    Hashtable args = iTween.Hash((object) "position", (object) this.m_targetLocalPos, (object) "isLocal", (object) true, (object) "time", (object) animTime, (object) nameof (delay), (object) delay, (object) "easetype", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "OnFlownIn", (object) "oncompletetarget", (object) this.gameObject);
    iTween.Stop(this.gameObject);
    iTween.MoveTo(this.gameObject, args);
    return true;
  }

  public void FlyOutImmediate()
  {
    iTween.Stop(this.gameObject);
    this.transform.localEulerAngles = this.m_flyOutLocalAngles;
    this.transform.localPosition = this.m_targetOffScreenLocalPos;
    this.OnHidden();
  }

  public bool FlyOut(float animTime, float delay)
  {
    if (this.m_state == AnimatedLowPolyPack.State.HIDDEN || this.m_state == AnimatedLowPolyPack.State.FLYING_OUT)
      return false;
    this.m_state = AnimatedLowPolyPack.State.FLYING_OUT;
    this.transform.localEulerAngles = this.m_flyOutLocalAngles;
    Hashtable args = iTween.Hash((object) "position", (object) this.m_targetOffScreenLocalPos, (object) "isLocal", (object) true, (object) "time", (object) animTime, (object) nameof (delay), (object) delay, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "OnHidden", (object) "oncompletetarget", (object) this.gameObject);
    iTween.Stop(this.gameObject);
    iTween.MoveTo(this.gameObject, args);
    if (!string.IsNullOrEmpty(this.m_FlyOutSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_FlyOutSound);
    return true;
  }

  public void SetFlyingLocalRotations(Vector3 flyInLocalAngles, Vector3 flyOutLocalAngles)
  {
    this.m_flyInLocalAngles = flyInLocalAngles;
    this.m_flyOutLocalAngles = flyOutLocalAngles;
  }

  public AnimatedLowPolyPack.State GetState() => this.m_state;

  public void Hide() => this.OnHidden();

  public FirstPurchaseBox GetFirstPurchaseBox() => this.m_FirstPurchaseBox;

  public void UpdateBannerCount(int count) => this.StartCoroutine(this.UpdateBannerCountCoroutine(count));

  public void UpdateBannerCountImmediately(int count)
  {
    if (!((Object) this.m_AmountBanner != (Object) null) || !((Object) this.m_AmountBannerText != (Object) null))
      return;
    this.m_AmountBanner.SetActive(true);
    this.m_lastVisibleBannerCount = count;
    this.m_AmountBannerText.Text = this.m_lastVisibleBannerCount.ToString();
  }

  private IEnumerator UpdateBannerCountCoroutine(int count)
  {
    if ((Object) this.m_AmountBanner != (Object) null && (Object) this.m_AmountBannerText != (Object) null)
    {
      this.m_AmountBanner.SetActive(true);
      if (this.m_lastVisibleBannerCount != count)
      {
        if (this.m_amountBannerFlashing)
          this.m_AmountBannerText.Text = this.m_lastVisibleBannerCount.ToString();
        this.m_lastVisibleBannerCount = count;
        if ((Object) this.m_amountFlash != (Object) null && (Object) this.m_amountFlashAnimController != (Object) null)
        {
          this.m_amountFlash.SetActive(false);
          this.m_amountFlash.SetActive(true);
          this.m_amountFlashAnimController.enabled = true;
          this.m_amountFlashAnimController.StopPlayback();
          yield return (object) new WaitForEndOfFrame();
          if ((Object) this.m_amountFlashAnimController == (Object) null)
          {
            yield break;
          }
          else
          {
            this.m_amountFlashAnimController.Play("Flash");
            this.m_amountBannerFlashing = true;
            while ((Object) this.m_amountFlashAnimController != (Object) null && (double) this.m_amountFlashAnimController.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5)
              yield return (object) null;
          }
        }
        this.m_AmountBannerText.Text = count.ToString();
        this.m_amountBannerFlashing = false;
      }
    }
  }

  public void HideBanner()
  {
    if (!((Object) this.m_AmountBanner != (Object) null))
      return;
    this.m_AmountBanner.SetActive(false);
  }

  private void OnHidden()
  {
    this.m_state = AnimatedLowPolyPack.State.HIDDEN;
    this.StopCoroutine("UpdateBannerCount");
    if (!this.m_changeActivation)
      return;
    this.gameObject.SetActive(false);
  }

  private void OnFlownIn()
  {
    this.m_DustParticle.Play();
    this.m_state = AnimatedLowPolyPack.State.FLOWN_IN;
    iTween.PunchPosition(this.gameObject, this.PUNCH_POSITION_AMOUNT, this.PUNCH_POSITION_TIME);
    if (!string.IsNullOrEmpty(this.m_FlyInSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_FlyInSound);
    if (!((Object) this.m_FirstPurchaseBox != (Object) null))
      return;
    this.m_FirstPurchaseBox.RevealContents();
  }

  private void PositionOffScreen()
  {
    iTween.Stop(this.gameObject);
    this.transform.localPosition = this.m_targetOffScreenLocalPos;
    this.OnHidden();
  }

  public enum State
  {
    UNKNOWN,
    FLOWN_IN,
    FLYING_IN,
    FLYING_OUT,
    HIDDEN,
  }
}
