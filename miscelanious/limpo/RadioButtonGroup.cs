using System.Collections.Generic;
using UnityEngine;

public class RadioButtonGroup : MonoBehaviour
{
  public GameObject m_buttonContainer;
  public FramedRadioButton m_framedRadioButtonPrefab;
  public GameObject m_firstRadioButtonBone;
  private List<FramedRadioButton> m_framedRadioButtons = new List<FramedRadioButton>();
  private RadioButtonGroup.DelButtonSelected m_buttonSelectedCB;
  private RadioButtonGroup.DelButtonDoubleClicked m_buttonDoubleClickedCB;
  private Vector3 m_spacingFudgeFactor = Vector3.zero;

  public void ShowButtons(
    List<RadioButtonGroup.ButtonData> buttonData,
    RadioButtonGroup.DelButtonSelected buttonSelectedCallback,
    RadioButtonGroup.DelButtonDoubleClicked buttonDoubleClickedCallback)
  {
    this.m_buttonContainer.SetActive(true);
    int count = buttonData.Count;
    while (this.m_framedRadioButtons.Count > count)
    {
      FramedRadioButton framedRadioButton = this.m_framedRadioButtons[0];
      this.m_framedRadioButtons.RemoveAt(0);
      Object.DestroyImmediate((Object) framedRadioButton);
    }
    bool flag = 1 == count;
    Vector3 position = this.m_buttonContainer.transform.position;
    GameObject relative = new GameObject();
    RadioButton radioButton = (RadioButton) null;
    for (int index = 0; index < count; ++index)
    {
      FramedRadioButton framedRadioButton;
      if (this.m_framedRadioButtons.Count > index)
      {
        framedRadioButton = this.m_framedRadioButtons[index];
      }
      else
      {
        framedRadioButton = this.CreateNewFramedRadioButton();
        this.m_framedRadioButtons.Add(framedRadioButton);
      }
      FramedRadioButton.FrameType frameType = !flag ? (index != 0 ? (count - 1 != index ? FramedRadioButton.FrameType.MULTI_MIDDLE : FramedRadioButton.FrameType.MULTI_RIGHT_END) : FramedRadioButton.FrameType.MULTI_LEFT_END) : FramedRadioButton.FrameType.SINGLE;
      RadioButtonGroup.ButtonData buttonData1 = buttonData[index];
      framedRadioButton.Show();
      framedRadioButton.Init(frameType, buttonData1.m_text, buttonData1.m_id, buttonData1.m_userData);
      if (buttonData1.m_selected)
      {
        if ((Object) radioButton != (Object) null)
        {
          Debug.LogWarning((object) "RadioButtonGroup.WaitThenShowButtons(): more than one button was set as selected. Selecting the FIRST provided option.");
          framedRadioButton.m_radioButton.SetSelected(false);
        }
        else
        {
          radioButton = framedRadioButton.m_radioButton;
          radioButton.SetSelected(true);
        }
      }
      else
        framedRadioButton.m_radioButton.SetSelected(false);
      if (index == 0)
        TransformUtil.SetPoint(framedRadioButton.gameObject, Anchor.LEFT, this.m_firstRadioButtonBone, Anchor.LEFT);
      else
        TransformUtil.SetPoint(framedRadioButton.gameObject, new Vector3(0.0f, 1f, 0.5f), relative, new Vector3(1f, 1f, 0.5f), this.m_spacingFudgeFactor);
      relative = framedRadioButton.m_frameFill;
    }
    position.x -= TransformUtil.GetBoundsOfChildren(this.m_buttonContainer).size.x / 2f;
    this.m_buttonContainer.transform.position = position;
    this.m_buttonSelectedCB = buttonSelectedCallback;
    this.m_buttonDoubleClickedCB = buttonDoubleClickedCallback;
    if ((Object) radioButton == (Object) null || this.m_buttonSelectedCB == null)
      return;
    this.m_buttonSelectedCB(radioButton.GetButtonID(), radioButton.GetUserData());
  }

  public void Hide() => this.m_buttonContainer.SetActive(false);

  public void SetSpacingFudgeFactor(Vector3 amount) => this.m_spacingFudgeFactor = amount;

  private FramedRadioButton CreateNewFramedRadioButton()
  {
    FramedRadioButton framedRadioButton = Object.Instantiate<FramedRadioButton>(this.m_framedRadioButtonPrefab);
    framedRadioButton.transform.parent = this.m_buttonContainer.transform;
    framedRadioButton.transform.localPosition = Vector3.zero;
    framedRadioButton.transform.localScale = Vector3.one;
    framedRadioButton.transform.localRotation = Quaternion.identity;
    framedRadioButton.m_radioButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRadioButtonReleased));
    framedRadioButton.m_radioButton.AddEventListener(UIEventType.DOUBLECLICK, new UIEvent.Handler(this.OnRadioButtonDoubleClicked));
    return framedRadioButton;
  }

  private void OnRadioButtonReleased(UIEvent e)
  {
    RadioButton element = e.GetElement() as RadioButton;
    if ((Object) element == (Object) null)
    {
      Debug.LogWarning((object) string.Format("RadioButtonGroup.OnRadioButtonReleased(): UIEvent {0} source is not a RadioButton!", (object) e));
    }
    else
    {
      bool flag = element.IsSelected();
      foreach (FramedRadioButton framedRadioButton in this.m_framedRadioButtons)
      {
        RadioButton radioButton = framedRadioButton.m_radioButton;
        bool selected = (Object) element == (Object) radioButton;
        radioButton.SetSelected(selected);
      }
      if (this.m_buttonSelectedCB == null)
        return;
      this.m_buttonSelectedCB(element.GetButtonID(), element.GetUserData());
      if (!(UniversalInputManager.Get().IsTouchMode() & flag))
        return;
      this.OnRadioButtonDoubleClicked(e);
    }
  }

  private void OnRadioButtonDoubleClicked(UIEvent e)
  {
    if (this.m_buttonDoubleClickedCB == null)
      return;
    RadioButton element = e.GetElement() as RadioButton;
    if ((Object) element == (Object) null)
    {
      Debug.LogWarning((object) string.Format("RadioButtonGroup.OnRadioButtonDoubleClicked(): UIEvent {0} source is not a RadioButton!", (object) e));
    }
    else
    {
      FramedRadioButton framedRadioButton1 = (FramedRadioButton) null;
      foreach (FramedRadioButton framedRadioButton2 in this.m_framedRadioButtons)
      {
        if (!((Object) element != (Object) framedRadioButton2.m_radioButton))
        {
          framedRadioButton1 = framedRadioButton2;
          break;
        }
      }
      if ((Object) framedRadioButton1 == (Object) null)
        Debug.LogWarning((object) string.Format("RadioButtonGroup.OnRadioButtonDoubleClicked(): could not find framed radio button for radio button ID {0}", (object) element.GetButtonID()));
      else
        this.m_buttonDoubleClickedCB(framedRadioButton1);
    }
  }

  public delegate void DelButtonSelected(int buttonID, object userData);

  public delegate void DelButtonDoubleClicked(FramedRadioButton framedRadioButton);

  public struct ButtonData
  {
    public int m_id;
    public string m_text;
    public bool m_selected;
    public object m_userData;

    public ButtonData(int id, string text, object userData, bool selected)
    {
      this.m_id = id;
      this.m_text = text;
      this.m_userData = userData;
      this.m_selected = selected;
    }
  }
}
