using UnityEngine;

[CustomEditClass]
public class NormalButton : PegUIElement
{
  [CustomEditField(Sections = "Button Properties")]
  public GameObject m_button;
  [CustomEditField(Sections = "Button Properties")]
  public TextMesh m_buttonText;
  [CustomEditField(Sections = "Button Properties")]
  public UberText m_buttonUberText;
  [CustomEditField(Sections = "Mouse Over Settings")]
  public GameObject m_mouseOverBone;
  [CustomEditField(Sections = "Mouse Over Settings")]
  public float m_userOverYOffset = -0.05f;
  private Vector3 m_originalButtonPosition;
  private int buttonID;

  protected override void Awake() => this.SetOriginalButtonPosition();

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    if ((Object) this.m_mouseOverBone != (Object) null)
      this.m_button.transform.position = this.m_mouseOverBone.transform.position;
    else
      TransformUtil.SetLocalPosY(this.m_button.gameObject, this.m_originalButtonPosition.y + this.m_userOverYOffset);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState) => this.m_button.gameObject.transform.localPosition = this.m_originalButtonPosition;

  public void SetUserOverYOffset(float userOverYOffset) => this.m_userOverYOffset = userOverYOffset;

  public void SetButtonID(int newID) => this.buttonID = newID;

  public int GetButtonID() => this.buttonID;

  public void SetText(string t)
  {
    if ((Object) this.m_buttonUberText == (Object) null)
      this.m_buttonText.text = t;
    else
      this.m_buttonUberText.Text = t;
  }

  public float GetTextWidth() => (Object) this.m_buttonUberText == (Object) null ? this.m_buttonText.GetComponent<Renderer>().bounds.extents.x * 2f : this.m_buttonUberText.Width;

  public float GetTextHeight() => (Object) this.m_buttonUberText == (Object) null ? this.m_buttonText.GetComponent<Renderer>().bounds.extents.y * 2f : this.m_buttonUberText.Height;

  public float GetRight() => this.GetComponent<BoxCollider>().bounds.max.x;

  public float GetLeft()
  {
    Bounds bounds = this.GetComponent<BoxCollider>().bounds;
    return bounds.center.x - bounds.extents.x;
  }

  public float GetTop()
  {
    Bounds bounds = this.GetComponent<BoxCollider>().bounds;
    return bounds.center.y + bounds.extents.y;
  }

  public float GetBottom()
  {
    Bounds bounds = this.GetComponent<BoxCollider>().bounds;
    return bounds.center.y - bounds.extents.y;
  }

  public void SetOriginalButtonPosition() => this.m_originalButtonPosition = this.m_button.transform.localPosition;

  public GameObject GetButtonTextGO() => (Object) this.m_buttonUberText == (Object) null ? this.m_buttonText.gameObject : this.m_buttonUberText.gameObject;

  public UberText GetButtonUberText() => this.m_buttonUberText;

  public string GetText() => (Object) this.m_buttonUberText == (Object) null ? this.m_buttonText.text : this.m_buttonUberText.Text;
}
