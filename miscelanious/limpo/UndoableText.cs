using System.Collections.Generic;

public class UndoableText
{
  private string currentText = string.Empty;
  private Stack<UndoableText.TextChange> UndoStack = new Stack<UndoableText.TextChange>();
  private Stack<UndoableText.TextChange> RedoStack = new Stack<UndoableText.TextChange>();

  public void ProcessChange(string newText)
  {
    UndoableText.TextChange textChange1 = this.SimpleDiff(this.currentText, newText);
    if (textChange1 == null)
      return;
    UndoableText.TextChange textChange2 = this.UndoStack.Count == 0 ? (UndoableText.TextChange) null : this.UndoStack.Peek();
    this.currentText = newText;
    if (textChange2 != null && textChange1.changeType == textChange2.changeType)
    {
      if (textChange1.changeType == UndoableText.TextChange.Type.Addition && textChange2.index + textChange2.text.Length == textChange1.index)
      {
        textChange2.text += textChange1.text;
        return;
      }
      if (textChange1.changeType == UndoableText.TextChange.Type.Deletion && textChange2.index - textChange1.text.Length == textChange1.index)
      {
        textChange2.index = textChange1.index;
        textChange2.text = textChange1.text + textChange2.text;
        return;
      }
    }
    this.UndoStack.Push(textChange1);
    this.RedoStack.Clear();
  }

  private UndoableText.TextChange SimpleDiff(string text1, string text2)
  {
    if (text1 == text2)
      return (UndoableText.TextChange) null;
    int num1 = -1;
    int startIndex = -1;
    int num2 = -1;
    int num3 = -1;
    for (int index = 0; index < text1.Length && index < text2.Length; ++index)
    {
      if ((int) text1[index] != (int) text2[index])
      {
        num1 = index;
        startIndex = index;
        break;
      }
    }
    if (num1 == -1)
    {
      if (text1.Length < text2.Length)
        return new UndoableText.TextChange(UndoableText.TextChange.Type.Addition, text1.Length, text2.Substring(text1.Length));
      return text1.Length > text2.Length ? new UndoableText.TextChange(UndoableText.TextChange.Type.Deletion, text2.Length, text1.Substring(text2.Length)) : (UndoableText.TextChange) null;
    }
    for (int index = 0; index < text1.Length - num1 && index < text2.Length - startIndex; ++index)
    {
      if ((int) text1[text1.Length - index - 1] != (int) text2[text2.Length - index - 1])
      {
        num2 = text1.Length - index;
        num3 = text2.Length - index;
        break;
      }
    }
    if (num2 == -1)
    {
      if (text1.Length < text2.Length)
      {
        num2 = num1;
        num3 = num1 + (text2.Length - text1.Length);
      }
      else if (text1.Length > text2.Length)
      {
        num2 = startIndex + (text1.Length - text2.Length);
        num3 = startIndex;
      }
    }
    string text3 = text1.Substring(num1, num2 - num1);
    string text4 = text2.Substring(startIndex, num3 - startIndex);
    if (string.IsNullOrEmpty(text3))
      return new UndoableText.TextChange(UndoableText.TextChange.Type.Addition, num1, text4);
    if (string.IsNullOrEmpty(text4))
      return new UndoableText.TextChange(UndoableText.TextChange.Type.Deletion, num1, text3);
    this.UndoStack.Push(new UndoableText.TextChange(UndoableText.TextChange.Type.Deletion, num1, text3));
    return new UndoableText.TextChange(UndoableText.TextChange.Type.Addition, num1, text4);
  }

  public string Undo()
  {
    if (this.UndoStack.Count > 0)
    {
      UndoableText.TextChange textChange = this.UndoStack.Pop();
      this.RedoStack.Push(textChange);
      switch (textChange.changeType)
      {
        case UndoableText.TextChange.Type.Addition:
          this.DeletionOperation(textChange);
          break;
        case UndoableText.TextChange.Type.Deletion:
          this.AdditionOperation(textChange);
          break;
      }
    }
    return this.currentText;
  }

  public string Redo()
  {
    if (this.RedoStack.Count > 0)
    {
      UndoableText.TextChange textChange = this.RedoStack.Pop();
      this.UndoStack.Push(textChange);
      switch (textChange.changeType)
      {
        case UndoableText.TextChange.Type.Addition:
          this.AdditionOperation(textChange);
          break;
        case UndoableText.TextChange.Type.Deletion:
          this.DeletionOperation(textChange);
          break;
      }
    }
    return this.currentText;
  }

  private void AdditionOperation(UndoableText.TextChange textChange) => this.currentText = this.currentText.Insert(textChange.index, textChange.text);

  private void DeletionOperation(UndoableText.TextChange textChange) => this.currentText = this.currentText.Remove(textChange.index, textChange.text.Length);

  public class TextChange
  {
    public UndoableText.TextChange.Type changeType;
    public int index;
    public string text;

    public TextChange(UndoableText.TextChange.Type changeType, int index, string text)
    {
      this.changeType = changeType;
      this.index = index;
      this.text = text;
    }

    public enum Type
    {
      Addition,
      Deletion,
    }
  }
}
