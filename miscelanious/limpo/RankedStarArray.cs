using Blizzard.T5.Core.Utils;
using Blizzard.T5.Services;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class RankedStarArray : MonoBehaviour
{
  [CustomEditField(Sections = "General")]
  public int m_starCount;
  [CustomEditField(Sections = "General")]
  public int m_starCountDarkened;
  [CustomEditField(Sections = "General")]
  public RankedStarArray.LayoutStyle m_layout;
  [CustomEditField(Sections = "Linear Layout")]
  public float m_xOffsetPerStar;
  [CustomEditField(Sections = "Linear Layout")]
  public float m_zOffsetPerStar;
  [CustomEditField(Sections = "Arc Layout")]
  public float m_arcRadius;
  [CustomEditField(Sections = "Arc Layout")]
  public float m_arcDegreesPerStar;
  [CustomEditField(Sections = "Arc Layout")]
  public float m_centerStarsAtDegrees;
  [CustomEditField(Sections = "Arc Layout")]
  public bool m_arcAlignEdge;
  private static readonly string s_starPrefab = "Star_Ranked.prefab:48d5a18072eff2445a3de1c9f7348bea";
  private List<RankChangeStar> m_stars = new List<RankChangeStar>();
  private Coroutine m_showCoroutine;
  private Coroutine m_loadCoroutine;

  private bool IsShowing { get; set; }

  private void Awake() => this.LoadStars();

  public void Show()
  {
    if (this.m_showCoroutine != null)
      this.StopCoroutine(this.m_showCoroutine);
    this.m_showCoroutine = this.StartCoroutine(this.ShowWhenReady());
  }

  public void Hide()
  {
    if (!this.IsShowing)
      return;
    foreach (Component star in this.m_stars)
      star.gameObject.SetActive(false);
  }

  private IEnumerator ShowWhenReady()
  {
    while (this.IsLoading())
      yield return (object) null;
    foreach (Component star in this.m_stars)
      star.gameObject.SetActive(true);
    this.IsShowing = true;
  }

  public void Init(int starCount, int starCountDarkened)
  {
    this.m_starCount = starCount;
    this.m_starCountDarkened = starCountDarkened;
    this.Reset();
  }

  public bool PopulateFsmArrayWithStars(
    PlayMakerFSM fsm,
    string varName,
    int startIndex = 0,
    int count = 0)
  {
    if ((UnityEngine.Object) fsm == (UnityEngine.Object) null || string.IsNullOrEmpty(varName))
      return false;
    FsmArray fsmArray = fsm.FsmVariables.GetFsmArray(varName);
    if (fsmArray == null)
      return false;
    if (count <= 0)
      count = this.m_stars.Count;
    fsmArray.objectReferences = this.m_stars.Skip<RankChangeStar>(startIndex).Take<RankChangeStar>(count).Select<RankChangeStar, UnityEngine.Object>((Func<RankChangeStar, UnityEngine.Object>) (star => (UnityEngine.Object) star.gameObject)).ToArray<UnityEngine.Object>();
    return true;
  }

  public bool IsLoading() => this.m_stars.Count < this.m_starCount;

  private void LoadStars()
  {
    if (this.m_loadCoroutine != null)
      this.StopCoroutine(this.m_loadCoroutine);
    this.m_loadCoroutine = this.StartCoroutine(this.LoadStarsWhenReady());
  }

  private IEnumerator LoadStarsWhenReady()
  {
    RankedStarArray rankedStarArray = this;
    if (rankedStarArray.m_starCount > 0)
    {
      while (!ServiceManager.IsAvailable<IAssetLoader>())
        yield return (object) null;
      for (int index = 0; index < rankedStarArray.m_starCount; ++index)
        AssetLoader.Get().InstantiatePrefab((AssetReference) RankedStarArray.s_starPrefab, new PrefabCallback<GameObject>(rankedStarArray.OnStarLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    }
  }

  private void OnStarLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    go.transform.localScale = Vector3.one;
    GameUtils.SetParent(go, this.gameObject);
    go.SetActive(false);
    this.m_stars.Add(go.GetComponent<RankChangeStar>());
    if (this.m_stars.Count != this.m_starCount)
      return;
    int starCountDarkened = this.m_starCountDarkened;
    for (int index = this.m_stars.Count - 1; index >= 0 && starCountDarkened > 0; --starCountDarkened)
    {
      this.m_stars[index].BlackOut();
      --index;
    }
    if (this.m_layout == RankedStarArray.LayoutStyle.Arc)
      this.LayoutStarsArc();
    else
      this.LayoutStarsLinear();
  }

  private void LayoutStarsArc()
  {
    float f1 = this.m_centerStarsAtDegrees * ((float) Math.PI / 180f);
    float num1 = this.m_arcDegreesPerStar * ((float) Math.PI / 180f);
    float num2 = num1 * (float) (this.m_stars.Count - 1);
    float f2 = f1 + num2 / 2f;
    Vector3 position1 = this.transform.position;
    position1.x += this.m_arcRadius * Mathf.Cos(f1);
    position1.z += this.m_arcRadius * Mathf.Sin(f1);
    foreach (RankChangeStar star in this.m_stars)
    {
      Vector3 position2 = this.transform.position;
      position2.x += this.m_arcRadius * Mathf.Cos(f2);
      position2.z += this.m_arcRadius * Mathf.Sin(f2);
      if (this.m_arcAlignEdge)
      {
        Vector3 vector3 = this.transform.position - position1;
        position2 += vector3;
      }
      star.transform.position = position2;
      f2 -= num1;
    }
  }

  private void LayoutStarsLinear()
  {
    int index1 = this.m_stars.Count / 2 - 1;
    int index2 = index1 + 1;
    float num1 = this.m_layout == RankedStarArray.LayoutStyle.Vertical ? 1f : -1f;
    float num2 = this.m_layout == RankedStarArray.LayoutStyle.Vertical ? -1f : 1f;
    float x = 0.0f;
    float z = 0.0f;
    int index3;
    if (GeneralUtils.IsOdd(this.m_stars.Count))
    {
      if (this.m_stars.Count < 3)
        return;
      index3 = index2 + 1;
    }
    else
    {
      if (this.m_stars.Count < 2)
        return;
      if (this.m_layout == RankedStarArray.LayoutStyle.Vertical)
      {
        z += this.m_zOffsetPerStar / 2f;
        TransformUtil.SetLocalPosZ((Component) this.m_stars[index1], z * -1f);
        TransformUtil.SetLocalPosZ((Component) this.m_stars[index2], z);
      }
      else
      {
        x += this.m_xOffsetPerStar / 2f;
        TransformUtil.SetLocalPosX((Component) this.m_stars[index1], x * -1f);
        TransformUtil.SetLocalPosX((Component) this.m_stars[index2], x);
      }
      --index1;
      index3 = index2 + 1;
    }
    while (index1 >= 0)
    {
      x += this.m_xOffsetPerStar;
      z += this.m_zOffsetPerStar;
      TransformUtil.SetLocalPosX((Component) this.m_stars[index1], x * num1);
      TransformUtil.SetLocalPosZ((Component) this.m_stars[index1], z * num2);
      --index1;
      TransformUtil.SetLocalPosX((Component) this.m_stars[index3], x);
      TransformUtil.SetLocalPosZ((Component) this.m_stars[index3], z);
      ++index3;
    }
  }

  [ContextMenu("Show")]
  private void ResetAndShow()
  {
    this.Reset();
    this.Show();
  }

  [ContextMenu("Reset")]
  private void Reset()
  {
    foreach (Component star in this.m_stars)
      UnityEngine.Object.Destroy((UnityEngine.Object) star.gameObject);
    this.m_stars.Clear();
    this.IsShowing = false;
    this.LoadStars();
  }

  public enum LayoutStyle
  {
    Horizontal,
    Vertical,
    Arc,
  }
}
