using Blizzard.T5.MaterialService.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomFrameButtonReskinController : MonoBehaviour
{
  [Header("Button")]
  public UIBButton[] Buttons;
  public Material ButtonMaterial;
  public Material HolderMaterial;
  [Header("Artist Name")]
  public GameObject ArtistCredit;
  public Material ArtistCreditMaterial;
  [NonSerialized]
  public Material OverrideButtonMaterial;
  [NonSerialized]
  public Material OverrideHolderMaterial;
  [NonSerialized]
  public Material OverrideArtistCreditMaterial;
  [NonSerialized]
  public float VerticalOffset;
  [NonSerialized]
  public float ArtistCreditVerticalOffset;
  private readonly HashSet<Renderer> m_buttonRenderers = new HashSet<Renderer>();
  private readonly HashSet<Renderer> m_holderRenderers = new HashSet<Renderer>();
  private readonly HashSet<Renderer> m_creditRenderers = new HashSet<Renderer>();
  private Vector3[] m_buttonPositions;
  private Vector3 m_creditPosition;

  private void Awake()
  {
    if (this.Buttons != null)
    {
      List<Vector3> vector3List = new List<Vector3>();
      foreach (UIBButton button in this.Buttons)
      {
        vector3List.Add(button.transform.localPosition);
        foreach (Renderer componentsInChild in button.GetComponentsInChildren<Renderer>())
        {
          Material sharedMaterial = componentsInChild.GetSharedMaterial();
          if ((UnityEngine.Object) sharedMaterial == (UnityEngine.Object) this.ButtonMaterial)
            this.m_buttonRenderers.Add(componentsInChild);
          else if ((UnityEngine.Object) sharedMaterial == (UnityEngine.Object) this.HolderMaterial)
            this.m_holderRenderers.Add(componentsInChild);
        }
      }
      this.m_buttonPositions = vector3List.ToArray();
    }
    if (!((UnityEngine.Object) this.ArtistCredit != (UnityEngine.Object) null))
      return;
    this.m_creditPosition = this.ArtistCredit.transform.localPosition;
    foreach (Renderer componentsInChild in this.ArtistCredit.GetComponentsInChildren<Renderer>())
    {
      if ((UnityEngine.Object) componentsInChild.GetSharedMaterial() == (UnityEngine.Object) this.ArtistCreditMaterial)
        this.m_creditRenderers.Add(componentsInChild);
    }
  }

  private void OnEnable() => this.AssignMaterials();

  private void OnDisable() => this.RestoreMaterials();

  public void UpdateMaterials(CustomFrameButtonReskinData reskinData)
  {
    if ((UnityEngine.Object) reskinData != (UnityEngine.Object) null)
    {
      this.OverrideButtonMaterial = reskinData.ButtonMaterial;
      this.OverrideHolderMaterial = reskinData.HolderMaterial;
      this.VerticalOffset = reskinData.VerticalOffset;
      this.ArtistCreditVerticalOffset = reskinData.ArtistCreditVerticalOffset;
    }
    else
    {
      this.OverrideButtonMaterial = (Material) null;
      this.OverrideHolderMaterial = (Material) null;
      this.VerticalOffset = 0.0f;
      this.ArtistCreditVerticalOffset = 0.0f;
    }
    if (!this.isActiveAndEnabled)
      return;
    this.AssignMaterials();
  }

  public void AssignMaterials()
  {
    if ((UnityEngine.Object) this.OverrideButtonMaterial != (UnityEngine.Object) null)
    {
      foreach (Renderer buttonRenderer in this.m_buttonRenderers)
        buttonRenderer.SetSharedMaterial(this.OverrideButtonMaterial);
    }
    if ((UnityEngine.Object) this.OverrideHolderMaterial != (UnityEngine.Object) null)
    {
      foreach (Renderer holderRenderer in this.m_holderRenderers)
        holderRenderer.SetSharedMaterial(this.OverrideHolderMaterial);
    }
    if ((UnityEngine.Object) this.OverrideArtistCreditMaterial != (UnityEngine.Object) null)
    {
      foreach (Renderer creditRenderer in this.m_creditRenderers)
        creditRenderer.SetSharedMaterial(this.OverrideArtistCreditMaterial);
    }
    if (this.Buttons != null && this.m_buttonPositions != null)
    {
      int length = this.Buttons.Length;
      for (int index = 0; index < length; ++index)
        this.Buttons[index].transform.localPosition = this.m_buttonPositions[index] + new Vector3(0.0f, 0.0f, this.VerticalOffset);
    }
    if (!((UnityEngine.Object) this.ArtistCredit != (UnityEngine.Object) null))
      return;
    this.ArtistCredit.transform.localPosition = this.m_creditPosition + new Vector3(0.0f, 0.0f, this.ArtistCreditVerticalOffset);
  }

  public void RestoreMaterials()
  {
    foreach (Renderer buttonRenderer in this.m_buttonRenderers)
      buttonRenderer.SetSharedMaterial(this.ButtonMaterial);
    foreach (Renderer holderRenderer in this.m_holderRenderers)
      holderRenderer.SetSharedMaterial(this.HolderMaterial);
    foreach (Renderer creditRenderer in this.m_creditRenderers)
      creditRenderer.SetSharedMaterial(this.ArtistCreditMaterial);
    if (this.Buttons != null && this.m_buttonPositions != null)
    {
      int length = this.Buttons.Length;
      for (int index = 0; index < length; ++index)
        this.Buttons[index].transform.localPosition = this.m_buttonPositions[index];
    }
    if (!((UnityEngine.Object) this.ArtistCredit != (UnityEngine.Object) null))
      return;
    this.ArtistCredit.transform.localPosition = this.m_creditPosition;
  }
}
