using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using System;
using System.Collections;
using UnityEngine;

public class Notification : MonoBehaviour
{
  public bool rotate180InGameplay;
  public UberText speechUberText;
  public UberText headlineUberText;
  public GameObject upperLeftBubble;
  public GameObject bottomLeftBubble;
  public GameObject upperRightBubble;
  public GameObject bottomRightBubble;
  public GameObject leftBubble;
  public GameObject rightBubble;
  public GameObject bounceObject;
  public GameObject fadeArrowObject;
  public GameObject leftPopupArrow;
  public GameObject rightPopupArrow;
  public GameObject bottomPopupArrow;
  public GameObject topPopupArrow;
  public GameObject bottomLeftPopupArrow;
  public GameObject bottomRightPopupArrow;
  public GameObject topRightPopupArrow;
  public GameObject topLeftPopupArrow;
  public GameObject winStreakEmote;
  public GameObject tripleEmote;
  public GameObject techLevelEmote;
  public GameObject bgEmote01;
  public GameObject bgEmote02;
  public GameObject bgEmote03;
  public GameObject bgEmote04;
  public GameObject bgEmote05;
  public GameObject bgEmote06;
  public GameObject bananaEmote;
  public GameObject heroBuddyEmote;
  public GameObject doubleHeroBuddyEmote;
  public GameObject questEmote;
  public Spell showEvent;
  public Spell destroyEvent;
  public PegUIElement clickOff;
  public BoxCollider clickBlocker;
  public bool ignoreAudioOnDestroy;
  public MeshRenderer artOverlay;
  public Material swapMaterial;
  public Action<int> OnFinishDeathState;
  public Action<Notification> OnDestroyCallback;
  private const float BOUNCE_SPEED = 0.75f;
  private const float FADE_SPEED = 0.5f;
  private const float FADE_PAUSE = 0.85f;
  private const int MAX_CHARACTERS = 20;
  private const int MAX_CHARACTERS_IN_DIALOG = 28;
  public const float DEATH_ANIMATION_DURATION = 0.5f;
  private bool isDying;
  private AudioSource m_accompaniedAudio;
  private Notification.SpeechBubbleDirection m_bubbleDirection;
  private Vector3 m_initialScale;
  private GameObject m_parentOffsetObject;
  private Map<Notification.SpeechBubbleDirection, Vector3> m_speechBubbleScales = new Map<Notification.SpeechBubbleDirection, Vector3>();
  private Vector3 m_localPosition = Vector3.zero;
  private Vector3 m_hiddenPosition = new Vector3(999f, 999f, 999f);
  public int notificationGroup;
  private bool m_hiding;
  private bool m_shrunk;

  public string PrefabPath { get; set; }

  public bool PersistCharacter { get; set; }

  public bool ShowWithExistingPopups { get; set; }

  private void Start()
  {
    foreach (Notification.SpeechBubbleDirection speechBubbleDirection in Enum.GetValues(typeof (Notification.SpeechBubbleDirection)))
    {
      GameObject speechBubble = this.GetSpeechBubble(speechBubbleDirection);
      if ((UnityEngine.Object) speechBubble != (UnityEngine.Object) null)
        this.m_speechBubbleScales.Add(speechBubbleDirection, speechBubble.transform.localScale);
    }
  }

  private void LateUpdate()
  {
    if ((UnityEngine.Object) this.upperLeftBubble != (UnityEngine.Object) null && (UnityEngine.Object) this.upperRightBubble != (UnityEngine.Object) null && (UnityEngine.Object) this.bottomLeftBubble != (UnityEngine.Object) null && (UnityEngine.Object) this.bottomRightBubble != (UnityEngine.Object) null)
      this.gameObject.transform.rotation = Quaternion.identity;
    bool isShowing = PopupDisplayManager.Get().IsShowing;
    if (isShowing && !this.m_hiding && !this.ShowWithExistingPopups)
    {
      Debug.LogFormat("Hiding notification {0} because something else is being shown.", (object) this.gameObject.name);
      this.m_hiding = true;
      this.m_localPosition = this.transform.localPosition;
      this.transform.localPosition = this.m_hiddenPosition;
    }
    else
    {
      if (isShowing || !this.m_hiding)
        return;
      this.m_hiding = false;
      this.transform.localPosition = this.m_localPosition;
    }
  }

