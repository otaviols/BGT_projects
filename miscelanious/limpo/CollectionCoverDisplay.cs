using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public class CollectionCoverDisplay : PegUIElement
{
  public GameObject m_bookCoverLatch;
  public GameObject m_bookCoverLatchJoint;
  public GameObject m_bookCover;
  public Material m_latchFadeMaterial;
  public Material m_latchOpaqueMaterial;
  private readonly string CRACK_LATCH_OPEN_ANIM_COROUTINE = "AnimateLatchCrackOpen";
  private readonly string LATCH_OPEN_ANIM_NAME = "CollectionManagerCoverV2_Lock_edit";
  private readonly float LATCH_OPEN_ANIM_SPEED = 4f;
  private readonly float LATCH_FADE_TIME = 0.1f;
  private readonly float LATCH_FADE_DELAY = 0.15f;
  private readonly float BOOK_COVER_FULLY_CLOSED_Z_ROTATION;
  private readonly float BOOK_COVER_FULLY_OPEN_Z_ROTATION = 280f;
  private readonly float BOOK_COVER_FULL_ANIM_TIME = 0.75f;
  private bool m_isAnimating;
  private BoxCollider m_boxCollider;

  protected override void Awake()
  {
    base.Awake();
    this.m_boxCollider = this.transform.GetComponent<BoxCollider>();
  }

  public bool IsAnimating() => this.m_isAnimating;

  public void Open(CollectionCoverDisplay.DelOnOpened callback)
  {
    if ((double) this.m_bookCover.transform.localEulerAngles.z == (double) this.BOOK_COVER_FULLY_OPEN_Z_ROTATION)
      return;
    this.EnableCollider(false);
    this.SetIsAnimating(true);
    this.AnimateLatchOpening();
    this.AnimateCoverOpening(callback);
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_book_open.prefab:e32dc00de806ee1478b67810b89947bb");
  }

  public void SetOpenState()
  {
    if (!this.m_bookCover.activeSelf)
      return;
    this.EnableCollider(false);
    this.SetIsAnimating(false);
    this.m_bookCover.SetActive(false);
    this.m_bookCoverLatchJoint.GetComponent<Renderer>().enabled = false;
  }

  public void Close()
  {
    this.m_bookCover.SetActive(true);
    if ((double) this.m_bookCover.transform.localEulerAngles.z == (double) this.BOOK_COVER_FULLY_CLOSED_Z_ROTATION)
      return;
    this.SetIsAnimating(true);
    this.AnimateCoverClosing();
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_book_close.prefab:872608cda202ca440aa60cd0918be9ad");
  }

  public void DisplayCover()
  {
    this.m_bookCover.SetActive(true);
    this.m_bookCoverLatch.SetActive(true);
  }

  private void SetIsAnimating(bool animating) => this.m_isAnimating = animating;

  private void EnableCollider(bool enabled)
  {
    this.SetEnabled(enabled);
    this.m_boxCollider.enabled = enabled;
  }

  private void AnimateLatchOpening()
  {
    Animation component = this.m_bookCoverLatch.GetComponent<Animation>();
    component[this.LATCH_OPEN_ANIM_NAME].speed = this.LATCH_OPEN_ANIM_SPEED;
    if (component.IsPlaying(this.LATCH_OPEN_ANIM_NAME))
    {
      this.StopCoroutine(this.CRACK_LATCH_OPEN_ANIM_COROUTINE);
    }
    else
    {
      component[this.LATCH_OPEN_ANIM_NAME].time = 0.0f;
      component.Play(this.LATCH_OPEN_ANIM_NAME);
    }
    iTween.FadeTo(this.m_bookCoverLatchJoint, iTween.Hash((object) "amount", (object) 0, (object) "delay", (object) this.LATCH_FADE_DELAY, (object) "time", (object) this.LATCH_FADE_TIME, (object) "easeType", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "OnLatchOpened", (object) "oncompletetarget", (object) this.gameObject));
  }

  private void AnimateCoverOpening(CollectionCoverDisplay.DelOnOpened callback)
  {
    RendererExtension.SetMaterial(this.m_bookCoverLatchJoint.GetComponent<Renderer>(), this.m_latchFadeMaterial);
    Hashtable args = iTween.Hash((object) "rotation", (object) (this.m_bookCover.transform.localEulerAngles with
    {
      z = this.BOOK_COVER_FULLY_OPEN_Z_ROTATION
    }), (object) "isLocal", (object) true, (object) "time", (object) this.BOOK_COVER_FULL_ANIM_TIME, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "OnCoverOpened", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) callback, (object) "name", (object) "rotation");
    iTween.StopByName(this.m_bookCover.gameObject, "rotation");
    iTween.RotateTo(this.m_bookCover.gameObject, args);
  }

  private void AnimateCoverClosing()
  {
    Hashtable args = iTween.Hash((object) "rotation", (object) (this.m_bookCover.transform.localEulerAngles with
    {
      z = this.BOOK_COVER_FULLY_CLOSED_Z_ROTATION
    }), (object) "isLocal", (object) true, (object) "time", (object) this.BOOK_COVER_FULL_ANIM_TIME, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "AnimateLatchClosing", (object) "oncompletetarget", (object) this.gameObject, (object) "name", (object) "rotation");
    iTween.StopByName(this.m_bookCover.gameObject, "rotation");
    iTween.RotateTo(this.m_bookCover.gameObject, args);
  }

  private void AnimateLatchClosing()
  {
    Animation component1 = this.m_bookCoverLatch.GetComponent<Animation>();
    Renderer component2 = this.m_bookCoverLatchJoint.GetComponent<Renderer>();
    component2.enabled = true;
    RendererExtension.SetMaterial(component2, this.m_latchFadeMaterial);
    component1[this.LATCH_OPEN_ANIM_NAME].time = component1[this.LATCH_OPEN_ANIM_NAME].length;
    component1[this.LATCH_OPEN_ANIM_NAME].speed = (float) (-(double) this.LATCH_OPEN_ANIM_SPEED * 2.0);
    Hashtable args = iTween.Hash((object) "amount", (object) 1, (object) "time", (object) this.LATCH_FADE_TIME, (object) "easeType", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "OnLatchClosed", (object) "oncompletetarget", (object) this.gameObject);
    component1.Play(this.LATCH_OPEN_ANIM_NAME);
    iTween.FadeTo(this.m_bookCoverLatchJoint, args);
  }

  private void OnCoverOpened(CollectionCoverDisplay.DelOnOpened callback)
  {
    this.m_bookCover.SetActive(false);
    this.SetIsAnimating(false);
    if (callback == null)
      return;
    callback();
  }

  private void OnLatchOpened() => this.m_bookCoverLatchJoint.GetComponent<Renderer>().enabled = false;

  private void OnLatchClosed()
  {
    this.EnableCollider(true);
    this.SetIsAnimating(false);
  }

  private void CrackOpen()
  {
    if (this.IsAnimating())
      return;
    this.StopCoroutine(this.CRACK_LATCH_OPEN_ANIM_COROUTINE);
    this.StartCoroutine(this.CRACK_LATCH_OPEN_ANIM_COROUTINE);
  }

  private IEnumerator AnimateLatchCrackOpen()
  {
    Animation bookCoverLatchAnimation = this.m_bookCoverLatch.GetComponent<Animation>();
    RendererExtension.SetMaterial(this.m_bookCoverLatchJoint.GetComponent<Renderer>(), this.m_latchOpaqueMaterial);
    bookCoverLatchAnimation[this.LATCH_OPEN_ANIM_NAME].time = 0.0f;
    bookCoverLatchAnimation[this.LATCH_OPEN_ANIM_NAME].speed = this.LATCH_OPEN_ANIM_SPEED;
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_book_latch_jiggle.prefab:45ddcdb304889ac48b14478fc78991ba");
    bookCoverLatchAnimation.Play(this.LATCH_OPEN_ANIM_NAME);
    while ((double) bookCoverLatchAnimation[this.LATCH_OPEN_ANIM_NAME].time < 0.75)
      yield return (object) null;
    bookCoverLatchAnimation[this.LATCH_OPEN_ANIM_NAME].speed = 0.0f;
  }

  private void CrackClose()
  {
    if (this.IsAnimating())
      return;
    Animation component = this.m_bookCoverLatch.GetComponent<Animation>();
    if (!component.IsPlaying(this.LATCH_OPEN_ANIM_NAME))
      return;
    this.StopCoroutine(this.CRACK_LATCH_OPEN_ANIM_COROUTINE);
    component[this.LATCH_OPEN_ANIM_NAME].speed = -this.LATCH_OPEN_ANIM_SPEED;
  }

  public delegate void DelOnOpened();
}
