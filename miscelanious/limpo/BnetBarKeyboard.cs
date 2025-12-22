using Blizzard.T5.MaterialService.Extensions;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;

public class BnetBarKeyboard : PegUIElement
{
  public Color m_highlight;
  public Color m_origColor;
  private List<OnKeyboardPressed> m_keyboardPressedListeners = new List<OnKeyboardPressed>();

  public void ShowHighlight(bool show)
  {
    Color color = this.m_origColor;
    if (show)
      color = this.m_highlight;
    this.gameObject.GetComponent<Renderer>().GetMaterial().SetColor("_Color", color);
  }

  protected override void OnPress()
  {
    ServiceManager.Get<ITouchScreenService>().ShowKeyboard();
    foreach (OnKeyboardPressed onKeyboardPressed in this.m_keyboardPressedListeners.ToArray())
      onKeyboardPressed();
  }

  protected override void OnOver(PegUIElement.InteractionState oldState) => this.ShowHighlight(true);

  protected override void OnOut(PegUIElement.InteractionState oldState) => this.ShowHighlight(false);

  public void RegisterKeyboardPressedListener(OnKeyboardPressed listener)
  {
    if (this.m_keyboardPressedListeners.Contains(listener))
      return;
    this.m_keyboardPressedListeners.Add(listener);
  }

  public void UnregisterKeyboardPressedListener(OnKeyboardPressed listener) => this.m_keyboardPressedListeners.Remove(listener);
}
