using System;
using UnityEngine;

public class CardBack : MonoBehaviour
{
  [Tooltip("Mesh for hidden card")]
  public Mesh m_CardBackMesh;
  [Tooltip("Material for hidden card")]
  public Material m_CardBackMaterial;
  [Tooltip("2nd Material for effects")]
  public Material m_CardBackMaterial1;
  [Tooltip("Alternative card back material for actors")]
  public Material m_CardBackMaterial2D;
  [Tooltip("Flat texture for decks and back of cards")]
  public Texture2D m_CardBackTexture;
  [Tooltip("Alternative meshes for the card deck. All must be provided to display in game!")]
  [SerializeField]
  public CardBack.CustomDeckMeshes m_CustomDeckMeshes;
  [Tooltip("Summon in echo effect texture")]
  public Texture2D m_HiddenCardEchoTexture;
  [Tooltip("Texture for card back highlight")]
  public Texture2D m_CardBackHighlightTexture;
  [Tooltip("Drag effects prefab")]
  public GameObject m_DragEffect;
  [Tooltip("Min Velocity that triggers effect")]
  public float m_EffectMinVelocity = 2f;
  [Tooltip("Max Velocity that stops the effect")]
  public float m_EffectMaxVelocity = 40f;
  public CardBack.cardBackHelpers cardBackHelper;

  public bool GetCustomDeckMeshes(out CardBack.CustomDeckMeshes meshes)
  {
    if (this.m_CustomDeckMeshes.IsComplete)
    {
      meshes = this.m_CustomDeckMeshes;
      return true;
    }
    meshes = new CardBack.CustomDeckMeshes();
    return false;
  }

  [Serializable]
  public struct CustomDeckMeshes
  {
    public Mesh ThicknessFull;
    public Mesh Thickness75;
    public Mesh Thickness50;
    public Mesh Thickness25;
    public Mesh Thickness1;

    public bool IsComplete => (UnityEngine.Object) this.ThicknessFull != (UnityEngine.Object) null && (UnityEngine.Object) this.Thickness75 != (UnityEngine.Object) null && (UnityEngine.Object) this.Thickness50 != (UnityEngine.Object) null && (UnityEngine.Object) this.Thickness25 != (UnityEngine.Object) null && (UnityEngine.Object) this.Thickness1 != (UnityEngine.Object) null;
  }

  public enum cardBackHelpers
  {
    None,
    CardBackHelperBubbleLevel,
    CardBackHelperFlipbook,
  }
}
