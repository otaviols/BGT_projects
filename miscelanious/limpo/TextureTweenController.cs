using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTweenController : MonoBehaviour
{
  public float ForwardTransitionDuration = 0.5f;
  public AnimationCurve ForwardTransitionCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
  public float ReverseTransitionDuration = 0.5f;
  public AnimationCurve ReverseTransitionCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
  public List<Renderer> AffectedRenderers = new List<Renderer>();
  private IEnumerator m_currentTween;

  public void StartForwardTransition()
  {
    this.StopCurrentTransition();
    this.m_currentTween = this.RunTransition(this.ForwardTransitionCurve, this.ForwardTransitionDuration);
    this.StartCoroutine(this.m_currentTween);
  }

  public void StartReverseTransition()
  {
    this.StopCurrentTransition();
    this.m_currentTween = this.RunTransition(this.ReverseTransitionCurve, this.ReverseTransitionDuration);
    this.StartCoroutine(this.m_currentTween);
  }

  public void StopCurrentTransition()
  {
    if (this.m_currentTween == null)
      return;
    this.StopCoroutine(this.m_currentTween);
    this.m_currentTween = (IEnumerator) null;
  }

  private IEnumerator RunTransition(AnimationCurve transitionCurve, float duration)
  {
    List<Material> affectedMaterials = new List<Material>();
    for (int index = 0; index < this.AffectedRenderers.Count; ++index)
      affectedMaterials.Add(this.AffectedRenderers[index].GetMaterial());
    float startTime = Time.time;
    float elapsedTime = 0.0f;
    while ((double) elapsedTime < (double) duration && (double) duration > 0.0)
    {
      elapsedTime = Time.time - startTime;
      float num = transitionCurve.Evaluate(elapsedTime / duration);
      for (int index = 0; index < affectedMaterials.Count; ++index)
        affectedMaterials[index].SetFloat("_Transistion", num);
      yield return (object) null;
    }
    float num1 = transitionCurve.Evaluate(1f);
    for (int index = 0; index < affectedMaterials.Count; ++index)
      affectedMaterials[index].SetFloat("_Transistion", num1);
    this.m_currentTween = (IEnumerator) null;
  }
}
