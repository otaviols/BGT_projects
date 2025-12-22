using Hearthstone.UI;
using UnityEngine;

public class MulliganButton : MonoBehaviour
{
  public UberText uberText;
  public GameObject buttonContainer;

  public void SetText(string text)
  {
    this.uberText.Text = text;
    this.uberText.UpdateText();
  }

  public void SetEnabled(bool active)
  {
    VisualController component = this.buttonContainer.GetComponent<VisualController>();
    if (active)
      component.SetState("Active");
    else
      component.SetState("Inactive");
  }

  public virtual bool AddEventListener(UIEventType type, UIEvent.Handler handler) => this.buttonContainer.GetComponent<Clickable>().AddEventListener(type, handler);
}