  private void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.m_accompaniedAudio && !this.ignoreAudioOnDestroy && SoundManager.Get() != null)
      SoundManager.Get().Destroy(this.m_accompaniedAudio);
    if ((UnityEngine.Object) this.m_parentOffsetObject != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_parentOffsetObject);
    if (this.OnDestroyCallback == null)
      return;
    this.OnDestroyCallback(this);
  }

  public void ChangeText(string newText) => this.speechUberText.Text = newText;

  public void ChangeEmote(NotificationManager.VisualEmoteType emoteType)
  {
    this.techLevelEmote.SetActive(false);
    this.tripleEmote.SetActive(false);
    this.winStreakEmote.SetActive(false);
    this.bgEmote01.SetActive(false);
    this.bgEmote02.SetActive(false);
    this.bgEmote03.SetActive(false);
    this.bgEmote04.SetActive(false);
    this.bgEmote05.SetActive(false);
    this.bgEmote06.SetActive(false);
    this.questEmote.SetActive(false);
    switch (emoteType)
    {
      case NotificationManager.VisualEmoteType.HOT_STREAK:
        this.winStreakEmote.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.TRIPLE:
        this.tripleEmote.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.TECH_UP_01:
        this.techLevelEmote.SetActive(true);
        this.UpdateTechLevelPlaymaker(1);
        break;
      case NotificationManager.VisualEmoteType.TECH_UP_02:
        this.techLevelEmote.SetActive(true);
        this.UpdateTechLevelPlaymaker(2);
        break;
      case NotificationManager.VisualEmoteType.TECH_UP_03:
        this.techLevelEmote.SetActive(true);
        this.UpdateTechLevelPlaymaker(3);
        break;
      case NotificationManager.VisualEmoteType.TECH_UP_04:
        this.techLevelEmote.SetActive(true);
        this.UpdateTechLevelPlaymaker(4);
        break;
      case NotificationManager.VisualEmoteType.TECH_UP_05:
        this.techLevelEmote.SetActive(true);
        this.UpdateTechLevelPlaymaker(5);
        break;
      case NotificationManager.VisualEmoteType.TECH_UP_06:
        this.techLevelEmote.SetActive(true);
        this.UpdateTechLevelPlaymaker(6);
        break;
      case NotificationManager.VisualEmoteType.BATTLEGROUNDS_01:
        this.bgEmote01.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.BATTLEGROUNDS_02:
        this.bgEmote02.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.BATTLEGROUNDS_03:
        this.bgEmote03.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.BATTLEGROUNDS_04:
        this.bgEmote04.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.BATTLEGROUNDS_05:
        this.bgEmote05.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.BATTLEGROUNDS_06:
        this.bgEmote06.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.BANANA:
        this.bananaEmote.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.HERO_BUDDY:
        this.heroBuddyEmote.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.DOUBLE_HERO_BUDDY:
        this.doubleHeroBuddyEmote.SetActive(true);
        break;
      case NotificationManager.VisualEmoteType.QUEST_COMPLETE:
        this.questEmote.SetActive(true);
        break;
    }
  }

  private void UpdateTechLevelPlaymaker(int techLevel)
  {
    PlayMakerFSM component = this.techLevelEmote.GetComponent<PlayMakerFSM>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("No playmaker attached to tech level icon.");
    }
    else
    {
      component.FsmVariables.GetFsmInt("TechLevel").Value = techLevel;
      component.SendEvent("Action");
    }
  }

  public void ChangeDialogText(
    string headlineString,
    string bodyString,
    string yesOrOKstring,
    string noString)
  {
    this.speechUberText.Text = bodyString;
    this.headlineUberText.Text = headlineString;
  }

  public void RepositionSpeechBubbleAroundBigQuote(
    Notification.SpeechBubbleDirection direction,
    bool animateSpeechBubble)
  {
    GameObject gameObject = this.FaceDirection(direction);
    if (animateSpeechBubble)
      Notification.PlayBirthAnim(gameObject, gameObject.transform.localScale * 0.75f, gameObject.transform.localScale);
    TransformUtil.AttachAndPreserveLocalTransform(this.speechUberText.transform, gameObject.transform);
  }

  public GameObject FaceDirection(Notification.SpeechBubbleDirection direction)
  {
    this.m_bubbleDirection = direction;
    foreach (Notification.SpeechBubbleDirection direction1 in Enum.GetValues(typeof (Notification.SpeechBubbleDirection)))
    {
      GameObject speechBubble = this.GetSpeechBubble(direction1);
      if ((UnityEngine.Object) speechBubble != (UnityEngine.Object) null)
      {
        iTween.Stop(speechBubble);
        speechBubble.GetComponent<Renderer>().enabled = false;
      }
    }
    GameObject speechBubble1 = this.GetSpeechBubble(direction);
    if ((UnityEngine.Object) speechBubble1 != (UnityEngine.Object) null)
    {
      if (this.m_speechBubbleScales.ContainsKey(direction))
        speechBubble1.transform.localScale = this.m_speechBubbleScales[direction];
      speechBubble1.GetComponent<Renderer>().enabled = true;
    }
    return speechBubble1;
  }

  private GameObject GetSpeechBubble(Notification.SpeechBubbleDirection direction)
  {
    switch (direction)
    {
      case Notification.SpeechBubbleDirection.TopLeft:
        return this.upperLeftBubble;
      case Notification.SpeechBubbleDirection.TopRight:
        return this.upperRightBubble;
      case Notification.SpeechBubbleDirection.BottomLeft:
        return this.bottomLeftBubble;
      case Notification.SpeechBubbleDirection.BottomRight:
        return this.bottomRightBubble;
      case Notification.SpeechBubbleDirection.MiddleLeft:
        return this.leftBubble;
      default:
        return (GameObject) null;
    }
  }

  public void PlaySpeechBubbleDeath()
  {
    Notification.SpeechBubbleDirection bubbleDirection = this.m_bubbleDirection;
    GameObject speechBubble = this.GetSpeechBubble(bubbleDirection);
    if (!((UnityEngine.Object) speechBubble != (UnityEngine.Object) null))
      return;
    iTween.ScaleTo(speechBubble, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "easetype", (object) iTween.EaseType.easeInExpo, (object) "time", (object) 0.5f, (object) "oncomplete", (object) "OnBubbleDeathComplete", (object) "oncompletetarget", (object) this.gameObject, (object) "oncompleteparams", (object) bubbleDirection));
  }

  private void OnBubbleDeathComplete(Notification.SpeechBubbleDirection direction)
  {
    GameObject speechBubble = this.GetSpeechBubble(direction);
    if (!((UnityEngine.Object) speechBubble != (UnityEngine.Object) null))
      return;
    speechBubble.GetComponent<Renderer>().enabled = false;
  }

  public Notification.SpeechBubbleDirection GetSpeechBubbleDirection() => this.m_bubbleDirection;

  public void ShowPopUpArrow(Notification.PopUpArrowDirection direction)
  {
    switch (direction)
    {
      case Notification.PopUpArrowDirection.Left:
        this.leftPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
      case Notification.PopUpArrowDirection.Right:
        this.rightPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
      case Notification.PopUpArrowDirection.Down:
        this.bottomPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
      case Notification.PopUpArrowDirection.Up:
        this.topPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
      case Notification.PopUpArrowDirection.LeftDown:
        this.bottomLeftPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
      case Notification.PopUpArrowDirection.RightDown:
        this.bottomRightPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
      case Notification.PopUpArrowDirection.RightUp:
        this.topRightPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
      case Notification.PopUpArrowDirection.LeftUp:
        this.topLeftPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
      case Notification.PopUpArrowDirection.BottomThree:
        this.bottomLeftPopupArrow.GetComponent<Renderer>().enabled = true;
        this.bottomRightPopupArrow.GetComponent<Renderer>().enabled = true;
        this.bottomPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
      case Notification.PopUpArrowDirection.TopThree:
        this.topLeftPopupArrow.GetComponent<Renderer>().enabled = true;
        this.topRightPopupArrow.GetComponent<Renderer>().enabled = true;
        this.topPopupArrow.GetComponent<Renderer>().enabled = true;
        break;
    }
  }

  public void SetPosition(Actor actor, Notification.SpeechBubbleDirection direction)
  {
    if ((UnityEngine.Object) actor.GetBones() == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Notification Error - Tried to set the position of a Speech Bubble, but the target actor has no bones!");
    }
    else
    {
      GameObject childBySubstring = GameObjectUtils.FindChildBySubstring(actor.GetBones(), "SpeechBubbleBones");
      if ((UnityEngine.Object) childBySubstring == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Notification Error - Tried to set the position of a Speech Bubble, but the target actor has no SpeechBubbleBones!");
      }
      else
      {
        Vector3 vector3 = Vector3.zero;
        switch (direction)
        {
          case Notification.SpeechBubbleDirection.TopLeft:
            vector3 = GameObjectUtils.FindChildBySubstring(childBySubstring, "BottomRight").transform.position;
            break;
          case Notification.SpeechBubbleDirection.TopRight:
            vector3 = GameObjectUtils.FindChildBySubstring(childBySubstring, "BottomLeft").transform.position;
            break;
          case Notification.SpeechBubbleDirection.BottomLeft:
            vector3 = GameObjectUtils.FindChildBySubstring(childBySubstring, "TopRight").transform.position;
            break;
          case Notification.SpeechBubbleDirection.BottomRight:
            vector3 = GameObjectUtils.FindChildBySubstring(childBySubstring, "TopLeft").transform.position;
            break;
          case Notification.SpeechBubbleDirection.MiddleLeft:
            vector3 = GameObjectUtils.FindChildBySubstring(childBySubstring, "MiddleRight").transform.position;
            break;
        }
        this.transform.position = vector3;
      }
    }
  }

  public void SetPosition(Vector3 position) => this.transform.position = position;

  public void SetPositionForSmallBubble(Actor actor)
  {
    if ((UnityEngine.Object) actor.GetBones() == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Notification Error - Tried to set the position of a Speech Bubble, but the target actor has no bones!");
    }
    else
    {
      GameObject childBySubstring = GameObjectUtils.FindChildBySubstring(actor.GetBones(), "SpeechBubbleBones");
      if ((UnityEngine.Object) childBySubstring == (UnityEngine.Object) null)
        Debug.LogError((object) "Notification Error - Tried to set the position of a Speech Bubble, but the target actor has no SpeechBubbleBones!");
      else
        this.transform.position = GameObjectUtils.FindChildBySubstring(childBySubstring, "SmallBubble").transform.position;
    }
  }

  public void CloseWithoutAnimation() => this.FinishDeath();

  private void FinishDeath()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    if (this.OnFinishDeathState == null)
      return;
    this.OnFinishDeathState(this.notificationGroup);
  }

  public void PlayDeath()
  {
    if ((UnityEngine.Object) this.destroyEvent != (UnityEngine.Object) null)
      this.destroyEvent.Activate();
    if ((UnityEngine.Object) this.bounceObject != (UnityEngine.Object) null || (UnityEngine.Object) this.fadeArrowObject != (UnityEngine.Object) null)
    {
      this.FinishDeath();
    }
    else
    {
      this.isDying = true;
      iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "easetype", (object) iTween.EaseType.easeInExpo, (object) "time", (object) 0.5f, (object) "oncomplete", (object) "FinishDeath", (object) "oncompletetarget", (object) this.gameObject));
    }
  }

  public void Shrink(float duration = -1f)
  {
    this.m_shrunk = true;
    if ((double) duration < 0.0)
      duration = 0.5f;
    iTween.Stop(this.gameObject);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) Vector3.zero, (object) "easetype", (object) iTween.EaseType.easeInExpo, (object) "time", (object) duration));
  }

  public void Unshrink(float duration = -1f)
  {
    if (this.isDying)
      return;
    if ((double) duration < 0.0)
      duration = 0.5f;
    iTween.Stop(this.gameObject);
    iTween.ScaleTo(this.gameObject, iTween.Hash((object) "scale", (object) this.m_initialScale, (object) "easetype", (object) iTween.EaseType.easeInExpo, (object) "time", (object) duration));
    this.m_shrunk = false;
  }

  public bool IsDying() => this.isDying;

  public virtual void PlayBirth()
  {
    if ((UnityEngine.Object) this.showEvent != (UnityEngine.Object) null)
      this.showEvent.Activate();
    if ((UnityEngine.Object) this.bounceObject == (UnityEngine.Object) null && (UnityEngine.Object) this.fadeArrowObject == (UnityEngine.Object) null)
    {
      Vector3 localScale = this.transform.localScale;
      Notification.PlayBirthAnim(this.gameObject, new Vector3(0.01f, 0.01f, 0.01f), localScale);
      this.m_initialScale = localScale;
    }
    else if ((UnityEngine.Object) this.bounceObject != (UnityEngine.Object) null)
    {
      this.BounceDown();
    }
    else
    {
      if (!((UnityEngine.Object) this.fadeArrowObject != (UnityEngine.Object) null))
        return;
      this.FadeOut();
    }
  }

  public void PlayBirthWithForcedScale(Vector3 targetScale)
  {
    Notification.PlayBirthAnim(this.gameObject, this.gameObject.transform.localScale, targetScale);
    this.m_initialScale = this.transform.localScale;
  }

  public void PlaySmallBirthForFakeBubble()
  {
    if ((UnityEngine.Object) this.showEvent != (UnityEngine.Object) null)
      this.showEvent.Activate();
    if ((UnityEngine.Object) this.bounceObject == (UnityEngine.Object) null && (UnityEngine.Object) this.fadeArrowObject == (UnityEngine.Object) null)
    {
      float num = 0.25f;
      Notification.PlayBirthAnim(this.gameObject, new Vector3(0.01f, 0.01f, 0.01f), new Vector3(num * this.transform.localScale.x, num * this.transform.localScale.y, num * this.transform.localScale.z));
    }
    else
      this.BounceDown();
  }

  public static void PlayBirthAnim(
    GameObject gameObject,
    Vector3 startingScale,
    Vector3 targetScale)
  {
    gameObject.transform.localScale = startingScale;
    iTween.ScaleTo(gameObject, iTween.Hash((object) "scale", (object) targetScale, (object) "easetype", (object) iTween.EaseType.easeOutElastic, (object) "time", (object) 1f));
  }

  public void PulseReminderEveryXSeconds(float seconds) => this.StartCoroutine(this.PulseReminder(seconds));

  private IEnumerator PulseReminder(float seconds)
  {
    Notification notification = this;
    WaitForSeconds waitForSecs = new WaitForSeconds(seconds);
    while (!notification.isDying)
    {
      yield return (object) waitForSecs;
      if (!notification.m_shrunk)
        iTween.PunchScale(notification.gameObject, iTween.Hash((object) "amount", (object) Vector3.one, (object) "time", (object) 1f));
    }
  }

  private void BounceUp() => iTween.MoveTo(this.bounceObject, iTween.Hash((object) "islocal", (object) true, (object) "z", (object) (float) ((double) this.bounceObject.transform.localPosition.z - 0.5), (object) "time", (object) 0.75f, (object) "easetype", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) "BounceDown", (object) "oncompletetarget", (object) this.gameObject));

  private void BounceDown() => iTween.MoveTo(this.bounceObject, iTween.Hash((object) "islocal", (object) true, (object) "z", (object) (float) ((double) this.bounceObject.transform.localPosition.z + 0.5), (object) "time", (object) 0.75f, (object) "easetype", (object) iTween.EaseType.easeOutCubic, (object) "oncomplete", (object) "BounceUp", (object) "oncompletetarget", (object) this.gameObject));

  private void FadeOut()
  {
    iTween.MoveTo(this.fadeArrowObject, iTween.Hash((object) "islocal", (object) true, (object) "z", (object) (float) ((double) this.fadeArrowObject.transform.localPosition.z - 0.5), (object) "time", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "FadeComplete", (object) "oncompletetarget", (object) this.gameObject));
    AnimationUtil.FadeTexture(this.fadeArrowObject.GetComponentInChildren<MeshRenderer>(), 1f, 0.0f, 0.5f, 0.15f);
  }

  private void FadeComplete()
  {
    iTween.MoveTo(this.fadeArrowObject, iTween.Hash((object) "islocal", (object) true, (object) "z", (object) (float) ((double) this.fadeArrowObject.transform.localPosition.z + 0.5), (object) "time", (object) 0.0f, (object) "delay", (object) 0.5f, (object) "easetype", (object) iTween.EaseType.linear, (object) "oncomplete", (object) "FadeOut", (object) "oncompletetarget", (object) this.gameObject));
    AnimationUtil.FadeTexture(this.fadeArrowObject.GetComponentInChildren<MeshRenderer>(), 0.0f, 1f, 0.0f, 0.85f);
  }

  public void AssignAudio(AudioSource source) => this.m_accompaniedAudio = source;

  public AudioSource GetAudio() => this.m_accompaniedAudio;

  public GameObject GetParentOffsetObject() => this.m_parentOffsetObject;

  public void SetParentOffsetObject(GameObject parentOffset)
  {
    if ((UnityEngine.Object) this.m_parentOffsetObject != (UnityEngine.Object) null)
    {
      this.transform.parent = (Transform) null;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_parentOffsetObject);
    }
    this.m_parentOffsetObject = parentOffset;
    this.transform.SetParent(parentOffset.transform);
  }

  public void SetClickBlockerActive(bool active)
  {
    if (!((UnityEngine.Object) this.clickBlocker != (UnityEngine.Object) null))
      return;
    this.clickBlocker.gameObject.SetActive(active);
  }

  public enum SpeechBubbleDirection
  {
    None,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    MiddleLeft,
  }

  public enum PopUpArrowDirection
  {
    Left,
    Right,
    Down,
    Up,
    LeftDown,
    RightDown,
    RightUp,
    LeftUp,
    BottomThree,
    TopThree,
  }
}
