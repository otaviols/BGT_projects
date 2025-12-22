using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UGUIButton : MonoBehaviour
{
  [SerializeField]
  private Text m_buttonText;
  [SerializeField]
  private Button m_button;

  public void SetupButton(string text, Action callback, Action closeAction)
  {
    if ((UnityEngine.Object) this.m_buttonText != (UnityEngine.Object) null)
      this.m_buttonText.text = text;
    if (!((UnityEngine.Object) this.m_button != (UnityEngine.Object) null))
      return;
    this.m_button.onClick.RemoveAllListeners();
    this.m_button.onClick.AddListener((UnityAction) (() =>
    {
      if (closeAction != null)
        closeAction();
      if (callback == null)
        return;
      callback();
    }));
  }
}
