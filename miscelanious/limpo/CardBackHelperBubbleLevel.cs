using Blizzard.T5.MaterialService.Extensions;
using UnityEngine;

public class CardBackHelperBubbleLevel : MonoBehaviour
{
  private Material m_material2;
  private float m_bubbleLiteralPosition;
  private float m_objectTilt;
  private float m_tiltRangeMin = -2f;
  private float m_tiltRangeMax = 2f;
  private float m_bubbleMomentum;
  private float m_bubblePosition;
  private float m_bubbleDistanceFromLiteral;
  public int TargetMaterialIndex;
  public float TiltSensitivity = 5f;
  public float BubbleMomentumBase = 0.05f;
  public float BubbleOffsetY = -1.22f;
  public string TexturePropertyName = "_AddTex";

  public void Awake() => this.m_material2 = this.GetComponent<Renderer>().GetMaterial(this.TargetMaterialIndex);

  private void Update()
  {
    this.m_objectTilt = CardBackHelperBubbleLevel.NormalizeRotation(this.gameObject.transform.eulerAngles.x) + CardBackHelperBubbleLevel.NormalizeRotation(this.gameObject.transform.eulerAngles.y - 180f) + CardBackHelperBubbleLevel.NormalizeRotation(this.gameObject.transform.eulerAngles.z);
    this.m_objectTilt = Mathf.Clamp(this.m_objectTilt / this.TiltSensitivity, this.m_tiltRangeMin, this.m_tiltRangeMax);
    this.m_objectTilt = (float) (((double) this.m_objectTilt + 2.0) * 0.5);
    this.m_bubbleLiteralPosition = this.m_objectTilt * 0.5f;
    this.m_bubbleDistanceFromLiteral = (float) (((double) this.m_bubblePosition - (double) this.m_bubbleLiteralPosition) * -1.0);
    this.m_bubbleMomentum = this.m_bubbleDistanceFromLiteral * this.BubbleMomentumBase;
    this.m_bubblePosition = Mathf.Clamp(this.m_bubblePosition + this.m_bubbleMomentum, 0.0f, 1f);
    this.m_material2.SetTextureOffset(this.TexturePropertyName, new Vector2(this.m_bubblePosition * -2f, this.BubbleOffsetY));
  }

  private static float NormalizeRotation(float inputRotation) => (double) inputRotation > 180.0 ? inputRotation - 360f : inputRotation;
}
