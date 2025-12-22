using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LettuceMinionInPlayFrame : MonoBehaviour
{
  public List<LettuceMinionInPlayFrame.GemAndScarfMapping> m_gemAndScarfMappings;
  public GameObject[] m_attackBaubles = new GameObject[3];
  private Map<GameObject, Vector3> m_attackBaublesStartingScales = new Map<GameObject, Vector3>();
  public GameObject[] m_healthBaubles = new GameObject[3];
  private Map<GameObject, Vector3> m_healthBaublesStartingScales = new Map<GameObject, Vector3>();

  private void Awake()
  {
    this.InitializeInitialScaleMap(this.m_attackBaubles, this.m_attackBaublesStartingScales);
    this.InitializeInitialScaleMap(this.m_healthBaubles, this.m_healthBaublesStartingScales);
  }

  private void InitializeInitialScaleMap(GameObject[] baubles, Map<GameObject, Vector3> map)
  {
    if (baubles == null || map == null)
      return;
    foreach (GameObject bauble in baubles)
    {
      if (!map.ContainsKey(bauble))
        map.Add(bauble, bauble.transform.localScale);
    }
  }

  public void UpdateFrameType(TAG_ROLE role)
  {
    foreach (LettuceMinionInPlayFrame.GemAndScarfMapping gemAndScarfMapping in this.m_gemAndScarfMappings)
      gemAndScarfMapping.m_scarfAndGemParent.SetActive(gemAndScarfMapping.m_role == role);
  }

  public void EnlargeAttackBauble(float scaleFactor) => this.EnlargeBauble(this.m_attackBaubles, this.m_attackBaublesStartingScales, scaleFactor);

  public void EnlargeHealthBauble(float scaleFactor) => this.EnlargeBauble(this.m_healthBaubles, this.m_healthBaublesStartingScales, scaleFactor);

  private void EnlargeBauble(
    GameObject[] baubles,
    Map<GameObject, Vector3> startingScales,
    float scaleFactor)
  {
    if (baubles == null || startingScales == null)
      return;
    foreach (GameObject bauble in baubles)
    {
      if (!((UnityEngine.Object) bauble == (UnityEngine.Object) null) && bauble.activeInHierarchy)
      {
        Vector3 one;
        if (!startingScales.TryGetValue(bauble, out one))
          one = Vector3.one;
        this.EnlargeBaubleTween(bauble, one, scaleFactor);
      }
    }
  }

  private void EnlargeBaubleTween(GameObject bauble, Vector3 startingScale, float scaleFactor)
  {
    if ((UnityEngine.Object) bauble == (UnityEngine.Object) null || !bauble.activeInHierarchy)
      return;
    iTween.Stop(bauble);
    iTween.ScaleTo(bauble, iTween.Hash((object) "scale", (object) new Vector3(startingScale.x * scaleFactor, startingScale.y * scaleFactor, startingScale.z * scaleFactor), (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
  }

  public void ShrinkAttackBauble() => this.ShrinkBauble(this.m_attackBaubles, this.m_attackBaublesStartingScales);

  public void ShrinkHealthBauble() => this.ShrinkBauble(this.m_healthBaubles, this.m_healthBaublesStartingScales);

  private void ShrinkBauble(GameObject[] baubles, Map<GameObject, Vector3> startingScales)
  {
    if (baubles == null || startingScales == null)
      return;
    foreach (GameObject bauble in baubles)
    {
      if (!((UnityEngine.Object) bauble == (UnityEngine.Object) null) && bauble.activeInHierarchy)
      {
        Vector3 one;
        if (!startingScales.TryGetValue(bauble, out one))
          one = Vector3.one;
        iTween.ScaleTo(bauble, one, 0.5f);
      }
    }
  }

  [Serializable]
  public class GemAndScarfMapping
  {
    [SerializeField]
    public TAG_ROLE m_role;
    [SerializeField]
    public GameObject m_scarfAndGemParent;
  }
}
