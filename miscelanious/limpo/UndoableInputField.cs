using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (InputField))]
public class UndoableInputField : MonoBehaviour
{
  private InputField inputField;
  private UndoableText text;

  private void Awake()
  {
    this.inputField = this.GetComponent<InputField>();
    this.text = new UndoableText();
  }

  private void Update()
  {
    if (!((Object) this.inputField != (Object) null) || !this.inputField.isFocused)
      return;
    if (this.IsModifierKeyDown && Input.GetKeyDown(KeyCode.Z))
      this.inputField.text = this.text.Undo();
    else if (this.IsModifierKeyDown && Input.GetKeyDown(KeyCode.Y))
    {
      this.inputField.text = this.text.Redo();
    }
    else
    {
      if (!Input.anyKeyDown)
        return;
      this.text.ProcessChange(this.inputField.text);
    }
  }

  private bool IsModifierKeyDown => Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightCommand) || Input.GetKey(KeyCode.LeftCommand);
}
