using Hearthstone.UI;
using System;
using UnityEngine;

public class StoreEmoteOption : MonoBehaviour
{
  [SerializeField]
  private StoreEmoteHandler m_emoteHandler;
  [SerializeField]
  private EmoteType m_EmoteType;
  [SerializeField]
  private GameObject m_bubbleMeshObj;
  [SerializeField]
  private GameObject m_uberTextObj;
  [SerializeField]
  private AsyncReference m_asyncWidgetClickableReference;
  private Vector3 m_startingScale;
  private bool m_shouldBeShowing;
  private bool m_isInitialized;

  private void Awake() => this.Initialize();

  private void Initialize()
  {
    if (this.m_isInitialized)
      return;
    if ((UnityEngine.Object) this.m_emoteHandler == (UnityEngine.Object) null)
      Debug.LogError((object) "StoreEmoteOption: Missing a required reference to an StoreEmoteHandler component");
    if (this.m_asyncWidgetClickableReference == null)
      Debug.LogError((object) "StoreEmoteOption: Missing a required AsyncReference to an clickable widget component");
    if ((UnityEngine.Object) this.m_bubbleMeshObj != (UnityEngine.Object) null)
      this.m_bubbleMeshObj.SetActive(false);
    if ((UnityEngine.Object) this.m_uberTextObj != (UnityEngine.Object) null)
      this.m_uberTextObj.SetActive(false);
    this.m_startingScale = this.transform.localScale;
    this.transform.localScale = Vector3.zero;
    this.m_isInitialized = true;
  }

  private void Start() => this.m_asyncWidgetClickableReference.RegisterReadyListener<Clickable>(new Action<Clickable>(this.OnWidgetClickableReady));

  public void Enable()
  {
    this.Initialize();
    this.m_shouldBeShowing = true;
    if ((UnityEngine.Object) this.m_bubbleMeshObj != (UnityEngine.Object) null)
      this.m_bubbleMeshObj.SetActive(true);
    if ((UnityEngine.Object) this.m_uberTextObj != (UnityEngine.Object) null)
      this.m_uberTextObj.SetActive(true);
    iTween.Stop(this.gameObject);
    this.transform.localScale = Vector3.zero;
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.m_startingScale, (object) "time", (object) 0.5f, (object) "ignoretimescale", (object) true, (object) "easetype", (object) iTween.EaseType.easeOutElastic));
  }

  public void Disable(bool isImmediateHide = false)
  {
    this.Initialize();
    this.m_shouldBeShowing = false;
    iTween.Stop(this.gameObject);
    if (isImmediateHide)
    {
      if ((UnityEngine.Object) this.m_bubbleMeshObj != (UnityEngine.Object) null)
        this.m_bubbleMeshObj.SetActive(false);
      if ((UnityEngine.Object) this.m_uberTextObj != (UnityEngine.Object) null)
        this.m_uberTextObj.SetActive(false);
      this.transform.localScale = Vector3.zero;
    }
    else
      iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "time", (object) 0.1f, (object) "ignoretimescale", (object) true, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncompletetarget", (object) this.gameObject, (object) "oncomplete", (object) "OnFinishHiding"));
  }

  private void OnFinishHiding()
  {
    if (this.m_shouldBeShowing)
      return;
    if ((UnityEngine.Object) this.m_bubbleMeshObj != (UnityEngine.Object) null)
      this.m_bubbleMeshObj.SetActive(false);
    if ((UnityEngine.Object) this.m_uberTextObj != (UnityEngine.Object) null)
      this.m_uberTextObj.SetActive(false);
    this.transform.localScale = Vector3.zero;
  }

  private void OnWidgetClickableReady(Clickable widgetClickable)
  {
    if ((UnityEngine.Object) widgetClickable == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "StoreEmoteOption: Failed to load clickable by reference.");
    }
    else
    {
      widgetClickable.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickableTriggered));
      widgetClickable.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnClickableMouseOut));
      widgetClickable.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnClickableMouseOver));
    }
  }

  private void OnClickableMouseOut(UIEvent e)
  {
    if (e == null || e.GetEventType() != UIEventType.ROLLOUT)
      return;
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.m_startingScale, (object) "time", (object) 0.2f, (object) "ignoretimescale", (object) true));
  }

  private void OnClickableMouseOver(UIEvent e)
  {
    if (e == null || e.GetEventType() != UIEventType.ROLLOVER)
      return;
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) (this.m_startingScale * 1.1f), (object) "time", (object) 0.2f, (object) "ignoretimescale", (object) true));
  }

  private void OnClickableTriggered(UIEvent e)
  {
    if (e == null || e.GetEventType() != UIEventType.RELEASE || this.m_EmoteType == EmoteType.INVALID)
      return;
    if ((UnityEngine.Object) this.m_emoteHandler == (UnityEngine.Object) null)
      Debug.LogError((object) ("StoreEmoteOption: Failed to trigger emote " + this.m_EmoteType.ToString() + " as missing StoreEmoteHandler reference."));
    else
      this.m_emoteHandler.PlayEmote(this.m_EmoteType);
  }
}
