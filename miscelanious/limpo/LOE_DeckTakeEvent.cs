using System.Collections;
using UnityEngine;

[CustomEditClass]
public class LOE_DeckTakeEvent : MonoBehaviour
{
  public Renderer m_friendlyDeckRenderer;
  public Animator m_takeDeckAnimator;
  public string m_takeDeckAnimName = "LOE_TakeDeck";
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_takeDeckSoundPrefab;
  public Animator m_replacementDeckAnimator;
  public string m_replacementDeckAnimName = "CardsToPlayerDeck";
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_replacementDeckSoundPrefab;
  private bool m_animIsPlaying;

  private void Start() => CardBackManager.Get().SetCardBackTexture(this.m_friendlyDeckRenderer, 0, CardBackManager.CardBackSlot.FRIENDLY);

  public IEnumerator PlayTakeDeckAnim()
  {
    this.m_animIsPlaying = true;
    this.m_takeDeckAnimator.enabled = true;
    this.m_takeDeckAnimator.Play(this.m_takeDeckAnimName);
    if (!string.IsNullOrEmpty(this.m_takeDeckSoundPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_takeDeckSoundPrefab);
    yield return (object) new WaitForEndOfFrame();
    yield return (object) new WaitForSeconds(this.m_takeDeckAnimator.GetCurrentAnimatorStateInfo(0).length);
    this.m_animIsPlaying = false;
  }

  public IEnumerator PlayReplacementDeckAnim()
  {
    this.m_animIsPlaying = true;
    this.m_replacementDeckAnimator.enabled = true;
    this.m_replacementDeckAnimator.Play(this.m_replacementDeckAnimName);
    if (!string.IsNullOrEmpty(this.m_replacementDeckSoundPrefab))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_replacementDeckSoundPrefab);
    yield return (object) new WaitForEndOfFrame();
    yield return (object) new WaitForSeconds(this.m_replacementDeckAnimator.GetCurrentAnimatorStateInfo(0).length);
    this.m_animIsPlaying = false;
  }

  public bool AnimIsPlaying() => this.m_animIsPlaying;
}
