using UnityEngine;

public static class ClipboardUtils
{
  public static void CopyToClipboard(string copyText) => GUIUtility.systemCopyBuffer = copyText;

  public static string PastedStringFromClipboard
  {
    get
    {
      TextEditor textEditor = new TextEditor()
      {
        multiline = true
      };
      if (!textEditor.CanPaste())
        return (string) null;
      textEditor.Paste();
      return textEditor.text;
    }
  }
}
