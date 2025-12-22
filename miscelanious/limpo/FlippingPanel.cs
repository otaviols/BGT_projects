using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class FlippingPanel : MonoBehaviour
{
  [CustomEditField(Sections = "Panels")]
  public List<GameObject> m_panelContent = new List<GameObject>();
  [CustomEditField(Sections = "Panels")]
  public List<Transform> m_faceBones = new List<Transform>();
  [CustomEditField(Sections = "Rotation")]
  public GameObject m_rotatingObject;
  [CustomEditField(Sections = "Rotation")]
  public float m_contentFlipAnimationTime = 0.5f;
  [CustomEditField(Sections = "Rotation")]
  public iTween.EaseType m_contentFlipEaseType = iTween.EaseType.easeOutBounce;
  [CustomEditField(Sections = "Rotation")]
  public float m_rotationDegrees = 120f;
  [CustomEditField(Sections = "Rotation")]
  public bool m_allowLoopingToStartOfContent = true;
  [CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
  public string m_contentFlipSound;
  private List<FlippingPanel.PanelContentChanged> m_panelContentChangedListeners = new List<FlippingPanel.PanelContentChanged>();
  private int m_currentContentOffset;
  private int m_currentFaceBone;
  private GameObject m_previousContent;
  private Quaternion m_desiredOrientation = Quaternion.identity;

  private void Awake()
  {
    for (int index = 0; index < this.m_panelContent.Count; ++index)
    {
      if (index == this.m_currentContentOffset)
      {
        GameObject gameObject = this.m_panelContent[index];
        gameObject.transform.parent = this.m_faceBones[this.m_currentFaceBone];
        GameUtils.ResetTransform(gameObject);
        gameObject.gameObject.SetActive(true);
      }
      else
        this.m_panelContent[index].SetActive(false);
    }
  }

  private void Start() => this.m_desiredOrientation = this.m_rotatingObject.transform.localRotation;

  public int CurrentContentOffset => this.m_currentContentOffset;

  public bool FlipPanel(bool forward)
  {
    if (iTween.CountByName(this.m_rotatingObject, "PANEL_ROTATION") > 0)
    {
      iTween.StopByName(this.m_rotatingObject, "PANEL_ROTATION");
      this.FinishFlip();
    }
    int num1 = this.m_currentContentOffset + (forward ? 1 : -1);
    if (num1 >= this.m_panelContent.Count)
    {
      if (!this.m_allowLoopingToStartOfContent)
        return false;
      num1 = 0;
    }
    else if (num1 < 0)
    {
      if (!this.m_allowLoopingToStartOfContent)
        return false;
      num1 = this.m_panelContent.Count - 1;
    }
    this.m_previousContent = this.m_panelContent[this.m_currentContentOffset];
    this.m_currentContentOffset = num1;
    GameObject gameObject = this.m_panelContent[this.m_currentContentOffset];
    int num2 = this.m_currentFaceBone + (forward ? 1 : -1);
    if (num2 >= this.m_faceBones.Count)
      num2 = 0;
    else if (num2 < 0)
      num2 = this.m_faceBones.Count - 1;
    this.m_currentFaceBone = num2;
    if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
    {
      gameObject.transform.parent = this.m_faceBones[this.m_currentFaceBone];
      GameUtils.ResetTransform(gameObject);
      gameObject.gameObject.SetActive(true);
    }
    if (!string.IsNullOrEmpty(this.m_contentFlipSound))
      SoundManager.Get().LoadAndPlay((AssetReference) this.m_contentFlipSound);
    this.m_desiredOrientation = this.m_rotatingObject.transform.localRotation * (forward ? Quaternion.AngleAxis(this.m_rotationDegrees, Vector3.right) : Quaternion.AngleAxis(this.m_rotationDegrees, Vector3.left));
    if ((double) this.m_contentFlipAnimationTime > 0.0)
      iTween.RotateAdd(this.m_rotatingObject, iTween.Hash((object) "amount", (object) (this.m_rotationDegrees * (forward ? Vector3.right : Vector3.left)), (object) "time", (object) this.m_contentFlipAnimationTime, (object) "easeType", (object) this.m_contentFlipEaseType, (object) "isLocal", (object) true, (object) "name", (object) "PANEL_ROTATION", (object) "oncomplete", (object) (Action<object>) (o => this.FinishFlip())));
    this.FirePanelContentChangedEvent(this.m_currentContentOffset);
    return true;
  }

  public void AddPanelContentChangedListener(FlippingPanel.PanelContentChanged listener) => this.m_panelContentChangedListeners.Add(listener);

  public void RemovePanelContentChangedListener(FlippingPanel.PanelContentChanged listener) => this.m_panelContentChangedListeners.Remove(listener);

  private void FirePanelContentChangedEvent(int newContentOffset)
  {
    foreach (FlippingPanel.PanelContentChanged panelContentChanged in this.m_panelContentChangedListeners.ToArray())
      panelContentChanged(newContentOffset);
  }

  private void FinishFlip()
  {
    this.m_rotatingObject.transform.localRotation = this.m_desiredOrientation;
    if (!((UnityEngine.Object) this.m_previousContent != (UnityEngine.Object) null))
      return;
    this.m_previousContent.gameObject.SetActive(false);
  }

  public delegate void PanelContentChanged(int newContentOffset);
}
