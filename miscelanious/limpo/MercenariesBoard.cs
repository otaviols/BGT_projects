using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MercenariesBoard : Board
{
  public List<GameObject> m_baseVisuals;
  public List<GameObject> m_pathVisuals;
  public List<MercenariesBoard.DecorationLayerWeighting> m_substrateVisuals;
  public List<MercenariesBoard.DecorationLayerWeighting> m_clickableVisuals;
  public List<MercenariesBoard.DecorationLayerWeighting> m_capVisuals;
  public List<MercenariesBoard.LightingSettings> m_lighting;
  public List<GameObject> m_weatherEffects;
  public GameObject m_bossDecorations;
  public List<MercenariesBoard.DecorationCountWeighting> m_cornerCountWeightList;
  public UIBButton m_debugRandomizeButton;
  public MusicPlaylistType m_bossMusic = MusicPlaylistType.InGame_MERCBOSS1;
  private bool m_isFinalBoss;
  private bool m_allowLightingChanges;

  public override void Start()
  {
    base.Start();
    if (!((UnityEngine.Object) this.m_debugRandomizeButton != (UnityEngine.Object) null))
      return;
    this.m_debugRandomizeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRandomizeButtonPressed));
  }

  protected override void ValidateInspectorReferences()
  {
  }

  public void RandomizeVisuals(bool isFinalBoss, bool allowLightingChanges, int seed = 0)
  {
    Log.Lettuce.PrintDebug("MercenariesBoard.RandomizeVisuals: seed={0}, isFinalBoss={1}", (object) seed, (object) isFinalBoss);
    this.m_isFinalBoss = isFinalBoss;
    this.m_allowLightingChanges = allowLightingChanges;
    if (seed > 0)
      UnityEngine.Random.InitState(seed);
    this.ActivateRandomElementFromList(this.m_baseVisuals);
    this.ActivateRandomElementFromList(this.m_pathVisuals, true);
    this.ActivateRandomCornerLayerFromList(this.m_substrateVisuals);
    this.ActivateRandomCornerLayerFromList(this.m_clickableVisuals);
    this.ActivateRandomCapLayerFromList(this.m_capVisuals);
    if (this.m_allowLightingChanges)
      this.ActivateRandomLightingFromList(this.m_lighting);
    this.ActivateRandomElementFromList(this.m_weatherEffects, true);
    this.SetupFinalBossState(isFinalBoss);
  }

  private void ActivateRandomElementFromList(List<GameObject> list, bool allowNone = false)
  {
    if (list == null || list.Count == 0)
      return;
    foreach (GameObject gameObject in list)
      gameObject.SetActive(false);
    int count = list.Count;
    if (allowNone)
      ++count;
    int index = UnityEngine.Random.Range(0, count);
    if (index >= list.Count)
      return;
    list[index].SetActive(true);
  }

  private void ActivateRandomCornerLayerFromList(
    List<MercenariesBoard.DecorationLayerWeighting> list)
  {
    if (list == null || list.Count == 0)
      return;
    this.SetupWeightedDecorations(list);
    List<MercenariesBoardDecorationLayer.DecorationPosition> arr = new List<MercenariesBoardDecorationLayer.DecorationPosition>()
    {
      MercenariesBoardDecorationLayer.DecorationPosition.TOP_LEFT,
      MercenariesBoardDecorationLayer.DecorationPosition.TOP_RIGHT,
      MercenariesBoardDecorationLayer.DecorationPosition.BOTTOM_LEFT,
      MercenariesBoardDecorationLayer.DecorationPosition.BOTTOM_RIGHT
    };
    GeneralUtils.Shuffle<MercenariesBoardDecorationLayer.DecorationPosition>((IList<MercenariesBoardDecorationLayer.DecorationPosition>) arr);
    int show = this.PickNumberOfCornersToShow();
    for (int index = 0; index < show; ++index)
      GeneralUtils.RollElementFromWeightedList<MercenariesBoard.DecorationLayerWeighting>(list, (GeneralUtils.WeightAccessorDelegate<MercenariesBoard.DecorationLayerWeighting>) (e => e.m_weight))?.m_decorationLayer.SetDecorationVisible(arr[index]);
  }

  private void ActivateRandomCapLayerFromList(
    List<MercenariesBoard.DecorationLayerWeighting> list)
  {
    if (list == null || list.Count == 0)
      return;
    this.SetupWeightedDecorations(list);
    List<MercenariesBoardDecorationLayer.DecorationPosition> decorationPositionList = new List<MercenariesBoardDecorationLayer.DecorationPosition>()
    {
      MercenariesBoardDecorationLayer.DecorationPosition.TOP_CENTER,
      MercenariesBoardDecorationLayer.DecorationPosition.BOTTOM_CENTER
    };
    for (int index = 0; index < decorationPositionList.Count; ++index)
      GeneralUtils.RollElementFromWeightedList<MercenariesBoard.DecorationLayerWeighting>(list, (GeneralUtils.WeightAccessorDelegate<MercenariesBoard.DecorationLayerWeighting>) (e => e.m_weight))?.m_decorationLayer.SetDecorationVisible(decorationPositionList[index]);
  }

  private void SetupWeightedDecorations(
    List<MercenariesBoard.DecorationLayerWeighting> list)
  {
    foreach (MercenariesBoard.DecorationLayerWeighting decorationLayerWeighting in list)
    {
      decorationLayerWeighting.m_decorationLayer.gameObject.SetActive(true);
      decorationLayerWeighting.m_decorationLayer.HideAllDecorations();
    }
  }

  private void ActivateRandomLightingFromList(List<MercenariesBoard.LightingSettings> list)
  {
    if (list == null || list.Count == 0)
      return;
    int index = UnityEngine.Random.Range(0, list.Count);
    MercenariesBoard.LightingSettings lightingSettings = list[index];
    this.m_AmbientColor = lightingSettings.m_ambientColor;
    this.m_DirectionalLight.color = lightingSettings.m_lightColor;
    this.m_DirectionalLight.intensity = lightingSettings.m_maxIntensity;
    this.m_DirectionalLightIntensity = lightingSettings.m_maxIntensity;
    this.ResetAmbientColor();
  }

  private int PickNumberOfCornersToShow()
  {
    MercenariesBoard.DecorationCountWeighting decorationCountWeighting = GeneralUtils.RollElementFromWeightedList<MercenariesBoard.DecorationCountWeighting>(this.m_cornerCountWeightList, (GeneralUtils.WeightAccessorDelegate<MercenariesBoard.DecorationCountWeighting>) (e => e.m_weight));
    return decorationCountWeighting == null ? 0 : decorationCountWeighting.m_numberOfDecorations;
  }

  private void SetupFinalBossState(bool isFinalBoss)
  {
    if ((UnityEngine.Object) this.m_bossDecorations != (UnityEngine.Object) null)
      this.m_bossDecorations.SetActive(isFinalBoss);
    if (!isFinalBoss)
      return;
    foreach (MercenariesBoard.DecorationLayerWeighting clickableVisual in this.m_clickableVisuals)
      clickableVisual.m_decorationLayer.HideTopDecorations();
    foreach (MercenariesBoard.DecorationLayerWeighting capVisual in this.m_capVisuals)
      capVisual.m_decorationLayer.HideTopDecorations();
    foreach (GameObject pathVisual in this.m_pathVisuals)
      pathVisual.SetActive(false);
    this.m_BoardMusic = this.m_bossMusic;
  }

  private void OnRandomizeButtonPressed(UIEvent e) => this.RandomizeVisuals(this.m_isFinalBoss, this.m_allowLightingChanges);

  [Serializable]
  public class LightingSettings
  {
    public Color m_lightColor;
    public Color m_ambientColor;
    public float m_maxIntensity;
  }

  [Serializable]
  public class DecorationCountWeighting
  {
    public int m_numberOfDecorations;
    public int m_weight;
  }

  [Serializable]
  public class DecorationLayerWeighting
  {
    public MercenariesBoardDecorationLayer m_decorationLayer;
    public int m_weight;
  }
}
