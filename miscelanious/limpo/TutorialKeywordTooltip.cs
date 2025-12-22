using System.Collections;
using UnityEngine;

public class TutorialKeywordTooltip : MonoBehaviour
{
  public UberText m_name;
  public UberText m_body;
  public PlayMakerFSM playMakerComponent;

  public void Initialize(string keywordName, string keywordText)
  {
    this.SetName(keywordName);
    this.SetBodyText(keywordText);
    this.StartCoroutine(this.WaitAFrameBeforeSendingEvent());
  }

  private IEnumerator WaitAFrameBeforeSendingEvent()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TutorialKeywordTooltip tutorialKeywordTooltip = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      tutorialKeywordTooltip.playMakerComponent.SendEvent("Birth");
      iTween.FadeTo(tutorialKeywordTooltip.gameObject, 1f, 0.5f);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    RenderUtils.SetAlpha(tutorialKeywordTooltip.gameObject, 0.0f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public void SetName(string s) => this.m_name.Text = s;

  public void SetBodyText(string s) => this.m_body.Text = s;

  public float GetHeight() => this.GetComponent<Renderer>().bounds.size.z;

  public float GetWidth() => this.GetComponent<Renderer>().bounds.size.x;
}
