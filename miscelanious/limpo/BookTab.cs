using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookTab : PegUIElement
{
  public GameObject m_glowMesh;
  public GameObject m_newItemCount;
  public UberText m_newItemCountText;
  public CollectionUtils.ViewMode m_tabViewMode;
  public Vector3 m_DeselectedLocalScale = new Vector3(0.44f, 0.44f, 0.44f);
  public Vector3 m_SelectedLocalScale = new Vector3(0.66f, 0.66f, 0.66f);
  public float m_SelectedLocalYPos = 0.1259841f;
  public float m_DeselectedLocalYPos;
  public string m_IconTextureName;
  [Tooltip("Local position offset. Applied after absolute local position.")]
  public Vector3 m_SelectedLocalOffset = Vector3.zero;
  protected int m_numNewItems;
  protected bool m_selected;
  protected Vector3 m_targetLocalPos;
  protected bool m_shouldBeVisible = true;
  protected bool m_isVisible = true;
  protected bool m_showLargeTab;
  protected MaterialPropertyBlock m_propertyBlock;
  public static readonly float SELECT_TAB_ANIM_TIME = 0.2f;

  public void Init()
  {
    this.SetTabIconsTextureOffset(this.gameObject.GetComponent<Renderer>());
    if ((Object) this.m_glowMesh != (Object) null)
      this.SetTabIconsTextureOffset(this.m_glowMesh.GetComponent<Renderer>());
    this.SetGlowActive(false);
    this.UpdateNewItemCount(0);
  }

  public void SetGlowActive(bool active)
  {
    if (this.m_selected)
      active = true;
    if (!((Object) this.m_glowMesh != (Object) null))
      return;
    this.m_glowMesh.SetActive(active);
  }

  public void SetSelected(bool selected)
  {
    if (this.m_selected == selected)
      return;
    this.m_selected = selected;
    this.SetGlowActive(this.m_selected);
  }

  public void UpdateNewItemCount(int numNewItems)
  {
    this.m_numNewItems = numNewItems;
    this.UpdateNewItemCountVisuals();
  }

  public void SetTargetLocalPosition(Vector3 targetLocalPos) => this.m_targetLocalPos = targetLocalPos;

  public void SetIsVisible(bool isVisible)
  {
    this.m_isVisible = isVisible;
    this.SetEnabled(this.m_isVisible);
  }

  public bool IsVisible() => this.m_isVisible;

  public void SetTargetVisibility(bool visible) => this.m_shouldBeVisible = visible;

  public bool ShouldBeVisible() => this.m_shouldBeVisible;

  public bool WillSlide() => (double) Mathf.Abs(this.m_targetLocalPos.x - this.transform.localPosition.x) > 0.0500000007450581;

  public void AnimateToTargetPosition(float animationTime, iTween.EaseType easeType)
  {
    Hashtable args = iTween.Hash((object) "position", (object) this.m_targetLocalPos, (object) "isLocal", (object) true, (object) "time", (object) animationTime, (object) "easetype", (object) easeType, (object) "name", (object) "position", (object) "oncomplete", (object) "OnMovedToTargetPos", (object) "oncompletetarget", (object) this.gameObject);
    iTween.StopByName(this.gameObject, "position");
    iTween.MoveTo(this.gameObject, args);
  }

  public virtual void SetLargeTab(bool large)
  {
    if (large == this.m_showLargeTab)
      return;
    if (large)
    {
      Vector3 localPosition = this.transform.localPosition with
      {
        y = this.m_SelectedLocalYPos
      };
      localPosition += this.m_SelectedLocalOffset;
      this.transform.localPosition = localPosition;
      iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.m_SelectedLocalScale, (object) "time", (object) BookTab.SELECT_TAB_ANIM_TIME, (object) "name", (object) "scale"));
      SoundManager.Get().LoadAndPlay((AssetReference) "class_tab_click.prefab:d9cb832f0de5c1947a97685e134ba0da", this.gameObject);
    }
    else
    {
      Vector3 localPosition = this.transform.localPosition with
      {
        y = this.m_DeselectedLocalYPos
      };
      localPosition.x -= this.m_SelectedLocalOffset.x;
      localPosition.z -= this.m_SelectedLocalOffset.z;
      this.transform.localPosition = localPosition;
      iTween.StopByName(this.gameObject, "scale");
      this.transform.localScale = this.m_DeselectedLocalScale;
    }
    this.m_showLargeTab = large;
  }

  protected virtual Vector2 GetTextureOffset() => Vector2.zero;

  protected void SetTabIconsTextureOffset(Renderer renderer)
  {
    if ((Object) renderer == (Object) null || string.IsNullOrEmpty(this.m_IconTextureName))
      return;
    if (this.m_propertyBlock == null)
      this.m_propertyBlock = new MaterialPropertyBlock();
    Vector2 textureOffset = this.GetTextureOffset();
    Vector4 vector4 = new Vector4(1f, 1f, textureOffset.x, textureOffset.y);
    List<Material> sharedMaterials = RendererExtension.GetSharedMaterials(renderer);
    for (int index = 0; index < sharedMaterials.Count; ++index)
    {
      Material material = sharedMaterials[index];
      if (!((Object) material.mainTexture == (Object) null) && material.mainTexture.name.Contains(this.m_IconTextureName))
      {
        renderer.GetPropertyBlock(this.m_propertyBlock, index);
        this.m_propertyBlock.SetVector("_MainTex_ST", vector4);
        renderer.SetPropertyBlock(this.m_propertyBlock, index);
      }
    }
  }

  private void UpdateNewItemCountVisuals()
  {
    if ((Object) this.m_newItemCountText != (Object) null)
      this.m_newItemCountText.Text = GameStrings.Format("GLUE_COLLECTION_NEW_CARD_CALLOUT", (object) this.m_numNewItems);
    if (!((Object) this.m_newItemCount != (Object) null))
      return;
    this.m_newItemCount.SetActive(this.m_numNewItems > 0);
  }

  private void OnMovedToTargetPos()
  {
    if (this.m_showLargeTab)
      return;
    this.transform.localPosition = this.transform.localPosition with
    {
      y = this.m_DeselectedLocalYPos
    };
  }
}
