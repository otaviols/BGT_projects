using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputFieldUI : MonoBehaviour
{
  [SerializeField]
  private Image m_inputFieldBackgroundImage;
  [SerializeField]
  private Font m_defaultInputFont;
  [SerializeField]
  private TextAnchor m_defaultInputTextAlignment = TextAnchor.MiddleLeft;
  [SerializeField]
  private RectTransform m_inputFieldRect;
  [SerializeField]
  private Canvas m_inputFieldCanvas;
  [SerializeField]
  private HSInputField m_inputField;
  [SerializeField]
  private float m_inputFieldPadding = 5f;

  public string Text
  {
    get => this.m_inputField.text;
    set => this.m_inputField.text = value;
  }

  public string RawText
  {
    get => this.m_inputField.textComponent.text;
    set => this.m_inputField.textComponent.text = value;
  }

  public bool IsFocused => this.m_inputField.isFocused;

  public float RectHeight => this.m_inputFieldRect.rect.height;

  public void SetTextInputParams(UniversalInputManager.TextInputParams parms)
  {
    this.m_inputField.contentType = parms.m_number ? HSInputField.ContentType.IntegerNumber : HSInputField.ContentType.Standard;
    this.m_inputField.contentType = parms.m_password ? HSInputField.ContentType.Password : this.m_inputField.contentType;
    this.m_inputField.lineType = parms.m_multiLine ? HSInputField.LineType.MultiLineNewline : HSInputField.LineType.SingleLine;
    this.m_inputField.characterLimit = parms.m_maxCharacters;
    this.m_inputField.textComponent.color = parms.m_color ?? this.m_inputField.textComponent.color;
    this.m_inputField.textComponent.font = parms.m_font ?? this.m_defaultInputFont;
    this.m_inputField.textComponent.alignment = (TextAnchor) ((int) parms.m_alignment ?? (int) this.m_defaultInputTextAlignment);
    this.m_inputField.text = parms.m_text ?? string.Empty;
    this.m_inputFieldBackgroundImage.enabled = parms.m_showBackground;
  }

  public void SetCanvasActive(bool active, bool showBackground = false) => this.m_inputFieldCanvas.enabled = active;

  public void SetInputRect(Rect r)
  {
    this.m_inputFieldRect.anchorMin = (Vector2) Vector3.zero;
    this.m_inputFieldRect.anchorMax = (Vector2) Vector3.one;
    this.m_inputFieldRect.sizeDelta = new Vector2(r.xMax - r.xMin - this.m_inputFieldPadding, r.yMax - r.yMin);
    this.m_inputFieldRect.anchoredPosition = new Vector2((float) (((double) r.xMin + (double) r.xMax) / 2.0), (float) (-((double) r.yMin + (double) r.yMax) / 2.0));
  }

  public void SetupTextProperties(int fontSize, Color? inputColor, TextAnchor? inputAlignment)
  {
    this.m_inputField.textComponent.fontSize = fontSize;
    if (inputColor.HasValue)
      this.m_inputField.textComponent.color = inputColor.Value;
    this.m_inputField.textComponent.alignment = (TextAnchor) ((int) inputAlignment ?? (int) this.m_defaultInputTextAlignment);
  }

  public void ActivateInputField() => this.m_inputField.ActivateInputField();

  public void MoveCursorToEnd() => this.m_inputField.caretPosition = this.m_inputField.text.Length;

  public void SetEndEditFunction(UnityAction<string> endEditFunc) => this.m_inputField.onEndEdit.AddListener(endEditFunc);
}
