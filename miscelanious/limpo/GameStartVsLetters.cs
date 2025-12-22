using UnityEngine;

public class GameStartVsLetters : MonoBehaviour
{
  public string m_fadeAnimName;
  private Animation m_anim;

  private void Awake()
  {
    this.m_anim = this.GetComponentInChildren<Animation>();
    if (!((Object) this.m_anim == (Object) null))
      return;
    Log.All.PrintError("GameStartVsLetters.Awake(): No Animator component found in children.");
  }

  public void FadeIn()
  {
    if (!((Object) this.m_anim != (Object) null))
      return;
    this.m_anim[this.m_fadeAnimName].speed = -1f;
    this.m_anim[this.m_fadeAnimName].time = 1f;
    this.m_anim.Play(this.m_fadeAnimName);
  }

  public void FadeOut()
  {
    if (!((Object) this.m_anim != (Object) null))
      return;
    this.m_anim[this.m_fadeAnimName].speed = 1f;
    this.m_anim[this.m_fadeAnimName].time = 0.0f;
    this.m_anim.Play(this.m_fadeAnimName);
  }
}
