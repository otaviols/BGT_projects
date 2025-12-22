using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SearchResultItem : MonoBehaviour
{
  public string m_text;
  public string m_card;
  public bool m_showCloseButton;
  private Text m_textElement;
  private Button m_closeButtonElement;

  public event Action OnClose;

  private void Start()
  {
    this.m_textElement = this.transform.Find("Text").GetComponent<Text>();
    this.m_textElement.text = this.m_text;
    this.m_closeButtonElement = this.transform.Find("CloseButton").GetComponent<Button>();
    if (this.m_showCloseButton)
    {
      this.m_closeButtonElement.gameObject.SetActive(true);
      this.m_closeButtonElement.onClick.AddListener((UnityAction) (() => this.OnClose()));
    }
    else
      this.m_closeButtonElement.gameObject.SetActive(false);
  }
}
