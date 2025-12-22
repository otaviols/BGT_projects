using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FatalErrorDialog : MonoBehaviour
{
  private const float DialogWidth = 600f;
  private const float DialogHeight = 347f;
  private const float DialogPadding = 20f;
  private const float ButtonWidth = 100f;
  private const float ButtonHeight = 31f;
  private GUIStyle m_dialogStyle;
  private string m_text;

  private float DialogTop => (float) (((double) Screen.height - 347.0) / 2.0);

  private float DialogLeft => (float) (((double) Screen.width - 600.0) / 2.0);

  private Rect DialogRect => new Rect(this.DialogLeft, this.DialogTop, 600f, 347f);

  private float ButtonTop => (float) ((double) this.DialogTop + 347.0 - 20.0 - 31.0);

  private float ButtonLeft => (float) (((double) Screen.width - 100.0) / 2.0);

  private Rect ButtonRect => new Rect(this.ButtonLeft, this.ButtonTop, 100f, 31f);

  private void Awake()
  {
    this.BuildText();
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
  }

  private void OnGUI()
  {
    this.InitGUIStyles();
    GUI.Box(this.DialogRect, string.Empty, this.m_dialogStyle);
    GUI.Box(this.DialogRect, this.m_text, this.m_dialogStyle);
    if (!GUI.Button(this.ButtonRect, GameStrings.Get("GLOBAL_EXIT")))
      return;
    FatalErrorMgr.Get().NotifyExitPressed();
  }

  private void InitGUIStyles()
  {
    if (this.m_dialogStyle != null)
      return;
    this.m_dialogStyle = new GUIStyle((GUIStyle) "box")
    {
      clipping = TextClipping.Overflow,
      stretchHeight = true,
      stretchWidth = true,
      wordWrap = true,
      fontSize = 16
    };
  }

  private void OnFatalError(FatalErrorMessage message, object userData)
  {
    FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    this.BuildText();
  }

  private void BuildText()
  {
    if (!FatalErrorMgr.Get().HasError())
    {
      this.m_text = string.Empty;
    }
    else
    {
      FatalErrorMessage[] messages = FatalErrorMgr.Get().GetMessages();
      List<string> stringList = new List<string>();
      for (int index = 0; index < messages.Length; ++index)
      {
        string text = messages[index].m_text;
        if (!stringList.Contains(text))
          stringList.Add(text);
      }
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < stringList.Count; ++index)
      {
        string str = stringList[index];
        stringBuilder.Append(str);
        stringBuilder.Append("\n");
      }
      stringBuilder.Remove(stringBuilder.Length - 1, 1);
      this.m_text = stringBuilder.ToString();
    }
  }
}
