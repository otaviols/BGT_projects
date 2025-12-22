using Hearthstone.UI;
using UnityEngine;

public class ProfileTabDisplay : MonoBehaviour
{
  public const string TURN_ARROW_GLOW_ON = "CODE_TURN_GLOW_ON";
  public const string TURN_ARROW_GLOW_OFF = "CODE_TURN_GLOW_OFF";
  public const string PROFILE_PAGE_ARROW_PRESSED = "PAGE_FLIP01";
  private Widget m_widget;

  private void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == "PAGE_FLIP01"))
        return;
      this.RightArrowPressed();
    }));
    if (this.GetProfilePageArrowGlow() <= 0)
      this.m_widget.TriggerEvent("CODE_TURN_GLOW_ON");
    else
      this.m_widget.TriggerEvent("CODE_TURN_GLOW_OFF");
  }

  private void RightArrowPressed()
  {
    this.SetProfilePageArrowGlow(1);
    this.m_widget.TriggerEvent("CODE_TURN_GLOW_OFF");
  }

  public bool SetProfilePageArrowGlow(int profilePageArrowGlow) => GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.PROGRESSION, GameSaveKeySubkeyId.PROGRESSION_PROFILE_PAGE_HAS_SEEN_ARROW_GLOW, new long[1]
  {
    (long) profilePageArrowGlow
  }));

  public int GetProfilePageArrowGlow()
  {
    long profilePageArrowGlow = 0;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.PROGRESSION, GameSaveKeySubkeyId.PROGRESSION_PROFILE_PAGE_HAS_SEEN_ARROW_GLOW, out profilePageArrowGlow);
    return (int) profilePageArrowGlow;
  }
}
