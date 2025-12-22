using Blizzard.T5.Core;
using Hearthstone;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugTextManager : MonoBehaviour
{
  private static DebugTextManager s_instance;
  private GUIStyle debugTextStyle;
  private List<DebugTextManager.DebugTextRequest> m_textRequests = new List<DebugTextManager.DebugTextRequest>();
  private Map<int, float> m_scrollBarValues = new Map<int, float>();

  public static DebugTextManager Get()
  {
    if ((UnityEngine.Object) DebugTextManager.s_instance == (UnityEngine.Object) null)
    {
      GameObject gameObject = new GameObject();
      DebugTextManager.s_instance = gameObject.AddComponent<DebugTextManager>();
      gameObject.name = "DebugTextManager (Dynamically created)";
      DebugTextManager.s_instance.debugTextStyle = new GUIStyle((GUIStyle) "box");
      DebugTextManager.s_instance.debugTextStyle.fontSize = 12;
      DebugTextManager.s_instance.debugTextStyle.fontStyle = FontStyle.Bold;
      DebugTextManager.s_instance.debugTextStyle.normal.textColor = Color.white;
      DebugTextManager.s_instance.debugTextStyle.alignment = TextAnchor.MiddleCenter;
    }
    return DebugTextManager.s_instance;
  }

  public static Vector2 WorldPosToScreenPos(Vector3 position) => (Vector2) Camera.main.WorldToScreenPoint(position);

  public Vector2 TextSize(string text) => this.debugTextStyle.CalcSize(new GUIContent(text));

  public void DrawDebugText(
    string text,
    Vector3 position,
    float duration = 5f,
    bool screenSpace = false,
    string requestIdentifier = "",
    GUIStyle textStyle = null)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.m_textRequests.Add(new DebugTextManager.DebugTextRequest(text, position, duration, screenSpace, requestIdentifier, textStyle));
  }

  private void LateUpdate()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    this.m_textRequests.RemoveAll((Predicate<DebugTextManager.DebugTextRequest>) (x => (double) x.m_remainingDuration < 0.0));
    this.m_textRequests.ForEach((Action<DebugTextManager.DebugTextRequest>) (x => x.m_remainingDuration -= Time.deltaTime));
  }

  private void OnGUI()
  {
    if (HearthstoneApplication.IsPublic())
      return;
    foreach (DebugTextManager.DebugTextRequest textRequest in this.m_textRequests)
    {
      Vector3 vector3 = textRequest.m_screenSpace ? textRequest.m_position : Camera.main.WorldToScreenPoint(textRequest.m_position);
      Vector2 vector2 = textRequest.m_textStyle != null ? textRequest.m_textStyle.CalcSize(new GUIContent(textRequest.m_text)) : this.debugTextStyle.CalcSize(new GUIContent(textRequest.m_text));
      Rect position = new Rect(vector3.x - vector2.x / 2f, (float) ((double) Screen.height - (double) vector3.y - (double) vector2.y / 2.0), vector2.x, vector2.y);
      if (textRequest.m_fitOnScreen)
      {
        if ((double) position.x < 0.0)
          position.x = 0.0f;
        else if ((double) position.x + (double) vector2.x > (double) Screen.width)
          position.x = (float) Screen.width - vector2.x;
        if ((double) position.y < 0.0)
          position.y = 0.0f;
        else if ((double) position.y + (double) vector2.y > (double) Screen.height)
          position.y = (float) Screen.height - vector2.y;
        if ((double) vector2.y > (double) Screen.height)
        {
          float num1 = 0.0f;
          int key = !string.IsNullOrEmpty(textRequest.m_requestIdentifier) ? textRequest.m_requestIdentifier.GetHashCode() : textRequest.m_text.GetHashCode();
          if (this.m_scrollBarValues.ContainsKey(key))
            num1 = this.m_scrollBarValues[key];
          int x = (int) position.x - 50;
          if (x <= 0)
            x = (int) position.x + (int) vector2.x + 50;
          this.m_scrollBarValues[key] = GUI.VerticalSlider(new Rect((float) x, position.y + 10f, 100f, (float) (Screen.height - 100)), num1, 0.0f, 1f);
          float num2 = vector2.y - (float) Screen.height;
          position.y -= num2 * this.m_scrollBarValues[key];
        }
      }
      if (textRequest.m_textStyle == null)
        GUI.Box(position, textRequest.m_text, this.debugTextStyle);
      else
        GUI.Box(position, textRequest.m_text, textRequest.m_textStyle);
    }
  }

  private class DebugTextRequest
  {
    public string m_text;
    public Vector3 m_position;
    public float m_remainingDuration;
    public bool m_screenSpace;
    public bool m_fitOnScreen;
    public string m_requestIdentifier;
    public GUIStyle m_textStyle;

    public DebugTextRequest(
      string text,
      Vector3 position,
      float duration,
      bool screenSpace,
      string requestIdentifier,
      GUIStyle textStyle = null)
    {
      this.m_text = text;
      this.m_position = position;
      this.m_remainingDuration = duration;
      this.m_screenSpace = screenSpace;
      this.m_fitOnScreen = true;
      this.m_requestIdentifier = requestIdentifier;
      this.m_textStyle = textStyle;
    }
  }
}
