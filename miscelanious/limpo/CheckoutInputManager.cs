using Blizzard.Commerce;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutInputManager : MonoBehaviour
{
  private const int INPUT_DELTA_DELTA_SCALE = 40;
  private Vec2D m_lastMousePosition;
  private HearthstoneCheckout m_checkout;
  private IScreenSpace screenSpace;
  private List<char> blockedCharacters = new List<char>()
  {
    '\t',
    '\n'
  };
  private Dictionary<KeyCode, CheckoutInputManager.KeyboardEventListener> m_KeyboardEventHandlers = new Dictionary<KeyCode, CheckoutInputManager.KeyboardEventListener>();

  public bool IsActive { get; set; }

  private int GetModifiers(UnityEngine.Event e)
  {
    int modifiers = 0;
    if (e.isKey)
    {
      if ((e.modifiers & EventModifiers.Shift) != EventModifiers.None)
        modifiers |= 2;
      if ((e.modifiers & EventModifiers.Control) != EventModifiers.None)
        modifiers |= 1;
      if ((e.modifiers & EventModifiers.Alt) != EventModifiers.None)
        modifiers |= 4;
    }
    return modifiers;
  }

  public void Setup(HearthstoneCheckout checkout, IScreenSpace screenSpace)
  {
    this.m_checkout = checkout;
    this.screenSpace = screenSpace;
  }

  public void AddKeyboardEventListener(
    KeyCode keyCode,
    CheckoutInputManager.KeyboardEventListener listener)
  {
    this.m_KeyboardEventHandlers[keyCode] = listener;
  }

  public void RemoveKeyboardEventListener(KeyCode keyCode) => this.m_KeyboardEventHandlers.Remove(keyCode);

  private Vec2D GetMousePosition(Rect window, Vector3 mousePosition, float inputScale) => window.Contains(mousePosition) ? new Vec2D((int) (((double) mousePosition.x - (double) window.x) / (double) inputScale), (int) (((double) Screen.height - (double) mousePosition.y - (double) window.y) / (double) inputScale)) : (Vec2D) null;

  public void OnGUI()
  {
    if (!this.IsActive || this.m_checkout == null || !this.m_checkout.CheckoutIsReady)
      return;
    UnityEngine.Event current = UnityEngine.Event.current;
    if (current == null)
      return;
    int modifiers = 0;
    KeyCode keyCode = KeyCode.None;
    char character = char.MinValue;
    bool isKeyDown1 = true;
    while (UnityEngine.Event.PopEvent(current))
    {
      Rect rect = new Rect((float) (Screen.width / 2 - this.m_checkout.CheckoutUi.BrowserWidth / 2), (float) (Screen.height / 2 - this.m_checkout.CheckoutUi.BrowserHeight / 2), (float) this.m_checkout.CheckoutUi.BrowserWidth, (float) this.m_checkout.CheckoutUi.BrowserHeight);
      if (current.isKey)
      {
        modifiers = this.GetModifiers(current);
        isKeyDown1 = current.type == EventType.KeyDown;
        if ((current.modifiers & EventModifiers.FunctionKey) != EventModifiers.None)
        {
          if (!CommerceWrapper.Instance.SendKeyboardInput(current.keyCode, isKeyDown1, (uint) modifiers, current.character))
          {
            Log.Store.PrintWarning("[CheckoutInputManager.OnGui] SendKeyboardInput failed");
            continue;
          }
          continue;
        }
        if (current.keyCode > KeyCode.None)
          keyCode = current.keyCode;
        if (current.character != char.MinValue)
          character = CheckoutInputManager.SwapCharacter(current.character);
      }
      Vector3 mousePosition1 = InputCollection.GetMousePosition();
      Vec2D mousePosition2 = this.GetMousePosition(this.screenSpace.GetScreenRect(), mousePosition1, this.screenSpace.GetScreenSpaceScale());
      if (mousePosition2 != null)
      {
        if (current.isScrollWheel)
        {
          if (current.type == EventType.ScrollWheel && !CommerceWrapper.Instance.SendMouseWheelEvent((int) ((0.0 - (double) current.delta.y) * 40.0), mousePosition2, (uint) modifiers))
            Log.Store.PrintWarning("[CheckoutInputManager.OnGui] SendMouseWheelEvent failed");
        }
        else if (current.type == EventType.MouseDown || current.type == EventType.MouseUp)
        {
          if (!CommerceWrapper.Instance.SendMouseInputEvent(current.type == EventType.MouseDown, current.button, mousePosition2, (uint) modifiers))
            Log.Store.PrintWarning("[CheckoutInputManager.OnGui] SendMouseInputEvent failed");
        }
        else if ((current.type == EventType.MouseEnterWindow || current.type == EventType.MouseMove || this.m_lastMousePosition == null || this.m_lastMousePosition.x != mousePosition2.x || this.m_lastMousePosition.y != mousePosition2.y) && !CommerceWrapper.Instance.SendMouseMoveEvent(mousePosition2, (uint) modifiers))
          Log.Store.PrintWarning("[CheckoutInputManager.OnGui] SendMouseMoveEvent failed");
        this.m_lastMousePosition = mousePosition2;
      }
    }
    CheckoutInputManager.KeyboardEventListener keyboardEventListener;
    if (!this.m_KeyboardEventHandlers.TryGetValue(current.keyCode, out keyboardEventListener))
    {
      if (keyCode <= KeyCode.None && character == char.MinValue)
        return;
      if (character == char.MinValue || character == '\t')
      {
        if ((KeyCode) Helper.KeycodeToVK(keyCode) == keyCode && character == char.MinValue)
        {
          int upper = (int) char.ToUpper(keyCode.ToString()[0]);
        }
        bool isKeyDown2 = current.type == EventType.KeyDown;
        if (!CommerceWrapper.Instance.SendKeyboardInput(keyCode, isKeyDown2, (uint) modifiers, character))
          Log.Store.PrintWarning("[CheckoutInputManager.OnGui] SendKeyboardInput failed");
      }
      if (this.blockedCharacters.Contains(character) || CommerceWrapper.Instance.SendCharacterEvent((int) character, (uint) modifiers))
        return;
      Log.Store.PrintWarning("[CheckoutInputManager.OnGui] SendCharacterEvent failed");
    }
    else
      keyboardEventListener(isKeyDown1);
  }

  private static char SwapCharacter(char character)
  {
    switch (character)
    {
      case '\n':
        return '\r';
      case '\u0019':
        return '\t';
      default:
        return character;
    }
  }

  public delegate void KeyboardEventListener(bool isKeyDown);
}
