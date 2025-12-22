using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class GeneralStorePackBuyCallout : MonoBehaviour
{
  [CustomEditField(ListTable = true, Sections = "MultiSliceElement")]
  public List<GeneralStorePackBuyCallout.CalloutSection> m_sections = new List<GeneralStorePackBuyCallout.CalloutSection>();
  [CustomEditField(Sections = "Size Variations")]
  public List<float> m_phonePerspectiveOffsetX = new List<float>();
  [CustomEditField(Sections = "Size Variations")]
  public List<int> m_phoneTextWidth = new List<int>();
  [CustomEditField(Sections = "Size Variations")]
  public List<GameObject> m_glowSizeVariations = new List<GameObject>();
  [CustomEditField(Sections = "Size Variations")]
  public List<GameObject> m_phoneSingleButtonMeshesToActivate = new List<GameObject>();
  [CustomEditField(Sections = "Text")]
  public UberText m_text;
  [CustomEditField(Sections = "Animation")]
  public Vector3 m_punchAmount = new Vector3(0.2f, 0.2f, 0.2f);
  private bool m_isShown;
  private bool m_initialized;
  private MultiSliceElement m_multiSlice;
  private const string ANIMATE_PULSE_FUNC = "AnimatePulse";
  private Vector3 m_origScale;

  private void Awake()
  {
  }

  public void Init()
  {
    this.m_multiSlice = this.GetComponent<MultiSliceElement>();
    this.m_origScale = this.transform.localScale;
    this.m_initialized = true;
  }

  public bool IsShown() => this.m_isShown;

  public void ShowCallout(
    GeneralStorePackBuyButton firstButton,
    GeneralStorePackBuyButton lastButton,
    int numButtons)
  {
    if (!this.m_initialized || this.m_isShown || (UnityEngine.Object) firstButton == (UnityEngine.Object) null || (UnityEngine.Object) lastButton == (UnityEngine.Object) null || numButtons <= 0)
      return;
    this.m_isShown = true;
    this.gameObject.SetActive(true);
    this.m_text.Text = GameStrings.Get("GLUE_STORE_LIMITED_TIME_OFFER");
    this.ToggleCalloutSections(numButtons);
    this.m_multiSlice.UpdateSlices();
    this.ToggleGameObjectActive(this.m_glowSizeVariations, numButtons - 1);
    float x = firstButton.transform.position.x;
    float num = (float) (((double) lastButton.transform.position.x - (double) x) / 2.0);
    TransformUtil.SetPosX(this.gameObject, x + num);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (numButtons <= this.m_phonePerspectiveOffsetX.Count)
        TransformUtil.SetLocalPosX(this.gameObject, this.transform.localPosition.x + this.m_phonePerspectiveOffsetX[numButtons - 1]);
      if (numButtons <= this.m_phoneTextWidth.Count)
        this.m_text.Width = (float) this.m_phoneTextWidth[numButtons - 1];
    }
    this.AnimateIn();
    this.InvokeRepeating("AnimatePulse", 3f, 3f);
  }

  public void HideCallout()
  {
    if (!this.m_isShown)
      return;
    this.CancelInvoke("AnimatePulse");
    this.AnimateOut();
  }

  public void DeactivateCallout()
  {
    this.CancelInvoke("AnimatePulse");
    this.gameObject.SetActive(false);
    this.m_isShown = false;
  }

  private void ToggleCalloutSections(int numCalloutSectionsNeeded)
  {
    bool flag1 = numCalloutSectionsNeeded == 1;
    if (flag1 && (bool) UniversalInputManager.UsePhoneUI && this.m_phoneSingleButtonMeshesToActivate.Count > 0)
    {
      this.ToggleCalloutSectionsForPhoneSingleButton();
    }
    else
    {
      for (int index = 0; index < this.m_sections.Count; ++index)
      {
        GeneralStorePackBuyCallout.CalloutSection section = this.m_sections[index];
        if (index < numCalloutSectionsNeeded)
        {
          bool flag2 = flag1 || index > 0;
          section.m_centerMesh.SetActive(flag2);
          section.m_arrowDownMesh1.SetActive(false);
          section.m_arrowDownMesh2.SetActive(false);
          if (GeneralUtils.IsEven(index))
            section.m_arrowDownMesh1.SetActive(true);
          else
            section.m_arrowDownMesh2.SetActive(true);
        }
        else
        {
          if (flag1 && index == 1)
            section.m_centerMesh.SetActive(true);
          else
            section.m_centerMesh.SetActive(false);
          section.m_arrowDownMesh1.SetActive(false);
          section.m_arrowDownMesh2.SetActive(false);
        }
      }
    }
  }

  private void ToggleCalloutSectionsForPhoneSingleButton()
  {
    foreach (GeneralStorePackBuyCallout.CalloutSection section in this.m_sections)
    {
      section.m_centerMesh.SetActive(false);
      section.m_arrowDownMesh1.SetActive(false);
      section.m_arrowDownMesh2.SetActive(false);
    }
    foreach (GameObject gameObject in this.m_phoneSingleButtonMeshesToActivate)
    {
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
        gameObject.SetActive(true);
    }
  }

  private void ToggleGameObjectActive(List<GameObject> gameObjects, int indexToActivate)
  {
    for (int index = 0; index < gameObjects.Count; ++index)
    {
      if ((UnityEngine.Object) gameObjects[index] != (UnityEngine.Object) null)
        gameObjects[index].SetActive(index == indexToActivate);
    }
  }

  private void AnimateIn()
  {
    iTween.Stop(this.gameObject);
    AnimationUtil.ShowWithPunch(this.gameObject, this.m_origScale * 0.01f, this.m_origScale * 1.2f, this.m_origScale);
  }

  private void AnimateOut()
  {
    iTween.Stop(this.gameObject);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) (this.transform.localScale * 0.01f), (object) "time", (object) 0.25f, (object) "easetype", (object) iTween.EaseType.easeOutQuad, (object) "oncomplete", (object) "DeactivateCallout"));
  }

  private void AnimatePulse() => iTween.PunchScale(this.gameObject, iTween.Hash((object) "amount", (object) this.m_punchAmount, (object) "time", (object) 1f));

  [Serializable]
  public class CalloutSection
  {
    public GameObject m_centerMesh;
    public GameObject m_arrowDownMesh1;
    public GameObject m_arrowDownMesh2;
  }
}
