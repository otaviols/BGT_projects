using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class ScoreScreen : MonoBehaviour
{
  public GameObject m_BackgroundCenter;
  public NestedPrefab m_ScoreBoxLeft;
  public NestedPrefab m_ScoreBoxCenter;
  public NestedPrefab m_ScoreBoxRight;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_ShowSoundPrefab;
  [CustomEditField(T = EditType.SOUND_PREFAB)]
  public string m_HideSoundPrefab;
  public UberText m_FooterText;
  public const float SHOW_SEC = 0.65f;
  public const float HIDE_SEC = 0.25f;
  private const float SHOW_INTERMED_SEC = 0.5f;
  private const float SHOW_FINAL_SEC = 0.15f;
  private const float DRIFT_CYCLE_SEC = 10f;
  private static readonly Vector3 START_SCALE = new Vector3(1f / 1000f, 1f / 1000f, 1f / 1000f);
  private static ScoreScreen s_instance;
  private Vector3 m_initialScale;

  private void Awake()
  {
    ScoreScreen.s_instance = this;
    this.Init();
  }

  private void OnDestroy() => ScoreScreen.s_instance = (ScoreScreen) null;

  public static ScoreScreen Get() => ScoreScreen.s_instance;

  public void Show()
  {
    this.gameObject.SetActive(true);
    Vector3 vector3 = 1.03f * this.m_initialScale;
    Vector3 initialScale = this.m_initialScale;
    this.transform.localScale = ScoreScreen.START_SCALE;
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) vector3, (object) "time", (object) 0.5f));
    Action<object> action = (Action<object>) (param => this.Drift());
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) initialScale, (object) "delay", (object) 0.5f, (object) "time", (object) 0.15f, (object) "oncomplete", (object) action));
    if (string.IsNullOrEmpty(this.m_ShowSoundPrefab))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_ShowSoundPrefab);
  }

  public void Hide()
  {
    Action<object> action = (Action<object>) (param => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject));
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) ScoreScreen.START_SCALE, (object) "time", (object) 0.25f, (object) "oncomplete", (object) action));
    if (string.IsNullOrEmpty(this.m_HideSoundPrefab))
      return;
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_HideSoundPrefab);
  }

  private void Init()
  {
    this.m_initialScale = this.transform.localScale;
    this.UpdateScoreBoxes();
    this.LayoutScoreBoxes();
    this.UpdateFooterText();
    this.gameObject.SetActive(false);
  }

  private void UpdateScoreBoxes()
  {
    this.UpdateScoreBox(this.m_ScoreBoxLeft, GAME_TAG.SCORE_LABELID_1, GAME_TAG.SCORE_VALUE_1);
    this.UpdateScoreBox(this.m_ScoreBoxCenter, GAME_TAG.SCORE_LABELID_2, GAME_TAG.SCORE_VALUE_2);
    this.UpdateScoreBox(this.m_ScoreBoxRight, GAME_TAG.SCORE_LABELID_3, GAME_TAG.SCORE_VALUE_3);
  }

  private void UpdateFooterText()
  {
    int labelId = 0;
    int num = -1;
    this.GetLabelAndScore(GAME_TAG.SCORE_FOOTERID, GAME_TAG.SCORE_FOOTERID, out labelId, out num);
    if (labelId <= 0)
    {
      this.m_FooterText.gameObject.SetActive(false);
    }
    else
    {
      ScoreLabelDbfRecord record = GameDbf.ScoreLabel.GetRecord(labelId);
      if (record == null)
      {
        Error.AddDevWarning("Error", "ScoreScreen.UpdateFooterText() - There is no ScoreLabel record for id {0}.", (object) labelId);
      }
      else
      {
        this.m_FooterText.Text = record.Text.GetString();
        this.m_FooterText.gameObject.SetActive(true);
      }
    }
  }

  private void UpdateScoreBox(NestedPrefab scoreBoxPrefab, GAME_TAG labelTag, GAME_TAG valueTag)
  {
    ScoreBox component = scoreBoxPrefab.PrefabGameObject(true).GetComponent<ScoreBox>();
    int labelId = 0;
    int num = 0;
    this.GetLabelAndScore(labelTag, valueTag, out labelId, out num);
    bool flag = labelId != 0;
    scoreBoxPrefab.gameObject.SetActive(flag);
    if (!flag)
      return;
    ScoreLabelDbfRecord record = GameDbf.ScoreLabel.GetRecord(labelId);
    if (record == null)
      Error.AddDevWarning("Error", "ScoreScreen.UpdateScoreBox() - There is no ScoreLabel record for id {0}.", (object) labelId);
    else
      component.m_Label.Text = record.Text.GetString();
    component.m_Value.Text = num.ToString();
  }

  private void GetLabelAndScore(
    GAME_TAG labelTag,
    GAME_TAG valueTag,
    out int labelId,
    out int value)
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    labelId = friendlySidePlayer.GetTag(labelTag);
    if (labelId != 0)
    {
      value = friendlySidePlayer.GetTag(valueTag);
    }
    else
    {
      GameEntity gameEntity = GameState.Get().GetGameEntity();
      labelId = gameEntity.GetTag(labelTag);
      value = gameEntity.GetTag(valueTag);
    }
  }

  private void LayoutScoreBoxes()
  {
    NestedPrefab[] array = new NestedPrefab[3]
    {
      this.m_ScoreBoxLeft,
      this.m_ScoreBoxCenter,
      this.m_ScoreBoxRight
    };
    int index1 = Array.FindIndex<NestedPrefab>(array, 0, (Predicate<NestedPrefab>) (scoreBox => scoreBox.gameObject.activeInHierarchy));
    int index2 = Array.FindIndex<NestedPrefab>(array, index1 + 1, (Predicate<NestedPrefab>) (scoreBox => scoreBox.gameObject.activeInHierarchy));
    int index3 = Array.FindIndex<NestedPrefab>(array, index2 + 1, (Predicate<NestedPrefab>) (scoreBox => scoreBox.gameObject.activeInHierarchy));
    NestedPrefab nestedPrefab1 = array[index1];
    if (index2 < 0)
    {
      nestedPrefab1.transform.position = this.m_ScoreBoxCenter.transform.position;
    }
    else
    {
      if (index3 >= 0)
        return;
      NestedPrefab nestedPrefab2 = array[index2];
      Vector3 vector3_1 = 0.5f * (this.m_ScoreBoxLeft.transform.position + this.m_ScoreBoxCenter.transform.position);
      Vector3 vector3_2 = 0.5f * (this.m_ScoreBoxCenter.transform.position + this.m_ScoreBoxRight.transform.position);
      nestedPrefab1.transform.position = vector3_1;
      nestedPrefab2.transform.position = vector3_2;
    }
  }

  private void Drift()
  {
    Vector3 position = this.transform.position;
    Vector3 vector3_1 = 0.02f * this.m_BackgroundCenter.GetComponent<Renderer>().bounds.size.x * this.transform.up;
    Vector3 vector3_2 = position + vector3_1;
    Vector3 vector3_3 = position - vector3_1;
    iTween.MoveTo(this.gameObject, iTween.Hash((object) "path", (object) new List<Vector3>()
    {
      position,
      vector3_2,
      position,
      vector3_3,
      position
    }.ToArray(), (object) "time", (object) 10f, (object) "easetype", (object) iTween.EaseType.linear, (object) "looptype", (object) iTween.LoopType.loop));
  }
}
