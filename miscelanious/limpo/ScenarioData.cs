using UnityEngine;

[CustomEditClass]
public class ScenarioData : ScriptableObject
{
  private bool _bottom;
  private float m_phoneoffset = -0.389f;
  [CustomEditField(T = EditType.TEXTURE)]
  public string m_Texture;
  [CustomEditField(Sections = "Phone", T = EditType.TEXTURE)]
  public string m_Texture_Phone;
  [CustomEditField(Hide = true)]
  public float m_Texture_Phone_offsetY;

  [CustomEditField(Label = "Use Bottom Image", Sections = "Phone")]
  public bool m_bottom
  {
    get => this._bottom;
    set
    {
      this._bottom = value;
      this.m_Texture_Phone_offsetY = value ? this.m_phoneoffset : 0.0f;
    }
  }
}
