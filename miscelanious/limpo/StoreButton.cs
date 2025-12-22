using System;
using System.Collections;
using UnityEngine;

public class StoreButton : PegUIElement
{
  public GameObject m_storeClosed;
  public UberText m_storeClosedText;
  public UberText m_storeText;
  public HighlightState m_highlightState;
  public GameObject m_highlight;
  private DateTime m_hubStartTime = DateTime.Now;
  private ShopAvailabilityError m_lastAvailabilityError;
  private StoreButton.State m_state;
  private const float SHOP_POLL_INTERVAL = 3f;

  protected override void Awake()
  {
    base.Awake();
    this.m_storeText.Text = GameStrings.Get("GLUE_STORE_OPEN_BUTTON_TEXT");
    this.m_storeClosedText.Text = GameStrings.Get("GLUE_STORE_CLOSED_BUTTON_TEXT");
  }

  private void Start()
  {
    this.m_storeClosed.SetActive(!StoreManager.Get().IsOpen());
    StoreManager.Get().RegisterStatusChangedListener(new Action<bool>(this.OnStoreStatusChanged));
    this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnButtonOver));
    this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonOut));
    if (SoundManager.Get() != null)
    {
      SoundManager.Get().Load((AssetReference) "store_button_mouse_over.prefab:11c9392d3449f064cb60420a61398732");
      SoundManager.Get().Load((AssetReference) "Store_window_shrink.prefab:b68247126e211224e8a904142d2a9895");
    }
    this.StartCoroutine(this.PollShopStatusForTelemetry());
  }

  public void Unload()
  {
    this.SetEnabled(false);
    StoreManager.Get().RemoveStatusChangedListener(new Action<bool>(this.OnStoreStatusChanged));
  }

  public bool IsVisualClosed() => (UnityEngine.Object) this.m_storeClosed != (UnityEngine.Object) null && this.m_storeClosed.activeInHierarchy;

  public StoreButton.State GetState() => this.m_state;

  public void UpdateState(StoreButton.State state)
  {
    this.m_state = state;
    if (state == StoreButton.State.SHOWN)
    {
      this.gameObject.SetActive(true);
    }
    else
    {
      if (state != StoreButton.State.HIDDEN)
        return;
      this.gameObject.SetActive(false);
    }
  }

  private void OnButtonOver(UIEvent e)
  {
    if (!this.IsVisualClosed())
      SoundManager.Get().LoadAndPlay((AssetReference) "store_button_mouse_over.prefab:11c9392d3449f064cb60420a61398732", this.gameObject);
    if ((UnityEngine.Object) this.m_highlightState != (UnityEngine.Object) null)
      this.m_highlightState.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
    if ((UnityEngine.Object) this.m_highlight != (UnityEngine.Object) null)
      this.m_highlight.SetActive(true);
    TooltipZone component = this.GetComponent<TooltipZone>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_STORE_HEADLINE"), GameStrings.Get("GLUE_TOOLTIP_BUTTON_STORE_DESC"));
  }

  private void OnButtonOut(UIEvent e)
  {
    if ((UnityEngine.Object) this.m_highlightState != (UnityEngine.Object) null)
      this.m_highlightState.ChangeState(ActorStateType.HIGHLIGHT_OFF);
    if ((UnityEngine.Object) this.m_highlight != (UnityEngine.Object) null)
      this.m_highlight.SetActive(false);
    TooltipZone component = this.GetComponent<TooltipZone>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.HideTooltip();
  }

  private void OnStoreStatusChanged(bool isOpen)
  {
    this.SendShopStatusTelemetry();
    if (!((UnityEngine.Object) this.m_storeClosed != (UnityEngine.Object) null))
      return;
    this.m_storeClosed.SetActive(!isOpen);
  }

  private IEnumerator PollShopStatusForTelemetry()
  {
    while (SceneMgr.Get().GetMode() != SceneMgr.Mode.HUB)
      yield return (object) null;
    this.m_hubStartTime = DateTime.Now;
    while (this.m_lastAvailabilityError != ShopAvailabilityError.NO_ERROR)
    {
      yield return (object) new WaitForSecondsRealtime(3f);
      this.SendShopStatusTelemetry();
    }
  }

  private void SendShopStatusTelemetry()
  {
    double timeInHubSec = 0.0;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.HUB)
      timeInHubSec = (DateTime.Now - this.m_hubStartTime).TotalSeconds;
    ShopAvailabilityError availabilityError = StoreManager.Get().GetStoreAvailabilityError();
    if (this.m_lastAvailabilityError == availabilityError)
      return;
    this.m_lastAvailabilityError = availabilityError;
    TelemetryManager.Client().SendShopStatus(availabilityError.ToString(), timeInHubSec);
  }

  public enum State
  {
    SHOWN,
    HIDDEN,
  }
}
