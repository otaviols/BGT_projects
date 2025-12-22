using UnityEngine;

public class FramedRadioButton : MonoBehaviour
{
  public GameObject m_root;
  public GameObject m_frameEndLeft;
  public GameObject m_frameEndRight;
  public GameObject m_frameLeft;
  public GameObject m_frameFill;
  public RadioButton m_radioButton;
  public UberText m_text;
  private float m_leftEdgeOffset;

  public int GetButtonID() => this.m_radioButton.GetButtonID();

  public float GetLeftEdgeOffset() => this.m_leftEdgeOffset;

  public virtual void Init(
    FramedRadioButton.FrameType frameType,
    string text,
    int buttonID,
    object userData)
  {
    this.m_radioButton.SetButtonID(buttonID);
    this.m_radioButton.SetUserData(userData);
    this.m_text.Text = text;
    this.m_text.UpdateNow();
    this.m_frameFill.SetActive(true);
    bool flag1 = false;
    bool flag2 = false;
    switch (frameType)
    {
      case FramedRadioButton.FrameType.SINGLE:
        flag1 = true;
        flag2 = true;
        break;
      case FramedRadioButton.FrameType.MULTI_LEFT_END:
        flag1 = true;
        flag2 = false;
        break;
      case FramedRadioButton.FrameType.MULTI_RIGHT_END:
        flag1 = false;
        flag2 = true;
        break;
      case FramedRadioButton.FrameType.MULTI_MIDDLE:
        flag1 = false;
        flag2 = false;
        break;
    }
    this.m_frameEndLeft.SetActive(flag1);
    this.m_frameLeft.SetActive(!flag1);
    this.m_frameEndRight.SetActive(flag2);
    this.m_leftEdgeOffset = (flag1 ? this.m_frameEndLeft.transform : this.m_frameLeft.transform).position.x - this.transform.position.x;
  }

  public void Show() => this.m_root.SetActive(true);

  public void Hide() => this.m_root.SetActive(false);

  public Bounds GetBounds()
  {
    Bounds bounds = this.m_frameFill.GetComponent<Renderer>().bounds;
    this.IncludeBoundsOfGameObject(this.m_frameEndLeft, ref bounds);
    this.IncludeBoundsOfGameObject(this.m_frameEndRight, ref bounds);
    this.IncludeBoundsOfGameObject(this.m_frameLeft, ref bounds);
    return bounds;
  }

  private void IncludeBoundsOfGameObject(GameObject go, ref Bounds bounds)
  {
    if (!go.activeSelf)
      return;
    Bounds bounds1 = go.GetComponent<Renderer>().bounds;
    Vector3 max = Vector3.Max(bounds1.max, bounds.max);
    Vector3 min = Vector3.Min(bounds1.min, bounds.min);
    bounds.SetMinMax(min, max);
  }

  public enum FrameType
  {
    SINGLE,
    MULTI_LEFT_END,
    MULTI_RIGHT_END,
    MULTI_MIDDLE,
  }
}
