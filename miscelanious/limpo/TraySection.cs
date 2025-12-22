using System;
using System.Collections;
using UnityEngine;

public class TraySection : MonoBehaviour
{
  public GameObject m_door;
  public CollectionDeckBoxVisual m_deckBox;
  public Animator m_deckFX;
  private const float DOOR_ANIM_SPEED = 6f;
  private readonly string DOOR_OPEN_ANIM_NAME = "Deck_DoorOpen";
  private readonly string DOOR_CLOSE_ANIM_NAME = "Deck_DoorClose";
  private static readonly Vector3 DECKBOX_LOCAL_EULER_ANGLES = new Vector3(90f, 180f, 0.0f);
  private bool m_isOpen;
  private bool m_wasTouchModeEnabled;
  private bool m_deckBoxShown;
  private bool m_showDoor = true;
  private Transform m_parent;

  public void ShowDoor(bool show)
  {
    if (!this.m_showDoor)
      show = false;
    this.m_door.gameObject.SetActive(show);
  }

  public bool IsOpen() => this.m_isOpen;

  public Bounds GetDoorBounds() => this.m_door.GetComponent<Renderer>().bounds;

  public void OpenDoor() => this.OpenDoor((TraySection.DelOnDoorStateChangedCallback) null);

  public void OpenDoor(TraySection.DelOnDoorStateChangedCallback callback) => this.OpenDoor(callback, (object) null);

  public void OpenDoor(TraySection.DelOnDoorStateChangedCallback callback, object callbackData) => this.OpenDoor(false, callback, callbackData);

  public void OpenDoorImmediately() => this.OpenDoorImmediately((TraySection.DelOnDoorStateChangedCallback) null);

  public void OpenDoorImmediately(TraySection.DelOnDoorStateChangedCallback callback) => this.OpenDoorImmediately(callback, (object) null);

  public void OpenDoorImmediately(
    TraySection.DelOnDoorStateChangedCallback callback,
    object callbackData)
  {
    this.OpenDoor(true, callback, callbackData);
  }

  public void CloseDoor() => this.CloseDoor((TraySection.DelOnDoorStateChangedCallback) null);

  public void CloseDoor(TraySection.DelOnDoorStateChangedCallback callback) => this.CloseDoor(callback, (object) null);

  public void CloseDoor(TraySection.DelOnDoorStateChangedCallback callback, object callbackData) => this.CloseDoor(false, callback, callbackData);

  public void CloseDoorImmediately() => this.CloseDoorImmediately((TraySection.DelOnDoorStateChangedCallback) null);

  public void CloseDoorImmediately(TraySection.DelOnDoorStateChangedCallback callback) => this.CloseDoorImmediately(callback, (object) null);

  public void CloseDoorImmediately(
    TraySection.DelOnDoorStateChangedCallback callback,
    object callbackData)
  {
    this.CloseDoor(true, callback, callbackData);
  }

  public bool IsDeckBoxShown() => this.m_deckBoxShown;

  public void EnableDoors(bool show) => this.m_showDoor = show;

  public void ShowDeckBox(bool immediate = false, TraySection.DelOnDoorStateChangedCallback callback = null)
  {
    this.gameObject.SetActive(true);
    this.m_deckBoxShown = true;
    if (this.m_showDoor)
      this.m_door.gameObject.SetActive(true);
    this.OpenDoor(immediate, (TraySection.DelOnDoorStateChangedCallback) (_1 =>
    {
      if ((UnityEngine.Object) this.m_deckBox != (UnityEngine.Object) null)
      {
        this.m_deckBox.Show();
        this.m_deckBox.PlayPopUpAnimation((CollectionDeckBoxVisual.DelOnAnimationFinished) (_2 =>
        {
          this.m_door.gameObject.SetActive(false);
          if (callback == null)
            return;
          callback((object) this);
        }));
      }
      else
      {
        this.m_door.gameObject.SetActive(false);
        if (callback == null)
          return;
        callback((object) this);
      }
    }), (object) null);
  }

  public void ShowDeckBoxNoAnim()
  {
    this.gameObject.SetActive(true);
    this.m_deckBoxShown = true;
    this.m_deckBox.Show();
  }

