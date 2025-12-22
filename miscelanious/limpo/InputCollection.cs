using System.Collections.Generic;
using UnityEngine;

public static class InputCollection
{
  private static List<IInput> m_Inputs = new List<IInput>();

  static InputCollection() => InputCollection.m_Inputs.Add((IInput) new UnityInput());

  public static Vector3 GetMousePosition()
  {
    for (int index = 0; index < InputCollection.m_Inputs.Count; ++index)
    {
      Vector3 position;
      if (InputCollection.m_Inputs[index].GetMousePosition(out position))
        return position;
    }
    return Vector3.zero;
  }

  public static bool GetAnyKey()
  {
    for (int index = 0; index < InputCollection.m_Inputs.Count; ++index)
    {
      bool anyKey;
      if (InputCollection.m_Inputs[index].GetAnyKey(out anyKey))
        return anyKey;
    }
    return false;
  }

  public static bool GetKey(KeyCode keycode)
  {
    for (int index = 0; index < InputCollection.m_Inputs.Count; ++index)
    {
      bool key;
      if (InputCollection.m_Inputs[index].GetKey(keycode, out key))
        return key;
    }
    return false;
  }

  public static bool GetKeyDown(KeyCode keycode)
  {
    for (int index = 0; index < InputCollection.m_Inputs.Count; ++index)
    {
      bool keyDown;
      if (InputCollection.m_Inputs[index].GetKeyDown(keycode, out keyDown))
        return keyDown;
    }
    return false;
  }

  public static bool GetKeyUp(KeyCode keycode)
  {
    for (int index = 0; index < InputCollection.m_Inputs.Count; ++index)
    {
      bool keyUp;
      if (InputCollection.m_Inputs[index].GetKeyUp(keycode, out keyUp))
        return keyUp;
    }
    return false;
  }

  public static bool GetMouseButton(int button)
  {
    for (int index = 0; index < InputCollection.m_Inputs.Count; ++index)
    {
      bool mouseButton;
      if (InputCollection.m_Inputs[index].GetMouseButton(button, out mouseButton))
        return mouseButton;
    }
    return false;
  }

  public static bool GetMouseButtonDown(int button)
  {
    for (int index = 0; index < InputCollection.m_Inputs.Count; ++index)
    {
      bool mouseButtonDown;
      if (InputCollection.m_Inputs[index].GetMouseButtonDown(button, out mouseButtonDown))
        return mouseButtonDown;
    }
    return false;
  }

  public static bool GetMouseButtonUp(int button)
  {
    for (int index = 0; index < InputCollection.m_Inputs.Count; ++index)
    {
      bool mouseButtonUp;
      if (InputCollection.m_Inputs[index].GetMouseButtonUp(button, out mouseButtonUp))
        return mouseButtonUp;
    }
    return false;
  }
}
