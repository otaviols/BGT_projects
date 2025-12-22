using System;
using System.Collections;
using UnityEngine;

public class CollectionDeckTrayButton : PegUIElement
{
  public HighlightState m_highlightState;
  public UberText m_buttonText;
  private const float BUTTON_POP_SPEED = 2.5f;
  private readonly string DECKBOX_POPUP_ANIM_NAME = "NewDeck_PopUp";
  private readonly string DECKBOX_POPDOWN_ANIM_NAME = "NewDeck_PopDown";
  private bool m_isPoppedUp;
  private bool m_isUsable;

  protected override void Awake()
  {
    base.Awake();
    this.SetEnabled(false);
    this.m_buttonText.Text = !SceneMgr.Get().IsInTavernBrawlMode() ? (!SceneMgr.Get().IsInLettuceMode() ? GameStrings.Get("GLUE_COLLECTION_NEW_DECK") : GameStrings.Get("GLUE_COLLECTION_NEW_TEAM")) : string.Empty;
    UIBScrollableItem component = this.GetComponent<UIBScrollableItem>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.SetCustomActiveState(new UIBScrollableItem.ActiveStateCallback(this.IsUsable));
  }

  public void SetIsUsable(bool isUsable) => this.m_isUsable = isUsable;

  public bool IsUsable() => this.m_isUsable;

  public void PlayPopUpAnimation() => this.PlayPopUpAnimation((CollectionDeckTrayButton.DelOnAnimationFinished) null);

  public void PlayPopUpAnimation(
    CollectionDeckTrayButton.DelOnAnimationFinished callback)
  {
    this.PlayPopUpAnimation(callback, (object) null);
  }

  public void PlayPopUpAnimation(
    CollectionDeckTrayButton.DelOnAnimationFinished callback,
    object callbackData,
    float? speed = null)
  {
    this.gameObject.SetActive(true);
    if (this.m_isPoppedUp)
    {
      if (callback == null)
        return;
      callback(callbackData);
    }
    else
    {
      this.m_isPoppedUp = true;
      Animation component = this.GetComponent<Animation>();
      component[this.DECKBOX_POPUP_ANIM_NAME].time = 0.0f;
      component[this.DECKBOX_POPUP_ANIM_NAME].speed = speed.HasValue ? speed.Value : 2.5f;
      this.PlayAnimation(this.DECKBOX_POPUP_ANIM_NAME, callback, callbackData);
    }
  }

  public void PlayPopDownAnimation() => this.PlayPopDownAnimation((CollectionDeckTrayButton.DelOnAnimationFinished) null);

  public void PlayPopDownAnimation(
    CollectionDeckTrayButton.DelOnAnimationFinished callback)
  {
    this.PlayPopDownAnimation(callback, (object) null);
  }

  public void PlayPopDownAnimation(
    CollectionDeckTrayButton.DelOnAnimationFinished callback,
    object callbackData,
    float? speed = null)
  {
    this.gameObject.SetActive(true);
    if (!this.m_isPoppedUp)
    {
      if (callback == null)
        return;
      callback(callbackData);
    }
    else
    {
      this.m_isPoppedUp = false;
      Animation component = this.GetComponent<Animation>();
      component[this.DECKBOX_POPDOWN_ANIM_NAME].time = 0.0f;
      component[this.DECKBOX_POPDOWN_ANIM_NAME].speed = speed.HasValue ? speed.Value : 2.5f;
      this.PlayAnimation(this.DECKBOX_POPDOWN_ANIM_NAME, callback, callbackData);
    }
  }

  public void FlipHalfOverAndHide(
    float animTime,
    CollectionDeckTrayButton.DelOnAnimationFinished finished = null)
  {
    if (!this.m_isPoppedUp)
    {
      Debug.LogWarning((object) "Can't flip over and hide button. It is currently not popped up.");
    }
    else
    {
      iTween.StopByName(this.gameObject, "rotation");
      iTween.RotateTo(this.gameObject, iTween.Hash((object) "rotation", (object) new Vector3(270f, 0.0f, 0.0f), (object) "isLocal", (object) true, (object) "time", (object) animTime, (object) "easeType", (object) iTween.EaseType.easeInCubic, (object) "oncomplete", (object) (Action<object>) (_1 =>
      {
        if (finished != null)
          finished((object) this);
        this.gameObject.SetActive(false);
        this.transform.localEulerAngles = Vector3.zero;
      }), (object) "name", (object) "rotation"));
      this.m_isPoppedUp = false;
    }
  }

  public bool IsPoppedUp() => this.m_isPoppedUp;

  private void PlayAnimation(string animationName) => this.PlayAnimation(animationName, (CollectionDeckTrayButton.DelOnAnimationFinished) null, (object) null);

  private void PlayAnimation(
    string animationName,
    CollectionDeckTrayButton.DelOnAnimationFinished callback,
    object callbackData)
  {
    this.GetComponent<Animation>().Play(animationName);
    CollectionDeckTrayButton.OnPopAnimationFinishedCallbackData finishedCallbackData = new CollectionDeckTrayButton.OnPopAnimationFinishedCallbackData()
    {
      m_callback = callback,
      m_callbackData = callbackData,
      m_animationName = animationName
    };
    this.StopCoroutine("WaitThenCallAnimationCallback");
    this.StartCoroutine("WaitThenCallAnimationCallback", (object) finishedCallbackData);
  }

  private IEnumerator WaitThenCallAnimationCallback(
    CollectionDeckTrayButton.OnPopAnimationFinishedCallbackData callbackData)
  {
    CollectionDeckTrayButton collectionDeckTrayButton = this;
    Animation component = collectionDeckTrayButton.GetComponent<Animation>();
    yield return (object) new WaitForSeconds(component[callbackData.m_animationName].length / component[callbackData.m_animationName].speed);
    bool enabled = callbackData.m_animationName.Equals(collectionDeckTrayButton.DECKBOX_POPUP_ANIM_NAME);
    collectionDeckTrayButton.SetEnabled(enabled);
    if (callbackData.m_callback != null)
      callbackData.m_callback(callbackData.m_callbackData);
  }

  protected override void OnOver(PegUIElement.InteractionState oldState)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Hub_Mouseover.prefab:40130da7b734190479c527d6bca1a4a8");
    this.m_highlightState.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
  }

  protected override void OnOut(PegUIElement.InteractionState oldState) => this.m_highlightState.ChangeState(ActorStateType.NONE);

  public delegate void DelOnAnimationFinished(object callbackData);

  private class OnPopAnimationFinishedCallbackData
  {
    public string m_animationName;
    public CollectionDeckTrayButton.DelOnAnimationFinished m_callback;
    public object m_callbackData;
  }
}
