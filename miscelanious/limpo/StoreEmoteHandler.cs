using Hearthstone.UI;
using Hearthstone.UI.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreEmoteHandler : MonoBehaviour
{
  private const float MinTimeBetweenEmotesSec = 1f;
  [SerializeField]
  private List<StoreEmoteOption> m_emotes;
  [SerializeField]
  [Tooltip("Reference to the Widget behaviour Card component that we want to derive emotes from.")]
  private AsyncReference m_asyncCardParentWidgetReference;
  [SerializeField]
  private StoreActorEmoteDriver m_emoteDriver;
  [SerializeField]
  private bool m_shouldDefaultToShown;
  [SerializeField]
  [Tooltip("When the player hits an emote bubble, should they hide while that emote is playing?")]
  private bool m_shouldEmotesHideOnPlay;
  [SerializeField]
  private float m_minTimeBetweenEmotesSec = 1f;
  private Hearthstone.UI.Card m_dataCard;
  private Actor m_actor;
  private float m_timeBetweenEmotesSec;
  private float m_emoteCooldownTimeSec;
  private bool m_isShowingEmotes;

  [Overridable]
  public bool ToggleEmotes
  {
    get => this.m_isShowingEmotes;
    set
    {
      if (value == this.m_isShowingEmotes)
        return;
      if (value)
        this.ShowEmotes();
      else
        this.HideEmotes();
    }
  }

  private void Awake()
  {
    this.m_timeBetweenEmotesSec = Math.Max(1f, this.m_minTimeBetweenEmotesSec);
    if (this.m_shouldDefaultToShown)
      this.ShowEmotes();
    else
      this.HideEmotes();
  }

  private void OnDestroy()
  {
    if (!((UnityEngine.Object) this.m_dataCard != (UnityEngine.Object) null))
      return;
    this.m_dataCard.UnregisterCardLoadedListener(new Hearthstone.UI.Card.OnCardActorLoadedDelegate(this.OnDataCardLoaded));
    this.m_dataCard = (Hearthstone.UI.Card) null;
  }

  private void Start() => this.m_asyncCardParentWidgetReference.RegisterReadyListener<Hearthstone.UI.Card>(new Action<Hearthstone.UI.Card>(this.OnCardParentWidgetReady));

  public void ShowEmotes()
  {
    if (this.m_isShowingEmotes)
      return;
    this.m_isShowingEmotes = true;
    if ((UnityEngine.Object) this.m_emoteDriver != (UnityEngine.Object) null)
      this.m_emoteDriver.StopEmote();
    foreach (StoreEmoteOption emote in this.m_emotes)
      emote.Enable();
  }

  public void HideEmotes(bool isImmediateHide = true)
  {
    if (!this.m_isShowingEmotes)
      return;
    this.m_isShowingEmotes = false;
    if (isImmediateHide && (UnityEngine.Object) this.m_emoteDriver != (UnityEngine.Object) null)
      this.m_emoteDriver.StopEmote();
    foreach (StoreEmoteOption emote in this.m_emotes)
      emote.Disable(isImmediateHide);
  }

  public void PlayEmote(EmoteType emoteType)
  {
    if (emoteType == EmoteType.INVALID)
      return;
    if ((UnityEngine.Object) this.m_emoteDriver == (UnityEngine.Object) null)
      Debug.LogError((object) ("StoreEmoteHandler: Failed to play emote " + emoteType.ToString() + " as no driver is set."));
    else if ((UnityEngine.Object) this.m_actor == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "StoreEmoteHandler: Failed to play emote as no actor is set to request emote for.");
    }
    else
    {
      if ((double) Time.unscaledTime < (double) this.m_emoteCooldownTimeSec)
        return;
      this.m_emoteCooldownTimeSec = Time.unscaledTime + this.m_timeBetweenEmotesSec;
      this.m_emoteDriver.PlayEmote(this.m_actor, emoteType, (Action<int>) (onFinished =>
      {
        if (!this.m_shouldEmotesHideOnPlay || !this.gameObject.activeInHierarchy)
          return;
        this.ShowEmotes();
      }));
      if (!this.m_shouldEmotesHideOnPlay)
        return;
      this.HideEmotes(false);
    }
  }

  private void TryLoadCardActor(Actor cardActor)
  {
    if ((UnityEngine.Object) cardActor == (UnityEngine.Object) null)
      Debug.LogError((object) "StoreEmoteHandler: Failed to load card actor as passed null.");
    else if ((UnityEngine.Object) this.m_emoteDriver == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "StoreEmoteHandler: Failed to load card actor as StoreActorEmoteDriver was null.");
    }
    else
    {
      this.m_actor = cardActor;
      this.m_emoteDriver.Actor = cardActor;
    }
  }

  private void OnCardParentWidgetReady(Hearthstone.UI.Card dataCard)
  {
    if ((UnityEngine.Object) dataCard == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "StoreEmoteHandler: Widget for card parent was null.");
    }
    else
    {
      this.m_dataCard = dataCard;
      if ((UnityEngine.Object) this.m_dataCard.CardActor == (UnityEngine.Object) null)
        dataCard.RegisterCardLoadedListener(new Hearthstone.UI.Card.OnCardActorLoadedDelegate(this.OnDataCardLoaded));
      else
        this.TryLoadCardActor(this.m_dataCard.CardActor);
    }
  }

  private void OnDataCardLoaded(Actor cardActor)
  {
    if ((UnityEngine.Object) cardActor == (UnityEngine.Object) null)
      Debug.LogError((object) "StoreEmoteHandler: Failed to find Ui.Card actor.");
    else
      this.TryLoadCardActor(cardActor);
  }
}
