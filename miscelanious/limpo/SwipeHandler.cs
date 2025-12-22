using UnityEngine;

public class SwipeHandler : PegUICustomBehavior
{
  public Transform m_upperLeftBone;
  public Transform m_lowerRightBone;
  private const float SWIPE_DETECT_DURATION = 0.1f;
  private const float SWIPE_DETECT_WIDTH = 0.09f;
  private const float SWIPE_FROM_TARGET_PENALTY = 0.035f;
  private float m_swipeDetectTimer;
  private bool m_checkingPossibleSwipe;
  private Vector3 m_swipeStart;
  private PegUIElement m_swipeElement;
  private Rect m_swipeRect;
  private static SwipeHandler.DelSwipeListener m_swipeListener;

  public override bool UpdateUI() => UniversalInputManager.Get().IsTouchMode() && this.HandleSwipeGesture();

  private bool InSwipeRect(Vector2 v) => (double) v.x >= (double) this.m_swipeRect.x && (double) v.x <= (double) this.m_swipeRect.x + (double) this.m_swipeRect.width && (double) v.y >= (double) this.m_swipeRect.y && (double) v.y <= (double) this.m_swipeRect.y + (double) this.m_swipeRect.height;

  private bool CheckSwipe()
  {
    float f = this.m_swipeStart.x - InputCollection.GetMousePosition().x;
    float num = (float) Screen.width * (float) (0.0900000035762787 + ((Object) this.m_swipeElement != (Object) null ? 0.0350000001490116 : 0.0));
    if ((double) Mathf.Abs(f) <= (double) num)
      return false;
    SwipeHandler.SWIPE_DIRECTION direction = (double) f >= 0.0 ? SwipeHandler.SWIPE_DIRECTION.LEFT : SwipeHandler.SWIPE_DIRECTION.RIGHT;
    if (SwipeHandler.m_swipeListener != null)
      SwipeHandler.m_swipeListener(direction);
    return true;
  }

  private bool HandleSwipeGesture()
  {
    this.m_swipeRect = CameraUtils.CreateGUIScreenRect(Camera.main, (Component) this.m_upperLeftBone, (Component) this.m_lowerRightBone);
    if (InputCollection.GetMouseButtonDown(0) && this.InSwipeRect((Vector2) InputCollection.GetMousePosition()))
    {
      this.m_checkingPossibleSwipe = true;
      this.m_swipeDetectTimer = 0.0f;
      this.m_swipeStart = InputCollection.GetMousePosition();
      this.m_swipeElement = PegUI.Get().FindHitElement();
      return true;
    }
    if (this.m_checkingPossibleSwipe)
    {
      this.m_swipeDetectTimer += Time.deltaTime;
      if (InputCollection.GetMouseButtonUp(0))
      {
        this.m_checkingPossibleSwipe = false;
        if (!this.CheckSwipe() && (Object) this.m_swipeElement != (Object) null && (Object) this.m_swipeElement == (Object) PegUI.Get().FindHitElement())
        {
          this.m_swipeElement.TriggerPress();
          this.m_swipeElement.TriggerRelease();
        }
        return true;
      }
      if ((double) this.m_swipeDetectTimer < 0.100000001490116)
        return true;
      this.m_checkingPossibleSwipe = false;
      if (this.CheckSwipe())
        return true;
      if ((Object) this.m_swipeElement != (Object) null)
        PegUI.Get().DoMouseDown(this.m_swipeElement, this.m_swipeStart);
    }
    return false;
  }

  public void RegisterSwipeListener(SwipeHandler.DelSwipeListener listener) => SwipeHandler.m_swipeListener = listener;

  public enum SWIPE_DIRECTION
  {
    RIGHT,
    LEFT,
  }

  public delegate void DelSwipeListener(SwipeHandler.SWIPE_DIRECTION direction);
}
