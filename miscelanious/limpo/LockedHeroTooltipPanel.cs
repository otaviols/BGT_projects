using Hearthstone.UI;
using UnityEngine;

public class LockedHeroTooltipPanel : TooltipPanel
{
  [SerializeField]
  private Widget m_button;
  private TAG_CLASS m_lockedClass;
  private const string LOCKED_HERO_TAKE_ME_THERE = "LOCKED_HERO_TAKE_ME_THERE";

  private void Awake()
  {
    LayerUtils.SetLayer(this.gameObject, GameLayer.Tooltip);
    this.m_scaleToUse = (float) TooltipPanel.GAMEPLAY_SCALE;
    if (!((Object) this.m_button != (Object) null))
      return;
    this.m_button.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleEvent));
  }

  public void SetLockedClass(TAG_CLASS lockedClass) => this.m_lockedClass = lockedClass;

  private void HandleEvent(string eventName)
  {
    if (!(eventName == "LOCKED_HERO_TAKE_ME_THERE"))
      return;
    this.TakeMeThereButtonReleased();
  }

  private void TakeMeThereButtonReleased()
  {
    string str;
    switch (this.m_lockedClass)
    {
      case TAG_CLASS.INVALID:
        return;
      case TAG_CLASS.DEATHKNIGHT:
        str = "hearthstone://adventure/ROTLK";
        break;
      default:
        str = "hearthstone://practice";
        break;
    }
    DeepLinkManager.ExecuteDeepLink(str.Substring("hearthstone://".Length).Split('/'), DeepLinkManager.DeepLinkSource.LOCKED_HERO_TOOLTIP, false);
  }
}