  public void HideDeckBox(bool immediate = false, TraySection.DelOnDoorStateChangedCallback callback = null)
  {
    this.m_deckBoxShown = false;
    this.CloseDoor(immediate, (TraySection.DelOnDoorStateChangedCallback) (_1 =>
    {
      this.m_door.gameObject.SetActive(this.m_showDoor);
      if ((UnityEngine.Object) this.m_deckBox != (UnityEngine.Object) null)
      {
        this.m_deckBox.PlayPopDownAnimation((CollectionDeckBoxVisual.DelOnAnimationFinished) (_2 =>
        {
          this.m_deckBox.Hide();
          if (callback == null)
            return;
          callback((object) this);
        }));
      }
      else
      {
        if (callback == null)
          return;
        callback((object) this);
      }
    }), (object) null);
  }

  public void MoveDeckBoxToEditPosition(
    Vector3 worldSpacePosition,
    float time,
    TraySection.DelOnDoorStateChangedCallback callback = null)
  {
    if ((UnityEngine.Object) this.m_deckBox == (UnityEngine.Object) null)
      return;
    this.m_deckBox.DisableButtonAnimation();
    this.m_door.gameObject.SetActive(this.m_showDoor);
    this.CloseDoor();
    Vector3 localSpacePosition = this.m_deckBox.transform.parent.InverseTransformPoint(worldSpacePosition);
    this.m_deckBox.PlayScaleUpAnimation((CollectionDeckBoxVisual.DelOnAnimationFinished) (_1 => iTween.MoveTo(this.m_deckBox.gameObject, iTween.Hash((object) "position", (object) localSpacePosition, (object) "islocal", (object) true, (object) nameof (time), (object) time, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) (Action<object>) (_2 =>
    {
      if (callback == null)
        return;
      callback((object) this);
    })))));
  }

  public void MoveDeckBoxBackToOriginalPosition(
    float time,
    TraySection.DelOnDoorStateChangedCallback callback = null)
  {
    if ((UnityEngine.Object) this.m_deckBox == (UnityEngine.Object) null)
      return;
    this.OpenDoor((TraySection.DelOnDoorStateChangedCallback) (_1 => this.m_door.gameObject.SetActive(false)));
    this.StartCoroutine(this.MoveToOriginalPosition(time, callback));
  }

  private IEnumerator MoveToOriginalPosition(
    float time,
    TraySection.DelOnDoorStateChangedCallback callback = null)
  {
    TraySection traySection = this;
    float timeLive = 0.0f;
    Vector3 startPos = traySection.m_deckBox.transform.position;
    Vector3 position1 = traySection.transform.GetChild(0).position;
    while ((double) timeLive < (double) time)
    {
      Vector3 position2 = traySection.m_parent.position;
      position2.y += 3.238702f;
      traySection.m_deckBox.transform.position = Vector3.Lerp(startPos, position2, timeLive / time);
      timeLive += Time.deltaTime;
      yield return (object) 0;
    }
    Vector3 position3 = traySection.m_parent.position;
    position3.y += 3.238702f;
    traySection.m_deckBox.transform.position = position3;
    traySection.m_deckBox.transform.parent = traySection.m_parent;
    traySection.m_deckBox.PlayScaleDownAnimation((CollectionDeckBoxVisual.DelOnAnimationFinished) (_2 =>
    {
      if (callback != null)
        callback((object) this);
      this.m_deckBox.EnableButtonAnimation();
      this.m_door.gameObject.SetActive(false);
    }));
  }

  public void FlipDeckBoxHalfOverToShow(
    float animTime,
    TraySection.DelOnDoorStateChangedCallback callback = null)
  {
    this.m_deckBox.gameObject.SetActive(true);
    this.m_deckBox.transform.localEulerAngles = new Vector3(0.0f, 180f, 0.0f);
    SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_new_deck_edge_flips.prefab:9af01c3ef83086746810abe60b8666fd", this.gameObject);
    iTween.StopByName(this.m_deckBox.gameObject, "rotation");
    iTween.RotateTo(this.m_deckBox.gameObject, iTween.Hash((object) "rotation", (object) TraySection.DECKBOX_LOCAL_EULER_ANGLES, (object) "isLocal", (object) true, (object) "time", (object) animTime, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) (Action<object>) (_1 =>
    {
      if (callback == null)
        return;
      callback((object) this);
    }), (object) "name", (object) "rotation"));
  }

  public void ClearDeckInfo()
  {
    if ((UnityEngine.Object) this.m_deckBox == (UnityEngine.Object) null)
      return;
    this.m_deckBox.SetDeckName("");
    this.m_deckBox.SetDeckID(-1L);
  }

  public bool HideIfNotInBounds(Bounds bounds)
  {
    UIBScrollableItem component = this.GetComponent<UIBScrollableItem>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "UIBScrollableItem not found on a TraySection! This section may not be hidden properly while entering or exiting Collection Manager!");
      return false;
    }
    Bounds bounds1 = new Bounds();
    Vector3 min;
    Vector3 max;
    component.GetWorldBounds(out min, out max);
    bounds1.SetMinMax(min, max);
    if (bounds.Intersects(bounds1))
      return false;
    this.gameObject.SetActive(false);
    return true;
  }

  private void Awake()
  {
    if ((UnityEngine.Object) this.m_deckBox != (UnityEngine.Object) null)
    {
      this.m_deckBox.transform.localPosition = CollectionDeckBoxVisual.POPPED_DOWN_LOCAL_POS;
      this.m_deckBox.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
      this.m_deckBox.transform.localEulerAngles = new Vector3(90f, 180f, 0.0f);
    }
    this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
    UIBScrollableItem component = this.GetComponent<UIBScrollableItem>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.SetCustomActiveState(new UIBScrollableItem.ActiveStateCallback(this.IsDeckBoxShown));
    this.m_parent = this.m_deckBox.transform.parent;
  }

  private void Update()
  {
    if (this.m_wasTouchModeEnabled == UniversalInputManager.Get().IsTouchMode())
      return;
    this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
  }

  private void OpenDoor(
    bool isImmediate,
    TraySection.DelOnDoorStateChangedCallback callback,
    object callbackData)
  {
    if (this.m_isOpen)
    {
      if (callback == null)
        return;
      callback(callbackData);
    }
    else
    {
      this.m_isOpen = true;
      Animation component = this.m_door.GetComponent<Animation>();
      component[this.DOOR_OPEN_ANIM_NAME].time = isImmediate ? component[this.DOOR_OPEN_ANIM_NAME].length : 0.0f;
      component[this.DOOR_OPEN_ANIM_NAME].speed = 6f;
      this.PlayDoorAnimation(this.DOOR_OPEN_ANIM_NAME, callback, callbackData);
    }
  }

  private void CloseDoor(
    bool isImmediate,
    TraySection.DelOnDoorStateChangedCallback callback,
    object callbackData)
  {
    if (!this.m_isOpen)
    {
      if (callback == null)
        return;
      callback(callbackData);
    }
    else
    {
      this.m_isOpen = false;
      Animation component = this.m_door.GetComponent<Animation>();
      component[this.DOOR_CLOSE_ANIM_NAME].time = isImmediate ? component[this.DOOR_CLOSE_ANIM_NAME].length : 0.0f;
      component[this.DOOR_CLOSE_ANIM_NAME].speed = 6f;
      this.PlayDoorAnimation(this.DOOR_CLOSE_ANIM_NAME, callback, callbackData);
    }
  }

  private void PlayDoorAnimation(
    string animationName,
    TraySection.DelOnDoorStateChangedCallback callback,
    object callbackData)
  {
    this.m_door.GetComponent<Animation>().Play(animationName);
    TraySection.OnDoorStateChangedCallbackData changedCallbackData = new TraySection.OnDoorStateChangedCallbackData()
    {
      m_callback = callback,
      m_callbackData = callbackData,
      m_animationName = animationName
    };
    this.StopCoroutine("WaitThenCallDoorAnimationCallback");
    this.StartCoroutine("WaitThenCallDoorAnimationCallback", (object) changedCallbackData);
  }

  private IEnumerator WaitThenCallDoorAnimationCallback(
    TraySection.OnDoorStateChangedCallbackData callbackData)
  {
    if (callbackData.m_callback != null)
    {
      Animation component = this.m_door.GetComponent<Animation>();
      yield return (object) new WaitForSeconds(component[callbackData.m_animationName].length / component[callbackData.m_animationName].speed);
      callbackData.m_callback(callbackData.m_callbackData);
    }
  }

  public delegate void DelOnDoorStateChangedCallback(object callbackData);

  private class OnDoorStateChangedCallbackData
  {
    public TraySection.DelOnDoorStateChangedCallback m_callback;
    public object m_callbackData;
    public string m_animationName;
  }
}
