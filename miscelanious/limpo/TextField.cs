using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class TextField : PegUIElement
{
  public Transform inputTopLeft;
  public Transform inputBottomRight;
  public Color textColor;
  public int maxCharacters;
  public bool autocorrect;
  public TextField.KeyboardType keyboardType;
  public TextField.KeyboardReturnKeyType returnKeyType;
  public bool hideInput = true;
  public bool useNativeKeyboard;
  public bool inputKeepFocusOnComplete;
  private static uint nextId;
  private static TextField instance;
  private Rect lastBounds;
  private Vector3 lastTopLeft;
  private Vector3 lastBottomRight;
  private Font m_InputFont;
  private UniversalInputManager.TextInputParams inputParams;

  public static Rect KeyboardArea { get; private set; }

  public bool Active => (UnityEngine.Object) this == (UnityEngine.Object) TextField.instance;

  public string Text
  {
    get => TextField.GetTextFieldText();
    set => TextField.SetTextFieldText(value);
  }

  public event Action Preprocess;

  public event Action<string> Changed;

  public event Action<string> Submitted;

  public event Action Canceled;

  protected override void Awake()
  {
    base.Awake();
    this.gameObject.name = string.Format("TextField_{0:000}", (object) TextField.nextId++);
    if ((UnityEngine.Object) this.gameObject.GetComponent<BoxCollider>() == (UnityEngine.Object) null)
      this.gameObject.AddComponent<BoxCollider>();
    this.UpdateCollider();
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
  }

  protected override void OnDestroy()
  {
    if (this.Active)
      this.Deactivate();
    FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    base.OnDestroy();
  }

  private void Update()
  {
    if (this.lastTopLeft != this.inputTopLeft.position || this.lastBottomRight != this.inputBottomRight.position)
    {
      this.UpdateCollider();
      this.lastTopLeft = this.inputTopLeft.position;
      this.lastBottomRight = this.inputBottomRight.position;
    }
    if (!this.Active)
      return;
    Rect bounds = this.ComputeBounds();
    if (!(bounds != this.lastBounds))
      return;
    this.lastBounds = bounds;
    TextField.SetTextFieldBounds((TextField.PluginRect) bounds);
  }

  public void SetInputFont(Font font) => this.m_InputFont = font;

  public void Activate()
  {
    if ((UnityEngine.Object) TextField.instance != (UnityEngine.Object) null && (UnityEngine.Object) TextField.instance != (UnityEngine.Object) this)
      TextField.instance.Deactivate();
    TextField.instance = this;
    this.lastBounds = this.ComputeBounds();
    TextField.KeyboardArea = (Rect) TextField.ActivateTextField(this.gameObject.name, (TextField.PluginRect) this.lastBounds, this.autocorrect ? 1 : 0, (uint) this.keyboardType, (uint) this.returnKeyType);
    TextField.SetTextFieldColor(this.textColor.r, this.textColor.g, this.textColor.b, this.textColor.a);
    TextField.SetTextFieldMaxCharacters(512);
  }

  public void Deactivate()
  {
    if (!((UnityEngine.Object) this == (UnityEngine.Object) TextField.instance))
      return;
    TextField.KeyboardArea = new Rect();
    TextField.DeactivateTextField();
    TextField.instance = (TextField) null;
  }

  public static void ToggleDebug()
  {
  }

  protected override void OnRelease()
  {
    if (this.Active)
      return;
    this.Activate();
  }

  private bool OnPreprocess()
  {
    if (this.Preprocess != null)
      this.Preprocess();
    return false;
  }

  private void OnChanged(string text)
  {
    if (this.Changed == null)
      return;
    this.Changed(text);
  }

  private void OnSubmitted(string text)
  {
    if (this.Submitted == null)
      return;
    this.Submitted(text);
  }

  private void OnCanceled()
  {
    if (this.Canceled != null)
      this.Canceled();
    this.Deactivate();
  }

  private void OnKeyboardAreaChanged(Rect area) => TextField.KeyboardArea = area;

  private void OnFatalError(FatalErrorMessage message, object userData) => this.Deactivate();

  private void UpdateCollider()
  {
    BoxCollider component = this.GetComponent<BoxCollider>();
    Vector3 vector3_1 = this.transform.InverseTransformPoint(this.inputTopLeft.transform.position);
    Vector3 vector3_2 = this.transform.InverseTransformPoint(this.inputBottomRight.transform.position);
    component.center = (vector3_1 + vector3_2) / 2f;
    component.size = VectorUtils.Abs(vector3_2 - vector3_1);
  }

  private Rect ComputeBounds()
  {
    Camera firstByLayer = CameraUtils.FindFirstByLayer(this.gameObject.layer);
    Vector2 screenPoint1 = (Vector2) firstByLayer.WorldToScreenPoint(this.inputTopLeft.transform.position);
    Vector2 screenPoint2 = (Vector2) firstByLayer.WorldToScreenPoint(this.inputBottomRight.transform.position);
    screenPoint1.y = (float) Screen.height - screenPoint1.y;
    screenPoint2.y = (float) Screen.height - screenPoint2.y;
    Vector2 vector2_1 = Vector2.Min(screenPoint1, screenPoint2);
    Vector2 vector2_2 = Vector2.Max(screenPoint1, screenPoint2);
    return Rect.MinMaxRect(Mathf.Round(vector2_1.x), Mathf.Round(vector2_1.y), Mathf.Round(vector2_2.x), Mathf.Round(vector2_2.y));
  }

  private static TextField.PluginRect Plugin_ActivateTextField(
    string name,
    [MarshalAs(UnmanagedType.Struct)] TextField.PluginRect bounds,
    int autocorrect,
    uint keyboardType,
    uint returnKeyType)
  {
    return new TextField.PluginRect();
  }

  private static void Plugin_DeactivateTextField()
  {
  }

  private static void Plugin_SetTextFieldBounds([MarshalAs(UnmanagedType.Struct)] TextField.PluginRect bounds)
  {
  }

  private static string Plugin_GetTextFieldText() => "";

  private static void Plugin_SetTextFieldText(string text)
  {
  }

  private static void Plugin_SetTextFieldColor(float r, float g, float b, float a)
  {
  }

  private static void Plugin_SetTextFieldMaxCharacters(int maxCharacters)
  {
  }

  private void Unity_TextInputChanged(string text)
  {
    if (!this.Active)
      return;
    this.OnChanged(text);
  }

  private void Unity_TextInputSubmitted(string text)
  {
    if (!this.Active)
      return;
    this.OnSubmitted(text);
  }

  private void Unity_TextInputCanceled(string unused)
  {
    if (!this.Active)
      return;
    this.OnCanceled();
  }

  private void Unity_KeyboardAreaChanged(string rectString)
  {
    if (!this.Active)
      return;
    Match match = Regex.Match(rectString, string.Format("x\\: (?<x>{0})\\, y\\: (?<y>{0})\\, width\\: (?<width>{0})\\, height\\: (?<height>{0})", (object) "[-+]?[0-9]*\\.?[0-9]+"));
    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
    this.OnKeyboardAreaChanged(new Rect(float.Parse(match.Groups["x"].Value, (IFormatProvider) invariantCulture), float.Parse(match.Groups["y"].Value, (IFormatProvider) invariantCulture), float.Parse(match.Groups["width"].Value, (IFormatProvider) invariantCulture), float.Parse(match.Groups["height"].Value, (IFormatProvider) invariantCulture)));
  }

  private static bool UseNativeKeyboard() => false;

  private static TextField.PluginRect ActivateTextField(
    string name,
    TextField.PluginRect bounds,
    int autocorrect,
    uint keyboardType,
    uint returnKeyType)
  {
    if (TextField.UseNativeKeyboard())
      return TextField.Plugin_ActivateTextField(name, bounds, autocorrect, keyboardType, returnKeyType);
    if (UniversalInputManager.Get() == null)
      return new TextField.PluginRect();
    TextField.instance.inputParams = new UniversalInputManager.TextInputParams()
    {
      m_owner = TextField.instance.gameObject,
      m_preprocessCallback = new UniversalInputManager.TextInputPreprocessCallback(TextField.instance.OnPreprocess),
      m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(TextField.instance.OnSubmitted),
      m_updatedCallback = new UniversalInputManager.TextInputUpdatedCallback(TextField.instance.OnChanged),
      m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(TextField.instance.InputCanceled),
      m_font = TextField.instance.m_InputFont,
      m_maxCharacters = TextField.instance.maxCharacters,
      m_inputKeepFocusOnComplete = TextField.instance.inputKeepFocusOnComplete,
      m_touchScreenKeyboardHideInput = TextField.instance.hideInput,
      m_useNativeKeyboard = TextField.UseNativeKeyboard()
    };
    UniversalInputManager.Get().UseTextInput(TextField.instance.inputParams);
    TextField.SetTextFieldBounds(bounds);
    return TextField.instance.Active ? (TextField.PluginRect) new Rect(0.0f, (float) Screen.height, (float) Screen.width, (float) Screen.height * 0.5f) : new TextField.PluginRect();
  }

  private static void DeactivateTextField()
  {
    if (TextField.UseNativeKeyboard())
    {
      TextField.Plugin_DeactivateTextField();
    }
    else
    {
      if (UniversalInputManager.Get() == null)
        return;
      UniversalInputManager.Get().CancelTextInput(TextField.instance.gameObject);
    }
  }

  private static void SetTextFieldBounds(TextField.PluginRect bounds)
  {
    if (TextField.UseNativeKeyboard())
    {
      TextField.Plugin_SetTextFieldBounds(bounds);
    }
    else
    {
      bounds.x /= (float) Screen.width;
      bounds.y /= (float) Screen.height;
      bounds.width /= (float) Screen.width;
      bounds.height /= (float) Screen.height;
      if (!(TextField.instance.inputParams.m_rect != (Rect) bounds))
        return;
      TextField.instance.inputParams.m_rect = (Rect) bounds;
      TextField.instance.UpdateTextInput();
    }
  }

  private static string GetTextFieldText() => TextField.UseNativeKeyboard() ? TextField.Plugin_GetTextFieldText() : UniversalInputManager.Get().GetInputText();

  private static void SetTextFieldText(string text)
  {
    if (TextField.UseNativeKeyboard())
    {
      TextField.Plugin_SetTextFieldText(text);
    }
    else
    {
      UniversalInputManager.Get().SetInputText(text);
      if (!((UnityEngine.Object) TextField.instance != (UnityEngine.Object) null) || TextField.instance.inputParams == null)
        return;
      TextField.instance.inputParams.m_text = text;
    }
  }

  private static void SetTextFieldColor(float r, float g, float b, float a)
  {
    if (!TextField.UseNativeKeyboard())
      return;
    TextField.Plugin_SetTextFieldColor(r, g, b, a);
  }

  private static void SetTextFieldMaxCharacters(int maxCharacters)
  {
    if (TextField.UseNativeKeyboard())
    {
      TextField.Plugin_SetTextFieldMaxCharacters(maxCharacters);
    }
    else
    {
      if (maxCharacters == TextField.instance.maxCharacters)
        return;
      TextField.instance.maxCharacters = maxCharacters;
      TextField.instance.UpdateTextInput();
    }
  }

  private void UpdateTextInput()
  {
    UniversalInputManager.Get().UseTextInput(TextField.instance.inputParams, true);
    UniversalInputManager.Get().FocusTextInput(TextField.instance.gameObject);
  }

  private void InputCanceled(bool userRequested, GameObject requester) => this.OnCanceled();

  private struct PluginRect
  {
    public float x;
    public float y;
    public float width;
    public float height;

    public PluginRect(float x, float y, float width, float height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    public static implicit operator TextField.PluginRect(Rect rect) => new TextField.PluginRect(rect.x, rect.y, rect.width, rect.height);

    public static implicit operator Rect(TextField.PluginRect rect) => new Rect(rect.x, rect.y, rect.width, rect.height);

    public override string ToString() => string.Format("[x: {0}, y: {1}, width: {2}, height: {3}]", (object) this.x, (object) this.y, (object) this.width, (object) this.height);
  }

  public enum KeyboardType
  {
    Default,
    ASCIICapable,
    NumbersAndPunctuation,
    URL,
    NumberPad,
    PhonePad,
    NamePhonePad,
    EmailAddress,
    DecimalPad,
    Twitter,
  }

  public enum KeyboardReturnKeyType
  {
    Default,
    Go,
    Google,
    Join,
    Next,
    Route,
    Search,
    Send,
    Yahoo,
    Done,
    EmergencyCall,
  }
}
