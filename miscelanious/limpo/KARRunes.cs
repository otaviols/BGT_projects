using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class KARRunes : MonoBehaviour
{
  public GameObject m_BookPageR;
  public GameObject m_BookPageL;
  public GameObject m_FloorRune;
  public Flipbook m_BookPageR_TurnedMesh;
  public Flipbook m_BookPageR_NotTurnedMesh;
  public Flipbook m_BookPageR_StaticMesh;
  public Flipbook m_BookPageL_StaticMesh;
  public Flipbook m_BookPageR_Glow;
  public Flipbook m_BookPageL_Glow;
  public Flipbook m_FloorPage;
  public Animator m_BookBooshAnim;
  public string m_BookBooshAnimState;
  public Animator m_BookGlowRAnim;
  public string m_BookGlowAnimRState;
  public Animator m_BookGlowLAnim;
  public string m_BookGlowAnimLState;
  public ParticleSystem m_FloorParticles;
  public Animator m_FloorGlowAnim;
  public string m_FloorGlowAnimState;
  public float m_bookBooshDelay = 0.5f;
  public float m_floorGlowDelay = 2.5f;
  public Animator m_LibraryBook;
  public string m_PageFlipRightAnimState;
  public string m_PageFlipLeftAnimState;
  public string m_BookShakeAnimState;
  public string m_RuneMatchSound;
  public List<string> m_PageFlipSounds;
  private bool m_isAnimating;
  private int m_leftIdx;
  private int m_rightIdx;
  private int m_floorRuneIdx;

  private void Start()
  {
    this.FlipBookPages();
    this.m_floorRuneIdx = Random.Range(0, 15);
    this.m_FloorPage.SetIndex(this.m_floorRuneIdx);
  }

  private void Update() => this.HandleHits();

  private void HandleHits()
  {
    if (InputCollection.GetMouseButtonUp(0) && this.IsOver(this.m_BookPageR) && !this.m_isAnimating)
      this.FlipBookPages();
    if (InputCollection.GetMouseButtonUp(0) && this.IsOver(this.m_BookPageL) && !this.m_isAnimating)
      this.FlipBookPages(false);
    if (!InputCollection.GetMouseButtonUp(0) || !this.IsOver(this.m_FloorRune) || this.m_isAnimating)
      return;
    this.StartCoroutine(this.CheckRuneMatches());
  }

  private void FlipBookPages(bool isRight = true)
  {
    this.m_isAnimating = true;
    this.m_LibraryBook.enabled = true;
    if (this.m_PageFlipSounds.Count > 0)
    {
      string pageFlipSound = this.m_PageFlipSounds[Random.Range(0, this.m_PageFlipSounds.Count - 1)];
      if (pageFlipSound != null)
        SoundManager.Get().LoadAndPlay((AssetReference) pageFlipSound);
    }
    if (isRight)
    {
      this.m_LibraryBook.Play(this.m_PageFlipRightAnimState, -1, 0.0f);
      this.m_BookPageL_StaticMesh.SetIndex(this.m_leftIdx);
      this.m_leftIdx = Random.Range(0, 15);
      this.m_BookPageR_TurnedMesh.SetIndex(this.m_leftIdx);
      this.m_BookPageL_Glow.SetIndex(this.m_leftIdx);
      this.m_BookPageR_NotTurnedMesh.SetIndex(this.m_rightIdx);
      this.m_rightIdx = Random.Range(0, 15);
      while (this.m_rightIdx == this.m_leftIdx)
        this.m_rightIdx = Random.Range(0, 15);
      this.m_BookPageR_StaticMesh.SetIndex(this.m_rightIdx);
      this.m_BookPageR_Glow.SetIndex(this.m_rightIdx);
    }
    else
    {
      this.m_LibraryBook.Play(this.m_PageFlipLeftAnimState, -1, 0.0f);
      this.m_BookPageR_StaticMesh.SetIndex(this.m_rightIdx);
      this.m_rightIdx = Random.Range(0, 15);
      this.m_BookPageR_NotTurnedMesh.SetIndex(this.m_rightIdx);
      this.m_BookPageR_Glow.SetIndex(this.m_rightIdx);
      this.m_BookPageR_TurnedMesh.SetIndex(this.m_leftIdx);
      this.m_leftIdx = Random.Range(0, 15);
      while (this.m_leftIdx == this.m_rightIdx)
        this.m_leftIdx = Random.Range(0, 15);
      this.m_BookPageL_StaticMesh.SetIndex(this.m_leftIdx);
      this.m_BookPageL_Glow.SetIndex(this.m_leftIdx);
    }
    this.m_isAnimating = false;
  }

  private IEnumerator CheckRuneMatches()
  {
    if ((this.m_floorRuneIdx == this.m_leftIdx || this.m_floorRuneIdx == this.m_rightIdx) && !this.m_isAnimating)
    {
      this.m_isAnimating = true;
      if (this.m_RuneMatchSound != string.Empty)
        SoundManager.Get().LoadAndPlay((AssetReference) this.m_RuneMatchSound);
      if ((Object) this.m_BookGlowRAnim != (Object) null && this.m_BookGlowAnimRState != null && this.m_floorRuneIdx == this.m_rightIdx)
      {
        this.m_BookGlowRAnim.enabled = true;
        this.m_BookGlowRAnim.Play(this.m_BookGlowAnimRState, -1, 0.0f);
      }
      if ((Object) this.m_BookGlowLAnim != (Object) null && this.m_BookGlowAnimLState != null && this.m_floorRuneIdx == this.m_leftIdx)
      {
        this.m_BookGlowLAnim.enabled = true;
        this.m_BookGlowLAnim.Play(this.m_BookGlowAnimLState, -1, 0.0f);
      }
      if ((Object) this.m_BookBooshAnim != (Object) null && this.m_BookBooshAnimState != null)
      {
        yield return (object) new WaitForSeconds(this.m_bookBooshDelay);
        this.m_BookBooshAnim.enabled = true;
        this.m_BookBooshAnim.Play(this.m_BookBooshAnimState, -1, 0.0f);
        this.m_LibraryBook.Play(this.m_BookShakeAnimState, -1, 0.0f);
      }
      if ((Object) this.m_FloorGlowAnim != (Object) null && this.m_FloorGlowAnimState != null)
      {
        yield return (object) new WaitForSeconds(this.m_floorGlowDelay);
        this.m_FloorGlowAnim.enabled = true;
        this.m_FloorGlowAnim.Play(this.m_FloorGlowAnimState, -1, 0.0f);
        this.m_FloorParticles.Play();
      }
      yield return (object) new WaitForSeconds(0.5f);
      this.m_floorRuneIdx = Random.Range(0, 15);
      this.m_FloorPage.SetIndex(this.m_floorRuneIdx);
      this.m_isAnimating = false;
    }
  }

  private bool IsOver(GameObject go) => (bool) (Object) go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
}
